using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;

namespace Excel_Bus
{
    public partial class TrainUserMaster : System.Web.UI.MasterPage
    {
        string apiUrl = System.Configuration.ConfigurationSettings.AppSettings["api_path"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // You can add train-specific initialization here
                // For example: LoadTrainRoutes();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Train.aspx", false);
        }

        // Optional: Method to load train routes dynamically (similar to bus trips)
        private void LoadTrainRoutes()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string url = apiUrl.TrimEnd('/') + "/Train/GetRoutes";
                    System.Diagnostics.Debug.WriteLine("Fetching train routes from: " + url);

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("Train Routes API Response: " + json);

                        // Process the routes as needed
                        // var routes = JsonConvert.DeserializeObject<List<TrainRoute>>(json);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Train Routes API failed: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading train routes: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }
    }

}