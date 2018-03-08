namespace bizilante.Deployment.BTSDeployForm
{
    partial class DeployForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpDeploy = new System.Windows.Forms.GroupBox();
            this.chkDeployItineraries = new System.Windows.Forms.CheckBox();
            this.chkDeploySSO = new System.Windows.Forms.CheckBox();
            this.grpLogging = new System.Windows.Forms.GroupBox();
            this.chkLogging = new System.Windows.Forms.CheckBox();
            this.btnShowMsi = new System.Windows.Forms.Button();
            this.grpInstallOn = new System.Windows.Forms.GroupBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.grpAction = new System.Windows.Forms.GroupBox();
            this.btnRecycle = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.grpInstall = new System.Windows.Forms.GroupBox();
            this.rbtnDev = new System.Windows.Forms.RadioButton();
            this.rbtnLoc = new System.Windows.Forms.RadioButton();
            this.rbtnHfx = new System.Windows.Forms.RadioButton();
            this.rbtnEdu = new System.Windows.Forms.RadioButton();
            this.rbtnPrd = new System.Windows.Forms.RadioButton();
            this.rbtnTst = new System.Windows.Forms.RadioButton();
            this.cboVersion = new System.Windows.Forms.ComboBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.grpMsi = new System.Windows.Forms.GroupBox();
            this.txtMsiFileName = new System.Windows.Forms.TextBox();
            this.txtMsiPath = new System.Windows.Forms.TextBox();
            this.btnGetMsi = new System.Windows.Forms.Button();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.rbtnAdapter = new System.Windows.Forms.RadioButton();
            this.rbtnPatch = new System.Windows.Forms.RadioButton();
            this.rbtnFull = new System.Windows.Forms.RadioButton();
            this.lblApps = new System.Windows.Forms.Label();
            this.cboApp = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialogMsi = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpDeploy.SuspendLayout();
            this.grpLogging.SuspendLayout();
            this.grpInstallOn.SuspendLayout();
            this.grpAction.SuspendLayout();
            this.grpInstall.SuspendLayout();
            this.grpMsi.SuspendLayout();
            this.grpType.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpDeploy);
            this.splitContainer1.Panel1.Controls.Add(this.grpLogging);
            this.splitContainer1.Panel1.Controls.Add(this.btnShowMsi);
            this.splitContainer1.Panel1.Controls.Add(this.grpInstallOn);
            this.splitContainer1.Panel1.Controls.Add(this.grpAction);
            this.splitContainer1.Panel1.Controls.Add(this.grpInstall);
            this.splitContainer1.Panel1.Controls.Add(this.cboVersion);
            this.splitContainer1.Panel1.Controls.Add(this.lblVersion);
            this.splitContainer1.Panel1.Controls.Add(this.grpMsi);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetMsi);
            this.splitContainer1.Panel1.Controls.Add(this.grpType);
            this.splitContainer1.Panel1.Controls.Add(this.lblApps);
            this.splitContainer1.Panel1.Controls.Add(this.cboApp);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(894, 558);
            this.splitContainer1.SplitterDistance = 182;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpDeploy
            // 
            this.grpDeploy.Controls.Add(this.chkDeployItineraries);
            this.grpDeploy.Controls.Add(this.chkDeploySSO);
            this.grpDeploy.Location = new System.Drawing.Point(97, 117);
            this.grpDeploy.Name = "grpDeploy";
            this.grpDeploy.Size = new System.Drawing.Size(174, 58);
            this.grpDeploy.TabIndex = 14;
            this.grpDeploy.TabStop = false;
            this.grpDeploy.Text = "Deploy";
            // 
            // chkDeployItineraries
            // 
            this.chkDeployItineraries.AutoSize = true;
            this.chkDeployItineraries.Checked = true;
            this.chkDeployItineraries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeployItineraries.Location = new System.Drawing.Point(99, 26);
            this.chkDeployItineraries.Name = "chkDeployItineraries";
            this.chkDeployItineraries.Size = new System.Drawing.Size(71, 17);
            this.chkDeployItineraries.TabIndex = 1;
            this.chkDeployItineraries.Text = "Itineraries";
            this.chkDeployItineraries.UseVisualStyleBackColor = true;
            // 
            // chkDeploySSO
            // 
            this.chkDeploySSO.AutoSize = true;
            this.chkDeploySSO.Checked = true;
            this.chkDeploySSO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeploySSO.Location = new System.Drawing.Point(13, 26);
            this.chkDeploySSO.Name = "chkDeploySSO";
            this.chkDeploySSO.Size = new System.Drawing.Size(48, 17);
            this.chkDeploySSO.TabIndex = 0;
            this.chkDeploySSO.Text = "SSO";
            this.chkDeploySSO.UseVisualStyleBackColor = true;
            // 
            // grpLogging
            // 
            this.grpLogging.Controls.Add(this.chkLogging);
            this.grpLogging.Location = new System.Drawing.Point(10, 117);
            this.grpLogging.Name = "grpLogging";
            this.grpLogging.Size = new System.Drawing.Size(81, 58);
            this.grpLogging.TabIndex = 13;
            this.grpLogging.TabStop = false;
            this.grpLogging.Text = "Logging";
            // 
            // chkLogging
            // 
            this.chkLogging.AutoSize = true;
            this.chkLogging.Checked = true;
            this.chkLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogging.Location = new System.Drawing.Point(23, 26);
            this.chkLogging.Name = "chkLogging";
            this.chkLogging.Size = new System.Drawing.Size(59, 17);
            this.chkLogging.TabIndex = 0;
            this.chkLogging.Text = "Enable";
            this.chkLogging.UseVisualStyleBackColor = true;
            this.chkLogging.CheckedChanged += new System.EventHandler(this.chkLogging_CheckedChanged);
            // 
            // btnShowMsi
            // 
            this.btnShowMsi.Location = new System.Drawing.Point(591, 6);
            this.btnShowMsi.Name = "btnShowMsi";
            this.btnShowMsi.Size = new System.Drawing.Size(75, 23);
            this.btnShowMsi.TabIndex = 12;
            this.btnShowMsi.Text = "Show Msi";
            this.btnShowMsi.UseVisualStyleBackColor = true;
            this.btnShowMsi.Click += new System.EventHandler(this.btnShowMsi_Click);
            // 
            // grpInstallOn
            // 
            this.grpInstallOn.Controls.Add(this.txtDatabase);
            this.grpInstallOn.Controls.Add(this.lblDatabase);
            this.grpInstallOn.Controls.Add(this.txtServer);
            this.grpInstallOn.Controls.Add(this.lblServer);
            this.grpInstallOn.Location = new System.Drawing.Point(278, 110);
            this.grpInstallOn.Name = "grpInstallOn";
            this.grpInstallOn.Size = new System.Drawing.Size(509, 65);
            this.grpInstallOn.TabIndex = 11;
            this.grpInstallOn.TabStop = false;
            this.grpInstallOn.Text = "Install On:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.BackColor = System.Drawing.SystemColors.Window;
            this.txtDatabase.Location = new System.Drawing.Point(75, 39);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(422, 20);
            this.txtDatabase.TabIndex = 3;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(12, 40);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(56, 13);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "Database:";
            // 
            // txtServer
            // 
            this.txtServer.BackColor = System.Drawing.SystemColors.Window;
            this.txtServer.Location = new System.Drawing.Point(75, 19);
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(422, 20);
            this.txtServer.TabIndex = 1;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // grpAction
            // 
            this.grpAction.Controls.Add(this.btnRecycle);
            this.grpAction.Controls.Add(this.btnStart);
            this.grpAction.Controls.Add(this.btnInstall);
            this.grpAction.Controls.Add(this.btnStop);
            this.grpAction.Controls.Add(this.btnRemove);
            this.grpAction.Location = new System.Drawing.Point(794, 6);
            this.grpAction.Name = "grpAction";
            this.grpAction.Size = new System.Drawing.Size(89, 169);
            this.grpAction.TabIndex = 10;
            this.grpAction.TabStop = false;
            this.grpAction.Text = "Action";
            // 
            // btnRecycle
            // 
            this.btnRecycle.Enabled = false;
            this.btnRecycle.Location = new System.Drawing.Point(7, 137);
            this.btnRecycle.Name = "btnRecycle";
            this.btnRecycle.Size = new System.Drawing.Size(75, 23);
            this.btnRecycle.TabIndex = 4;
            this.btnRecycle.Text = "Recycle";
            this.btnRecycle.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(7, 107);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.Location = new System.Drawing.Point(6, 19);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(6, 77);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(6, 48);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // grpInstall
            // 
            this.grpInstall.Controls.Add(this.rbtnDev);
            this.grpInstall.Controls.Add(this.rbtnLoc);
            this.grpInstall.Controls.Add(this.rbtnHfx);
            this.grpInstall.Controls.Add(this.rbtnEdu);
            this.grpInstall.Controls.Add(this.rbtnPrd);
            this.grpInstall.Controls.Add(this.rbtnTst);
            this.grpInstall.Location = new System.Drawing.Point(116, 28);
            this.grpInstall.Name = "grpInstall";
            this.grpInstall.Size = new System.Drawing.Size(155, 90);
            this.grpInstall.TabIndex = 1;
            this.grpInstall.TabStop = false;
            this.grpInstall.Text = "Install On";
            // 
            // rbtnDev
            // 
            this.rbtnDev.AutoSize = true;
            this.rbtnDev.Enabled = false;
            this.rbtnDev.Location = new System.Drawing.Point(6, 42);
            this.rbtnDev.Name = "rbtnDev";
            this.rbtnDev.Size = new System.Drawing.Size(47, 17);
            this.rbtnDev.TabIndex = 0;
            this.rbtnDev.TabStop = true;
            this.rbtnDev.Text = "DEV";
            this.rbtnDev.UseVisualStyleBackColor = true;
            this.rbtnDev.Visible = false;
            this.rbtnDev.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // rbtnLoc
            // 
            this.rbtnLoc.AutoSize = true;
            this.rbtnLoc.Enabled = false;
            this.rbtnLoc.Location = new System.Drawing.Point(6, 20);
            this.rbtnLoc.Name = "rbtnLoc";
            this.rbtnLoc.Size = new System.Drawing.Size(46, 17);
            this.rbtnLoc.TabIndex = 5;
            this.rbtnLoc.TabStop = true;
            this.rbtnLoc.Text = "LOC";
            this.rbtnLoc.UseVisualStyleBackColor = true;
            this.rbtnLoc.Visible = false;
            this.rbtnLoc.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // rbtnHfx
            // 
            this.rbtnHfx.AutoSize = true;
            this.rbtnHfx.Enabled = false;
            this.rbtnHfx.Location = new System.Drawing.Point(78, 42);
            this.rbtnHfx.Name = "rbtnHfx";
            this.rbtnHfx.Size = new System.Drawing.Size(46, 17);
            this.rbtnHfx.TabIndex = 1;
            this.rbtnHfx.TabStop = true;
            this.rbtnHfx.Text = "HFX";
            this.rbtnHfx.UseVisualStyleBackColor = true;
            this.rbtnHfx.Visible = false;
            this.rbtnHfx.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // rbtnEdu
            // 
            this.rbtnEdu.AutoSize = true;
            this.rbtnEdu.Enabled = false;
            this.rbtnEdu.Location = new System.Drawing.Point(78, 19);
            this.rbtnEdu.Name = "rbtnEdu";
            this.rbtnEdu.Size = new System.Drawing.Size(48, 17);
            this.rbtnEdu.TabIndex = 4;
            this.rbtnEdu.TabStop = true;
            this.rbtnEdu.Text = "EDU";
            this.rbtnEdu.UseVisualStyleBackColor = true;
            this.rbtnEdu.Visible = false;
            this.rbtnEdu.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // rbtnPrd
            // 
            this.rbtnPrd.AutoSize = true;
            this.rbtnPrd.Enabled = false;
            this.rbtnPrd.Location = new System.Drawing.Point(78, 65);
            this.rbtnPrd.Name = "rbtnPrd";
            this.rbtnPrd.Size = new System.Drawing.Size(48, 17);
            this.rbtnPrd.TabIndex = 3;
            this.rbtnPrd.TabStop = true;
            this.rbtnPrd.Text = "PRD";
            this.rbtnPrd.UseVisualStyleBackColor = true;
            this.rbtnPrd.Visible = false;
            this.rbtnPrd.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // rbtnTst
            // 
            this.rbtnTst.AutoSize = true;
            this.rbtnTst.Enabled = false;
            this.rbtnTst.Location = new System.Drawing.Point(6, 65);
            this.rbtnTst.Name = "rbtnTst";
            this.rbtnTst.Size = new System.Drawing.Size(46, 17);
            this.rbtnTst.TabIndex = 2;
            this.rbtnTst.TabStop = true;
            this.rbtnTst.Text = "TST";
            this.rbtnTst.UseVisualStyleBackColor = true;
            this.rbtnTst.Visible = false;
            this.rbtnTst.CheckedChanged += new System.EventHandler(this.rbtnLoc_CheckedChanged);
            // 
            // cboVersion
            // 
            this.cboVersion.FormattingEnabled = true;
            this.cboVersion.Location = new System.Drawing.Point(333, 6);
            this.cboVersion.Name = "cboVersion";
            this.cboVersion.Size = new System.Drawing.Size(148, 21);
            this.cboVersion.TabIndex = 3;
            this.cboVersion.SelectedIndexChanged += new System.EventHandler(this.cboVersion_SelectedIndexChanged);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(284, 8);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(42, 13);
            this.lblVersion.TabIndex = 7;
            this.lblVersion.Text = "Version";
            // 
            // grpMsi
            // 
            this.grpMsi.Controls.Add(this.txtMsiFileName);
            this.grpMsi.Controls.Add(this.txtMsiPath);
            this.grpMsi.Location = new System.Drawing.Point(277, 28);
            this.grpMsi.Name = "grpMsi";
            this.grpMsi.Size = new System.Drawing.Size(510, 78);
            this.grpMsi.TabIndex = 6;
            this.grpMsi.TabStop = false;
            this.grpMsi.Text = "Msi to install";
            // 
            // txtMsiFileName
            // 
            this.txtMsiFileName.BackColor = System.Drawing.SystemColors.Window;
            this.txtMsiFileName.Location = new System.Drawing.Point(6, 45);
            this.txtMsiFileName.Name = "txtMsiFileName";
            this.txtMsiFileName.ReadOnly = true;
            this.txtMsiFileName.Size = new System.Drawing.Size(492, 20);
            this.txtMsiFileName.TabIndex = 1;
            // 
            // txtMsiPath
            // 
            this.txtMsiPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtMsiPath.Location = new System.Drawing.Point(6, 19);
            this.txtMsiPath.Name = "txtMsiPath";
            this.txtMsiPath.ReadOnly = true;
            this.txtMsiPath.Size = new System.Drawing.Size(492, 20);
            this.txtMsiPath.TabIndex = 0;
            // 
            // btnGetMsi
            // 
            this.btnGetMsi.Location = new System.Drawing.Point(510, 6);
            this.btnGetMsi.Name = "btnGetMsi";
            this.btnGetMsi.Size = new System.Drawing.Size(75, 23);
            this.btnGetMsi.TabIndex = 4;
            this.btnGetMsi.Text = "Get Msi";
            this.btnGetMsi.UseVisualStyleBackColor = true;
            this.btnGetMsi.Click += new System.EventHandler(this.btnGetMsi_Click);
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.rbtnAdapter);
            this.grpType.Controls.Add(this.rbtnPatch);
            this.grpType.Controls.Add(this.rbtnFull);
            this.grpType.Location = new System.Drawing.Point(10, 28);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(100, 78);
            this.grpType.TabIndex = 0;
            this.grpType.TabStop = false;
            this.grpType.Text = "Installation Type";
            // 
            // rbtnAdapter
            // 
            this.rbtnAdapter.AutoSize = true;
            this.rbtnAdapter.Location = new System.Drawing.Point(6, 55);
            this.rbtnAdapter.Name = "rbtnAdapter";
            this.rbtnAdapter.Size = new System.Drawing.Size(62, 17);
            this.rbtnAdapter.TabIndex = 4;
            this.rbtnAdapter.TabStop = true;
            this.rbtnAdapter.Text = "Adapter";
            this.rbtnAdapter.UseVisualStyleBackColor = true;
            this.rbtnAdapter.CheckedChanged += new System.EventHandler(this.rbtnFull_CheckedChanged);
            // 
            // rbtnPatch
            // 
            this.rbtnPatch.AutoSize = true;
            this.rbtnPatch.Location = new System.Drawing.Point(6, 37);
            this.rbtnPatch.Name = "rbtnPatch";
            this.rbtnPatch.Size = new System.Drawing.Size(53, 17);
            this.rbtnPatch.TabIndex = 3;
            this.rbtnPatch.TabStop = true;
            this.rbtnPatch.Text = "Patch";
            this.rbtnPatch.UseVisualStyleBackColor = true;
            this.rbtnPatch.CheckedChanged += new System.EventHandler(this.rbtnFull_CheckedChanged);
            // 
            // rbtnFull
            // 
            this.rbtnFull.AutoSize = true;
            this.rbtnFull.Location = new System.Drawing.Point(6, 19);
            this.rbtnFull.Name = "rbtnFull";
            this.rbtnFull.Size = new System.Drawing.Size(41, 17);
            this.rbtnFull.TabIndex = 2;
            this.rbtnFull.TabStop = true;
            this.rbtnFull.Text = "Full";
            this.rbtnFull.UseVisualStyleBackColor = true;
            this.rbtnFull.CheckedChanged += new System.EventHandler(this.rbtnFull_CheckedChanged);
            // 
            // lblApps
            // 
            this.lblApps.AutoSize = true;
            this.lblApps.Location = new System.Drawing.Point(3, 9);
            this.lblApps.Name = "lblApps";
            this.lblApps.Size = new System.Drawing.Size(59, 13);
            this.lblApps.TabIndex = 1;
            this.lblApps.Text = "Application";
            // 
            // cboApp
            // 
            this.cboApp.FormattingEnabled = true;
            this.cboApp.Location = new System.Drawing.Point(68, 6);
            this.cboApp.Name = "cboApp";
            this.cboApp.Size = new System.Drawing.Size(199, 21);
            this.cboApp.TabIndex = 2;
            this.cboApp.SelectedIndexChanged += new System.EventHandler(this.cboApp_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtxtLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(894, 350);
            this.panel1.TabIndex = 1;
            // 
            // rtxtLog
            // 
            this.rtxtLog.AcceptsTab = true;
            this.rtxtLog.BackColor = System.Drawing.SystemColors.Window;
            this.rtxtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtLog.Location = new System.Drawing.Point(0, 0);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size(894, 350);
            this.rtxtLog.TabIndex = 0;
            this.rtxtLog.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 350);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(894, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(112, 17);
            this.toolStripStatusLabel.Text = "No logging enabled";
            this.toolStripStatusLabel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            // 
            // openFileDialogMsi
            // 
            this.openFileDialogMsi.Filter = "msi files|*.msi";
            this.openFileDialogMsi.Title = "BizTalk Application msi";
            // 
            // DeployForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 558);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DeployForm";
            this.Text = "BizTalk Deployment";
            this.Load += new System.EventHandler(this.DeployForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpDeploy.ResumeLayout(false);
            this.grpDeploy.PerformLayout();
            this.grpLogging.ResumeLayout(false);
            this.grpLogging.PerformLayout();
            this.grpInstallOn.ResumeLayout(false);
            this.grpInstallOn.PerformLayout();
            this.grpAction.ResumeLayout(false);
            this.grpInstall.ResumeLayout(false);
            this.grpInstall.PerformLayout();
            this.grpMsi.ResumeLayout(false);
            this.grpMsi.PerformLayout();
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cboVersion;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.GroupBox grpMsi;
        private System.Windows.Forms.TextBox txtMsiFileName;
        private System.Windows.Forms.TextBox txtMsiPath;
        private System.Windows.Forms.Button btnGetMsi;
        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton rbtnPatch;
        private System.Windows.Forms.RadioButton rbtnFull;
        private System.Windows.Forms.Label lblApps;
        private System.Windows.Forms.ComboBox cboApp;
        private System.Windows.Forms.OpenFileDialog openFileDialogMsi;
        private System.Windows.Forms.GroupBox grpInstall;
        private System.Windows.Forms.RadioButton rbtnEdu;
        private System.Windows.Forms.RadioButton rbtnPrd;
        private System.Windows.Forms.RadioButton rbtnTst;
        private System.Windows.Forms.RadioButton rbtnHfx;
        private System.Windows.Forms.RadioButton rbtnDev;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.RadioButton rbtnAdapter;
        private System.Windows.Forms.RadioButton rbtnLoc;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox grpAction;
        private System.Windows.Forms.GroupBox grpInstallOn;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Button btnShowMsi;
        private System.Windows.Forms.GroupBox grpLogging;
        private System.Windows.Forms.CheckBox chkLogging;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnRecycle;
        private System.Windows.Forms.GroupBox grpDeploy;
        private System.Windows.Forms.CheckBox chkDeployItineraries;
        private System.Windows.Forms.CheckBox chkDeploySSO;
    }
}

