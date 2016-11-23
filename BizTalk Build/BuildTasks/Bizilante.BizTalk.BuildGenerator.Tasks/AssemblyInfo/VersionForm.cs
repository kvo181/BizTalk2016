using System;
using System.Windows.Forms;

namespace bizilante.BuildGenerator.Tasks
{
    public partial class VersionForm : Form
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int BuildNumber { get; set; }
        public int Revision { get; set; }

        public bool MinorIncrement { get; set; }
        public bool BuildIncrement { get; set; }
        public bool RevisionIncrement { get; set; }

        public VersionForm(string title, string major, string minor, string build, string revision)
        {
            InitializeComponent();
            Text = title;
            MajorVersion = int.Parse(major);
            MinorVersion = int.Parse(minor);
            BuildNumber = int.Parse(build);
            Revision = int.Parse(revision);
        }

        public VersionForm()
        {
            InitializeComponent();
        }

        private void VersionForm_Load(object sender, EventArgs e)
        {
            numericUpDownMajor.Value = MajorVersion;
            numericUpDownMinor.Value = MinorVersion;
            if (BuildNumber <= numericUpDownBuild.Maximum)
                numericUpDownBuild.Value = BuildNumber;
            numericUpDownRevision.Value = Revision;

            if (MinorIncrement) numericUpDownMinor.Enabled = true;
            if (BuildIncrement) numericUpDownBuild.Enabled = true;
            if (RevisionIncrement) numericUpDownRevision.Enabled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MajorVersion = Convert.ToInt32(numericUpDownMajor.Value);
            MinorVersion = Convert.ToInt32(numericUpDownMinor.Value);
            BuildNumber = Convert.ToInt32(numericUpDownBuild.Value);
            Revision = Convert.ToInt32(numericUpDownRevision.Value);
        }
    }
}
