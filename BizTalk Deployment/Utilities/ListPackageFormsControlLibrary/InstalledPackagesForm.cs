using System.Collections.Generic;
using System.Windows.Forms;

namespace bizilante.Windows.Forms.Controls.ListPackageFormsControlLibrary
{
    public partial class frmInstalledPackages : Form
    {
        public frmInstalledPackages(string filename)
        {
            InitializeComponent();
            DoDataBinding(filename);
        }

        private void DoDataBinding(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;
            string[] filenames = new string[] { filename };
            userControlInstalledPackages1.FileNames = new List<string>(filenames);
        }

    }
}