<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_UserProfile.aspx.cs" Inherits="Excel_Bus.Train_UserProfile" Async="true" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@500;600;700&family=DM+Sans:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <style>
        *, *::before, *::after {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        :root {
            --ink: #1a1a2e;
            --ink-mid: #2d2d4e;
            --ink-soft: #5a5a7a;
            --ink-muted: #9090b0;
            --surface: #f7f7fb;
            --card: #ffffff;
            --border: #e8e8f2;
            --accent: #5c6bc0;
            --accent-2: #7e57c2;
            --accent-glow: rgba(92,107,192,0.18);
            --gold: #c9a84c;
            --danger: #e05c7a;
            --success: #43c59e;
            --brand: #47a17b;
            --brand-dark: #2e7d5a;
            --brand-light: #e6f7f1;
            --radius: 16px;
            --shadow-sm: 0 2px 8px rgba(26,26,46,0.06);
            --shadow-md: 0 8px 32px rgba(26,26,46,0.10);
            --shadow-lg: 0 20px 60px rgba(26,26,46,0.14);
            --transition: 0.25s cubic-bezier(.4,0,.2,1);
        }

        body {
            font-family: 'DM Sans', sans-serif;
            /* background: var(--brand);*/
            color: var(--ink);
        }

            /* ── BACKGROUND DECORATION ── */
            body::before {
                content: '';
                position: fixed;
                top: -200px;
                right: -200px;
                width: 600px;
                height: 600px;
                background: rgba(255,255,255,0.06);
                border-radius: 50%;
                pointer-events: none;
                z-index: 0;
            }

            body::after {
                content: '';
                position: fixed;
                bottom: -150px;
                left: -150px;
                width: 500px;
                height: 500px;
                background: rgba(255,255,255,0.04);
                border-radius: 50%;
                pointer-events: none;
                z-index: 0;
            }

        /* ── BANNER ── */
        .hero-banner {
            position: relative;
            background: var(--brand);
            padding: 64px 0 80px;
            overflow: hidden;
            z-index: 1;
        }

        .hero-orb {
            position: absolute;
            border-radius: 50%;
            filter: blur(80px);
            pointer-events: none;
        }

        .hero-orb-1 {
            width: 400px;
            height: 400px;
            background: rgba(255,255,255,0.10);
            top: -100px;
            right: -100px;
        }

        .hero-orb-2 {
            width: 300px;
            height: 300px;
            background: rgba(255,255,255,0.07);
            bottom: -80px;
            left: 5%;
        }

        .hero-content {
            position: relative;
            z-index: 2;
            text-align: center;
        }

        .hero-eyebrow {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: .75rem;
            font-weight: 600;
            letter-spacing: .12em;
            text-transform: uppercase;
            color: var(--gold);
            margin-bottom: 14px;
        }

            .hero-eyebrow span {
                width: 28px;
                height: 1px;
                background: rgba(201,168,76,0.5);
                display: inline-block;
            }

        .hero-title {
            font-family: 'Playfair Display', serif;
            font-size: clamp(2rem, 5vw, 3.2rem);
            font-weight: 700;
            color: #fff;
            letter-spacing: -.01em;
            line-height: 1.1;
        }

        .hero-subtitle {
            margin-top: 10px;
            font-size: .95rem;
            color: rgba(255,255,255,.5);
            font-weight: 300;
            letter-spacing: .01em;
        }

        /* ── LAYOUT ── */
        .profile-wrapper {
            position: relative;
            z-index: 1;
            max-width: 860px;
            margin: -32px auto 60px;
            padding: 0 24px;
            display: flex;
            flex-direction: column;
            gap: 24px;
        }

        /* ── CARD ── */
        .glass-card {
            background: var(--card);
            border-radius: var(--radius);
            border: 1px solid var(--border);
            box-shadow: var(--shadow-md);
            overflow: hidden;
            animation: slideUp .5s cubic-bezier(.4,0,.2,1) both;
        }

        @keyframes slideUp {
            from {
                opacity: 0;
                transform: translateY(24px);
            }

            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .glass-card.delay-1 {
            animation-delay: .12s;
        }

        .card-header-strip {
            height: 4px;
            background: linear-gradient(90deg, var(--brand), var(--accent));
        }

            .card-header-strip.gold-strip {
                background: linear-gradient(90deg, var(--gold), #e8c56e);
            }

        .card-inner {
            padding: 36px 40px;
            background-color: rgb(114 251 184 / 0.13);
        }

        .section-title {
            font-family: 'Playfair Display', serif;
            font-size: 17px;
            font-weight: 600;
            color: var(--ink);
            margin-bottom: 28px;
            display: flex;
            align-items: center;
            gap: 12px;
        }

            .section-title .icon-pill {
                width: 36px;
                height: 36px;
                background: var(--brand-light);
                border-radius: 10px;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: .95rem;
                color: var(--brand-dark);
                flex-shrink: 0;
            }

                .section-title .icon-pill.gold {
                    background: #fdf6e3;
                    color: var(--gold);
                }

        /* ── AVATAR ── */
        .avatar-section {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 14px;
            margin-bottom: 36px;
            padding-bottom: 36px;
            border-bottom: 1px solid var(--border);
        }

        .avatar-ring {
            position: relative;
            width: 120px;
            height: 120px;
            cursor: pointer;
        }

            .avatar-ring::before {
                content: '';
                position: absolute;
                inset: -4px;
                border-radius: 50%;
                background: linear-gradient(135deg, var(--brand), var(--accent));
                z-index: 0;
                transition: opacity var(--transition);
                opacity: .6;
            }

            .avatar-ring:hover::before {
                opacity: 1;
            }

        .avatar-inner {
            position: relative;
            z-index: 1;
            width: 100%;
            height: 100%;
            border-radius: 50%;
            overflow: hidden;
            background: var(--brand-light);
            border: 3px solid var(--card);
        }

            .avatar-inner img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }

        .avatar-overlay {
            position: absolute;
            inset: 0;
            z-index: 2;
            border-radius: 50%;
            background: rgba(0,0,0,0.35);
            display: flex;
            align-items: center;
            justify-content: center;
            opacity: 0;
            transition: opacity var(--transition);
        }

        .avatar-ring:hover .avatar-overlay {
            opacity: 1;
        }

        .avatar-overlay svg {
            color: #fff;
            width: 22px;
            height: 22px;
        }

        .avatar-name {
            font-family: 'Playfair Display', serif;
            font-size: 1.3rem;
            font-weight: 600;
            color: var(--ink);
        }

        .avatar-role {
            font-size: 11px;
            color: var(--ink-soft);
            font-weight: 500;
            letter-spacing: .06em;
            text-transform: uppercase;
        }

        /* ── FORM GRID ── */
        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 22px 28px;
        }

        .col-full {
            grid-column: 1 / -1;
        }

        /* ── FIELD ── */
        .field-group {
            display: flex;
            flex-direction: column;
            gap: 7px;
        }

        .field-label {
            font-size: 11px;
            font-weight: 600;
            color: var(--ink-soft);
            letter-spacing: .07em;
            text-transform: uppercase;
        }

        .req {
            color: var(--danger);
        }

        .field-wrap {
            position: relative;
            display: flex;
            align-items: center;
        }

        .field-icon {
            position: absolute;
            left: 14px;
            color: var(--ink-muted);
            font-size: .95rem;
            pointer-events: none;
            display: flex;
            align-items: center;
            transition: color var(--transition);
        }

        /* ── FORM INPUTS ── */
        .form-grid .form-control,
        .form-grid textarea.form-control {
            width: 100% !important;
            padding: 11px 14px 0px 40px !important;
            font-family: 'DM Sans', sans-serif !important;
            font-size: 14px !important;
            font-weight: 400 !important;
            color: var(--ink) !important;
            background: #ffffff !important;
            border: 1.5px solid var(--border) !important;
            border-radius: 10px !important;
            outline: none !important;
            box-shadow: none !important;
            transition: border-color var(--transition), box-shadow var(--transition) !important;
            resize: none;
        }

            .form-grid .form-control:focus,
            .form-grid textarea.form-control:focus {
                border-color: var(--accent) !important;
                box-shadow: 0 0 0 4px var(--accent-glow) !important;
            }

            .form-grid .form-control[readonly],
            .form-grid textarea.form-control[readonly] {
                color: var(--ink-muted) !important;
                cursor: not-allowed !important;
                background: var(--surface) !important;
            }

        textarea.form-control {
            padding-top: 13px !important;
            padding-bottom: 13px !important;
        }

        /* ── WALLET ── */
        .wallet-badge {
            display: flex;
            align-items: center;
            gap: 10px;
            background: var(--brand-light);
            border: 1.5px solid rgba(71,161,123,0.3);
            border-radius: 10px;
            padding: 11px 16px;
            width: 100%;
        }

            .wallet-badge svg {
                color: var(--gold);
                flex-shrink: 0;
            }

            .wallet-badge .wallet-label {
                font-size: 13px;
                color: var(--brand-dark);
                font-weight: 500;
            }

        .wallet-amount {
            margin-left: auto;
            font-size: 15px;
            font-weight: 700;
            color: var(--brand-dark);
        }

        /* ── PASSWORD TOGGLE ── */
        .pw-toggle {
            position: absolute;
            right: 13px;
            cursor: pointer;
            color: var(--ink-muted);
            transition: color var(--transition);
            background: none;
            border: none;
            padding: 0;
            display: flex;
            align-items: center;
        }

            .pw-toggle:hover {
                color: var(--accent);
            }

        /* ── STRENGTH BAR ── */
        .strength-bar {
            height: 5px;
            background: var(--surface);
            border-radius: 99px;
            overflow: hidden;
            margin-top: 8px;
        }

        .strength-progress {
            height: 100%;
            width: 0%;
            border-radius: 99px;
            transition: width .4s ease, background .4s ease;
        }

        .strength-label {
            font-size: .75rem;
            font-weight: 600;
            margin-top: 5px;
            letter-spacing: .04em;
        }

        /* ── BUTTONS ── */
        .btn-save {
            width: 100% !important;
            padding: 14px !important;
            background: var(--brand) !important;
            color: #fff !important;
            border: none !important;
            border-radius: 12px !important;
            font-family: 'DM Sans', sans-serif !important;
            font-size: .95rem !important;
            font-weight: 600 !important;
            letter-spacing: .03em !important;
            cursor: pointer !important;
            box-shadow: 0 6px 20px rgba(71,161,123,.35) !important;
            transition: transform var(--transition), box-shadow var(--transition), filter var(--transition) !important;
        }

            .btn-save:hover {
                transform: translateY(-2px) !important;
                box-shadow: 0 10px 28px rgba(71,161,123,.45) !important;
                filter: brightness(1.07) !important;
            }

            .btn-save:active {
                transform: translateY(0) !important;
            }

        .btn-change-pw {
            width: 100% !important;
            padding: 14px !important;
            background: transparent !important;
            color: var(--accent) !important;
            border: 1.5px solid var(--accent) !important;
            border-radius: 12px !important;
            font-family: 'DM Sans', sans-serif !important;
            font-size: .95rem !important;
            font-weight: 600 !important;
            cursor: pointer !important;
            transition: background var(--transition) !important;
        }

            .btn-change-pw:hover {
                background: rgba(92,107,192,0.08) !important;
            }

        /* ── TOAST ── */
        .toast-container {
            position: fixed;
            top: 24px;
            right: 24px;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .toast-item {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            background: var(--card);
            border: 1px solid var(--border);
            border-left: 4px solid;
            border-radius: 12px;
            padding: 14px 16px;
            box-shadow: var(--shadow-lg);
            min-width: 280px;
            max-width: 360px;
            animation: toastIn .35s cubic-bezier(.4,0,.2,1);
        }

        @keyframes toastIn {
            from {
                opacity: 0;
                transform: translateX(30px);
            }

            to {
                opacity: 1;
                transform: translateX(0);
            }
        }

        .toast-item.success {
            border-left-color: var(--success);
        }

        .toast-item.error {
            border-left-color: var(--danger);
        }

        .toast-icon {
            font-size: 1.1rem;
            margin-top: 1px;
        }

        .toast-item.success .toast-icon {
            color: var(--success);
        }

        .toast-item.error .toast-icon {
            color: var(--danger);
        }

        .toast-text strong {
            font-size: .88rem;
            font-weight: 600;
            color: var(--ink);
            display: block;
        }

        .toast-text p {
            font-size: .82rem;
            color: var(--ink-muted);
            margin-top: 2px;
        }

        /* ── RESPONSIVE ── */
        @media (max-width: 640px) {
            .card-inner {
                padding: 24px 20px;
            }

            .form-grid {
                grid-template-columns: 1fr;
            }

            .col-full {
                grid-column: 1;
            }
        }
    </style>

    <!-- Toast Container -->
    <div class="toast-container" id="toastContainer"></div>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hdnUserId" runat="server" />
    <asp:HiddenField ID="hdnCountryCode" runat="server" />
    <asp:HiddenField ID="hdnShowMessage" runat="server" Value="false" />
    <asp:HiddenField ID="hdnMessageType" runat="server" />
    <asp:HiddenField ID="hdnMessageText" runat="server" />

    <!-- ── HERO BANNER ── -->
    <div class="hero-banner">
        <div class="hero-orb hero-orb-1"></div>
        <div class="hero-orb hero-orb-2"></div>
        <div class="container hero-content">
            <div class="hero-eyebrow"><span></span>Account<span></span></div>
            <h1 class="hero-title">Profile Settings</h1>
            <p class="hero-subtitle">Manage your personal information and security</p>
        </div>
    </div>

    <!-- ── PROFILE WRAPPER ── -->
    <div class="profile-wrapper">

        <%-- ── PERSONAL INFORMATION CARD ── --%>
        <div class="glass-card">
            <div class="card-header-strip"></div>
            <div class="card-inner">

                <%-- Avatar Section --%>
                <div class="avatar-section">
                    <div class="avatar-ring" onclick="document.getElementById('avatarFileInput').click()">
                        <div class="avatar-inner">
                            <asp:Image ID="imgProfilePicture" runat="server"
                                ImageUrl="https://ui-avatars.com/api/?name=User&background=47a17b&color=fff&size=200"
                                AlternateText="Profile Picture"
                                CssClass="profile-img" />
                        </div>
                        <div class="avatar-overlay">
                            <svg fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z" />
                                <circle cx="12" cy="13" r="3" />
                            </svg>
                        </div>
                    </div>
                    <input type="file" id="avatarFileInput" accept="image/*" style="display: none"
                        onchange="previewAvatar(this)" />
                    <div class="avatar-name" id="displayName">User</div>
                    <div class="avatar-role">Passenger Account</div>
                </div>

                <div class="section-title">
                    <span class="icon-pill">
                        <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                    </span>
                    Personal Information
               
                </div>

                <div class="form-grid">

                    <%-- First Name --%>
                    <div class="field-group">
                        <label class="field-label">First Name <span class="req">*</span></label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"
                                placeholder="First name" required="required"
                                onkeyup="updateDisplayName()" />
                        </div>
                    </div>

                    <%-- Last Name --%>
                    <div class="field-group">
                        <label class="field-label">Last Name <span class="req">*</span></label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"
                                placeholder="Last name" required="required"
                                onkeyup="updateDisplayName()" />
                        </div>
                    </div>

                    <%-- Username --%>
                    <div class="field-group">
                        <label class="field-label">Username</label>
                        <div class="field-wrap">
                            <span class="field-icon" style="font-weight: 600; font-size: .9rem">@</span>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
                                placeholder="username" />
                        </div>
                    </div>

                    <%-- Email --%>
                    <div class="field-group">
                        <label class="field-label">Email Address <span class="req">*</span></label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
                                TextMode="Email" placeholder="you@example.com" ReadOnly="true" />
                        </div>
                    </div>

                    <%-- Dial Code --%>
                    <%--<div class="field-group">
                        <label class="field-label">Dial Code</label>
                        <div class="field-wrap">
                            <span class="field-icon">🌐</span>
                            <asp:TextBox ID="txtDialCode" runat="server" CssClass="form-control"
                                placeholder="+91" />
                        </div>
                    </div>--%>
                    <div class="field-group">
                        <label class="field-label">Dial Code</label>
                        <div class="field-wrap">
                            <span class="field-icon">🌐</span>
                            <asp:DropDownList ID="ddlDialCode" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <%-- Mobile --%>
                    <div class="field-group">
                        <label class="field-label">Mobile Number</label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"
                                placeholder="Enter mobile number" />
                        </div>
                    </div>

                    <%-- Address --%>
                    <div class="field-group col-full">
                        <label class="field-label">Address</label>
                        <div class="field-wrap" style="align-items: flex-start">
                            <span class="field-icon" style="position: absolute; top: 13px; left: 14px">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control"
                                TextMode="MultiLine" Rows="3"
                                placeholder="Street address, building, floor…" />
                        </div>
                    </div>

                    <%-- City --%>
                    <div class="field-group">
                        <label class="field-label">City</label>
                        <div class="field-wrap">
                            <span class="field-icon">🏙</span>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control"
                                placeholder="City" />
                        </div>
                    </div>

                    <%-- State --%>
                    <div class="field-group">
                        <label class="field-label">State</label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6H11l-1-1H5a2 2 0 00-2 2zm9-13.5V9" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtState" runat="server" CssClass="form-control"
                                placeholder="State / Province" />
                        </div>
                    </div>

                    <%-- Zip Code --%>
                    <div class="field-group">
                        <label class="field-label">Zip Code</label>
                        <div class="field-wrap">
                            <span class="field-icon" style="font-weight: 600; font-size: .9rem">#</span>
                            <asp:TextBox ID="txtZip" runat="server" CssClass="form-control"
                                placeholder="Zip / Postal code" />
                        </div>
                    </div>

                    <%-- Country --%>
                    <div class="field-group">
                        <label class="field-label">Country</label>
                        <div class="field-wrap">
                            <span class="field-icon">🌍</span>
                            <asp:TextBox ID="txtCountryName" runat="server" CssClass="form-control"
                                placeholder="Country" ReadOnly="true" />
                        </div>
                    </div>

                    <%-- Wallet Balance --%>
                    <div class="field-group col-full">
                        <label class="field-label">Wallet Balance</label>
                        <div class="wallet-badge">
                            <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                            </svg>
                            <span class="wallet-label">Available Balance</span>
                            <span class="wallet-amount">
                                <asp:Literal ID="litBalance" runat="server" />
                            </span>
                        </div>
                    </div>

                    <%-- Update Button --%>
                    <div class="col-full" style="margin-top: 8px">
                        <asp:Button ID="btnUpdateProfile" runat="server"
                            Text="Update Profile"
                            CssClass="btn-save"
                            OnClick="btnUpdateProfile_Click" />
                    </div>

                </div>
                <%-- End form-grid --%>
            </div>
        </div>
        <%-- End Personal Info Card --%>

        <%-- ── CHANGE PASSWORD CARD ── --%>
        <%-- FIX: moved INSIDE .profile-wrapper (was outside before) --%>
        <div class="glass-card delay-1">
            <div class="card-header-strip gold-strip"></div>
            <div class="card-inner">

                <div class="section-title">
                    <span class="icon-pill gold">
                        <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                        </svg>
                    </span>
                    Change Password
               
                </div>

                <div class="form-grid">

                    <%-- Current Password --%>
                    <div class="field-group col-full">
                        <label class="field-label">Current Password</label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control"
                                TextMode="Password" placeholder="Enter current password" />
                            <button type="button" class="pw-toggle"
                                onclick="togglePw('<%= txtCurrentPassword.ClientID %>', this)">
                                <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                </svg>
                            </button>
                        </div>
                    </div>

                    <%-- New Password --%>
                    <div class="field-group">
                        <label class="field-label">New Password</label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control"
                                TextMode="Password" placeholder="New password"
                                onkeyup="checkStrength(this.value)" />
                            <button type="button" class="pw-toggle"
                                onclick="togglePw('<%= txtNewPassword.ClientID %>', this)">
                                <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                </svg>
                            </button>
                        </div>
                        <div class="strength-bar">
                            <div class="strength-progress" id="strengthBar"></div>
                        </div>
                        <div class="strength-label" id="strengthText" style="color: var(--ink-muted)"></div>
                    </div>

                    <%-- Confirm Password --%>
                    <div class="field-group">
                        <label class="field-label">Confirm Password</label>
                        <div class="field-wrap">
                            <span class="field-icon">
                                <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                                </svg>
                            </span>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control"
                                TextMode="Password" placeholder="Confirm new password" />
                            <button type="button" class="pw-toggle"
                                onclick="togglePw('<%= txtConfirmPassword.ClientID %>', this)">
                                <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                </svg>
                            </button>
                        </div>
                    </div>

                    <%-- Change Password Button --%>
                    <div class="col-full" style="margin-top: 8px">
                        <asp:Button ID="btnChangePassword" runat="server"
                            Text="Change Password"
                            CssClass="btn-change-pw"
                            OnClick="btnChangePassword_Click" />
                    </div>

                </div>
                <%-- End form-grid --%>
            </div>
        </div>
        <%-- End Change Password Card --%>
    </div>
    <%-- End profile-wrapper --%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">

    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>

    <script>
        "use strict";

        /* ── Avatar Preview ── */
        function previewAvatar(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.querySelector('.profile-img').src = e.target.result;
                };
                reader.readAsDataURL(input.files[0]);
            }
        }

        /* ── Live Display Name ── */
        function updateDisplayName() {
            var first = document.getElementById('<%= txtFirstName.ClientID %>').value.trim();
            var last = document.getElementById('<%= txtLastName.ClientID %>').value.trim();
            var name = [first, last].filter(Boolean).join(' ') || 'User';
            document.getElementById('displayName').textContent = name;
            document.querySelector('.profile-img').src =
                'https://ui-avatars.com/api/?name=' + encodeURIComponent(name) +
                '&background=47a17b&color=fff&size=200';
        }

        /* ── Password Toggle ── */
        function togglePw(fieldId, btn) {
            var field = document.getElementById(fieldId);
            var isPassword = field.type === 'password';
            field.type = isPassword ? 'text' : 'password';
            btn.style.color = isPassword ? 'var(--accent)' : '';
        }

        /* ── Password Strength ── */
        function checkStrength(pw) {
            var bar = document.getElementById('strengthBar');
            var text = document.getElementById('strengthText');
            if (!pw) { bar.style.width = '0'; text.textContent = ''; return; }
            var s = 0;
            if (pw.length >= 8) s += 25;
            if (/[a-z]/.test(pw)) s += 25;
            if (/[A-Z]/.test(pw)) s += 25;
            if (/[0-9]/.test(pw) && /[^A-Za-z0-9]/.test(pw)) s += 25;
            bar.style.width = s + '%';
            var levels = {
                25: ['#e05c7a', 'Weak'],
                50: ['#ffc107', 'Fair'],
                75: ['#17a2b8', 'Good'],
                100: ['#43c59e', 'Strong']
            };
            var level = levels[s] || levels[25];
            bar.style.background = level[0];
            text.textContent = level[1];
            text.style.color = level[0];
        }

        /* ── Toast Notification ── */
        function showToast(type, title, message) {
            var container = document.getElementById('toastContainer');
            var icons = {
                success: '<svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>',
                error: '<svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>',
                warning: '<svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>',
                info: '<svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>'
            };
            var toast = document.createElement('div');
            toast.className = 'toast-item ' + type;
            toast.innerHTML =
                '<span class="toast-icon">' + (icons[type] || '') + '</span>' +
                '<div class="toast-text"><strong>' + title + '</strong><p>' + message + '</p></div>';
            container.appendChild(toast);
            setTimeout(function () {
                toast.style.opacity = '0';
                toast.style.transform = 'translateX(20px)';
                toast.style.transition = 'all .3s ease';
                setTimeout(function () { toast.remove(); }, 300);
            }, 3500);
        }

        /* ── Read server-side messages via HiddenFields ── */
        $(document).ready(function () {
            updateDisplayName();

            var showMsg = $('#<%= hdnShowMessage.ClientID %>').val();
            var msgType = $('#<%= hdnMessageType.ClientID %>').val();
            var msgText = $('#<%= hdnMessageText.ClientID %>').val();

            if (showMsg === 'true' && msgText) {
                var title = msgType.charAt(0).toUpperCase() + msgType.slice(1);
                showToast(msgType, title, msgText);
                $('#<%= hdnShowMessage.ClientID %>').val('false');
            }
        });
    </script>

</asp:Content>
