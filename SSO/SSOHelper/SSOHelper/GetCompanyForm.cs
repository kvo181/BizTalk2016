using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSOHelper
{
    public partial class GetCompanyForm : Form
    {
        public string Username { get; set; }
        public string CompanyName { get; set; }

        public GetCompanyForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Username = txtUsername.Text;
            CompanyName = txtCompanyname.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Username = string.Empty;
            CompanyName = string.Empty;
        }
    }
}
