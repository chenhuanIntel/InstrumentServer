using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// form for user to key in password as grant access right
    /// </summary>
    public partial class frmPanelAuthorize : Form
    {
        /// <summary>
        /// Password of user key in
        /// </summary>
        public string strPassword { get; set; }

        /// <summary>
        /// Initialize form component
        /// </summary>
        public frmPanelAuthorize()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            strPassword = txtPassword.Text;
            this.Close();
        }

        private void txtPassword_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnSubmit.PerformClick();
            }
        }
    }
}
