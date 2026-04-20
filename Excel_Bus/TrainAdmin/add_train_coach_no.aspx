<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="add_train_coach_no.aspx.cs" Inherits="Excel_Bus.TrainAdmin.add_train_coach_no" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .coach-page {
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

        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            margin-bottom: 24px;
        }

        .card-body {
            padding: 24px;
        }

        .form-row {
            display: flex;
            gap: 16px;
            align-items: flex-end;
            flex-wrap: wrap;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
            flex: 1;
            min-width: 180px;
        }

        .form-group.no-of-coach {
            max-width: 160px;
            flex: 0 0 160px;
        }

        .form-label {
            font-weight: 500;
            color: #2c3e50;
            font-size: 14px;
        }

        .required {
            color: #dc3545;
        }

        .form-control,
        .form-select {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
            color: #495057;
            background: white;
        }

            .form-control:focus,
            .form-select:focus {
                outline: none;
                border-color: #7367f0;
                box-shadow: 0 0 0 0.2rem rgba(115, 103, 240, 0.25);
            }

            .form-select:disabled,
            .form-control:disabled {
                background: #f8f9fa;
                color: #adb5bd;
                cursor: not-allowed;
            }

        .btn-add {
            padding: 10px 22px;
            background: #47a17b;
            color: white;
            border: 1px solid #28c76f;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            white-space: nowrap;
            height: 42px;
            align-self: flex-end;
        }

            .btn-add:hover {
                background: #24b263;
            }

            .btn-add:disabled {
                background: #adb5bd;
                border-color: #adb5bd;
                cursor: not-allowed;
            }

        .table-responsive {
            overflow-x: auto;
        }

        .coach-table {
            width: 100%;
            border-collapse: collapse;
        }

            .coach-table thead { 
                background: #f8f9fa;
            }

            .coach-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
                white-space: nowrap;
            }

            .coach-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .coach-table tbody tr:hover {
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

        .no-data {
            text-align: center;
            padding: 40px;
            color: #6c757d;
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

        .train-badge {
            background: #e8f4fd;
            color: #0066cc;
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            font-family: monospace;
        }

        .section-divider {
            font-size: 15px;
            font-weight: 600;
            color: #2c3e50;
            margin-bottom: 16px;
            padding-bottom: 8px;
            border-bottom: 1px solid #dee2e6;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
        <asp:Label ID="lblError" runat="server"></asp:Label>
    </asp:Panel>

    <div class="page-header">
        <h1 class="page-title">Train Coach Configuration</h1>
    </div>

    <!-- Add Form Card -->
    <div class="card">
        <div class="card-body">
            <div class="section-divider">Add Coach Details</div>
            <div class="form-row">

                <!-- Train Dropdown -->
                <div class="form-group">
                    <label class="form-label">Train <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlTrain" runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlTrain_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="-- Select Train --" />
                    </asp:DropDownList>
                </div>

                <!-- Coach Type Dropdown (populated after train selection) -->
                <div class="form-group">
                    <label class="form-label">Coach Type <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlCoachType" runat="server"
                        CssClass="form-select"
                        Enabled="false">
                        <asp:ListItem Value="" Text="-- Select Coach Type --" />
                    </asp:DropDownList>
                </div>

                <!-- No of Coach -->
                <div class="form-group no-of-coach">
                    <label class="form-label">No. of Coach <span class="required">*</span></label>
                    <asp:TextBox ID="txtNoOfCoach" runat="server"
                        CssClass="form-control"
                        placeholder="e.g. 4"
                        TextMode="Number"></asp:TextBox>
                </div>

                <!-- Add Button -->
                <asp:Button ID="btnAdd" runat="server"
                    Text="+ Add"
                    CssClass="btn-add"
                    OnClick="btnAdd_Click"
                    OnClientClick="return validateCoachForm();" />
            </div>
        </div>
    </div>

    <!-- List Card -->
    <div class="card">
        <div class="table-responsive">
            <asp:GridView ID="gvCoachDetails" runat="server"
                AutoGenerateColumns="false"
                CssClass="coach-table"
                GridLines="None">
                <Columns>

                    <asp:TemplateField HeaderText="Train">
                        <ItemTemplate>
                            <span class="train-badge"><%# Eval("trainId") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="trainName" HeaderText="Train Name" />
                    <asp:BoundField DataField="trainNumber" HeaderText="Train No." />
                    <asp:BoundField DataField="coachTypeId" HeaderText="Coach Type ID" />
                    <asp:BoundField DataField="noOfCoach" HeaderText="No. of Coach" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# GetStatusClass(Eval("status")) %>'>
                                <%# GetStatusBadge(Eval("status")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <%--<asp:BoundField DataField="createdBy" HeaderText="Created By" />--%>

                   <%-- <asp:TemplateField HeaderText="Created At">
                        <ItemTemplate>
                            <%# Eval("createdAt") != null ? Convert.ToDateTime(Eval("createdAt")).ToString("dd MMM yyyy, hh:mm tt") : "" %>
                        </ItemTemplate>
                    </asp:TemplateField>--%>

                </Columns>
                <EmptyDataTemplate>
                    <div class="no-data">
                        <p>No coach records found</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>

        function validateCoachForm() {
            var train = document.getElementById('<%= ddlTrain.ClientID %>').value;
            var coach = document.getElementById('<%= ddlCoachType.ClientID %>').value;
            var noOfCoach = document.getElementById('<%= txtNoOfCoach.ClientID %>').value.trim();

            if (!train) {
                alert('Please select a train.');
                return false;
            }
            if (!coach) {
                alert('Please select a coach type.');
                return false;
            }
            if (!noOfCoach || isNaN(parseInt(noOfCoach)) || parseInt(noOfCoach) <= 0) {
                alert('Please enter a valid number of coaches (must be greater than 0).');
                return false;
            }
            return true;
        }

        function showSuccess(message) {
            var div = document.createElement('div');
            div.className = 'success-message';
            div.textContent = message;
            document.body.appendChild(div);

            setTimeout(function () {
                div.style.opacity = '0';
                setTimeout(function () {
                    if (div.parentNode) div.parentNode.removeChild(div);
                }, 300);
            }, 3000);
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>
