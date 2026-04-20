using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Excel_Bus.Admin
{
    public partial class changePassword : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static changePassword()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
               // Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Session se AdminId get karo
                if (Session["AdminId"] != null)
                {
                    hfAdminId.Value = Session["AdminId"].ToString();
                }
                else
                {
                    // Agar session nahi hai to login page par redirect karo
                    Response.Redirect("~/Login.aspx");
                }
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

                int adminId = Convert.ToInt32(hfAdminId.Value);

                var requestData = new
                {
                    adminId = adminId,
                    oldPassword = txtOldPassword.Text.Trim(),
                    newPassword = txtNewPassword.Text.Trim(),
                    confirmPassword = txtConfirmPassword.Text.Trim()
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("Admins/ChangePassword", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                    if (result.Success)
                    {
                        ClearForm();

                        ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                            "showSuccess('Password changed successfully!');", true);

                        pnlSuccess.Visible = true;
                        lblSuccess.Text = result.Message;
                    }
                    else
                    {
                        ShowError(result.Message);
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var errorResult = JsonConvert.DeserializeObject<ApiResponse>(errorContent);
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
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            pnlSuccess.Visible = false;
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