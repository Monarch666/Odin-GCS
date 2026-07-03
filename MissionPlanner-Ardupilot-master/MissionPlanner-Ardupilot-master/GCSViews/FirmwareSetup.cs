using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using MissionPlanner.Radio;
using MissionPlanner.Utilities;
using System;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public partial class FirmwareSetup : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string lastpagename = "";

        public FirmwareSetup()
        {
            InitializeComponent();
        }

        public bool isConnected
        {
            get { return MainV2.comPort.BaseStream.IsOpen; }
        }

        public bool isDisConnected
        {
            get { return !MainV2.comPort.BaseStream.IsOpen; }
        }

        public bool isTracker
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == MissionPlanner.ArduPilot.Firmwares.ArduTracker; }
        }

        public bool isCopter
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == MissionPlanner.ArduPilot.Firmwares.ArduCopter2; }
        }

        public bool isCopter35plus
        {
            get { return MainV2.comPort.MAV.cs.version >= Version.Parse("3.5"); }
        }

        public bool isHeli
        {
            get { return isConnected && MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER; }
        }

        public bool isQuadPlane
        {
            get
            {
                return isConnected && isPlane &&
                       MainV2.comPort.MAV.param.ContainsKey("Q_ENABLE") &&
                       (MainV2.comPort.MAV.param["Q_ENABLE"].Value == 1.0);
            }
        }

        public bool isPlane
        {
            get
            {
                return isConnected &&
                       (MainV2.comPort.MAV.cs.firmware == MissionPlanner.ArduPilot.Firmwares.ArduPlane ||
                        MainV2.comPort.MAV.cs.firmware == MissionPlanner.ArduPilot.Firmwares.Ateryx);
            }
        }

        public bool isRover
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == MissionPlanner.ArduPilot.Firmwares.ArduRover; }
        }

        public bool gotAllParams
        {
            get
            {
                log.InfoFormat("TotalReceived {0} TotalReported {1}", MainV2.comPort.MAV.param.TotalReceived,
                    MainV2.comPort.MAV.param.TotalReported);
                if (MainV2.comPort.MAV.param.TotalReceived < MainV2.comPort.MAV.param.TotalReported)
                {
                    return false;
                }

                return true;
            }
        }

        public BackstageViewPage AddBackstageViewPage(Type userControl, string headerText, bool enabled = true,
            BackstageViewPage Parent = null, bool advanced = false)
        {
            try
            {
                if (enabled)
                    return backstageView.AddPage(userControl, headerText, Parent, advanced);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }

            return null;
        }

        public void Activate()
        {
            LoadPages();
        }

        private void FirmwareSetup_Load(object sender, EventArgs e)
        {
            LoadPages();
        }

        private void LoadPages()
        {
            backstageView.Clear();
            ResourceManager rm = new ResourceManager(typeof(InitialSetup));

            // 1. Firmware Installation Options
            if (MainV2.DisplayConfiguration.displayInstallFirmware)
            {
                AddBackstageViewPage(typeof(ConfigFirmwareDisabled), rm.GetString("backstageViewPagefw.Text"),
                    isConnected);
                AddBackstageViewPage(typeof(ConfigFirmwareManifest), rm.GetString("backstageViewPagefw.Text"),
                    isDisConnected);
                AddBackstageViewPage(typeof(ConfigFirmware), rm.GetString("backstageViewPagefw.Text") + " Legacy",
                    isDisConnected);
            }

            AddBackstageViewPage(typeof(ConfigSecureAP), "Secure",
                isDisConnected);

            // 2. Optional Hardware Options
            var opt = AddBackstageViewPage(typeof(ConfigOptional), rm.GetString("backstageViewPageopt.Text"));
            if (MainV2.DisplayConfiguration.displayRTKInject)
            {
                var rtcmStr = rm.GetString("backstageViewPageSerialInjectGPS.Text");
                if (rtcmStr == null)
                {
                    rtcmStr = "RTK/GPS Inject";
                }
                AddBackstageViewPage(typeof(ConfigSerialInjectGPS), rtcmStr, true, opt);
            }

            AddBackstageViewPage(typeof(ConfigCubeID), "CubeID Update",
                isConnected, opt);

            if (MainV2.DisplayConfiguration.displaySikRadio)
            {
                AddBackstageViewPage(typeof(Sikradio), rm.GetString("backstageViewPageSikradio.Text"), true, opt);
            }

            if (MainV2.DisplayConfiguration.displayGPSOrder)
                AddBackstageViewPage(typeof(ConfigGPSOrder), "CAN GPS Order", isConnected && gotAllParams, opt);

            if (MainV2.DisplayConfiguration.displayBattMonitor)
            {
                AddBackstageViewPage(typeof(ConfigBatteryMonitoring), rm.GetString("backstageViewPagebatmon.Text"), isConnected && gotAllParams, opt);
                AddBackstageViewPage(typeof(ConfigBatteryMonitoring2), rm.GetString("backstageViewPageBatt2.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayCAN)
            {
                AddBackstageViewPage(typeof(ConfigDroneCAN), "DroneCAN/UAVCAN", true, opt);
            }
            if (MainV2.DisplayConfiguration.displayJoystick)
            {
                AddBackstageViewPage(typeof(Joystick.JoystickSetup), "Joystick", true, opt);
            }

            if (MainV2.DisplayConfiguration.displayCompassMotorCalib)
            {
                AddBackstageViewPage(typeof(ConfigCompassMot), rm.GetString("backstageViewPagecompassmot.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayRangeFinder)
            {
                AddBackstageViewPage(typeof(ConfigHWRangeFinder), rm.GetString("backstageViewPagesonar.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayAirSpeed)
            {
                AddBackstageViewPage(typeof(ConfigHWAirspeed), rm.GetString("backstageViewPageairspeed.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayPx4Flow)
            {
                AddBackstageViewPage(typeof(ConfigHWPX4Flow), rm.GetString("backstageViewPagePX4Flow.Text"), true, opt);
            }
            if (MainV2.DisplayConfiguration.displayOpticalFlow)
            {
                AddBackstageViewPage(typeof(ConfigHWOptFlow), rm.GetString("backstageViewPageoptflow.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayOsd)
            {
                AddBackstageViewPage(typeof(ConfigHWOSD), rm.GetString("backstageViewPageosd.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayCameraGimbal)
            {
                AddBackstageViewPage(typeof(ConfigMount), rm.GetString("backstageViewPagegimbal.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayAntennaTracker)
            {
                AddBackstageViewPage(typeof(ConfigAntennaTracker), rm.GetString("backstageViewPageAntTrack.Text"), isTracker, opt);
            }
            if (MainV2.DisplayConfiguration.displayMotorTest)
            {
                AddBackstageViewPage(typeof(ConfigMotorTest), rm.GetString("backstageViewPageMotorTest.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayBluetooth)
            {
                AddBackstageViewPage(typeof(ConfigHWBT), rm.GetString("backstageViewPagehwbt.Text"), true, opt);
            }
            if (MainV2.DisplayConfiguration.displayParachute)
            {
                AddBackstageViewPage(typeof(ConfigHWParachute), rm.GetString("backstageViewPageParachute.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayEsp)
            {
                AddBackstageViewPage(typeof(ConfigHWESP8266), rm.GetString("backstageViewPageESP.Text"), isConnected && gotAllParams, opt);
            }
            if (MainV2.DisplayConfiguration.displayAntennaTracker)
            {
                AddBackstageViewPage(typeof(Antenna.TrackerUI), "Antenna Tracker", true, opt);
            }
            if (MainV2.DisplayConfiguration.displayFFTSetup)
            {
                AddBackstageViewPage(typeof(ConfigFFT), "FFT Setup", isConnected && gotAllParams, opt);
            }

            // 3. Advanced Options
            if (MainV2.DisplayConfiguration.isAdvancedMode)
            {
                var adv = AddBackstageViewPage(typeof(ConfigAdvanced), "Advanced");

                if (MainV2.DisplayConfiguration.displayTerminal)
                {
                    AddBackstageViewPage(typeof(ConfigTerminal), "Terminal", true, adv);
                }

                if (MainV2.DisplayConfiguration.displayREPL)
                {
                    AddBackstageViewPage(typeof(ConfigREPL), "Script REPL", isConnected, adv);
                }
            }

            // Remember last page accessed
            bool activated = false;
            foreach (BackstageViewPage page in backstageView.Pages)
            {
                if (page.LinkText == lastpagename && page.Show)
                {
                    backstageView.ActivatePage(page);
                    activated = true;
                    break;
                }
            }
            if (!activated && backstageView.Pages.Count > 0)
            {
                foreach (BackstageViewPage page in backstageView.Pages)
                {
                    if (page.Show)
                    {
                        backstageView.ActivatePage(page);
                        break;
                    }
                }
            }

            ThemeManager.ApplyThemeTo(this);
        }

        private void FirmwareSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}
