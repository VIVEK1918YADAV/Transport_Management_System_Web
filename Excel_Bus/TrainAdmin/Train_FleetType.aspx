<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_FleetType.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_FleetType" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .fleet-type-page {
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
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .fleet-table {
            width: 100%;
            border-collapse: collapse;
        }

            .fleet-table thead {
                background: #f8f9fa;
            }

            .fleet-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
            }

            .fleet-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .fleet-table tbody tr:hover {
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
            max-width: 550px;
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
                box-shadow: 0 0 0 0.2rem rgba(115, 103, 240, 0.25);
            }

        .toggle-container {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .toggle-switch {
            position: relative;
            width: 60px;
            height: 30px;
        }

            .toggle-switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .toggle-slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: 0.3s;
            border-radius: 30px;
        }

            .toggle-slider:before {
                position: absolute;
                content: "";
                height: 22px;
                width: 22px;
                left: 4px;
                bottom: 4px;
                background-color: white;
                transition: 0.3s;
                border-radius: 50%;
            }

        input:checked + .toggle-slider {
            background-color: #28c76f;
        }

            input:checked + .toggle-slider:before {
                transform: translateX(30px);
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

            .btn-submit:hover {
                background: #0b5ed7;
            }

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

            .error-panel.show {
                opacity: 1;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fleet-type-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Fleet Type</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new"
                OnClientClick="openAddNewModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvFleetTypes" runat="server" AutoGenerateColumns="false"
                    CssClass="fleet-table" GridLines="None"
                    OnRowCommand="gvFleetTypes_RowCommand"
                    DataKeyNames="Id">
                    <Columns>

                        <asp:BoundField DataField="Name" HeaderText="Name" />
                       <%-- <asp:BoundField DataField="CoachType" HeaderText="Coach Type" />
                        <asp:BoundField DataField="CoachLayout" HeaderText="Coach Layout" />--%>
                        <asp:BoundField DataField="Deck" HeaderText="No of Deck" />
                        <asp:BoundField DataField="NoOfSeats" HeaderText="Total Seats" />

                        <asp:TemplateField HeaderText="Facilities">
                            <ItemTemplate>
                                <%# ((Excel_Bus.TrainAdmin.Train_FleetType)Page)
                                    .GetFacilitiesDisplay(Eval("Facilities")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <%-- ✅ FIX: GetStatusClasss and GetStatusBadge now compare against "ACTIVE" --%>
                                <span class='<%# GetStatusClasss(Eval("Status")) %>'>
                                    <%# GetStatusBadge(Eval("Status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">

                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CssClass="btn-edit"
                                        CommandName="EditFleet"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>

                                    <%-- ✅ FIX: Compare against "ACTIVE" (uppercase) to match DB value --%>
                                    <asp:LinkButton ID="btnDisable" runat="server"
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
                            <p>No fleet types found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="fleetModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Fleet Type"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfFleetId" runat="server" />
                    <asp:HiddenField ID="hfIsEdit" runat="server" Value="false" />
                    <asp:HiddenField ID="hfDeckSeatsData" runat="server" />

                    <div class="form-group">
                        <label class="form-label">Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter name"></asp:TextBox>
                    </div>

                   <%-- <div class="form-group">
                        <label class="form-label">Coach Type <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlCoachType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select an option</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Coach Layout <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlCoachLayout" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select an option</asp:ListItem>
                        </asp:DropDownList>
                    </div>--%>

                    <div class="form-group">
                        <label class="form-label">No of Deck <span class="required">*</span></label>
                        <asp:TextBox ID="txtDeck" runat="server" CssClass="form-control"
                            TextMode="Number" placeholder="Enter number of decks">
                        </asp:TextBox>
                    </div>

                    <div id="deckSeatsContainer"></div>

                    <div class="form-group">
                        <label class="form-label">No of Seats <span class="required">*</span></label>
                        <asp:TextBox ID="txtSeatNo" runat="server" CssClass="form-control"
                            TextMode="Number" placeholder="Enter number of seats"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Facilities <span class="required">*</span></label>
                        <asp:TextBox ID="txtFacilities" runat="server" CssClass="form-control"
                            placeholder="Enter facilities"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label class="form-label">AC Status</label>
                        <div class="toggle-container">
                            <label class="toggle-switch">
                                <asp:CheckBox ID="chkHasAc" runat="server" />
                                <span class="toggle-slider"></span>
                            </label>
                            <span>Air Conditioned</span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        CssClass="btn-submit" OnClick="btnSubmit_Click" OnClientClick="return validateForm();" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // ─── Modal open/close ────────────────────────────────────────────────────────

        function openAddNewModal() {
            clearFleetForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Fleet Type';
            showModal();
            return false;
        }

        function showModal() {
            var modal = document.getElementById('fleetModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('fleetModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        // ─── Form clear ───────────────────────────────────────────────────────────────

        function clearFleetForm() {
            document.getElementById('<%= txtName.ClientID %>').value = '';
           <%-- document.getElementById('<%= ddlCoachType.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= ddlCoachLayout.ClientID %>').selectedIndex = 0;--%>
            document.getElementById('<%= txtDeck.ClientID %>').value = '';
            document.getElementById('<%= txtSeatNo.ClientID %>').value = '';
            document.getElementById('<%= txtFacilities.ClientID %>').value = '';
            document.getElementById('<%= chkHasAc.ClientID %>').checked = false;
            document.getElementById('<%= hfFleetId.ClientID %>').value = '';
            document.getElementById('<%= hfIsEdit.ClientID %>').value = 'false';
            document.getElementById('<%= hfDeckSeatsData.ClientID %>').value = '';
            document.getElementById('deckSeatsContainer').innerHTML = '';
        }

        // ─── Edit: repopulate dynamic fields ─────────────────────────────────────────

        function loadEditData() {
            var deckSeatsData = document.getElementById('<%= hfDeckSeatsData.ClientID %>').value;
            var deckValue = document.getElementById('<%= txtDeck.ClientID %>').value;

            if (deckSeatsData && deckValue) {
                try {
                    var deckSeats = JSON.parse(deckSeatsData);
                    generateDeckSeatsInputs(parseInt(deckValue, 10), deckSeats);
                } catch (e) {
                    console.warn('Could not parse deck seats data:', e);
                }
            }
        }

        // ─── Form validation ──────────────────────────────────────────────────────────

        function validateForm() {
            var name       = document.getElementById('<%= txtName.ClientID %>').value.trim();
           <%-- var coachType  = document.getElementById('<%= ddlCoachType.ClientID %>').value;
            var seatLayout = document.getElementById('<%= ddlCoachLayout.ClientID %>').value;--%>
            var seatNo     = document.getElementById('<%= txtSeatNo.ClientID %>').value;

            if (!name) {
                alert('Please enter fleet type name');
                return false;
            }

            if (!coachType) {
                alert('Please select a coach type');
                return false;
            }

            if (!seatLayout) {
                alert('Please select seat layout');
                return false;
            }

            if (!seatNo || parseInt(seatNo, 10) < 1) {
                alert('Please enter a valid total number of seats');
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
                    if (successDiv.parentNode) {
                        successDiv.parentNode.removeChild(successDiv);
                    }
                }, 300);
            }, 3000);
        }

        // ─── Close modal on outside click ─────────────────────────────────────────────

        window.onclick = function (event) {
            var modal = document.getElementById('fleetModal');
            if (event.target === modal) {
                hideModal();
            }
        };

        // ─── Prevent form resubmission on refresh ─────────────────────────────────────

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // ─── Page load ────────────────────────────────────────────────────────────────

        window.onload = function () {
            var isEdit = document.getElementById('<%= hfIsEdit.ClientID %>').value;
            if (isEdit === 'true') {
                document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Edit Fleet Type';
                showModal();
                loadEditData();
            }
        };
    </script>
</asp:Content>
