using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using COM3D2API;
using HarmonyLib;
using System;
using UnityEngine;
using UniverseLib.Config;
using UniverseLib.UI;

namespace COM3D25.PresetLoadCtr.Plugin
{
    public class MyAttribute
    {
        public const string PLAGIN_NAME = "PresetLoadCtr";
        public const string PLAGIN_VERSION = "22.02.22";
        public const string PLAGIN_FULL_NAME = "COM3D2.PresetLoadCtr.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInProcess("COM3D2x64.exe")]
    public class PresetLoadCtr : BaseUnityPlugin
    {
        #region
        // 단축키 설정파일로 연동
        private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        Harmony harmony;
        public static ManualLogSource Log;

        public static UIBase myUIBase { get; private set; }
        public static PresetLoadCtrPanel myPanel { get; private set; }


        // GUI ON OFF 설정파일로 저장
        private static ConfigEntry<bool> IsGUIOn;

        public static bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        #endregion
        public void Awake()
        {
            Log = Logger;
            PresetLoadCtr.Log.LogMessage("Awake https://github.com/customordermaid3d2/COM3D2.PresetLoadCtr.Plugin");


            // 단축키 기본값 설정
            ShowCounter = Config.Bind("KeyboardShortcut", "OnOff", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha8, KeyCode.LeftControl));

            // 일반 설정값
            IsGUIOn = Config.Bind("GUI", "isGUIOn", false);
            IsGUIOn.SettingChanged += IsGUIOn_SettingChanged;

            PresetLoadUtill.init(Config, Log);
            PresetLoadUtill.LoadList();

            UniverseLibConfig universeLibConfig = new UniverseLibConfig
            {
                Force_Unlock_Mouse = new bool?(false),
                Disable_EventSystem_Override = new bool?(true),
                Allow_UI_Selection_Outside_UIBase = new bool?(true)
            };
            UniverseLib.Universe.Init(0, UniverseInit, LogHandler, universeLibConfig);
            //UniverseInit();
        }

        private void UniverseInit()
        {
            Log.LogMessage("UniverseInit st");

            myUIBase = UniversalUI.RegisterUI(MyAttribute.PLAGIN_NAME, UiUpdate);
            myUIBase.Enabled = true;
            
            myPanel = new PresetLoadCtrPanel(myUIBase, Config, Log);
            myPanel.Enabled = true;

            Log.LogMessage("UniverseInit ed");
        }

        private static void LogHandler(string log, LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    Log.LogError(log);
                    break;
                case LogType.Assert:
                case LogType.Warning:
                    Log.LogWarning(log);
                    break;
                case LogType.Log:
                    Log.LogMessage(log);
                    break;
            }
        }

        private void IsGUIOn_SettingChanged(object sender, EventArgs e)
        {            
            myUIBase.Enabled = IsGUIOn.Value;
        }

        public void OnEnable()
        {
            PresetLoadCtr.Log.LogMessage("OnEnable");

            harmony = Harmony.CreateAndPatchAll(typeof(PresetLoadPatch));            
        }

        public void Start()
        {
            //PresetLoadUtill.Start();
            SystemShortcutAPI.AddButton("PresetLoadCtr", new Action(delegate () { myUIBase.Enabled=!myUIBase.Enabled; }), "PresetLoadCtr", COM3D2.PresetLoadCtr.Plugin.Properties.Resources.icon);
        }



        public void OnDisable()
        {
            //PresetLoadUtill.myWindowRect.save();
            PresetLoadPatch.presetType = PresetLoadPatch.PresetType.none;
            harmony?.UnpatchSelf();
        }

        internal void UiUpdate()
        {
            
        }

        /*
        public void OnGUI()
        {
            PresetLoadUtill.OnGUI(); 
        }
        */

        public void Update()
        {

            if (ShowCounter.Value.IsUp())
            {
                myUIBase.Enabled = true;
            }
        }

    }
}
