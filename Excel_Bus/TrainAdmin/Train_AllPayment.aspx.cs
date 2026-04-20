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
    public partial class Train_AllPayment : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_AllPayment()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                //Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

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

        private List<TrainPaymentsViewModel> AllPaymentsData
        {
            get
            {
                if (ViewState["AllPaymentsData"] != null)
                    return JsonConvert.DeserializeObject<List<TrainPaymentsViewModel>>(ViewState["AllPaymentsData"].ToString());
                return new List<TrainPaymentsViewModel>();
            }
            set { ViewState["AllPaymentsData"] = JsonConvert.SerializeObject(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllPayments()));
            }
        }
        private async Task LoadAllPayments(string searchTerm = "", string dateRange = "")
        {
            try
            {
                pnlError.Visible = false;

                // Fetch transactions using the new endpoint
                HttpResponseMessage transactionResponse = await client.GetAsync("TrainTransactions/GetTrainTransactions");

                if (!transactionResponse.IsSuccessStatusCode)
                {
                    ShowError($"Failed to load payments. Status Code: {transactionResponse.StatusCode}");
                    return;
                }

                string transactionJson = await transactionResponse.Content.ReadAsStringAsync();
                var transactions = JsonConvert.DeserializeObject<List<TrainPaymentsTransactionDto>>(transactionJson);

                // Fetch all users
                HttpResponseMessage userResponse = await client.GetAsync("TrainUsers/GetActiveTrainUsers");

                if (!userResponse.IsSuccessStatusCode)
                {
                    ShowError($"Failed to load user data. Status Code: {userResponse.StatusCode}");
                    return;
                }

                string userJson = await userResponse.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<TrainPaymentsUserDto>>(userJson);

                // Create dictionary for quick user lookup
                var userDict = users.ToDictionary(u => u.UserId, u => u);

                // Join transactions with users - get ALL transactions (no filtering)
                var allPayments = transactions
                    .Select(t => {
                        var user = userDict.ContainsKey(t.UserId) ? userDict[t.UserId] : null;
                        return new TrainPaymentsViewModel
                        {
                            Id = t.TrxId,
                            UserId = t.UserId.ToString(),
                            Amount = t.Amount,
                            ExtraCharge = t.ExtraCharge,
                            PostponeAmt1 = t.PostponeAmt1,
                            PostponeAmt2 = t.PostponeAmt2,
                            TrxTypeStatus = t.TrxTypeStatus,
                            TrxNo = t.TrxNo,
                            PaymentRemarks = t.PaymentRemarks,
                            PaymentStatus = t.PaymentStatus,
                            CreatedAt = t.CreatedAt,
                            UpdatedAt = t.UpdatedAt,
                            PnrNumber = t.PnrNumber,
                            BookingId = t.BookingId,
                            Status = t.PaymentStatus,
                            UserName = user?.UserName ?? "Unknown",
                            Firstname = user?.Firstname ?? "",
                            Lastname = user?.Lastname ?? "",
                            FullName = user != null ? $"{user.Firstname ?? ""} {user.Lastname ?? ""}".Trim() : "Unknown",
                            TotalAmount = t.Amount + t.ExtraCharge,
                            StatusText = t.TrxTypeStatus ?? "Unknown"
                        };
                    })
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allPayments = allPayments.Where(t =>
                        (!string.IsNullOrEmpty(t.TrxNo) && t.TrxNo.ToLower().Contains(searchTerm.ToLower())) ||
                        (!string.IsNullOrEmpty(t.UserName) && t.UserName.ToLower().Contains(searchTerm.ToLower()))
                    ).ToList();
                }

                // Apply date range filter if provided
                if (!string.IsNullOrEmpty(dateRange))
                {
                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    if (dateRange.Contains(" to "))
                    {
                        string[] dates = dateRange.Split(new[] { " to " }, StringSplitOptions.None);
                        if (dates.Length == 2)
                        {
                            if (DateTime.TryParse(dates[0].Trim(), out DateTime parsedStart))
                            {
                                startDate = parsedStart;
                            }
                            if (DateTime.TryParse(dates[1].Trim(), out DateTime parsedEnd))
                            {
                                endDate = parsedEnd.AddDays(1).AddSeconds(-1);
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(dateRange.Trim(), out DateTime singleDate))
                        {
                            startDate = singleDate;
                            endDate = singleDate.AddDays(1).AddSeconds(-1);
                        }
                    }

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        allPayments = allPayments.Where(t =>
                            t.CreatedAt >= startDate.Value && t.CreatedAt <= endDate.Value
                        ).ToList();
                    }
                }

                // Calculate summary totals
                decimal successfulTotal = allPayments
                    .Where(t => t.Status != null && t.Status.Equals("Successful", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.TotalAmount);

                decimal pendingTotal = allPayments
                    .Where(t => t.Status != null && t.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.TotalAmount);

                decimal rejectedTotal = allPayments
                    .Where(t => t.Status != null && t.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.TotalAmount);

                decimal initiatedTotal = allPayments
                    .Where(t => t.Status != null &&
                        (t.Status.Equals("Initiated", StringComparison.OrdinalIgnoreCase) ||
                         t.Status.Equals("Postponed", StringComparison.OrdinalIgnoreCase)))
                    .Sum(t => t.TotalAmount);

                lblSuccessfulAmount.Text = successfulTotal.ToString("N2");
                lblPendingAmount.Text = pendingTotal.ToString("N2");
                lblRejectedAmount.Text = rejectedTotal.ToString("N2");
                lblInitiatedAmount.Text = initiatedTotal.ToString("N2");

                // ✅ Store all payments for pagination
                AllPaymentsData = allPayments;

                // ✅ Bind first page
                CurrentPage = 1;
                BindPagedData();

                // Show/hide no data panel
                pnlNoData.Visible = allPayments.Count == 0;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading payments: {ex.Message}");
            }
        }

        // ✅ Pagination methods
        private void BindPagedData()
        {
            try
            {
                var allPayments = AllPaymentsData;

                if (allPayments == null || allPayments.Count == 0)
                {
                    rptPayments.DataSource = new List<AllPaymentsViewModel>();
                    rptPayments.DataBind();
                    UpdatePaginationControls(0, 0, 0);
                    pnlNoData.Visible = true;
                    return;
                }

                int totalRecords = allPayments.Count;
                int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                if (CurrentPage < 1) CurrentPage = 1;
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                int startIndex = (CurrentPage - 1) * PageSize;
                int endIndex = Math.Min(startIndex + PageSize, totalRecords);

                var pagedPayments = allPayments
                    .Skip(startIndex)
                    .Take(PageSize)
                    .ToList();

                rptPayments.DataSource = pagedPayments;
                rptPayments.DataBind();

                UpdatePaginationControls(startIndex + 1, endIndex, totalRecords);
                pnlNoData.Visible = false;
            }
            catch (Exception ex)
            {
                ShowError($"Error displaying paged data: {ex.Message}");
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
            int totalPages = (int)Math.Ceiling((double)AllPaymentsData.Count / PageSize);
            if (CurrentPage < totalPages)
            {
                CurrentPage++;
                BindPagedData();
            }
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)AllPaymentsData.Count / PageSize);
            CurrentPage = totalPages;
            BindPagedData();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageSize = int.Parse(ddlPageSize.SelectedValue);
            CurrentPage = 1;
            BindPagedData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            string dateRange = txtDateRange.Text.Trim();

            RegisterAsyncTask(new PageAsyncTask(() => LoadAllPayments(searchTerm, dateRange)));
        }

        protected string GetStatusBadge(object statusObj, object updatedAt)
        {
            string status = statusObj?.ToString() ?? "";
            string timeAgo = "";

            if (updatedAt != null && DateTime.TryParse(updatedAt.ToString(), out DateTime dt))
            {
                timeAgo = GetRelativeTime(dt);
            }

            string badgeClass = "";
            switch (status.ToLower())
            {
                case "success":
                case "approved":
                    badgeClass = "status-approved";
                    break;
                case "pending":
                    badgeClass = "status-pending";
                    break;
                case "cancelled":
                    badgeClass = "status-cancelled";
                    break;
                case "postponed":
                case "initiated":
                    badgeClass = "status-initiated";
                    break;
                case "rejected":
                    badgeClass = "status-rejected";
                    break;
                default:
                    badgeClass = "status-initiated";
                    break;
            }

            string badgeHtml = $"<span class='status-badge {badgeClass}'>{status}";

            if (!string.IsNullOrEmpty(timeAgo))
            {
                badgeHtml += $"<span class='status-time'>{timeAgo}</span>";
            }

            badgeHtml += "</span>";

            return badgeHtml;
        }

        protected string FormatDateTime(object dateTime)
        {
            if (dateTime == null) return "";

            DateTime dt;
            if (dateTime is DateTime)
            {
                dt = (DateTime)dateTime;
            }
            else if (DateTime.TryParse(dateTime.ToString(), out dt))
            {
                // Parsed successfully
            }
            else
            {
                return dateTime.ToString();
            }

            return dt.ToString("yyyy-MM-dd hh:mm tt");
        }

        protected string GetRelativeTime(object dateTime)
        {
            if (dateTime == null) return "";

            DateTime dt;
            if (dateTime is DateTime)
            {
                dt = (DateTime)dateTime;
            }
            else if (DateTime.TryParse(dateTime.ToString(), out dt))
            {
                // Parsed successfully
            }
            else
            {
                return "";
            }

            TimeSpan diff = DateTime.Now - dt;

            if (diff.TotalMinutes < 1)
                return "just now";
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} minute{((int)diff.TotalMinutes != 1 ? "s" : "")} ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} hour{((int)diff.TotalHours != 1 ? "s" : "")} ago";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} day{((int)diff.TotalDays != 1 ? "s" : "")} ago";
            if (diff.TotalDays < 30)
            {
                int weeks = (int)(diff.TotalDays / 7);
                return $"{weeks} week{(weeks != 1 ? "s" : "")} ago";
            }
            if (diff.TotalDays < 365)
            {
                int months = (int)(diff.TotalDays / 30);
                return $"{months} month{(months != 1 ? "s" : "")} ago";
            }

            int years = (int)(diff.TotalDays / 365);
            return $"{years} year{(years != 1 ? "s" : "")} ago";
        }

        protected string FormatAmount(object amount)
        {
            if (amount == null) return "0.00";

            decimal amountValue;
            if (amount is decimal)
            {
                amountValue = (decimal)amount;
            }
            else if (decimal.TryParse(amount.ToString(), out amountValue))
            {
                // Parsed successfully
            }
            else
            {
                return "0.00";
            }

            return amountValue.ToString("N2");
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }

    // DTOs for All Payments page
    public class TrainPaymentsTransactionDto
    {
        public int TrxId { get; set; }
        public string TrxNo { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal ExtraCharge { get; set; }
        public decimal PostponeAmt1 { get; set; }
        public decimal PostponeAmt2 { get; set; }
        public string TrxTypeStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentRemarks { get; set; }
        public DateTime? TrxPaymentAt { get; set; }
        public DateTime? OldDate { get; set; }
        public DateTime? NewDate { get; set; }
        public string PnrNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TrainPaymentsUserDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class TrainPaymentsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal ExtraCharge { get; set; }
        public decimal PostponeAmt1 { get; set; }
        public decimal PostponeAmt2 { get; set; }
        public string TrxTypeStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TrxNo { get; set; }

        public string PaymentRemarks { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PnrNumber { get; set; }
        public int? BookingId { get; set; }
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string FullName { get; set; }
        public string StatusText { get; set; }
        public decimal TotalAmount { get; set; }
    }

}