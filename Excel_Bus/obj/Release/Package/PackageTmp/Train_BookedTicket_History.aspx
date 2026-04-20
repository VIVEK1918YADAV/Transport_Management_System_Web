<%@ Page Title="" Language="C#" MasterPageFile="~/TrainUserMaster.Master" AutoEventWireup="true" CodeBehind="Train_BookedTicket_History.aspx.cs" Inherits="Excel_Bus.Train_BookedTicket_History" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Train Booking History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="shortcut icon" href="img/train_icon.png" type="image/x-icon" />

    <!-- Bootstrap -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <!-- Icons -->
    <link href="css/all.min.css" rel="stylesheet" />
    <link href="css/global-line-awesome.min.css" rel="stylesheet" />
    <link href="css/flaticon.css" rel="stylesheet" />
    <!-- Custom -->
    <link href="css/main.css" rel="stylesheet" />
    <link href="css/custom.css" rel="stylesheet" />
    <link href="css/color.css" rel="stylesheet" />
    <!-- iziToast -->
    <link href="css/iziToast.min.css" rel="stylesheet" />
    <link href="css/iziToast_custom.css" rel="stylesheet" />

    <style>
       /* .inner-banner {
            background: url('img/train-bg.jpg') center / cover no-repeat;
            padding: 60px 0;
            position: relative;
        }*/
       .inner-banner {
    background-image: url('img/train-bg.jpg');
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;
    padding: 60px 0;
    position: relative;
}
        .footer-top {
    background: #47a17b;
    padding: 80px 0;
}

            .inner-banner::before {
                content: '';
                position: absolute;
                inset: 0;
                background: #47a17b;

            }

            .inner-banner .container {
                position: relative;
                z-index: 1;
            }

        .inner-banner-content h2.title {
            color: #fff;
            font-size: 36px;
            font-weight: 700;
            margin: 0;
            letter-spacing: 1px;
        }

        /* ─── Table Wrapper ───────────────────────────────────── */
        /*.booking-table-wrapper {
            margin: 40px auto;
            max-width: 1200px;
            padding: 0 15px;
        }
*/
        /*.booking-table {
            width: 100%;
            border-collapse: collapse;
            background: #fff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
        }
*/
            .booking-table thead {
                background: #47a17b !important;
            }

                .booking-table thead th {
                    color: #fff;
                    padding: 14px 16px;
                    font-size: 13px;
                    font-weight: 600;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                    text-align: left;
                    white-space: nowrap;
                }

            .booking-table tbody tr {
                border-bottom: 1px solid #fff3cd;
                transition: background 0.2s ease;
            }

                .booking-table tbody tr:last-child {
                    border-bottom: none;
                }

                .booking-table tbody tr:hover {
                    background-color: #2d9b8b8a;
                }

            .booking-table tbody td {
                padding: 13px 16px;
                font-size: 14px;
                color: #fff3cd;
                vertical-align: middle;
            }

            .booking-table td.ticket-no {
                font-weight: 600;
                color: rgb(234 193 75 / 0.64);
            }

        .badge-status {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            text-transform: capitalize;
        }

        .badge-confirmed {
            background: #47a17b;
            color: #155724;
        }

        .badge-pending {
            background: #47a17b;
            color: #856404;
        }

        .badge-cancelled {
            background: #47a17b;
            color: #721c24;
        }

        .badge-postponed {
            background: #47a17b;
            color: #004085;
        }

        .form-select {
            background-color: #2d9b8b8a;
            border: 1px solid #ced4da;
            border-radius: 6px;
            font-size: 13px;
            color: #495057;
            padding: 6px 10px;
            cursor: pointer;
            min-width: 140px;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

            .form-select:focus {
                border-color: #0E9E4D;
                box-shadow: 0 0 0 3px rgba(14, 158, 77, 0.2);
                outline: none;
            }

        #noDataMessage {
            text-align: center;
            padding: 60px 20px;
        }

            #noDataMessage i {
                font-size: 56px;
                color: #ccc;
                display: block;
                margin-bottom: 12px;
            }

            #noDataMessage p {
                color: #999;
                font-size: 16px;
            }

       /* @media (max-width: 768px) {
            .booking-table thead {
                display: none;
            }

            .booking-table tbody tr {
                display: block;
                margin-bottom: 16px;
                border-radius: 10px;
                border: 1px solid #e0e0e0;
                box-shadow: 0 2px 8px rgba(0,0,0,0.06);
            }

            .booking-table tbody td {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 10px 14px;
                border-bottom: 1px solid #f0f0f0;
                font-size: 13px;
            }

                .booking-table tbody td:last-child {
                    border-bottom: none;
                }

                .booking-table tbody td::before {
                    content: attr(data-label);
                    font-weight: 600;
                    color: #0E9E4D;
                    flex-shrink: 0;
                    margin-right: 10px;
                }

            .form-select {
                min-width: 120px;
            }
        }
         @media (min-width: 1200px) {
    .container {
        width: 1350px;
       
    }*/

        @media (max-width: 768px) {
    .inner-banner {
        padding: 40px 0;
        text-align: center;
    }

    .inner-banner-content h2.title {
        font-size: 24px;
    }
}

