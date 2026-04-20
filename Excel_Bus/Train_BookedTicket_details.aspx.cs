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
using System.Web;

namespace Excel_Bus
{
    public partial class Train_BookedTicket_details : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        string apiBaseUrl = ConfigurationSettings.AppSettings["api_path"];

       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pnr = Request.QueryString["pnr"];
                if (string.IsNullOrEmpty(pnr)) { Response.Redirect("Train_MyBookings.aspx"); return; }
                if (Session["UserId"] == null) { Response.Redirect("~/TrainTicket.aspx"); return; }

                string userId = Session["UserId"].ToString();
                RegisterAsyncTask(new PageAsyncTask(() => LoadBookingData(userId, pnr)));
            }
        }

        private async Task LoadBookingData(string userId, string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray bookings = JArray.Parse(jsonResponse);

                    JObject matchingBooking = bookings
                        .OfType<JObject>()
                        .FirstOrDefault(b =>
                            (b["pnrNumber"]?.ToString() ?? "")
                            .Equals(pnrNumber, StringComparison.OrdinalIgnoreCase));

                    if (matchingBooking != null)
                    {
                        ViewState["CurrentBooking"] = matchingBooking.ToString();

                        string bookedId = matchingBooking["bookingId"]?.ToString();

                        
                        if (!string.IsNullOrEmpty(bookedId))
                            await LoadCancellationDetails(Convert.ToInt32(bookedId));

                       
                        PopulateBookingDetails(matchingBooking);
                    }
                    else
                    {
                        ShowAlert("Train booking not found.");
                        Response.Redirect("Train_MyBookings.aspx");
                    }
                }
                else
                {
                    ShowAlert("Failed to load train booking details.");
                    Response.Redirect("Train_MyBookings.aspx");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading train booking: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }
     
        private async Task LoadCancellationDetails(int bookingId)
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}TempTrainPassengers/GetTempTrainPassengerByBookedId?bookingId={bookingId}";
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    jsonResponse = jsonResponse.Trim();

                    JObject wrapper = JObject.Parse(jsonResponse);
                    JArray passengersArray = wrapper["passengers"] as JArray;
                    JArray transactions = wrapper["transactions"] as JArray;

                    var postponeTxns = transactions?
                        .OfType<JObject>()
                        .Where(t => (t["trxTypeStatus"]?.ToString() ?? "")
                            .Equals("Postpone", StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(t => t["trxId"]?.Value<int>() ?? 0)
                        .ToList() ?? new List<JObject>();

                    decimal pb1 = 0, pb2 = 0;

                    if (passengersArray != null)
                    {
                        var postponePassengers = passengersArray
                            .OfType<JObject>()
                            .Where(p => (p["trxTypeStatus"]?.ToString() ?? "")
                                .Equals("Postpone", StringComparison.OrdinalIgnoreCase))
                            .OrderByDescending(p => p["createdAt"]?.ToString())
                            .ToList();

                        if (postponePassengers.Count >= 1)
                            pb1 = postponePassengers[0]["amount"]?.Value<decimal>() ?? 0;
                        if (postponePassengers.Count >= 2)
                            pb2 = postponePassengers[1]["amount"]?.Value<decimal>() ?? 0;
                    }

                    if (pb1 == 0 && postponeTxns.Count >= 1)
                    {
                        ViewState["PostponeTxnCount"] = postponeTxns.Count;
                    }

                    ViewState["PostponeAmt1"] = pb1;
                    ViewState["PostponeAmt2"] = pb2;

                    List<JObject> allCancellations = new List<JObject>();

                    if (passengersArray != null)
                    {
                        allCancellations = passengersArray
                            .OfType<JObject>()
                            .Where(p => p["isActive"]?.Value<bool>() == false
                                     && (p["trxTypeStatus"]?.ToString() ?? "")
                                        .Equals("Refund", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (allCancellations.Count == 0)
                        {
                            allCancellations = passengersArray
                                .OfType<JObject>()
                                .Where(p => p["isActive"]?.Value<bool>() == false)
                                .ToList();
                        }
                    }

                    if (allCancellations.Count > 0)
                    {
                        decimal totalRefund = allCancellations
                            .Sum(c => c["amount"]?.Value<decimal>() ?? 0);

                        JObject latestRefundTxn = transactions?
                            .OfType<JObject>()
                            .FirstOrDefault(t => (t["trxTypeStatus"]?.ToString() ?? "")
                                .Equals("Refund", StringComparison.OrdinalIgnoreCase));

                        lblTransactionStatus.Text = latestRefundTxn?["trxTypeStatus"]?.ToString() ?? "N/A";
                        lblTransactionId.Text = latestRefundTxn?["trxNo"]?.ToString() ?? "N/A";
                        lblCancelledCount.Text = allCancellations.Count.ToString();
                        lblTotalRefund.Text = $"CDF {totalRefund:N0}";
                        lblrefundamount.Text = $"CDF {totalRefund:N0}";

                        var normalizedList = allCancellations.Select(c => new
                        {
                            passengerName = c["passengerName"]?.ToString() ?? "N/A",
                            seatNumber = c["seatNumber"]?.ToString() ?? "—",
                            Amount = c["amount"]?.Value<decimal>() ?? 0,
                            CreatedAt = c["createdAt"]?.ToString() ?? "",
                            trxTypeStatus = c["trxTypeStatus"]?.ToString() ?? "",
                            trxNo = c["trxNo"]?.ToString() ?? ""
                        }).ToList();

                        rptCancellations.DataSource = normalizedList;
                        rptCancellations.DataBind();

                        divCancellationDetails.Visible = totalRefund > 0;
                    }
                    else
                    {
                        lblrefundamount.Text = "CDF 0";
                        divCancellationDetails.Visible = false;
                    }
                }
                else
                {
                    lblrefundamount.Text = "CDF 0";
                    divCancellationDetails.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblrefundamount.Text = "CDF 0";
                divCancellationDetails.Visible = false;
                System.Diagnostics.Debug.WriteLine("Error loading cancellation details: " + ex.Message);
            }
        }
        private void PopulateBookingDetails(JObject b)
        {
            try
            {
                lblPNRNumber.Text = b["pnrNumber"]?.ToString() ?? "—";
                lblBookedId.Text = b["bookingId"]?.ToString() ?? "—";

                string trainName = b["trainName"]?.ToString() ?? "";
                string trainNumber = b["trainNumber"]?.ToString() ?? "";
                lblTrainName.Text = !string.IsNullOrEmpty(trainNumber)
                                     ? $"{trainName} ({trainNumber})" : trainName;

                string fromStation = b["fromStation"]?.ToString() ?? "—";
                string toStation = b["toStation"]?.ToString() ?? "—";
                lblFromStation.Text = fromStation;
                lblToStation.Text = toStation;
                lblRoute.Text = $"{fromStation} → {toStation}";
                lblFromCode.Text = fromStation.Split(' ')[0].ToUpper();
                lblToCode.Text = toStation.Split(' ')[0].ToUpper();

                lblDepartureTime.Text = "—";
                lblArrivalTime.Text = "—";
                lblDuration.Text = "—";

                lblJourneyDate.Text = FormatDate(b["journeyDate"]?.ToString() ?? "");
                lblClass.Text = b["coachType"]?.ToString() ?? "N/A";

                //string passengerCount = b["passengerCount"]?.ToString() ?? "";
                //if (string.IsNullOrEmpty(passengerCount) || passengerCount == "0")
                //    passengerCount = (b["passengers"] as JArray)?.Count.ToString() ?? "1";
                //lblTicketCount.Text = passengerCount;
                int activeCount = 0;
                if (b["passengers"] is JArray allPassengers)
                    activeCount = allPassengers.Count(p => p["isActive"]?.Value<bool>() ?? true);

                lblTicketCount.Text = activeCount > 0 ? activeCount.ToString()
                                      : (b["passengerCount"]?.ToString() ?? "1");

                lblSeats.Text = GetSeats(b);

                decimal unitPrice = b["unitPrice"]?.Value<decimal>() ?? 0;
                decimal totalAmt = b["totalAmount"]?.Value<decimal>() ?? 0;
                lblUnitPrice.Text = $"CDF {unitPrice:N0}";
                lblSubTotal.Text = $"CDF {totalAmt:N0}";

              
                if (string.IsNullOrEmpty(lblTransactionId.Text) || lblTransactionId.Text == "N/A")
                    lblTransactionId.Text = b["trxNo"]?.ToString() ?? "N/A";

                if (string.IsNullOrEmpty(lblTransactionStatus.Text) || lblTransactionStatus.Text == "N/A")
                    lblTransactionStatus.Text = b["paymentStatus"]?.ToString() ?? "N/A";

                decimal pb1 = b["postponeAmt1"]?.Value<decimal>() ?? 0;
                decimal pb2 = b["postponeAmt2"]?.Value<decimal>() ?? 0;

                
                if (pb1 == 0 && b["trainTransactions"] is JArray txns && txns.Count > 0)
                {
                    var postponeTxns = txns
                        .Where(t => (t["trxTypeStatus"]?.ToString() ?? "").ToLower().Contains("postpone"))
                        .OrderByDescending(t => t["transactionId"]?.ToString())
                        .ToList();

                    if (postponeTxns.Count >= 1)
                        pb1 = postponeTxns[0]["amount"]?.Value<decimal>() ?? 0;
                    if (postponeTxns.Count >= 2)
                        pb2 = postponeTxns[1]["amount"]?.Value<decimal>() ?? 0;
                }

                lblpostbalance.Text = pb1 > 0 ? $"CDF {pb1:N0}" : "N/A";
                lblpostbalance2.Text = pb2 > 0 ? $"CDF {pb2:N0}" : "N/A";


                decimal totalRefund = 0;
                string refundText = (lblrefundamount.Text ?? "").Replace("CDF", "").Replace(",", "").Trim();
                if (!string.IsNullOrEmpty(refundText))
                    decimal.TryParse(refundText, System.Globalization.NumberStyles.Any,
                                     System.Globalization.CultureInfo.InvariantCulture, out totalRefund);

                decimal bookingAmount = b["amount"]?.Value<decimal>() ?? 0;
                if (bookingAmount > 0)
                {
                    totalRefund += bookingAmount;
                    lblrefundamount.Text = $"CDF {totalRefund:N0}";
                }
                else if (totalRefund == 0)
                {
                    lblrefundamount.Text = "CDF 0";
                }


                lbloverallamount.Text = $"CDF {(totalAmt + pb1 + pb2 - totalRefund):N0}";

                string rawStatus = b["Status"]?.ToString() ?? b["bookingStatus"]?.ToString() ?? "";
                string normStatus = NormaliseStatus(rawStatus);
                //lblStatus.Text = string.IsNullOrEmpty(rawStatus) ? "Unknown"
                //                     : char.ToUpper(rawStatus[0]) + rawStatus.Substring(1).ToLower();
                //lblStatus.CssClass = $"status-text status-{normStatus}";

                string checkin = (b["checkinStatus"]?.ToString() ?? "").ToLower();
                lblCheckin.Text = checkin == "yes" || checkin == "checked-in" || checkin == "checked_in"
                    ? "<span style='color:#4CAF50;'><i class='fas fa-check-circle'></i> Checked In</span>"
                    : "<span style='color:#888;'><i class='fas fa-clock'></i> Not Checked In</span>";

                
                string bookingDateRaw = b["bookingDate"]?.ToString() ?? b["createdAt"]?.ToString() ?? "";
                lblBookingDate.Text = FormatDate(bookingDateRaw);

                string updatedAt = b["updatedAt"]?.ToString() ?? "";
                lblLastUpdated.Text = DateTime.TryParse(updatedAt, out DateTime upd)
                    ? upd.ToString("dd MMM yyyy hh:mm tt") : updatedAt;

                if (b["passengers"] is JArray passengers && passengers.Count > 0)
                {
                    string coachType = b["coachType"]?.ToString() ?? "";
                    string coachNumber = "";
                    string pnr = b["pnrNumber"]?.ToString() ?? "";

                    try
                    {
                        var appCoachMap = HttpContext.Current.Application["PNRCoachMap"]
                                          as Dictionary<string, string>;
                        if (appCoachMap != null && appCoachMap.ContainsKey(pnr))
                            coachNumber = appCoachMap[pnr];
                    }
                    catch (Exception mapEx)
                    {
                        System.Diagnostics.Debug.WriteLine("CoachMap error: " + mapEx.Message);
                    }

                    foreach (JObject p in passengers)
                    {
                        p["coachType"] = coachType;
                        p["coachNumber"] = !string.IsNullOrEmpty(coachNumber) ? coachNumber : coachType;
                    }

                    rptPassengers.DataSource = passengers;
                    rptPassengers.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error displaying train booking: " + ex.Message);
            }
        }
        protected string GetSeats(object item)
        {
            JObject b = (JObject)item;
            if (b["passengers"] is JArray passengers && passengers.Count > 0)
            {
                var seats = passengers
                    .Select(p => p["seatNumber"]?.ToString())
                    .Where(s => !string.IsNullOrEmpty(s));
                string result = string.Join(", ", seats);
                return string.IsNullOrEmpty(result) ? "—" : result;
            }
            return "—";
        }

        protected void btnDownloadTicket_Click(object sender, EventArgs e)
        {
            string pnr = Request.QueryString["pnr"];
            Response.Redirect($"~/Train_Ticket_Download.aspx?pnr={pnr}");
        }

        protected void btnBackToBookings_Click(object sender, EventArgs e)
        {
            Response.Redirect("Train_MyBookings.aspx");
        }

        protected void btnCancelBooking_Click(object sender, EventArgs e)
        {
            string pnr = Request.QueryString["pnr"];
            if (string.IsNullOrEmpty(pnr)) return;

            string bookingJson = ViewState["CurrentBooking"]?.ToString();
            if (string.IsNullOrEmpty(bookingJson)) return;

            JObject bookingData = JObject.Parse(bookingJson);
            string journeyDateString = bookingData["journeyDate"]?.ToString();

            if (DateTime.TryParse(journeyDateString, out DateTime journeyDate))
            {
                if (journeyDate >= DateTime.Today)
                    RegisterAsyncTask(new PageAsyncTask(() => CancelEntireBooking(pnr)));
                else
                    ShowAlert("The train ticket has already expired and cannot be cancelled.");
            }
            else
            {
                ShowAlert("Invalid journey date.");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            string bookingJson = ViewState["CurrentBooking"]?.ToString();
            if (string.IsNullOrEmpty(bookingJson)) { ShowAlert("Booking data not found. Please refresh."); return; }

            JObject bookingData = JObject.Parse(bookingJson);
            JArray passengers = bookingData["passengers"] as JArray;
            if (passengers == null || item.ItemIndex >= passengers.Count) return;

            JObject passenger = (JObject)passengers[item.ItemIndex];

            // ✅ FIX: isActive check karo pehle
            bool isActive = passenger["isActive"]?.Value<bool>() ?? true;
            if (!isActive) { ShowAlert("This passenger ticket is already cancelled."); return; }

            string passengerId = passenger["passengerId"]?.ToString();
            string seatNumber = passenger["seatNumber"]?.ToString();
            string passengerName = passenger["name"]?.ToString();

            if (string.IsNullOrEmpty(passengerId))
            {
                ShowAlert("Passenger ID not found."); return;
            }

            string journeyDateStr = bookingData["journeyDate"]?.ToString();
            if (!DateTime.TryParse(journeyDateStr, out DateTime journeyDate))
            {
                ShowAlert("Invalid journey date."); return;
            }
            if (journeyDate < DateTime.Today)
            {
                ShowAlert("This ticket has already expired."); return;
            }

            string pnr = Request.QueryString["pnr"];
            RegisterAsyncTask(new PageAsyncTask(() =>
                CancelPassengerTicket(pnr, passengerId, seatNumber, passengerName)));
        }

        private async Task CancelEntireBooking(string pnrNumber)
        {
            try
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (string.IsNullOrEmpty(bookingJson)) { ShowAlert("Booking data not found."); return; }

                JObject bookingData = JObject.Parse(bookingJson);

                string bookedId = bookingData["bookingId"]?.ToString();
                string tripId = bookingData["tripId"]?.ToString();
                JArray passengers = bookingData["passengers"] as JArray;
                var seatNumbers = new List<string>();

                if (passengers != null)
                    foreach (JObject p in passengers)
                    {
                        string seat = p["seatNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(seat)) seatNumbers.Add(seat);
                    }

                if (string.IsNullOrEmpty(bookedId)) { ShowAlert("Invalid booking ID."); return; }

                decimal refundAmount = bookingData["totalAmount"]?.Value<decimal>() ?? 0;

                var cancellationData = new
                {
                    pnrNumber = pnrNumber,
                    bookedId = bookedId,
                    tripId = tripId,
                    name = passengers?.Count > 0 ? passengers[0]["name"]?.ToString() : "",
                    seatNumbers = seatNumbers,
                    cancellationType = "FULL",
                    cancellationReason = "User requested full train booking cancellation",
                    userId = Session["UserId"]?.ToString(),
                    refundAmount = refundAmount,
                    cancellationDate = DateTime.Today.ToString("yyyy-MM-dd")
                };

                string jsonContent = JsonConvert.SerializeObject(cancellationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await httpClient.PostAsync(
                    $"{apiBaseUrl}TrainTicketBookings/CancelTrainEntireBooking", content);
                string responseContent = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseContent);
                    bool success = result["success"]?.Value<bool>() ?? false;

                    if (success)
                    {
                        decimal totalRefund = result["data"]?["totalRefundAmount"]?.Value<decimal>() ?? 0;

                        var cancelledPassengers = result["data"]?["cancelledPassengers"] as JArray;

                        if (cancelledPassengers != null && passengers != null)
                        {
                            var cancelledIds = cancelledPassengers
                                .Select(cp => cp["passengerId"]?.ToString())
                                .Where(id => !string.IsNullOrEmpty(id))
                                .ToHashSet();

                            int cancelledCount = cancelledPassengers.Count;

                            foreach (JObject p in passengers)
                            {
                                string passengerId = p["passengerId"]?.ToString();

                                if (!cancelledIds.Contains(passengerId)) continue;

                                bool isAlreadyInactive = !(p["isActive"]?.Value<bool>() ?? true);
                                if (isAlreadyInactive) continue;

                                p["isActive"] = false;

                                //var record = new
                                //{
                                //    Passengerid = passengerId,
                                //    Seatnumber = p["seatNumber"]?.ToString(),
                                //    Amount = cancelledCount > 0 ? totalRefund / cancelledCount : 0,
                                //    BookingId = bookedId,
                                //    PassengerName = p["name"]?.ToString(),
                                //    CreatedAt = DateTime.UtcNow,
                                //    UpdatedAt = DateTime.UtcNow,
                                //    Isactive = false
                                //};

                                // ✅ AFTER
                                var record = new
                                {
                                    passengerid = passengerId,
                                    seatnumber = p["seatNumber"]?.ToString(),
                                    amount = cancelledCount > 0 ? totalRefund / cancelledCount : 0,
                                    bookingId = bookedId,
                                    passengerName = p["name"]?.ToString(),   // ← matches Eval("passengerName")
                                    createdAt = DateTime.UtcNow,
                                    updatedAt = DateTime.UtcNow,
                                    isactive = false
                                };

                                await httpClient.PostAsync(
                                    $"{apiBaseUrl}TempTrainPassengers/PostTempTrainPassenger",
                                    new StringContent(JsonConvert.SerializeObject(record), Encoding.UTF8, "application/json"));
                            }
                        }

                        ShowAlertAndRedirect(
                            $"Train booking cancelled successfully!\\n\\nRefund Amount: CDF {totalRefund:N0}",
                            "Train_MyBookings.aspx");
                    }
                    else
                    {
                        ShowAlert(result["message"]?.ToString() ?? "Cancellation failed.");
                    }
                }
                else
                {
                    ShowAlert($"Failed to cancel booking. Status: {res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cancelling train booking: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }
      
        private async Task CancelPassengerTicket(
    string pnrNumber, string passengerId, string seatNumber, string passengerName)
        {
            try
            {
                string bookingJson = ViewState["CurrentBooking"]?.ToString();
                if (string.IsNullOrEmpty(bookingJson)) { ShowAlert("Booking data not found."); return; }

                JObject bookingData = JObject.Parse(bookingJson);

                string bookedId = bookingData["bookingId"]?.ToString();
                string tripId = bookingData["tripId"]?.ToString();
                decimal unitPrice = bookingData["unitPrice"]?.Value<decimal>() ?? 0;
                JArray passengers = bookingData["passengers"] as JArray;

                int activePassengers = passengers?
                    .Count(p => p["isActive"]?.Value<bool>() ?? true) ?? 0;

                if (activePassengers <= 1)
                {
                    await CancelEntireBooking(pnrNumber); 
                    return;
                }

               
                if (!int.TryParse(passengerId, out int passengerIdInt))
                {
                    ShowAlert("Invalid passenger ID."); return;
                }

                var cancellationData = new
                {
                    PassengerId = passengerIdInt,   
                    PnrNumber = pnrNumber,
                    BookedId = !string.IsNullOrEmpty(bookedId) ? int.Parse(bookedId) : 0,
                    TripId = tripId,
                    SeatNumber = seatNumber,
                    PassengerName = passengerName,
                    CancellationType = "PARTIAL",
                    CancellationReason = "Individual passenger train ticket cancelled by user",
                    UserId = Session["UserId"]?.ToString(),
                    RefundAmount = unitPrice
                };

                string jsonContent = JsonConvert.SerializeObject(cancellationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await httpClient.PostAsync(
                    $"{apiBaseUrl}TrainTicketBookings/CancelTrainPassengerTicket", content);
                string responseContent = await res.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("Cancel Passenger Response: " + responseContent); 

                if (res.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseContent);
                    bool success = result["success"]?.Value<bool>() ?? false;

                    if (success)
                    {
                        decimal refund = result["data"]?["refundAmount"]?.Value<decimal>() ?? 0;

                        //var record = new
                        //{
                        //    Passengerid = passengerIdInt,
                        //    SeatNumber = seatNumber,
                        //    Amount = refund,
                        //    BookingId = !string.IsNullOrEmpty(bookedId) ? int.Parse(bookedId) : 0,
                        //    PassengerName = passengerName,       
                        //    CreatedAt = DateTime.UtcNow,
                        //    UpdatedAt = DateTime.UtcNow,
                        //    IsActive = false
                        //};

                        // ✅ AFTER (in CancelPassengerTicket)
                        var record = new
                        {
                            passengerid = passengerIdInt,
                            seatNumber = seatNumber,           // ← matches Eval("Seatnumber")
                            amount = refund,
                            bookingId = !string.IsNullOrEmpty(bookedId) ? int.Parse(bookedId) : 0,
                            passengerName = passengerName,     // ← matches Eval("passengerName")
                            createdAt = DateTime.UtcNow,
                            updatedAt = DateTime.UtcNow,
                            isActive = false
                        };
                        await httpClient.PostAsync(
                            $"{apiBaseUrl}TempTrainPassengers/PostTempTrainPassenger",
                            new StringContent(JsonConvert.SerializeObject(record), Encoding.UTF8, "application/json"));

                        await LoadBookingData(Session["UserId"].ToString(), pnrNumber);
                        ShowAlert($"Ticket cancelled!\\n\\nPassenger: {passengerName}\\nRefund: CDF {refund:N0}");
                    }
                    else
                    {
                        string errMsg = result["message"]?.ToString() ?? "Failed to cancel ticket.";
                        ShowAlert("Error: " + errMsg);
                    }
                }
                else
                {
                    ShowAlert($"API Error {(int)res.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cancelling train passenger: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }

        private string NormaliseStatus(string raw)
        {
            string s = (raw ?? "").Trim().ToLower();
            if (s == "booked" || s == "confirmed") return "booked";
            if (s == "cancelled" || s == "canceled") return "cancelled";
            if (s == "completed" || s == "done") return "completed";
            if (s == "pending" || s == "awaiting") return "pending";
            return "default";
        }

        private string FormatDate(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "—";
            return DateTime.TryParse(raw, out DateTime d)
                ? d.ToString("dd MMM yyyy")
                : raw;
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
    }
}