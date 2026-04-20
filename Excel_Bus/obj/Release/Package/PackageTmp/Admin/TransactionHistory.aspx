<%@ Page Title="Transaction Logs" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TransactionHistory.aspx.cs" Inherits="Excel_Bus.TransactionHistory" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Flatpickr CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    
    <style>
        .page-header {
            margin-bottom: 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
        }

        .stats-summary {
            display: flex;
            gap: 20px;
            margin-bottom: 20px;
        }

        .stat-card {
            background: #fff;
            padding: 15px 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            flex: 1;
        }

        .stat-label {
            font-size: 12px;
            color: #666;
            text-transform: uppercase;
            margin-bottom: 5px;
        }

        .stat-value {
            font-size: 20px;
            font-weight: 600;
            color: #333;
        }

        .stat-value.positive {
            color: #28a745;
        }

        .stat-value.negative {
            color: #dc3545;
        }

        .filter-card {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 20px;
        }

        .filter-row {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            align-items: end;
        }

        .filter-group {
            flex: 1;
            min-width: 200px;
        }

        .filter-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #555;
            font-size: 14px;
        }

        .filter-group input,
        .filter-group select {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }

        .filter-group input:focus,
        .filter-group select:focus {
            outline: none;
            border-color: #1e88e5;
        }

        .btn-filter {
            background: #1e88e5;
            color: white;
            border: none;
            padding: 10px 30px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 500;
            height: 45px;
            white-space: nowrap;
            transition: background 0.3s;
        }

        .btn-filter:hover {
            background: #1565c0;
        }

        .btn-reset {
            background: #6c757d;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 500;
            height: 45px;
            white-space: nowrap;
            transition: background 0.3s;
        }

        .btn-reset:hover {
            background: #5a6268;
        }

        .data-card {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .transaction-table {
            width: 100%;
            border-collapse: collapse;
        }

        .transaction-table thead {
            background: #f8f9fa;
            border-bottom: 2px solid #dee2e6;
        }

        .transaction-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #333;
            font-size: 13px;
            text-transform: uppercase;
        }

        .transaction-table th:first-child {
            width: 60px;
            text-align: center;
        }

        .transaction-table td {
            padding: 15px;
            border-bottom: 1px solid #f0f0f0;
            vertical-align: top;
        }

        .transaction-table td:first-child {
            text-align: center;
            font-weight: 600;
            color: #666;
            font-size: 14px;
        }

        .transaction-table tbody tr:hover {
            background: #f9f9f9;
        }

        .user-info {
            display: flex;
            flex-direction: column;
            gap: 3px;
        }

        .user-name {
            font-weight: 600;
            color: #333;
            font-size: 14px;
        }

        .user-id {
            font-size: 12px;
            color: #666;
        }

        .user-username {
            font-size: 12px;
            color: #1e88e5;
        }

        .user-username a {
            color: #1e88e5;
            text-decoration: none;
        }

        .user-username a:hover {
            text-decoration: underline;
        }

        .trx-info {
            display: flex;
            flex-direction: column;
            gap: 5px;
        }

        .trx-code {
            font-family: 'Courier New', monospace;
            font-weight: 600;
            color: #333;
            font-size: 13px;
            word-break: break-all;
        }

        .trx-type {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 3px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
        }

        .trx-type.payment {
            background: #e3f2fd;
            color: #1976d2;
        }

        .trx-type.refund {
            background: #fff3e0;
            color: #f57c00;
        }

        .trx-type.cancellation {
            background: #ffebee;
            color: #c62828;
        }

        .trx-type.postpone {
            background: #f3e5f5;
            color: #7b1fa2;
        }

        .trx-type.booking {
            background: #e8f5e9;
            color: #388e3c;
        }

        .date-info {
            display: flex;
            flex-direction: column;
            gap: 3px;
        }

        .date-time {
            color: #333;
            font-size: 13px;
            font-weight: 500;
        }

        .relative-time {
            font-size: 12px;
            color: #666;
        }

        .amount-info {
            display: flex;
            flex-direction: column;
            gap: 3px;
        }

        .amount-value {
            font-size: 15px;
            font-weight: 700;
        }

        .amount-charge {
            font-size: 12px;
            color: #666;
        }

        .text-success {
            color: #28a745 !important;
        }

        .text-danger {
            color: #dc3545 !important;
        }

        .balance-value {
            font-weight: 600;
            color: #333;
            font-size: 14px;
        }

        .details-info {
            max-width: 250px;
        }

        .details-primary {
            font-weight: 500;
            color: #333;
            margin-bottom: 3px;
            font-size: 13px;
        }

        .details-secondary {
            font-size: 12px;
            color: #666;
        }

        .pnr-link {
            color: #1e88e5;
            text-decoration: none;
            font-size: 12px;
        }

        .pnr-link:hover {
            text-decoration: underline;
        }

        .error-panel {
            background: #fee;
            border: 1px solid #fcc;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 20px;
            color: #c33;
        }

        .no-data-panel {
            text-align: center;
            padding: 60px 20px;
            color: #999;
        }

        .no-data-icon {
            font-size: 48px;
            margin-bottom: 15px;
            opacity: 0.5;
        }

        .no-data-text {
            font-size: 16px;
            margin-bottom: 5px;
        }

        .no-data-subtext {
            font-size: 13px;
        }

        /* Pagination Styles */
        .pagination-container {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 20px;
            margin-top: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .pagination-info {
            font-size: 14px;
            color: #666;
        }

        .pagination-controls {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .pagination-right {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .items-per-page-label {
            font-size: 14px;
            color: #666;
        }

        .page-size-dropdown {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            cursor: pointer;
            background: white;
        }

        .page-size-dropdown:focus {
            outline: none;
            border-color: #4CAF50;
        }

        .pagination-button {
            background: #fff;
            border: 1px solid #ddd;
            color: #4CAF50;
            padding: 8px 16px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.3s;
            min-width: 80px;
        }

        .pagination-button:hover:not(:disabled) {
            background: #4CAF50;
            color: white;
            border-color: #4CAF50;
        }

        .pagination-button:disabled {
            opacity: 0.4;
            cursor: not-allowed;
            color: #999;
        }

        .page-info-center {
            padding: 8px 20px;
            background: #f5f5f5;
            border-radius: 5px;
            font-size: 14px;
            color: #333;
            font-weight: 500;
        }

        .page-text {
            white-space: nowrap;
        }

        /* Status badges */
        .status-badge {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 3px;
            font-size: 11px;
            font-weight: 600;
        }

        .status-success {
            background: #d4edda;
            color: #155724;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
        }

        .status-failed {
            background: #f8d7da;
            color: #721c24;
        }

        @media (max-width: 768px) {
            .filter-group {
                min-width: 100%;
            }

            .stats-summary {
                flex-direction: column;
            }

            .transaction-table {
                font-size: 12px;
            }

            .transaction-table th,
            .transaction-table td {
                padding: 10px 8px;
            }

            .details-info {
                max-width: 150px;
            }

            .pagination-container {
                flex-direction: column;
                text-align: center;
            }

            .pagination-controls {
                justify-content: center;
            }

            .page-numbers {
                flex-wrap: wrap;
                justify-content: center;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1 class="page-title">Transaction Logs</h1>
    </div>

    <!-- Stats Summary -->
    <asp:Panel ID="pnlStats" runat="server" CssClass="stats-summary" Visible="false">
        <div class="stat-card">
            <div class="stat-label">Total Transactions</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalCount" runat="server" Text="0"></asp:Label>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Total Credits</div>
            <div class="stat-value positive">
                <asp:Label ID="lblTotalCredits" runat="server" Text="0.00 CDF"></asp:Label>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Total Debits</div>
            <div class="stat-value negative">
                <asp:Label ID="lblTotalDebits" runat="server" Text="0.00 CDF"></asp:Label>
            </div>
        </div>
    </asp:Panel>

    <!-- Error Panel -->
    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="error-panel">
        <asp:Label ID="lblError" runat="server"></asp:Label>
    </asp:Panel>

    <!-- Filter Card -->
    <div class="filter-card">
        <div class="filter-row">
            <div class="filter-group">
                <label>TRX / Username / User ID</label>
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by TRX, Username or User ID"></asp:TextBox>
            </div>
            
            <div class="filter-group">
                <label>Transaction Type</label>
                <asp:DropDownList ID="ddlTrxType" runat="server">
                    <asp:ListItem Value="" Text="All Types"></asp:ListItem>
                    <asp:ListItem Value="Payment" Text="Payment"></asp:ListItem>
                    <asp:ListItem Value="Refund" Text="Refund"></asp:ListItem>
                    <asp:ListItem Value="Full cancellation" Text="Cancellation"></asp:ListItem>
                    <asp:ListItem Value="Postpone" Text="Postpone"></asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label>Date Range</label>
                <asp:TextBox ID="txtDateRange" runat="server" placeholder="Select Date Range"></asp:TextBox>
            </div>
            
            <div class="filter-group" style="flex: 0 0 auto;">
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-filter" OnClick="btnFilter_Click" />
            </div>
            
            <div class="filter-group" style="flex: 0 0 auto;">
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-reset" OnClick="btnReset_Click" />
            </div>
        </div>
    </div>

    <!-- Data Card -->
    <div class="data-card">
        <div class="table-responsive">
            <asp:GridView ID="gvTransactions" runat="server" 
                AutoGenerateColumns="False" 
                CssClass="transaction-table"
                OnRowDataBound="gvTransactions_RowDataBound"
                GridLines="None"
                ShowHeader="true">
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# GetSerialNumber(Container.DataItemIndex) %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="User">
                        <ItemTemplate>
                            <div class="user-info">
                                <span class="user-name">
                                    <%# GetDisplayUsername(Eval("Username"), Eval("UserId")) %>
                                </span>
                                <span class="user-id">ID: <%# Eval("UserId") %></span>
                                <span class="user-username">
                                    <a href='javascript:void(0);' onclick='filterByUser("<%# GetSearchUsername(Eval("Username"), Eval("UserId")) %>")'>
                                        View all transactions
                                    </a>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Transaction">
                        <ItemTemplate>
                            <div class="trx-info">
                                <div class="trx-code"><%# Eval("Trx") %></div>
                                <%# GetTrxTypeBadge(Eval("TrxType"), Eval("Details")) %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Date & Time">
                        <ItemTemplate>
                            <div class="date-info">
                                <span class="date-time"><%# FormatDateTime(Eval("CreatedAt")) %></span>
                                <span class="relative-time"><%# GetRelativeTime(Eval("CreatedAt")) %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <div class="amount-info">
                                <%# FormatAmount(Eval("Amount"), Eval("TrxType")) %>
                                <%# Eval("Charge") != null && Convert.ToDecimal(Eval("Charge")) > 0 
                                    ? "<div class='amount-charge'>Charge: " + Convert.ToDecimal(Eval("Charge")).ToString("N2") + " CDF</div>" 
                                    : "" %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Balance">
                        <ItemTemplate>
                            <span class="balance-value"><%# FormatBalance(Eval("PostBalance")) %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Details">
                        <ItemTemplate>
                            <div class="details-info">
                                <div class="details-primary"><%# Eval("Details") %></div>
                                <%# !string.IsNullOrEmpty(Eval("Remark")?.ToString()) 
                                    ? "<div class='details-secondary'>" + Eval("Remark") + "</div>" 
                                    : "" %>
                                <%# !string.IsNullOrEmpty(Eval("PnrNumber")?.ToString()) 
                                    ? "<a href='BookingDetails.aspx?pnr=" + Eval("PnrNumber") + "' class='pnr-link'>PNR: " + Eval("PnrNumber") + "</a>" 
                                    : "" %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="no-data-panel">
                        <div class="no-data-icon">📋</div>
                        <div class="no-data-text">No transactions found</div>
                        <div class="no-data-subtext">Try adjusting your filters</div>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>

            <!-- No Data Panel -->
            <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="no-data-panel">
                <div class="no-data-icon">📋</div>
                <p class="no-data-text">No transactions found</p>
                <p class="no-data-subtext">Try adjusting your filters</p>
            </asp:Panel>
        </div>
    </div>

    <!-- Pagination -->
    <asp:Panel ID="pnlPagination" runat="server" CssClass="pagination-container" Visible="false">
        <div class="pagination-info">
            <asp:Label ID="lblPageInfo" runat="server" Text="Showing 1 to 10 of 100 tickets"></asp:Label>
        </div>
        
        <div class="pagination-controls">
            <!-- First button -->
            <asp:Button ID="btnFirstPage" runat="server" Text="First" CssClass="pagination-button" 
                OnClick="btnFirstPage_Click" />
            
            <!-- Previous button -->
            <asp:Button ID="btnPrevPage" runat="server" Text="Previous" CssClass="pagination-button" 
                OnClick="btnPrevPage_Click" />
            
            <!-- Page info -->
            <div class="page-info-center">
                <span class="page-text">Page 
                    <asp:Label ID="lblCurrentPage" runat="server" Text="1"></asp:Label> 
                    of <asp:Label ID="lblTotalPages" runat="server" Text="1"></asp:Label>
                </span>
            </div>
            
            <!-- Next button -->
            <asp:Button ID="btnNextPage" runat="server" Text="Next" CssClass="pagination-button" 
                OnClick="btnNextPage_Click" />
            
            <!-- Last button -->
            <asp:Button ID="btnLastPage" runat="server" Text="Last" CssClass="pagination-button" 
                OnClick="btnLastPage_Click" />
        </div>

        <div class="pagination-right">
            <span class="items-per-page-label">Items per page:</span>
            <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="page-size-dropdown" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                <asp:ListItem Value="10" Selected="True">10</asp:ListItem>
                <asp:ListItem Value="20">20</asp:ListItem>
                <asp:ListItem Value="50">50</asp:ListItem>
                <asp:ListItem Value="100">100</asp:ListItem>
            </asp:DropDownList>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <!-- Flatpickr JS -->
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    
    <script>
        let dateRangePicker = null;

        // Function to filter by user
        function filterByUser(username) {
            document.getElementById('<%= txtSearch.ClientID %>').value = username;
            document.getElementById('<%= btnFilter.ClientID %>').click();
        }

        // Initialize date range picker when page loads
        window.addEventListener('load', function () {
            const dateInput = document.getElementById('<%= txtDateRange.ClientID %>');

            if (dateInput) {
                dateRangePicker = flatpickr(dateInput, {
                    mode: "range",
                    dateFormat: "Y-m-d",
                    maxDate: "today",
                    onChange: function (selectedDates, dateStr, instance) {
                        console.log("Date selected:", dateStr);
                    }
                });
            }

            // Auto-hide error messages after 5 seconds
            setTimeout(function () {
                const errorPanel = document.getElementById('<%= pnlError.ClientID %>');
                if (errorPanel) errorPanel.style.display = 'none';
            }, 5000);
        });
    </script>
</asp:Content>
