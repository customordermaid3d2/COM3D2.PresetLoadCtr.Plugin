using BepInEx;
using BepInEx.Configuration;
using COM3D2.Lilly.Plugin;
using COM3D2.Lilly.Plugin.Utill;
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
    public class PresetLoadCtr : BaseUnityPlugin
    {
        // 단축키 설정파일로 연동
        private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        // GUI ON OFF 설정파일로 저장
        private ConfigEntry<bool> IsGUIOn;
        private bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        Harmony harmony;

        public void Awake()
        {
            MyLog.LogMessage("Awake");

            // 단축키 기본값 설정
            ShowCounter = Config.Bind("KeyboardShortcut", "OnOff", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha8, KeyCode.LeftControl));

            // 일반 설정값
            IsGUIOn = Config.Bind("GUI", "isGUIOn", false);

            // 기어 메뉴 추가. 이 플러그인 기능 자체를 멈추려면 enabled 를 꺽어야함. 그러면 OnEnable(), OnDisable() 이 작동함
            SystemShortcutAPI.AddButton("PresetLoadCtr", new Action(delegate () { enabled = !enabled; }), "PresetLoadCtr", MyUtill.ExtractResource(Properties.Resources.icon));
            //SystemShortcutAPI.AddButton("Sample", new Action(delegate () { isGUIOn = !isGUIOn; }), "Sample", MyUtill.ExtractResource(Properties.Resources.sample_png));

            PresetLoadUtill.init(Config);
            PresetLoadUtill.LoadList();
        }


        public void OnEnable()
        {
            MyLog.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(PresetLoadPatch));
        }


        public void FixedUpdate()
        {
             
        }

        public void LateUpdate()
        {
             
        }

        public void OnDisable()
        {
            harmony.UnpatchSelf();
            PresetLoadPatch.presetType = PresetLoadPatch.PresetType.none;
        }

        private const float windowSpace = 40.0f;
        private Rect windowRect = new Rect(windowSpace, windowSpace, 400f, 400f);
        private int windowId = new System.Random().Next();

        public void OnGUI()
        {
            if (!isGUIOn)
            {
                return;
            }
            // 윈도우 리사이즈시 밖으로 나가버리는거 방지
            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + windowSpace, Screen.width - windowSpace);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + windowSpace, Screen.height - windowSpace);
            windowRect = GUILayout.Window(windowId, windowRect, PresetLoadUtill.WindowFunction, "PresetLoadCtr");
        }

        public static string scene_name = string.Empty;

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            MyLog.LogMessage("OnSceneLoaded", scene.name, scene.buildIndex);
            //  scene.buildIndex 는 쓰지 말자 제발
            scene_name = scene.name;
        }

        public void Pause()
        {
             
        }

        public void Resume()
        {
             
        }

        public void Start()
        {
             
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
                isGUIOn = !isGUIOn;
                MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

    }
}
