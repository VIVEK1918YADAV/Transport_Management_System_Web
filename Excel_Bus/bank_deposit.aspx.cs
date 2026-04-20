using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel_Bus
{
    public partial class bank_deposit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                decimal amount = 0;

                if (Request.QueryString["amount"] != null)
                {
                    decimal.TryParse(Request.QueryString["amount"], out amount);
                }

                lblAmount.Text = amount.ToString("N2"); 
                lblAmountDuplicate.Text = amount.ToString("N2"); 

                pnlAlert.Visible = amount > 0;
            }
        }

        protected void btnPayNow_Click(object sender, EventArgs e)
        {
           
            Response.Redirect($"Booked_ticket.aspx");
        }

    }
}