<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_ActiveUser.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_ActiveUser" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .active-user-page {
            /*padding: 20px;
            background: #f5f7fa;
            margin-top: 50px;*/

            background: #dff3dc;
    padding: 40px;
    border-radius: 10px;
    box-shadow: 0px 4px 20px rgba(0, 0, 0, 0.15);
    margin: 0 auto;
        }

      

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 700;
            color: #1a202c;
            margin: 0;
        }

        .card {
            background: white;
            border-radius: 10px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08), 0 4px 12px rgba(0,0,0,0.05);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .user-table {
            width: 100%;
            border-collapse: collapse;
        }

        .user-table thead {
            background: #f8f9fb;
        }

        .user-table th {
            padding: 13px 16px;
            text-align: left;
            font-weight: 600;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.6px;
            color: #6b7280;
            white-space: nowrap;
            border-bottom: 2px solid #e9ecef;
        }

        .user-table td {
            padding: 13px 16px;
            border-bottom: 1px solid #f1f3f5;
            color: #374151;
            font-size: 14px;
            vertical-align: middle;
        }

        .user-table tbody tr:last-child td {
            border-bottom: none;
        }

        .user-table tbody tr:hover td {
            background: #f9fafb;
        }

        .status-badge {
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            display: inline-block;
        }

        .status-enabled {
            background: #dcfce7;
            color: #16a34a;
        }

        .status-disabled {
            background: #fee2e2;
            color: #dc2626;
        }

        .user-avatar {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            color: white;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 700;
            flex-shrink: 0;
        }

        .avatar-color-0 { background: #7367f0; }
        .avatar-color-1 { background: #28c76f; }
        .avatar-color-2 { background: #ff9f43; }
        .avatar-color-3 { background: #ea5455; }
        .avatar-color-4 { background: #00cfe8; }
        .avatar-color-5 { background: #e83e8c; }
        .avatar-color-6 { background: #6610f2; }
        .avatar-color-7 { background: #20c997; }

        .user-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            min-width: 160px;
        }

        .user-fullname {
            font-weight: 600;
            color: #1a202c;
            font-size: 14px;
        }

        .balance-positive {
            font-weight: 600;
            color: #16a34a;
        }

        .balance-negative {
            font-weight: 600;
            color: #dc2626;
        }

        .balance-zero {
            font-weight: 500;
            color: #9ca3af;
        }

        .mobile-cell {
            white-space: nowrap;
            font-size: 13px;
            color: #4b5563;
        }

        .location-cell {
            font-size: 13px;
            color: #6b7280;
        }

        .joined-cell {
            font-size: 13px;
            color: #9ca3af;
            white-space: nowrap;
        }

        .search-wrapper {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .search-box {
            position: relative;
        }

        .search-input {
            padding: 9px 14px 9px 36px;
            border: 1px solid #e5e7eb;
            border-radius: 6px;
            font-size: 14px;
            width: 240px;
            background: #fff;
            color: #374151;
            transition: border-color 0.2s, box-shadow 0.2s;
        }

        .search-input:focus {
            outline: none;
            border-color: #7367f0;
            box-shadow: 0 0 0 3px rgba(115, 103, 240, 0.15);
        }

        .body-wrapper {
            padding: 25px;
            background-color: #dafbed;bodywrapper__inner
        }

        .search-icon-svg {
            position: absolute;
            left: 10px;
            top: 50%;
            transform: translateY(-50%);
            width: 16px;
            height: 16px;
            color: #9ca3af;
            pointer-events: none;
        }

        .total-count-badge {
            background: #ede9fe;
            color: #7367f0;
            font-size: 13px;
            font-weight: 600;
            padding: 6px 14px;
            border-radius: 20px;
            white-space: nowrap;
        }

        .error-panel {
            background: #f8d7da;
            border: 1px solid #f5c2c7;
            color: #842029;
            padding: 12px 20px;
            border-radius: 6px;
            margin-bottom: 20px;
            opacity: 0;
            transition: opacity 0.3s ease-out;
        }

        .error-panel.show { opacity: 1; }

        .no-data {
            text-align: center;
            padding: 60px;
            color: #9ca3af;
            font-size: 14px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="active-user-page">

        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Active Users</h1>
            <div class="search-wrapper">
                <div class="search-box">
                    <svg class="search-icon-svg" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
                    </svg>
                    <input type="text" id="searchInput" class="search-input"
                           placeholder="Search users..." oninput="filterTable()" />
                </div>
                <%--<span class="total-count-badge" id="totalCount"></span>--%>
            </div>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvActiveUsers" runat="server" AutoGenerateColumns="false"
                    CssClass="user-table" GridLines="None"
                    DataKeyNames="Id">
                    <Columns>

                        <asp:BoundField DataField="UserId" HeaderText="User ID" />

                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <div class="user-name-cell">
                                    <span class='<%# GetAvatarClass(Eval("Id")) %>'>
                                        <%# GetInitials(Eval("Firstname"), Eval("Lastname")) %>
                                    </span>
                                    <span class="user-fullname">
                                        <%# Eval("Firstname") %> <%# Eval("Lastname") %>
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="UserName" HeaderText="Username" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />

                        <asp:TemplateField HeaderText="Mobile">
                            <ItemTemplate>
                                <span class="mobile-cell">
                                    <%# Eval("CountryCode") %> <%# Eval("Mobile") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Country">
                            <ItemTemplate>
                                <span class="location-cell"><%# Eval("CountryName") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="City">
                            <ItemTemplate>
                                <span class="location-cell"><%# Eval("City") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="State">
                            <ItemTemplate>
                                <span class="location-cell"><%# Eval("State") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Balance">
                            <ItemTemplate>
                                <span class='<%# GetBalanceClass(Eval("Balance")) %>'>
                                    <%# GetBalanceDisplay(Eval("Balance")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# GetStatusClass(Eval("Status")) %>'>
                                    <%# GetStatusBadge(Eval("Status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Joined">
                            <ItemTemplate>
                                <span class="joined-cell">
                                    <%# Eval("CreatedAt") != null ? ((DateTime)Eval("CreatedAt")).ToString("dd MMM yyyy") : "-" %>
                                </span>
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

    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        window.onload = function () {
            updateCount();
        };

        //function updateCount() {
        //    var rows = document.querySelectorAll('.user-table tbody tr');
        //    var visible = 0;
        //    rows.forEach(function (r) {
        //        if (r.style.display !== 'none') visible++;
        //    });
        //    var el = document.getElementById('totalCount');
        //    if (el) el.textContent = visible + ' user(s)';
        //}

        function filterTable() {
            var input = document.getElementById('searchInput').value.toLowerCase();
            document.querySelectorAll('.user-table tbody tr').forEach(function (row) {
                row.style.display = row.textContent.toLowerCase().includes(input) ? '' : 'none';
            });
            updateCount();
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>
