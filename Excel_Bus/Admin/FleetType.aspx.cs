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
    public partial class FleetType : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static FleetType()
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
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await LoadSeatLayouts();

                if (!IsPostBack)
                {
                    await LoadFleetTypes();
                }
            }));
        }

        private async Task LoadSeatLayouts()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("SeatLayouts/GetSeatLayouts");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<SeatLayoutDto> layouts = JsonConvert.DeserializeObject<List<SeatLayoutDto>>(jsonResponse);

                    string currentSelected = ddlSeatLayout.SelectedValue;

                    ddlSeatLayout.Items.Clear();
                    ddlSeatLayout.Items.Add(new ListItem("Select an option", ""));

                    foreach (var layout in layouts)
                    {
                        ddlSeatLayout.Items.Add(new ListItem(layout.Layout, layout.Layout));
                    }

                    if (!string.IsNullOrEmpty(currentSelected) && ddlSeatLayout.Items.FindByValue(currentSelected) != null)
                    {
                        ddlSeatLayout.SelectedValue = currentSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading seat layouts: {ex.Message}");
            }
        }

        private async Task LoadFleetForEdit(int id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("FleetTypes/GetFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeDto> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeDto>>(jsonResponse);
                    var fleetType = fleetTypes.FirstOrDefault(f => f.Id == id);

                    if (fleetType != null)
                    {
                        hfFleetId.Value = fleetType.Id.ToString();
                        hfIsEdit.Value = "true";
                        txtName.Text = fleetType.Name;
                        txtDeck.Text = fleetType.Deck.ToString();
                        chkHasAc.Checked = fleetType.HasAc == "1";  // ✅ Changed to string comparison
                        hfFacilities.Value = fleetType.Facilities;
                        hfDeckSeatsData.Value = fleetType.DeckSeats;

                        lblModalTitle.Text = "Edit Fleet Type";

                        if (ddlSeatLayout.Items.FindByValue(fleetType.SeatLayout) != null)
                        {
                            ddlSeatLayout.SelectedValue = fleetType.SeatLayout;
                        }
                        else
                        {
                            await LoadSeatLayouts();
                            if (ddlSeatLayout.Items.FindByValue(fleetType.SeatLayout) != null)
                            {
                                ddlSeatLayout.SelectedValue = fleetType.SeatLayout;
                            }
                        }

                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                            @"Sys.Application.add_load(function() { 
                        loadEditData(); 
                        showModal(); 
                    });", true);
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

                HttpResponseMessage response = await client.GetAsync("FleetTypes/GetFleetTypes");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<FleetTypeDto> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeDto>>(jsonResponse);

                    fleetTypes = fleetTypes.OrderByDescending(f => f.CreatedAt).ToList();

                    gvFleetTypes.DataSource = fleetTypes;
                    gvFleetTypes.DataBind();
                }
                else
                {
                    ShowError($"Failed to load fleet types. Status Code: {response.StatusCode}");
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
                {
                    await UpdateFleetType();
                }
                else
                {
                    await AddFleetType();
                }
            }));
        }

        private async Task AddFleetType()
        {
            try
            {
                int deckCount = int.Parse(txtDeck.Text);
                List<string> deckSeats = new List<string>();

                for (int i = 1; i <= deckCount; i++)
                {
                    string seatControlId = $"deckSeat{i}";
                    var control = Request.Form[seatControlId];
                    if (!string.IsNullOrEmpty(control))
                    {
                        deckSeats.Add(control);
                    }
                }

                List<string> facilities = new List<string>();
                if (!string.IsNullOrEmpty(hfFacilities.Value))
                {
                    facilities = JsonConvert.DeserializeObject<List<string>>(hfFacilities.Value);
                }

                var requestData = new
                {
                    name = txtName.Text.Trim(),
                    seatLayout = ddlSeatLayout.SelectedValue,
                    deck = deckCount,
                    deckSeats = JsonConvert.SerializeObject(deckSeats),
                    facilities = JsonConvert.SerializeObject(facilities),
                    hasAc = chkHasAc.Checked ? "1" : "0",  // ✅ Changed to string
                    status = "1"  // ✅ Changed to string
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("FleetTypes/PostFleetType", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadFleetTypes();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Fleet type added successfully!');", true);
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

                HttpResponseMessage getResponse = await client.GetAsync("FleetTypes/GetFleetTypes");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load fleet type details");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                List<FleetTypeDto> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeDto>>(currentJsonResponse);
                var currentFleetType = fleetTypes.FirstOrDefault(f => f.Id == fleetId);

                if (currentFleetType == null)
                {
                    ShowError("Fleet type not found");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                int deckCount = int.Parse(txtDeck.Text);
                List<string> deckSeats = new List<string>();

                for (int i = 1; i <= deckCount; i++)
                {
                    string seatControlId = $"deckSeat{i}";
                    var control = Request.Form[seatControlId];
                    if (!string.IsNullOrEmpty(control))
                    {
                        deckSeats.Add(control);
                    }
                }

                List<string> facilities = new List<string>();
                if (!string.IsNullOrEmpty(hfFacilities.Value))
                {
                    facilities = JsonConvert.DeserializeObject<List<string>>(hfFacilities.Value);
                }

                var requestData = new
                {
                    id = fleetId,
                    name = txtName.Text.Trim(),
                    seatLayout = ddlSeatLayout.SelectedValue,
                    deck = deckCount,
                    deckSeats = JsonConvert.SerializeObject(deckSeats),
                    facilities = JsonConvert.SerializeObject(facilities),
                    hasAc = chkHasAc.Checked ? "1" : "0",  // ✅ Changed to string
                    status = currentFleetType.Status  // ✅ Keep existing status (now string)
                };

                string json = JsonConvert.SerializeObject(requestData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"FleetTypes/PutFleetType/{fleetId}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadFleetTypes();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Fleet type updated successfully!');", true);
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
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadFleetForEdit(fleetId)));
            }
            else if (e.CommandName == "ToggleStatus")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleStatus(fleetId)));
            }
        }

        private async Task ToggleStatus(int fleetId)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync("FleetTypes/GetFleetTypes");

                if (getResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                    List<FleetTypeDto> fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeDto>>(jsonResponse);

                    var fleetType = fleetTypes.FirstOrDefault(f => f.Id == fleetId);

                    if (fleetType != null)
                    {
                        // ✅ Toggle status using string comparison
                        string newStatus = fleetType.Status == "1" ? "0" : "1";

                        var requestData = new
                        {
                            id = fleetId,
                            name = fleetType.Name,
                            seatLayout = fleetType.SeatLayout,
                            deck = fleetType.Deck,
                            deckSeats = fleetType.DeckSeats,
                            facilities = fleetType.Facilities,
                            hasAc = fleetType.HasAc,  // ✅ Keep as string
                            status = newStatus  // ✅ Now string
                        };

                        string json = JsonConvert.SerializeObject(requestData);
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync($"FleetTypes/PutFleetType/{fleetId}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            string statusText = newStatus == "1" ? "enabled" : "disabled";
                            await LoadFleetTypes();

                            ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                                $"showSuccess('Fleet type {statusText} successfully!');", true);
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            ShowError($"Failed to update status: {errorContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        public string GetTotalSeats(object deckSeats)
        {
            if (deckSeats == null) return "0";

            try
            {
                List<string> seats = JsonConvert.DeserializeObject<List<string>>(deckSeats.ToString());
                int total = seats.Sum(s => int.Parse(s));
                return total.ToString();
            }
            catch
            {
                return "0";
            }
        }

        public string GetFacilitiesDisplay(object facilities)
        {
            if (facilities == null) return "";

            try
            {
                List<string> facilityList = JsonConvert.DeserializeObject<List<string>>(facilities.ToString());
                return string.Join(", ", facilityList);
            }
            catch
            {
                return "";
            }
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();  // ✅ Changed to string
            return statusValue == "1" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();  // ✅ Changed to string
            return statusValue == "1" ? "status-enabled" : "status-disabled";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }

        private void ClearForm()
        {
            txtName.Text = "";
            ddlSeatLayout.SelectedIndex = 0;
            txtDeck.Text = "";
            chkHasAc.Checked = false;
            hfFleetId.Value = "";
            hfIsEdit.Value = "false";
            hfFacilities.Value = "";
            hfDeckSeatsData.Value = "";
            lblModalTitle.Text = "Add New Fleet Type";
        }
    }

    public class FleetTypeDto
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
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class SeatLayoutDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("layout")]
        public string Layout { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}