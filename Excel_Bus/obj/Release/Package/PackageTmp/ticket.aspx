<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ticket.aspx.cs" Inherits="Excel_Bus.ticket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Buy Tickets
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdn.lineawesome.com/1.3.0/line-awesome/css/line-awesome.min.css">
    <div class="ticket-search-bar bg_img padding-top" style='background: url("img/bg_bus_img.jpg") left center;'>
        <div class="container">
            <div class="bus-search-header">
                <asp:Panel ID="pnlTicketSearch" runat="server" CssClass="ticket-form ticket-form-two row g-3 justify-content-center">
                    <div class="col-md-4 col-lg-3">
                        <div class="form--group">
                            <asp:DropDownList ID="ddlPickup" runat="server" CssClass="form--control select2">
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
                                Text="Find Tickets" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <div class="container mt-4">
        <div class="row">
            <div class="col-lg-2">
                <div class="filter-item">
                    <h5 class="title mb-3" style="font-weight: bold;">Schedules</h5>
                    <div class="schedule-list d-flex flex-column gap-2">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <div class="schedule-card p-3 text-center" onclick="toggleCheckbox(this)">
                                    <i class="fas fa-clock fa-2x mb-2"></i>
                                    <h6 class="mb-1">
                                        <%# string.Format("{0:hh\\:mm} - {1:hh\\:mm}", Eval("StartFrom"), Eval("EndAt")) %>
                                    </h6>
                                    <asp:HiddenField ID="hdnScheduleId" runat="server" Value='<%# Eval("Id") %>' />
                                    <asp:CheckBox ID="chkSchedule" runat="server" CssClass="d-none"
                                        AutoPostBack="true" OnCheckedChanged="chkSchedule_CheckedChanged" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <div class="col-lg-10">
                <div class="ticket-wrapper">
                    <asp:Repeater ID="rptRoutes" runat="server" OnItemDataBound="rptRoutes_ItemDataBound">
                        <ItemTemplate>
                            <!-- Route Header (Expandable) -->
                            <div class="route-header mb-3" data-route-index="<%# Container.ItemIndex %>">
                                <div class="route-header-content">
                                    <div class="route-info">
                                        <span class="expand-icon">
                                            <i class="fas fa-plus-circle"></i>
                                        </span>
                                        <h4 class="route-name">
                                            <i class="fas fa-route me-2"></i>
                                            <%# Eval("RouteName") %>
                                        </h4>
                                    </div>
                                    <div class="route-meta">
                                        <span class="badge bg-primary">
                                            <i class="fas fa-bus me-1"></i>
                                            <%# Eval("BusCount") %> Buses Available
                                        </span>
                                        <span class="route-path">
                                            <i class="fas fa-map-marker-alt me-1"></i>
                                            <%# Eval("StartFrom") %> <i class="fas fa-arrow-right mx-2"></i> <%# Eval("EndTo") %>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <!-- Bus Details Container (Initially Hidden) -->
                            <div class="bus-details-container" data-route-index="<%# Container.ItemIndex %>" style="display: none;">
                                <asp:Repeater ID="rptBuses" runat="server" OnItemDataBound="rptBuses_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="ticket-item mb-3 p-3 border rounded">
                                            <div class="ticket-item-inner">
                                                <h5 class="bus-name" style="font-weight: bold;">
                                                    <i class="fas fa-bus me-2"></i><%# Eval("VehicleNickname") %>
                                                </h5>
                                                <span class="bus-info">
                                                    <i class="fas fa-chair me-1"></i>Seat Layout - <%# Eval("SeatLayout") %>
                                                </span>
                                                <span class="ratting">
                                                    <i class="fas fa-bus me-1"></i><%# Eval("FleetTypeName") %>
                                                </span>
                                            </div>
                                            
                                            <div class="ticket-item-inner travel-time">
                                                <div class="bus-time">
                                                    <p class="time"><%# Eval("StartTime") %></p>
                                                    <p class="place"><%# Eval("StartFrom") %></p>
                                                </div>
                                                <div class="bus-time">
                                                    <i class="fas fa-arrow-right me-2"></i>
                                                    <p><%# Eval("duration") %></p>
                                                </div>
                                                <div class="bus-time">
                                                    <p class="time"><%# Eval("EndTime") %></p>
                                                    <p class="place"><%# Eval("EndTo") %></p>
                                                </div>
                                            </div>

                                            <div class="ticket-item-inner book-ticket">
                                                <span class="text-primary fw-bold fs-2">
                                                    <%# Eval("Price") != DBNull.Value ? "CDF " + String.Format("{0:N2}", Eval("Price")) : "CDF 0.00" %>
                                                </span>
                                                <br/>
                                                <div class="mb-2">
                                                    Off Days: 
                                                    <span style="color: #000000; background-color: #FFFF00; padding: 2px 6px; border-radius: 3px;">
                                                        <%# GetDayNames(Eval("DayOff")) %>
                                                    </span>
                                                    <br />
                                                </div>
                                                <asp:HyperLink ID="lnkSelectSeat" runat="server" CssClass="btn btn--base" Text="Select Seat" />
                                            </div>
                                            
                                            <div class="ticket-item-footer">
                                                <div class="d-flex content-justify-center">
                                                    <span>
                                                        <strong>Facilities - </strong>
                                                        <%# FormatFacilities(Eval("Facilities")) %>
                                                    </span>
                                                </div>
                                            </div>
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
        /* Schedule Styles */
        .schedule-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .schedule-card {
            border-radius: 10px;
            background-color: #f9f9f9;
            transition: 0.3s;
            cursor: pointer;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            text-align: center;
        }

        .schedule-card:hover {
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
            background-color: #e8f0fe;
        }

        .schedule-card i.fas.fa-clock {
            color: #0b89e9;
        }

        .schedule-card h6 {
            font-weight: 600;
            margin-bottom: 0;
        }

        /* Route Header Styles */
        .route-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 10px;
            padding: 20px;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }

        .route-header:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
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

        /* Bus Details Container */
        .bus-details-container {
            margin-left: 20px;
            margin-top: 15px;
            padding-left: 20px;
            border-left: 3px solid #667eea;
        }

        /* Ticket Item Styles */
        .ticket-item {
            background: #ffffff;
            transition: all 0.3s ease;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.08);
        }

        .ticket-item:hover {
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
            transform: translateY(-2px);
        }

        .ticket-item-inner {
            margin-bottom: 15px;
        }

        .bus-name {
            color: #2c3e50;
            margin-bottom: 10px;
        }

        .bus-info, .ratting {
            display: inline-block;
            margin-right: 15px;
            padding: 5px 10px;
            background: #f8f9fa;
            border-radius: 5px;
            font-size: 14px;
        }

        .travel-time {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 8px;
        }

        .bus-time {
            text-align: center;
        }

        .bus-time .time {
            font-size: 18px;
            font-weight: bold;
            color: #2c3e50;
            margin: 0;
        }

        .bus-time .place {
            font-size: 14px;
            color: #7f8c8d;
            margin: 5px 0 0 0;
        }

        .book-ticket {
            text-align: center;
            padding: 15px;
        }

        .ticket-item-footer {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .route-header-content {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .route-meta {
                width: 100%;
                flex-direction: column;
                align-items: flex-start;
            }
            
            .travel-time {
                flex-direction: column;
                gap: 15px;
            }
            
            .bus-details-container {
                margin-left: 0;
                padding-left: 10px;
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

        .bus-details-container[style*="display: block"] {
            animation: slideDown 0.3s ease;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            // Toggle route expansion
            $('.route-header').click(function () {
                const routeIndex = $(this).data('route-index');
                const $busContainer = $(`.bus-details-container[data-route-index="${routeIndex}"]`);
                const $icon = $(this).find('.expand-icon i');

                // Toggle expanded class
                $(this).toggleClass('expanded');

                // Toggle icon and container
                if ($(this).hasClass('expanded')) {
                    $icon.removeClass('fa-plus-circle').addClass('fa-minus-circle');
                    $busContainer.slideDown(400);
                } else {
                    $icon.removeClass('fa-minus-circle').addClass('fa-plus-circle');
                    $busContainer.slideUp(400);
                }
            });

            // Initialize select2 and date picker
            $('.select2').select2();
            $('.date-range').daterangepicker({
                autoUpdateInput: true,
                singleDatePicker: true,
                minDate: new Date()
            });
        });
    </script>

</asp:Content>

