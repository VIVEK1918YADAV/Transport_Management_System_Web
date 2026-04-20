using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class Booked_ticket : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        private string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        public Booked_ticket()
        {
            if (client.BaseAddress == null)
            {
                try
                {
                    string url = ConfigurationSettings.AppSettings["api_path"];
                    if (!string.IsNullOrEmpty(url))
                    {
                        client.BaseAddress = new Uri(url);
                        //client.Timeout = TimeSpan.FromSeconds(60);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error configuring HttpClient: {ex.Message}");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/UserLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {

            }

            PageAsyncTask task = new PageAsyncTask(LoadDashboardData);
            RegisterAsyncTask(task);
        }

        protected void ddlActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            string selectedAction = ddl.SelectedValue;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            string pnrNumber = ((HiddenField)item.FindControl("hfPnrNumber")).Value;

            switch (selectedAction)
            {
                case "View":
                    ViewBooking(pnrNumber);
                    break;

                case "Postpone":
                    PostponeBooking(pnrNumber);
                    break;

                //case "Cancel":
                //    CancelBooking(pnrNumber);
                //    break;

                //case "Download":
                //    DownloadBookingDetails(pnrNumber);
                //    break;

                default:
                    break;
            }

            ddl.SelectedIndex = 0;
        }

        // CRITICAL FIX for Booked_ticket.aspx.cs
        // Replace the PostponeBooking method starting from line 119


        //private async void PostponeBooking(string pnrNumber)
        //{
        //    string apiUrl = ConfigurationManager.AppSettings["api_path"];
        //    string endpoint = $"BookedTicketsNew/GetBookedTicketByPnr?pnr={pnrNumber}";
        //    string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

        //    try
        //    {
        //        using (var httpClient = new HttpClient())
        //        {
        //            httpClient.DefaultRequestHeaders.Accept.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
        //            if (response.StatusCode == HttpStatusCode.NotFound)
        //            {
        //                System.Diagnostics.Debug.WriteLine("Error: PNR number not found.");
        //                ShowAlert("No booking found for this PNR.");
        //            }
        //            else if (response.IsSuccessStatusCode)
        //            {
        //                string jsonResult = await response.Content.ReadAsStringAsync();

        //                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
        //                {
        //                    System.Diagnostics.Debug.WriteLine("Error: No data found for the provided PNR.");
        //                    ShowAlert("No booking found for this PNR.");
        //                    return;
        //                }

        //                var bookingDetails = JsonConvert.DeserializeObject<dynamic>(jsonResult);

        //                if (bookingDetails != null)
        //                {
        //                    // Directly access properties of the bookingDetails object (not an array)
        //                    string tripId = bookingDetails.tripId?.ToString() ?? string.Empty;
        //                    string dateOfJourney = bookingDetails.dateOfJourney?.ToString() ?? string.Empty;
        //                    string status = bookingDetails.status?.ToString() ?? string.Empty;
        //                    string ticketCount = bookingDetails.ticketCount?.ToString() ?? string.Empty;
        //                    string price = bookingDetails.subTotal?.ToString() ?? string.Empty;
        //                    string userID = bookingDetails.userId?.ToString() ?? string.Empty;
        //                    string transactionId = bookingDetails.transactionId?.ToString() ?? string.Empty;
        //                    // ✅ ADD THIS LINE
        //                    string postponeCount = bookingDetails.postponeCount?.ToString() ?? "0";

        //                    // ✅ CRITICAL FIX: Get bookedId from the API response
        //                    string bookedId = bookingDetails.bookedId?.ToString() ?? string.Empty;

        //                    // Extract gender and seat details from passengers array
        //                    string gender = string.Empty;
        //                    if (bookingDetails.passengers != null && bookingDetails.passengers.Count > 0)
        //                    {
        //                        // Assuming you need the gender of the first passenger (adjust as needed)
        //                        var firstPassenger = bookingDetails.passengers[0];
        //                        gender = firstPassenger.gender?.ToString() ?? string.Empty;
        //                        string passengerName = firstPassenger.name?.ToString() ?? string.Empty;
        //                        string passengerSeatNumber = firstPassenger.seatNumber?.ToString() ?? string.Empty;

        //                        // Log the extracted details (for debugging)
        //                        System.Diagnostics.Debug.WriteLine($"Passenger Gender: {gender}");
        //                        System.Diagnostics.Debug.WriteLine($"Passenger Name: {passengerName}");
        //                    }

        //                    // Extract seat numbers from passengers array
        //                    string selectedSeat = string.Empty;
        //                    if (bookingDetails.passengers != null)
        //                    {
        //                        var seats = new List<string>();
        //                        foreach (var passenger in bookingDetails.passengers)
        //                        {
        //                            if (passenger.isActive == true)
        //                            {
        //                                seats.Add(passenger.seatNumber?.ToString() ?? string.Empty);
        //                            }
        //                        }
        //                        selectedSeat = string.Join(",", seats);
        //                    }

        //                    System.Diagnostics.Debug.WriteLine($"=== POSTPONE REDIRECT DEBUG ===");
        //                    System.Diagnostics.Debug.WriteLine($"tripId: {tripId}");
        //                    System.Diagnostics.Debug.WriteLine($"pnrNumber: {pnrNumber}");
        //                    System.Diagnostics.Debug.WriteLine($"bookedId: {bookedId}");  // ✅ NOW LOGGING THIS!
        //                    System.Diagnostics.Debug.WriteLine($"selectedSeat: {selectedSeat}");
        //                    System.Diagnostics.Debug.WriteLine($"dateOfJourney: {dateOfJourney}");
        //                    System.Diagnostics.Debug.WriteLine($"status: {status}");
        //                    System.Diagnostics.Debug.WriteLine($"ticketCount: {ticketCount}");


        //                    string redirectUrl = $"~/select_seat.aspx?tripId={tripId}&pnrNumber={pnrNumber}&bookedId={bookedId}&selectedSeat={selectedSeat}&dateOfJourney={dateOfJourney}&gender={Uri.EscapeDataString(gender)}&status={status}&SeatPrice={price}&userId={userID}&transactionId={transactionId}&ticketCount={ticketCount}&postponeCount={postponeCount}";


        //                    System.Diagnostics.Debug.WriteLine($"Redirecting to: {redirectUrl}");
        //                    Response.Redirect(redirectUrl);
        //                }
        //                else
        //                {
        //                    System.Diagnostics.Debug.WriteLine("Error: Unable to parse booking details from API response.");
        //                    ShowAlert("Unable to parse booking details.");
        //                }
        //            }
        //            else
        //            {
        //                string errorContent = await response.Content.ReadAsStringAsync();
        //                System.Diagnostics.Debug.WriteLine($"Error: API call failed with status code {response.StatusCode}, Content: {errorContent}");
        //                ShowAlert("Error: Unable to retrieve booking details. Please try again later.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error fetching data: {ex.Message}");
        //        ShowAlert($"Error fetching data: {ex.Message}");
        //    }
        //}
        private async void PostponeBooking(string pnrNumber)
        {
            string apiUrl = ConfigurationManager.AppSettings["api_path"];
            string endpoint = $"BookedTicketsNew/GetBookedTicketByPnr?pnr={pnrNumber}";
            string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        System.Diagnostics.Debug.WriteLine("Error: PNR number not found.");
                        ShowAlert("No booking found for this PNR.");
                    }
                    else if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                        {
                            System.Diagnostics.Debug.WriteLine("Error: No data found for the provided PNR.");
                            ShowAlert("No booking found for this PNR.");
                            return;
                        }

                        var bookingDetails = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                        if (bookingDetails != null)
                        {
                            string tripId = bookingDetails.tripId?.ToString() ?? string.Empty;
                            string dateOfJourney = bookingDetails.dateOfJourney?.ToString() ?? string.Empty;
                            string status = bookingDetails.status?.ToString() ?? string.Empty;
                            string ticketCount = bookingDetails.ticketCount?.ToString() ?? string.Empty;
                            string price = bookingDetails.subTotal?.ToString() ?? string.Empty;
                            string userID = bookingDetails.userId?.ToString() ?? string.Empty;
                            string transactionId = bookingDetails.transactionId?.ToString() ?? string.Empty;
                            string postponeCount = bookingDetails.postponeCount?.ToString() ?? "0";
                            string bookedId = bookingDetails.bookedId?.ToString() ?? string.Empty;

                            DateTime journeyDate;
                            if (!DateTime.TryParse(dateOfJourney, out journeyDate))
                            {
                                ShowAlert("Invalid journey date.");
                                return;
                            }

                            if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                            {
                                ShowAlert("The ticket has already been cancelled and cannot be postponed.");
                                return;
                            }

                            if (journeyDate >= DateTime.Today)
                            {
                                string gender = string.Empty;
                                if (bookingDetails.passengers != null && bookingDetails.passengers.Count > 0)
                                {
                                    var firstPassenger = bookingDetails.passengers[0];
                                    gender = firstPassenger.gender?.ToString() ?? string.Empty;
                                    string passengerName = firstPassenger.name?.ToString() ?? string.Empty;
                                    string passengerSeatNumber = firstPassenger.seatNumber?.ToString() ?? string.Empty;
                                }

                                string selectedSeat = string.Empty;
                                if (bookingDetails.passengers != null)
                                {
                                    var seats = new List<string>();
                                    foreach (var passenger in bookingDetails.passengers)
                                    {
                                        if (passenger.isActive == true)
                                        {
                                            seats.Add(passenger.seatNumber?.ToString() ?? string.Empty);
                                        }
                                    }
                                    selectedSeat = string.Join(",", seats);
                                }

                                System.Diagnostics.Debug.WriteLine($"=== POSTPONE REDIRECT DEBUG ===");
                                System.Diagnostics.Debug.WriteLine($"tripId: {tripId}");
                                System.Diagnostics.Debug.WriteLine($"pnrNumber: {pnrNumber}");
                                System.Diagnostics.Debug.WriteLine($"bookedId: {bookedId}");
                                System.Diagnostics.Debug.WriteLine($"selectedSeat: {selectedSeat}");
                                System.Diagnostics.Debug.WriteLine($"dateOfJourney: {dateOfJourney}");
                                System.Diagnostics.Debug.WriteLine($"status: {status}");
                                System.Diagnostics.Debug.WriteLine($"ticketCount: {ticketCount}");

                                string redirectUrl = $"~/select_seat.aspx?tripId={tripId}&pnrNumber={pnrNumber}&bookedId={bookedId}&selectedSeat={selectedSeat}&dateOfJourney={dateOfJourney}&gender={Uri.EscapeDataString(gender)}&status={status}&SeatPrice={price}&userId={userID}&transactionId={transactionId}&ticketCount={ticketCount}&postponeCount={postponeCount}";

                                System.Diagnostics.Debug.WriteLine($"Redirecting to: {redirectUrl}");
                                Response.Redirect(redirectUrl);
                            }
                            else
                            {
                                ShowAlert("The ticket has already expired and cannot be postponed.");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error: Unable to parse booking details from API response.");
                            ShowAlert("Unable to parse booking details.");
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Error: API call failed with status code {response.StatusCode}, Content: {errorContent}");
                        ShowAlert("Error: Unable to retrieve booking details. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching data: {ex.Message}");
                ShowAlert($"Error fetching data: {ex.Message}");
            }
        }

        //private async void PostponeBooking(string pnrNumber)
        //{
        //    string apiUrl = ConfigurationManager.AppSettings["api_path"];
        //    string endpoint = $"BookedTicketsNew/GetBookedTicketByPnr?pnr={pnrNumber}";
        //    string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

        //    try
        //    {
        //        using (var httpClient = new HttpClient())
        //        {
        //            httpClient.DefaultRequestHeaders.Accept.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
        //            if (response.StatusCode == HttpStatusCode.NotFound)
        //            {
        //                System.Diagnostics.Debug.WriteLine("Error: PNR number not found.");
        //                ShowAlert("No booking found for this PNR.");
        //            }
        //            else if (response.IsSuccessStatusCode)
        //            {
        //                string jsonResult = await response.Content.ReadAsStringAsync();

        //                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
        //                {
        //                    System.Diagnostics.Debug.WriteLine("Error: No data found for the provided PNR.");
        //                    ShowAlert("No booking found for this PNR.");
        //                    return;
        //                }

        //                var bookingDetails = JsonConvert.DeserializeObject<dynamic>(jsonResult);

        //                if (bookingDetails != null)
        //                {
        //                    string tripId = bookingDetails.tripId?.ToString() ?? string.Empty;
        //                    string dateOfJourney = bookingDetails.dateOfJourney?.ToString() ?? string.Empty;
        //                    string status = bookingDetails.status?.ToString() ?? string.Empty;
        //                    string ticketCount = bookingDetails.ticketCount?.ToString() ?? string.Empty;
        //                    string price = bookingDetails.subTotal?.ToString() ?? string.Empty;
        //                    string userID = bookingDetails.userId?.ToString() ?? string.Empty;
        //                    string transactionId = bookingDetails.transactionId?.ToString() ?? string.Empty;
        //                    string postponeCount = bookingDetails.postponeCount?.ToString() ?? "0";
        //                    string bookedId = bookingDetails.bookedId?.ToString() ?? string.Empty;

        //                    // Extract journey date
        //                    DateTime journeyDate;
        //                    if (!DateTime.TryParse(dateOfJourney, out journeyDate))
        //                    {
        //                        ShowAlert("Invalid journey date.");
        //                        return;
        //                    }

        //                    // Check if the journey date is greater than or equal to today
        //                    if (journeyDate >= DateTime.Today)
        //                    {
        //                        // Proceed with the postponement if the journey date is valid
        //                        string gender = string.Empty;
        //                        if (bookingDetails.passengers != null && bookingDetails.passengers.Count > 0)
        //                        {
        //                            var firstPassenger = bookingDetails.passengers[0];
        //                            gender = firstPassenger.gender?.ToString() ?? string.Empty;
        //                            string passengerName = firstPassenger.name?.ToString() ?? string.Empty;
        //                            string passengerSeatNumber = firstPassenger.seatNumber?.ToString() ?? string.Empty;
        //                        }

        //                        // Extract seat numbers from passengers array
        //                        string selectedSeat = string.Empty;
        //                        if (bookingDetails.passengers != null)
        //                        {
        //                            var seats = new List<string>();
        //                            foreach (var passenger in bookingDetails.passengers)
        //                            {
        //                                if (passenger.isActive == true)
        //                                {
        //                                    seats.Add(passenger.seatNumber?.ToString() ?? string.Empty);
        //                                }
        //                            }
        //                            selectedSeat = string.Join(",", seats);
        //                        }

        //                        System.Diagnostics.Debug.WriteLine($"=== POSTPONE REDIRECT DEBUG ===");
        //                        System.Diagnostics.Debug.WriteLine($"tripId: {tripId}");
        //                        System.Diagnostics.Debug.WriteLine($"pnrNumber: {pnrNumber}");
        //                        System.Diagnostics.Debug.WriteLine($"bookedId: {bookedId}");
        //                        System.Diagnostics.Debug.WriteLine($"selectedSeat: {selectedSeat}");
        //                        System.Diagnostics.Debug.WriteLine($"dateOfJourney: {dateOfJourney}");
        //                        System.Diagnostics.Debug.WriteLine($"status: {status}");
        //                        System.Diagnostics.Debug.WriteLine($"ticketCount: {ticketCount}");

        //                        string redirectUrl = $"~/select_seat.aspx?tripId={tripId}&pnrNumber={pnrNumber}&bookedId={bookedId}&selectedSeat={selectedSeat}&dateOfJourney={dateOfJourney}&gender={Uri.EscapeDataString(gender)}&status={status}&SeatPrice={price}&userId={userID}&transactionId={transactionId}&ticketCount={ticketCount}&postponeCount={postponeCount}";

        //                        System.Diagnostics.Debug.WriteLine($"Redirecting to: {redirectUrl}");
        //                        Response.Redirect(redirectUrl);
        //                    }
        //                    else
        //                    {
        //                        // If the journey date has passed, show an alert
        //                        ShowAlert("The ticket has already expired and cannot be postponed.");
        //                    }
        //                }
        //                else
        //                {
        //                    System.Diagnostics.Debug.WriteLine("Error: Unable to parse booking details from API response.");
        //                    ShowAlert("Unable to parse booking details.");
        //                }
        //            }
        //            else
        //            {
        //                string errorContent = await response.Content.ReadAsStringAsync();
        //                System.Diagnostics.Debug.WriteLine($"Error: API call failed with status code {response.StatusCode}, Content: {errorContent}");
        //                ShowAlert("Error: Unable to retrieve booking details. Please try again later.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error fetching data: {ex.Message}");
        //        ShowAlert($"Error fetching data: {ex.Message}");
        //    }
        //}


        private void ShowAlert(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        private void ViewBooking(string pnrNumber)
        {
            Response.Redirect("BookingDetails.aspx?pnr=" + pnrNumber);
        }

        //private void CancelBooking(string pnrNumber)
        //{
        //    // Example: Call method to cancel booking
        //    // ShowCancelDialog(pnrNumber);
        //}

        //private void DownloadBookingDetails(string pnrNumber)
        //{
        //    // Example: Trigger file download or other download logic
        //    // TriggerDownload(pnrNumber);
        //}

        private async Task LoadDashboardData()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                var bookings = await GetBookedTickets(userId);

                if (bookings != null && bookings.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Successfully loaded {bookings.Count} bookings");

                    var bookingList = new List<BookingViewModel>();

                    foreach (var b in bookings)
                    {
                        try
                        {
                            var viewModel = new BookingViewModel
                            {
                                Id = b.BookedId,
                                PnrNumber = b.PnrNumber ?? "N/A",
                                DateOfJourney = ParseDate(b.DateOfJourney),
                                DateOfJourneyFormatted = FormatDate(b.DateOfJourney),
                                SubTotal = b.SubTotal,
                                Status = b.Status ?? "Unknown",
                                StatusText = b.Status ?? "Unknown",
                                PostponeCount = ParsePostponeCount(b.PostponeCount),
                                PickupName = ExtractLocationName(b.SourceDestination, true),
                                DropName = ExtractLocationName(b.SourceDestination, false),
                                //PickupTime = b.  // You can add the PickupTime if needed
                                SeatsDisplay = GetSeatsDisplay(b.Passengers),
                                IsAC = "AC",
                                BagsCount = b.BagsCount,
                                BagsCharge = b.BagsCharge,
                                TripTitle = b.TripTitle ?? ""
                            };

                            bookingList.Add(viewModel);
                            System.Diagnostics.Debug.WriteLine($"Added booking: PNR={viewModel.PnrNumber}, Status={viewModel.Status}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing booking {b.PnrNumber}: {ex.Message}");
                            // Continue processing other bookings
                        }
                    }

                    // Sort by CreatedAt field to show latest bookings on top
                    var sortedList = bookingList.OrderByDescending(b => b.CreatedAt).ToList();

                    rptBookings.DataSource = sortedList;
                    rptBookings.DataBind();

                    System.Diagnostics.Debug.WriteLine($"Bound {sortedList.Count} items to repeater");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No bookings found");
                    rptBookings.DataSource = new List<BookingViewModel>();
                    rptBookings.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                rptBookings.DataSource = new List<BookingViewModel>();
                rptBookings.DataBind();

                ShowMessage("Unable to load booking data. Please try again.", "error");
            }
        }


        private void ShowMessage(string message, string type)
        {
            message = message.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"notify('{type}', '{message}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Msg_" + Guid.NewGuid(), script, true);
        }

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date;

            return DateTime.MinValue;
        }

        private string FormatDate(string dateString)
        {
            DateTime date = ParseDate(dateString);
            return date != DateTime.MinValue ? date.ToString("dd MMM, yyyy") : "N/A";
        }

        private int ParsePostponeCount(string postponeCount)
        {
            if (string.IsNullOrEmpty(postponeCount))
                return 0;

            int count;
            return int.TryParse(postponeCount, out count) ? count : 0;
        }

        private string GetSeatsDisplay(List<Passenger> passengers)
        {
            if (passengers == null || passengers.Count == 0)
                return "N/A";

            try
            {
                var activePassengers = passengers.Where(p => p.IsActive).ToList();
                if (activePassengers.Count == 0)
                    return "N/A";

                var seatNumbers = activePassengers.Select(p => p.SeatNumber).ToList();
                return string.Join(", ", seatNumbers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting seats display: {ex.Message}");
                return "N/A";
            }
        }

        private string ExtractLocationName(string sourceDestination, bool isPickup)
        {
            if (string.IsNullOrEmpty(sourceDestination))
                return "N/A";

            try
            {
                var parts = sourceDestination.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                    return isPickup ? parts[0].Trim() : parts[1].Trim();

                return sourceDestination;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting location: {ex.Message}");
                return sourceDestination;
            }
        }

        protected string GetStatusBadge(object statusObj)
        {
            try
            {
                string status = statusObj?.ToString() ?? "Unknown";

                switch (status.ToLower())
                {
                    case "booked":
                        return "<span class='badge badge--success'>Booked</span>";
                    case "pending":
                        return "<span class='badge badge--warning'>Pending</span>";
                    case "cancelled":
                        return "<span class='badge badge--primary'>Cancelled</span>";
                    case "postponed":
                        return "<span class='badge badge--dark'>Postponed</span>";
                    case "rejected":
                        return "<span class='badge badge--danger'>Rejected</span>";
                    default:
                        return $"<span class='badge badge--secondary'>{status}</span>";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStatusBadge: {ex.Message}");
                return "<span class='badge badge--secondary'>Unknown</span>";
            }
        }

        protected string GetPostponedBadge(int count)
        {
            if (count > 0)
            {
                return $"<span class='badge badge--warning'>Yes ({count}x)</span>";
            }
            return "<span class='badge badge--secondary'>No</span>";
        }

        private async Task<List<BookedTicket>> GetBookedTickets(int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(apiUrl))
                {
                    System.Diagnostics.Debug.WriteLine("API URL is not configured");
                    return new List<BookedTicket>();
                }

                string endpoint = $"BookedTicketsNew/GetBookedTicketsByUserId/{userId}";
                string fullUrl = $"{apiUrl.TrimEnd('/')}/{endpoint}";

                System.Diagnostics.Debug.WriteLine($"Fetching bookings from: {fullUrl}");

                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return new List<BookedTicket>();
                }

                string jsonResult = await response.Content.ReadAsStringAsync();

                if (jsonResult.Length > 500)
                {
                    System.Diagnostics.Debug.WriteLine($"API Response received: {jsonResult.Substring(0, 500)}...");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Response: {jsonResult}");
                }

                if (string.IsNullOrEmpty(jsonResult) || jsonResult.Trim() == "[]")
                {
                    System.Diagnostics.Debug.WriteLine("Empty response received");
                    return new List<BookedTicket>();
                }

                var tickets = JsonConvert.DeserializeObject<List<BookedTicket>>(jsonResult);

                if (tickets != null && tickets.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Successfully deserialized {tickets.Count} tickets");
                    System.Diagnostics.Debug.WriteLine($"First ticket - PNR: {tickets[0].PnrNumber}, Status: {tickets[0].Status}");
                }

                return tickets ?? new List<BookedTicket>();
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
                return new List<BookedTicket>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting booked tickets: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new List<BookedTicket>();
            }
        }
    }

    // ViewModel for binding to repeater
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string PnrNumber { get; set; }
        public DateTime DateOfJourney { get; set; }
        public string DateOfJourneyFormatted { get; set; }
        public decimal SubTotal { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public decimal BagsCount { get; set; }
        public decimal BagsCharge { get; set; }
        public int PostponeCount { get; set; }
        public string PickupName { get; set; }
        public string DropName { get; set; }
        public string PickupTime { get; set; }
        public string SeatsDisplay { get; set; }
        public string IsAC { get; set; }
        public string TripTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Model classes for API response
    public class BookedTicket
    {
        [JsonProperty("bookedId")]
        public int BookedId { get; set; }

        [JsonProperty("pnrNumber")]
        public string PnrNumber { get; set; }

        [JsonProperty("tripId")]
        public int TripId { get; set; }

        [JsonProperty("tripTitle")]
        public string TripTitle { get; set; }

        [JsonProperty("sourceDestination")]
        public string SourceDestination { get; set; }

        [JsonProperty("pickupPoint")]
        public int PickupPoint { get; set; }

        [JsonProperty("droppingPoint")]
        public int DroppingPoint { get; set; }

        [JsonProperty("ticketCount")]
        public int TicketCount { get; set; }

        [JsonProperty("postponeCount")]
        public string PostponeCount { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("subTotal")]
        public decimal SubTotal { get; set; }

        [JsonProperty("dateOfJourney")]
        public string DateOfJourney { get; set; }

        [JsonProperty("checkin")]
        public string Checkin { get; set; }

        [JsonProperty("bagsCount")]
        public int BagsCount { get; set; }

        [JsonProperty("bagsWeight")]
        public string BagsWeight { get; set; }

        [JsonProperty("bagsCharge")]
        public decimal BagsCharge { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("transactionId")]
        public int? TransactionId { get; set; }  // Made nullable

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("passengers")]
        public List<Passenger> Passengers { get; set; }
    }

    public class Passenger
    {
        [JsonProperty("passengerId")]
        public int PassengerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("seatNumber")]
        public string SeatNumber { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public class Location
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
