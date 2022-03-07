using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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
        // 단축키 설정파일로 연동
        private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;


        Harmony harmony;
        public static ManualLogSource Log;

        public void Awake()
        {
            Log = Logger;
            PresetLoadCtr.Log.LogMessage("Awake https://github.com/customordermaid3d2/COM3D2.PresetLoadCtr.Plugin");

            
            // 단축키 기본값 설정
            ShowCounter = Config.Bind("KeyboardShortcut", "OnOff", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha8, KeyCode.LeftControl));


            PresetLoadUtill.init(Config);
            PresetLoadUtill.LoadList();
        }


        public void OnEnable()
        {
            PresetLoadCtr.Log.LogMessage("OnEnable");

            harmony = Harmony.CreateAndPatchAll(typeof(PresetLoadPatch));            
        }

        public void Start()
        {
            PresetLoadUtill.Start();
        }



        public void OnDisable()
        {
            //PresetLoadUtill.myWindowRect.save();
            PresetLoadPatch.presetType = PresetLoadPatch.PresetType.none;
            harmony.UnpatchSelf();
        }



        public void OnGUI()
        {
            PresetLoadUtill.OnGUI(); 
        }

        public void Update()
        {

            if (ShowCounter.Value.IsUp())
            {
                PresetLoadUtill.isGUIOn = !PresetLoadUtill.isGUIOn;                
            }
        }

    }
}
