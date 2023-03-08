using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// Form for user to do selection
    /// </summary>
    public partial class frmSelection : Form
    {
        /// <summary>
        /// User selected value
        /// </summary>
        public string SelectedValue { get; set; }

        /// <summary>
        /// Constructor of frmSelection
        /// </summary>
        /// <param name="regName">Register Name</param>
        /// <param name="lstSelection">Selection Value List</param>
        public frmSelection(string regName, List<string> lstSelection)
        {
            InitializeComponent();
            lblRegName.Text = $"{regName}:";
            cmbSelection.Items.Clear();
            cmbSelection.Items.AddRange(lstSelection.ToArray());
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            SelectedValue = cmbSelection.SelectedItem.ToString();
            Close();
        }
    }
}
