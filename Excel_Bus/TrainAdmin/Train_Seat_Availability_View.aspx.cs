//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Configuration;
//using System.Net.Http.Headers;

//namespace Excel_Bus.TrainAdmin
//{
//    public partial class Train_Seat_Availability_View : System.Web.UI.Page
//    {
//        private static readonly HttpClient client;
//        private static readonly string apiUrl;

//        static Train_Seat_Availability_View()
//        {
//            apiUrl = ConfigurationSettings.AppSettings["api_path"];
//            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
//            client.DefaultRequestHeaders.Accept.Clear();
//            client.DefaultRequestHeaders.Accept.Add(
//                new MediaTypeWithQualityHeaderValue("application/json"));
//        }
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                LoadTrainData();
//            }
//        }



//        private async Task LoadTrainData()
//        {
//            //string apiBase = System.Configuration.ConfigurationManager.AppSettings["api_path"];

//            using (HttpClient client = new HttpClient())
//            {
//                try
//                {
//                    // Example API call
//                    var tripsRes = await client.GetStringAsync(apiUrl + "TrainTrips/GetTrainTrips");
//                    var trainsRes = await client.GetStringAsync(apiUrl + "TblTrainsRegs/GetTblTrainsRegs");

//                    var result = new
//                    {
//                        trips = JsonConvert.DeserializeObject(tripsRes),
//                        trains = JsonConvert.DeserializeObject(trainsRes)
//                    };

