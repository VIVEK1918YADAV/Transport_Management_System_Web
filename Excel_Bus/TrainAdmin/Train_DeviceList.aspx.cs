using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_DeviceList : System.Web.UI.Page
    {
        string token_sess;
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    BindDeviceList();
                }
            }

            catch (Exception ex)
            {
                ShowError("Error loading page: " + ex.Message);
            }
        }
        private void BindDeviceList()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    // Use the new unified API endpoint
                    string url = apiUrl + "TrainDeviceInformations/GetTrainDeviceInformations";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(content))
                        {
                            var devices = JsonConvert.DeserializeObject<List<TrainDeviceData>>(content);

                            if (devices != null && devices.Count > 0)
                            {
                                // Sort devices by id (or srno if you prefer)
                                devices = devices.OrderBy(d => d.deviceId).ToList();

                                // Convert to DataTable for GridView binding
                                DataTable dt = ConvertToDataTable(devices);
                                                                
                                gvDeviceList.DataSource = dt;
                                gvDeviceList.DataBind();
                                lblError.Visible = false;
                            }
                            else
                            {
                                gvDeviceList.DataSource = null;
                                gvDeviceList.DataBind();
                                ShowError("No devices found.");
                            }
                        }
                        else
                        {
                            gvDeviceList.DataSource = null;
                            gvDeviceList.DataBind();
                            ShowError("No devices found.");
                        }
                    }
                    else
                    {
                        ShowError($"Error loading device list: API returned status code {response.StatusCode}");
                        gvDeviceList.DataSource = null;
                        gvDeviceList.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading device list: " + ex.Message);
                gvDeviceList.DataSource = null;
                gvDeviceList.DataBind();
            }
        }

        private DataTable ConvertToDataTable(List<TrainDeviceData> devices)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("deviceId", typeof(string));
            dt.Columns.Add("deviceName", typeof(string));
            dt.Columns.Add("deviceImei1", typeof(string));
            dt.Columns.Add("deviceImei2", typeof(string));
            dt.Columns.Add("modelNo", typeof(string));
            dt.Columns.Add("companyName", typeof(string));
            dt.Columns.Add("versionNo", typeof(string));
            dt.Columns.Add("isActive", typeof(bool));
            dt.Columns.Add("createdAt", typeof(string));
            dt.Columns.Add("createdBy", typeof(string));
            dt.Columns.Add("updatedAt", typeof(string));
            dt.Columns.Add("updatedBy", typeof(string));

            foreach (var device in devices)
            {
                DataRow row = dt.NewRow();
                row["deviceId"] = device.deviceId ?? "";
                row["deviceName"] = device.deviceName ?? "";
                row["deviceImei1"] = device.deviceImei1 ?? "";
                row["deviceImei2"] = device.deviceImei2 ?? "";
                row["modelNo"] = device.modelNo ?? "";
                row["companyName"] = device.companyName ?? "";
                row["versionNo"] = device.versionNo ?? "";
                row["isActive"] = device.isActive;
                row["createdAt"] = device.createdAt ?? "";
                row["createdBy"] = device.createdBy ?? "";
                row["updatedAt"] = device.updatedAt ?? "";
                row["updatedBy"] = device.updatedBy ?? "";
                dt.Rows.Add(row);
            }
            return dt;
        }

        protected void gvDeviceList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlIsActive = (DropDownList)e.Row.FindControl("ddlIsActive");
                if (ddlIsActive != null)
                {
                    bool currentStatus = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "isActive"));
                    ddlIsActive.SelectedValue = currentStatus.ToString().ToLower();

                    // Store all device data in Session for easy access during update
                    DataRowView drv = (DataRowView)e.Row.DataItem;
                    //string id = drv["deviceId"].ToString();
                    string deviceId = drv["deviceId"]?.ToString() ?? "";

                    var deviceData = new TrainDeviceData
                    {
                        deviceId = drv["deviceId"].ToString(),
                        deviceName = drv["deviceName"]?.ToString() ?? "",
                        deviceImei1 = drv["deviceImei1"]?.ToString() ?? "",
                        deviceImei2 = drv["deviceImei2"]?.ToString() ?? "",
                        modelNo = drv["modelNo"]?.ToString() ?? "",
                        companyName = drv["companyName"]?.ToString() ?? "",
                        versionNo = drv["versionNo"]?.ToString() ?? "",
                        isActive = Convert.ToBoolean(drv["isActive"]),
                        createdAt = drv["createdAt"]?.ToString() ?? "",
                        createdBy = drv["createdBy"]?.ToString() ?? "",
                        updatedAt = drv["updatedAt"]?.ToString() ?? "",
                        updatedBy = drv["updatedBy"]?.ToString() ?? ""
                    };

                    // Store in Session
                    if (Session["DeviceData"] == null)
                        Session["DeviceData"] = new Dictionary<string, TrainDeviceData>();

                    ((Dictionary<string, TrainDeviceData>)Session["DeviceData"])[deviceId] = deviceData;

                    // Store deviceId in the dropdown's Attributes for easy access
                    ddlIsActive.Attributes["data-device-id"] = deviceId;

                    // Add client-side confirmation
                    string script = $"handleStatusChange(this, '{currentStatus.ToString().ToLower()}')";
                    ddlIsActive.Attributes.Add("onchange", $"if(!{script}) {{ this.value = '{currentStatus.ToString().ToLower()}'; return false; }}");
                }
            }
        }

        private bool UpdateDeviceStatus(string deviceId, bool newStatus)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                System.Diagnostics.Debug.WriteLine("UpdateDeviceStatus called with empty deviceId!");
                return false;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token_sess))
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token_sess);

                    string url = apiUrl.TrimEnd('/') + "/TrainDeviceInformations/ToggleTrainDeviceStatus/ToggleTrainDeviceStatus";

                    var requestData = new { DeviceId = deviceId };
                    var json = JsonConvert.SerializeObject(requestData);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    System.Diagnostics.Debug.WriteLine($"POST to: {url}");
                    System.Diagnostics.Debug.WriteLine($"Body: {json}");

                    HttpResponseMessage response = client.PostAsync(url, data).Result;

                    System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response: {response.Content.ReadAsStringAsync().Result}");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }


        //private void UpdateRelatedAssignmentStatusForDevice(string deviceId)
        //{
        //    try
        //    {
        //        var assignmentsToUpdate = GetAssignmentsByDeviceId(deviceId);

        //        foreach (var assignment in assignmentsToUpdate)
        //        {
        //            CallCheckAndUpdateStatusAPI(assignment.assignId.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error updating related assignment status for device: {ex.Message}");
        //    }
        //}

        //private List<dynamic> GetAssignmentsByDeviceId(string deviceId)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            if (!string.IsNullOrEmpty(token_sess))
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
        //            }

        //            string url = apiUrl + "TblBusAssigns/GetTblBusAssigns";
        //            HttpResponseMessage response = client.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var responseContent = response.Content.ReadAsStringAsync().Result;
        //                var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);

        //                return allAssignments.Where(a => a.posid != null && a.posid.ToString() == deviceId).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error getting assignments by device id: {ex.Message}");
        //    }

        //    return new List<dynamic>();
        //}

        //private bool CallCheckAndUpdateStatusAPI(string assignId)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            if (!string.IsNullOrEmpty(token_sess))
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
        //            }

        //            var requestData = new
        //            {
        //                assignId = int.Parse(assignId)
        //            };

        //            var json = JsonConvert.SerializeObject(requestData);
        //            var data = new StringContent(json, Encoding.UTF8, "application/json");

        //            string url = apiUrl + "TblBusAssigns/checkandupdatestatus";
        //            HttpResponseMessage response = client.PostAsync(url, data).Result;

        //            System.Diagnostics.Debug.WriteLine($"Assignment {assignId} status update result: {response.StatusCode}");

        //            return response.IsSuccessStatusCode;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error calling check and update status API: {ex.Message}");
        //        return false;
        //    }
        //}

        protected void ddlIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.NamingContainer;
                int rowIndex = row.RowIndex;

                string id = gvDeviceList.DataKeys[rowIndex].Value.ToString();
                bool newStatus = Convert.ToBoolean(ddl.SelectedValue);

                // Get deviceId from Session
                string deviceId = "";
                if (Session["DeviceData"] != null)
                {
                    var deviceDataDict = (Dictionary<string, TrainDeviceData>)Session["DeviceData"];
                    if (deviceDataDict.ContainsKey(id))
                    {
                        deviceId = deviceDataDict[id].deviceId;
                    }
                }

                // Debug information
                System.Diagnostics.Debug.WriteLine($"Updating Device ID: {id}, DeviceId: {deviceId} to IsActive: {newStatus}");

               // Pass deviceId to UpdateDeviceStatus
                bool updateResult = UpdateDeviceStatus(deviceId, newStatus);

                if (updateResult)
                {
                    string statusText = newStatus ? "Enabled" : "Disabled";
                    ShowSuccess($"Device status updated to {statusText} successfully!");

                    // Update related assignment statuses
                    //UpdateRelatedAssignmentStatusForDevice(id);

                    // Small delay before refresh to ensure API update is complete
                    System.Threading.Thread.Sleep(500);
                    BindDeviceList(); // Refresh the grid
                }
                else
                {
                    ShowError("Failed to update device status. Please try again.");
                    // Revert dropdown selection on failure
                    BindDeviceList();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error updating device status: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Exception in ddlIsActive_SelectedIndexChanged: " + ex.ToString());
                BindDeviceList(); // Refresh to revert changes
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.CssClass = "error-message";
            lblError.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.CssClass = "success-message";
            lblSuccess.Visible = true;
            lblError.Visible = false;
        }

    }
        public class TrainDeviceData
        {
        public string deviceId { get; set; }
        public string deviceName { get; set; }
            public string deviceImei1 { get; set; }
            public string deviceImei2 { get; set; }
            public string modelNo { get; set; }
            public string companyName { get; set; }
            public string versionNo { get; set; }
            public bool isActive { get; set; }
            public string createdAt { get; set; }
            public string createdBy { get; set; }
            public string updatedAt { get; set; }
            public string updatedBy { get; set; }
        }
    
}