using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using System.IO;

namespace Utility
{

    /// <summary>
    /// Application Utilitize
    /// </summary>
    public static class ApplicationUtil
    {
        /// <summary>
        /// Event Handler of Set Tower Light
        /// </summary>
        public static event EventHandler<CSetTowerLight> SetTowerLight;

        /// <summary>
        /// Dictionary to store Product Type vs MSID Prefix
        /// </summary>
        private static Dictionary<string, string> _dictProductSNPrefix;
        
        /// <summary>
        /// Selected Product Type
        /// </summary>
        private static string _sSelectedProduct = "";

        /// <summary>
        /// Station Register Name
        /// </summary>
        public const string STATION = "Station";
        /// <summary>
        /// SEQUENCER Register Name
        /// </summary>
        public const string SEQUENCER = "Sequencer";
        /// <summary>
        /// Products Register Name
        /// </summary>
        public const string PRODUCTS = "Products";
        /// <summary>
        /// Registry Group Name
        /// </summary>
        public const string SPANINTELSPPD = "SPANIntelSPPD";

        /// <summary>
        /// Sequencer Type Option
        /// </summary>
        public enum SequencerType
        {
            /// <summary>
            /// No Sequencer is set
            /// </summary>
            NULL = -1,
            /// <summary>
            /// Legacy Sequencer
            /// </summary>
            SEQ100G = 0,
            /// <summary>
            /// Dynamic Sequencer
            /// </summary>
            SEQDYNAMIC,
            /// <summary>
            /// Sequencer for  SeqEng
            /// </summary>
            SEQENG,
            /// <summary>
            /// Sequencer for Soak Test
            /// </summary>
            SEQSOAK,
            /// <summary>
            /// Sequencer to for OARS Tx/RX FAU Alignment
            /// </summary>
            SEQSPA,
            /// <summary>
            /// Sequencer for Switch test
            /// </summary>
            SEQSWITCH
        }

        /// <summary>
        /// Atrribution of File
        /// </summary>
        public enum FileAttribute
        {
            /// <summary>
            /// Read Only
            /// </summary>
            ReadOnly = 0,
            /// <summary>
            /// Hidden
            /// </summary>
            Hidden,
            /// <summary>
            /// Password
            /// </summary>
            Password
        }

        /// <summary>
        /// Mode of Test Station
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Production Mode
            /// </summary>
            PRODUCTION = 0,
            /// <summary>
            /// Engineering Mode
            /// </summary>
            ENGINEERING,
            /// <summary>
            /// SPC Mode
            /// </summary>
            SPC
        }

        /// <summary>
        /// MSID Prefix of Product
        /// </summary>
        public static string ProductSNPrefix = "";

        /// <summary>
        /// Get Application version
        /// </summary>
        /// <returns>Application version with Application Name</returns>
        public static string getVersionInfo()
        {
            Assembly exeAssembly = Assembly.GetEntryAssembly();
            AssemblyName exeName = exeAssembly.GetName();
            Version exeVersion = exeName.Version;
            //string fullVersion = exeVersion.ToString(4);
            // change the GUI number from 4 digits to 3 digits, if the last digit is 0
            String sVersion = exeVersion.ToString();
            if (sVersion.Length == 7 && sVersion.EndsWith("0"))
                sVersion = sVersion.Substring(0, 5);

            return exeName.Name + " " + sVersion;
        }

        /// <summary>
        /// Get SPAN version Info
        /// </summary>
        /// <returns>SPAN version</returns>
        public static string getPureVersionInfo()
        {
            Assembly exeAssembly = Assembly.GetEntryAssembly();
            AssemblyName exeName = exeAssembly.GetName();
            Version exeVersion = exeName.Version;
            //string fullVersion = exeVersion.ToString(4);
            String sVersion = exeVersion.ToString();
            if (sVersion.Length == 7 && sVersion.EndsWith("0"))
                sVersion = sVersion.Substring(0, 5);

            return sVersion;
        }

