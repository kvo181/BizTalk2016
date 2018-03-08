using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Reflection;
using bizilante.Windows.Forms.Controls.ListPackageFormsControlLibrary;
using bizilante.Deployment.BTSDeployForm.Extensions;

namespace bizilante.Deployment.BTSDeployForm
{
    public partial class DeployForm : Form
    {
        private static readonly string Root = ConfigurationManager.AppSettings["MsiRootPath"];
        private static Dictionary<string, string> BTSEnvironments = new Dictionary<string, string>();
        private string _logInfo = string.Empty;
        private string _logFolder = string.Empty;
        private string _msiPath = string.Empty;
        private string _logFilename = string.Empty;
        private string _app = string.Empty;
        private string _version = string.Empty;
        private string _note = string.Empty;
        private string _action = string.Empty;

        #region Public interfaces
        public Form Form
        {
            get { return this; }
        }
        public RichTextBox OutputTextBox
        {
            get { return rtxtLog; }
        }
        public Button InvokeButton
        {
            get { return btnInstall; }
        }
        public Button RemoveButton
        {
            get { return btnRemove; }
        }
        public Button StopButton
        {
            get { return btnStop; }
        }
        public Button StartButton
        {
            get { return btnStart; }
        }
        public Button RecycleButton
        {
            get { return btnRecycle; }
        }
        public StatusStrip StatusStrip
        {
            get { return statusStrip1; }
        }
        public void SetInvokeButtonState(bool isEnabled)
        {
            btnInstall.Enabled = isEnabled;
            btnInstall.Focus();
        }
        public void SetRemoveButtonState(bool isEnabled)
        {
            btnRemove.Enabled = isEnabled;
            btnRemove.Focus();
        }
        public void SetStopButtonState(bool isEnabled)
        {
            btnStop.Enabled = isEnabled;
            btnStop.Focus();
        }
        public void SetStartButtonState(bool isEnabled)
        {
            btnStart.Enabled = isEnabled;
            btnStart.Focus();
        }
        public void SetRecycleButtonState(bool isEnabled)
        {
            btnRecycle.Enabled = isEnabled;
            btnRecycle.Focus();
        }
        public void SetOutputTextBoxContent(string text)
        {
            rtxtLog.AppendText(text);
            _logInfo = rtxtLog.Text;
        }
        public void SetOutputTextBoxColor(Color color, string text)
        {
            rtxtLog.SelectionColor = color;
            rtxtLog.AppendText(text);
            rtxtLog.SelectionColor = rtxtLog.ForeColor;
            _logInfo = rtxtLog.Text;
        }
        public void SetToolStripStatusLabel(string text)
        {
            toolStripStatusLabel.Text = text;
        }
        public void SetActionDone()
        {
            _logFilename = string.Empty;
            _action = string.Empty;
        }
        public delegate void SetInvokeButtonStateDelegate(bool isEnabled);
        public delegate void SetRemoveButtonStateDelegate(bool isEnabled);
        public delegate void SetStopButtonStateDelegate(bool isEnabled);
        public delegate void SetStartButtonStateDelegate(bool isEnabled);
        public delegate void SetRecycleButtonStateDelegate(bool isEnabled);
        public delegate void SetOutputTextBoxContentDelegate(string text);
        public delegate void SetOutputTextBoxColorDelegate(Color color, string text);
        public delegate void SetToolStripStatusLabelDelegate(string text);
        public delegate void SetActionDoneDelegate();

