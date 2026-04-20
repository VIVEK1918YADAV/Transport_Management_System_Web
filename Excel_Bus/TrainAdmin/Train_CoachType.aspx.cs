using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_CoachType : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_CoachType()
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
                RegisterAsyncTask(new PageAsyncTask(LoadCoachTypes));
            }
        }

        private async Task LoadCoachTypes()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("TrainCoachTypes/GetTrainCoachTypes");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<TrainCoachTypeModel>>(json);
                    gvSeatLayouts.DataSource = list;
                    gvSeatLayouts.DataBind();
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string coachType = txtCoachType.Text.Trim();

            if (string.IsNullOrEmpty(coachType))
            {
                ShowError("Coach Type cannot be empty.");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
                return;
            }

            int coachTypeId = Convert.ToInt32(hdnTypeId.Value);

            if (coachTypeId == 0)
                RegisterAsyncTask(new PageAsyncTask(() => AddCoachType(coachType)));
            else
                RegisterAsyncTask(new PageAsyncTask(() => UpdateCoachType(coachTypeId, coachType)));
        }

        private async Task AddCoachType(string coachType)
        {
            try
            {
                var newType = new { CoachType = coachType };  // ✅ Fixed: was missing CoachType value

                string jsonContent = JsonConvert.SerializeObject(newType);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("TrainCoachTypes/PostTrainCoachType", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Coach Type added successfully!");
                    txtCoachType.Text = string.Empty;
                    hdnTypeId.Value = "0";
                    await LoadCoachTypes();  // ✅ Refresh grid
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal",
                        "document.getElementById('modalOverlay').classList.remove('show'); document.body.style.overflow='';", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add coach type. {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding coach type: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
            }
        }

        private async Task UpdateCoachType(int coachTypeId, string coachType)
        {
            try
            {
                var updated = new { CoachTypeId = coachTypeId, CoachType = coachType };

                string jsonContent = JsonConvert.SerializeObject(updated);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"TrainCoachTypes/PutTrainCoachType/{coachTypeId}", content);  // ✅ PUT not POST

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Coach type updated successfully!");
                    txtCoachType.Text = string.Empty;
                    hdnTypeId.Value = "0";
                    await LoadCoachTypes();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal",
                        "document.getElementById('modalOverlay').classList.remove('show'); document.body.style.overflow='';", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update coach type. {error}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating coach type: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('modalOverlay').classList.add('show');", true);
            }
        }

        protected void gvSeatLayouts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int typeId = Convert.ToInt32(e.CommandArgument);
                RegisterAsyncTask(new PageAsyncTask(() => DeleteCoachType(typeId)));
            }
        }

        private async Task DeleteCoachType(int id)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"TrainCoachTypes/DeleteTrainCoachType/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Coach type removed successfully!");
                    await LoadCoachTypes();  // ✅ Refresh grid after delete
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to remove coach type. {error}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error removing coach type: {ex.Message}");
            }
        }

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
    }

    public class TrainCoachTypeModel
    {
        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachType")]
        public string CoachType { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}