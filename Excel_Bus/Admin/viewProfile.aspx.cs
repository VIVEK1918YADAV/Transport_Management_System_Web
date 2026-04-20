using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;

namespace Excel_Bus.Admin
{
    public partial class viewProfile : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static viewProfile()
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
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadAdminProfile();
                }));
            }
        }

        private async Task LoadAdminProfile()
        {
            try
            {
                pnlError.Visible = false;
                pnlLoading.Visible = true;
                pnlProfile.Visible = false;

                // Session se AdminId get karo
                int adminId = 1; // Default value

                if (Session["AdminId"] != null)
                {
                    adminId = Convert.ToInt32(Session["AdminId"]);
                }

                HttpResponseMessage response = await client.GetAsync($"Admins/GetAdmin/{adminId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    AdminProfileDto profile = JsonConvert.DeserializeObject<AdminProfileDto>(jsonResponse);

                    if (profile != null)
                    {
                        DisplayProfile(profile);
                        pnlProfile.Visible = true;
                    }
                    else
                    {
                        ShowError("Profile data not found.");
                    }
                }
                else
                {
                    ShowError($"Failed to load profile. Status: {response.StatusCode}");
                }

                pnlLoading.Visible = false;
            }
            catch (Exception ex)
            {
                pnlLoading.Visible = false;
                ShowError($"Error loading profile: {ex.Message}");
            }
        }

        private void DisplayProfile(AdminProfileDto profile)
        {
            // Avatar Initials
            //lblInitials.Text = GetInitials(profile.Name);

            // Basic Info
            //lblName.Text = profile.Name ?? "N/A";
            //lblUsername.Text = profile.Username ?? "N/A";
            lblUsernameValue.Text = profile.Username ?? "N/A";
            lblEmail.Text = profile.Email ?? "N/A";
            lblMobile.Text = !string.IsNullOrEmpty(profile.Mobile) ? profile.Mobile : "Not provided";

            // Role & Status
            lblRoleId.Text = profile.RoleId.HasValue ? profile.RoleId.Value.ToString() : "N/A";

            lblStatus.Text = profile.Status ?? "N/A";
            lblStatus.CssClass = "status-badge " + (profile.Status?.ToLower() == "active" ? "status-active" : "status-inactive");

            // Dates
            lblCreatedAt.Text = profile.CreatedAt.HasValue ? profile.CreatedAt.Value.ToString("MMM dd, yyyy") : "N/A";
            lblUpdatedAt.Text = profile.UpdatedAt.HasValue ? profile.UpdatedAt.Value.ToString("MMM dd, yyyy hh:mm tt") : "N/A";

            // Email Verification
            if (profile.EmailVerifiedAt.HasValue)
            {
                lblEmailVerified.Text = "✓ Verified on " + profile.EmailVerifiedAt.Value.ToString("MMM dd, yyyy");
            }
            else
            {
                lblEmailVerified.Text = "✗ Not verified";
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "?";

            var parts = name.Trim().Split(' ');
            if (parts.Length >= 2)
            {
                return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
            }
            else if (parts.Length == 1 && parts[0].Length >= 2)
            {
                return parts[0].Substring(0, 2).ToUpper();
            }
            else if (parts.Length == 1)
            {
                return parts[0][0].ToString().ToUpper();
            }

            return "?";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlProfile.Visible = false;
        }
    }

    public class AdminProfileDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("emailVerifiedAt")]
        public DateTime? EmailVerifiedAt { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("rememberToken")]
        public string RememberToken { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("roleId")]
        public int? RoleId { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("role")]
        public object Role { get; set; }
    }
}