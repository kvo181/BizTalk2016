using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSIXBizTalkBuildAndDeploy.Helpers.Options
{
    public partial class MyUserControl : UserControl
    {
        internal OptionPageCustom optionsPage;

        public MyUserControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            textBox1.Text = optionsPage.OptionString;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            optionsPage.OptionString = textBox1.Text;
        }
    }
}
