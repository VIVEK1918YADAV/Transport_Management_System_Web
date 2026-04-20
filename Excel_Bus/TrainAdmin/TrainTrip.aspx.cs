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
using static Excel_Bus.Select_Seat;
using static Excel_Bus.TrainTicket;

namespace Excel_Bus.TrainAdmin
{
    public partial class TrainTrip : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TrainTrip()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];
            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadTrips();
                    await LoadDropdownData();
                }));
            }
        }

        private async Task LoadTrips(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response = await client.GetAsync("TrainTrips/GetTrainTrips");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainTripModel> trips = JsonConvert.DeserializeObject<List<TrainTripModel>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        trips = trips.Where(t =>
                            (!string.IsNullOrEmpty(t.RouteName) && t.RouteName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(t.TrainName) && t.TrainName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(t.TrainNumber) && t.TrainNumber.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(t.Status) && t.Status.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                    }

                    var orderedTrips = trips.OrderByDescending(t => t.CreatedAt).ToList();
                    ViewState["AllTrips"] = JsonConvert.SerializeObject(orderedTrips);

                    gvTrips.DataSource = orderedTrips;
                    gvTrips.DataBind();
                }
                else
                {
                    ShowError($"Failed to load trips. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trips: {ex.Message}");
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                var fleetTypesResponse = await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes");
                if (fleetTypesResponse.IsSuccessStatusCode)
                {
                    string json = await fleetTypesResponse.Content.ReadAsStringAsync();
                    var fleetTypes = JsonConvert.DeserializeObject<List<DropdownItem>>(json);

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in fleetTypes)
                        ddlFleetType.Items.Add(new ListItem(item.Name, item.FleetTypeId.ToString()));
                }

                var trainResponse = await client.GetAsync("TblTrainsRegs/GetActiveTrains");
                if (trainResponse.IsSuccessStatusCode)
                {
                    string json = await trainResponse.Content.ReadAsStringAsync();
                    var trains = JsonConvert.DeserializeObject<List<TrainNumber_>>(json);

                    ddlTrainNo.Items.Clear();
                    ddlTrainNo.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in trains)
                    {
                        string displayText = item.TrainNumber + " - " + item.TrainName;
                        ddlTrainNo.Items.Add(new ListItem(displayText, item.TrainId));
                    }
                }

                var routesResponse = await client.GetAsync("TrainRoutes/GetActiveTrainRoutes");
                if (routesResponse.IsSuccessStatusCode)
                {
                    string json = await routesResponse.Content.ReadAsStringAsync();
                    var routes = JsonConvert.DeserializeObject<List<TrainRoute>>(json);

                    ddlRoute.Items.Clear();
                    ddlRoute.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in routes)
                        ddlRoute.Items.Add(new ListItem(item.RouteName, item.RouteId.ToString()));
                }

                var schedulesResponse = await client.GetAsync("TrainSchedules/GetActiveTrainSchedules");
                if (schedulesResponse.IsSuccessStatusCode)
                {
                    string json = await schedulesResponse.Content.ReadAsStringAsync();
                    var schedules = JsonConvert.DeserializeObject<List<ScheduleItem>>(json);

                    ddlSchedule.Items.Clear();
                    ddlSchedule.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in schedules)
                        ddlSchedule.Items.Add(new ListItem(item.DepartureTime, item.ScheduleId.ToString()));
                }

                var countersResponse = await client.GetAsync("RailwayStations/GetRailwayStations");
                if (countersResponse.IsSuccessStatusCode)
                {
                    string json = await countersResponse.Content.ReadAsStringAsync();
                    var stations = JsonConvert.DeserializeObject<List<RailwayStation>>(json);

                    ddlStartFrom.Items.Clear();
                    ddlStartFrom.Items.Add(new ListItem("Select an option", ""));
                    ddlEndTo.Items.Clear();
                    ddlEndTo.Items.Add(new ListItem("Select an option", ""));

                    foreach (var item in stations)
                    {
                        ddlStartFrom.Items.Add(new ListItem(item.StationName, item.StationId.ToString()));
                        ddlEndTo.Items.Add(new ListItem(item.StationName, item.StationId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading dropdown data: {ex.Message}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await LoadTrips(searchTerm);
            }));
        }

        protected void gvTrips_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                int tripId = Convert.ToInt32(e.CommandArgument);
                RegisterAsyncTask(new PageAsyncTask(() => ToggleTripStatus(tripId)));
            }
        }

        private async Task LoadTripForEdit(int tripId)
        {
            try
            {
                await LoadDropdownData();

                HttpResponseMessage response = await client.GetAsync($"TrainTrips/GetTrainTrip/{tripId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    TripDetailModel trip = JsonConvert.DeserializeObject<TripDetailModel>(jsonResponse);

                    hdnTripId.Value = trip.Id.ToString();
                    txtTitle.Text = trip.Title;

                    // Simple DropDownList — pehli FleetTypeId se select karo
                    if (trip.FleetTypeId != null && trip.FleetTypeId.Count > 0)
                    {
                        if (ddlFleetType.Items.FindByValue(trip.FleetTypeId[0].ToString()) != null)
                            ddlFleetType.SelectedValue = trip.FleetTypeId[0].ToString();
                    }

                    if (ddlTrainNo.Items.FindByValue(trip.TrainId) != null)
                        ddlTrainNo.SelectedValue = trip.TrainId;

                    if (ddlRoute.Items.FindByValue(trip.RouteId.ToString()) != null)
                        ddlRoute.SelectedValue = trip.RouteId.ToString();

                    if (ddlSchedule.Items.FindByValue(trip.ScheduleId.ToString()) != null)
                        ddlSchedule.SelectedValue = trip.ScheduleId.ToString();

                    if (ddlStartFrom.Items.FindByValue(trip.StartFrom.ToString()) != null)
                        ddlStartFrom.SelectedValue = trip.StartFrom.ToString();

                    if (ddlEndTo.Items.FindByValue(trip.EndTo.ToString()) != null)
                        ddlEndTo.SelectedValue = trip.EndTo.ToString();

                    lblModalTitle.Text = "Edit Trip";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        "Sys.Application.add_load(function() { showModal(); });", true);
                }
                else
                {
                    ShowError("Failed to load trip details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trip for edit: {ex.Message}");
            }
        }



        protected async void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int tripId = Convert.ToInt32(hdnTripId.Value);
                HttpResponseMessage response;

                var selectedFleetTypeIds = new List<int>();
                foreach (ListItem item in ddlFleetType.Items)
                {
                    if (item.Selected)
                    {
                        selectedFleetTypeIds.Add(int.Parse(item.Value));
                    }
                }

                if (selectedFleetTypeIds.Count == 0)
                {
                    ShowError("Please select at least one Fleet Type.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                    return;
                }
                if (tripId == 0)
                {
                    var tripData = new
                    {
                        id = tripId,
                        title = txtTitle.Text.Trim(),
                        trainId = ddlTrainNo.SelectedValue,
                        fleetTypeId = selectedFleetTypeIds[0],
                        RouteId = Convert.ToInt32(ddlRoute.SelectedValue),
                        scheduleId = Convert.ToInt32(ddlSchedule.SelectedValue),
                        startFrom = Convert.ToInt32(ddlStartFrom.SelectedValue),
                        endTo = Convert.ToInt32(ddlEndTo.SelectedValue),
                    };

                    string jsonContent = JsonConvert.SerializeObject(tripData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await client.PostAsync("TrainTrips/PostTrainTrip", content);
                }
                else
                {
                    HttpResponseMessage getResponse = await client.GetAsync($"TrainTrips/GetTrainTrip/{tripId}");

                    if (!getResponse.IsSuccessStatusCode)
                    {
                        ShowError("Failed to load current trip details");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }

                    string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                    TripDetailModel currentTrip = JsonConvert.DeserializeObject<TripDetailModel>(currentJsonResponse);

                    var updateData = new
                    {
                        id = tripId,
                        title = txtTitle.Text.Trim(),
                        trainId = ddlTrainNo.SelectedValue,
                        fleetTypeId = selectedFleetTypeIds[0],
                        RouteId = Convert.ToInt32(ddlRoute.SelectedValue),
                        scheduleId = Convert.ToInt32(ddlSchedule.SelectedValue),
                        startFrom = Convert.ToInt32(ddlStartFrom.SelectedValue),
                        endTo = Convert.ToInt32(ddlEndTo.SelectedValue),
                        status = currentTrip.Status
                    };

                    string jsonContent = JsonConvert.SerializeObject(updateData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"TrainTrips/PutTrainTrip/{tripId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    string message = tripId == 0 ? "Trip added successfully!" : "Trip updated successfully!";
                    await LoadTrips();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        $"hideModal(); showSuccess('{message}');", true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to save trip. Status: {response.StatusCode}. Error: {errorContent}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving trip: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        private async Task ToggleTripStatus(int tripId)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(
                    $"TrainTrips/ToggleTrainTripStatus/{tripId}",
                    new StringContent("", System.Text.Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    await LoadTrips();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Trip status updated successfully!');", true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update trip status. Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error toggling trip status: {ex.Message}");
            }
        }


        protected string GetStatusClass(object status)
        {
            if (status == null) return "status-badge";
            string statusValue = status.ToString();
            return statusValue == "ACTIVE"
                ? "status-badge status-active"
                : "status-badge status-inactive";
        }


        private void ClearForm()
        {
            hdnTripId.Value = "0";
            txtTitle.Text = string.Empty;

            if (ddlFleetType.Items.Count > 0) ddlFleetType.SelectedIndex = 0;
            if (ddlTrainNo.Items.Count > 0) ddlTrainNo.SelectedIndex = 0;
            if (ddlRoute.Items.Count > 0) ddlRoute.SelectedIndex = 0;
            if (ddlSchedule.Items.Count > 0) ddlSchedule.SelectedIndex = 0;
            if (ddlStartFrom.Items.Count > 0) ddlStartFrom.SelectedIndex = 0;
            if (ddlEndTo.Items.Count > 0) ddlEndTo.SelectedIndex = 0;

            lblModalTitle.Text = "Add New Trip";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }

    public class TrainTripModel
    {
        public int TripId { get; set; }
        public string Title { get; set; }
        public string RouteName { get; set; }
        public string Facilities { get; set; }
        public string TrainName { get; set; }
        public string TrainNumber { get; set; }
        public string DayOff { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TripDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> FleetTypeId { get; set; }
        public int RouteId { get; set; }
        public int ScheduleId { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public string DayOff { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string TrainId { get; set; }
    }

    public class DropdownItem
    {
        public int FleetTypeId { get; set; }
        public string Name { get; set; }
    }

    public class RailwayStation
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public string Status { get; set; }
    }

    public class TrainNumber_
    {
        public string TrainId { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
    }

    public class ScheduleItem
    {
        public int ScheduleId { get; set; }
        public string DepartureTime { get; set; }
    }

    public class TrainRoute
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
    }
}