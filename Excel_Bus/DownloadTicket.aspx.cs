using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class DownloadTicket : System.Web.UI.Page
    {
        private static readonly HttpClient httpClient = new HttpClient();
        string apiBaseUrl = ConfigurationSettings.AppSettings["api_path"];
        string Base_Url = ConfigurationSettings.AppSettings["BaseUrl"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pnr = Request.QueryString["pnr"];

                if (string.IsNullOrEmpty(pnr))
                {
                    Response.Redirect("~/MyBookings.aspx");
                    return;
                }

                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/ticket.aspx");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                RegisterAsyncTask(new PageAsyncTask(() => LoadTicketData(userId, pnr)));
            }
        }

        private async Task LoadTicketData(int userId, string pnrNumber)
        {
            try
            {
                string apiEndpoint = $"{apiBaseUrl}BookedticketsNew/GetBookedTicketsByUserId/{userId}";
                System.Diagnostics.Debug.WriteLine($"Loading ticket details from: {apiEndpoint}");

                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
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
                        PopulateTicketInfo(matchingBooking);
                    }
                    else
                    {
                        ShowAlert("Ticket not found.");
                        Response.Redirect("~/MyBookings.aspx");
                    }
                }
                else
                {
                    ShowAlert("Failed to load ticket details.");
                    Response.Redirect("~/MyBookings.aspx");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading ticket: " + ex.Message);
                ShowAlert("Error: " + ex.Message);
            }
        }



        private void PopulateTicketInfo(JObject bookingData)
        {
            try
            {
                // Set basic ticket information
                lblPNRNumber.Text = bookingData["pnrNumber"]?.ToString() ?? "";
                lblTripTitle.Text = bookingData["tripTitle"]?.ToString() ?? "";

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

                string dateOfJourney = bookingData["dateOfJourney"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(dateOfJourney))
                {
                    lblJourneyDate.Text = Convert.ToDateTime(dateOfJourney).ToString("dd MMMM yyyy");
                }

                lblTicketCount.Text = bookingData["ticketCount"]?.ToString() ?? "0";

                decimal subTotal = bookingData["subTotal"]?.Value<decimal>() ?? 0;
                lblTotalAmount.Text = $"CDF {subTotal:N0}";

                lblStatus.Text = bookingData["status"]?.ToString() ?? "N/A";

                string createdAt = bookingData["createdAt"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(createdAt))
                {
                    lblBookingDate.Text = Convert.ToDateTime(createdAt).ToString("dd MMM yyyy hh:mm tt");
                }


                string qrCodePath = bookingData["qrCodePath"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(qrCodePath))
                {
                    string fullQRCodeUrl = string.Concat(Base_Url.TrimEnd('/'), "/", qrCodePath.TrimStart('/'));

                    ViewState["QRCodePath"] = fullQRCodeUrl;
                    imgQRCode.ImageUrl = fullQRCodeUrl;  
                    qrCodeSection.Visible = true;

                    System.Diagnostics.Debug.WriteLine($"QR Code Path: {fullQRCodeUrl}");
                }
                else
                {
                    qrCodeSection.Visible = false;
                    System.Diagnostics.Debug.WriteLine("QR Code not available for this booking");
                }

                if (bookingData["passengers"] != null)
                {
                    JArray passengers = bookingData["passengers"] as JArray;
                    rptPassengers.DataSource = passengers;
                    rptPassengers.DataBind();
                }

                System.Diagnostics.Debug.WriteLine("Ticket details displayed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error displaying ticket: " + ex.Message);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "print",
                "window.print();", true);
        }

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "print",
                "window.print();", true);
        }

        protected void btnBackToBookings_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MyBookings.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        public string QRCodePath
        {
            get { return ViewState["QRCodePath"]?.ToString() ?? ""; }
        }
    }
}