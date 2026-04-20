using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class User_profile : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/UserLogin.aspx", false);
                    return;
                }

                RegisterAsyncTask(new PageAsyncTask(LoadUserProfile));
            }
            else
            {
                // Check for message in ViewState and display it
                if (ViewState["SuccessMessage"] != null)
                {
                    string message = ViewState["SuccessMessage"].ToString();
                    string type = ViewState["MessageType"].ToString();

                    string script = $"notify('{type}', '{message}');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);

                    // Clear the message
                    ViewState["SuccessMessage"] = null;
                    ViewState["MessageType"] = null;
                }
            }
        }

        private async Task LoadUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                var user = await GetUserById(userId);

                if (user != null)
                {
                    // Populate form fields with user data
                    hdnUserId.Value = user.Id.ToString();
                    txtFirstName.Text = user.Firstname ?? "";
                    txtLastName.Text = user.Lastname ?? "";
                    txtUsername.Text = user.Username ?? "";
                    txtEmail.Text = user.Email ?? "";
                    txtDialCode.Text = user.DialCode ?? "";
                    txtMobile.Text = user.Mobile ?? "";
                    txtAddress.Text = user.Address ?? "";
                    txtCity.Text = user.City ?? "";
                    txtState.Text = user.State ?? "";
                    txtZip.Text = user.Zip ?? "";
                    txtCountryName.Text = user.CountryName ?? "";
                    hdnCountryCode.Value = user.CountryCode ?? "";
                    txtBalance.Text = user.Balance.ToString("N2") + " CDF";
                }
                else
                {
                    ShowMessage("Unable to load profile information", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
                ShowMessage("An error occurred while loading your profile", "error");
            }
        }

        private async Task<User> GetUserById(int userId)
        {
            try
            {
                // Create NEW HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);

                    string endpoint = $"Users/GetUsers";

                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                        return null;
                    }

                    string jsonResult = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Users API Response: {jsonResult}");

                    var users = JsonConvert.DeserializeObject<List<User>>(jsonResult);
                    return users?.FirstOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get User Error: {ex.Message}");
                throw;
            }
        }
        protected async void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(hdnUserId.Value);
                var currentUser = await GetUserById(userId);

                if (currentUser == null)
                {
                    hdnShowMessage.Value = "true";
                    hdnMessageType.Value = "error";
                    hdnMessageText.Value = "Unable to retrieve user information";
                    return;
                }

                currentUser.Firstname = txtFirstName.Text.Trim();
                currentUser.Lastname = txtLastName.Text.Trim();
                currentUser.Username = txtUsername.Text.Trim();
                currentUser.DialCode = txtDialCode.Text.Trim();
                currentUser.Mobile = txtMobile.Text.Trim();
                currentUser.Address = txtAddress.Text.Trim();
                currentUser.City = txtCity.Text.Trim();
                currentUser.State = txtState.Text.Trim();
                currentUser.Zip = txtZip.Text.Trim();
                //currentUser.UpdatedAt = DateTime.Now;

                bool success = await UpdateUserProfile(currentUser);

                if (success)
                {
                    await LoadUserProfile();

                    hdnShowMessage.Value = "true";
                    hdnMessageType.Value = "success";
                    hdnMessageText.Value = "Profile updated successfully!";
                }
                else
                {
                    hdnShowMessage.Value = "true";
                    hdnMessageType.Value = "error";
                    hdnMessageText.Value = "Failed to update profile. Please try again.";
                }
            }
            catch (Exception ex)
            {
                hdnShowMessage.Value = "true";
                hdnMessageType.Value = "error";
                hdnMessageText.Value = "An error occurred while updating your profile";
            }
        }

        private async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                // Create NEW HttpClient instance for this request
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);

                    string endpoint = $"Users/PutUser/{user.Id}";

                    // Serialize user object to JSON
                    string jsonContent = JsonConvert.SerializeObject(user, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatString = "yyyy-MM-ddTHH:mm:ss"
                    });

                    System.Diagnostics.Debug.WriteLine($"=== UPDATE USER DEBUG ===");
                    System.Diagnostics.Debug.WriteLine($"API Base URL: {apiUrl}");
                    System.Diagnostics.Debug.WriteLine($"Full Endpoint: {client.BaseAddress}{endpoint}");
                    System.Diagnostics.Debug.WriteLine($"User ID: {user.Id}");
                    System.Diagnostics.Debug.WriteLine($"Payload: {jsonContent}");

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"Response Status Code: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");
                    System.Diagnostics.Debug.WriteLine($"=== END DEBUG ===");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"INNER EXCEPTION: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"STACK TRACE: {ex.StackTrace}");
                return false;
            }
        }

        //protected void btnLogout_Click(object sender, EventArgs e)
        //{
        //    Session.Clear();
        //    Session.Abandon();
        //    Response.Redirect("~/UserLogin.aspx", false);
        //}

        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"notify('{type}', '{message}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid(), script, true);
        }
    }

    // User model class
    public class User1
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

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
        public int Status { get; set; }

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