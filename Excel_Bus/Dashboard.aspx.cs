using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace ExcelBus
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        private string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        // Constructor to configure HttpClient once
        public Dashboard()
        {
            if (client.BaseAddress == null)
            {
                try
                {
                    string url = ConfigurationSettings.AppSettings["api_path"];
                    if (!string.IsNullOrEmpty(url))
                    {
                        client.BaseAddress = new Uri(url);
                        //client.Timeout = TimeSpan.FromSeconds(60);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error configuring HttpClient: {ex.Message}");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check session
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                // Load trips menu
                LoadTripsMenu();
            }

            // Always load dashboard data - regardless of postback
            PageAsyncTask task = new PageAsyncTask(LoadDashboardData);
            RegisterAsyncTask(task);
        }

        //private void LoadTripsMenu()
        //{
        //    try
        //    {
        //        using (var tripClient = new HttpClient())
        //        {
        //            tripClient.DefaultRequestHeaders.Clear();
        //            tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

        //            string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip1";
        //            System.Diagnostics.Debug.WriteLine("Fetching trips from: " + url);

        //            var response = tripClient.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = response.Content.ReadAsStringAsync().Result;
        //                System.Diagnostics.Debug.WriteLine("Trips API Response received");

        //                var trips = JsonConvert.DeserializeObject<List<TripDetail>>(json);

        //                if (trips != null && trips.Count > 0)
        //                {
        //                    tripsMenu.Controls.Clear();

        //                    // Add "All Trips" option
        //                    var allTripsLi = new HtmlGenericControl("li");
        //                    var allTripsLink = new HtmlGenericControl("a");
        //                    allTripsLink.Attributes["href"] = "ticket.aspx";
        //                    allTripsLink.InnerText = "-- All Trips --";
        //                    allTripsLi.Controls.Add(allTripsLink);
        //                    tripsMenu.Controls.Add(allTripsLi);

        //                    // Add each active trip
        //                    foreach (var trip in trips)
        //                    {
        //                        if (trip.Status == 1)
        //                        {
        //                            var li = new HtmlGenericControl("li");
        //                            var anchor = new HtmlGenericControl("a");
        //                            anchor.Attributes["href"] = $"ticket.aspx?tripId={trip.TripId}";

        //                            anchor.InnerText = trip.Title;

        //                            li.Controls.Add(anchor);
        //                            tripsMenu.Controls.Add(li);

        //                            System.Diagnostics.Debug.WriteLine($"Added trip: {trip.Title} (ID: {trip.TripId})");
        //                        }
        //                    }

        //                    System.Diagnostics.Debug.WriteLine($"Total trips loaded: {trips.Count}");
        //                }
        //                else
        //                {
        //                    AddNoTripsMessage();
        //                }
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Trips API failed: {response.StatusCode}");
        //                AddNoTripsMessage();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error loading trips menu: " + ex.Message);
        //        AddNoTripsMessage();
        //    }
        //}
        private void LoadTripsMenu()
        {
            try
            {
                using (var tripClient = new HttpClient())
                {
                    tripClient.DefaultRequestHeaders.Clear();
                    tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip1";
                    System.Diagnostics.Debug.WriteLine("Fetching trips from: " + url);

                    var response = tripClient.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("Trips API Response received");

                        // Deserialize the JSON response into a list of TripDetail
                        var trips = JsonConvert.DeserializeObject<List<TripDetail>>(json);

                        if (trips != null && trips.Count > 0)
                        {
                            //tripsMenu.Controls.Clear();

                            // Add "All Trips" option
                            var allTripsLi = new HtmlGenericControl("li");
                            var allTripsLink = new HtmlGenericControl("a");
                            allTripsLink.Attributes["href"] = "ticket.aspx";
                            allTripsLink.InnerText = "-- All Trips --";
                            allTripsLi.Controls.Add(allTripsLink);
                            //tripsMenu.Controls.Add(allTripsLi);

                            // Add each active trip
                            foreach (var trip in trips)
                            {
                                if (trip.Status == 1) // Only show active trips
                                {
                                    var li = new HtmlGenericControl("li");
                                    var anchor = new HtmlGenericControl("a");

                                    // Use vehicleRouteId as the query parameter
                                    anchor.Attributes["href"] = $"ticket.aspx?vehicleRouteId={trip.VehicleRouteId}";

                                    anchor.InnerText = trip.Title;

                                    li.Controls.Add(anchor);
                                    //tripsMenu.Controls.Add(li);

                                    System.Diagnostics.Debug.WriteLine($"Added trip: {trip.Title} (VehicleRouteId: {trip.VehicleRouteId})");
                                }
                            }

                            System.Diagnostics.Debug.WriteLine($"Total trips loaded: {trips.Count}");
                        }
                        else
                        {
                            AddNoTripsMessage();
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Trips API failed: {response.StatusCode}");
                        AddNoTripsMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading trips menu: " + ex.Message);
                AddNoTripsMessage();
            }
        }

        //private void LoadTripsMenu()
        //{
        //    try
        //    {
        //        using (var tripClient = new HttpClient())
        //        {
        //            tripClient.DefaultRequestHeaders.Clear();
        //            tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

        //            string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip1";
        //            System.Diagnostics.Debug.WriteLine("Fetching trips from: " + url);

        //            var response = tripClient.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = response.Content.ReadAsStringAsync().Result;
        //                System.Diagnostics.Debug.WriteLine("Trips API Response received");

        //                // Deserialize the JSON response into a list of TripDetail
        //                var trips = JsonConvert.DeserializeObject<List<TripDetail>>(json);
                        
        //                if (trips != null && trips.Count > 0)
        //                {
        //                    tripsMenu.Controls.Clear();

        //                    // Add "All Trips" option
        //                    var allTripsLi = new HtmlGenericControl("li");
        //                    var allTripsLink = new HtmlGenericControl("a");
        //                    allTripsLink.Attributes["href"] = "ticket.aspx";
        //                    allTripsLink.InnerText = "-- All Trips --";
        //                    allTripsLi.Controls.Add(allTripsLink);
        //                    tripsMenu.Controls.Add(allTripsLi);

        //                    // Add each active trip
        //                    foreach (var trip in trips)
        //                    {
        //                        if (trip.Status == 1) 
        //                        {
        //                            var li = new HtmlGenericControl("li");
        //                            var anchor = new HtmlGenericControl("a");

        //                            // Use vehicleRouteId as the query parameter
        //                            anchor.Attributes["href"] = $"ticket.aspx?vehicleRouteId={trip.TripId}";

        //                            anchor.InnerText = trip.Title;

        //                            li.Controls.Add(anchor);
        //                            tripsMenu.Controls.Add(li);

        //                            System.Diagnostics.Debug.WriteLine($"Added trip: {trip.Title} (VehicleRouteId: {trip.TripId})");
        //                        }
        //                    }

        //                    System.Diagnostics.Debug.WriteLine($"Total trips loaded: {trips.Count}");
        //                }
        //                else
        //                {
        //                    AddNoTripsMessage();
        //                }
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Trips API failed: {response.StatusCode}");
        //                AddNoTripsMessage();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error loading trips menu: " + ex.Message);
        //        AddNoTripsMessage();
        //    }
        //}



        private void AddNoTripsMessage()
        {
            //tripsMenu.Controls.Clear();
            var li = new HtmlGenericControl("li");
            var anchor = new HtmlGenericControl("a");
            anchor.Attributes["href"] = "#";
            anchor.InnerText = "No trips available";
            anchor.Attributes["style"] = "color: #999; cursor: default;";
            li.Controls.Add(anchor);
            //tripsMenu.Controls.Add(li);
        }

        private async Task LoadDashboardData()
        {
            try
            {
                // Verify session again
                if (Session["UserId"] == null)
                {
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine($"Loading dashboard for user: {userId}");

                var bookings = await GetBookedTickets(userId);

                // Initialize with zeros first
                litTotalBooked.Text = "0";
                litTotalRejected.Text = "0";
                litTotalPending.Text = "0";
                litTotalCancelled.Text = "0";
                litTotalPostponed.Text = "0";

                if (bookings != null && bookings.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Successfully loaded {bookings.Count} bookings");

                    // Calculate statistics based on actual status values with null checks
                    int totalBooked = bookings.Count(b => b.Status != null && b.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase));
                    int totalRejected = bookings.Count(b => b.Status != null && b.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase));
                    int totalPending = bookings.Count(b => b.Status != null && b.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
                    int totalCancelled = bookings.Count(b => b.IsActive != null && b.IsActive.Equals("false", StringComparison.OrdinalIgnoreCase));

                    // Count tickets with postponeCount > 0
                    int totalPostponed = bookings.Count(b =>
                    {
                        if (string.IsNullOrEmpty(b.PostponeCount))
                            return false;
                        int count;
                        return int.TryParse(b.PostponeCount, out count) && count > 0;
                    });

                    // Update literals
                    litTotalBooked.Text = totalBooked.ToString();
                    litTotalRejected.Text = totalRejected.ToString();
                    litTotalPending.Text = totalPending.ToString();
                    litTotalCancelled.Text = totalCancelled.ToString();
                    litTotalPostponed.Text = totalPostponed.ToString();

                    System.Diagnostics.Debug.WriteLine($"Stats - Booked: {totalBooked}, Pending: {totalPending}, Cancelled: {totalCancelled}, Rejected: {totalRejected}");

                    var bookingList = new List<BookingViewModel>();

                    foreach (var b in bookings)
                    {
                        try
                        {
                            var viewModel = new BookingViewModel
                            {
                                Id = b.BookedId,
                                PnrNumber = b.PnrNumber ?? "N/A",
                                DateOfJourney = ParseDate(b.DateOfJourney),
                                DateOfJourneyFormatted = FormatDate(b.DateOfJourney),
                                SubTotal = b.SubTotal,
                                Status = b.Status ?? "Unknown",
                                StatusText = b.Status ?? "Unknown",
                                PostponeCount = ParsePostponeCount(b.PostponeCount),
                                PickupName = ExtractLocationName(b.SourceDestination, true),
                                DropName = ExtractLocationName(b.SourceDestination, false),
                                PickupTime = "N/A", 
                                SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                IsAC = "AC",
                                BagsCount = b.BagsCount,
                                BagsCharge = b.BagsCharge,
                                TripTitle = b.TripTitle ?? ""
                            };

                            bookingList.Add(viewModel);
                            System.Diagnostics.Debug.WriteLine($"Added booking: PNR={viewModel.PnrNumber}, Status={viewModel.Status}, Seats={viewModel.SeatsDisplay}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing booking {b.PnrNumber}: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                            // Continue processing other bookings
                        }
                    }

                    var sortedList = bookingList.OrderByDescending(b => b.CreatedAt).ToList();

                    rptBookings.DataSource = sortedList;
                    rptBookings.DataBind();

                    System.Diagnostics.Debug.WriteLine($"Successfully bound {sortedList.Count} bookings to repeater");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No bookings found for this user");
                    rptBookings.DataSource = new List<BookingViewModel>();
                    rptBookings.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Set default values on error
                litTotalBooked.Text = "0";
                litTotalRejected.Text = "0";
                litTotalPending.Text = "0";
                litTotalCancelled.Text = "0";
                litTotalPostponed.Text = "0";

                rptBookings.DataSource = new List<BookingViewModel>();
                rptBookings.DataBind();

                ShowMessage("Unable to load dashboard data. Please try again.", "error");
            }
        }

        private async Task<List<BookedTicket>> GetBookedTickets(int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(apiUrl))
                {
                    System.Diagnostics.Debug.WriteLine("API URL is not configured");
                    return new List<BookedTicket>();
                }

                string endpoint = $"BookedTicketsNew/GetBookedTicketsByUserId/{userId}";
                string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

                System.Diagnostics.Debug.WriteLine($"Fetching bookings from: {fullUrl}");

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return new List<BookedTicket>();
                }

                string jsonResult = await response.Content.ReadAsStringAsync();

                 if (jsonResult.Length > 500)
                {
                    System.Diagnostics.Debug.WriteLine($"API Response received (first 500 chars): {jsonResult.Substring(0, 500)}...");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Response: {jsonResult}");
                }

                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                {
                    System.Diagnostics.Debug.WriteLine("Empty or no bookings in response");
                    return new List<BookedTicket>();
                }

                var tickets = JsonConvert.DeserializeObject<List<BookedTicket>>(jsonResult);

                if (tickets != null && tickets.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Successfully deserialized {tickets.Count} tickets");
                    foreach (var ticket in tickets)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ticket: PNR={ticket.PnrNumber}, Status={ticket.Status}, TransactionId={ticket.TransactionId?.ToString() ?? "NULL"}");
                    }
                }

                return tickets ?? new List<BookedTicket>();
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {jsonEx.StackTrace}");
                return new List<BookedTicket>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting booked tickets: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new List<BookedTicket>();
            }
        }

        // Helper methods for data parsing
        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date;

            return DateTime.MinValue;
        }

        private string FormatDate(string dateString)
        {
            DateTime date = ParseDate(dateString);
            return date != DateTime.MinValue ? date.ToString("dd MMM, yyyy") : "N/A";
        }

        private string FormatTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return "N/A";

            try
            {
                if (TimeSpan.TryParse(timeString, out TimeSpan time))
                {
                    DateTime dt = DateTime.Today.Add(time);
                    return dt.ToString("hh:mm tt");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting time: {ex.Message}");
            }

            return timeString;
        }

        private int ParsePostponeCount(string postponeCount)
        {
            if (string.IsNullOrEmpty(postponeCount))
                return 0;

            int count;
            return int.TryParse(postponeCount, out count) ? count : 0;
        }

        private string GetSeatsDisplay(List<Passenger> passengers)
        {
            if (passengers == null || passengers.Count == 0)
                return "--";

            try
            {
                var activePassengers = passengers.Where(p => p.IsActive).ToList();
                if (activePassengers.Count == 0)
                    return "--";

                var seatNumbers = activePassengers.Select(p => p.SeatNumber).ToList();
                return string.Join(", ", seatNumbers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting seats display: {ex.Message}");
                return "N/A";
            }
        }

        private string ExtractLocationName(string sourceDestination, bool isPickup)
        {
            if (string.IsNullOrEmpty(sourceDestination))
                return "N/A";

            try
            {
                var parts = sourceDestination.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                    return isPickup ? parts[0].Trim() : parts[1].Trim();

                return sourceDestination;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting location: {ex.Message}");
                return sourceDestination;
            }
        }

        // Methods for rendering UI elements
        protected string GetActionButton(object pnrNumber, object dateOfJourney, object pickup,
            object drop, object subTotal, object status)
        {
            try
            {
                var statusText = status?.ToString() ?? "Unknown";

                var jsonData = new
                {
                    dateOfJourney = dateOfJourney?.ToString() ?? "N/A",
                    pnrNumber = pnrNumber?.ToString() ?? "N/A",
                    pickup = pickup?.ToString() ?? "N/A",
                    drop = drop?.ToString() ?? "N/A",
                    subTotal = Convert.ToDecimal(subTotal),
                    statusText = statusText
                };

                string jsonString = JsonConvert.SerializeObject(jsonData);
                string escapedJson = Server.HtmlEncode(jsonString);

                return $@"<button type='button' class='btn btn-sm btn-info checkinfo' 
                          data-info='{escapedJson}' 
                          data-bs-toggle='modal' 
                          data-bs-target='#infoModal'>
                          <i class='las la-info-circle'></i>
                       </button>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating action button: {ex.Message}");
                return "<span class='text-muted'>-</span>";
            }
        }

        protected string GetStatusBadge(object statusObj)
        {
            try
            {
                string status = statusObj?.ToString() ?? "Unknown";

                switch (status.ToLower())
                {
                    case "booked":
                        return "<span class='badge badge--success'>Booked</span>";
                    case "pending":
                        return "<span class='badge badge--warning'>Pending</span>";
                    case "cancelled":
                        return "<span class='badge badge--primary'>Cancelled</span>";
                    case "postponed":
                        return "<span class='badge badge--dark'>Postponed</span>";
                    case "rejected":
                        return "<span class='badge badge--danger'>Rejected</span>";
                    default:
                        return $"<span class='badge badge--secondary'>{status}</span>";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStatusBadge: {ex.Message}");
                return "<span class='badge badge--secondary'>Unknown</span>";
            }
        }

        protected string GetPostponedBadge(int count)
        {
            if (count > 0)
            {
                return $"<span class='badge badge--warning'>Yes ({count}x)</span>";
            }
            return "<span class='badge badge--secondary'>No</span>";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/UserLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"notify('{type}', '{message}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid(), script, true);
        }
    }

    // ViewModel for binding to repeater
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string PnrNumber { get; set; }
        public DateTime DateOfJourney { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public decimal SubTotal { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public decimal BagsCount { get; set; }
        public decimal BagsCharge { get; set; }
        public int PostponeCount { get; set; }
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string PickupTime { get; set; }
        public string SeatsDisplay { get; set; }
        public string IsAC { get; set; }
        public string IsActive { get; set; }
        public string TripTitle { get; set; }
         public DateTime CreatedAt { get; set; }
    }

    // Model class for API response
    public class BookedTicket
    {
        [JsonProperty("bookedId")]
        public int BookedId { get; set; }

        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("tripTitle")]
        public string TripTitle { get; set; }

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
        public string DateOfJourney { get; set; }

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

        [JsonProperty("isactive")]
        public string IsActive { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }  // MADE NULLABLE - Critical fix!

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("passengers")]
        public List<Passenger> Passengers { get; set; }
    }

    public class Passenger
    {
        [JsonProperty("passengerId")]
        public int PassengerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("seatNumber")]
        public string SeatNumber { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    // Trip Detail Model
    public class TripDetail
    {
        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("startFrom")]
        public string StartFrom { get; set; }

        [JsonProperty("endTo")]
        public string EndTo { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }
        
        [JsonProperty("vehicleRouteId")]
        public int VehicleRouteId { get; set; } 
        
      
    }
    

}
