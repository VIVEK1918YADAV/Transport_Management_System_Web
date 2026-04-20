<%@ Page Title="All Vehicles" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Vehicles.aspx.cs" Inherits="Excel_Bus.Vehicles" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .vehicles-page {
            padding: 20px;
            background: #f5f7fa;
            margin-top: 50px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .header-actions {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }

        .search-box {
            display: flex;
            gap: 0;
        }

        .search-input {
            padding: 10px 15px;
            border: 1px solid #ddd;
            border-radius: 4px 0 0 4px;
            font-size: 14px;
            width: 250px;
        }

        .btn-search {
            padding: 10px 20px;
            background: #0d6efd;
            color: white;
            border: none;
            border-radius: 0 4px 4px 0;
            cursor: pointer;
            font-size: 14px;
        }

            .btn-search:hover {
                background: #0b5ed7;
            }

        .btn-add-new {
            padding: 10px 20px;
            background: #28c76f;
            color: white;
            border: 1px solid #28c76f;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 5px;
            white-space: nowrap;
        }

            .btn-add-new:hover {
                background: #24b263;
            }

        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .vehicles-table {
            width: 100%;
            border-collapse: collapse;
        }

            .vehicles-table thead {
                background: #fffbcc;
            }

            .vehicles-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .vehicles-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .vehicles-table tbody tr:hover {
                background: #f8f9fa;
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

        .action-buttons {
            display: flex;
            gap: 8px;
            align-items: center;
        }

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
            max-width: 500px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            animation: slideDown 0.3s ease-out;
            max-height: 90vh;
            overflow-y: auto;
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
            padding: 20px;
            border-bottom: 1px solid #dee2e6;
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: sticky;
            top: 0;
            background: white;
            z-index: 1;
        }

        .modal-title {
            font-size: 18px;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .btn-close {
            background: none;
            border: none;
            font-size: 24px;
            cursor: pointer;
            color: #6c757d;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

            .btn-close:hover {
                color: #000;
            }

        .modal-body {
            padding: 20px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #2c3e50;
        }

        .required {
            color: #dc3545;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
        }

            .form-control:focus {
                outline: none;
                border-color: #6c63ff;
                box-shadow: 0 0 0 0.2rem rgba(108, 99, 255, 0.25);
            }

        .form-select {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
            background: white;
            cursor: pointer;
        }

            .form-select:focus {
                outline: none;
                border-color: #6c63ff;
                box-shadow: 0 0 0 0.2rem rgba(108, 99, 255, 0.25);
            }

        .modal-footer {
            padding: 15px 20px;
            border-top: 1px solid #dee2e6;
            position: sticky;
            bottom: 0;
            background: white;
        }

        .btn-submit {
            width: 100%;
            padding: 12px;
            background: #0d6efd;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
        }

            .btn-submit:hover {
                background: #0b5ed7;
            }

        .alert {
            padding: 12px 20px;
            border-radius: 4px;
            margin-bottom: 20px;
            opacity: 0;
            transition: opacity 0.3s ease-out;
        }

            .alert.show {
                opacity: 1;
            }

        .error-panel {
            background: #f8d7da;
            border: 1px solid #f5c2c7;
            color: #842029;
        }

        .success-panel {
            background: #d1f4e0;
            border: 1px solid #badbcc;
            color: #0f5132;
        }

        .no-data {
            text-align: center;
            padding: 40px;
            color: #6c757d;
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
            transition: opacity 0.3s ease-out;
            font-weight: 500;
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

        /* Day Off Container Styles */
        .day-off-container {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 10px;
            min-height: 30px;
        }

        .day-tag {
            background: #e3f2fd;
            color: #1976d2;
            padding: 6px 12px;
            border-radius: 16px;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-weight: 500;
        }

        .remove-day {
            cursor: pointer;
            font-weight: bold;
            color: #d32f2f;
            font-size: 16px;
            line-height: 1;
        }

            .remove-day:hover {
                color: #b71c1c;
            }

        @media (max-width: 768px) {
            .vehicles-table {
                font-size: 12px;
            }

                .vehicles-table th,
                .vehicles-table td {
                    padding: 10px;
                }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="vehicles-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="alert error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert success-panel" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">All Vehicles</h1>
            <div class="header-actions">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by name..." autocomplete="off" />
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new" OnClientClick="openAddNewModal(); return false;" />
            </div>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvVehicles" runat="server" CssClass="vehicles-table" AutoGenerateColumns="false"
                    OnRowCommand="gvVehicles_RowCommand" GridLines="None" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="NickName" HeaderText="Vehicle Name" />
                        <asp:BoundField DataField="RegisterNo" HeaderText="Reg. No." />
                        <asp:BoundField DataField="EngineNo" HeaderText="Engine No." />
                        <asp:BoundField DataField="ChasisNo" HeaderText="Chassis No." />
                        <asp:BoundField DataField="ModelNo" HeaderText="Model No." />
                        <asp:TemplateField HeaderText="Day Off">
                            <ItemTemplate>
                                <%# GetDayOffDisplay(Eval("DayOff")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:TemplateField HeaderText="Has AC">
                            <ItemTemplate>
                                <%# GetHasAcDisplay(Eval("HasAc")) %>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="Fleet Type">
                            <ItemTemplate>
                                <%# GetFleetTypeName(Eval("FleetTypeId")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='status-badge <%# GetStatusClass(Eval("Status")) %>'>
                                    <%# GetStatusBadge(Eval("Status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CssClass="btn-edit"
                                        CommandName="EditVehicle"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>
                                   <%-- <asp:LinkButton ID="btnDisable" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "1" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="DisableVehicle"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status of this vehicle?');">
                                        <%# Eval("Status").ToString() == "1" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>--%>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No vehicles found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="vehicleModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Vehicle"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnVehicleId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnDayOffValues" runat="server" Value="[]" />
                    
                    <div class="form-group">
                        <label class="form-label">Nick Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtNickName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvNickName" runat="server"
                            ControlToValidate="txtNickName"
                            ErrorMessage="Nick Name is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group">
                        <label class="form-label">Fleet Type <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlFleetType" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="Select an option" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvFleetType" runat="server"
                            ControlToValidate="ddlFleetType"
                            InitialValue=""
                            ErrorMessage="Fleet Type is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group" id="divRegisterNo" runat="server">
                        <label class="form-label">Register No. <span class="required">*</span></label>
                        <asp:TextBox ID="txtRegisterNo" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvRegisterNo" runat="server"
                            ControlToValidate="txtRegisterNo"
                            ErrorMessage="Register No is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group" id="divEngineNo" runat="server">
                        <label class="form-label">Engine No. <span class="required">*</span></label>
                        <asp:TextBox ID="txtEngineNo" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvEngineNo" runat="server"
                            ControlToValidate="txtEngineNo"
                            ErrorMessage="Engine No is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group">
                        <label class="form-label">Chassis No.</label>
                        <asp:TextBox ID="txtChasisNo" runat="server" CssClass="form-control" />
                    </div>

                    <div class="form-group">
                        <label class="form-label">Model No. <span class="required">*</span></label>
                        <asp:TextBox ID="txtModelNo" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvModelNo" runat="server"
                            ControlToValidate="txtModelNo"
                            ErrorMessage="Model No is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group">
                        <label class="form-label">Day Off</label>
                        <asp:DropDownList ID="ddlDayOff" runat="server" CssClass="form-select" onchange="addDayOff(this)">
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

                    <div class="form-group">
                        <label class="form-label">Has AC <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlHasAc" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="Select an option" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                            <asp:ListItem Value="0" Text="No"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvHasAc" runat="server"
                            ControlToValidate="ddlHasAc"
                            InitialValue=""
                            ErrorMessage="Has AC is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group" id="divEmail" runat="server">
                        <label class="form-label">Email <span class="required">*</span></label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Invalid email format"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"
                            ValidationGroup="VehicleValidation" />
                    </div>

                    <div class="form-group" id="divPassword" runat="server">
                        <label class="form-label">Password <span class="required">*</span></label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword"
                            ErrorMessage="Password is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="VehicleValidation" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit"
                        OnClick="btnSubmit_Click" ValidationGroup="VehicleValidation" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        // Initialize selectedDays array
        var selectedDays = [];

        function openAddNewModal() {
            console.log('Opening Add New Modal');
            clearVehicleForm();

            var lblModalTitle = document.getElementById('<%= lblModalTitle.ClientID %>');
            if (lblModalTitle) {
                lblModalTitle.textContent = 'Add New Vehicle';
            }

            setTimeout(function () {
                showModal();
            }, 100);

            return false;
        }

        function clearVehicleForm() {
            console.log('Clearing vehicle form');

            var hdnVehicleId = document.getElementById('<%= hdnVehicleId.ClientID %>');
            if (hdnVehicleId) hdnVehicleId.value = '0';

            var txtNickName = document.getElementById('<%= txtNickName.ClientID %>');
            if (txtNickName) {
                txtNickName.value = '';
                txtNickName.disabled = false;
            }

            var txtRegisterNo = document.getElementById('<%= txtRegisterNo.ClientID %>');
            if (txtRegisterNo) txtRegisterNo.value = '';

            var txtEngineNo = document.getElementById('<%= txtEngineNo.ClientID %>');
            if (txtEngineNo) txtEngineNo.value = '';

            var txtChasisNo = document.getElementById('<%= txtChasisNo.ClientID %>');
            if (txtChasisNo) txtChasisNo.value = '';

            var txtModelNo = document.getElementById('<%= txtModelNo.ClientID %>');
            if (txtModelNo) {
                txtModelNo.value = '';
                txtModelNo.disabled = false;
            }

            var ddlDayOff = document.getElementById('<%= ddlDayOff.ClientID %>');
            if (ddlDayOff) ddlDayOff.selectedIndex = 0;

            var ddlHasAc = document.getElementById('<%= ddlHasAc.ClientID %>');
            if (ddlHasAc) ddlHasAc.selectedIndex = 0;

            selectedDays = [];
            updateDayOffDisplay();

            var txtEmail = document.getElementById('<%= txtEmail.ClientID %>');
            if (txtEmail) txtEmail.value = '';

            var txtPassword = document.getElementById('<%= txtPassword.ClientID %>');
            if (txtPassword) txtPassword.value = '';

            var ddlFleetType = document.getElementById('<%= ddlFleetType.ClientID %>');
            if (ddlFleetType) {
                ddlFleetType.selectedIndex = 0;
                ddlFleetType.disabled = false;
            }

            var divEmail = document.getElementById('<%= divEmail.ClientID %>');
            if (divEmail) {
                divEmail.style.display = 'block';
                divEmail.style.visibility = 'visible';
            }

            var divPassword = document.getElementById('<%= divPassword.ClientID %>');
            if (divPassword) {
                divPassword.style.display = 'block';
                divPassword.style.visibility = 'visible';
            }
        }

        function showModal() {
            console.log('Showing modal');
            var modal = document.getElementById('vehicleModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            console.log('Hiding modal');
            var modal = document.getElementById('vehicleModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        function addDayOff(select) {
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
                console.log('Loading days for edit:', daysJson);
                if (daysJson && daysJson !== '[]' && daysJson !== '' && daysJson !== 'null') {
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
        }

        function showSuccess(message) {
            console.log('showSuccess called with message:', message);

            // Remove any existing success messages
            var existingMessages = document.querySelectorAll('.success-message');
            existingMessages.forEach(function (msg) {
                msg.remove();
            });

            // Create and show new success message
            var successDiv = document.createElement('div');
            successDiv.className = 'success-message';
            successDiv.textContent = message;
            successDiv.style.opacity = '1';
            document.body.appendChild(successDiv);

            console.log('Success message element created and added to DOM');

            // Auto-hide after 3 seconds
            setTimeout(function () {
                successDiv.style.opacity = '0';
                setTimeout(function () {
                    successDiv.remove();
                }, 300);
            }, 3000);
        }

        window.onclick = function (event) {
            var modal = document.getElementById('vehicleModal');
            if (event.target == modal) {
                hideModal();
            }
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // Make showSuccess available globally
        window.showSuccess = showSuccess;

        console.log('Scripts loaded, showSuccess is:', typeof window.showSuccess);
    </script>
</asp:Content>