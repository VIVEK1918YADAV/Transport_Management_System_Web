using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class PendingPayments : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static PendingPayments()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                Timeout = TimeSpan.FromSeconds(30)
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

        private List<PendingTransactionViewModel> AllPaymentsData
        {
            get
            {
                if (ViewState["AllPaymentsData"] != null)
                    return JsonConvert.DeserializeObject<List<PendingTransactionViewModel>>(ViewState["AllPaymentsData"].ToString());
                return new List<PendingTransactionViewModel>();
            }
            set { ViewState["AllPaymentsData"] = JsonConvert.SerializeObject(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadPendingPayments()));
            }
            else
            {
                // Handle async postback for approval/rejection
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];

                if (!string.IsNullOrEmpty(eventTarget) && eventTarget == "UpdateStatus")
                {
                    string[] args = eventArgument.Split('|');
                    if (args.Length == 2)
                    {
                        int transactionId = int.Parse(args[0]);
                        string status = args[1];
                        RegisterAsyncTask(new PageAsyncTask(() => UpdateTransactionStatus(transactionId, status)));
                    }
                }
            }
        }

        private async Task LoadPendingPayments(string searchTerm = "", string dateRange = "")
        {
            try
            {
                pnlError.Visible = false;

                // Fetch transactions
                HttpResponseMessage transactionResponse = await client.GetAsync("TransactionsNew/GetTransactions");

                if (!transactionResponse.IsSuccessStatusCode)
                {
                    ShowError($"Failed to load payments. Status Code: {transactionResponse.StatusCode}");
                    return;
                }

                string transactionJson = await transactionResponse.Content.ReadAsStringAsync();
                List<PendingTransactionDto> transactions = JsonConvert.DeserializeObject<List<PendingTransactionDto>>(transactionJson);

                // Fetch all users
                HttpResponseMessage userResponse = await client.GetAsync("Users/GetUsers");

                if (!userResponse.IsSuccessStatusCode)
                {
                    ShowError($"Failed to load user data. Status Code: {userResponse.StatusCode}");
                    return;
                }

                string userJson = await userResponse.Content.ReadAsStringAsync();
                List<PendingUserDto> users = JsonConvert.DeserializeObject<List<PendingUserDto>>(userJson);

                // Create dictionary for quick user lookup
                var userDict = users.ToDictionary(u => u.Id, u => u);

                // Filter only pending payments
                var pendingPayments = transactions
                    .Where(t => t.Status != null && t.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    .Select(t => new PendingTransactionViewModel
                    {
                        Id = t.TrxId,
                        UserId = t.UserId,
                        Amount = t.Amount,
                        Charge = t.Charge,
                        PostBalance = t.PostBalance,
                        TrxType = t.TrxType,
                        Trx = t.TrxNumber,
                        Details = t.Details,
                        Remark = t.Remark,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        PnrNumber = t.PnrNumber,
                        BookedId = t.BookedId,
                        Username = userDict.ContainsKey(t.UserId) ? userDict[t.UserId].Username ?? "Unknown" : "Unknown",
                        Firstname = userDict.ContainsKey(t.UserId) ? userDict[t.UserId].Firstname ?? "" : "",
                        Lastname = userDict.ContainsKey(t.UserId) ? userDict[t.UserId].Lastname ?? "" : ""
                    })
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    pendingPayments = pendingPayments.Where(t =>
                        (!string.IsNullOrEmpty(t.Trx) && t.Trx.ToLower().Contains(searchTerm.ToLower())) ||
                        (!string.IsNullOrEmpty(t.Username) && t.Username.ToLower().Contains(searchTerm.ToLower()))
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
                        pendingPayments = pendingPayments.Where(t =>
                            t.CreatedAt >= startDate.Value && t.CreatedAt <= endDate.Value
                        ).ToList();
                    }
                }

                // ✅ Store all payments for pagination
                AllPaymentsData = pendingPayments;

                // ✅ Bind first page
                CurrentPage = 1;
                BindPagedData();

                // Show/hide no data panel
                pnlNoData.Visible = pendingPayments.Count == 0;
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
                    rptPayments.DataSource = new List<PendingTransactionViewModel>();
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

            RegisterAsyncTask(new PageAsyncTask(() => LoadPendingPayments(searchTerm, dateRange)));
        }

        private async Task UpdateTransactionStatus(int transactionId, string status)
        {
            try
            {
                var content = new StringContent($"\"{status}\"", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    $"Transactions/UpdateTransactionStatus?id={transactionId}",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var result = JsonConvert.DeserializeObject<PendingUpdateStatusResponse>(responseJson);
                        ShowSuccess($"Transaction {result.TransactionId} has been {status} successfully.");
                    }
                    catch
                    {
                        ShowSuccess($"Transaction status has been updated to {status} successfully.");
                    }

                    // Reload the page data
                    await LoadPendingPayments();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update transaction status. Status Code: {response.StatusCode}. Details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating transaction status: {ex.Message}");
            }
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

        protected string GetStatusBadgeClass(object status)
        {
            if (status == null) return "status-unknown";

            string statusStr = status.ToString();
            switch (statusStr.ToLower())
            {
                case "pending":
                    return "status-pending";
                case "success":
                case "booked":
                    return "status-booked";
                case "rejected":
                    return "status-rejected";
                case "cancelled":
                    return "status-cancelled";
                case "postponed":
                    return "status-postponed";
                default:
                    return "status-unknown";
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
        }
    }

    // DTOs for Pending Payments page
    public class PendingTransactionDto
    {
        public int TrxId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public decimal PostBalance { get; set; }
        public decimal PostBalance2 { get; set; }
        public string TrxType { get; set; }
        public string TrxNumber { get; set; }
        public string Details { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime? OldDate { get; set; }
        public DateTime? NewDate { get; set; }
        public int? BookedId { get; set; }
        public string PnrNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PendingUserDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class PendingUpdateStatusResponse
    {
        public string Message { get; set; }
        public int TransactionId { get; set; }
        public string NewStatus { get; set; }
    }

    public class PendingTransactionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public decimal PostBalance { get; set; }
        public string TrxType { get; set; }
        public string Trx { get; set; }
        public string Details { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PnrNumber { get; set; }
        public int? BookedId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string FullName => $"{Firstname} {Lastname}".Trim();
        public decimal TotalAmount => Amount + Charge;
    }
}