using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_ActiveUser : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_ActiveUser()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (!IsPostBack)
                {
                    await LoadActiveUsers();
                }
            }));
        }

        private async Task LoadActiveUsers()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainUsers/GetActiveTrainUsers");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<ActiveUserDto> users =
                        JsonConvert.DeserializeObject<List<ActiveUserDto>>(jsonResponse);

                    gvActiveUsers.DataSource = users;
                    gvActiveUsers.DataBind();
                }
                else
                {
                    ShowError($"Failed to load active users. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading active users: {ex.Message}");
            }
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "status-badge";
            return status.ToString() == "ACTIVE"
                ? "status-badge status-enabled"
                : "status-badge status-disabled";
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "Active" : "Inactive";
        }

        protected string GetAvatarClass(object id)
        {
            int index = id != null ? (Convert.ToInt32(id) % 8) : 0;
            return "user-avatar avatar-color-" + index;
        }

        protected string GetBalanceClass(object balance)
        {
            if (balance == null) return "balance-zero";
            decimal val = Convert.ToDecimal(balance);
            if (val > 0) return "balance-positive";
            if (val < 0) return "balance-negative";
            return "balance-zero";
        }

        protected string GetBalanceDisplay(object balance)
        {
            if (balance == null) return "0.00";
            return string.Format("{0:N2}", Convert.ToDecimal(balance));
        }

        protected string GetInitials(object firstname, object lastname)
        {
            string f = firstname?.ToString() ?? "";
            string l = lastname?.ToString() ?? "";
            string initials = "";
            if (f.Length > 0) initials += f[0];
            if (l.Length > 0) initials += l[0];
            return initials.ToUpper();
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID +
                "').classList.add('show');", true);
        }
    }

    public class ActiveUserDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}