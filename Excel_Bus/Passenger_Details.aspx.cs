using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class Passenger_Details : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get query parameters for postpone mode
                string pnrNumber = Request.QueryString["pnrNumber"] ?? "";
                string bookedId = Request.QueryString["bookedId"] ?? "";
                string status = Request.QueryString["status"] ?? "";

                System.Diagnostics.Debug.WriteLine("=== PAGE_LOAD DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"PNR from QueryString: {pnrNumber}");
                System.Diagnostics.Debug.WriteLine($"BookedId from QueryString: {bookedId}");
                System.Diagnostics.Debug.WriteLine($"Status from QueryString: {status}");

                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadBookingSummary();
                RegisterAsyncTask(new PageAsyncTask(LoadUserContactDetailsAsync));

                // Check if this is a postpone/modify booking
                if (!string.IsNullOrEmpty(pnrNumber))
                {
                    System.Diagnostics.Debug.WriteLine("=== POSTPONE MODE DETECTED ===");

                    // Store in session for use in LoadPassengerDetails and Payment page
                    Session["CurrentPnrNumber"] = pnrNumber;

                    // Make sure bookedId is stored properly
                    if (!string.IsNullOrEmpty(bookedId))
                    {
                        Session["CurrentBookedId"] = bookedId;
                        System.Diagnostics.Debug.WriteLine($"Stored CurrentBookedId: {bookedId}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("WARNING: BookedId is empty in query string!");
                    }

                    // Load existing passenger details
                    RegisterAsyncTask(new PageAsyncTask(() => LoadPassengerDetails(pnrNumber)));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("=== NEW BOOKING MODE ===");
                    // New booking: generate empty forms
                    GeneratePassengerForms();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("=== POSTBACK - Recreating Controls ===");

                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                GeneratePassengerForms();
            }
        }

        private async Task LoadUserContactDetailsAsync()
        {
            await LoadUserContactDetails();
        }

        private async Task LoadPassengerDetailsAsync()
        {
            // Retrieve the pnrNumber from session or query string, depending on your logic
            string pnrNumber = Session["pnrNumber"]?.ToString();

            // If pnrNumber is not found, handle this scenario (redirect, alert, etc.)
            if (string.IsNullOrEmpty(pnrNumber))
            {
                ShowAlert("PNR not found. Please go back and select seats again.");
                Response.Redirect("~/ticket.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // Now call the method to load passenger details
            await LoadPassengerDetails(pnrNumber);
        }

        private void LoadBookingSummary()
        {
            try
            {
                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string journeyDate = Session["JourneyDate"]?.ToString() ?? "";
                string pickupPoint = Session["PickupPoint"]?.ToString() ?? "";
                string droppingPoint = Session["DroppingPoint"]?.ToString() ?? "";
                decimal unitPrice = Convert.ToDecimal(Session["UnitPrice"] ?? "0");

                lblSelectedSeats.Text = selectedSeats.Replace(",", ", ");
                lblJourneyDate.Text = Convert.ToDateTime(journeyDate).ToString("dd MMM yyyy");
                lblRoute.Text = $"{pickupPoint} → {droppingPoint}";

                int seatCount = selectedSeats.Split(',').Length;
                decimal totalAmount = seatCount * unitPrice;
                lblTotalAmount.Text = $"CDF {totalAmount:N0}";

                ViewState["TotalAmount"] = totalAmount;
                ViewState["SeatCount"] = seatCount;
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading booking summary: " + ex.Message);
            }
        }

        private async Task LoadUserContactDetails()
        {
            try
            {
                string userId = Session["UserId"]?.ToString() ?? "";

                if (string.IsNullOrEmpty(userId))
                {
                    ShowAlert("User session not found. Please login again.");
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string apiEndpoint = $"{apiUrl}Users/GetUser/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading user details from: {apiEndpoint}");

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"User API Response: {jsonResponse}");

                    JObject userData = JObject.Parse(jsonResponse);

                    string userEmail = userData["email"]?.ToString() ?? "";
                    string userMobile = userData["mobile"]?.ToString() ?? "";

                    string userLocation = userData["address"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(userLocation))
                    {
                        List<string> locationParts = new List<string>();
                        string city = userData["city"]?.ToString() ?? "";
                        string state = userData["state"]?.ToString() ?? "";
                        string country = userData["countryName"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(city)) locationParts.Add(city);
                        if (!string.IsNullOrEmpty(state)) locationParts.Add(state);
                        if (!string.IsNullOrEmpty(country)) locationParts.Add(country);

                        userLocation = locationParts.Count > 0 ? string.Join(", ", locationParts) : "Not provided";
                    }

                    txtContactEmail.Text = userEmail;
                    txtContactMobile.Text = userMobile;
                    txtContactLocation.Text = userLocation;

                    Session["UserEmail"] = userEmail;
                    Session["UserMobile"] = userMobile;
                    Session["UserLocation"] = userLocation;

                    System.Diagnostics.Debug.WriteLine($"User details loaded - Email: {userEmail}, Mobile: {userMobile}");
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorMsg}");

                    string userEmail = Session["UserEmail"]?.ToString() ?? "";
                    string userMobile = Session["UserMobile"]?.ToString() ?? "";
                    string userLocation = Session["UserLocation"]?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        txtContactEmail.Text = userEmail;
                        txtContactMobile.Text = userMobile;
                        txtContactLocation.Text = userLocation;
                    }
                    else
                    {
                        ShowAlert("Failed to load user details. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading contact details: " + ex.Message);
                ShowAlert("Error loading contact details: " + ex.Message);
            }
        }

        private void GeneratePassengerForms()
        {
            try
            {
                pnlPassengers.Controls.Clear();

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string[] seats = selectedSeats.Split(',');

                System.Diagnostics.Debug.WriteLine($"Generating forms for {seats.Length} passengers");

                for (int i = 0; i < seats.Length; i++)
                {
                    Panel passengerPanel = new Panel();
                    passengerPanel.CssClass = "panel panel-default";
                    passengerPanel.Style.Add("margin-bottom", "20px");
                    passengerPanel.ID = $"pnlPassenger_{i}";

                    Panel headingPanel = new Panel();
                    headingPanel.CssClass = "panel-heading";

                    Literal headingLiteral = new Literal();
                    headingLiteral.Text = $"<h4 style='margin:0;'><i class='fa fa-user'></i> Passenger {i + 1} - Seat {seats[i].Trim()}</h4>";
                    headingPanel.Controls.Add(headingLiteral);
                    passengerPanel.Controls.Add(headingPanel);

                    Panel bodyPanel = new Panel();
                    bodyPanel.CssClass = "panel-body";
                    bodyPanel.ID = $"pnlBody_{i}";

                    Panel row = new Panel();
                    row.CssClass = "row";
                    row.ID = $"rowPassenger_{i}";

                    // Name field
                    Panel nameCol = new Panel();
                    nameCol.CssClass = "col-md-6";
                    Panel nameGroup = new Panel();
                    nameGroup.CssClass = "form-group";

                    Literal nameLabel = new Literal();
                    nameLabel.Text = "<label>Full Name <span style='color:red;'>*</span></label>";
                    nameGroup.Controls.Add(nameLabel);

                    TextBox nameTextBox = new TextBox();
                    nameTextBox.ID = $"txtName_{i}";
                    nameTextBox.CssClass = "form-control";
                    nameTextBox.Attributes.Add("placeholder", "Enter full name");
                    nameTextBox.Attributes.Add("required", "required");
                    nameTextBox.EnableViewState = true;
                    nameGroup.Controls.Add(nameTextBox);
                    nameCol.Controls.Add(nameGroup);
                    row.Controls.Add(nameCol);

                    // Age field
                    Panel ageCol = new Panel();
                    ageCol.CssClass = "col-md-3";
                    Panel ageGroup = new Panel();
                    ageGroup.CssClass = "form-group";

                    Literal ageLabel = new Literal();
                    ageLabel.Text = "<label>Age <span style='color:red;'>*</span></label>";
                    ageGroup.Controls.Add(ageLabel);

                    TextBox ageTextBox = new TextBox();
                    ageTextBox.ID = $"txtAge_{i}";
                    ageTextBox.CssClass = "form-control";
                    ageTextBox.TextMode = TextBoxMode.Number;
                    ageTextBox.Attributes.Add("placeholder", "Age");
                    ageTextBox.Attributes.Add("min", "1");
                    ageTextBox.Attributes.Add("max", "120");
                    ageTextBox.Attributes.Add("required", "required");
                    ageTextBox.EnableViewState = true;
                    ageGroup.Controls.Add(ageTextBox);
                    ageCol.Controls.Add(ageGroup);
                    row.Controls.Add(ageCol);

                    // Gender field
                    Panel genderCol = new Panel();
                    genderCol.CssClass = "col-md-3";
                    Panel genderGroup = new Panel();
                    genderGroup.CssClass = "form-group";

                    Literal genderLabel = new Literal();
                    genderLabel.Text = "<label>Gender <span style='color:red;'>*</span></label>";
                    genderGroup.Controls.Add(genderLabel);

                    DropDownList genderDropDown = new DropDownList();
                    genderDropDown.ID = $"ddlGender_{i}";
                    genderDropDown.CssClass = "form-control";
                    genderDropDown.Items.Add(new ListItem("Select", ""));
                    genderDropDown.Items.Add(new ListItem("Male", "Male"));
                    genderDropDown.Items.Add(new ListItem("Female", "Female"));
                    genderDropDown.Items.Add(new ListItem("Other", "Other"));
                    genderDropDown.Attributes.Add("required", "required");
                    genderDropDown.EnableViewState = true;
                    genderGroup.Controls.Add(genderDropDown);
                    genderCol.Controls.Add(genderGroup);
                    row.Controls.Add(genderCol);

                    bodyPanel.Controls.Add(row);
                    passengerPanel.Controls.Add(bodyPanel);
                    pnlPassengers.Controls.Add(passengerPanel);

                    System.Diagnostics.Debug.WriteLine($"Added controls for passenger {i}: txtName_{i}, txtAge_{i}, ddlGender_{i}");
                }

                // Store the number of passengers in ViewState
                ViewState["PassengerCount"] = seats.Length;
                ViewState["SeatNumbers"] = selectedSeats;

                System.Diagnostics.Debug.WriteLine($"Total controls in pnlPassengers: {pnlPassengers.Controls.Count}");
            }
            catch (Exception ex)
            {
                ShowAlert("Error generating passenger forms: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("GeneratePassengerForms Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }

        protected async void btnConfirmBooking_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CONFIRM BOOKING CLICKED ===");

                string contactEmail = txtContactEmail.Text.Trim();
                string contactMobile = txtContactMobile.Text.Trim();
                string contactLocation = txtContactLocation.Text.Trim();

                System.Diagnostics.Debug.WriteLine($"Contact Email: {contactEmail}");
                System.Diagnostics.Debug.WriteLine($"Contact Mobile: {contactMobile}");

                List<PassengerInfo> passengers = new List<PassengerInfo>();

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string[] seats = selectedSeats.Split(',');
                string tripId = Session["TripId"]?.ToString() ?? "";

                System.Diagnostics.Debug.WriteLine($"Selected Seats: {selectedSeats}");
                System.Diagnostics.Debug.WriteLine($"Trip ID: {tripId}");
                System.Diagnostics.Debug.WriteLine($"Total seats to process: {seats.Length}");
                System.Diagnostics.Debug.WriteLine($"pnlPassengers has {pnlPassengers.Controls.Count} controls");

                for (int i = 0; i < seats.Length; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"--- Processing passenger {i + 1} ---");

                    string name = "";
                    string ageStr = "";
                    string gender = "";

                    // Method 1: Try FindControl recursively
                    TextBox txtName = FindControlRecursive(pnlPassengers, $"txtName_{i}") as TextBox;
                    TextBox txtAge = FindControlRecursive(pnlPassengers, $"txtAge_{i}") as TextBox;
                    DropDownList ddlGender = FindControlRecursive(pnlPassengers, $"ddlGender_{i}") as DropDownList;

                    System.Diagnostics.Debug.WriteLine($"txtName found: {txtName != null}");
                    System.Diagnostics.Debug.WriteLine($"txtAge found: {txtAge != null}");
                    System.Diagnostics.Debug.WriteLine($"ddlGender found: {ddlGender != null}");

                    if (txtName != null && txtAge != null && ddlGender != null)
                    {
                        name = txtName.Text.Trim();
                        ageStr = txtAge.Text.Trim();
                        gender = ddlGender.SelectedValue;

                        System.Diagnostics.Debug.WriteLine($"Method 1 (Controls) - Name: {name}, Age: {ageStr}, Gender: {gender}");
                    }
                    else
                    {
                        // Method 2: Fallback to Request.Form
                        System.Diagnostics.Debug.WriteLine("Trying Request.Form fallback...");

                        // Get the actual rendered control IDs
                        foreach (string key in Request.Form.AllKeys)
                        {
                            if (key != null && key.Contains($"txtName_{i}"))
                            {
                                name = Request.Form[key];
                                System.Diagnostics.Debug.WriteLine($"Found name in Request.Form[{key}]: {name}");
                            }
                            if (key != null && key.Contains($"txtAge_{i}"))
                            {
                                ageStr = Request.Form[key];
                                System.Diagnostics.Debug.WriteLine($"Found age in Request.Form[{key}]: {ageStr}");
                            }
                            if (key != null && key.Contains($"ddlGender_{i}"))
                            {
                                gender = Request.Form[key];
                                System.Diagnostics.Debug.WriteLine($"Found gender in Request.Form[{key}]: {gender}");
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ageStr) || string.IsNullOrEmpty(gender))
                    {
                        System.Diagnostics.Debug.WriteLine($"Validation failed - Name: '{name}', Age: '{ageStr}', Gender: '{gender}'");
                        ShowAlert($"Please fill all required details for Passenger {i + 1}");
                        return;
                    }

                    if (!int.TryParse(ageStr, out int age) || age < 1 || age > 120)
                    {
                        ShowAlert($"Please enter valid age (1-120) for Passenger {i + 1}");
                        return;
                    }

                    passengers.Add(new PassengerInfo
                    {
                        Name = name,
                        Age = age,
                        Gender = gender,
                        SeatNumber = seats[i].Trim(),
                        BagsCount = 0,
                        BagsWeight = "0",
                        BagsCharge = 0
                    });

                    System.Diagnostics.Debug.WriteLine($"Added passenger: {name}, {age}, {gender}");
                }

                System.Diagnostics.Debug.WriteLine($"Total passengers collected: {passengers.Count}");

                if (passengers.Count == 0)
                {
                    ShowAlert("No passenger data collected. Please refresh the page and try again.");
                    return;
                }

                string status = Request.QueryString["status"] ?? "";
                if (status == "Pending" || string.IsNullOrEmpty(status))
                {
                    btnConfirmBooking.Enabled = false;
                    btnConfirmBooking.Text = "Processing...";

                    await CreateBooking(passengers);
                }
                //
                else if (status == "Booked")
                {
                    System.Diagnostics.Debug.WriteLine("=== POSTPONE FLOW - Redirecting to Payment ===");

                    string pnrNumber = Request.QueryString["pnrNumber"] ?? Session["PNRNumber"]?.ToString() ?? "";
                    string bookedIdStr = Request.QueryString["bookedId"] ?? Session["BookedId"]?.ToString() ?? "";

                    if (string.IsNullOrEmpty(bookedIdStr))
                    {
                        bookedIdStr = Session["CurrentBookedId"]?.ToString() ?? "";
                        System.Diagnostics.Debug.WriteLine($"BookedId from CurrentBookedId session: {bookedIdStr}");
                    }

                    if (string.IsNullOrEmpty(bookedIdStr))
                    {
                        System.Diagnostics.Debug.WriteLine("WARNING: BookedId is empty! This might cause issues.");
                        bookedIdStr = "0"; 
                    }

                    decimal unitPrice = Convert.ToDecimal(Session["UnitPrice"] ?? "0");
                    int seatCount = selectedSeats.Split(',').Length;
                    decimal totalAmount = seatCount * unitPrice;

                    Session["PNRNumber"] = pnrNumber;
                    Session["BookedId"] = bookedIdStr;
                    Session["SubTotal"] = totalAmount;
                    Session["status"] = status;

                    System.Diagnostics.Debug.WriteLine($"Postpone - PNR: {pnrNumber}, BookedId: {bookedIdStr}, Amount: {totalAmount}");

                    // Redirect to payment page with booking details
                    Response.Redirect($"~/payment.aspx?bookedId={bookedIdStr}&pnr={pnrNumber}&amount={totalAmount}&status=Booked", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // Other status - show error
                    ShowAlert($"Invalid booking status: {status}");
                }
            }
            catch (Exception ex)
            {
                btnConfirmBooking.Enabled = true;
                btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
                System.Diagnostics.Debug.WriteLine("Error in btnConfirmBooking_Click: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private async Task CreateBooking(List<PassengerInfo> passengers)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int tripId = Convert.ToInt32(Session["TripId"]);
                string pickupPoint = Session["PickupPoint"]?.ToString() ?? "";
                string droppingPoint = Session["DroppingPoint"]?.ToString() ?? "";
                string journeyDate = Session["JourneyDate"]?.ToString() ?? "";
                decimal unitPrice = Convert.ToDecimal(Session["UnitPrice"] ?? "0");
                string status = Request.QueryString["status"] ?? Session["tripStatus"]?.ToString();

                var bookingData = new
                {
                    userId = userId,
                    tripId = tripId,
                    sourceDestination = $"{pickupPoint}-{droppingPoint}",
                    unitPrice = unitPrice,
                    dateOfJourney = journeyDate,
                    passengers = passengers,
                    status = status
                };

                string jsonContent = JsonConvert.SerializeObject(bookingData);
                System.Diagnostics.Debug.WriteLine("=== BOOKING REQUEST ===");
                System.Diagnostics.Debug.WriteLine(jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl + "BookedTicketsNew/PostBookedTicket", content);
                string responseJson = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("=== BOOKING RESPONSE ===");
                System.Diagnostics.Debug.WriteLine($"Status Code: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine(responseJson);

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseJson);
                    bool success = result["success"]?.Value<bool>() ?? false;

                    if (success && result["data"] != null)
                    {
                        JObject data = result["data"] as JObject;
                        int bookedId = data["bookedId"]?.Value<int>() ?? 0;
                        decimal subTotal = data["subTotal"]?.Value<decimal>() ?? 0;
                        string pnrNumber = data["pnrNumber"]?.ToString() ?? "";

                        // Store booking details in session
                        Session["BookedId"] = bookedId;
                        Session["PNRNumber"] = pnrNumber;
                        Session["SubTotal"] = subTotal;
                        Session["status"] = status;

                        System.Diagnostics.Debug.WriteLine($"Booking successful - BookedId: {bookedId}, PNR: {pnrNumber}");
                        System.Diagnostics.Debug.WriteLine($"Session values: BookedId={Session["BookedId"]}, PNR={Session["PNRNumber"]}, SubTotal={Session["SubTotal"]}");

                        // Redirect to payment page with query parameters - FIXED
                        Response.Redirect($"~/Payment.aspx?bookedId={bookedId}&pnr={pnrNumber}&amount={subTotal}", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        string message = result["message"]?.ToString() ?? "Booking failed";
                        ShowAlert(message);
                        btnConfirmBooking.Enabled = true;
                        btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
                    }
                }
                else
                {
                    ShowAlert("Booking failed. Please try again.");
                    btnConfirmBooking.Enabled = true;
                    btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Booking Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowAlert("Booking error: " + ex.Message);
                btnConfirmBooking.Enabled = true;
                btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control control in root.Controls)
            {
                Control found = FindControlRecursive(control, id);
                if (found != null)
                    return found;
            }

            return null;
        }

        private async Task ProcessPayment(int userId, int bookedId, decimal amount)
        {
            try
            {
                var paymentData = new
                {
                    userId = userId,
                    amount = amount,
                    bookedId = bookedId
                };

                string jsonContent = JsonConvert.SerializeObject(paymentData);
                System.Diagnostics.Debug.WriteLine("=== PAYMENT REQUEST ===");
                System.Diagnostics.Debug.WriteLine(jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl + "TransactionsNew/PostTransaction", content);
                string responseJson = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("=== PAYMENT RESPONSE ===");
                System.Diagnostics.Debug.WriteLine($"Status Code: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine(responseJson);

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseJson);
                    bool success = result["success"]?.Value<bool>() ?? false;

                    if (success && result["data"] != null)
                    {
                        JObject data = result["data"] as JObject;
                        string transactionNumber = data["transactionNumber"]?.ToString() ?? "";
                        string pnrNumber = data["pnrNumber"]?.ToString() ?? "";

                        System.Diagnostics.Debug.WriteLine($"Payment successful - TXN: {transactionNumber}, PNR: {pnrNumber}");

                        ClearBookingSession();

                        Response.Redirect($"~/BookingConfirmation.aspx?pnr={pnrNumber}&txn={transactionNumber}", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        string message = result["message"]?.ToString() ?? "Payment failed";
                        ShowAlert(message);
                        btnConfirmBooking.Enabled = true;
                        btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
                    }
                }
                else
                {
                    ShowAlert("Payment failed. Please contact support.");
                    btnConfirmBooking.Enabled = true;
                    btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Payment Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowAlert("Payment error: " + ex.Message);
                btnConfirmBooking.Enabled = true;
                btnConfirmBooking.Text = "Confirm Booking & Proceed to Payment";
            }
        }

        private void ClearBookingSession()
        {
            Session.Remove("SelectedSeats");
            Session.Remove("JourneyDate");
            Session.Remove("PickupPoint");
            Session.Remove("DroppingPoint");
            Session.Remove("TripId");
            Session.Remove("UnitPrice");
            Session.Remove("BookedId");
            Session.Remove("PNRNumber");
            Session.Remove("SubTotal");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        public class PassengerInfo
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("mobileNumber")]
            public string MobileNumber { get; set; }

            [JsonProperty("emailId")]
            public string EmailId { get; set; }

            [JsonProperty("age")]
            public int Age { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("seatNumber")]
            public string SeatNumber { get; set; }

            [JsonProperty("bagsCount")]
            public int BagsCount { get; set; }

            [JsonProperty("bagsWeight")]
            public string BagsWeight { get; set; }

            [JsonProperty("bagsCharge")]
            public decimal BagsCharge { get; set; }
        }

        private async Task LoadPassengerDetails(string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}PassengerTables/GetPassengersByPnr?pnr={pnrNumber}";
                System.Diagnostics.Debug.WriteLine($"Loading passenger details from: {apiEndpoint}");

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Passenger API Response: {jsonResponse}");

                    JArray passengersData = JArray.Parse(jsonResponse);

                    if (!passengersData.Any())
                    {
                        ShowAlert("No passengers found for this PNR. Please proceed to enter details manually.");
                        GeneratePassengerForms(); 
                        return;
                    }

                    GeneratePassengerForms();

                    for (int i = 0; i < passengersData.Count; i++)
                    {
                        JToken passenger = passengersData[i];
                        string name = passenger["name"]?.ToString() ?? "";
                        string age = passenger["age"]?.ToString() ?? "";
                        string gender = passenger["gender"]?.ToString() ?? "";

                        TextBox txtName = FindControlRecursive(pnlPassengers, $"txtName_{i}") as TextBox;
                        TextBox txtAge = FindControlRecursive(pnlPassengers, $"txtAge_{i}") as TextBox;
                        DropDownList ddlGender = FindControlRecursive(pnlPassengers, $"ddlGender_{i}") as DropDownList;

                        if (txtName != null) txtName.Text = name;
                        if (txtAge != null) txtAge.Text = age;
                        if (ddlGender != null && !string.IsNullOrEmpty(gender))
                        {
                            ddlGender.SelectedValue = gender;
                        }

                        System.Diagnostics.Debug.WriteLine($"Bound passenger {i + 1}: {name}, {age}, {gender}");
                    }

                    Session["OriginalPassengerDetails"] = passengersData;
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorMsg}");
                    ShowAlert("Failed to load passenger details. Please enter details manually.");
                    GeneratePassengerForms(); 
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading passenger details: " + ex.Message);
                ShowAlert("Error loading passenger details: " + ex.Message);
                GeneratePassengerForms(); 
            }
        }
    }
}