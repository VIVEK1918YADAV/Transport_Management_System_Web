<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Train.aspx.cs" Inherits="Excel_Bus.Train" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head>
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelTrain - Home</title>
    
    <!-- BootStrap Link -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- Icon Link -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" rel="stylesheet">
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/global-line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />

    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />
  
   
    <!-- Custom Link -->
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />
    <link href="css/select2.min.css" rel="stylesheet" />
    <link href="css/daterangepicker.css" rel="stylesheet" />
    <link href="css/slick.css" rel="stylesheet" />

    <link rel="stylesheet" href="https://maxst.icons8.com/vue-static/landings/line-awesome/line-awesome/1.3.0/css/line-awesome.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <style>
        * {
    box-sizing: border-box;
}

img {
    max-width: 100%;
    height: auto;
}
        body {
    padding: 0;
    margin: 0;
    font-size: 17px;
   color: #0c5840;
    line-height: 28px;
    overflow-x: hidden !important;
    font-family: "Lato", sans-serif;
       background: #eaf7df;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
}
        .working-process-item {
    background-color: #2d9b8b8a;
}
        .working-process-item .thumb {
    box-shadow: 0 0 0 8px #f1f5f3;
    background: #47a17b;
}
        .banner-content .cmn--btn:hover {
    background-color: #2d9b8b8a !important;
    color: #043e11;
}

        .working-process-item .thumb {
    width: 80px;
    height: 80px;
    -webkit-box-shadow: 0 0 0 8px rgba(14, 158, 77, 0.1);
    box-shadow: 0 0 0 8px rgba(14, 158, 77, 0.1);
    margin: 0 auto 20px;
    border-radius: 50%;
    font-size: 32px;
    color: #f3f6f9;
    background: #47a17b;
}
        .working-process-item .thumb-wrapper span {
    position: absolute;
    position: absolute;
    content: "";
    width: 100%;
    height: 100%;
    width: 35px;
    height: 35px;
    right: -15px;
    top: -15px;
    border-radius: 50%;
    background: #47a17b;
    font-size: 14px;
    color: #194403;
}
         h4 {
    font-weight: 600;
    margin: 0;
    line-height: 1.2;
    color: #f5f5f9;
    font-family: "Georama", sans-serif;
}
        /* Google translate */
        div#google_translate_element {
            position: absolute;
            top: 0;
            right: 0;
        }
        div#google_translate_element .goog-te-gadget .goog-te-combo {
            margin: 2px 0 !important;
            height: 35px !important;
            border: 0;
            background-color:#2d9b8b8a;
            color: #fff;
            border-radius: 5px;
        }
        div#google_translate_element .goog-te-gadget .goog-te-combo:focus{
            outline:none;
            border: 0;
        }

        .header-top {
            padding: 8px 0;
            font-size: 14px;
            border-bottom: 0;
            background: #47a17b;
        }
        .header-top-area .left-content li i{
            color:#fff;
            font-size:18px;
        }
        .header-top-area .left-content li a {
            color: #fff;
            font-size: 16px;
        }
        .header-top-area .right-content .header-login li .sign-in {
            color: #000000;
        }
        .header-top-area .header-login li a{
           color: #f3ebeb;
        }

        .language-wrapper {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            width: max-content;
            margin-left: 12px;
            padding: 0;
            background-color: transparent;
            border: 0;
        }

        .language_flag {
            flex-shrink: 0;
            display: flex;
        }

        .language_flag img {
            height: 20px;
            width: 20px;
            object-fit: cover;
            border-radius: 50%;
        }

        .language-wrapper.show .collapse-icon {
            transform: rotate(180deg)
        }

        .collapse-icon {
            font-size: 14px;
            display: flex;
            transition: all linear 0.2s;
            color: #111
        }

        .language_text_select {
            font-size: 14px;
            font-weight: 400;
            color: #111;
        }

        .language-content {
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .language_text {
            color: #111
        }

        .language-list {
            display: flex;
            align-items: center;
            gap: 6px;
            padding: 6px 12px;
            cursor: pointer;
        }

        .language-list:hover {
            background-color: #2d9b8b8a;
        }

        .language .dropdown-menu {
            position: absolute;
            opacity: 0;
            visibility: hidden;
            top: 100%;
            display: unset;
            background: #ffffffea;
            box-shadow: 0px 0px 4px 0px rgba(0, 0, 0, 0.04), 0px 8px 16px 0px rgba(0, 0, 0, 0.08);
            min-width: 150px;
            padding: 7px 0 !important;
            border-radius: 8px;
            border: 1px solid rgb(255 255 255 / 10%);
        }

        .language .dropdown-menu.show {
            visibility: visible;
            opacity: 1;
        }

        /* Perfect Train Animation - RIGHT TO LEFT */
        .banner-section {
            position: relative;
            height: 700px;
            overflow: hidden;
            background:#7cbba0 !important;
        }

        .train-tracks {
            position: absolute;
            bottom: 80px;
            left: 0;
            width: 100%;
            height: 20px;
            background: #47a17b;
            border-top: 4px solid #696969;
            border-bottom: 4px solid #696969;
        }

        .train-tracks::before {
            content: '';
            position: absolute;
            top: 50%;
            left: 0;
            width: 100%;
            height: 2px;
            background: #a0bcbf;
            transform: translateY(-50%);
        }
        .attendance_column {
    width: 100%;
    height: auto;
    padding: 70px 0;
    background: linear-gradient(155deg, #21af8e, #cbcb85);
}
        .train-container {
            position: absolute;
            bottom: 105px;
            right: -500px;
            animation: trainMove 18s linear infinite;
            /* Flip the train horizontally so it faces left */
            transform: scaleX(-1);
        }

        .train {
            display: flex;
            align-items: flex-end;
        }

        /* Train Engine - REDUCED SIZE */
        .engine {
            width: 90px;
            height: 70px;
            background: #5d6c72ed;
            border-radius: 10px 10px 5px 5px;
            position: relative;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            margin-right: -5px;
        }

        .engine-top {
            position: absolute;
            top: -25px;
            left: 12px;
            width: 45px;
            height: 32px;
            background:#d57208;
            border-radius: 6px 6px 0 0;
        }

        .chimney {
            position: absolute;
            top: -40px;
            left: 23px;
            width: 12px;
            height: 18px;
            background: #1F2937;
            border-radius: 3px 3px 0 0;
        }

        .smoke {
            position: absolute;
            top: -55px;
            left: 20px;
            width: 18px;
            height: 18px;
            background: #47a17b;
            border-radius: 50%;
            animation: smoke 2s infinite;
        }

        @keyframes smoke {
            0% {
                opacity: 0.8;
                transform: translateY(0) scale(1);
            }
            100% {
                opacity: 0;
                transform: translateY(-50px) scale(1.5);
            }
        }

        .window {
            position: absolute;
            width: 22px;
            height: 20px;
            background: #b77b0a;
    border: 2px solid #8a2c1e;
            border-radius: 5px;
            top: 20px;
        }

        .window-1 { left: 15px; }
        .window-2 { left: 45px; }
        .window-3 { left: 75px; }

        /* Train Coaches - REDUCED SIZE */
        .coach {
            width: 75px;
            height: 65px;
            background: #99bcc7;
            border-radius: 6px 6px 5px 5px;
            position: relative;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            margin-right: -5px;
        }

        .coach .window {
            width: 20px;
            height: 18px;
            top: 16px;
        }

        .coach .window-1 { left: 8px; }
        .coach .window-2 { left: 30px; }
        .coach .window-3 { left: 52px; }

        .door {
            position: absolute;
            width: 18px;
            height: 38px;
            background: #933c08;
            border: 2px solid #1F2937;
            border-radius: 3px;
            bottom: 6px;
            left: 50%;
            transform: translateX(-50%);
        }

        /* Wheels */
        .wheels {
            position: absolute;
            bottom: -10px;
            left: 0;
            width: 100%;
            display: flex;
            justify-content: space-around;
            padding: 0 8px;
        }

        .wheel {
            width: 18px;
            height: 18px;
            background: #031e12;
            border-radius: 50%;
            border: 2px solid #374151;
            position: relative;
            animation: wheelRotate 0.5s linear infinite;
        }

        .wheel::after {
            content: '';
            position: absolute;
            top: 50%;
            left: 50%;
            width: 5px;
            height: 5px;
            background: #47a17b;
            border-radius: 50%;
            transform: translate(-50%, -50%);
        }

        @keyframes wheelRotate {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }

        /* UPDATED ANIMATION - RIGHT TO LEFT */
        @keyframes trainMove {
            0% {
                right: -500px;
            }
            100% {
                right: 100%;
            }
        }

        /* Mountain Background */
       /* .mountains {
            position: absolute;
            bottom: 120px;
            left: 0;
            width: 100%;
            height: 200px;
            z-index: 1;
        }

        .mountain {
            position: absolute;
            width: 0;
            height: 0;
            border-left: 150px solid transparent;
            border-right: 150px solid transparent;
            border-bottom: 200px solid #A0AEC0;
        }

        .mountain-1 {
            left: 5%;
            bottom: 0;
            opacity: 0.7;
        }

        .mountain-2 {
            left: 25%;
            bottom: 0;
            border-left-width: 120px;
            border-right-width: 120px;
            border-bottom-width: 160px;
            opacity: 0.6;
        }

        .mountain-3 {
            right: 20%;
            bottom: 0;
            opacity: 0.7;
        }*/

        .banner-wrapper {
            position: relative;
            z-index: 10;
        }

        .banner-content {
            position: relative;
            z-index: 15;
            padding-top: 50px;
        }

        .banner-content .title {
            margin-bottom: 8px;
            max-width: 600px;
        }
        .cmn--btn {
    text-transform: uppercase;
    font-size: 14px;
    padding: 10px 25px;
    border-color: rgba(14, 158, 77, 0.6);
    background: #47a17b;
    position: relative;
    border-radius: 34px;
    overflow: hidden;
    font-weight: 600;
}
        .banner-content .cmn--btn {
            display: inline-block;
            margin-top: 15px;
            margin-bottom: 120px;
        }
        h1 {
    font-weight: 600;
    margin: 12px;
    margin-top: -45px;
    line-height: 1.2;
    color: #f6f6f7;
    font-family: "Georama", sans-serif;
}
        h2 {
    font-weight: 700;
    margin: 8px;
    margin-top: -45px;
    line-height: 1.2;
    color: #0b5e50;
    font-family: "Georama", sans-serif;
}

     

        h1 {
    font-size: 48px;
}

h2 {
    font-size: 32px;
}

@media (max-width: 992px) {
    h1 {
        font-size: 36px;
    }
    h2 {
        font-size: 28px;
    }
}

@media (max-width: 576px) {
    h1 {
        font-size: 26px;
    }
    h2 {
        font-size: 22px;
    }
}
        .banner-section::before, .banner-section::after {
    background: #47a17b;
}
        .footer-top {
    background: #47a17b;
    padding: 80px 0;
}
        .logo img {
    max-width: 81px;
    max-height: 80px;
}
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <!-- Header Section -->
        <div class="header-top">
            <div class="container">
                <div class="header-top-area">
                    <ul class="left-content">
                        <li>
                            <i class="las la-phone"></i>
                            <a href="tel:+91 98717 71849">+91 98717 71849</a>
                        </li>
                        <li>
                            <i class="las la-envelope-open"></i>
                            <a href="mailto:rajesh.paul@excelgeomatics.com">rajesh.paul@excelgeomatics.com</a>
                        </li>
                    </ul>

                    <ul class="header-login">
                        <li><a class="sign-in" href="TrainUserReg.aspx"><i class="fas fa-sign-in-alt"></i> Sign In</a></li>
                    </ul>
          
                    <!-- Google Translate -->
                    <ul>
                        <div id="google_translate_element"></div>
                        <script type="text/javascript">
                            function googleTranslateElementInit() {
                                new google.translate.TranslateElement({
                                    pageLanguage: 'en',
                                    layout: google.translate.TranslateElement.InlineLayout.Vertical,
                                    includedLanguages: 'en,fr,hi,de,es'
                                }, 'google_translate_element');
                            }
                        </script>
                        <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
                    </ul>
                </div>
            </div>
        </div>
        
        <div class="header-bottom">
            <div class="container">
                <div class="header-bottom-area">
                   
                    <div class="logo">
                        <a href="Train.aspx">
                            <img src="img/train_icon.png" alt="Logo">
                        </a>
                    </div>&nbsp&nbsp
                    <ul class="menu">
                        <li><a href="Train.aspx">Home</a></li>
                        <li><a href="About.aspx">About</a></li>
                        <li><a href="Contact.aspx">Contact</a></li>
                    </ul>
                    <div class="d-flex flex-wrap algin-items-center">
                        <a href="TrainTicket.aspx" class="cmn--btn btn--sm">Buy Train Tickets</a>
                        <div class="header-trigger-wrapper d-flex d-lg-none ms-4">
                            <div class="header-trigger d-block d-lg-none">
                                <span></span>
                            </div>
                            <div class="top-bar-trigger">
                                <i class="las la-ellipsis-v"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Banner Section -->
        <section class="banner-section">
            <div class="container">
                <div class="banner-wrapper">
                    <div class="banner-content">
                        <h1 class="title">Get Your Train Ticket Online, Easy and Safely</h1>
                        <a href="TrainTicket.aspx" class="cmn--btn">Get ticket now</a>
                    </div>
                </div>
            </div>

            <!-- Mountains Background -->
            <div class="mountains">
                <div class="mountain mountain-1"></div>
                <div class="mountain mountain-2"></div>
                <div class="mountain mountain-3"></div>
            </div>

            <!-- Train Tracks -->
            <div class="train-tracks"></div>

            <!-- Animated Train Container -->
            <div class="train-container">
                <div class="train">
                    <!-- Coach 5 (NEW) -->
                    <div class="coach">
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="window window-3"></div>
                        <div class="door"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>

                    <!-- Coach 4 (NEW) -->
                    <div class="coach">
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="window window-3"></div>
                        <div class="door"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>

                    <!-- Coach 3 -->
                    <div class="coach">
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="window window-3"></div>
                        <div class="door"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>

                    <!-- Coach 2 -->
                    <div class="coach">
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="window window-3"></div>
                        <div class="door"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>

                    <!-- Coach 1 -->
                    <div class="coach">
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="window window-3"></div>
                        <div class="door"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>

                    <!-- Engine -->
                    <div class="engine">
                        <div class="chimney"></div>
                        <div class="smoke"></div>
                        <div class="engine-top"></div>
                        <div class="window window-1"></div>
                        <div class="window window-2"></div>
                        <div class="wheels">
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                            <div class="wheel"></div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Working Process Section -->
        <section class="working-process padding-top padding-bottom">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-6 col-md-10">
                        <div class="section-header text-center">
                            <h2 class="title">Get Your Train Tickets With Just 3 Steps</h2>
                            <p>Have a look at our popular reasons. Why you should choose our train service. Just book a train and get a ticket for your great journey!</p>
                        </div>
                    </div>
                </div>
                <div class="row g-4 gy-md-5 justify-content-center">
                    <div class="col-lg-4 col-md-6 col-sm-10">
                        <div class="working-process-item">
                            <div class="thumb-wrapper">
                                <span>01</span>
                                <div class="thumb">
                                    <i class="las la-search"></i>
                                </div>
                            </div>
                            <div class="content">
                                <h4 class="title">Search Your Train</h4>
                                <p>Choose your origin, destination, journey dates and search for trains</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 col-sm-10">
                        <div class="working-process-item">
                            <div class="thumb-wrapper">
                                <span>02</span>
                                <div class="thumb">
                                    <i class="las la-ticket-alt"></i>
                                </div>
                            </div>
                            <div class="content">
                                <h4 class="title">Choose The Ticket</h4>
                                <p>Select your preferred class and seat for your journey</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 col-sm-10">
                        <div class="working-process-item">
                            <div class="thumb-wrapper">
                                <span>03</span>
                                <div class="thumb">
                                    <i class="las la-money-bill-wave-alt"></i>
                                </div>
                            </div>
                            <div class="content">
                                <h4 class="title">Pay Bill</h4>
                                <p>Complete your booking with secure payment options</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Android App Section -->
        <section class="android_app">
            <div class="container">
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
                        <div class="aap_images">
                            <img src="img/App_ui1.jpg" alt="mobile screen ui" />
                            <img src="img/App_ui2.jpg" alt="mobile screen ui" class="third_image" />
                            <img src="img/App_ui3.jpg" alt="mobile screen ui" />
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
                        <div class="aap_contents">
                            <h1>Manage Passengers, Luggage & Train Status</h1>
                            <p>Our Smart Train Android System is designed for modern railway management. It allows conductors or train staff to take real-time attendance of passengers and record luggage information directly through an Android device.</p>
                            <ul>
                                <li class="itemList">✔ Passenger Attendance – Every traveler is marked present using a simple check-in interface.</li>
                                <li>✔ Luggage Tracking – Each passenger's luggage details are logged to avoid any confusion or loss.</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Attendance Section -->
        <section class="attendance_column">
            <div class="container">
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
                        <div class="ticket_contents">
                            <h1>Passengers <span>Attendance</span></h1>
                            <p>Our train ticket attendance system ensures all support queries are tracked, managed, and resolved efficiently. Each ticket is recorded, assigned, and handled based on urgency and category. This process helps maintain a smooth workflow, reduces delays, and improves customer satisfaction.</p>
                            <p>Every incoming ticket is recorded, categorized, and assigned to the right team member based on priority. This organized process helps in reducing response time, ensuring nothing is missed. By tracking progress from start to finish, we maintain transparency and accountability.</p>
                            <a href="Contact.aspx" class="conttBtn">Contact Now</a>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
                        <div class="ticket_img">
                            <img src="img/ticket-3.jpg" alt="picture" />
                            <img src="img/train_ticket-2.jpg" class="ticketPic2" alt="picture" />
                            <img src="img/train_ticket-3.jpg" class="ticketPic3" alt="picture" />
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Amenities Section -->
        <section class="amenities-section padding-bottom">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-6 col-md-10">
                        <div class="section-header text-center">
                            <h2 class="title">Our Amenities</h2>
                            <p>Have a look at our popular amenities. Why you should choose our train service. Just choose a train and get a ticket for your great journey!</p>
                        </div>
                    </div>
                </div>
                <div class="amenities-wrapper">
                    <div class="amenities-slider">
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="fas fa-wifi"></i>
                                </div>
                                <h6 class="title">Wifi</h6>
                            </div>
                        </div>
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="las la-bed"></i>
                                </div>
                                <h6 class="title">Sleeper</h6>
                            </div>
                        </div>
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="las la-prescription-bottle"></i>
                                </div>
                                <h6 class="title">Water Bottle</h6>
                            </div>
                        </div>
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="fas fa-utensils"></i>
                                </div>
                                <h6 class="title">Food Service</h6>
                            </div>
                        </div>
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="fas fa-snowflake"></i>
                                </div>
                                <h6 class="title">AC Coaches</h6>
                            </div>
                        </div>
                        <div class="single-slider">
                            <div class="amenities-item">
                                <div class="thumb">
                                    <i class="fas fa-plug"></i>
                                </div>
                                <h6 class="title">Charging Point</h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Footer Section -->
        <section class="footer-section">
            <div class="footer-top">
                <div class="container">
                    <div class="row footer-wrapper gy-sm-5 gy-4">
                        <div class="col-xl-4 col-lg-3 col-md-6 col-sm-6">
                            <div class="footer-widget">
                                <div class="logo">
                                    <img src="img/train_icon.png" alt="Logo">
                                </div>
                                <p>Fast, reliable, and comfortable train services connecting you to your destination safely.</p>
                                <ul class="social-icons">
                                    <li><a href="https://www.facebook.com/"><i class="lab la-facebook-f"></i></a></li>
                                    <li><a href="https://twitter.com/?lang=en"><i class="lab la-twitter"></i></a></li>
                                    <li><a href="https://vimeo.com/log_in"><i class="lab la-vimeo"></i></a></li>
                                    <li><a href="https://www.instagram.com/?hl=en"><i class="lab la-instagram"></i></a></li>
                                </ul>
                            </div>
                        </div>
                        <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6">
                            <div class="footer-widget">
                                <h4 class="widget-title">Useful Links</h4>
                                <ul class="footer-links">
                                    <li><a href="~/About.aspx">About</a></li>
                                    <li><a href="">FAQs</a></li>
                                    <li><a href="">Blog</a></li>
                                    <li><a href="Contact.aspx">Contact</a></li>
                                </ul>
                            </div>
                        </div>
                        <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6">
                            <div class="footer-widget">
                                <h4 class="widget-title">Policies</h4>
                                <ul class="footer-links">
                                    <li><a href="">Privacy Policy</a></li>
                                    <li><a href="">Terms of Service</a></li>
                                    <li><a href="">Ticket Policies</a></li>
                                    <li><a href="">Refund Policy</a></li>
                                </ul>
                            </div>
                        </div>
                        <div class="col-xl-3 col-lg-3 col-md-4 col-sm-6">
                            <div class="footer-widget">
                                <h4 class="widget-title">Contact Info</h4>
                                <ul class="footer-contacts">
                                    <li><i class="las la-map-pin"></i> NOIDA UP</li>
                                    <li><i class="las la-phone-volume"></i> <a href="tel:+91 98717 71849">+91 98717 71849</a></li>
                                    <li><i class="las la-envelope"></i> <a href="mailto:rajesh.paul@excelgeomatics.com">rajesh.paul@excelgeomatics.com</a></li>
                                </ul>
                                <div class="customBtn">
                                    <a href="Train.aspx" class="intraCityBtn">Train Service</a>
                                    <a href="TrainAdminLogin.aspx" target="_blank" class="interCityBtn">Admin Panel</a>
                                </div>
                            </div>
                        </div>
                    </div>
                                
                </div>
            </div>
        </section>

        <a href="javascript::void()" class="scrollToTop active"><i class="las la-chevron-up"></i></a>
    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>
    <script src="js/iziToast.min.js"></script>
    <script src="js/daterangepicker.min.js"></script>
    <script src="js/select2.min.js"></script>
    <script src="js/moment.min.js"></script>
    <script src="js/slick.min.js"></script>

    <script>
        "use strict";
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
                transitionIn: 'obunceInLeft'
            });
        }

        function notify(status, message) {
            if (typeof message == 'string') {
                triggerToaster(status, message);
            } else {
                $.each(message, (i, val) => triggerToaster(status, val));
            }
        }
    </script>

    <script>
        (function ($) {
            "use strict"
            $('.select2').select2();
            const datePicker = $('.date-range').daterangepicker({
                autoUpdateInput: true,
                singleDatePicker: true,
                minDate: new Date()
            })
        })(jQuery)
    </script>

    <script>
        $(function () {
            'use strict'
            $(".amenities-slider").slick({
                fade: false,
                slidesToShow: 6,
                slidesToScroll: 1,
                infinite: true,
                autoplay: true,
                pauseOnHover: true,
                centerMode: false,
                dots: false,
                arrows: false,
                nextArrow: '<i class="las la-arrow-right arrow-right"></i>',
                prevArrow: '<i class="las la-arrow-left arrow-left"></i> ',
                responsive: [{
                    breakpoint: 1199,
                    settings: {
                        slidesToShow: 5,
                        slidesToScroll: 1,
                    },
                },
                {
                    breakpoint: 991,
                    settings: {
                        slidesToShow: 4,
                        slidesToScroll: 1,
                    },
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 1,
                    },
                },
                {
                    breakpoint: 500,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 1,
                    },
                },
                ],
            });
        });
    </script>
</body>

</html>
