<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_BookedTicket.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_BookedTicket" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .booked-tickets-header {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

            .booked-tickets-header h3 {
                margin: 0 0 10px 0;
                color: #333;
                font-size: 24px;
            }

        .search-section {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .search-box {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }

            .search-box input[type="text"],
            .search-box input[type="date"] {
                flex: 1;
                min-width: 200px;
                padding: 10px 15px;
                border: 1px solid #ddd;
                border-radius: 4px;
                font-size: 14px;
            }

            .search-box button {
                padding: 10px 30px;
                background: #28c76f;
                color: #fff;
                border: none;
                border-radius: 4px;
                cursor: pointer;
                font-size: 14px;
                transition: background 0.3s;
            }

                .search-box button:hover {
                    background: #22a65d;
                }

            .search-box .btn-reset {
                background: #6c757d;
            }

                .search-box .btn-reset:hover {
                    background: #5a6268;
                }

        .filter-row {
            display: flex;
            gap: 10px;
            margin-top: 10px;
            flex-wrap: wrap;
        }

        .filter-group {
            flex: 1;
            min-width: 200px;
        }

            .filter-group label {
                display: block;
                margin-bottom: 5px;
                color: #666;
                font-size: 13px;
                font-weight: 500;
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
            min-width: 1800px;
        }

            .tickets-table thead {
                background: #28c76f;
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
            display: inline-block;
        }

        .badge--success {
            background: #28c76f;
            color: #fff;
        }

        .badge--warning {
            background: #ff9f43;
            color: #fff;
        }

        .badge--secondary {
            background: #6c757d;
            color: #fff;
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

        .status-confirmed {
            background: #d4edda;
            color: #155724;
            padding: 2px 8px;
            border-radius: 4px;
            font-weight: 500;
        }

        .date-badge {
            background: #e3f2fd;
            color: #1976d2;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
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
            color: #28c76f;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

            .page-btn:hover:not(:disabled) {
                background: #28c76f;
                color: #fff;
                border-color: #28c76f;
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
            .tickets-table-wrapper {
                padding: 10px;
            }

            .search-box {
                flex-direction: column;
            }

                .search-box input,
                .search-box button {
                    width: 100%;
                }

            .filter-group {
                width: 100%;
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
            .search-section,
            .pagination-wrapper,
            .booked-tickets-header button {
                display: none;
            }

            .tickets-table {
                font-size: 10px;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="booked-tickets-page">
        <!-- Header -->
        <div class="booked-tickets-header">
            <h3>
                <i class="las la-check-circle"></i>Booked Tickets (Confirmed)
                <span style="font-size: 18px; color: #28c76f; margin-left: 10px;">(Total:
                    <asp:Literal ID="litTotalBooked" runat="server">0</asp:Literal>)
                </span>
            </h3>
            <p style="margin: 0; color: #666; font-size: 14px;">
                Booked Tickets
            </p>
        </div>

        <!-- Search Section -->
        <div class="search-section">
            <div class="search-box">
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
                    CssClass="btn-search btn-reset" />
            </div>
            
            <!-- Date Filters -->
            <div class="filter-row">
                <div class="filter-group">
                    <label><i class="las la-calendar"></i> Journey Date:</label>
                    <asp:TextBox ID="txtJourneyDate" runat="server" 
                        TextMode="Date" 
                        CssClass="form-control" />
                </div>
                <div class="filter-group">
                    <label><i class="las la-calendar-check"></i> Booking Date:</label>
                    <asp:TextBox ID="txtBookingDate" runat="server" 
                        TextMode="Date" 
                        CssClass="form-control" />
                </div>
            </div>
        </div>

        <!-- Tickets Table -->
        <div class="tickets-table-wrapper">
            <asp:Repeater ID="rptBookedTickets" runat="server">
                <HeaderTemplate>
                    <table class="tickets-table">
                        <thead>
                            <tr>
                                <th>S.No</th>
                                <th>User</th>
                                <th>PNR Number</th>
                                <th>Booking Date</th>
                                <th>Journey Date</th>
                                <th>Fleet Type</th>
                                <th>Train Name</th>
                                <th>Trip</th>
                                <th>Seats</th>
                                <th>Status</th>
                                <th>Postponed</th>
                                <th>Check In</th>
                                <th>Luggage Details</th>
                                <th>Ticket Count</th>
                                <th>Fare</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
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
                                    style="color: #28c76f; font-size: 12px; text-decoration: none;">@<%# Eval("UserName") %>
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
                            <%# !string.IsNullOrEmpty(Eval("FleetTypeName")?.ToString()) && Eval("FleetTypeName").ToString() != "N/A" 
                                ? "<strong style='color: #28c76f;'>" + Eval("FleetTypeName") + "</strong>" 
                                : "<strong style='color: #999;'>N/A</strong>" %>
                        </td>
                        <td>
                            <%# !string.IsNullOrEmpty(Eval("TrainName")?.ToString()) && Eval("TrainName").ToString() != "N/A" 
                                ? "<span style='color: #333; font-weight: 500;'>" + Eval("TrainName") + "</span>" 
                                : "<span style='color: #999;'>N/A</span>" %>
                        </td>
                        <td>
                            <small style="color: #666;"><%# Eval("RouteName") %></small>
                        </td>
                        
                        <td><strong><%# Eval("SeatsDisplay") %></strong></td>
                        <td>
                            <span class="status-confirmed">
                                <i class="las la-check-circle"></i>Confirmed
                            </span>
                        </td>
                        <td>
                            <%# GetPostponedBadge(Convert.ToInt32(Eval("PostponeCount"))) %>
                        </td>
                        <td>
                            <%# GetYesNoBadge(Eval("Checkin").ToString()) %>
                        </td>
                        <td>
                            <div style="font-size: 12px;">
                                Total Bags: <%# Eval("BagsCount") %><br />
                                Total Weight: <%# Eval("BagsWeight") %><br />
                                Total Charge: <%# Convert.ToDecimal(Eval("BagsCharge")).ToString("N2") %> CDF
                            </div>
                        </td>
                        <td><%# Eval("TicketCount") %></td>
                        <td>
                            <strong style="color: #28c76f;"><%# Convert.ToDecimal(Eval("TotalAmount")).ToString("N2") %> CDF</strong>
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
                <i class="las la-ticket-alt"></i>
                <p>No booked tickets found.</p>
                <small style="color: #999;">All confirmed bookings will appear here.</small>
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

        $(document).ready(function () {
            console.log('Booked Tickets page loaded');

            var totalBooked = parseInt($('#MainContent_litTotalBooked').text());
            if (totalBooked > 0) {
                console.log('Total booked tickets:', totalBooked);
            }
        });

        function exportToExcel() {
            var table = document.querySelector('.tickets-table');
            var html = table.outerHTML;
            var url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
            var link = document.createElement('a');
            link.href = url;
            link.download = 'booked_tickets_' + new Date().toISOString().split('T')[0] + '.xls';
            link.click();
        }

        function printTickets() {
            window.print();
        }
      </script>
</asp:Content>
