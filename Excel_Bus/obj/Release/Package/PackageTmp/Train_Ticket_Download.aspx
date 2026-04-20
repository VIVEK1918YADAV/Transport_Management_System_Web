<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_Ticket_Download.aspx.cs" Inherits="Excel_Bus.Train_Ticket_Download" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Download Ticket
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">

    <style>
        .ticket-container {
            padding: 20px 0;
            background: #c9f3e1bf;
        }

        .ticket-wrapper {
            max-width: 1190px;
            margin: 0 auto;
            background: #daf3e8;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
            margin-bottom: 64px;
        }

        .ticket-header {
            background: #47a17b;
            color: white;
            padding: 20px;
            text-align: center;
        }

            .ticket-header h1 {
                margin: 0 0 5px 0;
                font-size: 24px;
                font-weight: bold;
            }

            .ticket-header p {
                margin: 0;
                opacity: 0.9;
                font-size: 13px;
            }

        .ticket-body {
            padding: 20px 30px;
        }

        .pnr-qr-section {
            display: grid;
            grid-template-columns: 1fr auto;
            gap: 20px;
            align-items: center;
            margin-bottom: 20px;
            padding: 15px;
           background: #47a17b;
            border-radius: 8px;
        }

        .pnr-info {
            flex: 1;
        }

        .pnr-label {
            font-size: 12px;
            color: #0f2602;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 5px;
        }

        .pnr-number {
            font-size: 26px;
            font-weight: bold;
            color: #f9f4f4;
            letter-spacing: 2px;
            font-family: 'Courier New', monospace;
        }

        /* ✅ Optimized QR Code */
        .qr-code-wrapper {
            text-align: center;
        }

            .qr-code-wrapper img {
                width: 180px; /* ✅ Perfect UI size */
                height: 180px;
                border-radius: 6px;
                padding: 5px;
                background: #47a17b;
                display: block;
                margin: 0 auto;
                /* ✅ Crisp rendering for better scanning */
                image-rendering: -moz-crisp-edges;
                image-rendering: -webkit-optimize-contrast;
                image-rendering: crisp-edges;
                image-rendering: pixelated;
                -ms-interpolation-mode: nearest-neighbor;
                object-fit: contain;
                transform: translateZ(0);
                backface-visibility: hidden;
                -webkit-backface-visibility: hidden;
            }

        .qr-label {
            font-size: 10px;
            color: #667eea;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-top: 8px;
            font-weight: 700;
        }

        .ticket-section {
            margin-bottom: 15px;
            margin-left: 40px;
            margin-right: 40px;
        }

        .section-title {
            color: #333;
            font-size: 15px;
            font-weight: bold;
            margin-bottom: 10px;
            padding-bottom: 5px;
            border-bottom: 2px solid #29500b;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(1, 1fr);
            gap: 12px;
            margin-bottom: 15px;
        }

        .info-item {
            /*padding: 10px;*/
            background: #47a17b;
            border-radius: 6px;
            border-left: 3px solid #1d4a09;

        }

        .info-label {
            font-size: 10px;
            color: #073618;
            text-transform: uppercase;
            letter-spacing: 0.3px;
            margin-bottom: 3px;
                flex: auto;
        }

        .info-value {
            font-size: 14px;
            color: #fff6f6;
            font-weight: 600;
        }

        .passengers-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
            font-size: 13px;
        }

            .passengers-table th {
                background: #47a17b;
                color: white;
                padding: 8px 10px;
                text-align: left;
                font-weight: 600;
                font-size: 12px;
            }

            .passengers-table td {
                padding: 8px 10px;
                border-bottom: 1px solid #e0e0e0;
            }

            .passengers-table tr:last-child td {
                border-bottom: none;
            }

        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 600;
            background: #47a17b;
            color: white;
        }

            .status-badge.pending {
                background: #47a17b;
            }

            .status-badge.rejected {
                background:#47a17b;
            }

        .action-buttons {
           display: flex;
    gap: 10px;
    margin-top: 20px;
    padding-top: 15px;
    /* border-top: 2px solid #e0e0e0; */
    margin-bottom: 33px;
    margin-left: 120px;
        }

        .btn-action {
            flex: 1;
            padding: 12px 20px;
            font-size: 14px;
            font-weight: 600;
            border-radius: 6px;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-primary-action {
            background: #47a17b;
            color: white;
        }

            .btn-primary-action:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
                color: #043c04;
            }

        .btn-secondary-action {
            background: #47a17b;
            color:white;
            border: 2px solid;
        }

            .btn-secondary-action:hover {
                background: #47a17b;
                color: #043c04;
            }

        .ticket-footer {
            background: #47a17b;
            padding: 12px 30px;
            text-align: center;
            color: #666;
            font-size: 11px;
        }

            .ticket-footer p {
                margin: 5px 0;
            }

        /* Print Styles */
        @media print {
            body * {
                visibility: hidden;
            }

            .ticket-wrapper,
            .ticket-wrapper * {
                visibility: visible;
            }

            .ticket-wrapper {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                box-shadow: none;
                border-radius: 0;
                margin-bottom: 64px;
            }

            .ticket-container {
                padding: 0;
                background: white;
            }

            .ticket-body {
                padding: 15px 20px;
            }

            .action-buttons {
                display: none !important;
            }

            .pnr-qr-section {
                margin-bottom: 15px;
                padding: 10px;
            }

            .ticket-section {
                margin-bottom: 12px;
            }

            .info-grid {
                gap: 10px;
                margin-bottom: 10px;
            }

            .info-item {
                padding: 8px;
            }

            .section-title {
                margin-bottom: 8px;
                font-size: 14px;
            }

            .passengers-table {
                font-size: 12px;
            }

                .passengers-table th,
                .passengers-table td {
                    padding: 6px 8px;
                }

            .ticket-footer {
                padding: 10px 20px;
            }

            /* ✅ QR code in print - slightly bigger */
            .qr-code-wrapper img {
                width: 200px !important;
                height: 200px !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
                image-rendering: crisp-edges;
                image-rendering: pixelated;
            }

            .pnr-qr-section,
            .ticket-section,
            .passengers-table {
                page-break-inside: avoid;
            }
        }

        @media (max-width: 768px) {
            .ticket-body {
                padding: 15px;
            }

            .info-grid {
                grid-template-columns: 1fr;
                gap: 10px;
            }

            .pnr-qr-section {
                grid-template-columns: 1fr;
                text-align: center;
            }

            .qr-code-wrapper img {
                width: 180px;
                height: 180px;
            }

            .action-buttons {
                flex-direction: column;
            }

            .pnr-number {
                font-size: 20px;
            }
        }
        .container {
    padding-right: 0px;
    padding-left: 0px;
    margin-right: auto;
    margin-left: auto;
    margin-top: 6px;
}
        .ticket-section {
    padding-top: 23px;
}
    </style>
     <%--<div class="ticket-container">--%>
       <div class="container">
            <div class="ticket-wrapper">
                <!-- Ticket Header -->
                <div class="ticket-header">
                    <h1><i class="fas fa-bus"></i>Excel Train Ticket</h1>
                    <p>Your journey confirmation</p>
                </div>

                <!-- Ticket Body -->
                <div class="ticket-body">
                    <!-- Combined PNR & QR Section -->
                    <div class="pnr-qr-section">
                        <div class="pnr-info">
                            <div class="pnr-label">PNR Number</div>
                            <div class="pnr-number">
                                <asp:Label ID="lblPNRNumber" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="qr-code-wrapper" id="qrCodeSection" runat="server" visible="false">
                            <asp:Image ID="imgQRCode" runat="server" AlternateText="QR Code" />

                        </div>
                    </div>

                    <!-- Journey Details -->
                    <div class="ticket-section">
                        <div class="section-title">
                            <i class="fas fa-route"></i>Journey Details
                        </div>

                        <div class="info-grid">
                            <div class="info-item">
                                <div class="info-label">Source</div>
                                <div class="info-value">
                                    <asp:Label ID="lblSource" runat="server"></asp:Label>
                                </div>
                            </div>

                            <div class="info-item">
                                <div class="info-label">Destination</div>
                                <div class="info-value">
                                    <asp:Label ID="lblDestination" runat="server"></asp:Label>
                                </div>
                            </div>

                            <div class="info-item">
                                <div class="info-label">Route</div>
                                <div class="info-value">
                                    <asp:Label ID="lblRoute" runat="server"></asp:Label>
                                </div>
                            </div>
                       

                        <div class="info-item">
                            <div class="info-label">Train No.</div>
                            <div class="info-value">
                                <asp:Label ID="lblTrainNo" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-item">
                            <div class="info-label">Train Name</div>
                            <div class="info-value">
                                <asp:Label ID="lblTrainName" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-item">
                            <div class="info-label">Journey Date</div>
                            <div class="info-value">
                                <i class="far fa-calendar-alt"></i>
                                <asp:Label ID="lblJourneyDate" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-item">
                            <div class="info-label">Passengers</div>
                            <div class="info-value">
                                <i class="fas fa-users"></i>
                                <asp:Label ID="lblTicketCount" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-item">
                            <div class="info-label">Amount</div>
                            <div class="info-value" style="color: #073618;">
                                <asp:Label ID="lblTotalAmount" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="info-item">
                            <div class="info-label">Status</div>
                            <div class="info-value">
                                <span class="status-badge">
                                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                 </div>
                <!-- Passenger Details -->
                <div class="ticket-section">
                    <div class="section-title">
                        <i class="fas fa-users"></i>Passenger Details
                    </div>

                    <table class="passengers-table">
                        <thead>
                            <tr>
                                <th style="width: 40px;">#</th>
                                <th>Passenger Name</th>
                                <th style="width: 100px;">Class</th>
                                <th style="width: 100px;">Coach No.</th>
                                <th style="width: 100px;">Seat No.</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPassengers" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Container.ItemIndex + 1 %></td>
                                        <td><%# Eval("name") %></td>
                                        <td><strong><%# Eval("CoachTypeId") %></strong></td>
                                        <td><strong><%# Eval("CoachNo") %></strong></td>
                                        <td><strong><%# Eval("seatNumber") %></strong></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <!-- Booking Information -->
                <div class="ticket-section">
                    <div class="section-title">
                        <i class="fas fa-info-circle"></i>Booking Info
                    </div>

                    <div class="info-grid">
                        <div class="info-item">
                            <div class="info-label">Booking Date</div>
                            <div class="info-value">
                                <asp:Label ID="lblBookingDate" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="action-buttons no-print">
                    <asp:Button ID="btnPrint" runat="server"
                        Text="🖨️ Print"
                        CssClass="btn btn-action btn-primary-action"
                        OnClick="btnPrint_Click" />

                    <asp:Button ID="btnDownloadPDF" runat="server"
                        Text="📥 Download PDF"
                        CssClass="btn btn-action btn-primary-action"
                        OnClick="btnDownloadPDF_Click" />

                    <asp:Button ID="btnBackToBookings" runat="server"
                        Text="⬅️ Back"
                        CssClass="btn btn-action btn-secondary-action"
                        OnClick="btnBackToBookings_Click" />
                </div>
            </div>

            <!-- Ticket Footer -->
            <%--<div class="ticket-footer">
                <p>
                    <i class="fas fa-info-circle"></i>
                    Please carry a valid ID proof. Present this QR code for verification.
                </p>
                <p>
                    <strong>Excel Transport Services</strong> | Contact: +91 98717 71849
                </p>
            </div>
        </div>--%>
    </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Download Ticket Page Loaded');

            // Add status badge color based on status
            var statusText = $('#<%= lblStatus.ClientID %>').text().trim();
            var statusBadge = $('#<%= lblStatus.ClientID %>').closest('.status-badge');

            if (statusText.toLowerCase() === 'pending') {
                statusBadge.addClass('pending');
            } else if (statusText.toLowerCase() === 'rejected') {
                statusBadge.addClass('rejected');
            }
        });
    </script>
</asp:Content>
