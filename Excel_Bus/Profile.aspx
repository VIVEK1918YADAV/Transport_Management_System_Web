<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="ExcelBus.Profile" Async="true" %>

<!doctype html>
<html lang="en" itemscope itemtype="http://schema.org/WebPage">

<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>ExcelBus - Profile</title>
    <meta name="title" content="ExcelBus - Profile">
    <link rel="shortcut icon" href="img/favicon.png" type="image/x-icon">

    <!-- BootStrap Link -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- Icon Link -->
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/global-line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />

    <!-- Custom Link -->
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />
 
    <!-- iziToast -->
    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">
        <!-- Google Translate -->
        <div id="google_translate_element"></div>

        <script type="text/javascript">
            function googleTranslateElementInit() {
                new google.translate.TranslateElement({
                    pageLanguage: 'en',
                    layout: google.translate.TranslateElement.InlineLayout.HORIZONTAL,
                    includedLanguages: 'en,fr,hi,de,es'
                }, 'google_translate_element');
            }
        </script>

        <script type="text/javascript"
            src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit">
        </script>

        <div class="overlay"></div>

        <!-- Preloader -->
        <div class="preloader">
            <div class="loader-wrapper">
                <div class="truck-wrapper">
                    <div class="preloader-content"></div>
                    <div class="truck">
                        <div class="truck-container"></div>
                        <div class="glases"></div>
                        <div class="bonet"></div>
                        <div class="base"></div>
                        <div class="base-aux"></div>
                        <div class="wheel-back"></div>
                        <div class="wheel-front"></div>
                        <div class="smoke"></div>
                    </div>
                </div>
            </div>
        </div>

     
        <!-- Banner Section -->
           <section class="inner-banner bg_img" style="background: url('img/bg_bus_img.jpg') center">
            <div class="container">
                <div class="inner-banner-content">
                    <h2 class="title">Profile Settings</h2>
                </div>
            </div>
        </section>
        <section class="dashboard-section padding-top padding-bottom">
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-lg-10">
                <!-- Profile Picture Section -->
             <%--   <div class="card mb-4">
                    <div class="card-body p-4">
                        <h5 class="card-title mb-4">Profile Picture</h5>
                        <div class="row align-items-center">
                            <div class="col-md-3 text-center">
                                <div class="profile-image-container">
                                    <asp:Image ID="imgProfile" runat="server" CssClass="profile-picture" ImageUrl="img/excel_bus_logo.png" AlternateText="Profile Picture" />
                                    <div class="profile-overlay">
                                        <i class="fas fa-camera"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-9">
                                <div class="mb-3">
                                    <label class="form-label">Upload New Picture</label>
                                    <asp:FileUpload ID="fileProfilePicture" runat="server" CssClass="form-control" accept="image/*" />
                                    <small class="text-muted">Allowed formats: JPG, PNG, GIF. Max size: 2MB</small>
                                </div>
                                <%--<asp:Button ID="btnUploadPicture" runat="server" Text="Upload Picture" CssClass="btn btn-primary" OnClick="btnUploadPicture_Click" />--%>
                                
                                <%--<asp:Button ID="btnRemovePicture" runat="server" Text="Remove Picture" CssClass="btn btn-outline-danger ms-2" OnClick="btnRemovePicture_Click" />
                               
                            </div>
                        </div>
                    </div>
                </div>--%>

                <!-- Personal Information -->
                <div class="card mb-4">
                    <div class="card-body p-4">
                        <h5 class="card-title mb-4">Personal Information</h5>
                        <div class="row gy-4">
                            <!-- First Name -->
                            <div class="col-md-6">
                                <label class="form-label">First Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter first name" required="required"></asp:TextBox>
                            </div>

                            <!-- Last Name -->
                            <div class="col-md-6">
                                <label class="form-label">Last Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter last name" required="required"></asp:TextBox>
                            </div>

                            <!-- Username -->
                            <div class="col-md-6">
                                <label class="form-label">Username</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
                            </div>

                            <!-- Email -->
                            <div class="col-md-6">
                                <label class="form-label">E-mail Address <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter email" ReadOnly="true"></asp:TextBox>
                            </div>

                            <!-- Dial Code -->
                            <div class="col-md-6">
                                <label class="form-label">Dial Code</label>
                                <asp:TextBox ID="txtDialCode" runat="server" CssClass="form-control" placeholder="e.g., +91"></asp:TextBox>
                            </div>

                            <!-- Mobile Number -->
                            <div class="col-md-6">
                                <label class="form-label">Mobile Number</label>
                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter mobile number"></asp:TextBox>
                            </div>

                            <!-- Address -->
                            <div class="col-md-12">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter address"></asp:TextBox>
                            </div>

                            <!-- City -->
                            <div class="col-md-6">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="Enter city"></asp:TextBox>
                            </div>

                            <!-- State -->
                            <div class="col-md-6">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtState" runat="server" CssClass="form-control" placeholder="Enter state"></asp:TextBox>
                            </div>

                            <!-- Zip Code -->
                            <div class="col-md-6">
                                <label class="form-label">Zip Code</label>
                                <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" placeholder="Enter zip code"></asp:TextBox>
                            </div>

                            <!-- Country -->
                            <div class="col-md-6">
                                <label class="form-label">Country</label>
                                <asp:TextBox ID="txtCountryName" runat="server" CssClass="form-control" placeholder="Enter country" ReadOnly="true"></asp:TextBox>
                            </div>

                            <!-- Balance (Read-only) -->
                            <div class="col-md-6">
                                <label class="form-label">Wallet Balance</label>
                                <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>

                            <!-- Submit Button -->
                           <%-- <div class="col-12">
                                <asp:Button ID="btnUpdateProfile" runat="server" Text="Update Profile" CssClass="cmn--btn btn--md w-100" OnClick="btnUpdateProfile_Click" />
                            </div>--%>
                        </div>
                    </div>
                </div>

                <!-- Change Password Section -->
               <%-- <div class="card">
                    <div class="card-body p-4">
                        <h5 class="card-title mb-4">Change Password</h5>
                        <div class="row gy-4">
                            <!-- Current Password -->
                            <div class="col-md-12">
                                <label class="form-label">Current Password <span class="text-danger">*</span></label>
                                <div class="password-field">
                                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter current password"></asp:TextBox>
                                    <span class="password-toggle" onclick="togglePassword('txtCurrentPassword', this)">
                                        <i class="fas fa-eye"></i>
                                    </span>
                                </div>
                            </div>

                            <!-- New Password -->
                            <div class="col-md-6">
                                <label class="form-label">New Password <span class="text-danger">*</span></label>
                                <div class="password-field">
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
                                    <span class="password-toggle" onclick="togglePassword('txtNewPassword', this)">
                                        <i class="fas fa-eye"></i>
                                    </span>
                                </div>
                                <small class="text-muted">Min 8 characters, include uppercase, lowercase, number & special character</small>
                            </div>

                            <!-- Confirm New Password -->
                            <div class="col-md-6">
                                <label class="form-label">Confirm New Password <span class="text-danger">*</span></label>
                                <div class="password-field">
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm new password"></asp:TextBox>
                                    <span class="password-toggle" onclick="togglePassword('txtConfirmPassword', this)">
                                        <i class="fas fa-eye"></i>
                                    </span>
                                </div>
                            </div>

                            <!-- Password Strength Indicator -->
                            <div class="col-md-12">
                                <div class="password-strength">
                                    <div class="strength-bar">
                                        <div id="strengthBar" class="strength-progress"></div>
                                    </div>
                                    <span id="strengthText" class="strength-text"></span>
                                </div>
                            </div>

                            <!-- Change Password Button -->
                            <div class="col-12">
                                <%--<asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-success btn--md w-100" OnClick="btnChangePassword_Click" />
                              
                            </div>
                        </div>
                    </div>
                </div>--%>
                </div>
            </div>
        </div>
           
