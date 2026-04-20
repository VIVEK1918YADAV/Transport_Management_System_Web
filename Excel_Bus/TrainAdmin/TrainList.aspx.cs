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
    public partial class TrainList : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TrainList()
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
                await LoadFleetTypes();

                if (!IsPostBack)
                {
                    await LoadTrains();
                }
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load Fleet Types into dropdown
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadFleetTypes()
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<TrainFleetTypeDto> fleetTypes =
                        JsonConvert.DeserializeObject<List<TrainFleetTypeDto>>(json);

                    string currentSelected = ddlFleetType.SelectedValue;

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select an option", ""));

                    foreach (var ft in fleetTypes)
                    {
                        ddlFleetType.Items.Add(
                            new ListItem(ft.Name, ft.FleetTypeId.ToString()));
                    }

                    if (!string.IsNullOrEmpty(currentSelected) &&
                        ddlFleetType.Items.FindByValue(currentSelected) != null)
                    {
                        ddlFleetType.SelectedValue = currentSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load all trains into GridView
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadTrains()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<TrainListDto> trains =
                        JsonConvert.DeserializeObject<List<TrainListDto>>(json);

                    trains = trains
                        .OrderByDescending(t => t.CreatedAt)
                        .ToList();

                    gvTrains.DataSource = trains;
                    gvTrains.DataBind();
                }
                else
                {
                    ShowError($"Failed to load trains. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trains: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Load single train for editing
        // ────────────────────────────────────────────────────────────────────────────

        private async Task LoadTrainForEdit(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync($"TblTrainsRegs/GetTblTrainsReg/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TrainEditDto train = JsonConvert.DeserializeObject<TrainEditDto>(json);

                    if (train != null)
                    {
                        hfTrainId.Value = train.Id.ToString();
                        hfIsEdit.Value = "true";
                        txtTrainName.Text = train.TrainName ?? "";
                        txtTrainNumber.Text = train.TrainNumber ?? "";
                        txtRegNumber.Text = train.RegistrationNumber ?? "";
                        txtEngineNo.Text = train.EngineNo ?? "";
                        txtChasisNo.Text = train.ChasisNo ?? "";
                        txtModelNo.Text = train.ModelNo ?? "";
                        lblModalTitle.Text = "Edit Train";

                        // Fleet Type
                        string ftVal = train.FleetTypeId.ToString();
                        if (ddlFleetType.Items.FindByValue(ftVal) != null)
                            ddlFleetType.SelectedValue = ftVal;

                        // Day Off — comma-separated numeric values e.g. "0,2,5"
                        hfDayOff.Value = train.DayOff ?? "";
                    }
                    else
                    {
                        ShowError("Train not found.");
                    }
                }
                else
                {
                    ShowError("Failed to load train details.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading train: {ex.Message}");
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
                    await UpdateTrain();
                else
                    await AddTrain();
            }));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Add new train
        // ────────────────────────────────────────────────────────────────────────────

        private async Task AddTrain()
        {
            try
            {
                var requestData = new
                {
                    trainName = txtTrainName.Text.Trim(),
                    trainNumber = txtTrainNumber.Text.Trim(),
                    fleetTypeId = int.Parse(ddlFleetType.SelectedValue),
                    registrationNumber = txtRegNumber.Text.Trim(),
                    engineNo = txtEngineNo.Text.Trim(),
                    chasisNo = txtChasisNo.Text.Trim(),
                    modelNo = txtModelNo.Text.Trim(),
                    dayOff = hfDayOff.Value,  // comma-separated e.g. "0,2,5"
                    status = "ACTIVE",
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TblTrainsRegs/PostTblTrainsReg", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadTrains();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Train added successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add train: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding train: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Update existing train
        // ────────────────────────────────────────────────────────────────────────────

        private async Task UpdateTrain()
        {
            try
            {
                int trainId = int.Parse(hfTrainId.Value);

                var requestData = new
                {
                    trainName = txtTrainName.Text.Trim(),
                    trainNumber = txtTrainNumber.Text.Trim(),
                    fleetTypeId = int.Parse(ddlFleetType.SelectedValue),
                    registrationNumber = txtRegNumber.Text.Trim(),
                    engineNo = txtEngineNo.Text.Trim(),
                    chasisNo = txtChasisNo.Text.Trim(),
                    modelNo = txtModelNo.Text.Trim(),
                    dayOff = hfDayOff.Value,  // comma-separated e.g. "0,2,5"
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(
                        $"TblTrainsRegs/UpdateTblTrainsReg/{trainId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadTrains();
                    ClearForm();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Train updated successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update train: {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating train: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GridView row commands
        // ────────────────────────────────────────────────────────────────────────────

        protected void gvTrains_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int trainId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditTrain")
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainForEdit(trainId)));
            else if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(trainId)));
        }

        // ────────────────────────────────────────────────────────────────────────────
        // Toggle ACTIVE / INACTIVE
        // ────────────────────────────────────────────────────────────────────────────

        private async Task ToggleStatus(int trainId)
        {
            try
            {
                // API expects { id: trainId } in body
                var requestData = new { id = trainId };
                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TblTrainsRegs/ToggleTrainStatus", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadTrains();
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Train status updated successfully!');", true);
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
            txtTrainName.Text = "";
            txtTrainNumber.Text = "";
            ddlFleetType.SelectedIndex = 0;
            txtRegNumber.Text = "";
            txtEngineNo.Text = "";
            txtChasisNo.Text = "";
            txtModelNo.Text = "";
            hfDayOff.Value = "";
            hfTrainId.Value = "";
            hfIsEdit.Value = "false";
            lblModalTitle.Text = "Add New Train";
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // DTOs
    // ────────────────────────────────────────────────────────────────────────────────

    // Used for GridView binding (from GetTblTrainsRegs — returns joined/computed fields)
    public class TrainListDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("engineNo")]
        public string EngineNo { get; set; }

        [JsonProperty("chasisNo")]
        public string ChasisNo { get; set; }

        [JsonProperty("modelNo")]
        public string ModelNo { get; set; }

        [JsonProperty("dayOff")]
        public string DayOff { get; set; }

        [JsonProperty("dayOffRaw")]
        public string DayOffRaw { get; set; }

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

    // Used for editing (from GetTblTrainsReg/{id} — returns raw TblTrainsReg model)
    public class TrainEditDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("engineNo")]
        public string EngineNo { get; set; }

        [JsonProperty("chasisNo")]
        public string ChasisNo { get; set; }

        [JsonProperty("modelNo")]
        public string ModelNo { get; set; }

        [JsonProperty("dayOff")]
        public string DayOff { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    // Used to populate Fleet Type dropdown
    public class TrainFleetTypeDto
    {
        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}