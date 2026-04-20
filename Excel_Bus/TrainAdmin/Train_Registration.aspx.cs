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
    public partial class Train_Registration : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_Registration()
        {
            apiUrl = ConfigurationManager.AppSettings["api_path"];

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
                    await LoadAllTrains();
                }));
            }
        }

        // ─── Load Fleet Types ─────────────────────────────────────────────────
        private async Task LoadFleetTypes()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeModelTrain> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeModelTrain>>(jsonResponse);

                    var activeFleetTypes = fleetTypes.Where(ft => ft.Status == "ACTIVE").ToList();
                    ViewState["FleetTypes"] = JsonConvert.SerializeObject(activeFleetTypes);

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select an option", ""));

                    foreach (var fleetType in activeFleetTypes)
                        ddlFleetType.Items.Add(new ListItem(fleetType.Name, fleetType.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
            }
        }

        // ─── Load All Trains ──────────────────────────────────────────────────
        private async Task LoadAllTrains(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                HttpResponseMessage response = await client.GetAsync("TblTrainsRegs/GetTblTrainsRegs");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainModel> trains = JsonConvert.DeserializeObject<List<TrainModel>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        trains = trains.Where(v =>
                            (!string.IsNullOrEmpty(v.TrainName) && v.TrainName.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.TrainNumber) && v.TrainNumber.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.RegisterNo) && v.RegisterNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.EngineNo) && v.EngineNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.ChasisNo) && v.ChasisNo.ToLower().Contains(searchTerm.ToLower()))
                        ).ToList();
                    }

                    var orderedTrains = trains.OrderByDescending(v => v.CreatedAt).ToList();
                    ViewState["AllTrains"] = JsonConvert.SerializeObject(orderedTrains);

                    gvTrains.DataSource = orderedTrains;
                    gvTrains.DataBind();
                }
                else
                {
                    ShowError($"Failed to load trains. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trains: {ex.Message}");
            }
        }

        // ─── Search ───────────────────────────────────────────────────────────
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllTrains(searchTerm)));
        }

        // ─── Submit (Add / Edit) ──────────────────────────────────────────────
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string trainId = hdnTrainId.Value;

                if (string.IsNullOrEmpty(trainId) || trainId == "0")
                    RegisterAsyncTask(new PageAsyncTask(() => AddTrain()));
                else
                    RegisterAsyncTask(new PageAsyncTask(() => UpdateTrain(trainId)));
            }
        }

        // ─── ADD Train ────────────────────────────────────────────────────────
        private async Task AddTrain()
        {
            try
            {
                string dayOffJson = hdnDayOffValues.Value;
                if (string.IsNullOrEmpty(dayOffJson)) dayOffJson = "[]";

                string dayOffValue = ParseDayOffForApi(dayOffJson);

                var dto = new TrainRequestDto
                {
                    TrainName = txtTrainName.Text.Trim(),
                    TrainNumber = txtTrainNumber.Text.Trim(),
                    FleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                    RegistrationNumber = txtRegisterNo.Text.Trim(),
                    EngineNo = txtEngineNo.Text.Trim(),
                    ChasisNo = txtChasisNo.Text.Trim(),
                    ModelNo = txtModelNo.Text.Trim(),
                    DayOff = dayOffValue,
                    HasAc = ddlHasAc.SelectedValue,
                    Status = "ACTIVE"
                };

                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("TblTrainsRegs/PostTblTrainsReg", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllTrains();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "Sys.Application.add_load(function() { hideModal(); setTimeout(function() { showSuccess('Train added successfully'); }, 400); });", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add Train: {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding Train: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        // ─── UPDATE Train (POST method) ───────────────────────────────────────
        private async Task UpdateTrain(string id)
        {
            try
            {
                TrainModel currentTrain = null;

                if (ViewState["AllTrains"] != null)
                {
                    var trains = JsonConvert.DeserializeObject<List<TrainModel>>(ViewState["AllTrains"].ToString());
                    currentTrain = trains.FirstOrDefault(v => v.Id == id);
                }

                if (currentTrain == null)
                {
                    await LoadAllTrains();
                    if (ViewState["AllTrains"] != null)
                    {
                        var trains = JsonConvert.DeserializeObject<List<TrainModel>>(ViewState["AllTrains"].ToString());
                        currentTrain = trains.FirstOrDefault(v => v.Id == id);
                    }

                    if (currentTrain == null)
                    {
                        ShowError("Failed to load Train details");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                        return;
                    }
                }

                string dayOffJson = hdnDayOffValues.Value;
                if (string.IsNullOrEmpty(dayOffJson)) dayOffJson = "[]";

                string dayOffValue = ParseDayOffForApi(dayOffJson);

                var dto = new TrainRequestDto
                {
                    Id = Convert.ToInt32(id),
                    TrainName = txtTrainName.Text.Trim(),
                    TrainNumber = txtTrainNumber.Text.Trim(),
                    FleetTypeId = Convert.ToInt32(ddlFleetType.SelectedValue),
                    RegistrationNumber = txtRegisterNo.Text.Trim(),
                    EngineNo = txtEngineNo.Text.Trim(),
                    ChasisNo = txtChasisNo.Text.Trim(),
                    ModelNo = txtModelNo.Text.Trim(),
                    DayOff = dayOffValue,
                    HasAc = ddlHasAc.SelectedValue,
                    Status = currentTrain.Status
                };

                string updateJson = JsonConvert.SerializeObject(dto);
                StringContent content = new StringContent(updateJson, Encoding.UTF8, "application/json");

                // POST instead of PUT — new endpoint name
                HttpResponseMessage response = await client.PostAsync($"TblTrainsRegs/UpdateTblTrainsReg/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllTrains();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "Sys.Application.add_load(function() { hideModal(); setTimeout(function() { showSuccess('Train updated successfully'); }, 400); });", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update Train: {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating Train: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        // ─── Helper: JSON array ["5"] → "5" for API ───────────────────────────
        private string ParseDayOffForApi(string dayOffJson)
        {
            try
            {
                if (string.IsNullOrEmpty(dayOffJson) || dayOffJson == "[]")
                    return "";

                var days = JsonConvert.DeserializeObject<List<string>>(dayOffJson);
                if (days == null || days.Count == 0) return "";

                // Single: "5"  |  Multiple: "1,5"
                return string.Join(",", days);
            }
            catch
            {
                return dayOffJson;
            }
        }

        // ─── Helper: "5" or "1,5" → ["5"] or ["1","5"] for JS modal ──────────
        private string ConvertDayOffRawToJson(string dayOffRaw)
        {
            if (string.IsNullOrEmpty(dayOffRaw) || dayOffRaw == "[]" || dayOffRaw == "null")
                return "[]";

            // Already JSON array
            if (dayOffRaw.StartsWith("["))
                return dayOffRaw;

            // Comma-separated "1,5" → ["1","5"]
            var parts = dayOffRaw.Split(',')
                                  .Select(d => d.Trim())
                                  .Where(d => !string.IsNullOrEmpty(d))
                                  .ToList();

            return JsonConvert.SerializeObject(parts);
        }

        // ─── GridView Row Commands ────────────────────────────────────────────
        protected void gvTrains_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string trainId = e.CommandArgument.ToString();

            if (e.CommandName == "EditTrain")
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainForEdit(trainId)));
            else if (e.CommandName == "DisableTrain")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleTrainStatus(trainId)));
        }

        // ─── Load Train for Edit ──────────────────────────────────────────────
        private async Task LoadTrainForEdit(string id)
        {
            try
            {
                TrainModel train = null;

                if (ViewState["AllTrains"] != null)
                {
                    var trains = JsonConvert.DeserializeObject<List<TrainModel>>(ViewState["AllTrains"].ToString());
                    train = trains.FirstOrDefault(v => v.Id == id);
                }

                if (train == null)
                {
                    HttpResponseMessage response = await client.GetAsync($"TblTrainsRegs/GetTblTrainsReg/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        train = JsonConvert.DeserializeObject<TrainModel>(jsonResponse);
                    }
                    else
                    {
                        ShowError("Failed to load Train details");
                        return;
                    }
                }

                if (train != null)
                {
                    hdnTrainId.Value = train.Id?.ToString();
                    txtTrainName.Text = train.TrainName;
                    txtTrainNumber.Text = train.TrainNumber;
                    txtRegisterNo.Text = train.RegisterNo;
                    txtEngineNo.Text = train.EngineNo;
                    txtChasisNo.Text = train.ChasisNo;
                    txtModelNo.Text = train.ModelNo;

                    if (ddlFleetType.Items.FindByValue(train.FleetTypeId.ToString()) != null)
                        ddlFleetType.SelectedValue = train.FleetTypeId.ToString();

                    if (!string.IsNullOrEmpty(train.HasAc) && ddlHasAc.Items.FindByValue(train.HasAc) != null)
                        ddlHasAc.SelectedValue = train.HasAc;

                    txtTrainName.Enabled = true;
                    ddlFleetType.Enabled = true;
                    txtModelNo.Enabled = true;

                    // DayOffRaw e.g. "5" → ["5"] for JS modal
                    string dayOffForEdit = ConvertDayOffRawToJson(train.DayOffRaw ?? "");
                    hdnDayOffValues.Value = dayOffForEdit;
                    lblModalTitle.Text = "Edit Train";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        @"Sys.Application.add_load(function() { 
                            loadDaysForEdit('" + hdnDayOffValues.Value.Replace("'", "\\'") + @"');
                            showModal(); 
                        });", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading Train: {ex.Message}");
            }
        }

        // ─── Toggle Status ────────────────────────────────────────────────────
        private async Task ToggleTrainStatus(string id)
        {
            try
            {
                TrainModel train = null;

                if (ViewState["AllTrains"] != null)
                {
                    var trains = JsonConvert.DeserializeObject<List<TrainModel>>(ViewState["AllTrains"].ToString());
                    train = trains.FirstOrDefault(v => v.Id == id);
                }

                if (train == null)
                {
                    HttpResponseMessage getResponse = await client.GetAsync($"TblTrainsRegs/GetTblTrainsReg/{id}");
                    if (getResponse.IsSuccessStatusCode)
                    {
                        string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                        train = JsonConvert.DeserializeObject<TrainModel>(jsonResponse);
                    }
                    else { ShowError("Failed to load Train details"); return; }
                }

                if (train == null) { ShowError("Train not found"); return; }

                string newStatus = train.Status == "ACTIVE" ? "INACTIVE" : "ACTIVE";

                var dto = new TrainRequestDto
                {
                    Id = Convert.ToInt32(id),
                    TrainName = train.TrainName,
                    TrainNumber = train.TrainNumber,
                    FleetTypeId = train.FleetTypeId,
                    RegistrationNumber = train.RegisterNo,
                    EngineNo = train.EngineNo,
                    ChasisNo = train.ChasisNo,
                    ModelNo = train.ModelNo,
                    DayOff = train.DayOffRaw ?? "",
                    HasAc = train.HasAc,
                    Status = newStatus
                };

                string updateJson = JsonConvert.SerializeObject(dto);
                StringContent content = new StringContent(updateJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"TblTrainsRegs/UpdateTblTrainsReg/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllTrains();
                    string statusText = newStatus == "ACTIVE" ? "enabled" : "disabled";

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        $"Sys.Application.add_load(function() {{ setTimeout(function() {{ showSuccess('Train {statusText} successfully'); }}, 300); }});", true);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update Train status: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        // ─── Clear Form ───────────────────────────────────────────────────────
        private void ClearForm()
        {
            hdnTrainId.Value = "0";
            txtTrainName.Text = string.Empty;
            txtTrainNumber.Text = string.Empty;
            txtTrainName.Enabled = true;
            ddlFleetType.SelectedIndex = 0;
            ddlFleetType.Enabled = true;
            txtRegisterNo.Text = string.Empty;
            txtEngineNo.Text = string.Empty;
            txtChasisNo.Text = string.Empty;
            txtModelNo.Text = string.Empty;
            txtModelNo.Enabled = true;
            if (ddlDayOff.Items.Count > 0) ddlDayOff.SelectedIndex = 0;
            if (ddlHasAc.Items.Count > 0) ddlHasAc.SelectedIndex = 0;
            hdnDayOffValues.Value = "[]";
            lblModalTitle.Text = "Add New Train";
        }

        // ─── UI Helpers ───────────────────────────────────────────────────────
        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                $"document.getElementById('{pnlError.ClientID}').classList.add('show');", true);
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess",
                $"document.getElementById('{pnlSuccess.ClientID}').classList.add('show');", true);
        }

        protected string GetFleetTypeName(object fleetTypeName)
        {
            if (fleetTypeName == null || string.IsNullOrEmpty(fleetTypeName.ToString()))
                return "N/A";
            return fleetTypeName.ToString();
        }

        protected string GetDayOffDisplay(object dayOff)
        {
            if (dayOff == null || string.IsNullOrEmpty(dayOff.ToString()))
                return "None";

            string dayOffStr = dayOff.ToString().Trim();
            if (dayOffStr == "[]" || dayOffStr == "null" || dayOffStr == "None")
                return "None";

            // JSON array ["1","5"]
            if (dayOffStr.StartsWith("["))
            {
                try
                {
                    var dayNames = new Dictionary<string, string>
                    {
                        {"0","Sun"},{"1","Mon"},{"2","Tue"},{"3","Wed"},
                        {"4","Thu"},{"5","Fri"},{"6","Sat"}
                    };
                    var days = JsonConvert.DeserializeObject<List<string>>(dayOffStr);
                    if (days == null || days.Count == 0) return "None";
                    return string.Join(", ", days.Select(d => dayNames.ContainsKey(d) ? dayNames[d] : d));
                }
                catch { return "None"; }
            }

            // Plain string from API e.g. "Friday"
            return dayOffStr;
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "status-enabled" : "status-disabled";
        }
    }

    // ─── DTO — matches controller's TrainRequestDto exactly ──────────────────
    public class TrainRequestDto
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

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

        [JsonProperty("hasAc")]
        public string HasAc { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }
    }

    // ─── Model for reading API GET response ───────────────────────────────────
    public class TrainModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegisterNo { get; set; }

        [JsonProperty("engineNo")]
        public string EngineNo { get; set; }

        [JsonProperty("chasisNo")]
        public string ChasisNo { get; set; }

        [JsonProperty("modelNo")]
        public string ModelNo { get; set; }

        // Converted display string from API e.g. "Friday"
        [JsonProperty("dayOff")]
        public string DayOff { get; set; }

        // Original numeric string e.g. "5" — for edit modal
        [JsonProperty("dayOffRaw")]
        public string DayOffRaw { get; set; }

        [JsonProperty("hasAc")]
        public string HasAc { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    // ─── Fleet Type Model ─────────────────────────────────────────────────────
    public class FleetTypeModelTrain
    {
        [JsonProperty("fleetTypeId")]   // API returns "fleetTypeId" not "id"
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }
    }
}