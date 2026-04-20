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
    public partial class TrainUserDashboard : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        private string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        // Session key for caching the full booking list
        private const string SESSION_BOOKINGS_KEY = "DashboardBookings";

        public TrainUserDashboard()
        {
            if (client.BaseAddress == null)
            {
                try
                {
                    string url = ConfigurationSettings.AppSettings["api_path"];
                    if (!string.IsNullOrEmpty(url))
                    {
                        client.BaseAddress = new Uri(url);
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

            if (!IsPostBack)
            {
                // Fresh load: fetch from API
                PageAsyncTask task = new PageAsyncTask(LoadDashboardData);
                RegisterAsyncTask(task);
            }
            // PostBack (pagination clicks) will call BindBookings directly from their handlers
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  DATA LOADING
        // ─────────────────────────────────────────────────────────────────────────

        private async Task LoadDashboardData()
        {
            try
            {
                string userId = Session["UserId"].ToString();
                var bookings = await GetTrainBookedTickets(userId);

                // Reset stat counters
                litTotalBooked.Text = "0";
                litTotalCancelled.Text = "0";
                litTotalPostponed.Text = "0";

                if (bookings != null && bookings.Count > 0)
                {
                    // ── Stats ──────────────────────────────────────────────────────
                    int totalBooked = bookings.Count(b => b.Status != null &&
                        (b.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase) ||
                         b.Status.Equals("Postponed", StringComparison.OrdinalIgnoreCase)));

                    int totalCancelled = bookings.Count(b => b.Status != null &&
                        b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));

                    int totalPostponed = bookings.Count(b =>
                    {
                        if (string.IsNullOrEmpty(b.PostponeCount)) return false;
                        return int.TryParse(b.PostponeCount, out int cnt) && cnt > 0;
                    });

                    litTotalBooked.Text = totalBooked.ToString();
                    litTotalCancelled.Text = totalCancelled.ToString();
                    litTotalPostponed.Text = totalPostponed.ToString();

                    // ── Build view-model list ──────────────────────────────────────
                    var bookingList = new List<TrainBookingViewModel>();

                    foreach (var b in bookings)
                    {
                        try
                        {
                            bookingList.Add(new TrainBookingViewModel
                            {
                                BookingId = b.BookedId,
                                PnrNumber = b.PnrNumber ?? "N/A",
                                JourneyDate = ParseDate(b.JourneyDate),
                                DateOfJourneyFormatted = FormatDate(b.JourneyDate),
                                SubTotal = b.SubTotal,
                                TotalAmount = b.SubTotal,
                                BookingStatus = b.Status ?? "Unknown",
                                PickupName = b.FromStation ?? "N/A",
                                DropName = b.ToStation ?? "N/A",
                                PickupTime = "N/A",
                                SeatsDisplay = GetSeatsDisplay(b.TrainPassengers),
                                LuggagesCount = b.LuggagesCount,
                                LuggagesCharge = b.LuggagesCharge,
                                TripTitle = b.TripTitle ?? "",
                                PostponeCount = b.PostponeCount,
                                CreatedAt = ParseDate(b.CreatedAt) == DateTime.MinValue
                                                           ? (DateTime?)null
                                                           : ParseDate(b.CreatedAt)
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing booking {b.PnrNumber}: {ex.Message}");
                        }
                    }

                    var sortedList = bookingList
                        .OrderByDescending(b => b.CreatedAt)
                        .ToList();

                    // ── Cache in Session for pagination postbacks ──────────────────
                    Session[SESSION_BOOKINGS_KEY] = sortedList;

                    // Reset to page 1 on fresh load
                    CurrentPage = 1;

                    BindBookings(sortedList);
                }
                else
                {
                    Session[SESSION_BOOKINGS_KEY] = new List<TrainBookingViewModel>();
                    BindBookings(new List<TrainBookingViewModel>());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");

                litTotalBooked.Text = "0";
                litTotalCancelled.Text = "0";
                litTotalPostponed.Text = "0";

                Session[SESSION_BOOKINGS_KEY] = new List<TrainBookingViewModel>();
                BindBookings(new List<TrainBookingViewModel>());

                ShowMessage("Unable to load dashboard data. Please try again.", "error");
            }
        }

        private async Task<List<TrainBookedTicket>> GetTrainBookedTickets(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(apiUrl))
                    return new List<TrainBookedTicket>();

                string endpoint = $"TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    return new List<TrainBookedTicket>();
                }

                string jsonResult = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                    return new List<TrainBookedTicket>();

                return JsonConvert.DeserializeObject<List<TrainBookedTicket>>(jsonResult)
                       ?? new List<TrainBookedTicket>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting booked tickets: {ex.Message}");
                return new List<TrainBookedTicket>();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  PAGINATION
        // ─────────────────────────────────────────────────────────────────────────

        private const int PageSize = 10;

        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        /// <summary>
        /// Retrieves the cached booking list from Session.
        /// Safe to call from PostBack handlers.
        /// </summary>
        private List<TrainBookingViewModel> GetBookingsFromSession()
        {
            return Session[SESSION_BOOKINGS_KEY] as List<TrainBookingViewModel>
                   ?? new List<TrainBookingViewModel>();
        }

        private void BindBookings(List<TrainBookingViewModel> bookings)
        {
            if (bookings == null || bookings.Count == 0)
            {
                rptBookings.Visible = false;
                noDataMessage.Visible = true;
                UpdatePaginationControls(0, 0);
                return;
            }

            int totalPages = (int)Math.Ceiling((double)bookings.Count / PageSize);

            // Clamp current page
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > totalPages) CurrentPage = totalPages;

            var pageItems = bookings
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            rptBookings.DataSource = pageItems;
            rptBookings.DataBind();

            noDataMessage.Visible = false;
            rptBookings.Visible = true;

            UpdatePaginationControls(totalPages, bookings.Count);
        }

        private void UpdatePaginationControls(int totalPages, int totalRecords)
        {
            if (totalRecords == 0)
            {
                lblRowCount.Text = "0 records";
                pnlPagination.Visible = false;
                return;
            }

            int start = (CurrentPage - 1) * PageSize + 1;
            int end = Math.Min(CurrentPage * PageSize, totalRecords);
            lblRowCount.Text = $"{start}–{end} of {totalRecords} records";

            btnPrev.Enabled = CurrentPage > 1;
            btnNext.Enabled = CurrentPage < totalPages;
            pnlPagination.Visible = totalPages > 1;

            // Rebuild numbered page buttons
            phPageNumbers.Controls.Clear();

            for (int i = 1; i <= totalPages; i++)
            {
                bool showPage = i == 1
                             || i == totalPages
                             || Math.Abs(i - CurrentPage) <= 2;

                if (showPage)
                {
                    var lb = new LinkButton
                    {
                        Text = i.ToString(),
                        CommandName = "GoToPage",
                        CommandArgument = i.ToString(),
                        CssClass = i == CurrentPage ? "pg-num pg-num--active" : "pg-num"
                    };
                    lb.Click += PageNumber_Click;
                    phPageNumbers.Controls.Add(lb);
                }
                else if (Math.Abs(i - CurrentPage) == 3)
                {
                    phPageNumbers.Controls.Add(new Literal
                    {
                        Text = "<span class=\"pg-ellipsis\">…</span>"
                    });
                }
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1) CurrentPage--;
            BindBookings(GetBookingsFromSession());
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            BindBookings(GetBookingsFromSession());
        }

        protected void PageNumber_Click(object sender, EventArgs e)
        {
            var lb = (LinkButton)sender;
            CurrentPage = int.Parse(lb.CommandArgument);
            BindBookings(GetBookingsFromSession());
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  HELPER / UTILITY METHODS
        // ─────────────────────────────────────────────────────────────────────────

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

        private string GetSeatsDisplay(List<TrainPassenger> passengers)
        {
            if (passengers == null || passengers.Count == 0) return "--";
            try
            {
                var active = passengers.Where(p => p.IsActive).Select(p => p.SeatNumber).ToList();
                return active.Count > 0 ? string.Join(", ", active) : "--";
            }
            catch { return "N/A"; }
        }

        protected string FormatSeatsHtml(string seatsDisplay)
        {
            if (string.IsNullOrEmpty(seatsDisplay) || seatsDisplay == "--")
                return "<span style='color:#9ba3b5;'>--</span>";

            var sb = new System.Text.StringBuilder();
            foreach (var seat in seatsDisplay.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                sb.AppendFormat("<span class='eb-seat-chip'>{0}</span>", seat.Trim());
            return sb.ToString();
        }

        protected string GetStatusBadge(object statusObj)
        {
            string status = statusObj?.ToString() ?? "Unknown";
            switch (status.ToLower())
            {
                case "booked": return "<span class='badge badge--success'>Booked</span>";
                case "pending": return "<span class='badge badge--warning'>Pending</span>";
                case "cancelled": return "<span class='badge badge--primary'>Cancelled</span>";
                case "postponed": return "<span class='badge badge--dark'>Postponed</span>";
                case "rejected": return "<span class='badge badge--danger'>Rejected</span>";
                default: return $"<span class='badge badge--secondary'>{status}</span>";
            }
        }

        protected string GetPostponedBadge(int count)
        {
            return count > 0
                ? $"<span class='badge badge--warning'>Yes ({count}x)</span>"
                : "<span class='badge badge--secondary'>No</span>";
        }

        protected string GetActionButton(object pnrNumber, object dateOfJourney, object pickup,
            object drop, object subTotal, object status)
        {
            try
            {
                var jsonData = new
                {
                    dateOfJourney = dateOfJourney?.ToString() ?? "N/A",
                    pnrNumber = pnrNumber?.ToString() ?? "N/A",
                    pickup = pickup?.ToString() ?? "N/A",
                    drop = drop?.ToString() ?? "N/A",
                    subTotal = Convert.ToDecimal(subTotal),
                    statusText = status?.ToString() ?? "Unknown"
                };

                string escapedJson = Server.HtmlEncode(JsonConvert.SerializeObject(jsonData));

                return $@"<button type='button' class='btn btn-sm btn-info checkinfo' 
                              data-info='{escapedJson}' 
                              data-bs-toggle='modal' 
                              data-bs-target='#infoModal'>
                              <i class='las la-info-circle'></i>
                           </button>";
            }
            catch { return "<span class='text-muted'>-</span>"; }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/TrainUSerReg.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            ClientScript.RegisterStartupScript(
                GetType(), "Msg_" + Guid.NewGuid(),
                $"notify('{type}', '{message}');", true);
        }

        private void AddNoTripsMessage()
        {
            // Placeholder – tripsMenu removed from UI
        }
    }

    // ── View-model ────────────────────────────────────────────────────────────────
    public class TrainBookingViewModel
    {
        public int BookingId { get; set; }
        public string PnrNumber { get; set; }
        public DateTime JourneyDate { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string CheckinStatus { get; set; }
        public int? LuggagesCount { get; set; }
        public decimal? LuggagesWeightKg { get; set; }
        public decimal? LuggagesCharge { get; set; }
        public string PostponeCount { get; set; }
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string PickupTime { get; set; }
        public int? TicketCount { get; set; }
        public string SeatsDisplay { get; set; }
        public string CoachType { get; set; }
        public bool? IsActive { get; set; }
        public string TripTitle { get; set; }
        public string QrCode { get; set; }
        public int? CancellationCount { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? PaymentDeadline { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal SubTotal { get; set; }
    }

    // ── API response models ───────────────────────────────────────────────────────
    public class TrainBookedTicket
    {
        [JsonProperty("bookingId")] public int BookedId { get; set; }
        [JsonProperty("pnrNumber")] public string PnrNumber { get; set; }
        [JsonProperty("tripId")] public int TripId { get; set; }
        [JsonProperty("trainName")] public string TripTitle { get; set; }
        [JsonProperty("fromStation")] public string FromStation { get; set; }
        [JsonProperty("toStation")] public string ToStation { get; set; }
        [JsonProperty("ticketCount")] public int TicketCount { get; set; }
        [JsonProperty("postponeCount")] public string PostponeCount { get; set; }
        [JsonProperty("unitPrice")] public decimal UnitPrice { get; set; }
        [JsonProperty("totalAmount")] public decimal SubTotal { get; set; }
        [JsonProperty("journeyDate")] public string JourneyDate { get; set; }
        [JsonProperty("checkinStatus")] public string Checkin { get; set; }
        [JsonProperty("luggageCount")] public int LuggagesCount { get; set; }
        [JsonProperty("luggageWeightKg")] public string LuggagesWeight { get; set; }
        [JsonProperty("luggageCharge")] public decimal LuggagesCharge { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("paymentStatus")] public string IsActive { get; set; }
        [JsonProperty("transactionId")] public int? TransactionId { get; set; }
        [JsonProperty("paymentMethod")] public string TransactionStatus { get; set; }
        [JsonProperty("createdAt")] public string CreatedAt { get; set; }
        [JsonProperty("updatedAt")] public string UpdatedAt { get; set; }
        [JsonProperty("passengers")] public List<TrainPassenger> TrainPassengers { get; set; }
    }

    public class TrainPassenger
    {
        [JsonProperty("passengerId")] public int TrainPassengerId { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("seatNumber")] public string SeatNumber { get; set; }
        [JsonProperty("isActive")] public bool IsActive { get; set; }
    }

    public class TrainTripDetail
    {
        [JsonProperty("tripId")] public int TripId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("status")] public int Status { get; set; }
        [JsonProperty("startFrom")] public string StartFrom { get; set; }
        [JsonProperty("endTo")] public string EndTo { get; set; }
        [JsonProperty("price")] public decimal? Price { get; set; }
        [JsonProperty("vehicleRouteId")] public int VehicleRouteId { get; set; }
    }
}