        public void SetType(string type)
        {
            if (string.IsNullOrEmpty(type)) return;
            switch (type.ToLower())
            {
                case "full":
                    rbtnFull.Checked = true;
                    break;
                case "patch":
                    rbtnPatch.Checked = true;
                    break;
                case "adapter":
                    rbtnAdapter.Checked = true;
                    break;
            }
        }
        public void SetInstallOn(string environment)
        {
            // Loop BizTalk Environments
            int index = 0;
            bool found = false;
            foreach (KeyValuePair<string, string> btsEnv in BTSEnvironments)
            {
                index++;
                switch (index)
                {
                    case 1:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnLoc.Checked = true;
                            found = true;
                        }
                        break;
                    case 2:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnDev.Checked = true;
                            found = true;
                        }
                        break;
                    case 3:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnTst.Checked = true;
                            found = true;
                        }
                        break;
                    case 4:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnEdu.Checked = true;
                            found = true;
                        }
                        break;
                    case 5:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnHfx.Checked = true;
                            found = true;
                        }
                        break;
                    case 6:
                        if (string.Compare(btsEnv.Key, environment, true) == 0)
                        {
                            rbtnPrd.Checked = true;
                            found = true;
                        }
                        break;
                }
                if (found) break;
            }
        }
        public void SetApplication(string application)
        {
            if (rbtnAdapter.Checked) return;
            if (string.IsNullOrEmpty(application)) return;
            cboApp.SelectedItem = application;
        }
        public void SetAdapter(string adapter)
        {
            if (!rbtnAdapter.Checked) return;
            if (string.IsNullOrEmpty(adapter)) return;
            cboApp.SelectedItem = adapter;
        }
        public void SetVersion(string version)
        {
            if (rbtnAdapter.Checked) return;
            if (string.IsNullOrEmpty(version)) return;
            cboVersion.SelectedItem = version;
        }
        public void SetAction(string action)
        {
            _action = action;
            if (string.IsNullOrEmpty(action)) return;
            switch (action.ToLower())
            {
                case "install":
                    btnGetMsi_Click(this, new EventArgs());
                    btnGetMsi.Enabled = false;
                    btnShowMsi.Enabled = false;
                    btnInstall.Enabled = true;
                    btnRemove.Enabled = false;
                    btnStop.Enabled = false;
                    btnStart.Enabled = false;
                    btnRecycle.Enabled = false;

                    cboApp.Enabled = false;
                    cboVersion.Enabled = false;

                    grpType.Enabled = false;
                    grpInstall.Enabled = false;
                    grpLogging.Enabled = false;

                    grpMsi.Enabled = false;
                    grpInstallOn.Enabled = false;

                    break;
                case "remove":
                    btnGetMsi_Click(this, new EventArgs());
                    btnGetMsi.Enabled = false;
                    btnShowMsi.Enabled = false;
                    btnInstall.Enabled = false;
                    btnRemove.Enabled = true;
                    btnStop.Enabled = false;
                    btnStart.Enabled = false;
                    btnRecycle.Enabled = false;

                    cboApp.Enabled = false;
                    cboVersion.Enabled = false;

                    grpType.Enabled = false;
                    grpInstall.Enabled = false;
                    grpLogging.Enabled = false;

                    grpMsi.Enabled = false;
                    grpInstallOn.Enabled = false;

                    break;
                case "stop":
                    btnGetMsi_Click(this, new EventArgs());
                    btnGetMsi.Enabled = false;
                    btnShowMsi.Enabled = false;
                    btnInstall.Enabled = false;
                    btnRemove.Enabled = false;
                    btnStop.Enabled = true;
                    btnStart.Enabled = false;
                    btnRecycle.Enabled = false;

                    cboApp.Enabled = false;
                    cboVersion.Enabled = false;

                    grpType.Enabled = false;
                    grpInstall.Enabled = false;
                    grpLogging.Enabled = false;

                    grpMsi.Enabled = false;
                    grpInstallOn.Enabled = false;

                    break;
                case "start":
                    btnGetMsi_Click(this, new EventArgs());
                    btnGetMsi.Enabled = false;
                    btnShowMsi.Enabled = false;
                    btnInstall.Enabled = false;
                    btnRemove.Enabled = false;
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                    btnRecycle.Enabled = false;

                    cboApp.Enabled = false;
                    cboVersion.Enabled = false;

                    grpType.Enabled = false;
                    grpInstall.Enabled = false;
                    grpLogging.Enabled = false;

                    grpMsi.Enabled = false;
                    grpInstallOn.Enabled = false;

                    break;
                case "recycle":
                    btnGetMsi.Enabled = false;
                    btnShowMsi.Enabled = false;
                    btnInstall.Enabled = false;
                    btnRemove.Enabled = false;
                    btnStop.Enabled = false;
                    btnStart.Enabled = false;
                    btnRecycle.Enabled = true;

                    cboApp.Enabled = false;
                    cboVersion.Enabled = false;

                    grpType.Enabled = false;
                    grpInstall.Enabled = false;
                    grpLogging.Enabled = false;

                    grpMsi.Enabled = false;
                    grpInstallOn.Enabled = false;

                    break;
            }
        }
        public void SetActionOnly(string action)
        {
            _action = action;
        }
        public void SetLogging(bool enabled, string logFolder)
        {
            chkLogging.Checked = enabled;
            DirectoryInfo di = new DirectoryInfo(logFolder);
            if (!di.Exists)
                throw new Exception(string.Format("Log Folder '{0}' does not exist", logFolder));
            _logFolder = di.FullName;
        }
        public bool GetLogFilename(out string filename)
        {
            filename = string.Empty;
            if (!string.IsNullOrEmpty(_logFilename))
            {
                filename = _logFilename;
                return chkLogging.Checked;
            }

            if (chkLogging.Checked)
            {
                switch (_action.ToLower())
                {
                    case "recycle":
                        filename = string.Format("{0}\\{1}_{2}.log", _logFolder, DateTime.UtcNow.ToString("yyyyMMddhhmmss"), "RecycleBizTalkServer");
                        break;
                    default:
                        if (string.IsNullOrEmpty(_msiPath))
                        {
                            filename = string.Empty;
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(_msiPath);
                            filename = string.Format("{0}\\{1}_{2}.log", _logFolder, DateTime.UtcNow.ToString("yyyyMMddhhmmss"), fi.Name.Replace(fi.Extension, string.Empty));
                        }
                        break;
                }
                toolStripStatusLabel.Text = string.Format("Log to {0}", filename);
            }
            else
                toolStripStatusLabel.Text = "No logging enabled!";
            _logFilename = filename;
            return chkLogging.Checked;
        }
        public string GetMsiPathToLog()
        {
            // We only return the path for the following actions:
            // - Install
            // (we only need to return the msi path in case we want to log the msi contents)
            switch (_action.ToLower())
            {
                case "install":
                    return _msiPath;
            }
            return string.Empty;
        }
        public string GetAppToLog()
        {
            return _app;
        }
        public string GetVersionToLog()
        {
            return _version;
        }
        public string GetLogInfo()
        {
            return _logInfo;
        }
        public string GetAction()
        {
            return _action;
        }
        #endregion

        public DeployForm()
        {
            InitializeComponent();
            ShowTitle();
            GetBizTalkDbServers();
            FillRadioButtons();
            cboApp.DataSource = GetApplications();
            rbtnFull.Checked = true;
        }

        private static List<string> GetApplications()
        {
            var di = new DirectoryInfo(Root);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", Root));

            var dis = di.GetDirectories();
            return dis.Where(d => d.Name != "Adapters").Select(d => d.Name).ToList();
        }
        private static List<string> GetAdapters()
        {
            var adapterRoot = Root + "\\Adapters";
            var di = new DirectoryInfo(adapterRoot);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", adapterRoot));

            var dis = di.GetDirectories();
            return dis.Select(d => d.Name).ToList();
        }

        private static List<string> GetVersions(string appName, bool patch)
        {
            var di = new DirectoryInfo(Root + "\\" + appName);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", di.FullName));

            FileInfo[] files;
            if (!patch)
            {
                files = di.GetFiles(string.Format("{0}_*-FULL.msi", appName));
                return files.Select(f => f.Name.Substring(f.Name.LastIndexOf("_") + 1, f.Name.IndexOf("-FULL") - f.Name.LastIndexOf("_") - 1)).ToList();
            }

            files = di.GetFiles(string.Format("{0}_*-Patch.msi", appName));
            return files.Select(f => f.Name.Substring(f.Name.LastIndexOf("_") + 1, f.Name.IndexOf("-Patch") - f.Name.LastIndexOf("_") - 1)).ToList();

        }

        private static Dictionary<string, string> GetBizTalkDbServers()
        {
            if (BTSEnvironments.Count != 0) return BTSEnvironments;

            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
                if (ConfigurationManager.AppSettings.GetKey(i).StartsWith("BizTalk_"))
                    BTSEnvironments.Add(ConfigurationManager.AppSettings.GetKey(i).Replace("BizTalk_", string.Empty), ConfigurationManager.AppSettings[i]);
            return BTSEnvironments;
        }

        private void ShowTitle()
        {
            AssemblyName asmName = Assembly.GetAssembly(typeof(DeployForm)).GetName();
            string addinname = string.Format(" - Version {0}", asmName.Version);
            Text += addinname;
        }

        private void FillRadioButtons()
        {
            // Loop BizTalk Environments
            int index = 0;
            foreach (KeyValuePair<string, string> btsEnv in BTSEnvironments)
            {
                index++;
                switch (index)
                {
                    case 1:
                        rbtnLoc.Enabled = true;
                        rbtnLoc.Visible = true;
                        rbtnLoc.Text = btsEnv.Key;
                        break;
                    case 2:
                        rbtnDev.Enabled = true;
                        rbtnDev.Visible = true;
                        rbtnDev.Text = btsEnv.Key;
                        break;
                    case 3:
                        rbtnTst.Enabled = true;
                        rbtnTst.Visible = true;
                        rbtnTst.Text = btsEnv.Key;
                        break;
                    case 4:
                        rbtnEdu.Enabled = true;
                        rbtnEdu.Visible = true;
                        rbtnEdu.Text = btsEnv.Key;
                        break;
                    case 5:
                        rbtnHfx.Enabled = true;
                        rbtnHfx.Visible = true;
                        rbtnHfx.Text = btsEnv.Key;
                        break;
                    case 6:
                        rbtnPrd.Enabled = true;
                        rbtnPrd.Visible = true;
                        rbtnPrd.Text = btsEnv.Key;
                        break;
                }
            }
        }

        private string GetServerNames()
        {
            if (rbtnDev.Checked) return GetBizTalkDbServers()[rbtnDev.Text];
            if (rbtnTst.Checked) return GetBizTalkDbServers()[rbtnTst.Text];
            if (rbtnEdu.Checked) return GetBizTalkDbServers()[rbtnEdu.Text];
            if (rbtnHfx.Checked) return GetBizTalkDbServers()[rbtnHfx.Text];
            if (rbtnPrd.Checked) return GetBizTalkDbServers()[rbtnPrd.Text];
            return GetBizTalkDbServers()[rbtnLoc.Text];
        }

        public string GetDbServerName()
        {
            string[] names = GetServerNames().Split(new string[] { ";" }, StringSplitOptions.None);
            return names[0];
        }
        public string GetDbName()
        {
            string[] names = GetServerNames().Split(new string[] { ";" }, StringSplitOptions.None);
            if (names.Length > 1)
                return names[1];
            else
                return "BizTalkMgmtDb";
        }
        public string GetMsiFilePath()
        {
            if (null == cboApp.SelectedValue) return null;
            if (null == cboVersion.SelectedValue) return null;

            string appName = cboApp.SelectedValue.ToString();
            string version = cboVersion.SelectedValue.ToString();

            var di = new DirectoryInfo(Root + "\\" + appName);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", di.FullName));

            var searchPattern = rbtnFull.Checked ? string.Format("{0}*_{1}-FULL.msi", appName, version) : string.Format("{0}*_{1}-Patch.msi", appName, version);
            var files = di.GetFiles(searchPattern);
            if (files.Length != 1)
                throw new Exception(string.Format("'{0}' cannot find it once in folder {1}", searchPattern, di.FullName));
            _msiPath = files[0].FullName;
            return files[0].FullName;
        }
        public string GetMsiAdapterFilePath()
        {
            if (null == cboApp.SelectedValue) return null;

            var adapterName = cboApp.SelectedValue.ToString();

            var di = new DirectoryInfo(Root + "\\Adapters\\" + adapterName);
            if (!di.Exists)
                throw new Exception(string.Format("'{0}' could not be located", di.FullName));

            var searchPattern = string.Format("*.msi");
            var files = di.GetFiles(searchPattern);
            if (files.Length != 1)
                throw new Exception(string.Format("'{0}' cannot find it once in folder {1}", searchPattern, di.FullName));

            return files[0].FullName;
        }
        public string GetApplicationName()
        {
            if (null == cboApp.SelectedValue) return null;
            _app = cboApp.SelectedValue.ToString();
            return _app;
        }
        public string GetVersion()
        {
            if (null == cboApp.SelectedValue) return null;
            if (null == cboVersion.SelectedValue) return null;
            _version = cboVersion.SelectedValue.ToString();
            return _version;
        }
        public string GetEnv()
        {
            if (rbtnDev.Checked) return rbtnDev.Text;
            if (rbtnTst.Checked) return rbtnTst.Text;
            if (rbtnEdu.Checked) return rbtnEdu.Text;
            if (rbtnHfx.Checked) return rbtnHfx.Text;
            if (rbtnPrd.Checked) return rbtnPrd.Text;
            return rbtnLoc.Text;
        }
        public string GetDeploySSO()
        {
            if (chkDeploySSO.Checked) return "true";
            return "false";
        }
        public string GetDeployItinerary()
        {
            if (chkDeployItineraries.Checked) return "true";
            return "false";
        }
        public bool IsPatch()
        {
            return rbtnPatch.Checked;
        }
        public bool IsAdapter()
        {
            return rbtnAdapter.Checked;
        }

        public void EnterNote()
        {
            NoteForm frm = new NoteForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
                _note = frm.Reason;
        }
        public string GetNote()
        {
            return _note;
        }

        /// <summary>
        /// Checks to see if the tobe deployed version is valid.
        /// (should be more recent)
        /// </summary>
        /// <param name="version">Currently deployed version</param>
        /// <param name="dbversion">Currently deployed version (dixit DeploymentDb)</param>
        /// <returns>true or false</returns>
        public bool ValidVersion(string version, string dbversion)
        {
            DeploymentDbFormsControlLibrary.VersionWarning frm =
                DeploymentDbFormsControlLibrary.VersionWarning.Create(GetApplicationName(), version, dbversion, GetVersion());
            if (frm == null) return true;
            if (frm.ShowDialog(this) == DialogResult.Yes)
                return true;
            return false;
        }

        private void DeployForm_Load(object sender, EventArgs e)
        {
            //cboApp.DataSource = GetApplications();
        }

        private void cboApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnAdapter.Checked) return;
            cboVersion.DataSource = GetVersions(GetApplicationName(), rbtnPatch.Checked);
            btnInstall.Enabled = false;
            btnRemove.Enabled = false;
            btnStop.Enabled = false;
            btnStart.Enabled = false;
            btnRecycle.Enabled = false;
            // We need to clear the MSI entry
            txtMsiPath.Clear();
            txtMsiFileName.Clear();
        }

        private void cboVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGetMsi.Enabled = (null != cboVersion.SelectedValue);
            _version = (null != cboVersion.SelectedValue) ? cboVersion.SelectedValue.ToString() : string.Empty;
            // We need to clear the MSI entry
            txtMsiPath.Clear();
            txtMsiFileName.Clear();
        }

        private void btnGetMsi_Click(object sender, EventArgs e)
        {
            string msiFile = null;
            if (rbtnAdapter.Checked)
                msiFile = GetMsiAdapterFilePath();
            else
                msiFile = GetMsiFilePath();
            if (null == msiFile)
            {
                btnInstall.Enabled = false;
                btnRemove.Enabled = false;
                btnStop.Enabled = false;
                btnStart.Enabled = false;
                btnRecycle.Enabled = false;
                txtMsiPath.Clear();
                txtMsiFileName.Clear();
                return;
            }
            var fi = new FileInfo(msiFile);
            if (fi.Exists)
            {
                txtMsiPath.Text = fi.DirectoryName;
                txtMsiFileName.Text = fi.Name;
                btnInstall.Enabled = true;
                if (!rbtnAdapter.Checked)
                {
                    btnRemove.Enabled = true;
                    btnStop.Enabled = true;
                    btnStart.Enabled = true;
                    btnRecycle.Enabled = true;
                }
                else
                {
                    btnRemove.Enabled = false;
                    btnStop.Enabled = false;
                    btnStart.Enabled = false;
                    btnRecycle.Enabled = false;
                }
                //string filename;
                //GetLogFilename(out filename);

                // Does the patch include a SSO and/or Itinerary file?
                // Use this info to enable/disable the corresponding checkBox controls.
                List<string> deployFiles = Helpers.ListPackageHelper.Helper.ListPackageContentAsList(fi.FullName);
                chkDeploySSO.Checked = deployFiles.Any(f => f.Contains("System.BizTalk:File;") && f.Contains("SSO"));
                chkDeployItineraries.Checked = deployFiles.Any(f => f.Contains("System.BizTalk:File;") && f.ToLower().Contains("itinerary"));
            }
            else
            {
                txtMsiPath.Text = string.Empty;
                txtMsiFileName.Text = string.Empty;
                btnInstall.Enabled = false;
                btnRemove.Enabled = false;
                btnStop.Enabled = false;
                btnStart.Enabled = false;
                btnRecycle.Enabled = false;
            }
        }

        private void btnShowMsi_Click(object sender, EventArgs e)
        {
            string msiFile = null;
            if (rbtnAdapter.Checked) return;

            msiFile = GetMsiFilePath();
            if (null == msiFile)
            {
                btnInstall.Enabled = false;
                btnRemove.Enabled = false;
                btnStop.Enabled = false;
                btnStart.Enabled = false;
                btnRecycle.Enabled = false;
                txtMsiPath.Clear();
                txtMsiFileName.Clear();
                return;
            }

            LoadingBox frm = new LoadingBox(msiFile);
            frm.Show(this);
            Application.DoEvents();
            frmInstalledPackages form = new frmInstalledPackages(msiFile);
            frm.Close();
            form.ShowDialog(this);
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters.Add("server", GetDbServerName());
            //parameters.Add("database", "BizTalkMgmtDb");
            //parameters.Add("application", cboApp.SelectedValue.ToString());
            //parameters.Add("msi", txtMsiPath.Text + "\\" + txtMsiFileName.Text);
            //// Install the selected msi via Powershell scripting
            ////var results = ExecuteStudioShellExecuteCommand("d:\\biztalk\\powershell\\getprocess.ps1");
            //Powershell ps = new Powershell();
            //ps.Log += new LogEventHandler(psLog);
            ////ps.GetProcess();
            ////ps.TestOutputAndError();
            ////ps.TestWithPipeline();
            ////ps.TestWithPipelineWithInput();
            //ps.TestDeployScript();
            ////ps.RunScript(this, "d:\\biztalk\\powershell\\deployapplication.ps1", parameters);

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            // We want to undeploy the application
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // We want to undeploy the application
        }

        private void btnPowershell_Click(object sender, EventArgs e)
        {
            //rtxtLog.Clear();
            // Start a powershell command or script
            //var results = Shell.Execute("Get-Process");
            //foreach (PSObject result in results)
            //    rtxtLog.Text += string.Format(
            //            "{0,-24}{1}",
            //            result.Members["ProcessName"].Value,
            //            result.Members["Id"].Value) + Environment.NewLine;
            //rtxtLog.ScrollToBottom();
        }

        void psLog(object sender, LogEventArgs e)
        {
            Color foreColor = rtxtLog.ForeColor;
            rtxtLog.SuspendLayout();
            switch (e.Type)
            {
                case System.Diagnostics.EventLogEntryType.Error:
                    rtxtLog.SelectionColor = Color.Red;
                    break;
                default:
                    rtxtLog.SelectionColor = foreColor;
                    break;
            }
            rtxtLog.AppendText(e.Message + Environment.NewLine);
            rtxtLog.ScrollToBottom();
            rtxtLog.ResumeLayout();
        }

        private void rbtnFull_CheckedChanged(object sender, EventArgs e)
        {
            btnInstall.Enabled = false;
            btnRemove.Enabled = false;
            btnStop.Enabled = false;
            btnStart.Enabled = false;
            btnRecycle.Enabled = false;
            if (rbtnAdapter.Checked)
            {
                if (lblApps.Text != "Adapter")
                {
                    lblApps.Text = "Adapter";
                    cboApp.DataSource = GetAdapters();
                }
                if (null == cboApp.SelectedValue) return;
                cboVersion.DataSource = new List<string>() { " " };
            }
            else
            {
                if (rbtnPatch.Checked)
                {
                    chkDeploySSO.Checked = false;
                    chkDeployItineraries.Checked = false;
                }
                else
                {
                    chkDeploySSO.Checked = true;
                    chkDeployItineraries.Checked = true;
                }

                if (lblApps.Text != "Application")
                {
                    lblApps.Text = "Application";
                    cboApp.DataSource = GetApplications();
                }
                if (null == cboApp.SelectedValue) return;
                cboVersion.DataSource = GetVersions(cboApp.SelectedValue.ToString(), rbtnPatch.Checked);
            }
        }

        private void rbtnLoc_CheckedChanged(object sender, EventArgs e)
        {
            RecycleButton.Enabled = true;
            txtServer.Text = GetDbServerName();
            txtDatabase.Text = GetDbName();
        }

        protected override bool ProcessCmdKey(
                       ref Message msg,
                       Keys keyData)
        {
            bool bHandled = false;
            // switch case is the easy way, a hash or map would be better,  
            // but more work to get set up. 
            switch (keyData)
            {
                case Keys.F5:
                    // do whatever 
                    rbtnFull_CheckedChanged(this, new EventArgs());
                    txtMsiPath.Clear();
                    txtMsiFileName.Clear();
                    bHandled = true;
                    break;
            }
            return bHandled;
        }

        private void chkLogging_CheckedChanged(object sender, EventArgs e)
        {
            // Refresh the status bar log info
            string filename;
            GetLogFilename(out filename);
        }

    }
}
