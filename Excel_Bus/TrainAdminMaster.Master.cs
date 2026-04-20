using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.IO;
using System.Web.UI.HtmlControls;


namespace Excel_Bus
{
    public partial class TrainAdminMaster : System.Web.UI.MasterPage
    {
        string token_sess;
        private HttpClient client = new HttpClient();
        string apiUrl = ConfigurationManager.AppSettings["api_path"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        // Check if admin is logged in
        //        if (Session["AdminId"] == null)
        //        {
        //            Response.Redirect("~/TrainAdminLogin.aspx");
        //            return;
        //        }
        //        user_access(Session["RoleId"].ToString());
        //       // LoadAdminProfile();
        //       // LoadNotifications();
        //      //  LoadBadgeCounts();
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AdminId"] == null)
                {
                    Response.Redirect("~/TrainAdminLogin.aspx");
                    return;
                }
                user_access(Session["RoleId"].ToString());
            }
        }

        //private void LoadAdminProfile()
        //{
        //    try
        //    {
        //        int adminId = Convert.ToInt32(Session["AdminId"]);
        //        ConfigureHttpClient();

        //        var response = client.GetAsync($"{apiUrl}/Admin/GetProfile?adminId={adminId}").Result;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var jsonResult = response.Content.ReadAsStringAsync().Result;
        //            var profile = JsonConvert.DeserializeObject<TrainAdminProfile>(jsonResult);

        //            adminName.InnerText = profile.Name;

        //            if (!string.IsNullOrEmpty(profile.ProfileImage))
        //            {
        //                imgAdminProfile.ImageUrl = profile.ProfileImage;
        //            }
        //            else
        //            {
        //                imgAdminProfile.ImageUrl = "~/assets/admin/images/profile/default.png";
        //            }
        //        }
        //        else
        //        {
        //            UseDummyProfile();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error loading admin profile: " + ex.Message);
        //        UseDummyProfile();
        //    }
        //}

        //private void UseDummyProfile()
        //{
        //    adminName.InnerText = "Admin User";
        //    imgAdminProfile.ImageUrl = "~/assets/admin/images/profile/default.png";
        //}

        private void LoadNotifications()
        {
            try
            {
                int adminId = Convert.ToInt32(Session["AdminId"]);
                ConfigureHttpClient();

                var response = client.GetAsync($"{apiUrl}/Admin/GetNotifications?adminId={adminId}&limit=10").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var notifications = JsonConvert.DeserializeObject<List<TrainAdminNotification>>(jsonResult);

                    // Add TimeAgo to each notification
                    foreach (var notification in notifications)
                    {
                        notification.TimeAgo = GetTimeAgo(notification.CreatedDate);
                    }

                    // rptNotifications.DataSource = notifications;
                    // rptNotifications.DataBind();

                    // Count unread notifications
                    int unreadCount = notifications.Count(n => !n.IsRead);

                    if (unreadCount > 9)
                    {
                        // notificationCount.InnerText = "9+";
                    }
                    else if (unreadCount > 0)
                    {
                        // notificationCount.InnerText = unreadCount.ToString();
                    }
                    else
                    {
                        // notificationCount.Visible = false;
                    }

                    //notificationMessage.InnerText = unreadCount > 0
                    // ? $"You have {unreadCount} unread notification{(unreadCount > 1 ? "s" : "")}"
                    //: "You have 0 unread notification";
                }
                else
                {
                    UseDummyNotifications();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading notifications: " + ex.Message);
                UseDummyNotifications();
            }
        }

        private void UseDummyNotifications()
        {
            var dummyNotifications = new List<TrainAdminNotification>
            {
                new TrainAdminNotification
                {
                    Id = 1,
                    Title = "New Booking Received",
                    Message = "A new booking has been made by John Doe",
                    CreatedDate = DateTime.Now.AddMinutes(-15),
                    IsRead = false,
                    TimeAgo = "15 minutes ago"
                },
                new TrainAdminNotification
                {
                    Id = 2,
                    Title = "Payment Pending",
                    Message = "Payment verification required for PNR001234",
                    CreatedDate = DateTime.Now.AddHours(-2),
                    IsRead = false,
                    TimeAgo = "2 hours ago"
                },
                new TrainAdminNotification
                {
                    Id = 3,
                    Title = "New User Registration",
                    Message = "Sarah Williams has registered",
                    CreatedDate = DateTime.Now.AddHours(-5),
                    IsRead = true,
                    TimeAgo = "5 hours ago"
                }
            };

            // rptNotifications.DataSource = dummyNotifications;
            // rptNotifications.DataBind();

            int unreadCount = dummyNotifications.Count(n => !n.IsRead);

            if (unreadCount > 0)
            {
                // notificationCount.InnerText = unreadCount.ToString();
                //notificationMessage.InnerText = $"You have {unreadCount} unread notification{(unreadCount > 1 ? "s" : "")}";
            }
            else
            {
                // notificationCount.Visible = false;
                // notificationMessage.InnerText = "You have 0 unread notification";
            }
        }

