using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class Train_Seat_Selection : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationManager.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool isLoggedIn = false;
                string userIdStr = "0";

                if (Session["userId"] != null)
                {
                    try
                    {
                        string sessionUserId = Session["userId"].ToString();
                        if (!string.IsNullOrEmpty(sessionUserId) && sessionUserId != "0")
                        {
                            isLoggedIn = true;
                            userIdStr = sessionUserId;
                        }
                    }
                    catch { }
                }

                if (isLoggedIn)
                {
                    hfUserId.Value = userIdStr;
                    ScriptManager.RegisterStartupScript(this, GetType(), "setLoggedInUser",
                        $@"sessionStorage.setItem('userId', '{userIdStr}');", true);
                }
                else
                {
                    hfUserId.Value = "0";
                    ScriptManager.RegisterStartupScript(this, GetType(), "clearStorage",
                        @"sessionStorage.removeItem('userId'); sessionStorage.removeItem('userData');", true);
                }

                // Read all query string params
                string tripId = Request.QueryString["tripId"];
                string coachTypeId = Request.QueryString["coachTypeId"];
                string coachId = Request.QueryString["coachId"];
                string trainId = Request.QueryString["trainId"];
                string priceParam = Request.QueryString["price"];
                string status = Request.QueryString["status"] ?? "";
                string pnrNumber = Request.QueryString["pnrNumber"] ?? "";
                string bookedId = Request.QueryString["bookedId"] ?? "";
                string dateOfJourneyParam = Request.QueryString["dateOfJourney"] ?? "";
                string ticketCountStr = Request.QueryString["ticketCount"] ?? "0";
                string postponeCountStr = Request.QueryString["postponeCount"] ?? "0";
                string fromStationId = Request.QueryString["FromStationId"];
                string toStationId = Request.QueryString["ToStationId"];

                // Store in hidden fields
                hfPnrNumber.Value = pnrNumber;
                hfBookedId.Value = bookedId;
                hfTicketCount.Value = ticketCountStr;
                hfPostponeCount.Value = postponeCountStr;
                hfFromStationId.Value = fromStationId ?? "";
                hfToStationId.Value = toStationId ?? "";

                // Set minimum date
                if (status.ToLower() == "booked" && !string.IsNullOrEmpty(dateOfJourneyParam))
                {
                    DateTime originalBookingDate;
                    if (DateTime.TryParse(dateOfJourneyParam, out originalBookingDate))
                    {
                        DateTime minAllowedDate = originalBookingDate.AddDays(1);
                        txtJourneyDate.Attributes["min"] = minAllowedDate.ToString("yyyy-MM-dd");
                        System.Diagnostics.Debug.WriteLine($"Postpone Mode: Original={originalBookingDate:yyyy-MM-dd}, Min={minAllowedDate:yyyy-MM-dd}");
                    }
                    else
                    {
                        txtJourneyDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                else
                {
                    txtJourneyDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(tripId) || string.IsNullOrEmpty(coachTypeId) || string.IsNullOrEmpty(trainId))
                {
                    ShowAlert("Invalid request. Missing required parameters.");
                    Response.Redirect("~/TrainTicket.aspx");
                    return;
                }

                hfTripId.Value = tripId;
                hfCoachTypeId.Value = coachTypeId;
                hfCoachNumber.Value = coachId;
                hfTrainId.Value = trainId;

                if (!string.IsNullOrEmpty(priceParam))
                {
                    decimal price = 0;
                    if (decimal.TryParse(priceParam, out price))
                    {
                        hfUnitPrice.Value = price.ToString();
                        lblSeatPrice.Text = price.ToString("N2");
                    }
                }

                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainSeatData(tripId, coachTypeId, trainId, coachId)));

                // Postpone mode
                if (status.ToLower() == "booked" && !string.IsNullOrEmpty(pnrNumber) && !string.IsNullOrEmpty(bookedId))
                {
                    PostponeTicket(pnrNumber, postponeCountStr);
                }
            }
            else
            {
                if (ViewState["CoachLayout"] != null)
                {
                    string coachTypeJson = ViewState["CoachLayout"].ToString();
                    var coachType = JsonConvert.DeserializeObject<TrainCoachType>(coachTypeJson);
                    GenerateTrainSeats(coachType);

                    if (!string.IsNullOrEmpty(txtJourneyDate.Text))
                    {
                        RegisterAsyncTask(new PageAsyncTask(() => LoadBookedSeats()));
                    }
                }
            }
        }

        
        private void PostponeTicket(string pnrNumber, string postponeCountStr)
        {
            int postponeCount = 0;
            int.TryParse(postponeCountStr, out postponeCount);

            if (postponeCount >= 2)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "postponeLimit",
                    @"$(document).ready(function(){
                        alert('Maximum postpone limit reached (2 times). You cannot postpone this booking again.');
                        window.location.href = 'TrainTicket.aspx';
                    });", true);
                return;
            }

            if (Session["userId"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showLoginForPostpone",
                    @"$(document).ready(function(){ $('#loginModal').modal('show'); });", true);
                return;
            }

            string feePercent = postponeCount == 0 ? "20%" : "60%";
            int postponeNumber = postponeCount + 1;
            double feeMultiplier = postponeCount == 0 ? 0.20 : 0.60;

            ScriptManager.RegisterStartupScript(this, GetType(), "showPostponeBanner",
                $@"$(document).ready(function(){{
                    $('#postponeBanner').show();
                    $('#postponePnrDisplay').text('{pnrNumber}');
                    $('#postponeFeePercent').text('{feePercent}');
                    $('#postponeCountDisplay').text('{postponeNumber}');
                    window.isPostponeMode      = true;
                    window.postponeFeePercent  = {feeMultiplier};
                    console.log('Postpone Mode Active - PNR: {pnrNumber}, Fee: {feePercent}');
                }});", true);
        }

        
        protected async void btnProceedToBook_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedSeats = hfSelectedSeats.Value;
                string pnrNumber = Request.QueryString["pnrNumber"] ?? "";
                string bookedId = Request.QueryString["bookedId"] ?? "";
                string status = Request.QueryString["status"] ?? "";
                string ticketCountStr = Request.QueryString["ticketCount"] ?? "0";
                string postponeCount = Request.QueryString["postponeCount"] ?? "0";
                int ticketCount = int.TryParse(ticketCountStr, out var tc) ? tc : 0;

                if (string.IsNullOrEmpty(selectedSeats))
                {
                    ShowClientAlert("Please select at least one seat.");
                    return;
                }

                // Exact seat count validate (postpone mode)
                if (ticketCount > 0 && status.ToLower() == "booked")
                {
                    string[] seatsArray = selectedSeats.Split(',');
                    if (seatsArray.Length != ticketCount)
                    {
                        ShowClientAlert($"You must select exactly {ticketCount} seat(s) for postponement. Currently selected: {seatsArray.Length}");
                        return;
                    }
                }

                string journeyDate = txtJourneyDate.Text;
                if (string.IsNullOrEmpty(journeyDate))
                {
                    ShowClientAlert("Please select journey date.");
                    return;
                }

                DateTime selectedDate;
                if (DateTime.TryParse(journeyDate, out selectedDate))
                {
                    if (selectedDate.Date < DateTime.Now.Date)
                    {
                        ShowClientAlert("Journey date cannot be in the past.");
                        return;
                    }
                }

                // Login check
                string userIdStr = null;
                if (Session["userId"] != null)
                {
                    string s = Session["userId"].ToString();
                    if (!string.IsNullOrEmpty(s) && s != "0") userIdStr = s;
                }
                if (string.IsNullOrEmpty(userIdStr) && !string.IsNullOrEmpty(hfUserId.Value) && hfUserId.Value != "0")
                {
                    userIdStr = hfUserId.Value;
                    int uid;
                    if (int.TryParse(userIdStr, out uid) && uid > 0)
                        Session["userId"] = uid;
                }

                if (string.IsNullOrEmpty(userIdStr))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showLogin",
                        @"$('#loginModal').modal('show');", true);
                    return;
                }

                int fromStationId = await GetStationIdByName(txtFromStation.Text);
                int toStationId = await GetStationIdByName(txtToStation.Text);

                // Store in session
                Session["SelectedSeats"] = selectedSeats;
                Session["JourneyDate"] = journeyDate;
                Session["TripId"] = hfTripId.Value;
                Session["TrainId"] = hfTrainId.Value;
                Session["CoachNumber"] = hfCoachNumber.Value;
                Session["CoachTypeId"] = hfCoachTypeId.Value;
                Session["UnitPrice"] = hfUnitPrice.Value;
                Session["FromStation"] = txtFromStation.Text;
                Session["ToStation"] = txtToStation.Text;
                Session["FromStationId"] = fromStationId;
                Session["ToStationId"] = toStationId;
                //if (fromStationId == 0)
                //    int.TryParse(hfFromStationId.Value, out fromStationId);
                //if (toStationId == 0)
                //    int.TryParse(hfToStationId.Value, out toStationId);
                Session["userId"] = userIdStr;

                if (!string.IsNullOrEmpty(pnrNumber) && !string.IsNullOrEmpty(bookedId) && status.ToLower() == "booked")
                {
                    Session["CurrentPnrNumber"] = pnrNumber;
                    Session["CurrentBookedId"] = bookedId;
                    Session["IsPostponeMode"] = true;
                    Session["PostponeCount"] = postponeCount;
                    System.Diagnostics.Debug.WriteLine($"Postpone Mode: PNR={pnrNumber}, BookedId={bookedId}, Count={postponeCount}");
                }
                else
                {
                    Session.Remove("CurrentPnrNumber");
                    Session.Remove("CurrentBookedId");
                    Session.Remove("IsPostponeMode");
                    Session.Remove("PostponeCount");
                }

                string redirectUrl = $"~/Train_Passeneger_Details.aspx?pnrNumber={pnrNumber}&status={status}&postponeCount={postponeCount}";
                Response.Redirect(redirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ShowClientAlert($"Error: {ex.Message}");
            }
        }

        
        private async Task<int> GetStationIdByName(string stationName)
        {
            int stationId = 0;
            try
            {
                if (string.IsNullOrEmpty(stationName)) return stationId;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(
                    $"{apiUrl}RailwayStations/GetRailwayStationss?name={Uri.EscapeDataString(stationName)}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var stations = JsonConvert.DeserializeObject<List<RailwayStation>>(json);
                        if (stations != null && stations.Any())
                        {
                            var station = stations.FirstOrDefault(s =>
                                s.StationName != null &&
                                s.StationName.Equals(stationName, StringComparison.OrdinalIgnoreCase));
                            if (station != null) stationId = station.StationId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStationIdByName: {ex.Message}");
            }
            return stationId;
        }

        
        private async Task LoadTrainSeatData(string tripId, string coachTypeId, string trainId, string coachId)
        {
            try
            {
                await LoadTripDetails(tripId);

                string fleetTypeId = await GetFleetTypeIdFromTrain(trainId);
                bool success = false;

                if (!string.IsNullOrEmpty(fleetTypeId))
                    success = await LoadFromFleetType(fleetTypeId, coachTypeId);

                if (!success)
                    success = await LoadFromCoachData(trainId, coachId, coachTypeId);

                if (!success)
                    await LoadCoachTypeWithDBLayout(coachTypeId);

                if (string.IsNullOrEmpty(hfUnitPrice.Value) || hfUnitPrice.Value == "0")
                {
                    await LoadSeatPrice(tripId, coachTypeId);
                    if ((string.IsNullOrEmpty(hfUnitPrice.Value) || hfUnitPrice.Value == "0") && !string.IsNullOrEmpty(fleetTypeId))
                        await LoadPriceByFleetType(fleetTypeId, coachTypeId);
                }

                // Apply postpone pricing last
                await ApplyPostponePricing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading train seat data: {ex.Message}");
                ShowAlert($"Error loading seat information: {ex.Message}");
            }
        }

        
        private async Task ApplyPostponePricing()
        {
            try
            {
                string status = Request.QueryString["status"] ?? "";
                string postponeCountStr = Request.QueryString["postponeCount"] ?? "0";
                string ticketCountStr = Request.QueryString["ticketCount"] ?? "0";
                int postponeCount = 0;
                int ticketCount = 0;
                int.TryParse(postponeCountStr, out postponeCount);
                int.TryParse(ticketCountStr, out ticketCount);

                if (status.ToLower() != "booked") return;

                decimal originalPrice = 0;
                decimal.TryParse(hfUnitPrice.Value, out originalPrice);
                if (originalPrice == 0) return;

                decimal adjustedPrice;
                if (postponeCount == 0)
                    adjustedPrice = originalPrice * 0.20M;
                else if (postponeCount == 1)
                    adjustedPrice = originalPrice * 0.60M;
                else
                {
                    ShowClientAlert("Maximum postpone limit reached (2 times).");
                    Response.Redirect("~/TrainTicket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                hfUnitPrice.Value = adjustedPrice.ToString();
                lblSeatPrice.Text = adjustedPrice.ToString("N2");

                ScriptManager.RegisterStartupScript(this, GetType(), "updatePostponePrice",
                    $@"unitPrice = {adjustedPrice};
                       window.maxSeatsAllowed = {ticketCount};
                       window.isPostponeMode  = true;
                       console.log('Postpone price: CDF {adjustedPrice}, Max seats: {ticketCount}');", true);

                System.Diagnostics.Debug.WriteLine($"Postpone pricing: Original={originalPrice}, Adjusted={adjustedPrice}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ApplyPostponePricing: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        
        private async Task<string> GetFleetTypeIdFromTrain(string trainId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}Trains/GetTrains");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray trainsArray = JArray.Parse(json);
                    var train = trainsArray.FirstOrDefault(t =>
                        t["trainId"]?.ToString() == trainId || t["TrainId"]?.ToString() == trainId ||
                        t["id"]?.ToString() == trainId || t["Id"]?.ToString() == trainId);

                    if (train != null)
                        return train["fleetTypeId"]?.ToString() ?? train["FleetTypeId"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting fleet type: {ex.Message}");
            }
            return null;
        }

        
        private async Task<bool> LoadFromFleetType(string fleetTypeId, string targetCoachTypeId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainFleetTypes/GetTrainFleetTypes");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray fleetTypesArray = JArray.Parse(json);

                    var fleetType = fleetTypesArray.FirstOrDefault(f =>
                    {
                        string ftId = f["fleetTypeId"]?.ToString() ?? f["FleetTypeId"]?.ToString();
                        string ctId = f["coachTypeId"]?.ToString() ?? f["CoachTypeId"]?.ToString();
                        return ftId == fleetTypeId && ctId == targetCoachTypeId;
                    });

                    if (fleetType != null)
                    {
                        int noOfSeats = fleetType["noOfSeats"]?.Value<int>() ?? fleetType["NoOfSeats"]?.Value<int>() ?? 0;
                        int coachLayoutId = fleetType["coachLayoutId"]?.Value<int>() ?? fleetType["CoachLayoutId"]?.Value<int>() ?? 0;
                        int coachTypeId = fleetType["coachTypeId"]?.Value<int>() ?? fleetType["CoachTypeId"]?.Value<int>() ?? 0;

                        string coachLayout = coachLayoutId > 0 ? await GetCoachLayout(coachLayoutId.ToString()) : "2x2";
                        string coachTypeName = await GetCoachTypeName(coachTypeId.ToString());

                        if (!string.IsNullOrEmpty(coachLayout) && noOfSeats > 0)
                        {
                            var coachTypeObj = new TrainCoachType
                            {
                                CoachTypeId = coachTypeId.ToString(),
                                CoachType = coachTypeName ?? "Standard Class",
                                CoachLayout = coachLayout,
                                NoOfSeats = noOfSeats
                            };
                            ViewState["CoachLayout"] = JsonConvert.SerializeObject(coachTypeObj);
                            SetCoachTypeStyling(coachTypeName ?? "Standard Class");
                            GenerateTrainSeats(coachTypeObj);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading fleet type: {ex.Message}");
                return false;
            }
        }

        
        private async Task<bool> LoadFromCoachData(string trainId, string coachId, string coachTypeId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainCoaches/GetTrainCoaches?trainId={trainId}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray coachesArray = JArray.Parse(json);
                    var coach = coachesArray.FirstOrDefault(c =>
                        c["coachNumber"]?.ToString() == coachId || c["CoachNumber"]?.ToString() == coachId);

                    if (coach != null)
                    {
                        if (coach["price"] != null)
                        {
                            decimal price = coach["price"].Value<decimal>();
                            if (price > 0) { lblSeatPrice.Text = price.ToString("N2"); hfUnitPrice.Value = price.ToString(); }
                        }

                        string ctId = coach["coachTypeId"]?.ToString() ?? coach["CoachTypeId"]?.ToString() ?? coachTypeId;
                        string coachTypeName = await GetCoachTypeName(ctId);
                        string layoutId = coach["coachLayoutId"]?.ToString() ?? coach["CoachLayoutId"]?.ToString();
                        string coachLayout = !string.IsNullOrEmpty(layoutId)
                            ? await GetCoachLayout(layoutId)
                            : await GetLayoutByCoachTypeId(ctId);

                        int noOfSeats = coach["noOfSeats"]?.Value<int>() ?? coach["NoOfSeats"]?.Value<int>() ?? 0;
                        if (noOfSeats == 0) noOfSeats = GetDefaultSeatsForCoachType(coachTypeName);

                        var coachTypeObj = new TrainCoachType
                        {
                            CoachTypeId = ctId,
                            CoachType = coachTypeName,
                            CoachLayout = coachLayout,
                            NoOfSeats = noOfSeats
                        };
                        ViewState["CoachLayout"] = JsonConvert.SerializeObject(coachTypeObj);
                        SetCoachTypeStyling(coachTypeName);
                        GenerateTrainSeats(coachTypeObj);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading coach data: {ex.Message}");
                return false;
            }
        }

        
        private async Task LoadCoachTypeWithDBLayout(string coachTypeId)
        {
            try
            {
                string coachTypeName = await GetCoachTypeName(coachTypeId);
                string layout = await GetLayoutByCoachTypeId(coachTypeId);
                int noOfSeats = GetDefaultSeatsForCoachType(coachTypeName);

                var coachTypeObj = new TrainCoachType
                {
                    CoachTypeId = coachTypeId,
                    CoachType = coachTypeName,
                    CoachLayout = layout,
                    NoOfSeats = noOfSeats
                };
                ViewState["CoachLayout"] = JsonConvert.SerializeObject(coachTypeObj);
                SetCoachTypeStyling(coachTypeName);
                GenerateTrainSeats(coachTypeObj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error using DB layout: {ex.Message}");
            }
        }

        
        private async Task<string> GetLayoutByCoachTypeId(string coachTypeId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainCoachLayouts/GetTrainCoachLayouts");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray layoutsArray = JArray.Parse(json);
                    var layout = layoutsArray.FirstOrDefault(l =>
                        (l["coachTypeId"]?.ToString() ?? l["CoachTypeId"]?.ToString()) == coachTypeId);
                    if (layout != null)
                        return layout["coachLayout"]?.ToString() ?? layout["CoachLayout"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting layout by coach type: {ex.Message}");
            }
            return "2x2";
        }

        
        private async Task<string> GetCoachLayout(string coachLayoutId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainCoachLayouts/GetTrainCoachLayouts");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray layoutsArray = JArray.Parse(json);
                    var layout = layoutsArray.FirstOrDefault(l =>
                        l["coachLayoutId"]?.ToString() == coachLayoutId || l["CoachLayoutId"]?.ToString() == coachLayoutId);
                    if (layout != null)
                        return layout["coachLayout"]?.ToString() ?? layout["CoachLayout"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting coach layout: {ex.Message}");
            }
            return "2x2";
        }

        
        private async Task<string> GetCoachTypeName(string coachTypeId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainCoachTypes/GetTrainCoachTypes");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray coachTypesArray = JArray.Parse(json);
                    var coachType = coachTypesArray.FirstOrDefault(c =>
                        c["coachTypeId"]?.ToString() == coachTypeId || c["CoachTypeId"]?.ToString() == coachTypeId);
                    if (coachType != null)
                        return coachType["coachType"]?.ToString() ?? coachType["CoachType"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting coach type name: {ex.Message}");
            }
            return "Standard Class";
        }

        
        private int GetDefaultSeatsForCoachType(string coachTypeName)
        {
            string lowerType = coachTypeName?.ToLower() ?? "";
            if (lowerType.Contains("luxury")) return 45;
            if (lowerType.Contains("first")) return 60;
            if (lowerType.Contains("second")) return 75;
            return 50;
        }

        private async Task LoadTripDetails(string tripId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainTrips/GetTrainTripDetailsByTripId?tripId={tripId}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject tripData = JObject.Parse(json);

                    string trainName = tripData["TrainName"]?.ToString() ?? tripData["trainName"]?.ToString() ?? "Train";
                    string trainNumber = tripData["TrainNumber"]?.ToString() ?? tripData["trainNumber"]?.ToString() ?? "";
                    string startStation = tripData["StartStation"]?.ToString() ?? tripData["startStation"]?.ToString() ?? "";
                    string endStation = tripData["EndStation"]?.ToString() ?? tripData["endStation"]?.ToString() ?? "";

                    lblTrainInfo.Text = $"{trainName} ({trainNumber}) - {startStation} to {endStation}";
                    txtFromStation.Text = startStation;
                    txtToStation.Text = endStation;
                    ViewState["TripData"] = json;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trip details: {ex.Message}");
            }
        }

    
        
        private void GenerateTrainSeats(TrainCoachType coachType)
        {
            SeatsContainer.Controls.Clear();
            GenerateSeatsByLayout(coachType.CoachLayout, coachType.NoOfSeats, coachType.CoachType);
        }

       
        private void GenerateSeatsByLayout(string layout, int totalSeats, string coachTypeName)
        {
            string cleanLayout = layout.Replace(" ", "").Replace("×", "x").Replace("X", "x").ToLower();
            var parts = cleanLayout.Split('x');

            int leftSeats, rightSeats;
            if (parts.Length != 2 || !int.TryParse(parts[0], out leftSeats) || !int.TryParse(parts[1], out rightSeats))
            {
                leftSeats = 2;
                rightSeats = 2;
            }

            AddCoachHeader(coachTypeName, totalSeats);
            AddToiletIndicator("top");

            int seatsPerRow = leftSeats + rightSeats;
            int rows = (int)Math.Ceiling((double)totalSeats / seatsPerRow);
            int seatCounter = 1;

            for (int row = 0; row < rows && seatCounter <= totalSeats; row++)
            {
                Panel rowPanel = new Panel { CssClass = "seat-row" };

                Panel leftGroup = new Panel { CssClass = "seat-group" };
                for (int col = 0; col < leftSeats && seatCounter <= totalSeats; col++)
                {
                    string seatClass = GetSeatClass(col, leftSeats, row, coachTypeName);
                    leftGroup.Controls.Add(CreateSeat(seatCounter++, seatClass));
                }
                rowPanel.Controls.Add(leftGroup);
                rowPanel.Controls.Add(new Panel { CssClass = "aisle" });

                Panel rightGroup = new Panel { CssClass = "seat-group" };
                for (int col = 0; col < rightSeats && seatCounter <= totalSeats; col++)
                {
                    string seatClass = GetSeatClass(col, rightSeats, row, coachTypeName);
                    rightGroup.Controls.Add(CreateSeat(seatCounter++, seatClass));
                }
                rowPanel.Controls.Add(rightGroup);

                SeatsContainer.Controls.Add(rowPanel);
            }

            AddToiletIndicator("bottom");
        }
        private string GetSeatClass(int colIndex, int totalCols, int row, string coachTypeName)
        {
            string lowerCoachType = coachTypeName?.ToLower() ?? "";

            if (lowerCoachType.Contains("luxury"))
                return "luxury-seat";
            else if (lowerCoachType.Contains("first"))
                return "first-ac-seat";
            else if (lowerCoachType.Contains("second"))
                return "second-class-seat";

            return "standard-seat";
        }

        
        private void SetCoachTypeStyling(string coachTypeName)
        {
            string coachNumber = hfCoachNumber.Value;
            coachTitle.InnerHtml = $"<i class='fas fa-subway'></i> Coach {coachNumber}";
            string lowerCoachType = coachTypeName?.ToLower() ?? "";

            if (lowerCoachType.Contains("luxury") || lowerCoachType.Contains("luxe"))
            {
                lblCoachType.Text = "Luxury Class";
                lblCoachType.CssClass = "coach-type-badge luxury";
            }
            else if (lowerCoachType.Contains("first"))
            {
                lblCoachType.Text = "First AC Class";
                lblCoachType.CssClass = "coach-type-badge first-class";
            }
            else if (lowerCoachType.Contains("second"))
            {
                lblCoachType.Text = "Second Seating (2S)";
                lblCoachType.CssClass = "coach-type-badge second-class";
            }
            else
            {
                lblCoachType.Text = coachTypeName;
                lblCoachType.CssClass = "coach-type-badge";
            }
        }
        private HyperLink CreateSeat(int seatNumber, string additionalClass)
        {
            HyperLink seat = new HyperLink
            {
                ID = $"seat_{seatNumber}",
                Text = seatNumber.ToString(),
                CssClass = $"seat {additionalClass} available",
                NavigateUrl = "javascript:void(0);",
                ToolTip = $"Seat {seatNumber}"
            };
            seat.Attributes.Add("data-seat", seatNumber.ToString());
            return seat;
        }

        private void AddCoachHeader(string coachName, int totalSeats)
        {
            string coachNumber = hfCoachNumber.Value;
            SeatsContainer.Controls.Add(new Literal
            {
                Text = $@"<div class='coach-header'>
                    <h4><i class='fas fa-train'></i> {coachName} - Coach {coachNumber}</h4>
                    <p>Total Seats: {totalSeats}</p>
                </div>"
            });
        }

        private void AddToiletIndicator(string position)
        {
            string style = position == "bottom" ? " style='margin-top: 20px;'" : "";
            SeatsContainer.Controls.Add(new Literal
            {
                Text = $"<div class='toilet-indicator'{style}><i class='fas fa-restroom'></i> Toilet</div>"
            });
        }

        
        private async Task LoadSeatPrice(string tripId, string coachTypeId)
        {
            try
            {
                string trainId = hfTrainId.Value;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainTicketPrices/GetTrainTicketPrices");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray pricesArray = JArray.Parse(json);

                    var priceEntry = pricesArray.FirstOrDefault(p =>
                    {
                        string pTrainId = p["trainId"]?.ToString() ?? p["TrainId"]?.ToString();
                        string pCoachTypeId = p["coachTypeId"]?.ToString() ?? p["CoachTypeId"]?.ToString();
                        return pTrainId == trainId && pCoachTypeId == coachTypeId;
                    });

                    if (priceEntry != null)
                    {
                        decimal price = priceEntry["onlinePrice"]?.Value<decimal>()
                                     ?? priceEntry["OnlinePrice"]?.Value<decimal>()
                                     ?? priceEntry["price"]?.Value<decimal>()
                                     ?? 0;
                        lblSeatPrice.Text = price.ToString("N2");
                        hfUnitPrice.Value = price.ToString();
                        return;
                    }
                }

                if (ViewState["TripData"] != null)
                {
                    JObject tripData = JObject.Parse(ViewState["TripData"].ToString());
                    decimal tripPrice = tripData["price"]?.Value<decimal>() ?? 0;
                    if (tripPrice > 0)
                    {
                        lblSeatPrice.Text = tripPrice.ToString("N2");
                        hfUnitPrice.Value = tripPrice.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading price: {ex.Message}");
            }
        }

        
        private async Task LoadPriceByFleetType(string fleetTypeId, string coachTypeId)
        {
            try
            {
                string trainId = hfTrainId.Value;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}TrainTicketPrices/GetTrainTicketPrices");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray pricesArray = JArray.Parse(json);

                    var priceEntry = pricesArray.FirstOrDefault(p =>
                    {
                        string pFleetTypeId = p["fleetTypeId"]?.ToString() ?? p["FleetTypeId"]?.ToString();
                        string pTrainId = p["trainId"]?.ToString() ?? p["TrainId"]?.ToString();
                        string pCoachTypeId = p["coachTypeId"]?.ToString() ?? p["CoachTypeId"]?.ToString();
                        return pFleetTypeId == fleetTypeId && pTrainId == trainId && pCoachTypeId == coachTypeId;
                    });

                    if (priceEntry != null)
                    {
                        decimal price = priceEntry["onlinePrice"]?.Value<decimal>()
                                     ?? priceEntry["OnlinePrice"]?.Value<decimal>()
                                     ?? priceEntry["price"]?.Value<decimal>()
                                     ?? 0;
                        lblSeatPrice.Text = price.ToString("N2");
                        hfUnitPrice.Value = price.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading price by fleet type: {ex.Message}");
            }
        }

        
        protected async void txtJourneyDate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtJourneyDate.Text)) return;

                if (DateTime.TryParse(txtJourneyDate.Text, out DateTime selectedDate))
                {
                    if (selectedDate.Date < DateTime.Now.Date)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "dateValidationError",
                            $"alert('Journey date cannot be in the past.'); $('#MainContent_txtJourneyDate').val('');", true);
                        txtJourneyDate.Text = "";
                        return;
                    }
                    await LoadBookedSeats();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in date validation: {ex.Message}");
            }
        }

        
        private async Task LoadBookedSeats()
        {
            try
            {
                string tripId = hfTripId.Value;
                string coachTypeId = hfCoachTypeId.Value;
                string trainId = hfTrainId.Value;
                string coachNumber = hfCoachNumber.Value;
                string journeyDate = txtJourneyDate.Text;

                if (string.IsNullOrEmpty(journeyDate) || string.IsNullOrEmpty(tripId)) return;

                DateTime parsedDate;
                if (DateTime.TryParse(journeyDate, out parsedDate))
                {
                    string formattedDate = parsedDate.ToString("yyyy-MM-dd");
                    string url = $"{apiUrl}TrainTicketBookings/GetTrainBookedSeats?tripId={tripId}&coachTypeId={coachTypeId}&coachNumber={coachNumber}&dateOfJourney={formattedDate}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BookedSeatsResponse>(json);
                        var bookedSeats = result?.BookedSeats;

                        if (bookedSeats != null && bookedSeats.Count > 0)
                            MarkBookedSeats(bookedSeats);
                        else
                            ClearBookedSeats();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading booked seats: {ex.Message}");
            }
        }
        private class BookedSeatsResponse
        {
            public int TripId { get; set; }
            public string TrainId { get; set; }
            public string CoachTypeId { get; set; }
            public string CoachNumber { get; set; }
            public string JourneyDate { get; set; }
            public List<string> BookedSeats { get; set; }
        }
        private void MarkBookedSeats(List<string> bookedSeats)
        {
            ClearBookedSeats();
            foreach (Control ctrl in SeatsContainer.Controls)
            {
                if (ctrl is Panel panel) MarkSeatsInPanel(panel, bookedSeats);
            }
        }

        private void ClearBookedSeats()
        {
            foreach (Control ctrl in SeatsContainer.Controls)
            {
                if (ctrl is Panel panel) ClearSeatsInPanel(panel);
            }
        }

        private void ClearSeatsInPanel(Panel panel)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Panel childPanel) ClearSeatsInPanel(childPanel);
                else if (ctrl is HyperLink seat && seat.CssClass.Contains("booked"))
                    seat.CssClass = seat.CssClass.Replace("booked", "available");
            }
        }

        //private void MarkSeatsInPanel(Panel panel, List<string> bookedSeats)
        //{
        //    foreach (Control ctrl in panel.Controls)
        //    {
        //        if (ctrl is Panel childPanel) MarkSeatsInPanel(childPanel, bookedSeats);
        //        else if (ctrl is HyperLink seat)
        //        {
        //            string seatNumber = seat.Attributes["data-seat"];
        //            if (bookedSeats.Contains(seatNumber))
        //                seat.CssClass = seat.CssClass.Replace("available", "booked");
        //        }
        //    }
        //}

        private void MarkSeatsInPanel(Panel panel, List<string> bookedSeats)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Panel childPanel) MarkSeatsInPanel(childPanel, bookedSeats);
                else if (ctrl is HyperLink seat)
                {
                    string seatNumber = seat.Attributes["data-seat"];
                    if (bookedSeats.Contains(seatNumber))
                    {
                        // Remove 'available', add 'booked'
                        seat.CssClass = seat.CssClass
                            .Replace("available", "")
                            .Replace("selected", "")
                            .Trim() + " booked";
                    }
                }
            }
        }
        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showAlertMsg",
                $"showAlert('{message.Replace("'", "\\'")}');", true);
        }

        private void ShowClientAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showClientAlertMsg",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        
        public class TrainCoachType
        {
            public string CoachTypeId { get; set; }
            public string CoachType { get; set; }
            public string CoachLayout { get; set; }
            public int NoOfSeats { get; set; }
        }


        public class RailwayStation
        {
            public int StationId { get; set; }
            public string StationName { get; set; }
        }
    }
}