using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_AdminDashboard : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Control declarations
        protected Literal TotalUsers;
        protected Literal ActiveUsers;
        protected Literal TotalCities;
        protected Literal TotalVehicles;
        protected Literal TodayTicketFare;
        protected Literal CashBookingToday;
        protected Literal OnlineBookingToday;
        protected Literal TrainRunningToday;
        protected GridView gvTrainLatestBookings;
        protected GridView gvtripSummary;
        protected Repeater rpttripDetails;
        protected System.Web.UI.HtmlControls.HtmlGenericControl paymentChartData;

        // Public properties for footer totals
        public decimal TotalTrainTripAmount { get; set; }
        public decimal TotalCashAmount { get; set; }
        public decimal TotalOnlineAmount { get; set; }
        static Train_AdminDashboard()
        {
            apiUrl = ConfigurationManager.AppSettings["api_path"];
            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                //Timeout = TimeSpan.FromSeconds(30)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if admin is logged in
                if (Session["AdminId"] == null)
                {
                    Response.Redirect("TrainAdmin.aspx");
                    return;
                }

                RegisterAsyncTask(new PageAsyncTask(LoadDashboardData));
            }
        }
        private async Task LoadDashboardData()
        {
            try
            {
                await Task.WhenAll(
                    LoadUsersData(),
                    LoadStationData(),
                    LoadVehiclesData(),
                    LoadBookingsData(),
                    LoadPaymentData(),
                    LoadTripSummary(),
                    LoadTripDetailsWithTrain()
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard: {ex.Message}");
                ShowErrorMessage("Unable to load dashboard data. Please try again later.");
            }
        }

        private async Task LoadUsersData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TrainUsers/GetActiveTrainUsers");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Add null handling for deserialization
                    List<UserTrainModel> users = JsonConvert.DeserializeObject<List<UserTrainModel>>(jsonResponse)
                                                 ?? new List<UserTrainModel>();

                    TotalUsers.Text = users.Count.ToString();
                    ActiveUsers.Text = users.Count(u => u.Status == "ACTIVE").ToString();
                }
                else
                {
                    TotalUsers.Text = "0";
                    ActiveUsers.Text = "0";
                }
            }
            catch (JsonException ex)  // Catch deserialization errors separately
            {
                System.Diagnostics.Debug.WriteLine($"JSON deserialization error: {ex.Message}");
                TotalUsers.Text = "0";
                ActiveUsers.Text = "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
                TotalUsers.Text = "0";
                ActiveUsers.Text = "0";
            }
        }
        private async Task LoadStationData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("RailwayStations/GetRailwayStations");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<RailwayStation> stations = JsonConvert.DeserializeObject<List<RailwayStation>>(jsonResponse);

                    if (stations != null)
                    {
                        TotalCities.Text = stations.Count(c => c.Status == "ACTIVE").ToString();
                    }
                }
                else
                {
                    TotalCities.Text = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cities: {ex.Message}");
                TotalCities.Text = null;
            }
        }

        private async Task LoadVehiclesData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TblVehicleUserReg> vehicles = JsonConvert.DeserializeObject<List<TblVehicleUserReg>>(jsonResponse);

                    if (vehicles != null)
                    {
                        TotalVehicles.Text = vehicles.Count(v => v.Status == "ACTIVE").ToString();
                    }
                }
                else
                {
                    TotalVehicles.Text = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading vehicles: {ex.Message}");
                TotalVehicles.Text = "0";
            }
        }

        private async Task LoadBookingsData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TrainTicketBookings/GetAllTrainTicketBookings_forDashboard");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Bookings API Response: {jsonResponse}");

                    List<TrainTicketBooking> bookings = JsonConvert.DeserializeObject<List<TrainTicketBooking>>(jsonResponse);

                    if (bookings != null && bookings.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Total bookings loaded: {bookings.Count}");

                        DateTime today = DateTime.Today;

                        var todayJourneyBookings = bookings.Where(b =>
                            b.JourneyDate.Date == today &&
                            b.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase)).ToList();

                        System.Diagnostics.Debug.WriteLine($"Today's journey bookings: {todayJourneyBookings.Count}");

                        decimal todayTotalFare = todayJourneyBookings.Sum(b => b.TotalAmount);

                        TodayTicketFare.Text = $"{todayTotalFare:N2} CDF";
                        CashBookingToday.Text = $"{todayTotalFare:N2} CDF";
                        OnlineBookingToday.Text = "0.00 CDF";

                        var uniqueVehicles = todayJourneyBookings
                            .Select(b => b.TrainName)
                            .Where(v => !string.IsNullOrEmpty(v))
                            .Distinct()
                            .Count();

                        TrainRunningToday.Text = uniqueVehicles.ToString();

                        System.Diagnostics.Debug.WriteLine($"Unique trains running today (journey date): {uniqueVehicles}");
                        foreach (var train in todayJourneyBookings.Select(b => b.TrainName).Distinct())
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {train}");
                        }

                        var latestBookings = bookings
                            .OrderByDescending(b => b.BookingDate)
                            .Take(5)
                            .Select(b => new TrainBookingDisplayModel
                            {
                                PnrNumber = b.PnrNumber ?? "N/A",
                                Route = b.SourceDestination ?? "N/A",
                                JourneyDate = b.JourneyDate.ToString("dd-MM-yyyy"),
                                User = b.UserName ?? $"User #{b.UserId}",
                                Email = "N/A",
                                TrainName = b.TrainName ?? "N/A",
                                TicketCount = b.TicketCount,
                                Amount = b.TotalAmount,
                                Status = b.BookingStatus
                            })
                            .ToList();

                        gvTrainLatestBookings.DataSource = latestBookings;
                        gvTrainLatestBookings.DataBind();
                    }
                    else
                    {
                        SetEmptyBookingsData();
                    }
                }
                else
                {
                    SetEmptyBookingsData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading bookings: {ex.Message}");
                SetEmptyBookingsData();
            }
        }

        private void SetEmptyBookingsData()
        {
            TodayTicketFare.Text = "0.00 CDF";
            CashBookingToday.Text = "0.00 CDF";
            OnlineBookingToday.Text = "0.00 CDF";
            TrainRunningToday.Text = "0";
            gvTrainLatestBookings.DataSource = new List<TrainBookingDisplayModel>();
            gvTrainLatestBookings.DataBind();
        }

        private async Task LoadPaymentData()
        {
            try
            {
                // Updated API endpoint
                HttpResponseMessage response = await client.GetAsync("TrainTransactions/GetTrainTransactions");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Transactions API Response: {jsonResponse}");

                    List<TrainTransaction> transactions = JsonConvert.DeserializeObject<List<TrainTransaction>>(jsonResponse);

                    if (transactions != null && transactions.Count > 0)
                    {
                        // Filter for successful transactions only (status = "Success")
                        //var successfulTransactions = transactions
                        //    .Where(t => t.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        //    .ToList();
                        var successfulTransactions = transactions
    .Where(t => t.PaymentStatus != null &&
                t.PaymentStatus.Equals("Successful", StringComparison.OrdinalIgnoreCase))
    .ToList();
                        System.Diagnostics.Debug.WriteLine($"Total transactions: {transactions.Count}, Successful: {successfulTransactions.Count}");

                        // Get date range for last 30 days
                        DateTime endDate = DateTime.Today;
                        DateTime startDate = endDate.AddDays(-29);

                        var chartData = new List<ChartDataPoint>();

                        // Create data point for each day
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            var dayAmount = successfulTransactions
                                .Where(t => t.CreatedAt.Date == date)
                                .Sum(t => t.Amount);

                            chartData.Add(new ChartDataPoint
                            {
                                Date = date.ToString("yyyy-MM-dd"),
                                Amount = dayAmount
                            });
                        }

                        System.Diagnostics.Debug.WriteLine($"Chart data points created: {chartData.Count}");

                        // Log some sample data
                        foreach (var point in chartData.Take(5))
                        {
                            System.Diagnostics.Debug.WriteLine($"Date: {point.Date}, Amount: {point.Amount}");
                        }

                        string chartJson = JsonConvert.SerializeObject(chartData, new JsonSerializerSettings
                        {
                            DateFormatString = "yyyy-MM-dd",
                            Formatting = Formatting.None
                        });

                        System.Diagnostics.Debug.WriteLine($"Chart JSON: {chartJson}");

                        // Register the script with proper syntax
                        string script = $@"
                    <script type='text/javascript'>
                        var paymentChartData = {chartJson};
                        console.log('Payment chart data loaded:', paymentChartData);
                    </script>";

                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "PaymentChartData",
                            script,
                            false
                        );
                    }
                    else
                    {
                        RegisterEmptyChartData();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Transactions API failed: {response.StatusCode}");
                    RegisterEmptyChartData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payment data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                RegisterEmptyChartData();
            }
        }

        private void RegisterEmptyChartData()
        {
            string script = @"
        <script type='text/javascript'>
            var paymentChartData = [];
            console.log('Empty payment chart data loaded');
        </script>";

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "PaymentChartData",
                script,
                false
            );
        }

        private async Task LoadTripSummary()
        {
            try
            {
                // Updated API endpoint
                HttpResponseMessage response = await client.GetAsync("TrainTicketBookings/GetAllTrainTicketBookings_forDashboard");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainTicketBooking> bookings = JsonConvert.DeserializeObject<List<TrainTicketBooking>>(jsonResponse);

                    if (bookings != null && bookings.Count > 0)
                    {
                        DateTime today = DateTime.Today;
                        foreach (var b in bookings)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"JourneyDate: {b.JourneyDate}, Status: {b.BookingStatus}, Today: {today}");
                        }
                        // Only consider Booked status
                        //var todayBookings = bookings
                        //    .Where(b => b.JourneyDate.Date == today &&
                        //           b.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                        //    .ToList();

                        var validStatuses = new[] { "Booked", "Postpone" };

                        var todayBookings = bookings
                            .Where(b => b.JourneyDate.Date == today &&
                                   validStatuses.Contains(b.BookingStatus, StringComparer.OrdinalIgnoreCase))
                            .ToList();

                        var tripSummary = todayBookings
                            .GroupBy(b => b.SourceDestination)
                            .Select(g => new TrainTripSummaryModel
                            {
                                TripName = g.Key,
                                TripDate = today,
                                TripCount = g.Count(),
                                TripAmount = g.Sum(b => b.TotalAmount),
                                CashAmount = g.Sum(b => b.TotalAmount),
                                OnlineAmount = 0
                            })
                            .ToList();

                        TotalTrainTripAmount = tripSummary.Sum(t => t.TripAmount);
                        TotalCashAmount = tripSummary.Sum(t => t.CashAmount);
                        TotalOnlineAmount = tripSummary.Sum(t => t.OnlineAmount);

                        gvtripSummary.DataSource = tripSummary;
                        gvtripSummary.DataBind();
                    }
                    else
                    {
                        TotalTrainTripAmount = 0;
                        TotalCashAmount = 0;
                        TotalOnlineAmount = 0;
                        gvtripSummary.DataSource = new List<TrainTripSummaryModel>();
                        gvtripSummary.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trip summary: {ex.Message}");
            }
        }

        private async Task<string> GetVehicleNickname(int trainId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"TblTrainsRegs/GetTblTrainsReg/{trainId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Vehicle {trainId} API Response: {jsonResponse}");

                    var vehicle = JsonConvert.DeserializeObject<TblTrainsReg>(jsonResponse);
                    if (vehicle != null && !string.IsNullOrEmpty(vehicle.TrainName))
                    {
                        return vehicle.TrainName;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to get vehicle {trainId}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting vehicle {trainId}: {ex.Message}");
            }

            return $"Vehicle_{trainId}";
        }

        private async Task<List<string>> GetMatchingVehicleNicknames(string route, int fallbackVehicleId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TblTrainsReg> vehicles = JsonConvert.DeserializeObject<List<TblTrainsReg>>(jsonResponse);

                    if (vehicles != null)
                    {
                        // Extract destination from route (e.g., "Kinshasa-Lufu" -> "Lufu")
                        string destination = route.Split('-').Last().Trim().ToLower();

                        // Find all vehicles whose nickname contains the destination
                        var matchingVehicles = vehicles
                            .Where(v => v.TrainName.ToLower().Contains("_to_" + destination))
                            .Select(v => v.TrainName)
                            .ToList();

                        if (matchingVehicles.Any())
                        {
                            System.Diagnostics.Debug.WriteLine($"✓ Found {matchingVehicles.Count} matching vehicles for route '{route}'");
                            return matchingVehicles;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠ No matching vehicles found for route '{route}', using fallback vehicle ID {fallbackVehicleId}");
                        }
                    }
                }

                // Fallback to getting a single vehicle by ID if no matching vehicles found
                var fallbackNickname = await GetVehicleNickname(fallbackVehicleId);
                return new List<string> { fallbackNickname };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting matching vehicle: {ex.Message}");
                var fallbackNickname = await GetVehicleNickname(fallbackVehicleId);
                return new List<string> { fallbackNickname };
            }
        }

        private async Task LoadTripDetailsWithTrain()
        {
            try
            {
                // Get all bookings (not filtered by tripId)
                HttpResponseMessage response = await client.GetAsync("TrainTicketBookings/GetAllTrainTicketBookings");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"All Bookings API Response: {jsonResponse}");

                    List<TrainTicketBooking> bookings = JsonConvert.DeserializeObject<List<TrainTicketBooking>>(jsonResponse);

                    if (bookings != null && bookings.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"✓ Total bookings deserialized: {bookings.Count}");

                        // Log first booking to check data
                        if (bookings.Count > 0)
                        {
                            var firstBooking = bookings[0];
                            System.Diagnostics.Debug.WriteLine($"First Booking - StartTime: {firstBooking.StartTime}, EndTime: {firstBooking.EndTime}, PassengerCount: {firstBooking.PassengerCount}");
                        }

                        DateTime today = DateTime.Today;

                        // Filter for today's bookings with Booked status
                        var todayBookings = bookings
                            .Where(b => b.JourneyDate.Date == today &&
                                       b.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"Today's booked bookings count: {todayBookings.Count}");

                        // Group by source-destination route
                        var tripDetails = new List<TrainTripDetailViewModel>();


                        //foreach (var routeGroup in todayBookings.GroupBy(b => $"{b.FromStationId}->{b.ToStationId}"))
                        foreach (var routeGroup in todayBookings.GroupBy(b => b.SourceDestination))
                        {
                            var trainDetails = new List<TrainDetailViewModel>();

                            // Group by TripId (or CoachNo, depending on your logic)
                            foreach (var trainGroup in routeGroup.GroupBy(b => b.TripId))
                            {
                                var trainName = trainGroup.First().TrainName;
                                var firstBooking = trainGroup.First();

                                int checkedInCount = trainGroup.Count(b =>
                                    b.CheckinStatus != null &&
                                    (b.CheckinStatus.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                     b.CheckinStatus.Equals("checked", StringComparison.OrdinalIgnoreCase)));

                                int notCheckedInCount = trainGroup.Count(b =>
                                    b.CheckinStatus == null ||
                                    b.CheckinStatus.Equals("no", StringComparison.OrdinalIgnoreCase));

                                var allWeights = trainGroup.Select(b => b.LuggageWeightKg).ToList();
                                string totalWeightDisplay = allWeights.Any()
                                    ? string.Join(", ", allWeights)
                                    : "0 kg";

                                var trainDetail = new TrainDetailViewModel
                                {
                                    TrainName = firstBooking.TrainName,
                                    TripCount = trainGroup.Count(),
                                    PassengerCount = trainGroup.Sum(b => b.TicketCount),
                                    StartTime = firstBooking.StartTime ?? "N/A",
                                    EndTime = firstBooking.EndTime ?? "N/A",
                                    TripAmount = trainGroup.Sum(b => b.TotalAmount),
                                    CashAmount = trainGroup.Sum(b => b.TotalAmount),
                                    OnlineAmount = 0,
                                    TotalBagsCount = trainGroup.Sum(b => b.LuggageCount),
                                    TotalBagsWeight = totalWeightDisplay,
                                    TotalBagsCharge = trainGroup.Sum(b => b.LuggageCharge),
                                    CheckedInCount = checkedInCount,
                                    NotCheckedInCount = notCheckedInCount
                                };

                                trainDetails.Add(trainDetail);
                            }

                            tripDetails.Add(new TrainTripDetailViewModel
                            {
                                TripName = routeGroup.Key,
                                TripDate = today,
                                TripAmount = routeGroup.Sum(b => b.TotalAmount),
                                CashAmount = routeGroup.Sum(b => b.TotalAmount),
                                OnlineAmount = 0,
                                TrainDetails = trainDetails
                            });
                        }

                        System.Diagnostics.Debug.WriteLine($"Total trip details count: {tripDetails.Count}");

                        // Bind the trip details to your front-end Repeater or grid
                        rptTrainTripDetails.DataSource = tripDetails;
                        rptTrainTripDetails.DataBind();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No bookings found");
                        rptTrainTripDetails.DataSource = new List<TrainTripDetailViewModel>();
                        rptTrainTripDetails.DataBind();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Bookings API failed: {response.StatusCode}");
                    rptTrainTripDetails.DataSource = new List<TrainTripDetailViewModel>();
                    rptTrainTripDetails.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trip details: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                rptTrainTripDetails.DataSource = new List<TrainTripDetailViewModel>();
                rptTrainTripDetails.DataBind();
            }
        }

        private void ShowErrorMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        protected void gvtripSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Text = "<b>Grand Total:</b>";
                e.Row.Cells[0].ColumnSpan = 3;
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "badge badge-secondary";

            switch (status.ToLowerInvariant())
            {
                case "booked":
                    return "badge badge-success";
                case "pending":
                    return "badge badge-warning";
                case "cancelled":
                case "rejected":
                    return "badge badge-danger";
                case "postponed":
                    return "badge badge-info";
                default:
                    return "badge badge-secondary";
            }
        }
        public class UserTrainModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("userId")]
            public string UserId { get; set; }

            [JsonProperty("firstname")]
            public string Firstname { get; set; }

            [JsonProperty("lastname")]
            public string Lastname { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("mobile")]
            public string Mobile { get; set; }

            [JsonProperty("userName")]
            public string userName { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("balance")]
            public decimal? Balance { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }
        }
        public class TblVehicleUserReg
        {
            [JsonProperty("vehicleUserId")]
            public string VehicleUserId { get; set; }

            [JsonProperty("vehicleUserName")]
            public string VehicleUserName { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class TrainTicketBooking
        {
            [JsonProperty("bookingId")]
            public int BookingId { get; set; }

            [JsonProperty("coachNo")]
            public int CoachNo { get; set; }

            [JsonProperty("pnrNumber")]
            public string PnrNumber { get; set; }

            [JsonProperty("trainName")]
            public string TrainName { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("userId")]
            public string UserId { get; set; }

            [JsonProperty("tripId")]
            public int TripId { get; set; }

            [JsonProperty("trxId")]
            public int TrxId { get; set; }

            [JsonProperty("fromStationId")]
            public int FromStationId { get; set; }

            [JsonProperty("toStationId")]
            public string ToStationId { get; set; }

            [JsonProperty("startTime")]
            public string StartTime { get; set; }

            [JsonProperty("endTime")]
            public string EndTime { get; set; }

            [JsonProperty("passengerCount")]
            public int PassengerCount { get; set; }

            [JsonProperty("sourceDestination")]
            public string SourceDestination { get; set; }

            [JsonProperty("pickupPoint")]
            public int PickupPoint { get; set; }

            [JsonProperty("droppingPoint")]
            public int DroppingPoint { get; set; }

            [JsonProperty("ticketCount")]
            public int TicketCount { get; set; }

            [JsonProperty("postponeCount")]
            public string PostponeCount { get; set; }

            [JsonProperty("unitPrice")]
            public decimal UnitPrice { get; set; }

            [JsonProperty("journeyDate")]
            public DateTime JourneyDate { get; set; }

            [JsonProperty("coachTypeId")]
            public string CoachTypeId { get; set; }

            [JsonProperty("totalAmount")]
            public decimal TotalAmount { get; set; }

            [JsonProperty("LuggageCount")]
            public int LuggageCount { get; set; }

            [JsonProperty("luggageWeightKg")]
            public decimal LuggageWeightKg { get; set; }

            [JsonProperty("luggageCharge")]
            public decimal LuggageCharge { get; set; }

            [JsonProperty("bookingStatus")]
            public string BookingStatus { get; set; }

            [JsonProperty("paymentStatus")]
            public string PaymentStatus { get; set; }

            [JsonProperty("paymentMethod")]
            public string PaymentMethod { get; set; }

            [JsonProperty("checkinStatus")]
            public string CheckinStatus { get; set; }

            [JsonProperty("transactionId")]
            public int? TransactionId { get; set; }

            [JsonProperty("cancellationCount")]
            public int? CancellationCount { get; set; }

            [JsonProperty("transactionStatus")]
            public string TransactionStatus { get; set; }

            [JsonProperty("isActive")]
            public string IsActive { get; set; }

            [JsonProperty("qrCode")]
            public string QrCode { get; set; }

            [JsonProperty("checkinTime")]
            public DateTime? CheckinTime { get; set; }

            [JsonProperty("bookingDate")]
            public DateTime? BookingDate { get; set; }

            [JsonProperty("cancelledAt")]
            public DateTime? CancelledAt { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime? UpdatedAt { get; set; }
        }

        public class TrainBookingDisplayModel
        {
            public string PnrNumber { get; set; }
            public string Route { get; set; }
            public string JourneyDate { get; set; }
            public string User { get; set; }
            public string Email { get; set; }
            public string TrainName { get; set; }
            public int TicketCount { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
        }

        public class TrainTransaction
        {
            [JsonProperty("trxId")]
            public int TrxId { get; set; }

            [JsonProperty("trxNo")]
            public string TrxNo { get; set; }

            [JsonProperty("bookingId")]
            public int BookingId { get; set; }

            [JsonProperty("paymentMethodId")]
            public int PaymentMethodId { get; set; }

            [JsonProperty("userId")]
            public int UserId { get; set; }

            [JsonProperty("amount")]
            public decimal Amount { get; set; }

            [JsonProperty("extraCharge")]
            public decimal ExtraCharge { get; set; }

            [JsonProperty("postponeAmt1")]
            public decimal PostponeAmt1 { get; set; }

            [JsonProperty("postponeAmt2")]
            public decimal PostponeAmt2 { get; set; }

            [JsonProperty("trxTypeStatus")]
            public string TrxTypeStatus { get; set; }

            [JsonProperty("paymentStatus")]
            public string PaymentStatus { get; set; }

            [JsonProperty("paymentRemarks")]
            public string PaymentRemarks { get; set; }

            [JsonProperty("trxPaymentAt")]
            public DateTime TrxPaymentAt { get; set; }

            [JsonProperty("oldDate")]
            public DateTime OldDate { get; set; }

            [JsonProperty("newDate")]
            public DateTime NewDate { get; set; }

            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime UpdatedAt { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

        }

        public class ChartDataPoint
        {
            public string Date { get; set; }
            public decimal Amount { get; set; }
        }

        public class TrainTripSummaryModel
        {
            public string TripName { get; set; }
            public DateTime TripDate { get; set; }
            public int TripCount { get; set; }
            public decimal TripAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal OnlineAmount { get; set; }
        }

        public partial class TblTrainsReg
        {
            public int Id { get; set; }

            public string TrainId { get; set; }

            public string TrainName { get; set; }

            public string TrainNumber { get; set; }

            public string RegistrationNumber { get; set; }

            public int AcquisitionYear { get; set; }

            public string OriginCountry { get; set; }

            public string EngineNo { get; set; }

            public string ChasisNo { get; set; }

            public string ModelNo { get; set; }

            public int FleetTypeId { get; set; }

            public string DayOff { get; set; }

            public bool IsActive { get; set; }

            public string Status { get; set; }

            public string CreatedBy { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        public class TrainTripDetailViewModel
        {
            public string TripName { get; set; }
            public DateTime TripDate { get; set; }
            public decimal TripAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal OnlineAmount { get; set; }
            public List<TrainDetailViewModel> TrainDetails { get; set; }
        }

        public class TrainDetailViewModel
        {
            public string TrainName { get; set; }
            public int TripCount { get; set; }
            public int PassengerCount { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public decimal TripAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal OnlineAmount { get; set; }
            public int TotalBagsCount { get; set; }
            public string TotalBagsWeight { get; set; }
            public decimal TotalBagsCharge { get; set; }
            public int CheckedInCount { get; set; }
            public int NotCheckedInCount { get; set; }
            public int FromStationId { get; set; }
            public int ToStationId { get; set; }
            public int TripId { get; set; }
            public string CoachNo { get; set; }
            public int TicketCount { get; set; }
        }
        protected string GetTrainOptions(List<TrainDetailViewModel> trainDetails)
        {
            if (trainDetails == null || !trainDetails.Any())
                return string.Empty;

            var options = new System.Text.StringBuilder();
            for (int i = 0; i < trainDetails.Count; i++)
            {
                options.AppendFormat("<option value='{0}'>{1}</option>",
                    i,
                    System.Web.HttpUtility.HtmlEncode(trainDetails[i].TrainName));
            }
            return options.ToString();
        }


    }
}