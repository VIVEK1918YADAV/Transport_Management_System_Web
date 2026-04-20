<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainList.aspx.cs" Inherits="Excel_Bus.TrainAdmin.TrainList" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .train-page {
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
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .train-table {
            width: 100%;
            border-collapse: collapse;
        }

            .train-table thead {
                background: #f8f9fa;
            }

            .train-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .train-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .train-table tbody tr:hover {
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
            color: #7367f0;
            border: 1px solid #7367f0;
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
                background: #7367f0;
                color: white;
            }

        .btn-disable {
            padding: 6px 12px;
            background: white;
            color: #ea5455;
            border: 1px solid #ea5455;
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
                background: #ea5455;
                color: white;
            }

        .btn-enable {
            padding: 6px 12px;
            background: white;
            color: #28c76f;
            border: 1px solid #28c76f;
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
                background: #28c76f;
                color: white;
            }

        /* ── Modal ── */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
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
            max-width: 620px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            animation: slideDown 0.3s ease-out;
            max-height: 90vh;
            overflow-y: auto;
        }

        @keyframes slideDown {
            from { transform: translateY(-50px); opacity: 0; }
            to   { transform: translateY(0);     opacity: 1; }
        }

        .modal-header {
            padding: 20px;
            border-bottom: 1px solid #dee2e6;
            display: flex;
            justify-content: space-between;
            align-items: center;
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

            .btn-close:hover { color: #000; }

        .modal-body { padding: 20px; }

        .form-row {
            display: flex;
            gap: 15px;
        }

            .form-row .form-group {
                flex: 1;
            }

        .form-group { margin-bottom: 15px; }

        .form-label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #2c3e50;
        }

        .required { color: #dc3545; }

        .form-control, .form-select {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
        }

            .form-control:focus, .form-select:focus {
                outline: none;
                border-color: #7367f0;
                box-shadow: 0 0 0 0.2rem rgba(115,103,240,0.25);
            }

        .modal-footer {
            padding: 15px 20px;
            border-top: 1px solid #dee2e6;
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

            .btn-submit:hover { background: #0b5ed7; }

        .error-panel {
            background: #f8d7da;
            border: 1px solid #f5c2c7;
            color: #842029;
            padding: 12px 20px;
            border-radius: 4px;
            margin-bottom: 20px;
            opacity: 0;
            transition: opacity 0.3s ease-out;
        }

            .error-panel.show { opacity: 1; }

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
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
            transition: opacity 0.3s ease-out;
        }

        @keyframes slideInRight {
            from { transform: translateX(100%); opacity: 0; }
            to   { transform: translateX(0);    opacity: 1; }
        }

        .train-id-badge {
            background: #f0eeff;
            color: #7367f0;
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            font-family: monospace;
        }

        /* ── Day Off checkboxes ── */
        .dayoff-group {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            background: #fff;
        }

        .dayoff-item {
            display: flex;
            align-items: center;
            gap: 5px;
            cursor: pointer;
            user-select: none;
        }

        .dayoff-item input[type="checkbox"] {
            display: none;
        }

        .dayoff-chip {
            padding: 4px 12px;
            border-radius: 20px;
            border: 1px solid #ced4da;
            font-size: 13px;
            color: #495057;
            background: #f8f9fa;
            cursor: pointer;
            transition: all 0.2s;
            white-space: nowrap;
        }

        .dayoff-item input[type="checkbox"]:checked + .dayoff-chip {
            background: #7367f0;
            color: white;
            border-color: #7367f0;
        }

        .dayoff-chip:hover {
            border-color: #7367f0;
            color: #7367f0;
        }

        .dayoff-item input[type="checkbox"]:checked + .dayoff-chip:hover {
            background: #6254e8;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="train-page">

        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Train List</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New"
                CssClass="btn-add-new"
                OnClientClick="openAddNewModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvTrains" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="train-table"
                    GridLines="None"
                    OnRowCommand="gvTrains_RowCommand"
                    DataKeyNames="Id">
                    <Columns>

                        <asp:TemplateField HeaderText="Train ID">
                            <ItemTemplate>
                                <span class="train-id-badge"><%# Eval("TrainId") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="TrainName"   HeaderText="Train Name" />
                        <asp:BoundField DataField="TrainNumber" HeaderText="Train No" />
                        <asp:BoundField DataField="FleetTypeName" HeaderText="Fleet Type" />
                        <asp:BoundField DataField="RegistrationNumber" HeaderText="Reg. No" />
                        <asp:BoundField DataField="DayOff"      HeaderText="Day Off" />

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# GetStatusClass(Eval("Status")) %>'>
                                    <%# GetStatusBadge(Eval("Status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">

                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CssClass="btn-edit"
                                        CommandName="EditTrain"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnToggle" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "ACTIVE" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status?');">
                                        <%# Eval("Status").ToString() == "ACTIVE" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>

                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No trains found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="trainModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Train"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">

                    <asp:HiddenField ID="hfTrainId" runat="server" />
                    <asp:HiddenField ID="hfIsEdit"  runat="server" Value="false" />

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Train Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtTrainName" runat="server" CssClass="form-control"
                                placeholder="e.g. Express Train"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Train Number <span class="required">*</span></label>
                            <asp:TextBox ID="txtTrainNumber" runat="server" CssClass="form-control"
                                placeholder="4-6 digits e.g. 12345"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Fleet Type <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlFleetType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select an option</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Registration Number <span class="required">*</span></label>
                            <asp:TextBox ID="txtRegNumber" runat="server" CssClass="form-control"
                                placeholder="e.g. REG-001"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Engine No</label>
                            <asp:TextBox ID="txtEngineNo" runat="server" CssClass="form-control"
                                placeholder="Engine number"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Chassis No</label>
                            <asp:TextBox ID="txtChasisNo" runat="server" CssClass="form-control"
                                placeholder="Chassis number"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Model No</label>
                        <asp:TextBox ID="txtModelNo" runat="server" CssClass="form-control"
                            placeholder="Model number"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Day Off</label>
                        <asp:HiddenField ID="hfDayOff" runat="server" />
                        <div class="dayoff-group" id="dayOffGroup">
                            <label class="dayoff-item">
                                <input type="checkbox" value="0" onchange="syncDayOff()" /> <span class="dayoff-chip">Sunday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="1" onchange="syncDayOff()" /> <span class="dayoff-chip">Monday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="2" onchange="syncDayOff()" /> <span class="dayoff-chip">Tuesday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="3" onchange="syncDayOff()" /> <span class="dayoff-chip">Wednesday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="4" onchange="syncDayOff()" /> <span class="dayoff-chip">Thursday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="5" onchange="syncDayOff()" /> <span class="dayoff-chip">Friday</span>
                            </label>
                            <label class="dayoff-item">
                                <input type="checkbox" value="6" onchange="syncDayOff()" /> <span class="dayoff-chip">Saturday</span>
                            </label>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        CssClass="btn-submit"
                        OnClick="btnSubmit_Click"
                        OnClientClick="return validateForm();" />
                </div>
            </div>
        </div>

    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // ─── Modal open/close ────────────────────────────────────────────────────────

        function openAddNewModal() {
            clearTrainForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Train';
            showModal();
            return false;
        }

        function showModal() {
            var modal = document.getElementById('trainModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('trainModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        // ─── Form clear ───────────────────────────────────────────────────────────────

        function clearTrainForm() {
            document.getElementById('<%= txtTrainName.ClientID %>').value = '';
            document.getElementById('<%= txtTrainNumber.ClientID %>').value = '';
            document.getElementById('<%= ddlFleetType.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= txtRegNumber.ClientID %>').value = '';
            document.getElementById('<%= txtEngineNo.ClientID %>').value    = '';
            document.getElementById('<%= txtChasisNo.ClientID %>').value    = '';
            document.getElementById('<%= txtModelNo.ClientID %>').value     = '';
            document.getElementById('<%= hfDayOff.ClientID %>').value       = '';
            document.getElementById('<%= hfTrainId.ClientID %>').value      = '';
            document.getElementById('<%= hfIsEdit.ClientID %>').value       = 'false';
            // Uncheck all day-off checkboxes
            document.querySelectorAll('#dayOffGroup input[type="checkbox"]')
                .forEach(function(cb) { cb.checked = false; });
        }

        // ─── Sync checked days → hidden field (comma-separated values) ───────────────

        function syncDayOff() {
            var checked = [];
            document.querySelectorAll('#dayOffGroup input[type="checkbox"]:checked')
                .forEach(function(cb) { checked.push(cb.value); });
            document.getElementById('<%= hfDayOff.ClientID %>').value = checked.join(',');
        }

        // ─── Pre-check boxes when editing ────────────────────────────────────────────

        function setDayOffCheckboxes(csvValues) {
            // Uncheck all first
            document.querySelectorAll('#dayOffGroup input[type="checkbox"]')
                .forEach(function(cb) { cb.checked = false; });

            if (!csvValues) return;
            var vals = csvValues.split(',').map(function(v) { return v.trim(); });
            vals.forEach(function(v) {
                var cb = document.querySelector('#dayOffGroup input[value="' + v + '"]');
                if (cb) cb.checked = true;
            });
            syncDayOff();
        }

        // ─── Form validation ──────────────────────────────────────────────────────────

        function validateForm() {
            var name      = document.getElementById('<%= txtTrainName.ClientID %>').value.trim();
            var number    = document.getElementById('<%= txtTrainNumber.ClientID %>').value.trim();
            var fleetType = document.getElementById('<%= ddlFleetType.ClientID %>').value;
            var regNo     = document.getElementById('<%= txtRegNumber.ClientID %>').value.trim();

            if (!name) {
                alert('Please enter the train name.');
                return false;
            }
            if (!number) {
                alert('Please enter the train number.');
                return false;
            }
            if (!/^\d{4,6}$/.test(number)) {
                alert('Train number must be 4-6 digits.');
                return false;
            }
            if (!fleetType) {
                alert('Please select a fleet type.');
                return false;
            }
            if (!regNo) {
                alert('Please enter the registration number.');
                return false;
            }
            return true;
        }

        // ─── Success toast ────────────────────────────────────────────────────────────

        function showSuccess(message) {
            var successDiv = document.createElement('div');
            successDiv.className = 'success-message';
            successDiv.textContent = message;
            document.body.appendChild(successDiv);

            setTimeout(function () {
                successDiv.style.opacity = '0';
                setTimeout(function () {
                    if (successDiv.parentNode) successDiv.parentNode.removeChild(successDiv);
                }, 300);
            }, 3000);
        }

        // ─── Close on outside click ──────────────────────────────────────────────────

        window.onclick = function (event) {
            var modal = document.getElementById('trainModal');
            if (event.target === modal) hideModal();
        };

        // ─── Prevent resubmission on refresh ─────────────────────────────────────────

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // ─── Page load ────────────────────────────────────────────────────────────────

        window.onload = function () {
            var isEdit = document.getElementById('<%= hfIsEdit.ClientID %>').value;
            if (isEdit === 'true') {
                document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Edit Train';
                // Pre-check day-off boxes from hidden field set by server
                var dayOffVal = document.getElementById('<%= hfDayOff.ClientID %>').value;
                setDayOffCheckboxes(dayOffVal);
                showModal();
            }
        };
    </script>
</asp:Content>
