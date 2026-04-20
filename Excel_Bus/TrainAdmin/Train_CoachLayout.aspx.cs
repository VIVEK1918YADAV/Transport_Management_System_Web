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
    public partial class Train_CoachLayout : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_CoachLayout()
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
                // Register async tasks - removed duplicate LoadCoachTypes()
                RegisterAsyncTask(new PageAsyncTask(() => LoadCoachTypes()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadCoachLayouts()));
            }
        }

        private async Task LoadCoachTypes()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TrainCoachTypes/GetTrainCoachTypes");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var coachTypes = JsonConvert.DeserializeObject<List<CoachTypeModel>>(jsonResponse);

                    ddlCoachType.DataSource = coachTypes;
                    ddlCoachType.DataTextField = "CoachType";
                    ddlCoachType.DataValueField = "CoachTypeId";
                    ddlCoachType.DataBind();

                    ddlCoachType.Items.Insert(0, new ListItem("Select Coach Type", "0"));
                }
                else
                {
                    ShowError("Failed to load coach types.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading coach types: {ex.Message}");
            }
        }

        private async Task LoadCoachLayouts()
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                HttpResponseMessage response = await client.GetAsync("TrainCoachLayouts/GetTrainCoachLayouts");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainSeatLayoutModel> layouts = JsonConvert.DeserializeObject<List<TrainSeatLayoutModel>>(jsonResponse);

                    gvSeatLayouts.DataSource = layouts;
                    gvSeatLayouts.DataBind();
                }
                else
                {
                    ShowError($"Failed to load seat layouts. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading seat layouts: {ex.Message}");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string layout = txtLayout.Text.Trim();
            int selectedCoachTypeId = Convert.ToInt32(ddlCoachType.SelectedValue);

            // Validation
            if (string.IsNullOrEmpty(layout))
            {
                ShowError("Please enter a layout value.");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
                return;
            }

            if (selectedCoachTypeId == 0)
            {
                ShowError("Please select a coach type.");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
                return;
            }

            // Format layout with separator if not already present
            if (!layout.Contains(" x ") && layout.Length >= 1)
            {
                if (layout.Length == 1)
                {
                    ShowError("Please enter both left and right values.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('modalOverlay').classList.add('show');", true);
                    return;
                }
                layout = layout[0] + " x " + layout.Substring(1);
            }

            int layoutId = Convert.ToInt32(hdnLayoutId.Value);

            if (layoutId == 0)
            {
                RegisterAsyncTask(new PageAsyncTask(() => AddSeatLayout(layout, selectedCoachTypeId)));
            }
            else
            {
                RegisterAsyncTask(new PageAsyncTask(() => UpdateSeatLayout(layoutId, layout, selectedCoachTypeId)));
            }
        }

        private async Task AddSeatLayout(string layout, int coachTypeId)
        {
            try
            {
                var newLayout = new
                {
                    CoachLayout = layout,
                    CoachTypeId = coachTypeId
                };

                string jsonContent = JsonConvert.SerializeObject(newLayout);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("TrainCoachLayouts/PostTrainCoachLayout", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout added successfully!");
                    await LoadCoachLayouts();

                    // Clear form
                    txtLayout.Text = string.Empty;
                    ddlCoachType.SelectedIndex = 0;
                    hdnLayoutId.Value = "0";

                    // Close modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal",
                        "document.getElementById('modalOverlay').classList.remove('show'); document.body.style.overflow = 'auto';", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add seat layout. {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding seat layout: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
            }
        }

        private async Task UpdateSeatLayout(int id, string coachLayout, int coachTypeId)
        {
            try
            {
                var updatedLayout = new
                {
                    CoachLayoutId = id,
                    CoachLayout = coachLayout,
                    CoachTypeId = coachTypeId
                };

                string jsonContent = JsonConvert.SerializeObject(updatedLayout);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"TrainCoachLayouts/PutTrainCoachLayout/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout updated successfully!");
                    await LoadCoachLayouts();

                    // Clear form
                    txtLayout.Text = string.Empty;
                    ddlCoachType.SelectedIndex = 0;
                    hdnLayoutId.Value = "0";

                    // Close modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal",
                        "document.getElementById('modalOverlay').classList.remove('show'); document.body.style.overflow = 'auto';", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update seat layout. {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating seat layout: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
            }
        }

        protected void gvSeatLayouts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int layoutId = Convert.ToInt32(e.CommandArgument);
                RegisterAsyncTask(new PageAsyncTask(() => DeleteSeatLayout(layoutId)));
            }
        }

        private async Task DeleteSeatLayout(int id)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"TrainCoachLayouts/DeleteTrainCoachLayout/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout removed successfully!");
                    await LoadCoachLayouts();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to remove seat layout. {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error removing seat layout: {ex.Message}");
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

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess",
                "document.getElementById('" + pnlSuccess.ClientID + "').classList.add('show');", true);
        }
    }

    public class TrainSeatLayoutModel
    {
        [JsonProperty("coachLayoutId")]
        public int Id { get; set; }

        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachType")]
        public string CoachType { get; set; }

        [JsonProperty("coachLayout")]
        public string CoachLayout { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class CoachTypeModel
    {
        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachType")]
        public string CoachType { get; set; }
    }
}