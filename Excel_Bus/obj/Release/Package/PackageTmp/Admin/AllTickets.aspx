<%@ Page Title="All Tickets" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AllTickets.aspx.cs" Inherits="Excel_Bus.AllTickets" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .all-tickets-header {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

            .all-tickets-header h3 {
                margin: 0 0 10px 0;
                color: #333;
                font-size: 24px;
            }

        .statistics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 15px;
            border-radius: 8px;
            color: #fff;
            text-align: center;
        }

            .stat-card.booked {
                background: linear-gradient(135deg, #28c76f 0%, #1fa358 100%);
            }

            .stat-card.pending {
                background: linear-gradient(135deg, #ff9f43 0%, #f77502 100%);
            }

            .stat-card.rejected {
                background: linear-gradient(135deg, #eb2222 0%, #c21e1e 100%);
            }

            .stat-card.cancelled {
                background: linear-gradient(135deg, #1e9ff2 0%, #1479c2 100%);
            }

            .stat-card.postponed {
                background: linear-gradient(135deg, #6c757d 0%, #545b62 100%);
            }

            .stat-card .stat-number {
                font-size: 28px;
                font-weight: bold;
                margin: 5px 0;
            }

            .stat-card .stat-label {
                font-size: 12px;
                opacity: 0.9;
            }

        .filter-section {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .filter-row {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }

            .filter-row select,
            .filter-row input {
                flex: 1;
                min-width: 200px;
                padding: 10px 15px;
                border: 1px solid #ddd;
                border-radius: 4px;
                font-size: 14px;
            }

            .filter-row button {
                padding: 10px 30px;
                background: #667eea;
                color: #fff;
                border: none;
                border-radius: 4px;
                cursor: pointer;
                font-size: 14px;
                transition: background 0.3s;
            }

                .filter-row button:hover {
                    background: #5568d3;
                }

        .tickets-table-wrapper {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow-x: auto;
        }

        .tickets-table {
            width: 100%;
            border-collapse: collapse;
            min-width: 1600px;
        }

            .tickets-table thead {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: #fff;
            }

            .tickets-table th {
                padding: 12px;
                text-align: left;
                font-weight: 600;
                font-size: 13px;
                white-space: nowrap;
            }

            .tickets-table td {
                padding: 12px;
                border-bottom: 1px solid #f0f0f0;
                font-size: 13px;
            }

            .tickets-table tbody tr:hover {
                background: #f8f9fa;
            }

        .badge {
            padding: 4px 10px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .badge--success {
            background: #d4edda;
            color: #155724;
        }

        .badge--warning {
            background: #fff3cd;
            color: #856404;
        }

        .badge--danger {
            background: #f8d7da;
            color: #721c24;
        }

        .badge--primary {
            background: #d1ecf1;
            color: #0c5460;
        }

        .badge--dark {
            background: #d6d8db;
            color: #1b1e21;
        }

        .badge--secondary {
            background: #e2e3e5;
            color: #383d41;
        }

        .date-badge {
            background: #e3f2fd;
            color: #1976d2;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
        }

        .no-data-container {
            text-align: center;
            padding: 60px 20px;
        }

            .no-data-container i {
                font-size: 64px;
                color: #ccc;
                margin-bottom: 20px;
            }

            .no-data-container p {
                color: #666;
                font-size: 16px;
                margin: 0;
            }

        .user-info {
            font-size: 12px;
        }

            .user-info .user-name {
                font-weight: 600;
                color: #333;
                font-size: 13px;
                display: block;
                margin-bottom: 3px;
            }

            .user-info .user-contact {
                color: #666;
                font-size: 11px;
            }

            .user-info a {
                color: #667eea;
                text-decoration: none;
                font-size: 11px;
            }

                .user-info a:hover {
                    text-decoration: underline;
                }

        .action-buttons-bar {
            background: #fff;
            padding: 15px 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .action-btn {
            padding: 8px 16px;
            color: #fff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

            .action-btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            }

        .btn-action-mini {
            padding: 4px 8px;
            background: #667eea;
            color: #fff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 11px;
            margin: 2px;
            transition: all 0.3s;
        }

            .btn-action-mini:hover {
                background: #5568d3;
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
            color: #667eea;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

            .page-btn:hover:not(:disabled) {
                background: #667eea;
                color: #fff;
                border-color: #667eea;
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

        @media (max-width: 768px) {
            .statistics-grid {
                grid-template-columns: repeat(2, 1fr);
            }

            .filter-row {
                flex-direction: column;
            }

                .filter-row select,
                .filter-row input,
                .filter-row button {
                    width: 100%;
                }

            .action-buttons-bar {
                flex-direction: column;
            }

            .action-btn {
                width: 100%;
                justify-content: center;
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
            .filter-section,
            .action-buttons-bar,
            .btn-action-mini,
            .pagination-wrapper {
                display: none;
            }

            .tickets-table {
                font-size: 10px;
            }

            .statistics-grid {
                page-break-after: avoid;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="all-tickets-page">
        <!-- Header with Statistics -->
        <div class="all-tickets-header">
            <h3>
                <i class="las la-ticket-alt"></i>All Tickets Overview
            </h3>
            <p style="margin: 0 0 15px 0; color: #666; font-size: 14px;">
                All Tickets
            </p>

            <!-- Statistics Cards -->
            <div class="statistics-grid">
                <div class="stat-card">
                    <div class="stat-label">TOTAL TICKETS</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalTickets" runat="server">0</asp:Literal></div>
                </div>
                <div class="stat-card booked">
                    <div class="stat-label">BOOKED</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalBooked" runat="server">0</asp:Literal></div>
                </div>
                <div class="stat-card pending">
                    <div class="stat-label">PENDING</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalPending" runat="server">0</asp:Literal></div>
                </div>
                <div class="stat-card rejected">
                    <div class="stat-label">REJECTED</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalRejected" runat="server">0</asp:Literal></div>
                </div>
                <div class="stat-card cancelled">
                    <div class="stat-label">CANCELLED</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalCancelled" runat="server">0</asp:Literal></div>
                </div>
                <div class="stat-card postponed">
                    <div class="stat-label">POSTPONED</div>
                    <div class="stat-number">
                        <asp:Literal ID="litTotalPostponed" runat="server">0</asp:Literal></div>
                </div>
            </div>
        </div>

        <!-- Filter Section -->
        <div class="filter-section">
            <div class="filter-row">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Value="all" Selected="True">All Statuses</asp:ListItem>
                    <asp:ListItem Value="1">Booked Only</asp:ListItem>
                    <asp:ListItem Value="2">Pending Only</asp:ListItem>
                    <asp:ListItem Value="5">Rejected Only</asp:ListItem>
                    <asp:ListItem Value="3">Cancelled Only</asp:ListItem>
                    <asp:ListItem Value="4">Postponed Only</asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearchPnr" runat="server"
                    placeholder="Search by PNR Number"
                    CssClass="form-control" />

                <asp:Button ID="btnSearch" runat="server"
                    Text="Search"
                    OnClick="btnSearch_Click"
                    CssClass="btn-search" />
                
                <asp:Button ID="btnReset" runat="server"
                    Text="Reset"
                    OnClick="btnReset_Click"
                    CssClass="btn-search"
                    style="background: #6c757d;" />
            </div>
            
            <!-- Date Filters -->
            <div class="filter-row" style="margin-top: 10px;">
                <div style="flex: 1; min-width: 200px;">
                    <label style="display: block; margin-bottom: 5px; color: #666; font-size: 13px; font-weight: 500;">
                        <i class="las la-calendar"></i> Journey Date:
                    </label>
                    <asp:TextBox ID="txtJourneyDate" runat="server" 
                        TextMode="Date" 
                        CssClass="form-control" />
                </div>
                <div style="flex: 1; min-width: 200px;">
                    <label style="display: block; margin-bottom: 5px; color: #666; font-size: 13px; font-weight: 500;">
                        <i class="las la-calendar-check"></i> Booking Date:
                    </label>
                    <asp:TextBox ID="txtBookingDate" runat="server" 
                        TextMode="Date" 
                        CssClass="form-control" />
                </div>
            </div>
        </div>

        <!-- Action Buttons Bar -->
        <div class="action-buttons-bar">
            <button type="button" onclick="exportToExcel()" class="action-btn" style="background: #28c76f;">
                <i class="las la-file-excel"></i>Export to Excel
            </button>
            <button type="button" onclick="exportToCSV()" class="action-btn" style="background: #00cfe8;">
                <i class="las la-file-csv"></i>Export CSV
            </button>
            <button type="button" onclick="refreshData()" class="action-btn" style="background: #667eea;">
                <i class="las la-sync"></i>Refresh
            </button>
        </div>

        <!-- Tickets Table -->
        <div class="tickets-table-wrapper">
            <asp:Repeater ID="rptAllTickets" runat="server">
                <HeaderTemplate>
                    <table class="tickets-table">
                        <thead>
                            <tr>
                                <th>S.No</th>
                                <th>User</th>
                                <th>PNR Number</th>
                                <th>Booking Date</th>
                                <th>Journey Date</th>
                                <th>Route</th>
                                <th>Fleet Type</th>
                                <th>Vehicle Name</th>
                                <th>Pickup Point</th>
                                <th>Dropping Point</th>
                                <th>Seats</th>
                                <th>Status</th>
                                <th>Postponed</th>
                                <th>Check In</th>
                                <th>Luggage Details</th>
                                <th>Ticket Count</th>
                                <th>Fare</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr data-ticket-id="<%# Eval("Id") %>">
                        <td><strong><%# Container.ItemIndex + 1 + ((CurrentPageNumber - 1) * PageSizeValue) %></strong></td>
                        <td>
                            <div style="line-height: 1.6;">
                                <strong style="color: #333; font-size: 13px;">
                                    <%# GetUserDisplayName(
                                        Eval("UserName").ToString(), 
                                        Eval("FirstName").ToString(), 
                                        Eval("LastName").ToString()) %>
                                </strong>
                                <br />
                                <a href="javascript:void(0)"
                                    onclick="viewUserDetails(<%# Eval("UserId") %>)"
                                    style="color: #667eea; font-size: 12px; text-decoration: none;">@<%# Eval("UserName") %>
                                </a>
                            </div>
                        </td>
                        <td><strong><%# Eval("PnrNumber") %></strong></td>
                        <td>
                            <span class="date-badge">
                                <i class="las la-calendar-check"></i>
                                <%# Eval("BookingDateFormatted") %>
                            </span>
                        </td>
                        <td>
                            <span class="date-badge">
                                <i class="las la-calendar"></i>
                                <%# Eval("DateOfJourneyFormatted") %>
                            </span>
                        </td>
                        <td>
                            <small style="color: #666;"><%# Eval("TripRoute") %></small>
                        </td>
                         <td><%# Eval("FleetTypeName") %></td>
                        <td><%# Eval("VehicleNickName") %></td>
                         <td><%# Eval("PickupName") %></td>
                        
                        <td><%# Eval("DropName") %></td>
                        <td><strong><%# Eval("SeatsDisplay") %></strong></td>
                        <td>
                            <%# GetStatusBadge(Eval("Status")) %>
                        </td>
                        <td>
                            <%# GetPostponedBadge(Convert.ToInt32(Eval("PostponeCount"))) %>
                        </td>
                        <td>
                            <%# GetYesNoBadge(Eval("Checkin").ToString()) %>
                        </td>
                        <td>
                            <div style="font-size: 12px;">
                                Bags: <%# Eval("BagsCount") %><br />
                                Weight: <%# Eval("BagsWeight") %><br />
                                Charge: <%# Convert.ToDecimal(Eval("BagsCharge")).ToString("N2") %> CDF
                            </div>
                        </td>
                        <td><%# Eval("TicketCount") %></td>
                        <td>
                            <strong style="color: #667eea;"><%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %> CDF</strong>
                        </td>
                        <td>
                            <button type="button" class="btn-action-mini"
                                data-pnr="<%# Eval("PnrNumber") %>"
                                data-id="<%# Eval("Id") %>"
                                onclick="viewTicketDetails(this)">
                                <i class="las la-eye"></i>View
                            </button>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <!-- No Data Message -->
            <div id="divNoData" runat="server" class="no-data-container" visible="false">
                <i class="las la-database"></i>
                <p>No tickets found.</p>
                <small style="color: #999;">All ticket bookings will appear here.</small>
            </div>

            <!-- ✅ Pagination Controls -->
            <div class="pagination-wrapper">
                <div class="pagination-info">
                    <span>Showing 
                        <asp:Literal ID="litPageStart" runat="server">0</asp:Literal> to 
                        <asp:Literal ID="litPageEnd" runat="server">0</asp:Literal> of 
                        <asp:Literal ID="litTotalRecords" runat="server">0</asp:Literal> tickets
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
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function viewUserDetails(userId) {
            console.log('View user details for ID:', userId);
            alert('User Details feature - User ID: ' + userId);
        }

        function viewTicketDetails(button) {
            var pnrNumber = button.getAttribute('data-pnr');
            var ticketId = button.getAttribute('data-id');
            console.log('View ticket details - PNR:', pnrNumber, 'ID:', ticketId);
            alert('Ticket Details:\nPNR: ' + pnrNumber + '\nID: ' + ticketId);
        }

        $(document).ready(function () {
            console.log('All Tickets page loaded');
            highlightStatusRows();
        });

        function highlightStatusRows() {
            $('.tickets-table tbody tr').each(function () {
                var statusBadge = $(this).find('td:nth-child(10)').text().trim();
                if (statusBadge.includes('Rejected')) {
                    $(this).css('border-left', '3px solid #eb2222');
                } else if (statusBadge.includes('Pending')) {
                    $(this).css('border-left', '3px solid #ff9f43');
                } else if (statusBadge.includes('Booked')) {
                    $(this).css('border-left', '3px solid #28c76f');
                } else if (statusBadge.includes('Cancelled')) {
                    $(this).css('border-left', '3px solid #1e9ff2');
                } else if (statusBadge.includes('Postponed')) {
                    $(this).css('border-left', '3px solid #6c757d');
                }
            });
        }

        function exportToExcel() {
            var table = document.querySelector('.tickets-table');
            if (!table) {
                notify('error', 'No data to export');
                return;
            }
            var html = table.outerHTML;
            var url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
            var link = document.createElement('a');
            link.href = url;
            link.download = 'all_tickets_' + new Date().toISOString().split('T')[0] + '.xls';
            link.click();
            notify('success', 'Export completed successfully');
        }

        function exportToCSV() {
            var csv = [];
            var rows = document.querySelectorAll('.tickets-table tr');
            for (var i = 0; i < rows.length; i++) {
                var row = [], cols = rows[i].querySelectorAll('td, th');
                for (var j = 0; j < cols.length - 1; j++) {
                    var data = cols[j].innerText.replace(/(\r\n|\n|\r)/gm, ' ').replace(/"/g, '""');
                    row.push('"' + data + '"');
                }
                csv.push(row.join(','));
            }
            var csvContent = csv.join('\n');
            var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            var link = document.createElement('a');
            var url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', 'all_tickets_' + new Date().toISOString().split('T')[0] + '.csv');
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            notify('success', 'CSV export completed successfully');
        }

        function refreshData() {
            if (confirm('Are you sure you want to refresh the data?')) {
                location.reload();
            }
        }
    </script>
</asp:Content>
