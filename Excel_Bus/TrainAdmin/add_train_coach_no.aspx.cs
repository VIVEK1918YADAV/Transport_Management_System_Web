using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class add_train_coach_no : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static add_train_coach_no()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (!IsPostBack)
                {
                    await LoadTrains();
                    await LoadCoachDetails();
                }
            }));
        }



        private async Task LoadTrains()
        {
            try
            {
                pnlError.Visible = false;

                HttpResponseMessage response =
                    await client.GetAsync("TblTrainsRegs/GetActiveTrains");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<CoachPageTrainDto> trains =
                        JsonConvert.DeserializeObject<List<CoachPageTrainDto>>(json);

                    ddlTrain.Items.Clear();
                    ddlTrain.Items.Add(new ListItem("-- Select Train --", ""));

                    if (trains != null)
                    {
                        foreach (var t in trains)
                        {
                            
                            ddlTrain.Items.Add(new ListItem(
                                $"{t.TrainName} ({t.TrainNumber})",
                                t.TrainId));
                        }
                    }

                    ViewState["TrainList"] = json;
                }
                else
                {
                    ShowError($"Failed to load trains. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trains: {ex.Message}");
            }
        }


        protected void ddlTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCoachType.Items.Clear();
            ddlCoachType.Items.Add(new ListItem("-- Select Coach Type --", ""));
            ddlCoachType.Enabled = false;

            string selectedTrainId = ddlTrain.SelectedValue;
            if (string.IsNullOrEmpty(selectedTrainId)) return;

            string json = ViewState["TrainList"] as string;
            if (string.IsNullOrEmpty(json)) return;

            List<CoachPageTrainDto> trains =
                JsonConvert.DeserializeObject<List<CoachPageTrainDto>>(json);

            CoachPageTrainDto selected = trains?.Find(t => t.TrainId == selectedTrainId);
            if (selected == null || selected.TrainTrips == null || selected.TrainTrips.Count == 0) return;

            int tripId = selected.TrainTrips[0].TripId;
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await LoadCoachTypesByTrip(tripId);
            }));
        }

        private async Task LoadCoachTypesByTrip(int tripId)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync($"TrainTrips/GetTrainTripDetailsByTripId?tripId={tripId}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TrainTripDetailDto tripDetail =
                        JsonConvert.DeserializeObject<TrainTripDetailDto>(json);

                    ddlCoachType.Items.Clear();
                    ddlCoachType.Items.Add(new ListItem("-- Select Coach Type --", ""));

                    if (tripDetail?.Prices != null && tripDetail.Prices.Count > 0)
                    {
                        foreach (var price in tripDetail.Prices)
                        {
                            if (price.CoachType != null)
                            {
                                ddlCoachType.Items.Add(new ListItem(
                                    price.CoachType.CoachTypeName,
                                    price.CoachType.CoachTypeId.ToString()));
                            }
                        }
                        ddlCoachType.Enabled = true;
                    }
                }
                else
                {
                    ShowError($"Failed to load coach types. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading coach types: {ex.Message}");
            }
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                await AddCoachDetail();
            }));
        }

        private async Task AddCoachDetail()
        {
            try
            {
                string trainId = ddlTrain.SelectedValue;
                string coachTypeId = ddlCoachType.SelectedValue;
                int noOfCoach = int.Parse(txtNoOfCoach.Text.Trim());

                var requestData = new
                {
                    trainId = trainId,
                    coachTypeId = int.Parse(coachTypeId),
                    noOfCoach = noOfCoach,
                    isActive = true,
                    status = "ACTIVE",
                    createdBy = HttpContext.Current.User?.Identity?.Name ?? "Admin"
                };

                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("TblTrainDetails/PostTblTrainDetail", content);

                if (response.IsSuccessStatusCode)
                {
                    ddlTrain.SelectedIndex = 0;
                    ddlCoachType.Items.Clear();
                    ddlCoachType.Items.Add(new ListItem("-- Select Coach Type --", ""));
                    ddlCoachType.Enabled = false;
                    txtNoOfCoach.Text = "";

                    await LoadCoachDetails();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                        "showSuccess('Coach details added successfully!');", true);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ShowError($"{error}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding coach details: {ex.Message}");
            }
        }

      

        private async Task LoadCoachDetails()
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync("TblTrainDetails/GetTblTrainDetails");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<CoachPageDetailDto> list =
                        JsonConvert.DeserializeObject<List<CoachPageDetailDto>>(json);

                    if (list != null)
                    {
                        list.Sort((CoachPageDetailDto a, CoachPageDetailDto b) =>
                            DateTime.Compare(
                                b.CreatedAt ?? DateTime.MinValue,
                                a.CreatedAt ?? DateTime.MinValue));
                    }

                    gvCoachDetails.DataSource = list;
                    gvCoachDetails.DataBind();
                }
                else
                {
                    ShowError($"Failed to load coach details. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading coach details: {ex.Message}");
            }
        }

 

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            return status.ToString() == "ACTIVE" ? "Active" : "Inactive";
        }

        protected string GetStatusClass(object status)
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
                $"document.getElementById('{pnlError.ClientID}').classList.add('show');",
                true);
        }
    }

    public class TrainTripDto
    {
        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("routeId")]
        public int RouteId { get; set; }

        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class TrainTripDetailDto
    {
        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("prices")]
        public List<TripPriceDto> Prices { get; set; }
    }

    public class TripPriceDto
    {
        [JsonProperty("ticketPriceId")]
        public int TicketPriceId { get; set; }

        [JsonProperty("coachType")]
        public CoachTypeDetailDto CoachType { get; set; }
    }

    public class CoachTypeDetailDto
    {
        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachType")]
        public string CoachTypeName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("noOfSeats")]
        public int NoOfSeats { get; set; }
    }

    public class CoachPageTrainDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("fleetTypeId")]
        public int FleetTypeId { get; set; }

        [JsonProperty("fleetTypeName")]
        public string FleetTypeName { get; set; }

        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("coachTypeName")]
        public string CoachTypeName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("trainTrips")]         
        public List<TrainTripDto> TrainTrips { get; set; }
    }


    public class CoachPageDetailDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNumber")]
        public string TrainNumber { get; set; }

        [JsonProperty("coachTypeId")]
        public int CoachTypeId { get; set; }

        [JsonProperty("noOfCoach")]
        public int NoOfCoach { get; set; }

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
}