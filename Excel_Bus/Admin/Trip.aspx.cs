using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Trip : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Static constructor to initialize HttpClient once
        static Trip()
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

                string endpoint = "Trips/GetTrips";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainTripModel> trips = JsonConvert.DeserializeObject<List<TrainTripModel>>(jsonResponse);

                    // Apply search filter if search term is provided
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        trips = trips.Where(t =>
                            (!string.IsNullOrEmpty(t.Title) && t.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(t.AcNonAc) && t.AcNonAc.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(t.DayOff) && t.DayOff.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
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
                // Load Fleet Types
                var fleetTypesResponse = await client.GetAsync("FleetTypes/GetFleetTypes");
                if (fleetTypesResponse.IsSuccessStatusCode)
                {
                    string json = await fleetTypesResponse.Content.ReadAsStringAsync();
                    var fleetTypes = JsonConvert.DeserializeObject<List<DropdownItem>>(json);

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in fleetTypes)
                    {
                        ddlFleetType.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                    }
                }

                // Load Vehicle Routes
                var routesResponse = await client.GetAsync("VehicleRoutes/GetVehicleRoutes");
                if (routesResponse.IsSuccessStatusCode)
                {
                    string json = await routesResponse.Content.ReadAsStringAsync();
                    var routes = JsonConvert.DeserializeObject<List<DropdownItem>>(json);

                    ddlRoute.Items.Clear();
                    ddlRoute.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in routes)
                    {
                        ddlRoute.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                    }
                }

                // Load Schedules
                var schedulesResponse = await client.GetAsync("Schedules/GetSchedules");
                if (schedulesResponse.IsSuccessStatusCode)
                {
                    string json = await schedulesResponse.Content.ReadAsStringAsync();
                    var schedules = JsonConvert.DeserializeObject<List<ScheduleItem>>(json);

                    ddlSchedule.Items.Clear();
                    ddlSchedule.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in schedules)
                    {
                        string displayText = $"{item.StartFrom} - {item.EndAt}";
                        ddlSchedule.Items.Add(new ListItem(displayText, item.Id.ToString()));
                    }
                }

                // Load Counters (for Start From and End To)
                var countersResponse = await client.GetAsync("Counters/GetCounters");
                if (countersResponse.IsSuccessStatusCode)
                {
                    string json = await countersResponse.Content.ReadAsStringAsync();
                    var counters = JsonConvert.DeserializeObject<List<DropdownItem>>(json);

                    // Start From dropdown
                    ddlStartFrom.Items.Clear();
                    ddlStartFrom.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in counters)
                    {
                        ddlStartFrom.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                    }

                    // End To dropdown
                    ddlEndTo.Items.Clear();
                    ddlEndTo.Items.Add(new ListItem("Select an option", ""));
                    foreach (var item in counters)
                    {
                        ddlEndTo.Items.Add(new ListItem(item.Name, item.Id.ToString()));
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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Additional row customization if needed
            }
        }

        protected void gvTrips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int tripId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditTrip")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadTripForEdit(tripId)));
            }
            else if (e.CommandName == "ToggleStatus")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleTripStatus(tripId)));
            }
        }

        private async Task LoadTripForEdit(int tripId)
        {
            try
            {
                // First ensure dropdown data is loaded
                await LoadDropdownData();

                HttpResponseMessage response = await client.GetAsync($"Trips/GetTrip/{tripId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    TripDetailModel trip = JsonConvert.DeserializeObject<TripDetailModel>(jsonResponse);

                    hdnTripId.Value = trip.Id.ToString();
                    txtTitle.Text = trip.Title;

                    if (ddlFleetType.Items.FindByValue(trip.FleetTypeId.ToString()) != null)
                        ddlFleetType.SelectedValue = trip.FleetTypeId.ToString();

                    if (ddlRoute.Items.FindByValue(trip.VehicleRouteId.ToString()) != null)
                        ddlRoute.SelectedValue = trip.VehicleRouteId.ToString();

                    if (ddlSchedule.Items.FindByValue(trip.ScheduleId.ToString()) != null)
                        ddlSchedule.SelectedValue = trip.ScheduleId.ToString();

                    if (ddlStartFrom.Items.FindByValue(trip.StartFrom.ToString()) != null)
                        ddlStartFrom.SelectedValue = trip.StartFrom.ToString();

                    if (ddlEndTo.Items.FindByValue(trip.EndTo.ToString()) != null)
                        ddlEndTo.SelectedValue = trip.EndTo.ToString();

                    // Set day off values - DayOff is already a JSON string from API
                    //if (!string.IsNullOrEmpty(trip.DayOff) && trip.DayOff != "[]")
                    //{
                    //    hdnDayOffValues.Value = trip.DayOff;
                    //}
                    //else
                    //{
                    //    hdnDayOffValues.Value = "[]";
                    //}

                    //lblModalTitle.Text = "Edit Trip";

                    //// Use Counter pattern for showing modal
                    //ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                    //    @"Sys.Application.add_load(function() { 
                    //        loadDaysForEdit('" + hdnDayOffValues.Value + @"');
                    //        showModal(); 
                    //    });", true);
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
                //string dayOffJson = hdnDayOffValues.Value;

                //List<string> dayOffList = new List<string>();
                //if (!string.IsNullOrEmpty(dayOffJson))
                //{
                //    dayOffList = JsonConvert.DeserializeObject<List<string>>(dayOffJson);
                //}

                HttpResponseMessage response;

                if (tripId == 0)
                {
                    // Add new trip
                    var tripData = new
                    {
                        id = tripId,
                        title = txtTitle.Text.Trim(),
                        fleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                        vehicleRouteId = Convert.ToInt32(ddlRoute.SelectedValue),
                        scheduleId = Convert.ToInt32(ddlSchedule.SelectedValue),
                        startFrom = Convert.ToInt32(ddlStartFrom.SelectedValue),
                        endTo = Convert.ToInt32(ddlEndTo.SelectedValue),
                        //dayOff = JsonConvert.SerializeObject(dayOffList)
                    };

                    string jsonContent = JsonConvert.SerializeObject(tripData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await client.PostAsync("Trips/PostTrip", content);
                }
                else
                {
                    // Update existing trip - preserve status by getting current trip first
                    HttpResponseMessage getResponse = await client.GetAsync($"Trips/GetTrip/{tripId}");

                    if (!getResponse.IsSuccessStatusCode)
                    {
                        ShowError("Failed to load current trip details");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }

                    string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                    TripDetailModel currentTrip = JsonConvert.DeserializeObject<TripDetailModel>(currentJsonResponse);

                    // Update with new values but preserve status
                    var updateData = new
                    {
                        id = tripId,
                        title = txtTitle.Text.Trim(),
                        fleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                        vehicleRouteId = Convert.ToInt32(ddlRoute.SelectedValue),
                        scheduleId = Convert.ToInt32(ddlSchedule.SelectedValue),
                        startFrom = Convert.ToInt32(ddlStartFrom.SelectedValue),
                        endTo = Convert.ToInt32(ddlEndTo.SelectedValue),
                       // dayOff = JsonConvert.SerializeObject(dayOffList),
                        status = currentTrip.Status  // ✅ Preserve existing status (now string)
                    };

                    string jsonContent = JsonConvert.SerializeObject(updateData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"Trips/PutTrip/{tripId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    string message = tripId == 0 ? "Trip added successfully!" : "Trip updated successfully!";

                    // Reload trips
                    await LoadTrips();

                    // Clear form
                    ClearForm();

                    // Show success message and hide modal - Counter pattern
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        $"hideModal(); showSuccess('{message}');", true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to save trip. Status: {response.StatusCode}. Error: {errorContent}");

                    // Keep modal open
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving trip: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        private async Task ToggleTripStatus(int tripId)
        {
            try
            {
                // Get current trip details first
                HttpResponseMessage getResponse = await client.GetAsync($"Trips/GetTrip/{tripId}");

                if (getResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                    TripDetailModel trip = JsonConvert.DeserializeObject<TripDetailModel>(jsonResponse);

                    // ✅ Toggle status using string comparison
                    string newStatus = trip.Status == "1" ? "0" : "1";

                    // Prepare complete update data with all required fields
                    // DayOff is already a JSON string from API, so use it directly
                    var updateData = new
                    {
                        id = tripId,
                        title = trip.Title,
                        fleetTypeId = trip.FleetTypeId,
                        vehicleRouteId = trip.VehicleRouteId,
                        scheduleId = trip.ScheduleId,
                        startFrom = trip.StartFrom,
                        endTo = trip.EndTo,
                      //  dayOff = string.IsNullOrEmpty(trip.DayOff) ? "[]" : trip.DayOff,
                        status = newStatus  // ✅ Now string
                    };

                    string jsonContent = JsonConvert.SerializeObject(updateData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"Trips/PutTrip/{tripId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Reload the data to show updated status
                        await LoadTrips();

                        string statusText = newStatus == "1" ? "enabled" : "disabled";
                        ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                            $"showSuccess('Trip {statusText} successfully');", true);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        ShowError($"Failed to update trip status. Error: {errorContent}");
                    }
                }
                else
                {
                    ShowError("Failed to load trip details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error toggling trip status: {ex.Message}");
            }
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "";

            string statusValue = status.ToString();
            return statusValue == "Enabled" ? "status-enabled" : "status-disabled";
        }

        private void ClearForm()
        {
            hdnTripId.Value = "0";
            txtTitle.Text = string.Empty;

            if (ddlFleetType.Items.Count > 0) ddlFleetType.SelectedIndex = 0;
            if (ddlRoute.Items.Count > 0) ddlRoute.SelectedIndex = 0;
            if (ddlSchedule.Items.Count > 0) ddlSchedule.SelectedIndex = 0;
            if (ddlStartFrom.Items.Count > 0) ddlStartFrom.SelectedIndex = 0;
            if (ddlEndTo.Items.Count > 0) ddlEndTo.SelectedIndex = 0;
            //if (ddlDayOff.Items.Count > 0) ddlDayOff.SelectedIndex = 0;

            //hdnDayOffValues.Value = "[]";
            lblModalTitle.Text = "Add New Trip";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            // Add animation class
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }
    }

    // Model classes
    public class TrainTripModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AcNonAc { get; set; }
        public string DayOff { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TripDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FleetTypeId { get; set; }
        public int VehicleRouteId { get; set; }
        public int ScheduleId { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public string DayOff { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ScheduleItem
    {
        public int Id { get; set; }
        public string StartFrom { get; set; }
        public string EndAt { get; set; }
    }
}