<%@ Page Title="All Routes" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Route.aspx.cs" Inherits="Excel_Bus.Route" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .route-page {
            padding: 20px;
            background: #f5f7fa;
            margin-top: 50px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .header-actions {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }

        .search-box {
            display: flex;
            gap: 0;
        }

        .search-input {
            padding: 10px 15px;
            border: 1px solid #ddd;
            border-radius: 4px 0 0 4px;
            font-size: 14px;
            width: 250px;
        }

        .btn-search {
            padding: 10px 20px;
            background: #0d6efd;
            color: white;
            border: none;
            border-radius: 0 4px 4px 0;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-search:hover {
            background: #0b5ed7;
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

        .route-table {
            width: 100%;
            border-collapse: collapse;
        }

        .route-table thead {
            background: #fffbcc;
        }

        .route-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #2c3e50;
            border-bottom: 2px solid #dee2e6;
        }

        .route-table td {
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            color: #495057;
        }

        .route-table tbody tr:hover {
            background: #f8f9fa;
        }

        .status-badge {
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .badge-success {
            background: #d1f4e0;
            color: #00a854;
        }

        .badge-danger {
            background: #ffe0e0;
            color: #ff4444;
        }

        .action-buttons {
            display: flex;
            gap: 8px;
        }

        .btn-edit {
            padding: 6px 12px;
            background: white;
            color: #6c63ff;
            border: 1px solid #6c63ff;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-disable {
            padding: 6px 12px;
            background: white;
            color: #dc3545;
            border: 1px solid #dc3545;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-edit:hover {
            background: #6c63ff;
            color: white;
        }

        .btn-disable:hover {
            background: #dc3545;
            color: white;
        }

        /* Modal Styles */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 9999;
            justify-content: center;
            align-items: center;
        }

        .modal-overlay.show {
            display: flex !important;
        }

        .modal-content {
            background: white;
            border-radius: 8px;
            width: 90%;
            max-width: 600px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            animation: slideDown 0.3s ease-out;
            max-height: 90vh;
            overflow-y: auto;
        }

        @keyframes slideDown {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .modal-header {
            padding: 20px;
            border-bottom: 1px solid #dee2e6;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .modal-title {
            font-size: 18px;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .btn-close {
            background: none;
            border: none;
            font-size: 24px;
            cursor: pointer;
            color: #6c757d;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-close:hover {
            color: #000;
        }

        .modal-body {
            padding: 20px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #2c3e50;
        }

        .required {
            color: #dc3545;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
        }

        .form-control:focus {
            outline: none;
            border-color: #6c63ff;
            box-shadow: 0 0 0 0.2rem rgba(108, 99, 255, 0.25);
        }

        .row {
            display: flex;
            margin: 0 -10px;
        }

        .col-md-6 {
            flex: 0 0 50%;
            padding: 0 10px;
        }

        .form-text {
            display: block;
            margin-top: 5px;
            font-size: 12px;
            color: #6c757d;
        }

        .modal-footer {
            padding: 15px 20px;
            border-top: 1px solid #dee2e6;
            display: flex;
            gap: 10px;
            justify-content: flex-end;
        }

        .btn-secondary {
            padding: 10px 20px;
            background: #6c757d;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-secondary:hover {
            background: #5a6268;
        }

        .btn-submit {
            padding: 10px 20px;
            background: #0d6efd;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-submit:hover {
            background: #0b5ed7;
        }

        .error-panel {
            background: #f8d7da;
            border: 1px solid #f5c2c7;
            color: #842029;
            padding: 12px 20px;
            border-radius: 4px;
            margin-bottom: 20px;
        }

        .success-message {
            position: fixed;
            top: 20px;
            right: 20px;
            background: #d1f4e0;
            color: #00a854;
            padding: 15px 20px;
            border-radius: 4px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
            z-index: 10000;
            animation: slideInRight 0.3s ease-out;
        }

        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        .no-data {
            text-align: center;
            padding: 40px;
            color: #6c757d;
        }

        .text-danger {
            color: #dc3545;
            font-size: 13px;
            margin-top: 5px;
            display: block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="route-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">All Routes</h1>
            <div class="header-actions">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by name..." />
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new" 
                    OnClick="btnAddNew_Click" CausesValidation="false" />
            </div>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvRoutes" runat="server" CssClass="route-table" AutoGenerateColumns="false"
                    OnRowCommand="gvRoutes_RowCommand" OnRowDataBound="gvRoutes_RowDataBound" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:TemplateField HeaderText="Starting Point">
                            <ItemTemplate>
                                <asp:Literal ID="litStartFrom" runat="server" Text='<%# Eval("StartFrom") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ending Point">
                            <ItemTemplate>
                                <asp:Literal ID="litEndTo" runat="server" Text='<%# Eval("EndTo") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Distance" HeaderText="Distance" />
                        <%--<asp:BoundField DataField="Time" HeaderText="Time" />--%>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" CssClass="status-badge"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-edit"
                                        CommandName="EditRoute" CommandArgument='<%# Eval("Id") %>'
                                        CausesValidation="false">
                                        ✏️ Edit
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDisable" runat="server" CssClass="btn-disable"
                                        CommandName="DisableRoute" CommandArgument='<%# Eval("Id") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Are you sure you want to change the status of this route?');">
                                        🚫 Disable
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No routes found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Create Modal -->
        <div id="createModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Route</h5>
                    <button type="button" class="btn-close" onclick="hideCreateModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label class="form-label">Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter route name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                            ControlToValidate="txtName" 
                            ErrorMessage="Name is required" 
                            CssClass="text-danger"
                            ValidationGroup="CreateRoute"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Start From <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlStartFrom" runat="server" CssClass="form-control"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvStartFrom" runat="server" 
                                    ControlToValidate="ddlStartFrom" 
                                    ErrorMessage="Starting point is required" 
                                    CssClass="text-danger"
                                    ValidationGroup="CreateRoute"
                                    Display="Dynamic"
                                    InitialValue=""></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">End To <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlEndTo" runat="server" CssClass="form-control"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvEndTo" runat="server" 
                                    ControlToValidate="ddlEndTo" 
                                    ErrorMessage="Ending point is required" 
                                    CssClass="text-danger"
                                    ValidationGroup="CreateRoute"
                                    Display="Dynamic"
                                    InitialValue=""></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Time</label>
                                <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" placeholder="e.g., 2 hours"></asp:TextBox>
                                <small class="form-text">Keep space between value & unit</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Distance</label>
                                <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control" placeholder="e.g., 100 km"></asp:TextBox>
                                <small class="form-text">Keep space between value & unit</small>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-secondary" onclick="hideCreateModal(); return false;">Close</button>
                    <asp:Button ID="btnSubmitCreate" runat="server" CssClass="btn-submit" Text="Submit" 
                        OnClick="btnSubmitCreate_Click" ValidationGroup="CreateRoute" />
                </div>
            </div>
        </div>

        <!-- Edit Modal -->
        <div id="editModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update Route</h5>
                    <button type="button" class="btn-close" onclick="hideEditModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfEditRouteId" runat="server" />
                    <div class="form-group">
                        <label class="form-label">Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtEditName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEditName" runat="server" 
                            ControlToValidate="txtEditName" 
                            ErrorMessage="Name is required" 
                            CssClass="text-danger"
                            ValidationGroup="EditRoute"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Start From <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlEditStartFrom" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">End To <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlEditEndTo" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Time</label>
                                <asp:TextBox ID="txtEditTime" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Distance</label>
                                <asp:TextBox ID="txtEditDistance" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-secondary" onclick="hideEditModal(); return false;">Close</button>
                    <asp:Button ID="btnSubmitEdit" runat="server" CssClass="btn-submit" Text="Submit" 
                        OnClick="btnSubmitEdit_Click" ValidationGroup="EditRoute" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        function showCreateModal() {
            var modal = document.getElementById('createModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideCreateModal() {
            var modal = document.getElementById('createModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        function showEditModal() {
            var modal = document.getElementById('editModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideEditModal() {
            var modal = document.getElementById('editModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        function showSuccess(message) {
            var successDiv = document.createElement('div');
            successDiv.className = 'success-message';
            successDiv.textContent = message;
            document.body.appendChild(successDiv);

            setTimeout(function () {
                successDiv.style.opacity = '0';
                setTimeout(function () {
                    if (successDiv.parentNode) {
                        successDiv.parentNode.removeChild(successDiv);
                    }
                    location.reload();
                }, 300);
            }, 2000);
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var createModal = document.getElementById('createModal');
            var editModal = document.getElementById('editModal');
            if (event.target == createModal) {
                hideCreateModal();
            }
            if (event.target == editModal) {
                hideEditModal();
            }
        }

        // Prevent form resubmission on page refresh
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>
