<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="bank_deposit.aspx.cs" Inherits="Excel_Bus.bank_deposit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <section class="inner-banner bg_img" 
             style="background: url('img/bg_bus_img.jpg') center;">
        <div class="container">
            <div class="inner-banner-content">
                <h2 class="title">Confirm Payment</h2>
            </div>
           
        </div>
    </section>

    <div class="container padding-top padding-bottom">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="card custom--card">
                    <div class="card-header card-header-bg">
                        <h5 class="card-title">Confirm Payment</h5>
                    </div>
                    <div class="card-body">
                        <asp:Panel runat="server" ID="pnlAlert" CssClass="alert alert-primary" Visible="false">
                            <p class="mb-0">
                                <i class="las la-info-circle"></i> You are requesting 
                                <b><asp:Label ID="lblAmount" runat="server" Text="0.00"></asp:Label> CDF</b> to payment. Please pay
                                <b><asp:Label ID="lblAmountDuplicate" runat="server" Text="0.00"></asp:Label> CDF</b> for successful payment.
                            </p>
                        </asp:Panel>

                        <asp:Panel runat="server" ID="pnlForm" CssClass="viser-form">
                            <asp:Button ID="btnPayNow" runat="server" CssClass="btn btn--base w-100" Text="Pay Now" OnClick="btnPayNow_Click" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
