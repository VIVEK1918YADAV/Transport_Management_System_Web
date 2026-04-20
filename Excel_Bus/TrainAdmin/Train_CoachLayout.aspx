<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_CoachLayout.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_CoachLayout" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        * { 
            margin: 0; 
            padding: 0; 
            box-sizing: border-box; 
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #f5f6fa;
        }
        
        .page-container {
            max-width: 1200px;
            margin: 20px auto;
            padding: 0 20px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            padding-bottom: 15px;
            border-bottom: 2px solid #e0e0e0;
        }

        .page-header h1 {
            font-size: 28px;
            color: #333;
            font-weight: 600;
        }

        .btn-add {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px 24px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
            text-decoration: none;
        }

        .btn-add:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
        }

        /* Alert Panels */
        .alert-panel {
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: none;
            animation: slideDown 0.3s ease;
        }

        .alert-panel.show { 
            display: block; 
        }

        .alert-error {
            background-color: #fee;
            border-left: 4px solid #dc3545;
            color: #721c24;
        }

        .alert-success {
            background-color: #d4edda;
            border-left: 4px solid #28a745;
            color: #155724;
        }

        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Table Styles */
        .table-container {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .table {
            width: 100%;
            border-collapse: collapse;
        }

        .table thead {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        .table thead th {
            padding: 16px;
            text-align: left;
            font-weight: 600;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .table tbody td {
            padding: 16px;
            border-bottom: 1px solid #f0f0f0;
            color: #333;
            font-size: 14px;
        }

        .table tbody tr:hover {
            background-color: #f8f9fa;
        }

        .table tbody tr:last-child td {
            border-bottom: none;
        }

        .btn-remove {
            background: #dc3545;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 13px;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .btn-remove:hover {
            background: #c82333;
            transform: translateY(-1px);
        }

        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #999;
            font-size: 16px;
        }

        /* Modal Styles */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.6);
            z-index: 9999;
            align-items: center;
            justify-content: center;
            backdrop-filter: blur(3px);
        }

        .modal-overlay.show {
            display: flex;
        }

        .modal-container {
            background: #ffffff;
            border-radius: 12px;
            width: 90%;
            max-width: 500px;
            max-height: 90vh;
            overflow-y: auto;
            animation: modalSlideIn 0.3s ease;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
            position: relative;
        }

        @keyframes modalSlideIn {
            from { 
                opacity: 0; 
                transform: scale(0.9) translateY(-20px); 
            }
            to { 
                opacity: 1; 
                transform: scale(1) translateY(0); 
            }
        }

        .modal-header {
            padding: 20px 24px;
            border-bottom: 2px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: #f8f9fa;
            border-radius: 12px 12px 0 0;
        }

        .modal-header h2 {
            font-size: 20px;
            color: #333;
            font-weight: 600;
            margin: 0;
        }

        .close-btn {
            background: #ffffff;
            border: 2px solid #e0e0e0;
            font-size: 24px;
            color: #666;
            cursor: pointer;
            padding: 0;
            width: 36px;
            height: 36px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: all 0.2s ease;
            line-height: 1;
        }

        .close-btn:hover {
            background: #dc3545;
            border-color: #dc3545;
            color: #ffffff;
            transform: rotate(90deg);
        }

        .modal-body {
            padding: 24px;
            background: #ffffff;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #333;
            font-size: 14px;
        }

        .required {
            color: #dc3545;
            margin-left: 2px;
        }

        .form-control {
            width: 100%;
            padding: 12px 16px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            transition: all 0.3s ease;
            background: #ffffff;
            color: #333;
        }

        .form-control:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
            background: #ffffff;
        }

        .form-help {
            display: block;
            margin-top: 6px;
            font-size: 12px;
            color: #666;
            font-style: italic;
        }

        .modal-footer {
            display: flex;
            gap: 12px;
            padding-top: 20px;
            margin-top: 20px;
            border-top: 2px solid #e0e0e0;
        }

        .btn-submit {
            flex: 1;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 14px 24px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 15px;
            font-weight: 600;
            transition: all 0.3s ease;
        }

        .btn-submit:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
        }

        .btn-submit:active {
            transform: translateY(0);
        }

        .btn-cancel {
            flex: 1;
            background: #6c757d;
            color: white;
            padding: 14px 24px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 15px;
            font-weight: 600;
            transition: all 0.3s ease;
        }

        .btn-cancel:hover {
            background: #5a6268;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(108, 117, 125, 0.3);
        }

        .btn-cancel:active {
            transform: translateY(0);
        }

        /* Scrollbar for modal */
        .modal-container::-webkit-scrollbar {
            width: 8px;
        }

        .modal-container::-webkit-scrollbar-track {
            background: #f1f1f1;
            border-radius: 0 12px 12px 0;
        }

        .modal-container::-webkit-scrollbar-thumb {
            background: #888;
            border-radius: 4px;
        }

        .modal-container::-webkit-scrollbar-thumb:hover {
            background: #555;
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
               <%-- <asp:GridView ID="gvSeatLayouts" runat="server"
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

                        <asp:BoundField DataField="CoachType" HeaderText="Coach Type" />
                        <asp:BoundField DataField="CoachLayout" HeaderText="Coach Layout" />

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <button type="button" class="btn-edit"
                                        onclick='openEditModal(<%# Eval("Id") %>, "<%# Eval("coachType") %>", "<%# Eval("coachLayout") %>"); return false;'>
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
                </asp:GridView>--%>
                <asp:GridView ID="gvSeatLayouts" runat="server" CssClass="table" AutoGenerateColumns="false" OnRowCommand="gvSeatLayouts_RowCommand">
    <Columns>
        <asp:TemplateField HeaderText="S.No">
            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Coach Type">
            <ItemTemplate>
                <%# Eval("CoachType") %>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField DataField="CoachLayout" HeaderText="Layout" />
        
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Eval("Id") %>' 
                    CssClass="btn-remove" OnClientClick="return confirm('Are you sure you want to delete this layout?');">
                    🗑️ Remove
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <div class="empty-state">No seat layouts found</div>
    </EmptyDataTemplate>
</asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add/Edit Modal -->
 <div class="modal-overlay" id="modalOverlay">
    <div class="modal-container">
        <div class="modal-header">
            <h2 id="modalTitle">Add New Layout</h2>
            <button type="button" class="close-btn" onclick="closeModal()">×</button>
        </div>
        <div class="modal-body">
            <asp:HiddenField ID="hdnLayoutId" runat="server" Value="0" />
            
            <div class="form-group">
                <label class="form-label">Coach Layout <span class="required">*</span></label>
                <asp:DropDownList
                    ID="ddlCoachType"
                    runat="server"
                    CssClass="form-control">
                </asp:DropDownList>
                <small class="form-help">ℹ️ Select the type of coach.</small>
            </div>

            <div class="form-group">
                <label class="form-label">Layout <span class="required">*</span></label>
                <asp:TextBox
                    ID="txtLayout"
                    runat="server"
                    CssClass="form-control"
                    placeholder="e.g., 23 (will become 2 x 3)"
                    MaxLength="10" />
                <small class="form-help">ℹ️ Just type left and right value, a separator (x) will be added automatically</small>
            </div>

            <div class="modal-footer">
                <asp:Button
                    ID="btnSubmit"
                    runat="server"
                    Text="Save Layout"
                    CssClass="btn-submit"
                    OnClick="btnSubmit_Click" />
                <button type="button" class="btn-cancel" onclick="closeModal()">Cancel</button>
            </div>
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
   
       <%-- function openAddModal() {
            document.getElementById('modalTitle').innerText = 'Add New Layout';
            document.getElementById('<%= txtLayout.ClientID %>').value = '';
            document.getElementById('<%= ddlCoachType.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= hdnLayoutId.ClientID %>').value = '0';
            document.getElementById('modalOverlay').classList.add('show');
            document.body.style.overflow = 'hidden';
        }--%>
     <script>
        function openAddModal() {
            document.getElementById('modalTitle').innerText = 'Add New Layout';
            document.getElementById('<%= txtLayout.ClientID %>').value = '';
    document.getElementById('<%= ddlCoachType.ClientID %>').selectedIndex = 0;  // Reset dropdown to the default value
            document.getElementById('<%= hdnLayoutId.ClientID %>').value = '0';
            document.getElementById('modalOverlay').classList.add('show');
            document.body.style.overflow = 'hidden';
        }


        function openEditModal(id, layout) {
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
        document.addEventListener('DOMContentLoaded', function () {
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
