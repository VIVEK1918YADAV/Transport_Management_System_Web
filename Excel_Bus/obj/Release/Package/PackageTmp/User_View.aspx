<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User_View.aspx.cs" Inherits="Excel_Bus.User_View" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <asp:GridView ID="gvBookings" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered" GridLines="None">
    <Columns>
        <asp:BoundField DataField="PNR" HeaderText="PNR Number" />
        <asp:BoundField DataField="ACNonAC" HeaderText="AC / Non-AC" />
        <asp:BoundField DataField="StartingPoint" HeaderText="Starting Point" />
        <asp:BoundField DataField="DroppingPoint" HeaderText="Dropping Point" />
        <asp:BoundField DataField="JourneyDate" HeaderText="Journey Date" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="PickupTime" HeaderText="Pickup Time" />
        <asp:BoundField DataField="BookedSeats" HeaderText="Booked Seats" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="Postponed" HeaderText="Postponed" />
        <asp:BoundField DataField="Fare" HeaderText="Fare" DataFormatString="{0:C}" />
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-info btn-sm"
                    CommandArgument='<%# Eval("PnrNumber") %>' OnClick="btnView_Click" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
