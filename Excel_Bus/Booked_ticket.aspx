<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Booked_ticket.aspx.cs" Inherits="Excel_Bus.Booked_ticket" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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

        /* Style for the dropdown button */
.form-select {
    background-color: #f8f9fa;
    border: 1px solid #ccc;
    font-size: 14px;
    color: #495057;
}

.form-select option {
    padding: 10px;
}

    </style>
    <section class="inner-banner bg_img" style="background: url('img/bg_bus_img.jpg') center">
        
        <div class="container">
            <div class="inner-banner-content">
                <h2 class="title">Booking History</h2>
            </div>
        </div>
    </section>
 
  





    <br />
    <br />

<%--     <div class="booking-table-wrapper">
                        <asp:Repeater ID="rptBookings" runat="server">
                            <HeaderTemplate>
                                <table class="booking-table">
                                    <thead>
                                        <tr>
                                            <th>PNR Number</th>
                                            <th>AC / Non-AC</th>
                                            <th>Starting Point</th>
                                            <th>Dropping Point</th>
                                            <th>Journey Date</th>
                                            <th>Pickup Time</th>
                                            <th>Booked Seats</th>
                                            <th>Status</th>
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
                                    <td class="time" data-label="Pickup Time"><%# Eval("PickupTime") %></td>
                                    <td class="seats" data-label="Booked Seats"><%# Eval("SeatsDisplay") %></td>
                                    <td data-label="Status">
                                        <%# GetStatusBadge(Convert.ToInt32(Eval("Status"))) %>
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
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
          </div>
                         </section>
                        <!-- No Data Message -->
                        <div id="noDataMessage" style="display:none; text-align:center; padding:40px;">
                            <i class="las la-inbox" style="font-size:48px; color:#ccc;"></i>
                            <p style="color:#666; margin-top:10px;">No booking records found.</p>
                        </div>
                   
   --%>
    <div class="booking-table-wrapper">
    <asp:Repeater ID="rptBookings" runat="server">
        <HeaderTemplate>
            <table class="booking-table">
                <thead>
                    <tr>
                        <th>PNR Number</th>
                         <th>AC / Non-AC</th>
                        <th>Starting Point</th>
                        <th>Dropping Point</th>
                        <th>Journey Date</th>
                        <th>Booked Seats</th>
                        <th>Status</th>
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
                <%--<td class="time" data-label="Pickup Time"><%# Eval("PickupTime") %></td>--%>
                <td class="seats" data-label="Booked Seats"><%# Eval("SeatsDisplay") %></td>
                <td data-label="Status">
                
                    <%# GetStatusBadge(Eval("Status")) %>
                </td>
                <td data-label="Postponed">
                    <%# GetPostponedBadge(Convert.ToInt32(Eval("PostponeCount"))) %>
                </td>
                <td class="fare" data-label="Fare"><%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %> CDF</td>
              
                <td class="action" data-label="Action">
    <asp:DropDownList ID="ddlActions" runat="server" CssClass="form-select btn-sm" AutoPostBack="True" 
                      OnSelectedIndexChanged="ddlActions_SelectedIndexChanged">
        <asp:ListItem Text="Select Action" Value="" />
        <asp:ListItem Text="View" Value="View" />
        <asp:ListItem Text="Postpone" Value="Postpone" />
        <%--<asp:ListItem Text="Cancel" Value="Cancel" />--%>
       <%-- <asp:ListItem Text="Download" Value="Download" />--%>
    </asp:DropDownList>
                     <asp:HiddenField ID="hfPnrNumber" runat="server" Value='<%# Eval("PnrNumber") %>' />
</td>

            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

</div>




<!-- No Data Message -->
<div id="noDataMessage" style="display:none; text-align:center; padding:40px;">
    <i class="las la-inbox" style="font-size:48px; color:#ccc;"></i>
    <p style="color:#666; margin-top:10px;">No booking records found.</p>
</div>


  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
 
</asp:Content>