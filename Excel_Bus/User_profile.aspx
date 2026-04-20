<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User_profile.aspx.cs" Inherits="Excel_Bus.User_profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

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
      <!-- Banner Section -->
        <section class="inner-banner bg_img" style="background: url(img/bg_bus_img.jpg) center">
            <img src="img/bg_bus_img.jpg" />
            <div class="container">
                <div class="inner-banner-content">
                    <h2 class="title">Profile Settings</h2>
                </div>
            </div>
        </section>

        <!-- Profile Section -->
        <section class="dashboard-section padding-top padding-bottom">
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

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
       <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="js/main.js"></script>
   <%-- <script src="https://excelbus.mavenxone.com/assets/global/js/jquery-3.7.1.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/global/js/bootstrap.bundle.min.js"></script>
    <script src="https://excelbus.mavenxone.com/assets/templates/basic/js/main.js"></script>--%>

    <!-- iziToast -->
    <script src="js/iziToast.min.js"></script>
 <%--   <script src="https://excelbus.mavenxone.com/assets/global/js/iziToast.min.js"></script>--%>

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
</asp:Content>
