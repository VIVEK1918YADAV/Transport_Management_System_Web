<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Select_Seat.aspx.cs" Inherits="Excel_Bus.Select_Seat" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxst.icons8.com/vue-static/landings/line-awesome/line-awesome/1.3.0/css/line-awesome.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">

    <section class="inner-banner bg_img" style="background: url('img/bg_bus_img.jpg') center">
        <div class="container">
            <div class="inner-banner-content">
                <h2 class="title">Select Your Seats</h2>
            </div>
        </div>
    </section>

    <style>
        .select2-container {
            z-index: 999999 !important;
        }

        .select2-dropdown {
            z-index: 999999 !important;
        }

        .seat-plan-inner {
            background: #f8f9fa;
            padding: 10px;
            position: relative;
            border: 1px solid #b5dcff;
            border-radius: 69px;
        }

        .frontArea {
            display: flex;
            align-items: center;
            justify-content: space-between;
            width: calc(100% - 200px);
            margin: 0 auto;
        }

            .frontArea span.d1 {
                color: #9d9d9d;
                font-weight: 600;
                font-size: 16px;
                text-transform: uppercase;
            }

            .frontArea span.d2 img {
                filter: invert(1);
            }

        .seat-plan-inner span.f_col {
            position: absolute;
            top: 0;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #e3e3e3;
            padding: 3px 15px;
            font-size: 14px;
            color: #000000;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }

        .seat-plan-inner span.rear_col {
            position: absolute;
            bottom: -25px;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #e3e3e3;
            padding: 3px 15px;
            font-size: 14px;
            color: #000000;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }

        .seats-dynamic-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-start;
            padding: 20px;
            width: 100%;
            min-width: 350px;
        }

        .seat-wrapper {
            display: flex;
            flex-direction: row;
            align-items: center;
            justify-content: flex-start;
            gap: 30px;
            margin: 10px 0;
            padding: 5px 0;
            width: auto;
            min-width: fit-content;
        }

        .left-side,
        .right-side {
            display: inline-flex;
            flex-direction: row;
            gap: 8px;
            align-items: center;
            flex-wrap: nowrap;
            white-space: nowrap;
        }

        .seat-container {
            display: inline-block;
            flex-shrink: 0;
        }

        .seat.btn-seat {
            display: inline-block;
            width: 50px;
            height: 50px;
            border-radius: 8px;
            text-align: center;
            line-height: 50px;
            font-size: 11px;
            font-weight: 600;
            text-decoration: none;
            border: 2px solid #dee2e6;
            background-color: #ffffff;
            color: #333;
            cursor: pointer;
            transition: all 0.3s ease;
            position: relative;
            margin: 0;
            padding: 0;
        }

            .seat.btn-seat.available:hover {
                background-color: #e8f5e9;
                border-color: #4CAF50;
                transform: scale(1.05);
                box-shadow: 0 2px 8px rgba(76, 175, 80, 0.3);
            }

            .seat.btn-seat.selected,
            .selected-seat {
                background-color: #4CAF50 !important;
                border-color: #45a049 !important;
                color: white !important;
            }

            .seat.btn-seat.booked,
            .btn-seat.booked {
                background-color: #6366f1 !important;
                border-color: #4f46e5 !important;
                color: white !important;
                cursor: not-allowed;
                opacity: 0.9;
                pointer-events: none;
            }

            .seat.btn-seat:disabled {
                opacity: 0.5;
                cursor: not-allowed;
                pointer-events: none;
            }

        .seat-legend {
            display: flex;
            gap: 20px;
            justify-content: center;
            margin: 20px 0;
            padding: 15px;
            background-color: #f5f5f5;
            border-radius: 8px;
            flex-wrap: wrap;
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .legend-box {
            width: 30px;
            height: 30px;
            border-radius: 5px;
            border: 2px solid;
        }

            .legend-box.available {
                background-color: #ffffff;
                border-color: #dee2e6;
            }

            .legend-box.selected {
                background-color: #4CAF50;
                border-color: #45a049;
            }

            .legend-box.booked {
                background-color: #6366f1;
                border-color: #4f46e5;
            }

        .selected-seats-summary {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 20px;
        }

            .selected-seats-summary h6 {
                margin-bottom: 10px;
                font-weight: bold;
            }

        .seat-item {
            display: flex;
            justify-content: space-between;
            padding: 8px;
            background: white;
            margin-bottom: 5px;
            border-radius: 4px;
        }

        @media (max-width: 768px) {
            .seat.btn-seat {
                width: 42px;
                height: 42px;
                line-height: 42px;
                font-size: 9px;
            }

            .left-side,
            .right-side {
                gap: 5px;
            }

            .seat-wrapper {
                gap: 20px;
            }
        }

        @media (max-width: 480px) {
            .seat.btn-seat {
                width: 38px;
                height: 38px;
                line-height: 38px;
                font-size: 8px;
            }

            .left-side,
            .right-side {
                gap: 3px;
            }

            .seat-wrapper {
                gap: 15px;
                margin: 5px 0;
            }
        }


   /* Main container */
.route-app {
    max-width: 100%;
    margin: 20px auto;
    padding: 20px;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.route-app label {
    font-size: 1rem;
    font-weight: 600;
    color: #2c3e50;
    margin-bottom: 8px;
    display: block;
}

.route-app select {
    width: 100%;
    padding: 10px;
    font-size: 0.95rem;
    border: 2px solid #ddd;
    border-radius: 8px;
    margin-bottom: 15px;
    background-color: white;
    cursor: pointer;
}

/* Route info header */
.route-info-header {
    text-align: center;
    padding: 12px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border-radius: 10px 10px 0 0;
    margin-bottom: 0;
}

.route-info-header h3 {
    margin: 0 0 6px 0;
    font-size: 1.3rem;
}

.route-info-header p {
    margin: 0;
    font-size: 0.9rem;
    opacity: 0.95;
}

/* Route container */
.route-container {
    background-color: #fff;
    border-radius: 10px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    overflow: hidden;
}

/* Map container - TRUE DIAGONAL */
.route-map-container {
    position: relative;
    width: 100%;
    height: 450px;
    max-height: 450px;
    background: linear-gradient(135deg, #f0f4ff 0%, #fafbff 100%);
    overflow: hidden;
}

/* Add decorative yellow patches */
.route-map-container::before {
    content: '';
    position: absolute;
    width: 160px;
    height: 160px;
    background: #FFE55C;
    opacity: 0.25;
    border-radius: 50%;
    bottom: 15%;
    left: 10%;
    z-index: 0;
}

.route-map-container::after {
    content: '';
    position: absolute;
    width: 200px;
    height: 200px;
    background: #FFE55C;
    opacity: 0.2;
    border-radius: 50%;
    top: 15%;
    right: 8%;
    z-index: 0;
}

/* SVG for the path */
.route-svg {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1;
}

/* Pins container */
.route-pins {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 2;
}

/* Individual route pin */
.route-pin {
    position: absolute;
    transform: translate(-50%, -50%);
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.route-pin:hover {
    z-index: 10;
}

.route-pin:hover .pin-icon {
    transform: scale(1.2);
}

.route-pin:hover .pin-label {
    box-shadow: 0 3px 10px rgba(0,0,0,0.25);
    transform: scale(1.05);
}

/* Pin icon */
.pin-icon {
    width: 20px;
    height: 28px;
    position: relative;
    filter: drop-shadow(0 2px 4px rgba(0,0,0,0.35));
    transition: transform 0.2s ease;
    flex-shrink: 0;
}

.pin-icon::before {
    content: '';
    position: absolute;
    width: 20px;
    height: 20px;
    border-radius: 50% 50% 50% 0;
    transform: rotate(-45deg);
    left: 0;
    top: 0;
}

.pin-icon::after {
    content: '';
    position: absolute;
    width: 6px;
    height: 6px;
    background: white;
    border-radius: 50%;
    left: 7px;
    top: 4px;
    transform: rotate(45deg);
}

/* Start pin (dark/black) */
.route-pin.start .pin-icon::before {
    background: #2c3e50;
    border: 2px solid #fff;
}

/* Stoppage pin (blue) */
.route-pin.stoppage .pin-icon::before {
    background: #3498db;
    border: 2px solid #fff;
}

/* End pin (red/orange) */
.route-pin.end .pin-icon::before {
    background: #FF6B6B;
    border: 2px solid #fff;
}

/* Pin labels - DIAGONAL positioning */
.pin-label {
    text-align: center;
    background: white;
    padding: 6px 12px;
    border-radius: 6px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.18);
    white-space: nowrap;
    min-width: 80px;
    max-width: 160px;
    border: 1.5px solid #f0f0f0;
    transition: all 0.2s ease;
}

/* Label positioning variations for diagonal flow */
.pin-label.label-left {
    position: absolute;
    right: 32px;
    top: 50%;
    transform: translateY(-50%);
}

.pin-label.label-right {
    position: absolute;
    left: 32px;
    top: 50%;
    transform: translateY(-50%);
}

.pin-label.label-bottom-left {
    position: absolute;
    top: 35px;
    right: -10px;
}

.pin-label.label-top-right {
    position: absolute;
    bottom: 35px;
    left: -10px;
}

.pin-name {
    font-weight: 700;
    color: #2c3e50;
    font-size: 0.85rem;
    margin: 0;
    overflow: hidden;
    text-overflow: ellipsis;
}

/* Route legend */
.route-legend {
    display: flex;
    justify-content: center;
    gap: 25px;
    padding: 12px;
    background: #f8f9fa;
    border-radius: 0 0 10px 10px;
}

.legend-item {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 0.85rem;
    color: #555;
    font-weight: 500;
}

.legend-dot {
    width: 16px;
    height: 16px;
    border-radius: 50%;
    display: inline-block;
    border: 2px solid white;
    box-shadow: 0 1px 3px rgba(0,0,0,0.2);
}

.legend-dot.start {
    background: #2c3e50;
}

.legend-dot.stoppage {
    background: #3498db;
}

.legend-dot.end {
    background: #FF6B6B;
}

/* Placeholder text */
.placeholder-text {
    text-align: center;
    padding: 60px 20px;
    color: #95a5a6;
    font-size: 1rem;
}

/* Responsive design */
@media (max-width: 768px) {
    .route-app {
        padding: 15px;
    }
    
    .route-map-container {
        height: 380px;
        max-height: 380px;
    }
    
    .pin-icon {
        width: 18px;
        height: 26px;
    }
    
    .pin-icon::before {
        width: 18px;
        height: 18px;
    }
    
    .pin-icon::after {
        width: 5px;
        height: 5px;
        left: 6.5px;
        top: 3.5px;
    }
    
    .pin-label {
        font-size: 0.75rem;
        padding: 5px 10px;
        min-width: 70px;
        max-width: 130px;
    }
    
    .pin-name {
        font-size: 0.8rem;
    }
    
    .pin-label.label-left {
        right: 28px;
    }
    
    .pin-label.label-right {
        left: 28px;
    }
    
    .pin-label.label-bottom-left {
        top: 30px;
    }
    
    .pin-label.label-top-right {
        bottom: 30px;
         position: absolute;
    left: 3px;
    top: 140%;
    transform: translateY(-50%);
    }
    
    .route-legend {
        flex-wrap: wrap;
        gap: 15px;
    }
   
}

@media (max-width: 480px) {
    .route-map-container {
        height: 320px;
        max-height: 320px;
    }
    
    .pin-label {
        min-width: 40px;
        max-width: 100px;
        padding: 4px 8px;
    }
    
    .pin-name {
        font-size: 0.75rem;
    }
    
    .route-legend {
        flex-direction: column;
        gap: 10px;
        align-items: center;
    }
}

    </style>

    <div class="padding-top padding-bottom">
        <div class="container">
            <div class="col-lg-6 col-md-6">
                <div class="seat-overview-wrapper">
                    <div class="row gx-xl-5 gy-4 gy-sm-5 justify-content-center">
                        <form style="place-content:center">
                            <div class="col-12">
                                <div class="form-group">
                                    <label for="date_of_journey" class="form-label">Journey Date</label>
                                    <asp:TextBox ID="date_of_journey" TextMode="Date" runat="server" AutoPostBack="true"
                                        OnTextChanged="date_of_journey_TextChanged" CssClass="form--control date-range"
                                        Placeholder="Date of Journey"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-12">
                                <div class="form--group">
                                    <label for="pickup" class="form-label">Pickup Point</label>
                                    <asp:DropDownList ID="ddlPickup" runat="server" CssClass="form--control select2">
                                    </asp:DropDownList>
                                </div>

                                <div class="form--group">
                                    <label for="destination" class="form-label">Dropping Point</label>
                                    <asp:DropDownList ID="ddlDestination" runat="server" CssClass="form--control select2">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <!-- Selected Seats Summary -->
                            <div id="selectedSeatsSummary" class="selected-seats-summary" style="display: none;">
                                <h6>Selected Seats</h6>
                                <div id="selectedSeatsContainer"></div>
                                <hr />
                                <div class="d-flex justify-content-between">
                                    <strong>Total Amount:</strong>
                                    <strong id="totalAmount">CDF 0</strong>
                                </div>
                            </div>

                            <asp:HiddenField ID="hfSelectedSeats" runat="server" />
                            <asp:HiddenField ID="hfUnitPrice" runat="server" />
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                            <asp:HiddenField ID="HiddenField2" runat="server" />
                            <asp:HiddenField ID="hfUserId" runat="server" />

                            <!-- FIXED: Added UseSubmitBehavior="true" for proper postback -->
                            <asp:Button ID="btnProceedToBook" runat="server"
                                Text="Proceed to Book"
                                CssClass="btn btn-primary w-100"
                                OnClick="btnProceedToBook_Click"
                                UseSubmitBehavior="true" />
                        </form>
                    </div>
                    </div>
                  <div class="route-app">
        <label for="routeSelect">Select a Route:</label>
        <select id="routeSelect" onchange="displaySelectedRoute()">
            <option value="">-- Loading Routes... --</option>
        </select>

        <div id="routeDisplay" class="route-container">
            <p class="placeholder-text">Please select a route to see the itinerary.</p>
        </div>
    </div>
                 </div>
              
                    <div class="col-lg-6 col-md-6">
                        <h6 class="title">Click on Seat to select or deselect</h6>

                        <div class="col-12">
                            <span style="color: #006400; background-color: #CCFFCC; padding: 2px 6px; border-radius: 3px;">
                                <asp:Label ID="days_off" runat="server" CssClass="form-label" Text="Off Days: "></asp:Label>
                            </span>
                        </div>

                        <div class="seat-plan-inner">
                            <span class="f_col">Front</span>
                            <div class="frontArea">
                                <span class="d1">Door</span>
                                <span class="d2">
                                    <img src="https://excelbus.mavenxone.com/assets/templates/basic/images/icon/wheel.svg" alt="icon" />
                                     <%-- <img src="img/driver.jpg"  alt="icon" />--%>
                                </span>
                            </div>
                           

                            <asp:Panel ID="SeatsContainer" runat="server" CssClass="seats-dynamic-container">
                            </asp:Panel>

                            <span class="rear_col">Rear</span>
                        </div>

                        <div class="seat-for-reserved">
                            <div class="seat-condition available-seat">
                                <span class="seat"><span></span></span>
                                <p>Available Seats</p>
                            </div>
                            <div class="seat-condition selected-by-you">
                                <span class="seat"><span></span></span>
                                <p>Selected by You</p>
                            </div>
                            <div class="seat-condition selected-by-gents">
                                <div class="seat"><span></span></div>
                                <p>Booked</p>
                            </div>
                        </div>
                   
            </div>
        </div>
    </div>
  

   

    <!-- Login/Signup Modal -->
    <div class="modal fade" id="loginModal" tabindex="-1" role="dialog" aria-labelledby="loginModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <h4 class="modal-title" id="loginModalLabel">Login to Continue</h4>
                </div>
                <div class="modal-body">
                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active">
                            <a href="#loginTab" aria-controls="loginTab" role="tab" data-toggle="tab">Login</a>
                        </li>
                        <li role="presentation">
                            <a href="#signupTab" aria-controls="signupTab" role="tab" data-toggle="tab">Sign Up</a>
                        </li>
                    </ul>

                    <div class="tab-content" style="padding-top: 15px;">
                        <!-- Login Tab -->
                        <div role="tabpanel" class="tab-pane active" id="loginTab">
                            <form id="loginForm">
                                <div class="form-group">
                                    <label>Username <span style="color: red;">*</span></label>
                                    <input type="text" class="form-control" id="loginUsername" placeholder="Enter username" required />
                                </div>
                                <div class="form-group">
                                    <label>Password <span style="color: red;">*</span></label>
                                    <input type="password" class="form-control" id="loginPassword" placeholder="Enter password" required />
                                </div>
                                <button type="submit" class="btn btn-primary btn-block">Login</button>
                            </form>
                        </div>

                        <!-- Signup Tab -->
                        <div role="tabpanel" class="tab-pane" id="signupTab">
                            <form id="signupForm">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label>First Name <span style="color: red;">*</span></label>
                                            <input type="text" class="form-control" id="signupFirstname" placeholder="First name" required />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label>Last Name <span style="color: red;">*</span></label>
                                            <input type="text" class="form-control" id="signupLastname" placeholder="Last name" required />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label>Username <span style="color: red;">*</span></label>
                                    <input type="text" class="form-control" id="signupUsername" placeholder="Choose username" required />
                                </div>
                                <div class="form-group">
                                    <label>Email <span style="color: red;">*</span></label>
                                    <input type="email" class="form-control" id="signupEmail" placeholder="Email address" required />
                                </div>
                                <div class="form-group">
                                    <label>Mobile <span style="color: red;">*</span></label>
                                    <input type="tel" class="form-control" id="signupMobile" placeholder="Mobile number" required pattern="[0-9]+" title="Please enter only numbers" />
                                    <small class="form-text text-muted"></small>
                                </div>
                                <div class="form-group">
                                    <label>Password <span style="color: red;">*</span></label>
                                    <input type="password" class="form-control" id="signupPassword" placeholder="Choose password" required />
                                </div>
                                <button type="submit" class="btn btn-success btn-block">Sign Up</button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Passenger Details Modal -->
    <div class="modal fade" id="passengerModal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Enter Passenger Details</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <form id="passengerForm">
                        <div id="passengerDetailsContainer"></div>
                        <button type="submit" class="btn btn-primary w-100 mt-3">Confirm Booking</button>
                    </form>
                </div>
            </div>
        </div>
    </div>

    <!-- Alert Modal -->
    <div class="modal fade" id="alertModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Alert Message</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <strong>
                        <p class="error-message text-danger"></p>
                    </strong>
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
        let allRoutes = [];

        async function loadRoutes() {
            try {
                const API_BASE = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';
                const url = API_BASE + "VehicleRoutes/GetVehicleRoutes";

                const response = await fetch(url);
                if (!response.ok) throw new Error("Failed to fetch");

                allRoutes = await response.json();

                const select = document.getElementById("routeSelect");
                select.innerHTML = '<option value="">-- Choose a Route --</option>';

                allRoutes.forEach(route => {
                    let option = document.createElement("option");
                    option.value = route.id;
                    option.textContent = route.name || `${route.startFrom} to ${route.endTo}`;
                    select.appendChild(option);
                });
            } catch (error) {
                console.error("Load Error:", error);
                document.getElementById("routeSelect").innerHTML = '<option value="">Error loading data</option>';
            }
        }

        function displaySelectedRoute() {
            const selectedId = document.getElementById("routeSelect").value;
            const display = document.getElementById("routeDisplay");

            if (!selectedId) {
                display.innerHTML = '<p class="placeholder-text">Select a route to view details.</p>';
                return;
            }

            const route = allRoutes.find(r => r.id == selectedId);
            if (!route) return;

            let steps = [];
            steps.push({ name: route.startFrom, desc: "Starting Point", type: "start" });

            if (route.stoppages) {
                try {
                    const stopList = JSON.parse(route.stoppages);
                    if (Array.isArray(stopList)) {
                        stopList.forEach(s => steps.push({ name: s, desc: "Stoppage", type: "stoppage" }));
                    }
                } catch (e) {
                    const fallbackList = route.stoppages.split(',');
                    fallbackList.forEach(s => steps.push({ name: s.trim(), desc: "Stoppage", type: "stoppage" }));
                }
            }

            steps.push({ name: route.endTo, desc: `Destination (${route.distance || 0} km)`, type: "end" });

            // Create the true diagonal visualization
            display.innerHTML = `
        <div class="route-info-header">
            <h3>${route.name || 'Route Details'}</h3>
            <p><strong>Total Time:</strong> ${route.time || 'N/A'} hrs | <strong>Distance:</strong> ${route.distance || '0'} km</p>
        </div>
        <div class="route-map-container">
            <svg id="routeSvg" class="route-svg"></svg>
            <div id="routePins" class="route-pins"></div>
        </div>
        <div class="route-legend">
            <div class="legend-item"><span class="legend-dot start"></span> Starting Point</div>
            <div class="legend-item"><span class="legend-dot stoppage"></span> Stoppage</div>
            <div class="legend-item"><span class="legend-dot end"></span> Destination</div>
        </div>
    `;

            drawTrueDiagonalRoute(steps);
        }

        function drawTrueDiagonalRoute(steps) {
            const container = document.querySelector('.route-map-container');
            const svg = document.getElementById('routeSvg');
            const pinsContainer = document.getElementById('routePins');

            const containerWidth = container.offsetWidth || 600;
            const containerHeight = 450;

            container.style.height = containerHeight + 'px';

            svg.setAttribute('width', containerWidth);
            svg.setAttribute('height', containerHeight);
            svg.setAttribute('viewBox', `0 0 ${containerWidth} ${containerHeight}`);

            // Calculate positions for true diagonal (bottom-left to top-right)
            const positions = calculateTrueDiagonalPositions(steps.length, containerWidth, containerHeight);

            // Draw smooth curved paths
            drawSmoothCurvedPath(svg, positions);

            // Draw dotted lines between points
            drawDottedConnections(svg, positions);

            // Create pins and labels
            pinsContainer.innerHTML = '';
            steps.forEach((step, index) => {
                const pos = positions[index];

                const pin = document.createElement('div');
                pin.className = `route-pin ${step.type}`;
                pin.style.left = `${pos.x}px`;
                pin.style.top = `${pos.y}px`;

                const labelClass = pos.labelPosition;

                pin.innerHTML = `
            <div class="pin-icon"></div>
            <div class="pin-label ${labelClass}">
                <div class="pin-name">${step.name}</div>
            </div>
        `;

                pinsContainer.appendChild(pin);
            });
        }

        function calculateTrueDiagonalPositions(count, width, height) {
            const positions = [];
            const margin = 70;

            const startX = margin;
            const endX = width - margin;
            const startY = height - margin - 20; // Bottom
            const endY = margin + 20; // Top

            const xStep = (endX - startX) / Math.max(count - 1, 1);
            const yStep = (endY - startY) / Math.max(count - 1, 1);

            // Create zigzag offset
            const zigzagAmount = 40;

            for (let i = 0; i < count; i++) {
                // Base diagonal movement
                const baseX = startX + (i * xStep);
                const baseY = startY + (i * yStep);

                // Add zigzag offset (alternate left-right)
                const zigzagOffset = (i % 2 === 0) ? -zigzagAmount : zigzagAmount;
                const x = baseX + zigzagOffset;
                const y = baseY;

                // Determine label position based on zigzag
                let labelPosition;
                if (i === 0) {
                    labelPosition = 'label-bottom-left';
                } else if (i === count - 1) {
                    labelPosition = 'label-top-right';
                } else {
                    labelPosition = (i % 2 === 0) ? 'label-left' : 'label-right';
                }

                positions.push({ x, y, labelPosition });
            }

            return positions;
        }

        function drawSmoothCurvedPath(svg, positions) {
            for (let i = 0; i < positions.length - 1; i++) {
                const start = positions[i];
                const end = positions[i + 1];

                const midX = (start.x + end.x) / 2;
                const midY = (start.y + end.y) / 2;

                // Control point for smooth curve
                const controlX = midX + (start.x < end.x ? 30 : -30);
                const controlY = midY;

                const pathData = `M ${start.x} ${start.y} Q ${controlX} ${controlY} ${end.x} ${end.y}`;

                const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
                path.setAttribute('d', pathData);
                path.setAttribute('fill', 'none');
                path.setAttribute('stroke', '#4169E1');
                path.setAttribute('stroke-width', '2.5');
                path.setAttribute('opacity', '0.5');
                svg.appendChild(path);
            }
        }

        function drawDottedConnections(svg, positions) {
            for (let i = 0; i < positions.length - 1; i++) {
                const start = positions[i];
                const end = positions[i + 1];

                const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                line.setAttribute('x1', start.x);
                line.setAttribute('y1', start.y);
                line.setAttribute('x2', end.x);
                line.setAttribute('y2', end.y);
                line.setAttribute('stroke', '#FFD700');
                line.setAttribute('stroke-width', '2.5');
                line.setAttribute('stroke-dasharray', '6,4');
                line.setAttribute('stroke-linecap', 'round');
                svg.appendChild(line);
            }
        }

        window.onload = loadRoutes;</script>

    <script>// ============================================================================
        // COMPLETE JAVASCRIPT CODE - Merge kiya hua with all features
        // Add this entire script to your ScriptsContent section in Select_Seat.aspx
        // ============================================================================

        let selectedSeats = [];
        let unitPrice = 0;
        let maxSeatsAllowed = 0; // Will be set by server if in postpone mode
        let isPostponeMode = false;

        $(document).ready(function () {
            console.log('Page loaded');

            // ============================================================================
            // DATE VALIDATION - Disable past dates (including original booking date)
            // ============================================================================
            var today = new Date().toISOString().split('T')[0];
            var minDateAttr = $('input[id*="date_of_journey"]').attr('min');
            var minDate = minDateAttr || today;

            console.log('Minimum date set to:', minDate);

            // Validate date selection - Prevent manual past date entry
            $('input[id*="date_of_journey"]').on('change', function () {
                var selectedDate = new Date($(this).val());
                var minimumDate = new Date(minDate);
                minimumDate.setHours(0, 0, 0, 0);
                selectedDate.setHours(0, 0, 0, 0);

                if (selectedDate < minimumDate) {
                    var minDateFormatted = new Date(minDate).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    });

                    showAlert('Journey date must be from ' + minDateFormatted + ' onwards.');
                    $(this).val('');
                    return false;
                }
            });

            // Prevent keyboard input - force calendar selection only
            $('input[id*="date_of_journey"]').on('keydown', function (e) {
                // Allow: backspace, delete, tab, escape, enter
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
                    (e.keyCode === 65 && e.ctrlKey === true) ||
                    (e.keyCode === 67 && e.ctrlKey === true) ||
                    (e.keyCode === 86 && e.ctrlKey === true) ||
                    (e.keyCode === 88 && e.ctrlKey === true)) {
                    return;
                }
                e.preventDefault();
            });

            // ============================================================================
            // LOAD UNIT PRICE
            // ============================================================================
            const priceField = $('input[id*="hfUnitPrice"]');
            unitPrice = parseFloat(priceField.val()) || 0;
            console.log('Unit Price:', unitPrice);

            // ============================================================================
            // POSTPONE MODE - Check if exact seat count is required
            // ============================================================================
            if (typeof window.maxSeatsAllowed !== 'undefined') {
                maxSeatsAllowed = window.maxSeatsAllowed;
                isPostponeMode = window.isPostponeMode;
                console.log('Postpone Mode Active - Must select exactly:', maxSeatsAllowed, 'seats');
                showPostponeInfo();
            }

            // ============================================================================
            // USER SESSION CHECK
            // ============================================================================
            const hfUserIdValue = $('input[id*="hfUserId"]').val();
            console.log('Hidden field userId on page load:', hfUserIdValue);

            if (!hfUserIdValue || hfUserIdValue === '0') {
                sessionStorage.removeItem('userId');
                sessionStorage.removeItem('userData');
                console.log('User not logged in - sessionStorage cleared');
            }

            // ============================================================================
            // SEAT SELECTION with validation
            // ============================================================================
            $('.seat.btn-seat:not(.booked)').on('click', function (e) {
                e.preventDefault();

                // Check if journey date is selected
                //var journeyDate = $('input[id*="date_of_journey"]').val();
                //if (!journeyDate) {
                //    showAlert('Please select a journey date first.');
                //    return;
                //}

                var journeyDate = $('input[id*="date_of_journey"]').val();
                if (!journeyDate || journeyDate.trim() === '') {
                    showAlert('Please select a journey date first before selecting seats.');
                    return; // Seat select nahi hogi
                }
                const seatLabel = $(this).text().trim();

                if ($(this).hasClass('selected')) {
                    // Deselect seat
                    $(this).removeClass('selected');
                    selectedSeats = selectedSeats.filter(s => s !== seatLabel);
                } else {
                    // Select seat - check limit for postpone mode
                    if (isPostponeMode && selectedSeats.length >= maxSeatsAllowed) {
                        showAlert(`You can only select exactly ${maxSeatsAllowed} seat(s) for postponement. Please deselect a seat first.`);
                        return;
                    }

                    $(this).addClass('selected');
                    if (!selectedSeats.includes(seatLabel)) {
                        selectedSeats.push(seatLabel);
                    }
                }

                updateSummary();
                validateExactSeatCount();
            });

            // ============================================================================
            // FORM HANDLERS
            // ============================================================================
            $('#loginForm').on('submit', function (e) {
                e.preventDefault();
                handleLogin();
            });

            $('#signupForm').on('submit', function (e) {
                e.preventDefault();
                handleSignup();
            });

            // Real-time validation for mobile number - only allow digits
            $('#signupMobile').on('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
        });

        // ============================================================================
        // SHOW POSTPONE INFO BANNER
        // ============================================================================
        function showPostponeInfo() {
            const infoBanner = `
        <div class="alert alert-info postpone-info-banner" style="margin-bottom: 15px; font-weight: bold;">
            <i class="fas fa-info-circle"></i> 
            POSTPONE MODE: You must select exactly ${maxSeatsAllowed} seat(s). No more, no less.
        </div>
    `;

            if ($('.postpone-info-banner').length === 0) {
                $(infoBanner).insertBefore('.seat-plan-inner');
            }
        }

        // ============================================================================
        // VALIDATE EXACT SEAT COUNT (for postpone mode)
        // ============================================================================
        function validateExactSeatCount() {
            const $proceedBtn = $('input[id*="btnProceedToBook"]');

            if (isPostponeMode) {
                if (selectedSeats.length === maxSeatsAllowed) {
                    // Exact match - enable button
                    $proceedBtn.prop('disabled', false)
                        .removeClass('btn-secondary')
                        .addClass('btn-primary');

                    $('.postpone-info-banner').removeClass('alert-info alert-warning')
                        .addClass('alert-success')
                        .html(`<i class="fas fa-check-circle"></i> Perfect! You have selected exactly ${maxSeatsAllowed} seat(s). You can now proceed.`);
                } else {
                    // Not exact - disable button
                    $proceedBtn.prop('disabled', true)
                        .removeClass('btn-primary')
                        .addClass('btn-secondary');

                    const remaining = maxSeatsAllowed - selectedSeats.length;
                    const message = remaining > 0
                        ? `You need to select ${remaining} more seat(s)`
                        : `You have selected ${Math.abs(remaining)} too many seat(s). Please deselect ${Math.abs(remaining)} seat(s)`;

                    $('.postpone-info-banner').removeClass('alert-info alert-success')
                        .addClass('alert-warning')
                        .html(`<i class="fas fa-exclamation-triangle"></i> ${message}. Current: ${selectedSeats.length}/${maxSeatsAllowed}`);
                }
            }
        }

        // ============================================================================
        // UPDATE SEAT SUMMARY
        // ============================================================================
        function updateSummary() {
            console.log('Selected seats:', selectedSeats);

            if (selectedSeats.length === 0) {
                $('#selectedSeatsSummary').hide();
                $('input[id*="hfSelectedSeats"]').val('');
                return;
            }

            $('#selectedSeatsSummary').show();

            // Build seat list HTML
            let html = '';
            selectedSeats.forEach(seat => {
                html += `<div class="seat-item">
            <span>Seat ${seat}</span>
            <span>CDF ${unitPrice.toLocaleString()}</span>
        </div>`;
            });
            $('#selectedSeatsContainer').html(html);

            // Update total
            const total = selectedSeats.length * unitPrice;
            $('#totalAmount').text('CDF ' + total.toLocaleString());

            // Save to hidden field
            $('input[id*="hfSelectedSeats"]').val(selectedSeats.join(','));
        }

        // ============================================================================
        // HANDLE SIGNUP
        // ============================================================================
        function handleSignup() {
            const API_BASE = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';

            const firstname = $('#signupFirstname').val().trim();
            const lastname = $('#signupLastname').val().trim();
            const username = $('#signupUsername').val().trim();
            const email = $('#signupEmail').val().trim();
            const mobile = $('#signupMobile').val().trim();
            const password = $('#signupPassword').val().trim();

            // Basic Validation
            if (!firstname || !lastname || !username || !email || !mobile || !password) {
                alert('Please fill all fields');
                return;
            }

            // Mobile numeric validation
            if (!/^\d+$/.test(mobile)) {
                alert('Mobile number must contain only digits (0-9)');
                $('#signupMobile').focus();
                return;
            }

            // Email validation
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                alert('Please enter a valid email address');
                $('#signupEmail').focus();
                return;
            }

            const $btn = $('#signupForm button[type="submit"]');
            $btn.prop('disabled', true).text('Checking...');

            // Check Existing Users
            $.ajax({
                type: "GET",
                url: API_BASE + "Users/GetUsers",
                contentType: "application/json; charset=utf-8",

                success: function (existingUsers) {
                    console.log('Checking existing users:', existingUsers);

                    // Email duplicate check
                    const emailExists = existingUsers.some(user =>
                        user.email && user.email.toLowerCase() === email.toLowerCase()
                    );

                    if (emailExists) {
                        alert('This email is already registered. Please use a different email.');
                        $('#signupEmail').focus();
                        $btn.prop('disabled', false).text('Sign Up');
                        return;
                    }

                    // Mobile duplicate check
                    const mobileExists = existingUsers.some(user =>
                        user.mobile && user.mobile === mobile
                    );

                    if (mobileExists) {
                        alert('This mobile number is already registered. Please use a different number.');
                        $('#signupMobile').focus();
                        $btn.prop('disabled', false).text('Sign Up');
                        return;
                    }

                    // Username duplicate check
                    const usernameExists = existingUsers.some(user =>
                        user.username && user.username.toLowerCase() === username.toLowerCase()
                    );

                    if (usernameExists) {
                        alert('This username is already taken. Please choose another one.');
                        $('#signupUsername').focus();
                        $btn.prop('disabled', false).text('Sign Up');
                        return;
                    }

                    // Register User
                    $btn.text('Signing up...');

                    $.ajax({
                        type: "POST",
                        url: API_BASE + "Users/PostUser",
                        data: JSON.stringify({
                            firstname, lastname, username, email, mobile, password
                        }),
                        contentType: "application/json; charset=utf-8",

                        success: function (response) {
                            if (response.success) {
                                $('#signupForm')[0].reset();
                                alert("Registration successful! Please login.");

                                // Switch to login tab
                                $('#signupTab').removeClass('active in');
                                $('#loginTab').addClass('active in');
                                $('a[href="#signupTab"]').parent().removeClass('active');
                                $('a[href="#loginTab"]').parent().addClass('active');
                                $('a[href="#loginTab"]').tab('show');

                                setTimeout(() => {
                                    $('#loginUsername').focus();
                                }, 200);
                            } else {
                                alert(response.message || "Registration failed");
                            }
                        },
                        error: function (xhr) {
                            console.error('Registration error:', xhr);
                            alert("Registration failed. Please try again.");
                        },
                        complete: function () {
                            $btn.prop('disabled', false).text("Sign Up");
                        }
                    });
                },
                error: function (xhr) {
                    console.error('Error checking existing users:', xhr);
                    alert("Unable to verify user details. Please try again.");
                    $btn.prop('disabled', false).text("Sign Up");
                }
            });
        }

        // ============================================================================
        // HANDLE LOGIN
        // ============================================================================
        function handleLogin() {
            const API_BASE = '<%= System.Configuration.ConfigurationManager.AppSettings["api_path"] %>';

            const username = $('#loginUsername').val().trim();
            const password = $('#loginPassword').val().trim();

            if (!username || !password) {
                alert('Please enter username and password');
                return;
            }

            const $btn = $('#loginForm button[type="submit"]');
            $btn.prop('disabled', true).text('Logging in...');

            $.ajax({
                type: "POST",
                url: API_BASE + "Users/Login",
                data: JSON.stringify({ username, password }),
                contentType: "application/json; charset=utf-8",

                success: function (response) {
                    const result = response;

                    if (result.success) {
                        console.log('=== LOGIN SUCCESS ===');
                        console.log('User ID:', result.data.id);

                        // Store in sessionStorage
                        sessionStorage.setItem("userId", result.data.id);
                        sessionStorage.setItem("userData", JSON.stringify(result.data));

                        // Store in hidden field BEFORE closing modal
                        $('input[id*="hfUserId"]').val(result.data.id);
                        console.log('Hidden field hfUserId set to:', $('input[id*="hfUserId"]').val());

                        // Close modal first
                        $('#loginModal').modal('hide');

                        // Wait for modal to close, then trigger button
                        setTimeout(function () {
                            console.log('Triggering proceed button after login...');
                            var $button = $('input[id*="btnProceedToBook"]');
                            if ($button.length > 0) {
                                $button.click();
                            } else {
                                console.error('Could not find proceed button');
                            }
                        }, 500);
                    }
                    else {
                        alert(result.message || "Login failed");
                    }
                },
                error: function (xhr) {
                    console.error('Login error:', xhr);
                    alert("Login failed. Please try again.");
                },
                complete: function () {
                    $btn.prop('disabled', false).text("Login");
                }
            });
        }

        // ============================================================================
        // SHOW ALERT MODAL
        // ============================================================================
        function showAlert(message) {
            $('.error-message').text(message);
            $('#alertModal').modal('show');
        }</script>
</asp:Content>
