using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Train_Payment : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();

        string apiUrl = ConfigurationManager.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Diagnostics.Debug.WriteLine("=== TRAIN PAYMENT PAGE LOAD ===");

                if (Session["userId"] == null && Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ UserId is NULL - redirecting to TrainTicket.aspx");
                    Response.Redirect("~/TrainTicket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                string selectedSeat = Request.QueryString["selectedSeats"] ?? "0";
                string bookingIdRaw = Request.QueryString["bookedId"] ?? "";
                string pnrFromQuery = Request.QueryString["pnr"] ?? "";
                string amountStr = Request.QueryString["amount"] ?? "";
                string routeFromQuery = Request.QueryString["route"] ?? "";
                string postponeCountStr = Request.QueryString["postponeCount"] ?? "0";

                System.Diagnostics.Debug.WriteLine($"QS bookedId     : {bookingIdRaw}");
                System.Diagnostics.Debug.WriteLine($"QS pnr          : {pnrFromQuery}");
                System.Diagnostics.Debug.WriteLine($"QS amount       : {amountStr}");
                System.Diagnostics.Debug.WriteLine($"QS postponeCount: {postponeCountStr}");

                decimal amountFromQuery = 0;
                if (!string.IsNullOrEmpty(amountStr))
                    decimal.TryParse(amountStr, out amountFromQuery);

                int bookedIdInt = 0;
                if (!string.IsNullOrEmpty(bookingIdRaw))
                    int.TryParse(bookingIdRaw, out bookedIdInt);

                if (bookedIdInt == 0)
                {
                    string currentBookedId = Session["CurrentBookedId"]?.ToString();
                    if (!string.IsNullOrEmpty(currentBookedId) && currentBookedId != "0")
                    {
                        int.TryParse(currentBookedId, out bookedIdInt);
                        System.Diagnostics.Debug.WriteLine($"Using CurrentBookedId from session: {bookedIdInt}");
                    }

                    if (bookedIdInt == 0)
                    {
                        string sessionBookedId = Session["BookedId"]?.ToString();
                        if (!string.IsNullOrEmpty(sessionBookedId))
                        {
                            int.TryParse(sessionBookedId, out bookedIdInt);
                            System.Diagnostics.Debug.WriteLine($"Using BookedId from session: {bookedIdInt}");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(pnrFromQuery))
                    Session["PNRNumber"] = pnrFromQuery;

                Session["BookedId"] = bookedIdInt;

                if (amountFromQuery > 0)
                {
                    Session["SubTotal"] = amountFromQuery;
                    System.Diagnostics.Debug.WriteLine($"✓ SubTotal set from QS: {amountFromQuery}");
                }

                // NOTE: Status is intentionally NOT read from QueryString here.
                // It will be fetched fresh from API inside LoadBookingDetailsAsync().

                if (!string.IsNullOrEmpty(routeFromQuery))
                    Session["Route"] = Uri.UnescapeDataString(routeFromQuery);

                Session["PostponeCount"] = postponeCountStr;
                System.Diagnostics.Debug.WriteLine($"✓ PostponeCount stored: {postponeCountStr}");

                // FIX: RegisterAsyncTask is the correct way to run async on Page_Load
                RegisterAsyncTask(new PageAsyncTask(LoadBookingDetailsAsync));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // LOAD BOOKING DETAILS  (async — status comes ONLY from API)
        // ─────────────────────────────────────────────────────────────────────────

        private async Task LoadBookingDetailsAsync()
        {
            try
            {
                string bookedIdStr = Session["BookedId"]?.ToString() ?? "0";
                string pnrNumber = Session["PNRNumber"]?.ToString() ?? "";
                string subTotalStr = Session["SubTotal"]?.ToString() ?? "0";

                int.TryParse(bookedIdStr, out int bookedId);
                decimal.TryParse(subTotalStr, out decimal subTotal);

                // ✅ FIX: Status fetched from API — QueryString value is ignored
                string status = await GetStatusFromApi(pnrNumber);
                Session["status"] = status;
                System.Diagnostics.Debug.WriteLine($"✓ Status fetched from API: '{status}'");

                // Recalculate subTotal if still 0
                if (subTotal == 0)
                {
                    string seatsTemp = Session["SelectedSeats"]?.ToString() ?? "";
                    decimal.TryParse(Session["UnitPrice"]?.ToString() ?? "0", out decimal priceTemp);

                    if (!string.IsNullOrEmpty(seatsTemp) && priceTemp > 0)
                    {
                        int cnt = seatsTemp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        subTotal = cnt * priceTemp;
                        Session["SubTotal"] = subTotal;
                        System.Diagnostics.Debug.WriteLine($"✓ SubTotal recalculated: {subTotal}");
                    }
                }

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string journeyDate = Session["JourneyDate"]?.ToString() ?? "";
                string fromStation = Session["FromStation"]?.ToString() ?? "";
                string toStation = Session["ToStation"]?.ToString() ?? "";

                System.Diagnostics.Debug.WriteLine($"LoadBookingDetails - bookedId:{bookedId}, pnr:{pnrNumber}, amount:{subTotal}, status:{status}");

                if (string.IsNullOrEmpty(pnrNumber))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: PNR is empty!");
                    ShowAlert("Booking details not found. Please try again.");
                    Response.Redirect("~/TrainTicket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // For new bookings (non-postpone / non-booked), bookedId must be valid
                if (bookedId == 0 && !status.Equals("booked", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: BookedId is 0 for non-postpone booking!");
                    ShowAlert("Booking details not found. Please try again.");
                    Response.Redirect("~/TrainTicket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // ── Populate UI ──────────────────────────────────────────────
                lblPNRNumber.Text = pnrNumber;
                lblBookedId.Text = bookedId > 0 ? bookedId.ToString() : "Pending";
                lblSelectedSeats.Text = selectedSeats.Replace(",", ", ");

                if (!string.IsNullOrEmpty(journeyDate))
                {
                    try { lblJourneyDate.Text = Convert.ToDateTime(journeyDate).ToString("dd MMM yyyy"); }
                    catch { lblJourneyDate.Text = journeyDate; }
                }

                string route = Session["Route"]?.ToString();
                if (string.IsNullOrEmpty(route))
                    route = $"{fromStation} → {toStation}";
                lblRoute.Text = route;

                lblAmount.Text = $"CDF {subTotal:N0}";
                lblTotalAmount.Text = $"CDF {subTotal:N0}";

                ViewState["BookedId"] = bookedId;
                ViewState["SubTotal"] = subTotal;
                ViewState["PNRNumber"] = pnrNumber;

                System.Diagnostics.Debug.WriteLine($"✓ Payment page loaded - BookedId:{bookedId}, PNR:{pnrNumber}, Amount:CDF {subTotal:N0}, Status:{status}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading booking details: {ex.Message}");
                ShowAlert($"Error loading booking details: {ex.Message}. Please contact support.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GET STATUS FROM API
        // ─────────────────────────────────────────────────────────────────────────

        private async Task<string> GetStatusFromApi(string pnrNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(pnrNumber))
                    return "";

                string endpoint = $"{apiUrl}TrainTicketBookings/GetBookedTrainTicketByPnr?pnr={pnrNumber}";
                System.Diagnostics.Debug.WriteLine($"Fetching status from: {endpoint}");

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    return "";
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status API Response: {jsonResponse}");

                JObject bookingData = JObject.Parse(jsonResponse);

                // ✅ Try both common field names
                string status = bookingData["status"]?.ToString()
                             ?? bookingData["bookingStatus"]?.ToString()
                             ?? "";

                return status;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching status: {ex.Message}");
                return "";
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // CONFIRM PAYMENT BUTTON CLICK
        // ─────────────────────────────────────────────────────────────────────────

        protected async void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                 btnConfirmPayment.Enabled = false;
                btnConfirmPayment.Text = "Processing Payment...";

                string userId = (Session["userId"] ?? Session["UserId"])?.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    ShowAlert("Session expired. Please log in again.");
                    Response.Redirect("~/TrainTicket.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!int.TryParse(ViewState["BookedId"]?.ToString(), out int bookedId))
                {
                    ShowAlert("Invalid booking ID. Please try again.");
                    btnConfirmPayment.Enabled = true;
                    btnConfirmPayment.Text = "Confirm Payment";
                    return;
                }

                if (!decimal.TryParse(ViewState["SubTotal"]?.ToString(), out decimal amount))
                {
                    ShowAlert("Invalid amount. Please try again.");
                    btnConfirmPayment.Enabled = true;
                    btnConfirmPayment.Text = "Confirm Payment";
                    return;
                }

                // ✅ FIX: Status always from Session (already set from API in LoadBookingDetailsAsync)
                string status = Session["status"]?.ToString() ?? "";
                string pnrNumber = Session["PNRNumber"]?.ToString() ?? "";
                string journeyDate = Session["JourneyDate"]?.ToString() ?? "";

                DateTime? newDateOfJourney = null;
                if (!string.IsNullOrEmpty(journeyDate) &&
                    DateTime.TryParse(journeyDate, out DateTime parsedDate))
                    newDateOfJourney = parsedDate;

                string selectedSeats = Session["SelectedSeats"]?.ToString() ?? "";
                string[] newSeats = selectedSeats
                                         .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Trim())
                                         .ToArray();

                System.Diagnostics.Debug.WriteLine($"=== PAYMENT CONFIRMATION ===");
                System.Diagnostics.Debug.WriteLine($"UserId    : {userId}");
                System.Diagnostics.Debug.WriteLine($"BookedId  : {bookedId}");
                System.Diagnostics.Debug.WriteLine($"Amount    : {amount}");
                System.Diagnostics.Debug.WriteLine($"Status    : {status}");
                System.Diagnostics.Debug.WriteLine($"PNR       : {pnrNumber}");
                System.Diagnostics.Debug.WriteLine($"New Seats : {string.Join(", ", newSeats)}");

                List<SeatUpdate> updatedSeatNumbers = null;

                if (status.Equals("booked", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(pnrNumber))
                {
                    System.Diagnostics.Debug.WriteLine("Fetching actual passenger IDs from API...");
                    updatedSeatNumbers = await GetPassengerSeatUpdates(pnrNumber, newSeats);

                    if (updatedSeatNumbers == null || updatedSeatNumbers.Count == 0)
                        throw new Exception("Failed to fetch passenger IDs. Cannot process postponement.");

                    System.Diagnostics.Debug.WriteLine($"Got {updatedSeatNumbers.Count} passenger seat update(s):");
                    foreach (var u in updatedSeatNumbers)
                        System.Diagnostics.Debug.WriteLine($"  PassengerId:{u.PassengerId}, NewSeat:{u.SeatNumber}");
                }

                await ProcessPayment(userId, bookedId, amount, status, pnrNumber, newDateOfJourney, updatedSeatNumbers);
            }
            catch (Exception ex)
            {
                btnConfirmPayment.Enabled = true;
                btnConfirmPayment.Text = "Confirm Payment";
                System.Diagnostics.Debug.WriteLine($"Error in btnConfirmPayment_Click: {ex.Message}");
                ShowAlert("Error: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GET PASSENGER SEAT UPDATES
        // ─────────────────────────────────────────────────────────────────────────

        private async Task<List<SeatUpdate>> GetPassengerSeatUpdates(string pnrNumber, string[] newSeats)
        {
            try
            {
                string endpoint = $"{apiUrl}TrainTicketBookings/GetBookedTrainTicketByPnr?pnr={pnrNumber}";
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

                var activePassengers = passengers
                                         .Where(p => p["isActive"]?.Value<bool>() == true)
                                         .ToList();

                if (activePassengers.Count != newSeats.Length)
                    System.Diagnostics.Debug.WriteLine(
                        $"WARNING: Count mismatch! Active:{activePassengers.Count}, NewSeats:{newSeats.Length}");

                var seatUpdates = new List<SeatUpdate>();

                for (int i = 0; i < Math.Min(activePassengers.Count, newSeats.Length); i++)
                {
                    int passengerId = activePassengers[i]["passengerId"]?.Value<int>() ?? 0;
                    if (passengerId > 0)
                    {
                        seatUpdates.Add(new SeatUpdate
                        {
                            PassengerId = passengerId,
                            SeatNumber = newSeats[i]
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

        // ─────────────────────────────────────────────────────────────────────────
        // PROCESS PAYMENT
        // ─────────────────────────────────────────────────────────────────────────

        private async Task ProcessPayment(
            string userId,
            int bookedId,
            decimal amount,
            string status,
            string pnrNumber = "",
            DateTime? newDateOfJourney = null,
            List<SeatUpdate> updatedSeatNumbers = null)
        {
            try
            {
                string newDateOfJourneyString = newDateOfJourney?.ToString("yyyy-MM-dd");

                var paymentData = new
                {
                    userId = userId,
                    amount = amount,
                    bookingId = bookedId,
                    pnrNumber = pnrNumber,
                    JourneyDate = newDateOfJourneyString,
                    PaymentStatus = "Successful",
                    updatedSeatNumbers = updatedSeatNumbers
                   // trainUpdatedSeatNumbers = updatedSeatNumbers
                };

                string jsonContent = JsonConvert.SerializeObject(paymentData);
                System.Diagnostics.Debug.WriteLine("=== PAYMENT REQUEST ===");
                System.Diagnostics.Debug.WriteLine(jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string apiEndpoint;

                // ✅ FIX: status already contains API value — used directly
                if (status.Equals("booked", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("postponed", StringComparison.OrdinalIgnoreCase))
                {
                    apiEndpoint = apiUrl + "TrainTicketBookings/PostponeTrainTicketBooking";
                    System.Diagnostics.Debug.WriteLine($"Using POSTPONE endpoint: {apiEndpoint}");
                }
                else
                {
                    apiEndpoint = apiUrl + "TrainTransactions/PostTrainTransaction";
                    System.Diagnostics.Debug.WriteLine($"Using NORMAL PAYMENT endpoint: {apiEndpoint}");
                }

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint) { Content = content };
                HttpResponseMessage response = await client.PostAsync(
    apiEndpoint,
    new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                //HttpResponseMessage response = await client.SendAsync(request);
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
                        string pnrNumberResponse = data["pnrNumber"]?.ToString() ?? "";
                        string transactionNumber = data["transactionNumber"]?.ToString() ?? "";

                        System.Diagnostics.Debug.WriteLine(
                            $"✓ Payment successful - TXN:{transactionNumber}, PNR:{pnrNumberResponse}");

                        ClearBookingSession();

                        Response.Redirect(
                            $"~/Train_Booking_Confirmation.aspx?pnr={pnrNumberResponse}&txn={transactionNumber}",
                            false);
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
                System.Diagnostics.Debug.WriteLine($"Payment Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowAlert("Payment error: " + ex.Message);
                btnConfirmPayment.Enabled = true;
                btnConfirmPayment.Text = "Confirm Payment";
            }
        }

        // ─────────────────────────────────────────────────────────────────────────

        public class SeatUpdate
        {
            public int PassengerId { get; set; }
            public string SeatNumber { get; set; }
        }

        private void ClearBookingSession()
        {
            Session.Remove("SelectedSeats");
            Session.Remove("JourneyDate");
            Session.Remove("FromStation");
            Session.Remove("ToStation");
            Session.Remove("TripId");
            Session.Remove("UnitPrice");
            Session.Remove("BookedId");
            Session.Remove("PNRNumber");
            Session.Remove("SubTotal");
            Session.Remove("Route");
            Session.Remove("status");
            Session.Remove("PostponeCount");
            Session.Remove("CurrentBookedId");
            Session.Remove("CurrentPnrNumber");
            Session.Remove("IsPostponeMode");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}