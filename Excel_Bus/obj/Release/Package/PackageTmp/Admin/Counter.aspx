<%@ Page Title="All Counter" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Counter.aspx.cs" Inherits="Excel_Bus.Counter" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .counter-page {
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

        .counter-table {
            width: 100%;
            border-collapse: collapse;
        }

            .counter-table thead {
                background: #fffbcc;
            }

            .counter-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
            }

            .counter-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .counter-table tbody tr:hover {
                background: #f8f9fa;
            }

        .status-badge {
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            display: inline-block;
        }

        .status-enabled {
            background: #d1f4e0;
            color: #00a854;
        }

        .status-disabled {
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

        .btn-enable {
            padding: 6px 12px;
            background: white;
            color: #28c76f;
            border: 1px solid #28c76f;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-enable:hover {
            background: #28c76f;
            color: white;
        }

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
            max-width: 500px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            animation: slideDown 0.3s ease-out;
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
            max-height: 60vh;
            overflow-y: auto;
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

        .modal-footer {
            padding: 15px 20px;
            border-top: 1px solid #dee2e6;
        }

        .btn-submit {
            width: 100%;
            padding: 12px;
            background: #0d6efd;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
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
            opacity: 0;
            transition: opacity 0.3s ease-out;
        }

            .error-panel.show {
                opacity: 1;
            }

        .no-data {
            text-align: center;
            padding: 40px;
            color: #6c757d;
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
            transition: opacity 0.3s ease-out;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="counter-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">All Counter</h1>
            <div class="header-actions">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by name..." />
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new" OnClientClick="openAddNewModal(); return false;" />
            </div>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvCounters" runat="server" CssClass="counter-table" AutoGenerateColumns="false"
                    OnRowCommand="gvCounters_RowCommand" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Mobile" HeaderText="Mobile Number" />
                        <asp:BoundField DataField="City" HeaderText="City" />
                        <asp:BoundField DataField="Location" HeaderText="Location" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='status-badge <%# GetStatusClass(Eval("Status")) %>'>
                                    <%# GetStatusBadge(Eval("Status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-edit"
                                        CommandName="EditCounter" CommandArgument='<%# Eval("Id") %>'>
                                        ✏️ Edit
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDisable" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "1" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="DisableCounter"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status of this counter?');">
    <%# Eval("Status").ToString() == "1" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No counters found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="counterModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Counter"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnCounterId" runat="server" Value="0" />
                    <div class="form-group">
                        <label class="form-label">Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server"
                            ControlToValidate="txtName"
                            ErrorMessage="Name is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="CounterValidation" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">City <span class="required">*</span></label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvCity" runat="server"
                            ControlToValidate="txtCity"
                            ErrorMessage="City is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="CounterValidation" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Location</label>
                        <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Mobile <span class="required">*</span></label>
                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvMobile" runat="server"
                            ControlToValidate="txtMobile"
                            ErrorMessage="Mobile is required"
                            ForeColor="Red"
                            Display="Dynamic"
                            ValidationGroup="CounterValidation" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit"
                        OnClick="btnSubmit_Click" ValidationGroup="CounterValidation" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        // Function to open Add New modal with cleared form
        function openAddNewModal() {
            clearForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Counter';
            showModal();
            return false;
        }

        // Function to clear form
        function clearForm() {
            document.getElementById('<%= hdnCounterId.ClientID %>').value = '0';
            document.getElementById('<%= txtName.ClientID %>').value = '';
            document.getElementById('<%= txtCity.ClientID %>').value = '';
            document.getElementById('<%= txtLocation.ClientID %>').value = '';
            document.getElementById('<%= txtMobile.ClientID %>').value = '';
        }

        function showModal() {
            var modal = document.getElementById('counterModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('counterModal');
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
                }, 300);
            }, 3000);
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var modal = document.getElementById('counterModal');
            if (event.target == modal) {
                hideModal();
            }
        }

        // Prevent form resubmission on page refresh
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>
