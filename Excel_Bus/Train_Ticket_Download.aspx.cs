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
    public partial class Train_Ticket_Download : System.Web.UI.Page
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
                    Response.Redirect("~/Train_MyBookings.aspx");
                    return;
                }

                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/TrainTicket.aspx");
                    return;
                }

                string userId = Session["UserId"].ToString();
                RegisterAsyncTask(new PageAsyncTask(() => LoadTicketData(userId, pnr)));
            }
        }
        private async Task LoadTicketData(string userId, string pnrNumber)
        {
            try
            {
                //string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";
                string apiEndpoint = $"{apiBaseUrl}TrainTicketBookings/GetBookedTrainTicketsAllByUserId/{userId}";
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
        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('{message.Replace("'", "\\'")}');", true);
        }

        private void PopulateTicketInfo(JObject bookingData)
        {
            try
            {
                // Set basic ticket information
                lblPNRNumber.Text = bookingData["pnrNumber"]?.ToString() ?? "";
                lblTrainNo.Text = (bookingData["trainNumber"] != null && int.TryParse(bookingData["trainNumber"].ToString(), out int trainNo))
    ? trainNo.ToString()
    : "0";
                lblTrainName.Text = bookingData["trainName"].ToString() ?? "";
                
                //lblSource.Text = bookingData["fromStation"].ToString() ?? "";
                //lblDestination.Text = bookingData["toStation"].ToString() ?? "";
              
                //string sourceDestination = bookingData["sourceDestination"]?.ToString() ?? "";
                //var parts = sourceDestination.Split('-');
                //if (parts.Length == 2)
                //{
                //    lblRoute.Text = $"{parts[0].Trim()} → {parts[1].Trim()}";
                //}
                //else
                //{
                //    lblRoute.Text = sourceDestination;
                //}
                // Getting values from bookingData and combining them
                lblSource.Text = bookingData["fromStation"].ToString() ?? "";
                lblDestination.Text = bookingData["toStation"].ToString() ?? "";

                // Combine source and destination into a route
                string sourceDestination = $"{lblSource.Text} → {lblDestination.Text}";
                lblRoute.Text = sourceDestination;
                string dateOfJourney = bookingData["journeyDate"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(dateOfJourney))
                {
                    lblJourneyDate.Text = Convert.ToDateTime(dateOfJourney).ToString("dd MMMM yyyy");
                }

                lblTicketCount.Text = bookingData["passengerCount"]?.ToString() ?? "0";

                decimal subTotal = bookingData["totalAmount"]?.Value<decimal>() ?? 0;
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
            Response.Redirect("~/Train_MyBookings.aspx");
        }
        public string QRCodePath
        {
            get { return ViewState["QRCodePath"]?.ToString() ?? ""; }
        }
    }
}