<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="RailwayStations.aspx.cs" Inherits="Excel_Bus.TrainAdmin.RailwayStations" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .station-page {
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
            background: #47a17b;
            color: white;
            border: 1px solid #28c76f;
            border-radius: 34px;
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

        .station-table {
            width: 100%;
            border-collapse: collapse;
        }

            .station-table thead {
                background: #f8f9fa;
            }

            .station-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .station-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .station-table tbody tr:hover {
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
            max-width: 600px;
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

        .form-row {
            display: flex;
            gap: 15px;
        }

            .form-row .form-group {
                flex: 1;
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

        .code-badge {
            background: #e8f4fd;
            color: #0066cc;
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            font-family: monospace;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <%-- <div class="station-page">--%>

        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Railway Stations</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New"
                CssClass="btn-add-new"
                OnClientClick="openAddNewModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvStations" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="station-table"
                    GridLines="None"
                    OnRowCommand="gvStations_RowCommand"
                    DataKeyNames="StationId">
                    <Columns>

                        <asp:BoundField DataField="StationName" HeaderText="Station Name" />

                        <asp:TemplateField HeaderText="Code">
                            <ItemTemplate>
                                <span class="code-badge"><%# Eval("StationCode") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="City"     HeaderText="City" />
                        <asp:BoundField DataField="Province" HeaderText="Province" />
                        <asp:BoundField DataField="LocationDescription" HeaderText="Location" />

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
                                        CommandName="EditStation"
                                        CommandArgument='<%# Eval("StationId") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnToggle" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "ACTIVE" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("StationId") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status?');">
                                        <%# Eval("Status").ToString() == "ACTIVE" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>

                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No railway stations found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="stationModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Railway Station"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">

                    <asp:HiddenField ID="hfStationId" runat="server" />
                    <asp:HiddenField ID="hfIsEdit"    runat="server" Value="false" />

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Station Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtStationName" runat="server" CssClass="form-control"
                                placeholder="e.g. New Delhi Railway Station"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Station Code <span class="required">*</span></label>
                            <asp:TextBox ID="txtStationCode" runat="server" CssClass="form-control"
                                placeholder="e.g. NDLS" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">City <span class="required">*</span></label>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control"
                                placeholder="Enter city"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Province</label>
                            <asp:TextBox ID="txtProvince" runat="server" CssClass="form-control"
                                placeholder="Enter province / state"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Location Description</label>
                        <asp:TextBox ID="txtLocationDesc" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="2"
                            placeholder="Brief description of station location"></asp:TextBox>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Latitude</label>
                            <asp:TextBox ID="txtLatitude" runat="server" CssClass="form-control"
                                placeholder="e.g. 28.6448"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Longitude</label>
                            <asp:TextBox ID="txtLongitude" runat="server" CssClass="form-control"
                                placeholder="e.g. 77.2167"></asp:TextBox>
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

   <%-- </div>--%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // ─── Modal open/close ────────────────────────────────────────────────────────

        function openAddNewModal() {
            clearStationForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Railway Station';
            showModal();
            return false;
        }

        function showModal() {
            var modal = document.getElementById('stationModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('stationModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        // ─── Form clear ───────────────────────────────────────────────────────────────

        function clearStationForm() {
            document.getElementById('<%= txtStationName.ClientID %>').value = '';
            document.getElementById('<%= txtStationCode.ClientID %>').value = '';
            document.getElementById('<%= txtCity.ClientID %>').value = '';
            document.getElementById('<%= txtProvince.ClientID %>').value = '';
            document.getElementById('<%= txtLocationDesc.ClientID %>').value = '';
            document.getElementById('<%= txtLatitude.ClientID %>').value = '';
            document.getElementById('<%= txtLongitude.ClientID %>').value = '';
            document.getElementById('<%= hfStationId.ClientID %>').value = '';
            document.getElementById('<%= hfIsEdit.ClientID %>').value = 'false';
        }

        // ─── Form validation ──────────────────────────────────────────────────────────

        function validateForm() {
            var name = document.getElementById('<%= txtStationName.ClientID %>').value.trim();
            var code = document.getElementById('<%= txtStationCode.ClientID %>').value.trim();
            var city = document.getElementById('<%= txtCity.ClientID %>').value.trim();

            if (!name) {
                alert('Please enter the station name.');
                return false;
            }
            if (!code) {
                alert('Please enter the station code.');
                return false;
            }
            if (!city) {
                alert('Please enter the city.');
                return false;
            }

            var lat = document.getElementById('<%= txtLatitude.ClientID %>').value.trim();
            var lng = document.getElementById('<%= txtLongitude.ClientID %>').value.trim();

            if (lat && isNaN(parseFloat(lat))) {
                alert('Please enter a valid latitude.');
                return false;
            }
            if (lng && isNaN(parseFloat(lng))) {
                alert('Please enter a valid longitude.');
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
            var modal = document.getElementById('stationModal');
            if (event.target === modal) {
                hideModal();
            }
        };

        // ─── Prevent form resubmission on refresh ─────────────────────────────────────

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // ─── Page load: reopen modal if edit postback ────────────────────────────────

        window.onload = function () {
            var isEdit = document.getElementById('<%= hfIsEdit.ClientID %>').value;
            if (isEdit === 'true') {
                document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Edit Railway Station';
                showModal();
            }
        };
    </script>
</asp:Content>
