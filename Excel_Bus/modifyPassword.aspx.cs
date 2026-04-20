using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Excel_Bus
{
    public partial class modifyPassword : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static modifyPassword()
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
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/UserLogin.aspx", false);
                    return;
                }

                hfUserId.Value = Session["UserId"].ToString();
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await ChangePasswordAsync();
            }));
        }

        private async Task ChangePasswordAsync()
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                int userId = Convert.ToInt32(hfUserId.Value);

                var requestData = new
                {
                    userId = userId,
                    oldPassword = txtOldPassword.Text.Trim(),
                    newPassword = txtNewPassword.Text.Trim(),
                    confirmPassword = txtConfirmPassword.Text.Trim()
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"=== CHANGE PASSWORD REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"User ID: {userId}");
                System.Diagnostics.Debug.WriteLine($"API URL: {apiUrl}Users/ChangePassword");
                System.Diagnostics.Debug.WriteLine($"Payload: {json}");

                HttpResponseMessage response = await client.PostAsync("Users/ChangePassword", content);

                string responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                    if (result.Success)
                    {
                        ClearForm();

                        // Show success message using ScriptManager
                        string successMsg = result.Message ?? "Password changed successfully!";

                        pnlSuccess.Visible = true;
                        lblSuccess.Text = successMsg;

                        // Trigger iziToast notification
                        string script = $"notify('success', '{successMsg}');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "SuccessMessage", script, true);
                    }
                    else
                    {
                        ShowError(result.Message ?? "Failed to change password");
                    }
                }
                else
                {
                    try
                    {
                        var errorResult = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                        ShowError(errorResult.Message ?? "Failed to change password");
                    }
                    catch
                    {
                        ShowError($"Failed to change password. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;

            // Trigger iziToast notification for error
            string script = $"notify('error', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", script, true);
        }

        private void ClearForm()
        {
            txtOldPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
        }
    }

    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}