<%@ Page Title="Ticket Price" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TicketPrice.aspx.cs" Inherits="Excel_Bus.TicketPrice" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .ticket-price-container {
            padding: 20px;
            background-color: #f5f5f5;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            margin-top: 50px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            margin: 0;
            margin-top: 50px;
        }

        .btn-add-new {
            padding: 10px 20px;
            background: #28c76f;
            color: white;
            border: 1px solid #28c76f;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 5px;
            white-space: nowrap;
        }

            .btn-add-new:hover {
                background: #24b263;
            }

        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .price-table {
            width: 100%;
            border-collapse: collapse;
        }

            .price-table thead {
                background-color: #fef5e7;
            }

            .price-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #333;
                border-bottom: 2px solid #e0e0e0;
            }

            .price-table td {
                padding: 15px;
                border-bottom: 1px solid #f0f0f0;
            }

            .price-table tbody tr:hover {
                background-color: #f9f9f9;
            }

        .btn-edit {
            padding: 6px 12px;
            background: white;
            color: #7c3aed;
            border: 1px solid #7c3aed;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            white-space: nowrap;
            min-width: 70px;
            height: 32px;
        }

            .btn-edit:hover {
                background: #7c3aed;
                color: white;
            }


        .btn-remove {
            background-color: white;
            color: #fc544b;
            border: 1px solid #fc544b;
            padding: 6px 15px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
        }

            .btn-remove:hover {
                background-color: #fc544b;
                color: white;
            }

        .form-panel {
            background: white;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .form-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .form-section-title {
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #333;
        }

        .form-group {
            margin-bottom: 20px;
        }

            .form-group label {
                display: block;
                margin-bottom: 8px;
                font-weight: 500;
                color: #555;
            }

                .form-group label .required {
                    color: #fc544b;
                }

        .form-control {
            width: 100%;
            padding: 10px 15px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            font-size: 14px;
        }

            .form-control:focus {
                outline: none;
                border-color: #6777ef;
            }

        .btn-submit {
            width: 100%;
            background-color: #6777ef;
            color: white;
            border: none;
            padding: 12px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
        }

            .btn-submit:hover {
                background-color: #4c63d2;
            }

        .btn-go-back {
            background-color: #6777ef;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
        }

            .btn-go-back:hover {
                background-color: #4c63d2;
            }

        .btn-back {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
        }

            .btn-back:hover {
                background-color: #5a6268;
            }

        .btn-update {
            background-color: #6777ef;
            color: white;
            border: none;
            padding: 10px 30px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            margin-left: 10px;
        }

            .btn-update:hover {
                background-color: #4c63d2;
            }

        .edit-price-group {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .currency-label {
            padding: 10px 15px;
            background-color: #f5f5f5;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            font-weight: 500;
        }

        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border-radius: 5px;
        }

        .alert-danger {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ticket-price-container">
        <!-- Error/Success Messages -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <!-- List Panel -->
        <asp:Panel ID="pnlList" runat="server" Visible="true">
            <div class="page-header">
                <h1 class="page-title">All Ticket Price</h1>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new" OnClick="btnAddNew_Click" />
            </div>

            <div class="card">
                <div class="table-responsive">
                    <asp:GridView ID="gvTicketPrices" runat="server" AutoGenerateColumns="False" CssClass="price-table"
                        DataKeyNames="Id" OnRowCommand="gvTicketPrices_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="FleetType" HeaderText="Fleet Type" />
                            <asp:BoundField DataField="VehicleRoute" HeaderText="Vehicle Route" />
                            <asp:BoundField DataField="Vehicle" HeaderText="Vehicle Name" />
                            <asp:TemplateField HeaderText="Price">
                                <ItemTemplate>
                                    <%# Excel_Bus.TicketPrice.FormatPrice(Eval("Price")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <div style="display: flex; gap: 10px; align-items: center;">
                                        <asp:Button ID="btnEdit" runat="server" Text="✏ Edit" CssClass="btn-edit"
                                            CommandName="EditPrice" CommandArgument="<%# Container.DataItemIndex %>" />
                                        <asp:Button ID="btnRemove" runat="server" Text="🗑 Remove" CssClass="btn-remove"
                                            CommandName="DeletePrice" CommandArgument="<%# Container.DataItemIndex %>"
                                            OnClientClick="return confirm('Are you sure you want to remove this price?');" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </asp:Panel>

        <!-- Add Panel -->
        <asp:Panel ID="pnlAdd" runat="server" Visible="false">
            <div class="form-panel">
                <div class="form-header">
                    <h1 class="page-title">Add Ticket Price</h1>
                    <asp:Button ID="btnGoBack" runat="server" Text="⬅ Go Back" CssClass="btn-go-back" OnClick="btnGoBack_Click" />
                </div>

                <h3 class="form-section-title">Information About Ticket Price</h3>

                <div class="form-group">
                    <label>Fleet Type <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlFleetType" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>

                 <div class="form-group">
                    <label>Vehicle Route <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlRoute" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Bus Name <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Price For Source To Destination <span class="required">*</span></label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="CDF" TextMode="Number" step="0.01"></asp:TextBox>
                </div>

                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit" OnClick="btnSubmit_Click" />
            </div>
        </asp:Panel>

        <!-- Edit Panel -->
        <asp:Panel ID="pnlEdit" runat="server" Visible="false">
            <div class="form-panel">
                <div class="form-header">
                    <h1 class="page-title">Update Ticket Price</h1>
                    <asp:Button ID="btnBack" runat="server" Text="⬅ Back" CssClass="btn-back" OnClick="btnBack_Click" />
                </div>

                <div class="form-group">
                    <label>
                        <asp:Label ID="lblEditRoute" runat="server"></asp:Label>
                        <span class="required">*</span></label>
                    <div class="edit-price-group">
                        <span class="currency-label">CDF</span>
                        <asp:TextBox ID="txtEditPrice" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn-update" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // Add any additional JavaScript if needed
    </script>
</asp:Content>
