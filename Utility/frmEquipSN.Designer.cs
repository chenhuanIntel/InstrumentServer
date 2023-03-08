
namespace Utility
{
    partial class frmEquipSN
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
            this.ScanSN = new System.Windows.Forms.TextBox();
            this.FiberCountSNLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScanSN
            // 
            this.ScanSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanSN.Location = new System.Drawing.Point(402, 47);
            this.ScanSN.Name = "ScanSN";
            this.ScanSN.Size = new System.Drawing.Size(355, 29);
            this.ScanSN.TabIndex = 2;
            this.ScanSN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScanSN_keydown);
            // 
            // FiberCountSNLabel
            // 
            this.FiberCountSNLabel.AutoSize = true;
            this.FiberCountSNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FiberCountSNLabel.Location = new System.Drawing.Point(12, 46);
            this.FiberCountSNLabel.Name = "FiberCountSNLabel";
            this.FiberCountSNLabel.Size = new System.Drawing.Size(0, 29);
            this.FiberCountSNLabel.TabIndex = 3;
            // 
            // frmEquipSN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 124);
            this.Controls.Add(this.FiberCountSNLabel);
            this.Controls.Add(this.ScanSN);
            this.Name = "frmEquipSN";
            this.Text = "frmEquipSN";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ScanSN;
        private System.Windows.Forms.Label FiberCountSNLabel;
    }
}