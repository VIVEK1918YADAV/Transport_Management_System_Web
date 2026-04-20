<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactInfo.aspx.cs" Inherits="NRDC_krishak_online_web.NRDC.ContactInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        
        body {
            background-color: #f5f7fa;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            align-items: center;
        }
        
        .header {
            text-align: center;
            margin-bottom: 40px;
            width: 100%;
            padding: 20px;
            background: linear-gradient(135deg, #2c3e50, #4a6491);
            border-radius: 12px;
            color: white;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
        }
        
        .header h1 {
            font-size: 32px;
            margin-bottom: 8px;
            font-weight: 700;
        }
        
        .header p {
            font-size: 18px;
            opacity: 0.9;
            font-weight: 300;
        }
        
        .repeater-container {
            width: 100%;
            max-width: 1000px;
            margin: 0 auto;
        }
        
        .contact-card {
            background-color: white;
            border-radius: 12px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            padding: 30px;
            margin-bottom: 25px;
            display: flex;
            flex-direction: column;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            position: relative;
            overflow: hidden;
        }
        
        .contact-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.12);
        }
        
        .ceo-card {
            border-top: 5px solid #e74c3c;
        }
        
        .country-manager-card {
            border-top: 5px solid #3498db;
        }
        
        .ceo-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            background-color: #e74c3c;
            color: white;
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.5px;
        }
        
        .name {
            font-size: 26px;
            font-weight: 700;
            color: #2c3e50;
            margin-bottom: 5px;
            line-height: 1.2;
        }
        
        .ceo-card .name {
            font-size: 32px;
        }
        
        .title {
            font-size: 18px;
            color: #3498db;
            margin-bottom: 20px;
            font-weight: 600;
        }
        
        .ceo-card .title {
            color: #e74c3c;
            font-size: 20px;
        }
        
        .company {
            font-size: 20px;
            color: #2c3e50;
            margin-bottom: 5px;
            font-weight: 600;
            padding-bottom: 10px;
            border-bottom: 1px solid #ecf0f1;
        }
        
        .ceo-card .company {
            font-size: 24px;
        }
        
        .tagline {
            font-size: 16px;
            color: #7f8c8d;
            margin-bottom: 25px;
            font-style: italic;
            font-weight: 500;
        }
        
        .contact-info {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 25px;
        }
        
        .phone, .email {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 16px;
        }
        
        .phone {
            color: #27ae60;
        }
        
        .email {
            color: #e74c3c;
        }
        
        .icon {
            font-size: 18px;
        }
        
        .address-container {
            background-color: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            border-left: 4px solid #95a5a6;
        }
        
        .address {
            color: #7f8c8d;
            font-size: 16px;
            margin-bottom: 10px;
            display: flex;
            align-items: flex-start;
            gap: 8px;
            line-height: 1.5;
        }
        
        .website {
            color: #3498db;
            font-size: 16px;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 8px;
            font-weight: 600;
            padding: 10px 15px;
            background-color: #f0f7ff;
            border-radius: 8px;
            transition: background-color 0.3s ease;
        }
        
        .website:hover {
            text-decoration: underline;
            background-color: #e1f0ff;
        }
        
        .save-contact-btn {
            background: linear-gradient(135deg, #27ae60, #229954);
            color: white;
            border: none;
            padding: 15px 30px;
            font-size: 16px;
            font-weight: 600;
            border-radius: 8px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 10px rgba(39, 174, 96, 0.3);
            margin-top: 20px;
        }
        
        .save-contact-btn:hover {
            background: linear-gradient(135deg, #229954, #1e8449);
            box-shadow: 0 6px 15px rgba(39, 174, 96, 0.4);
            transform: translateY(-2px);
        }
        
        .save-contact-btn:active {
            transform: translateY(0);
        }
        
        .footer {
            margin-top: 30px;
            text-align: center;
            color: #95a5a6;
            font-size: 14px;
            width: 100%;
            padding-top: 20px;
            border-top: 1px solid #ecf0f1;
        }
        
        @media (max-width: 768px) {
            .contact-card {
                padding: 20px;
            }
            
            .name {
                font-size: 22px;
            }
            
            .ceo-card .name {
                font-size: 26px;
            }
            
            .title {
                font-size: 16px;
            }
            
            .contact-info {
                flex-direction: column;
                gap: 10px;
            }
        }
    </style>
    <title>Contact Information - Excel Geomatics Limited</title>

</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1>Excel Geomatics Limited</h1>
            <p>Delivering Solutions to the World</p>
        </div>
        
        <!-- Yogesh Container -->
        <div class="repeater-container" id="yogeshContainer" runat="server" visible="false">
            <asp:Repeater ID="yogeshRepeater" runat="server">
                <ItemTemplate>
                    <div class="contact-card country-manager-card">
                        <div class="name"><%# Eval("Name") %></div>
                        <div class="title"><%# Eval("Title") %></div>
                        <div class="company">Excel Geomatics Limited</div>
                        <div class="tagline">Delivering Solutions to the World</div>

                        <div class="contact-info">
                            <div class="phone">
                                <span class="icon">📱</span>
                                <span><%# Eval("Phone") %></span>
                            </div>
                            <div class="email">
                                <span class="icon">✉️</span>
                                <span><%# Eval("Email") %></span>
                            </div>
                        </div>

                        <div class="address-container">
                            <div class="address">
                                <span class="icon">📍</span>
                                <span><%# Eval("Address") %></span>
                            </div>
                        </div>

                        <a href='http://<%# Eval("Website") %>' class="website" target="_blank">
                            <span class="icon">🌐</span>
                            <span><%# Eval("Website") %></span>
                        </a>

                        <a href="#" class="save-contact-btn" onclick="addToContacts(event, '<%# Eval("Name") %>', '<%# Eval("Phone") %>', '<%# Eval("Email") %>', '<%# Eval("Address") %>', '<%# Eval("Website") %>', '<%# Eval("Title") %>', 'Excel Geomatics Limited')">
                            <span class="icon">👤</span>
                            <span>Add to Contacts</span>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Rajesh Container -->
        <div class="repeater-container" id="rajeshContainer" runat="server" visible="false">
            <asp:Repeater ID="rajeshRepeater" runat="server">
                <ItemTemplate>
                    <div class="contact-card ceo-card">
                        <div class="ceo-badge">CEO</div>
                        <div class="name"><%# Eval("Name") %></div>
                        <div class="title"><%# Eval("Title") %></div>
                        <div class="company">Excel Geomatics Limited</div>
                        <div class="tagline">Delivering Solutions to the World</div>

                        <div class="contact-info">
                            <div class="phone">
                                <span class="icon">📱</span>
                                <span><%# Eval("Phone") %></span>
                            </div>
                            <div class="email">
                                <span class="icon">✉️</span>
                                <span><%# Eval("Email") %></span>
                            </div>
                        </div>

                        <div class="address-container">
                            <div class="address">
                                <span class="icon">📍</span>
                                <span><%# Eval("Address") %></span>
                            </div>
                        </div>

                        <a href='http://<%# Eval("Website") %>' class="website" target="_blank">
                            <span class="icon">🌐</span>
                            <span><%# Eval("Website") %></span>
                        </a>

                        <a href="#" class="save-contact-btn" onclick="addToContacts(event, '<%# Eval("Name") %>', '<%# Eval("Phone") %>', '<%# Eval("Email") %>', '<%# Eval("Address") %>', '<%# Eval("Website") %>', '<%# Eval("Title") %>', 'Excel Geomatics Limited')">
                            <span class="icon">👤</span>
                            <span>Add to Contacts</span>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        
        <div class="repeater-container" id="defaultMessage" runat="server" visible="false">
            <div class="contact-card" style="text-align: center; padding: 40px;">
                <h2 style="color: #e74c3c; margin-bottom: 20px;">No Contact Found</h2>
                <p style="color: #7f8c8d; margin-bottom: 20px;">Please scan a valid QR code or use a proper URL parameter.</p>
                <p style="color: #3498db;">Valid parameters: <strong>name=yogesh</strong> or <strong>name=rajesh</strong></p>
            </div>
        </div>

        <div class="footer">
            <p>© <% = DateTime.Now.Year %> Excel Geomatics Limited. All rights reserved.</p>
        </div>
    </form>

    <script>
        function addToContacts(event, name, phone, email, address, website, title, company) {
            event.preventDefault();

            var vcard = "BEGIN:VCARD\n";
            vcard += "VERSION:3.0\n";
            vcard += "FN:" + name + "\n";
            vcard += "N:" + name.split(' ').reverse().join(';') + ";;;\n";
            vcard += "ORG:" + company + "\n";
            vcard += "TITLE:" + title + "\n";
            vcard += "TEL;TYPE=CELL:" + phone + "\n";
            vcard += "EMAIL;TYPE=INTERNET:" + email + "\n";
            vcard += "ADR;TYPE=WORK:;;" + address.replace(/\n/g, ';') + "\n";
            vcard += "URL:" + website + "\n";
            vcard += "END:VCARD";

            var dataUri = "data:text/vcard;charset=utf-8," + encodeURIComponent(vcard);

            window.location.href = dataUri;
        }
    </script>
 
</body>
</html>
