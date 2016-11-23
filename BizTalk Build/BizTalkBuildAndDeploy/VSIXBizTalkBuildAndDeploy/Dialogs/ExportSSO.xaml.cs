using System.IO;
using System.Windows;
using System.Windows.Input;


namespace VSIXBizTalkBuildAndDeploy.Dialogs
{
    /// <summary>
    /// Interaction logic for EnvironmentEntry.xaml
    /// </summary>
    public partial class ExportSSO : Microsoft.VisualStudio.PlatformUI.DialogWindow
    {
        public ExportSSO()
        {
            InitializeComponent();
            this.HasMaximizeButton = false;
            this.HasMinimizeButton = false;
        }

        public bool OK { get; set; }

        public ExportSSO(string appName, string filename)  : this()
        {
            AppName = appName;
            FileInfo fi = new FileInfo(filename);
            FilePath = fi.DirectoryName;
            FileName = fi.Name;
            OK = false;
        }

        private string FilePath { get { return input.Text; } set { input.Text = value; } }
        private string FileName { get { return inputName.Text; } set { inputName.Text = value; } }
        private string AppName { get; set; }

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
