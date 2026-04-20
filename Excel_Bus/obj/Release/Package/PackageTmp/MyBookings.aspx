<%@ Page Title="My Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyBookings.aspx.cs" Inherits="Excel_Bus.MyBookings" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    My Bookings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
   
    <style>
        .my-bookings-container {
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
        
        .page-header p {
            margin: 5px 0 0 0;
            opacity: 0.9;
        }
        
        .booking-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 25px;
            margin-bottom: 20px;
            transition: all 0.3s ease;
        }
        
        .booking-card:hover {
            box-shadow: 0 4px 16px rgba(0,0,0,0.15);
            transform: translateY(-2px);
        }
        
        .booking-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #f0f0f0;
        }
        
        .pnr-info {
            flex: 1;
        }
        
        .pnr-label {
            font-size: 12px;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 1px;
        }
        
        .pnr-number {
            font-size: 20px;
            font-weight: bold;
            color: #667eea;
            font-family: 'Courier New', monospace;
        }
        
        .status-badge {
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 600;
        }
        
        .status-booked {
            background: #4CAF50;
            color: white;
        }
        
        .status-cancelled {
            background: #dc3545;
            color: white;
        }
        
        .status-completed {
            background: #17a2b8;
            color: white;
        }
        
        .status-pending {
            background: #ffc107;
            color: #333;
        }
        
        .status-default {
            background: #6c757d;
            color: white;
        }
        
        .booking-details {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 20px;
        }
        
        .detail-item {
            display: flex;
            flex-direction: column;
        }
        
        .detail-label {
            font-size: 12px;
            color: #666;
            margin-bottom: 5px;
        }
        
        .detail-value {
            font-size: 15px;
            color: #333;
            font-weight: 600;
        }
        
        .route-value {
            color: #667eea;
        }
        
        .booking-actions {
            display: flex;
            gap: 10px;
            margin-top: 15px;
        }
        
        .btn-view-details {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 600;
            transition: all 0.3s ease;
        }
        
        .btn-view-details:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
            color: white;
        }
        
        .no-bookings {
            background: white;
            border-radius: 12px;
            padding: 60px 40px;
            text-align: center;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .no-bookings-icon {
            width: 120px;
            height: 120px;
            background: #f0f4ff;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 30px;
        }
        
        .no-bookings-icon i {
            font-size: 60px;
            color: #667eea;
        }
        
        .no-bookings h3 {
            color: #333;
            margin-bottom: 15px;
        }
        
        .no-bookings p {
            color: #666;
            margin-bottom: 30px;
        }
        
        .btn-book-now {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 15px 40px;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            transition: all 0.3s ease;
        }
        
        .btn-book-now:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(102, 126, 234, 0.4);
            color: white;
        }
        
        @media (max-width: 768px) {
            .booking-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 15px;
            }
            
            .booking-details {
                grid-template-columns: 1fr;
                gap: 15px;
            }
            
            .booking-actions {
                flex-direction: column;
            }
            
            .btn-view-details {
                width: 100%;
            }
        }
    </style>

    <div class="my-bookings-container">
        <div class="page-header">
            <div class="container">
                <h2><i class="fas fa-ticket-alt"></i> My Bookings</h2>
                <p>View and manage your bus ticket bookings</p>
            </div>
        </div>

        <div class="container">
            <!-- Bookings List -->
            <asp:Panel ID="pnlBookings" runat="server" Visible="true">
                <asp:Repeater ID="rptBookings" runat="server">
                    <ItemTemplate>
                        <div class="booking-card">
                            <div class="booking-header">
                                <div class="pnr-info">
                                    <div class="pnr-label">PNR Number</div>
                                    <div class="pnr-number"><%# Eval("pnrNumber") %></div>
                                </div>
                                <div>
                                    <span class="status-badge <%# GetStatusClass(Eval("status")) %>">
                                        <%# Eval("status") %>
                                    </span>
                                </div>
                            </div>

                            <div class="booking-details">
                                <div class="detail-item">
                                    <span class="detail-label">Route</span>
                                    <span class="detail-value route-value">
                                        <%# GetRoute(Eval("sourceDestination")) %>
                                    </span>
                                </div>

                                <div class="detail-item">
                                    <span class="detail-label">Journey Date</span>
                                    <span class="detail-value">
                                        <i class="far fa-calendar-alt"></i>
                                        <%# GetFormattedDate(Eval("dateOfJourney")) %>
                                    </span>
                                </div>

                                <div class="detail-item">
                                    <span class="detail-label">Seat(s)</span>
                                    <span class="detail-value">
                                        <i class="fas fa-chair"></i>
                                        <%# GetSeatNumbers(Eval("passengers")) %>
                                    </span>
                                </div>

                                <div class="detail-item">
                                    <span class="detail-label">Passengers</span>
                                    <span class="detail-value">
                                        <i class="fas fa-users"></i>
                                        <%# Eval("ticketCount") %>
                                    </span>
                                </div>

                                <div class="detail-item">
                                    <span class="detail-label">Total Amount</span>
                                    <span class="detail-value" style="color: #4CAF50;">
                                        <%# GetFormattedAmount(Eval("subTotal")) %>
                                    </span>
                                </div>

                                <div class="detail-item">
                                    <span class="detail-label">Booked On</span>
                                    <span class="detail-value">
                                        <%# GetFormattedDate(Eval("createdAt")) %>
                                    </span>
                                </div>
                            </div>

                            <div class="booking-actions">
                                <asp:Button ID="btnViewDetails" runat="server" 
                                           Text="View Details" 
                                           CssClass="btn btn-view-details"
                                           CommandArgument='<%# Eval("pnrNumber") %>'
                                           OnClick="btnViewDetails_Click" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <!-- No Bookings Message -->
            <asp:Panel ID="pnlNoBookings" runat="server" Visible="false">
                <div class="no-bookings">
                    <div class="no-bookings-icon">
                        <i class="fas fa-ticket-alt"></i>
                    </div>
                    <h3>No Bookings Yet</h3>
                    <p>You haven't made any bus ticket bookings yet. Start your journey now!</p>
                    <asp:Button ID="btnBookNew" runat="server" 
                               Text="Book a Ticket Now" 
                               CssClass="btn btn-book-now"
                               OnClick="btnBookNew_Click" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('My Bookings Page Loaded');
        });
    </script>
</asp:Content>