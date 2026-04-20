using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_User : System.Web.UI.Page
    {
        string token_sess;
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    txtDateOfJoining.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    BindList();
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }

        
        protected void BindList()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage res = client.GetAsync(apiUrl + "TrainVehicleUserRegs/GetTrainVehicleUser").Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var empResponse = res.Content.ReadAsStringAsync().Result;
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(empResponse, typeof(DataTable));

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            gvTrainUsers.DataSource = dt;
                            gvTrainUsers.DataBind();
                        }
                        else
                        {
                            gvTrainUsers.DataSource = null;
                            gvTrainUsers.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }

        protected void gvTrainUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                if (ddlStatus != null)
                {
                    DataRowView drv = (DataRowView)e.Row.DataItem;

                    // Handle both "isActive" and "status" field names
                    bool currentStatus = false;
                    if (drv.Row.Table.Columns.Contains("isActive"))
                        currentStatus = Convert.ToBoolean(drv["isActive"]);
                    else if (drv.Row.Table.Columns.Contains("status"))
                        currentStatus = Convert.ToBoolean(drv["status"]);

                    ddlStatus.SelectedValue = currentStatus ? "Active" : "Inactive";
                }
            }
        }

        public string AddTrainVehicleUser(string name, string phoneNumber, string licenseNumber,
            string username, string password, string role, string dateOfJoining)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phoneNumber) ||
                    string.IsNullOrEmpty(licenseNumber) || string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password))
                {
                    return "Missing required fields";
                }

                DateTime joinDate = DateTime.Now;
                if (!string.IsNullOrEmpty(dateOfJoining))
                {
                    if (!DateTime.TryParse(dateOfJoining, out joinDate))
                    {
                        DateTime.TryParseExact(dateOfJoining, "yyyy-MM-dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out joinDate);
                    }
                }

                var trainUser = new
                {
                    TrainUserName = name.Trim(),
                    RoleId = int.Parse(role.Trim()),
                    MobileNo = phoneNumber.Trim(),
                    LicenseNo = licenseNumber.Trim(),
                    DateOfJoining = joinDate.ToString("yyyy-MM-dd"),
                    Username = username.Trim(),
                    Password = password.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    UpdatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                var json = JsonConvert.SerializeObject(trainUser);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Sending to API: {json}");

                using (var clientLocal = new HttpClient())
                {
                    clientLocal.Timeout = TimeSpan.FromSeconds(30);
                    clientLocal.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        clientLocal.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    var url = apiUrl + "TrainVehicleUserRegs/PostTrainVehicleUser";

                    HttpResponseMessage response = null;
                    Exception lastException = null;

                    for (int retry = 0; retry < 2; retry++)
                    {
                        try
                        {
                            response = clientLocal.PostAsync(url, data).Result;
                            break;
                        }
                        catch (Exception ex)
                        {
                            lastException = ex;
                            if (retry == 0)
                                System.Threading.Thread.Sleep(1000);
                        }
                    }

                    if (response == null)
                        return $"Failed to connect to API after retries. {lastException?.Message}";

                    string result = response.Content.ReadAsStringAsync().Result;

                    System.Diagnostics.Debug.WriteLine($"API Response Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"API Response: {result}");

                    if (response.IsSuccessStatusCode)
                    {
                        BindList();
                        return "SUCCESS";
                    }
                    else
                    {
                        string errorMsg = "";
                        string lowerResult = result.ToLower();

                        if (response.StatusCode == System.Net.HttpStatusCode.Conflict ||
                            response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            if (lowerResult.Contains("unique") || lowerResult.Contains("duplicate") ||
                                lowerResult.Contains("constraint") || lowerResult.Contains("already exists"))
                            {
                                if (lowerResult.Contains("username"))
                                    errorMsg = $"Username '{username}' already exists. Please choose a different username.";
                                else if (lowerResult.Contains("phone") || lowerResult.Contains("mobile"))
                                    errorMsg = $"Phone Number '{phoneNumber}' is already registered.";
                                else if (lowerResult.Contains("license"))
                                    errorMsg = $"License Number '{licenseNumber}' is already registered.";
                                else
                                    errorMsg = $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists.";
                            }
                            else
                            {
                                try
                                {
                                    var errorObj = JsonConvert.DeserializeObject<dynamic>(result);
                                    string apiError = errorObj?.message?.ToString() ??
                                                     errorObj?.title?.ToString() ??
                                                     errorObj?.error?.ToString() ?? result;
                                    errorMsg = $"Validation Error: {apiError}";
                                }
                                catch
                                {
                                    errorMsg = "Error: Unable to add train vehicle user. Please check your inputs.";
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var errorObj = JsonConvert.DeserializeObject<dynamic>(result);
                                errorMsg = errorObj?.message?.ToString() ?? errorObj?.title?.ToString() ?? result;
                            }
                            catch
                            {
                                errorMsg = $"Error ({response.StatusCode}): {result}";
                            }
                        }

                        return errorMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in AddTrainVehicleUser: {ex.Message}");

                string exMsg = ex.Message.ToLower();
                if (exMsg.Contains("unique") || exMsg.Contains("duplicate") || exMsg.Contains("constraint"))
                    return $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists.";

                return "Unable to add train vehicle user. Please verify your inputs and try again.";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string role = ddlRole.SelectedValue;
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string licenseNumber = txtLicenseNumber.Text.Trim();
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();
                string dateOfJoining = txtDateOfJoining.Text;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phoneNumber) ||
                    string.IsNullOrEmpty(licenseNumber) || string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dateOfJoining))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                        "alert('Please fill all required fields.');", true);
                    return;
                }

                string result = AddTrainVehicleUser(name, phoneNumber, licenseNumber,
                    username, password, role, dateOfJoining);

                if (result == "SUCCESS")
                {
                    string successScript = @"alert('Train vehicle user added successfully!');
                        setTimeout(function() { window.location.href = window.location.href; }, 1000);";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "success" + DateTime.Now.Ticks,
                        successScript, true);

                    ClearControls();
                }
                else
                {
                    string errorMessage = result == "Not Found"
                        ? "Failed to add train vehicle user. Please try again."
                        : result;

                    string alertScript = $"alert('Error: {errorMessage.Replace("'", "\\'")}');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                        alertScript, true);
                }
            }
            catch (Exception ex)
            {
                string alertScript = $"alert('An error occurred: {ex.Message.Replace("'", "\\'")}. Please try again.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                    alertScript, true);
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlStatus = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddlStatus.NamingContainer;

                Label lblTrainUserId = (Label)row.FindControl("lblTrainUserId");

                if (lblTrainUserId == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "err" + DateTime.Now.Ticks,
                        "alert('Could not find user ID. Please refresh the page.');", true);
                    return;
                }

                string trainUserId = lblTrainUserId.Text.Trim();
                bool newStatus = ddlStatus.SelectedValue == "Active";

                string result = UpdateTrainUserStatus(trainUserId, newStatus);

                if (result != "SUCCESS")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "err" + DateTime.Now.Ticks,
                        $"alert('Status update failed: {result.Replace("'", "\\'")}');", true);
                }

                BindList();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ex" + DateTime.Now.Ticks,
                    $"alert('An error occurred: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        public string UpdateTrainUserStatus(string trainUserId, bool newStatus)
        {
            try
            {
                var payload = new
                {
                    TrainUserId = trainUserId,
                    IsActive = newStatus
                };

                var json = JsonConvert.SerializeObject(payload);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var clientLocal = new HttpClient())
                {
                    clientLocal.Timeout = TimeSpan.FromSeconds(30);
                    clientLocal.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token_sess))
                        clientLocal.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token_sess);

                    var url = apiUrl + "TrainVehicleUserRegs/ToggleTrainVehicleUserStatus";
                    HttpResponseMessage response = clientLocal.PostAsync(url, data).Result;

                    string result = response.Content.ReadAsStringAsync().Result;
                    System.Diagnostics.Debug.WriteLine($"ToggleStatus Response: {response.StatusCode} - {result}");

                    if (response.IsSuccessStatusCode)
                        return "SUCCESS";
                    else
                        return $"API Error ({response.StatusCode}): {result}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }


        protected void ClearControls()
        {
            try
            {
                txtName.Text = "";
                txtPhoneNumber.Text = "";
                txtLicenseNumber.Text = "";
                txtUsername.Text = "";
                txtPassword.Text = "";
                txtDateOfJoining.Text = DateTime.Today.ToString("yyyy-MM-dd");
                ddlRole.SelectedValue = "7"; // Default: Loco Inspector
                lblResult.Text = "";
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }
    }
}