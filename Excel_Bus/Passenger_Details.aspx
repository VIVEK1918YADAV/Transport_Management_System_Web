<%@ Page Title="Passenger Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Passenger_Details.aspx.cs" Inherits="Excel_Bus.Passenger_Details" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Passenger Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    
    <style>
        .passenger-container {
            padding: 40px 0;
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
        
        .panel-default {
            border-color: #ddd;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .panel-heading {
            background: #f5f5f5 !important;
            border-bottom: 2px solid #667eea;
        }
        
        .panel-heading h4 {
            color: #333;
        }
        
        .form-control {
            border-radius: 4px;
            border: 1px solid #ddd;
            padding: 10px;
            height: auto;
            line-height: normal;
        }
        
        select.form-control {
            height: 42px;
            padding: 10px 12px;
        }
        
        .form-control:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
        }
        
        .summary-panel {
            background: #f9f9f9;
            border: 2px solid #667eea;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 30px;
        }
        
        .contact-panel {
            background: #f0f8ff;
            border: 2px solid #4CAF50;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 30px;
        }
        
        .summary-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #ddd;
        }
        
        .summary-row:last-child {
            border-bottom: none;
            font-weight: bold;
            font-size: 18px;
            color: #667eea;
        }
        
        .contact-row {
            margin-bottom: 15px;
        }
        
        .contact-row:last-child {
            margin-bottom: 0;
        }
        
        .btn-confirm {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 15px 40px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 5px;
            transition: all 0.3s ease;
            white-space: normal;
            height: auto;
            line-height: 1.5;
            min-height: 50px;
        }
        
        .btn-confirm:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(102, 126, 234, 0.3);
            color: white;
        }
        
        .btn-confirm:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }
        
        .section-title {
            border-bottom: 2px solid #667eea;
            padding-bottom: 10px;
            margin-bottom: 20px;
        }
        
        .contact-title {
            border-bottom: 2px solid #4CAF50;
            padding-bottom: 10px;
            margin-bottom: 15px;
        }
    </style>

    <div class="passenger-container">
        <div class="container">
            <div class="row">
                <div class="col-md-8">
                    <!-- Contact Details Section -->
                    <div class="contact-panel">
                        <h4 class="contact-title" style="margin-top: 0;">
                            <i class="fas fa-address-book"></i> Contact Details
                        </h4>
                        <p style="color: #666; margin-bottom: 15px;">
                            <small>These details will be used for booking confirmation and communication</small>
                        </p>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group contact-row">
                                    <label>Email Address</label>
                                    <asp:TextBox ID="txtContactEmail" runat="server" 
                                                CssClass="form-control" 
                                                TextMode="Email"
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5; cursor: not-allowed;"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group contact-row">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtContactMobile" runat="server" 
                                                CssClass="form-control" 
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5; cursor: not-allowed;"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group contact-row">
                                    <label>Location</label>
                                    <asp:TextBox ID="txtContactLocation" runat="server" 
                                                CssClass="form-control" 
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5; cursor: not-allowed;"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Passenger Details Section -->
                    <h3 class="section-title">
                        <i class="fas fa-users"></i> Passenger Details
                    </h3>
                    
                    <asp:Panel ID="pnlPassengers" runat="server">
                        <!-- Passenger forms will be generated here dynamically -->
                    </asp:Panel>

                    <div class="text-center" style="margin-top: 30px;">
                        <asp:Button ID="btnConfirmBooking" runat="server" 
                                   Text="Confirm Booking & Proceed to Payment" 
                                   CssClass="btn btn-confirm"
                                   OnClick="btnConfirmBooking_Click"
                                   UseSubmitBehavior="true" />
                    </div>
                </div>

                <div class="col-md-4">
                    <!-- Booking Summary -->
                    <div class="summary-panel">
                        <h4 style="margin-top: 0; border-bottom: 2px solid #667eea; padding-bottom: 10px;">
                            <i class="fas fa-clipboard-list"></i> Booking Summary
                        </h4>

                        <div class="summary-row">
                            <span><strong>Selected Seats:</strong></span>
                            <span><asp:Label ID="lblSelectedSeats" runat="server"></asp:Label></span>
                        </div>

                        <div class="summary-row">
                            <span><strong>Journey Date:</strong></span>
                            <span><asp:Label ID="lblJourneyDate" runat="server"></asp:Label></span>
                        </div>

                        <div class="summary-row">
                            <span><strong>Route:</strong></span>
                            <span><asp:Label ID="lblRoute" runat="server"></asp:Label></span>
                        </div>

                        <div class="summary-row">
                            <span><strong>Total Amount:</strong></span>
                            <span><asp:Label ID="lblTotalAmount" runat="server" CssClass="text-success"></asp:Label></span>
                        </div>
                    </div>

                    <div class="alert alert-info">
                        <i class="fas fa-info-circle"></i>
                        <strong>Note:</strong> Please ensure all passenger and contact details are correct. These will be used for boarding and communication.
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Passenger Details Page Loaded');

            // Disable client-side validation - let server handle it
            // This prevents form submission issues

            // Optional: Add visual feedback for filled fields
            $('input, select').on('change blur', function () {
                if ($(this).val() !== '' && $(this).val() !== null) {
                    $(this).css('border-color', '#4CAF50');
                } else {
                    $(this).css('border-color', '#ddd');
                }
            });
        });
    </script>
</asp:Content>