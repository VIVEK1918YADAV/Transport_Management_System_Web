<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainAdminLogin.aspx.cs" Inherits="ExcelBus.TrainAdminLogin" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>ExcelTrain - Admin Login</title>

    <link rel="shortcut icon" type="image/png" href="~/assets/images/logo_icon/favicon.png">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="~/assets/global/css/all.min.css">
    <link rel="stylesheet" href="~/assets/global/css/bootstrap.min.css">
    <link rel="stylesheet" href="~/assets/admin/css/app.css">

    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Poppins', sans-serif;
            background: #47a17b;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
           
        }

        body > form {
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
           /* padding: 20px;*/
           background-color: #c1e9d6;

        }

        /* ── Card ── */
        .login-box {
            background: #fff;
            width: 100%;
            max-width: 520px;
            border-radius: 12px;
            box-shadow: 0 4px 24px rgba(0,0,0,0.10);
            overflow: hidden;
        }

        /* ── Header ── */
        .login-box__header {
            background: #47a17b;
            padding: 28px 32px 24px;
            text-align: center;
        }

        .login-box__header .logo-wrap {
            width: 68px;
            height: 68px;
            background: #fff;
            border-radius: 50%;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 12px;
            overflow: hidden;
        }

        .login-box__header .logo-wrap img {
            width: 56px;
            height: 56px;
            object-fit: contain;
        }

        .login-box__header h4 {
            color: #fff;
            font-size: 17px;
            font-weight: 700;
            margin: 0 0 4px;
        }

        .login-box__header p {
            color: rgba(255,255,255,0.68);
            font-size: 12px;
            margin: 0;
        }

        /* ── Body ── */
        .login-box__body {
            padding: 28px 32px 32px;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-group label {
            display: block;
            font-size: 12.5px;
            font-weight: 600;
            color: #374151;
            margin-bottom: 6px;
        }

        .form-control {
            width: 100%;
            height: 44px;
            padding: 0 14px;
            border: 1.5px solid #d1d5db;
            border-radius: 8px;
            font-family: 'Poppins', sans-serif;
            font-size: 13.5px;
            color: #111827;
            background: #47a17b;
            outline: none;
            transition: border-color 0.15s, box-shadow 0.15s;
            appearance: none;
            -webkit-appearance: none;
        }

        .form-control:focus {
            border-color: #055224;
            background: #fff;
            box-shadow: 0 0 0 3px rgba(5,82,36,0.10);
        }

        select.form-control {
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%236b7280' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 14px center;
            background-color: #2d9b8b8a;
            padding-right: 36px;
        }

        /* ── Button ── */
        .btn-login {
            width: 100%;
            height: 44px;
            background: #47a17b;
            color: #fff;
            border: none;
            border-radius: 8px;
            font-family: 'Poppins', sans-serif;
            font-size: 14px;
            font-weight: 600;
            letter-spacing: 0.5px;
            cursor: pointer;
            margin-top: 4px;
            transition: background 0.15s;
        }

        .btn-login:hover {
            background: #47a17b;
        }

        /* ── Error ── */
        .error-label {
            display: block;
            background: #47a17b;
            border: 1px solid #fecaca;
            border-radius: 7px;
            padding: 9px 13px;
            margin-top: 12px;
            font-size: 12.5px;
            color: #dc2626;
            font-weight: 500;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- Google Translate (hidden) -->
        <div id="google_translate_element" style="display:none;"></div>
        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en', includedLanguages: 'fr',
                    layout: google.translate.TranslateElement.InlineLayout.SIMPLE
                }, 'google_translate_element');
            }
            function autoTranslateToFrench() {
                const interval = setInterval(() => {
                    const select = document.querySelector('.goog-te-combo');
                    if (select) { select.value = 'fr'; select.dispatchEvent(new Event('change')); clearInterval(interval); }
                }, 500);
            }
            window.addEventListener('load', () => { autoTranslateToFrench(); });
        </script>
        <script src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>

        <div class="login-box">

            <!-- Header -->
            <div class="login-box__header">
                <div class="logo-wrap">
                    <img src="~/assets/images/logo_icon/logo.png" alt="ExcelTrain Logo" onerror="this.style.display='none';this.parentElement.innerHTML='<i class=\'fas fa-train\' style=\'font-size:28px;color:#055224\'></i>'">
                </div>
                <h4>Excel Train Admin</h4>
                <p>Sign in to ExcelTransport Dashboard</p>
            </div>

            <!-- Body -->
            <div class="login-box__body">

                <div class="form-group">
                    <label>Role</label>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username" required="required"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter password" required="required"></asp:TextBox>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="LOGIN" CssClass="btn-login" OnClick="btnLogin_Click" />

                <asp:Label ID="lblMessage" runat="server" CssClass="error-label" Visible="false"></asp:Label>

            </div>
        </div>

    </form>

    <script src="~/assets/global/js/jquery-3.7.1.min.js"></script>
    <script src="~/assets/global/js/bootstrap.bundle.min.js"></script>
    <link href="~/assets/global/css/iziToast.min.css" rel="stylesheet">
    <link href="~/assets/global/css/iziToast_custom.css" rel="stylesheet">
    <script src="~/assets/global/js/iziToast.min.js"></script>

    <script>
        "use strict";
        const colors = { success: '#28c76f', error: '#eb2222', warning: '#ff9f43', info: '#1e9ff2' };
        const icons = { success: 'fas fa-check-circle', error: 'fas fa-times-circle', warning: 'fas fa-exclamation-triangle', info: 'fas fa-exclamation-circle' };

        const triggerToaster = (status, message) => {
            iziToast[status]({
                title: status.charAt(0).toUpperCase() + status.slice(1),
                message: message,
                position: "topRight",
                backgroundColor: '#fff',
                icon: icons[status],
                iconColor: colors[status],
                progressBarColor: colors[status],
                titleSize: '1rem', messageSize: '1rem',
                titleColor: '#474747', messageColor: '#a2a2a2',
                transitionIn: 'bounceInLeft'
            });
        };

        function notify(status, message) {
            if (typeof message === 'string') triggerToaster(status, message);
            else $.each(message, (i, val) => triggerToaster(status, val));
        }
    </script>

</body>
</html>
