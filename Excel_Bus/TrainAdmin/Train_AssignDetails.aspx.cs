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

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_AssignDetails : System.Web.UI.Page
    {
        string token_sess;
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        private List<dynamic> currentAssignments = new List<dynamic>();
        private System.Timers.Timer resourceCheckTimer;

        protected void Page_Load(object sender, EventArgs e)
        {
            token_sess = Session["token"] != null ? Session["token"].ToString() : "";
            string adminId = Session["AdminId"].ToString();
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainCrewAssignments/GetTrainCrewAssignments").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        // FIX 1: Only keep ACTIVE assignments
                        currentAssignments = allAssignments
                            .Where(a => a.status != null && a.status.ToString().ToUpper() == "ACTIVE")
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"=== Active assignments loaded: {currentAssignments.Count} ===");
                        foreach (var assign in currentAssignments)
                            System.Diagnostics.Debug.WriteLine($"Assignment {assign.crewAssignId}: train={assign.trainId}, Device={assign.deviceId}, Driver={assign.driverId}, ticketInspector={assign.ticketInspectors}");
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
            LoadTrains();
            LoadPilots();
            LoadTicketCollector();
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainRoutes/GetActiveTrainRoutes").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlRoutes.Items.Clear();
                        ddlRoutes.Items.Add(new ListItem("Select Route", ""));

                        foreach (var route in routes)
                        {
                            if (route.routeId != null && route.routeName != null)
                            {
                                string routeId = route.routeId.ToString();
                                string routeName = route.routeName.ToString();
                                string startFrom = route.startStation?.ToString() ?? "";
                                string endTo = route.endStation?.ToString() ?? "";

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

        private void LoadTrains()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblTrainsRegs/GetActiveTrains").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var trains = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlTrains.Items.Clear();
                        ddlTrains.Items.Add(new ListItem("Select Train", ""));

                        var assignedTrainIds = currentAssignments
                            .Where(a => a.trainId != null)
                            .Select(a => a.trainId.ToString())
                            .ToHashSet();

                        foreach (var train in trains)
                        {
                            // FIX 2: Use consistent trainId field for both null-check and value
                            if (train.trainId != null)
                            {
                                string trainId = train.trainId.ToString();
                                string trainName = train.trainName?.ToString() ?? "";
                                string trainNumber = train.trainNumber?.ToString() ?? "";

                                if (!assignedTrainIds.Contains(trainId))
                                {
                                    string displayText = !string.IsNullOrEmpty(trainName)
                                        ? $"{trainNumber} - {trainName}"
                                        : trainNumber;

                                    ddlTrains.Items.Add(new ListItem(displayText, trainId));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading trains: " + ex.Message, false);
            }
        }

        private void LoadPilots()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilot").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlDrivers.Items.Clear();
                        ddlDrivers.Items.Add(new ListItem("Select Driver", ""));

                        // FIX 3: Use correct field name driverId (not TrainUserId) to match assignment model
                        var assignedDriverIds = currentAssignments
                            .Where(a => a.driverId != null)
                            .Select(a => a.driverId.ToString())
                            .ToHashSet();

                        foreach (var driver in drivers)
                        {
                            if (driver.trainUserId != null && driver.trainUserName != null)
                            {
                                string driverId = driver.trainUserId.ToString();
                                string driverName = driver.trainUserName.ToString();

                                if (!assignedDriverIds.Contains(driverId))
                                    ddlDrivers.Items.Add(new ListItem(driverName, driverId));
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

        private void LoadTicketCollector()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilotInspectors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var ticketInspectors = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlUsers.Items.Clear();
                        ddlUsers.Items.Add(new ListItem("Select Ticket Inspector", ""));

                        // FIX 3: Use correct field name ticketInspectors (not TrainUserId)
                        var assignedticketInspectorss = currentAssignments
                            .Where(a => a.ticketInspectors != null)
                            .Select(a => a.ticketInspectors.ToString())
                            .ToHashSet();

                        foreach (var ticketInspector in ticketInspectors)
                        {
                            if (ticketInspector.trainUserId != null && ticketInspector.trainUserName != null)
                            {
                                string ticketInspectorId = ticketInspector.trainUserId.ToString();
                                string ticketInspectorName = ticketInspector.trainUserName.ToString();

                                if (!assignedticketInspectorss.Contains(ticketInspectorId))
                                    ddlUsers.Items.Add(new ListItem(ticketInspectorName, ticketInspectorId));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading Inspector: " + ex.Message, false);
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainDeviceInformations/GetActiveTrainDevices").Result;

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
            LoadEditTrains(currentAssignId);
            LoadEditDrivers(currentAssignId);
            LoadEditticketInspectors(currentAssignId);
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainRoutes/GetActiveTrainRoutes").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditRoutes.Items.Clear();
                        ddlEditRoutes.Items.Add(new ListItem("Select Route", ""));

                        foreach (var route in routes)
                        {
                            // FIX 4: Use same field names as LoadRoutes() — routeId / routeName
                            if (route.routeId != null && route.routeName != null)
                            {
                                string routeId = route.routeId.ToString();
                                string routeName = route.routeName.ToString();
                                string startFrom = route.startStation?.ToString() ?? "";
                                string endTo = route.endStation?.ToString() ?? "";

                                string displayText = $"{routeName} ({startFrom} - {endTo})";
                                ddlEditRoutes.Items.Add(new ListItem(displayText, routeId));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // FIX: Renamed from LoadEdittrains → LoadEditTrains (PascalCase)
        private void LoadEditTrains(string currentAssignId = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblTrainsRegs/GetActiveTrains").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var trains = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditTrains.Items.Clear();
                        ddlEditTrains.Items.Add(new ListItem("Select Train", ""));

                        var assignedTrainIds = currentAssignments
                            .Where(a => a.trainId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.crewAssignId.ToString() != currentAssignId))
                            .Select(a => a.trainId.ToString())
                            .ToHashSet();

                        foreach (var train in trains)
                        {
                            // FIX 2: Use trainId consistently
                            if (train.trainId != null)
                            {
                                string trainId = train.trainId.ToString();
                                string trainName = train.trainName?.ToString() ?? "";
                                string trainNumber = train.trainNumber?.ToString() ?? "";

                                if (!assignedTrainIds.Contains(trainId))
                                {
                                    string displayText = !string.IsNullOrEmpty(trainName)
                                        ? $"{trainNumber} - {trainName}"
                                        : trainNumber;

                                    ddlEditTrains.Items.Add(new ListItem(displayText, trainId));
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilot").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditDrivers.Items.Clear();
                        ddlEditDrivers.Items.Add(new ListItem("Select Driver", ""));

                        var assignedDriverIds = currentAssignments
                            .Where(a => a.driverId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.crewAssignId.ToString() != currentAssignId))
                            .Select(a => a.driverId.ToString())
                            .ToHashSet();

                        foreach (var driver in drivers)
                        {
                            if (driver.trainUserId != null && driver.trainUserName != null)
                            {
                                string driverId = driver.trainUserId.ToString();
                                string driverName = driver.trainUserName.ToString();

                                if (!assignedDriverIds.Contains(driverId))
                                    ddlEditDrivers.Items.Add(new ListItem(driverName, driverId));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void LoadEditticketInspectors(string currentAssignId = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilotInspectors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var ticketInspectors = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditUsers.Items.Clear();
                        ddlEditUsers.Items.Add(new ListItem("Select ticketInspector", ""));

                        var assignedticketInspectorss = currentAssignments
                            .Where(a => a.ticketInspectors != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.crewAssignId.ToString() != currentAssignId))
                            .Select(a => a.ticketInspectors.ToString())
                            .ToHashSet();

                        foreach (var ticketInspector in ticketInspectors)
                        {
                            if (ticketInspector.trainUserId != null && ticketInspector.trainUserName != null)
                            {
                                string ticketInspector_ = ticketInspector.trainUserId.ToString();
                                string ticketInspectorName = ticketInspector.trainUserName.ToString();

                                if (!assignedticketInspectorss.Contains(ticketInspector_))
                                    ddlEditUsers.Items.Add(new ListItem(ticketInspectorName, ticketInspector_));
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainDeviceInformations/GetActiveTrainDevices").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var devices = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        ddlEditPos.Items.Clear();
                        ddlEditPos.Items.Add(new ListItem("Select Device", ""));

                        var assignedDeviceIds = currentAssignments
                            .Where(a => a.deviceId != null &&
                                   (string.IsNullOrEmpty(currentAssignId) || a.crewAssignId.ToString() != currentAssignId))
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

                        System.Diagnostics.Debug.WriteLine($"Final dropdown count: {ddlEditPos.Items.Count - 1}");
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
                    string.IsNullOrEmpty(ddlTrains.SelectedValue) ||
                    string.IsNullOrEmpty(ddlPos.SelectedValue) ||
                    string.IsNullOrEmpty(ddlDrivers.SelectedValue) ||
                    string.IsNullOrEmpty(ddlUsers.SelectedValue))
                {
                    ShowMessage("Please select all required fields.", false);
                    return;
                }

                string result = CreateAssignment(
                    ddlRoutes.SelectedValue,
                    ddlTrains.SelectedValue,
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

        private string CreateAssignment(string tripId, string trainId, string deviceId, string driverId, string ticketInspectorId)
        {
            try
            {
                var assignment = new
                {
                    tripId = int.Parse(tripId),
                    trainId = trainId,
                    deviceId = deviceId,
                    driverId = driverId,
                    ticketInspectors = ticketInspectorId,
                    assignedBy = "Admin",
                    updatedBy = "Admin"
                };

                var json = JsonConvert.SerializeObject(assignment);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = apiUrl + "TrainCrewAssignments/PostTrainCrewAssignment";
                    var responseTask = httpClient.PostAsync(url, data);
                    responseTask.Wait();
                    HttpResponseMessage response = responseTask.Result;

                    if (response.IsSuccessStatusCode)
                        return response.Content.ReadAsStringAsync().Result;

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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainCrewAssignments/GetTrainCrewAssignments").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var assignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        CheckAndUpdateAssignmentStatus(assignments);

                        DataTable dt = new DataTable();
                        dt.Columns.Add("crewAssignId");
                        dt.Columns.Add("tripId");
                        dt.Columns.Add("TripName");
                        dt.Columns.Add("TrainId");
                        dt.Columns.Add("DeviceId");
                        dt.Columns.Add("DriverName");
                        dt.Columns.Add("ticketInspectors");
                        dt.Columns.Add("AssignedAt", typeof(DateTime));
                        dt.Columns.Add("Status");
                        dt.Columns.Add("DepartureTime");
                        dt.Columns.Add("Countdown");          // ← NEW
                        dt.Columns.Add("IsNearDeparture", typeof(bool));

                        foreach (var assignment in assignments)
                        {
                            DataRow row = dt.NewRow();
                            row["crewAssignId"] = assignment.crewAssignId != null ? assignment.crewAssignId.ToString() : "";

                            if (assignment.tripId != null)
                                row["tripId"] = GetRouteName(assignment.tripId.ToString());

                            if (assignment.trainId != null)
                                row["TrainId"] = GetTrainRegisterNo(assignment.trainId.ToString());

                            if (assignment.deviceId != null)
                                row["DeviceId"] = GetDeviceName(assignment.deviceId.ToString());

                            if (assignment.driverId != null)
                                row["DriverName"] = GetDriverName(assignment.driverId.ToString());

                            if (assignment.ticketInspectors != null)
                                row["ticketInspectors"] = GetticketInspectorName(assignment.ticketInspectors.ToString());

                            DateTime assignedAt = DateTime.Now;
                            if (assignment.assignedAt != null)
                                DateTime.TryParse(assignment.assignedAt.ToString(), out assignedAt);
                            row["AssignedAt"] = assignedAt;

                            // --- Departure time + countdown ---
                            string thisTripId = assignment.tripId != null ? assignment.tripId.ToString() : "";
                            TimeSpan? deptTime = GetDepartureTime(thisTripId);
                            bool isNearDeparture = false;
                            string rowStatus = assignment.status != null ? assignment.status.ToString().ToUpper() : "INACTIVE";

                            if (deptTime.HasValue)
                            {
                                TimeSpan nowTime = DateTime.Now.TimeOfDay;
                                TimeSpan diff = deptTime.Value - nowTime;
                                isNearDeparture = diff.TotalMinutes <= 15 && diff.TotalMinutes >= -5;

                                row["DepartureTime"] = deptTime.Value.ToString(@"hh\:mm");

                                // Human-readable countdown
                                if (diff.TotalMinutes < 0)
                                    row["Countdown"] = $"Departed {Math.Abs((int)diff.TotalMinutes)}m ago";
                                else if (diff.TotalMinutes <= 15)
                                    row["Countdown"] = $"⚠ {(int)diff.TotalMinutes}m {diff.Seconds}s left";
                                else
                                    row["Countdown"] = $"{(int)diff.TotalHours}h {diff.Minutes}m";
                            }
                            else
                            {
                                row["DepartureTime"] = "-";
                                row["Countdown"] = "-";
                            }

                            // --- 15-min check: blank field or inactive resource → force INACTIVE ---
                            if (isNearDeparture && rowStatus == "ACTIVE")
                            {
                                bool anyMissing = string.IsNullOrEmpty(assignment.trainId?.ToString())
                                               || string.IsNullOrEmpty(assignment.deviceId?.ToString())
                                               || string.IsNullOrEmpty(assignment.driverId?.ToString())
                                               || string.IsNullOrEmpty(assignment.ticketInspectors?.ToString());

                                if (anyMissing)
                                {
                                    rowStatus = "INACTIVE";
                                    if (int.TryParse(assignment.crewAssignId?.ToString(), out int cid))
                                        UpdateAssignStatus(cid, "INACTIVE");
                                }
                            }

                            row["IsNearDeparture"] = isNearDeparture;
                            row["Status"] = rowStatus;   // ← Only set ONCE (bug fix)

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

        protected void timerRefresh_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[TIMER TICK] {DateTime.Now} - Refreshing assignments");
            LoadCurrentAssignments();
            BindAssignments();
        }
        //private void CheckAndUpdateAssignmentStatus(List<dynamic> assignments)
        //{
        //    try
        //    {
        //        var activeTrains = GetActiveResourceIds("TblTrainsRegs/GetActiveTrains", "trainId");
        //        var activeDevices = GetActiveResourceIds("TrainDeviceInformations/GetActiveTrainDevices", "deviceId");
        //        var activeDrivers = GetActiveResourceIds("TrainVehicleUserRegs/GetActivePilot", "trainUserId");
        //        var activeticketInspectors = GetActiveResourceIds("TrainVehicleUserRegs/GetActivePilotInspectors", "trainUserId");
        //        var activeRoutes = GetActiveResourceIds("TrainRoutes/GetActiveTrainRoutes", "routeId");

        //        foreach (var assignment in assignments)
        //        {
        //            if (assignment.crewAssignId == null) continue;

        //            string assignId = assignment.crewAssignId.ToString();
        //            string currentStatus = assignment.Status?.ToString()?.ToUpper() ?? "INACTIVE";

        //            bool trainInactive = assignment.trainId != null && !activeTrains.Contains(assignment.trainId.ToString());
        //            bool deviceInactive = assignment.deviceId != null && !activeDevices.Contains(assignment.deviceId.ToString());
        //            bool driverInactive = assignment.driverId != null && !activeDrivers.Contains(assignment.driverId.ToString());
        //            bool ticketInspectorInactive = assignment.ticketInspectors != null && !activeticketInspectors.Contains(assignment.ticketInspectors.ToString());
        //            bool routeInactive = assignment.trainRouteId != null && !activeRoutes.Contains(assignment.trainRouteId.ToString());

        //            bool anyResourceInactive = trainInactive || deviceInactive || driverInactive || ticketInspectorInactive || routeInactive;
        //            bool allResourcesActive = !anyResourceInactive;

        //            if (anyResourceInactive && currentStatus == "ACTIVE")
        //            {
        //                System.Diagnostics.Debug.WriteLine($"[AUTO-INACTIVE] Assignment {assignId} has inactive resources.");
        //                UpdateAssignStatus(int.Parse(assignId), "INACTIVE");
        //                assignment.Status = "INACTIVE";
        //            }
        //            else if (allResourcesActive && currentStatus == "INACTIVE")
        //            {
        //                System.Diagnostics.Debug.WriteLine($"[AUTO-ACTIVE] Assignment {assignId} - All resources are now active.");
        //                UpdateAssignStatus(int.Parse(assignId), "ACTIVE");
        //                assignment.Status = "ACTIVE";
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine($"[NO-CHANGE] Assignment {assignId} status remains {currentStatus}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error checking assignment status: {ex.Message}");
        //    }
        //}

        private void CheckAndUpdateAssignmentStatus(List<dynamic> assignments)
        {
            try
            {
                foreach (var assignment in assignments)
                {
                    if (assignment.crewAssignId == null) continue;

                    string assignId = assignment.crewAssignId.ToString();
                    string currentStatus = assignment.status?.ToString()?.ToUpper() ?? "INACTIVE";

                    bool isIncomplete = string.IsNullOrEmpty(assignment.trainId?.ToString())
                                     || string.IsNullOrEmpty(assignment.deviceId?.ToString())
                                     || string.IsNullOrEmpty(assignment.driverId?.ToString())
                                     || string.IsNullOrEmpty(assignment.ticketInspectors?.ToString());

                    if (isIncomplete && currentStatus == "ACTIVE")
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUTO-INACTIVE] Assignment {assignId}: incomplete fields.");
                        UpdateAssignStatus(int.Parse(assignId), "INACTIVE");
                        assignment.status = "INACTIVE";
                    }
                    else if (!isIncomplete && currentStatus == "INACTIVE")
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUTO-ACTIVE] Assignment {assignId}: all fields complete.");
                        UpdateAssignStatus(int.Parse(assignId), "ACTIVE");
                        assignment.status = "ACTIVE";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[NO-CHANGE] Assignment {assignId}: status remains {currentStatus}");
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
            var activeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + endpoint).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var resources = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        foreach (var resource in resources)
                        {
                            var jObj = resource as Newtonsoft.Json.Linq.JObject;
                            if (jObj != null)
                            {
                                var prop = jObj.Properties()
                                    .FirstOrDefault(p => p.Name.Equals(idFieldName, StringComparison.OrdinalIgnoreCase));
                                if (prop != null && prop.Value != null)
                                    activeIds.Add(prop.Value.ToString());
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
            dt.Columns.Add("CrewAssignId");
            dt.Columns.Add("RouteName");
            // FIX 9: Use "TrainId" to match GridView BoundField DataField
            dt.Columns.Add("TrainId");
            dt.Columns.Add("DeviceId");
            dt.Columns.Add("DriverName");
            dt.Columns.Add("ticketInspectorName");
            dt.Columns.Add("AssignedDate", typeof(DateTime));
            dt.Columns.Add("Status");

            gvAssignments.DataSource = dt;
            gvAssignments.DataBind();
        }

        private string GetRouteName(string tripID_)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainTrips/GetTrainTrips").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var routes = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var route = routes.FirstOrDefault(r => r.tripId != null && r.tripId.ToString() == tripID_);
                        return route?.tripId != null ? route.tripId.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetTrainRegisterNo(string trainId_)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TblTrainsRegs/GetTblTrainsRegs").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var trainss = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var train = trainss?.FirstOrDefault(c =>
                            c.trainId != null &&
                            c.trainId.ToString() == trainId_);

                        return train?.trainNumber != null
                            ? train.trainNumber.ToString()
                            : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetDeviceName(string deviceId_)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainDeviceInformations/GetTrainDeviceInformations").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var devicess = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var device = devicess?.FirstOrDefault(c =>
                            c.deviceId != null &&
                            c.deviceId.ToString() == deviceId_);

                        return device?.deviceName != null
                            ? device.deviceName.ToString()
                            : "";
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilot").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var drivers = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var driver = drivers.FirstOrDefault(d => d.trainUserId != null && d.trainUserId.ToString() == driverId);
                        return driver?.trainUserName != null ? driver.trainUserName.ToString() : "";
                    }
                }
            }
            catch { }
            return "";
        }

        private string GetticketInspectorName(string ticketInspectorId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainVehicleUserRegs/GetActivePilotInspectors").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var ticketInspectorList = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var ticketInspector = ticketInspectorList?.FirstOrDefault(c =>
                            c.trainUserId != null &&
                            c.trainUserId.ToString() == ticketInspectorId);

                        return ticketInspector?.trainUserName != null
                            ? ticketInspector.trainUserName.ToString()
                            : "";
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
                ddlTrains.SelectedIndex = 0;
                ddlPos.SelectedIndex = 0;
                ddlDrivers.SelectedIndex = 0;
                ddlUsers.SelectedIndex = 0;
            }
            catch { }
        }

        //protected void gvAssignments_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
        //        // 15-min departure red row highlight
        //        bool nearDeparture = false;
        //        try
        //        {
        //            var nearVal = DataBinder.Eval(e.Row.DataItem, "IsNearDeparture");
        //            if (nearVal != null) nearDeparture = (bool)nearVal;
        //        }
        //        catch { }

        //        if (nearDeparture)
        //        {
        //            e.Row.BackColor = System.Drawing.Color.FromArgb(255, 220, 220); // Light red
        //            e.Row.ForeColor = System.Drawing.Color.DarkRed;
        //            e.Row.ToolTip = "⚠ Departure within 15 minutes – resource check required!";
        //            if (ddlStatus != null)
        //                ddlStatus.BackColor = System.Drawing.Color.FromArgb(255, 200, 200);
        //        }
        //        if (ddlStatus != null)
        //        {
        //            string status = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString() ?? "";

        //            if (ddlStatus.Items.FindByValue(status) != null)
        //                ddlStatus.SelectedValue = status;

        //            ddlStatus.CssClass = "status-dropdown";

        //            if (status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
        //                ddlStatus.CssClass += " status-active";
        //            else if (status.Equals("INACTIVE", StringComparison.OrdinalIgnoreCase))
        //                ddlStatus.CssClass += " status-inactive";
        //        }
        //    }
        //}

        protected void gvAssignments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");

                bool nearDeparture = false;
                try
                {
                    var nearVal = DataBinder.Eval(e.Row.DataItem, "IsNearDeparture");
                    if (nearVal != null) nearDeparture = (bool)nearVal;
                }
                catch { }

                string status = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString() ?? "";

                // Red row: near departure OR inactive
                if (nearDeparture || status.Equals("INACTIVE", StringComparison.OrdinalIgnoreCase))
                {
                    e.Row.BackColor = System.Drawing.Color.FromArgb(255, 220, 220);
                    e.Row.ForeColor = System.Drawing.Color.DarkRed;
                    if (nearDeparture)
                        e.Row.ToolTip = "⚠ Departure within 15 minutes – resource check required!";
                }

                if (ddlStatus != null)
                {
                    if (ddlStatus.Items.FindByValue(status) != null)
                        ddlStatus.SelectedValue = status;

                    ddlStatus.CssClass = "status-dropdown";
                    if (status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
                        ddlStatus.CssClass += " status-active";
                    else
                        ddlStatus.CssClass += " status-inactive";

                    if (nearDeparture)
                        ddlStatus.BackColor = System.Drawing.Color.FromArgb(255, 180, 180);
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

                    LoadCurrentAssignments();
                    hdnEditAssignId.Value = assignId;
                    ViewState["EditAssignId"] = assignId;

                    LoadAssignmentForEdit(assignId);
                    UpdatePanel2.Update();

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
                LoadEditDropdowns(assignId);
                hdnEditAssignId.Value = assignId;

                var assignment = currentAssignments.FirstOrDefault(a => a.crewAssignId != null && a.crewAssignId.ToString() == assignId);

                if (assignment == null)
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(token_sess))
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                        HttpResponseMessage response = httpClient.GetAsync(apiUrl + $"TrainCrewAssignments/GetTrainCrewAssignment/{assignId}").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var responseMessage = response.Content.ReadAsStringAsync().Result;
                            assignment = JsonConvert.DeserializeObject<dynamic>(responseMessage);
                        }
                        else
                        {
                            response = httpClient.GetAsync(apiUrl + "TrainCrewAssignments/GetTrainCrewAssignments").Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var responseMessage = response.Content.ReadAsStringAsync().Result;
                                var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);
                                assignment = allAssignments.FirstOrDefault(a => a.crewAssignId != null && a.crewAssignId.ToString() == assignId);
                            }
                        }
                    }
                }

                if (assignment != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Assignment found: {JsonConvert.SerializeObject(assignment)}");

                   
                    string routeId = null;
                    if (assignment.tripId != null)
                        routeId = assignment.tripId.ToString();
                    else if (assignment.trainRouteId != null)
                        routeId = assignment.trainRouteId.ToString();

                    if (!string.IsNullOrEmpty(routeId))
                    {
                        var routeItem = ddlEditRoutes.Items.FindByValue(routeId);
                        if (routeItem != null)
                            ddlEditRoutes.SelectedValue = routeId;
                        else
                            System.Diagnostics.Debug.WriteLine($"Route {routeId} not found in dropdown. Available: {string.Join(", ", ddlEditRoutes.Items.Cast<ListItem>().Select(i => i.Value))}");
                    }
                    // FIX 5: Check trainRouteId (not vehicleRouteId) before using it
                    //if (assignment.trainRouteId != null)
                    //{
                    //    string routeId = assignment.trainRouteId.ToString();
                    //    var routeItem = ddlEditRoutes.Items.FindByValue(routeId);
                    //    if (routeItem != null)
                    //        ddlEditRoutes.SelectedValue = routeId;
                    //    else
                    //        System.Diagnostics.Debug.WriteLine($"Route {routeId} not found in dropdown");
                    //}

                    if (assignment.trainId != null)
                    {
                        string trainId = assignment.trainId.ToString();
                        var trainItem = ddlEditTrains.Items.FindByValue(trainId);
                        if (trainItem != null)
                            ddlEditTrains.SelectedValue = trainId;
                        else
                            System.Diagnostics.Debug.WriteLine($"Train {trainId} not found in dropdown");
                    }

                    if (assignment.deviceId != null)
                    {
                        string deviceId = assignment.deviceId.ToString();
                        var deviceItem = ddlEditPos.Items.FindByValue(deviceId);
                        if (deviceItem != null)
                            ddlEditPos.SelectedValue = deviceId;
                        else
                            System.Diagnostics.Debug.WriteLine($"Device {deviceId} not found in dropdown");
                    }

                    if (assignment.driverId != null)
                    {
                        string driverId = assignment.driverId.ToString();
                        var driverItem = ddlEditDrivers.Items.FindByValue(driverId);
                        if (driverItem != null)
                            ddlEditDrivers.SelectedValue = driverId;
                        else
                            System.Diagnostics.Debug.WriteLine($"Driver {driverId} not found in dropdown");
                    }

                    if (assignment.ticketInspectors != null)
                    {
                        string ticketInspectors = assignment.ticketInspectors.ToString();
                        var ticketInspectorItem = ddlEditUsers.Items.FindByValue(ticketInspectors);
                        if (ticketInspectorItem != null)
                            ddlEditUsers.SelectedValue = ticketInspectors;
                        else
                            System.Diagnostics.Debug.WriteLine($"ticketInspector {ticketInspectors} not found in dropdown");
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

        //protected void btnReAssign_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string assignId = hdnEditAssignId.Value;

        //        if (string.IsNullOrEmpty(assignId))
        //        {
        //            ShowEditMessage("Assignment ID not found.", false);
        //            return;
        //        }

        //        if (string.IsNullOrEmpty(ddlEditRoutes.SelectedValue) ||
        //            string.IsNullOrEmpty(ddlEditTrains.SelectedValue) ||
        //            string.IsNullOrEmpty(ddlEditPos.SelectedValue) ||
        //            string.IsNullOrEmpty(ddlEditDrivers.SelectedValue) ||
        //            string.IsNullOrEmpty(ddlEditUsers.SelectedValue))
        //        {
        //            ShowEditMessage("Please select all resources.", false);
        //            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
        //            return;
        //        }

        //        var updateData = new Dictionary<string, object>
        //        {
        //            { "crewAssignId", int.Parse(assignId) },
        //            { "tripId", int.Parse(ddlEditRoutes.SelectedValue) },
        //            { "trainRouteId", int.Parse(ddlEditRoutes.SelectedValue) },
        //            { "trainRouteId", int.Parse(ddlEditRoutes.SelectedValue) },
        //            { "trainId", ddlEditTrains.SelectedValue },
        //            { "deviceId", ddlEditPos.SelectedValue },
        //            { "driverId", ddlEditDrivers.SelectedValue },
        //            { "ticketInspectors", ddlEditUsers.SelectedValue }
        //        };

        //        bool success = UpdateAssignment(assignId, updateData);

        //        if (success)
        //        {
        //            ShowEditMessage("Assignment updated successfully!", true);
        //            LoadCurrentAssignments();
        //            LoadDropdowns();
        //            BindAssignments();
        //        }
        //        else
        //        {
        //            ShowEditMessage("Failed to update assignment.", false);
        //            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowEditMessage($"An error occurred: {ex.Message}", false);
        //        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
        //        System.Diagnostics.Debug.WriteLine($"btnReAssign_Click Error: {ex.Message}");
        //    }
        //}
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

                // currentAssignments mein nahi mila toh directly API se fetch karo
                dynamic currentAssignment = currentAssignments.FirstOrDefault(
                    a => a.crewAssignId != null && a.crewAssignId.ToString() == assignId);

                if (currentAssignment == null)
                {
                    // API se directly fetch karo (ACTIVE + INACTIVE dono)
                    try
                    {
                        using (var httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            if (!string.IsNullOrEmpty(token_sess))
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                            // Pehle single endpoint try karo
                            HttpResponseMessage response = httpClient.GetAsync(
                                apiUrl + $"TrainCrewAssignments/GetTrainCrewAssignment/{assignId}").Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var responseMessage = response.Content.ReadAsStringAsync().Result;
                                currentAssignment = JsonConvert.DeserializeObject<dynamic>(responseMessage);
                            }
                            else
                            {
                                // Fallback: saari assignments mein se dhundo
                                response = httpClient.GetAsync(
                                    apiUrl + "TrainCrewAssignments/GetTrainCrewAssignments").Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var responseMessage = response.Content.ReadAsStringAsync().Result;
                                    var allAssignments = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);
                                    currentAssignment = allAssignments?.FirstOrDefault(
                                        a => a.crewAssignId != null && a.crewAssignId.ToString() == assignId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Fetch assignment error: {ex.Message}");
                    }
                }

                // Ab bhi null hai toh error
                if (currentAssignment == null)
                {
                    ShowEditMessage("Assignment data could not be loaded. Please try again.", false);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                    return;
                }

                // Build update payload — only include fields that were actually selected
                var updateData = new Dictionary<string, object>();
                updateData["crewAssignId"] = int.Parse(assignId);

                if (!string.IsNullOrEmpty(ddlEditRoutes.SelectedValue))
                    updateData["tripId"] = int.Parse(ddlEditRoutes.SelectedValue);
                else if (currentAssignment.tripId != null)
                    updateData["tripId"] = (int)currentAssignment.tripId;

                if (!string.IsNullOrEmpty(ddlEditTrains.SelectedValue))
                    updateData["trainId"] = ddlEditTrains.SelectedValue;
                else if (currentAssignment.trainId != null)
                    updateData["trainId"] = currentAssignment.trainId.ToString();

                if (!string.IsNullOrEmpty(ddlEditPos.SelectedValue))
                    updateData["deviceId"] = ddlEditPos.SelectedValue;
                else if (currentAssignment.deviceId != null)
                    updateData["deviceId"] = currentAssignment.deviceId.ToString();

                if (!string.IsNullOrEmpty(ddlEditDrivers.SelectedValue))
                    updateData["driverId"] = ddlEditDrivers.SelectedValue;
                else if (currentAssignment.driverId != null)
                    updateData["driverId"] = currentAssignment.driverId.ToString();

                if (!string.IsNullOrEmpty(ddlEditUsers.SelectedValue))
                    updateData["ticketInspectors"] = ddlEditUsers.SelectedValue;
                else if (currentAssignment.ticketInspectors != null)
                    updateData["ticketInspectors"] = currentAssignment.ticketInspectors.ToString();

                bool success = UpdateAssignment(assignId, updateData);

                if (success)
                {
                    ShowEditMessage("Assignment updated successfully!", true);
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
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = apiUrl + $"TrainCrewAssignments/PutTrainCrewAssignment/{assignId}";

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
                        ShowMessage("Status updated successfully!", true);
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

        private void StartResourceCheckTimer()
        {
            if (resourceCheckTimer == null)
            {
                resourceCheckTimer = new System.Timers.Timer(60000);
                resourceCheckTimer.Elapsed += CheckResourceAvailability;
                resourceCheckTimer.AutoReset = true;
                resourceCheckTimer.Start();
            }
        }

        private async void CheckResourceAvailability(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Reserved for future background status check logic
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Resource check error: {ex.Message}");
            }
        }

        private async Task<bool> UpdateAssignStatus(int assignId, string newStatus)
        {
            try
            {
                var requestData = new
                {
                    CrewAssignId = assignId,
                    Status = newStatus
                };

                string jsonData = JsonConvert.SerializeObject(requestData);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl + "TrainCrewAssignments/ToggleCrewAssignStatus", content);

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


        private TimeSpan? GetDepartureTime(string tripId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(token_sess))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);

                    HttpResponseMessage response = httpClient.GetAsync(apiUrl + "TrainTrips/GetTrainTrips").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseMessage = response.Content.ReadAsStringAsync().Result;
                        var trips = JsonConvert.DeserializeObject<List<dynamic>>(responseMessage);

                        var trip = trips?.FirstOrDefault(t => t.tripId != null && t.tripId.ToString() == tripId);
                        if (trip != null && trip.departureTime != null)
                        {
                            if (TimeSpan.TryParse(trip.departureTime.ToString(), out TimeSpan dept))
                                return dept;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetDepartureTime Error: {ex.Message}");
            }
            return null;
        }




        // ---- Model Classes ----

        public class AssignmentModel
        {
            public int CrewAssignId { get; set; }
            public int RouteId { get; set; }
            public string RouteName { get; set; }
            public string TrainId { get; set; }
            public string TrainNo { get; set; }
            public string DeviceId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string TicketInspectors { get; set; }
            public string TicketInspectorName { get; set; }
            public DateTime AssignedAt { get; set; }
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
            public string TrainUserId { get; set; }
            public DateTime AttandanceDate { get; set; }
            public string Status { get; set; }
        }

        public class DeviceModel
        {
            public string DeviceId { get; set; }
            public bool IsActive { get; set; }
        }

        public class TrainModel
        {
            public string TrainId { get; set; }
            public string TrainNo { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
