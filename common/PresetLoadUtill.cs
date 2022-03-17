using BepInEx.Configuration;

//using COM3D2.LillyUtill;
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


    class PresetLoadUtill
    {
        internal static ConfigFile Config;

        internal static System.Random rand = new System.Random();

        internal static List<string> listWear = new List<string>();
        internal static List<string> listBody = new List<string>();
        internal static List<string> listAll = new List<string>();
        internal static List<string> lists = new List<string>();

        private static ConfigEntry<int> selGridMod;//= (int)ModType.AllMaid_RandomPreset;
        private static ConfigEntry<int> selGridList;//= (int)ListType.All;
        private static ConfigEntry<int> selGridPreset;// = (int)CharacterMgrPatch.PresetType.none;
        private static int selGridmaid = 0;

        internal static string[] namesMod;
        internal static string[] namesList;
        internal static string[] namesPreset;

        internal static ConfigEntry<bool> isAuto;
        internal static bool isLoadList;

        internal static ConfigEntry<bool> Maid_SetProp_log;

        public static int SelGridPreset { get => selGridPreset.Value; set => selGridPreset.Value = value; }
        public static int SelGridList { get => selGridList.Value; set => selGridList.Value = value; }
        public static int SelGridMod { get => selGridMod.Value; set => selGridMod.Value = value; }
        public static bool IsAuto { get => isAuto.Value; set => isAuto.Value = value; }

        //public static PresetType presetType = PresetType.none;

        // private Rect windowRect = new Rect(windowSpace, windowSpace, 300f, 600f);
        public static WindowRectUtill myWindowRect;
        private static int windowId = new System.Random().Next();

        public static string windowName = MyAttribute.PLAGIN_NAME;
        public static string FullName = MyAttribute.PLAGIN_NAME;
        public static string ShortName = "SP";


        // GUI ON OFF 설정파일로 저장
        private static ConfigEntry<bool> IsGUIOn;

        public static bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        //public static MyLog myLog;

        public enum ListType
        {
            Wear,
            Body,
            WearAndBody,
            All
        }

        public enum ModType
        {
            OneMaid,
            AllMaid_OnePreset,
            AllMaid_RandomPreset
        }

        public static System.Windows.Forms.OpenFileDialog openDialog;
        static bool isShowDialogLoadRun = false;

        public static void init(ConfigFile Config, BepInEx.Logging.ManualLogSource log)
        {

            PresetLoadUtill.Config = Config;

            try
            {
                if (Config == null)
                {
                    PresetLoadCtr.Log.LogError("Config is null");
                }
                else
                {
                    PresetLoadCtr.Log.LogMessage("Config is not null");
                }
                myWindowRect = new WindowRectUtill(Config, log, "PresetLoadCtr", "PLCtr");
            }
            catch (Exception e)
            {
                PresetLoadCtr.Log.LogError("MyWindowRect : " + e.ToString());
            }
            //isOpen = isOpen;

            selGridPreset = Config.Bind("ConfigFile", "selGridPreset", (int)PresetLoadPatch.PresetType.none);
            selGridList = Config.Bind("ConfigFile", "selGridList", (int)ListType.All);
            selGridMod = Config.Bind("ConfigFile", "selGridMod", (int)ModType.AllMaid_RandomPreset);
            // 일반 설정값
            IsGUIOn = Config.Bind("GUI", "isGUIOn", false);
            isAuto = Config.Bind("ConfigFile", "isAuto", false);

            Maid_SetProp_log = Config.Bind("Maid", "SetProp log", true);

            namesMod = Enum.GetNames(typeof(ModType));
            namesPreset = Enum.GetNames(typeof(PresetLoadPatch.PresetType));
            namesList = Enum.GetNames(typeof(ListType));

            // GameMain.Instance.SerializeStorageManager.StoreDirectoryPath 는 Awake에서 못씀
            // 파일 열기창 설정 부분. 이런건 구글링 하기
            openDialog = new System.Windows.Forms.OpenFileDialog()
            {
                // 기본 확장자
                DefaultExt = "preset",
                // 기본 디렉토리
                InitialDirectory = Path.Combine(Environment.CurrentDirectory, "preset"),
                // 선택 가능 확장자
                Filter = "preset files (*.preset)|*.preset|All files (*.*)|*.*"
            };

        }

        public static void Start()
        {
            SystemShortcutAPI.AddButton("PresetLoadCtr", new Action(delegate () { isGUIOn = !isGUIOn; }), "PresetLoadCtr", ExtractResource(COM3D2.PresetLoadCtr.Plugin.Properties.Resources.icon));
        }

        public static byte[] ExtractResource(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static Vector2 scrollPosition;

        public static void OnGUI()
        {
            if (!isGUIOn)
            {
                return;
            }
            // 윈도우 리사이즈시 밖으로 나가버리는거 방지
            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, PresetLoadUtill.WindowFunction, "", GUI.skin.box);
        }

        public static void WindowFunction(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn = false; }
            GUILayout.EndHorizontal();

            if (!myWindowRect.IsOpen)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                GUILayout.Label("Wear Preset file : " + listWear.Count);
                GUILayout.Label("Body Preset file : " + listBody.Count);
                GUILayout.Label("Wear/Body Preset file : " + listAll.Count);
                GUILayout.Label("All  Preset file : " + lists.Count);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("List load")) { LoadList(); }
                if (GUILayout.Button($"Log {Maid_SetProp_log.Value}")) { Maid_SetProp_log.Value = !Maid_SetProp_log.Value;  }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("preset load")) { presetLoad(); };
                if (GUILayout.Button("preset save")) { presetSave(); };
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Random Run")) { RandPresetRun(); }
                if (GUILayout.Button("Random Auto " + IsAuto)) { IsAuto = !IsAuto; }
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
                if ((ModType)SelGridMod == ModType.OneMaid)
                {

                    selGridmaid = MaidActiveUtill.SelectionGrid(selGridmaid);
                }

                GUILayout.EndScrollView();


            }

            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }


        private static void presetLoad()
        {
            switch ((ModType)SelGridMod)
            {
                case ModType.OneMaid:
                    {
                        var maid = MaidActiveUtill.GetMaid(selGridmaid);
                        if (maid == null) return;

                        CharacterMgr.Preset preset = presetGet();
                        if (preset == null) return;

                        GameMain.Instance.CharacterMgr.PresetSet(maid, preset);

                        break;
                    }
                case ModType.AllMaid_OnePreset:
                case ModType.AllMaid_RandomPreset:
                    {
                        CharacterMgr.Preset preset = presetGet();
                        if (preset == null) return;

                        foreach (var maid in MaidActiveUtill.GetMaidAll())
                        {
                            if (maid == null)
                            {
                                continue;
                            }
                            GameMain.Instance.CharacterMgr.PresetSet(maid, preset);
                        }

                        break;
                    }
                default:
                    break;
            }
        }

        private static CharacterMgr.Preset presetGet()
        {
            CharacterMgr.Preset preset = null;
            if (isShowDialogLoadRun)
            {
                return preset;
            }
            isShowDialogLoadRun = true;
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)// 오픈했을때
            {
                preset = GameMain.Instance.CharacterMgr.PresetLoad(openDialog.FileName);
                if (preset == null)
                {
                    isShowDialogLoadRun = false;
                    return preset;
                }

                switch (PresetLoadPatch.presetType)
                {
                    case PresetLoadPatch.PresetType.Wear:
                        preset.ePreType = CharacterMgr.PresetType.Wear;
                        break;
                    case PresetLoadPatch.PresetType.Body:
                        preset.ePreType = CharacterMgr.PresetType.Body;
                        break;
                    case PresetLoadPatch.PresetType.none:
                        break;
                    case PresetLoadPatch.PresetType.All:
                        preset.ePreType = CharacterMgr.PresetType.All;
                        break;
                    default:
                        break;
                }
            }
            isShowDialogLoadRun = false;
            return preset;
        }

        private static void presetSave()
        {
            var maid = MaidActiveUtill.GetMaid(selGridmaid);
            if (maid == null)
            {
                return;
            }

            CharacterMgr.PresetType presetType = CharacterMgr.PresetType.All;
            switch (PresetLoadPatch.presetType)
            {
                case PresetLoadPatch.PresetType.Wear:
                    presetType = CharacterMgr.PresetType.Wear;
                    break;
                case PresetLoadPatch.PresetType.Body:
                    presetType = CharacterMgr.PresetType.Body;
                    break;
                case PresetLoadPatch.PresetType.none:
                case PresetLoadPatch.PresetType.All:
                    presetType = CharacterMgr.PresetType.All;
                    break;
                default:
                    break;
            }

            GameMain.Instance.CharacterMgr.PresetSave(maid, presetType);
        }

        private static void RandPresetRun()
        {
            List<string> list = lists;
            list = GetList((ListType)SelGridList, list);

            if (list.Count == 0)
            {
                PresetLoadCtr.Log.LogWarning("RandPreset No list");
                return;
            }

            Maid m_maid;
            string file;
            switch ((ModType)SelGridMod)
            {
                case ModType.OneMaid:
                    m_maid = MaidActiveUtill.GetMaid(selGridmaid);
                    if (m_maid == null)
                    {
                        break;
                    }
                    file = list[rand.Next(list.Count)];
                    SetMaidPreset(m_maid, file);
                    break;
                case ModType.AllMaid_OnePreset:
                    file = list[rand.Next(list.Count)];
                    foreach (var item in MaidActiveUtill.GetMaidAll())
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        SetMaidPreset(item, file);
                    }
                    break;
                case ModType.AllMaid_RandomPreset:
                    foreach (var item in MaidActiveUtill.GetMaidAll())
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        file = list[rand.Next(list.Count)];
                        SetMaidPreset(item, file);
                    }
                    break;
                default:
                    break;
            }
        }

        private static List<string> GetList(ListType listType, List<string> list)
        {
            switch (listType)
            {
                case ListType.Wear:
                    list = listWear;
                    break;
                case ListType.Body:
                    list = listBody;
                    break;
                case ListType.WearAndBody:
                    list = listAll;
                    break;
                case ListType.All:
                    list = lists;
                    break;
                default:
                    break;
            }

            if (list.Count == 0)
            {
                LoadList();
            }

            return list;
        }

        internal static void RandPreset(Maid m_maid = null, ListType listType = ListType.All)
        {

            if (m_maid == null)
            {
                m_maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                if (m_maid == null)
                {
                    //MyLog.LogWarning("RandPreset maid null");
                    return;
                }
            }
            PresetLoadCtr.Log.LogMessage($"RandPreset {m_maid.status.fullNameEnStyle}");

            List<string> list = lists;
            list = GetList(listType, list);

            if (list.Count == 0)
            {
                PresetLoadCtr.Log.LogWarning("RandPreset No list");
                return;
            }

            string file = list[rand.Next(list.Count)];

            SetMaidPreset(m_maid, file);
        }

        private static void SetMaidPreset(Maid m_maid, string file)
        {
            if (m_maid == null || m_maid.IsBusy)
            {
                return;
            }

            CharacterMgr.Preset preset = GameMain.Instance.CharacterMgr.PresetLoad(file);

            if (preset == null)
            {
                PresetLoadCtr.Log.LogWarning("SetMaidPreset preset null ");
                return;
            }
            try
            {
                GameMain.Instance.CharacterMgr.PresetSet(m_maid, preset);
            }
            catch (Exception e)
            {
                PresetLoadCtr.Log.LogError($"{e}");
            }
            if (Product.isPublic)
                SceneEdit.AllProcPropSeqStart(m_maid);
        }


        internal static void LoadList()
        {
            if (isLoadList)
            {
                return;
            }
            Task.Factory.StartNew(
                LoadListStart
            );
        }

        private static void LoadListStart()
        {
            isLoadList = true;

            listWear.Clear();
            listBody.Clear();
            listAll.Clear();
            lists.Clear();

            // 하위경로포함
            foreach (string f_strFileName in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Preset"), "*.preset", SearchOption.AllDirectories))
            {
                //jUnityEngine.Debug.Log("RandPreset load : " + f_strFileName);

                FileStream fileStream = new FileStream(f_strFileName, FileMode.Open);
                if (fileStream == null)
                {
                    continue;
                }
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                fileStream.Dispose();
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer));

                string a = binaryReader.ReadString();
                if (a != "CM3D2_PRESET")
                {
                    binaryReader.Close();
                    continue;
                }
                binaryReader.ReadInt32();
                switch ((CharacterMgr.PresetType)binaryReader.ReadInt32())
                {
                    case CharacterMgr.PresetType.Wear:
                        listWear.Add(f_strFileName);
                        break;
                    case CharacterMgr.PresetType.Body:
                        listBody.Add(f_strFileName);
                        break;
                    case CharacterMgr.PresetType.All:
                        listAll.Add(f_strFileName);
                        break;
                    default:
                        break;
                }
                binaryReader.Close();
            }

            lists.AddRange(listWear);
            lists.AddRange(listBody);
            lists.AddRange(listAll);

            isLoadList = false;
        }
    }


}