</section>

<style>
/* Profile Picture Styles */
.profile-image-container {
    position: relative;
    width: 150px;
    height: 150px;
    margin: 0 auto;
    cursor: pointer;
    border-radius: 50%;
    overflow: hidden;
    border: 4px solid #e9ecef;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.profile-picture {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.profile-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0,0,0,0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    opacity: 0;
    transition: opacity 0.3s ease;
}

.profile-image-container:hover .profile-overlay {
    opacity: 1;
}

.profile-overlay i {
    color: white;
    font-size: 2rem;
}

/* Password Field Styles */
.password-field {
    position: relative;
}

.password-toggle {
    position: absolute;
    right: 15px;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
    color: #6c757d;
    z-index: 10;
}

.password-toggle:hover {
    color: #495057;
}

/* Password Strength Indicator */
.password-strength {
    margin-top: 10px;
}

.strength-bar {
    height: 6px;
    background: #e9ecef;
    border-radius: 3px;
    overflow: hidden;
    margin-bottom: 8px;
}

.strength-progress {
    height: 100%;
    width: 0%;
    transition: all 0.3s ease;
    border-radius: 3px;
}

.strength-text {
    font-size: 0.875rem;
    font-weight: 600;
}

/* Strength levels */
.strength-weak {
    background: #dc3545;
}

