<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_AdminDashboard.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_AdminDashboard" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://maxst.icons8.com/vue-static/landings/line-awesome/line-awesome/1.3.0/css/line-awesome.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/line-awesome/1.3.0/line-awesome/css/line-awesome.min.css">
    <link rel="stylesheet" href="../assets/global/css/daterangepicker.min.css" />
    <style>
        /* =============================================
           SIDEBAR OVERLAP FIX
           Adjust left-offset to match your sidebar width.
           Common sidebar widths: 220px, 240px, 260px.
           Change the value below to match yours.
        ============================================= */
        body, html {
            background-color: #f0f2f5 !important;
        }

        /* If your MasterPage sidebar is 240px wide, use margin-left: 240px.
           This ensures the main content never slides under the sidebar. */
        .dashboard-wrapper {
            padding: 24px 28px 40px 28px;
            max-width: 100%;
            box-sizing: border-box;
            background: #f0f2f5;
            min-height: 100vh;
            /* ---- SIDEBAR FIX ---- */
            /* Remove or adjust this if your Master already handles content offset */
            overflow-x: hidden;
        }

        /* =============================================
           DASHBOARD HEADER
        ============================================= */
        .dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: linear-gradient(357deg, #37a77d 0%, #367452 100%);
            color: #ffffff;
            padding: 16px 24px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 1px 7px 8px rgb(37 118 78 / 60%);
        }

        .header-left h1 {
            font-size: 18px;
            font-weight: 600;
            margin: 0;
        }

        .header-left h1 i {
            margin-right: 8px;
            font-size: 20px;
        }

        .header-right p {
            font-size: 13px;
            margin: 0;
            color: rgba(255,255,255,0.85);
        }

        /* =============================================
           STATS GRID
        ============================================= */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 14px;
            margin-bottom: 20px;
        }

        /* =============================================
           WIDGET CARDS
        ============================================= */
        .widget-card {
            background: #ffffff;
            border-radius: 10px;
            padding: 18px 16px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.08);
            text-decoration: none;
            color: inherit;
            display: flex;
            justify-content: space-between;
            align-items: center;
            min-height: 90px;
            transition: box-shadow 0.2s ease, transform 0.2s ease;
            border-left: 4px solid transparent;
            box-sizing: border-box;
        }

        .widget-card:hover {
            box-shadow: 0 4px 14px rgba(0,0,0,0.12);
            transform: translateY(-2px);
            text-decoration: none;
            color: inherit;
        }

        .widget-card.card-users    { border-left-color: #2e7d32; }
        .widget-card.card-active   { border-left-color: #1565c0; }
        .widget-card.card-stations { border-left-color: #6a1b9a; }
        .widget-card.card-trains   { border-left-color: #e65100; }
        .widget-card.card-fare     { border-left-color: #00796b; }
        .widget-card.card-online   { border-left-color: #1565c0; }
        .widget-card.card-cash     { border-left-color: #f57f17; }
        .widget-card.card-running  { border-left-color: #c62828; }

        .widget-card__title {
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.6px;
            color: #7a8899;
            margin-bottom: 8px;
        }

        .widget-card__amount {
            font-size: 22px;
            font-weight: 700;
            color: #1a2332;
            margin: 0;
            line-height: 1.2;
        }

        .widget-card__icon {
            font-size: 32px;
            opacity: 0.18;
            color: #1a2332;
        }

        /* =============================================
           SECTION HEADER
        ============================================= */
        .section-header {
            display: flex;
            align-items: center;
            margin: 22px 0 14px 0;
            padding-bottom: 8px;
            border-bottom: 2px solid #e2e6ea;
        }

        .section-header h5 {
            margin: 0;
            font-size: 14px;
            font-weight: 600;
            color: #1a2332;
        }

        .section-header i {
            margin-right: 8px;
            color: #2e7d32;
            font-size: 18px;
        }

        /* =============================================
           TABLE CARD
        ============================================= */
        .table-card {
            background: #ffffff;
            border-radius: 10px;
            padding: 18px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.08);
            margin-bottom: 20px;
            display: flex;
            flex-direction: column;
        }

        .table-card .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 14px;
            padding-bottom: 10px;
            border-bottom: 1px solid #f0f0f0;
        }

        .table-card .card-header h5 {
            margin: 0;
            font-size: 14px;
            font-weight: 600;
            color: #1a2332;
        }

        .table-card .table-responsive {
            overflow-y: auto;
            max-height: 400px;
        }

        /* =============================================
           CHART CARD  — FIXED FOR PROPER API INTEGRATION
        ============================================= */
        .chart-card {
            background: #ffffff;
            border-radius: 10px;
            padding: 18px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.08);
            display: flex;
            flex-direction: column;
            height: 100%;
            box-sizing: border-box;
        }

        .chart-card h5 {
            font-size: 14px;
            font-weight: 600;
            color: #1a2332;
            margin: 0 0 14px 0;
        }

        #paymentChartArea {
            height: 340px;
            flex: 1;
            position: relative;
        }

        /* Chart loading/error states */
        .chart-loading {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 300px;
            color: #9aa3ad;
            font-size: 13px;
            flex-direction: column;
            gap: 10px;
        }

        .chart-loading .spinner {
            width: 32px;
            height: 32px;
            border: 3px solid #e9ecef;
            border-top-color: #2e7d32;
            border-radius: 50%;
            animation: spin 0.8s linear infinite;
        }

        @keyframes spin { to { transform: rotate(360deg); } }

        /* Chart legend (custom) */
        #chartLegend {
            display: flex;
            flex-wrap: wrap;
            gap: 6px 12px;
            margin-top: 8px;
            padding-top: 8px;
            border-top: 1px solid #f0f0f0;
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 5px;
            font-size: 11px;
            color: #5a6475;
        }

        .legend-dot {
            width: 10px;
            height: 10px;
            border-radius: 2px;
            flex-shrink: 0;
        }

        /* Chart summary row */
        .chart-summary {
            display: flex;
            gap: 20px;
            margin-bottom: 12px;
            padding: 10px 14px;
            background: #f8f9fa;
            border-radius: 8px;
        }

        .chart-summary-item {
            display: flex;
            flex-direction: column;
        }

        .chart-summary-item .label {
            font-size: 10px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #9aa3ad;
        }

        .chart-summary-item .value {
            font-size: 15px;
            font-weight: 700;
            color: #1a2332;
        }

        /* =============================================
           TWO-COLUMN ROW
        ============================================= */
        .two-col-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 14px;
            margin-bottom: 20px;
            align-items: start;
        }

        /* =============================================
           TABLE STYLES
        ============================================= */
        .table {
            width: 100%;
            margin-bottom: 0;
            border-collapse: collapse;
        }

        .table thead th {
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            padding: 10px 10px;
            background-color: #f8f9fa;
            color: #5a6475;
            border-bottom: 2px solid #e9ecef;
            white-space: nowrap;
        }

        .table tbody td {
            font-size: 12px;
            padding: 10px 10px;
            vertical-align: middle;
            border-bottom: 1px solid #f4f4f4;
            color: #3a4453;
        }

        .table tbody tr:last-child td { border-bottom: none; }
        .table-hover tbody tr:hover td { background-color: #f8fbff; }

        /* =============================================
           BADGES
        ============================================= */
        .badge {
            padding: 3px 9px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 500;
            display: inline-block;
        }

        .badge-success   { background: #e8f5e9; color: #2e7d32; }
        .badge-warning   { background: #fff8e1; color: #f57f17; }
        .badge-danger    { background: #fce4ec; color: #c62828; }
        .badge-info      { background: #e3f2fd; color: #1565c0; }
        .badge-secondary { background: #eceff1; color: #546e7a; }

        /* =============================================
           BOOKING SPECIFIC
        ============================================= */
        .booking-pnr     { font-weight: 600; color: #1565c0; font-size: 12px; }
        .booking-route   { color: #5a6475; font-size: 12px; }
        .booking-vehicle { color: #2e7d32; font-size: 11px; font-weight: 600; }
        .booking-date    { color: #9aa3ad; font-size: 11px; }
        .booking-amount  { font-weight: 600; color: #1a2332; font-size: 12px; }

        /* =============================================
           TRIP DETAILS TABLE
        ============================================= */
        .trip-main-row { cursor: pointer; transition: background-color 0.2s; }
        .trip-main-row:hover td { background-color: #f8fbff; }

        .expand-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 22px;
            height: 22px;
            border-radius: 50%;
            background: #e8f5e9;
            color: #2e7d32;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.2s;
            user-select: none;
            line-height: 1;
        }

        .expand-btn:hover, .expand-btn.expanded {
            background: #2e7d32;
            color: white;
        }

        .expand-btn.expanded { transform: rotate(45deg); }

        .details-row { display: none; background: #fafbfc; }

        .details-content {
            padding: 16px 20px;
            border-top: 1px solid #e9ecef;
        }

        .nested-table { width: 100%; border-collapse: collapse; margin-top: 12px; border-radius: 8px; overflow: hidden; }
        .nested-table thead tr { background: #2e7d32; }
        .nested-table th {
            padding: 10px 12px;
            text-align: left;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            color: #ffffff !important;
            background: #2e7d32 !important;
            border: none;
            white-space: nowrap;
        }
        .nested-table td {
            padding: 10px 12px;
            border-bottom: 1px solid #eeeeee;
            font-size: 12px;
            color: #3a4453;
            background: white;
        }
        .nested-table tbody tr:last-child td { border-bottom: none; }
        .nested-table tbody tr:hover td { background: #f5faf5; }

        .bus-dropdown {
            padding: 7px 12px;
            min-width: 280px;
            border: 1px solid #dde3ea;
            border-radius: 6px;
            font-size: 13px;
            cursor: pointer;
            color: #3a4453;
            background: white;
            outline: none;
        }

        .bus-dropdown:focus {
            border-color: #2e7d32;
            box-shadow: 0 0 0 3px rgba(46,125,50,0.1);
        }

        .bus-details-container h6 { font-size: 13px; font-weight: 600; color: #2e7d32; margin-bottom: 10px; }

        /* =============================================
           DATE PICKER
        ============================================= */
        #paymentDatePicker {
            font-size: 12px;
            padding: 5px 10px;
            border-radius: 6px;
            cursor: pointer;
            border: 1px solid #dde3ea;
            color: #5a6475;
            background: white;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        /* =============================================
           EMPTY STATE
        ============================================= */
        .empty-list { text-align: center; padding: 30px 15px; }
        .empty-list img { width: 90px; margin-bottom: 10px; opacity: 0.5; }
        .empty-list p { font-size: 13px; color: #9aa3ad; }

        /* =============================================
           FOOTER TOTALS
        ============================================= */
        .table tfoot td, .table tfoot th {
            font-size: 12px;
            padding: 10px 10px;
            background: #f8f9fa;
            font-weight: 600;
            border-top: 2px solid #e9ecef;
        }

        /* =============================================
           RESPONSIVE
        ============================================= */
        @media (max-width: 1200px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .two-col-row { grid-template-columns: 1fr; }
        }

        @media (max-width: 768px) {
            .dashboard-wrapper { padding: 12px 14px 30px 14px; }
            .stats-grid { grid-template-columns: 1fr 1fr; gap: 10px; }
            .dashboard-header { flex-direction: column; text-align: center; gap: 8px; }
        }

        @media (max-width: 480px) { .stats-grid { grid-template-columns: 1fr; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-wrapper">

        <!-- Dashboard Header -->
        <div class="dashboard-header">
            <div class="header-left">
                <h1><i class="las la-chart-line"></i>Dashboard Overview</h1>
            </div>
            <div class="header-right">
                <p>Welcome back! Here's what's happening with your train service today.</p>
            </div>
        </div>

        <!-- Main Statistics Cards -->
        <div class="stats-grid">
            <a href="AllUsers.aspx" class="widget-card card-users">
                <div>
                    <p class="widget-card__title">Total Users</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="TotalUsers" runat="server">0</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="las la-users"></i></span>
            </a>

            <a href="Train_ActiveUser.aspx" class="widget-card card-active">
                <div>
                    <p class="widget-card__title">Active Users</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="ActiveUsers" runat="server">0</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="las la-user-check"></i></span>
            </a>

            <a href="Counter.aspx" class="widget-card card-stations">
                <div>
                    <p class="widget-card__title">Total Stations</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="TotalCities" runat="server">0</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="fas fa-map-marker-alt"></i></span>
            </a>

            <a href="Vehicles.aspx" class="widget-card card-trains">
                <div>
                    <p class="widget-card__title">Total Trains</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="TotalVehicles" runat="server">0</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="fas fa-train"></i></span>
            </a>
        </div>

        <!-- Today's Performance -->
        <div class="section-header">
            <i class="las la-calendar-day"></i>
            <h5>Today's Performance</h5>
        </div>

        <div class="stats-grid">
            <a href="Payments.aspx" class="widget-card card-fare">
                <div>
                    <p class="widget-card__title">Total Ticket Fare</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="TodayTicketFare" runat="server">0.00 CDF</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="fas fa-money-bill-wave"></i></span>
            </a>

            <a href="#" class="widget-card card-online">
                <div>
                    <p class="widget-card__title">Online Booking</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="CashBookingToday" runat="server">0.00 CDF</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="las la-laptop"></i></span>
            </a>

            <a href="#" class="widget-card card-cash">
                <div>
                    <p class="widget-card__title">Cash Booking</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="OnlineBookingToday" runat="server">0.00 CDF</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="fas fa-cash-register"></i></span>
            </a>

            <a href="#" class="widget-card card-running">
                <div>
                    <p class="widget-card__title">Running Trains</p>
                    <h3 class="widget-card__amount"><asp:Literal ID="TrainRunningToday" runat="server">0</asp:Literal></h3>
                </div>
                <span class="widget-card__icon"><i class="fas fa-train"></i></span>
            </a>
        </div>

        <!-- Bookings + Chart -->
        <div class="two-col-row">
            <!-- Latest Booking History -->
            <div class="table-card">
                <div class="card-header">
                    <h5><i class="las la-ticket-alt" style="margin-right:6px;color:#2e7d32;"></i>Latest Booking History</h5>
                    <a href="Train_BookedTickets.aspx" class="btn btn-sm btn-success" style="font-size:12px;padding:4px 12px;">View All</a>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvTrainLatestBookings" runat="server" CssClass="table table-hover"
                        AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" GridLines="None">
                        <Columns>
                            <asp:TemplateField HeaderText="PNR Number">
                                <ItemTemplate>
                                    <div class="booking-pnr"><%# Eval("PnrNumber") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Route">
                                <ItemTemplate>
                                    <div class="booking-route"><%# Eval("Route") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Train">
                                <ItemTemplate>
                                    <div class="booking-vehicle"><%# Eval("TrainName") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Journey Date">
                                <ItemTemplate>
                                    <div class="booking-date"><%# Eval("JourneyDate") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tickets">
                                <ItemTemplate><%# Eval("TicketCount") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <div class="booking-amount"><%# string.Format("{0:N2} CDF", Eval("Amount")) %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class='<%# GetStatusBadgeClass(Eval("status") != null ? Eval("status").ToString() : "") %>'>
                                        <%# Eval("status") ?? "N/A" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-list">
                                <img src="../assets/images/empty_list.png" alt="empty" />
                                <p>No bookings available</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Payment Chart — FIXED: now uses trxTypeStatus grouping from API -->
            <div class="chart-card">
                <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:10px;">
                    <h5 style="margin:0;">
                        <i class="las la-chart-pie" style="margin-right:6px;color:#2e7d32;"></i>
                        Payment History (Last 30 Days)
                    </h5>
                    <div id="paymentDatePicker">
                        <i class="la la-calendar"></i>
                        <span></span>
                        <i class="la la-caret-down"></i>
                    </div>
                </div>

                <!-- Summary row above chart -->
                <div class="chart-summary" id="chartSummary" style="display:none;">
                    <div class="chart-summary-item">
                        <span class="label">Total Amount</span>
                        <span class="value" id="summaryTotal">0 CDF</span>
                    </div>
                    <div class="chart-summary-item">
                        <span class="label">Transactions</span>
                        <span class="value" id="summaryCount">0</span>
                    </div>
                    <div class="chart-summary-item">
                        <span class="label">Date Range</span>
                        <span class="value" id="summaryDays" style="font-size:12px;">—</span>
                    </div>
                </div>

                <!-- Chart container -->
                <div id="paymentChartArea">
                    <div class="chart-loading" id="chartLoading">
                        <div class="spinner"></div>
                        <span>Loading payment data...</span>
                    </div>
                </div>

                <!-- Custom legend -->
                <div id="chartLegend"></div>
            </div>
        </div>

        <!-- Trip Summary -->
        <div class="section-header">
            <i class="las la-route"></i>
            <h5>Today's Trip Details</h5>
        </div>

        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="gvtripSummary" runat="server" CssClass="table table-hover"
                    AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" ShowFooter="True"
                    OnRowDataBound="gvtripSummary_RowDataBound" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="TripName" HeaderText="Trip Name" />
                        <asp:BoundField DataField="TripDate" HeaderText="Trip Date" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:BoundField DataField="TripCount" HeaderText="Trip Count" />
                        <asp:TemplateField HeaderText="Trip Amount">
                            <ItemTemplate><%# string.Format("{0:N2} CDF", Eval("TripAmount")) %></ItemTemplate>
                            <FooterTemplate><b><%# TotalTrainTripAmount.ToString("N2") %> CDF</b></FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle Font-Bold="True" BackColor="#f8f9fa" />
                    <EmptyDataTemplate>
                        <div class="empty-list">
                            <img src="../assets/images/empty_list.png" alt="empty" />
                            <p>No trip data available</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Trip Details With Train -->
        <div class="section-header">
            <i class="las la-train"></i>
            <h5>Today's Trip Details With Train</h5>
        </div>

        <div class="table-card">
            <div class="table-responsive">
                <asp:Repeater ID="rptTrainTripDetails" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover">
                            <thead>
                                <tr>
                                    <th width="40"></th>
                                    <th>Trip Name</th>
                                    <th>Trip Date</th>
                                    <th>Trip Booking Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="trip-main-row" data-trip-index="<%# Container.ItemIndex %>">
                            <td><span class="expand-btn">+</span></td>
                            <td><strong><%# Eval("TripName") %></strong></td>
                            <td><%# Eval("TripDate", "{0:dd-MM-yyyy}") %></td>
                            <td><%# string.Format("{0:N2} CDF", Eval("TripAmount")) %></td>
                        </tr>
                        <tr class="details-row" data-details-index="<%# Container.ItemIndex %>" style="display:none;">
                            <td colspan="4" style="padding:0;">
                                <div class="details-content">
                                    <div style="margin-bottom:12px;">
                                        <label style="font-weight:600;font-size:13px;color:#3a4453;margin-right:10px;">Select Train:</label>
                                        <select class="bus-dropdown"
                                                data-trip-index="<%# Container.ItemIndex %>"
                                                onchange="showTrainDetails(this)">
                                            <option value="">-- Select Train --</option>
                                            <%# GetTrainOptions((List<TrainDetailViewModel>)Eval("TrainDetails")) %>
                                        </select>
                                    </div>

                                    <div class="bus-details-container" style="display:none;margin-top:16px;">
                                        <h6><i class="las la-train" style="margin-right:5px;"></i>Selected Train Details</h6>
                                        <div style="overflow-x:auto;">
                                            <table class="nested-table">
                                                <thead>
                                                    <tr>
                                                        <th>Train Name</th>
                                                        <th>Train Number</th>
                                                        <th>Start Time</th>
                                                        <th>End Time</th>
                                                        <th>Trip Count</th>
                                                        <th>Passengers</th>
                                                        <th>Total Bags</th>
                                                        <th>Bag Weight</th>
                                                        <th>Bags Charge</th>
                                                        <th>Check-in</th>
                                                        <th>Trip Amount</th>
                                                        <th>Online Amount</th>
                                                        <th>Cash Amount</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="busDetailsBody_<%# Container.ItemIndex %>">
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>

                                    <input type="hidden"
                                           id="busDetailsData_<%# Container.ItemIndex %>"
                                           value='<%# System.Web.HttpUtility.HtmlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(Eval("TrainDetails"))) %>' />
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div><%-- end dashboard-wrapper --%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/moment@2.29.4/moment.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/apexcharts@3.45.1/dist/apexcharts.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>

    <script type="text/javascript">

        /* =============================================
           API URL — change to match your config
        ============================================= */
        var API_BASE = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';

        /* =============================================
           TRAIN DETAILS DROPDOWN HANDLER
        ============================================= */
        function showTrainDetails(dropdown) {
            var tripIndex = dropdown.getAttribute('data-trip-index');
            var selectedIndex = dropdown.value;
            var detailsContent = dropdown.closest('.details-content');
            var detailsContainer = detailsContent.querySelector('.bus-details-container');
            var tbody = document.getElementById('busDetailsBody_' + tripIndex);
            var hiddenData = document.getElementById('busDetailsData_' + tripIndex);

            if (!selectedIndex) {
                if (detailsContainer) detailsContainer.style.display = 'none';
                if (tbody) tbody.innerHTML = '';
                return;
            }

            try {
                var trainDetails = JSON.parse(hiddenData.value);
                var selectedTrain = trainDetails[parseInt(selectedIndex)];

                if (selectedTrain) {
                    tbody.innerHTML = '';
                    var row = tbody.insertRow();
                    row.innerHTML =
                        '<td>' + escapeHtml(selectedTrain.TrainName || 'N/A') + '</td>' +
                        '<td>' + escapeHtml(selectedTrain.TrainNumber || 'N/A') + '</td>' +
                        '<td style="color:#2e7d32;font-weight:600;"><i class="las la-clock"></i> ' + escapeHtml(selectedTrain.StartTime || 'N/A') + '</td>' +
                        '<td style="color:#c62828;font-weight:600;"><i class="las la-clock"></i> ' + escapeHtml(selectedTrain.EndTime || 'N/A') + '</td>' +
                        '<td style="text-align:center;">' + (selectedTrain.TripCount || 0) + '</td>' +
                        '<td style="text-align:center;color:#1565c0;font-weight:600;"><i class="las la-users"></i> ' + (selectedTrain.PassengerCount || 0) + '</td>' +
                        '<td style="text-align:center;color:#e65100;font-weight:600;"><i class="las la-suitcase"></i> ' + (selectedTrain.TotalBagsCount || 0) + '</td>' +
                        '<td style="text-align:center;color:#6a1b9a;font-weight:600;">' + escapeHtml(selectedTrain.TotalBagsWeight || '0 kg') + '</td>' +
                        '<td style="text-align:center;">' + formatCurrency(selectedTrain.TotalBagsCharge) + '</td>' +
                        '<td style="text-align:center;">' + getCheckinStatus(selectedTrain.CheckedInCount, selectedTrain.NotCheckedInCount) + '</td>' +
                        '<td style="text-align:right;font-weight:600;">' + formatCurrency(selectedTrain.TripAmount) + '</td>' +
                        '<td style="text-align:right;">' + formatCurrency(selectedTrain.OnlineAmount) + '</td>' +
                        '<td style="text-align:right;">' + formatCurrency(selectedTrain.CashAmount) + '</td>';

                    detailsContainer.style.display = 'block';
                }
            } catch (err) {
                console.error('Error showing train details:', err);
            }
        }

        function formatCurrency(amount) {
            return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount || 0) + ' CDF';
        }

        function escapeHtml(text) {
            var d = document.createElement('div');
            d.textContent = text;
            return d.innerHTML;
        }

        function getCheckinStatus(checkedIn, notCheckedIn) {
            if (checkedIn > 0 && notCheckedIn === 0)
                return '<span style="color:#2e7d32;font-weight:600;"><i class="las la-check-circle"></i> Yes</span>';
            if (notCheckedIn > 0)
                return '<span style="color:#c62828;font-weight:600;"><i class="las la-times-circle"></i> No</span>';
            return '<span style="color:#9aa3ad;font-weight:600;"><i class="las la-question-circle"></i> N/A</span>';
        }

        /* =============================================
           EXPAND / COLLAPSE TRIP ROWS
        ============================================= */
        $(document).ready(function () {
            initTripToggle();
            initDatePicker();
            loadAndRenderChart(moment().subtract(29, 'days'), moment());
        });

        function initTripToggle() {
            $('.trip-main-row').off('click').on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();

                var $mainRow = $(this);
                var tripIndex = $mainRow.data('trip-index');
                var $btn = $mainRow.find('.expand-btn');
                var $details = $('.details-row[data-details-index="' + tripIndex + '"]');

                if ($details.is(':visible')) {
                    $details.slideUp(250);
                    $btn.text('+').removeClass('expanded');
                } else {
                    $('.details-row').slideUp(250);
                    $('.expand-btn').text('+').removeClass('expanded');
                    $details.slideDown(250);
                    $btn.text('−').addClass('expanded');
                }
            });
        }

        /* =============================================
           DATE RANGE PICKER — refreshes chart on change
        ============================================= */
        function initDatePicker() {
            var start = moment().subtract(29, 'days');
            var end = moment();

            $('#paymentDatePicker').daterangepicker({
                startDate: start,
                endDate: end,
                maxDate: moment(),
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 15 Days': [moment().subtract(14, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, function (s, e2) {
                $('#paymentDatePicker span').html(s.format('MMM D, YYYY') + ' – ' + e2.format('MMM D, YYYY'));
                loadAndRenderChart(s, e2);
            });

            $('#paymentDatePicker span').html(start.format('MMM D, YYYY') + ' – ' + end.format('MMM D, YYYY'));
        }

        /* =============================================
           CHART — FETCH DIRECTLY FROM API
           Groups by trxTypeStatus (Booking, Postpone, etc.)
           Only includes paymentStatus == "Successful"
        ============================================= */
        var paymentChart = null;

        function loadAndRenderChart(startDate, endDate) {
            var apiUrl = API_BASE + 'TrainTransactions/GetTrainTransactions';

            // Show loading state
            document.getElementById('chartLoading').style.display = 'flex';
            document.getElementById('paymentChartArea').style.position = 'relative';
            document.getElementById('chartSummary').style.display = 'none';
            document.getElementById('chartLegend').innerHTML = '';

            if (paymentChart) {
                paymentChart.destroy();
                paymentChart = null;
            }

            fetch(apiUrl)
                .then(function (res) {
                    if (!res.ok) throw new Error('API error: ' + res.status);
                    return res.json();
                })
                .then(function (data) {
                    console.log('Transactions loaded:', data.length);
                    processAndRenderChart(data, startDate, endDate);
                })
                .catch(function (err) {
                    console.error('Chart API error:', err);
                    showChartError('Unable to load payment data');
                });
        }

        function processAndRenderChart(transactions, startDate, endDate) {
            var start = startDate.clone().startOf('day');
            var end = endDate.clone().endOf('day');

            /* ---- Filter: Successful + within date range ---- */
            var filtered = transactions.filter(function (t) {
                if (!t.paymentStatus) return false;
                if (t.paymentStatus.toLowerCase() !== 'successful') return false;

                var txDate = moment(t.createdAt);
                return txDate.isBetween(start, end, null, '[]');
            });

            console.log('Filtered successful transactions:', filtered.length);

            if (filtered.length === 0) {
                showNoDataChart();
                return;
            }

            /* ---- Group by trxTypeStatus ---- */
            var groups = {};
            filtered.forEach(function (t) {
                var type = t.trxTypeStatus || 'Unknown';
                if (!groups[type]) groups[type] = { total: 0, count: 0 };
                groups[type].total += parseFloat(t.amount) || 0;
                groups[type].count += 1;
            });

            var labels = Object.keys(groups);
            var amounts = labels.map(function (k) { return groups[k].total; });
            var counts = labels.map(function (k) { return groups[k].count; });

            var totalAmount = amounts.reduce(function (a, b) { return a + b; }, 0);
            var totalCount = filtered.length;

            /* ---- Update summary row ---- */
            document.getElementById('summaryTotal').textContent =
                new Intl.NumberFormat('en-US', { minimumFractionDigits: 2 }).format(totalAmount) + ' CDF';
            document.getElementById('summaryCount').textContent = totalCount;
            document.getElementById('summaryDays').textContent =
                startDate.format('DD MMM') + ' – ' + endDate.format('DD MMM YYYY');
            document.getElementById('chartSummary').style.display = 'flex';

            /* ---- Color palette ---- */
            var palette = [
                '#2e7d32', '#1565c0', '#e65100', '#6a1b9a', '#00796b',
                '#c62828', '#f57f17', '#283593', '#4e342e', '#00838f'
            ];
            var chartColors = palette.slice(0, labels.length);

            /* ---- Hide loader ---- */
            document.getElementById('chartLoading').style.display = 'none';

            /* ---- Build custom legend ---- */
            var legendHtml = '';
            labels.forEach(function (label, i) {
                var pct = totalAmount > 0 ? ((amounts[i] / totalAmount) * 100).toFixed(1) : '0.0';
                var fmtAmt = new Intl.NumberFormat('en-US', { minimumFractionDigits: 0 }).format(amounts[i]);
                legendHtml +=
                    '<div class="legend-item">' +
                    '<div class="legend-dot" style="background:' + chartColors[i] + ';"></div>' +
                    '<span>' + escapeHtml(label) + ' — ' + fmtAmt + ' CDF (' + pct + '%)</span>' +
                    '</div>';
            });
            document.getElementById('chartLegend').innerHTML = legendHtml;

            /* ---- Render ApexCharts Donut ---- */
            var options = {
                series: amounts,
                labels: labels,
                chart: {
                    type: 'donut',
                    height: 300,
                    toolbar: { show: false },
                    animations: { enabled: true, speed: 600 }
                },
                colors: chartColors,
                plotOptions: {
                    pie: {
                        donut: {
                            size: '60%',
                            labels: {
                                show: true,
                                total: {
                                    show: true,
                                    label: 'Total',
                                    fontSize: '13px',
                                    fontWeight: 600,
                                    color: '#5a6475',
                                    formatter: function (w) {
                                        var sum = w.globals.seriesTotals.reduce(function (a, b) { return a + b; }, 0);
                                        return new Intl.NumberFormat('en-US', { minimumFractionDigits: 0 }).format(sum) + ' CDF';
                                    }
                                },
                                value: {
                                    fontSize: '15px',
                                    fontWeight: 700,
                                    color: '#1a2332',
                                    formatter: function (val) {
                                        return new Intl.NumberFormat('en-US', { minimumFractionDigits: 0 }).format(parseFloat(val)) + ' CDF';
                                    }
                                }
                            }
                        }
                    }
                },
                dataLabels: {
                    enabled: true,
                    formatter: function (val) {
                        return parseFloat(val).toFixed(1) + '%';
                    },
                    style: { fontSize: '11px', fontWeight: '600' },
                    dropShadow: { enabled: false }
                },
                /* Hide default legend — using custom HTML legend below chart */
                legend: { show: false },
                tooltip: {
                    y: {
                        formatter: function (val, opts) {
                            var cnt = counts[opts.seriesIndex];
                            return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2 }).format(val) + ' CDF (' + cnt + ' trx)';
                        }
                    }
                },
                stroke: { width: 2, colors: ['#fff'] },
                responsive: [{
                    breakpoint: 600,
                    options: { chart: { height: 240 } }
                }]
            };

            var el = document.querySelector('#paymentChartArea');
            if (!el) return;

            paymentChart = new ApexCharts(el, options);
            paymentChart.render();
        }

        function showNoDataChart() {
            document.getElementById('chartLoading').style.display = 'none';

            var options = {
                series: [1],
                labels: ['No Data'],
                chart: { type: 'donut', height: 300, toolbar: { show: false } },
                colors: ['#e9ecef'],
                plotOptions: {
                    pie: {
                        donut: {
                            size: '60%',
                            labels: {
                                show: true,
                                total: {
                                    show: true,
                                    label: 'No Transactions',
                                    fontSize: '12px',
                                    color: '#9aa3ad',
                                    formatter: function () { return 'For selected range'; }
                                }
                            }
                        }
                    }
                },
                dataLabels: { enabled: false },
                legend: { show: false },
                tooltip: { enabled: false }
            };

            var el = document.querySelector('#paymentChartArea');
            if (el) {
                paymentChart = new ApexCharts(el, options);
                paymentChart.render();
            }
        }

        function showChartError(msg) {
            document.getElementById('chartLoading').style.display = 'none';
            var el = document.querySelector('#paymentChartArea');
            if (el) {
                el.innerHTML = '<div style="display:flex;align-items:center;justify-content:center;height:200px;color:#c62828;font-size:13px;"><i class="las la-exclamation-circle" style="font-size:20px;margin-right:6px;"></i>' + escapeHtml(msg) + '</div>';
            }
        }

    </script>
</asp:Content>
