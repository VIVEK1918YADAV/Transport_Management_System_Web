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
    public partial class Counter : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Counter()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                //Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllCounters()));
            }
        }

        private async Task LoadAllCounters(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;

                string endpoint = "Counters/GetCounters";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<CounterModel> counters = JsonConvert.DeserializeObject<List<CounterModel>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        counters = counters.Where(c =>
                            (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(c.City) && c.City.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(c.Location) && c.Location.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(c.Mobile) && c.Mobile.Contains(searchTerm))
                        ).ToList();
                    }

                    var orderedCounters = counters.OrderByDescending(c => c.CreatedAt).ToList();

                    ViewState["AllCounters"] = JsonConvert.SerializeObject(orderedCounters);

                    gvCounters.DataSource = orderedCounters;
                    gvCounters.DataBind();
                }
                else
                {
                    ShowError($"Failed to load counters. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading counters: {ex.Message}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllCounters(searchTerm)));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int counterId = Convert.ToInt32(hdnCounterId.Value);

                if (counterId == 0)
                {
                    RegisterAsyncTask(new PageAsyncTask(() => AddCounter()));
                }
                else
                {
                    RegisterAsyncTask(new PageAsyncTask(() => UpdateCounter(counterId)));
                }
            }
        }

        private async Task AddCounter()
        {
            try
            {
                var counter = new CounterModel
                {
                    Name = txtName.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Mobile = txtMobile.Text.Trim(),
                    Status = "1"  // ✅ Changed to string
                };

                string json = JsonConvert.SerializeObject(counter);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("Counters/PostCounter", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllCounters();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Counter added successfully');", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add counter: {errorMessage}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding counter: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        private async Task UpdateCounter(int id)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync($"Counters/GetCounter/{id}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load counter details");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                    return;
                }

                string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                CounterModel currentCounter = JsonConvert.DeserializeObject<CounterModel>(currentJsonResponse);

                var counter = new CounterModel
                {
                    Id = id,
                    Name = txtName.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Mobile = txtMobile.Text.Trim(),
                    Status = currentCounter.Status  // ✅ Keep existing status (now string)
                };

                string json = JsonConvert.SerializeObject(counter);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"Counters/PutCounter/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllCounters();
                    ClearForm();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "hideModal(); showSuccess('Counter updated successfully');", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update counter: {errorMessage}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating counter: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "showModal();", true);
            }
        }

        protected void gvCounters_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int counterId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditCounter")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadCounterForEdit(counterId)));
            }
            else if (e.CommandName == "DisableCounter")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleCounterStatus(counterId)));
            }
        }

        private async Task LoadCounterForEdit(int id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"Counters/GetCounter/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    CounterModel counter = JsonConvert.DeserializeObject<CounterModel>(jsonResponse);

                    hdnCounterId.Value = counter.Id.ToString();
                    txtName.Text = counter.Name;
                    txtCity.Text = counter.City;
                    txtLocation.Text = counter.Location;
                    txtMobile.Text = counter.Mobile;

                    lblModalTitle.Text = "Edit Counter";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        @"Sys.Application.add_load(function() { 
                    showModal(); 
                });", true);
                }
                else
                {
                    ShowError("Failed to load counter details");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading counter: {ex.Message}");
            }
        }

        private async Task ToggleCounterStatus(int id)
        {
            try
            {
                // Get current counter data from API
                HttpResponseMessage getResponse = await client.GetAsync($"Counters/GetCounter/{id}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load counter details");
                    return;
                }

                string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                CounterModel counter = JsonConvert.DeserializeObject<CounterModel>(jsonResponse);

                // Toggle status - ✅ Now working with strings
                string newStatus = counter.Status == "1" ? "0" : "1";
                counter.Status = newStatus;

                string json = JsonConvert.SerializeObject(counter);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"Counters/PutCounter/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Reload grid data to update UI
                    await LoadAllCounters();

                    string statusText = newStatus == "1" ? "enabled" : "disabled";
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        $"showSuccess('Counter {statusText} successfully');", true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update counter status: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            hdnCounterId.Value = "0";
            txtName.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtLocation.Text = string.Empty;
            txtMobile.Text = string.Empty;
            lblModalTitle.Text = "Add New Counter";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
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
    }

    public class CounterModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }  // ✅ Changed from int to string

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}