.strength-fair {
    background: #ffc107;
}

.strength-good {
    background: #17a2b8;
}

.strength-strong {
    background: #28a745;
}

/* Card Styles */
.card {
    border: none;
    border-radius: 10px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}

.card-title {
    font-size: 1.25rem;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #f0f0f0;
    padding-bottom: 12px;
}

/* Form Styles */
.form-label {
    font-weight: 500;
    color: #495057;
    margin-bottom: 8px;
}

.form-control:focus {
    border-color: #667eea;
    box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
}

/* Button Styles */
.btn-primary {
    background: #667eea;
    border: none;
    padding: 8px 20px;
}

.btn-primary:hover {
    background: #5568d3;
}

.btn-outline-danger:hover {
    background: #dc3545;
    color: white;
}

.btn-success {
    background: #28a745;
    border: none;
}

.btn-success:hover {
    background: #218838;
}

/* Responsive */
@media (max-width: 768px) {
    .profile-image-container {
        width: 120px;
        height: 120px;
    }
    
    .card-body {
        padding: 1.5rem !important;
    }
}
</style>

<script>
    // Toggle Password Visibility
    function togglePassword(fieldId, icon) {
        const field = document.getElementById(fieldId);
        const iconElement = icon.querySelector('i');

        if (field.type === 'password') {
            field.type = 'text';
            iconElement.classList.remove('fa-eye');
            iconElement.classList.add('fa-eye-slash');
        } else {
            field.type = 'password';
            iconElement.classList.remove('fa-eye-slash');
            iconElement.classList.add('fa-eye');
        }
    }

    // Password Strength Checker
    <%--document.addEventListener('DOMContentLoaded', function () {
        const newPasswordField = document.getElementById('<%= txtNewPassword.ClientID %>');

    if (newPasswordField) {
        newPasswordField.addEventListener('input', function () {
            checkPasswordStrength(this.value);
        });
    }--%>
});

    function checkPasswordStrength(password) {
        const strengthBar = document.getElementById('strengthBar');
        const strengthText = document.getElementById('strengthText');

        if (!password) {
            strengthBar.style.width = '0%';
            strengthText.textContent = '';
            return;
        }

        let strength = 0;

        // Check length
        if (password.length >= 8) strength += 25;

        // Check for lowercase
        if (/[a-z]/.test(password)) strength += 25;

        // Check for uppercase
        if (/[A-Z]/.test(password)) strength += 25;

        // Check for numbers and special characters
        if (/[0-9]/.test(password) && /[^A-Za-z0-9]/.test(password)) strength += 25;

        // Update UI
        strengthBar.style.width = strength + '%';
        strengthBar.className = 'strength-progress';

        if (strength <= 25) {
            strengthBar.classList.add('strength-weak');
            strengthText.textContent = 'Weak';
            strengthText.style.color = '#dc3545';
        } else if (strength <= 50) {
            strengthBar.classList.add('strength-fair');
            strengthText.textContent = 'Fair';
            strengthText.style.color = '#ffc107';
        } else if (strength <= 75) {
            strengthBar.classList.add('strength-good');
            strengthText.textContent = 'Good';
            strengthText.style.color = '#17a2b8';
        } else {
            strengthBar.classList.add('strength-strong');
            strengthText.textContent = 'Strong';
            strengthText.style.color = '#28a745';
        }
    }
