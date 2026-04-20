using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class MyBookings : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx");
                    return;
                }

                // Load user bookings
                int userId = Convert.ToInt32(Session["UserId"]);
                RegisterAsyncTask(new PageAsyncTask(() => LoadUserBookings(userId)));
            }
        }

        private async Task LoadUserBookings(int userId)
        {
            try
            {
                string apiEndpoint = $"{apiUrl}BookedTicketsNew/GetBookedTicketsByUserId/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading bookings from: {apiEndpoint}");

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Bookings Response: {jsonResponse}");

                    JArray bookings = JArray.Parse(jsonResponse);

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

        protected string GetFormattedDate(object dateObj)
        {
            try
            {
                if (dateObj != null && !string.IsNullOrEmpty(dateObj.ToString()))
                {
                    DateTime date = Convert.ToDateTime(dateObj);
                    return date.ToString("dd MMM yyyy");
                }
            }
            catch { }
            return "N/A";
        }

        protected string GetFormattedAmount(object amountObj)
        {
            try
            {
                if (amountObj != null)
                {
                    decimal amount = Convert.ToDecimal(amountObj);
                    return $"CDF {amount:N0}";
                }
            }
            catch { }
            return "CDF 0";
        }

        protected string GetStatusClass(object statusObj)
        {
            string status = statusObj?.ToString()?.ToLower() ?? "";

            switch (status)
            {
                case "booked":
                    return "status-booked";
                case "cancelled":
                    return "status-cancelled";
                case "completed":
                    return "status-completed";
                case "pending":
                    return "status-pending";
                default:
                    return "status-default";
            }
        }

        protected string GetRoute(object sourceDestObj)
        {
            string sourceDest = sourceDestObj?.ToString() ?? "";
            if (!string.IsNullOrEmpty(sourceDest))
            {
                var parts = sourceDest.Split('-');
                if (parts.Length == 2)
                {
                    return $"{parts[0].Trim()} → {parts[1].Trim()}";
                }
            }
            return sourceDest;
        }

        protected string GetSeatNumbers(object passengersObj)
        {
            try
            {
                if (passengersObj != null)
                {
                    JArray passengers = JArray.Parse(passengersObj.ToString());
                    string seats = "";

                    foreach (JObject passenger in passengers)
                    {
                        string seat = passenger["seatNumber"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(seat))
                        {
                            seats += seat + ", ";
                        }
                    }

                    return seats.TrimEnd(',', ' ');
                }
            }
            catch { }
            return "N/A";
        }

        protected void btnViewDetails_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string pnr = btn.CommandArgument;
            Response.Redirect($"~/BookingDetails.aspx?pnr={pnr}");
        }

        protected void btnBookNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ticket.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}