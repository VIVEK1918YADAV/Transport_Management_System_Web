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
    public partial class Train_BookedTicket : System.Web.UI.Page
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

        private List<TrainBookedTicketViewModel> AllTicketsData
        {
            get
            {
                if (ViewState["AllTicketsData"] != null)
                    return JsonConvert.DeserializeObject<List<TrainBookedTicketViewModel>>(ViewState["AllTicketsData"].ToString());
                return new List<TrainBookedTicketViewModel>();
            }
            set { ViewState["AllTicketsData"] = JsonConvert.SerializeObject(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Diagnostics.Debug.WriteLine("=== BookedTickets Page Load ===");
                RegisterAsyncTask(new PageAsyncTask(() => LoadBookedTickets()));
            }
        }

        private async Task LoadBookedTickets()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadBookedTickets started");

                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetAllTrainTicketBookings";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        var allBookings = JsonConvert.DeserializeObject<List<TrainBookedTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            var bookedTickets = allBookings
                                .Where(b => !string.IsNullOrEmpty(b.BookingStatus) &&
                                           b.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            if (bookedTickets.Count > 0)
                            {
                                var ticketList = bookedTickets.Select(b => new TrainBookedTicketViewModel
                                {
                                    BookingId = b.BookingId,
                                    UserId = b.UserId ?? "N/A",
                                    UserName = b.UserName ?? "N/A",
                                    FirstName = "",
                                    LastName = "",
                                    PnrNumber = b.PnrNumber ?? "N/A",
                                    TrainNumber = b.TrainNumber,
                                    JourneyDate = ParseDate(b.JourneyDate),
                                    DateOfJourneyFormatted = FormatDate(b.JourneyDate),
                                    BookingDate = ParseDate(b.CreatedAt),
                                    BookingDateFormatted = FormatDateTime(b.CreatedAt),
                                    TripId = b.TripId ?? 0,
                                    //TrainRoute = b.TrainRoute ?? "N/A",
                                    FleetTypeName = b.FleetTypeName ?? "N/A", // ✅ Get fleet type from API
                                    TrainName = b.TrainName ?? "N/A", // ✅ Get vehicle nickname from API
                                   RouteName = ExtractLocationName(b.SourceDestination, true),
                                    //DropName = ExtractLocationName(b.SourceDestination, false),
                                    PickupTime = "N/A",
                                    SeatsDisplay = GetSeatsDisplay(b.TrainPassengers),
                                    TotalAmount = b.TotalAmount ?? 0,
                                    BagsCount = b.BagsCount ?? 0,
                                    BagsWeight = b.BagsWeight ?? "0",
                                    BagsCharge = b.BagsCharge ?? 0,
                                    Checkin = b.Checkin ?? "no",
                                    TicketCount = b.TicketCount ?? 0,
                                    PostponeCount = ParsePostponeCount(b.PostponeCount),
                                    CreatedAt = b.CreatedAt ?? "",
                                    TransactionStatus = b.TransactionStatus ?? "N/A"
                                })
                                .OrderByDescending(t => t.BookingDate)
                                .ToList();

                                // ✅ Store all tickets for pagination
                                AllTicketsData = ticketList;

                                // ✅ Bind first page
                                CurrentPage = 1;
                                BindPagedData();

                                if (litTotalBooked != null)
                                {
                                    litTotalBooked.Text = ticketList.Count.ToString();
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
                    ShowMessage("Failed to load booked tickets.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                SetNoDataState();
                ShowMessage("Error loading tickets. Please try again.", "error");
            }
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
                rptBookedTickets.DataSource = pagedTickets;
                rptBookedTickets.DataBind();

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
            if (rptBookedTickets != null)
            {
                rptBookedTickets.DataSource = new List<TrainBookedTicketViewModel>();
                rptBookedTickets.DataBind();
            }

            if (litTotalBooked != null)
            {
                litTotalBooked.Text = "0";
            }

            if (divNoData != null)
            {
                divNoData.Visible = true;
            }

            UpdatePaginationControls(0, 0, 0);
        }

        private string GetSeatsDisplay(List<TrainPassengerModel> passengers)
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
            RegisterAsyncTask(new PageAsyncTask(() => LoadBookedTickets()));
        }

        private async Task ApplyFilters()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ApplyFilters started");

                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetAllTrainTicketBookings";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse != "[]")
                    {
                        var allBookings = JsonConvert.DeserializeObject<List<TrainBookedTicketApiModel>>(jsonResponse);

                        if (allBookings != null && allBookings.Count > 0)
                        {
                            // Filter by status first
                            var bookedTickets = allBookings
                                .Where(b => !string.IsNullOrEmpty(b.BookingStatus) &&
                                           b.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            // Get filter values
                            string searchPnr = txtSearchPnr.Text.Trim();
                            DateTime? journeyDate = ParseFilterDate(txtJourneyDate.Text);
                            DateTime? bookingDate = ParseFilterDate(txtBookingDate.Text);

                            // Apply filters
                            var filteredTickets = bookedTickets.AsEnumerable();

                            // Filter by PNR
                            if (!string.IsNullOrEmpty(searchPnr))
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                    !string.IsNullOrEmpty(b.PnrNumber) &&
                                    b.PnrNumber.IndexOf(searchPnr, StringComparison.OrdinalIgnoreCase) >= 0);
                            }

                            // Filter by Journey Date (exact match)
                            if (journeyDate.HasValue)
                            {
                                filteredTickets = filteredTickets.Where(b =>
                                {
                                    var jDate = ParseDate(b.JourneyDate);
                                    return jDate.Date == journeyDate.Value.Date;
                                });
                            }

                            // Filter by Booking Date (matches any booking on that day)
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
                                var ticketList = finalTickets.Select(b => new TrainBookedTicketViewModel
                                {
                                    BookingId = b.BookingId,
                                    UserId = b.UserId ?? "N/A",
                                    UserName = b.UserName ?? "N/A",
                                    FirstName = "",
                                    LastName = "",
                                    PnrNumber = b.PnrNumber ?? "N/A",
                                    JourneyDate = ParseDate(b.JourneyDate),
                                    DateOfJourneyFormatted = FormatDate(b.JourneyDate),
                                    BookingDate = ParseDateTime(b.CreatedAt),
                                    BookingDateFormatted = FormatDateTime(b.CreatedAt),
                                    TripId = b.TripId ?? 0,
                                    //RouteName = b.RouteName ?? "N/A",
                                    FleetTypeName = b.FleetTypeName ?? "N/A", // ✅ Get fleet type from API
                                    TrainName = b.TrainName ?? "N/A", // ✅ Get vehicle nickname from API
                                    PickupName = ExtractLocationName(b.SourceDestination, true),
                                    DropName = ExtractLocationName(b.SourceDestination, false),
                                    PickupTime = "N/A",
                                    SeatsDisplay = GetSeatsDisplay(b.TrainPassengers),
                                    TotalAmount = b.TotalAmount ?? 0,
                                    BagsCount = b.BagsCount ?? 0,
                                    BagsWeight = b.BagsWeight ?? "0",
                                    BagsCharge = b.BagsCharge ?? 0,
                                    Checkin = b.Checkin ?? "no",
                                    TicketCount = b.TicketCount ?? 0,
                                    PostponeCount = ParsePostponeCount(b.PostponeCount),
                                    CreatedAt = b.CreatedAt ?? "",
                                    TransactionStatus = b.TransactionStatus ?? "N/A"
                                })
                                .OrderByDescending(t => t.BookingDate)
                                .ToList();

                                // Store and bind filtered data
                                AllTicketsData = ticketList;
                                CurrentPage = 1;
                                BindPagedData();

                                if (litTotalBooked != null)
                                {
                                    litTotalBooked.Text = ticketList.Count.ToString();
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
    public class TrainBookedTicketViewModel
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PnrNumber { get; set; }
        public string CoachNo { get; set; }
        public DateTime? JourneyDate { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDateFormatted { get; set; }
        public int TripId { get; set; }
        public string RouteName { get; set; }
        public string FleetTypeName { get; set; } 
        public string TrainName { get; set; } 
        public string TrainNumber { get; set; } 
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string PickupTime { get; set; }
        public string SeatsDisplay { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public int BagsCount { get; set; }
        public string BagsWeight { get; set; }
        public decimal BagsCharge { get; set; }
        public string Checkin { get; set; }
        public int TicketCount { get; set; }
        public int PostponeCount { get; set; }
        public string CreatedAt { get; set; }
        public string TransactionStatus { get; set; }
    }

    // API Model
    public class TrainBookedTicketApiModel
    {
        [JsonProperty("bookingId")]
        public int BookingId { get; set; }

        [JsonProperty("coachNo")]
        public string CoachNo { get; set; }
        
        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("tripId")]
        public int? TripId { get; set; }
        
        [JsonProperty("userId")]
        public string UserId { get; set; } 
        
        [JsonProperty("fromStationId")]
        public int? FromStationId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("toStationId")]
        public int? ToStationId { get; set; }
        
        [JsonProperty("coachTypeId")]
        public int? CoachTypeId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; } 
        
        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; } 

        [JsonProperty("fleetTypeId")]
        public int? FleetTypeId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; } 

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
        public string JourneyDate { get; set; }

        [JsonProperty("checkin")]
        public string Checkin { get; set; }

        [JsonProperty("bagsCount")]
        public int? BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal? BagsCharge { get; set; }

        [JsonProperty("bookingStatus")]
        public string BookingStatus { get; set; }

        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("passengers")]
        public List<TrainPassengerModel> TrainPassengers { get; set; }
    }

    // Passenger Model
    public class TrainPassengerModel
    {
        [JsonProperty("passengerId")]
        public int PassengerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("seatNumber")]
        public string SeatNumber { get; set; }

        [JsonProperty("bagsCount")]
        public int BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal BagsCharge { get; set; }

        [JsonProperty("checkin")]
        public string Checkin { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}