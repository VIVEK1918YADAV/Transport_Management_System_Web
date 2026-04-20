<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_BookedTicket_details.aspx.cs" Inherits="Excel_Bus.Train_BookedTicket_details" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Train Booking Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">



    <style>
        .details-container {
            padding: 0px 0;
            background: #47a17b;
            min-height: 100vh;
        }

        .page-header {
            background: #47a17b;
            color: white;
            padding: 30px 0;
            margin-bottom: 40px;
            box-shadow: 0 4px 12px rgba(201,79,0,0.3);
        }

            .page-header h2 {
                margin: 0;
                font-weight: bold;
                font-size: 26px;
            }

            .page-header p {
                margin: 5px 0 0;
                opacity: 0.85;
                font-size: 14px;
            }

        .details-card {
            background: white;
            border-radius: 14px;
           box-shadow: -1px 0px 13px 12px rgba(0, 0, 0, 0.08);
            overflow: hidden;
            margin-bottom: 24px;
            margin-top: 26px;
        }

        .card-header-custom {
            background: #47a17b;
            color: white;
           padding: 7px 23px;
            justify-content: space-between;
            align-items: center;
            border-top-left-radius: 17px;
    border-top-right-radius: 17px;
        }

        .pnr-info h3 {
            margin: 0 0 5px 0;
            font-size: 12px;
            opacity: 0.85;
            letter-spacing: 1px;
            text-transform: uppercase;
        }

        .pnr-number {
            font-size: 26px;
            font-weight: bold;
            font-family: 'Courier New', monospace;
            letter-spacing: 3px;
        }

        .train-badge {
            background: #47a17b;
            border: 1px solid rgba(255,255,255,0.3);
            color: white;
            padding: 5px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            margin-right: 10px;
        }

        /* Status badge colours */
        .status-badge {
            padding: 8px 20px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            background: white;
        }

        .status-text.status-booked {
            color: #43a047;
        }

        .status-text.status-pending {
            color: #f5a000;
        }

        .status-text.status-completed {
            color: #c94f00;
        }

        .status-text.status-cancelled {
            color: #e53935;
        }

        .status-text.status-default {
            color: #666;
        }

        .card-body-custom {
            padding: 30px;
        }

        .section-title {
            color: #c94f00;
            font-size: 17px;
            font-weight: bold;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #f5a000;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 16px;
            margin-bottom: 30px;
        }

        .info-box {
            background: #74cba638;
            padding: 18px 20px;
            border-radius: 10px;
            border-left: 4px solid #f5a000;
            transition: box-shadow 0.2s;
        }

            .info-box:hover {
               box-shadow: 0 2px 8px rgb(10 52 3 / 68%);
            }

            .info-box.highlight-refund {
                background:#47a17b;
                border-left-color: #f5a000;
            }

            .info-box.highlight-warning {
                background: #47a17b;
                border-left-color: #f5a000;
            }

            .info-box.highlight-train {
                background: #47a17b;
                border-left-color: #f5a000;
            }

        .info-label {
            font-size: 11px;
            color:#011408;
            text-transform: uppercase;
            letter-spacing: 0.7px;
            margin-bottom: 7px;
        }

        .info-value {
            font-size: 15px;
            color: #333;
            font-weight: 600;
        }

            .info-value.refund-amount {
                font-size: 20px;
                color: #134e15;
                font-weight: 700;
            }

        .route-visual {
            background: #47a17b;
            border-radius: 12px;
            padding: 24px 30px;
            margin-bottom: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 20px;
        }

        .station-block {
            text-align: center;
            flex: 1;
        }

        .station-code {
            font-size: 28px;
            font-weight: bold;
            color: #c94f00;
            letter-spacing: 2px;
        }

        .station-name {
            font-size: 13px;
            color: #f7f7f7;
            margin-top: 4px;
        }

        .station-time {
            font-size: 16px;
            font-weight: 600;
            color: #f7f7f7;
            margin-top: 6px;
        }

        .route-line {
            flex: 2;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 6px;
        }

        .route-dots {
            display: flex;
            align-items: center;
            width: 100%;
        }

        .dot {
            width: 12px;
            height: 12px;
            border-radius: 50%;
            background: #47a17b;
            flex-shrink: 0;
        }

        .route-dash {
            flex: 1;
            height: 3px;
            background: #47a17b;
        }

        .train-icon-center {
            font-size: 22px;
            color: #c94f00;
        }

        .duration-text {
            font-size: 12px;
            color: #888;
        }

        /* Passengers Table */
        .passengers-section {
            margin-top: 20px;
        }

        .passengers-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 12px;
            background: white;
            border-radius: 8px;
            overflow: hidden;
        }

            .passengers-table th {
               background: #47a17b;
                color: white;
                padding: 14px 16px;
                text-align: left;
                font-weight: 600;
                font-size: 13px;
            }

            .passengers-table td {
                padding: 14px 16px;
                border-bottom: 1px solid #fde8cc;
                font-size: 14px;
            }

            .passengers-table tr:last-child td {
                border-bottom: none;
            }

            .passengers-table tr:hover {
                background: #47a17b;
            }

            .passengers-table tr.cancelled-row {
                background:#0f4c33;
                opacity: 0.75;
            }

                .passengers-table tr.cancelled-row td {
                    text-decoration: line-through;
                    color: #aaa;
                }

        .class-badge {
            background: #47a17b;
            color: #c94f00;
            padding: 3px 10px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
        }

        /* Cancellation Section */
        .cancellation-section {
            margin-top: 24px;
            padding: 22px 24px;
            background: #47a17b;
            border-radius: 10px;
            border-left: 4px solid #FFC107;
        }

        .cancellation-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 14px;
            background: white;
            border-radius: 8px;
            overflow: hidden;
        }

            .cancellation-table th {
                background: #47a17b;
                color: white;
                padding: 13px 16px;
                text-align: left;
                font-weight: 600;
                font-size: 13px;
            }

            .cancellation-table td {
                padding: 13px 16px;
                border-bottom: 1px solid #e0e0e0;
                font-size: 14px;
            }

        .refund-summary {
            display: grid;
            grid-template-columns: repeat(auto-fit,minmax(180px,1fr));
            gap: 14px;
            margin-top: 18px;
            padding: 18px;
            background: white;
            border-radius: 10px;
        }

        .refund-item {
            text-align: center;
            padding: 14px;
            border-radius: 8px;
            background:#c4dfd4;
        }

        .refund-item-label {
            font-size: 11px;
            color: #888;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-bottom: 7px;
        }

        .refund-item-value {
            font-size: 18px;
            font-weight: bold;
            color: #4CAF50;
        }

        /* Action Buttons */
        .action-buttons {
            display: flex;
            gap: 14px;
            margin-top: 28px;
            flex-wrap: wrap;
        }

        .btn-action {
            flex: 1;
            min-width: 160px;
            padding: 14px 24px;
            font-size: 15px;
            font-weight: 600;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            transition: all .25s;
            text-align: center;
        }

        .btn-primary-action {
           background: #47a17b;
            color: white;
        }

            .btn-primary-action:hover {
                transform: translateY(-2px);
                box-shadow: 0 5px 14px rgba(201,79,0,0.35);
                color: #c94f00;
            }

        .btn-secondary-action {
            background: white;
            color: #c94f00;
            border: 2px solid #c94f00;
        }

            .btn-secondary-action:hover {
                background: #47a17b;
                color: #c94f00;
            }

        .btn-danger-action {
            background: white;
            color: #e53935;
            border: 2px solid #e53935;
        }

            .btn-danger-action:hover {
                background: #47a17b;
                color: white;
            }

        .cancel-button {
            color: #e53935;
            background: transparent;
            border: 2px solid #e53935;
            padding: 5px 13px;
            cursor: pointer;
            border-radius: 5px;
            font-size: 13px;
            transition: all .25s;
        }

            .cancel-button:hover {
                background: #47a17b;
                color: white;
            }

            .cancel-button:disabled {
                opacity: .45;
                cursor: not-allowed;
            }

        @media (max-width:768px) {
            .card-body-custom {
                padding: 18px;
            }

            .info-grid {
                grid-template-columns: 1fr 1fr;
            }

            .action-buttons {
                flex-direction: column;
            }

            .card-header-custom {
                flex-direction: column;
                align-items: flex-start;
                gap: 12px;
            }

            .route-visual {
                flex-direction: column;
                text-align: center;
            }

            .route-dots {
                display: none;
            }
        }
        @media (min-width: 1200px) {
    .container {
        width: 1320px;
    }
}
    </style>

  <%--  <div class="details-container">--%>
        <div class="container">
            <div class="details-card">

                <!-- Card Header: PNR + Status -->
                <div class="card-header-custom">
                      <div style="text-align: center;">
                           <h2><i class="fas fa-train"></i></h2> 
                           <h2>Train Booking Details</h2>
                            <p>View and manage your train reservation</p>
                        
                    <div class="pnr-info">
                        <small>PNR NUMBER</small>
                        <div class="pnr-number">
                            <small><asp:Label ID="lblPNRNumber" runat="server"></asp:Label></small>
                        </div>
                        </div>
                    </div>
                   <%-- <div style="display: flex; align-items: center; gap: 10px; flex-wrap: wrap;">
                        <span class="train-badge"><i class="fas fa-train"></i>TRAIN</span>
                        <span class="status-badge">
                            <asp:Label ID="lblStatus" runat="server"></asp:Label>
                        </span>
                    </div>--%>
                </div>

                <div class="card-body-custom">

                    <!-- Route Visual -->
                    <div class="route-visual">
                        <div class="station-block">
                            <div class="station-code">
                                <asp:Label ID="lblFromCode" runat="server">--</asp:Label>
                            </div>
                            <div class="station-name">
                                <asp:Label ID="lblFromStation" runat="server">Departure Station</asp:Label>
                            </div>
                            <div class="station-time">
                                <asp:Label ID="lblDepartureTime" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="route-line">
                            <div class="route-dots">
                                <div class="dot"></div>
                                <div class="route-dash"></div>
                                <div class="train-icon-center"><i class="fas fa-train"></i></div>
                                <div class="route-dash"></div>
                                <div class="dot"></div>
                            </div>
                            <div class="duration-text">
                                <asp:Label ID="lblDuration" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="station-block">
                            <div class="station-code">
                                <asp:Label ID="lblToCode" runat="server">--</asp:Label>
                            </div>
                            <div class="station-name">
                                <asp:Label ID="lblToStation" runat="server">Arrival Station</asp:Label>
                            </div>
                            <div class="station-time">
                                <asp:Label ID="lblArrivalTime" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- ══ Journey Information ══ -->
                    <div class="section-title"><i class="fas fa-route"></i>Journey Information</div>

                    <div class="info-grid">
                        <div class="info-box">
                            <div class="info-label">Booking ID</div>
                            <div class="info-value">
                                <asp:Label ID="lblBookedId" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label"><i class="fas fa-train"></i>Train Name / Number</div>
                            <div class="info-value">
                                <asp:Label ID="lblTrainName" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Route</div>
                            <div class="info-value">
                                <asp:Label ID="lblRoute" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Journey Date</div>
                            <div class="info-value">
                                <i class="far fa-calendar-alt" style="color: #f5a000;"></i>
                                <asp:Label ID="lblJourneyDate" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Class</div>
                            <div class="info-value">
                                <asp:Label ID="lblClass" runat="server" CssClass="class-badge"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Total Passengers</div>
                            <div class="info-value">
                                <i class="fas fa-users" style="color: #f5a000;"></i>
                                <asp:Label ID="lblTicketCount" runat="server"></asp:Label>
                            </div>
                        </div>
                        <%-- NEW: Seat Numbers (ported from Train_MyBookings.GetSeats) --%>
                        <div class="info-box">
                            <div class="info-label"><i class="fas fa-chair" style="color: #f5a000;"></i>Seat Numbers</div>
                            <div class="info-value">
                                <asp:Label ID="lblSeats" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Check-in Status</div>
                            <div class="info-value">
                                <asp:Label ID="lblCheckin" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- ══ Payment Information ══ -->
                    <div class="section-title" style="margin-top: 10px;"><i class="fas fa-credit-card"></i>Payment Information</div>

                    <div class="info-grid">
                        <div class="info-box">
                            <div class="info-label">Unit Price</div>
                            <div class="info-value" style="color: #FF9800;">
                                <asp:Label ID="lblUnitPrice" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Total Amount</div>
                            <div class="info-value" style="color: #FFC107;">
                                <asp:Label ID="lblSubTotal" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Transaction ID</div>
                            <div class="info-value">
                                <asp:Label ID="lblTransactionId" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Payment Status</div>
                            <div class="info-value">
                                <asp:Label ID="lblTransactionStatus" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">First Postpone Amount</div>
                            <div class="info-value">
                                <asp:Label ID="lblpostbalance" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Second Postpone Amount</div>
                            <div class="info-value">
                                <asp:Label ID="lblpostbalance2" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label"><i class="fas fa-undo-alt"></i>Refund Amount</div>
                            <div class="info-value">
                                <asp:Label ID="lblrefundamount" runat="server">CDF 0</asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label"><i class="fas fa-receipt"></i>Grand Total</div>
                            <div class="info-value">
                                <asp:Label ID="lbloverallamount" runat="server">CDF 0</asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- ══ Booking Timeline ══ -->
                    <div class="section-title" style="margin-top: 10px;"><i class="far fa-clock"></i>Booking Timeline</div>

                    <div class="info-grid">
                        <div class="info-box">
                            <div class="info-label">Booked On</div>
                            <div class="info-value">
                                <asp:Label ID="lblBookingDate" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="info-box">
                            <div class="info-label">Last Updated</div>
                            <div class="info-value">
                                <asp:Label ID="lblLastUpdated" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- ══ Passenger Details ══ -->
                    <div class="passengers-section">
                        <div class="section-title"><i class="fas fa-users"></i>Passenger Details</div>
                        <table class="passengers-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Passenger Name</th>
                                    <th>Coach Type</th>
                                    <th>Coach Number</th>
                                    <th>Seat / Berth</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPassengers" runat="server">
                                    <ItemTemplate>
                                        <tr class='<%# Convert.ToBoolean(Eval("isActive")) ? "" : "cancelled-row" %>'>

                                            <%-- 1. # --%>
                                            <td><%# Container.ItemIndex + 1 %></td>

                                            <%-- 2. Passenger Name --%>
                                            <td><strong><%# Eval("name") %></strong></td>

                                            <%-- 3. Coach Type --%>
                                            <td>
                                                <%# !string.IsNullOrEmpty(Eval("coachType")?.ToString()) ?
            "<span class='class-badge'>" + Eval("coachType") + "</span>" : "-" %>
                                            </td>

                                            <%-- 4. Coach Number --%>
                                            <td>
                                                <%# !string.IsNullOrEmpty(Eval("coachNumber")?.ToString()) ?
            "<span class='class-badge'>" + Eval("coachNumber") + "</span>" : "-" %>
                                            </td>

                                            <%-- 5. Seat / Berth --%>
                                            <td>
                                                <%# !string.IsNullOrEmpty(Eval("seatNumber")?.ToString()) ?
            "<span style='background:#3949ab;color:white;padding:3px 11px;border-radius:4px;font-size:13px;'>" + Eval("seatNumber") + "</span>" :
            "<span style='color:#bbb;'>Cancelled</span>" %>
                                            </td>

                                            <%-- 6. Status --%>
                                            <td>
                                                <%# Convert.ToBoolean(Eval("isActive")) ?
            "<span style='color:#e11a1a; font-weight:600;'><i class=\"fas fa-check-circle\"></i> Active</span>" :
            "<span style='color:#e53935;font-weight:600;'><i class=\"fas fa-times-circle\"></i> Cancelled</span>" %>
                                            </td>

                                            <%-- 7. Action --%>
                                            <td>
                                                <asp:Button ID="Button1" runat="server"
                                                    Text="Cancel"
                                                    CssClass="cancel-button"
                                                    OnClick="btnCancel_Click"
                                                    Enabled='<%# Convert.ToBoolean(Eval("isActive")) %>'
                                                    OnClientClick="return confirm('Cancel this passenger\'s ticket?');" />
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- ══ Cancellation Details ══ -->
                    <div class="cancellation-section" id="divCancellationDetails" runat="server" visible="false">
                        <div class="section-title" style="border-bottom-color: #FF9800; color: #e65100;">
                            <i class="fas fa-times-circle"></i>Cancellation Details
                       
                        </div>
                        <div class="refund-summary">
                            <div class="refund-item">
                                <div class="refund-item-label">Total Cancelled</div>
                                <div class="refund-item-value" style="color: #e53935;">
                                    <asp:Label ID="lblCancelledCount" runat="server">0</asp:Label>
                                </div>
                            </div>
                            <div class="refund-item">
                                <div class="refund-item-label">Refund Amount</div>
                                <div class="refund-item-value">
                                    <asp:Label ID="lblTotalRefund" runat="server">CDF 0</asp:Label>
                                </div>
                            </div>
                        </div>
                        <table class="cancellation-table">
                            <thead>
                                <tr>
                                    <th>Passenger Name</th>
                                    <th>Seat / Berth</th>
                                    <th>Refund Amount</th>
                                    <th>Cancelled On</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptCancellations" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("passengerName") %></td>
                                            <td><%# Eval("seatNumber") %></td>
                                            <td style="color: #FF9800; font-weight: bold;">CDF <%# String.Format("{0:N0}", Convert.ToDecimal(Eval("Amount"))) %>
                                            </td>
                                            <td><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("dd MMM yyyy hh:mm tt") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- ══ Action Buttons ══ -->
                    <div class="action-buttons">
                        <asp:Button ID="btnDownloadTicket" runat="server"
                            Text="📥 Download Ticket"
                            CssClass="btn btn-action btn-primary-action"
                            OnClick="btnDownloadTicket_Click" />
                        <asp:Button ID="btnBackToBookings" runat="server"
                            Text="⬅️ Back to Bookings"
                            CssClass="btn btn-action btn-secondary-action"
                            OnClick="btnBackToBookings_Click" />
                        <asp:Button ID="btnCancelBooking" runat="server"
                            Text="❌ Cancel Booking"
                            CssClass="btn btn-action btn-danger-action"
                            OnClick="btnCancelBooking_Click"
                            OnClientClick="return confirm('Are you sure you want to cancel this entire train booking? This action cannot be undone.');" />
                    </div>

                </div>
            </div>
        </div>
  <%--  </div>--%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Train Booking Details Page Loaded');
            if ($('#divCancellationDetails').is(':visible')) {
                $('html, body').animate({
                    scrollTop: $('#divCancellationDetails').offset().top - 100
                }, 900);
            }
        });
    </script>
    <script>
        $(document).ready(function () {
            // localStorage se coachNumber read karo
            var pnr = '<%= lblPNRNumber.Text %>';

            try {
                var map = JSON.parse(localStorage.getItem('PNRCoachMap') || '{}');
                var coachNum = map[pnr];

                if (coachNum) {
                    // Repeater ke har row mein coachNumber update karo
                    $('table.passengers-table tbody tr').each(function () {
                        var coachCell = $(this).find('td:nth-child(4)'); // Coach Number column
                        coachCell.html(
                            "<span class='class-badge'>" + coachNum + "</span>"
                        );
                    });

                    console.log('CoachNumber set from localStorage: ' + coachNum);
                } else {
                    console.log('CoachNumber not found in localStorage for PNR: ' + pnr);
                }
            } catch (e) {
                console.error('localStorage error:', e);
            }
        });
    </script>
</asp:Content>
