using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NRDC_krishak_online_web.NRDC
{
    public partial class ContactInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string nameParam = Request.QueryString["name"]?.ToLower();

                if (nameParam == "yogesh")
                {
                    ShowYogeshContact();
                }
                else if (nameParam == "rajesh")
                {
                    ShowRajeshContact();
                }
                else
                {
                    defaultMessage.Visible = true;
                }
            }
        }

        private void ShowYogeshContact()
        {
            yogeshContainer.Visible = true;
            rajeshContainer.Visible = false;
            defaultMessage.Visible = false;

            List<Contact> yogeshContact = new List<Contact>
            {
                new Contact
                {
                    Name = "Yogesh Kumar",
                    Title = "Country Manager",
                    Phone = "+265 984005067",
                    Email = "yogesh@excelgeomaticsmalawi.com",
                    Address = "9/182, Lilongwe, Malawi",
                    Website = "www.excelgeomaticsmalawi.com"
                }
            };

            yogeshRepeater.DataSource = yogeshContact;
            yogeshRepeater.DataBind();
        }

        private void ShowRajeshContact()
        {
            yogeshContainer.Visible = false;
            rajeshContainer.Visible = true;
            defaultMessage.Visible = false;

            // Bind Rajesh's data
            List<Contact> rajeshContact = new List<Contact>
            {
                new Contact
                {
                    Name = "Dr Rajesh Solomon Paul",
                    Title = "CEO",
                    Phone = "+91-9871771849",
                    Email = "paul@excelgeomaticsmalawi.com",
                    Address = "9/182, Lilongwe, Malawi",
                    Website = "www.excelgeomaticsmalawi.com"
                }
            };

            rajeshRepeater.DataSource = rajeshContact;
            rajeshRepeater.DataBind();
        }

        public class Contact
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Website { get; set; }
        }
    }
}