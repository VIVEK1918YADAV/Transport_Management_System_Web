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
    public partial class RailwayStations : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static RailwayStations()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

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
                if (!IsPostBack)
                {
                    await LoadStations();
                }
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load all stations into GridView
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadStations()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("RailwayStations/GetRailwayStations");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<RailwayStationDto> stations =
                        JsonConvert.DeserializeObject<List<RailwayStationDto>>(json);

                    stations = stations
                        .OrderByDescending(s => s.CreatedAt)
                        .ToList();

                    gvStations.DataSource = stations;
                    gvStations.DataBind();
                }
                else
                {
                    ShowError($"Failed to load railway stations. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading railway stations: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load single station for editing
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadStationForEdit(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync($"RailwayStations/GetRailwayStation/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    RailwayStationDto station =
                        JsonConvert.DeserializeObject<RailwayStationDto>(json);

                    if (station != null)
                    {
                        hfStationId.Value = station.StationId.ToString();
                        hfIsEdit.Value = "true";
                        txtStationName.Text = station.StationName ?? "";
                        txtStationCode.Text = station.StationCode ?? "";
                        txtCity.Text = station.City ?? "";
                        txtProvince.Text = station.Province ?? "";
                        txtLocationDesc.Text = station.LocationDescription ?? "";
                        txtLatitude.Text = station.Latitude?.ToString() ?? "";
                        txtLongitude.Text = station.Longitude?.ToString() ?? "";
                        lblModalTitle.Text = "Edit Railway Station";
                    }
                    else
                    {
                        ShowError("Railway station not found.");
                    }
                }
                else
                {
                    ShowError("Failed to load station details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading station: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Submit button – Add or Update
        // ────────────────────────────────────────────────────────────────────────────

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                bool isEdit = hfIsEdit.Value == "true";
                if (isEdit)
                    await UpdateStation();
                else
                    await AddStation();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Add new station
        // ────────────────────────────────────────────────────────────────────────────

        private async Task AddStation()
        {
            try
            {
                decimal? lat = decimal.TryParse(txtLatitude.Text, out decimal la)
                                   ? la : (decimal?)null;
                decimal? lng = decimal.TryParse(txtLongitude.Text, out decimal lo)
                                   ? lo : (decimal?)null;

                var requestData = new
                {
                    stationName = txtStationName.Text.Trim(),
                    stationCode = txtStationCode.Text.Trim().ToUpper(),
                    city = txtCity.Text.Trim(),
                    province = txtProvince.Text.Trim(),
                    locationDescription = txtLocationDesc.Text.Trim(),
                    latitude = lat,
                    longitude = lng,
                    isActive = true,
                    status = "ACTIVE",
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "system"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("RailwayStations/PostRailwayStation", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadStations();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Railway station added successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add station: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding station: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Update existing station
        // ────────────────────────────────────────────────────────────────────────────

        private async Task UpdateStation()
        {
            try
            {
                int stationId = int.Parse(hfStationId.Value);

                // Fetch current record to preserve status
                HttpResponseMessage getResponse =
                    await client.GetAsync($"RailwayStations/GetRailwayStation/{stationId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load station details.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                RailwayStationDto current =
                    JsonConvert.DeserializeObject<RailwayStationDto>(currentJson);

                if (current == null)
                {
                    ShowError("Station not found.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                decimal? lat = decimal.TryParse(txtLatitude.Text, out decimal la)
                                   ? la : (decimal?)null;
                decimal? lng = decimal.TryParse(txtLongitude.Text, out decimal lo)
                                   ? lo : (decimal?)null;

                var requestData = new
                {
                    stationId = stationId,
                    stationName = txtStationName.Text.Trim(),
                    stationCode = txtStationCode.Text.Trim().ToUpper(),
                    city = txtCity.Text.Trim(),
                    province = txtProvince.Text.Trim(),
                    locationDescription = txtLocationDesc.Text.Trim(),
                    latitude = lat,
                    longitude = lng,
                    isActive = current.IsActive,
                    status = current.Status,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "system"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(
                        $"RailwayStations/PutRailwayStation/{stationId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadStations();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Railway station updated successfully!');",
                        true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update station: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating station: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GridView row commands
        // ────────────────────────────────────────────────────────────────────────────

        protected void gvStations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int stationId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditStation")
                RegisterAsyncTask(new PageAsyncTask(() => LoadStationForEdit(stationId)));
            else if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(stationId)));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Toggle station ACTIVE / INACTIVE
        // ────────────────────────────────────────────────────────────────────────────

        private async Task ToggleStatus(int stationId)
        {
            try
            {
                // Fetch current record first
                HttpResponseMessage getResponse =
                    await client.GetAsync($"RailwayStations/GetRailwayStation/{stationId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load station details.");
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                RailwayStationDto current =
                    JsonConvert.DeserializeObject<RailwayStationDto>(currentJson);

                if (current == null)
                {
                    ShowError("Station not found.");
                    return;
                }

                bool newIsActive = !(current.IsActive == true);
                string newStatus = newIsActive ? "ACTIVE" : "INACTIVE";

                var requestData = new
                {
                    stationId = stationId,
                    stationName = current.StationName,
                    stationCode = current.StationCode,
                    city = current.City,
                    province = current.Province,
                    locationDescription = current.LocationDescription,
                    latitude = current.Latitude,
                    longitude = current.Longitude,
                    isActive = newIsActive,
                    status = newStatus,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "system"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(
                        $"RailwayStations/PutRailwayStation/{stationId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadStations();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Station status updated successfully!');", true);
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

        // ────────────────────────────────────────────────────────────────────────────
        // UI helpers
        // ────────────────────────────────────────────────────────────────────────────

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                $"document.getElementById('{pnlError.ClientID}').classList.add('show');",
                true);
        }

        private void ClearForm()
        {
            txtStationName.Text = "";
            txtStationCode.Text = "";
            txtCity.Text = "";
            txtProvince.Text = "";
            txtLocationDesc.Text = "";
            txtLatitude.Text = "";
            txtLongitude.Text = "";
            hfStationId.Value = "";
            hfIsEdit.Value = "false";
            lblModalTitle.Text = "Add New Railway Station";
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // DTO — matches API JSON exactly
    // ────────────────────────────────────────────────────────────────────────────────

    public class RailwayStationDto
    {
        [JsonProperty("stationId")]
        public int StationId { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("stationCode")]
        public string StationCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("locationDescription")]
        public string LocationDescription { get; set; }

        [JsonProperty("latitude")]
        public decimal? Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal? Longitude { get; set; }

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
}