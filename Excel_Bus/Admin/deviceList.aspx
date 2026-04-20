<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="deviceList.aspx.cs" Inherits="Excel_Bus.Admin.deviceList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #f5f7fa;
        }
        
        .device-page {
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
        }

        .table-responsive {
            overflow-x: auto;
        }

        .device-table {
            width: 100%;
            border-collapse: collapse;
        }

        .device-table thead {
            background: #ffffff;
            border-bottom: 2px solid #dee2e6;
        }

        .device-table thead tr {
            background: #ffffff !important;
        }

        .device-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #2c3e50 !important;
            border-bottom: 2px solid #dee2e6;
            font-size: 14px;
            background: #ffffff !important;
        }

        .device-table td {
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            color: #495057;
            font-size: 14px;
        }

        .device-table tbody tr {
            background-color: #ffffff;
            transition: all 0.2s ease;
        }

        .device-table tbody tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        .device-table tbody tr:hover {
            background-color: #edf2f7;
        }

        .device-table tbody tr:last-child td {
            border-bottom: none;
        }

        .status-dropdown {
            border: 2px solid #e2e8f0;
            border-radius: 6px;
            padding: 8px 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            min-width: 110px;
            font-size: 13px;
            appearance: none;
            background-position: right 10px center;
            background-repeat: no-repeat;
            background-size: 12px;
        }

        .status-dropdown.status-active {
            background-color: #d4edda;
            color: #155724;
            border-color: #28a745;
            background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 12 12"><path fill="%23155724" d="M10.293 3.293L6 7.586 1.707 3.293A1 1 0 00.293 4.707l5 5a1 1 0 001.414 0l5-5a1 1 0 10-1.414-1.414z"/></svg>');
        }

        .status-dropdown.status-inactive {
            background-color: #f8d7da;
            color: #721c24;
            border-color: #dc3545;
            background-image: url('data:image/svg+xml;charset=UTF-8,<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 12 12"><path fill="%23721c24" d="M10.293 3.293L6 7.586 1.707 3.293A1 1 0 00.293 4.707l5 5a1 1 0 001.414 0l5-5a1 1 0 10-1.414-1.414z"/></svg>');
        }

        .status-dropdown:hover {
            box-shadow: 0 4px 12px rgba(0,0,0,0.12);
            transform: translateY(-1px);
        }

        .status-dropdown:focus {
            outline: none;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.2);
            border-color: #667eea;
        }

        .status-column {
            text-align: center;
            width: 140px;
        }

        .id-column {
            width: 80px;
            text-align: center;
            font-weight: 600;
            color: #667eea;
        }

        .device-id-column {
            font-family: 'Courier New', monospace;
            font-size: 12px;
            color: #4a5568;
            font-weight: 500;
        }

        .device-name-column {
            font-weight: 500;
            color: #2d3748;
        }

        .model-column {
            color: #4a5568;
        }

        .error-message {
            color: #c53030;
            text-align: center;
            font-weight: 500;
            margin: 0 0 25px 0;
            padding: 14px 20px;
            background-color: #fff5f5;
            border: 1px solid #feb2b2;
            border-radius: 8px;
            font-size: 14px;
        }

        .success-message {
            color: #2f855a;
            text-align: center;
            font-weight: 500;
            margin: 0 0 25px 0;
            padding: 14px 20px;
            background-color: #f0fff4;
            border: 1px solid #9ae6b4;
            border-radius: 8px;
            font-size: 14px;
        }

        .no-data {
            text-align: center;
            padding: 40px;
            color: #6c757d;
        }

        @media (max-width: 768px) {
            .device-page {
                padding: 15px;
            }
            
            .page-title {
                font-size: 20px;
            }

            .device-table {
                font-size: 12px;
            }

            .device-table th, .device-table td {
                padding: 10px 12px;
            }

            .status-dropdown {
                min-width: 90px;
                font-size: 12px;
                padding: 6px 10px;
            }
        }
    </style>
    
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
        
     <div class="device-page">
       
        <div class="page-header">
            <h1 class="page-title">Device Information List</h1>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvDeviceList" runat="server" CssClass="device-table" AutoGenerateColumns="false" 
                              EmptyDataText="No devices found." DataKeyNames="id"
                              OnRowDataBound="gvDeviceList_RowDataBound" GridLines="None" ShowHeader="true">
                    <HeaderStyle BackColor="White" ForeColor="#2c3e50" Font-Bold="true" BorderColor="#dee2e6" BorderWidth="2px" BorderStyle="Solid" />
                    <Columns>
                        <asp:BoundField DataField="id" HeaderText="ID" ItemStyle-CssClass="id-column" 
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:BoundField DataField="deviceName" HeaderText="Device Name" ItemStyle-CssClass="device-name-column"
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:BoundField DataField="deviceId" HeaderText="Device ID" ItemStyle-CssClass="device-id-column"
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:BoundField DataField="modelNo" HeaderText="Model No" ItemStyle-CssClass="model-column"
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:BoundField DataField="companyName" HeaderText="Company Name"
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:BoundField DataField="versionNo" HeaderText="Version No"
                                        HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50" />
                        <asp:TemplateField HeaderText="Is Active" ItemStyle-CssClass="status-column"
                                          HeaderStyle-BackColor="White" HeaderStyle-ForeColor="#2c3e50">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlIsActive" runat="server" 
                                                CssClass='<%# "status-dropdown " + (Convert.ToBoolean(Eval("isActive")) ? "status-active" : "status-inactive") %>'
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlIsActive_SelectedIndexChanged">
                                    <asp:ListItem Value="true" Text="Enabled"></asp:ListItem>
                                    <asp:ListItem Value="false" Text="Disabled"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No devices found.</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
     <script type="text/javascript">
         // Global function to handle dropdown confirmation and selection
         function handleStatusChange(ddlElement, currentStatus) {
             var newStatus = ddlElement.value;
             if (newStatus !== currentStatus) {
                 var statusText = newStatus === 'true' ? 'enabled' : 'disabled';
                 var message = 'Are you sure you want to change this device status to ' + statusText + '?';

                 if (newStatus === 'false') {
                     message += '\n\nNote: This device will no longer be active.';
                 }

                 if (!confirm(message)) {
                     // Revert selection if cancelled
                     ddlElement.value = currentStatus;
                     return false;
                 }
             }
             return true;
         }
     </script>
</asp:Content>
