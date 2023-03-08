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
    /// Customizable Message box, 
    /// you can change message to show, 
    /// show/hide button to user 
    /// and get ok button is clicked by user.
    /// </summary>
    public partial class frmMsgBox : Form
    {
        /// <summary>
        /// Indicating whether Ok is been clicked.
        /// </summary>
        public bool bOKclick { get; set; }
        /// <summary>
        /// Define what message want to be prompt.
        /// </summary>
        public string sMessage { get; set; }
        /// <summary>
        /// Initialize form of customized Message Box
        /// </summary>
        /// <param name="bNoButton">Set this to true if no button want to be visible</param>
        public frmMsgBox(bool bNoButton = false)
        {
            InitializeComponent();

            if (bNoButton)
            {
                btnOK.Visible = false;
                btnOK.Visible = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bOKclick = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bOKclick = false;
            this.Close();
        }

        private void frmMsgBox_Load(object sender, EventArgs e)
        {
            txtMsg.Text = sMessage;
        }
    }
}
