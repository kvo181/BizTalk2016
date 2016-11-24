namespace bizilante.Deployment.BTSDeployHost.Form
{
    public partial class PromptForChoice : System.Windows.Forms.Form
    {
        public PromptForChoice()
        {
            InitializeComponent();
        }

        public static int GetConfirmChoice(System.IntPtr WinHandle,
            string caption, string message)
        {
            System.Windows.Forms.Form ParentForm = 
                (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(WinHandle);
            PromptForChoice form = new PromptForChoice();
            form.Text = caption;
            form.lblMessage.Text = message;
            System.Windows.Forms.DialogResult result = form.ShowDialog(ParentForm);
            if (result == System.Windows.Forms.DialogResult.Yes) return 1;
            return 2;
        }
    }
}
