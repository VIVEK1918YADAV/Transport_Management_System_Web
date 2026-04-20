<%@ Page Title="Active Users" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" Async="true" CodeBehind="ActiveUsers.aspx.cs" Inherits="Excel_Bus.ActiveUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .active-users-container {
            padding: 20px;
            margin-top: 20px;
        }
        
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            flex-wrap: wrap;
            gap: 15px;
            padding-top: 10px;
        }
        
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            margin: 0;
        }
        
        .search-box {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
        
        .search-input {
            padding: 8px 15px;
            border: 1px solid #ddd;
            border-radius: 4px;
            min-width: 250px;
            font-size: 14px;
        }
        
        .btn-search {
            padding: 8px 20px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }
        
        .btn-search:hover {
            background-color: #0056b3;
        }

        .btn-back {
            padding: 8px 20px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }
        
        .btn-back:hover {
            background-color: #5a6268;
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
        
        .users-table {
            width: 100%;
            border-collapse: collapse;
            background: white;
        }
        
        .users-table thead {
            background-color: #fffacd;
        }
        
        .users-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #333;
            border-bottom: 2px solid #dee2e6;
            font-size: 14px;
        }
        
        .users-table td {
            padding: 15px;
            border-bottom: 1px solid #f0f0f0;
            font-size: 14px;
            color: #666;
        }
        
        .users-table tbody tr:hover {
            background-color: #f8f9fa;
        }
        
        .user-name {
            font-weight: 600;
            color: #333;
            margin-bottom: 3px;
        }
        
        .username-link {
            color: #007bff;
            text-decoration: none;
            font-size: 12px;
            cursor: pointer;
        }
        
        .username-link:hover {
            text-decoration: underline;
        }
        
        .country-code {
            font-weight: 600;
            color: #666;
        }
        
        .date-time {
            color: #333;
            display: block;
        }
        
        .relative-time {
            color: #999;
            font-size: 12px;
        }
        
        .balance-amount {
            font-weight: 600;
            color: #333;
        }
        
        .btn-details {
            padding: 6px 15px;
            background-color: transparent;
            color: #007bff;
            border: 1px solid #007bff;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            text-decoration: none;
        }
        
        .btn-details:hover {
            background-color: #007bff;
            color: white;
        }
        
        .btn-details i {
            font-size: 14px;
        }
        
        .no-data {
            text-align: center;
            padding: 40px;
            color: #999;
        }
        
        .loading {
            text-align: center;
            padding: 40px;
            color: #666;
        }
        
        .error-message {
            background-color: #f8d7da;
            color: #721c24;
            padding: 15px;
            border-radius: 4px;
            margin-bottom: 20px;
        }

        /* User Details Styles */
        .user-details-container {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 30px;
        }

        .detail-header {
            border-bottom: 2px solid #fffacd;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }

        .detail-header h2 {
            margin: 0 0 5px 0;
            color: #333;
            font-size: 28px;
        }

        .detail-header .username {
            color: #007bff;
            font-size: 16px;
        }

        .details-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 25px;
            margin-bottom: 30px;
        }

        .detail-section {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 6px;
        }

        .detail-section h3 {
            font-size: 16px;
            color: #333;
            margin: 0 0 15px 0;
            font-weight: 600;
            border-bottom: 1px solid #dee2e6;
            padding-bottom: 10px;
        }

        .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #e9ecef;
        }

        .detail-row:last-child {
            border-bottom: none;
        }

        .detail-label {
            font-weight: 600;
            color: #666;
            font-size: 14px;
        }

        .detail-value {
            color: #333;
            font-size: 14px;
            text-align: right;
        }

        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-active {
            background-color: #d4edda;
            color: #155724;
        }

        .status-inactive {
            background-color: #f8d7da;
            color: #721c24;
        }

        .verified-yes {
            color: #28a745;
            font-weight: 600;
        }

        .verified-no {
            color: #dc3545;
            font-weight: 600;
        }

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .active-users-container {
                padding: 15px;
                margin-top: 15px;
            }
            
            .page-header {
                flex-direction: column;
                align-items: stretch;
            }
            
            .search-box {
                width: 100%;
            }
            
            .search-input {
                width: 100%;
                min-width: auto;
            }

            .details-grid {
                grid-template-columns: 1fr;
            }

            .user-details-container {
                padding: 20px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="active-users-container">
        
        <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>
        
        <!-- Users List Panel -->
        <asp:Panel ID="pnlUsersList" runat="server" Visible="true">
            <div class="page-header">
                <h6 class="page-title">Active Users</h6>
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                        placeholder="Username / Email" />
                    <asp:Button ID="btnSearch" runat="server" Text="Search" 
                        CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
            </div>
            
            <div class="card">
                <div class="table-responsive">
                    <asp:GridView ID="gvUsers" runat="server" 
                        CssClass="users-table" 
                        AutoGenerateColumns="false"
                        OnRowDataBound="gvUsers_RowDataBound"
                        OnRowCommand="gvUsers_RowCommand"
                        ShowHeader="true"
                        GridLines="None"
                        EmptyDataText="No active users found">
                        
                        <Columns>
                            <asp:TemplateField HeaderText="User">
                                <ItemTemplate>
                                    <div class="user-name"><%# Eval("Firstname") %> <%# Eval("Lastname") %></div>
                                    <asp:LinkButton ID="lnkUsername" runat="server" 
                                        CssClass="username-link"
                                        CommandName="ViewDetails"
                                        CommandArgument='<%# Eval("Id") %>'
                                        Text='<%# "@" + Eval("Username") %>' 
                                        Visible='<%# !string.IsNullOrEmpty(Eval("Username")?.ToString()) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Email-Mobile">
                                <ItemTemplate>
                                    <%# Eval("Email") %><br />
                                    <%# Eval("DialCode") %><%# Eval("Mobile") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Country">
                                <ItemTemplate>
                                    <span class="country-code" title='<%# Eval("CountryName") %>'>
                                        <%# Eval("CountryName") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Joined At">
                                <ItemTemplate>
                                    <span class="date-time"><%# FormatDateTime(Eval("CreatedAt")) %></span>
                                    <span class="relative-time"><%# GetRelativeTime(Eval("CreatedAt")) %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Balance">
                                <ItemTemplate>
                                    <span class="balance-amount">
                                        <%# FormatBalance(Eval("Balance")) %> CDF
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDetails" runat="server" 
                                        CssClass="btn-details"
                                        CommandName="ViewDetails"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        <i class="las la-desktop"></i> Details
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        
                        <EmptyDataTemplate>
                            <div class="no-data">
                                <p>No active users found</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </asp:Panel>

        <!-- User Details Panel -->
        <asp:Panel ID="pnlUserDetails" runat="server" Visible="false">
            <div class="page-header">
                <h6 class="page-title">User Details</h6>
                <asp:Button ID="btnBackToList" runat="server" CssClass="btn-back" 
                    Text="← Back to List" OnClick="btnBackToList_Click" />
            </div>

            <div class="user-details-container">
                <div class="detail-header">
                    <h2><asp:Label ID="lblUserName" runat="server" Text="" /></h2>
                    <span class="username"><asp:Label ID="Label1" runat="server" Text="" /></span>
                </div>

                <div class="details-grid">
                    <!-- Contact Information -->
                    <div class="detail-section">
                        <h3>Contact Information</h3>
                        <div class="detail-row">
                            <span class="detail-label">Email:</span>
                            <span class="detail-value"><asp:Label ID="lblEmail" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Mobile:</span>
                            <span class="detail-value"><asp:Label ID="lblMobile" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Email Verified:</span>
                            <span class="detail-value"><asp:Label ID="lblEmailVerified" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Mobile Verified:</span>
                            <span class="detail-value"><asp:Label ID="lblMobileVerified" runat="server" Text="" /></span>
                        </div>
                    </div>

                    <!-- Location Information -->
                    <div class="detail-section">
                        <h3>Location</h3>
                        <div class="detail-row">
                            <span class="detail-label">Country:</span>
                            <span class="detail-value"><asp:Label ID="lblCountry" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">State:</span>
                            <span class="detail-value"><asp:Label ID="lblState" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">City:</span>
                            <span class="detail-value"><asp:Label ID="lblCity" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">ZIP Code:</span>
                            <span class="detail-value"><asp:Label ID="lblZip" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Address:</span>
                            <span class="detail-value"><asp:Label ID="lblAddress" runat="server" Text="" /></span>
                        </div>
                    </div>

                    <!-- Account Information -->
                    <div class="detail-section">
                        <h3>Account Information</h3>
                        <div class="detail-row">
                            <span class="detail-label">Balance:</span>
                            <span class="detail-value"><asp:Label ID="lblBalance" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Status:</span>
                            <span class="detail-value"><asp:Label ID="lblStatus" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Profile Complete:</span>
                            <span class="detail-value"><asp:Label ID="lblProfileComplete" runat="server" Text="" /></span>
                        </div>
                    </div>

                    <!-- Timeline -->
                    <div class="detail-section">
                        <h3>Timeline</h3>
                        <div class="detail-row">
                            <span class="detail-label">Joined At:</span>
                            <span class="detail-value"><asp:Label ID="lblJoinedAt" runat="server" Text="" /></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Last Updated:</span>
                            <span class="detail-value"><asp:Label ID="lblUpdatedAt" runat="server" Text="" /></span>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>