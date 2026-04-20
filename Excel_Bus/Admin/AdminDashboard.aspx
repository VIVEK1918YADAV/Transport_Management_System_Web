<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="Excel_Bus.AdminDashboard" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../assets/global/css/daterangepicker.min.css" />
    <style>
        /* Main container to prevent overlap */
        .dashboard-container {
            max-width: 100%;
            margin: 0 auto;
            padding: 40px 20px 0 20px;
        }

        .dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: linear-gradient(135deg, #4e73df, #1cc88a);
            color: #fff;
            padding: 18px 22px;
            border-radius: 12px;
            box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
            margin-bottom: 22px;
            margin-top: 0;
            transition: transform 0.3s ease-in-out, box-shadow 0.3s ease-in-out;
        }

            .dashboard-header:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.12);
            }

        .header-left h1 {
            font-size: 20px;
            font-weight: 600;
            margin: 0;
            letter-spacing: 0.3px;
        }

            .header-left h1 i {
                font-size: 22px;
                margin-right: 8px;
            }

        .header-right p {
            font-size: 13px;
            margin: 0;
            color: rgba(255, 255, 255, 0.95);
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px;
            margin-bottom: 22px;
        }

        .widget-card {
            background: white;
            border-radius: 10px;
            padding: 18px 16px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.08);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            text-decoration: none;
            color: inherit;
            display: flex;
            justify-content: space-between;
            align-items: center;
            min-height: 100px;
        }

            .widget-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 15px rgba(0,0,0,0.12);
            }

            .widget-card.bg--primary {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
            }

            .widget-card.bg--info {
                background: linear-gradient(135deg, #36d1dc 0%, #5b86e5 100%);
                color: white;
            }

            .widget-card.bg--success {
                background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
                color: white;
            }

            .widget-card.bg--warning {
                background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
                color: white;
            }

        .widget-card__content-title {
            font-size: 12px;
            opacity: 0.92;
            margin-bottom: 6px;
            font-weight: 500;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .widget-card__content-amount {
            font-size: 24px;
            font-weight: 700;
            margin: 0;
            line-height: 1.2;
        }

        .widget-card__content-icon {
            font-size: 36px;
            opacity: 0.3;
        }

        .section-header {
            display: flex;
            align-items: center;
            margin: 22px 0 16px 0;
            padding-bottom: 8px;
            border-bottom: 2px solid #e8e8e8;
        }

            .section-header h5 {
                margin: 0;
                font-size: 1rem;
                font-weight: 600;
                color: #2c3e50;
            }

            .section-header i {
                margin-right: 8px;
                color: #667eea;
                font-size: 1.2rem;
            }

        .table-card {
            background: white;
            border-radius: 10px;
            padding: 18px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.08);
            margin-bottom: 22px;
            display: flex;
            flex-direction: column;
            height: 100%;
        }

            .table-card .card-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: 16px;
                padding-bottom: 10px;
                border-bottom: 2px solid #f0f0f0;
                flex-shrink: 0;
            }

                .table-card .card-header h5 {
                    margin: 0;
                    font-size: 0.95rem;
                    font-weight: 600;
                    color: #2c3e50;
                }

            .table-card .table-responsive {
                flex: 1;
                overflow-y: auto;
                max-height: 420px;
            }

        .chart-card {
            background: white;
            border-radius: 10px;
            padding: 18px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.08);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            display: flex;
            flex-direction: column;
            height: 100%;
        }

            .chart-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 15px rgba(0,0,0,0.12);
            }

            .chart-card h5 {
                font-size: 0.95rem;
                font-weight: 600;
                margin: 0 0 15px 0;
            }

            .chart-card .d-flex {
                margin-bottom: 15px;
                flex-shrink: 0;
            }

        /* Ensure equal height for row items */
        .row.gy-4 {
            display: flex;
            flex-wrap: wrap;
        }

            .row.gy-4 > .col-xl-6 {
                display: flex;
                flex-direction: column;
            }

        .expand-btn {
            cursor: pointer;
            font-size: 18px;
            color: #667eea;
            user-select: none;
            font-weight: bold;
            transition: all 0.3s ease;
            display: inline-block;
            width: 24px;
            height: 24px;
            line-height: 22px;
            text-align: center;
            border-radius: 50%;
            background: #f0f0f0;
        }

            .expand-btn:hover {
                background: #667eea;
                color: white;
            }

            .expand-btn.expanded {
                background: #667eea;
                color: white;
                transform: rotate(45deg);
            }

        .details-row {
            background-color: #f9f9f9;
            display: none;
        }

        .details-content {
            padding: 15px;
        }

        .nested-table {
            width: 100%;
            margin: 8px 0;
            background: white;
            border-radius: 6px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            display: table;
            border-collapse: collapse;
        }

            .nested-table thead {
                background: #667eea;
                color: white;
                display: table-header-group;
            }

            .nested-table th {
                padding: 12px;
                text-align: left;
                font-weight: 600;
                font-size: 13px;
                color: white !important;
                background: #667eea !important;
                border: 1px solid #5568d3;
            }
            
            .nested-table tbody {
                display: table-row-group;
            }

            .nested-table td {
                padding: 12px;
                border: 1px solid #e8e8e8;
                font-size: 12px;
                background: white;
            }

            .nested-table tbody tr:last-child td {
                border-bottom: 1px solid #e8e8e8;
            }

            .nested-table tbody tr:hover {
                background: #f5f5f5;
            }
            
            .nested-table tbody tr:hover td {
                background: #f5f5f5;
            }
            
        .bus-details-container h6 {
            margin-bottom: 10px;
            font-size: 14px;
            font-weight: 600;
            color: #667eea;
        }

        .empty-list {
            text-align: center;
            padding: 30px 15px;
        }

            .empty-list img {
                width: 110px;
                margin-bottom: 10px;
            }

            .empty-list p {
                font-size: 13px;
                color: #999;
            }

        .table-hover tbody tr:hover {
            background-color: #f8f9fa;
        }

        .table-hover thead th {
            font-size: 12px;
            font-weight: 600;
            padding: 10px 8px;
            background-color: #f8f9fa;
            border-bottom: 2px solid #dee2e6;
        }

        .table-hover tbody td {
            font-size: 12px;
            padding: 10px 8px;
            vertical-align: middle;
        }

        .table {
            margin-bottom: 0;
        }

        #paymentChartArea {
            height: 320px;
        }

        .badge-success {
            background: #d4edda;
            color: #155724;
            padding: 3px 9px;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: 500;
        }

        .badge-warning {
            background: #fff3cd;
            color: #856404;
            padding: 3px 9px;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: 500;
        }

        .badge-danger {
            background: #f8d7da;
            color: #721c24;
            padding: 3px 9px;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: 500;
        }

        .badge-info {
            background: #d1ecf1;
            color: #0c5460;
            padding: 3px 9px;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: 500;
        }

        .badge-secondary {
            background: #e2e3e5;
            color: #383d41;
            padding: 3px 9px;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: 500;
        }

        .booking-pnr {
            font-weight: 600;
            color: #667eea;
            font-size: 12px;
        }

        .booking-route {
            color: #666;
            font-size: 12px;
        }

        .booking-date {
            color: #999;
            font-size: 11px;
        }

        .booking-amount {
            font-weight: 600;
            color: #2c3e50;
            font-size: 12px;
        }

        .booking-vehicle {
            color: #28a745;
            font-size: 11px;
            font-weight: 600;
        }

        .trip-main-row {
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

            .trip-main-row:hover {
                background-color: #f5f5f5;
            }

        /* Date picker styling */
        #paymentDatePicker {
            font-size: 12px;
            padding: 6px 10px !important;
            border-radius: 6px;
        }

            #paymentDatePicker i {
                font-size: 14px;
            }

        /* Button styling */
        .btn-sm {
            font-size: 12px;
            padding: 6px 12px;
        }

        /* Responsive adjustments */
        @media (max-width: 1600px) {
            .stats-grid {
                grid-template-columns: repeat(4, 1fr);
                gap: 14px;
            }

            .widget-card {
                padding: 16px 14px;
                min-height: 95px;
            }

            .widget-card__content-amount {
                font-size: 22px;
            }

            .widget-card__content-icon {
                font-size: 32px;
            }
        }

        @media (max-width: 1200px) {
            .stats-grid {
                grid-template-columns: repeat(2, 1fr);
                gap: 12px;
            }

            .row.gy-4 > .col-xl-6 {
                width: 100%;
                max-width: 100%;
            }
        }

        @media (max-width: 768px) {
            .dashboard-container {
                padding: 10px 15px 0 15px;
            }

            .dashboard-header {
                flex-direction: column;
                text-align: center;
                padding: 15px 18px;
            }

            .header-left h1 {
                font-size: 18px;
                margin-bottom: 8px;
            }

            .header-right p {
                font-size: 12px;
            }

            .stats-grid {
                grid-template-columns: 1fr 1fr;
                gap: 10px;
            }
        }

        /* Improve row spacing */
        .row.gy-4 {
            margin-bottom: 18px;
            margin-left: -8px;
            margin-right: -8px;
        }

            .row.gy-4 > * {
                padding-left: 8px;
                padding-right: 8px;
            }

        .col-xl-6 {
            margin-bottom: 16px;
        }
    </style>
    <style>
    .details-content {
        background-color: #fafafa;
        padding: 20px;
        border-top: 2px solid #e0e0e0;
    }
    
    .bus-dropdown {
        padding: 8px 12px;
        min-width: 300px;
        border: 1px solid #ddd;
        border-radius: 4px;
        font-size: 14px;
        cursor: pointer;
    }
    
    .bus-dropdown:focus {
        outline: none;
        border-color: #4CAF50;
        box-shadow: 0 0 5px rgba(76, 175, 80, 0.3);
    }
    
    .nested-table {
        width: 100%;
        border-collapse: collapse;
        margin-top: 15px;
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    
    .nested-table thead tr {
        background-color: #f5f5f5;
    }
    
    .nested-table th,
    .nested-table td {
        padding: 12px;
        border: 1px solid #ddd;
    }
    
    .nested-table tbody tr:hover {
        background-color: #f9f9f9;
    }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-container">
        <!-- Dashboard Header -->
        <div class="dashboard-header">
            <div class="header-left">
                <h1><i class="las la-chart-line"></i>Dashboard Overview</h1>
            </div>
            <div class="header-right">
                <p>Welcome back! Here's what's happening with your bus service today.</p>
            </div>
        </div>

        <!-- Main Statistics Cards -->
        <div class="stats-grid">
            <a href="Users.aspx" class="widget-card bg--primary">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Total Users</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litTotalUsers" runat="server">0</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="las la-users"></i>
                </span>
            </a>

            <a href="ActiveUsers.aspx" class="widget-card bg--info">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Active Users</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litActiveUsers" runat="server">0</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="las la-user-check"></i>
                </span>
            </a>

            <a href="Counter.aspx" class="widget-card bg--success">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Total Cities</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litTotalCities" runat="server">0</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-map-marker-alt"></i>
                </span>
            </a>

            <a href="Vehicles.aspx" class="widget-card bg--warning">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Total Vehicles</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litTotalVehicles" runat="server">0</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-bus"></i>
                </span>
            </a>
        </div>

        <!-- Today's Statistics Section -->
        <div class="section-header">
            <i class="las la-calendar-day"></i>
            <h5>Today's Performance</h5>
        </div>

        <div class="stats-grid">
            <a href="Payments.aspx" class="widget-card bg--info">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Total Ticket Fare</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litTodayTicketFare" runat="server">0.00 CDF</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-money-bill-wave"></i>
                </span>
            </a>

            <a href="#" class="widget-card bg--success">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Online Booking</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litCashBookingToday" runat="server">0.00 CDF</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-cash-register"></i>
                </span>
            </a>

            <a href="#" class="widget-card bg--primary">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Cash Booking</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litOnlineBookingToday" runat="server">0.00 CDF</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-laptop"></i>
                </span>
            </a>

            <a href="#" class="widget-card bg--warning">
                <div class="widget-card__description">
                    <p class="widget-card__content-title">Running Buses</p>
                    <h3 class="widget-card__content-amount">
                        <asp:Literal ID="litBusRunningToday" runat="server">0</asp:Literal>
                    </h3>
                </div>
                <span class="widget-card__content-icon">
                    <i class="fas fa-bus-alt"></i>
                </span>
            </a>
        </div>

        <!-- Latest Booking History and Payment Chart -->
        <div class="row gy-4">
            <div class="col-xl-6">
                <div class="table-card">
                    <div class="card-header">
                        <h5><i class="las la-ticket-alt"></i>Latest Booking History</h5>
                        <a href="BookedTickets.aspx" class="btn btn-sm btn-primary">View All</a>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvLatestBookings" runat="server" CssClass="table table-hover"
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
                                <asp:TemplateField HeaderText="Vehicle">
                                    <ItemTemplate>
                                        <div class="booking-vehicle"><%# Eval("VehicleName") %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Journey Date">
                                    <ItemTemplate>
                                        <div class="booking-date"><%# Eval("JourneyDate") %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tickets">
                                    <ItemTemplate>
                                        <%# Eval("TicketCount") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <div class="booking-amount"><%# string.Format("{0:N2} CDF", Eval("Amount")) %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='<%# GetStatusBadgeClass(Eval("Status").ToString()) %>'>
                                            <%# Eval("Status") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="empty-list">
                                    <img src="../assets/images/empty_list.png" alt="empty" />
                                    <p class="text-muted">No bookings available</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="col-xl-6">
                <div class="chart-card">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h5><i class="las la-chart-line"></i>Payment History (Last 30 Days)</h5>
                        <div id="paymentDatePicker" class="border p-2 cursor-pointer rounded">
                            <i class="la la-calendar"></i>&nbsp;
                        <span></span><i class="la la-caret-down"></i>
                        </div>
                    </div>
                    <div id="paymentChartArea" style="height: 380px; min-width: 300px;"></div>
                </div>
            </div>
        </div>

        <!-- Trip Summary -->
        <div class="section-header mt-4">
            <i class="las la-route"></i>
            <h5>Today's Trip Details</h5>
        </div>

        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="gvTripSummary" runat="server" CssClass="table table-hover"
                    AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" ShowFooter="True"
                    OnRowDataBound="gvTripSummary_RowDataBound" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="TripName" HeaderText="Trip Name" />
                        <asp:BoundField DataField="TripDate" HeaderText="Trip Date" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:BoundField DataField="TripCount" HeaderText="Trip Count" />
                        <asp:TemplateField HeaderText="Trip Amount">
                            <ItemTemplate>
                                <%# string.Format("{0:N2} CDF", Eval("TripAmount")) %>
                            </ItemTemplate>
                            <FooterTemplate>
                                <b><%# TotalTripAmount.ToString("N2") %> CDF</b>
                            </FooterTemplate>
                        </asp:TemplateField>
                       
                    </Columns>
                    <FooterStyle Font-Bold="True" BackColor="#f8f9fa" />
                    <EmptyDataTemplate>
                        <div class="empty-list">
                            <img src="../assets/images/empty_list.png" alt="empty" />
                            <p class="text-muted">No trip data available</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Trip Details With Bus -->
      <div class="section-header">
            <i class="las la-bus"></i>
            <h5>Today's Trip Details With Bus</h5>
        </div>

        <div class="table-card">
            <div class="table-responsive">
                <asp:Repeater ID="rptTripDetails" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover" id="tripDetailsTable">
                            <thead>
                                <tr style="background-color: #fffacd;">
                                    <th width="50"></th>
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
                     <tr class="details-row" data-details-index="<%# Container.ItemIndex %>" style="display: none;">
    <td colspan="6" style="padding: 0;">
        <div class="details-content" style="padding: 20px;">
            <!-- Bus Selection Dropdown -->
            <div style="margin-bottom: 15px;">
                <label style="font-weight: bold; margin-right: 10px;">Select Bus:</label>
                <select class="bus-dropdown" 
                        data-trip-index="<%# Container.ItemIndex %>" 
                        onchange="showBusDetails(this)"
                        style="padding: 8px; min-width: 300px; border: 1px solid #ddd; border-radius: 4px;">
                    <option value="">-- Select Bus --</option>
                    <%# GetBusOptions((List<BusDetailViewModel>)Eval("BusDetails")) %>
                </select>
            </div>

            <!-- Bus Details Table (Initially Hidden) -->
            <div class="bus-details-container" style="display: none; margin-top: 20px;">
                <h6 style="color: #667eea; font-weight: 600; margin-bottom: 10px; font-size: 14px;">
                    <i class="las la-bus"></i> Selected Bus Details
                </h6>
                <table class="nested-table" style="width: 100%; border-collapse: collapse;">
                    <thead style="display: table-header-group;">
                        <tr style="background-color: #667eea; color: white;">
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: left; font-weight: 600; color: white; background-color: #667eea;">Bus Name</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Start Time</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">End Time</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Trip Count</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Passengers</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Total Bags</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Bag Weight</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Bags Charge</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: center; font-weight: 600; color: white; background-color: #667eea;">Check-in</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: right; font-weight: 600; color: white; background-color: #667eea;">Trip Amount</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: right; font-weight: 600; color: white; background-color: #667eea;">Online Amount</th>
                            <th style="padding: 12px; border: 1px solid #5568d3; text-align: right; font-weight: 600; color: white; background-color: #667eea;">Cash Amount</th>
                        </tr>
                    </thead>
                    <tbody id="busDetailsBody_<%# Container.ItemIndex %>" style="display: table-row-group;">
                        <!-- JavaScript will populate data here -->
                    </tbody>
                </table>
            </div>
            
            <!-- Hidden field for bus data -->
            <input type="hidden" 
                   id="busDetailsData_<%# Container.ItemIndex %>" 
                   value='<%# System.Web.HttpUtility.HtmlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(Eval("BusDetails"))) %>' />
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
    </div>
    <!-- Close dashboard-container -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <!-- Load libraries from CDN as fallback -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/moment@2.29.4/moment.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/apexcharts@3.45.1/dist/apexcharts.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
 

     <script type="text/javascript">
         function showBusDetails(dropdown) {
             var tripIndex = dropdown.getAttribute('data-trip-index');
             var selectedIndex = dropdown.value;

             console.log('Dropdown changed - tripIndex:', tripIndex, 'selectedIndex:', selectedIndex);

             // Get the container div
             var detailsContent = dropdown.closest('.details-content');
             var detailsContainer = detailsContent.querySelector('.bus-details-container');
             var tbody = document.getElementById('busDetailsBody_' + tripIndex);
             var hiddenData = document.getElementById('busDetailsData_' + tripIndex);

             console.log('Elements found:', {
                 detailsContainer: !!detailsContainer,
                 tbody: !!tbody,
                 hiddenData: !!hiddenData
             });

             // If nothing selected, hide the table
             if (selectedIndex === "" || selectedIndex === null) {
                 if (detailsContainer) {
                     detailsContainer.style.display = 'none';
                 }
                 if (tbody) {
                     tbody.innerHTML = '';
                 }
                 return;
             }

             try {
                 // Parse bus details from hidden field
                 var busDetails = JSON.parse(hiddenData.value);
                 console.log('Parsed bus details:', busDetails);
                 
                 var selectedBus = busDetails[parseInt(selectedIndex)];
                 console.log('Selected bus:', selectedBus);

                 if (selectedBus) {
                     // Clear previous data
                     tbody.innerHTML = '';

                     // Create new row with selected bus details including bags weight and yes/no checkin
                     var row = tbody.insertRow();
                     row.style.backgroundColor = 'white';
                     row.innerHTML =
                         '<td style="padding: 12px; border: 1px solid #ddd; background: white;">' + escapeHtml(selectedBus.BusName) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #28a745; font-weight: 600;">' + 
                             '<i class="las la-clock"></i> ' + escapeHtml(selectedBus.StartTime) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #dc3545; font-weight: 600;">' + 
                             '<i class="las la-clock"></i> ' + escapeHtml(selectedBus.EndTime) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white;">' + selectedBus.TripCount + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #667eea; font-weight: 600;">' + 
                             '<i class="las la-users"></i> ' + selectedBus.PassengerCount + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #ff6b6b; font-weight: 600;">' + 
                             '<i class="las la-suitcase"></i> ' + selectedBus.TotalBagsCount + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #9b59b6; font-weight: 600;">' + 
                             '<i class="las la-weight"></i> ' + escapeHtml(selectedBus.TotalBagsWeight || '0 kg') + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white; color: #f39c12; font-weight: 600;">' + 
                             formatCurrency(selectedBus.TotalBagsCharge) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: center; background: white;">' + 
                             getCheckinStatusYesNo(selectedBus.CheckedInCount, selectedBus.NotCheckedInCount) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: right; background: white; font-weight: 600;">' + formatCurrency(selectedBus.TripAmount) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: right; background: white;">' + formatCurrency(selectedBus.CashAmount) + '</td>' +
                         '<td style="padding: 12px; border: 1px solid #ddd; text-align: right; background: white;">' + formatCurrency(selectedBus.OnlineAmount) + '</td>';

                     // Show the details container with table - IMPORTANT
                     detailsContainer.style.display = 'block';
                     
                     console.log('✓ Bus details table displayed successfully');
                     console.log('Table container display:', detailsContainer.style.display);
                 }
             } catch (error) {
                 console.error('Error showing bus details:', error);
                 alert('Error loading bus details: ' + error.message);
             }
         }

         // Helper function to format currency
         function formatCurrency(amount) {
             return new Intl.NumberFormat('en-US', {
                 minimumFractionDigits: 2,
                 maximumFractionDigits: 2
             }).format(amount) + ' CDF';
         }

         // Helper function to escape HTML
         function escapeHtml(text) {
             var div = document.createElement('div');
             div.textContent = text;
             return div.innerHTML;
         }
         
         // Helper function to format check-in status as Yes/No
         function getCheckinStatusYesNo(checkedInCount, notCheckedInCount) {
             // Show simple Yes or No based on booking status
             if (checkedInCount > 0 && notCheckedInCount === 0) {
                 // All bookings checked in
                 return '<span style="color: #28a745; font-weight: 600; font-size: 16px;"><i class="las la-check-circle"></i> Yes</span>';
             } else if (notCheckedInCount > 0) {
                 // Any bookings not checked in = show No
                 return '<span style="color: #dc3545; font-weight: 600; font-size: 16px;"><i class="las la-times-circle"></i> No</span>';
             } else {
                 // No data
                 return '<span style="color: #6c757d; font-weight: 600; font-size: 16px;"><i class="las la-question-circle"></i> N/A</span>';
             }
         }
         
         // Debug: Check if tables are rendered on page load
         $(document).ready(function() {
             console.log('Checking for nested tables...');
             var tables = document.querySelectorAll('.nested-table');
             console.log('Found', tables.length, 'nested tables');
             
             tables.forEach(function(table, index) {
                 var thead = table.querySelector('thead');
                 var tbody = table.querySelector('tbody');
                 console.log('Table', index, '- thead:', !!thead, 'tbody:', !!tbody);
             });
         });
     </script>

    <script>
        var paymentLineChart;

        $(document).ready(function () {
            console.log('Page loaded - initializing...');

            // Initialize date range picker
            initDatePicker();

            // Wait for chart data and initialize
            initializeChart();

            // Initialize trip toggle after a delay
            //setTimeout(function () {
            //    initTripToggle();
            //}, 500);
        });

        function initDatePicker() {
            const start = moment().subtract(29, 'days');
            const end = moment();

            const dateRangeOptions = {
                startDate: start,
                endDate: end,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 15 Days': [moment().subtract(14, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                },
                maxDate: moment()
            };

            const changeDatePickerText = (element, startDate, endDate) => {
                $(element).html(startDate.format('MMM D, YYYY') + ' - ' + endDate.format('MMM D, YYYY'));
            };

            $('#paymentDatePicker').daterangepicker(dateRangeOptions, (start, end) => {
                changeDatePickerText('#paymentDatePicker span', start, end);
            });

            changeDatePickerText('#paymentDatePicker span', start, end);
        }

        function initializeChart() {
            // Check if data is available
            if (typeof paymentChartData === 'undefined') {
                console.log('Waiting for chart data...');
              /*  setTimeout(initializeChart, 100);*/
                return;
            }

            console.log('Chart data available:', paymentChartData);
            renderPaymentChart(paymentChartData);
        }

        function renderPaymentChart(chartData) {
            console.log('Rendering payment chart with data:', chartData);

            let categories = [];
            let amounts = [];

            if (!chartData || chartData.length === 0) {
                console.log('No data, showing last 30 days with zeros');
                // Show last 30 days with zero values
                for (let i = 29; i >= 0; i--) {
                    const date = moment().subtract(i, 'days');
                    categories.push(date.format('DD-MMM'));
                    amounts.push(0);
                }
            } else {
                categories = chartData.map(d => moment(d.Date).format('DD-MMM'));
                amounts = chartData.map(d => parseFloat(d.Amount) || 0);
                console.log('Processed categories:', categories);
                console.log('Processed amounts:', amounts);
            }

            var options = {
                series: [{
                    name: 'Payment Amount',
                    data: amounts
                }],
                chart: {
                    height: 380,
                    type: 'area',
                    toolbar: {
                        show: true,
                        tools: {
                            download: true,
                            zoom: true,
                            pan: true,
                            reset: true
                        }
                    },
                    zoom: {
                        enabled: true
                    },
                    animations: {
                        enabled: true,
                        easing: 'easeinout',
                        speed: 800
                    }
                },
                colors: ['#00E396'],
                dataLabels: {
                    enabled: false
                },
                stroke: {
                    curve: 'smooth',
                    width: 3
                },
                fill: {
                    type: 'gradient',
                    gradient: {
                        shadeIntensity: 1,
                        opacityFrom: 0.7,
                        opacityTo: 0.3,
                        stops: [0, 90, 100]
                    }
                },
                xaxis: {
                    categories: categories,
                    labels: {
                        rotate: -45,
                        rotateAlways: true,
                        style: {
                            fontSize: '11px'
                        }
                    },
                    tickPlacement: 'on'
                },
                yaxis: {
                    labels: {
                        formatter: function (val) {
                            if (val === null || val === undefined) return '0 CDF';
                            return val.toLocaleString('en-US', {
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            }) + ' CDF';
                        }
                    },
                    title: {
                        text: 'Amount (CDF)',
                        style: {
                            fontSize: '14px',
                            fontWeight: 600
                        }
                    }
                },
                tooltip: {
                    enabled: true,
                    shared: true,
                    intersect: false,
                    y: {
                        formatter: function (val) {
                            if (val === null || val === undefined) return '0.00 CDF';
                            return val.toLocaleString('en-US', {
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            }) + ' CDF';
                        }
                    }
                },
                grid: {
                    borderColor: '#e7e7e7',
                    strokeDashArray: 4,
                    padding: {
                        right: 30,
                        left: 20
                    }
                },
                markers: {
                    size: 4,
                    colors: ['#00E396'],
                    strokeColors: '#fff',
                    strokeWidth: 2,
                    hover: {
                        size: 7
                    }
                }
            };

            try {
                // Destroy existing chart if it exists
                if (paymentLineChart) {
                    paymentLineChart.destroy();
                }

                const chartElement = document.querySelector("#paymentChartArea");
                if (!chartElement) {
                    console.error('Chart container not found!');
                    return;
                }

                paymentLineChart = new ApexCharts(chartElement, options);
                paymentLineChart.render();
                console.log('✓ Chart rendered successfully with', amounts.length, 'data points');
            } catch (error) {
                console.error('✗ Error rendering chart:', error);
            }
        }

        function initTripToggle() {
            console.log('Initializing trip toggle...');

            var mainRows = $('.trip-main-row').length;
            var detailRows = $('.details-row').length;
            console.log('Found ' + mainRows + ' main rows and ' + detailRows + ' detail rows');

            $('.trip-main-row').off('click').on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();

                var $mainRow = $(this);
                var tripIndex = $mainRow.data('trip-index');
                var $expandBtn = $mainRow.find('.expand-btn');
                var $detailsRow = $('.details-row[data-details-index="' + tripIndex + '"]');

                if ($detailsRow.length > 0) {
                    if ($detailsRow.is(':visible')) {
                        $detailsRow.slideUp(300);
                        $expandBtn.text('+').removeClass('expanded');
                    } else {
                        $('.details-row').slideUp(300);
                        $('.expand-btn').text('+').removeClass('expanded');
                        $detailsRow.slideDown(300);
                        $expandBtn.text('−').addClass('expanded');
                    }
                }
            });

            console.log('✓ Trip toggle initialized');
        }
    </script>
</asp:Content>
