using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public class WzPageGeneralProperties : Microsoft.BizTalk.Wizard.WizardInteriorPage, IWizardControl
	{
		public event AddWizardResultEvent _AddWizardResultEvent;

		private const string ComponentVersionRegEx = @"[0-9]+\.[0-9]+$";
		private const string ComponentNameRegEx = @"(?i)^[a-z]+[0-9a-z]*$";
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtComponentName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ErrorProvider ErrProv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.PictureBox ComponentIcon;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtComponentVersion;
		private System.Windows.Forms.TextBox txtComponentDescription;
		private System.ComponentModel.IContainer components = null;

		public WzPageGeneralProperties()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected void AddWizardResult(string strName, object Value)
		{
			PropertyPairEvent PropertyPair = new PropertyPairEvent(strName, Value);
			OnAddWizardResult(PropertyPair);
		}

		// The protected OnRaiseProperty method raises the event by invoking 
		// the delegates. The sender is always this, the current instance 
		// of the class.
		protected virtual void OnAddWizardResult(PropertyPairEvent e)
		{
			if (e != null) 
			{
				// Invokes the delegates. 
				_AddWizardResultEvent(this,e);
			}
		}

		public bool NextButtonEnabled
		{
			get {	return GetAllStates();	}
		}
		
		public bool NeedSummary
		{
			get {	return false;	}
		}

		private bool GetAllStates()
		{
			return (Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) &&
				Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx));
		}

		private void WzPageGeneralProperties_Leave(object sender, System.EventArgs e)
		{
			AddWizardResult(WizardValues.ComponentName, txtComponentName.Text);
			AddWizardResult(WizardValues.ComponentDescription, txtComponentDescription.Text);
			AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
			AddWizardResult(WizardValues.ComponentVersion, txtComponentVersion.Text);
		}
	
		private void ComponentIcon_DoubleClick(object sender, System.EventArgs e)
		{
			if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
			{
				ComponentIcon.Image = Image.FromFile(openFileDialog1.FileName);
				AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
			}
		}

		private void ComponentIcon_Click(object sender, System.EventArgs e)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WzPageGeneralProperties));
			this.ComponentIcon.Image = ((System.Drawing.Image)(resources.GetObject("ComponentIcon.Image")));
			AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageGeneralProperties));
            this.label1 = new System.Windows.Forms.Label();
            this.txtComponentName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComponentVersion = new System.Windows.Forms.TextBox();
            this.ErrProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtComponentDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ComponentIcon = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComponentIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            resources.ApplyResources(this.panelHeader, "panelHeader");
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            // 
            // labelSubTitle
            // 
            resources.ApplyResources(this.labelSubTitle, "labelSubTitle");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtComponentName
            // 
            resources.ApplyResources(this.txtComponentName, "txtComponentName");
            this.txtComponentName.Name = "txtComponentName";
            this.txtComponentName.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtComponentName.Validating += new System.ComponentModel.CancelEventHandler(this.txtComponentName_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtComponentVersion
            // 
            resources.ApplyResources(this.txtComponentVersion, "txtComponentVersion");
            this.txtComponentVersion.Name = "txtComponentVersion";
            this.txtComponentVersion.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtComponentVersion.Validating += new System.ComponentModel.CancelEventHandler(this.txtComponentVersion_Validating);
            // 
            // ErrProv
            // 
            this.ErrProv.ContainerControl = this;
            resources.ApplyResources(this.ErrProv, "ErrProv");
            // 
            // txtComponentDescription
            // 
            resources.ApplyResources(this.txtComponentDescription, "txtComponentDescription");
            this.txtComponentDescription.Name = "txtComponentDescription";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ComponentIcon
            // 
            resources.ApplyResources(this.ComponentIcon, "ComponentIcon");
            this.ComponentIcon.Name = "ComponentIcon";
            this.ComponentIcon.TabStop = false;
            this.ComponentIcon.Click += new System.EventHandler(this.ComponentIcon_Click);
            this.ComponentIcon.DoubleClick += new System.EventHandler(this.ComponentIcon_DoubleClick);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "bmp";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // WzPageGeneralProperties
            // 
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComponentIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtComponentVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtComponentName);
            this.Controls.Add(this.txtComponentDescription);
            this.Controls.Add(this.label2);
            this.Name = "WzPageGeneralProperties";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify Component UI settings";
            this.Title = "UI settings";
            this.Click += new System.EventHandler(this.ComponentIcon_Click);
            this.DoubleClick += new System.EventHandler(this.ComponentIcon_DoubleClick);
            this.Leave += new System.EventHandler(this.WzPageGeneralProperties_Leave);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.txtComponentDescription, 0);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.txtComponentName, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtComponentVersion, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.ComponentIcon, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComponentIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void txtComponentVersion_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(!Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) && txtComponentVersion.Text.Length > 0)
			{
				ErrProv.SetError(txtComponentVersion,
					"ComponentVersion can only contain numbers and a single '.' character, e.g.: 1.0, 1.15 or 10.234");
			}
			else
			{
				EnableNext(GetAllStates());
				ErrProv.SetError(txtComponentVersion, "");
			}		
		}

		private void txtComponentName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(!Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx) && txtComponentName.Text.Length > 0)
			{
				ErrProv.SetError(txtComponentName,
					"txtComponentName can only contain alpha numeric characters and cannot start with a number");
			}
			else
			{
				EnableNext(GetAllStates());
				ErrProv.SetError(txtComponentName, "");
			}		
		}

		private void Element_Changed(object sender, System.EventArgs e)
		{
			EnableNext(GetAllStates());
		}
	}
}

