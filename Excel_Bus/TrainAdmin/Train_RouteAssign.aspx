<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_RouteAssign.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_RouteAssign" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .assign-page {
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

        .assign-table {
            width: 100%;
            border-collapse: collapse;
        }

            .assign-table thead { background: #f8f9fa; }

            .assign-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .assign-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .assign-table tbody tr:hover { background: #f8f9fa; }

        .status-badge {
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .status-enabled { background: #d1f4e0; color: #00a854; }
        .status-disabled { background: #ffe0e0; color: #ff4444; }

        .action-buttons { display: flex; gap: 8px; align-items: center; }

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
            max-width: 500px;
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

        .form-group { margin-bottom: 15px; }

        .form-label { display: block; margin-bottom: 5px; font-weight: 500; color: #2c3e50; }

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

        .train-badge {
            background: #e8f4fd; color: #0066cc;
            padding: 2px 8px; border-radius: 4px;
            font-size: 12px; font-weight: 600;
            font-family: monospace;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="assign-page">

        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Assigned Trains</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New"
                CssClass="btn-add-new"
                OnClientClick="openAddModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvAssignedTrains" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="assign-table"
                    GridLines="None"
                    OnRowCommand="gvAssignedTrains_RowCommand"
                    DataKeyNames="TrainAssignId">
                    <Columns>

                        <asp:BoundField DataField="RouteName" HeaderText="Route / Trip" />

                        <asp:TemplateField HeaderText="Train Name">
                            <ItemTemplate>
                                <%# Eval("TrainName") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Train No.">
                            <ItemTemplate>
                                <span class="train-badge"><%# Eval("TrainNumber") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="RegisterNo" HeaderText="Reg. No." />

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
                                    <asp:LinkButton ID="btnToggle" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "ACTIVE" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("TrainAssignId") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status?');">
                                        <%# Eval("Status").ToString() == "ACTIVE" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data"><p>No assigned trains found</p></div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="assignModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Assign Train to Route</h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">

                    <asp:HiddenField ID="hfKeepModalOpen" runat="server" Value="false" />
                    <%-- Stores JSON array of actively-assigned TrainIds for duplicate check --%>
                    <asp:HiddenField ID="hfAssignedTrainIds" runat="server" Value="[]" />

                    <div class="form-group">
                        <label class="form-label">Route <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlTrip" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select a route</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Train <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlTrain" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select a train</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        CssClass="btn-submit"
                        OnClick="btnSubmit_Click"
                        OnClientClick="return validateAssignForm();" />
                </div>
            </div>
        </div>

    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function openAddModal() {
            showModal();
            return false;
        }

        function showModal() {
            var modal = document.getElementById('assignModal');
            if (modal) { modal.classList.add('show'); document.body.style.overflow = 'hidden'; }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('assignModal');
            if (modal) { modal.classList.remove('show'); document.body.style.overflow = ''; }
            return false;
        }

        function validateAssignForm() {
            var route = document.getElementById('<%= ddlTrip.ClientID %>').value;
            var train = document.getElementById('<%= ddlTrain.ClientID %>').value;
            var trainText = document.getElementById('<%= ddlTrain.ClientID %>').options[
                document.getElementById('<%= ddlTrain.ClientID %>').selectedIndex
            ].text;

            if (!route) { alert('Please select a route.'); return false; }
            if (!train) { alert('Please select a train.'); return false; }

            // ── Duplicate check against already-active assignments ──
            try {
                var raw = document.getElementById('<%= hfAssignedTrainIds.ClientID %>').value;
                var assigned = JSON.parse(raw);  // array of { trainId, routeName }
                for (var i = 0; i < assigned.length; i++) {
                    if (assigned[i].trainId === train) {
                        alert('⚠️ "' + trainText + '" is already assigned to route "' +
                              assigned[i].routeName + '".\n\nPlease select a different train.');
                        return false;
                    }
                }
            } catch (e) { /* ignore parse errors, server will catch it */ }

            return true;
        }

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

        window.onclick = function (event) {
            var modal = document.getElementById('assignModal');
            if (event.target === modal) hideModal();
        };

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // Re-open modal after postback (e.g. trip selection changed)
        window.onload = function () {
            var keepOpen = document.getElementById('<%= hfKeepModalOpen.ClientID %>').value;
            if (keepOpen === 'true') {
                showModal();
            }
        };
    </script>
</asp:Content>
