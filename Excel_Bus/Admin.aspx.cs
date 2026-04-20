using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExcelBus
{
    public partial class Admin : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(BindRoles));
                if (Session["AdminId"] == null)
                    return;

                string adminId = Session["AdminId"].ToString();

                if (adminId == "1")
                    Response.Redirect("/Admin/AdminDashboard.aspx", false);
                else if (adminId == "8")
                    Response.Redirect("/TrainAdmin/Train_FleetType.aspx", false);
            }
        }

        private async Task BindRoles()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string requestUrl = apiUrl.TrimEnd('/') + "/tblRoleMaster/GetRoles";

                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    ddlRole.Items.Clear();
                    ddlRole.Items.Add(new ListItem("Select Role", "0"));

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var roles = JsonConvert.DeserializeObject<List<Role>>(json);

                        if (roles != null && roles.Count > 0)
                        {
                            ddlRole.DataSource = roles;
                            ddlRole.DataTextField = "RoleName";
                            ddlRole.DataValueField = "RoleId";
                            ddlRole.DataBind();
                        }
                        else
                        {
                            ddlRole.Items.Add(new ListItem("No roles found", "-1"));
                        }
                    }
                    else
                    {
                        ddlRole.Items.Add(new ListItem("Failed to load roles", "-1"));
                    }
                }
            }
            catch (Exception ex)
            {
                ddlRole.Items.Clear();
                ddlRole.Items.Add(new ListItem("Error loading roles", "-1"));
                System.Diagnostics.Debug.WriteLine($"BindRoles Exception: {ex.Message}");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(LoginAsync));
        }

        private async Task LoginAsync()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            int roleId = int.Parse(ddlRole.SelectedValue);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || roleId <= 0)
            {
                ShowError("Please enter username, password and select a role.");
                return;
            }

            try
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var loginRequest = new
                {
                    Username = username,
                    Password = password,
                    RoleId = roleId
                };

                string jsonRequest = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                string endpoint = "Admins/Login";
                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    ShowError("Username and password not correct. Please try again.");
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    return;
                }

                string jsonResult = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Response: {jsonResult}");

                var result = JsonConvert.DeserializeObject<ApiResponse>(jsonResult);

                if (result != null && result.Success)
                {
                    Session["AdminId"] = result.Data.Id;
                    Session["AdminName"] = result.Data.Name;
                    Session["AdminUsername"] = result.Data.Username;
                    Session["RoleId"] = result.Data.RoleId;
                    Session["AdminEmail"] = result.Data.Email;
                    Session["AdminImage"] = result.Data.Image;
                    Session["AdminStatus"] = result.Data.Status;
                    Session["LoginTime"] = DateTime.Now;
                    Session["IsAuthenticated"] = true;

                    string successMsg = result.Message ?? "Login successful";
                    ScriptManager.RegisterStartupScript(this, GetType(), "LoginSuccess",
                        $"notify('success', '{EscapeJavaScript(successMsg)} Redirecting...');", true);

                    int userRoleId = result.Data.RoleId;

                    if (userRoleId == 1)
                    {
                        Response.Redirect("/Admin/AdminDashboard.aspx", false);
                    }
                    else if (userRoleId == 2)
                    {
                        Response.Redirect("/Admin/ws_dashboard.aspx", false);
                    }
                    else if (userRoleId == 3)
                    {
                        Response.Redirect("/Admin/it_dashboard.aspx", false);
                    }
                    else if (userRoleId == 8)
                    {
                        Response.Redirect("/TrainAdmin/Train_FleetType.aspx", false); // ✅ FIXED
                    }
                    else
                    {
                        Response.Redirect("/Home.aspx", false);
                    }
                }
                else
                {
                    string errorMsg = result?.Message ?? "Login failed!";
                    ShowError(errorMsg);
                }
            }
            catch (HttpRequestException ex)
            {
                ShowError("Unable to connect to the server. Please check your internet connection.");
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError("An unexpected error occurred. Please try again.");
                System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;

            ScriptManager.RegisterStartupScript(this, GetType(), "LoginError",
                $"notify('error', '{EscapeJavaScript(message)}');", true);
        }

        private string EscapeJavaScript(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Replace("'", "\\'")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r");
        }

        public class ApiResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public AdminData Data { get; set; }
        }

        public class Role
        {
            [JsonProperty("RoleId")]
            public int RoleId { get; set; }

            [JsonProperty("RoleName")]
            public string RoleName { get; set; }
        }

        public class AdminData
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("roleId")]
            public int RoleId { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("emailVerifiedAt")]
            public string EmailVerifiedAt { get; set; }

            [JsonProperty("image")]
            public string Image { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("createdAt")]
            public string CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public string UpdatedAt { get; set; }
        }
    }
}