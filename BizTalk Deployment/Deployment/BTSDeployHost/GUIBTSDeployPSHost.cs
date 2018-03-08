using System;
using System.Configuration;
using System.Text;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Host;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Specialized;
using bizilante.Helpers.LogDeployment;
using bizilante.Deployment.BTSDeployHost.Host;

namespace bizilante.Deployment.BTSDeployHost
{
    public sealed class GUIBTSDeployPSHost : PSHost
    {
        private static readonly string PsRoot = ConfigurationManager.AppSettings["PSRootPath"];
        private static readonly string ToolsFolder = ConfigurationManager.AppSettings["BizTalkToolsFolder"];
        private static readonly string LogsFolder = ConfigurationManager.AppSettings["BizTalkLogsFolder"];
        private static readonly string BizTalkDomain = ConfigurationManager.AppSettings["BizTalkDomain"];
        private static readonly string BizTalkTmpInstall = ConfigurationManager.AppSettings["BizTalkTmpInstall"];

        // private data
        private Guid instanceId;
        private Version version;
        private const string privateData = "gui host private data";
        private BTSDeployForm.DeployForm gui;
        private Runspace runspace;
        private PSHostUserInterface _UI;
        private bool _workDone = false;

        public GUIBTSDeployPSHost(BTSDeployForm.DeployForm form)
            : base()
        {
            gui = form;
            gui.InvokeButton.Click += new EventHandler(InvokeButton_Click);
            gui.RemoveButton.Click += new EventHandler(RemoveButton_Click);
            gui.StopButton.Click += new EventHandler(StopButton_Click);
            gui.StartButton.Click += new EventHandler(StartButton_Click);
            gui.RecycleButton.Click += new EventHandler(RecycleButton_Click);
            Logging.Log += new Logging.LogEventHandler(Logging_Log);
            instanceId = Guid.NewGuid();
            version = new Version("1.0.0.0");
        }

