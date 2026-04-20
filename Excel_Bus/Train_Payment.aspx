<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_Payment.aspx.cs" Inherits="Excel_Bus.Train_Payment" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     Payment
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@300;400;600;700&family=DM+Sans:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

    <style>
        :root {
           --primary: #084802;
    --primary-light: #1eb31d;
    --primary-dim: rgb(101 197 61 / 35%);
    --primary-border: rgb(70 229 117 / 20%);
    --accent: #2b8d13;
    --success: #10B981;
    --bg: #c7f3b3;
    --surface: #FFFFFF;
    --surface-2: #b9e9b59e;
    --surface-3: #F1F5F9;
    --text-primary: #234e05;
    --text-secondary: #238d07;
    --text-dim: #349b13;
    --border: #bae9af;
    --border-focus: #e8fde6;
    --shadow-sm: 0 1px 3px rgb(85 175 75), 0 1px 2px rgb(192 205 196 / 4%);
    --shadow-md: 0 4px 16px rgb(89 169 63 / 8%), 0 2px 6px rgba(0, 0, 0, 0.04);
    --shadow-lg: 0 12px 40px rgb(88 177 77 / 12%), 0 4px 16px rgb(95 161 79 / 6%);
    --radius: 8px;
    --radius-lg: 16px;
        }

        * { box-sizing: border-box; margin: 0; padding: 0; }

      /*  .pay-wrap {
            min-height: 100vh;
            background: #c4fbe4a3;
            background-image:
                radial-gradient(ellipse 70% 50% at 50% -10%, rgba(79,70,229,0.06) 0%, transparent 60%),
                radial-gradient(ellipse 40% 30% at 90% 80%, rgba(6,182,212,0.05) 0%, transparent 50%);
            padding: 60px 20px;
            font-family: 'DM Sans', sans-serif;
        }

        .pay-inner {
            max-width: 720px;
            margin: 0 auto;
            animation: fadeUp 0.6s cubic-bezier(0.22,1,0.36,1) both;
        }
*/
        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(24px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* ── Header ── */
        .pay-header {
            text-align: center;
             margin-bottom: 32px;
             margin-top: 40px;
        }

        .pay-header .eyebrow {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 11px;
            letter-spacing: 0.18em;
            text-transform: uppercase;
            color: var(--primary);
            margin-bottom: 16px;
            padding: 6px 16px;
            border: 1px solid var(--primary-border);
            border-radius: 40px;
            background: #47a17b;
            font-weight: 600;
        }

        .pay-header h1 {
            font-family: 'Cormorant Garamond', serif;
            font-size: clamp(30px, 5vw, 44px);
            font-weight: 400;
           color: #011a07;
            line-height: 1.15;
        }

        .pay-header h1 strong {
            font-weight: 700;
           background: #47a17b;
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }

        .pay-header p {
            margin-top: 10px;
            color: var(--text-secondary);
            font-size: 14px;
        }

        /* ── Card shell ── */
        .pay-card {
            background: #47a17b;
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            box-shadow: var(--shadow-lg);
            overflow: hidden;
            margin-bottom: 32px;
        }

        /* ── Section ── */
        .pay-section {
            padding: 32px 36px;
            border-bottom: 1px solid var(--border);
        }

        .pay-section:last-child { border-bottom: none; }

        .section-label {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 13px;
            letter-spacing: 0.18em;
            text-transform: uppercase;
            color: white;
            font-weight: 700;
            margin-bottom: 20px;
        }

        .section-label::after {
            content: '';
            flex: 1;
            height: 1px;
            background: #47a17b;
        }

        /* ── Detail rows ── */
        .detail-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1px;
            background: #47a17b;
            border: 1px solid var(--border);
            border-radius: var(--radius);
            overflow: hidden;
        }

        .detail-cell {
            background: var(--surface);
            padding: 16px 20px;
            transition: background 0.2s;
        }

        .detail-cell:hover { background: #47a17b; }

        .detail-cell .lbl {
            font-size: 10px;
            letter-spacing: 0.12em;
            text-transform: uppercase;
            color: var(--text-dim);
            font-weight: 600;
            margin-bottom: 5px;
        }

        .detail-cell .val {
            font-size: 14px;
            color: var(--text-primary);
            font-weight: 600;
        }

        .detail-cell .val.pnr {
            font-family: 'Cormorant Garamond', serif;
            font-size: 22px;
            font-weight: 700;
            letter-spacing: 0.06em;
                background: #47a17b;
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }

        .detail-cell.full { grid-column: 1 / -1; }

        /* ── Amount Banner ── */
        .amount-banner {
            padding: 32px 36px;
            background: #beefda;
            border-top: 1px solid var(--border-focus);
            border-bottom: 1px solid var(--border-focus);
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .amount-banner .label-col .top {
            font-size: 10px;
            letter-spacing: 0.15em;
            text-transform: uppercase;
            color: var(--primary);
            font-weight: 700;
        }

        .amount-banner .label-col .bottom {
            margin-top: 4px;
            font-size: 13px;
            color: var(--text-secondary);
        }

        .amount-banner .amount-col {
            text-align: right;
        }

        .amount-banner .amount-col .currency {
            font-size: 20px;
            color: var(--primary);
            vertical-align: super;
            margin-right: 2px;
            font-weight: 700;
        }

        .amount-banner .amount-col .number {
            font-family: 'Cormorant Garamond', serif;
            font-size: 52px;
            font-weight: 700;
            color: var(--primary);
            line-height: 1;
        }

        /* ── Payment Methods ── */
        .method-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .method-item {
            display: flex;
            align-items: center;
            gap: 16px;
            padding: 16px 20px;
            border: 1.5px solid var(--border);
            border-radius: var(--radius);
            cursor: pointer;
            transition: all 0.2s ease;
            background:#bceed9;
            position: relative;
            overflow: hidden;
        }

        .method-item::before {
            content: '';
            position: absolute;
            inset: 0;
            background: #d4f7e8;
            opacity: 0;
            transition: opacity 0.2s;
        }

        .method-item:hover { border-color: var(--border-focus); box-shadow: var(--shadow-sm); }
        .method-item:hover::before { opacity: 1; }

        .method-item.selected {
            border-color: #a8e7a3;
    background: #c2f5e0;
            box-shadow: 0 0 0 3px rgba(79,70,229,0.08);
        }

        .method-item.selected::before { opacity: 1; }

        .method-item input[type="radio"] { display: none; }

        .method-radio {
            width: 20px;
            height: 20px;
            border-radius: 50%;
            border: 2px solid var(--text-dim);
            flex-shrink: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.2s;
            position: relative;
            z-index: 1;
        }

        .method-item.selected .method-radio {
            border-color: var(--primary);
        }

        .method-item.selected .method-radio::after {
            content: '';
            width: 8px;
            height: 8px;
            border-radius: 50%;
            background: #053420;
        }

        .method-icon {
            width: 42px;
            height: 42px;
            border-radius: var(--radius);
            background: #47a17b;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 16px;
            flex-shrink: 0;
            position: relative;
            z-index: 1;
            transition: all 0.2s;
        }

        .method-item.selected .method-icon {
            background: #47a17b;
            color: white;
        }

        .method-text {
            flex: 1;
            position: relative;
            z-index: 1;
        }

        .method-text .name {
            font-size: 14px;
            font-weight: 600;
            color: var(--text-primary);
        }

        .method-text .desc {
            font-size: 12px;
            color:#052c11;
            margin-top: 2px;
        }

        .method-badge {
            font-size: 10px;
            letter-spacing: 0.08em;
            text-transform: uppercase;
           color: #f5fbf9;
    background: #78d3ad;
            border: 1px solid rgba(16,185,129,0.2);
            padding: 3px 8px;
            border-radius: 20px;
            position: relative;
            z-index: 1;
            font-weight: 600;
        }

        /* ── CTA ── */
        .pay-cta-wrap {
            padding: 32px 36px;
        }

        .btn-pay {
            width: 100%;
            padding: 18px 40px;
            background: #02502f;
            background-size: 200% auto;
            color: #fff;
            font-family: 'DM Sans', sans-serif;
            font-size: 15px;
            font-weight: 700;
            letter-spacing: 0.1em;
            text-transform: uppercase;
            border: none;
            border-radius: var(--radius);
            cursor: pointer;
            transition: all 0.4s ease;
            position: relative;
            overflow: hidden;
            box-shadow: 0 4px 14px rgba(79,70,229,0.3);
        }

        .btn-pay::after {
            content: '';
            position: absolute;
            inset: 0;
            background: #47a17b;
            opacity: 0;
            transition: opacity 0.2s;
        }

        .btn-pay:hover {
            background-position: right center;
            box-shadow: 0 8px 28px rgba(79,70,229,0.4);
            transform: translateY(-2px);
            color: #47a17b;
        }

        .btn-pay:hover::after { opacity: 1; }

        .btn-pay:active {
            transform: translateY(0);
            box-shadow: 0 2px 8px rgba(79,70,229,0.2);
        }

        .btn-pay i { margin-right: 10px; }

        /* ── Trust bar ── */
        .trust-bar {
            display: flex;
            justify-content: center;
            gap: 28px;
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid var(--border);
            flex-wrap: wrap;
        }

        .trust-item {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 11px;
            color: white;
            letter-spacing: 0.04em;
            font-weight: 500;
        }

        .trust-item i {
            color: white;
            font-size: 12px;
        }

        /* ── Responsive ── */
        @media (max-width: 600px) {
            .pay-section { padding: 24px 20px; }
            .pay-cta-wrap { padding: 24px 20px; }
            .detail-grid { grid-template-columns: 1fr; }
            .detail-cell.full { grid-column: 1; }
            .amount-banner { flex-direction: column; gap: 16px; text-align: center; }
            .amount-banner .amount-col { text-align: center; }
            .amount-banner .label-col { text-align: center; }
            .trust-bar { gap: 16px; }
        }
         @media (min-width: 1200px) {
    .container {
        width: 1350px;
    }
}
    </style>

    <div class="pay-wrap">
        <div class="pay-inner">

            <!-- Header -->
            <div class="pay-header">
                <div class="eyebrow"><i class="fas fa-train"></i> Secure Checkout</div>
                <h1>Complete Your <strong>Payment</strong></h1>
                <p>Review your booking details before confirming</p>
            </div>

            <!-- Card -->
            <div class="pay-card">

                <!-- Booking Details -->
                <div class="pay-section">
                    <div class="section-label"><i class="fas fa-ticket-alt"></i> Booking Details</div>

                    <div class="detail-grid">
                        <div class="detail-cell">
                            <div class="lbl">PNR Number</div>
                            <div class="val pnr"><asp:Label ID="lblPNRNumber" runat="server"></asp:Label></div>
                        </div>
                        <div class="detail-cell">
                            <div class="lbl">Booking ID</div>
                            <div class="val"><asp:Label ID="lblBookedId" runat="server"></asp:Label></div>
                        </div>
                        <div class="detail-cell">
                            <div class="lbl">Journey Date</div>
                            <div class="val"><asp:Label ID="lblJourneyDate" runat="server"></asp:Label></div>
                        </div>
                        <div class="detail-cell">
                            <div class="lbl">Selected Seats</div>
                            <div class="val"><asp:Label ID="lblSelectedSeats" runat="server"></asp:Label></div>
                        </div>
                        <div class="detail-cell">
                            <div class="lbl">Route</div>
                            <div class="val"><asp:Label ID="lblRoute" runat="server"></asp:Label></div>
                        </div>
                        <div class="detail-cell">
                            <div class="lbl">Ticket Amount</div>
                            <div class="val"><asp:Label ID="lblAmount" runat="server"></asp:Label></div>
                        </div>
                    </div>
                </div>


                <!-- Amount -->
                <div class="amount-banner">
                    <div class="label-col">
                        <div class="top">Total Amount Due</div>
                        <div class="bottom">Inclusive of all taxes &amp; fees</div>
                    </div>
                    <div class="amount-col">
                        <span class="currency">₹</span>
                        <span class="number"><asp:Label ID="lblTotalAmount" runat="server"></asp:Label></span>
                    </div>
                </div>

                <!-- Payment Methods -->
                <div class="pay-section">
                    <div class="section-label"><i class="fas fa-wallet"></i> Payment Method</div>

                    <div class="method-list">
                        <div class="method-item selected" data-method="card">
                            <input type="radio" id="method1" name="paymentMethod" value="card" checked />
                            <div class="method-radio"></div>
                            <div class="method-icon"><i class="fas fa-credit-card"></i></div>
                            <div class="method-text">
                                <div class="name">Credit / Debit Card</div>
                                <div class="desc">Visa, Mastercard, RuPay &amp; more</div>
                            </div>
                            <div class="method-badge">Popular</div>
                        </div>

                        <div class="method-item" data-method="mobile">
                            <input type="radio" id="method2" name="paymentMethod" value="mobile" />
                            <div class="method-radio"></div>
                            <div class="method-icon"><i class="fas fa-mobile-alt"></i></div>
                            <div class="method-text">
                                <div class="name">Mobile Money / UPI</div>
                                <div class="desc">GPay, PhonePe, Paytm &amp; more</div>
                            </div>
                        </div>

                        <div class="method-item" data-method="bank">
                            <input type="radio" id="method3" name="paymentMethod" value="bank" />
                            <div class="method-radio"></div>
                            <div class="method-icon"><i class="fas fa-university"></i></div>
                            <div class="method-text">
                                <div class="name">Net Banking</div>
                                <div class="desc">All major Indian banks supported</div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- CTA -->
                <div class="pay-cta-wrap">
                   <%-- <asp:Button ID="btnConfirmPayment" runat="server"
                        Text="Confirm & Pay Now"
                        CssClass="btn-pay"
                        OnClick="btnConfirmPayment_Click" />--%>
                    <asp:Button ID="btnConfirmPayment" runat="server"
                    Text="Confirm &amp; Pay Now"
                    CssClass="btn-pay"
                    OnClick="btnConfirmPayment_Click"
                    OnClientClick="this.value='Processing...'; this.style.opacity='0.8';" />

                    <div class="trust-bar">
                        <div class="trust-item"><i class="fas fa-shield-alt"></i> 256-bit SSL Encrypted</div>
                        <div class="trust-item"><i class="fas fa-lock"></i> Secure Payments</div>
                        <div class="trust-item"><i class="fas fa-undo"></i> Instant Refund Policy</div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {

            // Payment method selection
            $('.method-item').on('click', function () {
                $('.method-item').removeClass('selected');
                $(this).addClass('selected');
                $(this).find('input[type="radio"]').prop('checked', true);
            });

            
            $('.btn-pay').on('click', function () {
                var $btn = $(this);
                // Small delay so postback registers before UI changes
                setTimeout(function () {
                    $btn.html('<i class="fas fa-circle-notch fa-spin"></i> Processing...');
                    $btn.css('opacity', '0.8');
                }, 100);
                // Do NOT call $btn.prop('disabled', true) here
            });

        });
    </script>
</asp:Content>
