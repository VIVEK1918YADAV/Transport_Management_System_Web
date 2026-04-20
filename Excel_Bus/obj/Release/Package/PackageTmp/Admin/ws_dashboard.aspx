<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ws_dashboard.aspx.cs" Inherits="Excel_Bus.Admin.ws_dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Include Bootstrap CSS for styling -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f4f6f9;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        .welcome-card {
            margin-top: 100px;
            padding: 40px;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            text-align: center;
        }
        .welcome-card h1 {
            font-size: 2.5rem;
            color: #007bff;
        }
        .welcome-card p {
            font-size: 1.2rem;
            color: #555555;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-6">
                <div class="welcome-card">
                    <h1>Welcome to Workshop</h1>
                    <p>Your gateway to manage vehicles workshop efficiently.</p>
                    <a href="ws_vehicle_list.aspx" class="btn btn-primary btn-lg mt-3">Go to your work.</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <!-- Include Bootstrap JS and optional Popper.js -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>
