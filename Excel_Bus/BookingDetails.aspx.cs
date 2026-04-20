using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Excel_Bus
{
    public partial class BookingDetails : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        string apiBaseUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pnr = Request.QueryString["pnr"];

                if (string.IsNullOrEmpty(pnr))
                {
                    Response.Redirect("MyBookings.aspx");
                    return;
                }

                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                RegisterAsyncTask(new PageAsyncTask(() => LoadBookingData(userId, pnr)));
            }
        }

        private async Task LoadBookingData(int userId, string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/GetBookedTicketsByUserId/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading booking details from: {apiEndpoint}");

                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray bookings = JArray.Parse(jsonResponse);

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
                        ViewState["CurrentBooking"] = matchingBooking.ToString();

                        // ✅ IMPORTANT: Load cancellation details FIRST to get accurate refund amount
                        string bookedId = matchingBooking["bookedId"]?.ToString();
                        if (!string.IsNullOrEmpty(bookedId))
                        {
                            await LoadCancellationDetails(bookedId);
                        }

                        // Then populate booking details
                        PopulateBookingDetails(matchingBooking);
                    }
                    else
                    {
                        ShowAlert("Booking not found.");
                        Response.Redirect("MyBookings.aspx");
                    }
                }
                else
                {
                    ShowAlert("Failed to load booking details.");
                    Response.Redirect("MyBookings.aspx");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading booking: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private async Task LoadCancellationDetails(string bookedId)
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}TempPassengerTables/GetByBookedId/{bookedId}";
                System.Diagnostics.Debug.WriteLine($"Loading cancellation details from: {apiEndpoint}");

                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray cancellations = JArray.Parse(jsonResponse);

                    if (cancellations != null && cancellations.Count > 0)
                    {
                        int cancelledCount = cancellations.Count;
                        decimal totalRefund = 0;

                        // ✅ Calculate total refund from all cancelled passengers
                        foreach (JObject cancellation in cancellations)
                        {
                            decimal amount = cancellation["amount"]?.Value<decimal>() ?? 0;
                            totalRefund += amount;
                        }

                        decimal cancellationCharge = totalRefund * 0.10m; // 10% charge

                        // ✅ CRITICAL FIX: Update ALL refund-related labels with cumulative values
                        lblCancelledCount.Text = cancelledCount.ToString();
                        lblTotalRefund.Text = $"CDF {totalRefund:N0}";
                        //lblCancellationCharge.Text = $"CDF {cancellationCharge:N0}";
                        //lblRefundPercentage.Text = "90%";

                        // ✅ MOST IMPORTANT: Update main refund amount label in Payment Information section
                        lblrefundamount.Text = $"CDF {totalRefund:N0}";

                        // Bind to repeater
                        rptCancellations.DataSource = cancellations;
                        rptCancellations.DataBind();

                        // Show section
                        divCancellationDetails.Visible = true;

                        System.Diagnostics.Debug.WriteLine($"✅ Refund amount set to: CDF {totalRefund:N0} (from {cancelledCount} cancellations)");
                    }
                    else
                    {
                        // No cancellations found
                        lblrefundamount.Text = "CDF 0";
                        divCancellationDetails.Visible = false;
                        System.Diagnostics.Debug.WriteLine("No cancellations found, refund = CDF 0");
                    }
                }
                else
                {
                    // API call failed - set refund to 0
                    lblrefundamount.Text = "CDF 0";
                    divCancellationDetails.Visible = false;
                    System.Diagnostics.Debug.WriteLine($"Failed to load cancellation details. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Error occurred - set refund to 0
                lblrefundamount.Text = "CDF 0";
                divCancellationDetails.Visible = false;
                System.Diagnostics.Debug.WriteLine("Error loading cancellation details: " + ex.Message);
            }
        }

        private void PopulateBookingDetails(JObject bookingData)
        {
            try
            {
                // Basic Info
                lblPNRNumber.Text = bookingData["pnrNumber"]?.ToString() ?? "";
                lblBookedId.Text = bookingData["bookedId"]?.ToString() ?? "";
                lblTripTitle.Text = bookingData["tripTitle"]?.ToString() ?? "";

                // Route
                string sourceDestination = bookingData["sourceDestination"]?.ToString() ?? "";
                var parts = sourceDestination.Split('-');
                if (parts.Length == 2)
                {
                    lblRoute.Text = $"{parts[0].Trim()} → {parts[1].Trim()}";
                }
                else
                {
                    lblRoute.Text = sourceDestination;
                }

                // Journey Date
                string dateOfJourney = bookingData["dateOfJourney"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(dateOfJourney))
                {
                    lblJourneyDate.Text = Convert.ToDateTime(dateOfJourney).ToString("dd MMMM yyyy");
                }

                // Ticket Count
                lblTicketCount.Text = bookingData["ticketCount"]?.ToString() ?? "0";

                // Amount
                decimal unitPrice = bookingData["unitPrice"]?.Value<decimal>() ?? 0;
                decimal subTotal = bookingData["subTotal"]?.Value<decimal>() ?? 0;
                lblUnitPrice.Text = $"CDF {unitPrice:N0}";
                lblSubTotal.Text = $"CDF {subTotal:N0}";

                // Status
                lblStatus.Text = bookingData["status"]?.ToString() ?? "N/A";
                lblTransactionStatus.Text = bookingData["transactionStatus"]?.ToString() ?? "N/A";
                lblpostbalance.Text = bookingData["postBalance"]?.ToString() ?? "N/A";
                lblpostbalance2.Text = bookingData["postBalance2"]?.ToString() ?? "N/A";

                lblTransactionId.Text = bookingData["transactionId"]?.ToString() ?? "N/A";

              
                decimal postBalance1 = 0;
                decimal postBalance2 = 0;

                string pb1 = bookingData["postBalance"]?.ToString() ?? "0";
                string pb2 = bookingData["postBalance2"]?.ToString() ?? "0";

                if (decimal.TryParse(pb1, out decimal pb1Value))
                {
                    postBalance1 = pb1Value;
                }

                if (decimal.TryParse(pb2, out decimal pb2Value))
                {
                    postBalance2 = pb2Value;
                }

                // Extract refund amount from label (remove "CDF " and parse)
                decimal totalRefund = 0;
                string refundText = lblrefundamount.Text.Replace("CDF ", "").Replace(",", "").Trim();
                if (decimal.TryParse(refundText, out decimal refundValue))
                {
                    totalRefund = refundValue;
                }

                // Calculate: Total amount + post_balance1 + post_balance2 - total refund
                decimal overallAmount = subTotal + postBalance1 + postBalance2 - totalRefund;

                lbloverallamount.Text = $"CDF {overallAmount:N0}";

                System.Diagnostics.Debug.WriteLine($"Overall Amount Calculation:");
                System.Diagnostics.Debug.WriteLine($"  Total Amount: CDF {subTotal:N0}");
                System.Diagnostics.Debug.WriteLine($"  Post Balance 1: CDF {postBalance1:N0}");
                System.Diagnostics.Debug.WriteLine($"  Post Balance 2: CDF {postBalance2:N0}");
                System.Diagnostics.Debug.WriteLine($"  Total Refund: CDF {totalRefund:N0}");
                System.Diagnostics.Debug.WriteLine($"  Overall Amount: CDF {overallAmount:N0}");

                // ✅ NOTE: lblrefundamount is already set by LoadCancellationDetails before this method is called
                System.Diagnostics.Debug.WriteLine($"PopulateBookingDetails: Refund amount label already set to: {lblrefundamount.Text}");

                // Dates
                string createdAt = bookingData["createdAt"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(createdAt))
                {
                    lblBookingDate.Text = Convert.ToDateTime(createdAt).ToString("dd MMM yyyy hh:mm tt");
                }

                string updatedAt = bookingData["updatedAt"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(updatedAt))
                {
                    lblLastUpdated.Text = Convert.ToDateTime(updatedAt).ToString("dd MMM yyyy hh:mm tt");
                }

                // Check-in Status
                lblCheckin.Text = bookingData["checkin"]?.ToString() == "yes" ? "Checked In" : "Not Checked In";

                // Display passengers
                if (bookingData["passengers"] != null)
                {
                    JArray passengers = bookingData["passengers"] as JArray;
                    rptPassengers.DataSource = passengers;
                    rptPassengers.DataBind();
                }

                System.Diagnostics.Debug.WriteLine("Booking details populated successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error displaying booking: " + ex.Message);
            }
        }

        protected void btnDownloadTicket_Click(object sender, EventArgs e)
        {
            string pnr = Request.QueryString["pnr"];
            Response.Redirect($"~/DownloadTicket.aspx?pnr={pnr}");
        }

        protected void btnBackToBookings_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyBookings.aspx");
        }

        protected void btnCancelBooking_Click(object sender, EventArgs e)
        {
            string pnr = Request.QueryString["pnr"];
            if (!string.IsNullOrEmpty(pnr))
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (!string.IsNullOrEmpty(bookingJson))
                {
                    JObject bookingData = JObject.Parse(bookingJson);
                    string journeyDateString = bookingData["dateOfJourney"]?.ToString();
                    DateTime journeyDate;

                    if (DateTime.TryParse(journeyDateString, out journeyDate))
                    {
                        if (journeyDate >= DateTime.Today)
                        {
                            RegisterAsyncTask(new PageAsyncTask(() => CancelEntireBooking(pnr)));
                        }
                        else
                        {
                            ShowAlert("The ticket has already expired and cannot be cancelled.");
                        }
                    }
                    else
                    {
                        ShowAlert("Invalid journey date.");
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            string bookingJson = ViewState["CurrentBooking"]?.ToString();
            if (!string.IsNullOrEmpty(bookingJson))
            {
                JObject bookingData = JObject.Parse(bookingJson);
                JArray passengers = bookingData["passengers"] as JArray;

                if (passengers != null && item.ItemIndex < passengers.Count)
                {
                    JObject passenger = passengers[item.ItemIndex] as JObject;
                    string passengerId = passenger["passengerId"]?.ToString();
                    string seatNumber = passenger["seatNumber"]?.ToString();
                    string passengerName = passenger["name"]?.ToString();
                    string journeyDateString = bookingData["dateOfJourney"]?.ToString();
                    DateTime journeyDate;

                    if (DateTime.TryParse(journeyDateString, out journeyDate))
                    {
                        if (journeyDate >= DateTime.Today)
                        {
                            string pnr = Request.QueryString["pnr"];
                            if (!string.IsNullOrEmpty(passengerId) && !string.IsNullOrEmpty(pnr))
                            {
                                RegisterAsyncTask(new PageAsyncTask(() =>
                                    CancelPassengerTicket(pnr, passengerId, seatNumber, passengerName)));
                            }
                        }
                        else
                        {
                            ShowAlert("This ticket has already expired and cannot be cancelled.");
                        }
                    }
                    else
                    {
                        ShowAlert("Invalid journey date.");
                    }
                }
            }
        }

        private async Task CancelEntireBooking(string pnrNumber)
        {
            try
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (string.IsNullOrEmpty(bookingJson))
                {
                    ShowAlert("Booking data not found. Please refresh the page.");
                    return;
                }

                JObject bookingData = JObject.Parse(bookingJson);
                string bookedId = bookingData["bookedId"]?.ToString();
                string tripId = bookingData["tripId"]?.ToString();

                JArray passengers = bookingData["passengers"] as JArray;
                List<string> seatNumbers = new List<string>();

                if (passengers != null)
                {
                    foreach (JObject passenger in passengers)
                    {
                        string seat = passenger["seatNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(seat))
                        {
                            seatNumbers.Add(seat);
                        }
                    }
                }

                if (string.IsNullOrEmpty(bookedId))
                {
                    ShowAlert("Invalid booking ID.");
                    return;
                }

                DateTime today = DateTime.Today;

                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/CancelEntireBooking";
                System.Diagnostics.Debug.WriteLine($"Cancelling entire booking: {apiEndpoint}");

                var cancellationData = new
                {
                    pnrNumber = pnrNumber,
                    bookedId = bookedId,
                    tripId = tripId,
                    name = passengers.Count > 0 ? passengers[0]["name"]?.ToString() : "",
                    seatNumbers = seatNumbers,
                    cancellationType = "FULL",
                    cancellationReason = "User requested full booking cancellation",
                    userId = Session["UserId"]?.ToString(),
                    refundAmount = bookingData["subTotal"]?.Value<decimal>() ?? 0,
                    cancellationDate = today.ToString("yyyy-MM-dd")
                };

                string jsonContent = JsonConvert.SerializeObject(cancellationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseContent);
                    bool success = result["success"]?.Value<bool>() ?? false;
                    string message = result["message"]?.ToString() ?? "Booking cancelled successfully.";

                    if (success)
                    {
                        decimal totalRefundAmount = 0;
                        decimal totalOriginalAmount = 0;

                        if (result["data"] != null)
                        {
                            totalRefundAmount = result["data"]["totalRefundAmount"]?.Value<decimal>() ?? 0;
                            totalOriginalAmount = result["data"]["originalTotalAmount"]?.Value<decimal>() ?? 0;
                        }

                        // Store cancellation data for each passenger
                        foreach (JObject passenger in passengers)
                        {
                            passenger["isActive"] = false;

                            // ✅ FIXED: Calculate dynamic refund per passenger (not static 90%)
                            // Total refund from API is already dynamically calculated based on journey date
                            // Divide it equally among all passengers
                            decimal passengerRefundAmount = totalRefundAmount / passengers.Count;

                            var cancellationRecord = new
                            {
                                Passengerid = passenger["passengerId"]?.ToString(),
                                Seatnumber = passenger["seatNumber"]?.ToString(),
                                Amount = passengerRefundAmount, // ✅ DYNAMIC refund (varies by days until journey)
                                BookedId = cancellationData.bookedId,
                                name = passenger["name"]?.ToString(),
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                Isactive = false
                            };

                            string postEndpoint = $"{apiBaseUrl}TempPassengerTables/PostTempPassengerTable";
                            string postJsonContent = JsonConvert.SerializeObject(cancellationRecord);
                            var postContent = new StringContent(postJsonContent, Encoding.UTF8, "application/json");

                            HttpResponseMessage postResponse = await httpClient.PostAsync(postEndpoint, postContent);

                            if (postResponse.IsSuccessStatusCode)
                            {
                                System.Diagnostics.Debug.WriteLine($"Passenger {passenger["name"]} cancellation stored with dynamic refund: CDF {passengerRefundAmount:N0}");
                            }
                        }

                        // Show success and redirect
                        ShowAlertAndRedirect(
                            $"Booking cancelled successfully!\\n\\nTotal Refund Amount: CDF {totalRefundAmount:N0}",
                            "MyBookings.aspx");
                    }
                    else
                    {
                        ShowAlert(message);
                    }
                }
                else
                {
                    ShowAlert($"Failed to cancel booking. Status: {response.StatusCode}\\nError: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cancelling booking: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private async Task CancelPassengerTicket(string pnrNumber, string passengerId, string seatNumber, string passengerName)
        {
            try
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (string.IsNullOrEmpty(bookingJson))
                {
                    ShowAlert("Booking data not found. Please refresh the page.");
                    return;
                }

                JObject bookingData = JObject.Parse(bookingJson);
                string bookedId = bookingData["bookedId"]?.ToString();
                string tripId = bookingData["tripId"]?.ToString();
                decimal unitPrice = bookingData["unitPrice"]?.Value<decimal>() ?? 0;

                if (string.IsNullOrEmpty(bookedId))
                {
                    ShowAlert("Invalid booking ID.");
                    return;
                }

                JArray passengers = bookingData["passengers"] as JArray;
                int totalPassengers = passengers?.Count ?? 0;
                int activePassengers = 0;

                if (passengers != null)
                {
                    foreach (JObject p in passengers)
                    {
                        bool isActive = p["isActive"]?.Value<bool>() ?? true;
                        if (isActive) activePassengers++;
                    }
                }

                if (activePassengers <= 1)
                {
                    ShowConfirmAndExecute(
                        "This is the last passenger. Cancelling will cancel the entire booking. Do you want to continue?",
                        () => CancelEntireBooking(pnrNumber));
                    return;
                }

                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/CancelPassengerTicket";
                System.Diagnostics.Debug.WriteLine($"Cancelling passenger ticket: {apiEndpoint}");

                var cancellationData = new
                {
                    pnrNumber = pnrNumber,
                    bookedId = bookedId,
                    tripId = tripId,
                    passengerId = passengerId,
                    seatNumber = seatNumber,
                    passengerName = passengerName,
                    cancellationType = "PARTIAL",
                    cancellationReason = "Individual passenger ticket cancelled by user",
                    userId = Session["UserId"]?.ToString(),
                    refundAmount = unitPrice
                };

                string jsonContent = JsonConvert.SerializeObject(cancellationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseContent);
                    bool success = result["success"]?.Value<bool>() ?? false;

                    if (success)
                    {
                        // Get refund amount from API response
                        decimal refundAmount = 0;
                        if (result["data"] != null)
                        {
                            refundAmount = result["data"]["refundAmount"]?.Value<decimal>() ?? 0;
                        }

                        // Store cancellation record
                        var cancellationRecord = new
                        {
                            Passengerid = int.Parse(passengerId),
                            Seatnumber = seatNumber,
                            Amount = refundAmount, // Store actual refund amount
                            BookedId = bookedId,
                            Name = passengerName,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Isactive = false
                        };

                        string postEndpoint = $"{apiBaseUrl}TempPassengerTables/PostTempPassengerTable";
                        string postJsonContent = JsonConvert.SerializeObject(cancellationRecord);
                        var postContent = new StringContent(postJsonContent, Encoding.UTF8, "application/json");

                        await httpClient.PostAsync(postEndpoint, postContent);

                        // ✅ CRITICAL FIX: Reload the entire page data to refresh all cancellation information
                        int userId = Convert.ToInt32(Session["UserId"]);
                        await LoadBookingData(userId, pnrNumber);

                        // Show success message WITHOUT redirect
                        ShowAlert($"Ticket cancelled successfully!\\n\\nPassenger: {passengerName}\\nRefund Amount: CDF {refundAmount:N0}");
                    }
                    else
                    {
                        string message = result["message"]?.ToString() ?? "Failed to cancel ticket.";
                        ShowAlert(message);
                    }
                }
                else
                {
                    ShowAlert($"Failed to cancel ticket. Status: {response.StatusCode}\\nError: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cancelling passenger: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        private void ShowAlertAndRedirect(string message, string redirectUrl)
        {
            string script = $"alert('{message.Replace("'", "\\'")}'); window.location.href='{redirectUrl}';";
            ScriptManager.RegisterStartupScript(this, GetType(), "alertRedirect", script, true);
        }

        private void ShowConfirmAndExecute(string message, Func<Task> action)
        {
            string script = $@"
                if(confirm('{message.Replace("'", "\\'")}')) {{
                    __doPostBack('{btnCancelBooking.UniqueID}', '');
                }}";
            ScriptManager.RegisterStartupScript(this, GetType(), "confirmAction", script, true);
        }

        protected void btnCancelSelected_Click(object sender, EventArgs e)
        {
            string[] selectedPassengers = Request.Form.GetValues("selectedPassengers");

            if (selectedPassengers == null || selectedPassengers.Length == 0)
            {
                ShowAlert("Please select at least one passenger to cancel.");
                return;
            }

            string pnr = Request.QueryString["pnr"];
            if (!string.IsNullOrEmpty(pnr))
            {
                RegisterAsyncTask(new PageAsyncTask(() =>
                    CancelMultiplePassengers(pnr, selectedPassengers)));
            }
        }

        private async Task CancelMultiplePassengers(string pnrNumber, string[] passengerIds)
        {
            try
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (string.IsNullOrEmpty(bookingJson))
                {
                    ShowAlert("Booking data not found. Please refresh the page.");
                    return;
                }

                JObject bookingData = JObject.Parse(bookingJson);
                string bookedId = bookingData["bookedId"]?.ToString();
                string tripId = bookingData["tripId"]?.ToString();
                decimal unitPrice = bookingData["unitPrice"]?.Value<decimal>() ?? 0;

                JArray passengers = bookingData["passengers"] as JArray;
                List<object> passengersToCancel = new List<object>();
                List<string> seatNumbers = new List<string>();
                List<string> passengerNames = new List<string>();
                int activeCount = 0;

                if (passengers != null)
                {
                    foreach (JObject passenger in passengers)
                    {
                        bool isActive = passenger["isActive"]?.Value<bool>() ?? true;
                        if (isActive) activeCount++;

                        string passengerId = passenger["passengerId"]?.ToString();
                        if (passengerIds.Contains(passengerId))
                        {
                            passengersToCancel.Add(new
                            {
                                passengerId = passengerId,
                                name = passenger["name"]?.ToString(),
                                seatNumber = passenger["seatNumber"]?.ToString()
                            });

                            seatNumbers.Add(passenger["seatNumber"]?.ToString());
                            passengerNames.Add(passenger["name"]?.ToString());
                        }
                    }
                }

                if (passengersToCancel.Count >= activeCount)
                {
                    await CancelEntireBooking(pnrNumber);
                    return;
                }

                if (string.IsNullOrEmpty(bookedId))
                {
                    ShowAlert("Invalid booking ID.");
                    return;
                }

                string apiEndpoint = $"{apiBaseUrl}BookedTicketsNew/CancelMultiplePassengers";
                System.Diagnostics.Debug.WriteLine($"Cancelling multiple passengers: {apiEndpoint}");

                decimal refundAmount = unitPrice * passengersToCancel.Count;

                var cancellationData = new
                {
                    pnrNumber = pnrNumber,
                    bookedId = bookedId,
                    tripId = tripId,
                    passengers = passengersToCancel,
                    seatNumbers = seatNumbers,
                    cancellationType = "PARTIAL_MULTIPLE",
                    cancellationReason = $"Multiple passenger tickets cancelled by user",
                    cancelledBy = Session["UserId"]?.ToString(),
                    refundAmount = refundAmount,
                    cancelledCount = passengersToCancel.Count
                };

                string jsonContent = JsonConvert.SerializeObject(cancellationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseContent);
                    bool success = result["success"]?.Value<bool>() ?? false;
                    string message = result["message"]?.ToString() ??
                        $"{passengersToCancel.Count} tickets cancelled successfully.";

                    if (success)
                    {
                        int userId = Convert.ToInt32(Session["UserId"]);
                        await LoadBookingData(userId, pnrNumber);

                        ShowAlert(
                            $"Tickets cancelled successfully!\\n\\n" +
                            $"Passengers: {string.Join(", ", passengerNames)}\\n" +
                            $"Seats Released: {string.Join(", ", seatNumbers)}\\n" +
                            $"Total Tickets Cancelled: {passengersToCancel.Count}\\n" +
                            $"Refund Amount: CDF {refundAmount:N0}\\n\\n" +
                            $"These seats are now available for booking.");
                    }
                    else
                    {
                        ShowAlert(message);
                    }
                }
                else
                {
                    ShowAlert($"Failed to cancel tickets. Status: {response.StatusCode}\\nError: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cancelling multiple passengers: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private class TempPassengerTable
        {
            public int Passengerid { get; set; }
            public string Seatnumber { get; set; }
            public decimal Amount { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool Isactive { get; set; }
        }
    }
}