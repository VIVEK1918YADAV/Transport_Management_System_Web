using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Train_BookedTicket_History : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        private string apiUrl = ConfigurationManager.AppSettings["api_path"];

        public Train_BookedTicket_History()
        {
            if (client.BaseAddress == null)
            {
                try
                {
                    string url = ConfigurationManager.AppSettings["api_path"];
                    if (!string.IsNullOrEmpty(url))
                    {
                        client.BaseAddress = new Uri(url);
                        client.Timeout = TimeSpan.FromSeconds(60);
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
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Train.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            PageAsyncTask task = new PageAsyncTask(LoadDashboardData);
            RegisterAsyncTask(task);
        }

        protected void ddlActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            string selectedAction = ddl.SelectedValue;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            string pnrNumber = ((HiddenField)item.FindControl("hfPnrNumber")).Value;

            switch (selectedAction)
            {
                case "View":
                    ViewBooking(pnrNumber);
                    break;
                case "Postpone":
                    PostponeBooking(pnrNumber);
                    break;
            }

            ddl.SelectedIndex = 0;
        }

        private async void PostponeBooking(string pnrNumber)
        {
            string endpoint = $"TrainTicketBookings/GetBookedTrainTicketByPnr?pnr={pnrNumber}";
            string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(fullUrl);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ShowAlert("No booking found for this PNR.");
                        return;
                    }
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                        {
                            ShowAlert("No booking found for this PNR.");
                            return;
                        }

                        var bookingDetails = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                        
                        if (bookingDetails != null)
                        { 
                            // Use correct API field names
                            string tripId = bookingDetails.tripId?.ToString() ?? string.Empty;
                            string dateOfJourney = bookingDetails.dateOfJourney?.ToString() ?? string.Empty;
                            string status = bookingDetails.status?.ToString() ?? string.Empty;
                            string ticketCount = bookingDetails.ticketCount?.ToString() ?? string.Empty;
                            string price = bookingDetails.subTotal?.ToString() ?? string.Empty;
                            string userID = bookingDetails.userId?.ToString() ?? string.Empty;
                            string transactionId = bookingDetails.transactionId?.ToString() ?? string.Empty;
                            string postponeCount = bookingDetails.postponeCount?.ToString() ?? "0";
                            string bookedId = bookingDetails.bookedId?.ToString() ?? string.Empty;
                            string coachNumber = bookingDetails.coachNumber?.ToString() ?? string.Empty;
                            string trainId = bookingDetails.trainId?.ToString() ?? string.Empty;
                            int coachTypeId = Convert.ToInt32(bookingDetails.passengers[0].coachtypeId);


                            if (!DateTime.TryParse(dateOfJourney, out DateTime journeyDate))
                            {
                                ShowAlert("Invalid journey date.");
                                return;
                            }

                            if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                            {
                                ShowAlert("The ticket has already been cancelled and cannot be postponed.");
                                return;
                            }

                            if (journeyDate >= DateTime.Today)
                            {
                                string gender = string.Empty;
                                if (bookingDetails.passengers != null && bookingDetails.passengers.Count > 0)
                                {
                                    var firstPassenger = bookingDetails.passengers[0];
                                    gender = firstPassenger.gender?.ToString() ?? string.Empty;
                                }

                                var seats = new List<string>();
                                if (bookingDetails.passengers != null)
                                {
                                    foreach (var passenger in bookingDetails.passengers)
                                    {
                                        if (passenger.isActive == true)
                                            seats.Add(passenger.seatNumber?.ToString() ?? string.Empty);
                                    }
                                }
                                string selectedSeat = string.Join(",", seats);

                                string redirectUrl =
                                    $"Train_Seat_Selection.aspx?trainId={trainId}&tripId={tripId}&pnrNumber={pnrNumber}" +
                                    $"&bookedId={bookedId}&selectedSeat={selectedSeat}" +
                                    $"&dateOfJourney={dateOfJourney}&gender={Uri.EscapeDataString(gender)}" +
                                    $"&status={status}&SeatPrice={price}&CoachTypeId={coachTypeId}"+ $"&CoachId={coachNumber}&userId={userID}" + $"&transactionId={transactionId}&ticketCount={ticketCount}" + $"&postponeCount={postponeCount}";

                                Response.Redirect(redirectUrl);
                            }
                            else
                            {
                                ShowAlert("The ticket has already expired and cannot be postponed.");
                            }
                        }
                        else
                        {
                            ShowAlert("Unable to parse booking details.");
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                        ShowAlert("Error: Unable to retrieve booking details. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching data: {ex.Message}");
                ShowAlert($"Error fetching data: {ex.Message}");
            }
        }

        private void ViewBooking(string pnrNumber)
        {
            Response.Redirect("Train_BookedTicket_details.aspx?pnr=" + pnrNumber);
            //Response.Redirect("Train_Booking_Confirmation.aspx?pnr=" + pnrNumber);
        }

        private void ShowAlert(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"notify('{type}', '{message}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid(), script, true);
        }

        private async Task LoadDashboardData()
        {
            try
            {
                if (Session["UserId"] == null) return;

                string userId = Session["UserId"].ToString();
                var bookings = await GetBookedTickets(userId);

                //var coachMap = Session["PNRCoachMap"] as Dictionary<string, string>
                //      ?? new Dictionary<string, string>();


                var coachMap = Application["PNRCoachMap"] as Dictionary<string, string>
    ?? new Dictionary<string, string>();
                //Session["PNRCoachMap"] = coachMap;

                if (bookings != null && bookings.Count > 0)
                {
                    var bookingList = new List<TrainBookingViewModel>();

                    foreach (var b in bookings)
                    {
                        try
                        {
                            string coachNumber = coachMap.ContainsKey(b.PnrNumber)
                                    ? coachMap[b.PnrNumber]
                                    : "N/A";
                            Session["PNRCoachMap"] = coachNumber;
                            var viewModel = new TrainBookingViewModel
                            {
                                BookingId = b.BookingId,          // ← was BookedId
                                PnrNumber = b.PnrNumber ?? "N/A",
                                TrainNumber = b.TrainNumber,
                                //TripId = b.TripId.ToString(),  // tripId used as train ref
                                CoachType = b.CoachType ?? "N/A", // ← new field
                                CoachNumber = coachNumber,
                                JourneyDate = ParseDate(b.JourneyDate),
                                DateOfJourneyFormatted = FormatDate(b.JourneyDate),
                                TotalAmount = b.TotalAmount,        // ← was SubTotal
                                BookingStatus = b.BookingStatus ?? "Unknown", // ← was Status
                                PostponeCount = ParsePostponeCount(b.PostponeCount),
                                PickupName = b.FromStation ?? "N/A", // ← was ExtractLocationName
                                DropName = b.ToStation ?? "N/A",   // ← was ExtractLocationName
                                //CoachNumber = b.CoachNumber ?? "N/A",
                                SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                LuggageCount = b.LuggageCount,       // ← was BagsCount
                                LuggageCharge = b.LuggageCharge,      // ← was BagsCharge
                                CreatedAt = b.CreatedAt ?? DateTime.MinValue
                            };

                            bookingList.Add(viewModel);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"Error processing booking {b.PnrNumber}: {ex.Message}");
                        }
                    }

                    var sortedList = bookingList.OrderByDescending(b => b.CreatedAt).ToList();

                    var pnrCoachDict = bookingList.ToDictionary(b => b.PnrNumber, b => b.CoachNumber);
                    ViewState["PNRCoachMap"] = JsonConvert.SerializeObject(pnrCoachDict);

                    rptBookings.DataSource = sortedList;
                    rptBookings.DataBind();
                }
                else
                {
                    rptBookings.DataSource = new List<TrainBookingViewModel>();
                    rptBookings.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading train booking data: {ex.Message}");
                rptBookings.DataSource = new List<TrainBookingViewModel>();
                rptBookings.DataBind();
                ShowMessage("Unable to load booking data. Please try again.", "error");
            }
        }

        private async Task<List<TrainBookedTicket>> GetBookedTickets(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(apiUrl))
                    return new List<TrainBookedTicket>();

                string endpoint = $"TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return new List<TrainBookedTicket>();

                string jsonResult = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                    return new List<TrainBookedTicket>();

                var tickets = JsonConvert.DeserializeObject<List<TrainBookedTicket>>(jsonResult);
                return tickets ?? new List<TrainBookedTicket>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting train booked tickets: {ex.Message}");
                return new List<TrainBookedTicket>();
            }
        }

        // ─── Helper Methods ──────────────────────────────────────────────────────

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return DateTime.MinValue;
            return DateTime.TryParse(dateString, out DateTime date) ? date : DateTime.MinValue;
        }

        private string FormatDate(string dateString)
        {
            DateTime date = ParseDate(dateString);
            return date != DateTime.MinValue ? date.ToString("dd MMM, yyyy") : "N/A";
        }

        private int ParsePostponeCount(string postponeCount)
        {
            if (string.IsNullOrEmpty(postponeCount)) return 0;
            return int.TryParse(postponeCount, out int count) ? count : 0;
        }

        //private string GetSeatsDisplay(List<TrainPassengerDto> passengers)
        //{
        //    if (passengers == null || passengers.Count == 0) return "N/A";
        //    var activeSeats = passengers
        //        .Where(p => p.IsActive)
        //        .Select(p => p.SeatNumber)
        //        .Where(s => !string.IsNullOrEmpty(s))
        //        .ToList();
        //    return activeSeats.Count > 0 ? string.Join(", ", activeSeats) : "N/A";
        //}
        private string GetSeatsDisplay(List<TrainPassengerDto> passengers, string coachNumber = "")
        {
            if (passengers == null || passengers.Count == 0) return "N/A";
            var activeSeats = passengers
                .Where(p => p.IsActive)
                .Select(p => p.SeatNumber)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            if (activeSeats.Count == 0) return "N/A";

            // Coach prefix agar available ho
            string prefix = !string.IsNullOrEmpty(coachNumber) ? $"C{coachNumber}-" : "";
            return string.Join(", ", activeSeats.Select(s => prefix + s));
        }
        protected string GetStatusBadge(object statusObj)
        {
            string status = statusObj?.ToString() ?? "Unknown";
            switch (status.ToLower())
            {
                case "booked": return "<span class='badge-status badge-confirmed'>Booked</span>";
                case "pending": return "<span class='badge-status badge-pending'>Pending</span>";
                case "cancelled": return "<span class='badge-status badge-cancelled'>Cancelled</span>";
                case "postponed": return "<span class='badge-status badge-postponed'>Postponed</span>";
                default: return $"<span class='badge-status'>{status}</span>";
            }
        }

        protected string GetPostponedBadge(int count)
        {
            return count > 0
                ? $"<span class='badge-status badge-pending'>Yes ({count}x)</span>"
                : "<span class='badge-status'>No</span>";
        }
    }

    // ─── DTOs matching the API response exactly ──────────────────────────────────

    public class TrainBookedTicket
    {
        [JsonProperty("bookingId")] public int BookingId { get; set; }       // ← was bookedId
        [JsonProperty("pnrNumber")] public string PnrNumber { get; set; }
        [JsonProperty("trainNumber")] public string TrainNumber { get; set; }
        [JsonProperty("tripId")] public int TripId { get; set; }
        [JsonProperty("fromStation")] public string FromStation { get; set; }  // ← new (was sourceDestination)
        [JsonProperty("toStation")] public string ToStation { get; set; }    // ← new
        [JsonProperty("coachType")] public string CoachType { get; set; }    // ← new
        [JsonProperty("journeyDate")] public string JourneyDate { get; set; }  // ← was dateOfJourney
        [JsonProperty("bookingDate")] public string BookingDate { get; set; }
        [JsonProperty("createdAt")] public DateTime? CreatedAt { get; set; }
        [JsonProperty("updatedAt")] public DateTime? UpdatedAt { get; set; }
        [JsonProperty("status")] public string BookingStatus { get; set; } // ← was status
        [JsonProperty("paymentStatus")] public string PaymentStatus { get; set; }
        [JsonProperty("paymentMethod")] public string PaymentMethod { get; set; }
        [JsonProperty("transactionId")] public int? TransactionId { get; set; }
        [JsonProperty("passengerCount")] public int PassengerCount { get; set; }
        [JsonProperty("activePassengerCount")] public int ActivePassengerCount { get; set; }
        [JsonProperty("unitPrice")] public decimal UnitPrice { get; set; }
        [JsonProperty("totalAmount")] public decimal TotalAmount { get; set; } // ← was subTotal
        [JsonProperty("luggageCount")] public int? LuggageCount { get; set; }  // ← was bagsCount
        [JsonProperty("luggageWeightKg")] public decimal? LuggageWeightKg { get; set; }
        [JsonProperty("luggageCharge")] public decimal? LuggageCharge { get; set; } // ← was bagsCharge
        [JsonProperty("postponeCount")] public string PostponeCount { get; set; }
        [JsonProperty("checkinStatus")] public string CheckinStatus { get; set; }
        [JsonProperty("qrCodePath")] public string QrCodePath { get; set; }
        [JsonProperty("passengers")] public List<TrainPassengerDto> Passengers { get; set; }
        [JsonProperty("coachNumber")] public string CoachNumber { get; set; }
    }

    public class TrainPassengerDto
    {
        [JsonProperty("passengerId")] public int PassengerId { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("seatNumber")] public string SeatNumber { get; set; }
        [JsonProperty("isActive")] public bool IsActive { get; set; }
        [JsonProperty("luggageCharge")] public decimal? LuggageCharge { get; set; }
    }

    // ─── ViewModel for the Repeater ───────────────────────────────────────────────

    public class TrainBookingViewModel
    {
        public int BookingId { get; set; }
        public string PnrNumber { get; set; }
        public string TrainNumber { get; set; }
        public string CoachType { get; set; }
        public DateTime JourneyDate { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; }
        public int PostponeCount { get; set; }
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string SeatsDisplay { get; set; }
        public int? LuggageCount { get; set; }
        public decimal? LuggageCharge { get; set; }
        public string TripTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CoachNumber { get; set; }
    }
}