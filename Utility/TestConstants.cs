using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
	public static class TestConstants
    {
        /// <summary>
        /// PROCESS CODES
        /// </summary>
        public const string PROCESS_FWDL = "PROCESS_FWDL";
        /// <summary>
        /// 
        /// </summary>
        public const string PROCESS_DCTESTS = "PROCESS_DCTESTS";
        /// <summary>
        /// 
        /// </summary>
        public const string PROCESS_HSTESTS = "PROCESS_HSTESTS";
        /// <summary>
        /// 
        /// </summary>
        public const string PROCESS_FINAL = "PROCESS_FINAL";

        // must add ThreadStatic attribute to declare the following as thread local storage
        // otherwise in multithread, all the memory are shared by all threads unless thread-safe (such as lock) is implemented
        // for example, local variable OperationBinList declared in getOperationBin() shares the same memory address as the member OperationBinList of TestConstants class in thread#1
        //              but it does not guarantee only thread#1 can access this memory location
        //              if there is no thread-safe lock nor declared as thread local storage, 
        //              we shall see "An item with the same key" exception when adding new elements into the dictionary in multi-threading
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, string> OperationBinList;// { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, int> _sOperationTestLocList;// { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, List<string>> testnameOperationListMap;// { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, string> opcodeName;// { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, string> mapProductToDbName;

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, string> mapTypeToSIDCollName;

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, string> mapCMToMSID;


        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_DC_PLBI = "DC_PLBI";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_CALIB = "DC_CALIB";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_EVT_LI = "DC_EVT_LI";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_EVT_TX = "DC_EVT_TX";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_MH = "DC_MH";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_QB = "DC_QB";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_RX_CAL = "DC_RX_CAL";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_RX = "DC_RX";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_SCREEN = "DC_SCREEN";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_FW = "DC_FW";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_HS_EVT_RXSENS = "HS_EVT_RXSENS";
        /// <summary>
        /// Flowname constant for HS_DispersionPenaltyTest_CAPI test class
        /// </summary>
		public const string FLOWNAME_HS_EVT_DISP_PENALTY = "HS_EVT_DISP_PENALTY";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_HS_TX = "HS_TX";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_HS_EVT_TX = "HS_EVT_TX";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_HS_EVT_TX_RESTEST = "HS_EVT_TX_RESTEST";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_HS_RIN = "HS_RIN";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_HS_EEYE = "HS_EEYE";
        /// <summary>
        /// 
        /// </summary>
		public const string FLOWNAME_DC_TEMP_LINK = "DC_TEMP_LINK";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_HS_TEMP_LINK = "HS_TEMP_LINK";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_DC_SAMPLING = "DC_SAMPLING";

        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_ALIGN_RX = "ALIGN_RX";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOWNAME_ALIGN_TX = "ALIGN_TX";
        /// <summary>
        /// Flow Name of CDB Reading
        /// </summary>
        public const string FLOWNAME_CDBReading = "CDBReading";

        //Test constants
        // WHEN ADDING NEW VALUES, PLEASE FOLLOW THE SEQUENCE IN ADDING NEW CONSTANTS.  PLS DON'T INSERT ANYWHERE - VALUES ALREADY SORTED!!!  LET'S MAKE OUR LIFE EASY!
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_NEW = "7001";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_SETQB = "7002";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_ALIGNMAIN = "7003";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_EPOXY = "7004";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_UVCURE = "7005";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_DONE = "7006";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_QBDiffScan = "7007";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_TX_QBScan = "7008";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_NEW = "7011";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_CHECKPOWER = "7012";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_ALIGNMAIN = "7013";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_EPOXY = "7014";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_UVCURE = "7015";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_ALIGN_RX_DONE = "7016";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_SCREEN = "8000";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_SN_CHECK_CAPI = "8001";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_MH = "8002";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_SKU_CONVERT = "8003";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_ETEST = "8004";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_QB = "8005";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_LI = "8006";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_RX_SHORT = "8007";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RIN = "8008";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EQCAL = "8009";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_PRE_BI = "8010";
        //public const string OPER_DC_FWDL = "8011";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_E_LI = "8011";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_E_LI_PRE = "8012";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_E_LI_POST = "8013";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_APC = "8014";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TX_COLD = "8015";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TX_ROOM = "8016";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TX_HOT = "8017";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_COLD = "8018";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_ROOM = "8019";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_POST_BI = "8020";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_HOT = "8021";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_SOAKTEST = "8022";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_MicroscopeInspcetion_Len = "8023";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_MicroscopeInspcetion_Fiber = "8024";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_ETEST_OPT = "8040";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_ETEST_SMT = "8041";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TX = "8100";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EVT_TX = "8101";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_PRE = "8102";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_TEST_PRE = "8103";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_CAL = "8104";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_COLD = "8105";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_ROOM = "8106";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_HOT = "8107";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EYE_COLD = "8109";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_LASERBIAS_SCREEN = "8110";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_QB_SCREEN = "8111";
        //public const string OPER_JA_STATION_CAL = "8112";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EYE_PRE = "8112";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EYE_ROOM = "8113";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EYE_HOT = "8114";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_PAM4_PRE = "8115";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_PAM4_COLD = "8116";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_PAM4_ROOM = "8117";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_PAM4_HOT = "8118";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_POWERCHECK_CAL = "8119";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TARGETPOWER_CAL = "8120";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_OSA_SCAN_PRE = "8122";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_TDECQ_SET = "8129";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_TDECQ = "8130";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_OSA_POWER_PRE = "8132";
        /// <summary>
        /// OPER_HS_PAM4_200G_COLD
        /// </summary>
        public const string OPER_HS_PAM4_200G_COLD = "8133";
        /// <summary>
        /// OPER_HS_PAM4_200G_ROOM
        /// </summary>
        public const string OPER_HS_PAM4_200G_ROOM = "8134";
        /// <summary>
        /// OPER_HS_PAM4_200G_HOT
        /// </summary>
        public const string OPER_HS_PAM4_200G_HOT = "8135";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_PLBI_PRE = "8201";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_POST = "8202";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_URS_POST = "8203";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_URS_PRE = "8204";

        /// <summary>
        /// Operation number constant for HS_DispersionPenalty_CAPI test class
        /// </summary>
        public const string OPER_HS_DISP_PENALTY_POST = "8205";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_OMA_TEST_POST = "8206";

        /// <summary>
        /// Operation Code of CDB Reading
        /// </summary>
        public const string OPER_CDBReading = "8207";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_EYE_POST = "8212";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_PLBI_MED_1 = "8213";
        /// <summary>
        /// 
        /// </summary>
		public const string OPER_DC_PLBI_MED_2 = "8214";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_PAM4_POST = "8215";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_PLBI_POST = "8221";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_OSA_SCAN_POST = "8222";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_PLBI_OPTICAL = "8231";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_OSA_POWER_POST = "8232";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TBI = "8241";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_NRZ_HOT = "8294";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_NRZ_ROOM = "8295";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_NRZ_COLD = "8296";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_HOT = "8297";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_ROOM = "8298";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_COLD = "8299";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_JITTER = "8301";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_VERIF = "8302";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_200G_HOT = "8303";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_200G_ROOM = "8304";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_RXSENS_200G_COLD = "8305";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK = "8400"; //RT
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_HV = "8401";//LT HV
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_LV = "8402";//LT LV
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_LT = "8403";//LT NV
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_HOUSING = "8404";//LT NV
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_ML = "8405"; //Machine learning
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_QBmin = "8406"; //Machine learning
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DDM_CHECK_HT = "8407";//HT NV
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_PCB_VISUALCHECK = "8408";  // for Automated ETester

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_EVALUATE_DUTBIN = "8413";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_EEPROM_WRITE = "8414";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_EEPROM_CHECK = "8415";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_OQC = "8416";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_FINALMODULE_CHECK = "8417";
        /// <summary>
        /// Operation code for OQC Sampling
        /// </summary>
        public const string OPER_OQC_SAMPLING = "8418";
        /// <summary>
        /// Operation code for final module check
        /// </summary>
        public const string OPER_FINALMODULE_CHECK_SAMPLING = "8419";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CAL_MOD_RX = "8801";
        /// <summary>
        /// 
        /// </summary>
		public const string OPER_DC_CAL_MOD_THERMAL = "8802";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CAL_MOD_TX = "8803";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CAL_MOD_VOLTAGE = "8804";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_CAL_STEP1 = "8805";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CAL_MOD_IBIAS = "8806";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CAL_MOD_QBD_CAL = "8807";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_APC = "8808";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_RxLOS_Check = "8809";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_RxLOS_Check_COLD = "8810";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_RxLOS_Check_HOT = "8811";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_THERMAL_CAL_STEP2 = "8815";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_FW_OVERLAY = "8880";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_FW_Customization = "8881";
        /// <summary>
        /// Operation code for FW customizating for sampling
        /// </summary>
        public const string OPER_FW_Customization_Sampling = "8882";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_SET_LASERBIAS = "8886";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_FW_DOWNLOAD = "8887";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_CALIB = "8888";

        /// <summary>
        /// Firmware Upgrade Operation Code
        /// </summary>
        public const string OPER_FW_UPGRADE = "8889";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_DC_TEMP_LINK = "8890";
        //public const string OPER_HS_TEMPCYLE_LINK = "8891";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_TEMP_LINK = "8891";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_SPC = "8900";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_EEYE = "8901";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_COMPARE = "8902";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_SAMPLING = "8903";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_SWITCH = "8990";
        /// <summary>
        /// 
        /// </summary>
        public const string OPER_HS_SWITCH_PRE = "8991";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_COUPLING_CHECK = "8999";

        //RETest constants
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_SCREEN = "9000";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_MH = "9002";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_ETEST = "9004";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_QB = "9005";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_EYE = "9005";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_LI = "9006";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_RX_SHORT = "9007";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_EQCAL = "9008";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PRE_BI = "9010";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_E_LI = "9012";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RIN = "9014";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_THERMAL_APC = "9014";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_POST_BI = "9020";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_EVT_TX = "9100";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_TX = "9101";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_HS_EVT_TX = "9101";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RXSENS_PRE = "9102";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_OMA_TEST_PRE = "9103";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_OMA_CAL = "9104";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_EVT_LI = "9106";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_LASERBIAS_SCREEN = "9110";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_QB_SCREEN = "9111";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_EYE_PRE = "9112";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_OSA_SCAN = "9113";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_EEPROM_WRITE = "9114";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_EEPROM_CHECK = "9115";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_OQC = "9116";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_OSA_SCAN_PRE = "9122";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PLBI_PRE = "9201";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RXSENS_POST = "9202";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_URS_POST = "9203";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_URS_PRE = "9204";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_OMA_TEST_POST = "9206";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_EYE_POST = "9212";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PLBI_MED_1 = "9213";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PLBI_MED_2 = "9214";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PLBI_POST = "9221";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_OSA_SCAN_POST = "9222";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_PLBI_OPTICAL = "9231";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RXSENS_HOT = "9297";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RXSENS_ROOM = "9298";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_HS_RXSENS_COLD = "9299";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DDM_CHECK = "9400";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DDM_CHECK_HV = "9401";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DDM_CHECK_LV = "9402";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DDM_CHECK_LT = "9403";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_RX = "9801";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_THERMAL = "9802";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_TX = "9803";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_VOLTAGE = "9804";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_IBIAS = "9806";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CAL_MOD_QBD_CAL = "9807";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_RxLOS_Check = "9809";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_SET_LASERBIAS = "9886";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_FW_DOWNLOAD = "9887";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_DC_CALIB = "9888";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_HS_TEMP_LINK = "9890";

        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_SPC = "9900";
        /// <summary>
        /// 
        /// </summary>
        public const string RETEST_OPER_COMPARE = "9900";

        /// <summary>
        /// 
        /// </summary>
        public const string OPER_UNKNOWN = "9999";

        /// <summary>
        /// opcodeName Add
        /// </summary>
        public static void populateDbNameMap()
        {
            mapProductToDbName = new Dictionary<string, string> { { "PSM4", "prism" },
                                                                { "CLR4", "prismCLR4" },
                                                                { "CWDM8", "prismCWDM8" },
                                                                { "SST_CWDM4", "prismCWDM4_SST" },
                                                                { "AvalB", "prismAvalB" },
                                                                { "OrchB", "prismOrchB" },
                                                                { "CyprB", "prismCyprB" },
                                                                { "WhitB", "prismWhitB" },
                                                                { "SimpB", "prismSimpB" },
                                                                { "VancB", "prismVancB" },
                                                                { "TeleB", "prismTeleB" },
                                                                { "CyprB-HN", "prismCyprB_HN" },
                                                                { "OrchB-HN", "prismOrchB_HN" },
                                                                { "DR4", "prismDR4" },
                                                                { "WhitBLR", "prismWhitB" }};
        }

        /// <summary>
        /// 
        /// </summary>
        public static void populateSIDCollNameMap()
        {
            mapTypeToSIDCollName = new Dictionary<string, string> { { "Assy", "SIDParametricData_Assy" },
                                                                //{ "EWLT", "SIDParametricData_EWLT" },
                                                                { "IQC", "SIDParametricData_IQC" },
                                                                { "EWLT", "SIDParametricData_EWLT" },
                                                                { "OWLT", "SIDParametricData_OWLT" },
                                                                { "MLT", "SIDParametricData_MLT" }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        public static void populateCMToMSIDMap()
        {
            mapCMToMSID = new Dictionary<string, string> { { "FBN", "^(CR|GR)" },
                //{ "FBN", "^(CL|GL|CR|GR|CS|VSPP)"
                                                            { "SST", "^(LH|GH)" },
                                                            { "SYT", "^(PJ|GJ)" }
            };
        }

        /// <summary>
        /// Mapping of Operation Code vs Test Name
        /// </summary>
        public static void populateTestnameOperationListMap()
        {
            testnameOperationListMap = new Dictionary<string, List<string>>();
            opcodeName = new Dictionary<string, string>();
            opcodeName.Add("8201", "OPER_DC_PLBI_PRE");
            opcodeName.Add("8213", "OPER_DC_PLBI_MED_1");
            opcodeName.Add("8214", "OPER_DC_PLBI_MED_2");
            opcodeName.Add("8221", "OPER_DC_PLBI_POST");
            opcodeName.Add("8231", "OPER_DC_PLBI_OPTICAL");
            opcodeName.Add("9999", "OPER_UNKNOWN");
            opcodeName.Add("8888", "OPER_DC_CALIB");
            opcodeName.Add("8889", "DC_FW_Upgrade_CAPI");
            opcodeName.Add("8801", "OPER_DC_CAL_MOD_RX");
            opcodeName.Add("8809", "OPER_DC_RxLOS_Check");
            opcodeName.Add("8802", "OPER_DC_CAL_MOD_THERMAL");
            opcodeName.Add("8803", "OPER_DC_CAL_MOD_TX");
            opcodeName.Add("8804", "OPER_DC_CAL_MOD_VOLTAGE");
            opcodeName.Add("8806", "OPER_DC_CAL_MOD_IBIAS");
            opcodeName.Add("8807", "OPER_DC_CAL_MOD_QBD_CAL");
            opcodeName.Add("8810", "OPER_DC_RxLOS_Check_COLD");
            opcodeName.Add("8811", "OPER_DC_RxLOS_Check_HOT");
            opcodeName.Add("8110", "OPER_DC_LASERBIAS_SCREEN");
            opcodeName.Add("8111", "OPER_DC_QB_SCREEN");
            opcodeName.Add("8241", "OPER_DC_TBI");
            //opcodeName.Add( OPER_JA_STATION_CAL" )"8112";
            opcodeName.Add("8122", "OPER_OSA_SCAN_PRE");
            opcodeName.Add("8222", "OPER_OSA_SCAN_POST");
            opcodeName.Add("8132", "OPER_OSA_POWER_PRE");
            opcodeName.Add("8232", "OPER_OSA_POWER_POST");
            opcodeName.Add("8413", "OPER_EVALUATE_DUTBIN");
            opcodeName.Add("8414", "OPER_EEPROM_WRITE");
            opcodeName.Add("8415", "OPER_EEPROM_CHECK");
            opcodeName.Add("8416", "OPER_OQC");
            opcodeName.Add("8417", "OPER_FINALMODULE_CHECK");
            opcodeName.Add("8418", "OPER_OQC_SAMPING");
            opcodeName.Add("8419", "OPER_FINALMODULE_CHECK_SAMPLING");
            opcodeName.Add("8400", "OPER_DDM_CHECK"); //RT
            opcodeName.Add("8401", "OPER_DDM_CHECK_HV");//LT HV
            opcodeName.Add("8402", "OPER_DDM_CHECK_LV");//LT LV
            opcodeName.Add("8403", "OPER_DDM_CHECK_LT");//LT NV
            opcodeName.Add("8407", "OPER_DDM_CHECK_HT");//HT NV
            opcodeName.Add("8404", "OPER_DDM_CHECK_HOUSING");//LT NV

            opcodeName.Add("8405", "OPER_DDM_CHECK_ML");//LT NV
            opcodeName.Add("8406", "OPER_DDM_CHECK_QBmin");//LT NV
            opcodeName.Add("8408", "OPER_PCB_VISUALCHECK");

            opcodeName.Add("8001", "OPER_SN_CHECK_CAPI");
            opcodeName.Add("8003", "OPER_SKU_CONVERT");

            opcodeName.Add("8999", "OPER_COUPLING_CHECK");


            opcodeName.Add("8006", "OPER_DC_LI");
            opcodeName.Add("8100", "OPER_DC_TX");
            opcodeName.Add("8002", "OPER_DC_MH");
            opcodeName.Add("8007", "OPER_DC_RX_SHORT");
            opcodeName.Add("8887", "OPER_FW_DOWNLOAD");
            opcodeName.Add("8880", "OPER_FW_OVERLAY");
            opcodeName.Add("8881", "OPER_FW_Customization");
            opcodeName.Add("8882", "OPER_FW_Customization_Sampling");
            opcodeName.Add("8886", "OPER_DC_SET_LASERBIAS");
            opcodeName.Add("8005", "OPER_DC_QB");
            opcodeName.Add("8000", "OPER_DC_SCREEN");
            opcodeName.Add("8010", "OPER_DC_PRE_BI");
            opcodeName.Add("8020", "OPER_DC_POST_BI");
            opcodeName.Add("8202", "OPER_HS_RXSENS_POST");
            opcodeName.Add("8102", "OPER_HS_RXSENS_PRE");
            opcodeName.Add("8299", "OPER_HS_RXSENS_COLD");
            opcodeName.Add("8298", "OPER_HS_RXSENS_ROOM");
            opcodeName.Add("8297", "OPER_HS_RXSENS_HOT");
            opcodeName.Add("8119", "OPER_DC_POWERCHECK_CAL");
            opcodeName.Add("8120", "OPER_DC_TARGETPOWER_CAL");
            opcodeName.Add("8805", "OPER_DC_THERMAL_CAL_STEP1");
            opcodeName.Add("8815", "OPER_DC_THERMAL_CAL_STEP2");

            opcodeName.Add("8203", "OPER_HS_URS_POST");
            opcodeName.Add("8204", "OPER_HS_URS_PRE");

            opcodeName.Add("8101", "OPER_HS_EVT_TX");
            opcodeName.Add("8008", "OPER_HS_RIN");
            opcodeName.Add("8103", "OPER_HS_OMA_TEST_PRE");
            opcodeName.Add("8206", "OPER_HS_OMA_TEST_POST");
            opcodeName.Add("8104", "OPER_HS_OMA_CAL");
            opcodeName.Add("8009", "OPER_HS_EQCAL");
            opcodeName.Add("8900", "OPER_SPC");
            opcodeName.Add("8901", "OPER_EEYE");
            opcodeName.Add("8902", "OPER_COMPARE");
            opcodeName.Add("8903", "OPER_SAMPLING");

            opcodeName.Add("8891", "OPER_HS_TEMP_LINK");
            opcodeName.Add("8990", "OPER_HS_SWITCH");
            opcodeName.Add("8991", "OPER_HS_SWITCH_PRE");
            opcodeName.Add("8112", "OPER_HS_EYE_PRE");
            opcodeName.Add("8212", "OPER_HS_EYE_POST");
            opcodeName.Add("8301", "OPER_HS_JITTER");
            //opcodeName.Add( "8891", "OPER_HS_TEMPCYLE_LINK" );
            opcodeName.Add("8302", "OPER_DC_VERIF");
            opcodeName.Add("8890", "OPER_DC_TEMP_LINK");
            opcodeName.Add("8808", "OPER_DC_APC");
            opcodeName.Add("8004", "OPER_DC_ETEST");
            opcodeName.Add("8040", "OPER_DC_ETEST_OPT");
            opcodeName.Add("8041", "OPER_DC_ETEST_SMT");

            //opcodeName.Add( OPER_DC_FWDL" )"8011";
            opcodeName.Add("8011", "OPER_DC_E_LI");
            opcodeName.Add("8012", "OPER_DC_E_LI_PRE");
            opcodeName.Add("8013", "OPER_DC_E_LI_POST");
            opcodeName.Add("8014", "OPER_DC_THERMAL_APC");

            opcodeName.Add("8015", "OPER_DC_TX_COLD");
            opcodeName.Add("8016", "OPER_DC_TX_ROOM");
            opcodeName.Add("8017", "OPER_DC_TX_HOT");
            opcodeName.Add("8018", "OPER_DC_THERMAL_COLD");
            opcodeName.Add("8019", "OPER_DC_THERMAL_ROOM");
            opcodeName.Add("8021", "OPER_DC_THERMAL_HOT");
            opcodeName.Add("8022", "OPER_DC_SOAKTEST");
            opcodeName.Add("8023", "OPER_DC_MicroscopeInspcetion_Len");
            opcodeName.Add("8024", "OPER_DC_MicroscopeInspcetion_Fiber");
            opcodeName.Add("7001", "OPER_ALIGN_TX_NEW");
            opcodeName.Add("7002", "OPER_ALIGN_TX_SETQB");
            opcodeName.Add("7003", "OPER_ALIGN_TX_ALIGNMAIN");

            opcodeName.Add("7004", "OPER_ALIGN_TX_EPOXY");
            opcodeName.Add("7005", "OPER_ALIGN_TX_UVCURE");
            opcodeName.Add("7006", "OPER_ALIGN_TX_DONE");
            opcodeName.Add("7007", "OPER_ALIGN_TX_QBDiffScan");
            opcodeName.Add("7008", "OPER_ALIGN_TX_QBScan");

            opcodeName.Add("7011", "OPER_ALIGN_RX_NEW");
            opcodeName.Add("7013", "OPER_ALIGN_RX_ALIGNMAIN");
            opcodeName.Add("7014", "OPER_ALIGN_RX_EPOXY");

            opcodeName.Add("7015", "OPER_ALIGN_RX_UVCURE");
            opcodeName.Add("7016", "OPER_ALIGN_RX_DONE");

            // public const string OPER_DC_THERMAL_APC = "8014";

            List<string> _operationList = new List<string>();
            _operationList.Add(OPER_DC_ETEST);
            testnameOperationListMap.Add("DC_Electrical_Test_CAPI", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_AvalB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_OrchB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_CyprB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_WhitB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_ETEST_SMT);
            testnameOperationListMap.Add("DC_Electrical_Test_SMT_AvalB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_SMT_OrchB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_SMT_CyprB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_SMT_WhitB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_SMT_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_ETEST_OPT);
            testnameOperationListMap.Add("DC_Electrical_Test_Opt_AvalB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_Opt_OrchB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_Opt_CyprB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_Opt_WhitB", _operationList);
            testnameOperationListMap.Add("DC_Electrical_Test_Opt_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_E_LI);
            testnameOperationListMap.Add("DC_E_LI_V02", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CLR4_V02", _operationList);
            testnameOperationListMap.Add("DC_E_LI_PSM4_V02", _operationList);
            testnameOperationListMap.Add("DC_E_LI_AvalB", _operationList);
            testnameOperationListMap.Add("DC_E_LI_OrchB", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CyprB", _operationList);
            testnameOperationListMap.Add("DC_E_LI_WhitB", _operationList);
            testnameOperationListMap.Add("DC_E_LI_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_E_LI_PRE);
            //_operationList.Add("RETEST_OPER_DC_E_LI", "9012");
            testnameOperationListMap.Add("DC_E_LI_CAPI_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CLR4_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_AvalB_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_OrchB_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CyprB_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_WhitB_PRE_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_VancB_PRE_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_E_LI_POST);
            //_operationList.Add("RETEST_OPER_DC_E_LI", "9012");	
            testnameOperationListMap.Add("DC_E_LI_CAPI_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CLR4_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_AvalB_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_OrchB_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_CyprB_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_WhitB_POST_V01", _operationList);
            testnameOperationListMap.Add("DC_E_LI_VancB_POST_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FW_DOWNLOAD);
            testnameOperationListMap.Add("DC_FW_DL_CAPI", _operationList);
            testnameOperationListMap.Add("DC_FW_DL_AvalB", _operationList);
            testnameOperationListMap.Add("DC_FW_DL_OrchB", _operationList);
            testnameOperationListMap.Add("DC_FW_DL_CyprB", _operationList);
            testnameOperationListMap.Add("DC_FW_DL_WhitB", _operationList);
            testnameOperationListMap.Add("DC_FW_DL_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FW_OVERLAY);
            //_operationList.Add("RETEST_OPER_FW_DOWNLOAD", "9887");	
            testnameOperationListMap.Add("DC_FW_Overlay_CAPI", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_CAPI_V01", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_CAPI_V02", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_PSM4R_V01", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_PSM4R_V02", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_CLR4_V01", _operationList);
            testnameOperationListMap.Add("DC_FW_Overlay_CLR4_V02", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FW_Customization);
            testnameOperationListMap.Add("DC_FW_Customization_CAPI", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FW_Customization_Sampling);
            testnameOperationListMap.Add("DC_FW_Customization_CAPI_Sampling", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_LI);
            //_operationList.Add("RETEST_OPER_DC_LI", "9006");			
            testnameOperationListMap.Add("DC_LI_Test_PSM4_V01", _operationList);
            testnameOperationListMap.Add("DC_LI_Test_CLR4_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_QB);
            //_operationList.Add("RETEST_OPER_DC_QB", "9005");
            testnameOperationListMap.Add("DC_QB_Test_PSM4_V01", _operationList);
            testnameOperationListMap.Add("DC_QB_Test_CLR4_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CALIB);
            testnameOperationListMap.Add("CalMod_Thermal_OneTemp_PSM4_V01", _operationList);
            testnameOperationListMap.Add("TxThermalAPC_PSM4_01", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_EEPROM_CHECK);
            //_operationList.Add("RETEST_OPER_EEPROM_CHECK", "9115");
            testnameOperationListMap.Add("EEPROMCheck_PSM4_01", _operationList);
            testnameOperationListMap.Add("EEPROMCheck_PSM4_2", _operationList);
            testnameOperationListMap.Add("EEPROMCheck_PSM4_02", _operationList);
            testnameOperationListMap.Add("EEPROMCheck_PSM4R_02", _operationList);
            testnameOperationListMap.Add("EEPROMCheck_CLR4_01", _operationList);
            testnameOperationListMap.Add("EEPROMCheck_CLR4_V02", _operationList);
            testnameOperationListMap.Add("DC_EEPROMCheck_CAPI", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_OQC);
            //_operationList.Add("RETEST_OQC", "9116");
            testnameOperationListMap.Add("QC_EEPROMCheck_PSM4_01", _operationList);
            testnameOperationListMap.Add("QC_EEPROMCheck_CLR4_01", _operationList);
            testnameOperationListMap.Add("OutputQualityControl_PSM4", _operationList);
            testnameOperationListMap.Add("OutputQualityControl_PSM4R", _operationList);
            testnameOperationListMap.Add("OutputQualityControl_CLR4", _operationList);
            testnameOperationListMap.Add("DC_EEPROMCheck_CAPI_OQC", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_OQC_SAMPLING);
            testnameOperationListMap.Add("DC_EEPROMCheck_CAPI_OQC_Sampling", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FINALMODULE_CHECK);
            testnameOperationListMap.Add("DC_FinalModuleCheck_CAPI", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_FINALMODULE_CHECK_SAMPLING);
            testnameOperationListMap.Add("DC_FinalModuleCheck_CAPI_Sampling", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_COUPLING_CHECK);
            testnameOperationListMap.Add("CouplingCheck_CAPI", _operationList);
            testnameOperationListMap.Add("CouplingCheck_AvalB", _operationList);
            testnameOperationListMap.Add("CouplingCheck_OrchB", _operationList);
            testnameOperationListMap.Add("CouplingCheck_CyprB", _operationList);
            testnameOperationListMap.Add("CouplingCheck_WhitB", _operationList);
            testnameOperationListMap.Add("CouplingCheck_VancB", _operationList);

            //_operationList.Add("RETEST_OPER_DDM_CHECK", "9400");			

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_HV);
            //_operationList.Add("RETEST_OPER_DDM_CHECK_HV", "9401");

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_LV);
            //_operationList.Add("RETEST_OPER_DDM_CHECK_LV", "9402");			

            //_operationList.Add("RETEST_OPER_DDM_CHECK_LT", "9403");			
            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK);
            testnameOperationListMap.Add("DDMTest_PSM4_01", _operationList);
            testnameOperationListMap.Add("DDMTest_PSM4_V02", _operationList);
            testnameOperationListMap.Add("DDMTest_PSM4_V03", _operationList);
            testnameOperationListMap.Add("EEPROMRead_No_Thermal_CLR4_01", _operationList);
            testnameOperationListMap.Add("EEPROMRead_No_Thermal_PSM4_01", _operationList);
            testnameOperationListMap.Add("DDMTest_CLR4_01", _operationList);
            testnameOperationListMap.Add("DDMTest_CLR4_02", _operationList);
            testnameOperationListMap.Add("DDMTest_PSM4R_QBMin_V01", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_CAPI", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_AvalB", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_OrchB", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_CyprB", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_WhitB", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_VancB", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_CAPI", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_AvalB", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_OrchB", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_CyprB", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_WhitB", _operationList);
            testnameOperationListMap.Add("DC_DDM_TempProfile_Test_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_LT);
            testnameOperationListMap.Add("DC_DDM_Test_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_HT);
            testnameOperationListMap.Add("DC_DDM_Test_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("DC_DDM_Test_VancB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_ML);
            testnameOperationListMap.Add("EEPROMRead_ML_v01", _operationList);
            testnameOperationListMap.Add("DDMTest_CLR4_ML_01", _operationList);
            testnameOperationListMap.Add("DDMTest_PSM4_ML_01", _operationList);
            testnameOperationListMap.Add("DDMTest_ML_v01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_QBmin);


            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CAL_MOD_TX);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_TX", "9803");
            testnameOperationListMap.Add("CalMod_TxCal_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_TxCal_PSM4_V02", _operationList);
            testnameOperationListMap.Add("CalMod_TxCal_PSM4_V03", _operationList);
            testnameOperationListMap.Add("CalMod_TxCal_CLR4_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CAL_MOD_VOLTAGE);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_VOLTAGE", "9804");
            testnameOperationListMap.Add("CalMod_VCC_Test_CAPI", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_CLR4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_AvalB", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_OrchB", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_CyprB", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_WhitB", _operationList);
            testnameOperationListMap.Add("CalMod_VCC_Test_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_EYE_PRE);
            //_operationList.Add("RETEST_OPER_HS_EYE_PRE", "9112");			
            _operationList.Add(OPER_HS_EYE_POST);
            //_operationList.Add("RETEST_OPER_HS_EYE_POST", "9212");			
            testnameOperationListMap.Add("HS_EyeTest_PSM4_V01", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_PSM4_Parallel_V01", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CLR4_Parallel_V01", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CLR4_V01", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CAPI", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_AvalB", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_OrchB", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CyprB", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_WhitB", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_EYE_COLD);
            testnameOperationListMap.Add("HS_EyeTest_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_EYE_ROOM);
            testnameOperationListMap.Add("HS_EyeTest_AvalB_Room", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_OrchB_Room", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CyprB_Room", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_VancB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_EYE_HOT);
            testnameOperationListMap.Add("HS_EyeTest_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_EyeTest_VancB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_PRE);
            _operationList.Add(OPER_HS_PAM4_POST);
            testnameOperationListMap.Add("HS_PAM4Test_CAPI", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_CAPI_PreOpt", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_COLD);
            testnameOperationListMap.Add("HS_PAM4Test_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_ROOM);
            testnameOperationListMap.Add("HS_PAM4Test_AvalB_Room", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_OrchB_Room", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_CyprB_Room", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_VancB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_HOT);
            testnameOperationListMap.Add("HS_PAM4Test_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_VancB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_TeleB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_200G_COLD);
            testnameOperationListMap.Add("HS_PAM4Test_200G_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_200G_TeleB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_200G_ROOM);
            testnameOperationListMap.Add("HS_PAM4Test_200G_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_200G_TeleB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_PAM4_200G_HOT);
            testnameOperationListMap.Add("HS_PAM4Test_200G_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_PAM4Test_200G_TeleB_Hot", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_HS_TDECQ_SET);
            testnameOperationListMap.Add("HS_ML_TDECQ_InitSetting_AvalB", _operationList);
            testnameOperationListMap.Add("HS_ML_TDECQ_InitSetting_OrchB", _operationList);
            testnameOperationListMap.Add("HS_ML_TDECQ_InitSetting_CyprB", _operationList);
            testnameOperationListMap.Add("HS_ML_TDECQ_InitSetting_WhitB", _operationList);
            testnameOperationListMap.Add("HS_ML_TDECQ_InitSetting_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_TDECQ);
            testnameOperationListMap.Add("HS_TDECQ_Optimization_OrchB", _operationList);
            testnameOperationListMap.Add("HS_TDECQ_Optimization_AvalB", _operationList);
            testnameOperationListMap.Add("HS_TDECQ_Optimization_CyprB", _operationList);
            testnameOperationListMap.Add("HS_TDECQ_Optimization_WhitB", _operationList);
            testnameOperationListMap.Add("HS_TDECQ_Optimization_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_PRE);
            //_operationList.Add("RETEST_OPER_HS_RXSENS_PRE", "9102");
            _operationList.Add(OPER_HS_RXSENS_POST);
            //_operationList.Add("RETEST_OPER_HS_RXSENS_POST", "9202");						
            testnameOperationListMap.Add("RxSensitivityTest_PSM4_V01", _operationList);
            testnameOperationListMap.Add("HS_RxSensitivityTest_CLR4_V01", _operationList);
            testnameOperationListMap.Add("HS_RxSensitivityTest_CLR4_V02", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_CAPI", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_CAPI_UnitTest", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_COLD);
            testnameOperationListMap.Add("HS_SensitivityTest_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_ROOM);
            testnameOperationListMap.Add("HS_SensitivityTest_AvalB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_OrchB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_CyprB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_VancB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_TeleB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_HOT);
            testnameOperationListMap.Add("HS_SensitivityTest_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_VancB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_200G_COLD);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_TeleB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_200G_ROOM);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_TeleB_Room", _operationList);

            // dispersion penalty
            _operationList = new List<string>();
            _operationList.Add(OPER_HS_DISP_PENALTY_POST);
            testnameOperationListMap.Add("HS_DispersionPenaltyTest_CAPI", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_200G_HOT);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_SensitivityTest_200G_TeleB_Hot", _operationList);



            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_NRZ_COLD);
            testnameOperationListMap.Add("HS_SensitivityTest_NRZ_OrchB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_NRZ_ROOM);
            testnameOperationListMap.Add("HS_SensitivityTest_NRZ_OrchB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_RXSENS_NRZ_HOT);
            testnameOperationListMap.Add("HS_SensitivityTest_NRZ_OrchB_Hot", _operationList);


            //_operationList = new List<string>();
            //_operationList.Add(OPER_DC_CALIB);
            ////_operationList.Add("RETEST_OPER_DC_CALIB", "9888");
            //testnameOperationListMap.Add("TxThermalAPC_PSM4_01", _operationList);
            //testnameOperationListMap.Add("TxThermalAPC_CLR4_01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_OSA_SCAN_PRE);
            //_operationList.Add("RETEST_OPER_OSA_SCAN_PRE", "9122");			
            _operationList = new List<string>();
            _operationList.Add(OPER_OSA_SCAN_POST);
            //_operationList.Add("RETEST_OPER_OSA_SCAN_POST", "9222");			
            testnameOperationListMap.Add("SMSRScan_PSM4_Test_V00_01", _operationList);
            testnameOperationListMap.Add("OSAScanLaserSweep_Test_V00_01", _operationList);
            testnameOperationListMap.Add("OSAScanLaserSweep_Test_V00_02", _operationList);
            testnameOperationListMap.Add("OSAScanLaserSweepCLR4_Test_V00_01", _operationList);
            testnameOperationListMap.Add("OSAScanLaserSweepCLR4_Test_V00_02", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_CAPI", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_AvalB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_OrchB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_CyprB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_WhitB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanLaserSweep_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_OSA_POWER_POST);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_CAPI", _operationList);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_AvalB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_OrchB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_CyprB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_WhitB", _operationList);
            testnameOperationListMap.Add("DC_OSAScanPowerCheck_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_PLBI_PRE);
            //_operationList.Add("RETEST_OPER_DC_PLBI_PRE", "9201");

            testnameOperationListMap.Add("DC_PLBI_Test_PRE_PSM4_V01", _operationList);

            _operationList.Add(OPER_DC_PLBI_MED_1);
            //_operationList.Add("RETEST_OPER_DC_PLBI_MED_1", "9213");			

            _operationList.Add(OPER_DC_PLBI_MED_2);
            //_operationList.Add("RETEST_OPER_DC_PLBI_MED_2", "9214");			

            _operationList.Add(OPER_DC_PLBI_POST);
            //_operationList.Add("RETEST_OPER_DC_PLBI_POST", "9221");			

            _operationList.Add(OPER_DC_PLBI_OPTICAL);
            //_operationList.Add("RETEST_OPER_DC_PLBI_OPTICAL", "9231");			
            testnameOperationListMap.Add("DC_PLBI_Test_PSM4_V01", _operationList);
            testnameOperationListMap.Add("DC_PLBI_Test_CLR4_V01", _operationList);
            _operationList = new List<string>();

            _operationList.Add(OPER_DC_CAL_MOD_THERMAL);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_THERMAL", "9802");
            testnameOperationListMap.Add("CalMod_Thermal_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_PSM4_V02", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_PSM4_V03", _operationList);
            testnameOperationListMap.Add("TxThermalAPC_CLR4_01", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_DC_TX_COLD);
            testnameOperationListMap.Add("CalMod_Tx_CAPI_Cold", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_TX_ROOM);
            testnameOperationListMap.Add("CalMod_Tx_CAPI_Room", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_AvalB_Room", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_OrchB_Room", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_CyprB_Room", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_WhitB_Room", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_VancB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_TX_HOT);
            testnameOperationListMap.Add("CalMod_Tx_CAPI_Hot", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("CalMod_Tx_VancB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_COLD);
            testnameOperationListMap.Add("CalMod_Thermal_CAPI_COLD", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_AvalB_COLD", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_OrchB_COLD", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_CyprB_COLD", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_WhitB_COLD", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_VancB_COLD", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_ROOM);
            testnameOperationListMap.Add("CalMod_Thermal_CAPI_ROOM", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_AvalB_ROOM", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_OrchB_ROOM", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_CyprB_ROOM", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_WhitB_ROOM", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_VancB_ROOM", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_HOT);
            testnameOperationListMap.Add("CalMod_Thermal_CAPI_HOT", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_AvalB_HOT", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_OrchB_HOT", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_CyprB_HOT", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_WhitB_HOT", _operationList);
            testnameOperationListMap.Add("CalMod_Thermal_VancB_HOT", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_MicroscopeInspcetion_Len);
            testnameOperationListMap.Add("DC_MicroscopeInspection_Len_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_MicroscopeInspcetion_Fiber);
            testnameOperationListMap.Add("DC_MicroscopeInspection_Fiber_V01", _operationList);

            //testnameOperationListMap.Add("CalMod_TxThermalAPC_AvalB_TempCal", _operationList);
            //testnameOperationListMap.Add("CalMod_TxThermalAPC_OrchB_TempCal", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_URS_POST);
            //_operationList.Add("RETEST_OPER_HS_URS_POST", "9203");			
            _operationList.Add(OPER_HS_URS_PRE);
            //_operationList.Add("RETEST_OPER_HS_URS_PRE", "9204");			
            testnameOperationListMap.Add("HS_RxURSTest_PSM4_V01", _operationList);
            testnameOperationListMap.Add("RxSensitivityTest_PSM4_V02", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_LI);
            //_operationList.Add("RETEST_OPER_DC_LI", "9006");			
            testnameOperationListMap.Add("DC_LI_Test_PSM4_V02", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CAL_MOD_RX);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_RX" , "9801");
            testnameOperationListMap.Add("CalMod_RxCal_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_RxCal_PSM4_V02", _operationList);
            testnameOperationListMap.Add("CalMod_RxCal_CLR4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_CAPI", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_OrchB", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_AvalB", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_CyprB", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_WhitB", _operationList);
            testnameOperationListMap.Add("CalMod_Rx_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_RxLOS_Check);
            testnameOperationListMap.Add("DC_RxLOSCheck_CAPI", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_EVALUATE_DUTBIN);
            testnameOperationListMap.Add("DC_EvaluateDUTBin_CAPI", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_RxLOS_Check_HOT);
            testnameOperationListMap.Add("DC_RxLOSCheck_AvalB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_RxLOS_Check_COLD);
            testnameOperationListMap.Add("DC_RxLOSCheck_AvalB_Cold", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_HS_TEMP_LINK);
            testnameOperationListMap.Add("HS_LinkTest_CAPI", _operationList);
            testnameOperationListMap.Add("HS_LinkTest_OrchB", _operationList);
            testnameOperationListMap.Add("HS_LinkTest_AvalB", _operationList);
            testnameOperationListMap.Add("HS_LinkTest_CyprB", _operationList);
            testnameOperationListMap.Add("HS_LinkTest_WhitB", _operationList);
            testnameOperationListMap.Add("HS_LinkTest_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_SWITCH);
            testnameOperationListMap.Add("HS_SwitchTest_V01", _operationList);
            testnameOperationListMap.Add("HS_SwitchTest_AvalB_V01", _operationList);
            testnameOperationListMap.Add("HS_SwitchTest_CyprB_V01", _operationList);
            testnameOperationListMap.Add("HS_SwitchTest_WhitB_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_SWITCH_PRE);
            testnameOperationListMap.Add("HS_Pre_SwitchTest_V01", _operationList);
            testnameOperationListMap.Add("HS_Pre_SwitchTest_AvalB_V01", _operationList);
            testnameOperationListMap.Add("HS_Pre_SwitchTest_CyprB_V01 ", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CAL_MOD_IBIAS);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_IBIAS", "9806");			
            testnameOperationListMap.Add("CalMod_IBias_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_IBias_CLR4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_CAPI", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_AvalB", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_OrchB", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_CyprB", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_WhitB", _operationList);
            testnameOperationListMap.Add("CalMod_LaserBias_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_SCREEN);
            testnameOperationListMap.Add("DC_Tx_Test_Short_CLR4_V01", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_CAPI", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_AvalB", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_OrchB", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_CyprB", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_WhitB", _operationList);
            testnameOperationListMap.Add("DC_PowerCheck_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_PRE_BI);
            _operationList.Add(OPER_DC_POST_BI);
            testnameOperationListMap.Add("DC_Tx_Test_Short_V00_01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_OMA_TEST_PRE);
            //_operationList.Add("RETEST_OPER_HS_OMA_TEST_PRE", "9103");	
            _operationList.Add(OPER_HS_OMA_TEST_POST);
            testnameOperationListMap.Add("HS_OMATest_V01", _operationList);
            testnameOperationListMap.Add("HS_OMATDPTest_V01", _operationList);
            testnameOperationListMap.Add("HS_OMATestParallel_V01", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CLR4_V01", _operationList);
            testnameOperationListMap.Add("HS_OMATestParallel_CLR4_V01", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CAPI", _operationList);
            testnameOperationListMap.Add("HS_OMATest_AvalB", _operationList);
            testnameOperationListMap.Add("HS_OMATest_OrchB", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CyprB", _operationList);
            testnameOperationListMap.Add("HS_OMATest_WhitB", _operationList);
            testnameOperationListMap.Add("HS_OMATest_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_OMA_COLD);
            testnameOperationListMap.Add("HS_OMATest_AvalB_Cold", _operationList);
            testnameOperationListMap.Add("HS_OMATest_OrchB_Cold", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CyprB_Cold", _operationList);
            testnameOperationListMap.Add("HS_OMATest_WhitB_Cold", _operationList);
            testnameOperationListMap.Add("HS_OMATest_VancB_Cold", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_OMA_ROOM);
            testnameOperationListMap.Add("HS_OMATest_AvalB_Room", _operationList);
            testnameOperationListMap.Add("HS_OMATest_OrchB_Room", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CyprB_Room", _operationList);
            testnameOperationListMap.Add("HS_OMATest_WhitB_Room", _operationList);
            testnameOperationListMap.Add("HS_OMATest_VancB_Room", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_OMA_HOT);
            testnameOperationListMap.Add("HS_OMATest_AvalB_Hot", _operationList);
            testnameOperationListMap.Add("HS_OMATest_OrchB_Hot", _operationList);
            testnameOperationListMap.Add("HS_OMATest_CyprB_Hot", _operationList);
            testnameOperationListMap.Add("HS_OMATest_WhitB_Hot", _operationList);
            testnameOperationListMap.Add("HS_OMATest_VancB_Hot", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_HS_OMA_CAL);
            //_operationList.Add("RETEST_OPER_HS_OMA_CAL", "9104");			
            testnameOperationListMap.Add("HS_OMACalibration_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_CAL_MOD_QBD_CAL);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("CalMod_QBD_Cal_PSM4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_Cal_PSM4_V02", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_Cal_PSM4CR", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_Cal_CLR4_V01", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_CAPI", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_AvalB", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_OrchB", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_CyprB", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_WhitB", _operationList);
            testnameOperationListMap.Add("CalMod_QBD_VancB", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_POWERCHECK_CAL);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("TX_PSM4_PowerCheck_Cal_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_TARGETPOWER_CAL);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("CalMod_TxCal_PSM4_TargetPowerSetting_v01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_CAL_STEP1);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("CalMod_Thermal_Step1_PSM4_V01", _operationList);


            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_CAL_STEP2);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("CalMod_Thermal_Step2_PSM4_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_THERMAL_APC);
            //_operationList.Add("RETEST_OPER_DC_CAL_MOD_QBD_CAL", "9807");			
            testnameOperationListMap.Add("TxThermalAPC_CLR4_V02", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DDM_CHECK_HOUSING);
            testnameOperationListMap.Add("PostHousingPowerCheck_CLR4_01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_DC_TBI);
            testnameOperationListMap.Add("DC_TBI_Test_CLR4_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_SPC);
            testnameOperationListMap.Add("DC_SPCAutoCal_V01", _operationList);
            _operationList = new List<string>();
            _operationList.Add(OPER_SN_CHECK_CAPI);
            testnameOperationListMap.Add("DC_SNCheck_CAPI", _operationList);
            testnameOperationListMap.Add("DC_CustomerSNCheck", _operationList);
            testnameOperationListMap.Add("DC_CustomSNGenCheck", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_SKU_CONVERT);
            testnameOperationListMap.Add("DC_SKUConverter", _operationList);


            //TX SPA tests
            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_NEW);
            testnameOperationListMap.Add("TxNewAlignment_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_SETQB);
            testnameOperationListMap.Add("TxSetQB_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_ALIGNMAIN);
            testnameOperationListMap.Add("TxMainAlignment_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_EPOXY);
            testnameOperationListMap.Add("TxEpoxyDispense_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_UVCURE);
            testnameOperationListMap.Add("TxUVCure_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_DONE);
            testnameOperationListMap.Add("TxDoneStep_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_QBDiffScan);
            testnameOperationListMap.Add("SPA_QBDiffScan_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_TX_QBScan);
            testnameOperationListMap.Add("SPA_QBScan_CAPI_V01", _operationList);

            //RX SPA new testnames
            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_NEW);
            testnameOperationListMap.Add("RxNewAlignment_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_CHECKPOWER);
            testnameOperationListMap.Add("RxCheckRefPower_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_ALIGNMAIN);
            testnameOperationListMap.Add("RxMainAlignment_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_EPOXY);
            testnameOperationListMap.Add("RxEpoxyDispense_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_UVCURE);
            testnameOperationListMap.Add("RxUVCure_CAPI_V01", _operationList);

            _operationList = new List<string>();
            _operationList.Add(OPER_ALIGN_RX_DONE);
            testnameOperationListMap.Add("RxDoneStep_CAPI_V01", _operationList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="operationList"></param>
		public static void getOperationListByTestname(string testName, ref List<string> operationList)
        {
            populateTestnameOperationListMap();
            operationList.Clear();
            foreach (KeyValuePair<string, List<string>> entry in testnameOperationListMap)
            {
                if (entry.Key.Equals(testName) == true)
                {
                    operationList = entry.Value;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="operationList"></param>
	    public static void getOperationListByTestnameLike(string testName, out List<string> operationList)
        {
            populateTestnameOperationListMap();
            operationList = new List<string>();
            foreach (KeyValuePair<string, List<string>> entry in testnameOperationListMap)
            {
                if (entry.Key.Contains(testName) == true)
                {
                    operationList = entry.Value;
                    break;
                }
            }
        }

        /// <summary>
        /// Get Operation Test Location
        /// </summary>
		public static void getOperationTestLoc()
        {
            _sOperationTestLocList = new Dictionary<string, int>();

            _sOperationTestLocList.Add(OPER_DC_ETEST, 1);           // E_Test
                                                                    //OperationTestLocList .Add("9004", "40");			// RETEST_OPER_DC_ETEST			

            _sOperationTestLocList.Add(OPER_DC_PLBI_PRE, 2);            // OPER_DC_PLBI_PRE
                                                                        //OperationTestLocList .Add("9201", "41");			// RETEST_OPER_DC_PLBI_PRE			

            //OperationTestLocList .Add("8213", "42");			// "OPER_DC_PLBI_MED_1"
            //OperationTestLocList .Add("9213", "42");			// "RETEST_OPER_DC_PLBI_MED_1"

            //OperationTestLocList .Add("8214", "43");			// "OPER_DC_PLBI_MED_2"
            //OperationTestLocList .Add("9214", "43");			// "RETEST_OPER_DC_PLBI_MED_2"

            _sOperationTestLocList.Add(OPER_DC_PLBI_POST, 3);           // "OPER_DC_PLBI_POST"
                                                                        //OperationTestLocList .Add("9221", "44");			// "RETEST_OPER_DC_PLBI_POST"

            _sOperationTestLocList.Add(OPER_DC_PLBI_OPTICAL, 4);            // "OPER_DC_PLBI_OPTICAL"
                                                                            //OperationTestLocList .Add("9231", "45");			// "RETEST_OPER_DC_PLBI_OPTICAL"			

            _sOperationTestLocList.Add(OPER_DC_CAL_MOD_RX, 5);          // CalMod_RxCal_PSM4_V01
                                                                        //OperationTestLocList .Add("9801", "46");			// RETEST_OPER_DC_CAL_MOD_RX			

            _sOperationTestLocList.Add(OPER_DC_CAL_MOD_VOLTAGE, 6);         // CalMod_VCC_Test_PSM4_V01
                                                                            //OperationTestLocList .Add("9804", "47");			// RETEST_OPER_DC_CAL_MOD_VOLTAGE			

            //OperationTestLocList .Add("8806", "48");			// CalMod_IBias_PSM4_V01
            //OperationTestLocList .Add("9806", "48");			// RETEST_OPER_DC_CAL_MOD_IBIAS

            _sOperationTestLocList.Add(OPER_DC_CAL_MOD_TX, 8);          // CalMod_TxCal_PSM4_V01
                                                                        //OperationTestLocList .Add("9803", "49");			// RETEST_OPER_DC_CAL_MOD_TX			

            _sOperationTestLocList.Add(OPER_DC_CAL_MOD_THERMAL, 7);         // CalMod_Thermal_PSM4_V01
                                                                            //OperationTestLocList .Add("9802", "50");			// RETEST_OPER_DC_CAL_MOD_THERMAL			

            //OperationTestLocList .Add("8122", "51");			// OPER_OSA_SCAN_PRE
            //OperationTestLocList .Add("9122", "51");			// RETEST_OPER_OSA_SCAN_PRE

            _sOperationTestLocList.Add(OPER_OSA_SCAN_POST, 9);          // OPER_OSA_SCAN_POST
                                                                        //OperationTestLocList .Add("9222", "57");			// RETEST_OPER_OSA_SCAN_POST

            //OperationTestLocList .Add("8112", "52");			// OPER_HS_EYE_PRE
            //OperationTestLocList .Add("9112", "52");			// RETEST_OPER_HS_EYE_PRE			

            _sOperationTestLocList.Add(OPER_HS_EYE_POST, 10);           // OPER_HS_EYE_POST
                                                                        //OperationTestLocList .Add("9212", "55");			// RETEST_OPER_HS_EYE_POST			

            //OperationTestLocList .Add("8102", "53");			// OPER_HS_RXSENS_PRE
            //OperationTestLocList .Add("9102", "53");			// RETEST_OPER_HS_RXSENS_PRE

            //OperationTestLocList .Add("8202",11);			// OPER_HS_RXSENS_POST
            //OperationTestLocList .Add("9202", "56");			// RETEST_OPER_HS_RXSENS_POST			

            //OperationTestLocList .Add("8204", "64");			// OPER_HS_URS_PRE
            //OperationTestLocList .Add("9204", "64");			// RETEST_OPER_HS_URS_PRE

            _sOperationTestLocList.Add(OPER_HS_URS_POST, 11);           // OPER_HS_URS_POST
                                                                        //OperationTestLocList .Add("9203", "74");			// RETEST_OPER_HS_URS_POST			

            _sOperationTestLocList.Add(OPER_DDM_CHECK, 12);         // OPER_DDM_CHECK
                                                                    //OperationTestLocList .Add("9400", "58");			// RETEST_OPER_DDM_CHECK			

            _sOperationTestLocList.Add(OPER_DDM_CHECK_HV, 13);          // OPER_DDM_CHECK_HV
                                                                        //OperationTestLocList .Add("9401", "58");			// RETEST_OPER_DDM_CHECK_HV			

            _sOperationTestLocList.Add(OPER_DDM_CHECK_LV, 14);          // OPER_DDM_CHECK_LV
                                                                        //OperationTestLocList .Add("9402", "58");			// RETEST_OPER_DDM_CHECK_LV			

            _sOperationTestLocList.Add(OPER_DDM_CHECK_LT, 15);          // OPER_DDM_CHECK_LT
                                                                        //OperationTestLocList .Add("9403", "58");			// RETEST_OPER_DDM_CHECK_LT			

            _sOperationTestLocList.Add(OPER_EEPROM_CHECK, 16);          // OPER_EEPROM_CHECK

            //OperationTestLocList .Add("9115", "60");			// RETEST_OPER_EEPROM_CHECK		

            //OperationTestLocList .Add("8012", "61");			// DC_E_LI_CAPI_V01
            //OperationTestLocList .Add("9012", "61");			// RETEST_OPER_DC_E_LI			

            _sOperationTestLocList.Add(OPER_FW_DOWNLOAD, 0);            // DC_FW_DL_PSM4_V01
                                                                        //OperationTestLocList .Add("9887", "70");			// RETEST_OPER_FW_DOWNLOAD

            //OperationTestLocList .Add("8104", "72");			// OPER_HS_OMA_CAL
            //OperationTestLocList .Add("9104", "72");			// RETEST_OPER_HS_OMA_CAL

            //OperationTestLocList .Add("8103", "73");			// OPER_HS_OMA_TEST_PRE
            //OperationTestLocList .Add("9103", "73");			// RETEST_OPER_HS_OMA_TEST_PRE

            _sOperationTestLocList.Add(OPER_HS_OMA_TEST_POST, 17);          // OPER_HS_OMA_TEST_POST	
                                                                            //OperationTestLocList .Add("9206", "73");			// RETEST_OPER_HS_OMA_TEST_POST

            _sOperationTestLocList.Add(OPER_DC_CAL_MOD_QBD_CAL, 18);            // OPER_DC_CAL_MOD_QBD_CAL					
                                                                                //OperationTestLocList .Add("9807", "71");			// RETEST_OPER_DC_CAL_MOD_QBD_CAL

            //OperationTestLocList .Add("8006", "75");			// OPER_DC_LI					
            //OperationTestLocList .Add("9006", "75");			// RETEST_OPER_DC_LI
            _sOperationTestLocList.Add(OPER_HS_TEMP_LINK, 19);          // OPER_HS_TEMP_LINK	
            _sOperationTestLocList.Add(OPER_DC_E_LI_PRE, 20);
            _sOperationTestLocList.Add(OPER_DC_E_LI_POST, 21);
            _sOperationTestLocList.Add(OPER_FW_OVERLAY, 22);

            _sOperationTestLocList.Add(OPER_DDM_CHECK_ML, 23);
            _sOperationTestLocList.Add(OPER_DDM_CHECK_QBmin, 24);
            _sOperationTestLocList.Add(OPER_HS_SWITCH, 25);
            _sOperationTestLocList.Add(OPER_SN_CHECK_CAPI, 26);
            _sOperationTestLocList.Add(OPER_SKU_CONVERT, 27);

            _sOperationTestLocList.Add(OPER_OQC, 28);
            _sOperationTestLocList.Add(OPER_DC_E_LI, 29);
            _sOperationTestLocList.Add(OPER_DC_TX_COLD, 30);
            _sOperationTestLocList.Add(OPER_DC_TX_ROOM, 31);
            _sOperationTestLocList.Add(OPER_DC_TX_HOT, 32);
            _sOperationTestLocList.Add(OPER_DC_THERMAL_COLD, 33);
            _sOperationTestLocList.Add(OPER_DC_THERMAL_ROOM, 34);
            _sOperationTestLocList.Add(OPER_DC_THERMAL_HOT, 35);
            _sOperationTestLocList.Add(OPER_OSA_POWER_POST, 36);
            _sOperationTestLocList.Add(OPER_FW_Customization, 37);

            _sOperationTestLocList.Add(OPER_DDM_CHECK_HT, 38);
            _sOperationTestLocList.Add(OPER_DC_ETEST_OPT, 39);           // E_Test
            _sOperationTestLocList.Add(OPER_DC_ETEST_SMT, 40);
            _sOperationTestLocList.Add(OPER_COUPLING_CHECK, 41);

            _sOperationTestLocList.Add(OPER_DC_RxLOS_Check, 42);
            _sOperationTestLocList.Add(OPER_FINALMODULE_CHECK, 43);
            _sOperationTestLocList.Add(OPER_DC_RxLOS_Check_COLD, 44);
            _sOperationTestLocList.Add(OPER_DC_RxLOS_Check_HOT, 45);
            _sOperationTestLocList.Add(OPER_HS_SWITCH_PRE, 46);

            _sOperationTestLocList.Add(OPER_DC_MicroscopeInspcetion_Len, 47);
            _sOperationTestLocList.Add(OPER_DC_MicroscopeInspcetion_Fiber, 48);
            _sOperationTestLocList.Add(OPER_FW_UPGRADE, 49);
            _sOperationTestLocList.Add(OPER_DC_SOAKTEST, 50);
        }

        /// <summary>
        /// Get Operation Bin number
        /// </summary>
        public static void getOperationBin()
        {
            OperationBinList = new Dictionary<string, string>();

            //Please fill up empty number from 01-99 first and add line in the order of bin number.
            OperationBinList.Add(OPER_COUPLING_CHECK, "01");

            OperationBinList.Add(OPER_DC_ETEST_OPT, "38");          // E_Test
            OperationBinList.Add(OPER_DC_ETEST_SMT, "39");          // E_Test
            OperationBinList.Add(OPER_DC_ETEST, "40");          // E_Test		
            OperationBinList.Add(OPER_DC_PLBI_PRE, "41");           // OPER_DC_PLBI_PRE	
            OperationBinList.Add(OPER_DC_PLBI_MED_1, "42");         // "OPER_DC_PLBI_MED_1"
            OperationBinList.Add(OPER_DC_PLBI_MED_2, "43");         // "OPER_DC_PLBI_MED_2"
            OperationBinList.Add(OPER_DC_PLBI_POST, "44");          // "OPER_DC_PLBI_POST"
            OperationBinList.Add(OPER_DC_PLBI_OPTICAL, "45");           // "OPER_DC_PLBI_OPTICAL"
            OperationBinList.Add(OPER_DC_CAL_MOD_RX, "46");         // CalMod_RxCal_PSM4_V01
            OperationBinList.Add(OPER_DC_CAL_MOD_VOLTAGE, "47");            // CalMod_VCC_Test_PSM4_V01
            OperationBinList.Add(OPER_DC_CAL_MOD_IBIAS, "48");          // CalMod_IBias_PSM4_V01
            OperationBinList.Add(OPER_DC_CAL_MOD_TX, "49");         // CalMod_TxCal_PSM4_V01
            OperationBinList.Add(OPER_DC_CAL_MOD_THERMAL, "50");            // CalMod_Thermal_PSM4_V01
            OperationBinList.Add(OPER_OSA_SCAN_PRE, "51");          // OPER_OSA_SCAN_PRE
            OperationBinList.Add(OPER_HS_EYE_PRE, "52");            // OPER_HS_EYE_PRE
            OperationBinList.Add(OPER_HS_RXSENS_PRE, "53");         // OPER_HS_RXSENS_PRE
            OperationBinList.Add(OPER_HS_EYE_POST, "55");           // OPER_HS_EYE_POST
            OperationBinList.Add(OPER_HS_RXSENS_POST, "56");            // OPER_HS_RXSENS_POST
            OperationBinList.Add(OPER_OSA_SCAN_POST, "57");         // OPER_OSA_SCAN_POST
            OperationBinList.Add(OPER_DDM_CHECK, "58");         // OPER_DDM_CHECK
            OperationBinList.Add(OPER_DDM_CHECK_HV, "58");          // OPER_DDM_CHECK_HV
            OperationBinList.Add(OPER_DDM_CHECK_LV, "58");          // OPER_DDM_CHECK_LV
            OperationBinList.Add(OPER_DDM_CHECK_LT, "59");          // OPER_DDM_CHECK_LT
            OperationBinList.Add(OPER_EEPROM_CHECK, "60");          // OPER_EEPROM_CHECK
            OperationBinList.Add(OPER_DC_E_LI_PRE, "61");			// DC_E_LI_CAPI_V01		
            OperationBinList.Add(OPER_DC_E_LI_POST, "62");			// DC_E_LI_POST_V01
            OperationBinList.Add(OPER_DC_E_LI, "63");          // DC_E_LI_POST_V01
            OperationBinList.Add(OPER_HS_URS_PRE, "64");            // OPER_HS_URS_PRE		
            OperationBinList.Add(OPER_DDM_CHECK_HT, "65");          // OPER_DDM_CHECK_LT
            OperationBinList.Add(OPER_FW_UPGRADE, "69");
            OperationBinList.Add(OPER_FW_DOWNLOAD, "70");           // DC_FW_DL_PSM4_V01
            OperationBinList.Add(OPER_DC_CAL_MOD_QBD_CAL, "71");            // OPER_DC_CAL_MOD_QBD_CAL
            OperationBinList.Add(OPER_HS_OMA_CAL, "72");            // OPER_HS_OMA_CAL
            OperationBinList.Add(OPER_HS_OMA_TEST_PRE, "73");			// OPER_HS_OMA_TEST_PRE
            OperationBinList.Add(OPER_HS_URS_POST, "74");           // OPER_HS_URS_POST
            OperationBinList.Add(OPER_DC_LI, "75");         // OPER_DC_LI
            OperationBinList.Add(OPER_HS_TEMP_LINK, "80");			// OPER_HS_TEMP_LINK	
            OperationBinList.Add(OPER_DC_POWERCHECK_CAL, "81");			// OPER_DC_POWERCHECK_CAL	
            OperationBinList.Add(OPER_DC_TARGETPOWER_CAL, "82");            // OPER_DC_TARGETPOWER_CAL
            OperationBinList.Add(OPER_HS_OMA_TEST_POST, "83");			// OPER_HS_OMA_TEST_POST	
            OperationBinList.Add(OPER_FW_Customization, "84");
            OperationBinList.Add(OPER_DC_THERMAL_CAL_STEP1, "85");          // OPER_DC_THERMAL_CAL_STEP1	
            OperationBinList.Add(OPER_DC_THERMAL_CAL_STEP2, "86");          // OPER_DC_THERMAL_CAL_STEP2
            OperationBinList.Add(OPER_FW_OVERLAY, "88");			// OPER_DC_TARGETPOWER_CAL
            OperationBinList.Add(OPER_HS_SWITCH, "89");			// OPER_DC_TARGETPOWER_CAL
            OperationBinList.Add(OPER_SN_CHECK_CAPI, "90");
            OperationBinList.Add(OPER_SKU_CONVERT, "91");
            OperationBinList.Add(OPER_OQC, "92");
            OperationBinList.Add(OPER_DC_TX_COLD, "93");
            OperationBinList.Add(OPER_DC_TX_ROOM, "94");
            OperationBinList.Add(OPER_DC_TX_HOT, "95");
            OperationBinList.Add(OPER_DC_THERMAL_COLD, "96");
            OperationBinList.Add(OPER_DC_THERMAL_ROOM, "97");
            OperationBinList.Add(OPER_DC_THERMAL_HOT, "98");
            OperationBinList.Add(OPER_OSA_POWER_POST, "99");
            OperationBinList.Add(OPER_HS_RXSENS_COLD, "100");            // OPER_HS_RXSENS_COLD
            OperationBinList.Add(OPER_HS_RXSENS_ROOM, "101");            // OPER_HS_RXSENS_ROOM
            OperationBinList.Add(OPER_HS_RXSENS_HOT, "102");            // OPER_HS_RXSENS_HOT
            OperationBinList.Add(OPER_HS_RXSENS_NRZ_COLD, "103");            // OPER_HS_RXSENS_COLD
            OperationBinList.Add(OPER_HS_RXSENS_NRZ_ROOM, "104");            // OPER_HS_RXSENS_ROOM
            OperationBinList.Add(OPER_HS_RXSENS_NRZ_HOT, "105");            // OPER_HS_RXSENS_HOT
            OperationBinList.Add(OPER_DC_RxLOS_Check, "106");
            OperationBinList.Add(OPER_FINALMODULE_CHECK, "107");
            OperationBinList.Add(OPER_DC_RxLOS_Check_COLD, "108");
            OperationBinList.Add(OPER_DC_RxLOS_Check_HOT, "109");
            OperationBinList.Add(OPER_HS_SWITCH_PRE, "110");
            OperationBinList.Add(OPER_DC_MicroscopeInspcetion_Len, "111");
            OperationBinList.Add(OPER_DC_MicroscopeInspcetion_Fiber, "112");
            //Please fill up empty number from 01-99 first and add line in the order of bin number.
        }
    }

    /// <summary>
    /// 
    /// </summary>
	public static class SIP_ProductConstants
    {
        /// <summary>
        /// 
        /// </summary>
		public const string Tx = "Tx";
        /// <summary>
        /// 
        /// </summary>
		public const string Rx = "Rx";
        /// <summary>
        /// 
        /// </summary>
        public const string TxRx = "TxRx"; // For EndFace MTP-type connector
        /// <summary>
        /// 
        /// </summary>
        public const string Micro = "Micro";
        /// <summary>
        /// 
        /// </summary>
		public const string Module = "Module";
        /// <summary>
        /// 
        /// </summary>
		public const string GenericDevice = "Device";
        /// <summary>
        /// 
        /// </summary>
		public const string ChannelLink = "ChannelLink";

        //01h 4x25
        /// <summary>
        /// 
        /// </summary>
        public const int PRODUCTID_4x25 = 0x1;
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTCODE_4x25 = "01";
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTNAME_4x25 = "4x25";

        //02h CWDM
        /// <summary>
        /// 
        /// </summary>
        public const int PRODUCTID_CWDM = 0x2;
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTCODE_CWDM = "02";
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTNAME_CWDM = "CWDM";

        //03h QSFP
        /// <summary>
        /// 
        /// </summary>
        public const int PRODUCTID_QSFP_MM = 0x3;
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTCODE_QSFP = "03";
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTNAME_QSFP = "QSFP";
        //04h PSM4
        /// <summary>
        /// 
        /// </summary>
        public const int PRODUCTID_PSM4_MM = 0x4;
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTCODE_PSM4 = "04";
        /// <summary>
        /// 
        /// </summary>
		public const string PRODUCTNAME_PSM4 = "PSM4";
    }
}
