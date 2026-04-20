using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class TicketPrice : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static TicketPrice()
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

        public static string FormatPrice(object price)
        {
            if (price == null) return "0.00 CDF";

            decimal amount;
            if (price is decimal)
            {
                amount = (decimal)price;
            }
            else if (decimal.TryParse(price.ToString(), out amount))
            {
                // Parsed successfully
            }
            else
            {
                return "0.00 CDF";
            }

            return amount.ToString("N2") + " CDF";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadTicketPrices()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadFleetTypes()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadVehicles()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadRoute()));
            }
        }

        private async Task LoadTicketPrices()
        {
            try
            {
                pnlError.Visible = false;
                string endpoint = "TicketPrices/GetTicketPrices";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TicketPriceModel> ticketPrices = JsonConvert.DeserializeObject<List<TicketPriceModel>>(jsonResponse);

                    var orderedPrices = ticketPrices.OrderByDescending(t => t.CreatedAt).ToList();

                    ViewState["TicketPrices"] = JsonConvert.SerializeObject(orderedPrices);

                    gvTicketPrices.DataSource = orderedPrices;
                    gvTicketPrices.DataBind();
                }
                else
                {
                    ShowError($"Failed to load ticket prices. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading ticket prices: {ex.Message}");
            }
        }

        private async Task LoadFleetTypes()
        {
            try
            {
                string endpoint = "FleetTypes/GetFleetTypes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var fleetTypes = JsonConvert.DeserializeObject<List<FleetTypeModel>>(jsonResponse);

                    // Debug: Log all fleet types and their status
                    System.Diagnostics.Debug.WriteLine($"Total Fleet Types: {fleetTypes?.Count ?? 0}");
                    if (fleetTypes != null)
                    {
                        foreach (var ft in fleetTypes)
                        {
                            System.Diagnostics.Debug.WriteLine($"Fleet: {ft.Name}, Status: '{ft.Status}'");
                        }
                    }

                    // Filter fleet types - try multiple status values
                    var enabledFleetTypes = fleetTypes?
                        .Where(f => f.Status != null &&
                               (f.Status.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
                                f.Status.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                                f.Status.Equals("Enabled", StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    // If no fleet types match the filter, show all fleet types
                    if (enabledFleetTypes == null || enabledFleetTypes.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No fleet types matched filter - showing all");
                        enabledFleetTypes = fleetTypes;
                    }

                    ddlFleetType.DataSource = enabledFleetTypes;
                    ddlFleetType.DataTextField = "Name";
                    ddlFleetType.DataValueField = "Id";
                    ddlFleetType.DataBind();
                    ddlFleetType.Items.Insert(0, new ListItem("Select Fleet Type", "0"));

                    System.Diagnostics.Debug.WriteLine($"Loaded {enabledFleetTypes?.Count ?? 0} fleet types to dropdown");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception in LoadFleetTypes: {ex.Message}");
            }
        }

        private async Task LoadRoute()
        {
            try
            {
                string endpoint = "VehicleRoutes/GetVehicleRoutes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var routes = JsonConvert.DeserializeObject<List<RouteModel>>(jsonResponse);

                    ddlRoute.DataSource = routes;
                    ddlRoute.DataTextField = "Name";
                    ddlRoute.DataValueField = "Id";
                    ddlRoute.DataBind();
                    ddlRoute.Items.Insert(0, new ListItem("Select Route", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading routes: {ex.Message}");
            }
        }
        private async Task LoadVehicles()
        {
            try
            {
                string endpoint = "Vehicles/GetVehicles";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var routes = JsonConvert.DeserializeObject<List<VehicleModel1>>(jsonResponse);

                    ddlVehicle.DataSource = routes;
                    ddlVehicle.DataTextField = "nickName";
                    ddlVehicle.DataValueField = "Id";
                    ddlVehicle.DataBind();
                    ddlVehicle.Items.Insert(0, new ListItem("Select Bus", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading routes: {ex.Message}");
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            pnlList.Visible = false;
            pnlAdd.Visible = true;
            pnlEdit.Visible = false;
            ClearAddForm();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => AddTicketPrice()));
        }

        private async Task AddTicketPrice()
        {
            try
            {
                if (ddlFleetType.SelectedValue == "0" || ddlVehicle.SelectedValue == "0")
                {
                    ShowError("Please select both Fleet Type and Route");
                    return;
                }

                decimal price;
                if (!decimal.TryParse(txtPrice.Text.Trim(), out price) || price <= 0)
                {
                    ShowError("Please enter a valid price");
                    return;
                }

                var newTicketPrice = new
                {
                    fleetTypeId = int.Parse(ddlFleetType.SelectedValue),
                    vehicleRouteId = int.Parse(ddlRoute.SelectedValue),
                    vehicleId = int.Parse(ddlVehicle.SelectedValue),
                    price = price
                };

                string endpoint = "TicketPrices/PostTicketPrice";
                var content = new StringContent(JsonConvert.SerializeObject(newTicketPrice), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price added successfully!");
                    await LoadTicketPrices();

                    pnlAdd.Visible = false;
                    pnlList.Visible = true;
                    ClearAddForm();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to add ticket price. Status: {response.StatusCode}. {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding ticket price: {ex.Message}");
            }
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            pnlAdd.Visible = false;
            pnlEdit.Visible = false;
            pnlList.Visible = true;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            pnlAdd.Visible = false;
            pnlList.Visible = true;
        }

        protected void gvTicketPrices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int ticketPriceId = Convert.ToInt32(gvTicketPrices.DataKeys[rowIndex].Value);

            if (e.CommandName == "EditPrice")
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadTicketPriceForEdit(ticketPriceId)));
            }
            else if (e.CommandName == "DeletePrice")
            {
                RegisterAsyncTask(new PageAsyncTask(() => DeleteTicketPrice(ticketPriceId)));
            }
        }

        private async Task LoadTicketPriceForEdit(int id)
        {
            try
            {
                string endpoint = $"TicketPrices/GetTicketPrice/{id}";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    TicketPriceModel ticketPrice = JsonConvert.DeserializeObject<TicketPriceModel>(jsonResponse);

                    ViewState["EditTicketPriceId"] = id;
                    lblEditRoute.Text = ticketPrice.Vehicle;
                    txtEditPrice.Text = ticketPrice.Price.ToString("0.00");

                    pnlList.Visible = false;
                    pnlAdd.Visible = false;
                    pnlEdit.Visible = true;
                }
                else
                {
                    ShowError($"Failed to load ticket price. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading ticket price: {ex.Message}");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(() => UpdateTicketPrice()));
        }

        private async Task UpdateTicketPrice()
        {
            try
            {
                if (ViewState["EditTicketPriceId"] == null)
                {
                    ShowError("Invalid ticket price ID");
                    return;
                }

                int id = Convert.ToInt32(ViewState["EditTicketPriceId"]);
                decimal price;

                if (!decimal.TryParse(txtEditPrice.Text.Trim(), out price) || price <= 0)
                {
                    ShowError("Please enter a valid price");
                    return;
                }

                var updateData = new
                {
                    id = id,
                    price = price
                };

                string endpoint = $"TicketPrices/PutTicketPrice/{id}";
                var content = new StringContent(JsonConvert.SerializeObject(updateData), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price updated successfully!");
                    await LoadTicketPrices();

                    pnlEdit.Visible = false;
                    pnlList.Visible = true;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update ticket price. Status: {response.StatusCode}. {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating ticket price: {ex.Message}");
            }
        }

        private async Task DeleteTicketPrice(int id)
        {
            try
            {
                string endpoint = $"TicketPrices/DeleteTicketPrice/{id}";
                HttpResponseMessage response = await client.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price removed successfully!");
                    await LoadTicketPrices();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to remove ticket price. Status: {response.StatusCode}. {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error removing ticket price: {ex.Message}");
            }
        }

        private void ClearAddForm()
        {
            ddlFleetType.SelectedIndex = 0;
            //ddlVehicle.SelectedIndex = 0;
            txtPrice.Text = "";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
        }
    }

    // Model specific to TicketPrice page
    public class TicketPriceModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fleetType")]
        public string FleetType { get; set; }

        [JsonProperty("vehicleRoute")]
        public string VehicleRoute { get; set; }

        [JsonProperty("vehicleName")]
        public string Vehicle { get; set; }  

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

  

    public class VehicleModel1
    {
        public int Id { get; set; }
        public string NickName { get; set; }
    }
    public class RouteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}