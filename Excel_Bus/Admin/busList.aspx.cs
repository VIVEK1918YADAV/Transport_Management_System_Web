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

namespace Excel_Bus.Admin
{
    public partial class busList : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;
        static busList()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
                
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadAllVehicles();
                }));
            }
        }

        protected void gvVehicles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int vehicleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DisableVehicle")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ToggleVehicleStatus(vehicleId)));
            }
        }

        private async Task LoadAllVehicles(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;
                pnlSuccess.Visible = false;

                HttpResponseMessage response = await client.GetAsync("Vehicles/GetVehicles");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<VehicleModel> vehicles = JsonConvert.DeserializeObject<List<VehicleModel>>(jsonResponse);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        vehicles = vehicles.Where(v =>
                            (!string.IsNullOrEmpty(v.NickName) && v.NickName.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.RegisterNo) && v.RegisterNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.EngineNo) && v.EngineNo.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(v.ChasisNo) && v.ChasisNo.ToLower().Contains(searchTerm.ToLower()))
                        ).ToList();
                    }

                    var orderedVehicles = vehicles.OrderByDescending(v => v.CreatedAt).ToList();

                    ViewState["AllVehicles"] = JsonConvert.SerializeObject(orderedVehicles);

                    gvVehicles.DataSource = orderedVehicles;
                    gvVehicles.DataBind(); 
                }
                else
                {
                    ShowError($"Failed to load vehicles. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vehicles: {ex.Message}");
            }
        }

      
        private async Task ToggleVehicleStatus(int vehicleId)
        {
            try
            {
                var vehicleData = new { Id = vehicleId };

                string jsonData = JsonConvert.SerializeObject(vehicleData);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(  
                    $"Vehicles/ToggleVehicleStatus",  
                    content 
                );

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllVehicles();

                    string script = @"
                setTimeout(function() {
                    if (typeof window.showSuccess === 'function') {
                        window.showSuccess('Vehicle status updated successfully');
                    } else {
                        alert('Vehicle status updated successfully');
                    }
                }, 300);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", script, true);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update vehicle status: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }
       
        private void ShowError(string message)
        {
            //pnlError.Visible = true;
            //pnlSuccess.Visible = false;
            //lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllVehicles(searchTerm)));
        }


    }
}