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

namespace Excel_Bus
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Control declarations
        protected Literal litTotalUsers;
        protected Literal litActiveUsers;
        protected Literal litTotalCities;
        protected Literal litTotalVehicles;
        protected Literal litTodayTicketFare;
        protected Literal litCashBookingToday;
        protected Literal litOnlineBookingToday;
        protected Literal litBusRunningToday;
        protected GridView gvLatestBookings;
        protected GridView gvTripSummary;
        protected Repeater rptTripDetails;
        protected System.Web.UI.HtmlControls.HtmlGenericControl paymentChartData;

        // Public properties for footer totals
        public decimal TotalTripAmount { get; set; }
        public decimal TotalCashAmount { get; set; }
        public decimal TotalOnlineAmount { get; set; }

        static AdminDashboard()
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
                    Response.Redirect("Admin.aspx");
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
                    LoadCitiesData(),
                    LoadVehiclesData(),
                    LoadBookingsData(),
                    LoadPaymentData(),
                    LoadTripSummary(),
                    LoadTripDetailsWithBus()
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
                HttpResponseMessage response = await client.GetAsync("Users/GetUsers");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<UserModel> users = JsonConvert.DeserializeObject<List<UserModel>>(jsonResponse);

                    if (users != null)
                    {
                        litTotalUsers.Text = users.Count.ToString();
                        litActiveUsers.Text = users.Count(u => u.Status == 1).ToString();
                    }
                }
                else
                {
                    litTotalUsers.Text = "0";
                    litActiveUsers.Text = "0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
                litTotalUsers.Text = "0";
                litActiveUsers.Text = "0";
            }
        }

        private async Task LoadCitiesData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("Counters/GetCounters");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<CounterModel> counters = JsonConvert.DeserializeObject<List<CounterModel>>(jsonResponse);

                    if (counters != null)
                    {
                        litTotalCities.Text = counters.Count(c => c.Status == 1).ToString();
                    }
                }
                else
                {
                    litTotalCities.Text = "0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cities: {ex.Message}");
                litTotalCities.Text = "0";
            }
        }

        private async Task LoadVehiclesData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("Vehicles/GetVehicles");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(jsonResponse);

                    if (vehicles != null)
                    {
                        litTotalVehicles.Text = vehicles.Count(v => v.Status == 1).ToString();
                    }
                }
                else
                {
                    litTotalVehicles.Text = "0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading vehicles: {ex.Message}");
                litTotalVehicles.Text = "0";
            }
        }

        private async Task LoadBookingsData()
        {
            try
            {
                // Updated API endpoint
                HttpResponseMessage response = await client.GetAsync("BookedTicketsNew/GetBookedTickets");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Bookings API Response: {jsonResponse}");

                    List<BookingModel> bookings = JsonConvert.DeserializeObject<List<BookingModel>>(jsonResponse);

                    if (bookings != null && bookings.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Total bookings loaded: {bookings.Count}");

                        DateTime today = DateTime.Today;

                        // Filter for today's JOURNEY bookings (dateOfJourney = today), not creation date
                        var todayJourneyBookings = bookings.Where(b =>
                            b.DateOfJourney.Date == today &&
                            b.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase)).ToList();

                        System.Diagnostics.Debug.WriteLine($"Today's journey bookings: {todayJourneyBookings.Count}");

                        // Calculate today's totals (only for Booked status with journey today)
                        decimal todayTotalFare = todayJourneyBookings.Sum(b => b.SubTotal);

                        litTodayTicketFare.Text = $"{todayTotalFare:N2} CDF";
                        litCashBookingToday.Text = $"{todayTotalFare:N2} CDF";
                        litOnlineBookingToday.Text = "0.00 CDF";

                        // Count unique vehicles running today (based on journey date)
                        var uniqueVehicles = todayJourneyBookings
                            .Select(b => b.VehicleNickName)
                            .Where(v => !string.IsNullOrEmpty(v))
                            .Distinct()
                            .Count();

                        litBusRunningToday.Text = uniqueVehicles.ToString();

                        System.Diagnostics.Debug.WriteLine($"Unique buses running today (journey date): {uniqueVehicles}");
                        foreach (var bus in todayJourneyBookings.Select(b => b.VehicleNickName).Distinct())
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {bus}");
                        }

                        // Get latest 5 bookings with correct mapping including vehicle name
                        var latestBookings = bookings
                            .OrderByDescending(b => b.CreatedAt)
                            .Take(5)
                            .Select(b => new BookingDisplayModel
                            {
                                PnrNumber = b.PnrNumber ?? "N/A",
                                Route = b.SourceDestination ?? "N/A",
                                JourneyDate = b.DateOfJourney.ToString("dd-MM-yyyy"),
                                User = b.UserName ?? $"User #{b.UserId}",
                                Email = "N/A",
                                Mobile = "N/A",
                                VehicleName = b.VehicleNickName ?? "N/A", // Added vehicle name
                                TicketCount = b.TicketCount,
                                Amount = b.SubTotal,
                                Status = b.Status
                            })
                            .ToList();

                        gvLatestBookings.DataSource = latestBookings;
                        gvLatestBookings.DataBind();
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
            litTodayTicketFare.Text = "0.00 CDF";
            litCashBookingToday.Text = "0.00 CDF";
            litOnlineBookingToday.Text = "0.00 CDF";
            litBusRunningToday.Text = "0";
            gvLatestBookings.DataSource = new List<BookingDisplayModel>();
            gvLatestBookings.DataBind();
        }

        private async Task LoadPaymentData()
        {
            try
            {
                // Updated API endpoint
                HttpResponseMessage response = await client.GetAsync("TransactionsNew/GetTransactions");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Transactions API Response: {jsonResponse}");

                    List<TransactionModel> transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(jsonResponse);

                    if (transactions != null && transactions.Count > 0)
                    {
                        // Filter for successful transactions only (status = "Success")
                        var successfulTransactions = transactions
                            .Where(t => t.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
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
                HttpResponseMessage response = await client.GetAsync("BookedTicketsNew/GetBookedTickets");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<BookingModel> bookings = JsonConvert.DeserializeObject<List<BookingModel>>(jsonResponse);

                    if (bookings != null && bookings.Count > 0)
                    {
                        DateTime today = DateTime.Today;
                        // Only consider Booked status
                        var todayBookings = bookings
                            .Where(b => b.DateOfJourney.Date == today &&
                                   b.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        var tripSummary = todayBookings
                            .GroupBy(b => b.SourceDestination)
                            .Select(g => new TripSummaryModel
                            {
                                TripName = g.Key,
                                TripDate = today,
                                TripCount = g.Count(),
                                TripAmount = g.Sum(b => b.SubTotal),
                                CashAmount = g.Sum(b => b.SubTotal),
                                OnlineAmount = 0
                            })
                            .ToList();

                        TotalTripAmount = tripSummary.Sum(t => t.TripAmount);
                        TotalCashAmount = tripSummary.Sum(t => t.CashAmount);
                        TotalOnlineAmount = tripSummary.Sum(t => t.OnlineAmount);

                        gvTripSummary.DataSource = tripSummary;
                        gvTripSummary.DataBind();
                    }
                    else
                    {
                        TotalTripAmount = 0;
                        TotalCashAmount = 0;
                        TotalOnlineAmount = 0;
                        gvTripSummary.DataSource = new List<TripSummaryModel>();
                        gvTripSummary.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trip summary: {ex.Message}");
            }
        }

        private async Task<string> GetVehicleNickname(int vehicleId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"Vehicles/GetVehicle/{vehicleId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Vehicle {vehicleId} API Response: {jsonResponse}");

                    var vehicle = JsonConvert.DeserializeObject<VehicleModel>(jsonResponse);
                    if (vehicle != null && !string.IsNullOrEmpty(vehicle.NickName))
                    {
                        return vehicle.NickName;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to get vehicle {vehicleId}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting vehicle {vehicleId}: {ex.Message}");
            }

            return $"Vehicle_{vehicleId}";
        }

        private async Task<List<string>> GetMatchingVehicleNicknames(string route, int fallbackVehicleId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("Vehicles/GetVehicles");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(jsonResponse);

                    if (vehicles != null)
                    {
                        // Extract destination from route (e.g., "Kinshasa-Lufu" -> "Lufu")
                        string destination = route.Split('-').Last().Trim().ToLower();

                        // Find all vehicles whose nickname contains the destination
                        var matchingVehicles = vehicles
                            .Where(v => v.NickName.ToLower().Contains("_to_" + destination))
                            .Select(v => v.NickName)
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

        private async Task LoadTripDetailsWithBus()
        {
            try
            {
                // Get all bookings (not filtered by tripId)
                HttpResponseMessage response = await client.GetAsync("BookedTicketsNew/GetBookedTickets");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"All Bookings API Response: {jsonResponse}");

                    List<BookingModel> bookings = JsonConvert.DeserializeObject<List<BookingModel>>(jsonResponse);

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
                            .Where(b => b.DateOfJourney.Date == today &&
                                       b.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"Today's booked bookings count: {todayBookings.Count}");

                        // Group by source-destination route
                        var tripDetails = new List<TripDetailViewModel>();

                        foreach (var routeGroup in todayBookings.GroupBy(b => b.SourceDestination))
                        {
                            var busDetails = new List<BusDetailViewModel>();

                            // Group by vehicle name within each route
                            foreach (var busGroup in routeGroup.GroupBy(b => b.VehicleNickName))
                            {
                                var busName = busGroup.Key ?? "Unknown Bus";

                                // Get first booking for timing info (all bookings in same group will have same timing)
                                var firstBooking = busGroup.First();

                                // Calculate checked-in BOOKINGS count (not passengers)
                                int checkedInCount = busGroup.Count(b => b.Checkin != null &&
                                    (b.Checkin.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                     b.Checkin.Equals("checked", StringComparison.OrdinalIgnoreCase)));

                                int notCheckedInCount = busGroup.Count(b => b.Checkin == null ||
                                    b.Checkin.Equals("no", StringComparison.OrdinalIgnoreCase));

                                // Collect all bag weights (filter out null/empty values)
                                var allWeights = busGroup
                                    .Where(b => !string.IsNullOrEmpty(b.BagsWeight))
                                    .Select(b => b.BagsWeight)
                                    .ToList();

                                string totalWeightDisplay = allWeights.Any()
                                    ? string.Join(", ", allWeights)
                                    : "0 kg";

                                var busDetail = new BusDetailViewModel
                                {
                                    BusName = busName,
                                    TripCount = busGroup.Count(),
                                    PassengerCount = busGroup.Sum(b => b.PassengerCount),
                                    StartTime = firstBooking.StartTime ?? "N/A",
                                    EndTime = firstBooking.EndTime ?? "N/A",
                                    TripAmount = busGroup.Sum(b => b.SubTotal),
                                    CashAmount = busGroup.Sum(b => b.SubTotal),
                                    OnlineAmount = 0,
                                    TotalBagsCount = busGroup.Sum(b => b.BagsCount),
                                    TotalBagsWeight = totalWeightDisplay,
                                    TotalBagsCharge = busGroup.Sum(b => b.BagsCharge),
                                    CheckedInCount = checkedInCount,
                                    NotCheckedInCount = notCheckedInCount
                                };

                                busDetails.Add(busDetail);

                                System.Diagnostics.Debug.WriteLine($"✓ Route: {routeGroup.Key}, Bus: {busName}");
                                System.Diagnostics.Debug.WriteLine($"  - Bookings: {busGroup.Count()}");
                                System.Diagnostics.Debug.WriteLine($"  - Passengers: {busGroup.Sum(b => b.PassengerCount)}");
                                System.Diagnostics.Debug.WriteLine($"  - Start: {firstBooking.StartTime}, End: {firstBooking.EndTime}");
                                System.Diagnostics.Debug.WriteLine($"  - Bags: {busGroup.Sum(b => b.BagsCount)}, Weight: {totalWeightDisplay}, Charge: {busGroup.Sum(b => b.BagsCharge)}");
                                System.Diagnostics.Debug.WriteLine($"  - Bookings Checked In: Yes={checkedInCount}, No={notCheckedInCount}");
                                System.Diagnostics.Debug.WriteLine($"  - Amount: {busGroup.Sum(b => b.SubTotal)}");
                            }

                            tripDetails.Add(new TripDetailViewModel
                            {
                                TripName = routeGroup.Key,
                                TripDate = today,
                                TripAmount = routeGroup.Sum(b => b.SubTotal),
                                CashAmount = routeGroup.Sum(b => b.SubTotal),
                                OnlineAmount = 0,
                                BusDetails = busDetails
                            });

                            System.Diagnostics.Debug.WriteLine($"Added trip detail for route: {routeGroup.Key} with {busDetails.Count} buses");
                        }

                        System.Diagnostics.Debug.WriteLine($"Total trip details count: {tripDetails.Count}");

                        // Bind the trip details to your front-end Repeater or grid
                        rptTripDetails.DataSource = tripDetails;
                        rptTripDetails.DataBind();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No bookings found");
                        rptTripDetails.DataSource = new List<TripDetailViewModel>();
                        rptTripDetails.DataBind();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Bookings API failed: {response.StatusCode}");
                    rptTripDetails.DataSource = new List<TripDetailViewModel>();
                    rptTripDetails.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading trip details: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                rptTripDetails.DataSource = new List<TripDetailViewModel>();
                rptTripDetails.DataBind();
            }
        }

        private void ShowErrorMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        protected void gvTripSummary_RowDataBound(object sender, GridViewRowEventArgs e)
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

            switch (status.ToLower())
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

        #region Model Classes

        public class UserModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("firstname")]
            public string Firstname { get; set; }

            [JsonProperty("lastname")]
            public string Lastname { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("mobile")]
            public string Mobile { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }
        }

        public class CounterModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }

            [JsonProperty("mobile")]
            public string Mobile { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }
        }

        public class VehicleModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("nickName")]
            public string NickName { get; set; }

            [JsonProperty("registerNo")]
            public string RegisterNo { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }
        }

        public class BookingModel
        {
            [JsonProperty("bookedId")]
            public int Id { get; set; }

            [JsonProperty("userId")]
            public int UserId { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("tripId")]
            public int TripId { get; set; }

            [JsonProperty("tripTitle")]
            public string TripTitle { get; set; }

            [JsonProperty("vehicleId")]
            public int VehicleId { get; set; }

            [JsonProperty("vehicleNickName")]
            public string VehicleNickName { get; set; }

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

            [JsonProperty("subTotal")]
            public decimal SubTotal { get; set; }

            [JsonProperty("dateOfJourney")]
            public DateTime DateOfJourney { get; set; }

            [JsonProperty("pnrNumber")]
            public string PnrNumber { get; set; }

            [JsonProperty("checkin")]
            public string Checkin { get; set; }

            [JsonProperty("bagsCount")]
            public int BagsCount { get; set; }

            [JsonProperty("bagsWeight")]
            public string BagsWeight { get; set; }

            [JsonProperty("bagsCharge")]
            public decimal BagsCharge { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("transactionId")]
            public int? TransactionId { get; set; }

            [JsonProperty("transactionStatus")]
            public string TransactionStatus { get; set; }

            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime UpdatedAt { get; set; }
        }

        public class BookingDisplayModel
        {
            public string PnrNumber { get; set; }
            public string Route { get; set; }
            public string JourneyDate { get; set; }
            public string User { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string VehicleName { get; set; } // Added vehicle name property
            public int TicketCount { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
        }

        public class TransactionModel
        {
            [JsonProperty("trxId")]
            public int Id { get; set; }

            [JsonProperty("userId")]
            public int UserId { get; set; }

            [JsonProperty("amount")]
            public decimal Amount { get; set; }

            [JsonProperty("charge")]
            public decimal Charge { get; set; }

            [JsonProperty("postBalance")]
            public decimal PostBalance { get; set; }

            [JsonProperty("trxType")]
            public string TrxType { get; set; }

            [JsonProperty("trxNumber")]
            public string Trx { get; set; }

            [JsonProperty("details")]
            public string Details { get; set; }

            [JsonProperty("remark")]
            public string Remark { get; set; }

            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime UpdatedAt { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("bookedId")]
            public int? BookedId { get; set; }

            [JsonProperty("pnrNumber")]
            public string PnrNumber { get; set; }
        }

        public class ChartDataPoint
        {
            public string Date { get; set; }
            public decimal Amount { get; set; }
        }

        public class TripSummaryModel
        {
            public string TripName { get; set; }
            public DateTime TripDate { get; set; }
            public int TripCount { get; set; }
            public decimal TripAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal OnlineAmount { get; set; }
        }

        public class TripDetailViewModel
        {
            public string TripName { get; set; }
            public DateTime TripDate { get; set; }
            public decimal TripAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal OnlineAmount { get; set; }
            public List<BusDetailViewModel> BusDetails { get; set; }
        }

        public class BusDetailViewModel
        {
            public string BusName { get; set; }
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
        }

        protected string GetBusOptions(List<BusDetailViewModel> busDetails)
        {
            if (busDetails == null || !busDetails.Any())
                return string.Empty;

            var options = new System.Text.StringBuilder();
            for (int i = 0; i < busDetails.Count; i++)
            {
                options.AppendFormat("<option value='{0}'>{1}</option>",
                    i,
                    System.Web.HttpUtility.HtmlEncode(busDetails[i].BusName));
            }
            return options.ToString();
        }
        #endregion
    }
}