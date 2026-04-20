<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainUserReg.aspx.cs" Inherits="ExcelBus.TrainUserReg" Async="true" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelBus - Train User Login</title>
    <meta name="title" content="ExcelBus - Train User Login">
    <meta name="description" content="Login to your ExcelBus Train account">
    <meta name="keywords" content="train booking system, railway booking">
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
        :root {
            --train-primary: #0f4601;
    --train-secondary: #446e04;
    --train-accent: #86eb58;
    --train-light: #f9fbfd;
    --train-dark: #0a5003;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            overflow-x: hidden;
        }

        .train-account-section {
            min-height: 100vh;
           background: #47a17b;
            position: relative;
            overflow: hidden;
        }

        .train-account-section::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: url('img/train_pattern.png') repeat;
            opacity: 0.05;
            z-index: 0;
        }

        .animated-bg {
            position: absolute;
            width: 100%;
            height: 100%;
            overflow: hidden;
            z-index: 0;
        }

        .train-spark {
            position: absolute;
            width: 3px;
            height: 3px;
            background: #47a17b;
            border-radius: 50%;
            animation: sparkle 3s infinite;
        }

        .train-spark:nth-child(1) { top: 20%; left: 20%; animation-delay: 0s; }
        .train-spark:nth-child(2) { top: 40%; left: 80%; animation-delay: 1s; }
        .train-spark:nth-child(3) { top: 60%; left: 40%; animation-delay: 2s; }
        .train-spark:nth-child(4) { top: 80%; left: 70%; animation-delay: 1.5s; }
        .train-spark:nth-child(5) { top: 30%; left: 60%; animation-delay: 0.5s; }

        @keyframes sparkle {
            0%, 100% { opacity: 0; transform: scale(0); }
            50% { opacity: 1; transform: scale(1.5); }
        }

        .train-container {
            position: relative;
            z-index: 1;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px 20px;
        }

        .train-card-wrapper {
            background: #b9ebd2;
    border-radius: 24px;
    box-shadow: 0 20px 60px rgb(241 241 241 / 30%);
            overflow: hidden;
            max-width: 1000px;
            width: 100%;
            display: flex;
            position: relative;
        }

        .train-left-section {
            flex: 1;
            background: #47a17b;
            padding: 60px 40px;
            color: white;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            position: relative;
            overflow: hidden;
        }

        .train-left-section::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -50%;
            width: 200%;
            height: 200%;
            background:#0a8355;
            animation: rotate 20s linear infinite;
        }

        @keyframes rotate {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }

        .train-logo-section {
            position: relative;
            z-index: 1;
            margin-bottom: 30px;
        }

        .train-logo-section img {
            width: 120px;
            height: 120px;
            object-fit: contain;
            filter: brightness(0) invert(1);
            animation: float 3s ease-in-out infinite;
        }

        @keyframes float {
            0%, 100% { transform: translateY(0px); }
            50% { transform: translateY(-10px); }
        }

        .train-left-content {
            position: relative;
            z-index: 1;
        }

        .train-left-content h2 {
            font-size: 31px;
    font-weight: 700;
    margin-bottom: 15px;
    color: #012606;
    text-shadow: 2px -4px 4px rgb(239 243 239);
        }

        .train-left-content p {
            font-size: 16px;
            opacity: 0.9;
            line-height: 1.6;
        }

        .train-features {
            margin-top: 30px;
            text-align: left;
        }

        .train-feature-item {
            display: flex;
            align-items: center;
            margin-bottom: 15px;
            font-size: 14px;
        }

        .train-feature-item i {
            font-size: 20px;
            margin-right: 12px;
            color: #ecf3e9;
        }

        .train-right-section {
            flex: 1;
            padding: 50px 40px;
            overflow-y: auto;
            max-height: 90vh;
        }

        .train-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .train-header h3 {
            font-size: 28px;
            font-weight: 700;
            color: #1c8b68;
            margin-bottom: 10px;
        }

        .train-header p {
            color:#145c46;
            font-size: 14px;
        }

        .train-tabs {
            display: flex;
            margin-bottom: 30px;
            border-bottom: 2px solid #e0e0e0;
            gap: 20px;
        }

        .train-tab {
            flex: 1;
            padding: 15px 20px;
            text-align: center;
            background: transparent;
            border: none;
            font-size: 15px;
            font-weight: 600;
            color: #0d4632;
            cursor: pointer;
            position: relative;
            transition: all 0.3s ease;
            text-decoration: none;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }

        .train-tab i {
            font-size: 18px;
        }

        .train-tab:hover {
            color: var(--train-primary);
        }

        .train-tab.active {
            color: var(--train-primary);
        }

        .train-tab.active::after {
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
            animation: fadeIn 0.3s ease;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .form--group {
            margin-bottom: 20px;
        }

        .form--group label {
            display: block;
            font-weight: 600;
            color: var(--train-dark);
            margin-bottom: 8px;
            font-size: 14px;
        }

        .form--group label.required::after {
            content: ' *';
            color: #dc3545;
        }

        .form--control {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            transition: all 0.3s ease;
            background:#47a17b;
        }

        .form--control:focus {
            outline: none;
            border-color: var(--train-primary);
            background: white;
            box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
        }

        .custom--checkbox {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 13px;
        }

        .custom--checkbox input[type="checkbox"] {
            width: 18px;
            height: 18px;
            cursor: pointer;
        }

        .custom--checkbox a {
            color: var(--train-primary);
            text-decoration: none;
            font-weight: 600;
        }

        .custom--checkbox a:hover {
            text-decoration: underline;
        }

        .account-button {
            width: 100%;
            padding: 14px 20px;
            background: #47a17b;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(0, 102, 204, 0.3);
        }

        .account-button:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0, 102, 204, 0.4);
        }

        .account-button:active {
            transform: translateY(0);
        }

        .account-page-link {
            text-align: center;
            margin-top: 20px;
            font-size: 14px;
            color: #55857a;
        }

        .account-page-link a {
            color: var(--train-primary);
            text-decoration: none;
            font-weight: 600;
            margin-left: 5px;
        }

        .account-page-link a:hover {
            text-decoration: underline;
        }

        .register-fields {
            display: none;
        }

        /* Google Translate Styling */
        #google_translate_element {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 1000;
            background: white;
            padding: 8px 12px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        /* Preloader */
        .preloader {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: white;
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .loader-wrapper {
            text-align: center;
        }

        .train-loader {
            width: 80px;
            height: 80px;
            border: 5px solid #f3f3f3;
            border-top: 5px solid var(--train-primary);
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        /* Responsive Design */
        @media (max-width: 991px) {
            .train-card-wrapper {
                flex-direction: column;
            }

            .train-left-section {
                padding: 40px 30px;
            }

            .train-right-section {
                padding: 40px 30px;
                max-height: none;
            }

            .train-logo-section img {
                width: 80px;
                height: 80px;
            }

            .train-left-content h2 {
                font-size: 24px;
            }

            .train-features {
                margin-top: 20px;
            }
        }

        @media (max-width: 576px) {
            .train-container {
                padding: 20px 10px;
            }

            .train-right-section {
                padding: 30px 20px;
            }

            .train-header h3 {
                font-size: 22px;
            }

            .train-tabs {
                gap: 10px;
            }

            .train-tab {
                font-size: 13px;
                padding: 12px 10px;
            }

            .train-tab i {
                font-size: 16px;
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

        <!-- Preloader -->
        <div class="preloader">
            <div class="loader-wrapper">
                <div class="train-loader"></div>
                <p style="margin-top: 20px; color: var(--train-primary); font-weight: 600;">Loading...</p>
            </div>
        </div>

        <section class="train-account-section">
            <div class="animated-bg">
                <span class="train-spark"></span>
                <span class="train-spark"></span>
                <span class="train-spark"></span>
                <span class="train-spark"></span>
                <span class="train-spark"></span>
            </div>

            <div class="train-container">
                <div class="train-card-wrapper">
                    <!-- Left Section -->
                    <div class="train-left-section">
                        <div class="train-logo-section">
                            <img src="img/excel_bus_logo.png" alt="ExcelTrain Logo">
                        </div>
                        <div class="train-left-content">
                            <h2>Welcome to<br/>Excel Train</h2>
                            <p>Book your train tickets with ease and comfort</p>
                            
                            <div class="train-features">
                                <div class="train-feature-item">
                                    <i class="fas fa-train"></i>
                                    <span>Book Train Tickets Instantly</span>
                                </div>
                                <div class="train-feature-item">
                                    <i class="fas fa-ticket-alt"></i>
                                    <span>Digital Ticket Management</span>
                                </div>
                                <div class="train-feature-item">
                                    <i class="fas fa-shield-alt"></i>
                                    <span>Secure & Safe Payments</span>
                                </div>
                                <div class="train-feature-item">
                                    <i class="fas fa-headset"></i>
                                    <span>24/7 Customer Support</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Right Section -->
                    <div class="train-right-section">
                        <div class="train-header">
                            <h3 id="pageTitle">Login to Your Account</h3>
                            <p id="pageSubtitle">Access your train booking dashboard</p>
                        </div>

                        <!-- Login Tabs -->
                        <div class="train-tabs">
                            <a href="javascript:void(0);" class="train-tab active" onclick="switchLoginTab('trainUser')">
                                <i class="fas fa-user"></i>
                                <span>Train User</span>
                            </a>
                            <a href="TrainAdminLogin" class="train-tab">
                                <i class="fas fa-user-shield"></i>
                                <span>Admin</span>
                            </a>
                        </div>

                        <!-- Train User Login Tab Content -->
                        <div id="trainUserTab" class="tab-content active">
                            <!-- Login/Register Form -->
                            <div class="train-form row">
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
                                        placeholder="Enter Your Username"></asp:TextBox>
                                </div>

                                <!-- Email Field (Register Only) -->
                                <div class="col-lg-12 form-group form--group register-fields">
                                    <label for="txtEmail" class="required">E-Mail Address</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form--control" 
                                        placeholder="Enter Your Email" TextMode="Email"></asp:TextBox>
                                </div>

                                <!-- Mobile Field (Register Only) -->
                                <div class="col-lg-12 form-group form--group register-fields">
                                    <label for="txtMobile" class="required">Mobile Number</label>
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form--control" 
                                        placeholder="Enter Mobile Number" TextMode="Phone"></asp:TextBox>
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
                                        <label for="chkAgreeTerms">I agree with <a href="#">Privacy Policy</a> & <a href="#">Terms of Service</a></label>
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

        // Hide preloader on page load
        $(window).on('load', function() {
            $('.preloader').fadeOut(500);
        });

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
                transitionIn: 'bounceInLeft'
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
                var mobile = $('#<%= txtMobile.ClientID %>').val().trim();
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
                if (!mobile) {
                    notify('warning', 'Mobile number is required');
                    return false;
                }
                if (!password) {
                    notify('warning', 'Password is required');
                    return false;
                }
                if (password.length < 6) {
                    notify('warning', 'Password must be at least 6 characters long');
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

        // Switch between Train User and Admin login tabs
        function switchLoginTab(tabType) {
            if (tabType === 'trainUser') {
                $('.train-tab').removeClass('active');
                $('.train-tab:first').addClass('active');
                $('#trainUserTab').addClass('active').show();
            }
        }

        // Toggle between Login and Register forms
        function toggleForm() {
            var isRegister = $('#<%= hdnIsRegister.ClientID %>').val() === 'true';

            if (isRegister) {
                // Switch to Login
                $('.register-fields').hide();
                $('#pageTitle').text('Login to Your Account');
                $('#pageSubtitle').text('Access your train booking dashboard');
                $('#<%= btnSubmit.ClientID %>').val('Sign In');
                $('#switchText').html('Don\'t have any Account? <a href="javascript:void(0);" onclick="toggleForm(); return false;">Sign Up</a>');
                $('#<%= hdnIsRegister.ClientID %>').val('false');

                // Clear registration fields
                $('#<%= txtFirstname.ClientID %>').val('');
                $('#<%= txtLastname.ClientID %>').val('');
                $('#<%= txtEmail.ClientID %>').val('');
                $('#<%= txtMobile.ClientID %>').val('');
                $('#<%= txtConfirmPassword.ClientID %>').val('');
                $('#<%= chkAgreeTerms.ClientID %>').prop('checked', false);
            } else {
                // Switch to Register
                $('.register-fields').show();
                $('#pageTitle').text('Create Your Account');
                $('#pageSubtitle').text('Join thousands of happy train travelers');
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
                $('#pageSubtitle').text('Join thousands of happy train travelers');
                $('#<%= btnSubmit.ClientID %>').val('Register');
                $('#switchText').html('Already have an Account? <a href="javascript:void(0);" onclick="toggleForm(); return false;">Login</a>');
            }
        });
    </script>
</body>
</html>
