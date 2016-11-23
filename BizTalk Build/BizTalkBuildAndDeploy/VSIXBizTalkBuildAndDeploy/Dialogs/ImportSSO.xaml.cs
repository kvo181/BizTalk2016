using System.IO;
using System.Windows;
using System.Windows.Input;


namespace VSIXBizTalkBuildAndDeploy.Dialogs
{
    /// <summary>
    /// Interaction logic for EnvironmentEntry.xaml
    /// </summary>
    public partial class ImportSSO : Microsoft.VisualStudio.PlatformUI.DialogWindow
    {
        public ImportSSO()
        {
            InitializeComponent();
            this.HasMaximizeButton = false;
            this.HasMinimizeButton = false;
        }

        public bool OK { get; set; }

        public ImportSSO(string filename)  : this()
        {
            FileInfo fi = new FileInfo(filename);
            FilePath = fi.DirectoryName;
            FileName = fi.Name;
            OK = false;
        }

        private string FilePath { get { return input.Text; } set { input.Text = value; } }
        private string FileName { get { return inputName.Text; } set { inputName.Text = value; } }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            OK = true;
            this.Close();
        }
        private void btnNok_Click(object sender, RoutedEventArgs e)
        {
            OK = false;
            this.Close();
        }

        private void input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOk_Click(sender, e);
            }
        }
    }
}
