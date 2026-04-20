using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class Train_MyBookings : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        // Stats exposed to the ASPX page
        protected int StatTotal = 0;
        protected int StatActive = 0;
        protected int StatUpcoming = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/TrainTicket.aspx");
                    return;
                }

                string userId = Session["UserId"].ToString();
                RegisterAsyncTask(new PageAsyncTask(() => LoadUserBookings(userId)));
            }
        }

        private async Task LoadUserBookings(string userId)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}TrainTicketBookings/GetBookedTrainTicketsByUserId/{userId}";
                //string apiEndpoint = $"{apiUrl}TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading bookings from: {apiEndpoint}");
             
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Bookings Response: {jsonResponse}");

                    JArray bookings = JArray.Parse(jsonResponse);

                    // Compute stats
                    StatTotal = bookings.Count;
                    foreach (JObject b in bookings)
                    {
                        string st = NormaliseStatus(b["status"]?.ToString());
                        if (st == "booked" || st == "pending") StatActive++;

                        string rawDate = b["journeyDate"]?.ToString() ?? b["travelDate"]?.ToString() ?? b["date"]?.ToString() ?? "";
                        DateTime travelDate;
                        if (DateTime.TryParse(rawDate, out travelDate) && travelDate > DateTime.Now && st != "cancelled")
                            StatUpcoming++;
                    }

                    if (bookings.Count > 0)
                    {
                        rptBookings.DataSource = bookings;
                        rptBookings.DataBind();

                        pnlNoBookings.Visible = false;
                        pnlBookings.Visible = true;
                    }
                    else
                    {
                        pnlNoBookings.Visible = true;
                        pnlBookings.Visible = false;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    pnlNoBookings.Visible = true;
                    pnlBookings.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading bookings: " + ex.Message);
                ShowAlert("Error loading bookings: " + ex.Message);
                pnlNoBookings.Visible = true;
                pnlBookings.Visible = false;
            }
        }

        // ── Helpers called from the ASPX Repeater ──────────────────────────

        protected string GetPnr(object item)
        {
            JObject b = (JObject)item;
            return b["pnrNumber"]?.ToString()
                ?? b["pnr"]?.ToString()
                ?? b["bookingId"]?.ToString()
                ?? "—";
        }

        protected string GetFrom(object item)
        {
            JObject b = (JObject)item;
            return b["fromStation"]?.ToString()
                ?? b["from"]?.ToString()
                ?? b["source"]?.ToString()
                ?? "—";
        }

        protected string GetTo(object item)
        {
            JObject b = (JObject)item;
            return b["toStation"]?.ToString()
                ?? b["to"]?.ToString()
                ?? b["destination"]?.ToString()
                ?? "—";
        }

        protected string GetJourneyDate(object item)
        {
            JObject b = (JObject)item;
            string raw = b["journeyDate"]?.ToString()
                      ?? b["travelDate"]?.ToString()
                      ?? b["date"]?.ToString()
                      ?? "";
            return FormatDate(raw);
        }

        protected string GetBookedOn(object item)
        {
            JObject b = (JObject)item;
            string raw = b["bookingDate"]?.ToString()
                      ?? b["createdAt"]?.ToString()
                      ?? b["bookedOn"]?.ToString()
                      ?? "";
            return FormatDate(raw);
        }

        protected string GetSeats(object item)
        {
            JObject b = (JObject)item;

            // Check passengers array
            if (b["passengers"] is JArray passengers && passengers.Count > 0)
            {
                var seats = passengers
                    .Select(p => p["seatNumber"]?.ToString())
                    .Where(s => !string.IsNullOrEmpty(s));

                string result = string.Join(", ", seats);
                return string.IsNullOrEmpty(result) ? "—" : result;
            }

            // Fallback: flat seatNumber (agar koi aur structure ho)
            string flatSeat = b["seatNumber"]?.ToString() ?? "";
            return string.IsNullOrEmpty(flatSeat) ? "—" : flatSeat;
        }

        protected string GetPassengerCount(object item)
        {
            JObject b = (JObject)item;
            string pc = b["passengerCount"]?.ToString()
                     ?? b["passengers"]?.ToString()
                     ?? "";

            if (!string.IsNullOrEmpty(pc)) return pc;

            // Derive from seats array length
            if (b["seats"] is JArray arr) return arr.Count.ToString();
            return "1";
        }

        protected string GetAmount(object item)
        {
            JObject b = (JObject)item;
            string raw = b["totalAmount"]?.ToString()
                      ?? b["amount"]?.ToString()
                      ?? b["fare"]?.ToString()
                      ?? "";

            if (string.IsNullOrEmpty(raw)) return "—";
            decimal n;
            if (decimal.TryParse(raw, out n))
                return "₹ " + n.ToString("N2");
            return raw;
        }

        protected string GetStatus(object item)
        {
            JObject b = (JObject)item;
            string raw = b["status"]?.ToString() ?? "";
            return string.IsNullOrEmpty(raw) ? "Unknown"
                : char.ToUpper(raw[0]) + raw.Substring(1).ToLower();
        }

        protected string GetStatusClass(object item)
        {
            JObject b = (JObject)item;
            return "status-" + NormaliseStatus(b["status"]?.ToString());
        }

        protected string GetCardStatusClass(object item)
        {
            JObject b = (JObject)item;
            string st = NormaliseStatus(b["status"]?.ToString());
            return $"booking-card {("status-" + st)}";
        }

        protected bool IsBooked(object item) => NormaliseStatus(((JObject)item)["status"]?.ToString()) == "booked";
        protected bool IsPending(object item) => NormaliseStatus(((JObject)item)["status"]?.ToString()) == "pending";
        protected bool IsCompleted(object item) => NormaliseStatus(((JObject)item)["status"]?.ToString()) == "postponed";

        // ── Private helpers ────────────────────────────────────────────────

        private string NormaliseStatus(string raw)
        {
            string s = (raw ?? "").Trim().ToLower();
            if (s == "booked" || s == "confirmed") return "booked";
            if (s == "cancelled" || s == "canceled") return "cancelled";
            if (s == "postponed" || s == "done") return "postponed";
            if (s == "pending" || s == "awaiting") return "pending";
            return "default";
        }

        private string FormatDate(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "—";
            DateTime d;
            if (DateTime.TryParse(raw, out d))
                return d.ToString("dd MMM yyyy");
            return raw;
        }

        // ── Button handlers ────────────────────────────────────────────────

        protected void btnViewDetails_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pnr = btn.CommandArgument;
            Response.Redirect($"~/Train_BookedTicket_details.aspx?pnr={pnr}");
            
        }

        protected void btnCancelBooking_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pnr = btn.CommandArgument;
            // TODO: call your cancellation API here, then reload
            RegisterAsyncTask(new PageAsyncTask(() => CancelBookingAsync(pnr)));
        }

        protected void btnDownloadTicket_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pnr = btn.CommandArgument;
            Response.Redirect($"~/Train_Ticket_Download.aspx?pnr={pnr}");
        }

        //protected void btnRateJourney_Click(object sender, EventArgs e)
        //{
        //    Button btn = (Button)sender;
        //    string pnr = btn.CommandArgument;
        //    Response.Redirect($"~/Train_RateJourney.aspx?pnr={pnr}");
        //}

        protected void btnBookNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ticket.aspx");
        }

        private async Task CancelBookingAsync(string pnr)
        {
            try
            {
                string endpoint = $"{apiUrl}BookedTicketsNew/Cancel/{pnr}";
                HttpResponseMessage res = await client.PostAsync(endpoint, null);
                if (!res.IsSuccessStatusCode)
                    ShowAlert($"Cancellation failed: {res.StatusCode}");
                else
                {
                    string userId = Session["UserId"].ToString();
                    await LoadUserBookings(userId);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Cancellation error: " + ex.Message);
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}