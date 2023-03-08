using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//add this namespace
using System.Runtime.InteropServices;
using System.Media;
using System.IO;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmMessage : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public frmMessage()
        {
            InitializeComponent();
            timer1.Tick += blinkTextbox;
            timer1.Interval = 250;
            timer1.Enabled = true;
        }

        private string _strDir = "";
        SoundPlayer _simpleSound;

        private void blinkTextbox(object sender, EventArgs e)
        {
            //if (textBox1.BackColor == Color.Red) textBox1.BackColor = Color.White;
            //else textBox1.BackColor = Color.Red;

            if (this.panel1.BackColor == Color.Red) panel1.BackColor = Color.White;
            else panel1.BackColor = Color.Red;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strmessage"></param>
        /// <param name="imagePath"></param>
        public frmMessage(string strmessage, string imagePath = "")
        {
            InitializeComponent();

            if (imagePath == "")
            {
                this.Size = new Size(800, 150);
                pictureBox1.Visible = false;
                Exitbutton.Location = new Point(Convert.ToInt16((800 - 104) / 2), 62);
            }
            timer1.Tick += blinkTextbox;
            timer1.Interval = 250;
            timer1.Enabled = true;
            this.label1.Text = strmessage;
            try
            {
                if (imagePath != "") this.pictureBox1.ImageLocation = imagePath;
            }
            catch (Exception)
            {
            }
        }

        private void Exitbutton_Click(object sender, EventArgs e)
        {
            if (_simpleSound != null)
            {
                _simpleSound.Stop();
            }

            this.Close();
        }

        private void frmMessage_Load(object sender, EventArgs e)
        {
            _strDir = AppDomain.CurrentDomain.BaseDirectory;
            string strSoundFile = Path.Combine(_strDir, "ding.wav");

            if (File.Exists(strSoundFile))
            {
                _simpleSound = new SoundPlayer(strSoundFile);
                _simpleSound.PlayLooping();
            }

            ApplicationUtil.TriggerSetTowerLight(this, bAlert: true);
        }

        private void frmMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_simpleSound != null)
            {
                _simpleSound.Stop();
            }
            ApplicationUtil.TriggerSetTowerLight(this, bDisabled: true, bAlert: true);
        }
    }

}
