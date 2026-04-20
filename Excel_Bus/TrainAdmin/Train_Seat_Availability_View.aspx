<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_Seat_Availability_View.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_Seat_Availability_View" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .seat-container { border: 2px solid #ddd; padding: 20px; border-radius: 10px; background: #f9f9f9; margin-top: 20px; }
        .seat-row { display: flex; justify-content: center; margin-bottom: 10px; }
        .seat {
            width: 40px; height: 40px; margin: 5px; display: flex; align-items: center;
            justify-content: center; border-radius: 5px; font-weight: bold; color: #fff; cursor: default;
        }
        .available { background-color: #28a745 !important; } /* Green */
        .booked { background-color: #007bff !important; }    /* Blue */
        .aisle { width: 30px; }
        .legend { display: flex; gap: 20px; margin-bottom: 15px; }
        .legend-item { display: flex; align-items: center; gap: 5px; }
    </style>

    <div class="container mt-4">
        <h2>Train Seat Availability Check</h2>
        <hr />
        
        <div class="row">
            <div class="col-md-3">
                <label>Select Date:</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" AutoPostBack="true" OnTextChanged="FilterChanged"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <label>Select Train:</label>
                <asp:DropDownList ID="ddlTrains" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTrains_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label>Coach Type:</label>
                <asp:DropDownList ID="ddlCoachType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged"></asp:DropDownList>
            </div>
        </div>

        <div class="seat-container">
            <div class="legend">
                <div class="legend-item"><div class="seat available" style="width:20px;height:20px"></div> Available</div>
                <div class="legend-item"><div class="seat booked" style="width:20px;height:20px"></div> Booked (Blue)</div>
            </div>

            <asp:Label ID="lblInfo" runat="server" CssClass="text-info font-weight-bold"></asp:Label>
            <asp:Panel ID="pnlSeats" runat="server" CssClass="mt-3"></asp:Panel>
        </div>
    </div>
</asp:Content>

