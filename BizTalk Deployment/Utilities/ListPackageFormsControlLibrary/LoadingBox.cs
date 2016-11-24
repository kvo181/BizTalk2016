using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace bizilante.Windows.Forms.Controls.ListPackageFormsControlLibrary
{
    public partial class LoadingBox : Form
    {
        public LoadingBox(string filename)
        {
            InitializeComponent();
            this.lblLoading.Text = "Loading " + filename + " ...";
        }
    }
}
