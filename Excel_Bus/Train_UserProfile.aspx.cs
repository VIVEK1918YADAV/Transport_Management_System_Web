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

namespace Excel_Bus
{
    public partial class Train_UserProfile : System.Web.UI.Page
    {
        // FIX: Use ConfigurationManager (not obsolete ConfigurationSettings)
        string apiUrl = ConfigurationManager.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Train.aspx", false);
                    return;
                   
                }
                BindDialCodes();
                RegisterAsyncTask(new PageAsyncTask(LoadUserProfile));
            }
            //else
            //{
            //    // Check for message in ViewState and display it
            //    if (ViewState["SuccessMessage"] != null)
            //    {
            //        string message = ViewState["SuccessMessage"].ToString();
            //        string type = ViewState["MessageType"] != null
            //                            ? ViewState["MessageType"].ToString()
            //                            : "info";

            //        string script = string.Format("notify('{0}', '{1}');", type, message);
            //        ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);

            //        // Clear the message
            //        ViewState["SuccessMessage"] = null;
            //        ViewState["MessageType"] = null;
            //    }
            //}
        }


        //private void ShowMessage(string message, string type)
        //{
        //    message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
        //    string title = char.ToUpper(type[0]) + type.Substring(1);
        //    string script = $"showToast('{type}', '{title}', '{message}');";
        //    ScriptManager.RegisterStartupScript(this, GetType(), "toast_" + Guid.NewGuid(), script, true);
        //}
        private async Task LoadUserProfile()
        {
            try
            {
                string userId = Session["UserId"].ToString();

                // FIX: Properly await the async method
                TrainUser user = await GetUserById(userId);

                if (user != null)
                {
                    // Populate form fields with user data
                    hdnUserId.Value = user.Id.ToString();
                    txtFirstName.Text = user.Firstname ?? string.Empty;
                    txtLastName.Text = user.Lastname ?? string.Empty;
                    txtUsername.Text = user.UserName ?? string.Empty;
                    txtEmail.Text = user.Email ?? string.Empty;
                    // ddlDialCode.Text = user.DialCode ?? string.Empty;
                    ListItem dialItem = ddlDialCode.Items.FindByValue(user.DialCode ?? string.Empty);
                    if (dialItem != null)
                        dialItem.Selected = true;
                    txtMobile.Text = user.Mobile ?? string.Empty;
                    txtAddress.Text = user.Address ?? string.Empty;
                    txtCity.Text = user.City ?? string.Empty;
                    txtState.Text = user.State ?? string.Empty;
                    txtZip.Text = user.Zip ?? string.Empty;  // FIX: was missing
                    txtCountryName.Text = user.CountryName ?? string.Empty;
                   hdnCountryCode.Value = user.CountryCode ?? string.Empty;
                   // litBalance.Text = user.Balance.ToString("N2") + " CDF";
                }
                else
                {
                    ShowMessage("Unable to load profile information", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
                ShowMessage("An error occurred while loading your profile", "error");
            }
        }

        // FIX: Return type changed from Task to Task<TrainUser>
        private async Task<TrainUser> GetUserById(string userId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    string endpoint = "TrainUsers/GetTrainUsers";

                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("API Error: " + response.StatusCode);
                        return null;  // FIX: was incorrectly returning 0
                    }

                    string jsonResult = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("Users API Response: " + jsonResult);

                    var users = JsonConvert.DeserializeObject<List<TrainUser>>(jsonResult);

                    // FIX: match on UserId string property
                    return users?.FirstOrDefault(u => u.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get User Error: " + ex.Message);
                throw;
            }
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            // Guard: ensure user session is still valid
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/TrainUserReg.aspx", false);
                return;
            }

            RegisterAsyncTask(new PageAsyncTask(SaveUserProfile));
        }

        private async Task SaveUserProfile()
        {
            try
            {
                string userId = hdnUserId.Value;

                var updatedUser = new TrainUser
                {
                    UserId = userId,
                    Firstname = txtFirstName.Text.Trim(),
                    Lastname = txtLastName.Text.Trim(),
                    UserName = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    DialCode = ddlDialCode.SelectedValue,
                    Mobile = txtMobile.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    State = txtState.Text.Trim(),
                    Zip = txtZip.Text.Trim(),
                    CountryCode = hdnCountryCode.Value
                };

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    string endpoint = "TrainUsers/UpdateTrainUser";

                    string json = JsonConvert.SerializeObject(updatedUser);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Store message in hidden fields so iziToast fires after redirect/postback
                        hdnShowMessage.Value = "true";
                        hdnMessageType.Value = "success";
                        hdnMessageText.Value = "Profile updated successfully.";
                    }
                    else
                    {
                        hdnShowMessage.Value = "true";
                        hdnMessageType.Value = "error";
                        hdnMessageText.Value = "Failed to update profile. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Save profile error: " + ex.Message);
                hdnShowMessage.Value = "true";
                hdnMessageType.Value = "error";
                hdnMessageText.Value = "An unexpected error occurred. Please try again.";
            }
        }

    
        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            hdnShowMessage.Value = "true";
            hdnMessageType.Value = type;
            hdnMessageText.Value = message;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Train.aspx", false);
                return;
            }

            RegisterAsyncTask(new PageAsyncTask(ChangeUserPassword));
        }

        private async Task ChangeUserPassword()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    ShowMessage("Please fill in all password fields.", "error");
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowMessage("New password and confirmation do not match.", "error");
                    return;
                }

                if (txtNewPassword.Text.Length < 8)
                {
                    ShowMessage("Password must be at least 8 characters long.", "error");
                    return;
                }

                int userId = Convert.ToInt32(hdnUserId.Value);

                var payload = new
                {
                    UserId = userId,
                    OldPassword = txtCurrentPassword.Text,
                    NewPassword = txtNewPassword.Text,
                    ConfirmPassword = txtConfirmPassword.Text
                };

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    string endpoint = "TrainUsers/ChangePassword";

                    string json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(endpoint, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Clear password fields
                        txtCurrentPassword.Text = string.Empty;
                        txtNewPassword.Text = string.Empty;
                        txtConfirmPassword.Text = string.Empty;

                        ShowMessage("Your password has been changed successfully.", "success");
                    }
                    else
                    {
                        // Try to extract API error message
                        dynamic result = JsonConvert.DeserializeObject(responseBody);
                        string msg = result?.message ?? result?.Message ?? "Failed to change password. Please try again.";
                        ShowMessage(msg.ToString(), "error");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Change password error: " + ex.Message);
                ShowMessage("An unexpected error occurred. Please try again.", "error");
            }
        }

        private void BindDialCodes()
        {
            var dialCodes = new List<ListItem>
    {
        new ListItem("-- Select --", ""),
        new ListItem("Afghanistan (+93)", "+93"),
        new ListItem("Albania (+355)", "+355"),
        new ListItem("Algeria (+213)", "+213"),
        new ListItem("Argentina (+54)", "+54"),
        new ListItem("Australia (+61)", "+61"),
        new ListItem("Austria (+43)", "+43"),
        new ListItem("Bangladesh (+880)", "+880"),
        new ListItem("Belgium (+32)", "+32"),
        new ListItem("Brazil (+55)", "+55"),
        new ListItem("Canada (+1)", "+1"),
        new ListItem("China (+86)", "+86"),
        new ListItem("Denmark (+45)", "+45"),
        new ListItem("Egypt (+20)", "+20"),
        new ListItem("Finland (+358)", "+358"),
        new ListItem("France (+33)", "+33"),
        new ListItem("Germany (+49)", "+49"),
        new ListItem("Ghana (+233)", "+233"),
        new ListItem("Greece (+30)", "+30"),
        new ListItem("India (+91)", "+91"),
        new ListItem("Indonesia (+62)", "+62"),
        new ListItem("Iran (+98)", "+98"),
        new ListItem("Iraq (+964)", "+964"),
        new ListItem("Ireland (+353)", "+353"),
        new ListItem("Israel (+972)", "+972"),
        new ListItem("Italy (+39)", "+39"),
        new ListItem("Japan (+81)", "+81"),
        new ListItem("Jordan (+962)", "+962"),
        new ListItem("Kenya (+254)", "+254"),
        new ListItem("Kuwait (+965)", "+965"),
        new ListItem("Malaysia (+60)", "+60"),
        new ListItem("Mexico (+52)", "+52"),
        new ListItem("Morocco (+212)", "+212"),
        new ListItem("Myanmar (+95)", "+95"),
        new ListItem("Nepal (+977)", "+977"),
        new ListItem("Netherlands (+31)", "+31"),
        new ListItem("New Zealand (+64)", "+64"),
        new ListItem("Nigeria (+234)", "+234"),
        new ListItem("Norway (+47)", "+47"),
        new ListItem("Oman (+968)", "+968"),
        new ListItem("Pakistan (+92)", "+92"),
        new ListItem("Philippines (+63)", "+63"),
        new ListItem("Poland (+48)", "+48"),
        new ListItem("Portugal (+351)", "+351"),
        new ListItem("Qatar (+974)", "+974"),
        new ListItem("Romania (+40)", "+40"),
        new ListItem("Russia (+7)", "+7"),
        new ListItem("Saudi Arabia (+966)", "+966"),
        new ListItem("Singapore (+65)", "+65"),
        new ListItem("South Africa (+27)", "+27"),
        new ListItem("South Korea (+82)", "+82"),
        new ListItem("Spain (+34)", "+34"),
        new ListItem("Sri Lanka (+94)", "+94"),
        new ListItem("Sweden (+46)", "+46"),
        new ListItem("Switzerland (+41)", "+41"),
        new ListItem("Thailand (+66)", "+66"),
        new ListItem("Turkey (+90)", "+90"),
        new ListItem("Ukraine (+380)", "+380"),
        new ListItem("United Arab Emirates (+971)", "+971"),
        new ListItem("United Kingdom (+44)", "+44"),
        new ListItem("United States (+1)", "+1"),
        new ListItem("Vietnam (+84)", "+84"),
    };

            ddlDialCode.DataSource = dialCodes;
            ddlDialCode.DataBind();

            // Pre-select India as default
            //ddlDialCode.Items.FindByValue("+91").Selected = true;
            // ❌ Yeh kaam nahi karta DropDownList ke liye
            
        }

    }
    // User model class
    public class TrainUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("dialCode")]
        public string DialCode { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("refBy")]
        public int RefBy { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("ev")]
        public int Ev { get; set; }

        [JsonProperty("sv")]
        public int Sv { get; set; }

        [JsonProperty("profileComplete")]
        public int ProfileComplete { get; set; }

        [JsonProperty("verCode")]
        public string VerCode { get; set; }

        [JsonProperty("verCodeSendAt")]
        public DateTime? VerCodeSendAt { get; set; }

        [JsonProperty("ts")]
        public int Ts { get; set; }

        [JsonProperty("tv")]
        public int Tv { get; set; }

        [JsonProperty("tsc")]
        public string Tsc { get; set; }

        [JsonProperty("banReason")]
        public string BanReason { get; set; }

        [JsonProperty("rememberToken")]
        public string RememberToken { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("providerId")]
        public string ProviderId { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("bookedTickets")]
        public List<object> BookedTickets { get; set; }
    }
}