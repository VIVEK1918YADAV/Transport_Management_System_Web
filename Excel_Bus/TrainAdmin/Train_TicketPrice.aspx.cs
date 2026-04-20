using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_TicketPrice : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static Train_TicketPrice()
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

        // Helper: tries multiple key names, returns first non-null value
        private static string GetString(JObject obj, params string[] keys)
        {
            foreach (var key in keys)
            {
                var token = obj[key];
                if (token != null && token.Type != JTokenType.Null && !string.IsNullOrWhiteSpace(token.ToString()))
                    return token.ToString();
            }
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainTicketPrices()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainFleetTypes()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrains()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadTrainRoute()));
                RegisterAsyncTask(new PageAsyncTask(() => LoadCoachType()));
            }
        }

        private async Task LoadTrainTicketPrices()
        {
            try
            {
                pnlError.Visible = false;
                string endpoint = "TrainTicketPrices/GetTrainTicketPricesAll";

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainTicketPriceModel> ticketPrices = JsonConvert.DeserializeObject<List<TrainTicketPriceModel>>(jsonResponse);

                    var orderedPrices = ticketPrices.OrderByDescending(t => t.CreatedAt).ToList();

                    ViewState["onlinePrice"] = JsonConvert.SerializeObject(orderedPrices);

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

        private async Task LoadTrainFleetTypes()
        {
            try
            {
                string endpoint = "TrainFleetTypes/GetTrainFleetTypes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var array = JArray.Parse(jsonResponse);

                    ddlFleetType.Items.Clear();
                    ddlFleetType.Items.Add(new ListItem("Select Fleet Type", "0"));

                    foreach (JObject item in array)
                    {
                        // FIX: API returns "name" not "FleetTypeName"
                        string displayName = GetString(item, "name", "Name", "fleetTypeName", "FleetTypeName");
                        string idValue = GetString(item, "fleetTypeId", "FleetTypeId", "id", "Id");

                        if (!string.IsNullOrWhiteSpace(displayName) && !string.IsNullOrWhiteSpace(idValue))
                            ddlFleetType.Items.Add(new ListItem(displayName, idValue));
                    }
                }
                else
                {
                    ShowError($"Error loading fleet types. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading fleet types: {ex.Message}");
            }
        }

        private async Task LoadTrainRoute()
        {
            try
            {
                string endpoint = "TrainRoutes/GetActiveTrainRoutes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var routes = JsonConvert.DeserializeObject<List<TrainRouteModel>>(jsonResponse);

                    ddlRoute.DataSource = routes;
                    ddlRoute.DataTextField = "RouteName";
                    ddlRoute.DataValueField = "RouteId";
                    ddlRoute.DataBind();
                    ddlRoute.Items.Insert(0, new ListItem("Select Route", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading routes: {ex.Message}");
            }
        }

        private async Task LoadTrains()
        {
            try
            {
                string endpoint = "TblTrainsRegs/GetTblTrainsRegs";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var trains = JsonConvert.DeserializeObject<List<TrainModel1>>(jsonResponse);

                    ddlVehicle.DataSource = trains;
                    ddlVehicle.DataTextField = "TrainName";
                    ddlVehicle.DataValueField = "TrainId";
                    ddlVehicle.DataBind();
                    ddlVehicle.Items.Insert(0, new ListItem("Select Train", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trains: {ex.Message}");
            }
        }

        private async Task LoadCoachType()
        {
            try
            {
                string endpoint = "TrainCoachTypes/GetTrainCoachTypes";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var array = JArray.Parse(jsonResponse);

                    ddlCoachType.Items.Clear();
                    ddlCoachType.Items.Add(new ListItem("Select Coach Type", "0"));

                    foreach (JObject item in array)
                    {
                        // FIX: API returns "coachType" not "coachTypeName"
                        string displayName = GetString(item, "coachType", "CoachType", "coachTypeName", "CoachTypeName", "name", "Name");
                        string idValue = GetString(item, "coachTypeId", "CoachTypeId", "id", "Id");

                        if (!string.IsNullOrWhiteSpace(displayName) && !string.IsNullOrWhiteSpace(idValue))
                            ddlCoachType.Items.Add(new ListItem(displayName, idValue));
                    }
                }
                else
                {
                    ShowError($"Error loading coach types. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading coach types: {ex.Message}");
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
            RegisterAsyncTask(new PageAsyncTask(() => AddTrainTicketPrice()));
        }

        private async Task AddTrainTicketPrice()
        {
            try
            {
                if (ddlFleetType.SelectedValue == "0" || ddlCoachType.SelectedValue == "0" || ddlVehicle.SelectedValue == "0" || ddlRoute.SelectedValue == "0")
                {
                    ShowError("Please select all");
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
                    RouteId = int.Parse(ddlRoute.SelectedValue),
                    trainId = ddlVehicle.SelectedValue,
                    coachTypeId = int.Parse(ddlCoachType.SelectedValue),
                    onlinePrice = price
                };

                string endpoint = "TrainTicketPrices/PostTrainTicketPrice";
                var content = new StringContent(JsonConvert.SerializeObject(newTicketPrice), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price added successfully!");
                    await LoadTrainTicketPrices();

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
                string endpoint = $"TrainTicketPrices/GetTrainTicketPrice/{id}";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    TrainTicketPriceModel ticketPrice = JsonConvert.DeserializeObject<TrainTicketPriceModel>(jsonResponse);

                    ViewState["EditTicketPriceId"] = id;
                    lblEditRoute.Text = ticketPrice.TrainId.ToString();
                    txtEditPrice.Text = ticketPrice.OnlinePrice.ToString("0.00");

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

                string trainId = ViewState["EditTicketPriceId"].ToString();
                decimal price;

                if (!decimal.TryParse(txtEditPrice.Text.Trim(), out price) || price <= 0)
                {
                    ShowError("Please enter a valid price");
                    return;
                }

                var updateData = new
                {
                    TrainId = trainId,
                    onlinePrice = price
                };

                string endpoint = $"TrainTicketPrices/PutTrainTicketPrice?trainId=" + trainId;
                var content = new StringContent(JsonConvert.SerializeObject(updateData), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price updated successfully!");
                    await LoadTrainTicketPrices();

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
                string endpoint = $"TrainTicketPrices/DeleteTrainTicketPrice/{id}";
                HttpResponseMessage response = await client.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    ShowSuccess("Ticket price removed successfully!");
                    await LoadTrainTicketPrices();
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
            ddlRoute.SelectedIndex = 0;
            ddlVehicle.SelectedIndex = 0;
            ddlCoachType.SelectedIndex = 0;
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

    public class TrainTicketPriceModel
    {
        [JsonProperty("ticketPriceId")] public int TicketPriceId { get; set; }
        [JsonProperty("fleetTypeId")] public int? FleetTypeId { get; set; }
        [JsonProperty("routeId")] public int? RouteId { get; set; }
        [JsonProperty("routeName")] public string RouteName { get; set; }
        [JsonProperty("trainName")] public string TrainName { get; set; }
        [JsonProperty("fleetTypeName")] public string fleetTypeName { get; set; }
        [JsonProperty("coachTypeName")] public string CoachTypeName { get; set; }
        [JsonProperty("trainId")] public string TrainId { get; set; }
        [JsonProperty("coachTypeId")] public int? CoachTypeId { get; set; }
        [JsonProperty("onlinePrice")] public decimal OnlinePrice { get; set; }
        [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
        [JsonProperty("updatedAt")] public DateTime UpdatedAt { get; set; }
    }

    public class TrainModel1
    {
        public int Id { get; set; }
        public string TrainId { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
    }

    public class CoachTypeModel1
    {
        public int CoachTypeId { get; set; }

        // FIX: API returns "coachType" not "coachTypeName"
        [JsonProperty("coachType")]
        public string CoachTypeName { get; set; }
    }

    public class TrainRouteModel
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string RouteCode { get; set; }
    }

    public partial class TrainFleetType
    {
        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        // FIX: API returns "name" not "FleetTypeName"
        [JsonProperty("name")]
        public string FleetTypeName { get; set; }

        public int CoachLayoutId { get; set; }
        public int CoachTypeId { get; set; }
        public int Deck { get; set; }
        public int NoOfSeats { get; set; }
        public string Facilities { get; set; }
        public bool HasAc { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}