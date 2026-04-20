using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class User_View : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Retrieve the booking list from session
               // var bookings = Session["BookingList"] as List<Booking>;

                //// Check if bookings data exists in session
                //if (bookings != null)
                //{
                //    // Bind the data to the GridView
                //    gvBookings.DataSource = bookings;
                //    gvBookings.DataBind();
                //}
                //else
                //{
                //    // Handle the case when there's no data in session (optional)
                //    lblErrorMessage.Text = "No booking data found!";
                //    lblErrorMessage.Visible = true;
                //}
            }
        }
    }
        }
