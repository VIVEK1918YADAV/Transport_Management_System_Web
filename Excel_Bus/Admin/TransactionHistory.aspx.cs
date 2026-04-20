using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class TransactionHistory : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Pagination settings
        private const int PageSize = 10; // Number of records per page

        static TransactionHistory()
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if there's a search parameter in query string
                string searchParam = Request.QueryString["search"];
                if (!string.IsNullOrEmpty(searchParam))
                {
                    txtSearch.Text = searchParam;
                }

                RegisterAsyncTask(new PageAsyncTask(() => LoadTransactions()));
            }
        }

        private async Task LoadTransactions(string searchTerm = "", string dateRange = "", string trxType = "")
        {
            try
            {
                pnlError.Visible = false;

                // Build API endpoint
                string endpoint = "TransactionsNew/GetTransactions";

                // Make API call
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(jsonResponse);

                    if (transactions == null || transactions.Count == 0)
                    {
                        pnlNoData.Visible = true;
                        gvTransactions.Visible = false;
                        pnlStats.Visible = false;
                        pnlPagination.Visible = false;
                        return;
                    }

                    // Fetch usernames for all users
                    await PopulateUsernames(transactions);

                    // Order by created date descending
                    var orderedTransactions = transactions.OrderByDescending(t => t.CreatedAt).ToList();

                    // Store in ViewState for filtering
                    ViewState["AllTransactions"] = JsonConvert.SerializeObject(orderedTransactions);

                    // Apply filters
                    var filteredTransactions = ApplyFilters(orderedTransactions, searchTerm, dateRange, trxType);

                    // Store filtered transactions
                    ViewState["FilteredTransactions"] = JsonConvert.SerializeObject(filteredTransactions);

                    // Reset to page 1
                    ViewState["CurrentPage"] = 1;

                    // Bind to GridView with pagination
                    BindData(filteredTransactions);
                }
                else
                {
                    ShowError($"Failed to load transactions. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading transactions: {ex.Message}");
            }
        }

        private async Task PopulateUsernames(List<Transaction> transactions)
        {
            try
            {
                // Get unique user IDs
                var userIds = transactions.Select(t => t.UserId).Distinct().ToList();

                // Fetch user details from API
                string endpoint = "Users/GetAllUsers"; // Adjust endpoint as per your API

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<UserInfo> users = JsonConvert.DeserializeObject<List<UserInfo>>(jsonResponse);

                    if (users != null && users.Count > 0)
                    {
                        // Create a dictionary for quick lookup
                        var userDict = users.ToDictionary(u => u.Id, u => u.Username);

                        // Populate usernames in transactions
                        foreach (var transaction in transactions)
                        {
                            if (userDict.ContainsKey(transaction.UserId))
                            {
                                transaction.Username = userDict[transaction.UserId];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If user fetch fails, continue without usernames
                System.Diagnostics.Debug.WriteLine($"Failed to fetch usernames: {ex.Message}");
            }
        }

        private List<Transaction> ApplyFilters(List<Transaction> transactions, string searchTerm, string dateRange, string trxType)
        {
            var filtered = transactions.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchLower = searchTerm.ToLower();
                filtered = filtered.Where(t =>
                    (!string.IsNullOrEmpty(t.Trx) && t.Trx.ToLower().Contains(searchLower)) ||
                    (t.UserId.ToString().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(t.Username) && t.Username.ToLower().Contains(searchLower))
                );
            }

            // Apply transaction type filter
            if (!string.IsNullOrEmpty(trxType))
            {
                string trxTypeLower = trxType.ToLower();

                // Filter based on exact match in Details field
                filtered = filtered.Where(t =>
                {
                    string details = (t.Details ?? "").ToLower();

                    // Check for exact transaction type matches
                    if (trxTypeLower == "payment")
                    {
                        return details.Contains("payment") && !details.Contains("refund") && !details.Contains("cancellation");
                    }
                    else if (trxTypeLower == "refund")
                    {
                        return details.Contains("refund");
                    }
                    else if (trxTypeLower == "full cancellation" || trxTypeLower == "cancellation")
                    {
                        return details.Contains("cancellation");
                    }
                    else if (trxTypeLower == "postpone")
                    {
                        return details.Contains("postpone");
                    }

                    return false;
                });
            }

            // Apply date range filter
            if (!string.IsNullOrEmpty(dateRange))
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                // Parse date range - Flatpickr format: "2025-11-05 to 2025-11-20"
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
                    // Try single date
                    if (DateTime.TryParse(dateRange.Trim(), out DateTime singleDate))
                    {
                        startDate = singleDate;
                        endDate = singleDate.AddDays(1).AddSeconds(-1);
                    }
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    filtered = filtered.Where(t =>
                        t.CreatedAt >= startDate.Value && t.CreatedAt <= endDate.Value
                    );
                }
            }

            return filtered.ToList();
        }

        private void BindData(List<Transaction> transactions)
        {
            int pageSize = ViewState["PageSize"] != null ? (int)ViewState["PageSize"] : PageSize;
            BindDataWithPageSize(transactions, pageSize);
        }

        private void BindDataWithPageSize(List<Transaction> transactions, int pageSize)
        {
            if (transactions.Count > 0)
            {
                // Get current page
                int currentPage = ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;

                // Calculate pagination
                int totalRecords = transactions.Count;
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Ensure current page is valid
                if (currentPage > totalPages) currentPage = totalPages;
                if (currentPage < 1) currentPage = 1;

                ViewState["CurrentPage"] = currentPage;
                ViewState["TotalPages"] = totalPages;
                ViewState["TotalRecords"] = totalRecords;

                // Get records for current page
                var pagedTransactions = transactions
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                gvTransactions.DataSource = pagedTransactions;
                gvTransactions.DataBind();
                gvTransactions.Visible = true;
                pnlNoData.Visible = false;

                // Update pagination controls
                UpdatePaginationControls(currentPage, totalPages, totalRecords);

                // Calculate and display stats (using all filtered transactions, not just current page)
                UpdateStatistics(transactions);
            }
            else
            {
                gvTransactions.Visible = false;
                pnlNoData.Visible = true;
                pnlStats.Visible = false;
                pnlPagination.Visible = false;
            }
        }

        private void UpdatePaginationControls(int currentPage, int totalPages, int totalRecords)
        {
            pnlPagination.Visible = totalPages > 1;

            // Get current page size
            int currentPageSize = PageSize;
            if (ViewState["PageSize"] != null)
            {
                currentPageSize = (int)ViewState["PageSize"];
            }

            // Calculate record range
            int startRecord = ((currentPage - 1) * currentPageSize) + 1;
            int endRecord = Math.Min(currentPage * currentPageSize, totalRecords);

            lblPageInfo.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} tickets";
            lblCurrentPage.Text = currentPage.ToString();
            lblTotalPages.Text = totalPages.ToString();

            // Enable/disable navigation buttons
            btnFirstPage.Enabled = currentPage > 1;
            btnPrevPage.Enabled = currentPage > 1;
            btnNextPage.Enabled = currentPage < totalPages;
            btnLastPage.Enabled = currentPage < totalPages;
        }

        private void UpdateStatistics(List<Transaction> transactions)
        {
            pnlStats.Visible = true;

            lblTotalCount.Text = transactions.Count.ToString();

            // Calculate total credits (positive amounts)
            decimal totalCredits = transactions
                .Where(t => t.TrxType == "+" || (t.Details != null && t.Details.Contains("Refund")))
                .Sum(t => t.Amount);
            lblTotalCredits.Text = $"{totalCredits:N2} CDF";

            // Calculate total debits (negative amounts)
            decimal totalDebits = transactions
                .Where(t => t.TrxType == "-" || (t.Details != null && (t.Details.Contains("Payment") || t.Details.Contains("cancellation"))))
                .Sum(t => t.Amount);
            lblTotalDebits.Text = $"{totalDebits:N2} CDF";
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            string dateRange = txtDateRange.Text.Trim();
            string trxType = ddlTrxType.SelectedValue;

            // Perform client-side filtering if data is in ViewState
            if (ViewState["AllTransactions"] != null)
            {
                string json = ViewState["AllTransactions"].ToString();
                List<Transaction> allTransactions = JsonConvert.DeserializeObject<List<Transaction>>(json);

                var filteredTransactions = ApplyFilters(allTransactions, search, dateRange, trxType);

                // Store filtered transactions
                ViewState["FilteredTransactions"] = JsonConvert.SerializeObject(filteredTransactions);

                // Reset to page 1
                ViewState["CurrentPage"] = 1;

                BindData(filteredTransactions);
            }
            else
            {
                // If ViewState is empty, reload from API with filters
                RegisterAsyncTask(new PageAsyncTask(() => LoadTransactions(search, dateRange, trxType)));
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtDateRange.Text = "";
            ddlTrxType.SelectedIndex = 0;

            // Reload all transactions
            if (ViewState["AllTransactions"] != null)
            {
                string json = ViewState["AllTransactions"].ToString();
                List<Transaction> allTransactions = JsonConvert.DeserializeObject<List<Transaction>>(json);

                // Store filtered transactions (all in this case)
                ViewState["FilteredTransactions"] = JsonConvert.SerializeObject(allTransactions);

                // Reset to page 1
                ViewState["CurrentPage"] = 1;

                BindData(allTransactions);
            }
            else
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadTransactions()));
            }
        }

        // Pagination event handlers
        protected void btnFirstPage_Click(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            ReloadCurrentPage();
        }

        protected void btnPrevPage_Click(object sender, EventArgs e)
        {
            int currentPage = ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
            if (currentPage > 1)
            {
                ViewState["CurrentPage"] = currentPage - 1;
                ReloadCurrentPage();
            }
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            int currentPage = ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
            int totalPages = ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1;

            if (currentPage < totalPages)
            {
                ViewState["CurrentPage"] = currentPage + 1;
                ReloadCurrentPage();
            }
        }

        protected void btnLastPage_Click(object sender, EventArgs e)
        {
            int totalPages = ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1;
            ViewState["CurrentPage"] = totalPages;
            ReloadCurrentPage();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update page size
            if (int.TryParse(ddlPageSize.SelectedValue, out int newPageSize))
            {
                ViewState["PageSize"] = newPageSize;
                ViewState["CurrentPage"] = 1; // Reset to first page

                // Reload with new page size
                if (ViewState["FilteredTransactions"] != null)
                {
                    string json = ViewState["FilteredTransactions"].ToString();
                    List<Transaction> filteredTransactions = JsonConvert.DeserializeObject<List<Transaction>>(json);
                    BindDataWithPageSize(filteredTransactions, newPageSize);
                }
            }
        }

        private void ReloadCurrentPage()
        {
            if (ViewState["FilteredTransactions"] != null)
            {
                string json = ViewState["FilteredTransactions"].ToString();
                List<Transaction> filteredTransactions = JsonConvert.DeserializeObject<List<Transaction>>(json);
                BindData(filteredTransactions);
            }
        }

        protected void gvTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Additional row customization can be added here if needed
            }
        }

        protected int GetSerialNumber(int rowIndex)
        {
            int currentPage = ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
            int pageSize = ViewState["PageSize"] != null ? (int)ViewState["PageSize"] : PageSize;
            return ((currentPage - 1) * pageSize) + rowIndex + 1;
        }

        protected string GetDisplayUsername(object username, object userId)
        {
            string user = username?.ToString();
            if (!string.IsNullOrEmpty(user))
            {
                return user;
            }
            return "User #" + userId?.ToString();
        }

        protected string GetSearchUsername(object username, object userId)
        {
            string user = username?.ToString();
            if (!string.IsNullOrEmpty(user))
            {
                return user;
            }
            return userId?.ToString();
        }

        protected string GetTrxTypeBadge(object trxType, object details)
        {
            string type = trxType?.ToString() ?? "";
            string detail = (details?.ToString() ?? "").ToLower();

            string badgeClass = "trx-type";
            string badgeText = type;

            // Determine badge type based on details and trxType
            if (detail.Contains("payment"))
            {
                badgeClass += " payment";
                badgeText = "Payment";
            }
            else if (detail.Contains("refund"))
            {
                badgeClass += " refund";
                badgeText = "Refund";
            }
            else if (detail.Contains("cancellation"))
            {
                badgeClass += " cancellation";
                badgeText = "Cancellation";
            }
            else if (detail.Contains("postpone"))
            {
                badgeClass += " postpone";
                badgeText = "Postpone";
            }
            else if (detail.Contains("booking"))
            {
                badgeClass += " booking";
                badgeText = "Booking";
            }

            return $"<span class='{badgeClass}'>{badgeText}</span>";
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

            return dt.ToString("dd MMM yyyy hh:mm tt");
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

        protected string FormatAmount(object amount, object trxType)
        {
            if (amount == null) return "0.00 CDF";

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
                return "0.00 CDF";
            }

            string type = (trxType?.ToString() ?? "").ToLower();

            // Determine if it's credit or debit based on common patterns
            bool isCredit = type == "+" || type.Contains("refund");

            string sign = isCredit ? "+ " : "- ";
            string colorClass = isCredit ? "text-success" : "text-danger";

            return $"<span class='amount-value {colorClass}'>{sign}{amountValue:N2} CDF</span>";
        }

        protected string FormatBalance(object balance)
        {
            if (balance == null) return "0.00 CDF";

            decimal balanceValue;
            if (balance is decimal)
            {
                balanceValue = (decimal)balance;
            }
            else if (decimal.TryParse(balance.ToString(), out balanceValue))
            {
                // Parsed successfully
            }
            else
            {
                return "0.00 CDF";
            }

            return $"{balanceValue:N2} CDF";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }

    public class Transaction
    {
        [JsonProperty("trxId")]
        public int TrxId { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        public string Username { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("charge")]
        public decimal Charge { get; set; }

        [JsonProperty("postBalance")]
        public decimal PostBalance { get; set; }

        [JsonProperty("postBalance2")]
        public decimal PostBalance2 { get; set; }

        [JsonProperty("trxType")]
        public string TrxType { get; set; }

        [JsonProperty("trxNumber")]
        public string Trx { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("bookedId")]
        public int? BookedId { get; set; }

        [JsonProperty("oldDate")]
        public string OldDate { get; set; }

        [JsonProperty("newDate")]
        public string NewDate { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }
    }
}