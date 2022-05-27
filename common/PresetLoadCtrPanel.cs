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
using UniverseLib.UI.Models;

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

            MaidActiveUtill.deactivate += MaidActiveUtill_Active;
            MaidActiveUtill.setActive += MaidActiveUtill_Active;
        }

        private void MaidActiveUtill_Active()
        {
            dropdown4.RefreshShownValue();
        }

        public override string Name => MyAttribute.PLAGIN_NAME;
        public override int MinWidth => 100;
        public override int MinHeight => 600;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.25f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.75f);
        public override bool CanDragAndResize => true;

        public override Vector2 DefaultPosition => new Vector2(0.80f, 0.20f);

        public override GameObject UIRoot => base.UIRoot;

        LayoutElement SetLayoutElement(GameObject parent, string name, string text, Action OnClick, UnityEngine.Color? normalColor = null)
        {
            ButtonRef ExpandButton = UIFactory.CreateButton(parent, name, text, normalColor);
            ExpandButton.OnClick += OnClick;
            return UIFactory.SetLayoutElement(ExpandButton.Component.gameObject);
        }

        LayoutElement SetLayoutElement(GameObject parent, string name, string text)
        {
            return UIFactory.SetLayoutElement(UIFactory.CreateLabel(parent, name, text).gameObject);
        }

        private Dropdown dropdown1;
        private Dropdown dropdown2;
        private Dropdown dropdown3;
        private Dropdown dropdown4;

        protected override void ConstructPanelContent()
        {

            // ------------------------------
            try
            {

                SetLayoutElement(ContentRoot, "Wear Preset file", PresetLoadUtill.listWear.Count.ToString());
                SetLayoutElement(ContentRoot, "Body Preset file", PresetLoadUtill.listBody.Count.ToString());
                SetLayoutElement(ContentRoot, "Wear/Body Preset file", PresetLoadUtill.listAll.Count.ToString());
                SetLayoutElement(ContentRoot, "All Preset file", PresetLoadUtill.lists.Count.ToString());
                /*
                ButtonRef ExpandButton = UIFactory.CreateButton(ContentRoot, "List load", "List load");
                ExpandButton.OnClick = () => PresetLoadUtill.LoadList();
                UIFactory.SetLayoutElement(ExpandButton.Component.gameObject);
                */
                SetLayoutElement(ContentRoot, "List load", "List load", PresetLoadUtill.LoadList);// TODO: 여기서 계속 오류

                SetLayoutElement(ContentRoot, "Preset Log", $"Preset Log {PresetLoadUtill.Maid_SetProp_log.Value}", () => { PresetLoadUtill.Maid_SetProp_log.Value = !PresetLoadUtill.Maid_SetProp_log.Value; });
                //UIFactory.CreateToggle(ContentRoot, "Preset Log", out var toggle, out var txt);

                SetLayoutElement(ContentRoot, "preset load", "preset load", () => { PresetLoadUtill.presetLoad(SelGridPreset); });
                SetLayoutElement(ContentRoot, "preset save", "preset save", PresetLoadUtill.presetSave);

                SetLayoutElement(ContentRoot, "Random Run", "Random Run", () => { PresetLoadUtill.RandPresetRun(SelGridList, SelGridMod); });
                SetLayoutElement(ContentRoot, "Random Auto", "Random Run" + PresetLoadUtill.IsAuto, () => { PresetLoadUtill.IsAuto = !PresetLoadUtill.IsAuto; });

                UIFactory.SetLayoutElement(UIFactory.CreateDropdown(ContentRoot, "PresetType", out dropdown1, namesPreset[SelGridPreset], 14
                    , (v) =>
                    {
                        PresetLoadPatch.presetType = (PresetLoadPatch.PresetType)(SelGridPreset = v);
                    }
                    , namesPreset));
                //dropdown.value = SelGridPreset;
                //dropdown.RefreshShownValue();

                UIFactory.SetLayoutElement(UIFactory.CreateDropdown(ContentRoot, "PresetType", out dropdown2, namesPreset[SelGridList], 14
                    , (v) =>
                    {
                        SelGridList = v;
                    }
                    , namesList));

                UIFactory.SetLayoutElement(UIFactory.CreateDropdown(ContentRoot, "ListType", out dropdown3, namesPreset[SelGridMod], 14
                    , (v) =>
                    {
                        SelGridMod = v;
                    }
                    , namesMod));

                UIFactory.SetLayoutElement(UIFactory.CreateDropdown(ContentRoot, "ModType", out dropdown4, MaidActiveUtill.GetMaidName(PresetLoadUtill.selGridmaid), 14
                    , (v) =>
                    {
                        PresetLoadUtill.selGridmaid = v;
                    }
                    , MaidActiveUtill.maidNames));
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
        }



    }
}