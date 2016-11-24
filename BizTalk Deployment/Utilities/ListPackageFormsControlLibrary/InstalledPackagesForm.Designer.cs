namespace bizilante.Windows.Forms.Controls.ListPackageFormsControlLibrary
{
    partial class frmInstalledPackages
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
            this.userControlInstalledPackages1 = new UserControlInstalledPackages();
            this.SuspendLayout();
            // 
            // userControlInstalledPackages1
            // 
            this.userControlInstalledPackages1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlInstalledPackages1.FileNames = null;
            this.userControlInstalledPackages1.Location = new System.Drawing.Point(0, 0);
            this.userControlInstalledPackages1.Name = "userControlInstalledPackages1";
            this.userControlInstalledPackages1.Size = new System.Drawing.Size(985, 359);
            this.userControlInstalledPackages1.TabIndex = 0;
            // 
            // frmInstalledPackages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 359);
            this.Controls.Add(this.userControlInstalledPackages1);
            this.Name = "frmInstalledPackages";
            this.Text = "BizTalk Installation Package Contents";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControlInstalledPackages userControlInstalledPackages1;
    }
}

