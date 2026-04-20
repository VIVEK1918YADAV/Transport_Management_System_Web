<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="vehicleUser.aspx.cs" Inherits="Excel_Bus.Admin.vehicleUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: linear-gradient(to right, #667eea, #764ba2);
        }

        .page-wrapper {
            min-height: 100vh;
            padding: 30px 20px;
        }

        .form-container {
            background: #ffffff;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0px 4px 20px rgba(0, 0, 0, 0.15);
            margin: 0 auto;
        }

        h2 {
            text-align: center;
            color: #333;
            margin-bottom: 30px;
        }

        .form-row {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 20px;
        }

        .form-group {
            flex: 1 1 calc(50% - 10px);
        }

        label {
            display: block;
            font-weight: bold;
            margin-bottom: 6px;
            color: #444;
        }

        .aspNetInput {
            width: 100%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 15px;
            box-sizing: border-box;
        }

        .btn {
            background-color: #667eea;
            color: white;
            padding: 12px 0;
            width: 100%;
            border: none;
            border-radius: 6px;
            font-size: 16px;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn:hover {
            background-color: #5a67d8;
        }

        .result-message {
            text-align: center;
            margin-top: 20px;
            font-weight: bold;
        }

        @media (max-width: 768px) {
            .form-group {
                flex: 1 1 100%;
            }
        }

        .popupbackground {
            background-color: #000000ad;
            position: absolute;
            top: 34px;
            width: calc(100% - 0px);
            left: 0px;
            border: 1px solid red;
            height: 100vh;
            display: none;
        }

        .popupbackground .popupmain {
            background-color: white;
            width: 40%;
            margin-top: 5% !important;
            margin: 0 auto;
            height: 40%;
        }

        .popupbackground .popupmain .header {
            display: flex;
            justify-content: space-between;
            border: 1px solid chartreuse;
            padding: 15px;
        }

        /* Custom styles for status dropdown */
        .status-dropdown {
            width: 100%;
            padding: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;
            background-color: white;
        }

        .status-active {
            background-color: #d4edda;
            color: #155724;
        }

        .status-inactive {
            background-color: #f8d7da;
            color: #721c24;
        }

        /* Action Buttons Styles */
        .top-action-buttons {
            text-align: center;
            margin: 20px 0;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .action-btn {
            background: linear-gradient(45deg, #007bff, #0056b3);
            color: white;
            border: none;
            padding: 12px 25px;
            margin: 5px 10px;
            border-radius: 6px;
            font-weight: bold;
            font-size: 14px;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 2px 5px rgba(0, 123, 255, 0.3);
        }

        .action-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(0, 123, 255, 0.4);
            color: white;
        }

        .action-btn.upload-btn {
            background: linear-gradient(45deg, #28a745, #20c997);
            box-shadow: 0 2px 5px rgba(40, 167, 69, 0.3);
        }

        .action-btn.upload-btn:hover {
            box-shadow: 0 4px 10px rgba(40, 167, 69, 0.4);
        }

        .action-btn.download-btn {
            background: linear-gradient(45deg, #17a2b8, #138496);
            box-shadow: 0 2px 5px rgba(23, 162, 184, 0.3);
        }

        .action-btn.download-btn:hover {
            box-shadow: 0 4px 10px rgba(23, 162, 184, 0.4);
        }

        .toggle-container { display: none !important; }

        /* Toggle Switch Styles */
        .toggle-container {
            display: flex;
            border: 1px solid #007bff;
            border-radius: 5px;
            overflow: hidden;
            margin-bottom: 20px;
        }

        .toggle-btn {
            flex: 1;
            padding: 12px 20px;
            border: none;
            background: #f8f9fa;
            color: #007bff;
            cursor: pointer;
            transition: all 0.3s ease;
            font-weight: bold;
        }

        .toggle-btn.active {
            background: #007bff;
            color: white;
        }

        .toggle-btn:hover:not(.active) {
            background: #e9ecef;
        }

        /* Excel Import Styles */
        .excel-import-section {
            background: #d4edda;
            border: 1px solid #c3e6cb;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 20px;
        }

        .file-info {
            margin-top: 5px;
            font-size: 12px;
        }

        .format {
            background: #e9ecef;
            padding: 2px 5px;
            border-radius: 3px;
            margin: 0 2px;
        }

        .preview-section {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 5px;
            padding: 15px;
            margin-top: 20px;
        }

        .sample-file-info {
            background: #d1ecf1;
            border: 1px solid #bee5eb;
            border-radius: 5px;
            padding: 15px;
            margin: 15px 0;
        }

        @media (max-width: 768px) {
            .action-btn {
                display: block;
                width: 200px;
                margin: 5px auto;
            }
        }

        .required {
            color: red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
       <div class="form-container">
            <h2>Add Vehicle User</h2>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtName">Name <span class="required">*</span></label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="aspNetInput" placeholder="e.g., John Doe" />
                </div>

                <div class="form-group">
                    <label for="ddlrole">Role <span class="required">*</span></label>
                    <asp:DropDownList runat="server" ID="ddlrole" CssClass="aspNetInput">
                        <asp:ListItem Value="4">Driver</asp:ListItem>
                        <asp:ListItem Value="5" Selected="True">Conductor</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtPhoneNumber">Phone Number <span class="required">*</span></label>
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="aspNetInput" placeholder="e.g., 9999999999" />
                </div>

                <div class="form-group">
                    <label for="txtLicenseNumber">License Number <span class="required">*</span></label>
                    <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="aspNetInput" placeholder="e.g., UP161232" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtUsername">Username <span class="required">*</span></label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="aspNetInput" placeholder="e.g., john.doe" />
                </div>

                <div class="form-group">
                    <label for="txtPassword">Password <span class="required">*</span></label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="aspNetInput" TextMode="Password" placeholder="Enter password" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtDateOfJoining">Date of Joining <span class="required">*</span></label>
                    <asp:TextBox ID="txtDateOfJoining" runat="server" CssClass="aspNetInput" TextMode="Date" />
                </div>
                <div class="form-group">
                    <!-- Empty space for alignment -->
                </div>
            </div>
           <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn" OnClick="btnSubmit_Click" />

<%--            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn" OnClick="btnSubmit_Click" 
                        OnClientClick="return validateMainForm();" />--%>
            <asp:Label ID="lblResult" runat="server" CssClass="result-message" ForeColor="Green" />
        </div>

     <!-- Vehicle Users Grid -->
        <div style="margin-top: 40px;" class="form-container">
            <asp:GridView ID="gvAdmins" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" OnRowDataBound="gvAdmins_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="S No.">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ID" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblid" runat="server" Text='<%# Eval("vehicleUserId") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("vehicleUserName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Role" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblrole" runat="server" Text='<%# Eval("RoleId") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Phone Number">
                        <ItemTemplate>
                            <asp:Label ID="lblphoneNumber" runat="server" Text='<%# Eval("MobileNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="License Number">
                        <ItemTemplate>
                            <asp:Label ID="lbllicenseNumber" runat="server" Text='<%# Eval("LicenseNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                   <%-- <asp:TemplateField HeaderText="Assigned Bus">
                        <ItemTemplate>
                            <asp:Label ID="lblassignedBusNumber" runat="server" Text='<%# Eval("AssignedBusNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="Date Joining">
                        <ItemTemplate>
                            <asp:Label ID="lbldateOfJoining" runat="server" Text='<%# Eval("DateOfJoining") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Username">
                        <ItemTemplate>
                            <asp:Label ID="lblusername" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                </Columns>
            </asp:GridView>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
