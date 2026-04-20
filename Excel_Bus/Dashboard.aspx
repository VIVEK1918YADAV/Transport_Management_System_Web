<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ExcelBus.Dashboard" Async="true"  %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelBus - Dashboard</title>
    <meta name="title" content="ExcelBus - Dashboard">
    <link rel="shortcut icon" href="img/excel_bus_logo.png" type="image/x-icon">

    <!-- BootStrap Link -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- Icon Link -->
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/global-line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />
   

    <!-- Custom Link -->
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />

    <!-- iziToast -->
    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />

    <style>
        /* Trips Dropdown Styles */
        .menu-dropdown {
            position: relative;
        }

            .menu-dropdown .sub-menu {
                display: none;
                list-style: none;
                padding-left: 0;
                margin: 5px 0 0 0;
                background: #fff;
                position: absolute;
                left: 0;
                min-width: 220px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                z-index: 1000;
                border-radius: 4px;
                max-height: 400px;
                overflow-y: auto;
            }

                .menu-dropdown .sub-menu li {
                    border-bottom: 1px solid #f0f0f0;
                }

                    .menu-dropdown .sub-menu li:last-child {
                        border-bottom: none;
                    }

                    .menu-dropdown .sub-menu li a {
                        display: block;
                        padding: 10px 15px;
                        text-decoration: none;
                        color: #333;
                        transition: all 0.3s ease;
                        font-size: 14px;
                    }

                        .menu-dropdown .sub-menu li a:hover {
                            background-color: #0E9E4D;
                            color: #fff;
                            padding-left: 20px;
                        }

            .menu-dropdown:hover .sub-menu,
            .menu-dropdown:focus-within .sub-menu {
                display: block;
            }

        /* Scrollbar styling */
        .sub-menu::-webkit-scrollbar {
            width: 6px;
        }

        .sub-menu::-webkit-scrollbar-track {
            background: #f1f1f1;
            border-radius: 3px;
        }

        .sub-menu::-webkit-scrollbar-thumb {
            background: #888;
            border-radius: 3px;
        }

            .sub-menu::-webkit-scrollbar-thumb:hover {
                background: #555;
            }


/*========== background image ==========*/
.inner-banner{
    width:100%;
    padding: 70px 0;
    background-image:url('img/bg_bus_img.jpg');
    background-position:center;
    background-repeat:no-repeat;

}


    </style>
</head>

