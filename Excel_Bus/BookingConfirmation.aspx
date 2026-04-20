<%@ Page Title="Booking Confirmation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookingConfirmation.aspx.cs" Inherits="Excel_Bus.BookingConfirmation" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Booking Confirmation
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    
    <style>
        .confirmation-container {
            padding: 60px 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        
        .confirmation-card {
            background: white;
            border-radius: 16px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            padding: 0;
            max-width: 700px;
            margin: 0 auto;
            overflow: hidden;
        }
        
        .success-header {
            background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }
        
        .success-icon {
            width: 80px;
            height: 80px;
            background: white;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 20px;
            animation: scaleIn 0.5s ease-in-out;
        }
        
        .success-icon i {
            font-size: 40px;
            color: #4CAF50;
        }
        
        @keyframes scaleIn {
            0% {
                transform: scale(0);
            }
            50% {
                transform: scale(1.1);
            }
            100% {
                transform: scale(1);
            }
        }
        
        .success-header h2 {
            margin: 0 0 10px 0;
            font-size: 28px;
            font-weight: bold;
        }
        
        .success-header p {
            margin: 0;
            font-size: 16px;
            opacity: 0.9;
        }
        
        .confirmation-body {
            padding: 40px 30px;
        }
        
        .pnr-section {
            background: #f0f4ff;
            border: 2px dashed #667eea;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            margin-bottom: 30px;
        }
        
        .pnr-section h4 {
            color: #667eea;
            margin: 0 0 10px 0;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }
        
        .pnr-number {
            font-size: 28px;
            font-weight: bold;
            color: #333;
            letter-spacing: 2px;
            font-family: 'Courier New', monospace;
        }
        
        .transaction-info {
            text-align: center;
            color: #666;
            font-size: 14px;
            margin-top: 10px;
        }
        
        .transaction-number {
            color: #4CAF50;
            font-weight: 600;
        }
        
        .booking-details-section {
            margin-bottom: 30px;
        }
        
        .booking-details-section h4 {
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e0e0e0;
            font-size: 18px;
        }
        
        .detail-item {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #f0f0f0;
        }
        
        .detail-item:last-child {
            border-bottom: none;
        }
        
        .detail-label {
            color: #666;
            font-size: 14px;
        }
        
        .detail-value {
            color: #333;
            font-weight: 600;
            font-size: 14px;
            text-align: right;
        }
        
        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            background: #4CAF50;
            color: white;
            font-size: 12px;
            font-weight: 600;
        }
        
        .action-buttons {
            display: flex;
            flex-direction: column;
            gap: 12px;
            margin-top: 30px;
        }
        
        .btn-action {
            padding: 15px 30px;
            font-size: 16px;
            font-weight: 600;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
            text-align: center;
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
        
        .info-note {
            background: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            border-radius: 4px;
            margin-top: 30px;
        }
        
        .info-note i {
            color: #ffc107;
            margin-right: 8px;
        }
        
        .info-note p {
            margin: 0;
            color: #856404;
            font-size: 14px;
        }
        
        @media (max-width: 768px) {
            .confirmation-container {
                padding: 30px 15px;
            }
            
            .confirmation-card {
                margin: 0 15px;
            }
            
            .confirmation-body {
                padding: 30px 20px;
            }
            
            .pnr-number {
                font-size: 22px;
            }
            
            .success-header h2 {
                font-size: 24px;
            }
        }
    </style>

    <div class="confirmation-container">
        <div class="container">
            <div class="confirmation-card">
                <!-- Success Header -->
                <div class="success-header">
                    <div class="success-icon">
                        <i class="fas fa-check"></i>
                    </div>
                    <h2>Booking Confirmed!</h2>
                    <p>Your ticket has been booked successfully</p>
                </div>

                <!-- Confirmation Body -->
                <div class="confirmation-body">
                    <!-- PNR Section -->
                    <div class="pnr-section">
                        <h4>Your PNR Number</h4>
                        <div class="pnr-number">
                            <asp:Label ID="lblPNRNumber" runat="server"></asp:Label>
                        </div>
                        <div class="transaction-info">
                            Transaction ID: <span class="transaction-number">
                                <asp:Label ID="lblTransactionNumber" runat="server"></asp:Label>
                            </span>
                        </div>
                    </div>

                    <!-- Booking Details -->
                    <div class="booking-details-section">
                        <h4><i class="fas fa-ticket-alt"></i> Booking Details</h4>
                        
                        <div class="detail-item">
                            <span class="detail-label">Route</span>
                            <span class="detail-value">
                                <asp:Label ID="lblRoute" runat="server" Text="N/A"></asp:Label>
                            </span>
                        </div>
                        
                        <div class="detail-item">
                            <span class="detail-label">Journey Date</span>
                            <span class="detail-value">
                                <asp:Label ID="lblJourneyDate" runat="server" Text="N/A"></asp:Label>
                            </span>
                        </div>
                        
                        <div class="detail-item">
                            <span class="detail-label">Seat(s)</span>
                            <span class="detail-value">
                                <asp:Label ID="lblSeats" runat="server" Text="N/A"></asp:Label>
                            </span>
                        </div>
                        
                        <div class="detail-item">
                            <span class="detail-label">Passengers</span>
                            <span class="detail-value">
                                <asp:Label ID="lblPassengerCount" runat="server" Text="N/A"></asp:Label>
                            </span>
                        </div>
                        
                        <div class="detail-item">
                            <span class="detail-label">Total Amount</span>
                            <span class="detail-value">
                                <asp:Label ID="lblAmount" runat="server" Text="CDF 0"></asp:Label>
                            </span>
                        </div>
                        
                        <div class="detail-item">
                            <span class="detail-label">Status</span>
                            <span class="detail-value">
                                <span class="status-badge">
                                    <asp:Label ID="lblStatus" runat="server" Text="Booked"></asp:Label>
                                </span>
                            </span>
                        </div>
                    </div>

                    <!-- Action Buttons -->
                    <div class="action-buttons">
                       <%-- <asp:Button ID="btnDownloadTicket" runat="server" 
                                   Text="Download Ticket" 
                                   CssClass="btn btn-action btn-primary-action"
                                   OnClick="btnDownloadTicket_Click" />--%>
                        
                        <asp:Button ID="btnViewBookings" runat="server" 
                                   Text="View My Bookings" 
                                   CssClass="btn btn-action btn-secondary-action"
                                   OnClick="btnViewBookings_Click" />
                        
                        <asp:Button ID="btnBookAnother" runat="server" 
                                   Text="Book Another Ticket" 
                                   CssClass="btn btn-action btn-secondary-action"
                                   OnClick="btnBookAnother_Click" />
                    </div>

                    <!-- Info Note -->
                    <div class="info-note">
                        <p>
                            <i class="fas fa-info-circle"></i>
                            <strong>Important:</strong> Please save your PNR number for future reference. 
                            A confirmation email has been sent to your registered email address.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Booking Confirmation Page Loaded');
            
          
        });
    </script>
</asp:Content>