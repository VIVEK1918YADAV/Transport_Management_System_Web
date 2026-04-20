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

namespace Excel_Bus.Admin
{
    public partial class ws_vehicle_list : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static ws_vehicle_list()
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
                    await LoadAllVehicles();
                }));
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

        protected void gvVehicles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int vehicleId = Convert.ToInt32(e.CommandArgument);
           
            if (e.CommandName == "DisableVehicle")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleVehicleStatus(vehicleId)));
            }
        }
        //protected void gvAssignedVehicles_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    int id = Convert.ToInt32(e.CommandArgument);

        //   if (e.CommandName == "DisableRecord")
        //    {
        //        // IMPORTANT: Don't update modal panel at all
        //        RegisterAsyncTask(new PageAsyncTask(async () => await ToggleStatus(id)));
        //    }
        //}

        private async System.Threading.Tasks.Task ToggleVehicleStatus(int id)
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

      

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
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
