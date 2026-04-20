using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class Select_Seat : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ Check if user is logged in on server side
                if (Session["userId"] == null || Convert.ToInt32(Session["userId"]) == 0)
                {
                    // Clear client-side storage since user is not logged in on server
                    ScriptManager.RegisterStartupScript(this, GetType(), "clearStorage",
                        @"sessionStorage.removeItem('userId'); 
                  sessionStorage.removeItem('userData'); 
                  $('input[id*=""hfUserId""]').val('');
                  console.log('User not logged in - cleared sessionStorage');", true);
                }

                string pnrNumber = Request.QueryString["pnrNumber"] ?? "";
                string tripId = Request.QueryString["tripId"];
                string vehicleId = Request.QueryString["vehicleId"]; // ✅ Get vehicleId
                string status = Request.QueryString["status"];
                string dateOfJourneyParam = Request.QueryString["dateOfJourney"]; // Get original booked date

                if (!string.IsNullOrEmpty(tripId))
                {
                    // ✅ SET MINIMUM DATE based on status
                    if (status == "Booked" && !string.IsNullOrEmpty(dateOfJourneyParam))
                    {
                        // POSTPONE MODE: Set minimum date to the day AFTER the original booking
                        DateTime originalBookingDate;
                        if (DateTime.TryParse(dateOfJourneyParam, out originalBookingDate))
                        {
                            DateTime minAllowedDate = originalBookingDate.AddDays(1);
                            date_of_journey.Attributes["min"] = minAllowedDate.ToString("yyyy-MM-dd");

                            System.Diagnostics.Debug.WriteLine($"Postpone Mode: Original Date: {originalBookingDate:yyyy-MM-dd}, Min Date: {minAllowedDate:yyyy-MM-dd}");
                        }
                        else
                        {
                            date_of_journey.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                        }
                    }
                    else
                    {
                        date_of_journey.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                    }

                    // ✅ Pass vehicleId to LoadTripDetailsAndSeats
                    RegisterAsyncTask(new PageAsyncTask(() => LoadTripDetailsAndSeats(tripId, vehicleId)));

                    if (status == "Booked")
                    {
                        PostponeTicket(pnrNumber);
                    }
                }
                else
                {
                    Response.Redirect("~/ticket.aspx");
                }
            }
            else
            {
                if (ViewState["FleetType"] != null)
                {
                    string fleetTypeJson = ViewState["FleetType"].ToString();
                    var fleetType = JsonConvert.DeserializeObject<FleetType>(fleetTypeJson);
                    GenerateDynamicSeats(fleetType);

                    if (!string.IsNullOrEmpty(date_of_journey.Text))
                    {
                        RegisterAsyncTask(new PageAsyncTask(() => LoadBookedSeatsForSelectedDate()));
                    }
                }
            }
        }

        private void PostponeTicket(string pnrNumber)
        {
            decimal seatPrice = 0;
            decimal adjustedPrice = seatPrice * 0.20M;
            //ScriptManager.RegisterStartupScript(this, GetType(), "postponeTicket",
            //    @"alert('The ticket has already been booked. Please contact support to postpone.');", true);
        }

        protected void btnProceedToBook_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedSeatsStr = hfSelectedSeats.Value;

                if (string.IsNullOrEmpty(selectedSeatsStr))
                {
                    ShowClientAlert("Please select at least one seat");
                    return;
                }

                // Get query parameters for postpone mode
                string pnrNumber = Request.QueryString["pnrNumber"] ?? "";
                string bookedIdStr = Request.QueryString["bookedId"] ?? "";
                string status = Request.QueryString["status"] ?? "";
                string ticketCountStr = Request.QueryString["ticketCount"] ?? "0";
                string vehicleId = Request.QueryString["vehicleId"] ?? ""; // ✅ Get vehicleId
                int ticketCount = int.TryParse(ticketCountStr, out var count) ? count : 0;

                System.Diagnostics.Debug.WriteLine("=== PROCEED TO BOOK DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"PNR Number: {pnrNumber}");
                System.Diagnostics.Debug.WriteLine($"Booked ID: {bookedIdStr}");
                System.Diagnostics.Debug.WriteLine($"Status: {status}");
                System.Diagnostics.Debug.WriteLine($"Vehicle ID: {vehicleId}");
                System.Diagnostics.Debug.WriteLine($"Ticket Count Required: {ticketCount}");
                System.Diagnostics.Debug.WriteLine($"Selected Seats: {selectedSeatsStr}");

                // ✅ VALIDATE EXACT SEAT COUNT for postpone mode
                if (ticketCount > 0 && status.ToLower() == "booked")
                {
                    string[] selectedSeatsArray = selectedSeatsStr.Split(',');
                    if (selectedSeatsArray.Length != ticketCount)
                    {
                        ShowClientAlert($"You must select exactly {ticketCount} seat(s) for postponement. Currently selected: {selectedSeatsArray.Length}");
                        return;
                    }
                }

                // ✅ VALIDATE JOURNEY DATE
                if (string.IsNullOrEmpty(date_of_journey.Text))
                {
                    ShowClientAlert("Please select journey date");
                    return;
                }

                // ✅ VALIDATE JOURNEY DATE IS NOT IN THE PAST
                DateTime selectedDate;
                if (DateTime.TryParse(date_of_journey.Text, out selectedDate))
                {
                    if (selectedDate.Date < DateTime.Now.Date)
                    {
                        ShowClientAlert("Journey date cannot be in the past. Please select today or a future date.");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(ddlPickup.SelectedValue) || string.IsNullOrEmpty(ddlDestination.SelectedValue))
                {
                    ShowClientAlert("Please select pickup and dropping points");
                    return;
                }

                string userIdStr = null;

                if (Session["userId"] != null && Convert.ToInt32(Session["userId"]) != 0)
                {
                    userIdStr = Session["userId"].ToString();
                }
                else if (!string.IsNullOrEmpty(hfUserId.Value) && hfUserId.Value != "0")
                {
                    userIdStr = hfUserId.Value;
                }

                System.Diagnostics.Debug.WriteLine($"Session userId: {Session["userId"]}");
                System.Diagnostics.Debug.WriteLine($"hfUserId value: {hfUserId.Value}");
                System.Diagnostics.Debug.WriteLine($"Final userIdStr: {userIdStr}");

                if (string.IsNullOrEmpty(userIdStr))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showLogin",
                        "$('#loginModal').modal('show'); console.log('User not logged in - showing modal');", true);
                    return;
                }

                // Store booking information in session
                Session["SelectedSeats"] = selectedSeatsStr;
                Session["JourneyDate"] = date_of_journey.Text;
                Session["PickupPoint"] = ddlPickup.SelectedValue;
                Session["DroppingPoint"] = ddlDestination.SelectedValue;
                Session["TripId"] = Request.QueryString["tripId"];
                Session["VehicleId"] = vehicleId; // ✅ Store vehicleId in session
                Session["UnitPrice"] = hfUnitPrice.Value;
                Session["UserId"] = userIdStr;

                // ✅ CRITICAL: If this is a postpone operation, store the original booking details
                if (!string.IsNullOrEmpty(pnrNumber) && !string.IsNullOrEmpty(bookedIdStr) && status.ToLower() == "booked")
                {
                    Session["CurrentPnrNumber"] = pnrNumber;
                    Session["CurrentBookedId"] = bookedIdStr;
                    Session["IsPostponeMode"] = true;

                    System.Diagnostics.Debug.WriteLine("=== POSTPONE MODE STORED IN SESSION ===");
                    System.Diagnostics.Debug.WriteLine($"Stored PNR: {pnrNumber}");
                    System.Diagnostics.Debug.WriteLine($"Stored BookedId: {bookedIdStr}");
                }
                else
                {
                    // Clear postpone-related session variables for new bookings
                    Session.Remove("CurrentPnrNumber");
                    Session.Remove("CurrentBookedId");
                    Session.Remove("IsPostponeMode");

                    System.Diagnostics.Debug.WriteLine("=== NEW BOOKING MODE ===");
                }

                System.Diagnostics.Debug.WriteLine("=== SESSION DATA STORED ===");
                System.Diagnostics.Debug.WriteLine($"UserId in Session: {Session["UserId"]}");
                System.Diagnostics.Debug.WriteLine($"VehicleId in Session: {Session["VehicleId"]}");

                // Redirect with parameters (for URL visibility and debugging)
                string redirectUrl = $"~/Passenger_Details.aspx?pnrNumber={pnrNumber}&status={status}";

                System.Diagnostics.Debug.WriteLine($"Redirecting to: {redirectUrl}");
                Response.Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in btnProceedToBook_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                ShowClientAlert("Error: " + ex.Message);
            }
        }

        // ✅ MODIFIED: Now accepts vehicleId parameter
        private async Task LoadTripDetailsAndSeats(string tripId, string vehicleId)
        {
            try
            {
                // ✅ Use new API endpoint with vehicleId
                string apiEndpoint = string.IsNullOrEmpty(vehicleId)
                    ? $"{apiUrl}Trips/GetTripDetails?tripId={tripId}"
                    : $"{apiUrl}Trips/GetTripDetailsbyvehicleid?tripId={tripId}&vehicleId={vehicleId}";

                System.Diagnostics.Debug.WriteLine($"=== LOADING TRIP DETAILS ===");
                System.Diagnostics.Debug.WriteLine($"API Endpoint: {apiEndpoint}");

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject tripData = JObject.Parse(jsonResponse);

                    System.Diagnostics.Debug.WriteLine($"Trip Data: {jsonResponse}");

                    ViewState["TripData"] = jsonResponse;
                    ViewState["TripTitle"] = tripData["title"]?.ToString();
                    ViewState["VehicleId"] = tripData["vehicleId"]?.ToString() ?? vehicleId; // ✅ Store vehicleId

                    ddlPickup.Items.Clear();
                    string startFrom = tripData["startFrom"]?.ToString() ?? "";
                    ddlPickup.Items.Add(new ListItem(startFrom, startFrom));
                    ddlPickup.Enabled = false;
                    ViewState["StartFrom"] = startFrom;

                    ddlDestination.Items.Clear();
                    string endTo = tripData["endTo"]?.ToString() ?? "";
                    ddlDestination.Items.Add(new ListItem(endTo, endTo));
                    ddlDestination.Enabled = false;
                    ViewState["EndTo"] = endTo;

                    string dayOffJson = tripData["dayOff"]?.ToString();
                    days_off.Text = "Off Days: " + GetDayNames(dayOffJson);

                    if (!string.IsNullOrWhiteSpace(dayOffJson))
                    {
                        ViewState["DayOffList"] = JsonConvert.DeserializeObject<List<string>>(dayOffJson);
                    }

                    Page.Title = tripData["title"]?.ToString() ?? "Trip Details";

                    string fleetTypeId = tripData["fleetTypeId"]?.ToString();
                    if (!string.IsNullOrEmpty(fleetTypeId))
                    {
                        await LoadFleetTypeAndGenerateSeats(fleetTypeId);
                    }

                    // ✅ Load price from the new API response (it includes price)
                    await LoadTicketPrice(tripData);
                }
                else
                {
                    ShowAlert("Failed to load trip details.");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading trip details: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // ✅ MODIFIED: Now accepts tripData to get price directly from trip details
        private async Task LoadTicketPrice(JObject tripData = null)
        {
            try
            {
                decimal originalPrice = 0;

                // ✅ First try to get price from trip data (new API includes price)
                if (tripData != null && tripData["price"] != null)
                {
                    originalPrice = tripData["price"].Value<decimal>();
                    System.Diagnostics.Debug.WriteLine($"Price from Trip API: {originalPrice}");
                }
                else
                {
                    // Fallback to TicketPrices API
                    string startFrom = ViewState["StartFrom"]?.ToString();
                    string endTo = ViewState["EndTo"]?.ToString();

                    if (string.IsNullOrEmpty(startFrom) || string.IsNullOrEmpty(endTo))
                        return;

                    string vehicleRoute = $"{startFrom} To {endTo}";
                    string apiEndpoint = $"{apiUrl}TicketPrices/GetTicketPrices";
                    HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var ticketPrices = JsonConvert.DeserializeObject<List<TicketPrice>>(jsonResponse);

                        var matchingPrice = ticketPrices.FirstOrDefault(tp =>
                            tp.VehicleRoute.Equals(vehicleRoute, StringComparison.OrdinalIgnoreCase));

                        if (matchingPrice != null)
                        {
                            originalPrice = matchingPrice.Price;
                            System.Diagnostics.Debug.WriteLine($"Price from TicketPrices API: {originalPrice}");
                        }
                    }
                }

                if (originalPrice == 0)
                {
                    hfUnitPrice.Value = "0";
                    System.Diagnostics.Debug.WriteLine("No price found");
                    return;
                }

                string status = Request.QueryString["status"] ?? Session["tripStatus"]?.ToString();
                string ticketCountStr = Request.QueryString["ticketCount"] ?? "0";
                int ticketCount = int.TryParse(ticketCountStr, out var count) ? count : 0;

                if (status != null && status.ToLower() == "booked")
                {
                    string postponeCountStr = Request.QueryString["postponeCount"] ?? "0";
                    int postponeCount = 0;
                    int.TryParse(postponeCountStr, out postponeCount);

                    decimal adjustedPrice;

                    System.Diagnostics.Debug.WriteLine($"=== LOAD TICKET PRICE ===");
                    System.Diagnostics.Debug.WriteLine($"Status: {status}");
                    System.Diagnostics.Debug.WriteLine($"PostponeCount: {postponeCount}");
                    System.Diagnostics.Debug.WriteLine($"Original Price: {originalPrice}");

                    if (postponeCount == 0)
                    {
                        // 1st Postpone: 20% charge
                        adjustedPrice = originalPrice * 0.20M;
                        System.Diagnostics.Debug.WriteLine($"1st Postpone (20%): Charge = {adjustedPrice}");
                    }
                    else if (postponeCount == 1)
                    {
                        // 2nd Postpone: 60% charge
                        adjustedPrice = originalPrice * 0.60M;
                        System.Diagnostics.Debug.WriteLine($"2nd Postpone (60%): Charge = {adjustedPrice}");
                    }
                    else
                    {
                        // 3rd+ Postpone: Not allowed (max 2 postpones)
                        System.Diagnostics.Debug.WriteLine($"Postpone limit exceeded: {postponeCount}");
                        ShowClientAlert("Maximum postpone limit reached (2 times). You cannot postpone this booking again.");
                        Response.Redirect("~/ticket.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    hfUnitPrice.Value = adjustedPrice.ToString();
                    ViewState["UnitPrice"] = adjustedPrice;
                }
                else
                {
                    // New booking - full price
                    hfUnitPrice.Value = originalPrice.ToString();
                    ViewState["UnitPrice"] = originalPrice;
                }

                // CRITICAL: Set exact seat count limit for postpone mode
                if (ticketCount > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "setTicketCountLimit",
                        $@"window.maxSeatsAllowed = {ticketCount};
                           window.isPostponeMode = true;
                           console.log('Postpone mode: Must select exactly ' + {ticketCount} + ' seats');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading ticket price: " + ex.Message);
            }
        }

        private async Task LoadFleetTypeAndGenerateSeats(string fleetTypeId)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}FleetTypes/GetFleetTypes";
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var fleetTypes = JsonConvert.DeserializeObject<List<FleetType>>(jsonResponse);

                    var fleetType = fleetTypes.FirstOrDefault(f => f.Id.ToString() == fleetTypeId);
                    if (fleetType != null)
                    {
                        ViewState["FleetType"] = JsonConvert.SerializeObject(fleetType);
                        GenerateDynamicSeats(fleetType);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading fleet type: " + ex.Message);
            }
        }

        private void GenerateDynamicSeats(FleetType fleetType)
        {
            try
            {
                SeatsContainer.Controls.Clear();

                var layoutParts = fleetType.SeatLayout.Split('x').Select(s => int.Parse(s.Trim())).ToArray();

                int leftSideColumns = layoutParts[0];
                int rightSideColumns = layoutParts.Length > 1 ? layoutParts[1] : layoutParts[0];

                var deckSeatsJson = fleetType.DeckSeats;
                List<int> deckSeatsList = new List<int>();
                try
                {
                    deckSeatsList = JsonConvert.DeserializeObject<List<string>>(deckSeatsJson)
                        .Select(s => int.Parse(s)).ToList();
                }
                catch
                {
                    deckSeatsList.Add(50);
                }

                int totalSeats = deckSeatsList[0];
                int seatsPerRow = leftSideColumns + rightSideColumns;
                int rows = (int)Math.Ceiling((double)totalSeats / seatsPerRow);

                char[] rowLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                int seatCounter = 1;

                for (int row = 0; row < rows && seatCounter <= totalSeats; row++)
                {
                    Panel rowPanel = new Panel();
                    rowPanel.CssClass = "seat-wrapper";
                    rowPanel.ID = $"Row{rowLetters[row]}";

                    Panel leftPanel = new Panel();
                    leftPanel.CssClass = "left-side";

                    for (int col = 0; col < leftSideColumns && seatCounter <= totalSeats; col++)
                    {
                        Panel seatDiv = new Panel();
                        seatDiv.CssClass = "seat-container";

                        string seatLabel = $"{rowLetters[row]}{col + 1}";

                        HyperLink seatBtn = new HyperLink
                        {
                            ID = $"seat_{seatLabel}",
                            Text = seatLabel,
                            CssClass = "seat btn-seat available",
                            NavigateUrl = "javascript:void(0);",
                            ToolTip = $"Seat {seatLabel}"
                        };

                        seatBtn.Attributes.Add("data-seat", seatLabel);
                        seatBtn.Attributes.Add("data-seat-number", seatCounter.ToString());

                        seatDiv.Controls.Add(seatBtn);
                        leftPanel.Controls.Add(seatDiv);
                        seatCounter++;
                    }
                    rowPanel.Controls.Add(leftPanel);

                    Panel aislePanel = new Panel();
                    aislePanel.CssClass = "aisle";
                    rowPanel.Controls.Add(aislePanel);

                    Panel rightPanel = new Panel();
                    rightPanel.CssClass = "right-side";

                    int rightColStart = leftSideColumns + 1;

                    for (int col = 0; col < rightSideColumns && seatCounter <= totalSeats; col++)
                    {
                        Panel seatDiv = new Panel();
                        seatDiv.CssClass = "seat-container";

                        string seatLabel = $"{rowLetters[row]}{rightColStart + col}";

                        HyperLink seatBtn = new HyperLink
                        {
                            ID = $"seat_{seatLabel}",
                            Text = seatLabel,
                            CssClass = "seat btn-seat available",
                            NavigateUrl = "javascript:void(0);",
                            ToolTip = $"Seat {seatLabel}"
                        };

                        seatBtn.Attributes.Add("data-seat", seatLabel);
                        seatBtn.Attributes.Add("data-seat-number", seatCounter.ToString());

                        seatDiv.Controls.Add(seatBtn);
                        rightPanel.Controls.Add(seatDiv);
                        seatCounter++;
                    }
                    rowPanel.Controls.Add(rightPanel);

                    SeatsContainer.Controls.Add(rowPanel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating seats: {ex.Message}");
            }
        }

        protected string GetDayNames(object dayOffObj)
        {
            if (dayOffObj == null || dayOffObj == DBNull.Value || string.IsNullOrWhiteSpace(dayOffObj.ToString()))
                return "Every day available";

            try
            {
                string dayOffStr = dayOffObj.ToString();

                if (dayOffStr.Contains("Monday") || dayOffStr.Contains("Tuesday"))
                {
                    return dayOffStr;
                }

                var dayNumbers = JsonConvert.DeserializeObject<List<string>>(dayOffStr);
                string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                List<string> result = new List<string>();

                foreach (var numStr in dayNumbers)
                {
                    if (int.TryParse(numStr, out int n) && n >= 0 && n <= 6)
                    {
                        result.Add(dayNames[n]);
                    }
                }

                return result.Count > 0 ? string.Join(", ", result) : "Every day available";
            }
            catch
            {
                return dayOffObj.ToString();
            }
        }

        protected async void date_of_journey_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(date_of_journey.Text))
                    return;

                if (DateTime.TryParse(date_of_journey.Text, out DateTime selectedDate))
                {
                    if (ViewState["TripData"] != null)
                    {
                        JObject tripData = JObject.Parse(ViewState["TripData"].ToString());
                        string dayOffStr = tripData["dayOff"]?.ToString();

                        if (!string.IsNullOrEmpty(dayOffStr))
                        {
                            string selectedDayName = selectedDate.DayOfWeek.ToString();
                            int selectedDayNumber = (int)selectedDate.DayOfWeek; // 0=Sunday, 1=Monday, etc.

                            System.Diagnostics.Debug.WriteLine($"Selected Date: {selectedDate}");
                            System.Diagnostics.Debug.WriteLine($"Selected Day: {selectedDayName} ({selectedDayNumber})");
                            System.Diagnostics.Debug.WriteLine($"Day Off String: {dayOffStr}");

                            bool isDayOff = false;

                            // Check if dayOffStr contains day names (like "Monday, Friday")
                            if (dayOffStr.Contains("Monday") || dayOffStr.Contains("Tuesday") ||
                                dayOffStr.Contains("Wednesday") || dayOffStr.Contains("Thursday") ||
                                dayOffStr.Contains("Friday") || dayOffStr.Contains("Saturday") ||
                                dayOffStr.Contains("Sunday"))
                            {
                                // It's stored as day names
                                isDayOff = dayOffStr.Contains(selectedDayName);
                            }
                            else
                            {
                                // It's stored as numbers in JSON array format
                                try
                                {
                                    var dayOffNumbers = JsonConvert.DeserializeObject<List<string>>(dayOffStr);
                                    isDayOff = dayOffNumbers.Contains(selectedDayNumber.ToString());
                                }
                                catch
                                {
                                    // If parsing fails, try direct string comparison
                                    isDayOff = dayOffStr.Contains(selectedDayNumber.ToString());
                                }
                            }

                            if (isDayOff)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "dayoff",
                                    $"alert('Trips are not available on {selectedDayName}. Please select another date.'); " +
                                    $"$('#MainContent_date_of_journey').val('');", true);
                                date_of_journey.Text = "";
                                return;
                            }
                        }
                    }

                    await LoadBookedSeatsForSelectedDate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in date validation: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('Error validating date: {ex.Message}');", true);
            }
        }

        private async Task LoadBookedSeatsForSelectedDate()
        {
            try
            {
                string tripId = Request.QueryString["tripId"];
                string vehicleId = Request.QueryString["vehicleId"]; // ✅ Get vehicleId
                string selectedDate = date_of_journey.Text;

                if (string.IsNullOrEmpty(selectedDate) || string.IsNullOrEmpty(tripId))
                    return;

                DateTime parsedDate;
                if (DateTime.TryParse(selectedDate, out parsedDate))
                {
                    string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                    // ✅ Include vehicleId in the API call if available
                    string endpoint = string.IsNullOrEmpty(vehicleId)
                        ? $"{apiUrl}BookedTicketsNew/GetBookedSeats?tripId={tripId}&dateOfJourney={formattedDate}"
                        : $"{apiUrl}BookedTicketsNew/GetBookedSeats?tripId={tripId}&vehicleId={vehicleId}&dateOfJourney={formattedDate}";

                    System.Diagnostics.Debug.WriteLine($"=== FETCHING BOOKED SEATS ===");
                    System.Diagnostics.Debug.WriteLine($"Selected Date: {formattedDate}");
                    System.Diagnostics.Debug.WriteLine($"Trip ID: {tripId}");
                    System.Diagnostics.Debug.WriteLine($"Vehicle ID: {vehicleId}");
                    System.Diagnostics.Debug.WriteLine($"API Endpoint: {endpoint}");

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine($"API Response: {json}");

                            try
                            {
                                var result = JsonConvert.DeserializeObject<BookedSeatsApiResponse>(json);

                                if (result != null && result.BookedSeats != null && result.BookedSeats.Count > 0)
                                {
                                    System.Diagnostics.Debug.WriteLine($"=== BOOKED SEATS FOR {formattedDate} ===");
                                    System.Diagnostics.Debug.WriteLine($"Total booked seats: {result.BookedSeats.Count}");
                                    System.Diagnostics.Debug.WriteLine($"Seats: {string.Join(", ", result.BookedSeats)}");

                                    MarkBookedSeats(result.BookedSeats);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"✓ No booked seats found for {formattedDate}");
                                    ClearBookedSeats();
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error parsing booked seats: {ex.Message}");
                                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"API request failed: {response.StatusCode}");
                            ClearBookedSeats();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading booked seats: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void MarkBookedSeats(List<string> bookedSeats)
        {
            // First, clear all previously marked booked seats
            ClearBookedSeats();

            // Then mark the new booked seats
            foreach (Control ctrl in SeatsContainer.Controls)
            {
                if (ctrl is Panel rowPanel)
                {
                    MarkSeatsInPanel(rowPanel, bookedSeats);
                }
            }
        }

        private void ClearBookedSeats()
        {
            System.Diagnostics.Debug.WriteLine("Clearing previously marked booked seats...");

            foreach (Control ctrl in SeatsContainer.Controls)
            {
                if (ctrl is Panel rowPanel)
                {
                    ClearSeatsInPanel(rowPanel);
                }
            }
        }

        private void ClearSeatsInPanel(Panel panel)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Panel childPanel)
                {
                    foreach (Control seatDiv in childPanel.Controls)
                    {
                        if (seatDiv is Panel)
                        {
                            foreach (Control seatCtrl in seatDiv.Controls)
                            {
                                if (seatCtrl is HyperLink hl)
                                {
                                    // Reset to available state only if it was booked
                                    if (hl.CssClass.Contains("booked"))
                                    {
                                        hl.CssClass = "seat btn-seat available";
                                        hl.Enabled = true;
                                        System.Diagnostics.Debug.WriteLine($"Cleared seat: {hl.Attributes["data-seat"]}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MarkSeatsInPanel(Panel panel, List<string> bookedSeats)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Panel childPanel)
                {
                    foreach (Control seatDiv in childPanel.Controls)
                    {
                        if (seatDiv is Panel)
                        {
                            foreach (Control seatCtrl in seatDiv.Controls)
                            {
                                if (seatCtrl is HyperLink hl)
                                {
                                    string seatLabel = hl.Attributes["data-seat"];
                                    string tripId = Request.QueryString["tripId"];

                                    bool isBooked = bookedSeats.Any(bs =>
                                        bs == seatLabel ||
                                        bs.EndsWith("-" + seatLabel) ||
                                        bs.Contains(seatLabel));

                                    if (isBooked)
                                    {
                                        hl.CssClass = "seat btn-seat booked";
                                        hl.Enabled = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"showAlert('{message.Replace("'", "\\'")}');", true);
        }

        private void ShowClientAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        public class FleetType
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SeatLayout { get; set; }
            public int Deck { get; set; }
            public string DeckSeats { get; set; }
            public string Facilities { get; set; }
            public int HasAc { get; set; }
            public string Status { get; set; }
        }

        public class TicketPrice
        {
            public int Id { get; set; }
            public string FleetType { get; set; }
            public string VehicleRoute { get; set; }
            public decimal Price { get; set; }
        }

        // New API response structure for GetBookedSeats
        public class BookedSeatsApiResponse
        {
            [JsonProperty("tripId")]
            public int TripId { get; set; }

            [JsonProperty("dateOfJourney")]
            public string DateOfJourney { get; set; }

            [JsonProperty("bookedSeats")]
            public List<string> BookedSeats { get; set; }
        }
    }
}