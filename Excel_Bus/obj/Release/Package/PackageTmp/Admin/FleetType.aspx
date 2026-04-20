<%@ Page Title="Fleet Type" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FleetType.aspx.cs" Inherits="Excel_Bus.FleetType" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .fleet-type-page {
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

        .card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .table-responsive {
            overflow-x: auto;
        }

        .fleet-table {
            width: 100%;
            border-collapse: collapse;
        }

            .fleet-table thead {
                background: #f8f9fa;
            }

            .fleet-table th {
                padding: 15px;
                text-align: left;
                font-weight: 600;
                color: #2c3e50;
                border-bottom: 2px solid #dee2e6;
            }

            .fleet-table td {
                padding: 15px;
                border-bottom: 1px solid #dee2e6;
                color: #495057;
            }

            .fleet-table tbody tr:hover {
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

        /* Disable/Enable Button */
        .btn-disable {
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

            .btn-disable:hover {
                background: #ea5455;
                color: white;
            }

        /* Enable Button (when status is disabled) */
        .btn-enable {
            padding: 6px 12px;
            background: white;
            color: #28c76f;
            border: 1px solid #28c76f;
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

            .btn-enable:hover {
                background: #28c76f;
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
            max-width: 550px;
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

        .form-control, .form-select {
            width: 100%;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
        }

            .form-control:focus, .form-select:focus {
                outline: none;
                border-color: #7367f0;
                box-shadow: 0 0 0 0.2rem rgba(115, 103, 240, 0.25);
            }

        .toggle-container {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .toggle-switch {
            position: relative;
            width: 60px;
            height: 30px;
        }

            .toggle-switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .toggle-slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: 0.3s;
            border-radius: 30px;
        }

            .toggle-slider:before {
                position: absolute;
                content: "";
                height: 22px;
                width: 22px;
                left: 4px;
                bottom: 4px;
                background-color: white;
                transition: 0.3s;
                border-radius: 50%;
            }

        input:checked + .toggle-slider {
            background-color: #28c76f;
        }

            input:checked + .toggle-slider:before {
                transform: translateX(30px);
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

        .facilities-input-container {
            position: relative;
        }

        .facilities-tags {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            padding: 8px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            min-height: 45px;
            cursor: text;
        }

        .facility-tag {
            background-color: #e0e7ff;
            color: #3730a3;
            padding: 4px 10px;
            border-radius: 4px;
            font-size: 13px;
            display: flex;
            align-items: center;
            gap: 5px;
        }

            .facility-tag .remove-tag {
                cursor: pointer;
                font-weight: bold;
                color: #6366f1;
            }

        .facilities-dropdown {
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border: 1px solid #ced4da;
            border-radius: 4px;
            margin-top: 4px;
            max-height: 200px;
            overflow-y: auto;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            display: none;
            z-index: 10;
        }

            .facilities-dropdown.show {
                display: block;
            }

        .facility-option {
            padding: 10px 12px;
            cursor: pointer;
        }

            .facility-option:hover {
                background-color: #f3f4f6;
            }

            .facility-option.selected {
                background-color: #e0e7ff;
                color: #3730a3;
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
    <div class="fleet-type-page">
        <asp:Panel ID="pnlError" runat="server" CssClass="error-panel" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="page-header">
            <h1 class="page-title">Fleet Type</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add New" CssClass="btn-add-new"
                OnClientClick="openAddNewModal(); return false;" />
        </div>

        <div class="card">
            <div class="table-responsive">
                <asp:GridView ID="gvFleetTypes" runat="server" AutoGenerateColumns="false"
                    CssClass="fleet-table" GridLines="None" OnRowCommand="gvFleetTypes_RowCommand"
                    DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="SeatLayout" HeaderText="Seat Layout" />
                        <asp:BoundField DataField="Deck" HeaderText="No of Deck" />
                        <asp:TemplateField HeaderText="Total Seat">
                            <ItemTemplate>
                                <%# ((Excel_Bus.FleetType)Page).GetTotalSeats(Eval("DeckSeats")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Facilities">
                            <ItemTemplate>
                                <%# ((Excel_Bus.FleetType)Page).GetFacilitiesDisplay(Eval("Facilities")) %>
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
                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CssClass="btn-edit"
                                        CommandName="EditFleet"
                                        CommandArgument='<%# Eval("Id") %>'>
                ✏️ Edit
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDisable" runat="server"
                                        CssClass='<%# Eval("Status").ToString() == "1" ? "btn-disable" : "btn-enable" %>'
                                        CommandName="ToggleStatus"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Are you sure you want to change the status?');">
                <%# Eval("Status").ToString() == "1" ? "🚫 Disable" : "✓ Enable" %>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="no-data">
                            <p>No fleet types found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal -->
        <div id="fleetModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Fleet Type"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" onclick="hideModal(); return false;">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfFleetId" runat="server" />
                    <asp:HiddenField ID="hfIsEdit" runat="server" Value="false" />
                    <asp:HiddenField ID="hfDeckSeatsData" runat="server" />

                    <div class="form-group">
                        <label class="form-label">Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter name"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Seat Layout <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlSeatLayout" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select an option</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label class="form-label">No of Deck <span class="required">*</span></label>
                        <asp:TextBox ID="txtDeck" runat="server" CssClass="form-control"
                            TextMode="Number" placeholder="Enter number of decks"
                            onchange="generateDeckSeatsInputs(this.value)"></asp:TextBox>
                    </div>

                    <div id="deckSeatsContainer"></div>

                    <div class="form-group">
                        <label class="form-label">Facilities</label>
                        <asp:HiddenField ID="hfFacilities" runat="server" />
                        <div class="facilities-input-container">
                            <div class="facilities-tags" id="facilitiesTags" onclick="toggleFacilitiesDropdown()">
                                <span style="color: #9ca3af; font-size: 14px;">Click to select facilities</span>
                            </div>
                            <div class="facilities-dropdown" id="facilitiesDropdown"></div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">AC Status</label>
                        <div class="toggle-container">
                            <label class="toggle-switch">
                                <asp:CheckBox ID="chkHasAc" runat="server" />
                                <span class="toggle-slider"></span>
                            </label>
                            <span>Air Conditioned</span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        CssClass="btn-submit" OnClick="btnSubmit_Click" OnClientClick="return validateForm();" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        let availableFacilities = ['Soft Drinks', 'Water Bottle', 'Pillow', 'Wifi', 'Charging Point'];
        let selectedFacilities = [];

        function openAddNewModal() {
            clearFleetForm();
            document.getElementById('<%= lblModalTitle.ClientID %>').textContent = 'Add New Fleet Type';
            showModal();
            return false;
        }

        function clearFleetForm() {
            document.getElementById('<%= txtName.ClientID %>').value = '';
            document.getElementById('<%= ddlSeatLayout.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= txtDeck.ClientID %>').value = '';
            document.getElementById('<%= chkHasAc.ClientID %>').checked = false;

            document.getElementById('<%= hfFleetId.ClientID %>').value = '';
            document.getElementById('<%= hfIsEdit.ClientID %>').value = 'false';
            document.getElementById('<%= hfFacilities.ClientID %>').value = '';
            document.getElementById('<%= hfDeckSeatsData.ClientID %>').value = '';

            document.getElementById('deckSeatsContainer').innerHTML = '';

            selectedFacilities = [];
            updateFacilitiesTags();
        }

        function showModal() {
            var modal = document.getElementById('fleetModal');
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
            }
            return false;
        }

        function hideModal() {
            var modal = document.getElementById('fleetModal');
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

        function loadEditData() {
            var deckSeatsData = document.getElementById('<%= hfDeckSeatsData.ClientID %>').value;
            var facilitiesData = document.getElementById('<%= hfFacilities.ClientID %>').value;
            var deckValue = document.getElementById('<%= txtDeck.ClientID %>').value;

            if (deckSeatsData && deckValue) {
                var deckSeats = JSON.parse(deckSeatsData);
                generateDeckSeatsInputs(deckValue, deckSeats);
            }

            if (facilitiesData) {
                selectedFacilities = JSON.parse(facilitiesData);
                loadFacilities();
                updateFacilitiesTags();
            }
        }

        function generateDeckSeatsInputs(deckCount, existingSeats = []) {
            const container = document.getElementById('deckSeatsContainer');
            container.innerHTML = '';

            for (let i = 1; i <= deckCount; i++) {
                const formGroup = document.createElement('div');
                formGroup.className = 'form-group';

                const label = document.createElement('label');
                label.className = 'form-label';
                label.innerHTML = `Seats of Deck - ${i} <span class="required">*</span>`;

                const input = document.createElement('input');
                input.type = 'number';
                input.className = 'form-control';
                input.placeholder = 'Enter number of seats';
                input.id = `deckSeat${i}`;
                input.name = `deckSeat${i}`;
                input.value = existingSeats[i - 1] || '';

                formGroup.appendChild(label);
                formGroup.appendChild(input);
                container.appendChild(formGroup);
            }
        }

        function loadFacilities() {
            const dropdown = document.getElementById('facilitiesDropdown');
            dropdown.innerHTML = '';

            availableFacilities.forEach(facility => {
                const option = document.createElement('div');
                option.className = 'facility-option';
                if (selectedFacilities.includes(facility)) {
                    option.classList.add('selected');
                }
                option.textContent = facility;
                option.onclick = () => toggleFacility(facility);
                dropdown.appendChild(option);
            });
        }

        function toggleFacilitiesDropdown() {
            const dropdown = document.getElementById('facilitiesDropdown');
            dropdown.classList.toggle('show');
        }

        function toggleFacility(facility) {
            const index = selectedFacilities.indexOf(facility);
            if (index > -1) {
                selectedFacilities.splice(index, 1);
            } else {
                selectedFacilities.push(facility);
            }
            loadFacilities();
            updateFacilitiesTags();
        }

        function updateFacilitiesTags() {
            const tagsContainer = document.getElementById('facilitiesTags');
            tagsContainer.innerHTML = '';

            if (selectedFacilities.length === 0) {
                tagsContainer.innerHTML = '<span style="color: #9ca3af; font-size: 14px;">Click to select facilities</span>';
            } else {
                selectedFacilities.forEach(facility => {
                    const tag = document.createElement('div');
                    tag.className = 'facility-tag';
                    tag.innerHTML = `${facility} <span class="remove-tag" onclick="event.stopPropagation(); toggleFacility('${facility}')">×</span>`;
                    tagsContainer.appendChild(tag);
                });
            }

            document.getElementById('<%= hfFacilities.ClientID %>').value = JSON.stringify(selectedFacilities);
        }

        function validateForm() {
            const name = document.getElementById('<%= txtName.ClientID %>').value.trim();
            const seatLayout = document.getElementById('<%= ddlSeatLayout.ClientID %>').value;
            const deck = document.getElementById('<%= txtDeck.ClientID %>').value;

            if (!name) {
                alert('Please enter fleet type name');
                return false;
            }

            if (!seatLayout) {
                alert('Please select seat layout');
                return false;
            }

            if (!deck || deck < 1) {
                alert('Please enter valid number of decks');
                return false;
            }

            for (let i = 1; i <= deck; i++) {
                const seatInput = document.getElementById(`deckSeat${i}`);
                if (!seatInput || !seatInput.value || seatInput.value < 1) {
                    alert(`Please enter valid number of seats for Deck ${i}`);
                    return false;
                }
            }

            document.getElementById('<%= hfFacilities.ClientID %>').value = JSON.stringify(selectedFacilities);

            return true;
        }

        document.addEventListener('click', function (event) {
            const dropdown = document.getElementById('facilitiesDropdown');
            const container = document.querySelector('.facilities-input-container');

            if (container && !container.contains(event.target)) {
                dropdown.classList.remove('show');
            }
        });

        window.onclick = function (event) {
            var modal = document.getElementById('fleetModal');
            if (event.target == modal) {
                hideModal();
            }
        }

        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        window.onload = function () {
            loadFacilities();
        }
    </script>
</asp:Content>
