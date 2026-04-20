<%@ Page Title="Payment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="payment.aspx.cs" Inherits="Excel_Bus.payment" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Payment
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    
    <style>
        .payment-container {
            padding: 40px 0;
            background: #f5f5f5;
        }
        
        .payment-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            padding: 40px;
            max-width: 800px;
            margin: 0 auto;
        }
        
        .page-header {
            text-align: center;
            margin-bottom: 30px;
        }
        
        .page-header h2 {
            color: #333;
            font-weight: bold;
            margin-bottom: 10px;
        }
        
        .page-header p {
            color: #666;
            font-size: 16px;
        }
        
        .booking-details {
            background: #f9f9f9;
            border-radius: 8px;
            padding: 25px;
            margin-bottom: 30px;
        }
        
        .booking-details h4 {
            color: #333;
            margin-top: 0;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #667eea;
        }
        
        .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .detail-row:last-child {
            border-bottom: none;
        }
        
        .detail-label {
            font-weight: 600;
            color: #555;
        }
        
        .detail-value {
            color: #333;
            text-align: right;
        }
        
        .pnr-highlight {
            color: #667eea;
            font-weight: bold;
            font-size: 18px;
        }
        
        .amount-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 8px;
            padding: 25px;
            margin-bottom: 30px;
            text-align: center;
        }
        
        .amount-section h3 {
            margin: 0 0 10px 0;
            font-size: 18px;
            font-weight: normal;
        }
        
        .amount-section .amount {
            font-size: 36px;
            font-weight: bold;
            margin: 0;
        }
        
        .payment-methods {
            margin-bottom: 30px;
        }
        
        .payment-methods h4 {
            color: #333;
            margin-bottom: 20px;
        }
        
        .payment-option {
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 15px;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .payment-option:hover {
            border-color: #667eea;
            background: #f9f9f9;
        }
        
        .payment-option.selected {
            border-color: #667eea;
            background: #f0f4ff;
        }
        
        .payment-option input[type="radio"] {
            margin-right: 10px;
        }
        
        .payment-option label {
            margin: 0;
            cursor: pointer;
            font-weight: 500;
        }
        
        .btn-confirm-payment {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 18px 50px;
            font-size: 18px;
            font-weight: bold;
            border-radius: 8px;
            width: 100%;
            transition: all 0.3s ease;
        }
        
        .btn-confirm-payment:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(102, 126, 234, 0.4);
            color: white;
        }
        
        .btn-confirm-payment:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }
        
        .security-note {
            text-align: center;
            color: #666;
            font-size: 14px;
            margin-top: 20px;
        }
        
        .security-note i {
            color: #4CAF50;
            margin-right: 5px;
        }
        
        @media (max-width: 768px) {
            .payment-card {
                padding: 20px;
            }
            
            .amount-section .amount {
                font-size: 28px;
            }
        }
    </style>

    <div class="payment-container">
        <div class="container">
            <div class="payment-card">
                <div class="page-header">
                    <h2><i class="fas fa-credit-card"></i> Complete Your Payment</h2>
                    <p>Review your booking details and confirm payment</p>
                </div>

                <!-- Booking Details -->
                <div class="booking-details">
                    <h4><i class="fas fa-ticket-alt"></i> Booking Details</h4>
                    
                    <div class="detail-row">
                        <span class="detail-label">PNR Number:</span>
                        <span class="detail-value pnr-highlight">
                            <asp:Label ID="lblPNRNumber" runat="server"></asp:Label>
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">Booking ID:</span>
                        <span class="detail-value">
                            <asp:Label ID="lblBookedId" runat="server"></asp:Label>
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">Selected Seats:</span>
                        <span class="detail-value">
                            <asp:Label ID="lblSelectedSeats" runat="server"></asp:Label>
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">Journey Date:</span>
                        <span class="detail-value">
                            <asp:Label ID="lblJourneyDate" runat="server"></asp:Label>
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">Route:</span>
                        <span class="detail-value">
                            <asp:Label ID="lblRoute" runat="server"></asp:Label>
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">Ticket Amount:</span>
                        <span class="detail-value">
                            <asp:Label ID="lblAmount" runat="server"></asp:Label>
                        </span>
                    </div>
                </div>

                <!-- Total Amount -->
                <div class="amount-section">
                    <h3>Total Amount to Pay</h3>
                    <p class="amount">
                        <asp:Label ID="lblTotalAmount" runat="server"></asp:Label>
                    </p>
                </div>

                <!-- Payment Methods -->
                <div class="payment-methods">
                    <h4><i class="fas fa-money-check-alt"></i> Select Payment Method</h4>
                    
                    <div class="payment-option">
                        <input type="radio" id="method1" name="paymentMethod" value="card" checked />
                        <label for="method1">
                            <i class="fas fa-credit-card"></i> Credit/Debit Card
                        </label>
                    </div>
                    
                    <div class="payment-option">
                        <input type="radio" id="method2" name="paymentMethod" value="mobile" />
                        <label for="method2">
                            <i class="fas fa-mobile-alt"></i> Mobile Money
                        </label>
                    </div>
                    
                    <div class="payment-option">
                        <input type="radio" id="method3" name="paymentMethod" value="bank" />
                        <label for="method3">
                            <i class="fas fa-university"></i> Bank Transfer
                        </label>
                    </div>
                </div>

                <!-- Confirm Payment Button -->
                <div>
                    <asp:Button ID="btnConfirmPayment" runat="server" 
                               Text="Confirm Payment" 
                               CssClass="btn btn-confirm-payment"
                               OnClick="btnConfirmPayment_Click" />
                </div>

                <!-- Security Note -->
                <div class="security-note">
                    <p>
                        <i class="fas fa-shield-alt"></i>
                        Your payment information is secure and encrypted
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Payment Page Loaded');
            
            // Add visual feedback for payment method selection
            $('.payment-option').on('click', function() {
                $('.payment-option').removeClass('selected');
                $(this).addClass('selected');
                $(this).find('input[type="radio"]').prop('checked', true);
            });
        });
    </script>
</asp:Content>