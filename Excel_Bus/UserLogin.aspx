<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="ExcelBus.UserLogin" Async="true" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelBus - Login</title>
    <meta name="title" content="ExcelBus - Login">
    <meta name="description" content="Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit">
    <meta name="keywords" content="bus booking system">
    <link rel="shortcut icon" href="img/excel_bus_logo.png" type="image/x-icon">
    <!-- BootStrap Link -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <!-- Icon Link -->
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />
    <!-- Custom Link -->
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />
    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />

    <style>
        .social-login-btn {
            border: 1px solid #cbc4c4;
            color: hsl(var(--black)) !important;
            display: flex !important;
            justify-content: center;
            align-items: center;
            gap: 10px;
            font-size: 14px;
            padding: 7px 10px;
            transition: .2s linear;
            line-height: 1;
        }

        .social-login-btn:hover {
            border-color: var(--main-color) !important;
            color: var(--main-color) !important;
        }

        .another-login {
            position: relative;
            z-index: 1;
        }

        .another-login__or {
            background-color: #fff;
            padding: 0 7px;
        }

        .another-login::after {
            position: absolute;
            content: '';
            top: 50%;
            left: 0;
            width: 100%;
            border-bottom: 1px dashed #cbc4c4;
            z-index: -1;
        }

        .account-wrapper {
            min-height: 100vh;
            background: #fff;
            padding-left: 120px;
            padding-right: 120px;
            width: 100%;
            position: relative;
            max-width: 720px;
            margin-left: auto;
            display: -webkit-box;
            display: -ms-flexbox;
            display: flex;
            -webkit-box-align: center;
            -ms-flex-align: center;
            align-items: center;
            padding-top: 50px;
            padding-bottom: 50px;
        }

        .account-form-wrapper {
            width: 100%;
            max-height: calc(100vh - 100px);
            overflow-y: auto;
        }

        .register-fields {
            display: none;
        }

        /* Tab Styles */
        .login-tabs {
            display: flex;
            margin-bottom: 30px;
            border-bottom: 2px solid #e0e0e0;
        }

        .login-tab {
            flex: 1;
            padding: 15px 20px;
            text-align: center;
            background: transparent;
            border: none;
            font-size: 16px;
            font-weight: 600;
            color: #666;
            cursor: pointer;
            position: relative;
            transition: all 0.3s ease;
            text-decoration: none;
        }

        .login-tab:hover {
            color: var(--main-color);
        }

        .login-tab.active {
            color: var(--main-color);
        }

        .login-tab.active::after {
            content: '';
            position: absolute;
            bottom: -2px;
            left: 0;
            right: 0;
            height: 2px;
            background: #47a17b;
        }

        .tab-content {
            display: none;
        }

        .tab-content.active {
            display: block;
        }

        @media (max-width: 991px) {
            .account-wrapper {
                padding-left: 30px;
                padding-right: 30px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <!-- Google Translate -->
        <div id="google_translate_element"></div>

        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en',
                    layout: google.translate.TranslateElement.InlineLayout.HORIZONTAL,
                    includedLanguages: 'en,fr,hi,de,es'
                }, 'google_translate_element');
            }
        </script>

        <script type="text/javascript"
            src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit">
        </script>

        <div class="overlay"></div>
        
        <!-- Preloader -->
        <div class="preloader">
            <div class="loader-wrapper">
                <div class="truck-wrapper">
                    <div class="preloader-content"></div>
                    <div class="truck">
                        <div class="truck-container"></div>
                        <div class="glases"></div>
                        <div class="bonet"></div>
                        <div class="base"></div>
                        <div class="base-aux"></div>
                        <div class="wheel-back"></div>
                        <div class="wheel-front"></div>
                        <div class="smoke"></div>
                    </div>
                </div>
            </div>
        </div>

        <section class="account-section bg_img" style="background: url('img/login_bg.jpg') bottom left;">
            <span class="spark"></span>
            <span class="spark2"></span>
            <div class="account-wrapper sign-up">
                <div class="account-form-wrapper">
                    <div class="account-header">
                        <div class="logo mb-4">
                            <a href="">
                                <img src="img/excel_bus_logo.png" alt="Logo">
                            </a>
                        </div>
                        <h3 class="account-title" id="pageTitle">Login to Your Account</h3>
                    </div>

                    <!-- Login Tabs -->
                    <div class="login-tabs">
                        <a href="javascript:void(0);" class="login-tab active" onclick="switchLoginTab('user')">
                            <i class="fas fa-user"></i> User Login
                        </a>
                        <a href="Admin/Admin" class="login-tab">
                            <i class="fas fa-user-shield"></i> Admin Login
                        </a>
                    </div>

                    <!-- User Login Tab Content -->
                    <div id="userLoginTab" class="tab-content active">
                        <!-- Login/Register Form -->
                        <div class="account-form row">
                            <!-- Registration Fields (Hidden by default) -->
                            <div class="col-lg-6 form-group form--group register-fields">
                                <label for="txtFirstname" class="required">First Name</label>
                                <asp:TextBox ID="txtFirstname" runat="server" CssClass="form--control" 
                                    placeholder="Enter First Name"></asp:TextBox>
                            </div>

                            <div class="col-lg-6 form-group form--group register-fields">
                                <label for="txtLastname" class="required">Last Name</label>
                                <asp:TextBox ID="txtLastname" runat="server" CssClass="form--control" 
                                    placeholder="Enter Last Name"></asp:TextBox>
                            </div>

                            <!-- Username Field (Both Login and Register) -->
                            <div class="col-lg-12 form-group form--group">
                                <label for="txtUsername" class="required">Username</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form--control" 
                                    placeholder="Enter Your username"></asp:TextBox>
                            </div>

                            <!-- Email Field (Register Only) -->
                            <div class="col-lg-12 form-group form--group register-fields">
                                <label for="txtEmail" class="required">E-Mail Address</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form--control" 
                                    placeholder="Enter Your Email" TextMode="Email"></asp:TextBox>
                            </div>

                            <!-- Password Field (Both Login and Register) -->
                            <div class="col-lg-12 form-group form--group">
                                <label for="txtPassword" class="required">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                                    CssClass="form--control" placeholder="Enter Your Password"></asp:TextBox>
                            </div>

                            <!-- Confirm Password (Register Only) -->
                            <div class="col-lg-12 form-group form--group register-fields">
                                <label for="txtConfirmPassword" class="required">Confirm Password</label>
                                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" 
                                    CssClass="form--control" placeholder="Confirm Your Password"></asp:TextBox>
                            </div>

                            <!-- Terms and Conditions (Register Only) -->
                            <div class="col-lg-12 form-group form--group register-fields">
                                <div class="custom--checkbox">
                                    <asp:CheckBox ID="chkAgreeTerms" runat="server" />
                                    <label for="chkAgreeTerms">I agree with <a href="#">Privacy Policy</a>, <a href="#">Terms of Service</a></label>
                                </div>
                            </div>

                            <!-- Submit Button -->
                            <div class="col-md-12 form-group form--group">
                                <asp:Button ID="btnSubmit" runat="server" Text="Sign In" 
                                    CssClass="account-button w-100" OnClick="btnSubmit_Click" 
                                    OnClientClick="return validateForm();" />
                            </div>

                            <!-- Switch Link -->
                            <div class="col-md-12">
                                <div class="account-page-link">
                                    <p id="switchText">
                                        Don't have any Account? 
                                        <a href="javascript:void(0);" id="switchLink" onclick="toggleForm(); return false;">Sign Up</a>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <asp:HiddenField ID="hdnIsRegister" runat="server" Value="false" />
    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/iziToast.min.js"></script>

    <script>
        "use strict";
        const colors = {
            success: '#28c76f',
            error: '#eb2222',
            warning: '#ff9f43',
            info: '#1e9ff2',
        }

        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-times-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-exclamation-circle',
        }

        const triggerToaster = (status, message) => {
            iziToast[status]({
                title: status.charAt(0).toUpperCase() + status.slice(1),
                message: message,
                position: "topRight",
                backgroundColor: '#fff',
                icon: icons[status],
                iconColor: colors[status],
                progressBarColor: colors[status],
                titleSize: '1rem',
                messageSize: '1rem',
                titleColor: '#474747',
                messageColor: '#a2a2a2',
                transitionIn: 'obunceInLeft'
            });
        }

        function notify(status, message) {
            if (typeof message == 'string') {
                triggerToaster(status, message);
            } else {
                $.each(message, (i, val) => triggerToaster(status, val));
            }
        }

        // Client-side validation
        function validateForm() {
            var isRegister = $('#<%= hdnIsRegister.ClientID %>').val() === 'true';

            if (isRegister) {
                // Registration validation
                var firstname = $('#<%= txtFirstname.ClientID %>').val().trim();
                var lastname = $('#<%= txtLastname.ClientID %>').val().trim();
                var username = $('#<%= txtUsername.ClientID %>').val().trim();
                var email = $('#<%= txtEmail.ClientID %>').val().trim();
                var password = $('#<%= txtPassword.ClientID %>').val();
                var confirmPassword = $('#<%= txtConfirmPassword.ClientID %>').val();
                var agreeTerms = $('#<%= chkAgreeTerms.ClientID %>').is(':checked');

                if (!firstname) {
                    notify('warning', 'First name is required');
                    return false;
                }
                if (!lastname) {
                    notify('warning', 'Last name is required');
                    return false;
                }
                if (!username) {
                    notify('warning', 'Username is required');
                    return false;
                }
                if (!email) {
                    notify('warning', 'Email is required');
                    return false;
                }
                if (!password) {
                    notify('warning', 'Password is required');
                    return false;
                }
                if (password !== confirmPassword) {
                    notify('warning', 'Password and confirm password do not match');
                    return false;
                }
                if (!agreeTerms) {
                    notify('warning', 'Please agree to the terms and conditions');
                    return false;
                }
            } else {
                // Login validation
                var username = $('#<%= txtUsername.ClientID %>').val().trim();
                var password = $('#<%= txtPassword.ClientID %>').val();

                if (!username) {
                    notify('warning', 'Username is required');
                    return false;
                }
                if (!password) {
                    notify('warning', 'Password is required');
                    return false;
                }
            }

            return true;
        }

        // Switch between User and Admin login tabs
        function switchLoginTab(tabType) {
            if (tabType === 'user') {
                $('.login-tab').removeClass('active');
                $('.login-tab:first').addClass('active');
                $('#userLoginTab').addClass('active').show();
            }
        }

        // Toggle between Login and Register forms
        function toggleForm() {
            var isRegister = $('#<%= hdnIsRegister.ClientID %>').val() === 'true';

            if (isRegister) {
                // Switch to Login
                $('.register-fields').hide();
                $('#pageTitle').text('Login to Your Account');
                $('#<%= btnSubmit.ClientID %>').val('Sign In');
                $('#switchText').html('Don\'t have any Account? <a href="javascript:void(0);" onclick="toggleForm(); return false;">Sign Up</a>');
                $('#<%= hdnIsRegister.ClientID %>').val('false');

                // Clear registration fields
                $('#<%= txtFirstname.ClientID %>').val('');
                $('#<%= txtLastname.ClientID %>').val('');
                $('#<%= txtEmail.ClientID %>').val('');
                $('#<%= txtConfirmPassword.ClientID %>').val('');
                $('#<%= chkAgreeTerms.ClientID %>').prop('checked', false);
            } else {
                // Switch to Register
                $('.register-fields').show();
                $('#pageTitle').text('Create Your Account');
                $('#<%= btnSubmit.ClientID %>').val('Register');
                $('#switchText').html('Already have an Account? <a href="javascript:void(0);" onclick="toggleForm(); return false;">Login</a>');
                $('#<%= hdnIsRegister.ClientID %>').val('true');
            }
            
            return false;
        }

        // Check if we need to show register form on page load
        $(document).ready(function() {
            var isRegister = $('#<%= hdnIsRegister.ClientID %>').val() === 'true';
            if (isRegister) {
                $('.register-fields').show();
                $('#pageTitle').text('Create Your Account');
                $('#<%= btnSubmit.ClientID %>').val('Register');
                $('#switchText').html('Already have an Account? <a href="javascript:void(0);" onclick="toggleForm(); return false;">Login</a>');
            }
        });
    </script>
</body>
</html>
