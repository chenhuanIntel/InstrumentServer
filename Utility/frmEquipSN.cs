using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// Form to SCAN equip SN that support insertion count feature such as Fiber/Test board.0
    /// </summary>
    public partial class frmEquipSN : Form
    {
        private string _sFormat;

        /// <summary>
        /// Scan SN information
        /// </summary>
        public string ScanEquipSN = "";

        /// <summary>
        /// Scan equipment SN form
        /// </summary>
        /// <param name="nSlot">Slot number</param>
        /// <param name="sFormat">SN Format</param>
        public frmEquipSN(int nSlot, string sFormat)
        {
            _sFormat = sFormat;
            InitializeComponent();
            FiberCountSNLabel.Text = $"DUT:{nSlot},Scan Fiber SN";
            ScanSN.Focus();
            DialogResult = DialogResult.No;
        }

        private void ScanSN_keydown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                bool bMatch = false;
                List<string> lstFormat = _sFormat.Split(';').ToList();
                foreach (string format in lstFormat)
                {
                    if (Regex.IsMatch(ScanSN.Text, format))
                    {
                        bMatch = true;
                        break;
                    }
                }
                if (!bMatch)
                {
                    MessageBox.Show("Wrong SN has been scanned!", "Error: Wrong SN Format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ScanEquipSN = ScanSN.Text;
                DialogResult = DialogResult.Yes;
            }
        }
    }
}
