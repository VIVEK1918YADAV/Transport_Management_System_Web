using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus.TrainAdmin
{
    public partial class AllUsers : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        // Static constructor to initialize HttpClient once
        static AllUsers()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                // Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
            protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user ID is in query string
                string userId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(userId))
                {
                    RegisterAsyncTask(new PageAsyncTask(() => LoadUserDetails(userId)));
                }
                else
                {
                    // Register async task for WebForms
                    RegisterAsyncTask(new PageAsyncTask(() => LoadAllUsers()));
                }
            }
        }

        private async Task LoadAllUsers(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;
                pnlUsersList.Visible = true;
                pnlUserDetails.Visible = false;

                // Build API endpoint
                string endpoint = "TrainUsers/GetTrainUsers";

                // Add search parameter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    endpoint += $"?search={Uri.EscapeDataString(searchTerm)}";
                }

                // Make API call
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<TrainUser> users = JsonConvert.DeserializeObject<List<TrainUser>>(jsonResponse);

                    // Get all users (no filtering) and order by created date
                    var allUsers = users.OrderByDescending(u => u.CreatedAt).ToList();

                    // Store in ViewState for search functionality
                    ViewState["AllUsers"] = JsonConvert.SerializeObject(allUsers);

                    // Bind to GridView
                    gvUsers.DataSource = allUsers;
                    gvUsers.DataBind();
                }
                else
                {
                    ShowError($"Failed to load users. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading users: {ex.Message}");
            }
        }

        private async Task LoadUserDetails(string userId)
        {
            try
            {
                pnlError.Visible = false;
                pnlUsersList.Visible = false;
                pnlUserDetails.Visible = true;

                // Make API call to get single user
                string endpoint = $"TrainUsers/GetTrainUserById/{userId}";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"User Details Response: {jsonResponse}");

                    TrainUser user = JsonConvert.DeserializeObject<TrainUser>(jsonResponse);

                    // Store in ViewState
                    ViewState["CurrentUser"] = JsonConvert.SerializeObject(user);

                    // Populate user details
                    DisplayUserDetails(user);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    ShowError($"Failed to load user details. Status Code: {response.StatusCode}");
                    pnlUsersList.Visible = true;
                    pnlUserDetails.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user details: {ex.Message}");
                ShowError($"Error loading user details: {ex.Message}");
                pnlUsersList.Visible = true;
                pnlUserDetails.Visible = false;
            }
        }

        private void DisplayUserDetails(TrainUser user)
        {
            lblUserName.Text = user.UserName ?? "N/A";
            lblEmail.Text = user.Email ?? "N/A";
            lblCountry.Text = !string.IsNullOrEmpty(user.CountryName) ? user.CountryName : "N/A";
            lblCity.Text = !string.IsNullOrEmpty(user.City) ? user.City : "N/A";
            lblState.Text = !string.IsNullOrEmpty(user.State) ? user.State : "N/A";
            lblZip.Text = !string.IsNullOrEmpty(user.Zip) ? user.Zip : "N/A";
            lblAddress.Text = !string.IsNullOrEmpty(user.Address) ? user.Address : "N/A";

            // ✅ Balance - string to decimal convert
            lblBalance.Text = $"{user.Balance:N2} CDF";

            // ✅ Status - string comparison
            lblStatus.Text = user.Status?.ToLower() == "active"
                ? "<span class='status-badge status-active'>Active</span>"
                : "<span class='status-badge status-banned'>InActive</span>";


            lblJoinedAt.Text = FormatDateTime(user.CreatedAt);
            lblUpdatedAt.Text = FormatDateTime(user.UpdatedAt);
        }
        private string FormatDateTime(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return "N/A";

            if (DateTime.TryParse(dateStr, out DateTime dt))
                return dt.ToString("dd MMM yyyy, hh:mm tt");

            return dateStr; // parse na ho toh as-is return karo
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Perform client-side filtering if data is in ViewState
                if (ViewState["AllUsers"] != null)
                {
                    string json = ViewState["AllUsers"].ToString();
                    List<TrainUser> allUsers = JsonConvert.DeserializeObject<List<TrainUser>>(json);

                    var filteredUsers = allUsers.Where(u =>
                        (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(searchTerm.ToLower())) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchTerm.ToLower())) ||
                        (!string.IsNullOrEmpty(u.Firstname) && u.Firstname.ToLower().Contains(searchTerm.ToLower())) ||
                        (!string.IsNullOrEmpty(u.Lastname) && u.Lastname.ToLower().Contains(searchTerm.ToLower()))
                    ).ToList();

                    gvUsers.DataSource = filteredUsers;
                    gvUsers.DataBind();
                }
                else
                {
                    // If ViewState is empty, reload from API
                    RegisterAsyncTask(new PageAsyncTask(() => LoadAllUsers(searchTerm)));
                }
            }
            else
            {
                // Reload all users if search is cleared
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllUsers()));
            }
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            // Clear query string and reload users list
            Response.Redirect(Request.Path);
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                string userId = e.CommandArgument.ToString();
                // Redirect with query string
                Response.Redirect($"{Request.Path}?id={userId}");
            }
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Additional row customization if needed
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // You can add additional styling or logic here
            }
        }

        // Helper method to format date time
        protected string FormatDateTime(object dateTime)
        {
            if (dateTime == null) return "N/A";

            DateTime dt;
            if (dateTime is DateTime)
            {
                dt = (DateTime)dateTime;
            }
            else if (DateTime.TryParse(dateTime.ToString(), out dt))
            {
                // Parsed successfully
            }
            else
            {
                return dateTime.ToString();
            }

            return dt.ToString("yyyy-MM-dd hh:mm tt");
        }

        // Helper method to get relative time
        protected string GetRelativeTime(object dateTime)
        {
            if (dateTime == null) return "";

            DateTime dt;
            if (dateTime is DateTime)
            {
                dt = (DateTime)dateTime;
            }
            else if (DateTime.TryParse(dateTime.ToString(), out dt))
            {
                // Parsed successfully
            }
            else
            {
                return "";
            }

            TimeSpan diff = DateTime.Now - dt;

            if (diff.TotalMinutes < 1)
                return "just now";
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes} minute{((int)diff.TotalMinutes != 1 ? "s" : "")} ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} hour{((int)diff.TotalHours != 1 ? "s" : "")} ago";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} day{((int)diff.TotalDays != 1 ? "s" : "")} ago";
            if (diff.TotalDays < 30)
            {
                int weeks = (int)(diff.TotalDays / 7);
                return $"{weeks} week{(weeks != 1 ? "s" : "")} ago";
            }
            if (diff.TotalDays < 365)
            {
                int months = (int)(diff.TotalDays / 30);
                return $"{months} month{(months != 1 ? "s" : "")} ago";
            }

            int years = (int)(diff.TotalDays / 365);
            return $"{years} year{(years != 1 ? "s" : "")} ago";
        }

        // Helper method to format balance
        protected string FormatBalance(object balance)
        {
            if (balance == null) return "0.00";

            decimal amount;
            if (balance is decimal)
            {
                amount = (decimal)balance;
            }
            else if (decimal.TryParse(balance.ToString(), out amount))
            {
                // Parsed successfully
            }
            else
            {
                return "0.00";
            }

            return amount.ToString("N2");
        }

        // Helper method to get status badge
        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";

            string statusValue = status.ToString();

            if (statusValue == "ACTIVE")
            {
                return "<span class='status-badge status-active'>Active</span>";
            }
            else
            {
                return "<span class='status-badge status-banned'>InActive</span>";
            }
        }

        // Helper method to get status class for row
        protected string GetStatusClass(object status)
        {
            if (status == null) return "";

            string statusValue = status.ToString();
            return statusValue == "true" ? "active-user-row" : "banned-user-row";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }
    }
}