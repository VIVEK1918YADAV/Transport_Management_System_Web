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
    public partial class UserLogin : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    Response.Redirect("~/Dashboard.aspx", false);
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

                System.Diagnostics.Debug.WriteLine($"Attempting login for user: {username}");

                var result = await CallLoginAPI(username, password);

                System.Diagnostics.Debug.WriteLine($"Login result - Success: {result?.Success}, Message: {result?.Message}");

                if (result != null && result.Success)
                {
                    // Store user data in session
                    if (result.Data != null)
                    {
                        Session["UserId"] = result.Data.Id;
                        Session["Username"] = result.Data.Username;
                        Session["Email"] = result.Data.Email;
                        Session["Firstname"] = result.Data.Firstname;
                        Session["Lastname"] = result.Data.Lastname;
                        Session["FullName"] = $"{result.Data.Firstname} {result.Data.Lastname}".Trim();
                        Session["Mobile"] = result.Data.Mobile;
                        Session["Balance"] = result.Data.Balance;
                        Session["UserData"] = JsonConvert.SerializeObject(result.Data);

                        System.Diagnostics.Debug.WriteLine($"Session data stored for user ID: {result.Data.Id}");
                    }

                    ShowMessage("Login successful! Redirecting to dashboard...", "success");

                    // Redirect after a short delay to allow message to display
                    string redirectScript = @"
                        <script type='text/javascript'>
                            setTimeout(function(){ 
                                window.location.href='Dashboard.aspx'; 
                            }, 1500);
                        </script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "RedirectToDashboard", redirectScript, false);
                }
                else
                {
                    // Show the error message from API or default message
                    string errorMessage = !string.IsNullOrEmpty(result?.Message)
                        ? result.Message
                        : "Invalid username or password.";

                    System.Diagnostics.Debug.WriteLine($"Login failed with message: {errorMessage}");
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
                string password = txtPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                // Server-side validation
                if (string.IsNullOrEmpty(firstname))
                {
                    ShowMessage("First name is required.", "warning");
                    hdnIsRegister.Value = "true"; // Keep form in register mode
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

                if (string.IsNullOrEmpty(password))
                {
                    ShowMessage("Password is required.", "warning");
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

                System.Diagnostics.Debug.WriteLine($"Attempting registration for user: {username}");

                var registerData = new RegisterRequest
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Username = username,
                    Email = email,
                    Password = password,
                    DialCode = "+91",
                    Mobile = "",
                    CountryName = "India",
                    CountryCode = "IN"
                };

                var result = await CallRegisterAPI(registerData);

                System.Diagnostics.Debug.WriteLine($"Registration result - Success: {result?.Success}, Message: {result?.Message}");

                if (result != null && result.Success)
                {
                    ShowMessage("Registration successful! Please login.", "success");

                    // Clear registration fields and switch to login mode
                    txtFirstname.Text = "";
                    txtLastname.Text = "";
                    txtEmail.Text = "";
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
                                window.location.href='UserLogin.aspx'; 
                            }, 1500);
                        </script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "RedirectToDashboard", redirectScript, false);
                }
                else
                {
                    string errorMessage = !string.IsNullOrEmpty(result?.Message)
                        ? result.Message
                        : "Registration failed. Please try again.";

                    System.Diagnostics.Debug.WriteLine($"Registration failed with message: {errorMessage}");
                    ShowMessage(errorMessage, "error");
                    hdnIsRegister.Value = "true"; // Keep form in register mode
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

        private async Task<LoginResponse> CallLoginAPI(string username, string password)
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

                System.Diagnostics.Debug.WriteLine($"Login Request: {jsonContent}");

                HttpResponseMessage response = await client.PostAsync("Users/Login", content);
                string jsonResult = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Login API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Login API Response: {jsonResult}");

                // Parse the response regardless of status code
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<LoginResponse>(jsonResult);
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                        return new LoginResponse
                        {
                            Success = false,
                            Message = "Invalid response from server."
                        };
                    }
                }

                return new LoginResponse
                {
                    Success = false,
                    Message = "No response from server."
                };
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return new LoginResponse
                {
                    Success = false,
                    Message = "Unable to connect to server. Please check your connection."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login API Error: {ex.Message}");
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred while connecting to the server."
                };
            }
        }

        private async Task<RegisterResponse> CallRegisterAPI(RegisterRequest registerData)
        {
            try
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);

                string endpoint = "Users/PostUser";

                string jsonContent = JsonConvert.SerializeObject(registerData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Registration Request: {jsonContent}");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);
                string jsonResult = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Register API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Register API Response: {jsonResult}");

                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<RegisterResponse>(jsonResult);
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                        return new RegisterResponse
                        {
                            Success = false,
                            Message = "Invalid response from server."
                        };
                    }
                }

                return new RegisterResponse
                {
                    Success = false,
                    Message = "No response from server."
                };
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Unable to connect to server. Please check your connection."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register API Error: {ex.Message}");
                return new RegisterResponse
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

    // Login API Response Models
    public class LoginResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public UserData Data { get; set; }
    }

    public class UserData
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

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("dialCode")]
        public string DialCode { get; set; }

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

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("ev")]
        public int Ev { get; set; }

        [JsonProperty("sv")]
        public int Sv { get; set; }

        [JsonProperty("ts")]
        public int Ts { get; set; }

        [JsonProperty("tv")]
        public int Tv { get; set; }

        [JsonProperty("profileComplete")]
        public int ProfileComplete { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    // Registration API Request Model
    public class RegisterRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 0;

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
        public int RefBy { get; set; } = 0;

        [JsonProperty("balance")]
        public decimal Balance { get; set; } = 0;

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
        public string Status { get; set; } = "1";

        [JsonProperty("ev")]
        public int Ev { get; set; } = 0;

        [JsonProperty("sv")]
        public int Sv { get; set; } = 0;

        [JsonProperty("profileComplete")]
        public int ProfileComplete { get; set; } = 0;

        [JsonProperty("verCode")]
        public string VerCode { get; set; }

        [JsonProperty("verCodeSendAt")]
        public DateTime? VerCodeSendAt { get; set; }

        [JsonProperty("ts")]
        public int Ts { get; set; } = 0;

        [JsonProperty("tv")]
        public int Tv { get; set; } = 1;

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
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // Registration API Response Model
    public class RegisterResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public UserData Data { get; set; }
    }
}