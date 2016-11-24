namespace DeploymentDbFormsControlLibrary
{
    partial class VersionWarning
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDbVersion = new System.Windows.Forms.TextBox();
            this.bindingSourceVersion = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.txtVersionNew = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVersionCurrent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtApplicationName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceVersion)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtDbVersion, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtVersionNew, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtVersionCurrent, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnYes, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnNo, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtApplicationName, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(472, 145);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 29);
            this.label5.TabIndex = 9;
            this.label5.Text = "DeploymentDb Version";
            // 
            // txtDbVersion
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtDbVersion, 2);
            this.txtDbVersion.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceVersion, "DbVersion", true));
            this.txtDbVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDbVersion.Location = new System.Drawing.Point(144, 61);
            this.txtDbVersion.Name = "txtDbVersion";
            this.txtDbVersion.ReadOnly = true;
            this.txtDbVersion.Size = new System.Drawing.Size(326, 20);
            this.txtDbVersion.TabIndex = 10;
            // 
            // bindingSourceVersion
            // 
            this.bindingSourceVersion.DataSource = typeof(bizilante.Windows.Forms.DeploymentDbFormsControlLibrary.DeploymentVersion);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 29);
            this.label4.TabIndex = 7;
            this.label4.Text = "New Version";
            // 
            // txtVersionNew
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtVersionNew, 2);
            this.txtVersionNew.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceVersion, "NewVersion", true));
            this.txtVersionNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVersionNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersionNew.Location = new System.Drawing.Point(144, 90);
            this.txtVersionNew.Name = "txtVersionNew";
            this.txtVersionNew.ReadOnly = true;
            this.txtVersionNew.Size = new System.Drawing.Size(326, 20);
            this.txtVersionNew.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "Current Version";
            // 
            // txtVersionCurrent
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtVersionCurrent, 2);
            this.txtVersionCurrent.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceVersion, "Version", true));
            this.txtVersionCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVersionCurrent.Location = new System.Drawing.Point(144, 32);
            this.txtVersionCurrent.Name = "txtVersionCurrent";
            this.txtVersionCurrent.ReadOnly = true;
            this.txtVersionCurrent.Size = new System.Drawing.Size(326, 20);
            this.txtVersionCurrent.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Continue?";
            // 
            // btnYes
            // 
            this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(144, 120);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(87, 21);
            this.btnYes.TabIndex = 2;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            // 
            // btnNo
            // 
            this.btnNo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(310, 120);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(87, 21);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "BizTalk Application";
            // 
            // txtApplicationName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtApplicationName, 2);
            this.txtApplicationName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourceVersion, "Application", true));
            this.txtApplicationName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtApplicationName.Location = new System.Drawing.Point(144, 3);
            this.txtApplicationName.Name = "txtApplicationName";
            this.txtApplicationName.ReadOnly = true;
            this.txtApplicationName.Size = new System.Drawing.Size(326, 20);
            this.txtApplicationName.TabIndex = 4;
            // 
            // VersionWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(472, 145);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "VersionWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Deployment Version Warning";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceVersion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVersionNew;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtVersionCurrent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtApplicationName;
        private System.Windows.Forms.BindingSource bindingSourceVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDbVersion;
    }
}