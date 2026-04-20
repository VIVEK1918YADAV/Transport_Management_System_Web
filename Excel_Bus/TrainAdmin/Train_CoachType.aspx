<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="Train_CoachType.aspx.cs" Inherits="Excel_Bus.TrainAdmin.Train_CoachType" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f6fa; }
        .page-container {
            max-width: 1200px;
    margin: 20px auto;
    padding: 21px 20px;
    background-color: #dff3dc;
        }

        .page-header {
            display: flex; justify-content: space-between; align-items: center;
            margin-bottom: 30px; padding-bottom: 15px; border-bottom: 2px solid #e0e0e0;
        }
        .page-header h1 { font-size: 28px; color: #47a17b; font-weight: 600; }

        .btn-add {
            background: linear-gradient(135deg, #043e1d 0%, #05440a 100%);
            color: white; padding: 12px 24px; border: none; border-radius: 8px;
            cursor: pointer; font-size: 14px; font-weight: 600;
            display: inline-flex; align-items: center; gap: 8px; transition: all 0.3s ease;
        }
        .btn-add:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102,126,234,0.4); }

        .alert-panel {
            padding: 15px 20px; border-radius: 8px; margin-bottom: 20px;
            display: none; animation: slideDown 0.3s ease;
        }
        .alert-panel.show { display: block; }
        .alert-error { background-color: #fee; border-left: 4px solid #dc3545; color: #721c24; }
        .alert-success { background-color: #d4edda; border-left: 4px solid #28a745; color: #155724; }

        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .table-container {
            background: white; border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1); overflow: hidden;
        }
        .table { width: 100%; border-collapse: collapse; }
        .table thead { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; }
        .table thead th { padding: 16px; text-align: left; font-weight: 600; font-size: 14px; text-transform: uppercase; letter-spacing: 0.5px; }
        .table tbody td { padding: 16px; border-bottom: 1px solid #f0f0f0; color: #333; font-size: 14px; }
        .table tbody tr:hover { background-color: #f8f9fa; }
        .table tbody tr:last-child td { border-bottom: none; }

        .btn-remove {
            background: #dc3545; color: white; padding: 8px 16px; border: none;
            border-radius: 6px; cursor: pointer; font-size: 13px; transition: all 0.3s ease;
            display: inline-flex; align-items: center; gap: 6px;
        }
        .btn-remove:hover { background: #c82333; transform: translateY(-1px); }

        .empty-state { text-align: center; padding: 60px 20px; color: #999; font-size: 16px; }

        .modal-overlay {
            display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(0,0,0,0.6); z-index: 9999;
            align-items: center; justify-content: center; backdrop-filter: blur(3px);
        }
        .modal-overlay.show { display: flex; }

        .modal-container {
            background: #fff; border-radius: 12px; width: 90%; max-width: 500px;
            max-height: 90vh; overflow-y: auto;
            animation: modalSlideIn 0.3s ease; box-shadow: 0 10px 40px rgba(0,0,0,0.3);
        }
        @keyframes modalSlideIn {
            from { opacity: 0; transform: scale(0.9) translateY(-20px); }
            to { opacity: 1; transform: scale(1) translateY(0); }
        }

        .modal-header {
            padding: 20px 24px; border-bottom: 2px solid #e0e0e0;
            display: flex; justify-content: space-between; align-items: center;
            background: #f8f9fa; border-radius: 12px 12px 0 0;
        }
        .modal-header h2 { font-size: 20px; color: #333; font-weight: 600; margin: 0; }

        .close-btn {
            background: #fff; border: 2px solid #e0e0e0; font-size: 24px; color: #666;
            cursor: pointer; width: 36px; height: 36px;
            display: flex; align-items: center; justify-content: center;
            border-radius: 50%; transition: all 0.2s ease; line-height: 1;
        }
        .close-btn:hover { background: #dc3545; border-color: #dc3545; color: #fff; transform: rotate(90deg); }

        .modal-body { padding: 24px; }

        .form-group { margin-bottom: 20px; }
        .form-label { display: block; margin-bottom: 8px; font-weight: 600; color: #333; font-size: 14px; }
        .required { color: #dc3545; margin-left: 2px; }

        .form-control {
            width: 100%; padding: 12px 16px; border: 2px solid #e0e0e0;
            border-radius: 8px; font-size: 14px; transition: all 0.3s ease;
        }
        .form-control:focus { outline: none; border-color: #667eea; box-shadow: 0 0 0 4px rgba(102,126,234,0.1); }

        .modal-footer {
            display: flex; gap: 12px; padding-top: 20px;
            margin-top: 20px; border-top: 2px solid #e0e0e0;
        }

        .btn-submit {
            flex: 1;
            background: linear-gradient(135deg, #083e03 0%, #0f3e03 100%);
            color: white; padding: 14px 24px; border: none; border-radius: 8px;
            cursor: pointer; font-size: 15px; font-weight: 600; transition: all 0.3s ease;
        }
        .btn-submit:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(102,126,234,0.4); }

        .btn-cancel {
            flex: 1; background: #6c757d; color: white; padding: 14px 24px;
            border: none; border-radius: 8px; cursor: pointer;
            font-size: 15px; font-weight: 600; transition: all 0.3s ease;
        }
        .btn-cancel:hover { background: #5a6268; transform: translateY(-1px); }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-container">

        <!-- Alert Panels -->
        <asp:Panel ID="pnlError" runat="server" CssClass="alert-panel alert-error" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert-panel alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <h1>Coach Types</h1>
            <button type="button" class="btn-add" onclick="openAddModal()">
                + Add New
            </button>
        </div>

        <!-- Table -->
        <div class="table-container">
            <asp:GridView ID="gvSeatLayouts" runat="server" CssClass="table"
                AutoGenerateColumns="false" OnRowCommand="gvSeatLayouts_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="CoachType" HeaderText="Coach Type" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Remove"
                                CommandArgument='<%# Eval("CoachTypeId") %>'
                                CssClass="btn-remove"
                                OnClientClick="return confirm('Are you sure you want to delete this coach type?');">
                                🗑️ Remove
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="empty-state">No coach types found</div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal-overlay" id="modalOverlay">
        <div class="modal-container">
            <div class="modal-header">
                <h2 id="modalTitle">Add New Coach Type</h2>
                <button type="button" class="close-btn" onclick="closeModal()">×</button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hdnTypeId" runat="server" Value="0" />

                <div class="form-group">
                    <label class="form-label">Coach Type <span class="required">*</span></label>
                    <asp:TextBox ID="txtCoachType" runat="server" CssClass="form-control"
                        placeholder="e.g. Luxury, First Class, Sleeper" MaxLength="50" />
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Save Coach Type"
                        CssClass="btn-submit" OnClick="btnSubmit_Click" />
                    <button type="button" class="btn-cancel" onclick="closeModal()">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function openAddModal() {
            document.getElementById('modalTitle').innerText = 'Add New Coach Type';
            document.getElementById('<%= txtCoachType.ClientID %>').value = '';
            document.getElementById('<%= hdnTypeId.ClientID %>').value = '0';
            document.getElementById('modalOverlay').classList.add('show');
            document.body.style.overflow = 'hidden';
        }

        function closeModal() {
            document.getElementById('modalOverlay').classList.remove('show');
            document.body.style.overflow = '';
        }

        // Close modal on outside click
        window.onclick = function (event) {
            var modal = document.getElementById('modalOverlay');
            if (event.target === modal) closeModal();
        }

        // Prevent form resubmission on refresh
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>