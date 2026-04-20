<%@ Page Title="Booking Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookingDetails.aspx.cs" Inherits="Excel_Bus.BookingDetails" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Booking Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">

    <style>
        .details-container {
            padding: 40px 0;
            background: #f5f5f5;
            min-height: 100vh;
        }

        .page-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px 0;
            margin-bottom: 40px;
        }

            .page-header h2 {
                margin: 0;
                font-weight: bold;
            }

        .details-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
            margin-bottom: 20px;
        }

        .card-header-custom {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .pnr-info h3 {
            margin: 0 0 5px 0;
            font-size: 14px;
            opacity: 0.9;
        }

        .pnr-number {
            font-size: 24px;
            font-weight: bold;
            font-family: 'Courier New', monospace;
            letter-spacing: 2px;
        }

        .status-badge {
            padding: 8px 20px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            background: white;
            color: #4CAF50;
        }

        .card-body-custom {
            padding: 30px;
        }

        .section-title {
            color: #333;
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #667eea;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .info-box {
            background: #f9f9f9;
            padding: 20px;
            border-radius: 8px;
            border-left: 4px solid #667eea;
        }

        /* ✅ ADDED: Highlight refund amount box */
        .info-box.highlight-refund {
            background: #e8f5e9;
            border-left: 4px solid #4CAF50;
        }

        .info-box.highlight-warning {
            background: #fff3e0;
            border-left: 4px solid #FF9800;
        }

        .info-label {
            font-size: 12px;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-bottom: 8px;
        }

        .info-value {
            font-size: 16px;
            color: #333;
            font-weight: 600;
        }

        /* ✅ ADDED: Special styling for refund amounts */
        .info-value.refund-amount {
            font-size: 20px;
            color: #4CAF50;
            font-weight: 700;
        }

        .passengers-section {
            margin-top: 30px;
        }

        .passengers-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
            background: white;
        }

            .passengers-table th {
                background: #667eea;
                color: white;
                padding: 15px;
                text-align: left;
                font-weight: 600;
            }

            .passengers-table td {
                padding: 15px;
                border-bottom: 1px solid #e0e0e0;
            }

            .passengers-table tr:last-child td {
                border-bottom: none;
            }

            .passengers-table tr:hover {
                background: #f9f9f9;
            }

            /* ✅ ADDED: Style for cancelled passengers */
            .passengers-table tr.cancelled-row {
                background: #ffebee;
                opacity: 0.7;
            }

            .passengers-table tr.cancelled-row td {
                text-decoration: line-through;
                color: #999;
            }

        /* ✅ ADDED: Cancellation Details Section Styles */
        .cancellation-section {
            margin-top: 30px;
            padding: 20px;
            background: #fff3e0;
            border-radius: 8px;
            border-left: 4px solid #FF9800;
        }

        .cancellation-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
            background: white;
        }

            .cancellation-table th {
                background: #FF9800;
                color: white;
                padding: 15px;
                text-align: left;
                font-weight: 600;
            }

            .cancellation-table td {
                padding: 15px;
                border-bottom: 1px solid #e0e0e0;
            }

        .refund-summary {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-top: 20px;
            padding: 20px;
            background: white;
            border-radius: 8px;
        }

        .refund-item {
            text-align: center;
            padding: 15px;
            border-radius: 8px;
            background: #f5f5f5;
        }

        .refund-item-label {
            font-size: 12px;
            color: #666;
            text-transform: uppercase;
            margin-bottom: 8px;
        }

        .refund-item-value {
            font-size: 18px;
            font-weight: bold;
            color: #4CAF50;
        }

        .action-buttons {
            display: flex;
            gap: 15px;
            margin-top: 30px;
        }

        .btn-action {
            flex: 1;
            padding: 15px 30px;
            font-size: 16px;
            font-weight: 600;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-primary-action {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

            .btn-primary-action:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
                color: white;
            }

        .btn-secondary-action {
            background: white;
            color: #667eea;
            border: 2px solid #667eea;
        }

            .btn-secondary-action:hover {
                background: #f0f4ff;
                color: #667eea;
            }

        .btn-danger-action {
            background: white;
            color: #dc3545;
            border: 2px solid #dc3545;
        }

            .btn-danger-action:hover {
                background: #dc3545;
                color: white;
            }

        .cancel-button {
            color: #F44336;
            background-color: transparent;
            border: 2px solid #F44336;
            padding: 5px 15px;
            cursor: pointer;
            border-radius: 4px;
            transition: all 0.3s ease;
        }

            .cancel-button:hover {
                background-color: #F44336;
                color: white;
            }

            .cancel-button:disabled {
                opacity: 0.5;
                cursor: not-allowed;
            }

        @media (max-width: 768px) {
            .card-body-custom {
                padding: 20px;
            }

            .info-grid {
                grid-template-columns: 1fr;
            }

            .action-buttons {
                flex-direction: column;
            }

            .card-header-custom {
                flex-direction: column;
                align-items: flex-start;
                gap: 15px;
            }
        }
    </style>

    <div class="details-container">
        <div class="page-header">
            <div class="container">
                <h2><i class="fas fa-file-alt"></i> Booking Details</h2>
            </div>
        </div>

        <div class="container">
            <!-- Main Details Card -->
            <div class="details-card">
                <div class="card-header-custom">
                    <div class="pnr-info">
                        <h3>PNR NUMBER</h3>
                        <div class="pnr-number">
                            <asp:Label ID="lblPNRNumber" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div>
                        <span class="status-badge">
                            <asp:Label ID="lblStatus" runat="server"></asp:Label>
                        </span>
                    </div>
                </div>

                <div class="card-body-custom">
                    <!-- Journey Information -->
                    <div class="section-title">
                        <i class="fas fa-route"></i> Journey Information
                    </div>

                    <div class="info-grid">
                        <div class="info-box">
                            <div class="info-label">Booking ID</div>
                            <div class="info-value">
                                <asp:Label ID="lblBookedId" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-box">
                            <div class="info-label">Trip</div>
                            <div class="info-value">
                                <asp:Label ID="lblTripTitle" runat="server"></asp:Label>
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
                                <i class="far fa-calendar-alt"></i>
                                <asp:Label ID="lblJourneyDate" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-box">
                            <div class="info-label">Total Passengers</div>
                            <div class="info-value">
                                <i class="fas fa-users"></i>
                                <asp:Label ID="lblTicketCount" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-box">
                            <div class="info-label">Check-in Status</div>
                            <div class="info-value">
                                <asp:Label ID="lblCheckin" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- Payment Information -->
                    <div class="section-title" style="margin-top: 30px;">
                        <i class="fas fa-credit-card"></i> Payment Information
                    </div>

                    <div class="info-grid">
                        <div class="info-box">
                            <div class="info-label">Unit Price</div>
                            <div class="info-value" style="color: #667eea;">
                                <asp:Label ID="lblUnitPrice" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-box">
                            <div class="info-label">Total Amount</div>
                            <div class="info-value" style="color: #4CAF50;">
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

                        <!-- ✅ ADDED: Highlighted Refund Amount Box -->
                        <div class="info-box highlight-refund">
                            <div class="info-label">
                                <i class="fas fa-undo-alt"></i> Refund Amount
                            </div>
                            <div class="info-value refund-amount">
                                <asp:Label ID="lblrefundamount" runat="server"></asp:Label>
                            </div>
                        </div>

                        <!-- ✅ ADDED: Overall Amount Box -->
                        <div class="info-box highlight-warning">
                            <div class="info-label">
                                <i class="fas fa-receipt"></i> Grand Total
                            </div>
                            <div class="info-value">
                                <asp:Label ID="lbloverallamount" runat="server">CDF 0</asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- Booking Timeline -->
                    <div class="section-title" style="margin-top: 30px;">
                        <i class="far fa-clock"></i> Booking Timeline
                    </div>

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

                    <!-- Passengers Section -->
                    <div class="passengers-section">
                        <div class="section-title">
                            <i class="fas fa-users"></i> Passenger Details
                        </div>

                        <table class="passengers-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Passenger Name</th>
                                    <th>Seat Number</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptPassengers" runat="server">
                                    <ItemTemplate>
                                        <!-- ✅ ADDED: Conditional row styling for cancelled passengers -->
                                        <tr class='<%# Convert.ToBoolean(Eval("isActive")) ? "" : "cancelled-row" %>'>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <td><strong><%# Eval("name") %></strong></td>
                                            <td>
                                                <%# !string.IsNullOrEmpty(Eval("seatNumber")?.ToString()) ? 
                                                    "<span style='background: #667eea; color: white; padding: 4px 12px; border-radius: 4px;'>" + 
                                                    Eval("seatNumber") + "</span>" : 
                                                    "<span style='color: #999;'>Cancelled</span>" %>
                                            </td>
                                            <td>
                                                <%# Convert.ToBoolean(Eval("isActive")) ? 
                                                    "<span style='color: #4CAF50;'>Active</span>" : 
                                                    "<span style='color: #F44336;'>Cancelled</span>" %>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnCancel" runat="server" 
                                                    Text="Cancel Ticket" 
                                                    CssClass="cancel-button" 
                                                    OnClick="btnCancel_Click"
                                                    Enabled='<%# Convert.ToBoolean(Eval("isActive")) %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- ✅ ADDED: Cancellation Details Section -->
                    <div class="cancellation-section" id="divCancellationDetails" runat="server" visible="false">
                        <div class="section-title" style="border-bottom-color: #FF9800;">
                            <i class="fas fa-times-circle"></i> Cancellation Details
                        </div>

                        <div class="refund-summary">
                            <div class="refund-item">
                                <div class="refund-item-label">Total Cancelled</div>
                                <div class="refund-item-value" style="color: #F44336;">
                                    <asp:Label ID="lblCancelledCount" runat="server">0</asp:Label>
                                </div>
                            </div>
                            <div class="refund-item">
                                <div class="refund-item-label">Refund Amount</div>
                                <div class="refund-item-value">
                                    <asp:Label ID="lblTotalRefund" runat="server">CDF 0</asp:Label>
                                </div>
                            </div>
                            <%--<div class="refund-item">
                                <div class="refund-item-label">Cancellation Charge</div>
                                <div class="refund-item-value" style="color: #FF9800;">
                                    <asp:Label ID="lblCancellationCharge" runat="server">CDF 0</asp:Label>
                                </div>
                            </div>
                            <div class="refund-item">
                                <div class="refund-item-label">Refund Percentage</div>
                                <div class="refund-item-value" style="color: #2196F3;">
                                    <asp:Label ID="lblRefundPercentage" runat="server">0%</asp:Label>
                                </div>
                            </div>--%>
                        </div>

                        <table class="cancellation-table">
                            <thead>
                                <tr>
                                    <th>Passenger Name</th>
                                    <th>Seat Number</th>
                                   <%-- <th>Original Amount</th>--%>
                                    <th>Refund Amount</th>
                                    <th>Cancelled On</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptCancellations" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Name") %></td>
                                            <td><%# Eval("Seatnumber") %></td>
                                           <%-- <td>CDF <%# String.Format("{0:N0}", Eval("Amount")) %></td>--%>
                                            <td style="color: #4CAF50; font-weight: bold;">
                                                CDF <%# String.Format("{0:N0}", Convert.ToDecimal(Eval("Amount"))) %>
                                            </td>
                                            <td><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("dd MMM yyyy hh:mm tt") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- Action Buttons -->
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
                            OnClientClick="return confirm('Are you sure you want to cancel this entire booking? This action cannot be undone.');" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Booking Details Page Loaded');

            // ✅ ADDED: Auto-scroll to cancellation section if visible
            if ($('#divCancellationDetails').is(':visible')) {
                $('html, body').animate({
                    scrollTop: $('#divCancellationDetails').offset().top - 100
                }, 1000);
            }
        });
    </script>
</asp:Content>