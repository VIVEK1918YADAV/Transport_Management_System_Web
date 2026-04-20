using ExcelBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class AllTickets : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string apiBaseUrl = ConfigurationSettings.AppSettings["api_path"];

        // FIX: Default PageSize changed from 10 to 25 to match the dropdown's Selected="True" on value 25
        private int PageSize
        {
            get
            {
                if (ViewState["PageSize"] != null)
                    return (int)ViewState["PageSize"];
                return 25; // matches dropdown default
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

        // Public properties for serial number calculation in ASPX
        public int CurrentPageNumber
        {
            get { return CurrentPage; }
        }

        public int PageSizeValue
        {
            get { return PageSize; }
        }

        private List<TrainTicketViewModel> AllTicketsData
        {
            get
            {
                if (ViewState["AllTicketsData"] != null)
                    return JsonConvert.DeserializeObject<List<TrainTicketViewModel>>(ViewState["AllTicketsData"].ToString());
                return new List<TrainTicketViewModel>();
            }
            set { ViewState["AllTicketsData"] = JsonConvert.SerializeObject(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllTickets()));
            }
        }

        private async Task LoadAllTickets()
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetAllTrainTicketBookings";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        List<TrainTicketApiModel> allBookings = JsonConvert.DeserializeObject<List<TrainTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            var ticketList = allBookings.Select(b => new TrainTicketViewModel
                            {
                                BookingId = b.BookingId,
                                UserId = b.UserId.ToString(),
                                UserName = b.UserName ?? "N/A",
                                FirstName = "",
                                LastName = "",
                                PnrNumber = b.PnrNumber ?? "N/A",

                                // FIX: JourneyDate stays as DateTime; formatted string is separate
                                JourneyDate = b.JourneyDate,
                                DateOfJourneyFormatted = b.JourneyDate.ToString("dd MMM, yyyy"),

                                // FIX: BookingDate parsed from CreatedAt string; formatted string is separate
                                BookingDate = ParseDateTime(b.CreatedAt),
                                BookingDateFormatted = FormatDateTime(b.CreatedAt),

                                TripId = b.TripId ?? 0,
                                TripRoute = b.SourceDestination ?? "N/A",
                                FleetTypeName = b.FleetTypeName ?? "N/A",
                                TrainName = b.TrainName ?? "N/A",
                                PickupName = ExtractLocationName(b.SourceDestination, true),
                                DropName = ExtractLocationName(b.SourceDestination, false),
                                PickupTime = "N/A",
                                SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                SubTotal = b.TotalAmount ?? 0,
                                BagsCount = b.BagsCount ?? 0,
                                BagsWeight = b.BagsWeight ?? "0",
                                BagsCharge = b.BagsCharge ?? 0,
                                Checkin = b.CheckinStatus ?? "no",
                                TicketCount = b.TicketCount ?? 0,
                                PostponeCount = ParsePostponeCount(b.PostponeCount),
                                BookingStatus = b.BookingStatus ?? "Unknown",
                                CreatedAt = b.CreatedAt ?? "",
                                TransactionStatus = b.TransactionStatus ?? "N/A"
                            })
                            .OrderByDescending(t => t.BookingDate)
                            .ToList();

                            AllTicketsData = ticketList;
                            CurrentPage = 1;
                            BindPagedData();
                            UpdateStatistics(ticketList);

                            if (divNoData != null) divNoData.Visible = false;
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
                    ShowMessage("Failed to load tickets.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in LoadAllTickets: {ex.Message}");
                SetNoDataState();
                ShowMessage($"Error: {ex.Message}", "error");
            }
        }

        private void UpdateStatistics(List<TrainTicketViewModel> ticketList)
        {
            var bookedCount = ticketList.Count(t => t.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase));
            var pendingCount = ticketList.Count(t => t.BookingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase));
            var rejectedCount = ticketList.Count(t => t.BookingStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase));
            var cancelledCount = ticketList.Count(t => t.BookingStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));
            var postponedCount = ticketList.Count(t => t.BookingStatus.Equals("Postponed", StringComparison.OrdinalIgnoreCase));

            if (litTotalTickets != null) litTotalTickets.Text = ticketList.Count.ToString();
            if (litTotalBooked != null) litTotalBooked.Text = bookedCount.ToString();
            //if (litTotalPending != null) litTotalPending.Text = pendingCount.ToString();
            //if (litTotalRejected != null) litTotalRejected.Text = rejectedCount.ToString();
            if (litTotalCancelled != null) litTotalCancelled.Text = cancelledCount.ToString();
            if (litTotalPostponed != null) litTotalPostponed.Text = postponedCount.ToString();
        }

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

                if (CurrentPage < 1) CurrentPage = 1;
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                int startIndex = (CurrentPage - 1) * PageSize;
                int endIndex = Math.Min(startIndex + PageSize, totalRecords);

                var pagedTickets = allTickets
                    .Skip(startIndex)
                    .Take(PageSize)
                    .ToList();

                rptAllTickets.DataSource = pagedTickets;
                rptAllTickets.DataBind();

                UpdatePaginationControls(startIndex + 1, endIndex, totalRecords);

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

                if (litPageStart != null) litPageStart.Text = start.ToString();
                if (litPageEnd != null) litPageEnd.Text = end.ToString();
                if (litTotalRecords != null) litTotalRecords.Text = total.ToString();
                if (litCurrentPage != null) litCurrentPage.Text = CurrentPage.ToString();
                if (litTotalPages != null) litTotalPages.Text = totalPages.ToString();

                if (btnFirst != null) btnFirst.Enabled = (CurrentPage > 1);
                if (btnPrevious != null) btnPrevious.Enabled = (CurrentPage > 1);
                if (btnNext != null) btnNext.Enabled = (CurrentPage < totalPages);
                if (btnLast != null) btnLast.Enabled = (CurrentPage < totalPages);

                if (ddlPageSize != null &&
                    ddlPageSize.Items != null &&
                    ddlPageSize.Items.FindByValue(PageSize.ToString()) != null)
                {
                    ddlPageSize.SelectedValue = PageSize.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdatePaginationControls Error: {ex.Message}");
            }
        }

        // Pagination event handlers
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
            if (rptAllTickets != null)
            {
                rptAllTickets.DataSource = new List<TrainTicketViewModel>();
                rptAllTickets.DataBind();
            }

            if (litTotalTickets != null) litTotalTickets.Text = "0";
            if (litTotalBooked != null) litTotalBooked.Text = "0";
            //if (litTotalPending != null) litTotalPending.Text = "0";
            //if (litTotalRejected != null) litTotalRejected.Text = "0";
            if (litTotalCancelled != null) litTotalCancelled.Text = "0";
            if (litTotalPostponed != null) litTotalPostponed.Text = "0";

            if (divNoData != null)
                divNoData.Visible = true;

            UpdatePaginationControls(0, 0, 0);
        }

        // FIX: Parameter type changed from List<PassengerModel> to List<TrainPassenger>
        private string GetSeatsDisplay(List<TrainPassenger> passengers)
        {
            if (passengers == null || passengers.Count == 0)
                return "N/A";

            var seatNumbers = passengers
                .Where(p => !string.IsNullOrEmpty(p.SeatNumber))
                .Select(p => p.SeatNumber)
                .ToList();

            return seatNumbers.Count > 0 ? string.Join(", ", seatNumbers) : "N/A";
        }

        private DateTime ParseDateTime(string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
                return dateTime;

            return DateTime.MinValue;
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
        protected string GetStatusBadge(object statusObj)
        {
            string status = statusObj?.ToString() ?? "";

            if (string.IsNullOrEmpty(status))
                return "<span class='badge badge--secondary'><i class='las la-question-circle'></i> Unknown</span>";

            switch (status.ToLower())
            {
                case "booked":
                    return "<span class='badge badge--success'><i class='las la-check-circle'></i> Booked</span>";
                case "pending":
                    return "<span class='badge badge--warning'><i class='las la-clock'></i> Pending</span>";
                case "cancelled":
                    return "<span class='badge badge--primary'><i class='las la-ban'></i> Cancelled</span>";
                case "postponed":
                    return "<span class='badge badge--dark'><i class='las la-pause-circle'></i> Postponed</span>";
                case "rejected":
                    return "<span class='badge badge--danger'><i class='las la-times-circle'></i> Rejected</span>";
                default:
                    return $"<span class='badge badge--secondary'><i class='las la-question-circle'></i> {status}</span>";
            }
        }

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

        protected string GetUserDisplayName(string userName, string firstName, string lastName)
        {
            if (!string.IsNullOrEmpty(userName))
                return userName;

            if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                return $"{firstName} {lastName}".Trim();

            return "N/A";
        }
        private string GetTempSeatsDisplay(List<TempTrainPassengers> tempPassengers)
        {
            if (tempPassengers == null || tempPassengers.Count == 0)
                return "N/A";

            var seatNumbers = tempPassengers
                .Where(p => !string.IsNullOrEmpty(p.Seatnumber))
                .Select(p => p.Seatnumber)
                .ToList();

            return seatNumbers.Count > 0 ? string.Join(", ", seatNumbers) : "N/A";
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

        // Search and Filter functionality
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => ApplyFilters()));
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchPnr.Text = string.Empty;
            txtJourneyDate.Text = string.Empty;
            txtBookingDate.Text = string.Empty;
            if (ddlStatusFilter != null)
                ddlStatusFilter.SelectedValue = "all";

            RegisterAsyncTask(new PageAsyncTask(() => LoadAllTickets()));
        }

        private async Task ApplyFilters()
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetAllTrainTicketBookings_tempdata";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        var allBookings = JsonConvert.DeserializeObject<List<TrainTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            string searchPnr = txtSearchPnr.Text.Trim();
                            string statusFilter = ddlStatusFilter.SelectedValue;
                            DateTime? journeyDate = ParseFilterDate(txtJourneyDate.Text);
                            DateTime? bookingDate = ParseFilterDate(txtBookingDate.Text);

                            var filteredTickets = allBookings.AsEnumerable();

                            // Filter by Status
                            if (statusFilter != "all")
                            {
                                string statusString = "";
                                switch (statusFilter)
                                {
                                    case "1": statusString = "Booked"; break;
                                    case "2": statusString = "Pending"; break;
                                    case "3": statusString = "Cancelled"; break;
                                    case "4": statusString = "Postponed"; break;
                                    case "5": statusString = "Rejected"; break;
                                }

                                if (!string.IsNullOrEmpty(statusString))
                                {
                                    filteredTickets = filteredTickets.Where(b =>
                                        !string.IsNullOrEmpty(b.BookingStatus) &&
                                        b.BookingStatus.Equals(statusString, StringComparison.OrdinalIgnoreCase));
                                }
                            }

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
                                    b.JourneyDate.Date == journeyDate.Value.Date);
                            }

                            // Filter by Booking Date
                            if (bookingDate.HasValue)
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                {
                                    var bDate = ParseDateTime(b.CreatedAt);
                                    return bDate != DateTime.MinValue && bDate.Date == bookingDate.Value.Date;
                                });
                            }

                            var finalTickets = filteredTickets.ToList();

                            if (finalTickets.Count > 0)
                            {
                                var ticketList = finalTickets.Select(b => new TrainTicketViewModel
                                {
                                    BookingId = b.BookingId,
                                    UserId = b.UserId.ToString(),
                                    UserName = b.UserName ?? "N/A",
                                    FirstName = "",
                                    LastName = "",
                                    PnrNumber = b.PnrNumber ?? "N/A",

                                    // FIX: consistent formatting same as LoadAllTickets
                                    JourneyDate = b.JourneyDate,
                                    DateOfJourneyFormatted = b.JourneyDate.ToString("dd MMM, yyyy"),
                                    BookingDate = ParseDateTime(b.CreatedAt),
                                    BookingDateFormatted = FormatDateTime(b.CreatedAt),

                                    TripId = b.TripId ?? 0,
                                    TripRoute = b.SourceDestination ?? "N/A",
                                    FleetTypeName = b.FleetTypeName ?? "N/A",
                                    TrainName = b.TrainName ?? "N/A",
                                    PickupName = ExtractLocationName(b.SourceDestination, true),
                                    DropName = ExtractLocationName(b.SourceDestination, false),
                                    PickupTime = "N/A",

                                    //                            SeatsDisplay = (b.BookingStatus ?? "").Equals("Cancelled", StringComparison.OrdinalIgnoreCase)
                                    //? GetTempSeatsDisplay(b.TempPassengers)
                                    //: GetSeatsDisplay(b.Passengers),
                                    //                            SubTotal = b.TotalAmount ?? 0,
                                    //                            BagsCount = (b.BookingStatus ?? "").Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && b.TempPassengers != null && b.TempPassengers.Any()
                                    //? (b.TempPassengers.FirstOrDefault()?.BagsCount ?? b.BagsCount ?? 0)
                                    //: b.BagsCount ?? 0,
                                    //                            BagsWeight = (b.BookingStatus ?? "").Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && b.TempPassengers != null && b.TempPassengers.Any()
                                    //? (b.TempPassengers.FirstOrDefault()?.BagsWeight ?? b.BagsWeight ?? "0")
                                    //: b.BagsWeight ?? "0",
                                    //                            BagsCharge = (b.BookingStatus ?? "").Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && b.TempPassengers != null && b.TempPassengers.Any()
                                    //? (b.TempPassengers.FirstOrDefault()?.BagsCharge ?? b.BagsCharge ?? 0)
                                    //: b.BagsCharge ?? 0,
                                    //                            Checkin = (b.BookingStatus ?? "").Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && b.TempPassengers != null && b.TempPassengers.Any()
                                    //? (b.TempPassengers.FirstOrDefault()?.Checkin ?? b.CheckinStatus ?? "no")
                                    //: b.CheckinStatus ?? "no",




                                    SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                    SubTotal = b.TotalAmount ?? 0,
                                    BagsCount = b.BagsCount ?? 0,
                                    BagsWeight = b.BagsWeight ?? "0",
                                    BagsCharge = b.BagsCharge ?? 0,
                                    Checkin = b.CheckinStatus ?? "no",
                                    TicketCount = b.TicketCount ?? 0,
                                    PostponeCount = ParsePostponeCount(b.PostponeCount),
                                    BookingStatus = b.BookingStatus ?? "Unknown",
                                    CreatedAt = b.CreatedAt ?? "",
                                    TransactionStatus = b.TransactionStatus ?? "N/A"
                                })
                                .OrderByDescending(t => t.BookingDate)
                                .ToList();

                                AllTicketsData = ticketList;
                                CurrentPage = 1;
                                BindPagedData();
                                UpdateStatistics(ticketList);

                                if (divNoData != null)
                                    divNoData.Visible = false;

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

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => ApplyFilters()));
        }
    }

    // ViewModel
    public class TrainTicketViewModel
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PnrNumber { get; set; }

        // FIX: JourneyDate is DateTime; DateOfJourneyFormatted is the display string
        public DateTime JourneyDate { get; set; }
        public string DateOfJourneyFormatted { get; set; }

        // FIX: BookingDate is DateTime; BookingDateFormatted is the display string
        public DateTime BookingDate { get; set; }
        public string BookingDateFormatted { get; set; }

        public int TripId { get; set; }
        public string TripRoute { get; set; }
        public string FleetTypeName { get; set; }

        // FIX: TrainName kept; VehicleNickName removed (not in API model)
        public string TrainName { get; set; }
        public string SourceDestination { get; set; }
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
        public string BookingStatus { get; set; }
        public string CreatedAt { get; set; }
        public string TransactionStatus { get; set; }
    }

    // API Model
    public class TrainTicketApiModel
    {
        [JsonProperty("bookingId")]
        public int BookingId { get; set; }

        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("tripId")]
        public int? TripId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

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

        [JsonProperty("totalAmount")]
        public decimal? TotalAmount { get; set; }

        [JsonProperty("journeyDate")]
        public DateTime JourneyDate { get; set; }

        [JsonProperty("checkinStatus")]
        public string CheckinStatus { get; set; }

        [JsonProperty("bagsCount")]
        public int? BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal? BagsCharge { get; set; }

        [JsonProperty("bookingStatus")]
        public string BookingStatus { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        // FIX: Changed from List<PassengerModel> to List<TrainPassenger> — matching the actual entity class
        [JsonProperty("passengers")]
        public List<TrainPassenger> Passengers { get; set; }  
        
        [JsonProperty("tempPassengers")]
        public List<TempTrainPassengers> TempPassengers { get; set; }
    }
    public class TempTrainPassengers
    {
        [JsonProperty("passengerid")]
        public int Passengerid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("seatnumber")]
        public string Seatnumber { get; set; }

        [JsonProperty("bagsCount")]
        public int BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal BagsCharge { get; set; }

        [JsonProperty("checkin")]
        public string Checkin { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}