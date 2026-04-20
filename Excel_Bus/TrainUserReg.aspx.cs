using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using System.Text;

namespace ExcelBus
{
    public partial class TrainUserReg : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if train user is already logged in
                if (Session["UserId"] != null)
                {
                    Response.Redirect("~/TrainTicket.aspx", false);
                    return;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isRegister = hdnIsRegister.Value == "true";

            if (isRegister)
            {
                Page.RegisterAsyncTask(new PageAsyncTask(HandleRegistration));
            }
            else
            {
                Page.RegisterAsyncTask(new PageAsyncTask(HandleLogin));
            }
        }

        private async Task HandleLogin()
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowMessage("Username and password are required.", "warning");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting train user login for: {username}");

                var result = await CallLoginAPI(username, password);

                System.Diagnostics.Debug.WriteLine($"Train Login result - Success: {result?.Success}, Message: {result?.Message}");

                if (result != null && result.Success)
                {
                    // Store train user data in session
                    if (result.Data != null)
                    {
                        Session["UserId"] = result.Data.UserId;
                        Session["TrainUsername"] = result.Data.Username;
                        Session["TrainEmail"] = result.Data.Email;
                        Session["TrainFirstname"] = result.Data.Firstname;
                        Session["TrainLastname"] = result.Data.Lastname;
                        Session["TrainFullName"] = $"{result.Data.Firstname} {result.Data.Lastname}".Trim();
                        Session["TrainMobile"] = result.Data.Mobile;
                        Session["TrainBalance"] = result.Data.Balance;
                        Session["TrainUserData"] = JsonConvert.SerializeObject(result.Data);

                        System.Diagnostics.Debug.WriteLine($"Train user session data stored for ID: {result.Data.UserId}");
                    }

                    ShowMessage("Login successful! Redirecting to dashboard...", "success");

                    // Redirect after a short delay to allow message to display
                    //string redirectScript = @"
                    //    <script type='text/javascript'>
                    //        setTimeout(function(){ 
                    //            window.location.href='TrainTicket.aspx'; 
                    //        }, 1500);
                    //    </script>";
                    //ClientScript.RegisterStartupScript(this.GetType(), "RedirectToTrainDashboard", redirectScript, false);
                    string redirectScript = string.Format(@"
        <script type='text/javascript'>
            setTimeout(function(){{ 
                window.location.href='TrainUserDashboard.aspx?UserId={0}'; 
            }}, 1500);
        </script>", result.Data.UserId);

                    ClientScript.RegisterStartupScript(this.GetType(), "RedirectToTrainDashboard", redirectScript, false);
                }
                else
                {
                    // Show the error message from API or default message
                    string errorMessage = !string.IsNullOrEmpty(result?.Message)
                        ? result.Message
                        : "Invalid username or password.";

                    System.Diagnostics.Debug.WriteLine($"Train login failed with message: {errorMessage}");
                    ShowMessage(errorMessage, "error");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                ShowMessage("Unable to connect to server. Please try again later.", "error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in HandleLogin: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowMessage("Login failed. Please try again.", "error");
            }
        }

        private async Task HandleRegistration()
        {
            try
            {
                string firstname = txtFirstname.Text.Trim();
                string lastname = txtLastname.Text.Trim();
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string mobile = txtMobile.Text.Trim();
                string password = txtPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                // Server-side validation
                if (string.IsNullOrEmpty(firstname))
                {
                    ShowMessage("First name is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (string.IsNullOrEmpty(lastname))
                {
                    ShowMessage("Last name is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (string.IsNullOrEmpty(username))
                {
                    ShowMessage("Username is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    ShowMessage("Email is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (string.IsNullOrEmpty(mobile))
                {
                    ShowMessage("Mobile number is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    ShowMessage("Password is required.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (password.Length < 6)
                {
                    ShowMessage("Password must be at least 6 characters long.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (password != confirmPassword)
                {
                    ShowMessage("Password and confirm password do not match.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                if (!chkAgreeTerms.Checked)
                {
                    ShowMessage("Please agree to the terms and conditions.", "warning");
                    hdnIsRegister.Value = "true";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Attempting train user registration for: {username}");

                var registerData = new TrainRegisterRequest
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Username = username,
                    Email = email,
                    Mobile = mobile,
                    Password = password,
                    CountryName = "India",
                    CountryCode = "IN",
                    Status = "ACTIVE",
                    IsActive = true
                };

                var result = await CallRegisterAPI(registerData);

                System.Diagnostics.Debug.WriteLine($"Train registration result - Success: {result?.Success}, Message: {result?.Message}");

                if (result != null && result.Success)
                {
                    ShowMessage("Registration successful! Please login.", "success");

                    // Clear registration fields and switch to login mode
                    txtFirstname.Text = "";
                    txtLastname.Text = "";
                    txtEmail.Text = "";
                    txtMobile.Text = "";
                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";
                    chkAgreeTerms.Checked = false;

                    // Keep username for login
                    string registeredUsername = username;
                    txtUsername.Text = registeredUsername;

                    // Switch to login mode
                    hdnIsRegister.Value = "false";

                    string redirectScript = @"
                        <script type='text/javascript'>
                            setTimeout(function(){ 
                                window.location.href='TrainUserReg.aspx'; 
                            }, 1500);
                        </script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "RedirectToLogin", redirectScript, false);
                }
                else
                {
                    string errorMessage = !string.IsNullOrEmpty(result?.Message)
                        ? result.Message
                        : "Registration failed. Please try again.";

                    System.Diagnostics.Debug.WriteLine($"Train registration failed with message: {errorMessage}");
                    ShowMessage(errorMessage, "error");
                    hdnIsRegister.Value = "true";
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                ShowMessage("Unable to connect to server. Please try again later.", "error");
                hdnIsRegister.Value = "true";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in HandleRegistration: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowMessage("Registration failed. Please try again.", "error");
                hdnIsRegister.Value = "true";
            }
        }

        private async Task<TrainLoginResponse> CallLoginAPI(string username, string password)
        {
            try
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);

                var loginRequest = new
                {
                    username = username,
                    password = password
                };

                string jsonContent = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Train Login Request: {jsonContent}");

                // Call the TrainUsers Login endpoint
                HttpResponseMessage response = await client.PostAsync("TrainUsers/Login", content);
                string jsonResult = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Train Login API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Train Login API Response: {jsonResult}");

                // Parse the response regardless of status code
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<TrainLoginResponse>(jsonResult);
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                        return new TrainLoginResponse
                        {
                            Success = false,
                            Message = "Invalid response from server."
                        };
                    }
                }

                return new TrainLoginResponse
                {
                    Success = false,
                    Message = "No response from server."
                };
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return new TrainLoginResponse
                {
                    Success = false,
                    Message = "Unable to connect to server. Please check your connection."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Train Login API Error: {ex.Message}");
                return new TrainLoginResponse
                {
                    Success = false,
                    Message = "An error occurred while connecting to the server."
                };
            }
        }

        private async Task<TrainRegisterResponse> CallRegisterAPI(TrainRegisterRequest registerData)
        {
            try
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);

                // Use PostTrainUser endpoint
                string endpoint = "TrainUsers/PostTrainUser";

                string jsonContent = JsonConvert.SerializeObject(registerData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Train Registration Request: {jsonContent}");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);
                string jsonResult = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Train Register API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Train Register API Response: {jsonResult}");

                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<TrainRegisterResponse>(jsonResult);
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                        return new TrainRegisterResponse
                        {
                            Success = false,
                            Message = "Invalid response from server."
                        };
                    }
                }

                return new TrainRegisterResponse
                {
                    Success = false,
                    Message = "No response from server."
                };
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return new TrainRegisterResponse
                {
                    Success = false,
                    Message = "Unable to connect to server. Please check your connection."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Train Register API Error: {ex.Message}");
                return new TrainRegisterResponse
                {
                    Success = false,
                    Message = "An error occurred while connecting to the server."
                };
            }
        }

        private void ShowMessage(string message, string type)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // Escape special characters for JavaScript
            message = message.Replace("\\", "\\\\")
                           .Replace("'", "\\'")
                           .Replace("\"", "\\\"")
                           .Replace("\n", "\\n")
                           .Replace("\r", "");

            string script = $@"
                <script type='text/javascript'>
                    window.addEventListener('load', function() {{
                        if (typeof notify === 'function') {{
                            notify('{type}', '{message}');
                        }}
                    }});
                </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid().ToString(), script, false);
        }
    }

    #region Train User API Response Models

    // Train Login API Response Models
    public class TrainLoginResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TrainUserData Data { get; set; }
    }

    public class TrainUserData
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
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public decimal? Balance { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    // Train Registration API Request Model
    public class TrainRegisterRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 0;

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

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

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public decimal? Balance { get; set; } = 0;

        [JsonProperty("status")]
        public string Status { get; set; } = "ACTIVE";

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; } = true;

        [JsonProperty("verCode")]
        public string VerCode { get; set; }

        [JsonProperty("banReason")]
        public string BanReason { get; set; }

        [JsonProperty("rememberToken")]
        public string RememberToken { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // Train Registration API Response Model
    public class TrainRegisterResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TrainUserData Data { get; set; }
    }

    #endregion
}