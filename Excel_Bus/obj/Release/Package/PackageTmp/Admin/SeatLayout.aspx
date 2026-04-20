<%@ Page Title="Seat Layouts" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SeatLayout.aspx.cs" Inherits="Excel_Bus.SeatLayout" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .seat-layout-container {
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

        .table-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            border: 1px solid #dee2e6;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .custom-table {
            width: 100%;
            border-collapse: collapse;
        }

        .custom-table thead {
            background: #f8f9fa;
        }

        .custom-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #2c3e50;
            border-bottom: 2px solid #dee2e6;
            border-right: 1px solid #dee2e6;
        }

        .custom-table th:last-child {
            border-right: none;
        }

        .custom-table td {
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            border-right: 1px solid #dee2e6;
            color: #495057;
        }

        .custom-table td:last-child {
            border-right: none;
        }

        .custom-table tbody tr:hover {
            background: #f8f9fa;
        }

        .custom-table tbody tr:last-child td {
            border-bottom: none;
        }

        /* Action Buttons Container */
        .action-buttons {
            display: flex;
            gap: 8px;
            align-items: center;
        }

        /* Edit Button */
        .btn-edit {
            padding: 6px 12px;
            background: white;
            color: #7367f0;
            border: 1px solid #7367f0;
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
            background: #7367f0;
            color: white;
        }

        /* Remove Button */
        .btn-remove {
            padding: 6px 12px;
            background: white;
            color: #ea5455;
            border: 1px solid #ea5455;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            white-space: nowrap;
            min-width: 80px;
            height: 32px;
        }

        .btn-remove:hover {
            background: #ea5455;
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
            max-width: 500px;
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
            border-color: #7367f0;
            box-shadow: 0 0 0 0.2rem rgba(115, 103, 240, 0.25);
        }

        .form-hint {
            margin-top: 8px;
            font-size: 12px;
            color: #6c757d;
            display: flex;
            align-items: center;
            gap: 5px;
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

        .success-panel {
            background: #d1f4e0;
            border: 1px solid #a3e4c0;
            color: #00a854;
            padding: 12px 20px;
            border-radius: 4px;
            margin-bottom: 20px;
            opacity: 0;
            transition: opacity 0.3s ease-out;
        }

        .success-panel.show {
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

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 15px;
            }

            .action-buttons {
                flex-direction: column;
                width: 100%;
            }

            .btn-edit, .btn-remove {
                width: 100%;
                justify-content: center;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="seat-layout-container">
        <!-- Alert Messages -->
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-panel" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <h1 class="page-title">Seat Layouts</h1>
            <button type="button" class="btn-add-new" onclick="openAddModal()">
                + Add New
            </button>
        </div>

        <!-- Seat Layouts Table -->
        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="gvSeatLayouts" runat="server" 
                    CssClass="custom-table" 
                    AutoGenerateColumns="False"
                    OnRowCommand="gvSeatLayouts_RowCommand"
                    GridLines="None"
                    DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField HeaderText="S.N.">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="CoachLayout" HeaderText="Coach Layout" />

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <button type="button" class="btn-edit" 
                                        onclick='openEditModal(<%# Eval("Id") %>, "<%# Eval("CoachLayout") %>"); return false;'>
                                        ✏️ Edit
                                    </button>
                                    <asp:LinkButton ID="btnRemove" runat="server" 
                                        CssClass="btn-remove"
                                        CommandName="Remove"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Are you sure you want to remove this layout?');">
                                        🗑️ Remove
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No seat layouts found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add/Edit Modal -->
    <div id="modalOverlay" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modalTitle">Add New Layout</h5>
                <button type="button" class="btn-close" onclick="closeModal(); return false;">&times;</button>
            </div>
            
            <div class="modal-body">
                <div class="form-group">
                    <label class="form-label">Layout <span class="required">*</span></label>
                    <asp:TextBox ID="txtLayout" runat="server" CssClass="form-control" 
                        placeholder="Eg: 2 x 3" MaxLength="5"></asp:TextBox>
                    <asp:HiddenField ID="hdnLayoutId" runat="server" Value="0" />
                    <div class="form-hint">
                        ℹ️ Just type left and right value, a separator (x) will be added automatically
                    </div>
                </div>
            </div>

            <div class="modal-footer">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                    CssClass="btn-submit" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function openAddModal() {
            document.getElementById('modalTitle').innerText = 'Add New Layout';
            document.getElementById('<%= txtLayout.ClientID %>').value = '';
            document.getElementById('<%= hdnLayoutId.ClientID %>').value = '0';
            document.getElementById('modalOverlay').classList.add('show');
            document.body.style.overflow = 'hidden';
        }

        function openEditModal(id, CoachLayout) {
            document.getElementById('modalTitle').innerText = 'Edit Layout';
            document.getElementById('<%= txtLayout.ClientID %>').value = CoachLayout;
            document.getElementById('<%= hdnLayoutId.ClientID %>').value = id;
            document.getElementById('modalOverlay').classList.add('show');
            document.body.style.overflow = 'hidden';
            return false;
        }

        function closeModal() {
            document.getElementById('modalOverlay').classList.remove('show');
            document.body.style.overflow = '';
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

        // Auto-format layout input
        document.addEventListener('DOMContentLoaded', function() {
            var layoutInput = document.getElementById('<%= txtLayout.ClientID %>');

            if (layoutInput) {
                layoutInput.addEventListener('keypress', function (e) {
                    var value = this.value;

                    // Allow only numbers and prevent if length exceeds
                    if (!/\d/.test(e.key) || value.length >= 5) {
                        e.preventDefault();
                        return;
                    }

                    // Auto-add separator after first digit
                    if (value.length === 1 && !value.includes(' x ')) {
                        this.value = value + ' x ';
                    }
                });

                layoutInput.addEventListener('keyup', function (e) {
                    var key = e.keyCode || e.charCode;
                    // Remove separator on backspace/delete
                    if (key === 8 || key === 46) {
                        this.value = this.value.replace(' x ', '');
                    }
                });
            }
        });

        // Close modal on outside click
        window.onclick = function (event) {
            var modal = document.getElementById('modalOverlay');
            if (event.target == modal) {
                closeModal();
            }
        }

        // Prevent form resubmission on page refresh
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>