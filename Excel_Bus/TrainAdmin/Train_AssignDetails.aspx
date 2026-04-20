<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_AssignDetails.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_AssignDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #MainContent, [id$="MainContent"] {
            display: block !important;
            width: 100%;
        }

        .form-container {
            background: #ffffff;
            padding: 30px;
            padding-top: 40px;
            border-radius: 10px;
            box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.1);
            margin: 0 0 20px 0;
            display: block;
        }

        h2, h3 {
            color: #333 !important;
            margin: 0 0 25px 0 !important;
            display: block !important;
            visibility: visible !important;
        }

        h2 {
            font-size: 28px !important;
            text-align: left !important;
            font-weight: 600 !important;
            margin-top: 10px !important;
        }

        h3 {
            font-size: 24px !important;
            text-align: left !important;
            font-weight: 600 !important;
        }

        .form-row {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 20px;
            width: 100%;
        }

        .form-group {
            flex: 1 1 calc(50% - 10px);
            min-width: 0;
            display: block;
        }

        label {
            display: block !important;
            font-weight: 600;
            margin-bottom: 8px;
            color: #333;
            font-size: 14px;
            visibility: visible !important;
        }

        .aspNetInput, .aspNetDropdown {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            box-sizing: border-box;
            background-color: #fff;
            transition: all 0.3s;
        }

        .aspNetInput:focus, .aspNetDropdown:focus {
            outline: none;
            border-color: #5b6ee6;
            box-shadow: 0 0 0 3px rgba(91, 110, 230, 0.1);
        }

        .btn {
            background-color: #5b6ee6;
            color: white;
            padding: 14px 0;
            width: 100%;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn:hover {
            background-color: #4a5dd5;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(91, 110, 230, 0.3);
        }

        .btn-small {
            background-color: #5b6ee6;
            color: white;
            padding: 8px 12px;
            border: none;
            border-radius: 5px;
            font-size: 12px;
            font-weight: 500;
            cursor: pointer;
            margin: 0;
            transition: all 0.3s;
            white-space: nowrap;
        }

        .btn-small:hover {
            background-color: #4a5dd5;
            transform: translateY(-1px);
        }

        .btn-warning {
            background-color: #f59e0b;
        }

        .btn-warning:hover {
            background-color: #d97706;
        }

        .result-message {
            text-align: center;
            margin-top: 20px;
            font-weight: bold;
            padding: 10px;
            border-radius: 5px;
            display: none;
        }

        .success {
            background-color: green;
            color: white;
        }

        .error {
            background-color: red;
            color: white;
        }

        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .table th, .table td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
        }

        .table th {
            background-color: #5b6ee6;
            color: white;
            font-weight: 600;
            text-align: center;
            padding: 14px 12px;
            font-size: 14px;
        }

        .table tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table tbody tr:hover {
            background-color: #f1f1f1;
        }

        .table td:last-child {
            text-align: center;
            white-space: nowrap;
        }

        .no-data {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 20px;
        }

        .status-dropdown {
            width: 120px;
            padding: 4px 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 12px;
            background-color: white;
            cursor: pointer;
        }

        .status-active {
            color: #28a745;
            border-color: #28a745;
        }

        .status-inactive {
            color: #dc3545;
            border-color: #dc3545;
        }

        .table-responsive {
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
            margin-top: 20px;
        }

        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 9999;
            align-items: center;
            justify-content: center;
        }

        .modal-overlay.show {
            display: flex;
        }

        .modal-content {
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0px 4px 20px rgba(0, 0, 0, 0.2);
            width: 90%;
            max-width: 800px;
            max-height: 80vh;
            overflow-y: auto;
            position: relative;
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 25px;
            padding-bottom: 15px;
            border-bottom: 2px solid #e0e0e0;
        }

        .modal-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
        }

        .close-btn {
            background: none;
            border: none;
            font-size: 30px;
            color: #999;
            cursor: pointer;
            line-height: 1;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .close-btn:hover {
            color: #333;
        }

        @media (max-width: 768px) {
            .form-group {
                flex: 1 1 100%;
            }

            .form-container {
                padding: 20px;
            }

            .modal-content {
                width: 95%;
                padding: 20px;
            }
        }
        /* In <style> block */
.countdown-warning {
    color: #dc3545;
    font-weight: 700;
    animation: blink 1s step-start infinite;
}

@keyframes blink {
    50% { opacity: 0.4; }
}

.table td.near-departure {
    background-color: #ffe0e0 !important;
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-container">
        <h2>Assign Resources</h2>

        <div class="form-row">
            <div class="form-group">
                <label for="ddlRoutes">Routes</label>
                <asp:DropDownList ID="ddlRoutes" runat="server" CssClass="aspNetDropdown">
                    <asp:ListItem Text="Select Route" Value="" />
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="ddlTrains">Train</label>
                <%-- FIX 11: Corrected typo "Select Teain" → "Select Train" --%>
                <asp:DropDownList ID="ddlTrains" runat="server" CssClass="aspNetDropdown">
                    <asp:ListItem Text="Select Train" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                <label for="ddlPos">Devices (POS)</label>
                <asp:DropDownList ID="ddlPos" runat="server" CssClass="aspNetDropdown">
                    <asp:ListItem Text="Select Device" Value="" />
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="ddlDrivers">Loco Pilot</label>
                <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="aspNetDropdown">
                    <asp:ListItem Text="Select Driver" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                <label for="ddlUsers">Ticket Inspector</label>
                <asp:DropDownList ID="ddlUsers" runat="server" CssClass="aspNetDropdown">
                    <asp:ListItem Text="Select Ticket Inspector" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <asp:Button ID="btnAssign" runat="server" Text="Assign" CssClass="btn" OnClick="btnAssign_Click" />
        <asp:Label ID="lblResult" runat="server" CssClass="result-message" />
    </div>

    <!-- Assignment List -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
   <ContentTemplate>
    <asp:Timer ID="timerRefresh" runat="server" Interval="30000" OnTick="timerRefresh_Tick" />

    <div class="form-container">
        <h3 style="display:flex; align-items:center; justify-content:space-between; flex-wrap:wrap; gap:10px;">
            <span>Assignment List</span>
            <span style="font-size:13px; color:#888; font-weight:400; display:flex; align-items:center; gap:6px;">
                <span>Auto-refreshes every 30s</span>
                <span style="color:#5b6ee6;">🕐</span>
                <span id="liveClock" style="font-size:15px; font-weight:600; color:#5b6ee6; font-family:monospace;"></span>
            </span>
        </h3>
        <div class="table-responsive">
            <asp:GridView ID="gvAssignments" runat="server" CssClass="table table-bordered"
                AutoGenerateColumns="false"
                EmptyDataText="No assignments found."
                OnRowCommand="gvAssignments_RowCommand"
                OnRowDataBound="gvAssignments_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="TripId" HeaderText="Trip ID" />
                    <asp:BoundField DataField="TrainId" HeaderText="Train No" />
                    <asp:BoundField DataField="DeviceId" HeaderText="Device ID" />
                    <asp:BoundField DataField="DriverName" HeaderText="Pilot" />
                    <asp:BoundField DataField="TicketInspectors" HeaderText="Ticket Inspector" />
                    <asp:BoundField DataField="AssignedAt" HeaderText="Assigned Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="DepartureTime" HeaderText="Departure" />
                    <asp:BoundField DataField="Countdown" HeaderText="Time Left" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="status-dropdown"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                <asp:ListItem Text="ACTIVE" Value="ACTIVE" />
                                <asp:ListItem Text="INACTIVE" Value="INACTIVE" />
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdnAssignId" runat="server" Value='<%# Eval("CrewAssignId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-small btn-warning"
                                CommandName="EditAssignment"
                                CommandArgument='<%# Eval("CrewAssignId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle CssClass="no-data" />
            </asp:GridView>
        </div>
    </div>
</ContentTemplate>
    </asp:UpdatePanel>
    
    <!-- Edit/Reassign Modal -->
    <div id="editModal" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <span class="modal-title">Edit / Reassign Resources</span>
                <button type="button" class="close-btn" onclick="closeEditModal()">&times;</button>
            </div>

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnEditAssignId" runat="server" />

                    <div class="form-row">
                        <div class="form-group">
                            <label for="ddlEditRoutes">Routes</label>
                            <asp:DropDownList ID="ddlEditRoutes" runat="server" CssClass="aspNetDropdown">
                                <asp:ListItem Text="Select Route" Value="" />
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="ddlEditTrains">Trains</label>
                            <asp:DropDownList ID="ddlEditTrains" runat="server" CssClass="aspNetDropdown">
                                <asp:ListItem Text="Select Train" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label for="ddlEditPos">Devices (POS)</label>
                            <asp:DropDownList ID="ddlEditPos" runat="server" CssClass="aspNetDropdown">
                                <asp:ListItem Text="Select Device" Value="" />
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="ddlEditDrivers">Loco Pilot</label>
                            <asp:DropDownList ID="ddlEditDrivers" runat="server" CssClass="aspNetDropdown">
                                <asp:ListItem Text="Select Loco Pilot" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label for="ddlEditUsers">Ticket Inspector</label>
                            <asp:DropDownList ID="ddlEditUsers" runat="server" CssClass="aspNetDropdown">
                                <asp:ListItem Text="Select Ticket Inspector" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                     
                    <asp:Button ID="btnReAssign" runat="server" Text="Update Assignment" CssClass="btn" OnClick="btnReAssign_Click" />
                    <asp:Label ID="lblEditResult" runat="server" CssClass="result-message" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        function showEditModal() {
            var modal = document.getElementById('editModal');
            if (modal) modal.classList.add('show');
        }

        function closeEditModal() {
            var modal = document.getElementById('editModal');
            if (modal) modal.classList.remove('show');
        }

        function updateStatusColor(ddl) {
            if (ddl.value === "ACTIVE") {
                ddl.style.color = "green";
            } else if (ddl.value === "INACTIVE") {
                ddl.style.color = "red";
            }
        }

        window.onload = function () {
            document.querySelectorAll('.status-dropdown').forEach(function (ddl) {
                updateStatusColor(ddl);
            });
        };

        function showMessage(message, isSuccess) {
            var resultMessage = document.getElementById("<%= lblResult.ClientID %>");
            if (resultMessage) {
                resultMessage.innerHTML = message;
                resultMessage.className = isSuccess ? "result-message success" : "result-message error";
                resultMessage.style.display = "block";
                setTimeout(function () {
                    resultMessage.style.display = "none";
                }, 5000);
            }
        }

        function showEditMessage(message, isSuccess) {
            var resultMessage = document.getElementById("<%= lblEditResult.ClientID %>");
            if (resultMessage) {
                resultMessage.innerHTML = message;
                resultMessage.className = isSuccess ? "result-message success" : "result-message error";
                resultMessage.style.display = "block";
                if (isSuccess) {
                    setTimeout(function () {
                        closeEditModal();
                        location.reload();
                    }, 2000);
                }
            }
        }

        if (typeof Sys !== 'undefined') {
            Sys.Application.add_load(function () {
                document.querySelectorAll('.status-dropdown').forEach(function (ddl) {
                    updateStatusColor(ddl);
                });
            });
        }

        // Live clock
        function startLiveClock() {
            function tick() {
                var now = new Date();
                var h = String(now.getHours()).padStart(2, '0');
                var m = String(now.getMinutes()).padStart(2, '0');
                var s = String(now.getSeconds()).padStart(2, '0');
                var el = document.getElementById('liveClock');
                if (el) el.textContent = h + ':' + m + ':' + s;
            }
            tick();
            setInterval(tick, 1000);
        }

        window.onload = function () {
            startLiveClock();
            document.querySelectorAll('.status-dropdown').forEach(updateStatusColor);
        };

        // Re-run after UpdatePanel partial refresh
        if (typeof Sys !== 'undefined') {
            Sys.Application.add_load(function () {
                startLiveClock();
                document.querySelectorAll('.status-dropdown').forEach(updateStatusColor);
            });
        }
    </script>
</asp:Content>
