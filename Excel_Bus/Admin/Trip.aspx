<%@ Page Title="All Trip" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Trip.aspx.cs" Inherits="Excel_Bus.Trip" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .trip-container {
            padding: 20px;
            background-color: #f5f5f5;
            margin-top: 50px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
        }

        .header-actions {
            display: flex;
            gap: 10px;
        }

        .search-box {
            display: flex;
            gap: 0;
        }

        .search-input {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 4px 0 0 4px;
            width: 250px;
            font-size: 14px;
        }

        .btn-search {
            padding: 8px 16px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 0 4px 4px 0;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-add {
            padding: 8px 16px;
            background-color: #28a745;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-search:hover {
            background-color: #0056b3;
        }

        .btn-add:hover {
            background-color: #218838;
        }

        .trip-table {
            width: 100%;
            background-color: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .trip-table table {
            width: 100%;
            border-collapse: collapse;
        }

        .trip-table thead {
            background-color: #ffffcc;
        }

        .trip-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #333;
            border-bottom: 2px solid #ddd;
        }

        .trip-table td {
            padding: 15px;
            border-bottom: 1px solid #eee;
        }

        .trip-table tbody tr:hover {
            background-color: #f9f9f9;
        }

         .status-badge {
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .status-enabled {
            background: #d1f4e0;
            color: #00a854;
        }

        .status-disabled {
            background: #ffe0e0;
            color: #ff4444;
        }

                /* Action Buttons Container */
.action-buttons {
    display: flex;
    gap: 8px;
    align-items: center;
}

/* Edit Button - Purple/Pink */
.btn-edit {
    padding: 6px 12px;
    background: white;
    color: #7c3aed;
    border: 1px solid #7c3aed;
    border-radius: 4px;
    cursor: pointer;
    font-size: 13px;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    white-space: nowrap;
    min-width: 70px;
    height: 32px;
}

.btn-edit:hover {
    background: #7c3aed;
    color: white;
}

/* Disable Button - Red */
.btn-disable {
    padding: 6px 12px;
    background: white;
    color: #ef4444;
    border: 1px solid #ef4444;
    border-radius: 4px;
    cursor: pointer;
    font-size: 13px;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    white-space: nowrap;
    min-width: 80px;
    height: 32px;
}

.btn-disable:hover {
    background: #ef4444;
    color: white;
}

/* Enable Button - Green */
.btn-enable {
    padding: 6px 12px;
    background: white;
    color: #10b981;
    border: 1px solid #10b981;
    border-radius: 4px;
    cursor: pointer;
    font-size: 13px;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    white-space: nowrap;
    min-width: 80px;
    height: 32px;
}

.btn-enable:hover {
    background: #10b981;
    color: white;
}

        /* Modal Styles - Counter Pattern */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 9999;
            justify-content: center;
            align-items: center;
        }

        .modal-overlay.show {
            display: flex !important;
        }

        .modal-content {
            background: white;
            border-radius: 8px;
            width: 90%;
            max-width: 700px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            animation: slideDown 0.3s ease-out;
        }

        @keyframes slideDown {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 20px;
            border-bottom: 1px solid #dee2e6;
        }

        .modal-title {
            font-size: 20px;
            font-weight: 600;
            color: #333;
            margin: 0;
        }

        .close-modal {
            background: none;
            border: none;
            font-size: 24px;
            cursor: pointer;
            color: #999;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .close-modal:hover {
            color: #333;
        }

        .modal-body {
            padding: 20px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group.full-width {
            grid-column: 1 / -1;
        }

        .form-label {
            margin-bottom: 8px;
            font-weight: 500;
            color: #333;
        }

        .required {
            color: red;
        }

        .form-control {
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
            width: 100%;
            box-sizing: border-box;
        }

        .form-control:focus {
            outline: none;
            border-color: #007bff;
        }

        .modal-footer {
            padding: 15px 20px;
            border-top: 1px solid #dee2e6;
        }

        .btn-submit {
            width: 100%;
            padding: 12px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
        }

        .btn-submit:hover {
            background-color: #0056b3;
        }

        .error-message {
            background-color: #f8d7da;
            color: #721c24;
            padding: 12px;
            border-radius: 4px;
            margin-bottom: 20px;
        }

        .success-message {
            position: fixed;
            top: 20px;
            right: 20px;
            background: #d1f4e0;
            color: #00a854;
            padding: 15px 20px;
            border-radius: 4px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
        }

        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        .day-off-container {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 4px;
            min-height: 50px;
        }

        .day-tag {
            background-color: #007bff;
            color: white;
            padding: 5px 10px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            gap: 5px;
            font-size: 13px;
        }

        .day-tag .remove-day {
            cursor: pointer;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="trip-container">
        <!-- Error/Success Messages -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="error-message">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <h1 class="page-title">All Trip</h1>
            <div class="header-actions">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by name..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <%--<asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add" OnClientClick="showModal(); return false;" />--%>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add" OnClientClick="clearFormAndShowModal(); return false;" />
            </div>
        </div>

        <!-- Trip Table -->
        <div class="trip-table">
            <asp:GridView ID="gvTrips" runat="server" AutoGenerateColumns="False" 
                OnRowDataBound="gvTrips_RowDataBound" 
                OnRowCommand="gvTrips_RowCommand"
                GridLines="None" ShowHeader="True">
                <HeaderStyle CssClass="grid-header" />
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="AcNonAc" HeaderText="AC / Non-AC" />
                    <%--<asp:BoundField DataField="DayOff" HeaderText="Day Off" />--%>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# GetStatusClass(Eval("Status")) %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <%--<asp:LinkButton ID="btnEdit" runat="server" Text="✏ Edit" 
                                    CssClass="btn-edit" 
                                    CommandName="EditTrip" 
                                    CommandArgument='<%# Eval("Id") %>' />--%>
                                <asp:LinkButton ID="btnToggleStatus" runat="server" 
                                    Text='<%# Eval("Status").ToString() == "Enabled" ? "🚫 Disable" : "✓ Enable" %>' 
                                    CssClass='<%# Eval("Status").ToString() == "Enabled" ? "btn-disable" : "btn-enable" %>' 
                                    CommandName="ToggleStatus" 
                                    CommandArgument='<%# Eval("Id") %>' 
                                    OnClientClick='<%# "return confirm(\"Are you sure you want to " + (Eval("Status").ToString() == "Enabled" ? "disable" : "enable") + " this trip?\");" %>' />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Add/Edit Modal -->
    <div id="tripModal" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Label ID="lblModalTitle" runat="server" Text="Add New Trip"></asp:Label>
                </h2>
                <button type="button" class="close-modal" onclick="hideModal(); return false;">&times;</button>
            </div>

            <div class="modal-body">
                <asp:HiddenField ID="hdnTripId" runat="server" Value="0" />
                <%--<asp:HiddenField ID="hdnDayOffValues" runat="server" />--%>

                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label">Title <span class="required">*</span></label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Fleet Type <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlFleetType" runat="server" CssClass="form-control" required></asp:DropDownList>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label">Route <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control" required></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Schedule <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlSchedule" runat="server" CssClass="form-control" required></asp:DropDownList>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label">Start From <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlStartFrom" runat="server" CssClass="form-control" required></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label class="form-label">End To <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlEndTo" runat="server" CssClass="form-control" required></asp:DropDownList>
                    </div>
                </div>

               <%-- <div class="form-row">
                    <div class="form-group full-width">
                        <label class="form-label">Day Off</label>
                        <asp:DropDownList ID="ddlDayOff" runat="server" CssClass="form-control" onchange="addDayOff(this)">
                            <asp:ListItem Value="">Select day...</asp:ListItem>
                            <asp:ListItem Value="0">Sunday</asp:ListItem>
                            <asp:ListItem Value="1">Monday</asp:ListItem>
                            <asp:ListItem Value="2">Tuesday</asp:ListItem>
                            <asp:ListItem Value="3">Wednesday</asp:ListItem>
                            <asp:ListItem Value="4">Thursday</asp:ListItem>
                            <asp:ListItem Value="5">Friday</asp:ListItem>
                            <asp:ListItem Value="6">Saturday</asp:ListItem>
                        </asp:DropDownList>
                        <div id="dayOffContainer" class="day-off-container"></div>
                    </div>
                </div>--%>
            </div>

            <div class="modal-footer">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        var selectedDays = [];

        function showModal() {
            var modal = document.getElementById('tripModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('tripModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        function clearForm() {
            document.getElementById('<%= hdnTripId.ClientID %>').value = '0';
            document.getElementById('<%= txtTitle.ClientID %>').value = '';

        var fleetType = document.getElementById('<%= ddlFleetType.ClientID %>');
        if (fleetType) fleetType.selectedIndex = 0;

        var route = document.getElementById('<%= ddlRoute.ClientID %>');
        if (route) route.selectedIndex = 0;

        var schedule = document.getElementById('<%= ddlSchedule.ClientID %>');
        if (schedule) schedule.selectedIndex = 0;

        var startFrom = document.getElementById('<%= ddlStartFrom.ClientID %>');
        if (startFrom) startFrom.selectedIndex = 0;

        var endTo = document.getElementById('<%= ddlEndTo.ClientID %>');
        if (endTo) endTo.selectedIndex = 0;

       <%-- var dayOff = document.getElementById('<%= ddlDayOff.ClientID %>');
        if (dayOff) dayOff.selectedIndex = 0;

        selectedDays = [];
        updateDayOffDisplay();--%>

        document.getElementById('<%= lblModalTitle.ClientID %>').innerText = 'Add New Trip';
        }

        // New function to clear form and show modal
        function clearFormAndShowModal() {
            clearForm();
            showModal();
            return false;
        }

     <%--   function addDayOff(select) {
            var value = select.value;
            var text = select.options[select.selectedIndex].text;

            if (value && !selectedDays.includes(value)) {
                selectedDays.push(value);
                updateDayOffDisplay();
            }

            select.selectedIndex = 0;
        }

        function removeDayOff(value) {
            selectedDays = selectedDays.filter(d => d !== value);
            updateDayOffDisplay();
        }

        function updateDayOffDisplay() {
            var container = document.getElementById('dayOffContainer');
            var hdnField = document.getElementById('<%= hdnDayOffValues.ClientID %>');

            if (!container || !hdnField) return;

            container.innerHTML = '';

            var dayNames = {
                '0': 'Sunday',
                '1': 'Monday',
                '2': 'Tuesday',
                '3': 'Wednesday',
                '4': 'Thursday',
                '5': 'Friday',
                '6': 'Saturday'
            };

            selectedDays.forEach(function (day) {
                var tag = document.createElement('div');
                tag.className = 'day-tag';
                tag.innerHTML = dayNames[day] + ' <span class="remove-day" onclick="removeDayOff(\'' + day + '\')">×</span>';
                container.appendChild(tag);
            });

            hdnField.value = JSON.stringify(selectedDays);
        }

        function loadDaysForEdit(daysJson) {
            try {
                if (daysJson && daysJson !== '[]' && daysJson !== '') {
                    selectedDays = JSON.parse(daysJson);
                } else {
                    selectedDays = [];
                }
                updateDayOffDisplay();
            } catch (e) {
                console.error('Error parsing day off data:', e);
                selectedDays = [];
                updateDayOffDisplay();
            }
        }--%>

        function showSuccess(message) {
            var successDiv = document.createElement('div');
            successDiv.className = 'success-message';
            successDiv.textContent = message;
            document.body.appendChild(successDiv);

            setTimeout(function () {
                successDiv.style.opacity = '0';
                setTimeout(function () {
                    if (successDiv.parentNode) {
                        successDiv.parentNode.removeChild(successDiv);
                    }
                }, 300);
            }, 3000);
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var modal = document.getElementById('tripModal');
            if (event.target == modal) {
                hideModal();
            }
        }

        // Prevent form resubmission on page refresh
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>