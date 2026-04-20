<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Excel_Bus.index" %>

<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>Excel Transport - Choose Your Journey</title>
    
    <!-- BootStrap Link -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    
    <!-- Icon Link -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" rel="stylesheet">
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(169deg, #15a79d 0%, #0d9b1980 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 15px;
        }

        .container-fluid {
            max-width: 1600px;
            width: 100%;
        }

        .main-header {
            text-align: center;
            margin-bottom: 35px;
            animation: fadeInDown 0.8s ease;
        }

        .main-header h1 {
            color: #fff;
            font-size: 2.8rem;
            font-weight: 700;
            margin-bottom: 8px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
        }

        .main-header p {
            color: rgba(255,255,255,0.9);
            font-size: 1.1rem;
            font-weight: 300;
        }

        .transport-selection {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 30px;
            margin-top: 25px;
        }

        .transport-card {
            background: #efffeb;
            border-radius: 20px;
            padding: 35px 25px;
            text-align: center;
            cursor: pointer;
            transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
            box-shadow: 0 20px 60px rgba(0,0,0,0.15);
            position: relative;
            overflow: hidden;
            animation: fadeInUp 0.8s ease;
        }

        .transport-card:nth-child(1) {
            animation-delay: 0.1s;
        }

        .transport-card:nth-child(2) {
            animation-delay: 0.2s;
        }

        .transport-card:nth-child(3) {
            animation-delay: 0.3s;
        }

        .transport-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
            transition: left 0.5s;
        }

        .transport-card:hover::before {
            left: 100%;
        }

        .transport-card:hover {
            transform: translateY(-10px) scale(1.02);
            box-shadow: 0 30px 80px rgba(0,0,0,0.25);
        }

        .transport-card.bus-card:hover {
            border-top: 5px solid #0c6cb7;
        }

        .transport-card.train-card:hover {
            border-top: 5px solid #e74c3c;
        }

        .transport-card.boat-card:hover {
            border-top: 5px solid #3498db;
        }

        .icon-wrapper {
            width: 100px;
            height: 100px;
            margin: 0 auto 20px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.4s ease;
        }

        .bus-card .icon-wrapper {
            background: linear-gradient(135deg, #0c6cb7 0%, #0a912e 100%);
        }

        .train-card .icon-wrapper {
            background: linear-gradient(135deg, #168967 0%, #c08d2bd9 100%);
        }

        .boat-card .icon-wrapper {
            background: linear-gradient(135deg, #3498db 0%, #29b985 100%);
        }

        .transport-card:hover .icon-wrapper {
            transform: scale(1.1) rotate(5deg);
        }

        .icon-wrapper i {
            font-size: 50px;
            color: #fff;
        }

        .transport-card h2 {
            font-size: 1.7rem;
            font-weight: 700;
            margin-bottom: 12px;
            color: #2c3e50;
        }

        .transport-card p {
            color: #000f10;
            font-size: 0.9rem;
            line-height: 1.5;
            margin-bottom: 18px;
        }

        .select-btn {
            display: inline-block;
            padding: 12px 35px;
            background: linear-gradient(348deg, #074a33 0%, #4db1b1 100%);
            color: #fff;
            text-decoration: none;
            border-radius: 50px;
            font-weight: 600;
            font-size: 0.95rem;
            transition: all 0.3s ease;
            border: none;
            cursor: pointer;
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .select-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(102, 126, 234, 0.6);
            color: #fff;
            text-decoration: none;
        }

        .select-btn:active {
            transform: translateY(0);
        }

        .features-list {
            list-style: none;
            padding: 0;
            margin: 15px 0;
            text-align: left;
        }

        .features-list li {
            padding: 6px 0;
            color: #020001;
            font-size: 0.88rem;
        }

        .features-list li i {
            color: #28a745;
            margin-right: 8px;
            font-size: 0.95rem;
        }

        @keyframes fadeInDown {
            from {
                opacity: 0;
                transform: translateY(-30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .status-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            background: #28a745;
            color: #fff;
            padding: 4px 13px;
            border-radius: 20px;
            font-size: 0.78rem;
            font-weight: 600;
        }

        .coming-soon-badge {
            background: #ffc107;
            color: #000;
        }

        .in-progress-badge {
            background: #ff9800;
            color: #fff;
        }

        /* Responsive for tablets and below */
        @media (max-width: 1200px) {
            .transport-selection {
                gap: 25px;
            }
            
            .transport-card {
                padding: 30px 20px;
            }
            
            .icon-wrapper {
                width: 90px;
                height: 90px;
            }
            
            .icon-wrapper i {
                font-size: 45px;
            }

            .main-header h1 {
                font-size: 2.5rem;
            }
        }

        @media (max-width: 992px) {
            .transport-selection {
                grid-template-columns: 1fr;
                gap: 30px;
            }
            
            .main-header h1 {
                font-size: 2.2rem;
            }
            
            .main-header p {
                font-size: 1rem;
            }
        }

        @media (max-width: 768px) {
            .main-header h1 {
                font-size: 2rem;
            }

            .transport-card {
                padding: 35px 25px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="main-header">
                <h1>Welcome to Excel Transport</h1>
                <p>Choose your preferred mode of transportation</p>
            </div>

            <div class="transport-selection">
                <!-- Bus Card -->
                <div class="transport-card bus-card">
                    <span class="status-badge">Available</span>
                    <div class="icon-wrapper">
                        <i class="fas fa-bus"></i>
                    </div>
                    <h2>Bus</h2>
                    <p>Comfortable and affordable bus journeys across the country</p>
                    <ul class="features-list">
                        <li><i class="fas fa-check-circle"></i> AC & Non-AC Options</li>
                        <li><i class="fas fa-check-circle"></i> WiFi Available</li>
                        <li><i class="fas fa-check-circle"></i> Online Booking</li>
                        <li><i class="fas fa-check-circle"></i> Real-time Tracking</li>
                    </ul>
                    <asp:Button ID="btnBus" runat="server" Text="Book Bus Ticket" CssClass="select-btn" OnClick="btnBus_Click" />
                </div>

                <!-- Train Card -->
                <div class="transport-card train-card">
                    <span class="status-badge in-progress-badge">In Progress</span>
                    <div class="icon-wrapper">
                        <i class="fas fa-train"></i>
                    </div>
                    <h2>Train</h2>
                    <p>Fast and reliable train services for long distance travel</p>
                    <ul class="features-list">
                        <li><i class="fas fa-check-circle"></i> Multiple Classes</li>
                        <li><i class="fas fa-check-circle"></i> Sleeper & Seating</li>
                       <%-- <li><i class="fas fa-check-circle"></i> Food Service</li>--%>
                        <li><i class="fas fa-check-circle"></i> Wide Network</li>
                    </ul>
                    <asp:Button ID="btnTrain" runat="server" Text="Book Train Ticket" CssClass="select-btn" OnClick="btnTrain_Click" />
                </div>

                <!-- Boat Card -->
                <div class="transport-card boat-card">
                    <span class="status-badge coming-soon-badge">Coming Soon</span>
                    <div class="icon-wrapper">
                        <i class="fas fa-ship"></i>
                    </div>
                    <h2>Boat</h2>
                    <p>Scenic water transport for coastal and river routes</p>
                    <ul class="features-list">
                        <li><i class="fas fa-check-circle"></i> Scenic Routes</li>
                        <li><i class="fas fa-check-circle"></i> Premium Amenities</li>
                        <li><i class="fas fa-check-circle"></i> Safe Journey</li>
                        <li><i class="fas fa-check-circle"></i> Eco-Friendly</li>
                    </ul>
                    <asp:Button ID="btnBoat" runat="server" Text="Book Boat Ticket" CssClass="select-btn" OnClick="btnBoat_Click" />
                </div>
            </div>
        </div>
    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
</body>
</html>
