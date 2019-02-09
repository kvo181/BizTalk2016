using bizilante.ManagementConsole.SSO.PropertyPages;
using Microsoft.ManagementConsole.Advanced;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO.UserControls
{
    internal class EncryptionKey_PropertiesControl : UserControl
    {
        private IContainer components;
        private GroupBox groupBox1;
        private TextBox txtKey;
        private Label label1;
        private Label label2;
        private EncryptionKey_PropertyPage encryptionKey_PropertyPage;

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
            this.groupBox1 = new GroupBox();
            this.label1 = new Label();
            this.txtKey = new TextBox();
            this.label2 = new Label();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtKey);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new Point(19, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(280, 133);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import/Export Encryption Key";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(22, 32);
            this.label1.Name = "label1";
            this.label1.Size = new Size(209, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "The key you provide on export must be the";
            this.txtKey.Location = new Point(25, 91);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new Size(215, 20);
            this.txtKey.TabIndex = 1;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(22, 48);
            this.label2.Name = "label2";
            this.label2.Size = new Size(159, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "same key that you use for import";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.groupBox1);
            base.Name = "EncryptionKey_PropertiesControl";
            base.Size = new Size(328, 180);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }

        public EncryptionKey_PropertiesControl(EncryptionKey_PropertyPage parentPropertyPage)
        {
            this.InitializeComponent();
            this.encryptionKey_PropertyPage = parentPropertyPage;
        }

        public void GetKeyValue(out string strEncryptionkey)
        {
            strEncryptionkey = this.txtKey.Text;
        }

        public bool CheckText()
        {
            return this.txtKey.Text.Length == 0;
        }

        public bool CanApplyChanges()
        {
            bool result = false;
            if (this.txtKey.Text.Trim().Length == 0)
            {
                MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                messageBoxParameters.Caption = "Key/Value Pair";
                messageBoxParameters.Text = "The key cannot be blank";
                this.encryptionKey_PropertyPage.ParentSheet.ShowDialog(messageBoxParameters);
            }
            else
            {
                result = true;
            }
            return result;
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            this.encryptionKey_PropertyPage.Dirty = true;
        }
    }
}