//                    // Convert to JSON and store in hidden field
//                    hdnTrainData.Value = JsonConvert.SerializeObject(result);
//                }
//                catch (Exception ex)
//                {
//                    hdnTrainData.Value = "{}";
//                }
//            }
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Bus.TrainAdmin
{
    public partial class Train_Seat_Availability_View : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationManager.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                RegisterAsyncTask(new PageAsyncTask(InitializeFilters));
            }
        }

        private async Task InitializeFilters()
        {
            await LoadTrains();

            // If you need to load the coach types for the first train, do it here
            if (ddlTrains.SelectedValue != "")
            {
                await LoadCoachTypes(int.Parse(ddlTrains.SelectedValue)); // Assuming trainId is numeric
            }
        }

        private async Task LoadTrains()
        {
            var response = await client.GetAsync($"{apiUrl}TblTrainsRegs/GetTblTrainsRegs1");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var trains = JsonConvert.DeserializeObject<List<dynamic>>(json);

                // Bind trains to the dropdown
                ddlTrains.DataSource = trains;
                ddlTrains.DataTextField = "trainName";  // Display field
                ddlTrains.DataValueField = "trainId";   // Value field
                ddlTrains.DataBind();
                ddlTrains.Items.Insert(0, new ListItem("-- Select Train --", ""));
            }
        }

        protected async void ddlTrains_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected train ID
            string selectedTrainId = ddlTrains.SelectedValue;

            if (string.IsNullOrEmpty(selectedTrainId))
            {
                return;  // No train selected
            }

            // Fetch details for the selected train
            string trainId = selectedTrainId; // Example trainId, replace with your actual value

            var response = await client.GetAsync($"{apiUrl}TblTrainsRegs/GetTblTrainsRegs2/{trainId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Deserialize into a single object, not a list
                var selectedTrain = JsonConvert.DeserializeObject<dynamic>(json);

                if (selectedTrain != null)
                {
                    // Ensure that fleetTypeId is an integer and not null
                    int fleetTypeId = selectedTrain.fleetTypeId;

                    // Load Coach Types based on FleetTypeId of selected train
                    await LoadCoachTypes(fleetTypeId);

                    // Update Seat Info and Display Layout
                    lblInfo.Text = $"Train: {selectedTrain.trainName}<br/>" +
                                   $"Seats Count: {selectedTrain.seatsCount}<br/>" +
                                   $"Coach Layout: {selectedTrain.layout}";

                    // Call function to display seat layout
                    // DisplaySeatLayout(selectedTrain.seatsCount, selectedTrain.layout);
                }
            }
        }
        private async Task LoadCoachTypes(int fleetTypeId)
        {
            var response = await client.GetAsync($"{apiUrl}TrainCoachTypes/GetTrainCoachTypes?fleetTypeId={fleetTypeId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var types = JsonConvert.DeserializeObject<List<dynamic>>(json);

                ddlCoachType.DataSource = types;
                ddlCoachType.DataTextField = "coachType";
                ddlCoachType.DataValueField = "coachTypeId";
                ddlCoachType.DataBind();
                ddlCoachType.Items.Insert(0, new ListItem("-- Select Coach --", ""));
            }
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDate.Text) && ddlTrains.SelectedIndex > 0 && ddlCoachType.SelectedIndex > 0)
            {
                RegisterAsyncTask(new PageAsyncTask(GenerateAvailabilityLayout));
            }
        }

        private async Task GenerateAvailabilityLayout()
        {
            pnlSeats.Controls.Clear();
            string trainId = ddlTrains.SelectedValue;
            string coachTypeId = ddlCoachType.SelectedValue;
            string date = txtDate.Text;

            // 1. Fetch Booked Seats from API
            List<string> bookedSeats = new List<string>();
            var bookedResponse = await client.GetAsync($"{apiUrl}TrainTicketBookings/GetTrainBookedSeats?trainId={trainId}&coachTypeId={coachTypeId}&dateOfJourney={date}");

            if (bookedResponse.IsSuccessStatusCode)
            {
                var json = await bookedResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                if (result.bookedSeats != null)
                    bookedSeats = ((JArray)result.bookedSeats).Select(x => x.ToString()).ToList();
            }

            // 2. Define Layout (Example 2x2, total 40 seats)
            // Aap isse dynamic bhi kr skte hain apne Database ke "NoOfSeats" column se
            int totalSeats = 40;
            int seatsPerRow = 4;

            Panel rowPanel = null;
            for (int i = 1; i <= totalSeats; i++)
            {
                if ((i - 1) % seatsPerRow == 0)
                {
                    rowPanel = new Panel { CssClass = "seat-row" };
                    pnlSeats.Controls.Add(rowPanel);
                }

                string seatNo = i.ToString();
                bool isBooked = bookedSeats.Contains(seatNo);

                Label lblSeat = new Label
                {
                    Text = seatNo,
                    CssClass = "seat " + (isBooked ? "booked" : "available"),
                    ToolTip = isBooked ? "Occupied" : "Available"
                };

                rowPanel.Controls.Add(lblSeat);

                // Add Aisle (Gali) in between 2x2
                if (i % 2 == 0 && i % 4 != 0)
                {
                    rowPanel.Controls.Add(new Panel { CssClass = "aisle" });
                }
            }

            lblInfo.Text = $"Showing layout for {ddlTrains.SelectedItem.Text} - {date}";
        }
        private void DisplaySeatLayout(int seatsCount, string layout)
        {
            // Validate and convert if necessary
            seatsCount = (seatsCount > 0) ? seatsCount : 0; // Default to 0 if invalid
            layout = string.IsNullOrEmpty(layout) ? "2 × 2" : layout; // Default to "2 × 2" if layout is empty

            // Clear previous seats
            pnlSeats.Controls.Clear();

            // Example: Handling layout '2 × 2' (2 seats per row)
            int seatsPerRow = layout == "2 × 1" ? 2 : (layout == "2 × 2" ? 4 : 2);  // Simple layout handling
            int totalRows = (int)Math.Ceiling((double)seatsCount / seatsPerRow);

            // Create seat rows dynamically
            for (int row = 0; row < totalRows; row++)
            {
                var seatRow = new HtmlGenericControl("div");
                seatRow.Attributes["class"] = "seat-row";

                for (int col = 0; col < seatsPerRow; col++)
                {
                    var seat = new HtmlGenericControl("div");
                    seat.Attributes["class"] = "seat available"; // You can dynamically check availability if needed
                    seat.InnerHtml = $"{row * seatsPerRow + col + 1}"; // Seat number
                    seatRow.Controls.Add(seat);
                }

                pnlSeats.Controls.Add(seatRow);
            }
        }

        // Usage
        //int seatsCount = selectedTrain.seatsCount ?? 0; // Default to 0 if null
        //string layout = selectedTrain.layout ?? "2 × 2"; // Default to "2 × 2" if layout is null

    }
}