        public void Initialize()
        {
            runspace = RunspaceFactory.CreateRunspace(this);
            runspace.Open();
        }
        private void InvokeButton_Click(object sender, EventArgs e)
        {
            if (!gui.IsAdapter())
            {
                // We need to display a warning when the tobe deployed version is older that the currently deployed version.
                // They have to confirm this, before we continue!
                string dbversion = Helpers.LogDeployment.Utils.GetApplicationVersion(gui.GetEnv(), gui.GetApplicationName());
                string version = "0.0.0.0";
                try
                {
                    Helpers.BizTalkGroupHelper.BizTalkGroup grp = new Helpers.BizTalkGroupHelper.BizTalkGroup(gui.GetDbServerName(), gui.GetDbName());
                    version = grp.GetApplicationVersion(gui.GetApplicationName());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (!gui.ValidVersion(version, dbversion)) return;
            }

            // State the reason first!
            gui.EnterNote();

            gui.OutputTextBox.Clear();
            gui.OutputTextBox.Text = "Start Install...\n";
            // disable invoke button to make sure only 1 command is running
            gui.InvokeButton.Enabled = false;
            gui.RemoveButton.Enabled = false;
            gui.StartButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.RecycleButton.Enabled = false;

            // Bug 05/11/2012: was previously "Remove".
            gui.SetActionOnly("Install");

            string parameters;
            if (gui.IsAdapter())
            {
                parameters =
                    string.Format("-server:'{0}' -database:'{1}' -msi:'{2}'",
                        gui.GetDbServerName(), gui.GetDbName(), gui.GetMsiAdapterFilePath());
            }
            else
            {
                string logOption = string.Empty;
                string logFilename;
                if (gui.GetLogFilename(out logFilename))
                    logOption = string.Format("-logfile:'{0}'", logFilename);
                if (null == gui.GetEnv())
                    parameters =
                        string.Format("-server:'{0}' -database:'{1}' -application:'{2}' -msi:'{3}' {4}",
                            gui.GetDbServerName(), gui.GetDbName(), gui.GetApplicationName(), gui.GetMsiFilePath(), logOption);
                else
                    parameters =
                        string.Format("-server:'{0}' -database:'{1}' -application:'{2}' -msi:'{3}' -env:{4} {5}",
                            gui.GetDbServerName(), gui.GetDbName(), gui.GetApplicationName(), gui.GetMsiFilePath(), gui.GetEnv(), logOption);
            }
            Pipeline pipeline;
            if (gui.IsAdapter())
                pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; Set-Variable BizTalkPSFolder {3} -scope global ; Set-Variable BizTalkDomain {4} -scope global ; Set-Variable BizTalkTmpInstall {5} -scope global ; Set-Variable BizTalkLogsFolder {6} -scope global ; .\deployadapter.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder, PsRoot, BizTalkDomain, BizTalkTmpInstall, LogsFolder));
            else if (!gui.IsPatch())
                pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; Set-Variable BizTalkPSFolder {3} -scope global ; Set-Variable BizTalkDomain {4} -scope global ; Set-Variable BizTalkTmpInstall {5} -scope global ; Set-Variable BizTalkLogsFolder {6} -scope global ; Set-Variable BizTalkDeploySSO {7} -scope global ; Set-Variable BizTalkDeployItinerary {8} -scope global ; .\deployapplication.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder, PsRoot, BizTalkDomain, BizTalkTmpInstall, LogsFolder, gui.GetDeploySSO(), gui.GetDeployItinerary()));
            else
                pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; Set-Variable BizTalkPSFolder {3} -scope global ; Set-Variable BizTalkDomain {4} -scope global ; Set-Variable BizTalkTmpInstall {5} -scope global ; Set-Variable BizTalkLogsFolder {6} -scope global ; Set-Variable BizTalkDeploySSO {7} -scope global ; Set-Variable BizTalkDeployItinerary {8} -scope global ; .\deployapplicationpatch.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder, PsRoot, BizTalkDomain, BizTalkTmpInstall, LogsFolder, gui.GetDeploySSO(), gui.GetDeployItinerary()));
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-default");
            pipeline.Input.Close();
            pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
            _workDone = true;
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // State the reason first!
            gui.EnterNote();

            gui.OutputTextBox.Clear();
            gui.OutputTextBox.Text = "Start Remove...\n";
            // disable invoke button to make sure only 1 
            // command is running
            gui.InvokeButton.Enabled = false;
            gui.RemoveButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.RecycleButton.Enabled = false;

            gui.SetActionOnly("Remove");

            string envOption = string.Empty;
            if (null != gui.GetEnv()) envOption = string.Format("-env:{0}", gui.GetEnv());

            string logOption = string.Empty;
            string logFilename;
            if (gui.GetLogFilename(out logFilename))
                logOption = string.Format("-logfile:'{0}'", logFilename);

            var parameters =
                string.Format("-server:'{0}' -database:'{1}' -application:'{2}' -version:'{3}' {4} {5}",
                    gui.GetDbServerName(), gui.GetDbName(), gui.GetApplicationName(), gui.GetVersion(), envOption, logOption);

            var pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; Set-Variable BizTalkPSFolder {3} -scope global ; Set-Variable BizTalkLogsFolder {4} -scope global ; .\undeployapplication.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder, PsRoot, LogsFolder));
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-default");
            pipeline.Input.Close();
            pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
            _workDone = true;
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            // State the reason first!
            gui.EnterNote();

            gui.OutputTextBox.Clear();
            gui.OutputTextBox.Text = "Stopping application...\n";
            // disable invoke button to make sure only 1 
            // command is running
            gui.InvokeButton.Enabled = false;
            gui.RemoveButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.StartButton.Enabled = false;
            gui.RecycleButton.Enabled = false;

            gui.SetActionOnly("Stop");

            string envOption = string.Empty;
            if (null != gui.GetEnv()) envOption = string.Format("-env:{0}", gui.GetEnv());

            string logOption = string.Empty;
            string logFilename;
            if (gui.GetLogFilename(out logFilename))
                logOption = string.Format("-logfile:'{0}'", logFilename);

            var parameters =
                string.Format("-server:'{0}' -database:'{1}' -application:'{2}' -version:'{3}' {4} {5}",
                    gui.GetDbServerName(), gui.GetDbName(), gui.GetApplicationName(), gui.GetVersion(), envOption, logOption);

            var pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; .\stopapplication.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder));
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-default");
            pipeline.Input.Close();
            pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
            _workDone = true;
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            // State the reason first!
            gui.EnterNote();

            gui.OutputTextBox.Clear();
            gui.OutputTextBox.Text = "Starting application...\n";
            // disable invoke button to make sure only 1 
            // command is running
            gui.InvokeButton.Enabled = false;
            gui.RemoveButton.Enabled = false;
            gui.StartButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.RecycleButton.Enabled = false;

            gui.SetActionOnly("Start");

            string envOption = string.Empty;
            if (null != gui.GetEnv()) envOption = string.Format("-env:{0}", gui.GetEnv());

            string logOption = string.Empty;
            string logFilename;
            if (gui.GetLogFilename(out logFilename))
                logOption = string.Format("-logfile:'{0}'", logFilename);

            var parameters =
                string.Format("-server:'{0}' -database:'{1}' -application:'{2}' -version:'{3}' {4} {5}",
                    gui.GetDbServerName(), gui.GetDbName(), gui.GetApplicationName(), gui.GetVersion(), envOption, logOption);

            var pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; .\startapplication.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder));
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-default");
            pipeline.Input.Close();
            pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
            _workDone = true;
        }
        private void RecycleButton_Click(object sender, EventArgs e)
        {
            // State the reason first!
            gui.EnterNote();

            gui.OutputTextBox.Clear();
            gui.OutputTextBox.Text = "Recycling BizTalk Server...\n";
            // disable invoke button to make sure only 1 
            // command is running
            gui.InvokeButton.Enabled = false;
            gui.RemoveButton.Enabled = false;
            gui.StopButton.Enabled = false;
            gui.StartButton.Enabled = false;
            gui.RecycleButton.Enabled = false;

            gui.SetActionOnly("Recycle");

            string envOption = string.Format("-env:{0}", gui.GetEnv());

            string logOption = string.Empty;
            string logFilename;
            if (gui.GetLogFilename(out logFilename))
                logOption = string.Format("-logfile:'{0}'", logFilename);

            var parameters =
                string.Format("-server:'{0}' -database:'{1}' {2} {3}",
                    gui.GetDbServerName(), gui.GetDbName(), envOption, logOption);

            var pipeline = runspace.CreatePipeline(string.Format(@"cd {0}; prompt ; Write-Host 'Execution policy = ' ( Get-ExecutionPolicy ) ; $DebugPreference = 'Continue' ; Set-Variable BizTalkToolsFolder {2} -scope global ; Set-Variable BizTalkPSFolder {3} -scope global ; Set-Variable BizTalkDomain {4} -scope global ; Set-Variable BizTalkTmpInstall {5} -scope global ; Set-Variable BizTalkLogsFolder {6} -scope global ; .\Admin\Stop_Start_BizTalk.ps1 {1}", PsRoot, parameters.Trim(), ToolsFolder, PsRoot, BizTalkDomain, BizTalkTmpInstall, LogsFolder));
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-default");
            pipeline.Input.Close();
            pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
            _workDone = true;
        }

        private void pipeline_StateChanged(object sender, PipelineStateEventArgs e)
        {
            Pipeline source = sender as Pipeline;
            // if the command completed update GUI.
            bool updateGUI = false;
            bool done = false;
            StringBuilder output = new StringBuilder();

            if (e.PipelineStateInfo.State == PipelineState.Completed)
            {
                done = true;
                updateGUI = true;
                Collection<PSObject> results = source.Output.ReadToEnd();
                foreach (PSObject result in results)
                {
                    string resultString =
                    (string)LanguagePrimitives.ConvertTo(result, typeof(string));
                    output.Append(resultString);
                }
                output.Append("\nDone!");

                Failed = false;
                ErrorMessage = string.Empty;

                // Do we need to log the result?
                LogInfo(Failed, ErrorMessage);
            }
            else if ((e.PipelineStateInfo.State == PipelineState.Stopped) ||
                     (e.PipelineStateInfo.State == PipelineState.Failed))
            {
                done = true;
                updateGUI = true;
                Collection<PSObject> results = source.Output.ReadToEnd();
                foreach (PSObject result in results)
                {
                    string resultString =
                    (string)LanguagePrimitives.ConvertTo(result, typeof(string));
                    output.Append(resultString);
                }
                output.Append("\nFailure!");

                Failed = true;
                ErrorMessage = string.Format("Command did not complete successfully. Reason: {0}", e.PipelineStateInfo.Reason.Message);

                // Do we need to log the result?
                LogInfo(Failed, ErrorMessage);
            }

            if (updateGUI)
            {
                BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate optDelegate =
                    new BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate(gui.SetOutputTextBoxContent);
                gui.OutputTextBox.Invoke(optDelegate, new object[] { output.ToString() });
                BTSDeployForm.DeployForm.SetRemoveButtonStateDelegate removeBtnDelegate =
                    new BTSDeployForm.DeployForm.SetRemoveButtonStateDelegate(gui.SetRemoveButtonState);
                gui.RemoveButton.Invoke(removeBtnDelegate, new object[] { false });
                BTSDeployForm.DeployForm.SetInvokeButtonStateDelegate invkBtnDelegate =
                    new BTSDeployForm.DeployForm.SetInvokeButtonStateDelegate(gui.SetInvokeButtonState);
                gui.InvokeButton.Invoke(invkBtnDelegate, new object[] { false });
                BTSDeployForm.DeployForm.SetStopButtonStateDelegate stpBtnDelegate =
                    new BTSDeployForm.DeployForm.SetStopButtonStateDelegate(gui.SetStopButtonState);
                gui.StopButton.Invoke(stpBtnDelegate, new object[] { false });
                BTSDeployForm.DeployForm.SetStartButtonStateDelegate strtBtnDelegate =
                    new BTSDeployForm.DeployForm.SetStartButtonStateDelegate(gui.SetStartButtonState);
                gui.StartButton.Invoke(strtBtnDelegate, new object[] { false });
                BTSDeployForm.DeployForm.SetRecycleButtonStateDelegate recycleBtnDelegate =
                    new BTSDeployForm.DeployForm.SetRecycleButtonStateDelegate(gui.SetRecycleButtonState);
                gui.RecycleButton.Invoke(recycleBtnDelegate, new object[] { false });
            }
            if (done)
            {
                BTSDeployForm.DeployForm.SetActionDoneDelegate doneDelegate =
                    new BTSDeployForm.DeployForm.SetActionDoneDelegate(gui.SetActionDone);
                gui.Form.Invoke(doneDelegate, new object[] {  });
            }

        }
        private void Logging_Log(LogEventArgs e)
        {
            BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate optDelegate =
                new BTSDeployForm.DeployForm.SetOutputTextBoxContentDelegate(gui.SetOutputTextBoxContent);
            gui.OutputTextBox.Invoke(optDelegate, new object[] { e.Message + Environment.NewLine });
            BTSDeployForm.DeployForm.SetToolStripStatusLabelDelegate stripDelegate =
                new BTSDeployForm.DeployForm.SetToolStripStatusLabelDelegate(gui.SetToolStripStatusLabel);
            gui.StatusStrip.Invoke(stripDelegate, new object[] { e.Message });
        }
        private void LogInfo(bool failed, string error)
        {
            string filename;
            if (gui.GetLogFilename(out filename))
            {
                try
                {
                    Logging.Execute(gui.GetAction(), gui.GetAppToLog(), gui.GetVersionToLog(), gui.GetNote(), gui.GetMsiPathToLog(), filename, gui.GetLogInfo(), failed, error);
                }
                catch (Exception ex)
                {
                    Failed = true;
                    ErrorMessage = "Logging failed due to: " + ex.Message;
                }
            }
        }

        public override Guid InstanceId
        {
            get { return instanceId; }
        }
        public override string Name
        {
            get { return "PS.Deploy.Host"; }
        }
        public override Version Version
        {
            get { return version; }
        }
        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }
        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }
        public override PSObject PrivateData
        {
            get
            {
                return PSObject.AsPSObject(privateData);
            }
        }
        public override void EnterNestedPrompt()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override void ExitNestedPrompt()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override void NotifyBeginApplication()
        {
            return;
        }
        public override void NotifyEndApplication()
        {
            return;
        }
        public override void SetShouldExit(int exitCode)
        {
            string message = string.Format("Exiting with exit code: {0}", exitCode);
            MessageBox.Show(message);
            runspace.CloseAsync();
            Application.Exit();
        }
        public override PSHostUserInterface UI
        {
            get
            {
                if (null == _UI) _UI = new HostUI(gui);
                return _UI;
            }
        }
        public bool Failed { get; set; }
        public string ErrorMessage { get; set; }

        [STAThread]
        public static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BTSDeployForm.DeployForm form = new BTSDeployForm.DeployForm();
            form.SetLogging(true, LogsFolder); // by default we enable logging
            GUIBTSDeployPSHost host = new GUIBTSDeployPSHost(form);
            host.Initialize();
            Application.Run(form);
            if (host.Failed) throw new Exception(host.ErrorMessage);
            if (!host._workDone) throw new Exception("Nothing done!");
        }
        [STAThread]
        public static void Run(NameValueCollection nameValueArgs)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BTSDeployForm.DeployForm form = new BTSDeployForm.DeployForm();
            // We need to pass the info
            form.SetType(nameValueArgs["Type"]);
            form.SetInstallOn(nameValueArgs["Environment"]);
            form.SetApplication(nameValueArgs["Application"]);
            form.SetAdapter(nameValueArgs["Adapter"]);
            form.SetVersion(nameValueArgs["Version"]);
            if (!string.IsNullOrEmpty(nameValueArgs["Log"]))
                form.SetLogging(Convert.ToBoolean(nameValueArgs["Log"]), LogsFolder);
            else
                form.SetLogging(true, LogsFolder); // by default we enable logging

            // We need to set the action as last operation on the form.
            form.SetAction(nameValueArgs["Action"]);

            // Initialize the Powershell host
            GUIBTSDeployPSHost host = new GUIBTSDeployPSHost(form);
            host.Initialize();
            Application.Run(form);
            if (host.Failed) throw new Exception(host.ErrorMessage);
            if (!host._workDone) throw new Exception("Nothing done!");
        }
    }
}
