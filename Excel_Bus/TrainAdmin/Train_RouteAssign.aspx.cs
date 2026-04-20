using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_RouteAssign : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_RouteAssign()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];
            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Page Load
        // ─────────────────────────────────────────────────────────────────────────

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await LoadRoutesDropdown();
                await LoadTrainsDropdown();

                if (!IsPostBack)
                    await LoadAssignedTrains();
            }));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Load Routes into dropdown  (value = RouteId, displayed = RouteName)
        // ─────────────────────────────────────────────────────────────────────────

        private async Task LoadRoutesDropdown()
        {
            try
            {
                string selVal = ddlTrip.SelectedValue;   // ddlTrip control re-used for Routes

                HttpResponseMessage response =
                    await client.GetAsync("TrainRoutes/GetActiveTrainRoutes");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<RouteDropdownDto> routes =
                        JsonConvert.DeserializeObject<List<RouteDropdownDto>>(json);

                    ddlTrip.Items.Clear();
                    ddlTrip.Items.Add(new ListItem("Select a route", ""));

                    foreach (var r in routes)
                    {
                        string label = $"{r.RouteName} ({r.RouteCode})";
                        ddlTrip.Items.Add(new ListItem(label, r.RouteId.ToString()));
                    }

                    if (!string.IsNullOrEmpty(selVal) &&
                        ddlTrip.Items.FindByValue(selVal) != null)
                        ddlTrip.SelectedValue = selVal;
                }
                else
                {
                    ShowError($"Failed to load routes. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading routes: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Load Trains into dropdown
        // ─────────────────────────────────────────────────────────────────────────

        private async Task LoadTrainsDropdown()
        {
            try
            {
                string selVal = ddlTrain.SelectedValue;

                HttpResponseMessage response = await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<TrainDto> trains = JsonConvert.DeserializeObject<List<TrainDto>>(json);

                    ddlTrain.Items.Clear();
                    ddlTrain.Items.Add(new ListItem("Select a train", ""));

                    foreach (var tr in trains)
                    {
                        string label = $"{tr.TrainName} ({tr.TrainNumber ?? tr.TrainId})";
                        ddlTrain.Items.Add(new ListItem(label, tr.TrainId));
                    }

                    if (!string.IsNullOrEmpty(selVal) &&
                        ddlTrain.Items.FindByValue(selVal) != null)
                        ddlTrain.SelectedValue = selVal;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trains: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Load all assigned trains into GridView
        // ─────────────────────────────────────────────────────────────────────────

        private async Task LoadAssignedTrains()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainAssigneds/GetTrainAssigneds");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<AssignedTrainViewDto> list =
                        JsonConvert.DeserializeObject<List<AssignedTrainViewDto>>(json);

                    // Enrich with Route name and Reg number from joined data
                    // (API already returns TrainName, TrainNumber from Include)
                    // Fetch trips once to map RouteName
                    HttpResponseMessage tripsResp = await client.GetAsync("TrainTrips/GetTrainTrips");
                    List<TripDto> trips = new List<TripDto>();
                    if (tripsResp.IsSuccessStatusCode)
                    {
                        string tj = await tripsResp.Content.ReadAsStringAsync();
                        trips = JsonConvert.DeserializeObject<List<TripDto>>(tj);
                    }

                    // Fetch trains to get RegistrationNumber
                    HttpResponseMessage trainsResp = await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");
                    List<TrainDto> trains = new List<TrainDto>();
                    if (trainsResp.IsSuccessStatusCode)
                    {
                        string vj = await trainsResp.Content.ReadAsStringAsync();
                        trains = JsonConvert.DeserializeObject<List<TrainDto>>(vj);
                    }

                    foreach (var item in list)
                    {
                        var trip = trips.FirstOrDefault(t => t.TripId == item.TripId);
                        if (trip != null && string.IsNullOrEmpty(item.RouteName))
                            item.RouteName = trip.RouteName ?? $"Trip #{item.TripId}";

                        var train = trains.FirstOrDefault(t => t.TrainId == item.TrainId);
                        if (train != null)
                        {
                            if (string.IsNullOrEmpty(item.TrainName))
                                item.TrainName = train.TrainName;
                            if (string.IsNullOrEmpty(item.TrainNumber))
                                item.TrainNumber = train.TrainNumber ?? train.TrainId;
                            item.RegisterNo = train.RegistrationNumber ?? "N/A";
                        }
                    }

                    list = list.OrderByDescending(x => x.CreatedAt).ToList();

                    // ── Populate hidden field for JS duplicate-train check ────────
                    var activeAssigned = list
                        .Where(x => x.Status == "ACTIVE")
                        .Select(x => new { trainId = x.TrainId, routeName = x.RouteName ?? $"Trip #{x.TripId}" })
                        .ToList();
                    hfAssignedTrainIds.Value = JsonConvert.SerializeObject(activeAssigned);

                    gvAssignedTrains.DataSource = list;
                    gvAssignedTrains.DataBind();
                }
                else
                {
                    ShowError($"Failed to load assigned trains. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading assigned trains: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Submit – Add assignment
        // ─────────────────────────────────────────────────────────────────────────

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(AddAssignment));
        }

        private async Task AddAssignment()
        {
            try
            {
                if (string.IsNullOrEmpty(ddlTrip.SelectedValue) ||
                    string.IsNullOrEmpty(ddlTrain.SelectedValue))
                {
                    ShowError("Please select both a route and a train.");
                    hfKeepModalOpen.Value = "true";
                    return;
                }

                int selectedRouteId = int.Parse(ddlTrip.SelectedValue);
                string selectedTrainId = ddlTrain.SelectedValue;

                // ── Server-side duplicate check: is this train already ACTIVE on any route? ──
                HttpResponseMessage activeResp =
                    await client.GetAsync("TrainAssigneds/GetTrainAssigneds");

                if (activeResp.IsSuccessStatusCode)
                {
                    string activeJson = await activeResp.Content.ReadAsStringAsync();
                    List<AssignedTrainViewDto> allAssigned =
                        JsonConvert.DeserializeObject<List<AssignedTrainViewDto>>(activeJson);

                    var conflict = allAssigned.FirstOrDefault(a =>
                        a.TrainId == selectedTrainId && a.Status == "ACTIVE");

                    if (conflict != null)
                    {
                        // Get the route name for the conflict
                        string conflictRoute = !string.IsNullOrEmpty(conflict.RouteName)
                            ? conflict.RouteName
                            : $"Trip #{conflict.TripId}";

                        ShowError($"This train is already assigned to route \"{conflictRoute}\". " +
                                  $"A train cannot be assigned to more than one route at a time.");
                        hfKeepModalOpen.Value = "true";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                            "showModal();", true);
                        return;
                    }
                }

                // ── Find the Trip that belongs to the selected Route ──────────────
                HttpResponseMessage tripsResp =
                    await client.GetAsync("TrainTrips/GetTrainTrips");

                if (!tripsResp.IsSuccessStatusCode)
                {
                    ShowError("Failed to load trips. Please try again.");
                    hfKeepModalOpen.Value = "true";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string tripsJson = await tripsResp.Content.ReadAsStringAsync();
                List<TripDto> trips = JsonConvert.DeserializeObject<List<TripDto>>(tripsJson);

                TripDto matchedTrip = trips.FirstOrDefault(t => t.RouteId == selectedRouteId);

                if (matchedTrip == null)
                {
                    ShowError("No trip found for the selected route. Please create a trip for this route first.");
                    hfKeepModalOpen.Value = "true";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                var requestData = new
                {
                    tripId = matchedTrip.TripId,
                    trainId = selectedTrainId,
                    status = "ACTIVE",
                    isActive = true,
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "Admin",
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TrainAssigneds/PostTrainAssigned", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAssignedTrains();
                    ClearForm();
                    hfKeepModalOpen.Value = "false";
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Train assigned successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to assign train: {error}");
                    hfKeepModalOpen.Value = "true";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error assigning train: {ex.Message}");
                hfKeepModalOpen.Value = "true";
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GridView row commands
        // ─────────────────────────────────────────────────────────────────────────

        protected void gvAssignedTrains_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int assignId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(assignId)));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Toggle ACTIVE / INACTIVE
        // ─────────────────────────────────────────────────────────────────────────

        private async Task ToggleStatus(int assignId)
        {
            try
            {
                // Fetch the single record
                HttpResponseMessage getResponse =
                    await client.GetAsync($"TrainAssigneds/GetTrainAssigned/{assignId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load assignment details.");
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                TrainAssignedRawDto current =
                    JsonConvert.DeserializeObject<TrainAssignedRawDto>(currentJson);

                if (current == null) { ShowError("Assignment not found."); return; }

                bool newIsActive = !(current.IsActive == true);
                string newStatus = newIsActive ? "ACTIVE" : "INACTIVE";

                var requestData = new
                {
                    trainAssignId = assignId,
                    tripId = current.TripId,
                    trainId = current.TrainId,
                    startFrom = current.StartFrom,
                    endAt = current.EndAt,
                    isActive = newIsActive,
                    status = newStatus,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin",
                    updatedAt = DateTime.Now
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync($"TrainAssigneds/PutTrainAssigned/{assignId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAssignedTrains();
                    ScriptManager.RegisterStartupScript(this, GetType(), "StatusSuccess",
                        "showSuccess('Assignment status updated successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update status: {error}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Helper methods
        // ─────────────────────────────────────────────────────────────────────────

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "status-badge";
            return status.ToString() == "ACTIVE"
                ? "status-badge status-enabled"
                : "status-badge status-disabled";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                $"document.getElementById('{pnlError.ClientID}').classList.add('show');", true);
        }

        private void ClearForm()
        {
            ddlTrip.SelectedIndex = 0;
            ddlTrain.SelectedIndex = 0;
            hfKeepModalOpen.Value = "false";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // DTOs
    // ─────────────────────────────────────────────────────────────────────────────

    // GridView binding
    public class AssignedTrainViewDto
    {
        [JsonProperty("trainAssignId")]
        public int TrainAssignId { get; set; }

        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        // Enriched client-side
        public string RouteName { get; set; }
        public string RegisterNo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    // Raw model from GetTrainAssigned/{id} for toggle
    public class TrainAssignedRawDto
    {
        [JsonProperty("trainAssignId")]
        public int TrainAssignId { get; set; }

        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("startFrom")]
        public string StartFrom { get; set; }

        [JsonProperty("endAt")]
        public string EndAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }
    }

    // Route dropdown  (value = RouteId)
    public class RouteDropdownDto
    {
        [JsonProperty("routeId")]
        public int RouteId { get; set; }

        [JsonProperty("routeName")]
        public string RouteName { get; set; }

        [JsonProperty("routeCode")]
        public string RouteCode { get; set; }
    }

    // Trip – used only internally to resolve RouteId → TripId on submit
    public class TripDto
    {
        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("routeId")]
        public int RouteId { get; set; }

        [JsonProperty("routeName")]
        public string RouteName { get; set; }
    }

    // Train dropdown
    public class TrainDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }
    }
}