<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainUserDashboard.aspx.cs" Inherits="ExcelBus.TrainUserDashboard" Async="true" AsyncTimeout="90" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelTrain - Dashboard</title>
    <meta name="title" content="ExcelTrain - Dashboard">
    <link rel="shortcut icon" href="img/excel_bus_logo.png" type="image/x-icon">

    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/global-line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/line-awesome/1.3.0/line-awesome/css/line-awesome.min.css">
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />
    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Syne:wght@400;600;700;800&family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,400;0,9..40,500;1,9..40,300&display=swap" rel="stylesheet">

    <style>
        :root {
            --gold: #629361;
            --gold-dk: #054e06;
            --gold-lt: #629b426b;
            --gold-xlt: #d0dfce91;
            --ink: #296e33;
            --ink-soft: #165004;
            --ink-muted: #308118;
            --surface: #ecf3e3;
            --white: #c7ebb354;
            --border: #b8e9bb;
            --red: #5c9147;
            --red-lt: #c3e9abf2;
            --slate: #185412;
            --slate-lt: #e1efd4;
            --blue: #064207;
            --blue-lt: #cd9c68;
            --radius-sm: 8px;
            --radius-md: 14px;
            --radius-lg: 22px;
            --shadow-sm: -3 2px 8px rgb(170 199 143 / 95%);
            --shadow-md: 0 8px 28px rgb(70 52 22 / 12%);
            --shadow-lg: 0 20px 50px rgba(44, 31, 10, .16);
            --font-head: 'Syne', sans-serif;
            --font-body: 'DM Sans', sans-serif;
        }

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: var(--font-body);
            background: #cff1e3;
            color: #1d8d6a;
            font-size: 15px;
            line-height: 1.6;
            -webkit-font-smoothing: antialiased;
        }

        .preloader {
            position: fixed; inset: 0;
            background: var(--white);
            z-index: 9999;
            display: flex; align-items: center; justify-content: center;
            transition: opacity .4s;
        }
        .preloader-ring {
            width: 48px; height: 48px;
            border: 3px solid var(--border);
            border-top-color: var(--gold);
            border-radius: 50%;
            animation: spin .7s linear infinite;
        }
        @keyframes spin { to { transform: rotate(360deg); } }

        .eb-header-top { background: #47a17b; padding: 2px 0; }
        .eb-header-top .inner {
            max-width: 1280px; margin: 0 auto; padding: 9px 24px;
            display: flex; align-items: center; gap: 294px; flex-wrap: wrap;
        }
        .eb-header-top a {
            color: rgba(255,255,255);
            font-size: 13px; text-decoration: none;
            display: flex; align-items: center; gap: 7px;
            transition: color .2s;
        }
        .eb-header-top a:hover { color: var(--gold); }
        .eb-header-top i { color: #efeeea; }

        .eb-navbar {
            background: #cff1e3;
            border-bottom: 1px solid #59938d;
            position: sticky; top: 0; z-index: 900;
            box-shadow: var(--shadow-sm);
        }
        .eb-navbar .inner {
            max-width: 1280px; margin: 0 auto; padding: 0 24px;
            display: flex; align-items: center; justify-content: space-between;
            height: 66px;
        }
        .eb-logo img { height: 44px; }
        .eb-nav-menu { list-style: none; display: flex; align-items: center; gap: 4px; }
        .eb-nav-menu > li { position: relative; }
        .eb-nav-menu > li > a {
            display: flex; align-items: center; gap: 6px;
            padding: 9px 14px;
            border-radius: var(--radius-sm);
            font-family: var(--font-head);
            font-size: 14px; font-weight: 600;
            color: var(--ink); text-decoration: none;
            transition: background .18s, color .18s;
        }
        .eb-nav-menu > li > a .chev { font-size: 10px; opacity: .45; transition: transform .22s, opacity .22s; }
        .eb-nav-menu > li:hover > a { background: #ffff; color: #47a17b; }
        .eb-nav-menu > li:hover > a .chev { transform: rotate(180deg); opacity: 1; }

        .eb-sub {
            display: none;
            position: absolute; top: calc(100% + 6px); left: 0;
            min-width: 210px;
            background: var(--white);
            border: 1px solid var(--border);
            border-radius: var(--radius-md);
            box-shadow: var(--shadow-md);
            list-style: none; overflow: hidden;
            z-index: 999;
        }
        .eb-nav-menu li:hover .eb-sub,
        .eb-nav-menu li:focus-within .eb-sub { display: block; }
        .eb-sub li a,
        .eb-sub li .asp-btn {
            display: block; padding: 11px 18px;
            font-size: 14px; color: #158355;
            text-decoration: none;
            transition: background .15s, color .15s, padding-left .15s;
            border-bottom: 1px solid var(--border);
            background: #cff1e3; border-left: none; border-right: none; border-top: none;
            width: 100%; text-align: left; cursor: pointer;
            font-family: var(--font-body);
        }
        .eb-sub li:last-child a,
        .eb-sub li:last-child .asp-btn { border-bottom: none; }
        .eb-sub li a:hover,
        .eb-sub li .asp-btn:hover { background: #47a17b; color: #183a1d; padding-left: 24px; }
        .eb-sub li .logout-btn { color: var(--red); }
        .eb-sub li .logout-btn:hover { background: #47a17b; color: var(--red); }

        .eb-cta {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 22px;
            background: #47a17b; color: #ffff;
            font-family: var(--font-head); font-size: 14px; font-weight: 700;
            border-radius: 50px; text-decoration: none;
            transition: background .2s, transform .15s, box-shadow .2s;
            box-shadow: -4px 5px 16px rgb(73 116 65 / 35%);
            white-space: nowrap;
        }
        .eb-cta:hover {
            background: #054e41; color: #2d8358;
            transform: translateY(-1px);
            box-shadow: 0 8px 24px rgb(39 98 71 / 45%);
        }

        #google_translate_element {
            bottom: 448px; right: 20px; z-index: 9999;
            background: var(--white); border-radius: var(--radius-sm);
            box-shadow: var(--shadow-md);
        }

        .eb-hero {
            background: #47a17b;
            background-image: url('img/bg_train_img.webp');
            background-size: cover; background-position: center;
            background-blend-mode: overlay;
            padding: 60px 0 52px;
            position: relative; overflow: hidden;
        }
        .eb-hero::before { content: ''; position: absolute; inset: 0; background: #47a17b; }
        .eb-hero .wrap { position: relative; max-width: 1280px; margin: 0 auto; padding: 0 24px; }
        .eb-hero-inner {
            display: flex; align-items: center; justify-content: space-between;
            flex-wrap: wrap; gap: 20px;
        }
        .eb-hero-title {
            font-family: var(--font-head);
            font-size: clamp(26px, 4vw, 42px); font-weight: 800;
            color: #ffff; letter-spacing: -.5px; line-height: 1.1;
        }
        .eb-hero-title em { font-style: normal; color: #11664c; }
        .eb-hero-sub { color: rgba(255,255,255); font-size: 15px; margin-top: 6px; }

        .eb-stats { padding: 36px 0 0; }
        .eb-stats .wrap { max-width: 1280px; margin: 0 auto; padding: 0 24px; }
        .eb-stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; }

        .eb-card {
            background: var(--white);
            border-radius: var(--radius-lg);
            padding: 26px 24px 22px;
            border: 1px solid var(--border);
            box-shadow: var(--shadow-sm);
            display: flex; align-items: flex-start; gap: 18px;
            position: relative; overflow: hidden;
            transition: transform .22s, box-shadow .22s;
            animation: popIn .4s ease both;
        }
        .eb-card:hover { transform: translateY(-3px); box-shadow: var(--shadow-md); }
        .eb-card::before {
            content: ''; position: absolute;
            top: 0; left: 0; right: 0; height: 3px;
            border-radius: var(--radius-lg) var(--radius-lg) 0 0;
        }
        .eb-card.c-green::before, .eb-card.c-blue::before, .eb-card.c-gold::before { background: #47a17b; }
        .eb-card:nth-child(1) { animation-delay: .05s; }
        .eb-card:nth-child(2) { animation-delay: .12s; }
        .eb-card:nth-child(3) { animation-delay: .19s; }

        .eb-card-ico {
            width: 50px; height: 50px; border-radius: var(--radius-md);
            display: flex; align-items: center; justify-content: center;
            font-size: 22px; flex-shrink: 0;
        }
        .c-green .eb-card-ico, .c-blue .eb-card-ico, .c-gold .eb-card-ico { background: #caddbd; color: #083a2b; }

        .eb-card-body { flex: 1; }
        .eb-card-lbl { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .09em; color: #074230; margin-bottom: 6px; }
        .eb-card-val { font-family: var(--font-head); font-size: 38px; font-weight: 800; line-height: 1; color: #0b462d; }

        .eb-tbl-section { padding: 32px 0 60px; }
        .eb-tbl-section .wrap { max-width: 1631px; margin: 0 auto; padding: 0 24px; }
        .eb-sec-head {
            display: flex; align-items: center; justify-content: space-between;
            flex-wrap: wrap; gap: 12px; margin-bottom: 18px;
        }
        .eb-sec-title { font-family: var(--font-head); font-size: 20px; font-weight: 700; color: #0b462d; }
        .eb-sec-title em { font-style: normal; color: var(--gold); }
        .eb-count-pill { font-size: 12px; font-weight: 700; padding: 5px 14px; background: var(--gold-lt); color: #0b462d; border-radius: 50px; }

        .eb-tbl-wrap {
            background: #24854754;
            border-radius: var(--radius-lg);
            border: 1px solid var(--border);
            box-shadow: var(--shadow-sm);
            overflow: hidden;
        }
        .eb-tbl { width: 100%; border-collapse: collapse; }
        .eb-tbl thead tr { background: #47a17b; }
        .eb-tbl thead th {
            font-family: var(--font-head); font-size: 11px; font-weight: 700;
            text-transform: uppercase; letter-spacing: .08em;
            color: white; padding: 16px 18px; text-align: left; white-space: nowrap;
        }
        .eb-tbl tbody tr { border-bottom: 1px solid var(--border); transition: background .15s; animation: fadeUp .35s ease both; }
        .eb-tbl tbody tr:last-child { border-bottom: none; }
        .eb-tbl tbody tr:hover { background: var(--surface); }
        .eb-tbl tbody td { padding: 15px 18px; font-size: 14px; color: var(--ink-soft); vertical-align: middle; }

        .eb-tbl tbody tr:nth-child(1)  { animation-delay: .04s; }
        .eb-tbl tbody tr:nth-child(2)  { animation-delay: .08s; }
        .eb-tbl tbody tr:nth-child(3)  { animation-delay: .12s; }
        .eb-tbl tbody tr:nth-child(4)  { animation-delay: .16s; }
        .eb-tbl tbody tr:nth-child(5)  { animation-delay: .20s; }
        .eb-tbl tbody tr:nth-child(6)  { animation-delay: .24s; }
        .eb-tbl tbody tr:nth-child(7)  { animation-delay: .28s; }
        .eb-tbl tbody tr:nth-child(8)  { animation-delay: .32s; }
        .eb-tbl tbody tr:nth-child(9)  { animation-delay: .36s; }
        .eb-tbl tbody tr:nth-child(10) { animation-delay: .40s; }

        .eb-pnr {
            font-family: var(--font-head); font-size: 13px; font-weight: 700;
            color: #0b462d; background: var(--gold-lt);
            padding: 4px 10px; border-radius: 6px;
            letter-spacing: .04em; white-space: nowrap;
            border: 1px solid rgba(211,168,68,.3);
        }
        .eb-route { display: flex; align-items: center; gap: 7px; white-space: nowrap; }
        .eb-route .from, .eb-route .to { font-weight: 600; color: var(--ink); font-size: 13px; }
        .eb-route .arr { color: var(--gold); font-size: 12px; }

        .eb-date-cell { display: flex; align-items: center; gap: 6px; color: var(--ink-soft); font-size: 13px; }
        .eb-date-cell i { color: var(--gold); }

        .eb-seats { display: flex; flex-wrap: wrap; gap: 4px; }
        .eb-seat-chip {
            background: var(--gold-lt); color: var(--gold-dk);
            font-size: 11px; font-weight: 700;
            padding: 2px 8px; border-radius: 4px;
            border: 1px solid rgba(211,168,68,.25);
        }

        .eb-lugg { display: flex; align-items: center; gap: 5px; font-size: 13px; color: var(--ink-soft); }
        .eb-lugg i { color: var(--gold); }

        .eb-fare { font-family: var(--font-head); font-weight: 700; font-size: 14px; color: var(--ink); white-space: nowrap; }
        .eb-fare .cur { font-size: 11px; font-weight: 600; color: var(--ink-muted); margin-left: 3px; }

        .badge {
            display: inline-flex; align-items: center; gap: 5px;
            font-family: var(--font-head); font-size: 11px; font-weight: 700;
            letter-spacing: .06em; text-transform: uppercase;
            padding: 4px 12px; border-radius: 50px;
        }
        .badge::before { content: ''; width: 6px; height: 6px; border-radius: 50%; flex-shrink: 0; }
        .badge--success { background: var(--gold-lt); color: var(--gold-dk); }
        .badge--success::before { background: var(--gold); }
        .badge--warning { background: #47a17b; color: #0f3608; border: 1px solid #0b440d; }
        .badge--warning::before { background: #47a17b; }
        .badge--primary { background: #47a17b; color: #f3f5f1; }
        .badge--primary::before { background: #ffff; }
        .badge--dark { background: var(--slate-lt); color: var(--slate); }
        .badge--dark::before { background: var(--slate); }
        .badge--danger { background: #47a17b; color: var(--red); }
        .badge--danger::before { background: #47a17b; }
        .badge--secondary { background: #47a17b; color: var(--ink-muted); }
        .badge--secondary::before { background: #47a17b; }

        /* Empty state */
        .eb-empty {
            display: none;
            flex-direction: column; align-items: center; justify-content: center;
            padding: 72px 24px; text-align: center;
        }
        .eb-empty.visible { display: flex; }
        .eb-empty-ico {
            width: 80px; height: 80px; background: var(--gold-lt); border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            font-size: 36px; color: var(--gold); margin-bottom: 20px;
        }
        .eb-empty h4 { font-family: var(--font-head); font-size: 18px; font-weight: 700; color: var(--ink); margin-bottom: 8px; }
        .eb-empty p  { color: var(--ink-muted); font-size: 14px; max-width: 300px; }

        /* Mobile cards */
        @media (max-width: 992px) {
            .eb-tbl { display: none; }
            .eb-mobile-list { display: block !important; }
        }
        .eb-mobile-list { display: none; }
        .eb-mob-card {
            background: var(--white); border: 1px solid var(--border);
            border-radius: var(--radius-md); padding: 20px; margin-bottom: 12px;
            box-shadow: var(--shadow-sm); animation: fadeUp .3s ease both;
        }
        .eb-mob-head {
            display: flex; align-items: center; justify-content: space-between;
            flex-wrap: wrap; gap: 8px; margin-bottom: 14px;
            padding-bottom: 12px; border-bottom: 1px solid var(--border);
        }
        .eb-mob-row {
            display: flex; justify-content: space-between; align-items: center;
            padding: 8px 0; border-bottom: 1px solid var(--border);
            font-size: 13px; gap: 8px;
        }
        .eb-mob-row:last-child { border-bottom: none; }
        .eb-mob-lbl { color: var(--ink-muted); font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .07em; flex-shrink: 0; }
        .eb-mob-val { color: var(--ink); font-weight: 500; text-align: right; }

        /* Modal */
        .modal-content { border: none; border-radius: var(--radius-lg); box-shadow: var(--shadow-lg); overflow: hidden; }
        .modal-header { background: #47a17b; padding: 20px 24px; border: none; display: flex; align-items: center; justify-content: space-between; }
        .modal-title { font-family: var(--font-head); font-size: 17px; font-weight: 700; color: var(--white); }
        .modal-close-btn {
            background: #47a17b; border: none; color: var(--white);
            width: 32px; height: 32px; border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            cursor: pointer; transition: background .15s; font-size: 16px;
        }
        .modal-close-btn:hover { background: #47a17b; }
        .modal-body { padding: 24px; }
        .modal-footer { border-top: 1px solid var(--border); padding: 16px 24px; }
        .info-row { display: flex; justify-content: space-between; align-items: center; padding: 12px 0; border-bottom: 1px solid var(--border); gap: 12px; }
        .info-row:last-child { border-bottom: none; }
        .info-lbl { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .07em; color: var(--ink-muted); }
        .info-val { font-size: 14px; font-weight: 500; color: var(--ink); text-align: right; }

        /* Footer */
        .eb-footer { background: #47a17b; padding: 36px 0; }
        .eb-footer .wrap { max-width: 1280px; margin: 0 auto; padding: 0 24px; display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 16px; }
        .eb-footer img { height: 40px; opacity: .8; }
        .eb-footer p { color: rgba(255,255,255); font-size: 13px; margin-top: 6px; }
        .eb-footer-copy { color: rgba(255,255,255); font-size: 12px; }

        .eb-scroll-top {
            position: fixed; bottom: 28px; right: 28px;
            width: 44px; height: 44px; background: var(--gold); color: var(--ink);
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            font-size: 18px; text-decoration: none;
            box-shadow: 0 4px 16px rgba(211,168,68,.4);
            transition: background .2s, transform .2s, opacity .3s;
            z-index: 500; opacity: 0; pointer-events: none;
        }
        .eb-scroll-top.show { opacity: 1; pointer-events: all; }
        .eb-scroll-top:hover { background: var(--gold-dk); color: var(--ink); transform: translateY(-3px); }

        @keyframes popIn  { from { opacity:0; transform: scale(.94) translateY(10px); } to { opacity:1; transform: scale(1) translateY(0); } }
        @keyframes fadeUp { from { opacity:0; transform: translateY(14px); }           to { opacity:1; transform: translateY(0); } }

        /* Pagination */
        .eb-pagination { display: flex; align-items: center; justify-content: center; gap: 8px; margin-top: 24px; flex-wrap: wrap; }
        .pg-btn {
            display: inline-flex; align-items: center; gap: 4px;
            padding: 6px 14px; border: 1px solid #d0d0d0;
            border-radius: 6px; background: #fff; color: #333;
            font-size: 13px; cursor: pointer; text-decoration: none;
        }
        .pg-btn:disabled, .pg-btn[disabled] { opacity: 0.4; cursor: default; }
        .pg-btn:not([disabled]):hover { background: #f5f5f5; }
        .pg-numbers { display: flex; align-items: center; gap: 4px; }
        .pg-num {
            min-width: 34px; height: 34px;
            display: inline-flex; align-items: center; justify-content: center;
            border: 1px solid #d0d0d0; border-radius: 6px;
            background: #fff; font-size: 13px; color: #333;
            cursor: pointer; text-decoration: none;
        }
        .pg-num--active { background: #1a73e8; border-color: #1a73e8; color: #fff; font-weight: 500; cursor: default; }
        .pg-num:not(.pg-num--active):hover { background: #f5f5f5; }
        .pg-ellipsis { font-size: 13px; color: #888; padding: 0 4px; }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <!-- Preloader -->
        <div class="preloader" id="preloader">
            <div class="preloader-ring"></div>
        </div>

        <!-- HEADER TOP -->
        <div class="eb-header-top">
            <div class="inner">
                <a href="tel:+919871771849"><i class="las la-phone"></i> +91 98717 71849</a>
                <a href="mailto:rajesh.paul@excelgeomatics.com"><i class="las la-envelope-open"></i> rajesh.paul@excelgeomatics.com</a>
                <a><div id="google_translate_element"></div></a>
            </div>
        </div>

        <!-- NAVBAR -->
        <nav class="eb-navbar">
            <div class="inner">
                <div class="eb-logo">
                    <a href="~/Default.aspx" runat="server"><img src="img/train_icon.png" alt="ExcelBus"></a>
                </div>

                <ul class="eb-nav-menu">
                    <li>
                        <a href="~/TrainUserDashboard.aspx" runat="server">
                            <i class="las la-tachometer-alt"></i> Dashboard
                        </a>
                    </li>
                    <li>
                        <a href="javascript:void(0)">
                            <i class="las la-ticket-alt"></i> Booking <span class="chev">&#9660;</span>
                        </a>
                        <ul class="eb-sub">
                            <li><a href="TrainTicket.aspx"><i class="las la-shopping-cart"></i>&nbsp; Buy Ticket</a></li>
                            <li><a href="Train_BookedTicket_History.aspx"><i class="las la-history"></i>&nbsp; Booking History</a></li>
                        </ul>
                    </li>
                    <li>
                        <a href="javascript:void(0)">
                            <i class="las la-user-circle"></i> Profile <span class="chev">&#9660;</span>
                        </a>
                        <ul class="eb-sub">
                            <li><a href="~/Train_MyBookings.aspx" runat="server"><i class="las la-clipboard-list"></i>&nbsp; My Bookings</a></li>
                            <li><a href="Train_UserProfile.aspx"><i class="las la-id-card"></i>&nbsp; Profile</a></li>
                            <li>
                                <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click" CssClass="asp-btn logout-btn">
                                    <i class="las la-sign-out-alt"></i>&nbsp; Logout
                                </asp:LinkButton>
                            </li>
                        </ul>
                    </li>
                </ul>

                <a href="TrainTicket.aspx" class="eb-cta">
                    <i class="las la-ticket-alt"></i> Buy Tickets
                </a>
            </div>
        </nav>

        <!-- HERO -->
        <section class="eb-hero">
            <div class="wrap">
                <div class="eb-hero-inner">
                    <div>
                        <div class="eb-hero-title">My <em>Dashboard</em></div>
                        <div class="eb-hero-sub">Track your journeys, bookings &amp; travel history</div>
                    </div>
                </div>
            </div>
        </section>

        <!-- STATS -->
        <section class="eb-stats">
            <div class="wrap">
                <div class="eb-stats-grid">
                    <div class="eb-card c-green">
                        <div class="eb-card-ico"><i class="las la-ticket-alt"></i></div>
                        <div class="eb-card-body">
                            <div class="eb-card-lbl">Total Booked</div>
                            <div class="eb-card-val"><asp:Literal ID="litTotalBooked" runat="server">0</asp:Literal></div>
                        </div>
                    </div>
                    <div class="eb-card c-blue">
                        <div class="eb-card-ico"><i class="las la-times-circle"></i></div>
                        <div class="eb-card-body">
                            <div class="eb-card-lbl">Cancelled</div>
                            <div class="eb-card-val"><asp:Literal ID="litTotalCancelled" runat="server">0</asp:Literal></div>
                        </div>
                    </div>
                    <div class="eb-card c-gold">
                        <div class="eb-card-ico"><i class="las la-calendar-plus"></i></div>
                        <div class="eb-card-body">
                            <div class="eb-card-lbl">Postponed</div>
                            <div class="eb-card-val"><asp:Literal ID="litTotalPostponed" runat="server">0</asp:Literal></div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- BOOKINGS TABLE -->
        <section class="eb-tbl-section">
            <div class="wrap">
                <div class="eb-sec-head">
                    <div class="eb-sec-title">Booking <em>History</em></div>
                    <span class="eb-count-pill">
                        <asp:Label ID="lblRowCount" runat="server" Text="0 records"></asp:Label>
                    </span>
                </div>

                <div class="eb-tbl-wrap">
                    <%-- ── REPEATER ── --%>
                    <asp:Repeater ID="rptBookings" runat="server">
                        <HeaderTemplate>
                            <table class="eb-tbl">
                                <thead>
                                    <tr>
                                        <th>PNR Number</th>
                                        <th>Type</th>
                                        <th>Route</th>
                                        <th>Journey Date</th>
                                        <th>Seats</th>
                                        <th>Luggage</th>
                                        <th>Luggage Charge</th>
                                        <th>Status</th>
                                        <th>Postponed</th>
                                        <th>Fare</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><span class="eb-pnr"><%# Eval("PnrNumber") %></span></td>
                                <td><span class="badge badge--success">AC</span></td>
                                <td>
                                    <div class="eb-route">
                                        <span class="from"><%# Eval("PickupName") %></span>
                                        <span class="arr"><i class="las la-long-arrow-alt-right"></i></span>
                                        <span class="to"><%# Eval("DropName") %></span>
                                    </div>
                                </td>
                                <td>
                                    <div class="eb-date-cell">
                                        <i class="las la-calendar-alt"></i>
                                        <%# Eval("DateOfJourneyFormatted") %>
                                    </div>
                                </td>
                                <td>
                                    <div class="eb-seats">
                                        <%# FormatSeatsHtml(Eval("SeatsDisplay")?.ToString()) %>
                                    </div>
                                </td>
                                <td>
                                    <div class="eb-lugg">
                                        <i class="las la-suitcase"></i>
                                        <%# Eval("LuggagesCount") %> bag(s)
                                    </div>
                                </td>
                                <td>
                                    <div class="eb-lugg">
                                        <i class="las la-tag"></i>
                                        <%# Convert.ToDecimal(Eval("LuggagesCharge") ?? 0m).ToString("N2") %> CDF
                                    </div>
                                </td>
                                <td><%# GetStatusBadge(Eval("BookingStatus")) %></td>
                                <td>
                                    <%# GetPostponedBadge(
                                        int.TryParse(Eval("PostponeCount")?.ToString(), out int pc) ? pc : 0
                                    ) %>
                                </td>
                                <td>
                                    <div class="eb-fare">
                                        <%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %>
                                        <span class="cur">CDF</span>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>

                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <%-- ── EMPTY STATE (server-side visibility controlled in code-behind) ── --%>
                    <asp:Panel ID="noDataMessage" runat="server" CssClass="eb-empty" Visible="false">
                        <div class="eb-empty-ico"><i class="las la-train"></i></div>
                        <h4>No Bookings Yet</h4>
                        <p>You haven't made any train bookings. Start your journey today!</p>
                        <a href="TrainTicket.aspx" class="eb-cta" style="margin-top:20px;">
                            <i class="las la-ticket-alt"></i> Book a Ticket
                        </a>
                    </asp:Panel>
                </div>

                <%-- ── PAGINATION BAR ── --%>
                <asp:Panel ID="pnlPagination" runat="server" CssClass="eb-pagination" Visible="false">
                    <asp:LinkButton ID="btnPrev" runat="server" CssClass="pg-btn" OnClick="btnPrev_Click">
                        <i class="las la-angle-left"></i> Prev
                    </asp:LinkButton>

                    <div class="pg-numbers">
                        <asp:PlaceHolder ID="phPageNumbers" runat="server" />
                    </div>

                    <asp:LinkButton ID="btnNext" runat="server" CssClass="pg-btn" OnClick="btnNext_Click">
                        Next <i class="las la-angle-right"></i>
                    </asp:LinkButton>
                </asp:Panel>

                <!-- Mobile Cards (populated by JS) -->
                <div class="eb-mobile-list" id="mobileCards"></div>
            </div>
        </section>

        <!-- INFO MODAL -->
        <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Ticket Details</h5>
                        <button type="button" class="modal-close-btn" data-bs-dismiss="modal">
                            <i class="las la-times"></i>
                        </button>
                    </div>
                    <div class="modal-body"></div>
                    <div class="modal-footer">
                        <button type="button" class="eb-cta" data-bs-dismiss="modal"
                            style="background:#47a17b; box-shadow:none; padding:8px 20px;">
                            <i class="las la-times"></i> Close
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- FOOTER -->
        <footer class="eb-footer">
            <div class="wrap">
                <div>
                    <img src="img/train_icon.png" alt="ExcelBus">
                    <p>Your trusted train booking partner</p>
                </div>
                <div class="eb-footer-copy">&copy; <%=DateTime.Now.Year %> ExcelTrain. All rights reserved.</div>
            </div>
        </footer>

        <a href="javascript:void(0)" class="eb-scroll-top" id="scrollTop">
            <i class="las la-chevron-up"></i>
        </a>

    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>
    <script src="js/iziToast.min.js"></script>

    <script>
        "use strict";

        const COLORS = { success: '#28c76f', error: '#eb2222', warning: '#ff9f43', info: '#1e9ff2' };
        const ICONS  = { success: 'fas fa-check-circle', error: 'fas fa-times-circle', warning: 'fas fa-exclamation-triangle', info: 'fas fa-exclamation-circle' };

        function triggerToaster(status, message) {
            iziToast[status]({
                title: status.charAt(0).toUpperCase() + status.slice(1),
                message,
                position: 'topRight',
                backgroundColor: '#fff',
                icon: ICONS[status], iconColor: COLORS[status], progressBarColor: COLORS[status],
                titleSize: '1rem', messageSize: '1rem',
                titleColor: '#474747', messageColor: '#a2a2a2',
                transitionIn: 'bounceInLeft'
            });
        }
        function notify(status, message) {
            if (typeof message === 'string') triggerToaster(status, message);
            else $.each(message, (i, v) => triggerToaster(status, v));
        }

        /* Preloader */
        window.addEventListener('load', () => {
            const pre = document.getElementById('preloader');
            if (pre) { pre.style.opacity = '0'; setTimeout(() => pre.remove(), 400); }
        });

        /* Scroll-to-top */
        const scrollBtn = document.getElementById('scrollTop');
        window.addEventListener('scroll', () => {
            scrollBtn.classList.toggle('show', window.scrollY > 300);
        });
        scrollBtn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));

        /* Mobile cards */
        $(document).ready(function () {
            const rows = $('.eb-tbl tbody tr');
            if (rows.length > 0) buildMobileCards(rows);
        });

        function buildMobileCards(rows) {
            const container = document.getElementById('mobileCards');
            if (!container) return;
            rows.each(function (i) {
                const cells = $(this).find('td');
                const card = `
                <div class="eb-mob-card" style="animation-delay:${i * .06}s">
                    <div class="eb-mob-head">
                        ${cells.eq(0).html()}
                        ${cells.eq(7).html()}
                    </div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Route</span><span class="eb-mob-val">${cells.eq(2).html()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Date</span><span class="eb-mob-val">${cells.eq(3).text().trim()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Seats</span><span class="eb-mob-val">${cells.eq(4).html()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Luggage</span><span class="eb-mob-val">${cells.eq(5).html()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Luggage Charge</span><span class="eb-mob-val">${cells.eq(6).text().trim()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Postponed</span><span class="eb-mob-val">${cells.eq(8).html()}</span></div>
                    <div class="eb-mob-row"><span class="eb-mob-lbl">Fare</span><span class="eb-mob-val">${cells.eq(9).html()}</span></div>
                </div>`;
                container.insertAdjacentHTML('beforeend', card);
            });
        }

        function getStatusBadge(text) {
            const map = { booked: 'badge--success', pending: 'badge--warning', cancelled: 'badge--primary', postponed: 'badge--dark', rejected: 'badge--danger' };
            const cls = map[(text || '').toLowerCase()] || 'badge--secondary';
            return `<span class="badge ${cls}">${text}</span>`;
        }

        /* Modal */
        $(document).on('click', '.checkinfo', function (e) {
            e.preventDefault();
            const encoded = $(this).attr('data-info');
            if (!encoded) return;
            try {
                const ta = document.createElement('textarea');
                ta.innerHTML = encoded;
                const info = JSON.parse(ta.value);
                const html = `
                    <div class="info-row"><span class="info-lbl">PNR</span><span class="info-val">${info.pnrNumber}</span></div>
                    <div class="info-row"><span class="info-lbl">Route</span><span class="info-val">${info.pickup} &#8594; ${info.drop}</span></div>
                    <div class="info-row"><span class="info-lbl">Journey Date</span><span class="info-val">${info.dateOfJourney}</span></div>
                    <div class="info-row"><span class="info-lbl">Fare</span><span class="info-val">${parseFloat(info.subTotal).toFixed(2)} CDF</span></div>
                    <div class="info-row"><span class="info-lbl">Status</span><span class="info-val">${getStatusBadge(info.statusText)}</span></div>`;
                $('#infoModal .modal-body').html(html);
            } catch (err) {
                notify('error', 'Error loading ticket information');
            }
        });
    </script>

    <script type="text/javascript">
        function googleTranslateElementInit() {
            new google.translate.TranslateElement({
                pageLanguage: 'en',
                layout: google.translate.TranslateElement.InlineLayout.HORIZONTAL,
                includedLanguages: 'en,fr,hi,de,es'
            }, 'google_translate_element');
        }
    </script>
    <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
</body>
</html>
