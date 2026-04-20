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

namespace Excel_Bus
{
    public partial class SeatLayout : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Static constructor to initialize HttpClient once
        static SeatLayout()
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
                // Register async task for WebForms
                RegisterAsyncTask(new PageAsyncTask(() => LoadSeatLayouts()));
            }
        }

        private async Task LoadSeatLayouts()
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                // Make API call
                HttpResponseMessage response = await client.GetAsync("SeatLayouts/GetSeatLayouts");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<SeatLayoutModel> layouts = JsonConvert.DeserializeObject<List<SeatLayoutModel>>(jsonResponse);

                    // Bind to GridView
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
            // Validate input
            string layout = txtLayout.Text.Trim();

            if (string.IsNullOrEmpty(layout))
            {
                ShowError("Please enter a layout value.");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
                return;
            }

            // Format layout with separator if not already present
            if (!layout.Contains(" x ") && layout.Length >= 1)
            {
                if (layout.Length == 1)
                {
                    ShowError("Please enter both left and right values.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
                    return;
                }

                // Add separator: "23" -> "2 x 3"
                layout = layout[0] + " x " + layout.Substring(1);
            }

            int layoutId = Convert.ToInt32(hdnLayoutId.Value);

            if (layoutId == 0)
            {
                // Add new layout
                RegisterAsyncTask(new PageAsyncTask(() => AddSeatLayout(layout)));
            }
            else
            {
                // Update existing layout
                RegisterAsyncTask(new PageAsyncTask(() => UpdateSeatLayout(layoutId, layout)));
            }
        }

        private async Task AddSeatLayout(string layout)
        {
            try
            {
                var newLayout = new { layout = layout };
                string jsonContent = JsonConvert.SerializeObject(newLayout);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("SeatLayouts/PostSeatLayout", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout added successfully!");
                    await LoadSeatLayouts();

                    // Clear form
                    txtLayout.Text = string.Empty;
                    hdnLayoutId.Value = "0";
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add seat layout. {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding seat layout: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
            }
        }

        private async Task UpdateSeatLayout(int id, string layout)
        {
            try
            {
                var updatedLayout = new
                {
                    id = id,
                    layout = layout
                };

                string jsonContent = JsonConvert.SerializeObject(updatedLayout);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"SeatLayouts/PutSeatLayout/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout updated successfully!");
                    await LoadSeatLayouts();

                    // Clear form
                    txtLayout.Text = string.Empty;
                    hdnLayoutId.Value = "0";
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update seat layout. {errorMessage}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating seat layout: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "document.getElementById('modalOverlay').classList.add('show');", true);
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
                HttpResponseMessage response = await client.DeleteAsync($"SeatLayouts/DeleteSeatLayout/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Seat layout removed successfully!");
                    await LoadSeatLayouts();
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

            // Add show class for animation
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;

            // Add show class for animation
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess",
                "document.getElementById('" + pnlSuccess.ClientID + "').classList.add('show');", true);
        }
    }

    // Model class for SeatLayout
    public class SeatLayoutModel
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