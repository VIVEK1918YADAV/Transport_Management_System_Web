<%@ Page Title="All Assigned Vehicles" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="VehicleAssign.aspx.cs" Inherits="Excel_Bus.VehicleAssign" Async="true" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .vehicle-assign-page {
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

        .vehicle-table {
            width: 100%;
            border-collapse: collapse;
        }

            .vehicle-table thead {
                background: #f8f9fa;
            }

            .vehicle-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
            }

            .vehicle-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .vehicle-table tbody tr:hover {
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
            color: #7367f0;
            border: 1px solid #7367f0;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-disable {
            padding: 6px 12px;
            background: white;
            color: #ea5455;
            border: 1px solid #ea5455;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-block;
        }

        .btn-edit:hover {
            background: #7367f0;
            color: white;
        }

        .btn-disable:hover {
            background: #ea5455;
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
                border-color: #7367f0;
                box-shadow: 0 0 0 0.2rem rgba(115, 103, 240, 0.25);
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
    <div class="vehicle-assign-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">All Assigned Vehicles</h1>
            <div class="header-actions">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by trip or vehicle..." />
                    <asp:Button ID="btnSearch" runat="server" Text="🔍" CssClass="btn-search" OnClick="btnSearch_Click" />
                </div>
                <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new"
                    OnClick="btnAddNew_Click" CausesValidation="false" />
            </div>
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:UpdatePanel ID="upGridView" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvAssignedVehicles" runat="server" CssClass="vehicle-table" AutoGenerateColumns="false"
                            OnRowCommand="gvAssignedVehicles_RowCommand" GridLines="None" DataKeyNames="Id">
                            <Columns>
                                <asp:TemplateField HeaderText="Trip">
                                    <ItemTemplate>
                                        <%# Eval("TripTitle") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Vehicle's Nick Name">
                                    <ItemTemplate>
                                        <%# Eval("VehicleNickName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Reg. No.">
                                    <ItemTemplate>
                                        <%# Eval("RegisterNo") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

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
                                                CommandName="EditRecord" CommandArgument='<%# Eval("Id") %>'
                                                CausesValidation="false">
                                                ✏️ Edit
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnDisable" runat="server"
                                                CssClass='<%# Convert.ToInt32(Eval("Status")) == 1 ? "btn-disable" : "btn-enable" %>'
                                                CommandName="DisableRecord"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClientClick="return confirm('Are you sure you want to change the status?');"
                                                CausesValidation="false">
                                                <%# Convert.ToInt32(Eval("Status")) == 1 ? "🚫 Disable" : "✓ Enable" %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="no-data">
                                    <p>No assigned vehicles found</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div id="vehicleModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalTitle">Assign Trip Vehicle</h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <asp:UpdatePanel ID="upModal" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-body">
                            <asp:HiddenField ID="hdnEditId" runat="server" Value="0" />

                            <!-- Add Mode Fields -->
                            <div id="addFields">
                                <div class="form-group">
                                    <label class="form-label">Trip <span class="required">*</span></label>
                                    <asp:DropDownList ID="ddlTripAdd" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlTripAdd_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTripAdd" runat="server"
                                        ControlToValidate="ddlTripAdd"
                                        InitialValue=""
                                        ErrorMessage="Please select a trip"
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        ValidationGroup="AddValidation" />
                                </div>

                                <div class="form-group">
                                    <label class="form-label">Vehicle <span class="required">*</span></label>
                                    <asp:DropDownList ID="ddlVehicleAdd" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvVehicleAdd" runat="server"
                                        ControlToValidate="ddlVehicleAdd"
                                        InitialValue=""
                                        ErrorMessage="Please select a vehicle"
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        ValidationGroup="AddValidation" />
                                </div>
                            </div>

                            <!-- Edit Mode Fields -->
                            <div id="editFields" style="display: none;">
                                <div class="form-group">
                                    <label class="form-label">Trip <span class="required">*</span></label>
                                    <asp:DropDownList ID="ddlTripEdit" runat="server" CssClass="form-control"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlTripEdit_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTripEdit" runat="server"
                                        ControlToValidate="ddlTripEdit"
                                        InitialValue=""
                                        ErrorMessage="Please select a trip"
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        ValidationGroup="EditValidation"
                                        Enabled="false" />
                                </div>

                                <div class="form-group">
                                    <label class="form-label">Vehicle <span class="required">*</span></label>
                                    <asp:DropDownList ID="ddlVehicleEdit" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvVehicleEdit" runat="server"
                                        ControlToValidate="ddlVehicleEdit"
                                        InitialValue=""
                                        ErrorMessage="Please select a vehicle"
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        ValidationGroup="EditValidation"
                                        Enabled="false" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitAdd" runat="server" Text="Submit" CssClass="btn-submit"
                                OnClick="btnSubmitAdd_Click" ValidationGroup="AddValidation" />
                            <asp:Button ID="btnSubmitEdit" runat="server" Text="Submit" CssClass="btn-submit"
                                OnClick="btnSubmitEdit_Click" ValidationGroup="EditValidation" Style="display: none;" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        var modalOpenFlag = false;

        function showAddModal() {
            hideModal();
            modalOpenFlag = true;

            setTimeout(function () {
                var modal = document.getElementById('vehicleModal');
                if (modal && modalOpenFlag) {
                    var modalTitle = document.getElementById('modalTitle');
                    if (modalTitle) {
                        modalTitle.textContent = 'Assign Trip Vehicle';
                    }

                    var addFields = document.getElementById('addFields');
                    var editFields = document.getElementById('editFields');
                    if (addFields) addFields.style.display = 'block';
                    if (editFields) editFields.style.display = 'none';

                    var btnSubmitAdd = document.getElementById('<%= btnSubmitAdd.ClientID %>');
                    var btnSubmitEdit = document.getElementById('<%= btnSubmitEdit.ClientID %>');
                    if (btnSubmitAdd) btnSubmitAdd.style.display = 'block';
                    if (btnSubmitEdit) btnSubmitEdit.style.display = 'none';

                    modal.classList.add('show');
                    document.body.style.overflow = 'hidden';
                }
            }, 50);
            return false;
        }

        function showEditModal() {
            hideModal();
            modalOpenFlag = true;

            setTimeout(function () {
                var modal = document.getElementById('vehicleModal');
                if (modal && modalOpenFlag) {
                    var modalTitle = document.getElementById('modalTitle');
                    if (modalTitle) {
                        modalTitle.textContent = 'Update Trip Assigned Vehicle';
                    }

                    var addFields = document.getElementById('addFields');
                    var editFields = document.getElementById('editFields');
                    if (addFields) addFields.style.display = 'none';
                    if (editFields) editFields.style.display = 'block';

                    var btnSubmitAdd = document.getElementById('<%= btnSubmitAdd.ClientID %>');
                var btnSubmitEdit = document.getElementById('<%= btnSubmitEdit.ClientID %>');
                    if (btnSubmitAdd) btnSubmitAdd.style.display = 'none';
                    if (btnSubmitEdit) btnSubmitEdit.style.display = 'block';

                    modal.classList.add('show');
                    document.body.style.overflow = 'hidden';
                }
            }, 50);
            return false;
        }

        function hideModal() {
            modalOpenFlag = false;
            var modal = document.getElementById('vehicleModal');
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
            return false;
        }

        function showSuccess(message) {
            // Ensure modal is closed
            hideModal();

            var existingMessages = document.querySelectorAll('.success-message');
            existingMessages.forEach(function (msg) {
                msg.remove();
            });

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

        window.onclick = function (event) {
            var modal = document.getElementById('vehicleModal');
            if (event.target == modal) {
                hideModal();
            }
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        window.showAddModal = showAddModal;
        window.showEditModal = showEditModal;
        window.hideModal = hideModal;
        window.showSuccess = showSuccess;
    </script>
</asp:Content>
