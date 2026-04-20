<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="TrainTicket.aspx.cs" Inherits="Excel_Bus.TrainTicket" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Buy Train Tickets
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdn.lineawesome.com/1.3.0/line-awesome/css/line-awesome.min.css">
    


    <!-- Search Section -->
    <div class="ticket-search-bar bg_img padding-top" style='background: url("img/train-bg.jpg") left center;'>
        <div class="container">
            <div class="train-search-header"> 
                <asp:Panel ID="pnlTicketSearch" runat="server" CssClass="ticket-form ticket-form-two row g-3 justify-content-center">
                    <div class="col-md-4 col-lg-3">
                        <div class="form--group">
                            <asp:DropDownList ID="ddlSource" runat="server" CssClass="form--control select2">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4 col-lg-3">
                        <div class="form--group">
                            <asp:DropDownList ID="ddlDestination" runat="server" CssClass="form--control select2">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-4 col-lg-3">
                        <div class="form--group">
                            <asp:TextBox ID="txtJourneyDate" TextMode="Date" runat="server" CssClass="form--control date-range"
                                Placeholder="Date of Journey"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-6 col-lg-3">
                        <div class="form--group">
                            <asp:Button ID="btnFindTickets" OnClick="btnFindTickets_Click" runat="server" CssClass="btn btn--base w-100"
                                Text="Search Trains" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <div class="container mt-4">
        <div class="row">
            <!-- Sidebar Filters -->
            <div class="col-lg-2">
                <!-- RESET FILTERS BUTTON - MOVED TO TOP -->
                <div class="filter-item mb-3">
                    <asp:Button ID="btnResetFilters" runat="server" CssClass="btn btn-danger w-100" 
                        Text="Reset All Filters" OnClick="btnResetFilters_Click" />
                </div>

                <!-- Schedule Filter -->
                <div class="filter-item">
                    <h5 class="title mb-3" style="font-weight: bold;">
                        <i class="las la-clock"></i> Departure Time
                    </h5>
                    <div class="schedule-list d-flex flex-column gap-2">
                        <asp:Repeater ID="rptSchedules" runat="server">
                            <ItemTemplate>
                                <div class="schedule-card p-3 text-center" onclick="toggleCheckbox(this)">
                                    <i class="las la-clock fa-2x mb-2"></i>
                                    <h6 class="mb-1">
                                        <%# string.Format("{0:hh\\:mm} - {1:hh\\:mm}", Eval("DepartureTime"), Eval("ArrivalTime")) %>
                                    </h6>
                                    <asp:HiddenField ID="hdnScheduleId" runat="server" Value='<%# Eval("ScheduleId") %>' />
                                    <asp:CheckBox ID="chkSchedule" runat="server" CssClass="d-none"
                                        AutoPostBack="true" OnCheckedChanged="chkSchedule_CheckedChanged" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <!-- Class Filter -->
                <%--<div class="filter-item mt-3">
                    <h5 class="title mb-3" style="font-weight: bold;">
                        <i class="las la-layer-group"></i> Train Class
                    </h5>
                    <div class="class-filter-list d-flex flex-column gap-2">
                        <div class="class-filter-item p-2">
                            <asp:CheckBox ID="chkLuxury" runat="server" AutoPostBack="true" OnCheckedChanged="chkClass_CheckedChanged" />
                            <label>
                                <i class="las la-gem"></i> Luxury Class
                            </label>
                        </div>
                        <div class="class-filter-item p-2">
                            <asp:CheckBox ID="chkFirstClass" runat="server" AutoPostBack="true" OnCheckedChanged="chkClass_CheckedChanged" />
                            <label>
                                <i class="las la-bed"></i> First Class
                            </label>
                        </div>
                        <div class="class-filter-item p-2">
                            <asp:CheckBox ID="chkSecondClass" runat="server" AutoPostBack="true" OnCheckedChanged="chkClass_CheckedChanged" />
                            <label>
                                <i class="las la-chair"></i> Second Class
                            </label>
                        </div>
                    </div>
                </div>--%>

                <!-- Price Range Filter -->
               <%-- <div class="filter-item mt-3">
                    <h5 class="title mb-3" style="font-weight: bold;">
                        <i class="las la-tag"></i> Price Range
                    </h5>
                    <div class="price-range">
                        <asp:TextBox ID="txtMinPrice" runat="server" CssClass="form-control mb-2" Placeholder="Min Price" TextMode="Number" />
                        <asp:TextBox ID="txtMaxPrice" runat="server" CssClass="form-control" Placeholder="Max Price" TextMode="Number" />
                        <asp:Button ID="btnApplyPrice" runat="server" CssClass="btn btn--base btn-sm w-100 mt-2" Text="Apply" OnClick="btnApplyPrice_Click" />
                    </div>
                </div>--%>

            </div>

            <!-- Train Listings -->
            <div class="col-lg-10">
                <div class="ticket-wrapper">
                    <!-- LEVEL 1: ROUTES -->
                    <asp:Repeater ID="rptRoutes" runat="server" OnItemDataBound="rptRoutes_ItemDataBound">
                        <ItemTemplate>
                            <!-- Route Header (Expandable) -->
                            <div class="route-header mb-3" data-route-index="<%# Container.ItemIndex %>">
                                <div class="route-header-content">
                                    <div class="route-info">
                                        <span class="expand-icon">
                                            <i class="las la-plus-circle"></i>
                                        </span>
                                        <h4 class="route-name">
                                            <i class="las la-route me-2"></i>
                                            <%# Eval("RouteName") %>
                                        </h4>
                                    </div>
                                    <div class="route-meta">
                                        <span class="badge bg-primary">
                                            <i class="las la-train me-1"></i>
                                            <%# Eval("TrainCount") %> Trains Available
                                        </span>
                                        <span class="route-path">
                                            <i class="las la-map-marker-alt"></i>
                                            <%# Eval("StartStation") %> 
                                            <i class="las la-arrow-right mx-2"></i> 
                                            <%# Eval("EndStation") %>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <!-- Route Container (Initially Hidden) -->
                            <div class="route-container" data-route-index="<%# Container.ItemIndex %>" style="display: none;">
                                <!-- LEVEL 2: TRAINS -->
                                <asp:Repeater ID="rptTrains" runat="server" OnItemDataBound="rptTrains_ItemDataBound">
                                    <ItemTemplate>
                                        <!-- Train Header (Expandable) -->
                                        <div class="train-header mb-3" data-train-index="<%# Container.ItemIndex %>">
                                            <div class="train-header-content">
                                                <div class="train-info">
                                                    <span class="expand-icon-train">
                                                        <i class="las la-plus-circle"></i>
                                                    </span>
                                                    <div>
                                                        <h5 class="train-name">
                                                            <i class="las la-train me-2"></i>
                                                            <%# Eval("TrainName") %>
                                                        </h5>
                                                        <span class="train-number">Train No: <%# Eval("TrainNumber") %></span>
                                                    </div>
                                                </div>
                                                <div class="train-meta">
                                                    <span class="train-time">
                                                        <i class="las la-clock"></i>
                                                        <%# Eval("DepartureTime") %> - <%# Eval("ArrivalTime") %>
                                                    </span>
                                                    <span class="train-duration">
                                                        <i class="las la-hourglass-half"></i>
                                                        <%# Eval("Duration") %>
                                                    </span>
                                                    <span class="train-off-days">
                                                        <i class="las la-calendar-times me-1"></i>
                                                        <span class="off-days-badge">
                                                            <%# GetDayNames(Eval("DayOff")) %>
                                                        </span>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Train Container (Initially Hidden) -->
                                        <div class="train-container" data-train-index="<%# Container.ItemIndex %>" style="display: none;">
                                            <!-- LEVEL 3: COACH TYPES -->
                                            <asp:Repeater ID="rptCoachTypes" runat="server" OnItemDataBound="rptCoachTypes_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="coach-type-item mb-3 p-3 border rounded">
                                                        <div class="coach-type-header">
                                                            <div class="class-badge <%# GetClassBadgeColor(Eval("CoachType")?.ToString()) %>">
                                                                <%# GetClassIcon(Eval("CoachType")?.ToString()) %>
                                                                <%# Eval("CoachType") %>
                                                            </div>
                                                            <div class="coach-type-meta">
                                                                <span class="me-3">
                                                                    <i class="las la-subway"></i>
                                                                    <%# Eval("NoOfCoaches") %> Coaches
                                                                </span>
                                                                <span class="me-3">
                                                                    <i class="las la-chair"></i>
                                                                    <%# Eval("NoOfSeats") %> Seats/Coach
                                                                </span>
                                                                <span class="text-success fw-bold fs-4">
                                                                    <i class="las la-tag"></i>
                                                                    CDF <%# String.Format("{0:N2}", Convert.ToDecimal(Eval("Price") ?? 0)) %>
                                                                </span>
                                                            </div>
                                                        </div>

                                                        <!-- Facilities Display -->
                                                        <div class="coach-facilities mt-2 mb-3">
                                                            <strong>Facilities - </strong>
                                                            <%# FormatFacilities(Eval("Facilities")) %>
                                                        </div>

                                                        <!-- LEVEL 4: COACHES GRID -->
                                                        <div class="coaches-grid">
                                                            <asp:Repeater ID="rptCoaches" runat="server" OnItemDataBound="rptCoaches_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <div class="coach-card">
                                                                        <div class="coach-icon">
                                                                            <i class="las la-subway"></i>
                                                                        </div>
                                                                        <h6 class="coach-number"><%# Eval("CoachNumber") %></h6>
                                                                        <asp:HyperLink ID="lnkSelectSeat" runat="server" CssClass="btn btn--base btn-sm">
                                                                            Select Seats
                                                                        </asp:HyperLink>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function toggleCheckbox(card) {
            var checkbox = card.querySelector('input[type="checkbox"]');
            if (checkbox) {
                checkbox.checked = !checkbox.checked;
                __doPostBack(checkbox.name, '');
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    
    <style>
        :root {
            --primary: #a5880b;
            --primary-dark: #af902b;
            --secondary: #b3d34c;
            --luxury: #d4af37;
            --first-class: #9c27b0;
            --second-class: #00bcd4;
            --danger: #ef4444;
            --dark: #1f2937;
            --gray-100: #f3f4f6;
            --gray-200: #e5e7eb;
            --gray-600: #4b5563;
        }

        /* Search Section Styles */
        .train-search-header {
           background: #47a17b4a;
            padding: 30px;
            border-radius: 20px;
           
        }
        body {
    font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
    font-size: 14px;
    line-height: 1.42857143;
    color: #073c15;
    background-color: #ecfdf7;
}
        .btn--base, .btn--base:focus-visible {
    color: #fff;
   background: #0c643f;
    border-color: #dde2e7;
    border: 0;
    border-radius: 14px;
}
        .btn:hover{
           box-shadow: 0 4px 6px rgb(24 182 134 / 0.99);
        }
        /* Schedule Styles */
        .schedule-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        
.filter-item:last-child {
    border-bottom: 0;
    border-radius: 0 0 5px 5px;
    margin-bottom: 32px;
}

        .schedule-card {
            border-radius: 10px;
             background-color: #e6edeaf5;
            transition: 0.3s;
            cursor: pointer;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            text-align: center;
        }
        button, input, optgroup, select, textarea {
    color: #0ab778;
    font: inherit;
    margin: 0;
}
        h6{
            color: #0f7241;

        }
        .schedule-card:hover {
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
             background-color: #2d9b8b8a; 
        }

        .schedule-card i.las.la-clock {
            color: #097e59;
        }

        .schedule-card h6 {
            font-weight: 600;
            margin-bottom: 0;
        }

        /* Filter Items */
        .filter-item {
            background: #47a17b;
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
        }

        .filter-item .title {
            font-weight: 700;
            color:#ffffff;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid var(--gray-200);
        }

        /* Class Filter */
        .class-filter-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .class-filter-item {
            background: #47a17b;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .class-filter-item:hover {
            background: #47a17b;
        }

        .class-filter-item label {
            cursor: pointer;
            margin: 0;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        /*.header-top {*/
           /* background: #47a17b;
    padding: 40px !important;*/
           /*padding: 5px 0;
    border-bottom: 1px solid rgba(27, 39, 61, 0.1);
    font-size: 14px;
        }*/

        /* LEVEL 1: Route Header Styles */
        .route-header {
             background: #47a17b;
            border-radius: 10px;
            padding: 20px;
            cursor: pointer;
            transition: all 0.3s ease;
           box-shadow: 0 4px 6px rgb(83 129 54 / 48%);
        }

        .route-header:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 6px rgb(147 229 156 / 0.99);
        }

        .route-header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
        }

        .route-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .expand-icon {
            font-size: 24px;
            color: #ffffff;
            transition: transform 0.3s ease;
        }

        .route-header.expanded .expand-icon i {
            transform: rotate(45deg);
        }

        .route-name {
            margin: 0;
            color: #ffffff;
            font-size: 22px;
            font-weight: 600;
        }

        .route-meta {
            display: flex;
            align-items: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .route-meta .badge {
            font-size: 14px;
            padding: 8px 15px;
        }

        .route-path {
            color: #ffffff;
            font-size: 15px;
            font-weight: 500;
        }

        /* Route Container */
        .route-container {
            margin-left: 20px;
            margin-top: 15px;
            padding-left: 20px;
           border-left: 3px solid #7bd5b7;
        }

        /* LEVEL 2: Train Header Styles */
        .train-header {
             background: #47a17b;
            border-radius: 10px;
            padding: 15px;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 3px 5px rgba(0, 0, 0, 0.1);
        }

        .train-header:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 10px rgba(0, 0, 0, 0.15);
        }

        .train-header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
        }
        .bg-primary {
    --bs-bg-opacity: 1;
    background: #47a17b;
}
        .train-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .expand-icon-train {
            font-size: 20px;
            color: #ffffff;
            transition: transform 0.3s ease;
        }

        .train-header.expanded .expand-icon-train i {
            transform: rotate(45deg);
        }

        .train-name {
            margin: 0;
            color: #ffffff;
            font-size: 18px;
            font-weight: 600;
        }

        .train-number {
            display: block;
            color: #ffffff;
            font-size: 13px;
            margin-top: 3px;
        }

        .train-meta {
            display: flex;
            align-items: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        .train-time,
        .train-duration,
        .train-off-days {
            color: #ffffff;
            font-size: 14px;
            font-weight: 500;
        }

        .train-off-days {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .off-days-badge {
            
             background-color: #58b584;
            color: #ffffff;
            padding: 4px 10px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13px;
        }

        /* Train Container */
        .train-container {
            margin-left: 20px;
            margin-top: 10px;
            padding-left: 15px;
            border-left: 3px solid #10b981;
        }

        /* Coach Type Item Styles */
        .coach-type-item {
             background: #47a17b61;
            transition: all 0.3s ease;
            box-shadow: 5px 5px 4px rgb(86 159 126);
        }

        .coach-type-item:hover {
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
        }

        .coach-type-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 10px;
        }

        .coach-type-meta {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
        }

        .class-badge {
            padding: 8px 15px;
            border-radius: 20px;
            font-weight: 700;
            font-size: 13px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .class-badge i {
            font-size: 16px;
        }

        .class-badge.luxury-badge {
            background: #eef1ec;
    color: #dd7c26;
        }

        .class-badge.first-class-badge {
            background: #eef1ec;
            color:  #dd4126;
        }

        .class-badge.second-class-badge {
                background:#eef1ec;
            color: #41a5c5;
        }

        .coach-facilities {
            background: #47a17b;
            padding: 10px;
            border-radius: 8px;
        }

        /* LEVEL 4: Coaches Grid */
        .coaches-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .coach-card {
            background: #47a17b;
            /*border: 2px solid var(--gray-200);*/
            border-radius: 10px;
            padding: 15px;
            text-align: center;
            transition: all 0.3s ease;
        }

        .coach-card:hover {
            border-color: #ffffff;
            transform: translateY(-3px);
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
        }

        .coach-icon {
            font-size: 40px;
            color:#ffffff;
            margin-bottom: 10px;
        }

        .coach-number {
            font-weight: 700;
            color: #ffffff;
            margin-bottom: 10px;
        }
        .btn-danger {
   color:#0c724d;
     background-color: #fafffe;
    border-color: #dde2e7;
    border-radius: 24px;
}
        .bg-primary {
    --bs-bg-opacity: 1;
    background-color: rgb(45 151 110 / 83%) !important;
}
        .text-success {
    color: #fbfffb !important;
    background: #2d855ddb;
    padding: 7px;
    border-radius: 8px;
}

        .btn:hover{
             background-color: #2d9b8b8a;
            border-color: #dde2e7;
            box-shadow: #39742e;
        }
        /* Responsive Design */
        @media (max-width: 768px) {
            .route-header-content,
            .train-header-content,
            .coach-type-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .route-container,
            .train-container {
                margin-left: 0;
                padding-left: 10px;
            }

            .coaches-grid {
                grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
            }

            .route-meta {
                width: 100%;
                flex-direction: column;
                align-items: flex-start;
            }
        }

        /* Animation */
        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .route-container[style*="display: block"],
        .train-container[style*="display: block"] {
            animation: slideDown 0.3s ease;
        }

        .ticket-form {
            background: #97cdb700;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            console.log('Page loaded - attaching click handlers');

            // Toggle ROUTE expansion
            $(document).on('click', '.route-header', function (e) {
                e.preventDefault();
                e.stopPropagation();

                const routeIndex = $(this).data('route-index');
                const $routeContainer = $(`.route-container[data-route-index="${routeIndex}"]`);
                const $icon = $(this).find('.expand-icon i');

                console.log('Route clicked:', routeIndex);

                $(this).toggleClass('expanded');

                if ($(this).hasClass('expanded')) {
                    $icon.removeClass('la-plus-circle').addClass('la-minus-circle');
                    $routeContainer.slideDown(400);
                } else {
                    $icon.removeClass('la-minus-circle').addClass('la-plus-circle');
                    $routeContainer.slideUp(400);
                }
            });

            // Toggle TRAIN expansion
            $(document).on('click', '.train-header', function (e) {
                e.preventDefault();
                e.stopPropagation();

                const $this = $(this);
                const trainIndex = $this.data('train-index');

                // Find the train container within the same parent route container
                const $routeContainer = $this.closest('.route-container');
                const $trainContainer = $routeContainer.find(`.train-container[data-train-index="${trainIndex}"]`);
                const $icon = $this.find('.expand-icon-train i');

                console.log('Train clicked:', trainIndex);

                $this.toggleClass('expanded');

                if ($this.hasClass('expanded')) {
                    $icon.removeClass('la-plus-circle').addClass('la-minus-circle');
                    $trainContainer.slideDown(400);
                } else {
                    $icon.removeClass('la-minus-circle').addClass('la-plus-circle');
                    $trainContainer.slideUp(400);
                }
            });

            // Initialize select2 and date picker
            $('.select2').select2();

            // Set minimum date
            const today = new Date().toISOString().split('T')[0];
            $('#<%= txtJourneyDate.ClientID %>').attr('min', today);
        });
    </script>

</asp:Content>
