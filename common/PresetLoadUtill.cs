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
        //
        //private static ConfigEntry<int> selGridMod;//= (int)ModType.AllMaid_RandomPreset;
        //private static ConfigEntry<int> selGridList;//= (int)ListType.All;
        //private static ConfigEntry<int> selGridPreset;// = (int)CharacterMgrPatch.PresetType.none;
        internal static int selGridmaid = 0;

        //internal static string[] namesMod;
        //internal static string[] namesList;
        //internal static string[] namesPreset;

        internal static ConfigEntry<bool> isAuto;
        internal static bool isLoadList;

        internal static ConfigEntry<bool> Maid_SetProp_log;

        //public static int SelGridPreset { get => selGridPreset.Value; set => selGridPreset.Value = value; }
        //public static int SelGridList { get => selGridList.Value; set => selGridList.Value = value; }
        //public static int SelGridMod { get => selGridMod.Value; set => selGridMod.Value = value; }
        public static bool IsAuto { get => isAuto.Value; set => isAuto.Value = value; }

        //public static PresetType presetType = PresetType.none;

       
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
                    log.LogError("Config is null");
                }
                else
                {
                    log.LogMessage("Config is not null");
                }
                //myWindowRect = new WindowRectUtill(Config, log, "PresetLoadCtr", "PLCtr");
            }
            catch (Exception e)
            {
                PresetLoadCtr.Log.LogError("MyWindowRect : " + e.ToString());
            }

            isAuto = Config.Bind("ConfigFile", "isAuto", false);
            
            Maid_SetProp_log = Config.Bind("Maid", "SetProp log", true);


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

        internal static void presetLoad(int SelGridMod)
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

        internal static CharacterMgr.Preset presetGet()
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

        internal static void presetSave()
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

        internal static void RandPresetRun(int SelGridList, int SelGridMod)
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
