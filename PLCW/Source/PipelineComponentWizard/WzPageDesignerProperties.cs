using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public class WzPageDesignerProperties : Microsoft.BizTalk.Wizard.WizardInteriorPage, IWizardControl
	{
		public event AddDesignerPropertyEvent _AddDesignerPropertyEvent; 
		private bool _IsLoaded = false;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox txtDesignerProperty;
		private System.Windows.Forms.Button cmdDesignerPropertyDel;
		private System.Windows.Forms.Button cmdDesignerPropertyAdd;
		private System.Windows.Forms.ComboBox cmbDesignerPropertyDataType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox lstDesignerProperties;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblHelpDesignerProperties;
		private System.ComponentModel.IContainer components = null;

		public WzPageDesignerProperties()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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

		public bool NextButtonEnabled
		{
			get {	return true;	}
		}

		public bool NeedSummary
		{
			get {	return false;	}
		}

		protected void AddDesignerProperty(string strName, string strValue)
		{
			PropertyPairEvent PropertyPair = new PropertyPairEvent(strName, strValue);
			OnAddDesignerProperty(PropertyPair);
		}

		protected void RemoveDesignerProperty(string strName)
		{
			PropertyPairEvent PropertyPair = new PropertyPairEvent(strName, null, true);
			OnAddDesignerProperty(PropertyPair);
		}

		// The protected OnAddReceiveHandlerProperty method raises the event by invoking 
		// the delegates. The sender is always this, the current instance 
		// of the class.
		protected virtual void OnAddDesignerProperty(PropertyPairEvent e)
		{
			if (e != null) 
			{
				// Invokes the delegates. 
				_AddDesignerPropertyEvent(this, e);
			}
		}
		
		private void WzPageDesignerProperties_Leave(object sender, System.EventArgs e)
		{
			try
			{
				foreach(object objItem in lstDesignerProperties.Items)
				{
					string strVal = objItem.ToString();

					string strPropName = strVal.Substring(0,strVal.IndexOf("(") - 1);

					string strPropType = strVal.Replace(strPropName + " (", string.Empty);
					strPropType = strPropType.Replace(")", string.Empty);

					AddDesignerProperty(strPropName, strPropType);
				}
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}		
		}

		private void WzPageDesignerProperties_Load(object sender, System.EventArgs e)
		{
			try
			{
				if(_IsLoaded)
					return;

				foreach(string strDataType in DesignerVariableType.ToArray())
				{
					cmbDesignerPropertyDataType.Items.Add(strDataType);
				}
				//cmbDesignerPropertyDataType.Text = strDataTypes[0];
				cmbDesignerPropertyDataType.SelectedIndex = 0;

				_IsLoaded = true;
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
			cmbDesignerPropertyDataType.Focus();
		}

		private bool VarNameAlreadyExists(string strValue)
		{
			foreach(object o in lstDesignerProperties.Items)
			{
				string strObjVal = o.ToString();
				strObjVal = strObjVal.Remove(strObjVal.IndexOf(" ("),strObjVal.Length - strObjVal.IndexOf(" ("));
				if (strObjVal == strValue)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Resets all of the errorproviders when anything succeeds
		/// </summary>
		private void ResetAllErrProviders()
		{
			foreach(Control ctl in this.Controls)
			{
				errorProvider.SetError(ctl, "");
			}
		}

		private void cmdRecvHandlerDel_Click(object sender, System.EventArgs e)
		{
			try
			{
				ResetAllErrProviders();
				if (lstDesignerProperties.SelectedItem == null)
				{
					errorProvider.SetError(cmdDesignerPropertyDel,
						"Please select a value in the property list");
					return;
				}

				Object objItem = lstDesignerProperties.SelectedItem;
				string strVal = objItem.ToString();
				string strPropName = strVal.Substring(0,strVal.IndexOf("(") - 1);
				RemoveDesignerProperty(strPropName);
				lstDesignerProperties.Items.Remove(lstDesignerProperties.SelectedItem);

			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}	
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageDesignerProperties));
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtDesignerProperty = new System.Windows.Forms.TextBox();
            this.cmdDesignerPropertyDel = new System.Windows.Forms.Button();
            this.cmdDesignerPropertyAdd = new System.Windows.Forms.Button();
            this.cmbDesignerPropertyDataType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstDesignerProperties = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHelpDesignerProperties = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
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
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // txtDesignerProperty
            // 
            resources.ApplyResources(this.txtDesignerProperty, "txtDesignerProperty");
            this.txtDesignerProperty.Name = "txtDesignerProperty";
            // 
            // cmdDesignerPropertyDel
            // 
            resources.ApplyResources(this.cmdDesignerPropertyDel, "cmdDesignerPropertyDel");
            this.cmdDesignerPropertyDel.Name = "cmdDesignerPropertyDel";
            this.cmdDesignerPropertyDel.Click += new System.EventHandler(this.cmdDesignerPropertyDel_Click);
            // 
            // cmdDesignerPropertyAdd
            // 
            resources.ApplyResources(this.cmdDesignerPropertyAdd, "cmdDesignerPropertyAdd");
            this.cmdDesignerPropertyAdd.Name = "cmdDesignerPropertyAdd";
            this.cmdDesignerPropertyAdd.Click += new System.EventHandler(this.cmdDesignerPropertyAdd_Click);
            // 
            // cmbDesignerPropertyDataType
            // 
            this.cmbDesignerPropertyDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbDesignerPropertyDataType, "cmbDesignerPropertyDataType");
            this.cmbDesignerPropertyDataType.Name = "cmbDesignerPropertyDataType";
            this.cmbDesignerPropertyDataType.SelectedIndexChanged += new System.EventHandler(this.cmbDesignerPropertyDataType_Changed);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lstDesignerProperties
            // 
            resources.ApplyResources(this.lstDesignerProperties, "lstDesignerProperties");
            this.lstDesignerProperties.Name = "lstDesignerProperties";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblHelpDesignerProperties
            // 
            resources.ApplyResources(this.lblHelpDesignerProperties, "lblHelpDesignerProperties");
            this.lblHelpDesignerProperties.Name = "lblHelpDesignerProperties";
            // 
            // WzPageDesignerProperties
            // 
            this.Controls.Add(this.lblHelpDesignerProperties);
            this.Controls.Add(this.txtDesignerProperty);
            this.Controls.Add(this.cmdDesignerPropertyDel);
            this.Controls.Add(this.cmdDesignerPropertyAdd);
            this.Controls.Add(this.cmbDesignerPropertyDataType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstDesignerProperties);
            this.Controls.Add(this.label1);
            this.Name = "WzPageDesignerProperties";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify design-time variables";
            this.Title = "Design-time variables";
            this.Load += new System.EventHandler(this.WzPageDesignerProperties_Load);
            this.Leave += new System.EventHandler(this.WzPageDesignerProperties_Leave);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lstDesignerProperties, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cmbDesignerPropertyDataType, 0);
            this.Controls.SetChildIndex(this.cmdDesignerPropertyAdd, 0);
            this.Controls.SetChildIndex(this.cmdDesignerPropertyDel, 0);
            this.Controls.SetChildIndex(this.txtDesignerProperty, 0);
            this.Controls.SetChildIndex(this.lblHelpDesignerProperties, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void cmdDesignerPropertyAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				ResetAllErrProviders();
				if (!Regex.IsMatch(txtDesignerProperty.Text,@"^[_a-zA-Z][_a-zA-Z0-9]*$"))
				{
					errorProvider.SetError(txtDesignerProperty,
						"Please enter a valid name for the new property");
					return;
				}
				if (VarNameAlreadyExists(txtDesignerProperty.Text))
				{
					errorProvider.SetError(txtDesignerProperty,
						"Please enter a unique name. No two properties can have the same name");
					return;
				}
				lstDesignerProperties.Items.Add(txtDesignerProperty.Text + " (" + cmbDesignerPropertyDataType.Text + ")");
				txtDesignerProperty.Clear();
				cmbDesignerPropertyDataType.Text = "string";

			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
		
		}

		private void cmbDesignerPropertyDataType_Changed(object sender, System.EventArgs e)
		{
			string currentSelection = cmbDesignerPropertyDataType.Items[cmbDesignerPropertyDataType.SelectedIndex].ToString();
			if(currentSelection == "SchemaList")
			{
				lblHelpDesignerProperties.Text = "SchemaList allows for a dialog to pick any number of referenced schemas";
				lblHelpDesignerProperties.Visible = true;
			} 
			else if(currentSelection == "SchemaWithNone")
			{
				lblHelpDesignerProperties.Text = "SchemaWithNone allows for a dropdown listbox with referenced schemas, selecting one only";
				lblHelpDesignerProperties.Visible = true;
			}
			else
			{
				lblHelpDesignerProperties.Visible = false;
			}
		}

        private void cmdDesignerPropertyDel_Click(object sender, EventArgs e)
        {
            try
            {
                ResetAllErrProviders();

                if (lstDesignerProperties.SelectedIndex == -1) return;

                lstDesignerProperties.Items.RemoveAt(lstDesignerProperties.SelectedIndex);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }
		
        }
	}
}

