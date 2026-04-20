using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize page on first load
                // You can add any initialization logic here
            }
        }

        protected void btnBus_Click(object sender, EventArgs e)
        {
            // Redirect to Bus home page
            Response.Redirect("Home.aspx");
        }

        protected void btnTrain_Click(object sender, EventArgs e)
        {
            // Redirect to Train page (create this page later)
            Response.Redirect("Train.aspx");
        }

        protected void btnBoat_Click(object sender, EventArgs e)
        {
            // Redirect to Boat page (create this page later)
            Response.Redirect("Boat.aspx");
        }
    }
}