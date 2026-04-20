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
    public partial class Schedule : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;
        internal int Columns;
        internal string LayoutType;
        internal int Rows;
        internal string DeckSeats;

        static Schedule()
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
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllSchedules()));
            }
        }

        private async Task LoadAllSchedules()
        {
            try
            {
                pnlError.Visible = false;

                string endpoint = "Schedules/GetSchedules";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<ScheduleModel> schedules = JsonConvert.DeserializeObject<List<ScheduleModel>>(jsonResponse);

                    var orderedSchedules = schedules.OrderByDescending(s => s.CreatedAt).ToList();

                    ViewState["AllSchedules"] = JsonConvert.SerializeObject(orderedSchedules);

                    gvSchedules.DataSource = orderedSchedules;
                    gvSchedules.DataBind();
                }
                else
                {
                    ShowError($"Failed to load schedules. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading schedules: {ex.Message}");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int scheduleId = Convert.ToInt32(hdnScheduleId.Value);

                if (scheduleId == 0)
                {
                    RegisterAsyncTask(new PageAsyncTask(() => AddNewSchedule()));
                }
                else
                {
                    RegisterAsyncTask(new PageAsyncTask(() => UpdateSchedule(scheduleId)));
                }
            }
        }

        private async Task AddNewSchedule()
        {
            try
            {
                string startFrom = txtStartFrom.Text.Trim() + ":00";
                string endAt = txtEndAt.Text.Trim() + ":00";

                var scheduleData = new
                {
                    startFrom = startFrom,
                    endAt = endAt
                };

                string json = JsonConvert.SerializeObject(scheduleData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("Schedules/PostSchedule", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllSchedules();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Schedule added successfully');", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add schedule: {errorMessage}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "setTimeout(function() { showModal(); }, 100);", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding schedule: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "setTimeout(function() { showModal(); }, 100);", true);
            }
        }

        private async Task UpdateSchedule(int id)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync($"Schedules/GetSchedule/{id}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load schedule details");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "setTimeout(function() { showModal(); }, 100);", true);
                    return;
                }

                string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                ScheduleModel currentSchedule = JsonConvert.DeserializeObject<ScheduleModel>(currentJsonResponse);

                string startFrom = txtStartFrom.Text.Trim() + ":00";
                string endAt = txtEndAt.Text.Trim() + ":00";

                var scheduleData = new
                {
                    id = id,
                    startFrom = startFrom,
                    endAt = endAt,
                    status = currentSchedule.Status  // ✅ Keep existing status (now string)
                };

                string json = JsonConvert.SerializeObject(scheduleData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"Schedules/PutSchedule/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllSchedules();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Schedule updated successfully');", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update schedule: {errorMessage}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "setTimeout(function() { showModal(); }, 100);", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating schedule: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "setTimeout(function() { showModal(); }, 100);", true);
            }
        }

        protected void gvSchedules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int scheduleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditSchedule")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadScheduleForEdit(scheduleId)));
            }
            else if (e.CommandName == "ToggleStatus")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleScheduleStatus(scheduleId)));
            }
        }

        private async Task LoadScheduleForEdit(int id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"Schedules/GetSchedule/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ScheduleModel schedule = JsonConvert.DeserializeObject<ScheduleModel>(jsonResponse);

                    hdnScheduleId.Value = schedule.Id.ToString();
                    txtStartFrom.Text = schedule.StartFrom.Substring(0, 5);
                    txtEndAt.Text = schedule.EndAt.Substring(0, 5);

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        @"Sys.Application.add_load(function() { 
                            document.getElementById('modalTitle').textContent = 'Edit Schedule';
                            showModal(); 
                        });", true);
                }
                else
                {
                    ShowError("Failed to load schedule details");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading schedule: {ex.Message}");
            }
        }

        private async Task ToggleScheduleStatus(int id)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync($"Schedules/GetSchedule/{id}");

                if (getResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                    ScheduleModel schedule = JsonConvert.DeserializeObject<ScheduleModel>(jsonResponse);

                    // ✅ Toggle status using string comparison
                    string newStatus = schedule.Status == "1" ? "0" : "1";

                    var scheduleData = new
                    {
                        id = schedule.Id,
                        startFrom = schedule.StartFrom,
                        endAt = schedule.EndAt,
                        status = newStatus  // ✅ Now string
                    };

                    string json = JsonConvert.SerializeObject(scheduleData);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"Schedules/PutSchedule/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await LoadAllSchedules();

                        string statusText = newStatus == "1" ? "enabled" : "disabled";
                        ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                            $"showSuccess('Schedule {statusText} successfully');", true);
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        ShowError($"Failed to update schedule status: {errorMessage}");
                    }
                }
                else
                {
                    ShowError("Failed to load schedule details");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        protected void gvSchedules_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ScheduleModel schedule = (ScheduleModel)e.Row.DataItem;

                // Set status label
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus != null && schedule != null)
                {
                    lblStatus.Text = schedule.Status == "1" ? "Enabled" : "Disabled";
                    lblStatus.CssClass = schedule.Status == "1" ? "status-badge status-enabled" : "status-badge status-disabled";
                }

                // Set toggle button
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");
                if (btnToggle != null && schedule != null)
                {
                    btnToggle.Text = schedule.Status == "1" ? "🚫 Disable" : "✓ Enable";
                    btnToggle.CssClass = schedule.Status == "1" ? "btn-disable" : "btn-enable";
                }
            }
        }

        private void ClearForm()
        {
            hdnScheduleId.Value = "0";
            txtStartFrom.Text = string.Empty;
            txtEndAt.Text = string.Empty;
        }

        protected string FormatTime(object time)
        {
            if (time == null) return "";

            string timeStr = time.ToString();

            // Try parsing full time (HH:mm:ss or HH:mm)
            if (TimeSpan.TryParse(timeStr, out TimeSpan ts))
            {
                DateTime dt = DateTime.Today.Add(ts);
                return dt.ToString("hh:mm tt"); // AM/PM format
            }

            return timeStr; // return original if parsing fails
        }


        protected string CalculateDuration(object startFrom, object endAt)
        {
            try
            {
                if (startFrom == null || endAt == null) return "N/A";

                TimeSpan start = TimeSpan.Parse(startFrom.ToString());
                TimeSpan end = TimeSpan.Parse(endAt.ToString());

                TimeSpan duration;
                if (end < start)
                {
                    duration = (TimeSpan.FromHours(24) - start) + end;
                }
                else
                {
                    duration = end - start;
                }

                int hours = (int)duration.TotalHours;
                int minutes = duration.Minutes;

                return $"{hours} hours {minutes} minutes";
            }
            catch
            {
                return "N/A";
            }
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();  // ✅ Changed to string
            return statusValue == "1" ? "Enabled" : "Disabled";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }

    public class ScheduleModel
    {
        public int Id { get; set; }
        public string StartFrom { get; set; }
        public string EndAt { get; set; }
        public string Status { get; set; }  // ✅ Changed from int to string
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<object> Trips { get; set; }
    }
}