using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.Admin
{
    public partial class vehicleUser : System.Web.UI.Page
    {

        string token_sess;
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                    bindlist();
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }

        protected void bindlist()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync(apiUrl + string.Format("tblVehicleUser/GetVerhicleUser")).Result;

                    if (Res.IsSuccessStatusCode)
                    {
                        var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(EmpResponse, (typeof(DataTable)));

                        if (dt.Rows.Count > 0)
                        {
                            gvAdmins.DataSource = dt;
                            gvAdmins.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }
        protected void gvAdmins_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                if (ddlStatus != null)
                {
                    // Get the current status value from the data
                    DataRowView drv = (DataRowView)e.Row.DataItem;
                    bool currentStatus = Convert.ToBoolean(drv["status"]);

                    // Set the dropdown value based on the boolean status
                    ddlStatus.SelectedValue = currentStatus ? "Active" : "Inactive";
                }
            }
        }
        public string add_vehicleuser(string name, string phoneNumber, string licenseNumber, string username, string password, string role, string dateOfJoining)
        {
            try
            {
                // Validate input parameters
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
                        DateTime.TryParseExact(dateOfJoining, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out joinDate);
                    }
                }

                var vehicleUser = new
                {
                    VehicleUserName = name.Trim(),
                    RoleId = role.Trim(),
                    MobileNo = phoneNumber.Trim(),
                    LicenseNo = licenseNumber.Trim(),
                    dateOfJoining = joinDate.ToString("yyyy-MM-dd"),
                    username = username.Trim(),
                    password = password.Trim(),
                    status = true,
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    updatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                var json = JsonConvert.SerializeObject(vehicleUser);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Sending to API: {json}");

                using (var clientLocal = new HttpClient())
                {
                    clientLocal.Timeout = TimeSpan.FromSeconds(30);
                    clientLocal.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        clientLocal.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }


                    var url = apiUrl + "tblVehicleUser/PostVehicleUser";

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
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }

                    if (response == null)
                    {
                        return $"Failed to connect to API after retries. {lastException?.Message}";
                    }

                    string result = response.Content.ReadAsStringAsync().Result;

                    System.Diagnostics.Debug.WriteLine($"API Response Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"API Response: {result}");

                    if (response.IsSuccessStatusCode)
                    {
                        bindlist();
                        return "SUCCESS";
                    }
                    else
                    {
                        // Enhanced error handling for specific constraint violations
                        string errorMsg = "";

                        if (response.StatusCode == System.Net.HttpStatusCode.Conflict ||
                            response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            // Check for constraint violation patterns in the response
                            string lowerResult = result.ToLower();

                            if (lowerResult.Contains("unique") ||
                                lowerResult.Contains("duplicate") ||
                                lowerResult.Contains("constraint") ||
                                lowerResult.Contains("violates unique constraint") ||
                                lowerResult.Contains("duplicate key") ||
                                lowerResult.Contains("already exists"))
                            {
                                // Check specific patterns for license and phone combination
                                if ((lowerResult.Contains("license") && lowerResult.Contains("phone")) ||
                                    lowerResult.Contains("licensenumber") ||
                                    lowerResult.Contains("phonenumber"))
                                {
                                    errorMsg = $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists. Please use different values.";
                                }
                                else if (lowerResult.Contains("username"))
                                {
                                    errorMsg = $"Username '{username}' already exists. Please choose a different username.";
                                }
                                else if (lowerResult.Contains("phone"))
                                {
                                    errorMsg = $"Phone Number '{phoneNumber}' is already registered. Please use a different phone number.";
                                }
                                else if (lowerResult.Contains("license"))
                                {
                                    errorMsg = $"License Number '{licenseNumber}' is already registered. Please use a different license number.";
                                }
                                else
                                {
                                    errorMsg = $"This combination of License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists. Please use different values.";
                                }
                            }
                            else if (lowerResult.Contains("internal server error") &&
                                     lowerResult.Contains("entity changes"))
                            {
                                // This is likely a constraint violation that wasn't caught above
                                errorMsg = $"A user with this License Number '{licenseNumber}' and Phone Number '{phoneNumber}' combination already exists. Please use different values.";
                            }
                            else
                            {
                                // Try to parse JSON error response
                                try
                                {
                                    var errorObj = JsonConvert.DeserializeObject<dynamic>(result);
                                    string apiError = errorObj?.message?.ToString() ??
                                                    errorObj?.title?.ToString() ??
                                                    errorObj?.error?.ToString() ??
                                                    result;

                                    // Check if the parsed error mentions constraints
                                    if (apiError.ToLower().Contains("unique") ||
                                        apiError.ToLower().Contains("duplicate") ||
                                        apiError.ToLower().Contains("constraint"))
                                    {
                                        errorMsg = $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists. Please use different values.";
                                    }
                                    else
                                    {
                                        errorMsg = $"Validation Error: {apiError}";
                                    }
                                }
                                catch
                                {
                                    // If JSON parsing fails, check for common constraint error patterns
                                    if (result.Contains("UNIQUE constraint failed") ||
                                        result.Contains("Violation of UNIQUE KEY constraint") ||
                                        result.Contains("duplicate key value violates unique constraint"))
                                    {
                                        errorMsg = $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists. Please use different values.";
                                    }
                                    else
                                    {
                                        errorMsg = $"Error: Unable to add user. Please check if the License Number and Phone Number combination is unique.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            // For other HTTP status codes
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
                System.Diagnostics.Debug.WriteLine($"Exception in add_vehicleuser: {ex.Message}");

                // Check if exception message contains constraint violation info
                string exceptionMsg = ex.Message.ToLower();
                if (exceptionMsg.Contains("unique") ||
                    exceptionMsg.Contains("duplicate") ||
                    exceptionMsg.Contains("constraint"))
                {
                    return $"A user with License Number '{licenseNumber}' and Phone Number '{phoneNumber}' already exists. Please use different values.";
                }

                return "Unable to add user. Please verify that the License Number and Phone Number combination is unique.";
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Button clickedButton = sender as Button;
                bool isFromModal = (clickedButton != null && (clickedButton.ID == "btn_add_vehicle_user"));

                string name, role, phoneNumber, licenseNumber, username, password, dateOfJoining;


                name = txtName.Text.Trim();
                role = ddlrole.SelectedValue;
                phoneNumber = txtPhoneNumber.Text.Trim();
                licenseNumber = txtLicenseNumber.Text.Trim();
                username = txtUsername.Text.Trim();
                password = txtPassword.Text.Trim();
                dateOfJoining = txtDateOfJoining.Text;

                if (string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(phoneNumber) ||
                    string.IsNullOrEmpty(licenseNumber) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(dateOfJoining))
                {
                    string alertScript = "alert('Please fill all required fields.');";
                    if (isFromModal)
                    {
                        alertScript += " $('#vehicleUserModal').modal('show');";
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                        alertScript, true);
                    return;
                }

                string result = add_vehicleuser(name, phoneNumber, licenseNumber, username, password, role, dateOfJoining);

                if (result == "SUCCESS")
                {
                    string successScript = "alert('Vehicle user added successfully');";

                    if (isFromModal)
                    {
                        successScript += @"
                            $('#vehicleUserModal').modal('hide');
                            setTimeout(function() {
                                window.location.href = window.location.href;
                            }, 1000);
                        ";
                    }
                    else
                    {
                        successScript += @"
                            setTimeout(function() {
                                window.location.href = window.location.href;
                            }, 1000);
                        ";
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "success" + DateTime.Now.Ticks,
                        successScript, true);

                    ClearControls();
                    ddlrole.SelectedValue = "Conductor";
                    txtDateOfJoining.Text = DateTime.Today.ToString("yyyy-MM-dd");

                }
                else
                {
                    string errorMessage = result == "Not Found" ? "Failed to add vehicle user. Please try again." : result;
                    string alertScript = $"alert('Error: {errorMessage.Replace("'", "\\'")}');";

                    if (isFromModal)
                    {
                        alertScript += " $('#vehicleUserModal').modal('show');";
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                        alertScript, true);
                }
            }
            catch (Exception ex)
            {
                bool isFromModal = (sender as Button)?.ID == "btn_add_vehicle_user";
                string alertScript = $"alert('An error occurred: {ex.Message.Replace("'", "\\'")}. Please try again.');";

                if (isFromModal)
                {
                    alertScript += " $('#vehicleUserModal').modal('show');";
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert" + DateTime.Now.Ticks,
                    alertScript, true);
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
                lblResult.Text = "";
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }
        }


    }
}