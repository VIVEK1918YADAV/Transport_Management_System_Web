using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class assign_details : System.Web.UI.Page
    {
        string token_sess;
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        private List<dynamic> currentAssignments = new List<dynamic>();
        private System.Timers.Timer resourceCheckTimer;

        protected void Page_Load(object sender, EventArgs e)
        {
            token_sess = Session["token"] != null ? Session["token"].ToString() : "";

            if (!Page.IsPostBack)
            {
                StartResourceCheckTimer();
                 Task.Run(() => CheckResourceAvailability(this, null));
                LoadCurrentAssignments();
                LoadDropdowns();
                BindAssignments();
                
            }
        }

        private void LoadCurrentAssignments()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblAssignDetail/GetTblAssignDetails").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        // Only get ACTIVE assignments
                        currentAssignments = allAssignments
                            .Where(a => a.status != null && a.status.ToString().ToUpper() == "ACTIVE")
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"=== Active assignments loaded: {currentAssignments.Count} ===");

                        // Debug: Print all active assignments
                        foreach (var assign in currentAssignments)
                        {
                            System.Diagnostics.Debug.WriteLine($"Assignment {assign.assignId}: Vehicle={assign.vehicleId}, Device={assign.deviceId}, Driver={assign.driverId}, Conductor={assign.conductorId}");
                        }
                    }
                    else
                    {
                        currentAssignments = new List<dynamic>();
                        System.Diagnostics.Debug.WriteLine($"Failed to load assignments. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                currentAssignments = new List<dynamic>();
                System.Diagnostics.Debug.WriteLine($"Error loading assignments: {ex.Message}");
            }
        }

        private void LoadDropdowns()
        {
            LoadRoutes();
            LoadVehicles();
            LoadDrivers();
            LoadConductors();
            LoadDevices();
        }

        private void LoadRoutes()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "VehicleRoutes/GetActiveVehicleRoutes").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlRoutes.Items.Clear();
                        ddlRoutes.Items.Add(new ListItem("Select Route", ""));

                        foreach (var route in routes)
                        {
                            if (route.id != null && route.name != null)
                            {
                                string routeId = route.id.ToString();
                                string routeName = route.name.ToString();
                                string startFrom = route.startFrom?.ToString() ?? "";
                                string endTo = route.endTo?.ToString() ?? "";

                                string displayText = $"{routeName} ({startFrom} - {endTo})";
                                ddlRoutes.Items.Add(new ListItem(displayText, routeId));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading routes: " + ex.Message, false);
            }
        }

        private void LoadVehicles()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "Vehicles/GetActiveVehicles").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var vehicles = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlBuses.Items.Clear();
                        ddlBuses.Items.Add(new ListItem("Select Vehicle", ""));

                        var assignedVehicleIds = currentAssignments
                            .Where(a => a.vehicleId != null)
                            .Select(a => a.vehicleId.ToString())
                            .ToHashSet();

                        foreach (var vehicle in vehicles)
                        {
                            if (vehicle.id != null)
                            {
                                string vehicleId = vehicle.id.ToString();
                                string nickName = vehicle.nickName?.ToString() ?? "";
                                string registerNo = vehicle.registerNo?.ToString() ?? "";

                                if (!assignedVehicleIds.Contains(vehicleId))
                                {
                                    string displayText = !string.IsNullOrEmpty(nickName)
                                        ? $"{registerNo} - {nickName}"
                                        : registerNo;

                                    ddlBuses.Items.Add(new ListItem(displayText, vehicleId));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading vehicles: " + ex.Message, false);
            }
        }

        private void LoadDrivers()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveDrivers").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlDrivers.Items.Clear();
                        ddlDrivers.Items.Add(new ListItem("Select Driver", ""));

                        var assignedDriverIds = currentAssignments
                            .Where(a => a.driverId != null)
                            .Select(a => a.driverId.ToString())
                            .ToHashSet();

                        foreach (var driver in drivers)
                        {
                            if (driver.vehicleUserId != null && driver.vehicleUserName != null)
                            {
                                string driverId = driver.vehicleUserId.ToString();
                                string driverName = driver.vehicleUserName.ToString();

                                if (!assignedDriverIds.Contains(driverId))
                                {
                                    ddlDrivers.Items.Add(new ListItem(driverName, driverId));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading drivers: " + ex.Message, false);
            }
        }

        private void LoadConductors()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveConductors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var conductors = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlUsers.Items.Clear();
                        ddlUsers.Items.Add(new ListItem("Select Conductor", ""));

                        var assignedConductorIds = currentAssignments
                            .Where(a => a.conductorId != null)
                            .Select(a => a.conductorId.ToString())
                            .ToHashSet();

                        foreach (var conductor in conductors)
                        {
                            if (conductor.vehicleUserId != null && conductor.vehicleUserName != null)
                            {
                                string conductorId = conductor.vehicleUserId.ToString();
                                string conductorName = conductor.vehicleUserName.ToString();

                                if (!assignedConductorIds.Contains(conductorId))
                                {
                                    ddlUsers.Items.Add(new ListItem(conductorName, conductorId));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading conductors: " + ex.Message, false);
            }
        }

        private void LoadDevices()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblDeviceInformations/GetActiveDevices").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var devices = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlPos.Items.Clear();
                        ddlPos.Items.Add(new ListItem("Select Device", ""));

                        var assignedDeviceIds = currentAssignments
                            .Where(a => a.deviceId != null)
                            .Select(a => a.deviceId.ToString())
                            .ToHashSet();

                        foreach (var device in devices)
                        {
                            if (device.deviceId != null)
                            {
                                string deviceId = device.deviceId.ToString();
                                string deviceName = device.deviceName?.ToString() ?? "";

                                if (!assignedDeviceIds.Contains(deviceId))
                                {
                                    string displayText = !string.IsNullOrEmpty(deviceName)
                                        ? $"{deviceId} - {deviceName}"
                                        : deviceId;

                                    ddlPos.Items.Add(new ListItem(displayText, deviceId));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading devices: " + ex.Message, false);
            }
        }

        private void LoadEditDropdowns(string currentAssignId = null)
        {
            LoadEditRoutes();
            LoadEditVehicles(currentAssignId);
            LoadEditDrivers(currentAssignId);
            LoadEditConductors(currentAssignId);
            LoadEditDevices(currentAssignId);
        }

        private void LoadEditRoutes()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "VehicleRoutes/GetActiveVehicleRoutes").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditRoutes.Items.Clear();
                        ddlEditRoutes.Items.Add(new ListItem("Select Route", ""));

                        foreach (var route in routes)
                        {
                            if (route.id != null && route.name != null)
                            {
                                string routeId = route.id.ToString();
                                string routeName = route.name.ToString();
                                string startFrom = route.startFrom?.ToString() ?? "";
                                string endTo = route.endTo?.ToString() ?? "";

                                string displayText = $"{routeName} ({startFrom} - {endTo})";
                                ddlEditRoutes.Items.Add(new ListItem(displayText, routeId));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadEditVehicles(string currentAssignId = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "Vehicles/GetActiveVehicles").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var vehicles = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditBuses.Items.Clear();
                        ddlEditBuses.Items.Add(new ListItem("Select Vehicle", ""));

                        // Get vehicle assigned to OTHER assignments (not current one)
                        var assignedVehicleIds = currentAssignments
                            .Where(a => a.vehicleId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.assignId.ToString() != currentAssignId))
                            .Select(a => a.vehicleId.ToString())
                            .ToHashSet();

                        foreach (var vehicle in vehicles)
                        {
                            if (vehicle.id != null)
                            {
                                string vehicleId = vehicle.id.ToString();
                                string nickName = vehicle.nickName?.ToString() ?? "";
                                string registerNo = vehicle.registerNo?.ToString() ?? "";

                                // Only show if not assigned to other assignments
                                if (!assignedVehicleIds.Contains(vehicleId))
                                {
                                    string displayText = !string.IsNullOrEmpty(nickName)
                                        ? $"{registerNo} - {nickName}"
                                        : registerNo;

                                    ddlEditBuses.Items.Add(new ListItem(displayText, vehicleId));
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadEditDrivers(string currentAssignId = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveDrivers").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditDrivers.Items.Clear();
                        ddlEditDrivers.Items.Add(new ListItem("Select Driver", ""));

                        // Get drivers assigned to OTHER assignments (not current one)
                        var assignedDriverIds = currentAssignments
                            .Where(a => a.driverId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.assignId.ToString() != currentAssignId))
                            .Select(a => a.driverId.ToString())
                            .ToHashSet();

                        foreach (var driver in drivers)
                        {
                            if (driver.vehicleUserId != null && driver.vehicleUserName != null)
                            {
                                string driverId = driver.vehicleUserId.ToString();
                                string driverName = driver.vehicleUserName.ToString();

                                // Only show if not assigned to other assignments
                                if (!assignedDriverIds.Contains(driverId))
                                {
                                    ddlEditDrivers.Items.Add(new ListItem(driverName, driverId));
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadEditConductors(string currentAssignId = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveConductors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var conductors = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditUsers.Items.Clear();
                        ddlEditUsers.Items.Add(new ListItem("Select Conductor", ""));

                        // Get conductors assigned to OTHER assignments (not current one)
                        var assignedConductorIds = currentAssignments
                            .Where(a => a.conductorId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.assignId.ToString() != currentAssignId))
                            .Select(a => a.conductorId.ToString())
                            .ToHashSet();

                        foreach (var conductor in conductors)
                        {
                            if (conductor.vehicleUserId != null && conductor.vehicleUserName != null)
                            {
                                string conductorId = conductor.vehicleUserId.ToString();
                                string conductorName = conductor.vehicleUserName.ToString();

                                // Only show if not assigned to other assignments
                                if (!assignedConductorIds.Contains(conductorId))
                                {
                                    ddlEditUsers.Items.Add(new ListItem(conductorName, conductorId));
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadEditDevices(string currentAssignId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadEditDevices called with currentAssignId: {currentAssignId} ===");
                System.Diagnostics.Debug.WriteLine($"Total currentAssignments count: {currentAssignments.Count}");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblDeviceInformations/GetActiveDevices").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var devices = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditPos.Items.Clear();
                        ddlEditPos.Items.Add(new ListItem("Select Device", ""));

                        // Get devices assigned to OTHER assignments (not current one)
                        var assignedDeviceIds = currentAssignments
                            .Where(a => a.deviceId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.assignId.ToString() != currentAssignId))
                            .Select(a => a.deviceId.ToString())
                            .ToHashSet();

                        System.Diagnostics.Debug.WriteLine($"Assigned device IDs (excluding current): {string.Join(", ", assignedDeviceIds)}");
                        System.Diagnostics.Debug.WriteLine($"Total devices from API: {devices.Count}");

                        foreach (var device in devices)
                        {
                            if (device.deviceId != null)
                            {
                                string deviceId = device.deviceId.ToString();
                                string deviceName = device.deviceName?.ToString() ?? "";

                                bool isAssigned = assignedDeviceIds.Contains(deviceId);
                                System.Diagnostics.Debug.WriteLine($"Device {deviceId}: Assigned to others? {isAssigned}");

                                // Only show if not assigned to other assignments
                                if (!isAssigned)
                                {
                                    string displayText = !string.IsNullOrEmpty(deviceName)
                                        ? $"{deviceId} - {deviceName}"
                                        : deviceId;

                                    ddlEditPos.Items.Add(new ListItem(displayText, deviceId));
                                    System.Diagnostics.Debug.WriteLine($"  -> Added to dropdown: {displayText}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"  -> SKIPPED (assigned to other assignment)");
                                }
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"Final dropdown count: {ddlEditPos.Items.Count - 1}"); // -1 for "Select Device"
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadEditDevices: {ex.Message}");
            }
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlRoutes.SelectedValue) ||
                    string.IsNullOrEmpty(ddlBuses.SelectedValue) ||
                    string.IsNullOrEmpty(ddlPos.SelectedValue) ||
                    string.IsNullOrEmpty(ddlDrivers.SelectedValue) ||
                    string.IsNullOrEmpty(ddlUsers.SelectedValue))
                {
                    ShowMessage("Please select all required fields.", false);
                    return;
                }

                string result = CreateAssignment(
                    ddlRoutes.SelectedValue,
                    ddlBuses.SelectedValue,
                    ddlPos.SelectedValue,
                    ddlDrivers.SelectedValue,
                    ddlUsers.SelectedValue
                );

                if (!string.IsNullOrEmpty(result))
                {
                    ShowMessage("Assignment created successfully!", true);
                    ClearSelections();
                    LoadCurrentAssignments();
                    LoadDropdowns();
                    BindAssignments();
                }
                else
                {
                    ShowMessage("Failed to create assignment.", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred: {ex.Message}", false);
            }
        }

        private string CreateAssignment(string routeId, string vehicleId, string deviceId, string driverId, string conductorId)
        {
            try
            {
                var assignment = new
                {
                    vehicleRouteId = int.Parse(routeId),
                    vehicleId = int.Parse(vehicleId),
                    deviceId = deviceId,
                    driverId = driverId,
                    conductorId = conductorId,
                    assignedBy = "Admin",
                    updatedBy = "Admin"
                };

                var json = JsonConvert.SerializeObject(assignment);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = apiUrl + "TblAssignDetail/PostTblAssignDetail";
                    var responseTask = httpClient.PostAsync(url, data);
                    responseTask.Wait();
                    HttpResponseMessage response = responseTask.Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateAssignment Exception: {ex.Message}");
                return "";
            }
        }

        private void BindAssignments()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblAssignDetail/GetTblAssignDetails").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var assignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        // Check and update assignment status based on resource status
                        CheckAndUpdateAssignmentStatus(assignments);

                        DataTable dt = new DataTable();
                        dt.Columns.Add("AssignId");
                        dt.Columns.Add("RouteName");
                        dt.Columns.Add("VehicleNo");
                        dt.Columns.Add("DeviceId");
                        dt.Columns.Add("DriverName");
                        dt.Columns.Add("ConductorName");
                        dt.Columns.Add("AssignedDate", typeof(DateTime));
                        dt.Columns.Add("Status");

                        foreach (var assignment in assignments)
                        {
                            DataRow row = dt.NewRow();
                            row["AssignId"] = assignment.assignId != null ? assignment.assignId.ToString() : "";

                            if (assignment.vehicleRouteId != null)
                            {
                                string routeName = GetRouteName(assignment.vehicleRouteId.ToString());
                                row["RouteName"] = routeName;
                            }

                            if (assignment.vehicleId != null)
                            {
                                string vehicleNo = GetVehicleRegisterNo(assignment.vehicleId.ToString());
                                row["VehicleNo"] = vehicleNo;
                            }

                            row["DeviceId"] = assignment.deviceId != null ? assignment.deviceId.ToString() : "";

                            if (assignment.driverId != null)
                            {
                                string driverName = GetDriverName(assignment.driverId.ToString());
                                row["DriverName"] = driverName;
                            }

                            if (assignment.conductorId != null)
                            {
                                string conductorName = GetConductorName(assignment.conductorId.ToString());
                                row["ConductorName"] = conductorName;
                            }

                            DateTime assignedDate = DateTime.Now;
                            if (assignment.assignedDate != null)
                            {
                                DateTime.TryParse(assignment.assignedDate.ToString(), out assignedDate);
                            }
                            row["AssignedDate"] = assignedDate;

                            row["Status"] = assignment.status != null ? assignment.status.ToString() : "";
                            dt.Rows.Add(row);
                        }

                        gvAssignments.DataSource = dt;
                        gvAssignments.DataBind();
                    }
                    else
                    {
                        CreateEmptyAssignmentTable();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading assignments: " + ex.Message, false);
                CreateEmptyAssignmentTable();
            }
        }

        private void CheckAndUpdateAssignmentStatus(List<dynamic> assignments)
        {
            try
            {
                // Get all active resources
                var activeVehicles = GetActiveResourceIds("Vehicles/GetActiveVehicles", "id");
                var activeDevices = GetActiveResourceIds("TblDeviceInformations/GetActiveDevices", "deviceId");
                var activeDrivers = GetActiveResourceIds("tblVehicleUser/GetActiveDrivers", "vehicleUserId");
                var activeConductors = GetActiveResourceIds("tblVehicleUser/GetActiveConductors", "vehicleUserId");
                var activeRoutes = GetActiveResourceIds("VehicleRoutes/GetActiveVehicleRoutes", "id");

                foreach (var assignment in assignments)
                {
                    if (assignment.assignId == null) continue;

                    string assignId = assignment.assignId.ToString();
                    string currentStatus = assignment.status?.ToString()?.ToUpper() ?? "INACTIVE";

                    // Check if any resource is inactive
                    bool vehicleInactive = assignment.vehicleId != null && !activeVehicles.Contains(assignment.vehicleId.ToString());
                    bool deviceInactive = assignment.deviceId != null && !activeDevices.Contains(assignment.deviceId.ToString());
                    bool driverInactive = assignment.driverId != null && !activeDrivers.Contains(assignment.driverId.ToString());
                    bool conductorInactive = assignment.conductorId != null && !activeConductors.Contains(assignment.conductorId.ToString());
                    bool routeInactive = assignment.vehicleRouteId != null && !activeRoutes.Contains(assignment.vehicleRouteId.ToString());

                    bool anyResourceInactive = vehicleInactive || deviceInactive || driverInactive || conductorInactive || routeInactive;
                    bool allResourcesActive = !anyResourceInactive;

                    // CASE 1: If any resource is inactive but assignment is still ACTIVE, make it INACTIVE
                    if (anyResourceInactive && currentStatus == "ACTIVE")
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUTO-INACTIVE] Assignment {assignId} has inactive resources:");
                        if (vehicleInactive) System.Diagnostics.Debug.WriteLine($"  - Vehicle {assignment.vehicleId} is inactive");
                        if (deviceInactive) System.Diagnostics.Debug.WriteLine($"  - Device {assignment.deviceId} is inactive");
                        if (driverInactive) System.Diagnostics.Debug.WriteLine($"  - Driver {assignment.driverId} is inactive");
                        if (conductorInactive) System.Diagnostics.Debug.WriteLine($"  - Conductor {assignment.conductorId} is inactive");
                        if (routeInactive) System.Diagnostics.Debug.WriteLine($"  - Route {assignment.vehicleRouteId} is inactive");

                        // Update assignment status to INACTIVE
                        UpdateAssignStatus(int.Parse(assignId), "INACTIVE");

                        // Update in current list
                        assignment.status = "INACTIVE";
                    }
                    // CASE 2: If all resources are active but assignment is INACTIVE, make it ACTIVE
                    else if (allResourcesActive && currentStatus == "INACTIVE")
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUTO-ACTIVE] Assignment {assignId} - All resources are now active:");
                        System.Diagnostics.Debug.WriteLine($"  ✓ Vehicle {assignment.vehicleId} is active");
                        System.Diagnostics.Debug.WriteLine($"  ✓ Device {assignment.deviceId} is active");
                        System.Diagnostics.Debug.WriteLine($"  ✓ Driver {assignment.driverId} is active");
                        System.Diagnostics.Debug.WriteLine($"  ✓ Conductor {assignment.conductorId} is active");
                        System.Diagnostics.Debug.WriteLine($"  ✓ Route {assignment.vehicleRouteId} is active");

                        // Update assignment status to ACTIVE
                        UpdateAssignStatus(int.Parse(assignId), "ACTIVE");

                        // Update in current list
                        assignment.status = "ACTIVE";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[NO-CHANGE] Assignment {assignId} status remains {currentStatus}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking assignment status: {ex.Message}");
            }
        }

        private HashSet<string> GetActiveResourceIds(string endpoint, string idFieldName)
        {
            var activeIds = new HashSet<string>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + endpoint).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var resources = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        foreach (var resource in resources)
                        {
                            if (resource[idFieldName] != null)
                            {
                                activeIds.Add(resource[idFieldName].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active resources from {endpoint}: {ex.Message}");
            }
            return activeIds;
        }

        private void CreateEmptyAssignmentTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AssignId");
            dt.Columns.Add("RouteName");
            dt.Columns.Add("VehicleNo");
            dt.Columns.Add("DeviceId");
            dt.Columns.Add("DriverName");
            dt.Columns.Add("ConductorName");
            dt.Columns.Add("AssignedDate", typeof(DateTime));
            dt.Columns.Add("Status");

            gvAssignments.DataSource = dt;
            gvAssignments.DataBind();
        }

        private string GetRouteName(string routeId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "VehicleRoutes/GetActiveVehicleRoutes").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var route = routes.FirstOrDefault(r => r.id != null && r.id.ToString() == routeId);
                        return route?.name != null ? route.name.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetVehicleRegisterNo(string vehicleId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "Vehicles/GetActiveVehicles").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var vehicles = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var vehicle = vehicles.FirstOrDefault(v => v.id != null && v.id.ToString() == vehicleId);
                        return vehicle?.registerNo != null ? vehicle.registerNo.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetDriverName(string driverId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveDrivers").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var driver = drivers.FirstOrDefault(d => d.vehicleUserId != null && d.vehicleUserId.ToString() == driverId);
                        return driver?.vehicleUserName != null ? driver.vehicleUserName.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetConductorName(string conductorId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "tblVehicleUser/GetActiveConductors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var conductors = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var conductor = conductors.FirstOrDefault(c => c.vehicleUserId != null && c.vehicleUserId.ToString() == conductorId);
                        return conductor?.vehicleUserName != null ? conductor.vehicleUserName.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        protected void ClearSelections()
        {
            try
            {
                ddlRoutes.SelectedIndex = 0;
                ddlBuses.SelectedIndex = 0;
                ddlPos.SelectedIndex = 0;
                ddlDrivers.SelectedIndex = 0;
                ddlUsers.SelectedIndex = 0;
            }
            catch { }
        }

        protected void gvAssignments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                if (ddlStatus != null)
                {
                    string status = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString() ?? "";

                    if (ddlStatus.Items.FindByValue(status) != null)
                    {
                        ddlStatus.SelectedValue = status;
                    }

                    ddlStatus.CssClass = "status-dropdown";
                    if (status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
                    {
                        ddlStatus.CssClass += " status-active";
                    }
                    else if (status.Equals("INACTIVE", StringComparison.OrdinalIgnoreCase))
                    {
                        ddlStatus.CssClass += " status-inactive";
                    }
                }
            }
        }

        protected void gvAssignments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string assignId = e.CommandArgument.ToString();

            if (e.CommandName == "EditAssignment")
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"=== Edit button clicked for AssignId: {assignId} ===");

                    // CRITICAL: Reload current assignments to get latest data
                    LoadCurrentAssignments();

                    // Store assignment ID first
                    hdnEditAssignId.Value = assignId;
                    ViewState["EditAssignId"] = assignId;

                    // Load assignment details (this will call LoadEditDropdowns with currentAssignId)
                    LoadAssignmentForEdit(assignId);

                    // Update the modal UpdatePanel
                    UpdatePanel2.Update();

                    // Show modal using JavaScript with a slight delay to ensure UpdatePanel completes
                    string script = @"
                        setTimeout(function() {
                            var modal = document.getElementById('editModal');
                            if (modal) {
                                modal.classList.add('show');
                                console.log('Modal opened');
                            } else {
                                console.error('Modal element not found');
                            }
                        }, 200);
                    ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", script, true);

                    System.Diagnostics.Debug.WriteLine("=== Edit assignment completed successfully ===");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in EditAssignment: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    ShowMessage("Error loading assignment for edit: " + ex.Message, false);
                }
            }
        }

        private void LoadAssignmentForEdit(string assignId)
        {
            try
            {
                // First load all edit dropdowns with available resources (excluding OTHER assignments' resources)
                LoadEditDropdowns(assignId);

                // Store assignment ID
                hdnEditAssignId.Value = assignId;

                // Try to find assignment from the list we already loaded
                var assignment = currentAssignments.FirstOrDefault(a => a.assignId != null && a.assignId.ToString() == assignId);

                if (assignment == null)
                {
                    // If not found in cache, try API call
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(token_sess))
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                        }

                        // Try single assignment endpoint first
                        HttpResponseMessage response = httpClient.GetAsync(apiUrl + $"TblAssignDetail/GetTblAssignDetail/{assignId}").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var responseMessage = response.Content.ReadAsStringAsync().Result;
                            assignment = JsonConvert.DeserializeObject<dynamic>(responseMessage);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Single assignment API failed: {response.StatusCode}");
                            // Try getting from all assignments
                            response = httpClient.GetAsync(apiUrl + "TblAssignDetail/GetTblAssignDetails").Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var responseMessage = response.Content.ReadAsStringAsync().Result;
                                var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);
                                assignment = allAssignments.FirstOrDefault(a => a.assignId != null && a.assignId.ToString() == assignId);
                            }
                        }
                    }
                }

                if (assignment != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Assignment found: {JsonConvert.SerializeObject(assignment)}");

                    // Set Route
                    if (assignment.vehicleRouteId != null)
                    {
                        string routeId = assignment.vehicleRouteId.ToString();
                        System.Diagnostics.Debug.WriteLine($"Setting route: {routeId}, Available items: {ddlEditRoutes.Items.Count}");

                        var routeItem = ddlEditRoutes.Items.FindByValue(routeId);
                        if (routeItem != null)
                        {
                            ddlEditRoutes.SelectedValue = routeId;
                            System.Diagnostics.Debug.WriteLine($"Route selected: {ddlEditRoutes.SelectedValue}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Route {routeId} not found in dropdown");
                            for (int i = 0; i < ddlEditRoutes.Items.Count; i++)
                            {
                                System.Diagnostics.Debug.WriteLine($"  Route item {i}: Value={ddlEditRoutes.Items[i].Value}, Text={ddlEditRoutes.Items[i].Text}");
                            }
                        }
                    }

                    // Set Vehicle
                    if (assignment.vehicleId != null)
                    {
                        string vehicleId = assignment.vehicleId.ToString();
                        System.Diagnostics.Debug.WriteLine($"Setting vehicle: {vehicleId}, Available items: {ddlEditBuses.Items.Count}");

                        var vehicleItem = ddlEditBuses.Items.FindByValue(vehicleId);
                        if (vehicleItem != null)
                        {
                            ddlEditBuses.SelectedValue = vehicleId;
                            System.Diagnostics.Debug.WriteLine($"Vehicle selected: {ddlEditBuses.SelectedValue}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Vehicle {vehicleId} not found in dropdown");
                        }
                    }

                    // Set Device
                    if (assignment.deviceId != null)
                    {
                        string deviceId = assignment.deviceId.ToString();
                        System.Diagnostics.Debug.WriteLine($"Setting device: {deviceId}, Available items: {ddlEditPos.Items.Count}");

                        var deviceItem = ddlEditPos.Items.FindByValue(deviceId);
                        if (deviceItem != null)
                        {
                            ddlEditPos.SelectedValue = deviceId;
                            System.Diagnostics.Debug.WriteLine($"Device selected: {ddlEditPos.SelectedValue}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Device {deviceId} not found in dropdown");
                        }
                    }

                    // Set Driver
                    if (assignment.driverId != null)
                    {
                        string driverId = assignment.driverId.ToString();
                        System.Diagnostics.Debug.WriteLine($"Setting driver: {driverId}, Available items: {ddlEditDrivers.Items.Count}");

                        var driverItem = ddlEditDrivers.Items.FindByValue(driverId);
                        if (driverItem != null)
                        {
                            ddlEditDrivers.SelectedValue = driverId;
                            System.Diagnostics.Debug.WriteLine($"Driver selected: {ddlEditDrivers.SelectedValue}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Driver {driverId} not found in dropdown");
                        }
                    }

                    // Set Conductor
                    if (assignment.conductorId != null)
                    {
                        string conductorId = assignment.conductorId.ToString();
                        System.Diagnostics.Debug.WriteLine($"Setting conductor: {conductorId}, Available items: {ddlEditUsers.Items.Count}");

                        var conductorItem = ddlEditUsers.Items.FindByValue(conductorId);
                        if (conductorItem != null)
                        {
                            ddlEditUsers.SelectedValue = conductorId;
                            System.Diagnostics.Debug.WriteLine($"Conductor selected: {ddlEditUsers.SelectedValue}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Conductor {conductorId} not found in dropdown");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Assignment {assignId} not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading assignment: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        protected void btnReAssign_Click(object sender, EventArgs e)
        {
            try
            {
                string assignId = hdnEditAssignId.Value;

                if (string.IsNullOrEmpty(assignId))
                {
                    ShowEditMessage("Assignment ID not found.", false);
                    return;
                }

                // Validate that all fields are selected
                if (string.IsNullOrEmpty(ddlEditRoutes.SelectedValue) ||
                    string.IsNullOrEmpty(ddlEditBuses.SelectedValue) ||
                    string.IsNullOrEmpty(ddlEditPos.SelectedValue) ||
                    string.IsNullOrEmpty(ddlEditDrivers.SelectedValue) ||
                    string.IsNullOrEmpty(ddlEditUsers.SelectedValue))
                {
                    ShowEditMessage("Please select all resources.", false);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                    return;
                }

                // Build update object with all fields
                var updateData = new Dictionary<string, object>
                {
                    { "assignId", int.Parse(assignId) },
                    { "vehicleRouteId", int.Parse(ddlEditRoutes.SelectedValue) },
                    { "vehicleId", int.Parse(ddlEditBuses.SelectedValue) },
                    { "deviceId", ddlEditPos.SelectedValue },
                    { "driverId", ddlEditDrivers.SelectedValue },
                    { "conductorId", ddlEditUsers.SelectedValue }
                };

                bool success = UpdateAssignment(assignId, updateData);

                if (success)
                {
                    ShowEditMessage("Assignment updated successfully!", true);

                    // Refresh data
                    LoadCurrentAssignments();
                    LoadDropdowns();
                    BindAssignments();
                }
                else
                {
                    ShowEditMessage("Failed to update assignment.", false);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage($"An error occurred: {ex.Message}", false);
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                System.Diagnostics.Debug.WriteLine($"btnReAssign_Click Error: {ex.Message}");
            }
        }

        private bool UpdateAssignment(string assignId, Dictionary<string, object> updateData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updateData);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = apiUrl + $"TblAssignDetail/PutTblAssignDetail/{assignId}";

                    System.Diagnostics.Debug.WriteLine($"Update API URL: {url}");
                    System.Diagnostics.Debug.WriteLine($"Update JSON: {json}");

                    var responseTask = httpClient.PostAsync(url, data);
                    responseTask.Wait();
                    HttpResponseMessage response = responseTask.Result;

                    string result = response.Content.ReadAsStringAsync().Result;
                    System.Diagnostics.Debug.WriteLine($"Update Response: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Update Result: {result}");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAssignment Error: {ex.Message}");
                return false;
            }
        }

        protected async void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlStatus = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddlStatus.NamingContainer;
                HiddenField hdnAssignId = (HiddenField)row.FindControl("hdnAssignId");

                if (hdnAssignId != null && !string.IsNullOrEmpty(hdnAssignId.Value))
                {
                    int assignId = Convert.ToInt32(hdnAssignId.Value);
                    string newStatus = ddlStatus.SelectedValue;

                    bool success = await UpdateAssignStatus(assignId, newStatus);

                    if (success)
                    {
                        ShowMessage("Status updated successfully!", true);
                    }
                    else
                    {
                        ShowMessage("Failed to update status.", false);
                        ddlStatus.SelectedValue = newStatus == "ACTIVE" ? "INACTIVE" : "ACTIVE";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
        }

        //private bool UpdateAssignStatus(int assignId, string newStatus)
        //{
        //    try
        //    {
        //        var requestData = new
        //        {
        //            AssignId = assignId,
        //            Status = newStatus
        //        };

        //        string jsonData = JsonConvert.SerializeObject(requestData);

        //        using (var client = new HttpClient())
        //        {
        //            client.Timeout = TimeSpan.FromSeconds(10);

        //            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //            client.DefaultRequestHeaders.Add("Accept", "application/json");

        //            if (!string.IsNullOrEmpty(token_sess))
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
        //            }

        //            var response = client.PostAsync(apiUrl + "TblAssignDetail/ToggleAssignStatus", content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string result = response.Content.ReadAsStringAsync().Result;
        //                return result.Contains("updated successfully");
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"UpdateAssignStatus Exception: {ex.Message}");
        //        return false;
        //    }
        //}

        private void ShowMessage(string message, bool isSuccess)
        {
            lblResult.Text = message;
            lblResult.CssClass = isSuccess ? "result-message success" : "result-message error";
            lblResult.Style["display"] = "block";

            string script = $"showMessage('{message.Replace("'", "\\'")}', {isSuccess.ToString().ToLower()});";
            ScriptManager.RegisterStartupScript(this, GetType(), "showMessage", script, true);
        }

        private void ShowEditMessage(string message, bool isSuccess)
        {
            lblEditResult.Text = message;
            lblEditResult.CssClass = isSuccess ? "result-message success" : "result-message error";
            lblEditResult.Style["display"] = "block";

            string script = $"showEditMessage('{message.Replace("'", "\\'")}', {isSuccess.ToString().ToLower()});";
            ScriptManager.RegisterStartupScript(this, GetType(), "showEditMessage", script, true);
        }


        //--------


        private void StartResourceCheckTimer()
        {
            if (resourceCheckTimer == null)
            {
                resourceCheckTimer = new System.Timers.Timer(60000); // Check every 1 minute
                resourceCheckTimer.Elapsed += CheckResourceAvailability;
                resourceCheckTimer.AutoReset = true;
                resourceCheckTimer.Start();
            }
        }

        private async void CheckResourceAvailability(object sender, ElapsedEventArgs e)
        {
            try
            {
                await CheckAndUpdateAssignmentStatus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Resource check error: {ex.Message}");
            }
        }


        private async Task CheckAndUpdateAssignmentStatus()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //client.Timeout = TimeSpan.FromSeconds(30);

                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    // Get all assignments
                    var assignmentsResponse = await client.GetAsync(apiUrl + "TblAssignDetail/GetTblAssignDetail");
                    assignmentsResponse.EnsureSuccessStatusCode();
                    var assignmentsJson = await assignmentsResponse.Content.ReadAsStringAsync();
                    var assignments = JsonConvert.DeserializeObject<List<AssignmentModel>>(assignmentsJson);

                    if (assignments == null || !assignments.Any())
                    {
                        System.Diagnostics.Debug.WriteLine("No assignments found");
                        return;
                    }

                    // Get schedules
                    var schedulesResponse = await client.GetAsync(apiUrl + "Schedules/GetSchedules");
                    schedulesResponse.EnsureSuccessStatusCode();
                    var schedulesJson = await schedulesResponse.Content.ReadAsStringAsync();
                    var schedules = JsonConvert.DeserializeObject<List<ScheduleModel>>(schedulesJson);

                    // Get attendance data - FILTER for Present status only
                    var attendanceResponse = await client.GetAsync(apiUrl + "TblAttandance/GetTblAttandance");
                    attendanceResponse.EnsureSuccessStatusCode();
                    var attendanceJson = await attendanceResponse.Content.ReadAsStringAsync();
                    var allAttendance = JsonConvert.DeserializeObject<List<AttendanceModel>>(attendanceJson);

                    // Filter only Present status attendance for today
                    DateTime today = DateTime.Today;
                    var attendanceList = allAttendance?
                        .Where(a => a.Status?.Trim().Equals("Present", StringComparison.OrdinalIgnoreCase) == true
                                    && a.AttandanceDate.Date == today)
                        .ToList() ?? new List<AttendanceModel>();

                    System.Diagnostics.Debug.WriteLine($"Present attendance count for today: {attendanceList.Count}");

                    // Get device data
                    var devicesResponse = await client.GetAsync(apiUrl + "TblDeviceInformations/GetTblDeviceInformations");
                    devicesResponse.EnsureSuccessStatusCode();
                    var devicesJson = await devicesResponse.Content.ReadAsStringAsync();
                    var devices = JsonConvert.DeserializeObject<List<DeviceModel>>(devicesJson);

                    // Get vehicle data
                    var vehiclesResponse = await client.GetAsync(apiUrl + "Vehicles/GetVehicles");
                    vehiclesResponse.EnsureSuccessStatusCode();
                    var vehiclesJson = await vehiclesResponse.Content.ReadAsStringAsync();
                    var vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(vehiclesJson);

                    DateTime currentTime = DateTime.Now;

                    foreach (var assignment in assignments.Where(a => a.Status?.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase) == true))
                    {
                        // Find schedule for this route
                        var schedule = schedules?.FirstOrDefault(s => s.RouteId == assignment.RouteId);

                        if (schedule != null && schedule.DepartureTime.HasValue)
                        {
                            DateTime departureDateTime = today.Add(schedule.DepartureTime.Value);
                            DateTime checkTime = departureDateTime.AddMinutes(-15);

                            // Check if we are within 15 minutes before departure
                            if (currentTime >= checkTime && currentTime <= departureDateTime)
                            {
                                bool shouldDeactivate = false;
                                List<string> reasons = new List<string>();

                                // Check Driver availability
                                var driverAttendance = attendanceList.FirstOrDefault(a =>
                                    a.VehicleUserId == assignment.DriverId);

                                if (driverAttendance == null)
                                {
                                    shouldDeactivate = true;
                                    reasons.Add($"Driver {assignment.DriverId} not present");
                                    System.Diagnostics.Debug.WriteLine($"Driver {assignment.DriverId} not available for Assignment {assignment.AssignId}");
                                }

                                // Check Conductor availability
                                var conductorAttendance = attendanceList.FirstOrDefault(a =>
                                    a.VehicleUserId == assignment.ConductorId);

                                if (conductorAttendance == null)
                                {
                                    shouldDeactivate = true;
                                    reasons.Add($"Conductor {assignment.ConductorId} not present");
                                    System.Diagnostics.Debug.WriteLine($"Conductor {assignment.ConductorId} not available for Assignment {assignment.AssignId}");
                                }

                                // Check Device availability
                                var device = devices?.FirstOrDefault(d =>
                                    d.DeviceId == assignment.DeviceId &&
                                    d.IsActive == true);

                                if (device == null)
                                {
                                    shouldDeactivate = true;
                                    reasons.Add($"Device {assignment.DeviceId} not active");
                                    System.Diagnostics.Debug.WriteLine($"Device {assignment.DeviceId} not available for Assignment {assignment.AssignId}");
                                }

                                // Check Vehicle availability
                                var vehicle = vehicles?.FirstOrDefault(v =>
                                    v.VehicleId == assignment.VehicleId &&
                                    v.IsActive == true);

                                if (vehicle == null)
                                {
                                    shouldDeactivate = true;
                                    reasons.Add($"Vehicle {assignment.VehicleId} not active");
                                    System.Diagnostics.Debug.WriteLine($"Vehicle {assignment.VehicleId} not available for Assignment {assignment.AssignId}");
                                }

                                // If any resource is unavailable, deactivate the assignment
                                if (shouldDeactivate)
                                {
                                    await UpdateAssignStatus(assignment.AssignId, "INACTIVE");
                                    System.Diagnostics.Debug.WriteLine($"Assignment {assignment.AssignId} deactivated. Reasons: {string.Join(", ", reasons)}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"Assignment {assignment.AssignId} is valid and remains ACTIVE");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckAndUpdateAssignmentStatus Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public class AssignmentModel
        {
            public int AssignId { get; set; }
            public int RouteId { get; set; }
            public string RouteName { get; set; }
            public int VehicleId { get; set; }
            public string VehicleNo { get; set; }
            public string DeviceId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string ConductorId { get; set; }
            public string ConductorName { get; set; }
            public DateTime AssignedDate { get; set; }
            public string Status { get; set; }
        }

        public class ScheduleModel
        {
            public int ScheduleId { get; set; }
            public int RouteId { get; set; }
            public TimeSpan? DepartureTime { get; set; }
            public TimeSpan? ArrivalTime { get; set; }
        }

        public class AttendanceModel
        {
            public int AttandanceId { get; set; }
            public string VehicleUserId { get; set; }
            public DateTime AttandanceDate { get; set; }
            public string Status { get; set; }
        }

        public class DeviceModel
        {
            public string DeviceId { get; set; }
            public bool IsActive { get; set; }
        }

        public class VehicleModel
        {
            public int VehicleId { get; set; }
            public string VehicleNo { get; set; }
            public bool IsActive { get; set; }
        }

        private async Task<bool> UpdateAssignStatus(int assignId, string newStatus)
        {
            try
            {
                var requestData = new
                {
                    AssignId = assignId,
                    Status = newStatus
                };

                string jsonData = JsonConvert.SerializeObject(requestData);

                using (var client = new HttpClient())
                {
                   // client.Timeout = TimeSpan.FromSeconds(10);

                    // Set Accept header BEFORE adding Authorization
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    if (!string.IsNullOrEmpty(token_sess))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    }

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl + "TblAssignDetail/ToggleAssignStatus", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result.Contains("updated successfully");
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAssignStatus Exception: {ex.Message}");
                return false;
            }
        }
    }
}