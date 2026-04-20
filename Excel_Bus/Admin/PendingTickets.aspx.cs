using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace Excel_Bus
{
    public partial class PendingTickets : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string apiBaseUrl = ConfigurationSettings.AppSettings["api_path"];

        // ✅ Pagination properties
        private int PageSize
        {
            get
            {
                if (ViewState["PageSize"] != null)
                    return (int)ViewState["PageSize"];
                return 10; // default
            }
            set { ViewState["PageSize"] = value; }
        }

        private int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] != null)
                    return (int)ViewState["CurrentPage"];
                return 1;
            }
            set { ViewState["CurrentPage"] = value; }
        }

        // ✅ Public properties for serial number calculation in ASPX
        public int CurrentPageNumber
        {
            get { return CurrentPage; }
        }

        public int PageSizeValue
        {
            get { return PageSize; }
        }

        private List<PendingTicketViewModel> AllTicketsData
        {
            get
            {
                if (ViewState["AllTicketsData"] != null)
                    return JsonConvert.DeserializeObject<List<PendingTicketViewModel>>(ViewState["AllTicketsData"].ToString());
                return new List<PendingTicketViewModel>();
            }
            set { ViewState["AllTicketsData"] = JsonConvert.SerializeObject(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Diagnostics.Debug.WriteLine("=== PendingTickets Page Load ===");
                RegisterAsyncTask(new PageAsyncTask(() => LoadPendingTickets()));
            }
        }

        private async Task LoadPendingTickets()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadPendingTickets started");

                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/GetBookedTickets";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        var allBookings = JsonConvert.DeserializeObject<List<PendingTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            var pendingTickets = allBookings
                                .Where(b => !string.IsNullOrEmpty(b.Status) &&
                                           b.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            if (pendingTickets.Count > 0)
                            {
                                var ticketList = pendingTickets.Select(b => new PendingTicketViewModel
                                {
                                    Id = b.BookedId,
                                    UserId = b.UserId ?? 0,
                                    UserName = b.UserName ?? "N/A",
                                    FirstName = "",
                                    LastName = "",
                                    PnrNumber = b.PnrNumber ?? "N/A",
                                    DateOfJourney = ParseDate(b.DateOfJourney),
                                    DateOfJourneyFormatted = FormatDate(b.DateOfJourney),
                                    BookingDate = ParseDateTime(b.CreatedAt),
                                    BookingDateFormatted = FormatDateTime(b.CreatedAt),
                                    TripId = b.TripId ?? 0,
                                    TripRoute = b.TripTitle ?? "N/A",
                                    FleetTypeName = b.FleetTypeName ?? "N/A", // ✅ Get fleet type from API
                                    VehicleNickName = b.VehicleNickName ?? "N/A", // ✅ Get vehicle nickname from API
                                    PickupName = ExtractLocationName(b.SourceDestination, true),
                                    DropName = ExtractLocationName(b.SourceDestination, false),
                                    PickupTime = "N/A",
                                    SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                    SubTotal = b.SubTotal ?? 0,
                                    BagsCount = b.BagsCount ?? 0,
                                    BagsWeight = b.BagsWeight ?? "0",
                                    BagsCharge = b.BagsCharge ?? 0,
                                    Checkin = b.Checkin ?? "no",
                                    TicketCount = b.TicketCount ?? 0,
                                    PostponeCount = ParsePostponeCount(b.PostponeCount),
                                    Status = b.Status ?? "N/A",
                                    TransactionId = b.TransactionId ?? 0,
                                    TrxAmount = b.SubTotal ?? 0,
                                    TransactionStatus = b.TransactionStatus ?? "N/A"
                                })
                                .OrderByDescending(t => t.BookingDate)
                                .ToList();

                                // ✅ Store all tickets for pagination
                                AllTicketsData = ticketList;

                                // ✅ Bind first page
                                CurrentPage = 1;
                                BindPagedData();

                                if (litTotalPending != null)
                                {
                                    litTotalPending.Text = ticketList.Count.ToString();
                                }

                                if (divNoData != null)
                                {
                                    divNoData.Visible = false;
                                }
                            }
                            else
                            {
                                SetNoDataState();
                            }
                        }
                        else
                        {
                            SetNoDataState();
                        }
                    }
                    else
                    {
                        SetNoDataState();
                    }
                }
                else
                {
                    SetNoDataState();
                    ShowMessage("Failed to load pending tickets.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                SetNoDataState();
                ShowMessage("Error loading pending tickets. Please try again.", "error");
            }
        }

        // ✅ Pagination methods
        private void BindPagedData()
        {
            try
            {
                var allTickets = AllTicketsData;

                if (allTickets == null || allTickets.Count == 0)
                {
                    SetNoDataState();
                    UpdatePaginationControls(0, 0, 0);
                    return;
                }

                int totalRecords = allTickets.Count;
                int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                // Ensure CurrentPage is valid
                if (CurrentPage < 1) CurrentPage = 1;
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                // Calculate start and end indices
                int startIndex = (CurrentPage - 1) * PageSize;
                int endIndex = Math.Min(startIndex + PageSize, totalRecords);

                // Get paged data
                var pagedTickets = allTickets
                    .Skip(startIndex)
                    .Take(PageSize)
                    .ToList();

                // Bind to repeater
                rptPendingTickets.DataSource = pagedTickets;
                rptPendingTickets.DataBind();

                // Update pagination info
                UpdatePaginationControls(startIndex + 1, endIndex, totalRecords);

                // Hide/show no data message
                if (divNoData != null)
                    divNoData.Visible = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BindPagedData Error: {ex.Message}");
                ShowMessage("Error displaying paged data", "error");
            }
        }

        private void UpdatePaginationControls(int start, int end, int total)
        {
            try
            {
                int totalPages = total > 0 ? (int)Math.Ceiling((double)total / PageSize) : 1;

                // Update literals - with null checks
                if (litPageStart != null) litPageStart.Text = start.ToString();
                if (litPageEnd != null) litPageEnd.Text = end.ToString();
                if (litTotalRecords != null) litTotalRecords.Text = total.ToString();
                if (litCurrentPage != null) litCurrentPage.Text = CurrentPage.ToString();
                if (litTotalPages != null) litTotalPages.Text = totalPages.ToString();

                // Enable/disable buttons - with null checks
                if (btnFirst != null) btnFirst.Enabled = (CurrentPage > 1);
                if (btnPrevious != null) btnPrevious.Enabled = (CurrentPage > 1);
                if (btnNext != null) btnNext.Enabled = (CurrentPage < totalPages);
                if (btnLast != null) btnLast.Enabled = (CurrentPage < totalPages);

                // Set page size dropdown - with null checks
                if (ddlPageSize != null && ddlPageSize.Items != null && ddlPageSize.Items.FindByValue(PageSize.ToString()) != null)
                {
                    ddlPageSize.SelectedValue = PageSize.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdatePaginationControls Error: {ex.Message}");
            }
        }

        // ✅ Pagination event handlers
        protected void btnFirst_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindPagedData();
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                BindPagedData();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)AllTicketsData.Count / PageSize);
            if (CurrentPage < totalPages)
            {
                CurrentPage++;
                BindPagedData();
            }
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)AllTicketsData.Count / PageSize);
            CurrentPage = totalPages;
            BindPagedData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageSize = int.Parse(ddlPageSize.SelectedValue);
            CurrentPage = 1;
            BindPagedData();
        }

        private void SetNoDataState()
        {
            if (rptPendingTickets != null)
            {
                rptPendingTickets.DataSource = new List<PendingTicketViewModel>();
                rptPendingTickets.DataBind();
            }

            if (litTotalPending != null)
            {
                litTotalPending.Text = "0";
            }

            if (divNoData != null)
            {
                divNoData.Visible = true;
            }

            UpdatePaginationControls(0, 0, 0);
        }

        private string GetSeatsDisplay(List<PassengerModel> passengers)
        {
            if (passengers == null || passengers.Count == 0)
                return "N/A";

            var seatNumbers = passengers
                .Where(p => !string.IsNullOrEmpty(p.SeatNumber))
                .Select(p => p.SeatNumber)
                .ToList();

            return seatNumbers.Count > 0 ? string.Join(", ", seatNumbers) : "N/A";
        }

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date;

            return DateTime.MinValue;
        }

        private DateTime ParseDateTime(string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
                return dateTime;

            return DateTime.MinValue;
        }

        private string FormatDate(string dateString)
        {
            DateTime date = ParseDate(dateString);
            return date != DateTime.MinValue ? date.ToString("dd MMM, yyyy") : "N/A";
        }

        private string FormatDateTime(string dateTimeString)
        {
            DateTime dateTime = ParseDateTime(dateTimeString);
            return dateTime != DateTime.MinValue ? dateTime.ToString("dd MMM, yyyy hh:mm tt") : "N/A";
        }

        private int ParsePostponeCount(string postponeCount)
        {
            if (string.IsNullOrEmpty(postponeCount))
                return 0;

            return int.TryParse(postponeCount, out int count) ? count : 0;
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
            catch
            {
                return sourceDestination;
            }
        }

        // UI Helper Methods
        protected string GetYesNoBadge(string value)
        {
            if (string.IsNullOrEmpty(value) || value.ToLower() == "no")
                return "<span class='badge badge--secondary'>No</span>";

            return "<span class='badge badge--success'>Yes</span>";
        }

        protected string GetPostponedBadge(int count)
        {
            if (count > 0)
                return $"<span class='badge badge--warning'>Yes ({count}x)</span>";

            return "<span class='badge badge--secondary'>No</span>";
        }

        protected string GetStatusBadge(object statusObj)
        {
            string status = statusObj?.ToString() ?? "";

            if (string.IsNullOrEmpty(status))
                return "<span class='badge badge--secondary'>Unknown</span>";

            switch (status.ToLower())
            {
                case "booked":
                    return "<span class='badge badge--success'>Booked</span>";
                case "pending":
                    return "<span class='badge badge--warning'>Pending</span>";
                case "cancelled":
                    return "<span class='badge badge--danger'>Cancelled</span>";
                case "postponed":
                    return "<span class='badge badge--info'>Postponed</span>";
                case "rejected":
                    return "<span class='badge badge--danger'>Rejected</span>";
                default:
                    return $"<span class='badge badge--secondary'>{status}</span>";
            }
        }

        protected string GetUserDisplayName(string userName, string firstName, string lastName)
        {
            if (!string.IsNullOrEmpty(userName))
                return userName;

            if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                return $"{firstName} {lastName}".Trim();

            return "N/A";
        }

        private void ShowMessage(string message, string type)
        {
            try
            {
                message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
                string script = $"notify('{type}', '{message}');";
                ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid(), script, true);
            }
            catch { }
        }

        // ✅ Search and Filter functionality
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => ApplyFilters()));
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Clear all filter inputs
            txtSearchPnr.Text = string.Empty;
            txtJourneyDate.Text = string.Empty;
            txtBookingDate.Text = string.Empty;

            // Reload all tickets
            RegisterAsyncTask(new PageAsyncTask(() => LoadPendingTickets()));
        }

        private async Task ApplyFilters()
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/GetBookedTickets";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        var allBookings = JsonConvert.DeserializeObject<List<PendingTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            // Filter by status first (Pending only)
                            var pendingTickets = allBookings
                                .Where(b => !string.IsNullOrEmpty(b.Status) &&
                                           b.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            // Get filter values
                            string searchPnr = txtSearchPnr.Text.Trim();
                            DateTime? journeyDate = ParseFilterDate(txtJourneyDate.Text);
                            DateTime? bookingDate = ParseFilterDate(txtBookingDate.Text);

                            // Apply filters
                            var filteredTickets = pendingTickets.AsEnumerable();

                            // Filter by PNR
                            if (!string.IsNullOrEmpty(searchPnr))
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                    !string.IsNullOrEmpty(b.PnrNumber) &&
                                    b.PnrNumber.IndexOf(searchPnr, StringComparison.OrdinalIgnoreCase) >= 0);
                            }

                            // Filter by Journey Date
                            if (journeyDate.HasValue)
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                {
                                    var jDate = ParseDate(b.DateOfJourney);
                                    return jDate.Date == journeyDate.Value.Date;
                                });
                            }

                            // Filter by Booking Date
                            if (bookingDate.HasValue)
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                {
                                    var bDate = ParseDateTime(b.CreatedAt);
                                    return bDate.Date == bookingDate.Value.Date;
                                });
                            }

                            var finalTickets = filteredTickets.ToList();

                            if (finalTickets.Count > 0)
                            {
                                var ticketList = finalTickets.Select(b => new PendingTicketViewModel
                                {
                                    Id = b.BookedId,
                                    UserId = b.UserId ?? 0,
                                    UserName = b.UserName ?? "N/A",
                                    FirstName = "",
                                    LastName = "",
                                    PnrNumber = b.PnrNumber ?? "N/A",
                                    DateOfJourney = ParseDate(b.DateOfJourney),
                                    DateOfJourneyFormatted = FormatDate(b.DateOfJourney),
                                    BookingDate = ParseDateTime(b.CreatedAt),
                                    BookingDateFormatted = FormatDateTime(b.CreatedAt),
                                    TripId = b.TripId ?? 0,
                                    TripRoute = b.TripTitle ?? "N/A",
                                    FleetTypeName = b.FleetTypeName ?? "N/A", // ✅ Get fleet type from API
                                    VehicleNickName = b.VehicleNickName ?? "N/A", // ✅ Get vehicle nickname from API
                                    PickupName = ExtractLocationName(b.SourceDestination, true),
                                    DropName = ExtractLocationName(b.SourceDestination, false),
                                    PickupTime = "N/A",
                                    SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                    SubTotal = b.SubTotal ?? 0,
                                    BagsCount = b.BagsCount ?? 0,
                                    BagsWeight = b.BagsWeight ?? "0",
                                    BagsCharge = b.BagsCharge ?? 0,
                                    Checkin = b.Checkin ?? "no",
                                    TicketCount = b.TicketCount ?? 0,
                                    PostponeCount = ParsePostponeCount(b.PostponeCount),
                                    Status = b.Status ?? "N/A",
                                    TransactionId = b.TransactionId ?? 0,
                                    TrxAmount = b.SubTotal ?? 0,
                                    TransactionStatus = b.TransactionStatus ?? "N/A"
                                })
                                .OrderByDescending(t => t.BookingDate)
                                .ToList();

                                // Store and bind filtered data
                                AllTicketsData = ticketList;
                                CurrentPage = 1;
                                BindPagedData();

                                if (litTotalPending != null)
                                {
                                    litTotalPending.Text = ticketList.Count.ToString();
                                }

                                if (divNoData != null)
                                {
                                    divNoData.Visible = false;
                                }

                                ShowMessage($"Found {ticketList.Count} ticket(s)", "success");
                            }
                            else
                            {
                                SetNoDataState();
                                ShowMessage("No tickets found matching your filters", "info");
                            }
                        }
                        else
                        {
                            SetNoDataState();
                        }
                    }
                    else
                    {
                        SetNoDataState();
                    }
                }
                else
                {
                    SetNoDataState();
                    ShowMessage("Failed to search tickets.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Filter Error: {ex.Message}");
                ShowMessage("Error applying filters.", "error");
            }
        }

        private DateTime? ParseFilterDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date;

            return null;
        }
    }

    // View Model
    public class PendingTicketViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PnrNumber { get; set; }
        public DateTime DateOfJourney { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDateFormatted { get; set; }
        public int TripId { get; set; }
        public string TripRoute { get; set; }
        public string FleetTypeName { get; set; } // ✅ Added fleet type property
        public string VehicleNickName { get; set; } // ✅ Added vehicle nickname property
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string PickupTime { get; set; }
        public string SeatsDisplay { get; set; }
        public decimal SubTotal { get; set; }
        public int BagsCount { get; set; }
        public string BagsWeight { get; set; }
        public decimal BagsCharge { get; set; }
        public string Checkin { get; set; }
        public int TicketCount { get; set; }
        public int PostponeCount { get; set; }
        public string Status { get; set; }
        public int TransactionId { get; set; }
        public decimal TrxAmount { get; set; }
        public string TransactionStatus { get; set; }
    }

    // API Model
    public class PendingTicketApiModel
    {
        [JsonProperty("bookedId")]
        public int BookedId { get; set; }

        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("tripId")]
        public int? TripId { get; set; }

        [JsonProperty("tripTitle")]
        public string TripTitle { get; set; }

        [JsonProperty("vehicleNickName")]
        public string VehicleNickName { get; set; } // ✅ Added vehicle nickname property

        [JsonProperty("fleetTypeId")]
        public int? FleetTypeId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; } // ✅ Added fleet type property

        [JsonProperty("sourceDestination")]
        public string SourceDestination { get; set; }

        [JsonProperty("pickupPoint")]
        public int? PickupPoint { get; set; }

        [JsonProperty("droppingPoint")]
        public int? DroppingPoint { get; set; }

        [JsonProperty("ticketCount")]
        public int? TicketCount { get; set; }

        [JsonProperty("postponeCount")]
        public string PostponeCount { get; set; }

        [JsonProperty("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonProperty("subTotal")]
        public decimal? SubTotal { get; set; }

        [JsonProperty("dateOfJourney")]
        public string DateOfJourney { get; set; }

        [JsonProperty("checkin")]
        public string Checkin { get; set; }

        [JsonProperty("bagsCount")]
        public int? BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal? BagsCharge { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("passengers")]
        public List<PassengerModel> Passengers { get; set; }
    }
}