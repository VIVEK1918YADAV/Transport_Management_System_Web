<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="modifyPassword.aspx.cs" Inherits="Excel_Bus.modifyPassword" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password - ExcelBus
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- ✅ ADD THIS ScriptManager -->
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
   
</asp:ScriptManagerProxy>


    <style>
        .change-password-section {
            padding: 60px 0;
            background: #f8f9fa;
            min-height: calc(100vh - 200px);
        }

        .password-card {
            background: white;
            border-radius: 15px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
            overflow: hidden;
            max-width: 600px;
            margin: 0 auto;
        }

        .card-header-custom {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }

        .card-header-custom h3 {
            margin: 0;
            font-size: 24px;
            font-weight: 600;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }

        .card-header-custom p {
            margin: 8px 0 0 0;
            opacity: 0.9;
            font-size: 14px;
        }

        .card-body-custom {
            padding: 40px 35px;
        }

        .form-group-custom {
            margin-bottom: 25px;
        }

        .form-label-custom {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #2c3e50;
            font-size: 14px;
        }

        .required-star {
            color: #dc3545;
            margin-left: 3px;
        }

        .password-input-wrapper {
            position: relative;
        }

        .form-control-custom {
            width: 100%;
            padding: 13px 45px 13px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 15px;
            transition: all 0.3s ease;
            box-sizing: border-box;
        }

        .form-control-custom:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }

        .password-toggle {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: #6c757d;
            font-size: 18px;
            user-select: none;
            z-index: 10;
        }

        .password-toggle:hover {
            color: #495057;
        }

        .btn-submit-custom {
            width: 100%;
            padding: 15px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: all 0.3s ease;
            margin-top: 10px;
        }

        .btn-submit-custom:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
        }

        .btn-submit-custom:active {
            transform: translateY(0);
        }

        .password-requirements {
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            border-radius: 8px;
            margin-top: 25px;
        }

        .password-requirements h4 {
            margin: 0 0 15px 0;
            font-size: 15px;
            color: #2c3e50;
            font-weight: 600;
        }

        .password-requirements ul {
            margin: 0;
            padding-left: 20px;
        }

        .password-requirements ul li {
            margin-bottom: 8px;
            font-size: 14px;
            color: #6c757d;
        }

        .alert-custom {
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 25px;
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

        .alert-success-custom {
            background: #d1f4e0;
            color: #00a854;
            border: 1px solid #00a854;
        }

        .alert-error-custom {
            background: #ffe0e0;
            color: #dc3545;
            border: 1px solid #dc3545;
        }

        .back-link {
            text-align: center;
            margin-top: 20px;
        }

        .back-link a {
            color: #667eea;
            text-decoration: none;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

        .back-link a:hover {
            text-decoration: underline;
        }

        @media (max-width: 768px) {
            .card-header-custom {
                padding: 25px 20px;
            }

            .card-body-custom {
                padding: 30px 20px;
            }

            .change-password-section {
                padding: 30px 0;
            }
        }
    </style>

    <section class="change-password-section">
        <div class="container">
            <div class="password-card">
                <div class="card-header-custom">
                    <h3>
                        <i class="fas fa-lock"></i>
                        Change Password
                    </h3>
                    <p>Keep your account secure by updating your password regularly</p>
                </div>

                <div class="card-body-custom">
                    <asp:Panel ID="pnlError" runat="server" CssClass="alert-custom alert-error-custom" Visible="false">
                        <asp:Label ID="lblError" runat="server"></asp:Label>
                    </asp:Panel>

                    <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert-custom alert-success-custom" Visible="false">
                        <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                    </asp:Panel>

                    <asp:HiddenField ID="hfUserId" runat="server" />

                    <div class="form-group-custom">
                        <label class="form-label-custom">
                            Current Password <span class="required-star">*</span>
                        </label>
                        <div class="password-input-wrapper">
                            <asp:TextBox ID="txtOldPassword" runat="server" 
                                CssClass="form-control-custom" 
                                TextMode="Password" 
                                placeholder="Enter your current password"></asp:TextBox>
                            <span class="password-toggle" onclick="togglePasswordField('txtOldPassword')">
                                <i class="fas fa-eye"></i>
                            </span>
                        </div>
                    </div>

                    <div class="form-group-custom">
                        <label class="form-label-custom">
                            New Password <span class="required-star">*</span>
                        </label>
                        <div class="password-input-wrapper">
                            <asp:TextBox ID="txtNewPassword" runat="server" 
                                CssClass="form-control-custom" 
                                TextMode="Password" 
                                placeholder="Enter your new password"></asp:TextBox>
                            <span class="password-toggle" onclick="togglePasswordField('txtNewPassword')">
                                <i class="fas fa-eye"></i>
                            </span>
                        </div>
                    </div>

                    <div class="form-group-custom">
                        <label class="form-label-custom">
                            Confirm New Password <span class="required-star">*</span>
                        </label>
                        <div class="password-input-wrapper">
                            <asp:TextBox ID="txtConfirmPassword" runat="server" 
                                CssClass="form-control-custom" 
                                TextMode="Password" 
                                placeholder="Re-enter your new password"></asp:TextBox>
                            <span class="password-toggle" onclick="togglePasswordField('txtConfirmPassword')">
                                <i class="fas fa-eye"></i>
                            </span>
                        </div>
                    </div>

                    <div class="password-requirements">
                        <h4><i class="fas fa-info-circle"></i> Password Requirements:</h4>
                        <ul>
                            <li>Minimum 6 characters long</li>
                            <li>Must be different from current password</li>
                            <li>New password and confirm password must match</li>
                        </ul>
                    </div>

                    <asp:Button ID="btnChangePassword" runat="server" 
                        Text="Change Password" 
                        CssClass="btn-submit-custom" 
                        OnClick="btnChangePassword_Click" 
                        OnClientClick="return validatePasswordForm();" />

                    <div class="back-link">
                        <a href="Profile.aspx">
                            <i class="fas fa-arrow-left"></i>
                            Back to Profile
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function togglePasswordField(inputId) {
            var input = document.getElementById('<%= txtOldPassword.ClientID %>');
            var icon = null;

            if (inputId === 'txtOldPassword') {
                input = document.getElementById('<%= txtOldPassword.ClientID %>');
                icon = event.currentTarget.querySelector('i');
            } else if (inputId === 'txtNewPassword') {
                input = document.getElementById('<%= txtNewPassword.ClientID %>');
                icon = event.currentTarget.querySelector('i');
            } else if (inputId === 'txtConfirmPassword') {
                input = document.getElementById('<%= txtConfirmPassword.ClientID %>');
                icon = event.currentTarget.querySelector('i');
            }

            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        }

        function validatePasswordForm() {
            var oldPassword = document.getElementById('<%= txtOldPassword.ClientID %>').value.trim();
            var newPassword = document.getElementById('<%= txtNewPassword.ClientID %>').value.trim();
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value.trim();

            if (!oldPassword) {
                notify('error', 'Please enter your current password');
                return false;
            }

            if (!newPassword) {
                notify('error', 'Please enter new password');
                return false;
            }

            if (newPassword.length < 6) {
                notify('error', 'New password must be at least 6 characters long');
                return false;
            }

            if (!confirmPassword) {
                notify('error', 'Please confirm your new password');
                return false;
            }

            if (newPassword !== confirmPassword) {
                notify('error', 'New password and confirm password do not match');
                return false;
            }

            if (oldPassword === newPassword) {
                notify('error', 'New password must be different from current password');
                return false;
            }

            return true;
        }
    </script>
</asp:Content>