<body>
    <form id="form1" runat="server">
        <!-- Google Translate -->
        <div id="google_translate_element"></div>

        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en',
                    layout: google.translate.TranslateElement.InlineLayout.HORIZONTAL,
                    includedLanguages: 'en,fr,hi,de,es'
                }, 'google_translate_element');
            }
        </script>

        <script type="text/javascript"
            src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit">
        </script>

        <div class="overlay"></div>

        <!-- Preloader -->
        <div class="preloader">
            <div class="loader-wrapper">
                <div class="truck-wrapper">
                    <div class="preloader-content"></div>
                    <div class="truck">
                        <div class="truck-container"></div>
                        <div class="glases"></div>
                        <div class="bonet"></div>
                        <div class="base"></div>
                        <div class="base-aux"></div>
                        <div class="wheel-back"></div>
                        <div class="wheel-front"></div>
                        <div class="smoke"></div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Header Section -->
        <div class="header-top">
            <div class="container">
                <div class="header-top-area">
                    <ul class="left-content">
                        <li>
                            <i class="las la-phone"></i>
                            <a href="tel:+91 98717 71849">+91 98717 71849</a>
                        </li>
                        <li>
                            <i class="las la-envelope-open"></i>
                            <a href="mailto:rajesh.paul@excelgeomatics.com">rajesh.paul@excelgeomatics.com</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="header-bottom">
            <div class="container">
                <div class="header-bottom-area">
                    <div class="logo">
                        <a href="~/Default.aspx" runat="server">
                             <img src="img/excel_bus_logo.png" alt="Logo">
                        </a>
                       
                    </div>
                    <ul class="menu">
                        <li>
                            <a href="~/Dashboard.aspx" runat="server">Dashboard</a>
                        </li>
                       <%-- <li class="menu-dropdown">
                            <a href="javascript:void(0)">Trips</a>
                            <ul class="sub-menu" id="tripsMenu" runat="server">
                                <!-- Trips will be loaded dynamically -->
                            </ul>
                        </li>--%>
                        
                        <li>
                            <a href="javascript:void(0)">Booking</a>
                            <ul class="sub-menu">
                                <li><a href="ticket.aspx">Buy Ticket</a></li>
                                <li><a href="booked_ticket.aspx">Booking History</a></li>
                            </ul>
                        </li>
                        <li>
                            <a href="javascript:void(0)">Profile</a>
                            <ul class="sub-menu">
                                <li><a href="~/MyBookings.aspx" runat="server">MyBookings</a></li>
                                <li><a href="#">Wallet</a></li>
                                <li><a href="Profile.aspx">Profile</a></li>
                                <li><a href="~/modifyPassword.aspx" runat="server">Change Password</a></li>
                                <li>
                                    <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click">Logout</asp:LinkButton>
                                </li>
                            </ul>
                        </li>
                    </ul>
                    <div class="d-flex flex-wrap algin-items-center">
                        <a href="ticket.aspx" class="cmn--btn btn--sm">Buy Tickets</a>
                    </div>
                </div>
            </div>
        </div>

        <!-- Banner Section -->
      <section class="inner-banner bg_img">
           <!-- <section class="inner-banner">
    <img src="img/bg_bus_img.jpg" alt="Banner Image" /> -->


            <div class="container">
                <div class="inner-banner-content">
                    <h2 class="title">Dashboard</h2>
                </div>
            </div>
        </section>

        <!-- Dashboard Section -->
        <section class="dashboard-section padding-top padding-bottom">
            <div class="container">
                <div class="dashboard-wrapper">
                    <div class="row pb-60 gy-4 justify-content-center">
                        <div class="col-lg-4 col-md-6 col-sm-10">
                            <div class="dashboard-widget">
                                <div class="dashboard-widget__content">
                                    <p>Total Booked Ticket</p>
                                    <h3 class="title">
                                        <asp:Literal ID="litTotalBooked" runat="server">0</asp:Literal></h3>
                                </div>
                                <div class="dashboard-widget__icon">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-10">
                            <div class="dashboard-widget">
                                <div class="dashboard-widget__content">
                                    <p>Total Rejected Ticket</p>
                                    <h3 class="title">
                                        <asp:Literal ID="litTotalRejected" runat="server">0</asp:Literal></h3>
                                </div>
                                <div class="dashboard-widget__icon">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-10">
                            <div class="dashboard-widget">
                                <div class="dashboard-widget__content">
                                    <p>Total Pending Ticket</p>
                                    <h3 class="title">
                                        <asp:Literal ID="litTotalPending" runat="server">0</asp:Literal></h3>
                                </div>
                                <div class="dashboard-widget__icon">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-10">
                            <div class="dashboard-widget border-primary">
                                <div class="dashboard-widget__content">
                                    <p>Total Cancelled Ticket</p>
                                    <h3 class="title">
                                        <asp:Literal ID="litTotalCancelled" runat="server">0</asp:Literal></h3>
                                </div>
                                <div class="dashboard-widget__icon bg-primary">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-10">
                            <div class="dashboard-widget border-dark">
                                <div class="dashboard-widget__content">
                                    <p>Total Postponed Ticket</p>
                                    <h3 class="title">
                                        <asp:Literal ID="litTotalPostponed" runat="server">0</asp:Literal></h3>
                                </div>
                                <div class="dashboard-widget__icon bg-dark">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="booking-table-wrapper">
                        <asp:Repeater ID="rptBookings" runat="server" >
                            <HeaderTemplate>
                                <table class="booking-table">
                                    <thead>
                                        <tr>
                                            <th>PNR Number</th>
                                            <th>AC / Non-AC</th>
                                            <th>Starting Point</th>
                                            <th>Dropping Point</th>
                                            <th>Journey Date</th>
                                          <%--  <th>Pickup Time</th>--%>
                                            <th>Booked Seats</th>
                                            <th>Bags Count</th>
                                            <th>Bags Charge</th>
                                            <th>Status</th>
                                          <%--  <th>CheckIn</th>--%>
                                            <th>Postponed</th>
                                            <th>Fare</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="ticket-no" data-label="PNR Number"><%# Eval("PnrNumber") %></td>
                                    <td data-label="AC / Non-AC">AC</td>
                                    <td class="pickup" data-label="Starting Point"><%# Eval("PickupName") %></td>
                                    <td class="drop" data-label="Dropping Point"><%# Eval("DropName") %></td>
                                    <td class="date" data-label="Journey Date"><%# Eval("DateOfJourneyFormatted") %></td>
                                   <%-- <td class="time" data-label="Pickup Time"><%# Eval("PickupTime") %></td>--%>
                                    <td class="seats" data-label="Booked Seats"><%# Eval("SeatsDisplay") %></td>
                                    <td class="b_count" data-label="Bags Count"><%# Eval("BagsCount") %></td>
                                    <td class="b_price" data-label="Bags Price"><%# Eval("BagsCharge") %></td>
                                    <td data-label="Status">
                                        <%--<%# GetStatusBadge(Convert.ToInt32(Eval("Status"))) %>--%>
                                        <%# GetStatusBadge(Eval("Status")) %>
                                    </td>
                                    <td data-label="Postponed">
                                        <%# GetPostponedBadge(Convert.ToInt32(Eval("PostponeCount"))) %>
                                    </td>
                                    <td class="fare" data-label="Fare"><%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %> CDF</td>
                                    <td class="action" data-label="Action">
                                        <%# GetActionButton(
                        Eval("PnrNumber"), 
                        Eval("DateOfJourneyFormatted"),
                        Eval("PickupName"),
                        Eval("DropName"),
                        Eval("SubTotal"),
                        Eval("Status")
                    ) %>
                                    </td>
                                    <%--<td data-label="CheckIn">
                                        <!-- Added checkbox for CheckIn -->
                                         <input type="checkbox" name="CheckIn_<%# Eval("PnrNumber") %>" <%# (Eval("CheckIn") != null && Convert.ToInt32(Eval("CheckIn")) == 1 ? "checked" : "") %> />
                                    </td>--%>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>

                        <!-- No Data Message -->
                        <div id="noDataMessage" style="display: none; text-align: center; padding: 40px;">
                            <i class="las la-inbox" style="font-size: 48px; color: #ccc;"></i>
                            <p style="color: #666; margin-top: 10px;">No booking records found.</p>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Info Modal -->
        <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Ticket Booking History</h5>
                        <button type="button" class="w-auto btn--close" data-bs-dismiss="modal"><i class="las la-times"></i></button>
                    </div>
                    <div class="modal-body">
                        <!-- Content will be loaded dynamically -->
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn--danger w-auto btn--sm px-3" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <section class="footer-section">
            <div class="footer-top">
                <div class="container">
                    <div class="row footer-wrapper gy-sm-5 gy-4">
                        <div class="col-xl-4 col-lg-3 col-md-6 col-sm-6">
                            <div class="footer-widget">
                                <div class="logo">
                                    <img src="img/excel_bus_logo.png" alt="Logo">
                                </div>
                               
                                <p>Excel Bus - Your trusted bus booking partner</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <a href="javascript::void()" class="scrollToTop"><i class="las la-chevron-up"></i></a>
    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>
    <script src="js/iziToast.min.js"></script>

   
    <script>
        "use strict";

        // Notification colors and icons
        const colors = {
            success: '#28c76f',
            error: '#eb2222',
            warning: '#ff9f43',
            info: '#1e9ff2',
        }

        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-times-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-exclamation-circle',
        }

        const triggerToaster = (status, message) => {
            iziToast[status]({
                title: status.charAt(0).toUpperCase() + status.slice(1),
                message: message,
                position: "topRight",
                backgroundColor: '#fff',
                icon: icons[status],
                iconColor: colors[status],
                progressBarColor: colors[status],
                titleSize: '1rem',
                messageSize: '1rem',
                titleColor: '#474747',
                messageColor: '#a2a2a2',
                transitionIn: 'bounceInLeft'
            });
        }

        function notify(status, message) {
            if (typeof message == 'string') {
                triggerToaster(status, message);
            } else {
                $.each(message, (i, val) => triggerToaster(status, val));
            }
        }

        // Check if table has data on page load
        $(document).ready(function () {
            console.log('Document ready - checking for booking data');

            var tableRows = $('.booking-table tbody tr').length;
            console.log('Number of table rows:', tableRows);

            if (tableRows === 0) {
                console.log('No data - showing message');
                $('#noDataMessage').show();
                $('.booking-table').hide();
            } else {
                console.log('Data found - showing table');
                $('#noDataMessage').hide();
                $('.booking-table').show();
            }
        });

        // Modal handler for ticket information
        $(document).on('click', '.checkinfo', function (e) {
            e.preventDefault();
            console.log('Info button clicked');

            var encodedInfo = $(this).attr('data-info');
            console.log('Encoded data:', encodedInfo);

            if (!encodedInfo) {
                console.error('No data-info attribute found');
                notify('error', 'Unable to load ticket information');
                return;
            }

            try {
                // Decode HTML entities
                var textarea = document.createElement('textarea');
                textarea.innerHTML = encodedInfo;
                var decodedInfo = textarea.value;

                console.log('Decoded data:', decodedInfo);

                // Parse JSON
                var info = JSON.parse(decodedInfo);
                console.log('Parsed info:', info);

                // Get modal
                var modal = $('#infoModal');

                // Create status badge
                var statusBadge = getStatusBadge(info.statusText);

                // Build modal content
                var html = `
                    <div class="ticket-info-container">
                        <div class="info-row">
                            <span class="info-label">Journey Date</span>
                            <span class="info-value">${info.dateOfJourney}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">PNR Number</span>
                            <span class="info-value">${info.pnrNumber}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">Route</span>
                            <span class="info-value">${info.pickup} to ${info.drop}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">Fare</span>
                            <span class="info-value">${parseFloat(info.subTotal).toFixed(2)} CDF</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">Status</span>
                            <span class="info-value">${statusBadge}</span>
                        </div>
                    </div>
                `;

                modal.find('.modal-body').html(html);

            } catch (error) {
                console.error('Error parsing ticket info:', error);
                console.error('Error details:', error.message);
                notify('error', 'Error loading ticket information');
                $('#infoModal .modal-body').html(
                    '<div class="alert alert-danger">Error loading ticket information. Please try again.</div>'
                );
            }
        });

        // Helper function to create status badge
        function getStatusBadge(statusText) {
            var badgeClass = '';
            switch (statusText) {
                case 'Booked':
                    badgeClass = 'badge--success';
                    break;
                case 'Pending':
                    badgeClass = 'badge--warning';
                    break;
                case 'Cancelled':
                    badgeClass = 'badge--primary';
                    break;
                case 'Postponed':
                    badgeClass = 'badge--dark';
                    break;
                default:
                    badgeClass = 'badge--danger';
            }
            return `<span class="badge ${badgeClass}">${statusText}</span>`;
        }

        // Add custom styles for modal
        $(document).ready(function () {
            var customStyles = `
                <style>
                    .ticket-info-container {
                        padding: 10px 0;
                    }
                    .info-row {
                        display: flex;
                        justify-content: space-between;
                        align-items: center;
                        padding: 12px 0;
                        border-bottom: 1px solid #f0f0f0;
                    }
                    .info-row:last-child {
                        border-bottom: none;
                    }
                    .info-label {
                        font-weight: 600;
                        color: #333;
                        font-size: 14px;
                    }
                    .info-value {
                        color: #666;
                        font-size: 14px;
                        text-align: right;
                    }
                    .modal-header {
                        background-color: #f8f9fa;
                        border-bottom: 2px solid #0E9E4D;
                    }
                    .modal-title {
                        color: #0E9E4D;
                        font-weight: 600;
                    }
                    .booking-table {
                        width: 100%;
                        margin-top: 20px;
                        border-collapse: collapse;
                    }
                    .booking-table thead {
                        background-color: #0E9E4D;
                        color: white;
                    }
                    .booking-table th {
                        padding: 12px;
                        text-align: left;
                        font-weight: 600;
                        font-size: 14px;
                    }
                    .booking-table td {
                        padding: 12px;
                        border-bottom: 1px solid #f0f0f0;
                        font-size: 14px;
                    }
                    .booking-table tbody tr:hover {
                        background-color: #f8f9fa;
                    }
                    @media (max-width: 768px) {
                        .booking-table {
                            display: block;
                            overflow-x: auto;
                        }
                        .booking-table thead {
                            display: none;
                        }
                        .booking-table tr {
                            display: block;
                            margin-bottom: 15px;
                            border: 1px solid #ddd;
                            border-radius: 8px;
                            padding: 10px;
                        }
                        .booking-table td {
                            display: flex;
                            justify-content: space-between;
                            padding: 8px 0;
                            border: none;
                        }
                        .booking-table td:before {
                            content: attr(data-label);
                            font-weight: 600;
                            color: #333;
                        }
                    }
                </style>
            `;
            $('head').append(customStyles);
        });
    </script>
</body>
</html>