        private void LoadBadgeCounts()
        {
            try
            {
                ConfigureHttpClient();

                var response = client.GetAsync($"{apiUrl}/Admin/GetBadgeCounts").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var badges = JsonConvert.DeserializeObject<TrainBadgeCounts>(jsonResult);

                    // Pending Payments Badge
                    if (badges.PendingPayments > 0)
                    {
                        // badgePendingPayments.InnerText = badges.PendingPayments.ToString();
                    }
                    else
                    {
                        // badgePendingPayments.Visible = false;
                    }

                    // Pending Tickets Badge
                    if (badges.PendingTickets > 0)
                    {
                        // badgePendingTickets.InnerText = badges.PendingTickets.ToString();
                    }
                    else
                    {
                        //badgePendingTickets.Visible = false;
                    }

                    // Pending Support Badge
                    if (badges.PendingSupport > 0)
                    {
                        //badgePendingSupport.InnerText = badges.PendingSupport.ToString();
                    }
                    else
                    {
                        //badgePendingSupport.Visible = false;
                    }
                }
                else
                {
                    //UseDummyBadges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading badge counts: " + ex.Message);
                //UseDummyBadges();
            }
        }

        //private void UseDummyBadges()
        //{
        //    // Dummy badge counts
        //    badgePendingPayments.InnerText = "5";
        //    badgePendingTickets.InnerText = "8";
        //    //badgePendingSupport.InnerText = "3";
        //}

        private string GetTimeAgo(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) > 1 ? "s" : "")} ago";

            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) > 1 ? "s" : "")} ago";
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                int adminId = Convert.ToInt32(Session["AdminId"]);

                // Log the logout activity via API
                LogActivity(adminId, "Logout", "Admin logged out successfully");

                // Clear session
                Session.Clear();
                Session.Abandon();

                // Redirect to login page
                Response.Redirect("~/Train.aspx");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error during logout: " + ex.Message);

                // Even if logging fails, still logout
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/Train.aspx");
            }
        }

        private void LogActivity(int adminId, string action, string description)
        {
            try
            {
                ConfigureHttpClient();

                var activityLog = new
                {
                    AdminId = adminId,
                    Action = action,
                    Description = description,
                    IpAddress = GetUserIP(),
                    UserAgent = Request.UserAgent ?? "",
                    CreatedDate = DateTime.Now
                };

                var jsonContent = JsonConvert.SerializeObject(activityLog);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = client.PostAsync($"{apiUrl}/Admin/LogActivity", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to log activity via API");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error logging activity: " + ex.Message);
            }
        }

        private string GetUserIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                ip = ip.Split(',')[0].Trim();
            }

            return ip ?? "0.0.0.0";
        }

        private void ConfigureHttpClient()
        {
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public void ShowNotification(string type, string message)
        {
            string script = $"notify('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "notification", script, true);
        }




        void user_access(string userId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_sess);
                    HttpResponseMessage Res = client.GetAsync(apiUrl + string.Format("TblAccessRight/GetTblAccessRight?UserId=" + userId)).Result;

                    if (Res.IsSuccessStatusCode)
                    {
                        var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(EmpResponse, (typeof(DataTable)));

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                Boolean is_active = Convert.ToBoolean(dr["IsActive"].ToString());
                                string m_id = dr["ModuleId"].ToString();
                                string sm_id = dr["SubModuleId"].ToString();

                                if (is_active)
                                {
                                    Control myControl1 = FindControl(m_id);
                                    myControl1.Visible = true;

                                    Control myControl2 = FindControl(sm_id);
                                    myControl2.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }
    }

    #region API Models for Master Page

    public class TrainAdminProfile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("profileImage")]
        public string ProfileImage { get; set; }
    }

    public class TrainAdminNotification
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("isRead")]
        public bool IsRead { get; set; }

        public string TimeAgo { get; set; }
    }

    public class TrainBadgeCounts
    {
        [JsonProperty("pendingPayments")]
        public int PendingPayments { get; set; }

        [JsonProperty("pendingTickets")]
        public int PendingTickets { get; set; }

        [JsonProperty("pendingSupport")]
        public int PendingSupport { get; set; }
    }

    #endregion
}