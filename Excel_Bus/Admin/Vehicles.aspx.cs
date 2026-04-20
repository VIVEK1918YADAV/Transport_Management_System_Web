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
    public partial class Vehicles : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Vehicles()
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
                txtSearch.Text = string.Empty;
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadFleetTypes();
                    await LoadAllVehicles();
                }));
            }
        }

        private async Task LoadFleetTypes()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("FleetTypes/GetFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeModel> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeModel>>(jsonResponse);

                    var activeFleetTypes = fleetTypes.Where(ft => ft.Status == "1").ToList();

                    ViewState["FleetTypes"] = JsonConvert.SerializeObject(activeFleetTypes);

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select an option", ""));

                    foreach (var fleetType in activeFleetTypes)
                    {
                        ddlFleetType.Items.Add(new ListItem(fleetType.Name, fleetType.Id.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
            }
        }

        private async Task LoadAllVehicles(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                HttpResponseMessage response = await client.GetAsync("Vehicles/GetVehicles");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        vehicles = vehicles.Where(v =>
                            (!string.IsNullOrEmpty(v.NickName) && v.NickName.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.RegisterNo) && v.RegisterNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.EngineNo) && v.EngineNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.ChasisNo) && v.ChasisNo.ToLower().Contains(searchTerm.ToLower()))
                        ).ToList();
                    }

                    var orderedVehicles = vehicles.OrderByDescending(v => v.CreatedAt).ToList();

                    ViewState["AllVehicles"] = JsonConvert.SerializeObject(orderedVehicles);

                    gvVehicles.DataSource = orderedVehicles;
                    gvVehicles.DataBind();
                }
                else
                {
                    ShowError($"Failed to load vehicles. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vehicles: {ex.Message}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllVehicles(searchTerm)));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int vehicleId = Convert.ToInt32(hdnVehicleId.Value);

                if (vehicleId == 0)
                {
                    RegisterAsyncTask(new PageAsyncTask(() => AddVehicle()));
                }
                else
                {
                    RegisterAsyncTask(new PageAsyncTask(() => UpdateVehicle(vehicleId)));
                }
            }
        }

        private async Task AddVehicle()
        {
            try
            {
                // Get day off values from hidden field
                string dayOffJson = hdnDayOffValues.Value;
                if (string.IsNullOrEmpty(dayOffJson))
                {
                    dayOffJson = "[]";
                }

                var vehicle = new VehicleModel
                {
                    NickName = txtNickName.Text.Trim(),
                    FleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                    RegisterNo = txtRegisterNo.Text.Trim(),
                    EngineNo = txtEngineNo.Text.Trim(),
                    ChasisNo = txtChasisNo.Text.Trim(),
                    ModelNo = txtModelNo.Text.Trim(),
                    DayOff = dayOffJson,
                    HasAc = ddlHasAc.SelectedValue,
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    Status = "1"
                };

                string json = JsonConvert.SerializeObject(vehicle);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("Vehicles/PostVehicle", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllVehicles();
                    ClearForm();

                    string script = @"
                        hideModal();
                        setTimeout(function() {
                            console.log('Calling showSuccess for Add');
                            if (typeof window.showSuccess === 'function') {
                                window.showSuccess('Vehicle added successfully');
                            } else {
                                alert('Vehicle added successfully');
                            }
                        }, 500);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", script, true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add vehicle: {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding vehicle: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        private async Task UpdateVehicle(int id)
        {
            try
            {
                VehicleModel currentVehicle = null;

                if (ViewState["AllVehicles"] != null)
                {
                    string json = ViewState["AllVehicles"].ToString();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(json);
                    currentVehicle = vehicles.FirstOrDefault(v => v.Id == id);
                }

                if (currentVehicle == null)
                {
                    await LoadAllVehicles();

                    if (ViewState["AllVehicles"] != null)
                    {
                        string json = ViewState["AllVehicles"].ToString();
                        List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(json);
                        currentVehicle = vehicles.FirstOrDefault(v => v.Id == id);
                    }

                    if (currentVehicle == null)
                    {
                        ShowError("Failed to load vehicle details");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }

                // Get day off values from hidden field
                string dayOffJson = hdnDayOffValues.Value;
                if (string.IsNullOrEmpty(dayOffJson))
                {
                    dayOffJson = "[]";
                }

                var vehicle = new VehicleModel
                {
                    Id = id,
                    NickName = txtNickName.Text.Trim(),
                    FleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                    RegisterNo = txtRegisterNo.Text.Trim(),
                    EngineNo = txtEngineNo.Text.Trim(),
                    ChasisNo = txtChasisNo.Text.Trim(),
                    ModelNo = txtModelNo.Text.Trim(),
                    DayOff = dayOffJson,
                    HasAc = ddlHasAc.SelectedValue,
                    Email = currentVehicle.Email,
                    Password = currentVehicle.Password,
                    Status = currentVehicle.Status
                };

                string updateJson = JsonConvert.SerializeObject(vehicle);
                StringContent content = new StringContent(updateJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"Vehicles/PutVehicle/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllVehicles();
                    ClearForm();

                    string script = @"
                        hideModal();
                        setTimeout(function() {
                            console.log('Calling showSuccess for Update');
                            if (typeof window.showSuccess === 'function') {
                                window.showSuccess('Vehicle updated successfully');
                            } else {
                                alert('Vehicle updated successfully');
                            }
                        }, 500);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", script, true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update vehicle: {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating vehicle: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        protected void gvVehicles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int vehicleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditVehicle")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadVehicleForEdit(vehicleId)));
            }
            else if (e.CommandName == "DisableVehicle")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleVehicleStatus(vehicleId)));
            }
        }

        private async Task LoadVehicleForEdit(int id)
        {
            try
            {
                VehicleModel vehicle = null;

                if (ViewState["AllVehicles"] != null)
                {
                    string json = ViewState["AllVehicles"].ToString();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(json);
                    vehicle = vehicles.FirstOrDefault(v => v.Id == id);
                }

                if (vehicle == null)
                {
                    HttpResponseMessage response = await client.GetAsync($"Vehicles/GetVehicle/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        vehicle = JsonConvert.DeserializeObject<VehicleModel>(jsonResponse);
                    }
                    else
                    {
                        ShowError("Failed to load vehicle details");
                        return;
                    }
                }

                if (vehicle != null)
                {
                    hdnVehicleId.Value = vehicle.Id.ToString();
                    txtNickName.Text = vehicle.NickName;
                    ddlFleetType.SelectedValue = vehicle.FleetTypeId.ToString();
                    txtRegisterNo.Text = vehicle.RegisterNo;
                    txtEngineNo.Text = vehicle.EngineNo;
                    txtChasisNo.Text = vehicle.ChasisNo;
                    txtModelNo.Text = vehicle.ModelNo;

                    // Set Has AC dropdown
                    if (!string.IsNullOrEmpty(vehicle.HasAc))
                    {
                        ddlHasAc.SelectedValue = vehicle.HasAc;
                    }

                    txtNickName.Enabled = true;
                    ddlFleetType.Enabled = true;
                    txtModelNo.Enabled = true;

                    divEmail.Visible = false;
                    divPassword.Visible = false;

                    // Set day off values - DayOff is already a JSON string from API
                    if (!string.IsNullOrEmpty(vehicle.DayOff) && vehicle.DayOff != "[]" && vehicle.DayOff != "null")
                    {
                        hdnDayOffValues.Value = vehicle.DayOff;
                    }
                    else
                    {
                        hdnDayOffValues.Value = "[]";
                    }

                    rfvEmail.Enabled = false;
                    rfvPassword.Enabled = false;

                    lblModalTitle.Text = "Edit Vehicle";

                    // Use Sys.Application.add_load for showing modal with day off data
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        @"Sys.Application.add_load(function() { 
                            loadDaysForEdit('" + hdnDayOffValues.Value.Replace("'", "\\'") + @"');
                            showModal(); 
                        });", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vehicle: {ex.Message}");
            }
        }

        private async Task ToggleVehicleStatus(int id)
        {
            try
            {
                VehicleModel vehicle = null;

                if (ViewState["AllVehicles"] != null)
                {
                    string json = ViewState["AllVehicles"].ToString();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(json);
                    vehicle = vehicles.FirstOrDefault(v => v.Id == id);
                }

                if (vehicle == null)
                {
                    HttpResponseMessage getResponse = await client.GetAsync($"Vehicles/GetVehicle/{id}");

                    if (getResponse.IsSuccessStatusCode)
                    {
                        string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                        vehicle = JsonConvert.DeserializeObject<VehicleModel>(jsonResponse);
                    }
                    else
                    {
                        ShowError("Failed to load vehicle details");
                        return;
                    }
                }

                if (vehicle == null)
                {
                    ShowError("Vehicle not found");
                    return;
                }

                // Toggle status using string comparison
                string newStatus = vehicle.Status == "1" ? "0" : "1";

                var updateVehicle = new VehicleModel
                {
                    Id = vehicle.Id,
                    NickName = vehicle.NickName,
                    FleetTypeId = vehicle.FleetTypeId,
                    RegisterNo = vehicle.RegisterNo,
                    EngineNo = vehicle.EngineNo,
                    ChasisNo = vehicle.ChasisNo,
                    ModelNo = vehicle.ModelNo,
                    DayOff = vehicle.DayOff,
                    HasAc = vehicle.HasAc,
                    Email = vehicle.Email,
                    Password = vehicle.Password,
                    Status = newStatus
                };

                string updateJson = JsonConvert.SerializeObject(updateVehicle);
                StringContent content = new StringContent(updateJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"Vehicles/PutVehicle/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllVehicles();

                    string statusText = newStatus == "1" ? "enabled" : "disabled";

                    string script = $@"
                        setTimeout(function() {{
                            console.log('Calling showSuccess for Status Toggle');
                            if (typeof window.showSuccess === 'function') {{
                                window.showSuccess('Vehicle {statusText} successfully');
                            }} else {{
                                alert('Vehicle {statusText} successfully');
                            }}
                        }}, 300);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", script, true);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update vehicle status: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            hdnVehicleId.Value = "0";
            txtNickName.Text = string.Empty;
            txtNickName.Enabled = true;
            ddlFleetType.SelectedIndex = 0;
            ddlFleetType.Enabled = true;
            txtRegisterNo.Text = string.Empty;
            txtEngineNo.Text = string.Empty;
            txtChasisNo.Text = string.Empty;
            txtModelNo.Text = string.Empty;
            txtModelNo.Enabled = true;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;

            if (ddlDayOff.Items.Count > 0) ddlDayOff.SelectedIndex = 0;
            if (ddlHasAc.Items.Count > 0) ddlHasAc.SelectedIndex = 0;

            hdnDayOffValues.Value = "[]";

            divEmail.Visible = true;
            divPassword.Visible = true;

            rfvEmail.Enabled = true;
            rfvPassword.Enabled = true;

            lblModalTitle.Text = "Add New Vehicle";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess",
                "document.getElementById('" + pnlSuccess.ClientID + "').classList.add('show');", true);
        }

        protected string GetFleetTypeName(object fleetTypeId)
        {
            if (fleetTypeId == null || ViewState["FleetTypes"] == null)
                return "N/A";

            try
            {
                int id = Convert.ToInt32(fleetTypeId);
                string json = ViewState["FleetTypes"].ToString();
                List<FleetTypeModel> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeModel>>(json);

                var fleetType = fleetTypes.FirstOrDefault(ft => ft.Id == id);
                return fleetType != null ? fleetType.Name : "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        protected string GetDayOffDisplay(object dayOff)
        {
            if (dayOff == null || string.IsNullOrEmpty(dayOff.ToString()))
                return "None";

            try
            {
                string dayOffJson = dayOff.ToString();
                if (dayOffJson == "[]" || dayOffJson == "null")
                    return "None";

                List<string> days = JsonConvert.DeserializeObject<List<string>>(dayOffJson);
                if (days == null || days.Count == 0)
                    return "None";

                var dayNames = new Dictionary<string, string>
                {
                    { "0", "Sun" },
                    { "1", "Mon" },
                    { "2", "Tue" },
                    { "3", "Wed" },
                    { "4", "Thu" },
                    { "5", "Fri" },
                    { "6", "Sat" }
                };

                var displayNames = days.Select(d => dayNames.ContainsKey(d) ? dayNames[d] : d);
                return string.Join(", ", displayNames);
            }
            catch
            {
                return "None";
            }
        }

        protected string GetHasAcDisplay(object hasAc)
        {
            if (hasAc == null)
                return "N/A";

            string hasAcValue = hasAc.ToString();
            return hasAcValue == "1" ? "Yes" : "No";
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();
            return statusValue == "1" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();
            return statusValue == "1" ? "status-enabled" : "status-disabled";
        }
    }

    public class VehicleModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nickName")]
        public string NickName { get; set; }

        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("registerNo")]
        public string RegisterNo { get; set; }

        [JsonProperty("engineNo")]
        public string EngineNo { get; set; }

        [JsonProperty("chasisNo")]
        public string ChasisNo { get; set; }

        [JsonProperty("modelNo")]
        public string ModelNo { get; set; }

        [JsonProperty("dayOff")]
        public string DayOff { get; set; }

        [JsonProperty("hasAc")]
        public string HasAc { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class FleetTypeModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("seatLayout")]
        public string SeatLayout { get; set; }

        [JsonProperty("deck")]
        public int Deck { get; set; }

        [JsonProperty("deckSeats")]
        public string DeckSeats { get; set; }

        [JsonProperty("facilities")]
        public string Facilities { get; set; }

        [JsonProperty("hasAc")]
        public string HasAc { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}