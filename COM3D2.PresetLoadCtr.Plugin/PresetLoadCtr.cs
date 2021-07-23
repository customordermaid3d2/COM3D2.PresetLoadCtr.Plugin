using BepInEx;
using BepInEx.Configuration;
using COM3D2.Lilly.Plugin;
using COM3D2.Lilly.Plugin.Utill;
using COM3D2.LillyUtill;
using COM3D2API;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.PresetLoadCtr.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "PresetLoadCtr";
        public const string PLAGIN_VERSION = "21.7.22";
        public const string PLAGIN_FULL_NAME = "COM3D2.PresetLoadCtr.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class PresetLoadCtr : BaseUnityPlugin
    {
        // 단축키 설정파일로 연동
        private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        public static MyLog myLog = new MyLog(MyAttribute.PLAGIN_NAME);

        Harmony harmony;

        public void Awake()
        {
            //MyLog.log = Logger;// BepInEx.Logging.Logger.CreateLogSource("PresetLoadCtr");
            PresetLoadCtr.myLog.LogMessage("Awake");
            PresetLoadCtr.myLog.LogMessage("https://github.com/customordermaid3d2/COM3D2.PresetLoadCtr.Plugin");

            /*
            var overversion = new Version(1,56);
            var gameversion = new Version(GameUty.GetBuildVersionText());

            //if (!Misc.GAME_VERSION.ToString().StartsWith("155"))
            if(gameversion > overversion)
            {
                MyLog.LogWarning("GAME_VERSION not support");
                MyLog.LogError("Misc.GAME_VERSION : "+ Misc.GAME_VERSION);
                MyLog.LogWarning("GAME_VERSION not support");

                enabled = false;
                return;
            }
            */
            
            // 단축키 기본값 설정
            ShowCounter = Config.Bind("KeyboardShortcut", "OnOff", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha8, KeyCode.LeftControl));

            // 기어 메뉴 추가. 이 플러그인 기능 자체를 멈추려면 enabled 를 꺽어야함. 그러면 OnEnable(), OnDisable() 이 작동함
            //SystemShortcutAPI.AddButton("PresetLoadCtr", new Action(delegate () { enabled = !enabled; }), "PresetLoadCtr", MyUtill.ExtractResource(Properties.Resources.icon));
            
            PresetLoadUtill.init(Config);
            PresetLoadUtill.LoadList();
        }


        public void OnEnable()
        {
            PresetLoadCtr.myLog.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(PresetLoadPatch));
            PresetLoadUtill.myWindowRect.load();
        }


        public void OnDisable()
        {
            PresetLoadUtill.myWindowRect.save();
            PresetLoadPatch.presetType = PresetLoadPatch.PresetType.none;
            harmony.UnpatchSelf();
        }



        public void OnGUI()
        {
            PresetLoadUtill.OnGUI(); 
        }

        public static string scene_name = string.Empty;

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PresetLoadCtr.myLog.LogMessage("OnSceneLoaded", scene.name, scene.buildIndex);
            //  scene.buildIndex 는 쓰지 말자 제발
            scene_name = scene.name;
        }

        public void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            if (ShowCounter.Value.IsUp())
            {
                PresetLoadUtill.isGUIOn = !PresetLoadUtill.isGUIOn;
                PresetLoadCtr.myLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

    }
}
