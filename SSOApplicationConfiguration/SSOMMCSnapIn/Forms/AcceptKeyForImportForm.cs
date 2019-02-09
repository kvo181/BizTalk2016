using bizilante.ManagementConsole.SSO.PropertyPages;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO.Forms
{
    internal class AcceptKeyForImportForm : Form
    {
        private EncryptionKey_PropertyPage _propertyPage;
        private IContainer components;
        private TabPage tabPage1;
        private TabControl tabControl1;
        private Button OkButton;
        private new Button CancelButton;
        private Panel panel1;

        public AcceptKeyForImportForm(EncryptionKey_PropertyPage propertyPage) : this()
        {
            this._propertyPage = propertyPage;
            if (this._propertyPage != null)
            {
                this.Text = string.Format("Enter Key For {0}", this._propertyPage.ScopeNodeAction);
                this.panel1.Controls.Add(this._propertyPage.Control);
                return;
            }
            throw new InvalidOperationException("EncryptionKey PropertyPage Cannot be null.");
        }

        public AcceptKeyForImportForm()
        {
            this.InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this._propertyPage.Save())
            {
                base.Close();
                return;
            }
            string text = "Error importing the application.  Please check the event log for further information";
            string caption = "Import Application Error";
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            base.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabPage1 = new TabPage();
            this.panel1 = new Panel();
            this.OkButton = new Button();
            this.CancelButton = new Button();
            this.tabControl1 = new TabControl();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            base.SuspendLayout();
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.OkButton);
            this.tabPage1.Controls.Add(this.CancelButton);
            this.tabPage1.Location = new Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(341, 223);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Enter Key";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.panel1.Location = new Point(4, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(334, 159);
            this.panel1.TabIndex = 0;
            this.OkButton.Location = new Point(63, 182);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new Size(75, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new EventHandler(this.OkButton_Click);
            this.CancelButton.Location = new Point(192, 182);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new EventHandler(this.CancelButton_Click);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(349, 249);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            base.AcceptButton = this.OkButton;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(373, 277);
            base.Controls.Add(this.tabControl1);
            base.Name = "AcceptKeyForImportForm";
            this.Text = "Enter Key For Export";
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}
