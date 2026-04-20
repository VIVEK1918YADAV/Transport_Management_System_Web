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
    public partial class TrainTransaction : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TrainTransaction()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];
            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ────────────────────────────────────────────────────────────────────────
        // Page Load
        // ────────────────────────────────────────────────────────────────────────

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (!IsPostBack)
                    await LoadTransactions();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────
        // Load all transactions (with optional filter params)
        // ────────────────────────────────────────────────────────────────────────

        private async Task LoadTransactions(
            string search   = "",
            string status   = "",
            string type     = "",
            string fromDate = "",
            string toDate   = "")
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainTransactions/GetTrainTransactions");

                if (!response.IsSuccessStatusCode)
                {
                    ShowError($"Failed to load transactions. Status: {response.StatusCode}");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                List<TrainTransactionDto> list =
                    JsonConvert.DeserializeObject<List<TrainTransactionDto>>(json)
                    ?? new List<TrainTransactionDto>();

                // ── Client-side filtering ──────────────────────────────────────

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string s = search.Trim().ToLower();
                    list = list.Where(t =>
                        (t.TrxNo    ?? "").ToLower().Contains(s) ||
                        t.BookingId.ToString().Contains(s) ||
                        (t.UserId ?? "").ToLower().Contains(s)
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(status))
                    list = list.Where(t => string.Equals(t.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrWhiteSpace(type))
                    list = list.Where(t => string.Equals(t.TrxTypeStatus, type, StringComparison.OrdinalIgnoreCase)).ToList();

                if (DateTime.TryParse(fromDate, out DateTime from))
                    list = list.Where(t => t.CreatedAt.HasValue && t.CreatedAt.Value >= from).ToList();

                if (DateTime.TryParse(toDate, out DateTime to))
                    list = list.Where(t => t.CreatedAt.HasValue && t.CreatedAt.Value <= to.AddDays(1).AddSeconds(-1)).ToList();

                // ── Sort newest first ──────────────────────────────────────────
                list = list.OrderByDescending(t => t.CreatedAt).ToList();

                // ── Stats ──────────────────────────────────────────────────────
                int total   = list.Count;
                int success = list.Count(t => string.Equals(t.Status, "Success", StringComparison.OrdinalIgnoreCase));
                int failed  = list.Count(t => string.Equals(t.Status, "Failed",  StringComparison.OrdinalIgnoreCase));
                decimal revenue = list
                    .Where(t => string.Equals(t.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount);

                lblTotal.Text       = total.ToString();
                lblSuccess.Text     = success.ToString();
                lblFailed.Text      = failed.ToString();
                lblRevenue.Text     = revenue.ToString("N2");
                lblRecordCount.Text = $"{total} record{(total != 1 ? "s" : "")}";

                // ── Bind GridView ──────────────────────────────────────────────
                gvTransactions.DataSource = list;
                gvTransactions.DataBind();

                // Store full list in ViewState for paging
                ViewState["TransactionList"] = json;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading transactions: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Filter Button
        // ────────────────────────────────────────────────────────────────────────

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                gvTransactions.PageIndex = 0;
                hfShowModal.Value = "false";
                await LoadTransactions(
                    txtSearch.Text.Trim(),
                    ddlStatus.SelectedValue,
                    ddlType.SelectedValue,
                    txtFromDate.Text,
                    txtToDate.Text
                );
            }));
        }

        // ────────────────────────────────────────────────────────────────────────
        // Reset Button
        // ────────────────────────────────────────────────────────────────────────

        protected void btnReset_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                txtSearch.Text = "";
                ddlStatus.SelectedIndex = 0;
                ddlType.SelectedIndex   = 0;
                txtFromDate.Text = "";
                txtToDate.Text   = "";
                gvTransactions.PageIndex = 0;
                hfShowModal.Value = "false";
                await LoadTransactions();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────
        // GridView Paging
        // ────────────────────────────────────────────────────────────────────────

        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                gvTransactions.PageIndex = e.NewPageIndex;
                hfShowModal.Value = "false";
                await LoadTransactions(
                    txtSearch.Text.Trim(),
                    ddlStatus.SelectedValue,
                    ddlType.SelectedValue,
                    txtFromDate.Text,
                    txtToDate.Text
                );
            }));
        }

        // ────────────────────────────────────────────────────────────────────────
        // GridView Row Commands (View Detail)
        // ────────────────────────────────────────────────────────────────────────

        protected void gvTransactions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetail")
            {
                int trxId = Convert.ToInt32(e.CommandArgument);
                RegisterAsyncTask(new PageAsyncTask(() => LoadTransactionDetail(trxId)));
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Load single transaction into modal labels
        // ────────────────────────────────────────────────────────────────────────

        private async Task LoadTransactionDetail(int trxId)
        {
            try
            {
                // Try ViewState first (data already fetched on page load)
                TrainTransactionDto t = null;

                if (ViewState["TransactionList"] != null)
                {
                    var cachedList = JsonConvert.DeserializeObject<List<TrainTransactionDto>>(
                        ViewState["TransactionList"].ToString());
                    t = cachedList?.FirstOrDefault(x => x.TrxId == trxId);
                }

                // Fallback: re-fetch all and find by TrxId
                if (t == null)
                {
                    HttpResponseMessage response =
                        await client.GetAsync("TrainTransactions/GetTrainTransactions");

                    if (!response.IsSuccessStatusCode)
                    {
                        ShowError("Failed to load transaction details.");
                        return;
                    }

                    string allJson = await response.Content.ReadAsStringAsync();
                    var allList = JsonConvert.DeserializeObject<List<TrainTransactionDto>>(allJson);
                    t = allList?.FirstOrDefault(x => x.TrxId == trxId);
                }

                if (t == null) { ShowError("Transaction not found."); return; }

                lblDTrxId.Text       = "#" + t.TrxId;
                lblDTrxNo.Text       = t.TrxNo        ?? "—";
                lblDBookingId.Text   = "#" + t.BookingId;
                lblDUserId.Text      = t.UserId ?? "—";
                lblDAmount.Text      = t.Amount.ToString("N2");
                lblDExtraCharge.Text = (t.ExtraCharge ?? 0).ToString("N2");
                lblDPostpone1.Text   = (t.PostponeAmt1 ?? 0).ToString("N2");
                lblDPostpone2.Text   = (t.PostponeAmt2 ?? 0).ToString("N2");
                lblDTrxType.Text     = t.TrxTypeStatus  ?? "—";
                lblDPayStatus.Text   = t.PaymentStatus  ?? "—";
                lblDStatus.Text      = t.Status         ?? "—";
                lblDRemarks.Text     = t.PaymentRemarks ?? "—";
                lblDCreatedAt.Text   = t.CreatedAt.HasValue ? t.CreatedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "—";
                lblDUpdatedAt.Text   = t.UpdatedAt.HasValue ? t.UpdatedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "—";
                lblDUpdatedBy.Text   = t.UpdatedBy ?? "—";
                lblDIsActive.Text    = (t.IsActive == true) ? "Yes" : "No";

                // Signal JS to open the modal after page render
                hfShowModal.Value = "true";

                // Re-bind grid to keep current filters/page
                await LoadTransactions(
                    txtSearch.Text.Trim(),
                    ddlStatus.SelectedValue,
                    ddlType.SelectedValue,
                    txtFromDate.Text,
                    txtToDate.Text
                );
            }
            catch (Exception ex)
            {
                ShowError($"Error loading transaction detail: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Helper methods (called from ASPX markup)
        // ────────────────────────────────────────────────────────────────────────

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "badge badge-default";
            switch (status.ToString().ToLower())
            {
                case "success": return "badge badge-success";
                case "failed":  return "badge badge-failed";
                case "pending": return "badge badge-pending";
                default:        return "badge badge-default";
            }
        }

        protected string GetTypeBadge(object type)
        {
            if (type == null) return "badge badge-default";
            switch (type.ToString().ToLower())
            {
                case "payment":  return "badge badge-payment";
                case "refund":   return "badge badge-refund";
                case "postpone": return "badge badge-postpone";
                default:         return "badge badge-default";
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text    = message;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // DTO — mirrors TrainTransaction model from the API
    // ────────────────────────────────────────────────────────────────────────────

    public class TrainTransactionDto
    {
        [JsonProperty("trxId")]
        public int TrxId { get; set; }

        [JsonProperty("trxNo")]
        public string TrxNo { get; set; }

        [JsonProperty("bookingId")]
        public int BookingId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("extraCharge")]
        public decimal? ExtraCharge { get; set; }

        [JsonProperty("postponeAmt1")]
        public decimal? PostponeAmt1 { get; set; }

        [JsonProperty("postponeAmt2")]
        public decimal? PostponeAmt2 { get; set; }

        [JsonProperty("trxTypeStatus")]
        public string TrxTypeStatus { get; set; }

        [JsonProperty("paymentRemarks")]
        public string PaymentRemarks { get; set; }

        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}