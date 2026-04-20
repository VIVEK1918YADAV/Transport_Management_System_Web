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
    public partial class TrainRoutes : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TrainRoutes()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Page Load
        // ────────────────────────────────────────────────────────────────────────────

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await LoadStations();

                if (!IsPostBack)
                    await LoadRoutes();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load stations into both dropdowns
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadStations()
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync("RailwayStations/GetRailwayStations");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<StationDropdownDto> stations =
                        JsonConvert.DeserializeObject<List<StationDropdownDto>>(json);

                    // preserve selected values across postbacks
                    string selStart = ddlStartStation.SelectedValue;
                    string selEnd = ddlEndStation.SelectedValue;

                    ddlStartStation.Items.Clear();
                    ddlStartStation.Items.Add(new ListItem("Select start station", ""));
                    ddlEndStation.Items.Clear();
                    ddlEndStation.Items.Add(new ListItem("Select end station", ""));

                    foreach (var s in stations)
                    {
                        string label = $"{s.StationName} ({s.StationCode})";
                        ddlStartStation.Items.Add(new ListItem(label, s.StationId.ToString()));
                        ddlEndStation.Items.Add(new ListItem(label, s.StationId.ToString()));
                    }

                    if (!string.IsNullOrEmpty(selStart) &&
                        ddlStartStation.Items.FindByValue(selStart) != null)
                        ddlStartStation.SelectedValue = selStart;

                    if (!string.IsNullOrEmpty(selEnd) &&
                        ddlEndStation.Items.FindByValue(selEnd) != null)
                        ddlEndStation.SelectedValue = selEnd;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading stations: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load all routes into GridView
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadRoutes()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainRoutes/GetTrainRoutes");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<TrainRouteDto> routes =
                        JsonConvert.DeserializeObject<List<TrainRouteDto>>(json);

                    routes = routes.OrderByDescending(r => r.CreatedAt).ToList();

                    gvRoutes.DataSource = routes;
                    gvRoutes.DataBind();
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

        // ────────────────────────────────────────────────────────────────────────────
        // Load single route for editing
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadRouteForEdit(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync($"TrainRoutes/GetTrainRoute/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TrainRouteEditDto route =
                        JsonConvert.DeserializeObject<TrainRouteEditDto>(json);

                    if (route != null)
                    {
                        hfRouteId.Value = route.RouteId.ToString();
                        hfIsEdit.Value = "true";
                        txtRouteName.Text = route.RouteName ?? "";
                        txtRouteCode.Text = route.RouteCode ?? "";
                        txtIntermediateStations.Text = route.IntermediateStations ?? "";
                        lblModalTitle.Text = "Edit Train Route";

                        string startVal = route.StartStationId.ToString();
                        if (ddlStartStation.Items.FindByValue(startVal) != null)
                            ddlStartStation.SelectedValue = startVal;

                        string endVal = route.EndStationId.ToString();
                        if (ddlEndStation.Items.FindByValue(endVal) != null)
                            ddlEndStation.SelectedValue = endVal;
                    }
                    else
                    {
                        ShowError("Route not found.");
                    }
                }
                else
                {
                    ShowError("Failed to load route details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading route: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Submit button – Add or Update
        // ────────────────────────────────────────────────────────────────────────────

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (hfIsEdit.Value == "true")
                    await UpdateRoute();
                else
                    await AddRoute();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Add new route
        // ────────────────────────────────────────────────────────────────────────────

        private async Task AddRoute()
        {
            try
            {
                var requestData = new
                {
                    routeName = txtRouteName.Text.Trim(),
                    routeCode = txtRouteCode.Text.Trim().ToUpper(),
                    startStationId = int.Parse(ddlStartStation.SelectedValue),
                    endStationId = int.Parse(ddlEndStation.SelectedValue),
                    intermediateStations = txtIntermediateStations.Text.Trim(),
                    isActive = true,
                    status = "ACTIVE",
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TrainRoutes/PostTrainRoute", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadRoutes();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Train route added successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add route: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding route: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Update existing route
        // ────────────────────────────────────────────────────────────────────────────

        private async Task UpdateRoute()
        {
            try
            {
                int routeId = int.Parse(hfRouteId.Value);

                // Fetch current to preserve status/isActive
                HttpResponseMessage getResponse =
                    await client.GetAsync($"TrainRoutes/GetTrainRoute/{routeId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load route details.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                TrainRouteEditDto current =
                    JsonConvert.DeserializeObject<TrainRouteEditDto>(currentJson);

                if (current == null)
                {
                    ShowError("Route not found.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                var requestData = new
                {
                    routeId = routeId,
                    routeName = txtRouteName.Text.Trim(),
                    routeCode = txtRouteCode.Text.Trim().ToUpper(),
                    startStationId = int.Parse(ddlStartStation.SelectedValue),
                    endStationId = int.Parse(ddlEndStation.SelectedValue),
                    intermediateStations = txtIntermediateStations.Text.Trim(),
                    isActive = current.IsActive,
                    status = current.Status,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync($"TrainRoutes/PutTrainRoute/{routeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadRoutes();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Train route updated successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update route: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating route: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GridView row commands
        // ────────────────────────────────────────────────────────────────────────────

        protected void gvRoutes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int routeId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRoute")
                RegisterAsyncTask(new PageAsyncTask(() => LoadRouteForEdit(routeId)));
            else if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(routeId)));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Toggle ACTIVE / INACTIVE
        // ────────────────────────────────────────────────────────────────────────────

        private async Task ToggleStatus(int routeId)
        {
            try
            {
                // Fetch current record first
                HttpResponseMessage getResponse =
                    await client.GetAsync($"TrainRoutes/GetTrainRoute/{routeId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load route details.");
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                TrainRouteEditDto current =
                    JsonConvert.DeserializeObject<TrainRouteEditDto>(currentJson);

                if (current == null) { ShowError("Route not found."); return; }

                bool newIsActive = !(current.IsActive == true);
                string newStatus = newIsActive ? "ACTIVE" : "INACTIVE";

                var requestData = new
                {
                    routeId = routeId,
                    routeName = current.RouteName,
                    routeCode = current.RouteCode,
                    startStationId = current.StartStationId,
                    endStationId = current.EndStationId,
                    intermediateStations = current.IntermediateStations,
                    isActive = newIsActive,
                    status = newStatus,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync($"TrainRoutes/PutTrainRoute/{routeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadRoutes();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Route status updated successfully!');", true);
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

        // ────────────────────────────────────────────────────────────────────────────
        // Helper methods called from ASPX markup
        // ────────────────────────────────────────────────────────────────────────────

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
            txtRouteName.Text = "";
            txtRouteCode.Text = "";
            ddlStartStation.SelectedIndex = 0;
            ddlEndStation.SelectedIndex = 0;
            txtIntermediateStations.Text = "";
            hfRouteId.Value = "";
            hfIsEdit.Value = "false";
            lblModalTitle.Text = "Add New Train Route";
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // DTOs
    // ────────────────────────────────────────────────────────────────────────────────

    // GridView binding — joined response from GetTrainRoutes
    public class TrainRouteDto
    {
        [JsonProperty("routeId")]
        public int RouteId { get; set; }

        [JsonProperty("routeName")]
        public string RouteName { get; set; }

        [JsonProperty("routeCode")]
        public string RouteCode { get; set; }

        [JsonProperty("startStationId")]
        public int StartStationId { get; set; }

        [JsonProperty("startStation")]
        public string StartStation { get; set; }

        [JsonProperty("startStationCode")]
        public string StartStationCode { get; set; }

        [JsonProperty("endStationId")]
        public int EndStationId { get; set; }

        [JsonProperty("endStation")]
        public string EndStation { get; set; }

        [JsonProperty("endStationCode")]
        public string EndStationCode { get; set; }

        [JsonProperty("intermediateStations")]
        public string IntermediateStations { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    // Edit form — raw model from GetTrainRoute/{id}
    public class TrainRouteEditDto
    {
        [JsonProperty("routeId")]
        public int RouteId { get; set; }

        [JsonProperty("routeName")]
        public string RouteName { get; set; }

        [JsonProperty("routeCode")]
        public string RouteCode { get; set; }

        [JsonProperty("startStationId")]
        public int StartStationId { get; set; }

        [JsonProperty("endStationId")]
        public int EndStationId { get; set; }

        [JsonProperty("intermediateStations")]
        public string IntermediateStations { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    // Station dropdown population
    public class StationDropdownDto
    {
        [JsonProperty("stationId")]
        public int StationId { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("stationCode")]
        public string StationCode { get; set; }
    }
}