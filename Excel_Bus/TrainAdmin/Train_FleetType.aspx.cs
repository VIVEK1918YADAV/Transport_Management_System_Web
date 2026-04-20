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
    public partial class Train_FleetType : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_FleetType()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                //await LoadCoachLayouts();
                //await LoadCoachTypes();

                if (!IsPostBack)
                {
                    await LoadFleetTypes();
                }
            }));
        }

        //private async Task LoadCoachLayouts()
        //{
        //    try
        //    {
        //        HttpResponseMessage response =
        //            await client.GetAsync("TrainCoachLayouts/GetTrainCoachLayouts");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResponse = await response.Content.ReadAsStringAsync();
        //            List<CoachLayoutDto> layouts =
        //                JsonConvert.DeserializeObject<List<CoachLayoutDto>>(jsonResponse);

        //            string currentSelected = ddlCoachLayout.SelectedValue;

        //            ddlCoachLayout.Items.Clear();
        //            ddlCoachLayout.Items.Add(new ListItem("Select an option", ""));

        //            foreach (var layout in layouts)
        //            {
        //                ddlCoachLayout.Items.Add(
        //                    new ListItem(layout.Layout, layout.Id.ToString()));
        //            }

        //            if (!string.IsNullOrEmpty(currentSelected) &&
        //                ddlCoachLayout.Items.FindByValue(currentSelected) != null)
        //            {
        //                ddlCoachLayout.SelectedValue = currentSelected;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError($"Error loading coach layouts: {ex.Message}");
        //    }
        //}

        //private async Task LoadCoachTypes()
        //{
        //    try
        //    {
        //        HttpResponseMessage response =
        //            await client.GetAsync("TrainCoachTypes/GetTrainCoachTypes");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResponse = await response.Content.ReadAsStringAsync();
        //            List<CoachTypeDto> types =
        //                JsonConvert.DeserializeObject<List<CoachTypeDto>>(jsonResponse);

        //            string currentSelected = ddlCoachType.SelectedValue;

        //            ddlCoachType.Items.Clear();
        //            ddlCoachType.Items.Add(new ListItem("Select an option", ""));

        //            foreach (var type in types)
        //            {
        //                ddlCoachType.Items.Add(
        //                    new ListItem(type.CoachType, type.Id.ToString()));
        //            }

        //            if (!string.IsNullOrEmpty(currentSelected) &&
        //                ddlCoachType.Items.FindByValue(currentSelected) != null)
        //            {
        //                ddlCoachType.SelectedValue = currentSelected;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError($"Error loading coach types: {ex.Message}");
        //    }
        //}

        private async Task LoadFleetForEdit(int id)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeDto> fleetTypes =
                        JsonConvert.DeserializeObject<List<FleetTypeDto>>(jsonResponse);
                    var fleetType = fleetTypes.FirstOrDefault(f => f.Id == id);

                    if (fleetType != null)
                    {
                        hfFleetId.Value = fleetType.Id.ToString();
                        hfIsEdit.Value = "true";
                        txtName.Text = fleetType.Name;
                        txtDeck.Text = fleetType.Deck.HasValue
                                                   ? fleetType.Deck.Value.ToString()
                                                   : "";
                        txtSeatNo.Text = fleetType.NoOfSeats.ToString();
                        txtFacilities.Text = fleetType.Facilities ?? "";
                        chkHasAc.Checked = fleetType.HasAc == true;
                        lblModalTitle.Text = "Edit Fleet Type";

                        //string layoutVal = fleetType.CoachLayoutId.ToString();
                        //if (ddlCoachLayout.Items.FindByValue(layoutVal) != null)
                        //    ddlCoachLayout.SelectedValue = layoutVal;

                        //string typeVal = fleetType.CoachTypeId.ToString();
                        //if (ddlCoachType.Items.FindByValue(typeVal) != null)
                        //    ddlCoachType.SelectedValue = typeVal;

                        ScriptManager.RegisterStartupScript(this, GetType(),
                            "ShowEditModal",
                            @"Sys.Application.add_load(function() { showModal(); });",
                            true);
                    }
                    else
                    {
                        ShowError("Fleet type not found");
                    }
                }
                else
                {
                    ShowError("Failed to load fleet type details");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet type: {ex.Message}");
            }
        }

        private async Task LoadFleetTypes()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes_All");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeDto> fleetTypes =
                        JsonConvert.DeserializeObject<List<FleetTypeDto>>(jsonResponse);

                    fleetTypes = fleetTypes
                        .OrderByDescending(f => f.CreatedAt)
                        .ToList();

                    gvFleetTypes.DataSource = fleetTypes;
                    gvFleetTypes.DataBind();
                }
                else
                {
                    ShowError(
                        $"Failed to load fleet types. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                bool isEdit = hfIsEdit.Value == "true";

                if (isEdit)
                    await UpdateFleetType();
                else
                    await AddFleetType();
            }));
        }

        private async Task AddFleetType()
        {
            try
            {
                int noOfSeats = int.TryParse(txtSeatNo.Text, out int s) ? s : 0;
                int deck = int.TryParse(txtDeck.Text, out int d) ? d : 0;

                var requestData = new
                {
                    name = txtName.Text.Trim(),
                    //coachLayoutId = int.Parse(ddlCoachLayout.SelectedValue),
                    //coachTypeId = int.Parse(ddlCoachType.SelectedValue),
                    deck = deck,
                    noOfSeats = noOfSeats,
                    facilities = txtFacilities.Text.Trim(),
                    hasAc = chkHasAc.Checked,
                    isActive = true,
                    status = "ACTIVE",   // ✅ FIX: use "ACTIVE" not "Active"
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "system"
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TrainFleetTypes/PostTrainFleetType", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadFleetTypes();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Fleet type added successfully!');",
                        true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add fleet type: {errorContent}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding fleet type: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        private async Task UpdateFleetType()
        {
            try
            {
                int fleetId = int.Parse(hfFleetId.Value);

                HttpResponseMessage getResponse =
                    await client.GetAsync("TrainFleetTypes/GetTrainFleetTypes_All");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load fleet type details");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJson = await getResponse.Content.ReadAsStringAsync();
                List<FleetTypeDto> fleetTypes =
                    JsonConvert.DeserializeObject<List<FleetTypeDto>>(currentJson);
                var current = fleetTypes.FirstOrDefault(f => f.Id == fleetId);

                if (current == null)
                {
                    ShowError("Fleet type not found");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                int noOfSeats = int.TryParse(txtSeatNo.Text, out int s) ? s : 0;
                int deck = int.TryParse(txtDeck.Text, out int d) ? d : 0;

                var requestData = new
                {
                    fleetTypeId = fleetId,
                    name = txtName.Text.Trim(),
                    //coachLayoutId = int.Parse(ddlCoachLayout.SelectedValue),
                    //coachTypeId = int.Parse(ddlCoachType.SelectedValue),
                    deck = deck,
                    noOfSeats = noOfSeats,
                    facilities = txtFacilities.Text.Trim(),
                    hasAc = chkHasAc.Checked,
                    isActive = current.IsActive,
                    status = current.Status,
                    updatedBy = HttpContext.Current.User?.Identity?.Name ?? "system"
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    $"TrainFleetTypes/PutTrainFleetType/{fleetId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadFleetTypes();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Fleet type updated successfully!');",
                        true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update fleet type: {errorContent}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating fleet type: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        protected void gvFleetTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int fleetId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditFleet")
                RegisterAsyncTask(new PageAsyncTask(() => LoadFleetForEdit(fleetId)));
            else if (e.CommandName == "ToggleStatus")
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(fleetId)));
        }

        private async Task ToggleStatus(int fleetId)
        {
            try
            {
                // ✅ FIX: Use the correct route-parameter endpoint
                HttpResponseMessage response = await client.PostAsync(
                    $"TrainFleetTypes/ToggleFleetTypeStatus/{fleetId}",
                    new StringContent("", Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    await LoadFleetTypes();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Fleet type status updated successfully!');",
                        true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update status: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        public string GetTotalSeats(object noOfSeats)
        {
            if (noOfSeats == null) return "0";
            return noOfSeats.ToString();
        }

        public string GetFacilitiesDisplay(object facilities)
        {
            if (facilities == null) return "";
            return facilities.ToString();
        }

        // ✅ FIX: Compare against "ACTIVE" (uppercase) to match DB value
        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "Enabled" : "Disabled";
        }

        // ✅ FIX: Compare against "ACTIVE" (uppercase) to match DB value
        protected string GetStatusClasss(object status)
        {
            if (status == null) return "status-badge";
            return status.ToString() == "ACTIVE"
                ? "status-badge status-enabled"
                : "status-badge status-disabled";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID +
                "').classList.add('show');", true);
        }

        private void ClearForm()
        {
            txtName.Text = "";
            //ddlCoachLayout.SelectedIndex = 0;
            //ddlCoachType.SelectedIndex = 0;
            txtDeck.Text = "";
            txtSeatNo.Text = "";
            txtFacilities.Text = "";
            chkHasAc.Checked = false;
            hfFleetId.Value = "";
            hfIsEdit.Value = "false";
            lblModalTitle.Text = "Add New Fleet Type";
        }
    }

    public class FleetTypeDto
    {
        [JsonProperty("fleetTypeId")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("coachLayoutId")]
        public int CoachLayoutId { get; set; }

        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachLayout")]
        public string CoachLayout { get; set; }

        [JsonProperty("coachType")]
        public string CoachType { get; set; }

        [JsonProperty("deck")]
        public int? Deck { get; set; }

        [JsonProperty("noOfSeats")]
        public int NoOfSeats { get; set; }

        [JsonProperty("facilities")]
        public string Facilities { get; set; }

        [JsonProperty("hasAc")]
        public bool? HasAc { get; set; }

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

    public class CoachLayoutDto
    {
        [JsonProperty("coachLayoutId")]
        public int Id { get; set; }

        [JsonProperty("coachLayout")]
        public string Layout { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class CoachTypeDto
    {
        [JsonProperty("coachTypeId")]
        public int Id { get; set; }

        [JsonProperty("coachType")]
        public string CoachType { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}