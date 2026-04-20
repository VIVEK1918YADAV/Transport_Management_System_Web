
        using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
    {
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize page on first load
               
            }
        }

      


        protected void BuyTicketsButton_Click(object sender, EventArgs e)
        {
            // Redirect to tickets page
            Response.Redirect("ticket.aspx");
        }


    }
      
      
    }
