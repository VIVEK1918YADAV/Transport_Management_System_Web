using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class BookingConfirmation : System.Web.UI.Page
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
                    Response.Redirect("~/ticket.aspx");
                    return;
                }

                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx");
                    return;
                }

                // Display basic info from query string
                lblPNRNumber.Text = pnr;
                lblTransactionNumber.Text = txn;

                // Load detailed booking information using userId
                int userId = Convert.ToInt32(Session["UserId"]);
                RegisterAsyncTask(new PageAsyncTask(() => LoadBookingDetails(userId, pnr)));
            }
        }

        private async Task LoadBookingDetails(int userId, string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}BookedTicketsNew/GetBookedTicketsByUserId/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading booking details from: {apiEndpoint}");

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Booking Details Response: {jsonResponse}");

                    JArray bookings = JArray.Parse(jsonResponse);

                    // Find the booking with matching PNR
                    JObject matchingBooking = null;
                    foreach (JObject booking in bookings)
                    {
                        string bookingPnr = booking["pnrNumber"]?.ToString() ?? "";
                        if (bookingPnr.Equals(pnrNumber, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingBooking = booking;
                            break;
                        }
                    }

                    if (matchingBooking != null)
                    {
                        DisplayBookingInfo(matchingBooking);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No booking found with PNR: {pnrNumber}");

                        // If no match found, display the most recent booking
                        if (bookings.Count > 0)
                        {
                            DisplayBookingInfo(bookings[0] as JObject);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Response: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading booking details: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }

        //private void DisplayBookingInfo(JObject bookingData)
        //{
        //    try
        //    {
        //        // Extract booking information
        //        string sourceDestination = bookingData["sourceDestination"]?.ToString() ?? "";
        //        string dateOfJourney = bookingData["dateOfJourney"]?.ToString() ?? "";
        //        decimal subTotal = bookingData["subTotal"]?.Value<decimal>() ?? 0;
        //        string bookingStatus = bookingData["status"]?.ToString() ?? "Booked";
        //        int ticketCount = bookingData["ticketCount"]?.Value<int>() ?? 0;

        //        // Display route
        //        if (!string.IsNullOrEmpty(sourceDestination))
        //        {
        //            var parts = sourceDestination.Split('-');
        //            if (parts.Length == 2)
        //            {
        //                lblRoute.Text = $"{parts[0].Trim()} → {parts[1].Trim()}";
        //            }
        //            else
        //            {
        //                lblRoute.Text = sourceDestination;
        //            }
        //        }

        //        // Display journey date
        //        if (!string.IsNullOrEmpty(dateOfJourney))
        //        {
        //            try
        //            {
        //                DateTime journeyDate = Convert.ToDateTime(dateOfJourney);
        //                lblJourneyDate.Text = journeyDate.ToString("dd MMM yyyy");
        //            }
        //            catch
        //            {
        //                lblJourneyDate.Text = dateOfJourney;
        //            }
        //        }

        //        // Display amount
        //        lblAmount.Text = $"CDF {subTotal:N0}";

        //        // Display status
        //        lblStatus.Text = bookingStatus;

        //        // Display passenger count
        //        lblPassengerCount.Text = ticketCount.ToString();

        //        // Extract passenger details and seat numbers
        //        if (bookingData["passengers"] != null)
        //        {
        //            JArray passengers = bookingData["passengers"] as JArray;
        //            string seatNumbers = "";

        //            foreach (JObject passenger in passengers)
        //            {
        //                string seatNumber = passenger["seatNumber"]?.ToString() ?? "";
        //                if (!string.IsNullOrEmpty(seatNumber))
        //                {
        //                    seatNumbers += seatNumber + ", ";
        //                }
        //            }

        //            if (!string.IsNullOrEmpty(seatNumbers))
        //            {
        //                lblSeats.Text = seatNumbers.TrimEnd(',', ' ');
        //            }
        //            else
        //            {
        //                lblSeats.Text = "N/A";
        //            }
        //        }
        //        else
        //        {
        //            lblSeats.Text = "N/A";
        //        }

        //        System.Diagnostics.Debug.WriteLine("Booking details displayed successfully");
        //        System.Diagnostics.Debug.WriteLine($"Route: {lblRoute.Text}");
        //        System.Diagnostics.Debug.WriteLine($"Journey Date: {lblJourneyDate.Text}");
        //        System.Diagnostics.Debug.WriteLine($"Seats: {lblSeats.Text}");
        //        System.Diagnostics.Debug.WriteLine($"Amount: {lblAmount.Text}");
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error displaying booking info: " + ex.Message);
        //        System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
        //    }
        //}
        private void DisplayBookingInfo(JObject bookingData)
        {
            try
            {
                // Extract booking information
                string sourceDestination = bookingData["sourceDestination"]?.ToString() ?? "";
                string dateOfJourney = bookingData["dateOfJourney"]?.ToString() ?? "";
                decimal subTotal = bookingData["subTotal"]?.Value<decimal>() ?? 0;
                string bookingStatus = bookingData["status"]?.ToString() ?? "Booked";
                int ticketCount = bookingData["ticketCount"]?.Value<int>() ?? 0;

                // ✓ Extract postpone balance fields
                decimal? postponeBalance = bookingData["postBalance"]?.Value<decimal?>();
                decimal? postponeBalance2 = bookingData["postBalance2"]?.Value<decimal?>();

                // Display route
                if (!string.IsNullOrEmpty(sourceDestination))
                {
                    var parts = sourceDestination.Split('-');
                    if (parts.Length == 2)
                    {
                        lblRoute.Text = $"{parts[0].Trim()} → {parts[1].Trim()}";
                    }
                    else
                    {
                        lblRoute.Text = sourceDestination;
                    }
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
                decimal amountToShow = subTotal;

                // Check if postponed twice (2nd time)
                if (postponeBalance2.HasValue && postponeBalance2.Value > 0)
                {
                    amountToShow = postponeBalance2.Value;
                    lblAmount.Text = $"CDF {amountToShow:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing PostponeBalance2: {amountToShow}");
                }
                // Check if postponed once (1st time)
                else if (postponeBalance.HasValue && postponeBalance.Value > 0)
                {
                    amountToShow = postponeBalance.Value;
                    lblAmount.Text = $"CDF {amountToShow:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing PostponeBalance: {amountToShow}");
                }
                // Show subtotal (not postponed)
                else
                {
                    lblAmount.Text = $"CDF {subTotal:N0}";
                    System.Diagnostics.Debug.WriteLine($"Showing SubTotal: {subTotal}");
                }

                // Display status
                lblStatus.Text = bookingStatus;

                // Display passenger count
                lblPassengerCount.Text = ticketCount.ToString();

                // Extract passenger details and seat numbers
                if (bookingData["passengers"] != null)
                {
                    JArray passengers = bookingData["passengers"] as JArray;
                    string seatNumbers = "";

                    foreach (JObject passenger in passengers)
                    {
                        string seatNumber = passenger["seatNumber"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(seatNumber))
                        {
                            seatNumbers += seatNumber + ", ";
                        }
                    }

                    if (!string.IsNullOrEmpty(seatNumbers))
                    {
                        lblSeats.Text = seatNumbers.TrimEnd(',', ' ');
                    }
                    else
                    {
                        lblSeats.Text = "N/A";
                    }
                }
                else
                {
                    lblSeats.Text = "N/A";
                }

                System.Diagnostics.Debug.WriteLine("Booking details displayed successfully");
                System.Diagnostics.Debug.WriteLine($"Route: {lblRoute.Text}");
                System.Diagnostics.Debug.WriteLine($"Journey Date: {lblJourneyDate.Text}");
                System.Diagnostics.Debug.WriteLine($"Seats: {lblSeats.Text}");
                System.Diagnostics.Debug.WriteLine($"Amount: {lblAmount.Text}");
                System.Diagnostics.Debug.WriteLine($"Status: {lblStatus.Text}");
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
                Response.Redirect("~/MyBookings.aspx");
            }
            else
            {
                Response.Redirect("~/ticket.aspx");
            }
        }

        protected void btnBookAnother_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ticket.aspx");
        }
    }
}