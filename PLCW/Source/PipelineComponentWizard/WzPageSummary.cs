using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public class WzPageSummary : Microsoft.BizTalk.Wizard.WizardCompletionPage, IWizardControl
	{
		private System.ComponentModel.IContainer components = null;
        private PictureBox pictureBox1;
		private string _Summary = null;

		public WzPageSummary()
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
			get {	return true;	}
		}

		public string Summary
		{
			get {	return _Summary;	}
			set 
			{
				_Summary = value;
				textBoxSubTitle.Text = Summary;
			}

		}
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageSummary));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            // 
            // checkBoxRunAgain
            // 
            resources.ApplyResources(this.checkBoxRunAgain, "checkBoxRunAgain");
            // 
            // labelNavigation
            // 
            resources.ApplyResources(this.labelNavigation, "labelNavigation");
            // 
            // textBoxSubTitle
            // 
            resources.ApplyResources(this.textBoxSubTitle, "textBoxSubTitle");
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.Properties.Resources.WizardGlyph;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // WzPageSummary
            // 
            this.Controls.Add(this.pictureBox1);
            this.Name = "WzPageSummary";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "The pipeline component wizard will create the following projects:";
            this.Title = "BizTalk Server Pipeline Component Wizard";
            this.Load += new System.EventHandler(this.WzPageSummary_Load);
            this.VisibleChanged += new System.EventHandler(this.WzPageSummary_VisibleChanged);
            this.Controls.SetChildIndex(this.labelTitle, 0);
            this.Controls.SetChildIndex(this.checkBoxRunAgain, 0);
            this.Controls.SetChildIndex(this.labelNavigation, 0);
            this.Controls.SetChildIndex(this.textBoxSubTitle, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void WzPageSummary_Load(object sender, System.EventArgs e)
		{
		}

		private void WzPageSummary_VisibleChanged(object sender, System.EventArgs e)
		{
		}
	}
}

