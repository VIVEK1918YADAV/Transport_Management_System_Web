<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="ExcelBus.Admin" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>ExcelBus - Admin Login</title>

    <link rel="shortcut icon" href="img/favicon.png" type="image/x-icon">
    <link href="https://fonts.googleapis.com/css2?family=Syne:wght@400;600;700;800&family=DM+Sans:wght@300;400;500&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/global/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/admin/css/vendor/bootstrap-toggle.min.css">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/global/css/all.min.css">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/global/css/line-awesome.min.css">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/global/css/select2.min.css">
    <link rel="stylesheet" href="https://excelbus.mavenxone.com/assets/admin/css/app.css">

    <style>
        :root {
           --gold: #f7f164;
    --gold-light: #f5cd66;
    --gold-dim: rgba(201, 168, 76, 0.18);
    --dark-deep: #123783;
    --dark-card: rgb(21 70 157 / 82%);
    --dark-border: rgb(243 217 143 / 22%);
    --text-muted: rgb(239 230 230 / 59%);
    --radius: 14px;
    --transition: 0.35s cubic-bezier(0.4, 0, 0.2, 1);
        }

        *, *::before, *::after { 
            box-sizing: border-box;
                                 position: relative; 
                                 margin: 0;
                                 padding: 0; 

        }

        body {
            font-family: 'DM Sans', sans-serif;
            background: var(--dark-deep);
            min-height: 100vh;
            overflow: hidden;
        }

        /* ── ANIMATED BACKGROUND ── */
        .login-main {
            position: relative;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #487de7;
            overflow: hidden;
        }

        /* Radial ambient glows */
        .login-main::before,
        .login-main::after {
            content: '';
            position: absolute;
            border-radius: 50%;
            pointer-events: none;
        }

        .login-main::before {
            width: 700px; height: 700px;
            top: -180px; left: -160px;
            background: radial-gradient(circle, rgba(201,168,76,0.12) 0%, transparent 70%);
            animation: pulseGlow 8s ease-in-out infinite;
        }

        .login-main::after {
            width: 500px; height: 500px;
            bottom: -120px; right: -100px;
            background: radial-gradient(circle, rgba(30,80,180,0.14) 0%, transparent 70%);
            animation: pulseGlow 10s ease-in-out 2s infinite;
        }

        @keyframes pulseGlow {
            0%,100% { transform: scale(1); opacity: 1; }
            50%      { transform: scale(1.15); opacity: 0.7; }
        }

        /* Floating grid lines */
        .grid-overlay {
            position: absolute;
            inset: 0;
            background-image:
                linear-gradient(rgba(201,168,76,0.04) 1px, transparent 1px),
                linear-gradient(90deg, rgba(201,168,76,0.04) 1px, transparent 1px);
            background-size: 60px 60px;
            pointer-events: none;
        }

        /* Floating orbs */
        .orb {
            position: absolute;
            border-radius: 50%;
            pointer-events: none;
            animation: floatOrb linear infinite;
        }
        .orb-1 { 
            width:8px;
            height:8px;
            background:var(--gold);
            top:15%; 
            left:25%; 
            opacity:0.6;
            animation-duration:14s;
            animation-delay:0s;

        }
        .orb-2 { 
            width:5px;
            height:5px;
            background:var(--gold-light); 
            top:65%;
            left:70%;
            opacity:0.4; 
            animation-duration:18s; 
            animation-delay:3s;

        }
        .orb-3 {
            width:10px;
            height:10px;
            background:rgba(30,130,255,0.7);
            top:80%; 
            left:15%;
            opacity:0.5;
            animation-duration:22s;
            animation-delay:6s;

        }
        .orb-4 { 
            width:6px; 
            height:6px; 
            background:var(--gold); 
            top:35%;
            left:85%;
            opacity:0.5; 
            animation-duration:16s;
            animation-delay:1s;

        }
        .orb-5 { 
            width:4px;
            height:4px; 
            background:var(--gold-light); 
            top:50%; 
            left:50%; 
            opacity:0.3;
            animation-duration:12s; 
            animation-delay:9s;

        }

        @keyframes floatOrb {
            0%   { transform: translate(0,0) scale(1); }
            25%  { transform: translate(40px,-60px) scale(1.3); }
            50%  { transform: translate(-30px,-120px) scale(0.8); }
            75%  { transform: translate(-70px,-60px) scale(1.1); }
            100% { transform: translate(0,0) scale(1); }
        }

        /* ── CARD ── */
        .login-area {
            position: relative;
            z-index: 10;
            animation: slideUp 0.7s cubic-bezier(0.4,0,0.2,1) both;
        }

        @keyframes slideUp {
            from { opacity: 0; transform: translateY(40px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .login-wrapper {
            background: var(--dark-card);
            backdrop-filter: blur(28px) saturate(1.4);
            -webkit-backdrop-filter: blur(28px) saturate(1.4);
            border: 1px solid var(--dark-border);
            border-radius: 24px;
            overflow: hidden;
            box-shadow:
                0 0 0 1px rgba(201,168,76,0.08),
                0 40px 80px rgba(0,0,0,0.6),
                0 0 60px rgba(201,168,76,0.06) inset;
        }

        /* Gold top stripe */
        .login-wrapper::before {
            content: '';
            display: block;
            height: 3px;
            background: linear-gradient(90deg, transparent, var(--gold), var(--gold-light), var(--gold), transparent);
            animation: shimmer 3s ease-in-out infinite;
            background-size: 200% 100%;
        }

        @keyframes shimmer {
            0%,100% { background-position: -100% 0; }
            50%      { background-position: 100% 0; }
        }

        /* ── HEADER ── */
        .login-wrapper__top {
            padding: 40px 44px 28px;
            position: relative;
        }

        .brand-row {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 18px;
        }

        .brand-icon {
            width: 46px; height: 46px;
            background: linear-gradient(135deg, var(--gold), var(--gold-light));
            border-radius: 12px;
            display: flex; align-items: center; justify-content: center;
            font-size: 20px; color: var(--dark-deep);
            box-shadow: 0 4px 20px rgba(201,168,76,0.4);
            flex-shrink: 0;
        }

        .brand-icon svg { width: 22px; height: 22px; }

        .title {
            font-family: 'Syne', sans-serif;
            font-size: 1.65rem;
            font-weight: 800;
            color: #fff;
            letter-spacing: -0.02em;
            line-height: 1.1;
        }

        .title strong {
            background: linear-gradient(135deg, var(--gold), var(--gold-light));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }

        .subtitle {
            font-size: 0.82rem;
            color: var(--text-muted);
            letter-spacing: 0.08em;
            text-transform: uppercase;
            font-weight: 500;
            margin-top: 4px;
        }

        /* Divider */
        .header-divider {
            height: 1px;
            background: linear-gradient(90deg, var(--dark-border), transparent);
            margin: 0 44px;
        }

        /* ── FORM BODY ── */
        .login-wrapper__body {
            padding: 32px 44px 44px;
        }

        .form-group {
            margin-bottom: 22px;
        }

        .form-group label {
            display: block;
            font-size: 0.72rem;
            font-weight: 600;
            letter-spacing: 0.1em;
            text-transform: uppercase;
            color: var(--gold-light);
            margin-bottom: 8px;
        }

        /* ── INPUTS ── */
        .form-control,
        select.form-control {
            width: 100%;
            background: rgba(255,255,255,0.04);
            border: 1px solid rgba(255,255,255,0.1);
            border-radius: var(--radius);
            color: #fff;
            font-family: 'DM Sans', sans-serif;
            font-size: 0.9rem;
            padding: 13px 18px;
            transition: border-color var(--transition), background var(--transition), box-shadow var(--transition);
            outline: none;
            -webkit-appearance: none;
            appearance: none;
        }

        /* Dropdown arrow */
        select.form-control {
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' fill='none'%3E%3Cpath d='M1 1l5 5 5-5' stroke='%23C9A84C' stroke-width='1.8' stroke-linecap='round' stroke-linejoin='round'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 16px center;
            padding-right: 42px;
            cursor: pointer;
        }

        select.form-control option {
            background: #0e1520;
            color: #fff;
        }

        .form-control:focus,
        select.form-control:focus {
            border-color: var(--gold);
            background: rgba(201,168,76,0.06);
            box-shadow: 0 0 0 3px rgba(201,168,76,0.12), 0 4px 20px rgba(201,168,76,0.08);
        }

        .form-control::placeholder { color: rgba(255,255,255,0.25); }

        /* Input icons wrapper (optional enhancement) */
        .input-icon-wrap {
            position: relative;
        }
        .input-icon-wrap .form-control {
            padding-left: 46px;
        }
        .input-icon-wrap .icon {
            position: absolute;
            left: 16px;
            top: 50%;
            transform: translateY(-50%);
            color: rgba(255,255,255,0.25);
            font-size: 0.95rem;
            transition: color var(--transition);
            pointer-events: none;
        }
        .input-icon-wrap:focus-within .icon {
            color: var(--gold);
        }

        /* ── BUTTON ── */
        .btn.cmn-btn {
            width: 100%;
            padding: 14px;
            border: none;
            border-radius: var(--radius);
            background: linear-gradient(135deg, var(--gold) 0%, var(--gold-light) 50%, var(--gold) 100%);
            background-size: 200% 100%;
            color: var(--dark-deep);
            font-family: 'Syne', sans-serif;
            font-size: 0.85rem;
            font-weight: 700;
            letter-spacing: 0.14em;
            text-transform: uppercase;
            cursor: pointer;
            position: relative;
            overflow: hidden;
            transition: background-position var(--transition), transform 0.15s, box-shadow var(--transition);
            box-shadow: 0 4px 24px rgba(201,168,76,0.35);
            margin-top: 8px;
        }

        .btn.cmn-btn::after {
            content: '';
            position: absolute;
            inset: 0;
            background: linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.25) 50%, transparent 100%);
            transform: translateX(-100%);
            transition: transform 0.5s ease;
        }

        .btn.cmn-btn:hover {
            background-position: 100% 0;
            transform: translateY(-2px);
            box-shadow: 0 8px 36px rgba(201,168,76,0.45);
        }
        .btn.cmn-btn:hover::after {
            transform: translateX(100%);
        }
        .btn.cmn-btn:active {
            transform: translateY(0);
            box-shadow: 0 2px 12px rgba(201,168,76,0.3);
        }

        /* ── ERROR LABEL ── */
        .text-danger {
            font-size: 0.8rem;
            color: #ff6b6b !important;
            background: rgba(255,107,107,0.1);
            border: 1px solid rgba(255,107,107,0.2);
            border-radius: 8px;
            padding: 10px 14px;
            margin-top: 14px;
            display: block;
            text-align: center;
            animation: errorShake 0.4s ease;
        }

        @keyframes errorShake {
            0%,100% { transform: translateX(0); }
            20%      { transform: translateX(-6px); }
            40%      { transform: translateX(6px); }
            60%      { transform: translateX(-4px); }
            80%      { transform: translateX(4px); }
        }

        /* ── FOOTER BADGE ── */
        .login-footer {
            padding: 18px 44px 28px;
            text-align: center;
        }

        .secure-badge {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            font-size: 0.7rem;
            color: var(--text-muted);
            letter-spacing: 0.06em;
            text-transform: uppercase;
        }

        .secure-badge .dot {
            width: 6px; height: 6px;
            border-radius: 50%;
            background: #2ecc71;
            box-shadow: 0 0 6px #2ecc71;
            animation: blink 2s ease-in-out infinite;
        }

        @keyframes blink {
            0%,100% { opacity: 1; }
            50%      { opacity: 0.3; }
        }

        /* ── RESPONSIVE ── */
        @media (max-width: 576px) {
            .login-wrapper__top,
            .login-wrapper__body,
            .header-divider { 
                padding-left: 28px;
                padding-right: 28px;

            }
            .header-divider { 
                margin-left: 28px;
                margin-right: 28px; 

            }
            .login-footer { 
                padding-left: 28px;
                padding-right: 28px; 

            }
            .title { 
                font-size: 1.35rem;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- Google Translate Element (Hidden) -->
        <div id="google_translate_element" style="display:none;"></div>
        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en',
                    includedLanguages: 'fr',
                    layout: google.translate.TranslateElement.InlineLayout.SIMPLE
                }, 'google_translate_element');
            }
            function autoTranslateToFrench() {
                const interval = setInterval(() => {
                    const select = document.querySelector('.goog-te-combo');
                    if (select) {
                        select.value = 'fr';
                        select.dispatchEvent(new Event('change'));
                        clearInterval(interval);
                    }
                }, 500);
            }
            window.addEventListener('load', () => { autoTranslateToFrench(); });
        </script>
        <script src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

        <div class="login-main">
            <!-- Background decorations -->
            <div class="grid-overlay"></div>
            <span class="orb orb-1"></span>
            <span class="orb orb-2"></span>
            <span class="orb orb-3"></span>
            <span class="orb orb-4"></span>
            <span class="orb orb-5"></span>

            <div class="container custom-container">
                <div class="row justify-content-center">
                    <div class="col-xxl-5 col-xl-5 col-lg-6 col-md-8 col-sm-11">
                        <div class="login-area">
                            <div class="login-wrapper">

                                <!-- Header -->
                                <div class="login-wrapper__top">
                                    <div class="brand-row">
                                        <div class="brand-icon">
                                            <!-- Bus icon -->
                                            <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <rect x="2" y="5" width="20" height="12" rx="2" fill="currentColor" opacity="0.9"/>
                                                <rect x="4" y="7" width="4" height="4" rx="1" fill="#C9A84C" opacity="0.5"/>
                                                <rect x="10" y="7" width="4" height="4" rx="1" fill="#C9A84C" opacity="0.5"/>
                                                <rect x="16" y="7" width="4" height="4" rx="1" fill="#C9A84C" opacity="0.5"/>
                                                <circle cx="7" cy="19" r="2" fill="currentColor"/>
                                                <circle cx="17" cy="19" r="2" fill="currentColor"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <h3 class="title">Welcome to <strong>ExcelBus</strong></h3>
                                            <p class="subtitle">Admin Control Panel</p>
                                        </div>
                                    </div>
                                </div>

                                <div class="header-divider"></div>

                                <!-- Body -->
                                <div class="login-wrapper__body">

                                    <div class="form-group">
                                        <label>&#9679;&nbsp; Role</label>
                                        <div class="input-icon-wrap">
                                            <i class="fas fa-user-shield icon"></i>
                                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control" style="padding-left:46px;">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label>&#9679;&nbsp; Username</label>
                                        <div class="input-icon-wrap">
                                            <i class="fas fa-user icon"></i>
                                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username" required="required"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label>&#9679;&nbsp; Password</label>
                                        <div class="input-icon-wrap">
                                            <i class="fas fa-lock icon"></i>
                                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" required="required"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:Button ID="btnLogin" runat="server" Text="LOGIN →" CssClass="btn cmn-btn w-100" OnClick="btnLogin_Click" />

                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-2 d-block" Visible="false"></asp:Label>

                                </div><!-- /body -->

                                <!-- Footer -->
                                <div class="login-footer">
                                    <span class="secure-badge">
                                        <span class="dot"></span>
                                        Secured &amp; Encrypted Connection
                                    </span>
                                </div>

                            </div><!-- /login-wrapper -->
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script src="https://excelbus.mavenxone.com/assets/global/js/jquery-3.7.1.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/bootstrap.bundle.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/admin/js/vendor/bootstrap-toggle.min.js"></script>
    <link href="https://excelbus.mavenxone.com/assets/global/css/iziToast.min.css" rel="stylesheet">
    <link href="https://excelbus.mavenxone.com/assets/global/css/iziToast_custom.css" rel="stylesheet">
    <script src="https://excelbus.mavenxone.com/assets/global/js/iziToast.min.js"></script>

    <script>
        "use strict";
        const colors = { success: '#28c76f', error: '#eb2222', warning: '#ff9f43', info: '#1e9ff2' };
        const icons = { success: 'fas fa-check-circle', error: 'fas fa-times-circle', warning: 'fas fa-exclamation-triangle', info: 'fas fa-exclamation-circle' };

        const triggerToaster = (status, message) => {
            iziToast[status]({
                title: status.charAt(0).toUpperCase() + status.slice(1),
                message,
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
        };

        function notify(status, message) {
            if (typeof message == 'string') triggerToaster(status, message);
            else $.each(message, (i, val) => triggerToaster(status, val));
        }
    </script>

    <script src="js/nicEdit.js"></script>
    <script src="js/select2.min.js"></script>
    <script src="js/app.js"></script>
</body>
</html>
