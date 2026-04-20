<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainSchedules.aspx.cs" Inherits="Excel_Bus.TrainAdmin.TrainSchedules" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .schedule-page {
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

            .btn-add-new:hover { background: #24b263; }

        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .table-responsive { overflow-x: auto; }

        .schedule-table {
            width: 100%;
            border-collapse: collapse;
        }

            .schedule-table thead { background: #f8f9fa; }

            .schedule-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .schedule-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .schedule-table tbody tr:hover { background: #f8f9fa; }

        .status-badge {
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .status-enabled  { background: #d1f4e0; color: #00a854; }
        .status-disabled { background: #ffe0e0; color: #ff4444; }

        .action-buttons { display: flex; gap: 8px; align-items: center; }

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

            .btn-edit:hover { background: #7367f0; color: white; }

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

            .btn-disable:hover { background: #ea5455; color: white; }

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

            .btn-enable:hover { background: #28c76f; color: white; }

        /* ── Modal ── */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0; left: 0;
            width: 100%; height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 9999;
            justify-content: center;
            align-items: center;
        }

            .modal-overlay.show { display: flex !important; }

        .modal-content {
            background: white;
            border-radius: 8px;
            width: 90%;
            max-width: 580px;
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

        .modal-title { font-size: 18px; font-weight: 600; color: #2c3e50; margin: 0; }

        .btn-close {
            background: none; border: none;
            font-size: 24px; cursor: pointer; color: #6c757d;
            padding: 0; width: 30px; height: 30px;
            display: flex; align-items: center; justify-content: center;
        }

            .btn-close:hover { color: #000; }

        .modal-body { padding: 20px; }

        .form-row { display: flex; gap: 15px; }

            .form-row .form-group { flex: 1; }

        .form-group { margin-bottom: 15px; }

        .form-label { display: block; margin-bottom: 5px; font-weight: 500; color: #2c3e50; }

        .required { color: #dc3545; }

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
                border-color: #7367f0;
                box-shadow: 0 0 0 0.2rem rgba(115,103,240,0.25);
            }

        .modal-footer { padding: 15px 20px; border-top: 1px solid #dee2e6; }

        .btn-submit {
            width: 100%; padding: 12px;
            background: #0d6efd; color: white;
            border: none; border-radius: 4px;
            cursor: pointer; font-size: 16px; font-weight: 500;
        }

            .btn-submit:hover { background: #0b5ed7; }

        .error-panel {
            background: #f8d7da; border: 1px solid #f5c2c7;
            color: #842029; padding: 12px 20px;
            border-radius: 4px; margin-bottom: 20px;
            opacity: 0; transition: opacity 0.3s ease-out;
        }

            .error-panel.show { opacity: 1; }

        .no-data { text-align: center; padding: 40px; color: #6c757d; }

        .success-message {
            position: fixed; top: 20px; right: 20px;
            background: #d1f4e0; color: #00a854;
            padding: 15px 20px; border-radius: 4px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
            transition: opacity 0.3s ease-out;
        }

        @keyframes slideInRight {
            from { transform: translateX(100%); opacity: 0; }
            to   { transform: translateX(0);    opacity: 1; }
        }

        .time-badge {
            background: #f0f4ff;
            color: #3d5af1;
            padding: 3px 10px;
            border-radius: 4px;
            font-size: 13px;
            font-weight: 600;
            font-family: monospace;
            white-space: nowrap;
        }

        .duration-badge {
            background: #fff8e1;
            color: #e67e00;
            padding: 3px 10px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            white-space: nowrap;
        }

        .hint-text {
            font-size: 11px;
            color: #6c757d;
            margin-top: 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="schedule-page">

        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Train Schedules</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New"
                CssClass="btn-add-new"
                OnClientClick="openAddNewModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvSchedules" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="schedule-table"
                    GridLines="None"
                    OnRowCommand="gvSchedules_RowCommand"
                    DataKeyNames="ScheduleId">
                    <Columns>

                        <asp:BoundField DataField="ScheduleId" HeaderText="ID" />

                        <asp:TemplateField HeaderText="Departure">
                            <ItemTemplate>
                                <span class="time-badge"><%# Eval("DepartureTime") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Arrival">
                            <ItemTemplate>
                                <span class="time-badge"><%# Eval("ArrivalTime") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Duration (hrs)">
                            <ItemTemplate>
                                <span class="duration-badge">
                                    <%# Eval("EstimatedDurationHours") != null && Eval("EstimatedDurationHours").ToString() != ""
                                        ? Eval("EstimatedDurationHours") + " hrs" : "—" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Distance (km)">
                            <ItemTemplate>
                                <%# Eval("TotalDistanceKm") != null && Eval("TotalDistanceKm").ToString() != ""
                                    ? Eval("TotalDistanceKm") + " km" : "—" %>
                            </ItemTemplate>
                        </asp:TemplateField>

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
                                        CommandName="EditSchedule"
                                        CommandArgument='<%# Eval("ScheduleId") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnToggle" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "1" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("ScheduleId") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status?');">
                                        <%# Eval("Status").ToString() == "1" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>

                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data"><p>No schedules found</p></div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="scheduleModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Schedule"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">

                    <asp:HiddenField ID="hfScheduleId" runat="server" />
                    <asp:HiddenField ID="hfIsEdit"     runat="server" Value="false" />

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Departure Time <span class="required">*</span></label>
                            <asp:TextBox ID="txtDepartureTime" runat="server" CssClass="form-control"
                                TextMode="Time"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Arrival Time</label>
                            <asp:TextBox ID="txtArrivalTime" runat="server" CssClass="form-control"
                                TextMode="Time"></asp:TextBox>
                            <div class="hint-text">* If arrival is before departure, overnight journey is assumed.</div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Total Distance (km)</label>
                            <asp:TextBox ID="txtTotalDistance" runat="server" CssClass="form-control"
                                TextMode="Number" placeholder="e.g. 350"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <!-- Duration is auto-calculated by API from departure/arrival -->
                            <label class="form-label">Est. Duration (hrs)</label>
                            <asp:TextBox ID="txtDuration" runat="server" CssClass="form-control"
                                placeholder="Auto-calculated" ReadOnly="true"
                                style="background:#f8f9fa; color:#6c757d;"></asp:TextBox>
                            <div class="hint-text">* Auto-calculated from departure & arrival time.</div>
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
        // ─── Auto-calculate duration when times change ────────────────────────────────

        function calcDuration() {
            var dep = document.getElementById('<%= txtDepartureTime.ClientID %>').value;
            var arr = document.getElementById('<%= txtArrivalTime.ClientID %>').value;
            var durField = document.getElementById('<%= txtDuration.ClientID %>');

            if (dep && arr) {
                var depParts = dep.split(':').map(Number);
                var arrParts = arr.split(':').map(Number);
                var depMins  = depParts[0] * 60 + depParts[1];
                var arrMins  = arrParts[0] * 60 + arrParts[1];

                // overnight support
                if (arrMins < depMins) arrMins += 1440;

                var diffMins = arrMins - depMins;
                var hrs  = Math.floor(diffMins / 60);
                var mins = diffMins % 60;
                durField.value = hrs + 'h ' + (mins < 10 ? '0' : '') + mins + 'm (' + (diffMins / 60).toFixed(2) + ' hrs)';
            } else {
                durField.value = '';
            }
        }

        // ─── Modal open/close ────────────────────────────────────────────────────────

        function openAddNewModal() {
            clearScheduleForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Schedule';
            showModal();
            return false;
        }

        function showModal() {
            var modal = document.getElementById('scheduleModal');
            if (modal) { modal.classList.add('show'); document.body.style.overflow = 'hidden'; }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('scheduleModal');
            if (modal) { modal.classList.remove('show'); document.body.style.overflow = ''; }
            return false;
        }

        // ─── Form clear ───────────────────────────────────────────────────────────────

        function clearScheduleForm() {
            document.getElementById('<%= txtDepartureTime.ClientID %>').value = '';
            document.getElementById('<%= txtArrivalTime.ClientID %>').value   = '';
            document.getElementById('<%= txtTotalDistance.ClientID %>').value = '';
            document.getElementById('<%= txtDuration.ClientID %>').value      = '';
            document.getElementById('<%= hfScheduleId.ClientID %>').value     = '';
            document.getElementById('<%= hfIsEdit.ClientID %>').value         = 'false';
        }

        // ─── Form validation ──────────────────────────────────────────────────────────

        function validateForm() {
            var dep = document.getElementById('<%= txtDepartureTime.ClientID %>').value;
            if (!dep) {
                alert('Please enter the departure time.');
                return false;
            }
            return true;
        }

        // ─── Success toast ────────────────────────────────────────────────────────────

        function showSuccess(message) {
            var div = document.createElement('div');
            div.className = 'success-message';
            div.textContent = message;
            document.body.appendChild(div);
            setTimeout(function () {
                div.style.opacity = '0';
                setTimeout(function () { if (div.parentNode) div.parentNode.removeChild(div); }, 300);
            }, 3000);
        }

        // ─── Close on outside click ──────────────────────────────────────────────────

        window.onclick = function (event) {
            var modal = document.getElementById('scheduleModal');
            if (event.target === modal) hideModal();
        };

        // ─── Prevent resubmission on refresh ─────────────────────────────────────────

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // ─── Attach time change listeners after DOM ready ─────────────────────────────

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('<%= txtDepartureTime.ClientID %>').addEventListener('change', calcDuration);
            document.getElementById('<%= txtArrivalTime.ClientID %>').addEventListener('change', calcDuration);
        });

        // ─── Page load: reopen if edit postback ──────────────────────────────────────

        window.onload = function () {
            var isEdit = document.getElementById('<%= hfIsEdit.ClientID %>').value;
            if (isEdit === 'true') {
                document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Edit Schedule';
                calcDuration();
                showModal();
            }
        };
    </script>
</asp:Content>
