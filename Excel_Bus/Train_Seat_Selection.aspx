<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" 
    AutoEventWireup="true" CodeBehind="Train_Seat_Selection.aspx.cs" 
    Inherits="Excel_Bus.Train_Seat_Selection" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Train Seat Selection
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">

  
    <section class="inner-banner">
    <div class="container">
        <div class="inner-banner-content">
            <h2 class="title">Select Your Train Seat</h2>
            <p class="subtitle">
                <asp:Label ID="lblTrainInfo" runat="server" Text=""></asp:Label>
            </p>
        </div>
    </div>
</section>
    <style>
.inner-banner {
    position: relative;
    padding: 60px 0;
    background: #47a17b;
    overflow: hidden;
}

/* Floating train icons pattern */
.inner-banner::before {
    position: absolute;
    content: "";
    width: 100%;
    height: 100%;
    left: 0;
    top: 0;
    background: #47a17b;
    z-index: 1;
}

/* Decorative SVG icon pattern */
.inner-banner::after {
    position: absolute;
    content: "";
    width: 100%;
    height: 100%;
    left: 0;
    top: 0;
    background-image: url();
    background-size: 80px 80px;
    opacity: 0.25;
    z-index: 0;
}

.inner-banner .container {
    position: relative;
    z-index: 2;
}

.inner-banner-content {
    text-align: center;
}

.inner-banner-content .title {
    color: #fff;
    font-size: 2rem;
    font-weight: 700;
    margin-bottom: 10px;
}

        .inner-banner-content .subtitle {
            color: #f1f1eb;
            font-size: 2rem;
        }
