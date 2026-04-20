using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Excel_Bus
{
    public partial class ticket : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    pickup_point();
                    dropping_point();
                    BindSchedules();
                    txtJourneyDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");

                    string tripId = Request.QueryString["tripId"];

                    System.Diagnostics.Debug.WriteLine("Page Load - Trip ID from QueryString: " + tripId);

                    if (!string.IsNullOrEmpty(tripId) && tripId != "0")
                    {
                        BindTripDetails(tripId);
                    }
                    else
                    {

                        BindTicketsGroupedByRoute();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Page_Load Error: " + ex.Message);
            }
        }

        private void BindTicketsGroupedByRoute()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = apiUrl.TrimEnd('/') + "/Trips/GetTripDetailsbyvehicleid";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseMessage = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("API Response: " + responseMessage);

                        // Parse JSON array and create DataTable manually to preserve all fields
                        JArray jsonArray = JArray.Parse(responseMessage);
                        DataTable dt = JsonArrayToDataTable(jsonArray);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            // Extract and store VehicleId in session
                            string vehicleId = dt.Rows[0]["VehicleId"]?.ToString();
                            if (!string.IsNullOrEmpty(vehicleId))
                            {
                                HttpContext.Current.Session["VehicleId"] = vehicleId;
                            }

                            // Group trips by route
                            var routeGroups = GroupTripsByRoute(dt);
                            rptRoutes.DataSource = routeGroups;
                            rptRoutes.DataBind();
                        }
                        else
                        {
                            rptRoutes.DataSource = null;
                            rptRoutes.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error while binding tickets: " + ex.Message);
                rptRoutes.DataSource = null;
                rptRoutes.DataBind();
            }
        }

        // New method to convert JArray to DataTable while preserving all fields including Duration
        private DataTable JsonArrayToDataTable(JArray jsonArray)
        {
            DataTable dt = new DataTable();

            if (jsonArray.Count == 0)
                return dt;

            // Get all unique property names from all objects - preserve exact casing from JSON
            var properties = new HashSet<string>();
            foreach (JObject obj in jsonArray)
            {
                foreach (var prop in obj.Properties())
                {
                    properties.Add(prop.Name); // Keep exact property name from JSON
                }
            }

            // Create columns with exact names from JSON
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop);
            }

            // Add duration column if it doesn't exist
            if (!dt.Columns.Contains("duration") && !dt.Columns.Contains("Duration"))
            {
                dt.Columns.Add("duration");
            }

            // Add rows
            foreach (JObject obj in jsonArray)
            {
                DataRow row = dt.NewRow();
                foreach (var prop in obj.Properties())
                {
                    row[prop.Name] = prop.Value.Type == JTokenType.Null ? DBNull.Value : (object)prop.Value.ToString();
                }

                // Calculate duration if not present - check all possible field name variations
                bool needsCalculation = true;
                string durationValue = "";

                // Check if duration already exists in any form
                if (row.Table.Columns.Contains("duration") && row["duration"] != null && row["duration"] != DBNull.Value)
                {
                    durationValue = row["duration"].ToString();
                    if (!string.IsNullOrEmpty(durationValue))
                        needsCalculation = false;
                }
                else if (row.Table.Columns.Contains("Duration") && row["Duration"] != null && row["Duration"] != DBNull.Value)
                {
                    durationValue = row["Duration"].ToString();
                    if (!string.IsNullOrEmpty(durationValue))
                    {
                        row["duration"] = durationValue;
                        needsCalculation = false;
                    }
                }

                if (needsCalculation)
                {
                    try
                    {
                        string startTimeStr = null;
                        string endTimeStr = null;

                        // Check for startTime/StartTime
                        if (row.Table.Columns.Contains("startTime") && row["startTime"] != null && row["startTime"] != DBNull.Value)
                            startTimeStr = row["startTime"].ToString();
                        else if (row.Table.Columns.Contains("StartTime") && row["StartTime"] != null && row["StartTime"] != DBNull.Value)
                            startTimeStr = row["StartTime"].ToString();

                        // Check for endTime/EndTime
                        if (row.Table.Columns.Contains("endTime") && row["endTime"] != null && row["endTime"] != DBNull.Value)
                            endTimeStr = row["endTime"].ToString();
                        else if (row.Table.Columns.Contains("EndTime") && row["EndTime"] != null && row["EndTime"] != DBNull.Value)
                            endTimeStr = row["EndTime"].ToString();

                        if (!string.IsNullOrEmpty(startTimeStr) && !string.IsNullOrEmpty(endTimeStr))
                        {
                            TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                            TimeSpan endTime = TimeSpan.Parse(endTimeStr);

                            TimeSpan duration;
                            if (endTime < startTime)
                            {
                                // Next day
                                duration = (TimeSpan.FromHours(24) - startTime) + endTime;
                            }
                            else
                            {
                                duration = endTime - startTime;
                            }

                            row["duration"] = $"{duration.Hours:D2}:{duration.Minutes:D2} hour";
                            System.Diagnostics.Debug.WriteLine($"Calculated duration: {row["duration"]} (from {startTimeStr} to {endTimeStr})");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Could not calculate duration - StartTime: {startTimeStr}, EndTime: {endTimeStr}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Duration calculation error: " + ex.Message);
                    }
                }

                dt.Rows.Add(row);
            }

            // Debug: Print all column names
            System.Diagnostics.Debug.WriteLine("DataTable Columns: " + string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            // Debug: Print first row data
            if (dt.Rows.Count > 0)
            {
                var firstRow = dt.Rows[0];
                System.Diagnostics.Debug.WriteLine("First row duration: " + (firstRow["duration"] ?? "NULL"));

                // Print all time-related fields for debugging
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.ToLower().Contains("time") || col.ColumnName.ToLower().Contains("duration"))
                    {
                        System.Diagnostics.Debug.WriteLine($"  {col.ColumnName}: {firstRow[col.ColumnName]}");
                    }
                }
            }

            return dt;
        }

        private List<RouteGroup> GroupTripsByRoute(DataTable dt)
        {
            var routes = new List<RouteGroup>();

            // Group by StartFrom and EndTo
            var grouped = dt.AsEnumerable()
                .GroupBy(row => new
                {
                    StartFrom = row.Field<string>("StartFrom"),
                    EndTo = row.Field<string>("EndTo")
                });

            foreach (var group in grouped)
            {
                var routeGroup = new RouteGroup
                {
                    RouteName = $"{group.Key.StartFrom} to {group.Key.EndTo}",
                    StartFrom = group.Key.StartFrom,
                    EndTo = group.Key.EndTo,
                    BusCount = group.Count(),
                    Buses = group.CopyToDataTable()
                };

                routes.Add(routeGroup);
            }

            return routes;
        }

        public class RouteGroup
        {
            public string RouteName { get; set; }
            public string StartFrom { get; set; }
            public string EndTo { get; set; }
            public int BusCount { get; set; }
            public DataTable Buses { get; set; }
        }

        protected void rptRoutes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptBuses = (Repeater)e.Item.FindControl("rptBuses");

                if (rptBuses != null)
                {
                    RouteGroup routeData = (RouteGroup)e.Item.DataItem;
                    rptBuses.DataSource = routeData.Buses;
                    rptBuses.DataBind();
                }
            }
        }

        protected void rptBuses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink lnkSelectSeat = (HyperLink)e.Item.FindControl("lnkSelectSeat");
                if (lnkSelectSeat != null)
                {
                    DataRowView drv = e.Item.DataItem as DataRowView;
                    string tripId = drv?["TripId"]?.ToString() ?? drv?["Id"]?.ToString();
                    string vehicleId = drv?["VehicleId"]?.ToString();

                    if (!string.IsNullOrEmpty(tripId) && !string.IsNullOrEmpty(vehicleId))
                    {
                        // Set the NavigateUrl to pass both tripId and vehicleId
                        lnkSelectSeat.NavigateUrl = $"~/Select_Seat.aspx?tripId={Server.UrlEncode(tripId)}&vehicleId={Server.UrlEncode(vehicleId)}";
                    }
                    else if (!string.IsNullOrEmpty(tripId))
                    {
                        // If VehicleId is not available, just pass tripId
                        lnkSelectSeat.NavigateUrl = $"~/Select_Seat.aspx?tripId={Server.UrlEncode(tripId)}";
                    }
                }
            }
        }


        private void BindTripDetails(string tripId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/Trips/GetTripDetails?tripId={tripId}";
                    System.Diagnostics.Debug.WriteLine("Calling API: " + url);

                    HttpResponseMessage res = client.GetAsync(url).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var json = res.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("Trip Details Response: " + json);

                        JObject tripObject = JObject.Parse(json);
                        JArray tripArray = new JArray { tripObject };

                        // Use the new method to preserve all fields including Duration
                        DataTable dt = JsonArrayToDataTable(tripArray);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var routeGroups = GroupTripsByRoute(dt);
                            rptRoutes.DataSource = routeGroups;
                            rptRoutes.DataBind();
                        }
                        else
                        {
                            rptRoutes.DataSource = null;
                            rptRoutes.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                rptRoutes.DataSource = null;
                rptRoutes.DataBind();
            }
        }

        private async Task pickup_point()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync(apiUrl + ("Counters/GetCounters")).Result;

                    if (Res.IsSuccessStatusCode)
                    {
                        var ResponseMessage = await Res.Content.ReadAsStringAsync();
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(ResponseMessage, typeof(DataTable));

                        ddlPickup.DataSource = dt;
                        ddlPickup.DataTextField = "Name";
                        ddlPickup.DataValueField = "Id";
                        ddlPickup.DataBind();

                        ddlPickup.Items.Insert(0, new ListItem("Pickup Point", "ALL"));
                    }
                    else
                    {
                        ddlPickup.Items.Clear();
                        ddlPickup.Items.Add(new ListItem("No data available", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                ddlPickup.Items.Clear();
                ddlPickup.Items.Add(new ListItem("Error loading data", ""));
                throw;
            }
        }

        private async Task dropping_point()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync(apiUrl + ("Counters/GetCounters")).Result;

                    if (Res.IsSuccessStatusCode)
                    {
                        var ResponseMessage = await Res.Content.ReadAsStringAsync();
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(ResponseMessage, typeof(DataTable));

                        ddlDestination.DataSource = dt;
                        ddlDestination.DataTextField = "Name";
                        ddlDestination.DataValueField = "Id";
                        ddlDestination.DataBind();

                        ddlDestination.Items.Insert(0, new ListItem("Dropping Point", "ALL"));
                    }
                    else
                    {
                        ddlDestination.Items.Clear();
                        ddlDestination.Items.Add(new ListItem("No data available", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                ddlDestination.Items.Clear();
                ddlDestination.Items.Add(new ListItem("Error loading data", ""));
                throw;
            }
        }

        protected string GetDayNames(object dayOffObj)
        {
            if (dayOffObj == null || dayOffObj == DBNull.Value || dayOffObj.ToString().Trim() == "[]")
                return "Every day available";

            try
            {
                var dayNumbers = JsonConvert.DeserializeObject<List<string>>(dayOffObj.ToString());
                string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

                List<string> result = new List<string>();
                foreach (var numStr in dayNumbers)
                {
                    if (int.TryParse(numStr, out int n) && n >= 0 && n <= 6)
                    {
                        result.Add(dayNames[n]);
                    }
                }

                return string.Join(", ", result);
            }
            catch
            {
                return "";
            }
        }

        protected string FormatFacilities(object facilitiesObj)
        {
            if (facilitiesObj == null || facilitiesObj == DBNull.Value)
                return "";

            try
            {
                var facilities = JsonConvert.DeserializeObject<List<string>>(facilitiesObj.ToString());

                if (facilities != null && facilities.Count > 0)
                {
                    return string.Join("&nbsp;&nbsp;&nbsp;&nbsp;", facilities);
                }
                return "";
            }
            catch
            {
                return facilitiesObj.ToString();
            }
        }

        protected void btnFindTickets_Click(object sender, EventArgs e)
        {
            string pickupId = ddlPickup.SelectedValue;
            string droppingId = ddlDestination.SelectedValue;
            string journeyDate = txtJourneyDate.Text;

            if (string.IsNullOrWhiteSpace(pickupId) || pickupId == "ALL" ||
                string.IsNullOrWhiteSpace(droppingId) || droppingId == "ALL" ||
                string.IsNullOrWhiteSpace(journeyDate))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Please select pickup, destination, and journey date.');", true);
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = $"{apiUrl.TrimEnd('/')}/Trips/GetTripsByRoute" +
                              $"?startLocationId={HttpUtility.UrlEncode(pickupId)}" +
                              $"&endLocationId={HttpUtility.UrlEncode(droppingId)}" +
                              $"&dateOfJourney={HttpUtility.UrlEncode(journeyDate)}";

                    var res = client.GetAsync(url).Result;
                    var json = res.Content.ReadAsStringAsync().Result;

                    System.Diagnostics.Debug.WriteLine("Search response: " + json);

                    DataTable dt = new DataTable();

                    if (res.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(json))
                    {
                        try
                        {
                            var token = JToken.Parse(json);

                            if (token is JObject obj)
                            {
                                var tripsArray = obj["trips"] as JArray;
                                if (tripsArray != null && tripsArray.HasValues)
                                {
                                    // Use JsonArrayToDataTable to preserve all fields including duration
                                    dt = JsonArrayToDataTable(tripsArray);
                                }
                            }
                            else if (token is JArray arr)
                            {
                                // Use JsonArrayToDataTable to preserve all fields including duration
                                dt = JsonArrayToDataTable(arr);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("JSON Parsing Error: " + ex.Message);
                        }
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var routeGroups = GroupTripsByRoute(dt);
                        rptRoutes.DataSource = routeGroups;
                        rptRoutes.DataBind();
                    }
                    else
                    {
                        rptRoutes.DataSource = null;
                        rptRoutes.DataBind();
                        ClientScript.RegisterStartupScript(GetType(), "alert",
                            "alert('No trips found for the selected route and date.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                rptRoutes.DataSource = null;
                rptRoutes.DataBind();
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private void BindSchedules()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = apiUrl.TrimEnd('/') + "/Schedules/GetSchedules";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;

                        DataTable dt = null;
                        try
                        {
                            dt = JsonConvert.DeserializeObject<DataTable>(json);
                        }
                        catch
                        {
                            JObject obj = JsonConvert.DeserializeObject<JObject>(json);
                            if (obj["data"] != null)
                            {
                                dt = JsonConvert.DeserializeObject<DataTable>(obj["data"].ToString());
                            }
                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Repeater1.DataSource = dt;
                            Repeater1.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error binding schedules: " + ex.Message);
            }
        }

        protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                if (cb != null && cb.Checked)
                {
                    RepeaterItem item = cb.NamingContainer as RepeaterItem;
                    HiddenField hdn = item.FindControl("hdnScheduleId") as HiddenField;

                    if (hdn != null && !string.IsNullOrEmpty(hdn.Value))
                    {
                        if (int.TryParse(hdn.Value, out int scheduleId))
                        {
                            BindTicketsBySchedule(scheduleId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CheckedChanged Error: " + ex.Message);
            }
        }

        private void BindTicketsBySchedule(int scheduleId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Construct the URL for the API call
                    string url = apiUrl.TrimEnd('/') + $"/Trips/GetTripsBySchedule?scheduleId={scheduleId}";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        JObject jsonObj = JObject.Parse(json);
                        JArray tripsArray = (JArray)jsonObj["trips"];

                        // Check if the trips array contains data
                        if (tripsArray != null && tripsArray.Count > 0)
                        {
                            // Use the new method to preserve all fields including Duration
                            DataTable dt = JsonArrayToDataTable(tripsArray);

                            // Group the trips by route
                            var routeGroups = GroupTripsByRoute(dt);

                            // Set the data source for the Repeater and bind it
                            rptRoutes.DataSource = routeGroups;
                            rptRoutes.DataBind();
                        }
                        else
                        {
                            // If no data, clear the previous data and set the DataSource to null
                            rptRoutes.DataSource = null;
                            rptRoutes.DataBind();
                        }
                    }
                    else
                    {
                        // In case of an unsuccessful API call, clear the previous data
                        rptRoutes.DataSource = null;
                        rptRoutes.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (you can add a logger here if needed)
                System.Diagnostics.Debug.WriteLine("BindTicketsBySchedule Error: " + ex.Message);

                // Clear the previous data on error
                rptRoutes.DataSource = null;
                rptRoutes.DataBind();
            }
        }



    }
}