<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_MyBookings.aspx.cs" Inherits="Excel_Bus.Train_MyBookings" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
      My Bookings
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        *, *::before, *::after { box-sizing: border-box; position: relative; margin: 0; padding: 0; }

        :root {
            --cream: #FAF8F4;
            --warm-white: #FFFFFF;
            --sand: #F0EBE1;
            --terracotta: #C8603A;
            --terracotta-light: #FCEEE9;
            --terracotta-dark: #A84E2C;
            --ink: #1A1612;
            --graphite: #3D3530;
            --muted: #8C7E75;
            --border: #E8E0D6;
            --green: #3D8B5C;
            --green-light: #EAF5EE;
            --blue: #2D6FA3;
            --blue-light: #EAF2FA;
            --red: #C0392B;
            --red-light: #FCEAE9;
            --amber: #C47F17;
            --amber-light: #FDF3E0;
            --shadow-sm: 0 1px 4px rgba(26,22,18,0.06);
            --shadow-md: 0 4px 20px rgba(26,22,18,0.08);
            --radius: 16px;
        }

        body {
            font-family: 'DM Sans', sans-serif; 
            background: #d6f3e7;
            color: var(--ink);
            min-height: 100vh;

        }

        /* ── PAGE HEADER (SLIM) ── */
        .page-header {
            background: #47a17b;
            position: relative;
            overflow: hidden;
            padding: 0;
        }

        .page-header::before {
            content: '';
            position: absolute;
            top: 0; right: 0;
            width: 200px; height: 100%;
            background: #47a17b;
            pointer-events: none;
            opacity: 0.4;
        }

        .header-inner {
            max-width: 900px;
            margin: 0 auto;
            padding: 12px 32px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
        }

        .header-title-group { 
            display: flex; 
            flex-direction: column;
            gap: 4px;
            margin-left: -145px;

        }

        .header-eyebrow {
            font-size: 10px;
    font-weight: 600;
    letter-spacing: 1px;
    text-transform: uppercase;
    color: #042c21;
        }

        .header-title {
            font-family: 'Playfair Display', serif;
            font-size: clamp(20px, 3vw, 26px);
            color: #f1efed;
            line-height: 1.1;
        }

        .header-subtitle {
            font-size: 13px;
            color:#f7f1ee;
            font-weight: 400;
            margin-top: 1px;
        }

        .header-stats {
            display: flex;
            gap: 20px;
            align-items: center;
            flex-shrink: 0;
        }

        .stat-item { text-align: center; }

        .stat-number {
            font-family: 'Playfair Display', serif;
            font-size: 20px;
            color: #ffffff;
            line-height: 1;
        }

        .stat-label {
            font-size: 10px;
            color: #ffffff;
            letter-spacing: 0.5px;
            margin-top: 2px;
        }

        .stat-divider {
            width: 1px; 
            height: 24px; 
            background: #47a17b; 

        }

        /* ── MAIN CONTENT ── */
        .container { 
            max-width: 1320px;
            margin: 0 auto; 
            /*padding: 32px 32px 80px;
*/
        }

        /* ── BOOKING CARD ── */
        .booking-card {
            background: #47a17b;
            border-radius: var(--radius);
            border: 1px solid var(--border); 
            margin-bottom: 20px; 
            box-shadow: var(--shadow-sm);
            transition: box-shadow .25s, transform .25s;
            overflow: hidden;
            animation: slideUp .4s ease both;

        }
        @keyframes slideUp { 
            from { 
                opacity:0;
                transform:translateY(16px);

            } to {
                  opacity:1;
                  transform:translateY(0); 

              } }
        .booking-card:hover { 
            box-shadow: var(--shadow-md);
            transform: translateY(-2px);
        }
        .booking-card::before { 
            content:'';
            display:block;
            height:3px; 
            background:#47a17b;

        }
        .booking-card.status-cancelled::before {
            background:#47a17b;

        }
        .booking-card.status-completed::before {
            background:#47a17b;

        }
        .booking-card.status-pending::before   { 
            background:#47a17b;

        }

        /* ── CARD HEADER ── */
        .booking-header { 
            display:flex;
            justify-content:space-between;
            align-items:center; 
            padding:20px 24px 16px; 
            border-bottom:1px solid #0f6c2c;

        }
        .pnr-label { 
            font-size:10px;
            font-weight:600; 
            letter-spacing:2px; 
            text-transform:uppercase; 
            color:#02421c;
            margin-bottom:6px;

        }
        .pnr-number { 
            font-family:'Playfair Display',serif;
            font-size:20px; 
            color:var(--terracotta);
            letter-spacing:1px; 

        }

        /* ── STATUS BADGE ── */
        .status-badge { 
            display:inline-flex; 
            align-items:center; 
            gap:6px; 
            padding:6px 14px; 
            border-radius:100px;
            font-size:12px; 
            font-weight:600; 
            letter-spacing:.3px;

        }
        .status-badge::before {
            content:'';
            width:6px;
            height:6px;
            border-radius:50%;
            background:currentColor;

        }
        .status-booked    {
          background: #a4ddc5;
    color: #043819;

        }
        .status-cancelled { 
            background:#47a17b; 
            color:var(--red);  

        }
        .status-completed {
            background:#47a17b; 
            color:var(--blue); 

        }
        .status-pending   {
            background:#47a17b;
            color:var(--amber); 

        }
        .status-default   {
            background:#a4ddc5;
            color:#dd672c;

        }

        /* ── BOOKING DETAILS ── */
        .booking-details { 
            display:grid; 
            grid-template-columns:repeat(3,1fr);
            padding:20px 24px;
            gap:0;

        }
        .detail-item { 
            padding:0 20px 0 0;
            border-right:1px solid var(--sand);
            margin-right:20px;

        }
        .detail-item:last-child, .detail-item:nth-child(3) {
            border-right:none;
            margin-right:0;

        }
        .detail-item:nth-child(n+4) { 
            padding-top:16px; 
            border-top:1px solid var(--sand);
            margin-top:16px; 

        }
        .detail-label { 
            font-size: 13px;
            font-weight: 700;
            letter-spacing: 1.5px;
            text-transform: uppercase;
            color: #010600;
            margin-bottom: 5px;

        }
        .detail-value {
            font-size:14px; 
            font-weight:500; 
            color:var(--graphite);
            display:flex;
            align-items:center;
            gap:7px;

        }
        .detail-value i { 
            color:var(--terracotta);
            font-size:13px; 

        }
        .route-value {
            font-family:'Playfair Display',serif;
            font-size:15px;
            color:var(--ink);

        }
        .amount-value { 
            color:#033c1a;
            font-weight:600;

        }

        /* ── ACTIONS ── */
        .booking-actions { 
            padding:12px 24px 20px; 
            display:flex; 
            align-items:center; 
            gap:12px;

        }
        .btn-view-details { 
            display:inline-flex;
            align-items:center;
            gap:8px; background:#47a17b;
            color:var(--cream);
            border:none; 
            padding:10px 22px; 
            border-radius:8px;
            font-family:'DM Sans',sans-serif;
            font-size:13px; 
            font-weight:600; 
            cursor:pointer; 
            letter-spacing:.2px;
            transition:background .2s,transform .2s;

        }
        .btn-view-details:hover {
            background:#47a17b;
            transform:translateY(-1px);

        }
        .btn-secondary { 
            display:inline-flex;
            align-items:center; 
            gap:8px;
            background:transparent; 
            color: #02220f;
            border: 1px solid #113003;
            padding:9px 18px;
            border-radius:8px;
            font-family:'DM Sans',sans-serif; 
            font-size:13px; 
            font-weight:500; 
            cursor:pointer;
            transition:all .2s;

        }
        .btn-secondary:hover {
            border-color: #47a17b;
            color:#194e06;

        }

        /* ── NO BOOKINGS ── */
        .no-bookings { 
            background:var(--warm-white);
            border-radius:var(--radius); 
            border:1px solid var(--border); 
            padding:80px 40px; 
            text-align:center; 

        }
        .no-bookings-icon { 
            width:90px; 
            height:90px;
            background:#cdf7ec; 
            border-radius:50%; 
            display:flex;
            align-items:center;
            justify-content:center;
            margin:0 auto 24px; 

        }
        .no-bookings-icon i {
            font-size:38px;
            color: #38a171;

        }
        .no-bookings h3 { 
            font-family:'Playfair Display',serif;
            font-size:24px; 
            color:var(--ink); 
            margin-bottom:10px;

        }
        .no-bookings p  { 
            color:var(--muted);
            font-size:14px;
            margin-bottom:28px;
            max-width:360px;
            margin-left:auto; 
            margin-right:auto;

        }
        .btn-book-now { 
            display:inline-flex;
            align-items:center;
            gap:10px; 
            background:#0e8b52;
            color:white;
            border:none; 
            padding:12px 32px;
            border-radius:8px;
            font-family:'DM Sans',sans-serif; 
            font-size:14px;
            font-weight:600; 
            cursor:pointer; 
            transition:background .2s,transform .2s;

        }
        .btn-book-now:hover {
            background:#47a17b;
            transform:translateY(-2px);

        }

        /* ── FILTER PILLS ── */
        .filter-bar { 
            display:flex;
            gap:8px;
            margin-bottom:24px;
            flex-wrap:wrap; 

        }
        .filter-pill {
            padding:6px 14px;
            border-radius:100px;
            font-size:12px;
            font-weight:500; 
            border:1px solid var(--border); 
            background:var(--warm-white); 
            color:var(--muted);
            cursor:pointer;
            transition:all .2s;

        }
        .filter-pill.active, .filter-pill:hover {
            background: #47a17b;
            color:var(--cream); 
            border-color:#ffffff;

        }

        /* ── RESPONSIVE ── */
        @media (max-width: 600px) {
            .header-inner { flex-direction: column; align-items: flex-start; padding: 10px 16px; gap: 10px; }
            .header-stats { gap: 14px; }
            .booking-details { grid-template-columns: 1fr 1fr; }
            .detail-item:nth-child(2n) { border-right:none; margin-right:0; }
            .detail-item:nth-child(n+3) { padding-top:16px; border-top:1px solid var(--sand); margin-top:16px; }
            .booking-header { flex-direction:column; align-items:flex-start; gap:12px; }
            .container { padding: 20px 16px 60px; }
            .header-title { font-size: 20px; }
        }
        .header-top {
    padding-top: 41px;
    background: #47a17b;
}
         div#google_translate_element {
          position: absolute;
            top: 0;
            right: 0;
        }
    </style>


    <!-- PAGE HEADER -->
    <div class="page-header">
        <div class="header-inner">
            <div class="header-title-group">
                <div class="header-eyebrow">Passenger Portal</div>
                <h1 class="header-title">My Bookings</h1>
                <p class="header-subtitle">View and manage your train ticket reservations</p>
            </div>
            <div class="header-stats">
                <div class="stat-item">
                    <div class="stat-number"><%: StatTotal %></div>
                    <div class="stat-label">Total Trips</div>
                </div>
                <div class="stat-divider"></div>
                <div class="stat-item">
                    <div class="stat-number"><%: StatActive %></div>
                    <div class="stat-label">Active</div>
                </div>
                <div class="stat-divider"></div>
                <div class="stat-item">
                    <div class="stat-number"><%: StatUpcoming %></div>
                    <div class="stat-label">Upcoming</div>
                </div>
            </div>
        </div>
    </div>

    <!-- MAIN CONTENT -->
    <div class="container">

        <div class="filter-bar">
            <div class="filter-pill active" data-filter="all">All Bookings</div>
            <div class="filter-pill" data-filter="booked">Upcoming</div>
            <div class="filter-pill" data-filter="postponed">Postponed</div>
            <div class="filter-pill" data-filter="cancelled">Cancelled</div>
        </div>

        <asp:Panel ID="pnlNoBookings" runat="server" Visible="false">
            <div class="no-bookings">
                <div class="no-bookings-icon"><i class="fas fa-ticket-alt"></i></div>
                <h3>No bookings found</h3>
                <p>You don't have any bookings yet. Start your journey today!</p>
                <asp:Button ID="btnBookNew" runat="server"
                    CssClass="btn-book-now"
                    Text="+ Book a Ticket"
                    OnClick="btnBookNew_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlBookings" runat="server" Visible="false">
            <asp:Repeater ID="rptBookings" runat="server">
                <ItemTemplate>
                    <div class="<%# GetCardStatusClass(Container.DataItem) %>"
                         data-status="<%# GetStatusClass(Container.DataItem).Replace("status-","") %>"
                         style="animation-delay:<%# (Container.ItemIndex * 0.05).ToString("F2", System.Globalization.CultureInfo.InvariantCulture) %>s">

                        <div class="booking-header">
                            <div>
                                <div class="pnr-label">PNR Number</div>
                                <div class="pnr-number"><%# GetPnr(Container.DataItem) %></div>
                            </div>
                            <span class="status-badge <%# GetStatusClass(Container.DataItem) %>">
                                <%# GetStatus(Container.DataItem) %>
                            </span>
                        </div>

                        <div class="booking-details">
                            <div class="detail-item">
                                <div class="detail-label">Route</div>
                                <div class="detail-value route-value">
                                    <%# GetFrom(Container.DataItem) %> &#8594; <%# GetTo(Container.DataItem) %>
                                </div>
                            </div>
                            <div class="detail-item">
                                <div class="detail-label">Journey Date</div>
                                <div class="detail-value">
                                    <i class="far fa-calendar-alt"></i> <%# GetJourneyDate(Container.DataItem) %>
                                </div>
                            </div>
                            <div class="detail-item">
                                <div class="detail-label">Seat(s)</div>
                                <div class="detail-value">
                                    <i class="fas fa-chair"></i> <%# GetSeats(Container.DataItem) %>
                                </div>
                            </div>
                            <div class="detail-item">
                                <div class="detail-label">Passengers</div>
                                <div class="detail-value">
                                    <i class="fas fa-users"></i> <%# GetPassengerCount(Container.DataItem) %>
                                </div>
                            </div>
                            <div class="detail-item">
                                <div class="detail-label">Total Amount</div>
                                <div class="detail-value amount-value"><%# GetAmount(Container.DataItem) %></div>
                            </div>
                            <div class="detail-item">
                                <div class="detail-label">Booked On</div>
                                <div class="detail-value"><%# GetBookedOn(Container.DataItem) %></div>
                            </div>
                        </div>

                        <div class="booking-actions">
                            <asp:Button ID="btnViewDetails" runat="server"
                                CssClass="btn-view-details"
                                Text="View Details"
                                CommandArgument='<%# GetPnr(Container.DataItem) %>'
                                OnClick="btnViewDetails_Click" />

                            <asp:Button ID="btnDownloadTicket" runat="server"
                                CssClass="btn-secondary"
                                Text="E-Ticket"
                                CommandArgument='<%# GetPnr(Container.DataItem) %>'
                                OnClick="btnDownloadTicket_Click"
                                Visible='<%# IsBooked(Container.DataItem) %>' />

                            <asp:Button ID="btnCancelBooking" runat="server"
                                CssClass="btn-secondary"
                                Text="Cancel"
                                CommandArgument='<%# GetPnr(Container.DataItem) %>'
                                OnClick="btnCancelBooking_Click"
                                OnClientClick="return confirm('Cancel this booking?');"
                                Visible='<%# IsPending(Container.DataItem) %>' />

                           <%-- <asp:Button ID="btnRateJourney" runat="server"
                                CssClass="btn-secondary"
                                Text="Rate Journey"
                                CommandArgument='<%# GetPnr(Container.DataItem) %>'
                                OnClick="btnRateJourney_Click"
                                Visible='<%# IsCompleted(Container.DataItem) %>' />--%>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var pills = document.querySelectorAll('.filter-pill');
            var getCards = function () { return document.querySelectorAll('.booking-card'); };

            pills.forEach(function (pill) {
                pill.addEventListener('click', function () {
                    pills.forEach(function (p) { p.classList.remove('active'); });
                    pill.classList.add('active');
                    var filter = pill.dataset.filter;
                    getCards().forEach(function (card) {
                        var status = card.dataset.status || '';
                        card.style.display = (filter === 'all' || status === filter) ? '' : 'none';
                    });
                });
            });
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>
