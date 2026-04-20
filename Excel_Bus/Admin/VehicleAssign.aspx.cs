using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class VehicleAssign : System.Web.UI.Page
    {
        private static readonly HttpClient client;
        private static readonly string apiUrl;

        static VehicleAssign()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_path"];

            client = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(() => LoadAllVehicleData()));
            }
        }

        private async Task LoadAllVehicleData()
        {
            await LoadTrips();
            await LoadVehicles();
            await LoadAssignedVehicles();
        }


        private async Task LoadAssignedVehicles(string searchTerm = "")
        {
            try
            {
                pnlError.Visible = false;

                string endpoint = "AssignedVehicles/GetAssignedVehicles";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<AssignedVehicle> assignedVehicles = JsonConvert.DeserializeObject<List<AssignedVehicle>>(jsonResponse);

                    var trips = await GetTrips();
                    var vehicles = await GetVehicles();

                    foreach (var av in assignedVehicles)
                    {
                        var trip = trips.FirstOrDefault(t => t.Id == av.TripId);
                        var vehicle = vehicles.FirstOrDefault(v => v.Id == av.VehicleId);

                        av.TripTitle = trip?.Title ?? "Unknown";
                        av.VehicleNickName = vehicle?.NickName ?? "Unknown";
                        av.RegisterNo = vehicle?.RegisterNo ?? "N/A";
                    }

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        assignedVehicles = assignedVehicles.Where(av =>
                            (!string.IsNullOrEmpty(av.TripTitle) && av.TripTitle.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(av.VehicleNickName) && av.VehicleNickName.ToLower().Contains(searchTerm.ToLower())) ||
                            (!string.IsNullOrEmpty(av.RegisterNo) && av.RegisterNo.ToLower().Contains(searchTerm.ToLower()))
                        ).ToList();
                    }

                    var orderedVehicles = assignedVehicles.OrderByDescending(av => av.CreatedAt).ToList();

                    ViewState["AssignedVehicles"] = JsonConvert.SerializeObject(orderedVehicles);

                    gvAssignedVehicles.DataSource = orderedVehicles;
                    gvAssignedVehicles.DataBind();

                    upGridView.Update();
                }
                else
                {
                    ShowError($"Failed to load assigned vehicles. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading assigned vehicles: {ex.Message}");
            }
        }

        private async Task LoadTrips()
        {
            try
            {
                var trips = await GetTrips();

                ddlTripAdd.Items.Clear();
                ddlTripAdd.DataSource = trips;
                ddlTripAdd.DataTextField = "Title";
                ddlTripAdd.DataValueField = "Id";
                ddlTripAdd.DataBind();
                ddlTripAdd.Items.Insert(0, new ListItem("Select an option", ""));

                ddlTripEdit.Items.Clear();
                ddlTripEdit.DataSource = trips;
                ddlTripEdit.DataTextField = "Title";
                ddlTripEdit.DataValueField = "Id";
                ddlTripEdit.DataBind();
                ddlTripEdit.Items.Insert(0, new ListItem("Select an option", ""));
            }
            catch (Exception ex)
            {
                ShowError($"Error loading trips: {ex.Message}");
            }
        }

        private async Task LoadVehicles()
        {
            try
            {
                var vehicles = await GetVehicles();
                ViewState["AllVehicles"] = JsonConvert.SerializeObject(vehicles);
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vehicles: {ex.Message}");
            }
        }

        private async Task<List<Trip>> GetTrips()
        {
            HttpResponseMessage response = await client.GetAsync("Trips/GetTrips");
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Trip>>(jsonResponse);
            }
            return new List<Trip>();
        }

        private async Task<List<Vehicle>> GetVehicles()
        {
            HttpResponseMessage response = await client.GetAsync("Vehicles/GetVehicles");
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Vehicle>>(jsonResponse);
            }
            return new List<Vehicle>();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            RegisterAsyncTask(new PageAsyncTask(() => LoadAssignedVehicles(searchTerm)));
        }



        protected void btnSubmitAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                RegisterAsyncTask(new PageAsyncTask(AddAssignment));
            }
        }

        private async Task AddAssignment()
        {
            try
            {
                if (string.IsNullOrEmpty(ddlTripAdd.SelectedValue) || string.IsNullOrEmpty(ddlVehicleAdd.SelectedValue))
                {
                    ShowError("Please select both Trip and Vehicle");
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showAddModal();", true);
                    return;
                }

                var newAssignment = new AssignedVehicle
                {
                    TripId = int.Parse(ddlTripAdd.SelectedValue),
                    VehicleId = int.Parse(ddlVehicleAdd.SelectedValue),
                    StartFrom = "07:00:00",
                    EndAt = "16:00:00",
                    Status = "1",  // ✅ Changed to string
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                string jsonContent = JsonConvert.SerializeObject(newAssignment);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("AssignedVehicles/PostAssignedVehicle", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAssignedVehicles();
                    ClearForm();

                    // Force close modal and show success
                    ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAdd_" + DateTime.Now.Ticks,
                        @"setTimeout(function() { 
                            hideModal(); 
                            modalOpenFlag = false;
                            showSuccess('Vehicle assigned successfully!');
                        }, 100);", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to assign vehicle: {errorMessage}");
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showAddModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error assigning vehicle: {ex.Message}");
                upModal.Update();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showAddModal();", true);
            }
        }


        protected void btnSubmitEdit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int id = int.Parse(hdnEditId.Value);
                RegisterAsyncTask(new PageAsyncTask(async () => await UpdateAssignment(id)));
            }
        }

        protected void gvAssignedVehicles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditRecord")
            {
                // Clear any previous modal state
                ScriptManager.RegisterStartupScript(this, GetType(), "HideBeforeEdit", "hideModal();", true);
                RegisterAsyncTask(new PageAsyncTask(async () => await LoadEditData(id)));
            }
            else if (e.CommandName == "DisableRecord")
            {
                // IMPORTANT: Don't update modal panel at all
                RegisterAsyncTask(new PageAsyncTask(async () => await ToggleStatus(id)));
            }
        }

        private async Task LoadEditData(int id)
        {
            try
            {
                if (ddlTripEdit.Items.Count == 0)
                {
                    await LoadTrips();
                }

                HttpResponseMessage response = await client.GetAsync($"AssignedVehicles/GetAssignedVehicles");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<AssignedVehicle> assignedVehicles = JsonConvert.DeserializeObject<List<AssignedVehicle>>(jsonResponse);
                    var assignedVehicle = assignedVehicles.FirstOrDefault(av => av.Id == id);

                    if (assignedVehicle != null)
                    {
                        hdnEditId.Value = id.ToString();

                        ddlTripEdit.SelectedValue = assignedVehicle.TripId.ToString();

                        await LoadVehiclesForTrip(assignedVehicle.TripId, ddlVehicleEdit);
                        ddlVehicleEdit.SelectedValue = assignedVehicle.VehicleId.ToString();

                        rfvTripEdit.Enabled = true;
                        rfvVehicleEdit.Enabled = true;
                        rfvTripAdd.Enabled = false;
                        rfvVehicleAdd.Enabled = false;

                        upModal.Update();

                        // Use setTimeout to ensure modal opens fresh
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                            @"setTimeout(function() { 
                        hideModal(); 
                        setTimeout(function() { showEditModal(); }, 100); 
                    }, 100);", true);
                    }
                    else
                    {
                        ShowError("Assignment not found");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading edit data: {ex.Message}");
            }
        }

        private async Task ToggleStatus(int id)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync($"AssignedVehicles/GetAssignedVehicles");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load assignment details");
                    return;
                }

                string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                List<AssignedVehicle> assignments = JsonConvert.DeserializeObject<List<AssignedVehicle>>(jsonResponse);
                var assignment = assignments.FirstOrDefault(a => a.Id == id);

                if (assignment == null)
                {
                    ShowError("Assignment not found");
                    return;
                }

                // ✅ Toggle status using string comparison
                string newStatus = assignment.Status == "1" ? "0" : "1";
                assignment.Status = newStatus;
                assignment.UpdatedAt = DateTime.Now;

                string json = JsonConvert.SerializeObject(assignment);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"AssignedVehicles/PutAssignedVehicle/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAssignedVehicles();

                    string statusText = newStatus == "1" ? "enabled" : "disabled";

                    // CRITICAL: Only show success, NO modal operations
                    ScriptManager.RegisterStartupScript(this, GetType(), "StatusSuccess_" + DateTime.Now.Ticks,
                        $"setTimeout(function() {{ showSuccess('Vehicle assignment {statusText} successfully!'); }}, 100);", true);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update assignment status: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }


        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearForm();

            rfvTripAdd.Enabled = true;
            rfvVehicleAdd.Enabled = true;
            rfvTripEdit.Enabled = false;
            rfvVehicleEdit.Enabled = false;

            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (ddlTripAdd.Items.Count == 0)
                {
                    await LoadTrips();
                }

                // ADD THIS: Load all vehicles for the Add form
                await LoadVehiclesForTrip(0, ddlVehicleAdd); // Pass 0 to load all vehicles

                upModal.Update();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowAddModal",
                    "Sys.Application.add_load(function() { showAddModal(); });", true);
            }));
        }

        private async Task UpdateAssignment(int id)
        {
            try
            {
                HttpResponseMessage getResponse = await client.GetAsync($"AssignedVehicles/GetAssignedVehicles");

                if (!getResponse.IsSuccessStatusCode)
                {
                    ShowError("Failed to load assignment details");
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                    return;
                }

                string currentJsonResponse = await getResponse.Content.ReadAsStringAsync();
                List<AssignedVehicle> assignments = JsonConvert.DeserializeObject<List<AssignedVehicle>>(currentJsonResponse);
                var currentAssignment = assignments.FirstOrDefault(a => a.Id == id);

                if (currentAssignment == null)
                {
                    ShowError("Assignment not found");
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                    return;
                }

                var updatedAssignment = new AssignedVehicle
                {
                    Id = id,
                    TripId = int.Parse(ddlTripEdit.SelectedValue),
                    VehicleId = int.Parse(ddlVehicleEdit.SelectedValue),
                    StartFrom = "07:00:00",
                    EndAt = "16:00:00",
                    Status = currentAssignment.Status,  // ✅ Preserve existing status (now string)
                    UpdatedAt = DateTime.Now
                };

                string jsonContent = JsonConvert.SerializeObject(updatedAssignment);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"AssignedVehicles/PutAssignedVehicle/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAssignedVehicles();
                    ClearForm();

                    // Force close modal and show success
                    ScriptManager.RegisterStartupScript(this, GetType(), "SuccessEdit_" + DateTime.Now.Ticks,
                        @"setTimeout(function() { 
                            hideModal(); 
                            modalOpenFlag = false;
                            showSuccess('Vehicle assignment updated successfully!');
                        }, 100);", true);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ShowError($"Failed to update assignment: {errorMessage}");
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating assignment: {ex.Message}");
                upModal.Update();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
            }
        }


        protected void ddlTripAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlTripAdd.SelectedValue))
            {
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadVehiclesForTrip(int.Parse(ddlTripAdd.SelectedValue), ddlVehicleAdd);
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "KeepAddModal", "showAddModal();", true);
                }));
            }
        }

        protected void ddlTripEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlTripEdit.SelectedValue))
            {
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await LoadVehiclesForTrip(int.Parse(ddlTripEdit.SelectedValue), ddlVehicleEdit);
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "KeepEditModal", "showEditModal();", true);
                }));
            }
        }

        private async Task LoadVehiclesForTrip(int tripId, DropDownList ddl)
        {
            try
            {
                var vehicles = await GetVehicles();

                ddl.DataSource = vehicles;
                ddl.DataTextField = "NickName";
                ddl.DataValueField = "Id";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("Select an option", ""));
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vehicles: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            hdnEditId.Value = "0";

            if (ddlTripAdd.Items.Count > 0)
                ddlTripAdd.SelectedIndex = 0;

            ddlVehicleAdd.Items.Clear();
            ddlVehicleAdd.Items.Insert(0, new ListItem("Select an option", ""));

            if (ddlTripEdit.Items.Count > 0)
                ddlTripEdit.SelectedIndex = 0;

            ddlVehicleEdit.Items.Clear();
            ddlVehicleEdit.Items.Insert(0, new ListItem("Select an option", ""));

            rfvTripAdd.Enabled = false;
            rfvVehicleAdd.Enabled = false;
            rfvTripEdit.Enabled = false;
            rfvVehicleEdit.Enabled = false;
        }

        protected string GetStatusBadge(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();  // ✅ Changed to string
            return statusValue == "1" ? "Enabled" : "Disabled";
        }

        protected string GetStatusClass(object status)
        {
            if (status == null) return "";
            string statusValue = status.ToString();  // ✅ Changed to string
            return statusValue == "1" ? "status-enabled" : "status-disabled";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                "document.getElementById('" + pnlError.ClientID + "').classList.add('show');", true);
        }
    }

    public partial class AssignedVehicle
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int VehicleId { get; set; }
        public string StartFrom { get; set; }
        public string EndAt { get; set; }
        public string Status { get; set; }  // ✅ Changed from int to string
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TripTitle { get; set; }
        public string VehicleNickName { get; set; }
        public string RegisterNo { get; set; }
    }

    public partial class Trip
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AcNonAc { get; set; }
        public string DayOff { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public partial class Vehicle
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public int FleetTypeId { get; set; }
        public string RegisterNo { get; set; }
        public string EngineNo { get; set; }
        public string ChasisNo { get; set; }
        public string ModelNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }  // ✅ Changed from int to string
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}