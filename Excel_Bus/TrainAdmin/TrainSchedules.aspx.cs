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
    public partial class TrainSchedules : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TrainSchedules()
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
                if (!IsPostBack)
                    await LoadSchedules();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load all schedules into GridView
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadSchedules()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainSchedules/GetTrainSchedules");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<TrainScheduleDto> schedules =
                        JsonConvert.DeserializeObject<List<TrainScheduleDto>>(json);

                    schedules = schedules
                        .OrderByDescending(s => s.CreatedAt)
                        .ToList();

                    gvSchedules.DataSource = schedules;
                    gvSchedules.DataBind();
                }
                else
                {
                    ShowError($"Failed to load schedules. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading schedules: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load single schedule for editing
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadScheduleForEdit(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync($"TrainSchedules/GetTrainSchedule/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TrainScheduleDto schedule =
                        JsonConvert.DeserializeObject<TrainScheduleDto>(json);

                    if (schedule != null)
                    {
                        hfScheduleId.Value = schedule.ScheduleId.ToString();
                        hfIsEdit.Value = "true";
                        lblModalTitle.Text = "Edit Schedule";

                        // DepartureTime comes as "HH:mm:ss" — trim to "HH:mm" for time input
                        txtDepartureTime.Text = TrimTime(schedule.DepartureTime);
                        txtArrivalTime.Text = TrimTime(schedule.ArrivalTime);
                        txtTotalDistance.Text = schedule.TotalDistanceKm?.ToString() ?? "";
                        // Duration is read-only / auto-calculated; leave blank (JS calculates on load)
                        txtDuration.Text = schedule.EstimatedDurationHours.HasValue
                            ? schedule.EstimatedDurationHours.Value + " hrs"
                            : "";
                    }
                    else
                    {
                        ShowError("Schedule not found.");
                    }
                }
                else
                {
                    ShowError("Failed to load schedule details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading schedule: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Submit – Add or Update
        // ────────────────────────────────────────────────────────────────────────────

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (hfIsEdit.Value == "true")
                    await UpdateSchedule();
                else
                    await AddSchedule();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Add new schedule
        // ────────────────────────────────────────────────────────────────────────────

        private async Task AddSchedule()
        {
            try
            {
                decimal? distance = decimal.TryParse(txtTotalDistance.Text, out decimal d)
                                        ? d : (decimal?)null;

                // TimeOnly requires "HH:mm:ss" — pad from "HH:mm"
                string depTime = ToTimeOnlyString(txtDepartureTime.Text.Trim());
                string arrTime = string.IsNullOrWhiteSpace(txtArrivalTime.Text)
                                     ? null
                                     : ToTimeOnlyString(txtArrivalTime.Text.Trim());

                var requestData = new
                {
                    departureTime = depTime,
                    arrivalTime = arrTime,
                    totalDistanceKm = distance,
                    isActive = true,
                    status = "1",
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TrainSchedules/PostTrainSchedule", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadSchedules();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Schedule added successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add schedule: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding schedule: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Update existing schedule
        // ────────────────────────────────────────────────────────────────────────────

        private async Task UpdateSchedule()
        {
            try
            {
                int scheduleId = int.Parse(hfScheduleId.Value);

                // Fetch current to preserve status/isActive
                HttpResponseMessage getResponse =
                    await client.GetAsync($"TrainSchedules/GetTrainSchedule/{scheduleId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load schedule details.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                TrainScheduleDto current =
                    JsonConvert.DeserializeObject<TrainScheduleDto>(currentJson);

                if (current == null)
                {
                    ShowError("Schedule not found.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                decimal? distance = decimal.TryParse(txtTotalDistance.Text, out decimal d)
                                        ? d : (decimal?)null;

                // TimeOnly requires "HH:mm:ss" — pad from "HH:mm"
                string depTime = ToTimeOnlyString(txtDepartureTime.Text.Trim());
                string arrTime = string.IsNullOrWhiteSpace(txtArrivalTime.Text)
                                     ? null
                                     : ToTimeOnlyString(txtArrivalTime.Text.Trim());

                var requestData = new
                {
                    scheduleId = scheduleId,
                    departureTime = depTime,
                    arrivalTime = arrTime,
                    totalDistanceKm = distance,
                    isActive = current.IsActive,
                    status = current.Status,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(
                        $"TrainSchedules/PutTrainSchedule/{scheduleId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadSchedules();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Schedule updated successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update schedule: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating schedule: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GridView row commands
        // ────────────────────────────────────────────────────────────────────────────

        protected void gvSchedules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int scheduleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditSchedule")
                RegisterAsyncTask(new PageAsyncTask(() => LoadScheduleForEdit(scheduleId)));
            else if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(scheduleId)));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Toggle status  "1" (Active) ↔ "0" (Inactive)
        // ────────────────────────────────────────────────────────────────────────────

        private async Task ToggleStatus(int scheduleId)
        {
            try
            {
                HttpResponseMessage getResponse =
                    await client.GetAsync($"TrainSchedules/GetTrainSchedule/{scheduleId}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load schedule details.");
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                TrainScheduleDto current =
                    JsonConvert.DeserializeObject<TrainScheduleDto>(currentJson);

                if (current == null) { ShowError("Schedule not found."); return; }

                bool newIsActive = !(current.IsActive == true);
                string newStatus = newIsActive ? "1" : "0";

                var requestData = new
                {
                    scheduleId = scheduleId,
                    // current.DepartureTime already "HH:mm:ss" from API — ensure correct format
                    departureTime = ToTimeOnlyString(TrimTime(current.DepartureTime)),
                    arrivalTime = string.IsNullOrWhiteSpace(current.ArrivalTime)
                                          ? (string)null
                                          : ToTimeOnlyString(TrimTime(current.ArrivalTime)),
                    totalDistanceKm = current.TotalDistanceKm,
                    isActive = newIsActive,
                    status = newStatus,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(
                        $"TrainSchedules/PutTrainSchedule/{scheduleId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadSchedules();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Schedule status updated successfully!');", true);
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
        // Helper: trim "HH:mm:ss" → "HH:mm" for HTML time input
        // ────────────────────────────────────────────────────────────────────────────

        private string TrimTime(string timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return "";
            // API returns "HH:mm:ss" — take first 5 chars
            return timeStr.Length >= 5 ? timeStr.Substring(0, 5) : timeStr;
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Helper: ensure "HH:mm" → "HH:mm:ss" so System.TimeOnly can deserialize it
        // ────────────────────────────────────────────────────────────────────────────

        private string ToTimeOnlyString(string timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return timeStr;
            // Already "HH:mm:ss"
            if (timeStr.Length == 8) return timeStr;
            // "HH:mm" → "HH:mm:00"
            if (timeStr.Length == 5) return timeStr + ":00";
            return timeStr;
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Status helpers called from ASPX markup
        // Note: API stores status as "1" (active) / "0" (inactive)
        // ────────────────────────────────────────────────────────────────────────────

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "1" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "status-badge";
            return status.ToString() == "1"
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
                $"document.getElementById('{pnlError.ClientID}').classList.add('show');", true);
        }

        private void ClearForm()
        {
            txtDepartureTime.Text = "";
            txtArrivalTime.Text = "";
            txtTotalDistance.Text = "";
            txtDuration.Text = "";
            hfScheduleId.Value = "";
            hfIsEdit.Value = "false";
            lblModalTitle.Text = "Add New Schedule";
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // DTO — matches TrainSchedule model returned by API
    // ────────────────────────────────────────────────────────────────────────────────

    public class TrainScheduleDto
    {
        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }

        [JsonProperty("departureTime")]
        public string DepartureTime { get; set; }   // "HH:mm:ss"

        [JsonProperty("arrivalTime")]
        public string ArrivalTime { get; set; }     // "HH:mm:ss" or null

        [JsonProperty("estimatedDurationHours")]
        public decimal? EstimatedDurationHours { get; set; }

        [JsonProperty("totalDistanceKm")]
        public decimal? TotalDistanceKm { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }          // "1" = active, "0" = inactive

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