@media (max-width: 480px) {
    .inner-banner-content h2.title {
        font-size: 20px;
    }
}
         @media (max-width: 768px) {

    .booking-table-wrapper {
        padding: 0 10px;
    }


    .booking-table {
        min-width: 100%;
        border: none;
    }

    .booking-table thead {
        display: none;
    }

    .booking-table tbody tr {
        display: block;
        margin-bottom: 18px;
        border-radius: 12px;
        background: #ffffff;
        box-shadow: 0 4px 12px rgba(0,0,0,0.08);
        padding: 10px 0;
    }

    .booking-table tbody td {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 12px 15px;
        font-size: 14px;
        color: #333;
        border-bottom: 1px solid #eee;
    }

    .booking-table tbody td:last-child {
        border-bottom: none;
    }

    .booking-table tbody td::before {
        content: attr(data-label);
        font-weight: 600;
        color: #0E9E4D;
        font-size: 13px;
    }

    .ticket-no {
        font-size: 15px;
        font-weight: bold;
        color: #222 !important;
    }

    .form-select {
        width: 100%;
        margin-top: 5px;
    }
}
         .badge-status {
    font-size: 11px;
    padding: 4px 10px;
}

@media (max-width: 768px) {
    .badge-status {
        font-size: 12px;
    }
}
@media (max-width: 768px) {
    .action {
        flex-direction: column;
        align-items: flex-start !important;
    }

    .action select {
        width: 100%;
    }
}

#noDataMessage {
    padding: 40px 15px;
}
@media (max-width: 768px) {
    .booking-table tbody td {
        color: #333 !important;
    }
}
@media (max-width: 768px) {
    #noDataMessage i {
        font-size: 40px;
    }

    #noDataMessage p {
        font-size: 14px;
    }
}
         @media (min-width: 1200px) {
    .container {
       width: 100%;
    max-width: 1320px;
    }
}
         .booking-table-wrapper {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
    border-radius: 10px;
}

@media (max-width: 992px) {
    .booking-table {
        min-width: 100%;
    }
}


    </style>

    <section class="inner-banner">
        <div class="container">
            <div class="inner-banner-content">
                <h2 class="title">
                    <i class="las la-train" style="margin-right: 10px;"></i>Train Booking History
                </h2>
            </div>
        </div>
    </section>

    <br />

    <div class="booking-table-wrapper">
        <asp:Repeater ID="rptBookings" runat="server">
            <HeaderTemplate>
                <table class="booking-table">
                    <thead>
                        <tr>
                            <th>PNR Number</th>
                            <th>Train Number</th>
                            <th>Class</th>
                            <th>Coach No.</th>
                            <th>Starting Point</th>
                            <th>Dropping Point</th>
                            <th>Journey Date</th>
                            <th>Booked Seats</th>
                            <th>Status</th>
                            <th>Postponed</th>
                            <th>Fare</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>

            </HeaderTemplate>

            <ItemTemplate>
                <tr>
                    <td class="ticket-no" data-label="PNR Number"><%# Eval("PnrNumber") %></td>
                    <td data-label="Train Number"><%# Eval("TrainNumber") %></td>
                    <td data-label="Class"><%# Eval("CoachType") %></td>
                    <td data-label="Coach No."><%# Eval("CoachNumber") %></td>
                    <td data-label="Starting Point"><%# Eval("PickupName") %></td>
                    <td data-label="Dropping Point"><%# Eval("DropName") %></td>
                    <td data-label="Journey Date"><%# Eval("DateOfJourneyFormatted") %></td>
                    <td data-label="Booked Seats"><%# Eval("SeatsDisplay") %></td>
                    <td data-label="Status">
                        <%# GetStatusBadge(Eval("BookingStatus")) %>
                    </td>
                    <td data-label="Postponed">
                        <%# GetPostponedBadge(Convert.ToInt32(Eval("PostponeCount"))) %>
                    </td>
                    <td data-label="Fare">
                        <%# Convert.ToDecimal(Eval("TotalAmount")).ToString("N2") %> CDF
                    </td>
                    <td class="action" data-label="Action">
                        <asp:DropDownList ID="ddlActions" runat="server"
                            CssClass="form-select btn-sm"
                            AutoPostBack="True"
                            OnSelectedIndexChanged="ddlActions_SelectedIndexChanged">
                            <asp:ListItem Text="Select Action" Value="" />
                            <asp:ListItem Text="View" Value="View" />
                            <asp:ListItem Text="Postpone" Value="Postpone" />
                        </asp:DropDownList>
                        <asp:HiddenField ID="hfPnrNumber" runat="server"
                            Value='<%# Eval("PnrNumber") %>' />
                    </td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>

    <div id="noDataMessage" style="display: none;">
        <i class="las la-inbox"></i>
        <p>No train booking records found.</p>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
<script>
    document.addEventListener('DOMContentLoaded', function () {
        try {
            var map = JSON.parse(localStorage.getItem('PNRCoachMap') || '{}');

            var rows = document.querySelectorAll('.booking-table tbody tr');
            rows.forEach(function (row) {
                var pnrCell = row.querySelector('td.ticket-no');
                var coachCell = row.querySelector('td[data-label="Coach No."]');

                if (pnrCell && coachCell) {
                    var pnr = pnrCell.innerText.trim();
                    if (coachCell.innerText.trim() === 'N/A' && map[pnr]) {
                        coachCell.innerText = map[pnr];
                    }
                }
            });
        } catch (e) {
            console.error('Error reading coach map:', e);
        }
    });
</script>
</asp:Content>