        /// <summary>
        /// Prompt message with alert sound
        /// </summary>
        /// <param name="message">Message to Prompt</param>
        public static void AlertMessage(string message)
        {
            using (frmMessage frm = new frmMessage(message))
            {
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Prompt message with alert sound and throw exception after
        /// </summary>
        /// <param name="message">error message</param>
        public static void ThrowError(string message)
        {
            ApplicationUtil.AlertMessage(message);
            throw new Exception(message);
        }

        /// <summary>
        /// Set Station Name and Run Mode to form
        /// </summary>
        /// <param name="station">Station Name to Display</param>
        /// <param name="mode">Run Mode to Display</param>
        /// <param name="operation">Operation Name</param>
        /// <param name="button">Button object of Form</param>
        /// <param name="obj">Form object</param>
        public static void SetStationMode(string station, ApplicationUtil.Mode mode, string operation, Button button, Control obj)
        {
            if (obj.InvokeRequired)
            {
                //Call the delegate
                obj.Invoke(new MethodInvoker(() => { SetStationMode(station, mode, operation, button, obj); }));
            }
            else
            {
                button.ForeColor = Color.White;
                switch (mode)
                {
                    case ApplicationUtil.Mode.PRODUCTION:
                        button.BackColor = Color.LimeGreen;
                        break;
                    case ApplicationUtil.Mode.ENGINEERING:
                        button.BackColor = Color.Red;
                        break;
                    case ApplicationUtil.Mode.SPC:
                        button.BackColor = Color.Yellow;
                        button.ForeColor = Color.Black;
                        break;
                }
                button.Text = $"{station} | {mode} | {operation}";
            }
        }

        /// <summary>
        /// Check is Tester already set Station Name and Sequencer in Registry
        /// </summary>
        /// <returns>Whether set/no set</returns>
        public static bool IsRegisterSettingsComplete()
        {
            bool ret = true;
            string station = "";
            string sequencer = "";
            // create/get main key
            RegistryKey _key = Registry.CurrentUser.CreateSubKey(SPANINTELSPPD, true);
            // read Station value
            RegistryKey keyStation = _key.CreateSubKey(ApplicationUtil.STATION, true);
            station = keyStation.GetValue(ApplicationUtil.STATION, "").ToString();
            if (station == "") ret &= false;

            // read Sequencer value
            RegistryKey keySequencer = _key.CreateSubKey(ApplicationUtil.SEQUENCER, true);
            sequencer = keySequencer.GetValue(ApplicationUtil.SEQUENCER, "").ToString();
            if (sequencer == "") ret &= false;

            // final status   
            return ret;
        }

        private static LocalSetting _Local;

        /// <summary>
        /// Deserialize LocalSetting.config file
        /// </summary>
        private static void DeserializeLocalSetting()
        {
            string strLocalSettingFile = Path.Combine(Application.StartupPath, LocalSetting.LocalSettingFile);
            if (File.Exists(strLocalSettingFile))
            {

                frmPanelAuthorize frmPassword = new frmPanelAuthorize();
                frmPassword.ShowDialog();
                if (frmPassword.strPassword == CSecret.Instance.dicSecret[CSecret.COMMON])
                {
                    _Local = GenericSerializer.DeserializeFromXML<LocalSetting>(strLocalSettingFile);
                }
            }

            if (_Local is null)
            {
                _Local = new LocalSetting();
            }
        }

        /// <summary>
        /// Station Name from Registry or Local File
        /// </summary>
        /// <returns>Station Name</returns>
        public static string GetRegistryStation(bool bTool = false)
        {
            if (!bTool)
            {
                if (_Local is null) DeserializeLocalSetting();
                if (_Local.StationName != null) return _Local.StationName;
            }

            RegistryKey _key = Registry.CurrentUser.CreateSubKey(SPANINTELSPPD, true);
            // read Station value
            RegistryKey keyStation = _key.CreateSubKey(ApplicationUtil.STATION, true);
            string stationName = keyStation.GetValue(ApplicationUtil.STATION, "").ToString();

            if (!bTool)
            {
                List<string> ProductTypes = GetRegistryProductList();
                // Let User select product if support multi product
                if (ProductTypes.Count > 0)
                {
                    string[] arStation = stationName.Split('-');
                    if (_sSelectedProduct == "")
                    {
                        frmSelection frmProduct = new frmSelection("PRODUCT TYPE", ProductTypes);
                        frmProduct.ShowDialog();
                        _sSelectedProduct = frmProduct.SelectedValue;
                    }
                    arStation[1] = _sSelectedProduct;

                    ProductSNPrefix = _dictProductSNPrefix[arStation[1]];
                    stationName = "";
                    foreach (string s in arStation)
                    {
                        stationName += $"{s}-";
                    }
                    stationName = stationName.Trim('-');
                }
            }

            return stationName;
        }

        /// <summary>
        /// Sequencer Type from Registry or Local File
        /// </summary>
        /// <returns>Sequencer Type</returns>
        public static string GetRegistrySequencer(bool bTool = false)
        {
            if (!bTool)
            {
                if (_Local is null) DeserializeLocalSetting();
                if (_Local.SeqType != SequencerType.NULL) return _Local.SeqType.ToString();
            }

            RegistryKey _key = Registry.CurrentUser.CreateSubKey(SPANINTELSPPD, true);
            RegistryKey keySequencer = _key.CreateSubKey(ApplicationUtil.SEQUENCER, true);
            return keySequencer.GetValue(ApplicationUtil.SEQUENCER, "SEQ100G").ToString();
        }

        /// <summary>
        /// Get Supported Product Types from registry
        /// </summary>
        /// <returns>Supported Product Types stated in registry</returns>
        public static string GetRegistryProducts()
        {
            RegistryKey _key = Registry.CurrentUser.CreateSubKey(SPANINTELSPPD, true);
            RegistryKey keyProducts = _key.CreateSubKey(ApplicationUtil.PRODUCTS, true);
            return keyProducts.GetValue(ApplicationUtil.PRODUCTS, "").ToString();
        }

        /// <summary>
        /// Get Supported Product Types List
        /// </summary>
        /// <returns>List of Supported Product Types</returns>
        public static List<string> GetRegistryProductList()
        {
            List<string> ret = new List<string>();
            _dictProductSNPrefix = new Dictionary<string, string>();

            string Products = GetRegistryProducts();
            if (Products != "")
            {
                ret = Products.Split(';').ToList();
                for (int i = 0; i < ret.Count; i++)
                {
                    string[] arProduct = ret[i].Split(':');
                    _dictProductSNPrefix[arProduct[0].Trim()] = arProduct[1].Trim();
                    ret[i] = arProduct[0].Trim();
                }
            }

            return ret;
        }

        /// <summary>
        /// Estimate a fit Text Font to a control.
        /// </summary>
        /// <param name="graphics">Graphics Object of the control</param>
        /// <param name="size">Size of control</param>
        /// <param name="font">Font Size of control</param>
        /// <param name="str">Message String</param>
        /// <returns>Calculated estimate font size</returns>

        public static float EstimateFontSize(Graphics graphics, Size size, Font font, string str)
        {
            SizeF stringSize = graphics.MeasureString(str, font);
            float wRatio = (size.Width - 14) / stringSize.Width;
            float hRatio = size.Height / stringSize.Height;
            float ratio = Math.Min(hRatio, wRatio);
            return font.Size * ratio;
        }

        /// <summary>
        /// Get a fit Font Size
        /// </summary>
        /// <param name="control">Control that request auto resize font</param>
        /// <param name="graphic">Graphic of Control</param>
        /// <param name="padding">Padding needed the message on the control</param>
        /// <param name="MaxFontSize">Max font size threshold</param>
        /// <returns>Final Calculated font size to set</returns>
        public static Font SizeTextToControl(Control control, Graphics graphic, int padding, Single MaxFontSize = 16f)
        {
            // Create a close fit font size
            Font font;
            float newSize = EstimateFontSize(control.CreateGraphics(), control.Size, control.Font, control.Text);
            if (newSize <= 0) return control.Font;
            font = new Font(control.Font.FontFamily, newSize, control.Font.Style);
            SizeF textSize = graphic.MeasureString(control.Text, font);

            //increase size first
            while ((textSize.Width < control.Width - padding))
            {
                if (newSize > MaxFontSize)
                {
                    newSize = MaxFontSize;
                    break;
                }
                newSize += 0.5f;
                font = new Font(font.FontFamily, newSize, font.Style);
                textSize = graphic.MeasureString(control.Text, font);
            }

            //Loop until it fits perfect
            while ((textSize.Width > control.Width - padding) || (textSize.Height > control.Height))
            {
                newSize -= 0.5f;
                font = new Font(font.FontFamily, newSize, font.Style);
                textSize = graphic.MeasureString(control.Text, font);
            }
            if (newSize < MaxFontSize) newSize -= 0.5f;
            else newSize = MaxFontSize;
            font = new Font(font.FontFamily, newSize, font.Style);
            return font;
        }

        /// <summary>
        /// Trigger Set Tower Light Event Occur
        /// </summary>
        /// <param name="caller">Caller Object</param>
        /// <param name="bDisabled">Flag to Turn Off Tower Light</param>
        /// <param name="bCompleted">Flag to trigger Complete Test Mode</param>
        /// <param name="bAlert">Flag to trigger Alert Mode</param>
        /// <param name="bTesting">Flag to trigger Testing Mode</param>
        public static void TriggerSetTowerLight(Object caller, bool bDisabled = false, bool bCompleted = false, bool bAlert = false, bool bTesting = false)
        {
            if (null != SetTowerLight)
            {
                SetTowerLight(caller, new CSetTowerLight(bDisabled, bCompleted, bAlert, bTesting));
            }
        }
    }

    /// <summary>
    /// Local Setting for Overwrite registry setting or others.
    /// </summary>
    [Serializable]
    public class LocalSetting
    {
        /// <summary>
        /// Local Setting File Name
        /// </summary>
        public const string LocalSettingFile = "LocalSetting.config";

        /// <summary>
        /// Sequencer Type to be applied
        /// </summary>
        public ApplicationUtil.SequencerType SeqType { get; set; }

        /// <summary>
        /// Station Name to be applied
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// Define default setting
        /// </summary>
        public LocalSetting()
        {
            SeqType = Utility.ApplicationUtil.SequencerType.NULL;
            StationName = null;
        }
    }

    /// <summary>
    /// Event Argument for Set Tower Light
    /// </summary>
    public class CSetTowerLight : EventArgs
    {
        /// <summary>
        /// Flag to Turn Off Tower Light
        /// </summary>
        public bool bDisable { get; set; }

        /// <summary>
        /// Flag to trigger Complete Test Mode
        /// </summary>
        public bool bCompleted { get; set; }

        /// <summary>
        /// Flag to trigger Alert Mode
        /// </summary>
        public bool bAlert { get; set; }

        /// <summary>
        /// Flag to trigger Testing Mode
        /// </summary>
        public bool bTesting { get; set; }

        /// <summary>
        /// Constructor of CSetTowerLight
        /// </summary>
        /// <param name="_bDisabled">Flag to trigger Complete Test Mode</param>
        /// <param name="_bCompleted">Flag to trigger Complete Test Mode</param>
        /// <param name="_bAlert">Flag to trigger Alert Mode</param>
        /// <param name="_bTesting">Flag to trigger Testing Mode</param>
        public CSetTowerLight(bool _bDisabled = false, bool _bCompleted = false, bool _bAlert = false, bool _bTesting = false)
        {
            bDisable = _bDisabled;
            bCompleted = _bCompleted;
            bAlert = _bAlert;
            bTesting = _bTesting;
        }
    }
}
