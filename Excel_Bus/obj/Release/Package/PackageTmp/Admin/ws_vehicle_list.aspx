<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ws_vehicle_list.aspx.cs" Inherits="Excel_Bus.Admin.ws_vehicle_list" Async="true" %>

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
                                           <%-- <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-edit"
                                                CommandName="EditRecord" CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false">
                                                ✏️ Edit
                                            </asp:LinkButton>--%>
                                            <asp:LinkButton ID="btnDisable" runat="server"
                                                CssClass='<%# Convert.ToInt32(Eval("Status")) == 1 ? "btn-disable" : "btn-enable" %>'
                                                CommandName="DisableVehicle"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClientClick="return confirm('Are you sure you want to change the status?');"
                                                CausesValidation="false">
                                                <%# Convert.ToInt32(Eval("Status")) == 1 ? "🚫 Disable" : "✓ Enable" %>
                                            </asp:LinkButton>
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

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
