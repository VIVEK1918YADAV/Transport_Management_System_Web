using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class payment : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Diagnostics.Debug.WriteLine("=== PAYMENT PAGE LOAD ===");

                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("UserId is NULL - redirecting to ticket.aspx");
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Try to get booking details from query string first (for fresh redirects from booking)
                string bookedIdStr = Request.QueryString["bookedId"];
                string pnrFromQuery = Request.QueryString["pnr"];
                string amountStr = Request.QueryString["amount"];
                string statusFromQuery = Request.QueryString["status"];

                System.Diagnostics.Debug.WriteLine($"QueryString - bookedId: {bookedIdStr}, pnr: {pnrFromQuery}, amount: {amountStr}, status: {statusFromQuery}");

                // If we have query string parameters, update session with them
                if (!string.IsNullOrEmpty(pnrFromQuery))
                {
                    // Check if bookedId is 0 or empty, try to get from CurrentBookedId session
                    if (string.IsNullOrEmpty(bookedIdStr) || bookedIdStr == "0")
                    {
                        string currentBookedId = Session["CurrentBookedId"]?.ToString();
                        if (!string.IsNullOrEmpty(currentBookedId) && currentBookedId != "0")
                        {
                            bookedIdStr = currentBookedId;
                            System.Diagnostics.Debug.WriteLine($"Using CurrentBookedId from session: {bookedIdStr}");
                        }
                    }

                    Session["BookedId"] = bookedIdStr;
                    Session["PNRNumber"] = pnrFromQuery;
                    if (!string.IsNullOrEmpty(amountStr))
                    {
                        Session["SubTotal"] = amountStr;
                    }
                    if (!string.IsNullOrEmpty(statusFromQuery))
                    {
                        Session["status"] = statusFromQuery;
                    }
                    System.Diagnostics.Debug.WriteLine("Session updated from QueryString");
                }

                System.Diagnostics.Debug.WriteLine($"Session - BookedId: {Session["BookedId"]}, PNR: {Session["PNRNumber"]}, SubTotal: {Session["SubTotal"]}, Status: {Session["status"]}");

                // Load booking details from session
                LoadBookingDetails();
            }
        }

        private void LoadBookingDetails()
        {
            try
            {
                // Retrieve booking details from session
                string bookedIdStr = Session["BookedId"]?.ToString() ?? "0";
                int bookedId = 0;

                // Try to parse bookedId safely
                if (!int.TryParse(bookedIdStr, out bookedId))
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Invalid BookedId format: '{bookedIdStr}'");
                    bookedId = 0;
                }

                string pnrNumber = Session["PNRNumber"]?.ToString() ?? "";
                string subTotalStr = Session["SubTotal"]?.ToString() ?? "0";
                decimal subTotal = 0;

                // Try to parse subTotal safely
                if (!decimal.TryParse(subTotalStr, out subTotal))
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Invalid SubTotal format: '{subTotalStr}'");
                    subTotal = 0;
                }

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string journeyDate = Session["JourneyDate"]?.ToString() ?? "";
                string pickupPoint = Session["PickupPoint"]?.ToString() ?? "";
                string droppingPoint = Session["DroppingPoint"]?.ToString() ?? "";
                string status = Session["status"]?.ToString() ?? "";


                System.Diagnostics.Debug.WriteLine($"LoadBookingDetails - bookedId: {bookedId}, pnr: {pnrNumber}, amount: {subTotal}, status: {status}");

                // For postpone flow, PNR is more important than bookedId
                // Check if we at least have PNR
                if (string.IsNullOrEmpty(pnrNumber))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: PNR is empty!");
                    ShowAlert("Booking details not found. Please try again.");
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // If this is a postpone (status=Booked) and bookedId is 0, it's okay - we'll fetch it via async call
                if (bookedId == 0 && status != "Booked")
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: BookedId is 0 for non-postpone booking!");
                    ShowAlert("Booking details not found. Please try again.");
                    Response.Redirect("~/ticket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Display the booking details on the UI
                lblPNRNumber.Text = pnrNumber;
                lblBookedId.Text = bookedId > 0 ? bookedId.ToString() : "Pending";
                lblSelectedSeats.Text = selectedSeats.Replace(",", ", ");

                if (!string.IsNullOrEmpty(journeyDate))
                {
                    try
                    {
                        lblJourneyDate.Text = Convert.ToDateTime(journeyDate).ToString("dd MMM yyyy");
                    }
                    catch
                    {
                        lblJourneyDate.Text = journeyDate;
                    }
                }

                lblRoute.Text = $"{pickupPoint} → {droppingPoint}";
                lblAmount.Text = $"CDF {subTotal:N0}";
                lblTotalAmount.Text = $"CDF {subTotal:N0}";

                // Store values for payment processing
                ViewState["BookedId"] = bookedId;
                ViewState["SubTotal"] = subTotal;
                ViewState["PNRNumber"] = pnrNumber;

                // Log the details for debugging
                System.Diagnostics.Debug.WriteLine($"Payment page loaded successfully - BookedId: {bookedId}, PNR: {pnrNumber}, Amount: {subTotal}");
            }
            catch (Exception ex)
            {
                // Log and display error in case of any issues
                System.Diagnostics.Debug.WriteLine("Error loading booking details: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);

                // Don't redirect immediately - show error on same page
                ShowAlert($"Error loading booking details: {ex.Message}. Please contact support.");
            }
        }

        protected async void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                btnConfirmPayment.Enabled = false;
                btnConfirmPayment.Text = "Processing Payment...";

                int userId = Convert.ToInt32(Session["UserId"]);
                int bookedId = Convert.ToInt32(ViewState["BookedId"]);
                decimal amount = Convert.ToDecimal(ViewState["SubTotal"]);

                string status = Session["status"]?.ToString();
                string pnrNumber = Session["PNRNumber"]?.ToString();

                string journeyDate = Session["JourneyDate"]?.ToString();
                DateTime? newDateOfJourney = null;

                if (!string.IsNullOrEmpty(journeyDate))
                {
                    newDateOfJourney = DateTime.Parse(journeyDate);
                }

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string[] newSeats = selectedSeats.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(s => s.Trim())
                                                   .ToArray();

                System.Diagnostics.Debug.WriteLine($"=== PAYMENT CONFIRMATION DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Status: {status}");
                System.Diagnostics.Debug.WriteLine($"PNR: {pnrNumber}");
                System.Diagnostics.Debug.WriteLine($"New Seats: {string.Join(", ", newSeats)}");

                List<SeatUpdate> updatedSeatNumbers = null;

                // ✅ CRITICAL FIX: For postpone, get actual passenger IDs from API
                if (status == "Booked" && !string.IsNullOrEmpty(pnrNumber))
                {
                    System.Diagnostics.Debug.WriteLine("Fetching actual passenger IDs from API...");
                    updatedSeatNumbers = await GetPassengerSeatUpdates(pnrNumber, newSeats);

                    if (updatedSeatNumbers == null || updatedSeatNumbers.Count == 0)
                    {
                        throw new Exception("Failed to fetch passenger IDs. Cannot process postponement.");
                    }

                    System.Diagnostics.Debug.WriteLine($"Got {updatedSeatNumbers.Count} passenger IDs:");
                    foreach (var update in updatedSeatNumbers)
                    {
                        System.Diagnostics.Debug.WriteLine($"  PassengerId: {update.passengerId}, NewSeat: {update.newSeatNumber}");
                    }
                }

                // Call the payment processing method
                await ProcessPayment(userId, bookedId, amount, status, pnrNumber, newDateOfJourney, updatedSeatNumbers);
            }
            catch (Exception ex)
            {
                btnConfirmPayment.Enabled = true;
                btnConfirmPayment.Text = "Confirm Payment";
                System.Diagnostics.Debug.WriteLine("Error in btnConfirmPayment_Click: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private async Task<List<SeatUpdate>> GetPassengerSeatUpdates(string pnrNumber, string[] newSeats)
        {
            try
            {
                string endpoint = $"{apiUrl}BookedTicketsNew/GetBookedTicketByPnr?pnr={pnrNumber}";
                System.Diagnostics.Debug.WriteLine($"Fetching passenger details from: {endpoint}");

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Response: {jsonResponse}");

                JObject bookingData = JObject.Parse(jsonResponse);
                JArray passengers = bookingData["passengers"] as JArray;

                if (passengers == null || passengers.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No passengers found in booking");
                    return null;
                }

                var activePassengers = passengers.Where(p => p["isActive"]?.Value<bool>() == true).ToList();

                if (activePassengers.Count != newSeats.Length)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Passenger count mismatch! Active: {activePassengers.Count}, New Seats: {newSeats.Length}");
                }

                var seatUpdates = new List<SeatUpdate>();

                for (int i = 0; i < Math.Min(activePassengers.Count, newSeats.Length); i++)
                {
                    int passengerId = activePassengers[i]["passengerId"]?.Value<int>() ?? 0;

                    if (passengerId > 0)
                    {
                        seatUpdates.Add(new SeatUpdate
                        {
                            passengerId = passengerId,
                            newSeatNumber = newSeats[i]
                        });
                    }
                }

                return seatUpdates;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching passenger IDs: {ex.Message}");
                return null;
            }
        }

        private async Task ProcessPayment(int userId, int bookedId, decimal amount, string status, string pnrNumber = "", DateTime? newDateOfJourney = null, List<SeatUpdate> updatedSeatNumbers = null)
        {
            try
            {
                string newDateOfJourneyString = newDateOfJourney?.ToString("yyyy-MM-dd");

                var paymentData = new
                {
                    userId = userId,
                    amount = amount,
                    bookedId = bookedId,
                    pnrNumber = pnrNumber,
                    newDateOfJourney = newDateOfJourneyString,
                    updatedSeatNumbers = updatedSeatNumbers
                };

                string jsonContent = JsonConvert.SerializeObject(paymentData);
                System.Diagnostics.Debug.WriteLine("=== PAYMENT REQUEST ===");
                System.Diagnostics.Debug.WriteLine(jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string apiEndpoint = string.Empty;
                HttpMethod method;

                if (status == "Booked" || status == "Postponed")
                {
                    apiEndpoint = apiUrl + "BookedTicketsNew/PostponeBooking";
                    method = HttpMethod.Post;
                }

                else
                {
                    // Use POST for normal payment
                    apiEndpoint = apiUrl + "TransactionsNew/PostTransaction";
                    method = HttpMethod.Post;
                }

                // Make the API request
                HttpRequestMessage request = new HttpRequestMessage(method, apiEndpoint) { Content = content };
                HttpResponseMessage response = await client.SendAsync(request);

                // Log the response content for debugging
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
                        string pnrNumberResponse = data["pnrNumber"]?.ToString() ?? "";

                        System.Diagnostics.Debug.WriteLine($"Payment successful - TXN: {transactionNumber}, PNR: {pnrNumberResponse}");

                        ClearBookingSession();

                        Response.Redirect($"~/BookingConfirmation.aspx?pnr={pnrNumberResponse}&txn={transactionNumber}", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        string message = result["message"]?.ToString() ?? "Payment failed";
                        ShowAlert(message);
                        btnConfirmPayment.Enabled = true;
                        btnConfirmPayment.Text = "Confirm Payment";
                    }
                }
                else
                {
                    ShowAlert("Payment failed. Please contact support. " + responseJson);
                    btnConfirmPayment.Enabled = true;
                    btnConfirmPayment.Text = "Confirm Payment";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Payment Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowAlert("Payment error: " + ex.Message);
                btnConfirmPayment.Enabled = true;
                btnConfirmPayment.Text = "Confirm Payment";
            }
        }

        public class SeatUpdate
        {
            public int passengerId { get; set; }
            public string newSeatNumber { get; set; }
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
    }
}