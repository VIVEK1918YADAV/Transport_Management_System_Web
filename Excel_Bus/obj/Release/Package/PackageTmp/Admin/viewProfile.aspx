<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="viewProfile.aspx.cs" Inherits="Excel_Bus.Admin.viewProfile" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-page {
            padding: 20px;
            background: #f5f7fa;
            margin-top: 50px;
            min-height: calc(100vh - 100px);
        }

        .profile-container {
            max-width: 900px;
            margin: 0 auto;
        }

        .page-header {
            margin-bottom: 30px;
        }

        .page-title {
            font-size: 28px;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .profile-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .profile-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 40px 30px;
            text-align: center;
            position: relative;
        }

        .profile-avatar {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            background: white;
            margin: 0 auto 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
            font-weight: 600;
            color: #667eea;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        }

        .profile-name {
            font-size: 24px;
            font-weight: 600;
            color: white;
            margin: 0 0 8px 0;
        }

        .profile-username {
            font-size: 16px;
            color: rgba(255, 255, 255, 0.9);
            margin: 0;
        }

        .profile-body {
            padding: 30px;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 25px;
        }

        .info-item {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .info-label {
            font-size: 13px;
            font-weight: 600;
            color: #6c757d;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .info-value {
            font-size: 16px;
            color: #2c3e50;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .info-icon {
            font-size: 18px;
        }

        .status-badge {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
        }

        .status-active {
            background: #d1f4e0;
            color: #00a854;
        }

        .status-inactive {
            background: #ffe0e0;
            color: #ff4444;
        }

        .divider {
            height: 1px;
            background: #e9ecef;
            margin: 25px 0;
        }

        .actions-section {
            padding: 20px 30px;
            background: #f8f9fa;
            border-top: 1px solid #e9ecef;
            display: flex;
            gap: 15px;
            justify-content: center;
            flex-wrap: wrap;
        }

        .btn {
            padding: 12px 24px;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            border: none;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
        }

        .btn-primary {
            background: #667eea;
            color: white;
        }

            .btn-primary:hover {
                background: #5568d3;
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
            }

        .btn-secondary {
            background: white;
            color: #667eea;
            border: 2px solid #667eea;
        }

            .btn-secondary:hover {
                background: #667eea;
                color: white;
                transform: translateY(-2px);
            }

        .alert {
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            animation: slideDown 0.3s ease-out;
        }

        @keyframes slideDown {
            from {
                transform: translateY(-10px);
                opacity: 0;
            }

            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .alert-error {
            background: #ffe0e0;
            color: #dc3545;
            border: 1px solid #dc3545;
        }

        .loading-spinner {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 60px;
            font-size: 18px;
            color: #6c757d;
        }

        .no-data {
            text-align: center;
            padding: 60px 20px;
            color: #6c757d;
        }

            .no-data h3 {
                font-size: 20px;
                margin-bottom: 10px;
            }

        @media (max-width: 768px) {
            .profile-page {
                padding: 15px;
            }

            .profile-header {
                padding: 30px 20px;
            }

            .profile-avatar {
                width: 100px;
                height: 100px;
                font-size: 40px;
            }

            .profile-name {
                font-size: 20px;
            }

            .profile-body {
                padding: 20px;
            }

            .info-grid {
                grid-template-columns: 1fr;
            }

            .actions-section {
                flex-direction: column;
            }

            .btn {
                width: 100%;
                justify-content: center;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-page">
        <div class="profile-container">
            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <div class="page-header">
                <h1 class="page-title">My Profile</h1>
            </div>

            <asp:Panel ID="pnlProfile" runat="server" Visible="false">
                <div class="profile-card">
                    <%--<div class="profile-header">
                        <div class="profile-avatar">
                            <asp:Label ID="lblInitials" runat="server"></asp:Label>
                        </div>
                        <h2 class="profile-name">
                            <asp:Label ID="lblName" runat="server"></asp:Label>
                        </h2>
                        <p class="profile-username">
                            @<asp:Label ID="lblUsername" runat="server"></asp:Label>
                        </p>
                    </div>--%>

                    <div class="profile-body">
                        <div class="info-grid">
                            <div class="info-item">
                                <span class="info-label">📧 Email Address</span>
                                <span class="info-value">
                                    <asp:Label ID="lblEmail" runat="server"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">📱 Mobile Number</span>
                                <span class="info-value">
                                    <asp:Label ID="lblMobile" runat="server"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">👤 Username</span>
                                <span class="info-value">
                                    <asp:Label ID="lblUsernameValue" runat="server"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">🎯 Role ID</span>
                                <span class="info-value">
                                    <asp:Label ID="lblRoleId" runat="server"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">✅ Status</span>
                                <span class="info-value">
                                    <asp:Label ID="lblStatus" runat="server" CssClass="status-badge"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">📅 Member Since</span>
                                <span class="info-value">
                                    <asp:Label ID="lblCreatedAt" runat="server"></asp:Label>
                                </span>
                            </div>
                        </div>

                        <div class="divider"></div>

                        <div class="info-grid">
                            <div class="info-item">
                                <span class="info-label">🔄 Last Updated</span>
                                <span class="info-value">
                                    <asp:Label ID="lblUpdatedAt" runat="server"></asp:Label>
                                </span>
                            </div>

                            <div class="info-item">
                                <span class="info-label">📧 Email Verified</span>
                                <span class="info-value">
                                    <asp:Label ID="lblEmailVerified" runat="server"></asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="actions-section">
                       <%-- <a href="editProfile.aspx" class="btn btn-primary">
                            ✏️ Edit Profile
                        </a>--%>
                        <a href="changePassword.aspx" class="btn btn-secondary">
                            🔒 Change Password
                        </a>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlLoading" runat="server" Visible="true">
                <div class="loading-spinner">
                    Loading profile...
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // Auto hide error messages after 5 seconds
        window.onload = function () {
            var errorPanel = document.getElementById('<%= pnlError.ClientID %>');
            if (errorPanel && errorPanel.style.display !== 'none') {
                setTimeout(function () {
                    errorPanel.style.opacity = '0';
                    errorPanel.style.transition = 'opacity 0.3s ease-out';
                    setTimeout(function () {
                        errorPanel.style.display = 'none';
                    }, 300);
                }, 5000);
            }
        }
    </script>
</asp:Content>