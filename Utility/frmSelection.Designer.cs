
namespace Utility
{
    partial class frmSelection
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
            this.lblRegName = new System.Windows.Forms.Label();
            this.cmbSelection = new System.Windows.Forms.ComboBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblRegName
            // 
            this.lblRegName.AutoSize = true;
            this.lblRegName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.lblRegName.Location = new System.Drawing.Point(12, 15);
            this.lblRegName.Name = "lblRegName";
            this.lblRegName.Size = new System.Drawing.Size(268, 31);
            this.lblRegName.TabIndex = 0;
            this.lblRegName.Text = "OPERATION NAME:";
            // 
            // cmbSelection
            // 
            this.cmbSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.cmbSelection.FormattingEnabled = true;
            this.cmbSelection.Location = new System.Drawing.Point(286, 12);
            this.cmbSelection.Name = "cmbSelection";
            this.cmbSelection.Size = new System.Drawing.Size(261, 39);
            this.cmbSelection.TabIndex = 1;
            // 
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.btnSet.Location = new System.Drawing.Point(213, 67);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(111, 42);
            this.btnSet.TabIndex = 2;
            this.btnSet.Text = "SET";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // frmSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 121);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.cmbSelection);
            this.Controls.Add(this.lblRegName);
            this.Name = "frmSelection";
            this.Text = "Select Operation";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRegName;
        private System.Windows.Forms.ComboBox cmbSelection;
        private System.Windows.Forms.Button btnSet;
    }
}