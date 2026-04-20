using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Route : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Route()
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
                LoadAllRoutesSync();
                LoadCountersSync();
            }
        }

        private void LoadAllRoutesSync()
        {
            RegisterAsyncTask(new PageAsyncTask(async () => await LoadAllRoutes()));
        }

        private void LoadCountersSync()
        {
            RegisterAsyncTask(new PageAsyncTask(async () => await LoadCounters()));
        }

        private async Task LoadAllRoutes(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;

                string endpoint = "VehicleRoutes/GetVehicleRoutes";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleRouteDto> routes = JsonConvert.DeserializeObject<List<VehicleRouteDto>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        routes = routes.Where(r =>
                            (!string.IsNullOrEmpty(r.Name) && r.Name.ToLower().Contains(searchTerm.ToLower()))
                        ).ToList();
                    }

                    var orderedRoutes = routes.OrderByDescending(r => r.CreatedAt).ToList();

                    ViewState["AllRoutes"] = JsonConvert.SerializeObject(orderedRoutes);

                    gvRoutes.DataSource = orderedRoutes;
                    gvRoutes.DataBind();
                }
                else
                {
                    ShowError($"Failed to load routes. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading routes: {ex.Message}");
            }
        }

        private async Task LoadCounters()
        {
            try
            {
                string endpoint = "Counters/GetCounters";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<CounterDto> counters = JsonConvert.DeserializeObject<List<CounterDto>>(jsonResponse);

                    var activeCounters = counters.Where(c => c.Status == "1").ToList();  // ✅ Changed to string comparison

                    ViewState["Counters"] = JsonConvert.SerializeObject(activeCounters);

                    ddlStartFrom.DataSource = activeCounters;
                    ddlStartFrom.DataTextField = "Name";
                    ddlStartFrom.DataValueField = "Id";
                    ddlStartFrom.DataBind();
                    ddlStartFrom.Items.Insert(0, new ListItem("Select an option", ""));

                    ddlEndTo.DataSource = activeCounters;
                    ddlEndTo.DataTextField = "Name";
                    ddlEndTo.DataValueField = "Id";
                    ddlEndTo.DataBind();
                    ddlEndTo.Items.Insert(0, new ListItem("Select an option", ""));

                    ddlEditStartFrom.DataSource = activeCounters;
                    ddlEditStartFrom.DataTextField = "Name";
                    ddlEditStartFrom.DataValueField = "Id";
                    ddlEditStartFrom.DataBind();

                    ddlEditEndTo.DataSource = activeCounters;
                    ddlEditEndTo.DataTextField = "Name";
                    ddlEditEndTo.DataValueField = "Id";
                    ddlEditEndTo.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading counters: {ex.Message}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllRoutes(searchTerm)));
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearCreateForm();

            if (ddlStartFrom.Items.Count == 0)
            {
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadCounters();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowCreateModal",
                        "Sys.Application.add_load(function() { showCreateModal(); });", true);
                }));
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowCreateModal",
                    "Sys.Application.add_load(function() { showCreateModal(); });", true);
            }
        }

        protected void btnSubmitCreate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                RegisterAsyncTask(new PageAsyncTask(CreateRoute));
            }
        }

        private async Task CreateRoute()
        {
            try
            {
                // Handle empty time and distance with defaults or empty string
                string time = string.IsNullOrWhiteSpace(txtTime.Text) ? "" : txtTime.Text.Trim();
                string distance = string.IsNullOrWhiteSpace(txtDistance.Text) ? "" : txtDistance.Text.Trim();

                var newRoute = new
                {
                    name = txtName.Text.Trim(),
                    startFrom = Convert.ToInt32(ddlStartFrom.SelectedValue),
                    endTo = Convert.ToInt32(ddlEndTo.SelectedValue),
                    distance = distance,
                    time = time
                };

                string json = JsonConvert.SerializeObject(newRoute);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("VehicleRoutes/PostVehicleRoute", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllRoutes();
                    ClearCreateForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideCreateModal(); showSuccess('Route created successfully!');", true);
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to create route: {errorMsg}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowCreateModal",
                        "showCreateModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error creating route: {ex.Message}");

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowCreateModal",
                    "showCreateModal();", true);
            }
        }

        protected void gvRoutes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int routeId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRoute")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadRouteForEdit(routeId)));
            }
            else if (e.CommandName == "DisableRoute")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleRouteStatus(routeId)));
            }
        }


        private async Task LoadRouteForEdit(int routeId)
        {
            try
            {
                if (ddlEditStartFrom.Items.Count == 0)
                {
                    await LoadCounters();
                }

                string endpoint = $"VehicleRoutes/GetVehicleRoutes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleRouteDto> routes = JsonConvert.DeserializeObject<List<VehicleRouteDto>>(jsonResponse);

                    VehicleRouteDto route = routes.FirstOrDefault(r => r.Id == routeId);

                    if (route != null)
                    {
                        hfEditRouteId.Value = route.Id.ToString();
                        txtEditName.Text = route.Name;

                        if (ViewState["Counters"] != null)
                        {
                            string json = ViewState["Counters"].ToString();
                            List<CounterDto> counters = JsonConvert.DeserializeObject<List<CounterDto>>(json);

                            var startCounter = counters.FirstOrDefault(c =>
                                c.Name.Equals(route.StartFrom, StringComparison.OrdinalIgnoreCase));

                            if (startCounter != null && ddlEditStartFrom.Items.FindByValue(startCounter.Id.ToString()) != null)
                            {
                                ddlEditStartFrom.SelectedValue = startCounter.Id.ToString();
                            }

                            var endCounter = counters.FirstOrDefault(c =>
                                c.Name.Equals(route.EndTo, StringComparison.OrdinalIgnoreCase));

                            if (endCounter != null && ddlEditEndTo.Items.FindByValue(endCounter.Id.ToString()) != null)
                            {
                                ddlEditEndTo.SelectedValue = endCounter.Id.ToString();
                            }
                        }

                        txtEditDistance.Text = route.Distance.Replace("km", "").Replace("KM", "").Trim();
                        txtEditTime.Text = route.Time.Replace("hours", "").Replace("hour", "").Trim();

                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                            "Sys.Application.add_load(function() { showEditModal(); });", true);
                    }
                    else
                    {
                        ShowError("Route not found");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading route: {ex.Message}");
            }
        }

        protected void btnSubmitEdit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                RegisterAsyncTask(new PageAsyncTask(UpdateRoute));
            }
        }

        private async Task UpdateRoute()
        {
            try
            {
                int routeId = Convert.ToInt32(hfEditRouteId.Value);

                // Handle empty time and distance with defaults or empty string
                string time = string.IsNullOrWhiteSpace(txtEditTime.Text) ? "" : txtEditTime.Text.Trim();
                string distance = string.IsNullOrWhiteSpace(txtEditDistance.Text) ? "" : txtEditDistance.Text.Trim();

                var updateRoute = new
                {
                    id = routeId,
                    name = txtEditName.Text.Trim(),
                    startFrom = Convert.ToInt32(ddlEditStartFrom.SelectedValue),
                    endTo = Convert.ToInt32(ddlEditEndTo.SelectedValue),
                    distance = distance,
                    time = time
                };

                string json = JsonConvert.SerializeObject(updateRoute);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"VehicleRoutes/PutVehicleRoute/{routeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllRoutes();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideEditModal(); showSuccess('Route updated successfully!');", true);
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update route: {errorMsg}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        "showEditModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating route: {ex.Message}");

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                    "showEditModal();", true);
            }
        }

        private async Task ToggleRouteStatus(int routeId)
        {
            try
            {
                string endpoint = $"VehicleRoutes/GetVehicleRoutes";
                HttpResponseMessage getResponse = await client.GetAsync(endpoint);

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to fetch route details");
                    return;
                }

                string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                List<VehicleRouteDto> routes = JsonConvert.DeserializeObject<List<VehicleRouteDto>>(jsonResponse);
                VehicleRouteDto route = routes.FirstOrDefault(r => r.Id == routeId);

                if (route == null)
                {
                    ShowError("Route not found");
                    return;
                }

                // ✅ Toggle status using string comparison
                string newStatus = route.Status == "1" ? "0" : "1";

                int startFromId = 0;
                int endToId = 0;

                if (ViewState["Counters"] != null)
                {
                    string countersJson = ViewState["Counters"].ToString();
                    List<CounterDto> counters = JsonConvert.DeserializeObject<List<CounterDto>>(countersJson);

                    var startCounter = counters.FirstOrDefault(c =>
                        c.Name.Equals(route.StartFrom, StringComparison.OrdinalIgnoreCase));
                    var endCounter = counters.FirstOrDefault(c =>
                        c.Name.Equals(route.EndTo, StringComparison.OrdinalIgnoreCase));

                    if (startCounter != null)
                        startFromId = startCounter.Id;
                    if (endCounter != null)
                        endToId = endCounter.Id;
                }

                if (startFromId == 0 || endToId == 0)
                {
                    ShowError("Failed to resolve counter IDs");
                    return;
                }

                string cleanDistance = route.Distance.Replace("km", "").Replace("KM", "").Trim();
                string cleanTime = route.Time.Replace("hours", "").Replace("hour", "").Trim();

                var updateRoute = new
                {
                    id = routeId,
                    name = route.Name,
                    startFrom = startFromId,
                    endTo = endToId,
                    distance = cleanDistance,
                    time = cleanTime,
                    status = newStatus  // ✅ Now string
                };

                string json = JsonConvert.SerializeObject(updateRoute);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"VehicleRoutes/PutVehicleRoute/{routeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllRoutes();

                    ScriptManager.RegisterStartupScript(this, GetType(), "StatusChanged",
                        "showSuccess('Route status updated successfully!');", true);
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update status: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }


        protected void gvRoutes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                VehicleRouteDto route = (VehicleRouteDto)e.Row.DataItem;

                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus != null)
                {
                    // ✅ Changed to string comparison
                    lblStatus.Text = route.Status == "1" ? "Enabled" : "Disabled";
                    lblStatus.CssClass = route.Status == "1" ? "status-badge badge-success" : "status-badge badge-danger";
                }

                LinkButton btnDisable = (LinkButton)e.Row.FindControl("btnDisable");
                if (btnDisable != null)
                {
                    // ✅ Changed to string comparison
                    btnDisable.Text = route.Status == "1" ? "🚫 Disable" : "✓ Enable";
                    btnDisable.CssClass = route.Status == "1" ? "btn-disable" : "btn-edit";
                }

                Literal litStartFrom = (Literal)e.Row.FindControl("litStartFrom");
                Literal litEndTo = (Literal)e.Row.FindControl("litEndTo");

                if (litStartFrom != null)
                    litStartFrom.Text = route.StartFrom;

                if (litEndTo != null)
                    litEndTo.Text = route.EndTo;
            }
        }

        private void ClearCreateForm()
        {
            txtName.Text = "";
            if (ddlStartFrom.Items.Count > 0)
                ddlStartFrom.SelectedIndex = 0;
            if (ddlEndTo.Items.Count > 0)
                ddlEndTo.SelectedIndex = 0;
            txtDistance.Text = "";
            txtTime.Text = "";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }



    public class VehicleRouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartFrom { get; set; }
        public string EndTo { get; set; }
        public string Stoppages { get; set; }
        public string Distance { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }  // ✅ Changed from int to string
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CounterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string Mobile { get; set; }
        public string Status { get; set; }  // ✅ Changed from short? to string
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}