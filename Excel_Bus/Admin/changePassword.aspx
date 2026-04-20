<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="changePassword.aspx.cs" Inherits="Excel_Bus.Admin.changePassword" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .change-password-page {
            padding: 20px;
            background: #f5f7fa;
            margin-top: 50px;
            min-height: calc(100vh - 100px);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .password-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 500px;
            overflow: hidden;
        }

        .card-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 25px;
            text-align: center;
        }

        .card-title {
            font-size: 24px;
            font-weight: 600;
            margin: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }

        .card-body {
            padding: 30px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: #2c3e50;
            font-size: 14px;
        }

        .required {
            color: #dc3545;
        }

        .password-input-wrapper {
            position: relative;
        }

        .form-control {
            width: 100%;
            padding: 12px 45px 12px 15px;
            border: 1px solid #ced4da;
            border-radius: 6px;
            font-size: 14px;
            box-sizing: border-box;
            transition: all 0.3s ease;
        }

            .form-control:focus {
                outline: none;
                border-color: #667eea;
                box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
            }

        .toggle-password {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: #6c757d;
            font-size: 18px;
            user-select: none;
        }

            .toggle-password:hover {
                color: #495057;
            }

        .btn-submit {
            width: 100%;
            padding: 14px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: transform 0.2s ease;
            margin-top: 10px;
        }

            .btn-submit:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
            }

            .btn-submit:active {
                transform: translateY(0);
            }

        .alert {
            padding: 12px 20px;
            border-radius: 6px;
            margin-bottom: 20px;
            display: none;
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

        .alert.show {
            display: block;
        }

        .alert-success {
            background: #d1f4e0;
            color: #00a854;
            border: 1px solid #00a854;
        }

        .alert-error {
            background: #ffe0e0;
            color: #dc3545;
            border: 1px solid #dc3545;
        }

        .password-requirements {
            background: #f8f9fa;
            border-left: 3px solid #667eea;
            padding: 15px;
            border-radius: 4px;
            margin-top: 20px;
            font-size: 13px;
            color: #6c757d;
        }

            .password-requirements h4 {
                margin: 0 0 10px 0;
                font-size: 14px;
                color: #495057;
            }

            .password-requirements ul {
                margin: 0;
                padding-left: 20px;
            }

                .password-requirements ul li {
                    margin-bottom: 5px;
                }

        .success-message {
            position: fixed;
            top: 20px;
            right: 20px;
            background: #d1f4e0;
            color: #00a854;
            padding: 15px 25px;
            border-radius: 6px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }

            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="change-password-page">
        <div class="password-card">
            <div class="card-header">
                <h2 class="card-title">
                    🔒 Change Password
                </h2>
            </div>

            <div class="card-body">
                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:HiddenField ID="hfAdminId" runat="server" />

                <div class="form-group">
                    <label class="form-label">Old Password <span class="required">*</span></label>
                    <div class="password-input-wrapper">
                        <asp:TextBox ID="txtOldPassword" runat="server" 
                            CssClass="form-control" 
                            TextMode="Password" 
                            placeholder="Enter your old password"></asp:TextBox>
                        <span class="toggle-password" onclick="togglePassword('txtOldPassword')">👁️</span>
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">New Password <span class="required">*</span></label>
                    <div class="password-input-wrapper">
                        <asp:TextBox ID="txtNewPassword" runat="server" 
                            CssClass="form-control" 
                            TextMode="Password" 
                            placeholder="Enter your new password"></asp:TextBox>
                        <span class="toggle-password" onclick="togglePassword('txtNewPassword')">👁️</span>
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">Confirm Password <span class="required">*</span></label>
                    <div class="password-input-wrapper">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" 
                            CssClass="form-control" 
                            TextMode="Password" 
                            placeholder="Re-enter your new password"></asp:TextBox>
                        <span class="toggle-password" onclick="togglePassword('txtConfirmPassword')">👁️</span>
                    </div>
                </div>

                <div class="password-requirements">
                    <h4>Password Requirements:</h4>
                    <ul>
                        <li>Minimum 6 characters long</li>
                        <li>Should be different from old password</li>
                        <li>New password and confirm password must match</li>
                    </ul>
                </div>

                <asp:Button ID="btnChangePassword" runat="server" 
                    Text="Change Password" 
                    CssClass="btn-submit" 
                    OnClick="btnChangePassword_Click" 
                    OnClientClick="return validatePasswordForm();" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function togglePassword(inputId) {
            var input = document.getElementById('<%= txtOldPassword.ClientID %>');
            
            if (inputId === 'txtOldPassword') {
                input = document.getElementById('<%= txtOldPassword.ClientID %>');
            } else if (inputId === 'txtNewPassword') {
                input = document.getElementById('<%= txtNewPassword.ClientID %>');
            } else if (inputId === 'txtConfirmPassword') {
                input = document.getElementById('<%= txtConfirmPassword.ClientID %>');
            }

            if (input.type === 'password') {
                input.type = 'text';
            } else {
                input.type = 'password';
            }
        }

        function validatePasswordForm() {
            var oldPassword = document.getElementById('<%= txtOldPassword.ClientID %>').value.trim();
            var newPassword = document.getElementById('<%= txtNewPassword.ClientID %>').value.trim();
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value.trim();

            if (!oldPassword) {
                alert('Please enter your old password');
                return false;
            }

            if (!newPassword) {
                alert('Please enter new password');
                return false;
            }

            if (newPassword.length < 6) {
                alert('New password must be at least 6 characters long');
                return false;
            }

            if (!confirmPassword) {
                alert('Please confirm your new password');
                return false;
            }

            if (newPassword !== confirmPassword) {
                alert('New password and confirm password do not match');
                return false;
            }

            if (oldPassword === newPassword) {
                alert('New password must be different from old password');
                return false;
            }

            return true;
        }

        function showSuccess(message) {
            var successDiv = document.createElement('div');
            successDiv.className = 'success-message';
            successDiv.innerHTML = '✓ ' + message;
            document.body.appendChild(successDiv);

            setTimeout(function () {
                successDiv.style.opacity = '0';
                successDiv.style.transition = 'opacity 0.3s ease-out';
                setTimeout(function () {
                    if (successDiv.parentNode) {
                        successDiv.parentNode.removeChild(successDiv);
                    }
                }, 300);
            }, 3000);
        }

        window.onload = function () {
            var successPanel = document.getElementById('<%= pnlSuccess.ClientID %>');
            var errorPanel = document.getElementById('<%= pnlError.ClientID %>');

            if (successPanel && successPanel.style.display !== 'none') {
                successPanel.classList.add('show');
            }

            if (errorPanel && errorPanel.style.display !== 'none') {
                errorPanel.classList.add('show');
            }
        }
    </script>
</asp:Content>