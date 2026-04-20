<%@ Page Title="Train Passenger Details" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_Passeneger_Details.aspx.cs" Inherits="Excel_Bus.Train_Passeneger_Details" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Train Passenger Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    
    <style>
        .passenger-container {
            padding: 40px 0;
        }
        
        .page-header {
            background: #47a17b;
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
            background: #47a17b !important;
            border-bottom: 2px solid #667eea;
        }
        
        .form-control {
            border-radius: 4px;
            border: 1px solid #ddd;
            padding: 8px;
        }
        
        .form-control:focus {
            border-color: #f2f7f6fc;
            box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
        }
        
        .summary-panel {
            background: #47a17b;
    border: 2px solid #2cb329;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 30px;
        }
        
        .contact-panel {
            background: #47a17b;
            border: 2px solid #48b792;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 30px;
        }
        .panel-default>.panel-heading {
    color: #051c02;
    background-color: #2d9b8b8a;
    border-color: #eff5ef;
}
        .panel-body {
    padding: 15px;
    background: #47a17b;
}
        .alert-info {
    color: #074e1b;
    background-color: #2d9b8b8a;
    border-color: #abe9cd;
}
        .summary-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #ddd;
        }
        
        .seat-item {
            display: flex;
            justify-content: space-between;
            padding: 12px;
            background: #47a17b;
            margin-bottom: 10px;
            border-radius: 8px;
            border-left: 4px solid #4CAF50;
        }
        
        .btn-confirm {
            background: #47a17b;
            color: white;
            border: none;
           padding: 15px 45px;
            font-size: 11px;
            font-weight: bold;
            border-radius: 5px;
            transition: all 0.3s ease;
        }
        
        .btn-confirm:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(102, 126, 234, 0.3);
            color:#47a17b;
        }
         @media (min-width: 1200px) {
    .container {
        width: 1350px;
    }
}
    </style>

    <div class="passenger-container">
        <div class="container">
            <div class="row">
                <div class="col-md-8">
                    <!-- Contact Details -->
                    <div class="contact-panel">
                        <h4 style="margin-top: 0; border-bottom: 2px solid #edf3ed; padding-bottom: 10px;">
                            <i class="fas fa-address-book"></i> Contact Details
                        </h4>
                        <p style="color:#fdf4f4; margin-bottom: 15px;">
                            <small>These details will be used for booking confirmation</small>
                        </p>

                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>User Name</label>
                                    <asp:TextBox ID="txtUserName" runat="server" 
                                                CssClass="form-control" 
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5;"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Email Address</label>
                                    <asp:TextBox ID="txtContactEmail" runat="server" 
                                                CssClass="form-control" 
                                                TextMode="Email"
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5;"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Mobile Number</label>
                                    <asp:TextBox ID="txtContactMobile" runat="server" 
                                                CssClass="form-control" 
                                                ReadOnly="true"
                                                style="background-color: #f5f5f5;"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Passenger Details -->
                    <h3 style="border-bottom: 2px solid #2d856c;padding-bottom: 10px; margin-bottom: 20px;">
                        <i class="fas fa-users"></i> Passenger Details
                    </h3>
                    <asp:HiddenField ID="hfUserEmail" runat="server" />
<asp:HiddenField ID="hfUserMobile" runat="server" />
<asp:HiddenField ID="hfUserName" runat="server" />
                    <asp:Panel ID="pnlPassengers" runat="server">
                    </asp:Panel>

                    <div class="text-center" style="margin-top: 30px;">
                        <asp:Button ID="btnConfirmBooking" runat="server" 
                                   Text="Confirm Booking & Proceed to Payment" 
                                   CssClass="btn btn-confirm btn-lg"
                                   OnClick="btnConfirmBooking_Click" />
                    </div>
                </div>

                <div class="col-md-4">
                    <!-- Booking Summary -->
                    <div class="summary-panel">
                        <h4 style="margin-top: 0; border-bottom: 2px solid #e4e5eb; padding-bottom: 10px;">
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
                        <strong>Note:</strong> Please ensure all details are correct.
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            console.log('Train Passenger Details Page Loaded');
        });
    </script>
    <script>
        $(document).ready(function () {
            // If contact fields are empty, fill from sessionStorage
            if (!$('#<%=txtContactEmail.ClientID%>').val()) {
                var userData = sessionStorage.getItem('userData');
                if (userData) {
                    var u = JSON.parse(userData);
                    $('#<%=txtContactEmail.ClientID%>').val(u.email || '');
            $('#<%=txtContactMobile.ClientID%>').val(u.mobile || '');
                    $('#<%=txtUserName.ClientID%>').val(u.username || u.userName || '');
                }
            }
        });
</script>
</asp:Content>
