namespace HarvestBug
{
    partial class BotControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLogin = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblCounter = new System.Windows.Forms.Label();
            this.numericMax = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMax)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblLogin);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 54);
            this.panel1.TabIndex = 0;
            // 
            // lblLogin
            // 
            this.lblLogin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(3, 20);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(35, 13);
            this.lblLogin.TabIndex = 0;
            this.lblLogin.Text = "label1";
            this.lblLogin.Click += new System.EventHandler(this.lblLogin_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tableLayoutPanel1.Controls.Add(this.progress, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCounter, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericMax, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(177, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(218, 54);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // progress
            // 
            this.progress.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.progress.Location = new System.Drawing.Point(115, 15);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(100, 23);
            this.progress.TabIndex = 5;
            // 
            // lblCounter
            // 
            this.lblCounter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCounter.AutoSize = true;
            this.lblCounter.Location = new System.Drawing.Point(66, 20);
            this.lblCounter.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(35, 13);
            this.lblCounter.TabIndex = 4;
            this.lblCounter.Text = "label3";
            this.lblCounter.TextChanged += new System.EventHandler(this.lblCounter_TextChanged);
            // 
            // numericMax
            // 
            this.numericMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericMax.Location = new System.Drawing.Point(3, 17);
            this.numericMax.Name = "numericMax";
            this.numericMax.Size = new System.Drawing.Size(37, 20);
            this.numericMax.TabIndex = 3;
            this.numericMax.ValueChanged += new System.EventHandler(this.numericMax_ValueChanged);
            this.numericMax.Leave += new System.EventHandler(this.numericMax_Leave);
            // 
            // BotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "BotControl";
            this.Size = new System.Drawing.Size(395, 54);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMax)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numericMax;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lblCounter;
    }
}
