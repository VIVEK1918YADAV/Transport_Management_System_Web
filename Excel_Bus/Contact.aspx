<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Excel_Bus.Contact" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Scoped styles - won't affect master page */
        #contactPageContent {
            padding: 20px 0;
        }

        #contactPageContent .page-title {
            font-size: 2rem;
            font-weight: 600;
            color: #2c3e50;
            margin-bottom: 10px;
        }

        #contactPageContent .page-subtitle {
            font-size: 1.1rem;
            color: #6c757d;
            margin-bottom: 30px;
        }

        #contactPageContent .contact-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            margin-top: 20px;
        }

        #contactPageContent .contact-card {
            background: #fff;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        #contactPageContent .card-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #007bff;
        }

        #contactPageContent .contact-item {
            margin-bottom: 15px;
            padding: 12px;
            background: #f8f9fa;
            border-radius: 5px;
        }

        #contactPageContent .contact-label {
            font-weight: 600;
            color: #495057;
            font-size: 0.9rem;
            text-transform: uppercase;
            margin-bottom: 8px;
            display: block;
        }

        #contactPageContent .contact-value {
            color: #2c3e50;
            font-size: 1.05rem;
            line-height: 1.6;
        }

        #contactPageContent .contact-value a {
            color: #007bff;
            text-decoration: none;
            font-size: 1.05rem;
        }

        #contactPageContent .contact-value a:hover {
            text-decoration: underline;
        }

        #contactPageContent .map-container {
            position: relative;
            height: 350px;
            background: #f0f0f0;
            border-radius: 5px;
            overflow: hidden;
        }

        #contactPageContent .map-frame {
            width: 100%;
            height: 100%;
            border: 0;
        }

        #contactPageContent .view-map-btn {
            display: inline-block;
            margin-top: 15px;
            padding: 12px 24px;
            background: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            font-weight: 600;
            font-size: 1rem;
            transition: background 0.3s ease;
        }

        #contactPageContent .view-map-btn:hover {
            background: #0056b3;
            color: white;
            text-decoration: none;
        }

        @media (max-width: 768px) {
            #contactPageContent .contact-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>

    <div id="contactPageContent">
        <h2 class="page-title">Contact</h2>
        <h3 class="page-subtitle">Get in touch with us.</h3>

        <div class="contact-grid">
            <!-- Contact Information Card -->
            <div class="contact-card">
                <h4 class="card-title">Excel Geomatics</h4>
                
                <div class="contact-item">
                    <span class="contact-label">📍 ADDRESS</span>
                    <div class="contact-value">
                        H-43, 3rd Floor, H Block, Sector-63<br />
                        Noida, Uttar Pradesh 201301<br />
                        NOIDA UP
                    </div>
                </div>

                <div class="contact-item">
                    <span class="contact-label">📞 PHONE</span>
                    <div class="contact-value">
                        <a href="tel:+919871771849">+91-9871771849</a>
                    </div>
                </div>

                <div class="contact-item">
                    <span class="contact-label">✉️ EMAIL</span>
                    <div class="contact-value">
                        <a href="mailto:info@excelgeomatics.com">info@excelgeomatics.com</a>
                    </div>
                </div>

                <div class="contact-item">
                    <span class="contact-label">📧 ALTERNATIVE EMAIL</span>
                    <div class="contact-value">
                        <a href="mailto:rajesh.paul@excelgeomatics.com">rajesh.paul@excelgeomatics.com</a>
                    </div>
                </div>
            </div>

            <!-- Location Map Card -->
            <div class="contact-card">
                <h4 class="card-title">Location Map</h4>
                <div class="map-container">
                    <!-- Replace this iframe src with your actual Google Maps embed code from Share > Embed a map -->
                    <iframe 
                        class="map-frame"
                        src="https://www.google.com/maps?q=H-43,+3rd+Floor,+H+Block,+Sector-63,+Noida,+Uttar+Pradesh+201301&output=embed" 
                        allowfullscreen="" 
                        loading="lazy"
                        referrerpolicy="no-referrer-when-downgrade">
                    </iframe>
                </div>
                <a href="https://www.google.com/maps/search/Excel+Geomatics+Pvt+Ltd+H-43+Sector+63+Noida" 
                   target="_blank" 
                   class="view-map-btn">
                    🗺️ View Larger Map
                </a>
            </div>
        </div>
    </div>
</asp:Content>
