using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Train_Booking_Confirmation : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pnr = Request.QueryString["pnr"];
                string txn = Request.QueryString["txn"];

                if (string.IsNullOrEmpty(pnr) || string.IsNullOrEmpty(txn))
                {
                    Response.Redirect("~/TrainTicket.aspx");
                    return;
                }

                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/TrainTicket.aspx");
                    return;
                }

                lblPNRNumber.Text = pnr;
                lblTransactionNumber.Text = txn;

                string userId = Session["UserId"].ToString();
                RegisterAsyncTask(new PageAsyncTask(() => LoadBookingDetails(userId, pnr)));
            }
        }
    
        private async Task LoadBookingDetails(string userId, string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}TrainTicketBookings/GetBookedTrainTicketByPnr?pnr={Uri.EscapeDataString(pnrNumber)}";
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Booking Details Response: {jsonResponse}");

                    JObject matchingBooking = null;

                    // Handle both object and array responses
                    var token = JToken.Parse(jsonResponse);

                    if (token is JArray bookings)
                    {
                        foreach (JObject booking in bookings)
                        {
                            string bookingPnr = booking["pnrNumber"]?.ToString() ?? "";
                            if (bookingPnr.Equals(pnrNumber, StringComparison.OrdinalIgnoreCase))
                            {
                                matchingBooking = booking;
                                break;
                            }
                        }

                        // Fallback to first item if no PNR match
                        if (matchingBooking == null && bookings.Count > 0)
                            matchingBooking = bookings[0] as JObject;
                    }
                    else if (token is JObject singleBooking)
                    {
                        // API returned a single object directly
                        matchingBooking = singleBooking;
                    }

                    if (matchingBooking != null)
                        DisplayBookingInfo(matchingBooking);
                    else
                        System.Diagnostics.Debug.WriteLine($"No booking found with PNR: {pnrNumber}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading booking details: " + ex.Message);
            }
        }
      
        private void DisplayBookingInfo(JObject bookingData)
        {
            try
            {
                // Extract booking information
                string TrainName = bookingData["trainName"]?.ToString() ?? "";
                string TrainNumber = bookingData["trainNumber"]?.ToString() ?? "";
                string sourceDestination = bookingData["sourceDestination"]?.ToString() ?? "";
                string dateOfJourney = bookingData["dateOfJourney"]?.ToString() ?? "";
                decimal subTotal = bookingData["subTotal"]?.Value<decimal>() ?? 0;
                string bookingStatus = bookingData["status"]?.ToString() ?? "Booked";
                int ticketCount = bookingData["ticketCount"]?.Value<int>() ?? 0;

                // ✓ Extract postponeAmt1 & postponeAmt2 from transactions
                decimal? postponeAmt1 = null;
                decimal? postponeAmt2 = null;

                if (bookingData["transactions"] is JArray transactions && transactions.Count > 0)
                {
                    // Get the latest transaction by createdAt (or simply last with status "Success" / highest trxId)
                    JObject latestTransaction = transactions
                        .OfType<JObject>()
                        .OrderByDescending(t => t["trxId"]?.Value<int>() ?? 0)
                        .FirstOrDefault();

                    if (latestTransaction != null)
                    {
                        postponeAmt1 = latestTransaction["postponeAmt1"]?.Value<decimal?>();
                        postponeAmt2 = latestTransaction["postponeAmt2"]?.Value<decimal?>();

                        System.Diagnostics.Debug.WriteLine($"Latest TrxId: {latestTransaction["trxId"]}");
                        System.Diagnostics.Debug.WriteLine($"PostponeAmt1: {postponeAmt1}, PostponeAmt2: {postponeAmt2}");
                    }
                }

                lblTrainName.Text = TrainName;
                lblTrainNumber.Text = TrainNumber;

                // Display route
                if (!string.IsNullOrEmpty(sourceDestination))
                {
                    var parts = sourceDestination.Split('-');
                    if (parts.Length == 2)
                        lblRoute.Text = $"{parts[0].Trim()} → {parts[1].Trim()}";
                    else
                        lblRoute.Text = sourceDestination;
                }

                // Display journey date
                if (!string.IsNullOrEmpty(dateOfJourney))
                {
                    try
                    {
                        DateTime journeyDate = Convert.ToDateTime(dateOfJourney);
                        lblJourneyDate.Text = journeyDate.ToString("dd MMM yyyy");
                    }
                    catch
                    {
                        lblJourneyDate.Text = dateOfJourney;
                    }
                }

                // ✓ Display amount based on postpone status
                if (postponeAmt2.HasValue && postponeAmt2.Value > 0)
                {
                    lblAmount.Text = $"CDF {postponeAmt2.Value:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing PostponeAmt2: {postponeAmt2.Value}");
                }
                else if (postponeAmt1.HasValue && postponeAmt1.Value > 0)
                {
                    lblAmount.Text = $"CDF {postponeAmt1.Value:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing PostponeAmt1: {postponeAmt1.Value}");
                }
                else
                {
                    lblAmount.Text = $"CDF {subTotal:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing SubTotal: {subTotal}");
                }

                // Display status
                lblStatus.Text = bookingStatus;

                // Display passenger count
                lblPassengerCount.Text = ticketCount.ToString();

                // Extract passenger seat numbers
                if (bookingData["passengers"] is JArray passengers)
                {
                    string seatNumbers = string.Join(", ", passengers
                        .OfType<JObject>()
                        .Select(p => p["seatNumber"]?.ToString())
                        .Where(s => !string.IsNullOrEmpty(s)));

                    lblSeats.Text = !string.IsNullOrEmpty(seatNumbers) ? seatNumbers : "N/A";
                }
                else
                {
                    lblSeats.Text = "N/A";
                }

                System.Diagnostics.Debug.WriteLine($"Train: {lblTrainName.Text} | Route: {lblRoute.Text}");
                System.Diagnostics.Debug.WriteLine($"Amount: {lblAmount.Text} | Status: {lblStatus.Text}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error displaying booking info: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }
        protected void btnDownloadTicket_Click(object sender, EventArgs e)
        {
            string pnr = Request.QueryString["pnr"];
            Response.Redirect($"~/DownloadTicket.aspx?pnr={pnr}");
        }

        protected void btnViewBookings_Click(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserId"] != null)
            {
                Response.Redirect("~/Train_MyBookings.aspx");
            }
            else
            {
                Response.Redirect("~/TrainTicket.aspx");
            }
        }

        protected void btnBookAnother_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TrainTicket.aspx");
        }

    }
}