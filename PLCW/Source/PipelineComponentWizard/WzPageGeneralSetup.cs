using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public class WzPageGeneralSetup : Microsoft.BizTalk.Wizard.WizardInteriorPage, IWizardControl
	{
		private const string TransportRegEx = @"^[_a-zA-Z][_a-zA-Z0-9]*$";
		private const string NamespaceRegEx = @"(?i)^([a-z].?)*$";
		public event AddWizardResultEvent _AddWizardResultEvent;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ErrorProvider ErrProv;
		private System.Windows.Forms.TextBox txtClassName;
		private System.Windows.Forms.TextBox txtNameSpace;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cboPipelineType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboComponentStage;
		private System.Windows.Forms.CheckBox chkImplementIProbeMessage;
		private System.Windows.Forms.ComboBox cboImplementationLanguage;
		private System.Windows.Forms.Label label5;
		private System.ComponentModel.IContainer components = null;

		public WzPageGeneralSetup()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// re-clear all items from the stage dropdown
			cboComponentStage.Items.Clear();
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

		/// <summary>
		/// The protected OnRaiseProperty method raises the event by invoking 
		/// the delegates. The sender is always this, the current instance 
		/// of the class.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnAddWizardResult(PropertyPairEvent e)
		{
			if (e != null) 
			{
				// Invokes the delegates. 
				_AddWizardResultEvent(this,e);
			}
		}

		/// <summary>
		/// Returns true if all fields are correctly entered
		/// </summary>
		/// <returns></returns>
		private bool GetAllStates()
		{
			return (txtClassName.Text.Length > 0 && Regex.IsMatch(txtClassName.Text,TransportRegEx) &&
				txtNameSpace.Text.Length > 0 && Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) && 
				cboComponentStage.SelectedIndex > -1 && cboPipelineType.SelectedIndex > -1);
		}

		private void WzPageGeneralSetup_Leave(object sender, System.EventArgs e)
		{
			try
			{
				AddWizardResult(WizardValues.ClassName, txtClassName.Text);
				AddWizardResult(WizardValues.Namespace, txtNameSpace.Text);
				AddWizardResult(WizardValues.PipelineType, cboPipelineType.Items[cboPipelineType.SelectedIndex].ToString());
				AddWizardResult(WizardValues.ComponentStage, cboComponentStage.Items[cboComponentStage.SelectedIndex].ToString());
				AddWizardResult(WizardValues.ImplementationLanguage, (implementationLanguages) cboImplementationLanguage.SelectedIndex);
				AddWizardResult(WizardValues.ImplementIProbeMessage, chkImplementIProbeMessage.Checked);
			}
			catch(Exception err)
			{
#if DEBUG
				MessageBox.Show(err.Message);
#endif
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
		}

		private void txtClassName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Regex.IsMatch(txtClassName.Text,TransportRegEx) && txtClassName.Text.Length > 0)
			{
				ErrProv.SetError(txtClassName,
					"TransportType must start with a non-alphanumeric character and may only include special character '_'");
			}
			else
			{
				ErrProv.SetError(txtClassName,"");
			}		
			EnableNext(GetAllStates());	
		}

		private void txtNamespace_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) && txtNameSpace.Text.Length > 0)
			{
				ErrProv.SetError(txtNameSpace,
					"Namespace must be a valid identifier");
			}
			else
			{
				ErrProv.SetError(txtNameSpace,"");
			}
			EnableNext(GetAllStates());	
		}

		public bool NextButtonEnabled
		{
			get {	return GetAllStates();	}
		}

		public bool NeedSummary
		{
			get {	return false;	}
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageGeneralSetup));
            this.label1 = new System.Windows.Forms.Label();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.ErrProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboComponentStage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboPipelineType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkImplementIProbeMessage = new System.Windows.Forms.CheckBox();
            this.cboImplementationLanguage = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).BeginInit();
            this.SuspendLayout();
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
            // txtClassName
            // 
            resources.ApplyResources(this.txtClassName, "txtClassName");
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtClassName.Validating += new System.ComponentModel.CancelEventHandler(this.txtClassName_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtNameSpace
            // 
            resources.ApplyResources(this.txtNameSpace, "txtNameSpace");
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtNameSpace.Validating += new System.ComponentModel.CancelEventHandler(this.txtNamespace_Validating);
            // 
            // ErrProv
            // 
            this.ErrProv.ContainerControl = this;
            resources.ApplyResources(this.ErrProv, "ErrProv");
            // 
            // cboComponentStage
            // 
            resources.ApplyResources(this.cboComponentStage, "cboComponentStage");
            this.cboComponentStage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboComponentStage.Name = "cboComponentStage";
            this.cboComponentStage.SelectedIndexChanged += new System.EventHandler(this.cboComponentStage_Changed);
            this.cboComponentStage.SelectedValueChanged += new System.EventHandler(this.Element_Changed);
            this.cboComponentStage.Validating += new System.ComponentModel.CancelEventHandler(this.cboComponentStage_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cboPipelineType
            // 
            this.cboPipelineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboPipelineType, "cboPipelineType");
            this.cboPipelineType.Items.AddRange(new object[] {
            resources.GetString("cboPipelineType.Items"),
            resources.GetString("cboPipelineType.Items1"),
            resources.GetString("cboPipelineType.Items2")});
            this.cboPipelineType.Name = "cboPipelineType";
            this.cboPipelineType.SelectedIndexChanged += new System.EventHandler(this.PipelineType_Changed);
            this.cboPipelineType.SelectedValueChanged += new System.EventHandler(this.Element_Changed);
            this.cboPipelineType.Validating += new System.ComponentModel.CancelEventHandler(this.cboPipelineType_Validating);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // chkImplementIProbeMessage
            // 
            resources.ApplyResources(this.chkImplementIProbeMessage, "chkImplementIProbeMessage");
            this.chkImplementIProbeMessage.Name = "chkImplementIProbeMessage";
            // 
            // cboImplementationLanguage
            // 
            resources.ApplyResources(this.cboImplementationLanguage, "cboImplementationLanguage");
            this.cboImplementationLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImplementationLanguage.Items.AddRange(new object[] {
            resources.GetString("cboImplementationLanguage.Items"),
            resources.GetString("cboImplementationLanguage.Items1")});
            this.cboImplementationLanguage.Name = "cboImplementationLanguage";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // WzPageGeneralSetup
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboImplementationLanguage);
            this.Controls.Add(this.chkImplementIProbeMessage);
            this.Controls.Add(this.cboPipelineType);
            this.Controls.Add(this.cboComponentStage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtClassName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Name = "WzPageGeneralSetup";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify generic component properties";
            this.Title = "General setup";
            this.Leave += new System.EventHandler(this.WzPageGeneralSetup_Leave);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.txtClassName, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtNameSpace, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cboComponentStage, 0);
            this.Controls.SetChildIndex(this.cboPipelineType, 0);
            this.Controls.SetChildIndex(this.chkImplementIProbeMessage, 0);
            this.Controls.SetChildIndex(this.cboImplementationLanguage, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void PipelineType_Changed(object sender, System.EventArgs e)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WzPageGeneralSetup));

			this.cboComponentStage.Items.Clear();
			this.cboComponentStage.Enabled = true;

			switch(cboPipelineType.SelectedIndex)
			{
				// do we have a receive pipeline component selected?
				case 0:
					this.cboComponentStage.Items.AddRange(new object[] 
					{
						componentTypes.Decoder.ToString(),
						componentTypes.DisassemblingParser.ToString(),
						componentTypes.Validate.ToString(),
						componentTypes.PartyResolver.ToString(),
						componentTypes.Any.ToString()
					});
					this.cboComponentStage.SelectedIndex = 0;
					break;
				case 1:
					this.cboComponentStage.Items.AddRange(new object[] 
					{
						componentTypes.Encoder.ToString(),
						componentTypes.AssemblingSerializer.ToString(),
						componentTypes.Any.ToString()
					});
					this.cboComponentStage.SelectedIndex = 0;
					break;
				case 2:
					this.cboComponentStage.Items.Add(componentTypes.Any.ToString());
					this.cboComponentStage.Enabled = false;
					this.cboComponentStage.SelectedIndex = 0;
					break;
				default:
					throw new ApplicationException("Unsupported pipeline type selected");
			}
		}

		private void cboPipelineType_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void cboComponentStage_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void Element_Changed(object sender, System.EventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void cboComponentStage_Changed(object sender, System.EventArgs e)
		{
			// do we have a disassembler selected?
			// only disassemblers can implement IProbeMessage
			if(cboComponentStage.Items[cboComponentStage.SelectedIndex].ToString() == componentTypes.DisassemblingParser.ToString())
			{
				chkImplementIProbeMessage.Visible = true;
			}
			else 
			{
				chkImplementIProbeMessage.Visible = false;
				chkImplementIProbeMessage.Checked = false;
			}
		}
	}
}