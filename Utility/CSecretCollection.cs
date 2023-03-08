using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// Credential data structure
    /// </summary>
    [Serializable]
    public class CSecretCollection
    {
        /// <summary>
        /// Credential File Name
        /// </summary>
        public const string SecretFileName = "secret.xml";

        /// <summary>
        /// Secret collection in Hash
        /// </summary>
        public Dictionary<string, string> dicSecret { get; set; }

        /// <summary>
        /// Default Setting
        /// </summary>
        public CSecretCollection()
        {
            dicSecret = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Class to collect all Secret
    /// </summary>
    public class CSecret
    {
        /// <summary>
        /// Commonly used
        /// </summary>
        public const string COMMON = "COMMON";
        /// <summary>
        /// Intel Cloud
        /// </summary>
        public const string INTELCLOUD = "INTELCLOUD";
        /// <summary>
        /// Commonly used by DB
        /// </summary>
        public const string DBCOMMON = "DBCOMMON";
        /// <summary>
        /// Notifier Email
        /// </summary>
        public const string NOTIFIER = "NOTIFIER";
        /// <summary>
        /// QnR Database
        /// </summary>
        public const string QNRDB = "QNRDB";

        /// <summary>
        /// Meadow River header
        /// </summary>
        public const string MEADOWRIVER = "MEADOWRIVER";

        /// <summary>
        /// Pike River header
        /// </summary>
        public const string PIKERIVER = "PIKERIVER";

        /// <summary>
        /// G53 header
        /// </summary>
        public const string G53 = "G53";

        /// <summary>
        /// G55 header
        /// </summary>
        public const string G55 = "G55";

        /// <summary>
        /// Store credential that after decrypted
        /// </summary>
        public Dictionary<string, string> dicSecret;

        private static CSecret _instance;
        /// <summary>
        /// Return CSecret Object
        /// </summary>
        public static CSecret Instance 
        {
            get
            {
                if (_instance is null) _instance = new CSecret();
                return _instance;
            } 
        }

        /// <summary>
        /// Constructor: Read Credential File and Decrypt them to store in dicSecret Collection.
        /// </summary>
        public CSecret()
        {
            CSecretCollection objSecret = GenericSerializer.DeserializeFromXML<CSecretCollection>(Path.Combine(Application.StartupPath, CSecretCollection.SecretFileName));
            dicSecret = new Dictionary<string, string>();
            foreach (string key in objSecret.dicSecret.Keys)
            {
                dicSecret[key] = CCypher.Decrypt(objSecret.dicSecret[key]);
            }
        }
    }
}
