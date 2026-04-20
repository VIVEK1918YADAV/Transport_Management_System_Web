<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_User.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_User" %>
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

        /*.page-wrapper {
            min-height: 100vh;
            padding: 30px 20px;
        }*/

        .form-container {
            background: #dff3dc;
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

        .btn-submit {
            background-color: #12460b;
            color: white;
            padding: 12px 0;
            width: 100%;
            border: none;
            border-radius: 6px;
            font-size: 16px;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn-submit:hover {
            background-color: #209710;
        }

        .result-message {
            text-align: center;
            margin-top: 20px;
            font-weight: bold;
        }

        .required {
            color: red;
        }

        .status-dropdown {
            padding: 5px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            background-color: white;
            font-size: 13px;
        }

        .status-active {
            background-color: #d4edda;
            color: #155724;
            border-color: #c3e6cb;
        }

        .status-inactive {
            background-color: #f8d7da;
            color: #721c24;
            border-color: #f5c6cb;
        }

        .btn-update-status {
            display: none; /* hidden - no longer needed */
        }

        .status-cell {
            display: flex;
            align-items: center;
            gap: 6px;
        }

        @media (max-width: 768px) {
            .form-group {
                flex: 1 1 100%;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Add Train  User Form -->
    <div class="form-container">
        <h2>Add Train  User</h2>

        <div class="form-row">
            <div class="form-group">
                <label for="txtName">Name <span class="required">*</span></label>
                <asp:TextBox ID="txtName" runat="server" CssClass="aspNetInput" placeholder="e.g., Ramesh Kumar" />
            </div>

            <div class="form-group">
                <label for="ddlRole">Role <span class="required">*</span></label>
                <asp:DropDownList runat="server" ID="ddlRole" CssClass="aspNetInput">
                    <asp:ListItem Value="6">Loco Pilot</asp:ListItem>
                    <asp:ListItem Value="7" Selected="True">Loco Inspector</asp:ListItem>
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
                <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="aspNetInput" placeholder="e.g., LIC123456" />
            </div>
        </div>

        <div class="form-row">
            <div class="form-group">
                <label for="txtUsername">Username <span class="required">*</span></label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="aspNetInput" placeholder="e.g., ramesh.kumar" />
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
                <!-- Empty for alignment -->
            </div>
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit" OnClick="btnSubmit_Click" />
        <asp:Label ID="lblResult" runat="server" CssClass="result-message" ForeColor="Green" />
    </div>

    <!-- Train  Users Grid -->
    <div style="margin-top: 40px;" class="form-container">
        <h2>Train Users List</h2>
        <asp:GridView ID="gvTrainUsers" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" OnRowDataBound="gvTrainUsers_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="S No.">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Train User ID" Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="lblTrainUserId" runat="server" Text='<%# Eval("trainUserId") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("trainUserName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate>
                        <asp:Label ID="lblRole" runat="server" Text='<%# Eval("roleId") != null ? (Convert.ToInt32(Eval("roleId")) == 6 ? "Loco Pilot" : "Loco Inspector") : "" %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Phone Number">
                    <ItemTemplate>
                        <asp:Label ID="lblPhoneNumber" runat="server" Text='<%# Eval("mobileNo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="License Number">
                    <ItemTemplate>
                        <asp:Label ID="lblLicenseNumber" runat="server" Text='<%# Eval("licenseNo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Date of Joining">
                    <ItemTemplate>
                        <asp:Label ID="lblDateOfJoining" runat="server" Text='<%# Eval("dateOfJoining") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Username">
                    <ItemTemplate>
                        <asp:Label ID="lblUsername" runat="server" Text='<%# Eval("username") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="status-dropdown"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"
                            data-userid='<%# Eval("trainUserId") %>'>
                            <asp:ListItem Value="Active">Active</asp:ListItem>
                            <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        // Color dropdown based on selected value
        function colorStatusDropdown(ddl) {
            if (ddl.value === 'Active') {
                ddl.className = 'status-dropdown status-active';
            } else {
                ddl.className = 'status-dropdown status-inactive';
            }
        }

        // Apply colors on page load
        window.onload = function () {
            var dropdowns = document.querySelectorAll('.status-dropdown');
            dropdowns.forEach(function (ddl) {
                colorStatusDropdown(ddl);
                ddl.addEventListener('change', function () {
                    colorStatusDropdown(this);
                });
            });
        };
    </script>
    </asp:Content>