</script>
        <!-- Profile Section -->
       <%-- <section class="dashboard-section padding-top padding-bottom">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-10">
                        <div class="card">
                            <div class="card-body p-4">
                                <div class="row gy-4">
                                    <!-- First Name -->
                                    <div class="col-md-6">
                                        <label class="form-label">First Name <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter first name" required="required"></asp:TextBox>
                                    </div>

                                    <!-- Last Name -->
                                    <div class="col-md-6">
                                        <label class="form-label">Last Name <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter last name" required="required"></asp:TextBox>
                                    </div>

                                    <!-- Username -->
                                    <div class="col-md-6">
                                        <label class="form-label">Username</label>
                                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
                                    </div>

                                    <!-- Email -->
                                    <div class="col-md-6">
                                        <label class="form-label">E-mail Address <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter email" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <!-- Dial Code -->
                                    <div class="col-md-6">
                                        <label class="form-label">Dial Code</label>
                                        <asp:TextBox ID="txtDialCode" runat="server" CssClass="form-control" placeholder="e.g., +91"></asp:TextBox>
                                    </div>

                                    <!-- Mobile Number -->
                                    <div class="col-md-6">
                                        <label class="form-label">Mobile Number</label>
                                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter mobile number"></asp:TextBox>
                                    </div>

                                    <!-- Address -->
                                    <div class="col-md-12">
                                        <label class="form-label">Address</label>
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter address"></asp:TextBox>
                                    </div>

                                    <!-- City -->
                                    <div class="col-md-6">
                                        <label class="form-label">City</label>
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="Enter city"></asp:TextBox>
                                    </div>

                                    <!-- State -->
                                    <div class="col-md-6">
                                        <label class="form-label">State</label>
                                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" placeholder="Enter state"></asp:TextBox>
                                    </div>

                                    <!-- Zip Code -->
                                    <div class="col-md-6">
                                        <label class="form-label">Zip Code</label>
                                        <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" placeholder="Enter zip code"></asp:TextBox>
                                    </div>

                                    <!-- Country -->
                                    <div class="col-md-6">
                                        <label class="form-label">Country</label>
                                        <asp:TextBox ID="txtCountryName" runat="server" CssClass="form-control" placeholder="Enter country" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <!-- Balance (Read-only) -->
                                    <div class="col-md-6">
                                        <label class="form-label">Wallet Balance</label>
                                        <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <!-- Submit Button -->
                                    <div class="col-12">
                                        <asp:Button ID="btnUpdateProfile" runat="server" Text="Update Profile" CssClass="cmn--btn btn--md w-100" OnClick="btnUpdateProfile_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>--%>

        <!-- Footer -->
        <section class="footer-section">
            <div class="footer-top">
                <div class="container">
                    <div class="row footer-wrapper gy-sm-5 gy-4">
                        <div class="col-xl-4 col-lg-3 col-md-6 col-sm-6">
                            <div class="footer-widget">
                                <div class="logo">
                                    <img src="img/excel_bus_logo.png" alt="Logo">
                                </div>
                                <p>Excel Bus - Your trusted bus booking partner</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <a href="javascript::void()" class="scrollToTop"><i class="las la-chevron-up"></i></a>

        <!-- Hidden fields for data storage -->
        <asp:HiddenField ID="hdnUserId" runat="server" />
        <asp:HiddenField ID="hdnCountryCode" runat="server" />
        
        <!-- Hidden fields for message display -->
        <asp:HiddenField ID="hdnShowMessage" runat="server" Value="false" />
        <asp:HiddenField ID="hdnMessageType" runat="server" />
        <asp:HiddenField ID="hdnMessageText" runat="server" />
    </form>

    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>
 
    <script src="js/iziToast.min.js"></script>
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

        // Check for messages after page load
        $(document).ready(function () {
            var showMsg = $('#<%= hdnShowMessage.ClientID %>').val();
            var msgType = $('#<%= hdnMessageType.ClientID %>').val();
            var msgText = $('#<%= hdnMessageText.ClientID %>').val();
            
            if (showMsg === 'true' && msgText) {
                notify(msgType, msgText);
                // Clear the hidden field
                $('#<%= hdnShowMessage.ClientID %>').val('false');
            }
        });
    </script>
</body>
</html>