using bizilante.ManagementConsole.SSO.PropertyPages;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO.UserControls
{
    internal class Add_KeyValue_PropertiesControl : UserControl
    {
        private Add_KeyValue_PropertyPage add_KeyValue_PropertyPage;
        private IContainer components;
        private GroupBox UserInfo;
        private TextBox txtKeyName;
        private Label KeyPrompt;
        private TextBox txtKeyValue;
        private Label ValuePrompt;

        public Add_KeyValue_PropertiesControl(Add_KeyValue_PropertyPage parentPropertyPage)
        {
            this.InitializeComponent();
            this.add_KeyValue_PropertyPage = parentPropertyPage;
        }

        public void RefreshData(ResultNode userNode)
        {
            this.txtKeyName.Text = userNode.DisplayName;
            this.txtKeyValue.Text = userNode.SubItemDisplayNames[0];
            this.add_KeyValue_PropertyPage.Dirty = false;
        }

        public void UpdateData(ResultNode userNode)
        {
            userNode.DisplayName = this.txtKeyName.Text;
            userNode.SubItemDisplayNames[0] = this.txtKeyValue.Text;
            this.add_KeyValue_PropertyPage.Dirty = false;
        }

        public void GetKeyValue(out string strkey, out string strValue)
        {
            strkey = this.txtKeyName.Text;
            strValue = this.txtKeyValue.Text;
        }

        public void KeyExist(string strKeyname)
        {
            MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
            messageBoxParameters.Caption = "Key/Value Pair";
            messageBoxParameters.Text = "The key " + strKeyname + " already exists";
            this.add_KeyValue_PropertyPage.ParentSheet.ShowDialog(messageBoxParameters);
        }

        public bool CheckText()
        {
            return this.txtKeyName.Text == "" || this.txtKeyValue.Text == "";
        }

        public bool CanApplyChanges()
        {
            bool result = false;
            if (this.txtKeyName.Text.Trim().Length == 0)
            {
                MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                messageBoxParameters.Caption = "Key/Value Pair";
                messageBoxParameters.Text = "The key cannot be blank";
                this.add_KeyValue_PropertyPage.ParentSheet.ShowDialog(messageBoxParameters);
            }
            else if (this.txtKeyValue.Text.Trim().Length == 0)
            {
                MessageBoxParameters messageBoxParameters2 = new MessageBoxParameters();
                messageBoxParameters2.Caption = "Key/Value Pair";
                messageBoxParameters2.Text = "The value cannot be blank";
                this.add_KeyValue_PropertyPage.ParentSheet.ShowDialog(messageBoxParameters2);
            }
            else
            {
                result = true;
            }
            return result;
        }

        private void txtKeyName_TextChanged(object sender, EventArgs e)
        {
            this.add_KeyValue_PropertyPage.Dirty = true;
        }

        private void txtKeyValue_TextChanged(object sender, EventArgs e)
        {
            this.add_KeyValue_PropertyPage.Dirty = true;
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
            this.UserInfo = new GroupBox();
            this.txtKeyName = new TextBox();
            this.KeyPrompt = new Label();
            this.txtKeyValue = new TextBox();
            this.ValuePrompt = new Label();
            this.UserInfo.SuspendLayout();
            base.SuspendLayout();
            this.UserInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.UserInfo.Controls.Add(this.txtKeyName);
            this.UserInfo.Controls.Add(this.KeyPrompt);
            this.UserInfo.Controls.Add(this.txtKeyValue);
            this.UserInfo.Controls.Add(this.ValuePrompt);
            this.UserInfo.Location = new Point(11, 20);
            this.UserInfo.Name = "UserInfo";
            this.UserInfo.Size = new Size(518, 75);
            this.UserInfo.TabIndex = 5;
            this.UserInfo.TabStop = false;
            this.UserInfo.Text = "Key/Value Pair";
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
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.UserInfo);
            base.Name = "Add_KeyValue_PropertiesControl";
            base.Size = new Size(545, 115);
            this.UserInfo.ResumeLayout(false);
            this.UserInfo.PerformLayout();
            base.ResumeLayout(false);
        }
    }
}
