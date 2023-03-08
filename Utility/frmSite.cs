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
    /// 
    /// </summary>
    public partial class frmSite : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public string strSite { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strDbName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstSite"></param>
        /// <param name="lstDb"></param>
        public frmSite(List<string> lstSite, List<string> lstDb)
        {
            InitializeComponent();
            cmbSite.Items.AddRange(lstSite.ToArray());
            TestConstants.populateDbNameMap();
            foreach (string sProduct in TestConstants.mapProductToDbName.Keys)
            {
                if (lstDb.Contains(TestConstants.mapProductToDbName[sProduct]))
                {
                    cmbProduct.Items.Add(sProduct);
                }
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            strSite = cmbSite.SelectedItem.ToString();
            strDbName = TestConstants.mapProductToDbName[cmbProduct.SelectedItem.ToString()];
            Close();
        }
    }
}
