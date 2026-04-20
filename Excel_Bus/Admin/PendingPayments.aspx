<%@ Page Title="Pending Deposits" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="PendingPayments.aspx.cs" Inherits="Excel_Bus.PendingPayments" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Add Flatpickr CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    
    <style>
        .pending-payments-container {
            padding: 20px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            flex-wrap: wrap;
            gap: 15px;
            margin-top: 50px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            margin: 0;
        }

        .search-controls {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .search-input-group {
            display: flex;
            align-items: center;
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 4px;
            overflow: hidden;
        }

        .search-input {
            border: none;
            padding: 10px 15px;
            outline: none;
            min-width: 200px;
        }

        .search-select {
            border: none;
            padding: 10px 15px;
            outline: none;
            min-width: 150px;
            background: #fff;
        }

        .btn-search {
            background: #ffc107;
            color: #fff;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-search:hover {
            background: #e0a800;
        }

        .payments-card {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .payments-table {
            width: 100%;
            border-collapse: collapse;
        }

        .payments-table thead {
            background: #fff3cd;
        }

        .payments-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #856404;
            border-bottom: 2px solid #ffeaa7;
        }

        .payments-table td {
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            vertical-align: middle;
        }

        .payments-table tbody tr:hover {
            background: #fffbea;
        }

        .gateway-info {
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }

        .gateway-link {
            color: #ffc107;
            text-decoration: none;
        }

        .gateway-link:hover {
            text-decoration: underline;
        }

        .trx-code {
            font-size: 12px;
            color: #666;
        }

        .date-time {
            font-weight: 500;
            color: #333;
        }

        .relative-time {
            font-size: 12px;
            color: #666;
        }

        .user-name {
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }

        .username-link {
            font-size: 12px;
            color: #ffc107;
            text-decoration: none;
        }

        .username-link:hover {
            text-decoration: underline;
        }

        .amount-main {
            font-weight: 500;
            color: #333;
        }

        .charge-text {
            color: #dc3545;
            font-size: 12px;
        }

        .total-amount {
            font-weight: 600;
            color: #333;
            margin-top: 5px;
        }

        .conversion-rate {
            font-size: 14px;
            color: #666;
            margin-bottom: 5px;
        }

        .converted-amount {
            font-weight: 600;
            color: #333;
        }

        .status-badge {
            display: inline-block;
            padding: 5px 12px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
        }

        .status-booked,
        .status-success {
            background: #d4edda;
            color: #155724;
        }

        .status-rejected {
            background: #f8d7da;
            color: #721c24;
        }

        .status-cancelled {
            background: #d6d8db;
            color: #383d41;
        }

        .status-postponed {
            background: #d1ecf1;
            color: #0c5460;
        }

        .status-unknown {
            background: #e2e3e5;
            color: #6c757d;
        }

        .btn-details {
            background: #fff;
            color: #ffc107;
            border: 1px solid #ffc107;
            padding: 6px 15px;
            border-radius: 4px;
            text-decoration: none;
            font-size: 14px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            transition: all 0.3s;
            cursor: pointer;
        }

        .btn-details:hover {
            background: #ffc107;
            color: #fff;
        }

        .no-data-panel {
            text-align: center;
            padding: 60px 20px;
        }

        .no-data-panel img {
            max-width: 200px;
            margin-bottom: 20px;
            opacity: 0.6;
        }

        .no-data-text {
            color: #666;
            font-size: 16px;
        }

        .error-panel {
            background: #f8d7da;
            color: #721c24;
            padding: 15px;
            border-radius: 4px;
            margin-bottom: 20px;
            border: 1px solid #f5c6cb;
        }

        .success-panel {
            background: #d4edda;
            color: #155724;
            padding: 15px;
            border-radius: 4px;
            margin-bottom: 20px;
            border: 1px solid #c3e6cb;
        }

        /* Pagination Styles */
        .pagination-wrapper {
            margin-top: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 4px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .pagination-info {
            color: #666;
            font-size: 14px;
        }

        .pagination-controls {
            display: flex;
            gap: 5px;
            align-items: center;
        }

        .page-btn {
            padding: 8px 15px;
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 4px;
            cursor: pointer;
            transition: all 0.3s;
            text-decoration: none;
            color: #ffc107;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

            .page-btn:hover:not(:disabled) {
                background: #ffc107;
                color: #fff;
                border-color: #ffc107;
            }

            .page-btn:disabled {
                opacity: 0.5;
                cursor: not-allowed;
            }

        .page-size-selector {
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .page-size-selector label {
                font-size: 14px;
                color: #666;
            }

        .page-size-dropdown {
            padding: 6px 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
            cursor: pointer;
        }

        /* Modal Styles */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 1000;
            justify-content: center;
            align-items: center;
        }

        .modal-overlay.active {
            display: flex;
        }

        .modal-content {
            background: #fff;
            border-radius: 8px;
            max-width: 1200px;
            width: 90%;
            max-height: 90vh;
            overflow-y: auto;
            position: relative;
            animation: modalSlideIn 0.3s ease;
        }

        @keyframes modalSlideIn {
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
            font-size: 20px;
            font-weight: 600;
            color: #333;
            margin: 0;
        }

        .modal-close {
            background: none;
            border: none;
            font-size: 28px;
            color: #666;
            cursor: pointer;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 4px;
            transition: background 0.3s;
        }

        .modal-close:hover {
            background: #f8f9fa;
            color: #333;
        }

        .modal-body {
            padding: 20px;
        }

        .detail-section {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
        }

        .detail-card {
            background: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
        }

        .detail-card h3 {
            font-size: 16px;
            font-weight: 600;
            color: #333;
            margin: 0 0 15px 0;
        }

        .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #dee2e6;
        }

        .detail-row:last-child {
            border-bottom: none;
        }

        .detail-label {
            color: #666;
            font-weight: 500;
        }

        .detail-value {
            color: #333;
            font-weight: 600;
            text-align: right;
        }

        .action-buttons {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin-top: 20px;
            flex-wrap: wrap;
        }

        .btn-approve {
            background: #28a745;
            color: #fff;
            border: none;
            padding: 10px 30px;
            border-radius: 4px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-approve:hover {
            background: #218838;
        }

        .btn-reject {
            background: #dc3545;
            color: #fff;
            border: none;
            padding: 10px 30px;
            border-radius: 4px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-reject:hover {
            background: #c82333;
        }

        .btn-cancel {
            background: #6c757d;
            color: #fff;
            border: none;
            padding: 10px 30px;
            border-radius: 4px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-cancel:hover {
            background: #5a6268;
        }

        .btn-postpone {
            background: #17a2b8;
            color: #fff;
            border: none;
            padding: 10px 30px;
            border-radius: 4px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-postpone:hover {
            background: #138496;
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .search-controls {
                width: 100%;
            }

            .search-input-group {
                width: 100%;
            }

            .search-input,
            .search-select {
                width: 100%;
            }

            .detail-section {
                grid-template-columns: 1fr;
            }

            .modal-content {
                width: 95%;
                margin: 10px;
            }

            .pagination-wrapper {
                flex-direction: column;
                text-align: center;
            }

            .pagination-controls {
                width: 100%;
                justify-content: center;
            }
        }

        @media print {
            .search-controls,
            .pagination-wrapper,
            .btn-details,
            .action-buttons {
                display: none !important;
            }

            .payments-table {
                font-size: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pending-payments-container">
        <!-- Page Header -->
        <div class="page-header">
            <h6 class="page-title">Pending Deposits</h6>
            <div class="search-controls">
                <div class="search-input-group">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Username / TRX"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <div class="search-input-group">
                    <asp:TextBox ID="txtDateRange" runat="server" CssClass="search-input" placeholder="Select Date Range"></asp:TextBox>
                    <asp:Button ID="btnDateSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <!-- Success Panel -->
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-panel" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Error Panel -->
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Payments Table -->
        <div class="payments-card">
            <div class="table-responsive">
                <table class="payments-table">
                    <thead>
                        <tr>
                            <th>S.No</th>
                            <th>Gateway | Transaction</th>
                            <th>Initiated</th>
                            <th>User</th>
                            <th>Amount</th>
                            <th>Conversion</th>
                            <th>Status</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptPayments" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Container.ItemIndex + 1 + ((CurrentPageNumber - 1) * PageSizeValue) %></strong></td>
                                    <td>
                                        <div class="gateway-info">
                                            <a href="#" class="gateway-link"><%# Eval("TrxType") %></a>
                                        </div>
                                        <div class="trx-code"><%# Eval("Trx") %></div>
                                    </td>
                                    <td>
                                        <div class="date-time"><%# FormatDateTime(Eval("CreatedAt")) %></div>
                                        <div class="relative-time"><%# GetRelativeTime(Eval("CreatedAt")) %></div>
                                    </td>
                                    <td>
                                        <div class="user-name"><%# Eval("FullName") %></div>
                                        <div>
                                            <a href="#" class="username-link">@<%# Eval("Username") %></a>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="amount-main">
                                            <%# FormatAmount(Eval("Amount")) %> CDF + 
                                            <span class="charge-text"><%# FormatAmount(Eval("Charge")) %> CDF</span>
                                        </div>
                                        <div class="total-amount">
                                            <%# FormatAmount(Eval("TotalAmount")) %> CDF
                                        </div>
                                    </td>
                                    <td>
                                        <div class="conversion-rate">1.00 CDF = 1.00 INR</div>
                                        <div class="converted-amount">
                                            <%# FormatAmount(Eval("TotalAmount")) %> INR
                                        </div>
                                    </td>
                                    <td>
                                        <span class="status-badge <%# GetStatusBadgeClass(Eval("Status")) %>">
                                            <%# Eval("Status") %>
                                        </span>
                                    </td>
                                    <td>
                                        <button type="button" class="btn-details" 
                                            data-id="<%# Eval("Id") %>"
                                            data-trx="<%# Eval("Trx") %>"
                                            data-username="<%# Eval("Username") %>"
                                            data-fullname="<%# Eval("FullName") %>"
                                            data-date="<%# FormatDateTime(Eval("CreatedAt")) %>"
                                            data-amount="<%# FormatAmount(Eval("Amount")) %>"
                                            data-charge="<%# FormatAmount(Eval("Charge")) %>"
                                            data-total="<%# FormatAmount(Eval("TotalAmount")) %>"
                                            data-trxtype="<%# Eval("TrxType") %>"
                                            data-details="<%# Eval("Details") %>"
                                            data-status="<%# Eval("Status") %>"
                                            data-pnr="<%# Eval("PnrNumber") %>"
                                            onclick="showDetails(this)">
                                            <i class="las la-desktop"></i> Details
                                        </button>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>

            <!-- ✅ Pagination Controls -->
            <div class="pagination-wrapper">
                <div class="pagination-info">
                    <span>Showing 
                        <asp:Literal ID="litPageStart" runat="server">0</asp:Literal> to 
                        <asp:Literal ID="litPageEnd" runat="server">0</asp:Literal> of 
                        <asp:Literal ID="litTotalRecords" runat="server">0</asp:Literal> payments
                    </span>
                </div>
                <div class="pagination-controls">
                    <asp:LinkButton ID="btnFirst" runat="server" OnClick="btnFirst_Click" CssClass="page-btn" CausesValidation="false">
                        <i class="las la-angle-double-left"></i> First
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnPrevious" runat="server" OnClick="btnPrevious_Click" CssClass="page-btn" CausesValidation="false">
                        <i class="las la-angle-left"></i> Previous
                    </asp:LinkButton>
                    <span style="margin: 0 15px;">
                        Page <asp:Literal ID="litCurrentPage" runat="server">1</asp:Literal> of 
                        <asp:Literal ID="litTotalPages" runat="server">1</asp:Literal>
                    </span>
                    <asp:LinkButton ID="btnNext" runat="server" OnClick="btnNext_Click" CssClass="page-btn" CausesValidation="false">
                        Next <i class="las la-angle-right"></i>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnLast" runat="server" OnClick="btnLast_Click" CssClass="page-btn" CausesValidation="false">
                        Last <i class="las la-angle-double-right"></i>
                    </asp:LinkButton>
                </div>
                <div class="page-size-selector">
                    <label>Items per page:</label>
                    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="page-size-dropdown">
                        <asp:ListItem Value="10">10</asp:ListItem>
                        <asp:ListItem Value="25" Selected="True">25</asp:ListItem>
                        <asp:ListItem Value="50">50</asp:ListItem>
                        <asp:ListItem Value="100">100</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- No Data Panel -->
        <asp:Panel ID="pnlNoData" runat="server" CssClass="no-data-panel" Visible="false">
            <img src="assets/images/empty_list.png" alt="No data" />
            <p class="no-data-text">No transactions found</p>
        </asp:Panel>
    </div>

    <!-- Modal for Payment Details -->
    <div id="detailsModal" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title"><span id="modalUsername"></span> requested <span id="modalAmount"></span> CDF</h2>
                <button class="modal-close" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <div class="detail-section">
                    <div class="detail-card">
                        <h3>Transaction Details - <span id="detailTrxType"></span></h3>
                        <div class="detail-row">
                            <span class="detail-label">Date</span>
                            <span class="detail-value" id="detailDate"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Transaction Number</span>
                            <span class="detail-value" id="detailTrx"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Username</span>
                            <span class="detail-value" id="detailUsername"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Method</span>
                            <span class="detail-value" id="detailMethod"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">PNR Number</span>
                            <span class="detail-value" id="detailPnr"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Details</span>
                            <span class="detail-value" id="detailDetails"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Amount</span>
                            <span class="detail-value" id="detailAmount"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Charge</span>
                            <span class="detail-value" id="detailCharge"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">After Charge</span>
                            <span class="detail-value" id="detailAfterCharge"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Rate</span>
                            <span class="detail-value">1 CDF = 1.00 INR</span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">After Rate Conversion</span>
                            <span class="detail-value" id="detailConversion"></span>
                        </div>
                        <div class="detail-row">
                            <span class="detail-label">Status</span>
                            <span class="detail-value">
                                <span class="status-badge" id="detailStatusBadge"></span>
                            </span>
                        </div>
                    </div>
                    
                    <div class="detail-card">
                        <h3>User Deposit Information</h3>
                        <div style="text-align: center; padding: 40px; color: #666;">
                            <p>Additional deposit information can be displayed here</p>
                        </div>
                    </div>
                </div>

                <div class="action-buttons" id="actionButtons">
                    <button class="btn-approve" onclick="updateStatus('Success')">
                        ✓ Book / Approve
                    </button>
                    <button class="btn-reject" onclick="updateStatus('Rejected')">
                        ✕ Reject
                    </button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <!-- Add Flatpickr JS -->
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    
    <script>
        let currentPaymentId = null;
        let dateRangePicker = null;

        // Initialize date range picker when page loads
        window.addEventListener('load', function () {
            const dateInput = document.getElementById('<%= txtDateRange.ClientID %>');
            
            if (dateInput) {
                dateRangePicker = flatpickr(dateInput, {
                    mode: "range",
                    dateFormat: "Y-m-d",
                    maxDate: "today",
                    onChange: function(selectedDates, dateStr, instance) {
                        console.log("Date selected:", dateStr);
                    }
                });
            }

            // Auto-hide success/error messages after 5 seconds
            setTimeout(function () {
                const successPanel = document.getElementById('<%= pnlSuccess.ClientID %>');
                const errorPanel = document.getElementById('<%= pnlError.ClientID %>');
                if (successPanel) successPanel.style.display = 'none';
                if (errorPanel) errorPanel.style.display = 'none';
            }, 5000);
        });

        function showDetails(button) {
            // Get data from button attributes
            currentPaymentId = button.getAttribute('data-id');
            const trx = button.getAttribute('data-trx');
            const username = button.getAttribute('data-username');
            const fullname = button.getAttribute('data-fullname');
            const date = button.getAttribute('data-date');
            const amount = button.getAttribute('data-amount');
            const charge = button.getAttribute('data-charge');
            const total = button.getAttribute('data-total');
            const trxType = button.getAttribute('data-trxtype');
            const details = button.getAttribute('data-details');
            const status = button.getAttribute('data-status');
            const pnr = button.getAttribute('data-pnr');

            // Update modal content
            document.getElementById('modalUsername').textContent = fullname || username;
            document.getElementById('modalAmount').textContent = amount;
            document.getElementById('detailDate').textContent = date;
            document.getElementById('detailTrx').textContent = trx;
            document.getElementById('detailUsername').textContent = '@' + username;
            document.getElementById('detailTrxType').textContent = trxType;
            document.getElementById('detailMethod').textContent = trxType;
            document.getElementById('detailPnr').textContent = pnr || 'N/A';
            document.getElementById('detailDetails').textContent = details || 'N/A';
            document.getElementById('detailAmount').textContent = amount + ' CDF';
            document.getElementById('detailCharge').textContent = charge + ' CDF';
            document.getElementById('detailAfterCharge').textContent = total + ' CDF';
            document.getElementById('detailConversion').textContent = total + ' INR';

            // Update status badge
            const statusBadge = document.getElementById('detailStatusBadge');
            statusBadge.textContent = status;
            statusBadge.className = 'status-badge ' + getStatusClass(status);

            // Show modal
            document.getElementById('detailsModal').classList.add('active');
            document.body.style.overflow = 'hidden';
        }

        function getStatusClass(status) {
            if (!status) return 'status-unknown';

            const statusLower = status.toLowerCase();
            switch (statusLower) {
                case 'pending':
                    return 'status-pending';
                case 'success':
                case 'booked':
                    return 'status-booked';
                case 'rejected':
                    return 'status-rejected';
                case 'cancelled':
                    return 'status-cancelled';
                case 'postponed':
                    return 'status-postponed';
                default:
                    return 'status-unknown';
            }
        }

        function closeModal() {
            document.getElementById('detailsModal').classList.remove('active');
            document.body.style.overflow = 'auto';
            currentPaymentId = null;
        }

        function updateStatus(newStatus) {
            if (!currentPaymentId) return;

            const statusText = newStatus === 'Success' ? 'Book/Approve' : newStatus;
            if (confirm(`Are you sure you want to ${statusText} this transaction?`)) {
                // Use ASP.NET postback mechanism
                __doPostBack('UpdateStatus', currentPaymentId + '|' + newStatus);
                closeModal();
            }
        }

        // Close modal when clicking outside
        document.addEventListener('click', function (event) {
            const modal = document.getElementById('detailsModal');
            if (event.target === modal) {
                closeModal();
            }
        });

        // Close modal with Escape key
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                closeModal();
            }
        });
    </script>
</asp:Content>
