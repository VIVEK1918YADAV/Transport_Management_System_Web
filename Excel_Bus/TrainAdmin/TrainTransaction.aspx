<%@ Page Title="" Language="C#" MasterPageFile="~/TrainAdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainTransaction.aspx.cs" Inherits="Excel_Bus.TrainAdmin.TrainTransaction" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=JetBrains+Mono:wght@500;600&display=swap');

        * { box-sizing: border-box; }
        body, .trx-page * { font-family: 'DM Sans', sans-serif; }

        .trx-page { padding: 24px; background: #f0f2f5; min-height: 100vh; }

        .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 12px; }
        .page-title  { font-size: 22px; font-weight: 700; color: #1a1d23; margin: 0; letter-spacing: -0.3px; }
        .page-subtitle { font-size: 13px; color: #8b8fa8; margin: 2px 0 0 0; }

        /* ── Stats ── */
        .stats-row { display: grid; grid-template-columns: repeat(auto-fit, minmax(190px, 1fr)); gap: 16px; margin-bottom: 24px; }
        .stat-card  { background: white; border-radius: 10px; padding: 18px 20px; box-shadow: 0 1px 3px rgba(0,0,0,.07); display: flex; align-items: center; gap: 14px; }
        .stat-icon  { width: 46px; height: 46px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 20px; flex-shrink: 0; }
        .stat-icon.blue   { background: #eff3ff; }
        .stat-icon.green  { background: #edfaf1; }
        .stat-icon.red    { background: #fff0f0; }
        .stat-icon.orange { background: #fff7ec; }
        .stat-value { font-size: 22px; font-weight: 700; color: #1a1d23; line-height: 1.1; }
        .stat-label { font-size: 12px; color: #8b8fa8; margin-top: 3px; }

        /* ── Filters ── */
        .filter-card { background: white; border-radius: 10px; padding: 16px 20px; box-shadow: 0 1px 3px rgba(0,0,0,.07); margin-bottom: 20px; display: flex; flex-wrap: wrap; gap: 12px; align-items: flex-end; }
        .filter-group { display: flex; flex-direction: column; gap: 5px; flex: 1; min-width: 150px; }
        .filter-label { font-size: 11px; font-weight: 600; color: #8b8fa8; text-transform: uppercase; letter-spacing: 0.5px; }
        .filter-control { padding: 8px 12px; border: 1.5px solid #e8eaf0; border-radius: 7px; font-size: 13px; color: #1a1d23; font-family: 'DM Sans', sans-serif; background: #fafbfc; width: 100%; }
        .filter-control:focus { outline: none; border-color: #5b7fff; background: white; }
        .btn-filter { padding: 9px 20px; background: #5b7fff; color: white; border: none; border-radius: 7px; font-size: 13px; font-weight: 600; cursor: pointer; font-family: 'DM Sans', sans-serif; white-space: nowrap; align-self: flex-end; }
        .btn-filter:hover { background: #4366e8; }
        .btn-reset  { padding: 9px 16px; background: white; color: #6b7280; border: 1.5px solid #e8eaf0; border-radius: 7px; font-size: 13px; cursor: pointer; font-family: 'DM Sans', sans-serif; white-space: nowrap; align-self: flex-end; }
        .btn-reset:hover { background: #f5f7fa; }

        /* ── Card / Table ── */
        .card { background: white; border-radius: 10px; box-shadow: 0 1px 3px rgba(0,0,0,.07); overflow: hidden; }
        .card-header { padding: 16px 20px; border-bottom: 1px solid #f0f2f5; display: flex; align-items: center; justify-content: space-between; }
        .card-title  { font-size: 15px; font-weight: 600; color: #1a1d23; margin: 0; }
        .record-count { font-size: 12px; color: #8b8fa8; background: #f0f2f5; padding: 3px 10px; border-radius: 20px; }
        .table-responsive { overflow-x: auto; }

        .trx-table { width: 100%; border-collapse: collapse; }
        .trx-table thead { background: #fafbfc; }
        .trx-table th { padding: 13px 16px; text-align: left; font-size: 11px; font-weight: 700; color: #8b8fa8; text-transform: uppercase; letter-spacing: 0.6px; border-bottom: 2px solid #f0f2f5; white-space: nowrap; }
        .trx-table td { padding: 13px 16px; border-bottom: 1px solid #f5f6f8; color: #374151; font-size: 13.5px; vertical-align: middle; }
        .trx-table tbody tr:hover { background: #fafbff; }
        .trx-table tbody tr:last-child td { border-bottom: none; }

        /* GridView pager row */
        .trx-table tr.pager-row td { padding: 12px 16px; border-top: 1px solid #f0f2f5; text-align: center; }
        .trx-table tr.pager-row a, .trx-table tr.pager-row span {
            display: inline-block; padding: 5px 12px; border: 1.5px solid #e8eaf0;
            border-radius: 6px; font-size: 12px; font-weight: 600; color: #374151;
            text-decoration: none; margin: 0 2px;
        }
        .trx-table tr.pager-row span { background: #5b7fff; color: white; border-color: #5b7fff; }

        /* Badges */
        .badge { display: inline-block; padding: 4px 11px; border-radius: 20px; font-size: 11.5px; font-weight: 600; white-space: nowrap; }
        .badge-success  { background: #edfaf1; color: #16a34a; }
        .badge-failed   { background: #fff0f0; color: #dc2626; }
        .badge-pending  { background: #fff7ec; color: #d97706; }
        .badge-payment  { background: #eff3ff; color: #4f46e5; }
        .badge-refund   { background: #fdf4ff; color: #9333ea; }
        .badge-postpone { background: #f0fdf4; color: #15803d; }
        .badge-default  { background: #f3f4f6; color: #6b7280; }

        .trxno-cell  { font-family: 'JetBrains Mono', monospace; font-size: 12px; color: #4f46e5; font-weight: 600; }
        .amount-cell { font-weight: 700; color: #1a1d23; font-size: 14px; }
        .date-cell   { font-size: 12.5px; color: #6b7280; }

        .btn-view { padding: 5px 13px; background: white; color: #5b7fff; border: 1.5px solid #5b7fff; border-radius: 6px; cursor: pointer; font-size: 12px; font-weight: 600; font-family: 'DM Sans', sans-serif; text-decoration: none; display: inline-flex; align-items: center; gap: 4px; }
        .btn-view:hover { background: #5b7fff; color: white; }

        .no-data { text-align: center; padding: 60px 20px; color: #8b8fa8; }
        .no-data-icon { font-size: 44px; margin-bottom: 10px; }

        /* Error */
        .error-panel { background: #fff0f0; border: 1px solid #fecaca; color: #dc2626; padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; font-size: 13px; }

        /* Toast */
        .success-toast { position: fixed; top: 20px; right: 20px; background: #edfaf1; color: #16a34a; padding: 14px 20px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,.12); z-index: 10000; font-size: 13px; font-weight: 600; animation: slideIn .3s ease; }
        @keyframes slideIn { from { transform: translateX(100%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

        /* Modal */
        .modal-overlay { display: none; position: fixed; inset: 0; background: rgba(0,0,0,.45); z-index: 9999; justify-content: center; align-items: center; }
        .modal-overlay.show { display: flex !important; }
        .modal-content { background: white; border-radius: 12px; width: 90%; max-width: 560px; box-shadow: 0 20px 60px rgba(0,0,0,.15); animation: fadeUp .25s ease; max-height: 90vh; overflow-y: auto; }
        @keyframes fadeUp { from { transform: translateY(20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
        .modal-header { padding: 18px 22px; border-bottom: 1px solid #f0f2f5; display: flex; justify-content: space-between; align-items: center; }
        .modal-title  { font-size: 16px; font-weight: 700; color: #1a1d23; margin: 0; }
        .btn-close { background: none; border: none; font-size: 22px; cursor: pointer; color: #9ca3af; width: 32px; height: 32px; display: flex; align-items: center; justify-content: center; border-radius: 6px; }
        .btn-close:hover { background: #f3f4f6; color: #374151; }
        .modal-body { padding: 22px; }

        .detail-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
        .detail-item { display: flex; flex-direction: column; gap: 3px; }
        .detail-item.full { grid-column: 1 / -1; }
        .detail-key { font-size: 11px; font-weight: 600; color: #9ca3af; text-transform: uppercase; letter-spacing: 0.5px; }
        .detail-val { font-size: 14px; color: #1a1d23; font-weight: 500; }
        .detail-val.mono { font-family: 'JetBrains Mono', monospace; font-size: 12.5px; color: #4f46e5; }
        .detail-divider { border: none; border-top: 1px solid #f0f2f5; margin: 4px 0; grid-column: 1/-1; }

        @media (max-width: 600px) {
            .stats-row { grid-template-columns: 1fr 1fr; }
            .detail-grid { grid-template-columns: 1fr; }
            .detail-item.full { grid-column: 1; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="trx-page">

    <!-- Error Panel -->
    <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
        <asp:Label ID="lblError" runat="server"></asp:Label>
    </asp:Panel>

    <!-- Page Header -->
    <div class="page-header">
        <div>
            <h1 class="page-title">🚆 Train Transaction History</h1>
            <p class="page-subtitle">All payment transactions for train bookings</p>
        </div>
    </div>

    <!-- Stats -->
    <div class="stats-row">
        <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div>
                <div class="stat-value"><asp:Label ID="lblTotal"   runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Transactions</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div>
                <div class="stat-value"><asp:Label ID="lblSuccess" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Successful</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon red">❌</div>
            <div>
                <div class="stat-value"><asp:Label ID="lblFailed"  runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Failed</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon orange">💰</div>
            <div>
                <div class="stat-value"><asp:Label ID="lblRevenue" runat="server" Text="0.00"></asp:Label></div>
                <div class="stat-label">Total Revenue</div>
            </div>
        </div>
    </div>

    <!-- Filters -->
    <div class="filter-card">
        <div class="filter-group">
            <span class="filter-label">Search TrxNo / Booking</span>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-control" placeholder="TrxNo or Booking ID..."></asp:TextBox>
        </div>
        <div class="filter-group">
            <span class="filter-label">Status</span>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-control">
                <asp:ListItem Value="">All Status</asp:ListItem>
                <asp:ListItem Value="Success">Success</asp:ListItem>
                <asp:ListItem Value="Failed">Failed</asp:ListItem>
                <asp:ListItem Value="Pending">Pending</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="filter-group">
            <span class="filter-label">Payment Type</span>
            <asp:DropDownList ID="ddlType" runat="server" CssClass="filter-control">
                <asp:ListItem Value="">All Types</asp:ListItem>
                <asp:ListItem Value="Payment">Payment</asp:ListItem>
                <asp:ListItem Value="Refund">Refund</asp:ListItem>
                <asp:ListItem Value="Postpone">Postpone</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="filter-group">
            <span class="filter-label">From Date</span>
            <asp:TextBox ID="txtFromDate" runat="server" CssClass="filter-control" TextMode="Date"></asp:TextBox>
        </div>
        <div class="filter-group">
            <span class="filter-label">To Date</span>
            <asp:TextBox ID="txtToDate"   runat="server" CssClass="filter-control" TextMode="Date"></asp:TextBox>
        </div>
        <asp:Button ID="btnFilter" runat="server" Text="🔍 Filter" CssClass="btn-filter" OnClick="btnFilter_Click" />
        <asp:Button ID="btnReset"  runat="server" Text="↺ Reset"  CssClass="btn-reset"  OnClick="btnReset_Click"  />
    </div>

    <!-- Table -->
    <div class="card">
        <div class="card-header">
            <h2 class="card-title">All Transactions</h2>
            <span class="record-count"><asp:Label ID="lblRecordCount" runat="server" Text="0 records"></asp:Label></span>
        </div>
        <div class="table-responsive">
            <asp:GridView ID="gvTransactions" runat="server"
                AutoGenerateColumns="false"
                CssClass="trx-table"
                GridLines="None"
                AllowPaging="true"
                PageSize="10"
                OnPageIndexChanging="gvTransactions_PageIndexChanging"
                OnRowCommand="gvTransactions_RowCommand"
                DataKeyNames="TrxId"
                PagerStyle-CssClass="pager-row">
                <Columns>

                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <span style="color:#9ca3af;font-size:12px;"><%# Container.DataItemIndex + 1 %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="TRX NO">
                        <ItemTemplate>
                            <span class="trxno-cell"><%# Eval("TrxNo") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="UserId" HeaderText="USER ID" />

                    <asp:TemplateField HeaderText="TYPE">
                        <ItemTemplate>
                            <span class='<%# GetTypeBadge(Eval("TrxTypeStatus")) %>'><%# Eval("TrxTypeStatus") ?? "—" %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="AMOUNT">
                        <ItemTemplate>
                            <span class="amount-cell"><%# string.Format("{0:N2}", Eval("Amount")) %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="EXTRA CHARGE">
                        <ItemTemplate>
                            <span style="color:#6b7280;"><%# string.Format("{0:N2}", Eval("ExtraCharge")) %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="PAY STATUS">
                        <ItemTemplate>
                            <span class="badge badge-success"><%# Eval("PaymentStatus") ?? "—" %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="STATUS">
                        <ItemTemplate>
                            <span class='<%# GetStatusBadge(Eval("Status")) %>'><%# Eval("Status") ?? "—" %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="DATE">
                        <ItemTemplate>
                            <span class="date-cell">
                                <%# Eval("CreatedAt") != null ? Convert.ToDateTime(Eval("CreatedAt")).ToString("dd MMM yyyy, hh:mm tt") : "—" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="ACTION">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnView" runat="server"
                                CssClass="btn-view"
                                CommandName="ViewDetail"
                                CommandArgument='<%# Eval("TrxId") %>'>
                                👁 View
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                    <div class="no-data">
                        <div class="no-data-icon">🔍</div>
                        <p>No transactions found</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

</div>

<!-- Detail Modal -->
<div id="detailModal" class="modal-overlay">
    <div class="modal-content">
        <div class="modal-header">
            <h5 class="modal-title">📄 Transaction Detail</h5>
            <button type="button" class="btn-close" onclick="closeModal()">&times;</button>
        </div>
        <div class="modal-body">
            <asp:HiddenField ID="hfShowModal" runat="server" Value="false" />
            <div class="detail-grid">

                <div class="detail-item">
                    <span class="detail-key">Transaction ID</span>
                    <span class="detail-val"><asp:Label ID="lblDTrxId"        runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">TRX Number</span>
                    <span class="detail-val mono"><asp:Label ID="lblDTrxNo"   runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Booking ID</span>
                    <span class="detail-val"><asp:Label ID="lblDBookingId"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">User ID</span>
                    <span class="detail-val"><asp:Label ID="lblDUserId"       runat="server"></asp:Label></span>
                </div>

                <hr class="detail-divider" />

                <div class="detail-item">
                    <span class="detail-key">Amount</span>
                    <span class="detail-val"><asp:Label ID="lblDAmount"       runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Extra Charge</span>
                    <span class="detail-val"><asp:Label ID="lblDExtraCharge"  runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Postpone Amt 1</span>
                    <span class="detail-val"><asp:Label ID="lblDPostpone1"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Postpone Amt 2</span>
                    <span class="detail-val"><asp:Label ID="lblDPostpone2"    runat="server"></asp:Label></span>
                </div>

                <hr class="detail-divider" />

                <div class="detail-item">
                    <span class="detail-key">Payment Type</span>
                    <span class="detail-val"><asp:Label ID="lblDTrxType"      runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Payment Status</span>
                    <span class="detail-val"><asp:Label ID="lblDPayStatus"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Status</span>
                    <span class="detail-val"><asp:Label ID="lblDStatus"       runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Remarks</span>
                    <span class="detail-val"><asp:Label ID="lblDRemarks"      runat="server"></asp:Label></span>
                </div>

                <hr class="detail-divider" />

                <div class="detail-item">
                    <span class="detail-key">Created At</span>
                    <span class="detail-val"><asp:Label ID="lblDCreatedAt"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Updated At</span>
                    <span class="detail-val"><asp:Label ID="lblDUpdatedAt"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Updated By</span>
                    <span class="detail-val"><asp:Label ID="lblDUpdatedBy"    runat="server"></asp:Label></span>
                </div>
                <div class="detail-item">
                    <span class="detail-key">Active</span>
                    <span class="detail-val"><asp:Label ID="lblDIsActive"     runat="server"></asp:Label></span>
                </div>

            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function closeModal() {
            document.getElementById('detailModal').classList.remove('show');
            document.body.style.overflow = '';
        }

        window.onclick = function (e) {
            if (e.target === document.getElementById('detailModal')) closeModal();
        };

        // Server sets hfShowModal = "true" when a row is clicked
        window.onload = function () {
            var flag = document.getElementById('<%= hfShowModal.ClientID %>').value;
            if (flag === 'true') {
                document.getElementById('detailModal').classList.add('show');
                document.body.style.overflow = 'hidden';
            }
        };

        function showSuccess(msg) {
            var d = document.createElement('div');
            d.className = 'success-toast';
            d.textContent = msg;
            document.body.appendChild(d);
            setTimeout(function () {
                d.style.opacity = '0';
                setTimeout(function () { if (d.parentNode) d.parentNode.removeChild(d); }, 300);
            }, 3000);
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
    </script>
</asp:Content>
