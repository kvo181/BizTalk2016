using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace BizTalkBuildAndDeployLauncherLibrary
{
    public partial class BizTalkBuildAndDeployLauncher : Form
    {
        public event EventHandler<UpdateEventArgs> UpdateEvent;

        public BizTalkBuildAndDeployLauncher(List<string> scripts)
        {
            InitializeComponent();
            bsScripts.DataSource = ScriptItem.Create(scripts);
        }

        private void BizTalkBuildAndDeployLauncher_Load(object sender, EventArgs e)
        {
            DoUpdateEvent(string.Format("{0} loaded", this.Name));
        }

        private void DoUpdateEvent(string message)
        {
            if (null == UpdateEvent) return;

            UpdateEventArgs args = new UpdateEventArgs(message);
            UpdateEvent(this, args);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem == null)
                return;

            ScriptItem item = lstScripts.SelectedItem as ScriptItem;
            DoUpdateEvent(string.Format("Ready to launch {0}", item.Name));
            LaunchScript(item.FullName);
        }

        System.Diagnostics.Process proc;
        System.Diagnostics.Stopwatch watch;
        private void LaunchScript(string fullName)
        {
            System.Diagnostics.ProcessStartInfo p =
                new System.Diagnostics.ProcessStartInfo(fullName);
            //p.Arguments = args;
            p.WorkingDirectory = new FileInfo(fullName).DirectoryName;
            p.RedirectStandardOutput = true;
            p.CreateNoWindow = true;
            p.UseShellExecute = false;
            p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            proc = new System.Diagnostics.Process();
            proc.StartInfo = p;

            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            DoUpdateEvent(string.Format("Build started on {0}", DateTime.Now));

            System.Threading.Thread prcessThread = 
                new System.Threading.Thread(new System.Threading.ThreadStart(procStart));
            prcessThread.Start();

            return;
        }
        void procStart()
        {
            proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(proc_OutputDataReceived);
            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit();
            watch.Stop();
            DoUpdateEvent(string.Format("Build finished at {0}. It took {1} minutes", DateTime.Now, watch.Elapsed.TotalMinutes));
            this.Close();
        }
        void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(e.Data))
                DoUpdateEvent(e.Data);
        }
    }
}
