using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using BepInEx.Configuration;
using COM3D2API;
using LillyUtill.MyMaidActive;
using LillyUtill.MyWindowRect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace COM3D25.PresetLoadCtr.Plugin
{

    public class PresetLoadCtrPanel : UniverseLib.UI.Panels.PanelBase
    {
        #region
        internal ConfigFile Config;
        ManualLogSource log;

        //internal static List<string> listWear = new List<string>();
        //internal static List<string> listBody = new List<string>();
        //internal static List<string> listAll = new List<string>();
        //internal static List<string> lists = new List<string>();

        private static ConfigEntry<int> selGridMod;//= (int)ModType.AllMaid_RandomPreset;
        private static ConfigEntry<int> selGridList;//= (int)ListType.All;
        private static ConfigEntry<int> selGridPreset;// = (int)CharacterMgrPatch.PresetType.none;
        public static int SelGridPreset { get => selGridPreset.Value; set => selGridPreset.Value = value; }
        public static int SelGridList { get => selGridList.Value; set => selGridList.Value = value; }
        public static int SelGridMod { get => selGridMod.Value; set => selGridMod.Value = value; }

        internal static string[] namesMod;
        internal static string[] namesList;
        internal static string[] namesPreset;



        //internal static ConfigEntry<bool> isAuto;
        //internal static ConfigEntry<bool> Maid_SetProp_log;

        public static System.Windows.Forms.OpenFileDialog openDialog;
        #endregion

        public PresetLoadCtrPanel(UIBase owner, ConfigFile Config, BepInEx.Logging.ManualLogSource log) : base(owner)
        {
            this.Config = Config;
            this.log = log;

            selGridPreset = Config.Bind("ConfigFile", "selGridPreset", (int)PresetLoadPatch.PresetType.none);
            selGridList = Config.Bind("ConfigFile", "selGridList", (int)PresetLoadUtill.ListType.All);
            selGridMod = Config.Bind("ConfigFile", "selGridMod", (int)PresetLoadUtill.ModType.AllMaid_RandomPreset);

            //isAuto = Config.Bind("ConfigFile", "isAuto", false);

            //Maid_SetProp_log = Config.Bind("Maid", "SetProp log", true);

            namesMod = Enum.GetNames(typeof(PresetLoadUtill.ModType));
            namesPreset = Enum.GetNames(typeof(PresetLoadPatch.PresetType));
            namesList = Enum.GetNames(typeof(PresetLoadUtill.ListType));


        }

        public override string Name => MyAttribute.PLAGIN_NAME;
        public override int MinWidth => 100;
        public override int MinHeight => 600;
        public override Vector2 DefaultAnchorMin => new Vector2(0.25f, 0.25f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.75f, 0.75f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {

            // ------------------------------

            UIFactory.SetLayoutElement(UIFactory.CreateLabel(ContentRoot, "Wear Preset file", PresetLoadUtill.listWear.Count.ToString()).gameObject);
            UIFactory.SetLayoutElement(UIFactory.CreateLabel(ContentRoot, "Body Preset file", PresetLoadUtill.listBody.Count.ToString()).gameObject);
            UIFactory.SetLayoutElement(UIFactory.CreateLabel(ContentRoot, "Wear/Body Preset file", PresetLoadUtill.listAll.Count.ToString()).gameObject);
            UIFactory.SetLayoutElement(UIFactory.CreateLabel(ContentRoot, "All Preset file", PresetLoadUtill.lists.Count.ToString()).gameObject);
            /*
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            GUILayout.Label("Wear Preset file : " + PresetLoadUtill.listWear.Count);
            GUILayout.Label("Body Preset file : " + PresetLoadUtill.listBody.Count);
            GUILayout.Label("Wear/Body Preset file : " + PresetLoadUtill.listAll.Count);
            GUILayout.Label("All  Preset file : " + PresetLoadUtill.lists.Count);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("List load")) { PresetLoadUtill.LoadList(); }
            if (GUILayout.Button($"Log {PresetLoadUtill.Maid_SetProp_log.Value}")) { PresetLoadUtill.Maid_SetProp_log.Value = !PresetLoadUtill.Maid_SetProp_log.Value; }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("preset load")) { PresetLoadUtill.presetLoad(SelGridPreset); };
            if (GUILayout.Button("preset save")) { PresetLoadUtill.presetSave(); };
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Random Run")) { PresetLoadUtill.RandPresetRun(SelGridList, SelGridMod); }
            if (GUILayout.Button("Random Auto " + PresetLoadUtill.IsAuto)) { PresetLoadUtill.IsAuto = !PresetLoadUtill.IsAuto; }
            GUILayout.EndHorizontal();


            GUILayout.Label("PresetType " + SelGridPreset);
            SelGridPreset = GUILayout.Toolbar(SelGridPreset, namesPreset);

            if (GUI.changed)
            {
                PresetLoadPatch.presetType = (PresetLoadPatch.PresetType)SelGridPreset;
                GUI.changed = false;
            }

            GUILayout.Label("ListType " + SelGridList);
            SelGridList = GUILayout.SelectionGrid(SelGridList, namesList, 2);
            GUILayout.Label("ModType " + SelGridMod);
            SelGridMod = GUILayout.SelectionGrid(SelGridMod, namesMod, 1);
            if ((PresetLoadUtill.ModType)SelGridMod == PresetLoadUtill.ModType.OneMaid)
            {

                PresetLoadUtill.selGridmaid = MaidActiveUtill.SelectionGrid(PresetLoadUtill.selGridmaid);
            }

            GUILayout.EndScrollView();
            */
        }

        // override other methods as desired

    }
}