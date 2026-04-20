using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ExcelBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace Excel_Bus
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        string apiUrl = System.Configuration.ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                LoadTripsMenu();
                //BindTripsDropdown();
                //RestoreSelectedTrip();
            }
        }


        private void LoadTripsMenu()
        {
            try
            {
                using (var tripClient = new HttpClient())
                {
                    tripClient.DefaultRequestHeaders.Clear();
                    tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip1";
                    System.Diagnostics.Debug.WriteLine("Fetching trips from: " + url);

                    var response = tripClient.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("Trips API Response: " + json);

                        var trips = JsonConvert.DeserializeObject<List<TripDetail>>(json);

                        if (trips != null && trips.Count > 0)
                        {
                            //tripsMenu.Controls.Clear();

                            System.Diagnostics.Debug.WriteLine($"Total trips received from API: {trips.Count}");

                            // Add "All Trips" option
                            var allTripsLi = new HtmlGenericControl("li");
                            var allTripsLink = new HtmlGenericControl("a");
                            allTripsLink.Attributes["href"] = "ticket.aspx";
                            allTripsLink.InnerText = "-- All Trips --";
                            allTripsLi.Controls.Add(allTripsLink);
                            //tripsMenu.Controls.Add(allTripsLi);

                            int addedCount = 0;
                            foreach (var trip in trips)
                            {
                                if (trip.Status == 1)
                                {
                                    var li = new HtmlGenericControl("li");
                                    var anchor = new HtmlGenericControl("a");
                                    anchor.Attributes["href"] = $"ticket.aspx?tripId={trip.TripId}";
                                    anchor.InnerText = trip.Title;

                                    li.Controls.Add(anchor);
                                    //tripsMenu.Controls.Add(li);

                                    addedCount++;
                                    System.Diagnostics.Debug.WriteLine($"Added trip #{addedCount}: {trip.Title} (ID: {trip.TripId})");
                                }
                            }

                            System.Diagnostics.Debug.WriteLine($"Total trips added to menu: {addedCount}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No trips found in API response");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Trips API failed: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading trips menu: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }

        //private async Task BindTripsDropdown()
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new MediaTypeWithQualityHeaderValue("application/json"));

        //            string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip";
        //            HttpResponseMessage response = await client.GetAsync(url);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = await response.Content.ReadAsStringAsync();
        //                System.Diagnostics.Debug.WriteLine("Trips API Response: " + json);

        //                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    ddlTrips.DataSource = dt;
        //                    ddlTrips.DataTextField = "title";
        //                    ddlTrips.DataValueField = "tripId";
        //                    ddlTrips.DataBind();

        //                    ddlTrips.Items.Insert(0, new ListItem("-- Select Trip --", "0"));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error loading trips: " + ex.Message);
        //    }
        //}


        //private void RestoreSelectedTrip()
        //{
        //    string tripId = Request.QueryString["tripId"];

        //    System.Diagnostics.Debug.WriteLine("Restoring Trip ID from QueryString: " + tripId);

        //    if (!string.IsNullOrEmpty(tripId) && tripId != "0")
        //    {
        //        ListItem item = ddlTrips.Items.FindByValue(tripId);
        //        if (item != null)
        //        {
        //            ddlTrips.ClearSelection();
        //            item.Selected = true;
        //            System.Diagnostics.Debug.WriteLine("Trip restored and selected: " + item.Text);
        //        }
        //    }
        //}

        //private void LoadTripsMenu()
        //{
        //    try
        //    {
        //        using (var tripClient = new HttpClient())
        //        {
        //            tripClient.DefaultRequestHeaders.Clear();
        //            tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

        //            string url = apiUrl.TrimEnd('/') + "/Trips/GetTrip";
        //            System.Diagnostics.Debug.WriteLine("Fetching trips from: " + url);

        //            var response = tripClient.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = response.Content.ReadAsStringAsync().Result;
        //                System.Diagnostics.Debug.WriteLine("Trips API Response received");

        //                var trips = JsonConvert.DeserializeObject<List<TripDetail>>(json);

        //                if (trips != null && trips.Count > 0)
        //                {
        //                    tripsMenu.Controls.Clear();

        //                    // Add "All Trips" option
        //                    var allTripsLi = new HtmlGenericControl("li");
        //                    var allTripsLink = new HtmlGenericControl("a");
        //                    allTripsLink.Attributes["href"] = "ticket.aspx";
        //                    allTripsLink.InnerText = "-- All Trips --";
        //                    allTripsLi.Controls.Add(allTripsLink);
        //                    tripsMenu.Controls.Add(allTripsLi);

        //                    // Add each active trip
        //                    foreach (var trip in trips)
        //                    {
        //                        if (trip.Status == 1)
        //                        {
        //                            var li = new HtmlGenericControl("li");
        //                            var anchor = new HtmlGenericControl("a");
        //                            anchor.Attributes["href"] = $"ticket.aspx?tripId={trip.TripId}";
        //                            anchor.InnerText = trip.Title;

        //                            li.Controls.Add(anchor);
        //                            tripsMenu.Controls.Add(li);

        //                            System.Diagnostics.Debug.WriteLine($"Added trip: {trip.Title} (ID: {trip.TripId})");
        //                        }
        //                    }

        //                    System.Diagnostics.Debug.WriteLine($"Total trips loaded: {trips.Count}");
        //                }
        //                else
        //                {
        //                    //AddNoTripsMessage();
        //                }
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Trips API failed: {response.StatusCode}");
        //                //AddNoTripsMessage();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error loading trips menu: " + ex.Message);
        //        //AddNoTripsMessage();
        //    }
        //}


        //private void FetchTripDetailsAndAddToMenu(int tripId)
        //{
        //    try
        //    {
        //        using (var tripClient = new HttpClient())
        //        {
        //            tripClient.DefaultRequestHeaders.Clear();
        //            tripClient.DefaultRequestHeaders.Add("Accept", "application/json");

        //            string url = $"{apiUrl.TrimEnd('/')}/Trips/GetTrip/{tripId}";
        //            System.Diagnostics.Debug.WriteLine($"Fetching details for trip ID {tripId} from: {url}");

        //            var response = tripClient.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = response.Content.ReadAsStringAsync().Result;
        //                System.Diagnostics.Debug.WriteLine($"Details for trip ID {tripId} received");

        //                var trip = JsonConvert.DeserializeObject<TripDetail>(json);

        //                if (trip != null)
        //                {
        //                    // Add trip to the menu
        //                    var li = new HtmlGenericControl("li");
        //                    var anchor = new HtmlGenericControl("a");
        //                    anchor.Attributes["href"] = $"ticket.aspx?tripId={trip.TripId}";
        //                    anchor.InnerText = trip.Title;

        //                    li.Controls.Add(anchor);
        //                    tripsMenu.Controls.Add(li);

        //                    System.Diagnostics.Debug.WriteLine($"Added trip: {trip.Title} (ID: {trip.TripId})");
        //                }
        //                else
        //                {
        //                    System.Diagnostics.Debug.WriteLine($"No details found for trip ID {tripId}");
        //                }
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Failed to fetch details for trip ID {tripId}: {response.StatusCode}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error fetching trip details for ID {tripId}: " + ex.Message);
        //    }
        //}


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Home.aspx", false);
        }
        //protected void ddlTrips_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string selectedTripId = ddlTrips.SelectedValue;

        //    System.Diagnostics.Debug.WriteLine("=== SelectedIndexChanged Event ===");
        //    System.Diagnostics.Debug.WriteLine("Selected Trip ID: " + selectedTripId);
        //    System.Diagnostics.Debug.WriteLine("Selected Trip Text: " + ddlTrips.SelectedItem.Text);

        //    if (selectedTripId == "0")
        //    {
        //        Response.Redirect("ticket.aspx", false);
        //        return;
        //    }

        //    try
        //    {
        //        System.Diagnostics.Debug.WriteLine("Redirecting to: ticket.aspx?tripId=" + selectedTripId);
        //        Response.Redirect($"ticket.aspx?tripId={selectedTripId}", true);
            

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error in SelectedIndexChanged: " + ex.Message);
        //    }
        //}
    }
}