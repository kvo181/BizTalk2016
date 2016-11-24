using System;
using System.Windows.Forms;

namespace bizilante.Deployment.BTSDeployForm
{
    public partial class NoteForm : Form
    {
        public string Reason { get; set; }
        public NoteForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Reason = txtReason.Text;
            Close();
        }

        private void txtReason_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
            if (!string.IsNullOrEmpty(txtReason.Text))
                btnOK.Enabled = true;
        }
    }
}
