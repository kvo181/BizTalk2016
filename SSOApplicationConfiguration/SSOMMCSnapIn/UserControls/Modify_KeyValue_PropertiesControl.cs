using bizilante.ManagementConsole.SSO.PropertyPages;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO.UserControls
{
    internal class Modify_KeyValue_PropertiesControl : UserControl
    {
        private ModifyPropertyPage userPropertyPage;
        private IContainer components;
        private TextBox txtKeyValue;
        private Label ValuePrompt;
        private TextBox txtKeyName;
        private Label KeyPrompt;
        private GroupBox UserInfo;

        public Modify_KeyValue_PropertiesControl(ModifyPropertyPage parentPropertyPage)
        {
            this.InitializeComponent();
            this.userPropertyPage = parentPropertyPage;
        }

        public void RefreshData(ResultNode userNode)
        {
            this.txtKeyName.Text = userNode.DisplayName;
            this.txtKeyValue.Text = userNode.SubItemDisplayNames[0];
            this.userPropertyPage.Dirty = false;
        }

        public void GetKeyValue(out string strKeyName, out string strValue)
        {
            strKeyName = this.txtKeyName.Text.Trim();
            strValue = this.txtKeyValue.Text.Trim();
        }

        public void UpdateData(ResultNode userNode)
        {
            userNode.DisplayName = this.txtKeyName.Text;
            userNode.SubItemDisplayNames[0] = this.txtKeyValue.Text;
            this.userPropertyPage.Dirty = false;
        }

        public bool CanApplyChanges()
        {
            bool result = false;
            if (this.txtKeyName.Text.Trim().Length == 0)
            {
                MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                messageBoxParameters.Caption = "Key/Value Pair";
                messageBoxParameters.Text = "The key cannot be blank";
                this.userPropertyPage.ParentSheet.ShowDialog(messageBoxParameters);
            }
            else if (this.txtKeyValue.Text.Trim().Length == 0)
            {
                MessageBoxParameters messageBoxParameters2 = new MessageBoxParameters();
                messageBoxParameters2.Caption = "Key/Value Pair";
                messageBoxParameters2.Text = "The value cannot be blank";
                this.userPropertyPage.ParentSheet.ShowDialog(messageBoxParameters2);
            }
            else
            {
                result = true;
            }
            return result;
        }

        public void KeyExistException()
        {
            MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
            messageBoxParameters.Caption = "Key/Value Pair";
            messageBoxParameters.Text = "The key " + this.txtKeyName.Text + " already exists";
            this.userPropertyPage.ParentSheet.ShowDialog(messageBoxParameters);
        }

        private void txtKeyName_TextChanged(object sender, EventArgs e)
        {
            this.userPropertyPage.Dirty = true;
        }

        private void txtKeyValue_TextChanged(object sender, EventArgs e)
        {
            this.userPropertyPage.Dirty = true;
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
            this.txtKeyValue = new TextBox();
            this.ValuePrompt = new Label();
            this.txtKeyName = new TextBox();
            this.KeyPrompt = new Label();
            this.UserInfo = new GroupBox();
            this.UserInfo.SuspendLayout();
            base.SuspendLayout();
            this.txtKeyValue.Location = new Point(88, 42);
            this.txtKeyValue.Name = "txtKeyValue";
            this.txtKeyValue.Size = new Size(411, 20);
            this.txtKeyValue.TabIndex = 1;
            this.txtKeyValue.TextChanged += new EventHandler(this.txtKeyValue_TextChanged);
            this.ValuePrompt.AutoSize = true;
            this.ValuePrompt.Location = new Point(22, 49);
            this.ValuePrompt.Name = "ValuePrompt";
            this.ValuePrompt.Size = new Size(34, 13);
            this.ValuePrompt.TabIndex = 1;
            this.ValuePrompt.Text = "Value";
            this.txtKeyName.Location = new Point(88, 16);
            this.txtKeyName.Name = "txtKeyName";
            this.txtKeyName.Size = new Size(411, 20);
            this.txtKeyName.TabIndex = 0;
            this.txtKeyName.TextChanged += new EventHandler(this.txtKeyName_TextChanged);
            this.KeyPrompt.AutoSize = true;
            this.KeyPrompt.Location = new Point(22, 23);
            this.KeyPrompt.Name = "KeyPrompt";
            this.KeyPrompt.Size = new Size(25, 13);
            this.KeyPrompt.TabIndex = 3;
            this.KeyPrompt.Text = "Key";
            this.UserInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.UserInfo.Controls.Add(this.txtKeyName);
            this.UserInfo.Controls.Add(this.KeyPrompt);
            this.UserInfo.Controls.Add(this.txtKeyValue);
            this.UserInfo.Controls.Add(this.ValuePrompt);
            this.UserInfo.Location = new Point(11, 18);
            this.UserInfo.Name = "UserInfo";
            this.UserInfo.Size = new Size(518, 75);
            this.UserInfo.TabIndex = 4;
            this.UserInfo.TabStop = false;
            this.UserInfo.Text = "Key/Value Pair";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.UserInfo);
            base.Name = "Modify_KeyValue_PropertiesControl";
            base.Size = new Size(545, 115);
            this.UserInfo.ResumeLayout(false);
            this.UserInfo.PerformLayout();
            base.ResumeLayout(false);
        }
    }
}
