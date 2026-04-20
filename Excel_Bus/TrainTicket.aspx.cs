using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.UI.WebControls;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Excel_Bus
{
    public partial class TrainTicket : System.Web.UI.Page
    {
        HttpClient client = new HttpClient();
        string apiUrl = ConfigurationSettings.AppSettings["api_path"];
        private static int? selectedScheduleId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    
                    LoadStations();
                    LoadSchedules();
                    txtJourneyDate.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");

                    string tripId = Request.QueryString["tripId"];
                    string userId = Request.QueryString["UserId"];
                    
                    if (!string.IsNullOrEmpty(tripId) && tripId != "0")
                    {
                        BindTripDetails(tripId);
                    }
                    else
                    {
                       BindTrainTripsGroupedByRoute();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Page_Load Error: " + ex.Message);
            }
        }

        //private void BindTrainTripsGroupedByRoute()
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            string url = apiUrl.TrimEnd('/') + "/TrainTrips/GetTrainTripDetailsByTrainId";
        //            HttpResponseMessage response = client.GetAsync(url).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseMessage = response.Content.ReadAsStringAsync().Result;
        //                JArray jsonArray = JArray.Parse(responseMessage);
        //                DataTable dt = JsonArrayToDataTable(jsonArray);

        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    // Filter by schedule if selected
        //                    if (selectedScheduleId.HasValue)
        //                    {
        //                        dt = FilterBySchedule(dt, selectedScheduleId.Value);
        //                    }

        //                    // Filter by class if selected
        //                  //  dt = FilterByClass(dt);

        //                    // Filter by price range if specified
                           

        //                    // Filter by journey date if applicable
        //                    dt = FilterByJourneyDate(dt);

        //                    var routeGroups = GroupTripsByRoute(dt);
        //                    rptRoutes.DataSource = routeGroups;
        //                    rptRoutes.DataBind();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("❌ BindTrainTripsGroupedByRoute Error: " + ex.Message);
        //    }
        //}

        private DataTable FilterBySchedule(DataTable dt, int scheduleId)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                    return dt;

                var filteredRows = dt.AsEnumerable()
                    .Where(row =>
                    {
                        var scheduleIdValue = GetStringValue(row, "ScheduleId");
                        return scheduleIdValue == scheduleId.ToString();
                    });

                if (filteredRows.Any())
                {
                    return filteredRows.CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FilterBySchedule Error: " + ex.Message);
            }

            return dt;
        }

        //private DataTable FilterByClass(DataTable dt)
        //{
        //    try
        //    {
        //        bool hasFilter = chkLuxury.Checked || chkFirstClass.Checked || chkSecondClass.Checked;

        //        if (!hasFilter)
        //            return dt; // No filter checked, return all data

        //        if (dt == null || dt.Rows.Count == 0)
        //            return dt;

        //        var filteredRows = dt.AsEnumerable()
        //            .Where(row =>
        //            {
        //                string coachType = GetStringValue(row, "coachType").ToLower();

        //                // Check each filter independently
        //                if (chkLuxury.Checked && (coachType.Contains("luxury") || coachType.Contains("luxe")))
        //                    return true;
        //                if (chkFirstClass.Checked && coachType.Contains("first"))
        //                    return true;
        //                if (chkSecondClass.Checked && coachType.Contains("second"))
        //                    return true;

        //                return false;
        //            });

        //        if (filteredRows.Any())
        //        {
        //            return filteredRows.CopyToDataTable();
        //        }
        //        else
        //        {
        //            System.Diagnostics.Debug.WriteLine("⚠️ Class filter applied but no matching coach types found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("FilterByClass Error: " + ex.Message);
        //    }

        //    return dt; // Return original data if no matches (don't return empty table)
        //}

        //private DataTable FilterByPriceRange(DataTable dt)
        //{
        //    try
        //    {
        //        decimal minPrice = 0;
        //        decimal maxPrice = decimal.MaxValue;

        //        if (!string.IsNullOrEmpty(txtMinPrice.Text))
        //        {
        //            decimal.TryParse(txtMinPrice.Text, out minPrice);
        //        }

        //        if (!string.IsNullOrEmpty(txtMaxPrice.Text))
        //        {
        //            decimal.TryParse(txtMaxPrice.Text, out maxPrice);
        //        }

        //        if (minPrice > 0 || maxPrice < decimal.MaxValue)
        //        {
        //            var filteredRows = dt.AsEnumerable()
        //                .Where(row =>
        //                {
        //                    string priceStr = GetStringValue(row, "Price");
        //                    if (decimal.TryParse(priceStr, out decimal price))
        //                    {
        //                        return price >= minPrice && price <= maxPrice;
        //                    }
        //                    return false;
        //                });

        //            if (filteredRows.Any())
        //            {
        //                return filteredRows.CopyToDataTable();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("FilterByPriceRange Error: " + ex.Message);
        //    }

        //    return dt;
        //}

        private DataTable FilterByJourneyDate(DataTable dt)
        {
            try
            {
                if (string.IsNullOrEmpty(txtJourneyDate.Text))
                    return dt;

                DateTime journeyDate;
                if (!DateTime.TryParse(txtJourneyDate.Text, out journeyDate))
                    return dt;

                // Convert .NET DayOfWeek to API format
                // .NET: Sunday=0, Monday=1, Tuesday=2, ..., Saturday=6
                // API: Monday=1, Tuesday=2, ..., Saturday=6, Sunday=7
                int dayOfWeek = (int)journeyDate.DayOfWeek;
                int apiDayFormat = dayOfWeek == 0 ? 7 : dayOfWeek;

                System.Diagnostics.Debug.WriteLine($"📅 Journey Date: {journeyDate:yyyy-MM-dd} ({journeyDate.DayOfWeek}) = API Day {apiDayFormat}");

                var filteredRows = dt.AsEnumerable()
                    .Where(row =>
                    {
                        string trainId = GetStringValue(row, "TrainId");
                        if (string.IsNullOrEmpty(trainId))
                            return true;

                        string dayOffStr = GetTrainDayOff(trainId);

                        if (string.IsNullOrWhiteSpace(dayOffStr) || dayOffStr == "[]" || dayOffStr == "")
                            return true; // No off days

                        try
                        {
                            dayOffStr = dayOffStr.Trim('[', ']').Trim();

                            if (string.IsNullOrWhiteSpace(dayOffStr))
                                return true;

                            string[] dayOffNumbers = dayOffStr.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var numStr in dayOffNumbers)
                            {
                                if (int.TryParse(numStr.Trim(), out int offDay) && offDay == apiDayFormat)
                                {
                                    System.Diagnostics.Debug.WriteLine($"🚫 Excluding train {trainId} - off on day {apiDayFormat}");
                                    return false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing day off: {ex.Message}");
                        }

                        return true;
                    });

                if (filteredRows.Any())
                {
                    return filteredRows.CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FilterByJourneyDate Error: " + ex.Message);
            }

            return dt;
        }

        private DataTable JsonArrayToDataTable(JArray jsonArray)
        {
            DataTable dt = new DataTable();

            try
            {
                if (jsonArray.Count == 0) return dt;

                var properties = new HashSet<string>();
                foreach (JObject obj in jsonArray)
                {
                    foreach (var prop in obj.Properties())
                    {
                        properties.Add(prop.Name);
                    }
                }

                foreach (var prop in properties)
                {
                    dt.Columns.Add(prop, typeof(string));
                }

                if (!dt.Columns.Contains("Duration"))
                {
                    dt.Columns.Add("Duration", typeof(string));
                }

                foreach (JObject obj in jsonArray)
                {
                    DataRow row = dt.NewRow();

                    foreach (var prop in obj.Properties())
                    {
                        if (prop.Value.Type == JTokenType.Null)
                        {
                            row[prop.Name] = DBNull.Value;
                        }
                        else
                        {
                            row[prop.Name] = prop.Value.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(row["Duration"]?.ToString()))
                    {
                        try
                        {
                            string depStr = row["DepartureTime"]?.ToString();
                            string arrStr = row["ArrivalTime"]?.ToString();

                            if (!string.IsNullOrEmpty(depStr) && !string.IsNullOrEmpty(arrStr))
                            {
                                TimeSpan departureTime = TimeSpan.Parse(depStr);
                                TimeSpan arrivalTime = TimeSpan.Parse(arrStr);
                                TimeSpan duration;
                                if (arrivalTime < departureTime)
                                {
                                    duration = (TimeSpan.FromHours(24) - departureTime) + arrivalTime;
                                }
                                else
                                {
                                    duration = arrivalTime - departureTime;
                                }
                                row["Duration"] = $"{duration.Hours:D2}:{duration.Minutes:D2} hour";
                            }
                        }
                        catch
                        {
                            row["Duration"] = "N/A";
                        }
                    }

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ JsonArrayToDataTable Error: " + ex.Message);
            }

            return dt;
        }

        private List<RouteGroup> GroupTripsByRoute(DataTable dt)
        {
            var routes = new List<RouteGroup>();

            try
            {
                var routeGrouped = dt.AsEnumerable()
                    .GroupBy(row => new
                    {
                        StartStation = GetStringValue(row, "StartStation"),
                        EndStation = GetStringValue(row, "EndStation")
                    });

                foreach (var routeGroup in routeGrouped)
                {
                    var route = new RouteGroup
                    {
                        RouteName = $"{routeGroup.Key.StartStation} to {routeGroup.Key.EndStation}",
                        StartStation = routeGroup.Key.StartStation,
                        EndStation = routeGroup.Key.EndStation,
                        Trains = new List<TrainGroup>()
                    };

                    var trainGrouped = routeGroup.GroupBy(row => new
                    {
                        TrainId = GetStringValue(row, "TrainId"),
                        TrainName = GetStringValue(row, "TrainName"),
                        TrainNumber = GetStringValue(row, "TrainNumber")
                    });

                    foreach (var trainGroup in trainGrouped)
                    {
                        var firstRow = trainGroup.First();
                        string dayOff = GetTrainDayOff(trainGroup.Key.TrainId);

                        var train = new TrainGroup
                        {
                            TrainId = trainGroup.Key.TrainId,
                            TrainName = trainGroup.Key.TrainName,
                            TrainNumber = trainGroup.Key.TrainNumber,
                            DepartureTime = GetStringValue(firstRow, "DepartureTime"),
                            ArrivalTime = GetStringValue(firstRow, "ArrivalTime"),
                            Duration = GetStringValue(firstRow, "Duration"),
                            DayOff = dayOff,
                            CoachTypes = new List<CoachTypeGroup>()
                        };

                        System.Diagnostics.Debug.WriteLine($"\n🚂 Processing Train: {train.TrainName} (ID: {train.TrainId})");
                        System.Diagnostics.Debug.WriteLine($"📅 Off Days: {dayOff}");

                        var coachTypeGrouped = trainGroup.GroupBy(row =>
                            GetStringValue(row, "CoachTypeId"));

                        System.Diagnostics.Debug.WriteLine($"📊 Found {coachTypeGrouped.Count()} coach types in data");

                        foreach (var coachTypeGroup in coachTypeGrouped)
                        {
                            var coachTypeRow = coachTypeGroup.First();
                            string coachTypeId = coachTypeGroup.Key;

                            if (string.IsNullOrEmpty(coachTypeId))
                            {
                                System.Diagnostics.Debug.WriteLine($"  ⚠️ Skipping - no CoachTypeId");
                                continue;
                            }

                            string coachType = GetStringValue(coachTypeRow, "coachType");
                            if (string.IsNullOrEmpty(coachType) || coachType == "Unknown Fleet Type")
                            {
                                coachType = GetCoachTypeName(coachTypeId);
                            }

                            int noOfCoaches = GetCoachCount(train.TrainId, coachTypeId);

                            // ✅ CRITICAL FIX: Get price by trainId + coachTypeId
                            //string price = GetTrainCoachPrice(train.TrainId, coachTypeId);
                            // GroupTripsByRoute mein ye line:

                            string price = GetStringValue(coachTypeRow, "Price", "0.00");
                            if (price == "0.00" || string.IsNullOrEmpty(price))
                            {
                                price = GetTrainCoachPrice(train.TrainId, coachTypeId);
                            }
                            string noOfSeats = GetStringValue(coachTypeRow, "NoOfSeats", "0");
                            string coachLayout = GetStringValue(coachTypeRow, "CoachLayout");
                            string fleetTypeId = GetStringValue(coachTypeRow, "FleetTypeId");
                            string facilities = GetFleetTypeFacilities(fleetTypeId);

                            train.CoachTypes.Add(new CoachTypeGroup
                            {
                                CoachTypeId = coachTypeId,
                                CoachType = coachType,
                                NoOfCoaches = noOfCoaches,
                                TrainId = train.TrainId,
                                TripId = GetStringValue(coachTypeRow, "TripId"),
                                Price = price,
                                NoOfSeats = noOfSeats,
                                CoachLayout = coachLayout,
                                Facilities = facilities,
                                FleetTypeId = fleetTypeId
                            });

                            System.Diagnostics.Debug.WriteLine($"  ➕ Added: {coachType} - {noOfCoaches} coaches, Price: CDF {price}");
                        }

                        route.Trains.Add(train);
                        System.Diagnostics.Debug.WriteLine($"✅ Added train {train.TrainName} with {train.CoachTypes.Count} coach types");
                    }

                    route.TrainCount = route.Trains.Count;
                    routes.Add(route);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ GroupTripsByRoute Error: " + ex.Message);
            }

            return routes;
        }

        // ✅ NEW METHOD: Get price specific to trainId + coachTypeId
        private string GetTrainCoachPrice(string trainId, string coachTypeId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GETTING PRICE FOR TRAIN ===");
                System.Diagnostics.Debug.WriteLine($"Train ID: {trainId}");
                System.Diagnostics.Debug.WriteLine($"Coach Type ID: {coachTypeId}");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainTicketPrices/GetTrainTicketPrices";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JArray pricesArray = JArray.Parse(json);

                        // ✅ MATCH BY trainId + coachTypeId
                        var priceEntry = pricesArray.FirstOrDefault(p =>
                        {
                            string pTrainId = p["trainId"]?.ToString() ?? p["TrainId"]?.ToString();
                            string pCoachTypeId = p["coachTypeId"]?.ToString() ?? p["CoachTypeId"]?.ToString();

                            bool trainMatch = pTrainId == trainId;
                            bool coachMatch = pCoachTypeId == coachTypeId;

                            return trainMatch && coachMatch;
                        });

                        if (priceEntry != null)
                        {
                            // Use onlinePrice
                            decimal price = priceEntry["onlinePrice"]?.Value<decimal>()
                                         ?? priceEntry["OnlinePrice"]?.Value<decimal>()
                                         ?? priceEntry["price"]?.Value<decimal>()
                                         ?? priceEntry["Price"]?.Value<decimal>()
                                         ?? 0;

                            System.Diagnostics.Debug.WriteLine($"✓ Price found: CDF {price}");
                            return price.ToString("0.00");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"✗ No price found for trainId={trainId}, coachTypeId={coachTypeId}");
                            System.Diagnostics.Debug.WriteLine($"Available entries:");
                            foreach (var p in pricesArray)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - trainId={p["trainId"]}, coachTypeId={p["coachTypeId"]}, price={p["onlinePrice"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTrainCoachPrice Error: {ex.Message}");
            }

            return "0.00";
        }

        private int GetCoachCount(string trainId, string coachTypeId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TblTrainDetails/GetTblTrainDetails";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JArray detailsArray = JArray.Parse(json);

                        var matchingRecord = detailsArray.FirstOrDefault(d =>
                            (d["trainId"]?.ToString() ?? d["TrainId"]?.ToString()) == trainId &&
                            (d["coachTypeId"]?.ToString() ?? d["CoachTypeId"]?.ToString()) == coachTypeId
                        );

                        if (matchingRecord != null)
                        {
                            int count = matchingRecord["noOfCoach"]?.Value<int>() ?? matchingRecord["NoOfCoach"]?.Value<int>() ?? 0;
                            return count > 0 ? count : 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCoachCount Error: {ex.Message}");
            }

            return 1;
        }

        private string GetCoachTypeName(string coachTypeId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainCoachTypes/GetTrainCoachTypes";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JArray typesArray = JArray.Parse(json);

                        var matchingType = typesArray.FirstOrDefault(t =>
                            t["coachTypeId"]?.ToString() == coachTypeId ||
                            t["CoachTypeId"]?.ToString() == coachTypeId);

                        if (matchingType != null)
                        {
                            string typeName = matchingType["coachType"]?.ToString() ?? matchingType["CoachType"]?.ToString() ?? "Unknown";
                            return typeName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetCoachTypeName Error: " + ex.Message);
            }

            return "Unknown";
        }

        private string GetFleetTypeFacilities(string fleetTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(fleetTypeId))
                    return "";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainFleetTypes/GetTrainFleetTypes";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JArray fleetTypesArray = JArray.Parse(json);

                        var matchingFleetType = fleetTypesArray.FirstOrDefault(f =>
                            f["fleetTypeId"]?.ToString() == fleetTypeId ||
                            f["FleetTypeId"]?.ToString() == fleetTypeId);

                        if (matchingFleetType != null)
                        {
                            string facilities = matchingFleetType["facilities"]?.ToString() ?? matchingFleetType["Facilities"]?.ToString() ?? "";
                            return facilities;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetFleetTypeFacilities Error: {ex.Message}");
            }

            return "";
        }

        private string GetTrainDayOff(string trainId)
        {
            try
            {
                if (string.IsNullOrEmpty(trainId))
                    return "";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainsRegs/GetTblTrainsRegs";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JArray trainsArray = JArray.Parse(json);

                        var matchingTrain = trainsArray.FirstOrDefault(t =>
                            t["trainId"]?.ToString() == trainId ||
                            t["TrainId"]?.ToString() == trainId);

                        if (matchingTrain != null)
                        {
                            string dayOff = matchingTrain["dayOff"]?.ToString() ?? matchingTrain["DayOff"]?.ToString() ?? "";
                            return dayOff;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTrainDayOff Error: {ex.Message}");
            }

            return "";
        }

        private string GetStringValue(DataRow row, string columnName, string defaultValue = "")
        {
            try
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    var value = row[columnName];
                    if (value != null && value != DBNull.Value)
                    {
                        return value.ToString();
                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public class RouteGroup
        {
            public string RouteName { get; set; }
            public string StartStation { get; set; }
            public string EndStation { get; set; }
            public int TrainCount { get; set; }
            public List<TrainGroup> Trains { get; set; }
        }

        public class TrainGroup
        {
            public string TrainId { get; set; }
            public string TrainName { get; set; }
            public string TrainNumber { get; set; }
            public string DepartureTime { get; set; }
            public string ArrivalTime { get; set; }
            public string Duration { get; set; }
            public string DayOff { get; set; }
            public List<CoachTypeGroup> CoachTypes { get; set; }
        }

        public class CoachTypeGroup
        {
            public string CoachTypeId { get; set; }
            public string CoachType { get; set; }
            public int NoOfCoaches { get; set; }
            public string TrainId { get; set; }
            public string TripId { get; set; }
            public string Price { get; set; }
            public string NoOfSeats { get; set; }
            public string CoachLayout { get; set; }
            public string Facilities { get; set; }
            public string FleetTypeId { get; set; }
        }

        public class Coach
        {
            public string CoachNumber { get; set; }
            public int CoachId { get; set; }
            public string TripId { get; set; }
            public string FleetTypeId { get; set; }
            public string TrainId { get; set; }
            public string CoachTypeId { get; set; }
        }

        protected void rptRoutes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptTrains = (Repeater)e.Item.FindControl("rptTrains");

                if (rptTrains != null)
                {
                    RouteGroup routeData = (RouteGroup)e.Item.DataItem;
                    rptTrains.DataSource = routeData.Trains;
                    rptTrains.DataBind();
                }
            }
        }

        protected void rptTrains_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptCoachTypes = (Repeater)e.Item.FindControl("rptCoachTypes");

                if (rptCoachTypes != null)
                {
                    TrainGroup trainData = (TrainGroup)e.Item.DataItem;
                    rptCoachTypes.DataSource = trainData.CoachTypes;
                    rptCoachTypes.DataBind();
                }
            }
        }

        protected void rptCoachTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CoachTypeGroup coachTypeData = (CoachTypeGroup)e.Item.DataItem;

                Repeater rptCoaches = (Repeater)e.Item.FindControl("rptCoaches");
                if (rptCoaches != null)
                {
                    var coaches = new List<Coach>();

                    for (int i = 1; i <= coachTypeData.NoOfCoaches; i++)
                    {
                        coaches.Add(new Coach
                        {
                            CoachNumber = $"Coach {i}",
                            CoachId = i,
                            TripId = coachTypeData.TripId,
                            CoachTypeId = coachTypeData.CoachTypeId,
                            TrainId = coachTypeData.TrainId
                        });
                    }

                    rptCoaches.DataSource = coaches;
                    rptCoaches.DataBind();
                }
            }
        }

        protected void rptCoaches_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink lnkSelectSeat = (HyperLink)e.Item.FindControl("lnkSelectSeat");
                if (lnkSelectSeat != null)
                {
                    Coach coachData = (Coach)e.Item.DataItem;

                    lnkSelectSeat.NavigateUrl = $"~/Train_Seat_Selection.aspx?tripId={Server.UrlEncode(coachData.TripId)}" +
                        $"&coachTypeId={Server.UrlEncode(coachData.CoachTypeId)}" +
                        $"&coachId={coachData.CoachId}" +
                        $"&trainId={Server.UrlEncode(coachData.TrainId)}";


                }
            }
        }


        //protected void rptCoaches_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        HyperLink lnkSelectSeat = (HyperLink)e.Item.FindControl("lnkSelectSeat");
        //        if (lnkSelectSeat != null)
        //        {
        //            Coach coachData = (Coach)e.Item.DataItem;

        //            // ✅ "ALL" check fix - default to 0 if not selected
        //            string fromVal = ddlSource.SelectedValue;
        //            string toVal = ddlDestination.SelectedValue;

        //            string fromStationId = (string.IsNullOrEmpty(fromVal) || fromVal == "ALL") ? "0" : fromVal;
        //            string toStationId = (string.IsNullOrEmpty(toVal) || toVal == "ALL") ? "0" : toVal;

        //            lnkSelectSeat.NavigateUrl = $"~/Train_Seat_Selection.aspx" +
        //                $"?tripId={Server.UrlEncode(coachData.TripId)}" +
        //                $"&coachTypeId={Server.UrlEncode(coachData.CoachTypeId)}" +
        //                $"&coachId={coachData.CoachId}" +
        //                $"&trainId={Server.UrlEncode(coachData.TrainId)}" +
        //                $"&FromStationId={fromStationId}" +
        //                $"&ToStationId={toStationId}";
        //        }
        //    }
        //}
        private void BindTripDetails(string tripId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainTrips/GetTrainTripDetailsByTripId?tripId={tripId}";
                    HttpResponseMessage res = client.GetAsync(url).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var json = res.Content.ReadAsStringAsync().Result;
                        JObject tripObject = JObject.Parse(json);

                        JArray pricesArray = tripObject["prices"] as JArray;
                        JArray expandedArray = new JArray();

                        if (pricesArray != null && pricesArray.Count > 0)
                        {
                            foreach (var price in pricesArray)
                            {
                                JObject row = new JObject();

                                // Saare main trip fields copy karo (prices chhod ke)
                                foreach (var prop in tripObject.Properties())
                                {
                                    if (prop.Name != "prices")
                                        row[prop.Name] = prop.Value;
                                }

                                // Har price entry ke liye coach type override karo
                                var coachTypeObj = price["coachType"] as JObject;
                                if (coachTypeObj != null)
                                {
                                    row["CoachTypeId"] = coachTypeObj["coachTypeId"]?.ToString();
                                    row["coachType"] = coachTypeObj["coachType"]?.ToString();
                                }

                                row["Price"] = price["onlinePrice"]?.ToString();

                                expandedArray.Add(row);
                            }
                        }
                        else
                        {
                            // prices array nahi mila - original trip as-is use karo
                            expandedArray.Add(tripObject);
                        }

                        DataTable dt = JsonArrayToDataTable(expandedArray);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var routeGroups = GroupTripsByRoute(dt);
                            rptRoutes.DataSource = routeGroups;
                            rptRoutes.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindTripDetails Error: " + ex.Message);
            }
        }
        private void BindTrainTripsGroupedByRoute()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = apiUrl.TrimEnd('/') + "/TrainTrips/GetTrainTripDetailsByTrainId";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseMessage = response.Content.ReadAsStringAsync().Result;
                        JArray jsonArray = JArray.Parse(responseMessage);

                        // ✅ Har trip ke liye prices expand karo
                        JArray expandedArray = ExpandTripsByCoachType(jsonArray);

                        DataTable dt = JsonArrayToDataTable(expandedArray);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (selectedScheduleId.HasValue)
                                dt = FilterBySchedule(dt, selectedScheduleId.Value);

                            dt = FilterByJourneyDate(dt);

                            var routeGroups = GroupTripsByRoute(dt);
                            rptRoutes.DataSource = routeGroups;
                            rptRoutes.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ BindTrainTripsGroupedByRoute Error: " + ex.Message);
            }
        }

        // ✅ Naya method - har trip ke liye TripId se prices fetch karo
        private JArray ExpandTripsByCoachType(JArray tripsArray)
        {
            JArray expandedArray = new JArray();

            try
            {
                // Unique TripIds nikalo
                var tripIds = tripsArray
                    .Select(t => t["TripId"]?.ToString() ?? t["tripId"]?.ToString())
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                foreach (var tripId in tripIds)
                {
                    // Original trip row lo
                    JObject originalTrip = tripsArray
                        .FirstOrDefault(t =>
                            (t["TripId"]?.ToString() ?? t["tripId"]?.ToString()) == tripId)
                        as JObject;

                    if (originalTrip == null) continue;

                    // Is trip ke liye prices fetch karo
                    JArray prices = GetTripPrices(tripId);

                    if (prices != null && prices.Count > 0)
                    {
                        // Har price/coach type ke liye alag row banao
                        foreach (var price in prices)
                        {
                            JObject newRow = new JObject();

                            // Original trip fields copy karo
                            foreach (var prop in originalTrip.Properties())
                                newRow[prop.Name] = prop.Value;

                            // Coach type override karo
                            var coachTypeObj = price["coachType"] as JObject;
                            if (coachTypeObj != null)
                            {
                                newRow["CoachTypeId"] = coachTypeObj["coachTypeId"]?.ToString();
                                newRow["coachType"] = coachTypeObj["coachType"]?.ToString();
                            }

                            newRow["Price"] = price["onlinePrice"]?.ToString();

                            expandedArray.Add(newRow);
                        }
                    }
                    else
                    {
                        // Prices nahi mili - original row as-is add karo
                        expandedArray.Add(originalTrip);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ExpandTripsByCoachType Error: " + ex.Message);
                return tripsArray; // Fallback
            }

            return expandedArray;
        }

        // ✅ Ek trip ke prices fetch karo
        private JArray GetTripPrices(string tripId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = $"{apiUrl.TrimEnd('/')}/TrainTrips/GetTrainTripDetailsByTripId?tripId={tripId}";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        JObject tripObj = JObject.Parse(json);

                        JArray prices = tripObj["prices"] as JArray;
                        return prices;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTripPrices Error for tripId {tripId}: " + ex.Message);
            }

            return null;
        }

        //private void BindTripDetails(string tripId)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            string url = $"{apiUrl.TrimEnd('/')}/TrainTrips/GetTrainTripDetailsByTripId?tripId={tripId}";
        //            HttpResponseMessage res = client.GetAsync(url).Result;

        //            if (res.IsSuccessStatusCode)
        //            {
        //                var json = res.Content.ReadAsStringAsync().Result;
        //                JObject tripObject = JObject.Parse(json);
        //                JArray tripArray = new JArray { tripObject };

        //                DataTable dt = JsonArrayToDataTable(tripArray);

        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    var routeGroups = GroupTripsByRoute(dt);
        //                    rptRoutes.DataSource = routeGroups;
        //                    rptRoutes.DataBind();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("BindTripDetails Error: " + ex.Message);
        //    }
        //}

        private void LoadStations()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = client.GetAsync(apiUrl + "RailwayStations/GetRailwayStations").Result;

                    if (Res.IsSuccessStatusCode)
                    {
                        var ResponseMessage = Res.Content.ReadAsStringAsync().Result;
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(ResponseMessage, typeof(DataTable));

                        ddlSource.DataSource = dt;
                        ddlSource.DataTextField = "StationName";
                        ddlSource.DataValueField = "stationId";
                        ddlSource.DataBind();
                        ddlSource.Items.Insert(0, new ListItem("From Station", "ALL"));

                        ddlDestination.DataSource = dt;
                        ddlDestination.DataTextField = "StationName";
                        ddlDestination.DataValueField = "stationId";
                        ddlDestination.DataBind();
                        ddlDestination.Items.Insert(0, new ListItem("To Station", "ALL"));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadStations Error: " + ex.Message);
            }
        }

        private void LoadSchedules()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string url = apiUrl.TrimEnd('/') + "/TrainSchedules/GetTrainSchedules";
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
                            rptSchedules.DataSource = dt;
                            rptSchedules.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadSchedules Error: " + ex.Message);
            }
        }

        protected string GetClassIcon(string className)
        {
            if (string.IsNullOrEmpty(className)) return "<i class='las la-train'></i>";

            className = className.ToLower();
            if (className.Contains("luxury") || className.Contains("luxe")) return "<i class='las la-gem'></i>";
            if (className.Contains("first")) return "<i class='las la-bed'></i>";
            if (className.Contains("second")) return "<i class='las la-chair'></i>";
            return "<i class='las la-train'></i>";
        }

        protected string GetClassBadgeColor(string className)
        {
            if (string.IsNullOrEmpty(className)) return "";

            className = className.ToLower();
            if (className.Contains("luxury") || className.Contains("luxe")) return "luxury-badge";
            if (className.Contains("first")) return "first-class-badge";
            if (className.Contains("second")) return "second-class-badge";
            return "";
        }

        protected string GetDayNames(object dayOffObj)
        {
            if (dayOffObj == null || dayOffObj == DBNull.Value || string.IsNullOrWhiteSpace(dayOffObj.ToString()))
                return "Every day available";

            try
            {
                string dayOffStr = dayOffObj.ToString().Trim();

                if (dayOffStr == "[]" || dayOffStr == "")
                    return "Every day available";

                // API format: 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday, 7=Sunday
                string[] dayNames = { "", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                List<string> result = new List<string>();

                // Remove brackets if present
                if (dayOffStr.StartsWith("["))
                {
                    dayOffStr = dayOffStr.Trim('[', ']').Trim();
                }

                if (string.IsNullOrWhiteSpace(dayOffStr))
                    return "Every day available";

                // Split by comma and process each number
                string[] numbers = dayOffStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var numStr in numbers)
                {
                    string cleanNum = numStr.Trim();
                    if (int.TryParse(cleanNum, out int n) && n >= 1 && n <= 7)
                    {
                        result.Add(dayNames[n]);
                    }
                }

                if (result.Count == 0)
                    return "Every day available";

                return string.Join(", ", result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetDayNames Error: {ex.Message}");
                return "Every day available";
            }
        }

        protected string FormatFacilities(object facilitiesObj)
        {
            if (facilitiesObj == null || facilitiesObj == DBNull.Value)
                return "No facilities listed";

            try
            {
                string facilitiesStr = facilitiesObj.ToString().Trim();

                if (string.IsNullOrWhiteSpace(facilitiesStr) || facilitiesStr == "[]")
                    return "No facilities listed";

                if (facilitiesStr.StartsWith("["))
                {
                    var facilities = JsonConvert.DeserializeObject<List<string>>(facilitiesStr);
                    if (facilities != null && facilities.Count > 0)
                    {
                        return string.Join("&nbsp;&nbsp;&nbsp;&nbsp;", facilities);
                    }
                }
                else if (facilitiesStr.Contains(","))
                {
                    var facilities = facilitiesStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(f => f.Trim())
                                                 .Where(f => !string.IsNullOrWhiteSpace(f))
                                                 .ToList();

                    if (facilities.Count > 0)
                    {
                        return string.Join("&nbsp;&nbsp;&nbsp;&nbsp;", facilities);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(facilitiesStr))
                {
                    return facilitiesStr;
                }

                return "No facilities listed";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatFacilities Error: {ex.Message}");

                string facilitiesStr = facilitiesObj.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(facilitiesStr) && facilitiesStr != "[]")
                {
                    return facilitiesStr;
                }

                return "No facilities listed";
            }
        }

        protected void btnFindTickets_Click(object sender, EventArgs e)
        {
            int pickupId = 0;
            int droppingId = 0;
            string journeyDate = txtJourneyDate.Text;

            if (string.IsNullOrWhiteSpace(ddlSource.SelectedValue) || ddlSource.SelectedValue == "ALL" ||
                string.IsNullOrWhiteSpace(ddlDestination.SelectedValue) || ddlDestination.SelectedValue == "ALL" ||
                string.IsNullOrWhiteSpace(journeyDate))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Please select from station, to station, and journey date.');", true);
                return;
            }

            if (!int.TryParse(ddlSource.SelectedValue, out pickupId) || !int.TryParse(ddlDestination.SelectedValue, out droppingId))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Invalid station selection.');", true);
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var url = $"{apiUrl.TrimEnd('/')}/TrainTrips/GetTrainTripsByRoute" +
                              $"?startStationId={pickupId}" +
                              $"&endStationId={droppingId}" +
                              $"&dateOfJourney={System.Web.HttpUtility.UrlEncode(journeyDate)}";

                    var res = client.GetAsync(url).Result;
                    var json = res.Content.ReadAsStringAsync().Result;

                    DataTable dt = new DataTable();

                    if (res.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(json))
                    {
                        try
                        {
                            var responseObj = JObject.Parse(json);

                            if (responseObj["success"] != null && responseObj["success"].Value<bool>())
                            {
                                var tripsArray = responseObj["trips"] as JArray;
                                if (tripsArray != null && tripsArray.HasValues)
                                {
                                    dt = JsonArrayToDataTable(tripsArray);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("JSON Parsing Error: " + ex.Message);
                        }
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt = FilterByJourneyDate(dt);

                        if (selectedScheduleId.HasValue)
                        {
                            dt = FilterBySchedule(dt, selectedScheduleId.Value);
                        }

                        //dt = FilterByClass(dt);
                      

                        var routeGroups = GroupTripsByRoute(dt);
                        rptRoutes.DataSource = routeGroups;
                        rptRoutes.DataBind();

                        if (routeGroups.Count == 0)
                        {
                            ClientScript.RegisterStartupScript(GetType(), "alert",
                                "alert('No train trips found for the selected route and date.');", true);
                        }
                    }
                    else
                    {
                        rptRoutes.DataSource = null;
                        rptRoutes.DataBind();
                        ClientScript.RegisterStartupScript(GetType(), "alert",
                            "alert('No train trips found for the selected route and date.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Search Error: " + ex.Message);
            }
        }

        protected void btnResetFilters_Click(object sender, EventArgs e)
        {
            try
            {
                selectedScheduleId = null;

                foreach (RepeaterItem item in rptSchedules.Items)
                {
                    CheckBox cb = item.FindControl("chkSchedule") as CheckBox;
                    if (cb != null)
                    {
                        cb.Checked = false;
                    }
                }

                //chkLuxury.Checked = false;
                //chkFirstClass.Checked = false;
                //chkSecondClass.Checked = false;

             

                BindTrainTripsGroupedByRoute();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ResetFilters Error: " + ex.Message);
            }
        }

        protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (RepeaterItem item in rptSchedules.Items)
                {
                    CheckBox cb = item.FindControl("chkSchedule") as CheckBox;
                    if (cb != null && cb != sender)
                    {
                        cb.Checked = false;
                    }
                }

                CheckBox currentCheckBox = sender as CheckBox;
                if (currentCheckBox != null && currentCheckBox.Checked)
                {
                    RepeaterItem item = currentCheckBox.NamingContainer as RepeaterItem;
                    HiddenField hdn = item.FindControl("hdnScheduleId") as HiddenField;

                    if (hdn != null && !string.IsNullOrEmpty(hdn.Value))
                    {
                        if (int.TryParse(hdn.Value, out int scheduleId))
                        {
                            selectedScheduleId = scheduleId;
                            BindTrainTripsGroupedByRoute();
                        }
                    }
                }
                else
                {
                    selectedScheduleId = null;
                    BindTrainTripsGroupedByRoute();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CheckedChanged Error: " + ex.Message);
            }
        }

        protected void chkClass_CheckedChanged(object sender, EventArgs e)
        {
            BindTrainTripsGroupedByRoute();
        }

        protected void btnApplyPrice_Click(object sender, EventArgs e)
        {
            BindTrainTripsGroupedByRoute();
        }
    }
}