</style>

    <style>
        .train-seat-selection-container {
    padding: 18px 0;
    background: #e4fff3;
    width: 100%;
    margin-bottom: 17px;
}
        .coach-type-badge { display: inline-block; padding: 10px 25px; border-radius: 25px; font-weight: 700; font-size: 18px; margin-bottom: 20px; box-shadow: 0 3px 10px rgba(0,0,0,0.15); }
        .coach-type-badge.luxury { 
            background: #47a17b;
                                   color: #333;

        }


        @media (min-width: 1200px) {
    .col-lg-4 {
        width: 30.333333%;
    }
}
        .coach-type-badge.first-class { background: #47a17b; color: white; }
        .coach-type-badge.second-class { background: #47a17b; color: white; }
        .train-coach-container { background: #68c79f61;
    border-radius: 20px;
    padding: 30px;
    box-shadow: 0 5px 25px rgb(69 177 32 / 29%);
    margin-bottom: 30px;
    border: 3px solid #d7e9d6; }
        .coach-header { text-align: center; padding: 18px; 
                        background: #47a17b;
                        color: white; border-radius: 10px 10px 0 0; margin: -30px -30px 25px -30px; }
        .coach-header h4 { margin: 0; font-size: 22px; font-weight: 700; }
        .coach-header p { margin: 8px 0 0 0; font-size: 15px; opacity: .95; }
        .toilet-indicator {background: #47a17b;
    border: 2px dashed #095c0d;
    border-radius: 10px;
    padding: 18px;
    text-align: center;
    font-weight: 700;
    color: #173405;
    margin: 15px 0;
    font-size: 15px; }

        .toilet-indicator i { font-size: 26px; display: block; margin-bottom: 8px; }
        .seat-row { display: flex; justify-content: center; align-items: center; margin: 10px 0; }
        .seat-group { display: flex; gap: 12px; align-items: center; }
        .aisle { width: 70px; display: inline-block; }
       .seat { 
    min-width: 50px; height: 50px; border-radius: 10px; 
    text-align: center; line-height: 50px; font-size: 13px; 
    font-weight: 700; border: 2.5px solid #dee2e6; 
    background-color: #fff; color: #333; cursor: pointer; 
    transition: all .3s ease; display: inline-block; 
    box-shadow: 0 2px 5px rgba(0,0,0,.08); 
}
       .seat.second-class-seat { 
    width: 65px; height: 48px; line-height: 48px; 
    font-size: 12px; background: #fff; border-color: #66bb6a; 
}
        .seat.window { background: #47a17b; border-color: #66bb6a; }
        .seat.aisle-seat { background: #47a17b; border-color: #29b6f6; }
        .seat.middle { background: #47a17b; border-color: #ffa726; }
       .seat.first-ac-seat { 
    width: 70px; height: 55px; line-height: 55px; 
    font-size: 14px; background: #fff; border-color: #ab47bc; 
}
        .seat.first-ac-window { background: #47a17b; border-color: #9c27b0; }
        .seat.first-ac-aisle { background: linear-gradient(135deg, #d1c4e9 0%, #b39ddb 100%); border-color: #7e57c2; }
        .seat.luxury-seat { 
    width: 80px; height: 65px; line-height: 65px; 
    font-size: 15px; border-radius: 14px; 
    background: #fff; border-color: #fbc02d; font-weight: 800; 
}
       
        .seat.luxury-window { background: #47a17b; border-color: #F9A825; }
        .seat.available:hover { 
    transform: scale(1.1); 
    box-shadow: 0 5px 15px rgba(0,0,0,.25); z-index: 10; 
}
       .seat.selected { 
    background: #47a17b !important; border-color: #388E3C !important; 
    color: white !important; transform: scale(1.08); 
    box-shadow: 0 5px 15px rgba(76,175,80,.4); 
}
.seat.booked { 
    background: #4f46e5 !important; border-color: #4f46e5 !important; 
    color: white !important; cursor: not-allowed; 
    opacity: .75; pointer-events: none; 
}
        .seat-legend { display: flex; gap: 30px; justify-content: center; flex-wrap: wrap; padding: 25px; background: #47a17b; border-radius: 12px; margin-top: 25px; }
        .legend-item { display: flex; align-items: center; gap: 12px; font-size: 14px; font-weight: 600; color: #333; }
        .legend-box { width: 40px; height: 40px; border-radius: 8px; border: 2.5px solid; }
        .selected-seats-summary { background: #fff; border: 3px solid #4CAF50; border-radius: 15px; padding: 25px; margin-bottom: 25px; }
        .selected-seats-summary h5 { margin-bottom: 18px; font-weight: 700; color: #2e7d32; font-size: 18px; }
        .seat-item { display: flex; justify-content: space-between; padding: 12px; background: #47a17b; margin-bottom: 10px; border-radius: 8px; border-left: 4px solid #4CAF50; }
        .alert-info {
    color: #03440f;
    background-color: #2d9b8b8a;
    border-color: #abd99d;
}
        .card {
    --bs-card-spacer-y: 1rem;
    --bs-card-spacer-x: 1rem;
    --bs-card-title-spacer-y: 0.5rem;
    --bs-card-border-width: 1px;
   --bs-card-border-color: rgb(117 205 186 / 37%);
    --bs-card-border-radius: 1.375rem;
    --bs-card-box-shadow: ;
    --bs-card-inner-border-radius: calc(0.375rem - 1px);
    --bs-card-cap-padding-y: 0.5rem;
    --bs-card-cap-padding-x: 1rem;
    --bs-card-cap-bg: rgb(80 215 182 / 20%);
    --bs-card-cap-color: ;
    --bs-card-height: ;
    --bs-card-color: ;
    --bs-card-bg: #78bf9991;
    --bs-card-img-overlay-padding: 1rem;
    --bs-card-group-margin: 0.75rem;
    position: relative;
    display: flex;
    flex-direction: column;
    min-width: 0;
    height: var(--bs-card-height);
    word-wrap: break-word;
    background-color: var(--bs-card-bg);
    background-clip: border-box;
    border: var(--bs-card-border-width) solid var(--bs-card-border-color);
    border-radius: var(--bs-card-border-radius);
}
        .form-control[disabled], .form-control[readonly], fieldset[disabled] .form-control {
    background-color: #d8dddb;
    opacity: 1;
}
        .total-section { margin-top: 18px; padding-top: 18px; border-top: 2px solid #e0e0e0; display: flex; justify-content: space-between; align-items: center; }
        .total-amount { font-size: 26px; font-weight: 700; color: #2e7d32; }
        .postpone-banner { background: #47a17b; border: 2px solid #ffc107; border-radius: 12px; padding: 18px; margin-bottom: 20px; }
        .postpone-banner h5 { color: #856404; margin: 0 0 10px 0; }
        .postpone-banner p { margin: 4px 0; font-size: 14px; }
        .btn-success {
    color: #fff;
   background-color: #428b6c;
    border-color: #3f8d6ed1;
}
        @media(max-width:768px) {
            .seat { 
                min-width: 42px;
                height: 42px; line-height: 42px; font-size: 11px; }
            .seat-group { gap: 8px; }
            .aisle { width: 50px; }
        }

  @media (min-width: 1200px) {
    .container {
        width: 1350px !important;
    }
}
    </style>

    <div class="train-seat-selection-container">
        <div class="container">
            <div class="row">
                <!-- Left: Coach & Seats -->
                <div class="col-lg-8 col-md-7">

                    <!-- ✅ POSTPONE BANNER -->
                    <div id="postponeBanner" class="postpone-banner" style="display:none;">
                        <h5><i class="fas fa-calendar-alt"></i> Postpone Mode Active</h5>
                        <p><strong>PNR:</strong> <span id="postponePnrDisplay" style="color:#764ba2; font-weight:700;"></span></p>
                        <p><strong>Postpone #:</strong> <span id="postponeCountDisplay"></span> of 2</p>
                        <p><strong>Fee Applied:</strong> <span id="postponeFeePercent" style="color:#d32f2f; font-weight:700;"></span> of original ticket price</p>
                        <p style="font-size:13px; color:#ffeeee; margin-top:8px;">
                            <i class="fas fa-info-circle"></i>
                            Select the same number of seat as original booking and choose a new journey date.
                        </p>
                    </div>

                    <div class="text-center mb-4">
                        <asp:Label ID="lblCoachType" runat="server" CssClass="coach-type-badge"></asp:Label>
                        <h4 id="coachTitle" runat="server"></h4>
                    </div>

                    <div class="train-coach-container">
                        <asp:Panel ID="SeatsContainer" runat="server"></asp:Panel>
                    </div>

                    <!-- Legend -->
                    <div class="seat-legend">
                        <div class="legend-item">
                            <div class="legend-box" id="legendBox1" runat="server" style="background:#dcab25; border-color:#dcab25;"></div>
                            <span id="legendLabel1" runat="server">Window Seat</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" id="legendBox2" runat="server" style="background:#e7b27b;border-color:#e7b27b;"></div>
                            <span id="legendLabel2" runat="server">Aisle Seat</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" id="legendBox3" runat="server" style="background:#efeaab;border-color:#efeaab;"></div>
                            <span id="legendLabel3" runat="server">Middle Seat</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" style="background:#064209; border-color:#064209;"></div>
                            <span>Selected</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" style="background:#4f46e5;border-color:#4f46e5;"></div>
                            <span>Booked</span>
                        </div>
                    </div>
                </div>

                <!-- Right: Booking Summary -->
                <div class="col-lg-4 col-md-5">
                    <div class="card mb-3">
                        <div class="card-body">
                            <h5 class="card-title"><i class="fas fa-train"></i> Journey Details</h5>
                            <div class="form-group">
                                <label>Journey Date</label>
                                <asp:TextBox ID="txtJourneyDate" TextMode="Date" runat="server"
                                    CssClass="form-control" AutoPostBack="true"
                                    OnTextChanged="txtJourneyDate_TextChanged"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>From Station</label>
                                <asp:TextBox ID="txtFromStation" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>To Station</label>
                                <asp:TextBox ID="txtToStation" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="alert alert-info">
                                <strong>Seat Price:</strong>
                                <span class="pull-right">CDF <asp:Label ID="lblSeatPrice" runat="server" Text="0"></asp:Label></span>
                            </div>
                        </div>
                    </div>

                    <!-- Selected Seats Summary -->
                    <div id="selectedSeatsSummary" class="selected-seats-summary" style="display:none;">
                        <h5><i class="fas fa-check-circle"></i> Selected Seat</h5>
                        <div id="selectedSeatsContainer"></div>
                        <div class="total-section">
                            <strong>Total Amount:</strong>
                            <span class="total-amount" id="totalAmount">CDF 0</span>
                        </div>
                    </div>

                    <!-- Hidden Fields -->
                    <asp:HiddenField ID="hfSelectedSeats"  runat="server" />
                    <asp:HiddenField ID="hfUnitPrice"      runat="server" />
                    <asp:HiddenField ID="hfCoachNumber"    runat="server" />
                    <asp:HiddenField ID="hfCoachTypeId"    runat="server" />
                    <asp:HiddenField ID="hfTrainId"        runat="server" />
                    <asp:HiddenField ID="hfTripId"         runat="server" />
                    <asp:HiddenField ID="hfUserId"         runat="server" />
                    <asp:HiddenField ID="hfFromStationId"  runat="server" />
                    <asp:HiddenField ID="hfToStationId"    runat="server" />
                    <asp:HiddenField ID="hfPnrNumber"      runat="server" />
                    <asp:HiddenField ID="hfBookedId"       runat="server" />
                    <asp:HiddenField ID="hfTicketCount"    runat="server" />
                    <asp:HiddenField ID="hfPostponeCount"  runat="server" />

                    <asp:Button ID="btnProceedToBook" runat="server"
                        Text="Proceed to Passenger Details"
                        CssClass="btn btn-success btn-lg btn-block"
                        OnClick="btnProceedToBook_Click"
                        UseSubmitBehavior="false" />
                </div>
            </div>
        </div>
   

    <!-- Login Modal -->
    <div class="modal fade" id="loginModal" tabindex="-1" role="dialog" data-backdrop="static">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Login to Continue</h4>
                </div>
                <div class="modal-body">
                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active">
                            <a href="#loginTab" role="tab" data-toggle="tab">Login</a>
                        </li>
                        <li role="presentation">
                            <a href="#signupTab" role="tab" data-toggle="tab">Sign Up</a>
                        </li>
                    </ul>
                    <div class="tab-content" style="padding-top:15px;">
                        <!-- Login Tab -->
                        <div role="tabpanel" class="tab-pane active" id="loginTab">
                            <div class="form-group">
                                <label>Username <span style="color:red;">*</span></label>
                                <input type="text" class="form-control" id="loginUsername" placeholder="Enter username" />
                            </div>
                            <div class="form-group">
                                <label>Password <span style="color:red;">*</span></label>
                                <input type="password" class="form-control" id="loginPassword" placeholder="Enter password" />
                            </div>
                            <button type="button" onclick="handleLogin()" class="btn btn-primary btn-block">Login</button>
                        </div>
                        <!-- Signup Tab -->
                        <div role="tabpanel" class="tab-pane" id="signupTab">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>First Name <span style="color:red;">*</span></label>
                                        <input type="text" class="form-control" id="signupFirstname" placeholder="First name" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Last Name <span style="color:red;">*</span></label>
                                        <input type="text" class="form-control" id="signupLastname" placeholder="Last name" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label>Username <span style="color:red;">*</span></label>
                                <input type="text" class="form-control" id="signupUsername" placeholder="Choose username" />
                            </div>
                            <div class="form-group">
                                <label>Email <span style="color:red;">*</span></label>
                                <input type="email" class="form-control" id="signupEmail" placeholder="Email address" />
                            </div>
                            <div class="form-group">
                                <label>Mobile <span style="color:red;">*</span></label>
                                <input type="tel" class="form-control" id="signupMobile" placeholder="Mobile number" />
                            </div>
                            <div class="form-group">
                                <label>Password <span style="color:red;">*</span></label>
                                <input type="password" class="form-control" id="signupPassword" placeholder="Choose password" />
                            </div>
                            <button type="button" onclick="handleSignup()" class="btn btn-success btn-block">Sign Up</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
         </div>
    <!-- Alert Modal -->
    <div class="modal fade" id="alertModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h5 class="modal-title">Alert</h5>
                </div>
                <div class="modal-body">
                    <strong><p class="alert-message text-danger"></p></strong>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">OK</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        let selectedSeats = [];
        let unitPrice = 0;

        $(document).ready(function () {

            // ── LOGIN CHECK ──────────────────────────────────────────
            checkExistingLogin();

            // ── DATE SETUP ───────────────────────────────────────────
            var minDate = $('input[id*="txtJourneyDate"]').attr('min') ||
                new Date().toISOString().split('T')[0];

            $('input[id*="txtJourneyDate"]').on('change', function () {
                var sel = new Date($(this).val());
                var min = new Date(minDate);
                sel.setHours(0, 0, 0, 0); min.setHours(0, 0, 0, 0);
                if (sel < min) {
                    showAlert('Journey date must be from ' +
                        new Date(minDate).toLocaleDateString('en-GB',
                            { day: '2-digit', month: 'short', year: 'numeric' }) + ' onwards.');
                    $(this).val('');
                }
            });

            $('input[id*="txtJourneyDate"]').on('keydown', function (e) {
                if ([46, 8, 9, 27, 13].indexOf(e.keyCode) === -1 &&
                    !(e.ctrlKey && [65, 67, 86, 88].indexOf(e.keyCode) !== -1))
                    e.preventDefault();
            });

            // ── UNIT PRICE ───────────────────────────────────────────
            unitPrice = parseFloat($('input[id*="hfUnitPrice"]').val()) || 0;

            // ── SEAT CLICK ───────────────────────────────────────────
            //$(document).on('click', '.seat.available', function (e) {
            //    e.preventDefault();


            //    if (!$('input[id*="txtJourneyDate"]').val()) {
            //        showAlert('Please select a journey date first.');
            //        return;
            //    }


                //$(document).on('click', '.seat', function (e) {
                //    e.preventDefault();

                //    // ✅ Booked seat pe click block karo
                //    if ($(this).hasClass('booked')) return;




            $(document).on('click', '.seat', function (e) {
                e.preventDefault();
                e.stopPropagation();

                // Block booked seats — must be FIRST check
                if ($(this).hasClass('booked')) return false;
                if (!$(this).hasClass('available') && !$(this).hasClass('selected')) return false;
                        // Double-safety: never allow booked seats
                    if (!$('input[id*="txtJourneyDate"]').val()) {
                        showAlert('Please select a journey date first.');
                        return;
                    }


                var seatLabel = $(this).text().trim();

                // ✅ Postpone mode — enforce max seat limit
                if (window.isPostponeMode && window.maxSeatsAllowed > 0) {
                    if (!$(this).hasClass('selected') && selectedSeats.length >= window.maxSeatsAllowed) {
                        showAlert('You can only select ' + window.maxSeatsAllowed + ' seat(s) for postponement.');
                        return;
                    }
                }

                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                    selectedSeats = selectedSeats.filter(s => s !== seatLabel);
                } else {
                    $(this).addClass('selected');
                    if (!selectedSeats.includes(seatLabel)) selectedSeats.push(seatLabel);
                }

                updateSummary();
            });

            $('#signupMobile').on('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
        });

        // ── CHECK LOGIN ──────────────────────────────────────────────
        function checkExistingLogin() {
            var hfVal = $('input[id*="hfUserId"]').val();
            if (hfVal && hfVal !== '0') {
                sessionStorage.setItem('userId', hfVal);
                return;
            }
            var ssVal = sessionStorage.getItem('userId');
            if (ssVal && ssVal !== '0') {
                $('input[id*="hfUserId"]').val(ssVal);
            } else {
                $('input[id*="hfUserId"]').val('0');
            }
        }

        // ── UPDATE SUMMARY ───────────────────────────────────────────
        function updateSummary() {
            if (selectedSeats.length === 0) {
                $('#selectedSeatsSummary').hide();
                $('input[id*="hfSelectedSeats"]').val('');
                return;
            }

            $('#selectedSeatsSummary').show();

            var isPostpone = window.isPostponeMode || false;
            var feePercent = window.postponeFeePercent || 1;

            // Reload unitPrice from hidden field (server may have updated it)
            unitPrice = parseFloat($('input[id*="hfUnitPrice"]').val()) || unitPrice;

            var html = '';
            selectedSeats.forEach(function (seat) {
                html += '<div class="seat-item">' +
                    '<span><i class="fas fa-chair"></i> Seat ' + seat + '</span>' +
                    '<span class="text-success font-weight-bold">CDF ' + unitPrice.toLocaleString() + '</span>' +
                    '</div>';
            });

            // ✅ Postpone fee note
            if (isPostpone) {
                html += '<div style="margin-top:8px;padding:8px;background:#47a17b;' +
                    'border-radius:6px;font-size:13px;color:#856404;">' +
                    '<i class="fas fa-info-circle"></i> ' +
                    'This is the postponement fee (' + (feePercent * 100) + '% of original price)</div>';
            }

            $('#selectedSeatsContainer').html(html);
            var total = selectedSeats.length * unitPrice;
            $('#totalAmount').text('CDF ' + total.toLocaleString());
            $('input[id*="hfSelectedSeats"]').val(selectedSeats.join(','));
        }

        // ── HANDLE LOGIN ─────────────────────────────────────────────
        function handleLogin() {
            var API_BASE = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';
            var username = $('#loginUsername').val().trim();
            var password = $('#loginPassword').val().trim();

            if (!username || !password) { alert('Please enter username and password'); return; }

            var $btn = $('.btn-primary');
            $btn.prop('disabled', true).text('Logging in...');

            $.ajax({
                type: "POST",
                url: API_BASE + "TrainUsers/Login",
                data: JSON.stringify({ username: username, password: password }),
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    if (response.success) {
                        sessionStorage.setItem('userId',    response.data.id);
                        sessionStorage.setItem('userEmail', response.data.email);
                        sessionStorage.setItem('userMobile', response.data.mobile);
                        sessionStorage.setItem('userName', response.data.username || response.data.userName || '');
                        sessionStorage.setItem('userData',  JSON.stringify(response.data));
                        $('input[id*="hfUserId"]').val(response.data.id);
                        $('input[id*="hfSelectedSeats"]').val(selectedSeats.join(','));

                        $('#loginModal').modal('hide');
                        $('#loginModal').removeClass('in show').hide();
                        $('.modal-backdrop').remove();
                        $('body').removeClass('modal-open');
                        $('#loginUsername, #loginPassword').val('');

                        setTimeout(function () {

                            $('input[id*="hfSelectedSeats"]').val(selectedSeats.join(','));
                            $('input[id*="hfUserId"]').val(response.data.id);

                            if (!$('input[id*="txtJourneyDate"]').val()) {
                                showAlert('Please select a journey date before proceeding.');
                                return;
                            }

                            var btnName = $('input[id*="btnProceedToBook"]').attr('name');
                            if (typeof __doPostBack === 'function') {
                                __doPostBack(btnName, '');
                            }
                        }, 500);
                    } else {
                        alert(response.message || 'Login failed');
                    }
                },
                error: function () { alert('Login failed. Please try again.'); },
                complete: function () { $btn.prop('disabled', false).text('Login'); }
            });
        }

        // ── HANDLE SIGNUP ────────────────────────────────────────────
        function handleSignup() {
            var API_BASE   = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';
            var firstname = $('#signupFirstname').val().trim();
            var lastname = $('#signupLastname').val().trim();
            var username = $('#signupUsername').val().trim();
            var email = $('#signupEmail').val().trim();
            var mobile = $('#signupMobile').val().trim();
            var password = $('#signupPassword').val().trim();

            if (!firstname || !lastname || !username || !email || !mobile || !password) {
                alert('Please fill all fields'); return;
            }
            if (!/^\d+$/.test(mobile)) { alert('Mobile number must contain only digits'); return; }
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { alert('Please enter a valid email'); return; }

            var $btn = $('button[onclick="handleSignup()"]');
            $btn.prop('disabled', true).text('Checking...');

            $.ajax({
                type: "GET", url: API_BASE + "TrainUsers/GetTrainUsers",
                success: function (existingUsers) {
                    if (existingUsers.some(u => u.email && u.email.toLowerCase() === email.toLowerCase())) { alert('Email already registered.'); $btn.prop('disabled', false).text('Sign Up'); return; }
                    if (existingUsers.some(u => u.mobile && u.mobile === mobile)) { alert('Mobile already registered.'); $btn.prop('disabled', false).text('Sign Up'); return; }
                    if (existingUsers.some(u => u.username && u.username.toLowerCase() === username.toLowerCase())) { alert('Username already taken.'); $btn.prop('disabled', false).text('Sign Up'); return; }

                    $btn.text('Signing up...');
                    $.ajax({
                        type: "POST", url: API_BASE + "TrainUsers/PostTrainUser",
                        data: JSON.stringify({ firstname, lastname, username, email, mobile, password }),
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            if (response.success) {
                                $('#signupFirstname,#signupLastname,#signupUsername,#signupEmail,#signupMobile,#signupPassword').val('');
                                alert('Registration successful! Please login.');
                                $('a[href="#loginTab"]').tab('show');
                            } else {
                                alert(response.message || 'Registration failed');
                            }
                        },
                        error: function () { alert('Registration failed. Please try again.'); },
                        complete: function () { $btn.prop('disabled', false).text('Sign Up'); }
                    });
                },
                error: function () { alert('Unable to verify. Please try again.'); $btn.prop('disabled', false).text('Sign Up'); }
            });
        }

        // ── SHOW ALERT ───────────────────────────────────────────────
        function showAlert(message) {
            $('.alert-message').text(message);
            $('#alertModal').modal('show');
        }
    </script>
</asp:Content>