namespace bizilante.BuildGenerator.Tasks
{
    partial class VersionForm
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
            this.numericUpDownMajor = new System.Windows.Forms.NumericUpDown();
            this.lblMajorVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownMinor = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownBuild = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownRevision = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMajor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBuild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownMajor
            // 
            this.numericUpDownMajor.Enabled = false;
            this.numericUpDownMajor.Location = new System.Drawing.Point(77, 12);
            this.numericUpDownMajor.Name = "numericUpDownMajor";
            this.numericUpDownMajor.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownMajor.TabIndex = 0;
            // 
            // lblMajorVersion
            // 
            this.lblMajorVersion.AutoSize = true;
            this.lblMajorVersion.Location = new System.Drawing.Point(22, 14);
            this.lblMajorVersion.Name = "lblMajorVersion";
            this.lblMajorVersion.Size = new System.Drawing.Size(39, 13);
            this.lblMajorVersion.TabIndex = 1;
            this.lblMajorVersion.Text = "Major: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Minor: ";
            // 
            // numericUpDownMinor
            // 
            this.numericUpDownMinor.Enabled = false;
            this.numericUpDownMinor.Location = new System.Drawing.Point(77, 38);
            this.numericUpDownMinor.Name = "numericUpDownMinor";
            this.numericUpDownMinor.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownMinor.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Build: ";
            // 
            // numericUpDownBuild
            // 
            this.numericUpDownBuild.Enabled = false;
            this.numericUpDownBuild.Location = new System.Drawing.Point(77, 64);
            this.numericUpDownBuild.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownBuild.Name = "numericUpDownBuild";
            this.numericUpDownBuild.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownBuild.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Revision: ";
            // 
            // numericUpDownRevision
            // 
            this.numericUpDownRevision.Enabled = false;
            this.numericUpDownRevision.Location = new System.Drawing.Point(77, 90);
            this.numericUpDownRevision.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownRevision.Name = "numericUpDownRevision";
            this.numericUpDownRevision.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownRevision.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 135);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(153, 135);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // VersionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 170);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownRevision);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownBuild);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownMinor);
            this.Controls.Add(this.lblMajorVersion);
            this.Controls.Add(this.numericUpDownMajor);
            this.Name = "VersionForm";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assembly File Version";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VersionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMajor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBuild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRevision)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownMajor;
        private System.Windows.Forms.Label lblMajorVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownMinor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownBuild;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownRevision;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}