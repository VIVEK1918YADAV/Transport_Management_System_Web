<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Excel_Bus.Home" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head>
    <!-- Required meta tags -->
  <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelBus - Home</title>
    
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
            background-color:#252525;
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
            background: linear-gradient(45deg, #0c6cb7 50%, yellow 50%);
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
            color: #000;
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
            background-color: rgba(0, 0, 0, 0.04);
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
    </style>
</head>

<body>
    <form id="form1" runat="server">

       <%-- <div class="overlay"></div>--%>
        
        <!-- Preloader -->
       

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
                             
                            <li><a class="sign-in"  href="UserLogin.aspx"><i class="fas fa-sign-in-alt" ></i>
Sign In</a></li>
                          <%--  <li>/</li>--%>
                           <%-- <li><a class="sign-up" href=""><i class="fas fa-user-plus"></i>Sign Up</a></li>--%>
                        </ul>
                          
   


                      <!-- Google Translate -->
              <ul> <div id="google_translate_element"></div>

        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en',
                    layout: google.translate.TranslateElement.InlineLayout.Vertical,
                    includedLanguages: 'en,fr,hi,de,es'
                }, 'google_translate_element');
            }
        </script>

     <script type="text/javascript" src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script></ul>
                     

                    </div>
                </div>
            </div>
        
        <div class="header-bottom">
            <div class="container">
                <div class="header-bottom-area">
                   
                    <div class="logo">
                         <a href="Home.aspx">
                            <img src="img/excel_bus_logo.png" alt="Logo">
                        </a>
                    </div>&nbsp&nbsp
                    <ul class="menu">
                        
                        <li> <a href="Home.aspx">Home</a></li>
                        <li><a href="About.aspx">About</a></li>
                        <%--<li>
                            <a href="javascript::void()">Trips</a>
                            <ul class="sub-menu">
                              </ul>--%>
                        <li> <a href="Contact.aspx">Contact</a></li>
                    </ul>
                    <div class="d-flex flex-wrap algin-items-center">
                        <a href="ticket.aspx" class="cmn--btn btn--sm">Buy Tickets</a>
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
         <section  class="banner-section" style="background: url('img/bus_running_bg.png')repeat-x bottom;">
            
    
            <div class="container">
                <div class="banner-wrapper">
                    <div class="banner-content">
                        <h1 class="title">Get Your Ticket Online, Easy and Safely</h1>
                        <a  href="ticket.aspx" class="cmn--btn">Get ticket now</a>
                    </div>
                </div>
            </div>
            <div class="shape">
                <img src="img/Running_Bus.png" alt="bg">
            </div>
        </section>

        <!-- Working Process Section -->
        <section class="working-process padding-top padding-bottom">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-6 col-md-10">
                        <div class="section-header text-center">
                            <h2 class="title">Get Your Tickets With Just 3 Steps</h2>
                            <p>Have a look at our popular reason. why you should choose you bus. Just a Bus and get a ticket for your great journey. !</p>
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
                                <h4 class="title">Search Your Bus</h4>
                                <p>Choose your origin, destination, Just choose a Bus journey dates and search for buses</p>
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
                                <p>Choose your origin, destination, Just a Bus for your great journey dates and search for buses</p>
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
                                <p>Choose your origin, destination, choose a Bus for your great journey dates and search for buses</p>
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
                            <h1>Manage Passengers, Luggage & Missed Departures</h1>
                            <p>Our Smart Bus Android System is designed for modern fleet management. It allows conductors or bus staff to take real-time attendance of passengers and record luggage information directly through an Android device.</p>
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
                            <p>Our ticket attendance system ensures all support queries are tracked, managed, and resolved efficiently. Each ticket is recorded, assigned, and handled based on urgency and category. This process helps maintain a smooth workflow, reduces delays, and improves customer satisfaction. By monitoring every step from ticket creation to resolution, we ensure accountability and timely service for every request, contributing to better communication and issue management.</p>
                            <p>Every incoming ticket is recorded, categorized, and assigned to the right team member based on priority. This organized process helps in reducing response time, ensuring nothing is missed. By tracking progress from start to finish, we maintain transparency and accountability. The goal is to deliver fast, reliable solutions and enhance customer satisfaction through smooth and structured ticket handling.</p>
                            <a href="Contact.aspx" class="conttBtn">Contact Now</a>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
                        <div class="ticket_img">
                            <img src="img/ticket-1.jpg" alt="picture" />
                            <img src="img/ticket-2.jpg" class="ticketPic2" alt="picture" />
                            <img src="img/ticket-3.jpg" class="ticketPic3" alt="picture" />
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
                            <p>Have a look at our popular reason. why you should choose you bus. Just choose a Bus and get a ticket for your great journey!</p>
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
                                <h6 class="title">Pillow</h6>
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
                                    <i class="fas fa-wine-glass-alt"></i>
                                </div>
                                <h6 class="title">Soft Drinks</h6>
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
                                    <img src="img/excel_bus_logo.png" alt="Logo">
                                </div>
                                <p>Delectus culpa laboriosam debitis saepe. Commodi earum minus ut obcaecati veniam deserunt est!</p>
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
                                    <a href="http://180.151.15.18:9008/InterCityBus/index" target="_blank" class="intraCityBtn">Intra City</a>
                                   <%-- <a href="Admin.aspx" target="_blank" class="interCityBtn">Inter City</a>--%>
                                    <a href="Admin/Admin.aspx" target="_blank" class="interCityBtn" >Inter City</a>
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
  <%--  <script src="https://excelbus.mavenxone.com/assets/global/js/jquery-3.7.1.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/bootstrap.bundle.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/templates/basic/js/main.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/select2.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/moment.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/daterangepicker.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/templates/basic/js/slick.min.js"></script>
    
    <link href="https://excelbus.mavenxone.com/assets/global/css/iziToast.min.css" rel="stylesheet">
    <link href="https://excelbus.mavenxone.com/assets/global/css/iziToast_custom.css" rel="stylesheet">
    <script src="https://excelbus.mavenxone.com/assets/global/js/iziToast.min.js"></script>--%>
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

    <script>
        $(function () {
            'use strict'
            $(".testimonial-slider").slick({
                fade: false,
                slidesToShow: 1,
                slidesToScroll: 1,
                infinite: true,
                autoplay: false,
                pauseOnHover: true,
                centerMode: true,
                dots: true,
                arrows: false,
                nextArrow: '<i class="las la-arrow-right arrow-right"></i>',
                prevArrow: '<i class="las la-arrow-left arrow-left"></i> ',
            });
        });
    </script>

    <script>
        "use strict";
        $('.policy').on('click', function () {
            $.get('https://excelbus.mavenxone.com/cookie/accept', function (response) {
                $('.cookies-card').addClass('d-none');
            });
        });

        setTimeout(function () {
            $('.cookies-card').removeClass('hide')
        }, 2000);

        const userTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        console.log(userTimezone);
    </script>

    <script>
        $(document).ready(function () {
            "use strict";
            $(".langSel").on("click", function () {
                window.location.href = "https://excelbus.mavenxone.com/change/" + $(this).data('code');
            });
        });
    </script>

    <script>
        (function ($) {
            "use strict";
            $('.search').on('change', function () {
                $('#filterForm').submit();
            });
        })(jQuery);
    </script>

    <script>
        (function ($) {
            "use strict";
            var inputElements = $('[type=text],select,textarea');

            $.each(inputElements, function (index, element) {
                element = $(element);
                element.closest('.form-group').find('label').attr('for', element.attr('name'));
                element.attr('id', element.attr('name'));
            });

            $.each($('input, select, textarea'), function (i, element) {
                var elementType = $(element);
                if (elementType.attr('type') != 'checkbox') {
                    if (element.hasAttribute('required')) {
                        $(element).closest('.form-group').find('label').addClass('required');
                    }
                }
            });

            let disableSubmission = false;
            $('.disableSubmission').on('submit', function (e) {
                if (disableSubmission) {
                    e.preventDefault()
                } else {
                    disableSubmission = true;
                }
            });

            $("#confirmationModal").find('.btn--primary').removeClass('btn--primary').addClass('btn--base');
        })(jQuery);
    </script>
</body>

</html>