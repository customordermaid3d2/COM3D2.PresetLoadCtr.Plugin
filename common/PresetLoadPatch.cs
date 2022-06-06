//using COM3D2.LillyUtill;
using HarmonyLib;
using MaidStatus;
using scoutmode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COM3D25.PresetLoadCtr.Plugin
{
    //[MyHarmony(MyHarmonyType.Base)]
    class PresetLoadPatch
    {

        public static PresetType presetType = PresetType.none;

        private static bool isNewMaid = false;
        internal static bool loaded = false;

        public enum PresetType
        {
            none,
            Wear,
            Body,
            All
        }


        // 테스팅 완료
#if COM3D2
        [HarmonyPatch(typeof(CharacterMgr), "PresetSet", new Type[] { typeof(Maid), typeof(CharacterMgr.Preset) })] // 2.0
#elif COM3D25
        [HarmonyPatch(typeof(CharacterMgr), "PresetSet", new Type[] { typeof(Maid), typeof(CharacterMgr.Preset), typeof(bool) })] // 2.5
#endif
        [HarmonyPrefix]
        public static void PresetSet( CharacterMgr.Preset f_prest)//Maid f_maid,
        {
            if (PresetLoadUtill.Maid_SetProp_log.Value && loaded && !isLoading)
            {
                PresetLoadCtr.Log.LogMessage($"PresetSet , {f_prest.strFileName}");
                foreach (var mp in f_prest.listMprop)
                {
                    PresetLoadCtr.Log.LogMessage($"SetProp , {mp.name} , {mp.strFileName} ");
                }
            }
            switch (presetType)
            {
                case PresetType.Wear:
                    f_prest.ePreType = CharacterMgr.PresetType.Wear;
                    break;
                case PresetType.Body:
                    f_prest.ePreType = CharacterMgr.PresetType.Body;
                    break;
                case PresetType.All:
                    f_prest.ePreType = CharacterMgr.PresetType.All;
                    break;
                default:
                    break;
            }
        }

        public static bool isLoading = false;

        [HarmonyPatch(typeof(CharacterMgr), "PresetListLoad")]
        [HarmonyPrefix]
        public static void PresetListLoadPre()
        {
            isLoading = true;
        }

        [HarmonyPatch(typeof(CharacterMgr), "PresetListLoad")]
        [HarmonyPostfix]
        public static void PresetListLoadPost()
        {
            isLoading = false;
        }

        // public Preset PresetLoad(string f_strFileName)
        // public CharacterMgr.Preset PresetLoad(BinaryReader brRead, string f_strFileName)
        [HarmonyPatch(typeof(CharacterMgr), "PresetLoad", typeof(BinaryReader), typeof(string))]
        [HarmonyPrefix]
        public static void PresetLoad(string f_strFileName)
        {
            if (PresetLoadUtill.Maid_SetProp_log.Value && loaded && !isLoading)
            {
                PresetLoadCtr.Log.LogMessage($"PresetLoad , {f_strFileName}");
            }
        }

        //[HarmonyPatch(typeof(Maid), "SetProp", typeof(MaidProp), typeof(string), typeof(int), typeof(bool), typeof(bool))]
        //[HarmonyPrefix]
        //public static void SetProp(MaidProp mp, string filename, int f_nFileNameRID, bool f_bTemp, bool f_bNoScale = false)
        //{
        //    if (PresetLoadUtill.Maid_SetProp_log.Value && loaded && !isLoading)
        //    {
        //        PresetLoadCtr.Log.LogMessage($"SetProp , {mp.name} , {mp.strFileName} , {filename}");
        //    }
        //}

        /// <summary>
        /// 세이브 파일 로딩 시작
        /// </summary>
        [HarmonyPatch(typeof(GameMain), "Deserialize", new Type[] { typeof(int), typeof(bool) })]
        [HarmonyPrefix]
        public static void Deserialize1()//GameMain __instance, ref bool __result, int f_nSaveNo, bool scriptExec = true)
        {
            loaded = false;
        }

        [HarmonyPatch(typeof(GameMain), "Deserialize", new Type[] { typeof(int), typeof(bool) })]
        [HarmonyPostfix]
        public static void Deserialize2()//GameMain __instance, ref bool __result, int f_nSaveNo, bool scriptExec = true)
        {
             loaded = true;
        }

        /// <summary>
        /// 고용 ok 누를시
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(MaidManagementMain), "Employment")]
        public static void Employment(string ___new_edit_label_)
        {
            PresetLoadCtr.Log.LogMessage($"MaidManagementMain.Employment { PresetLoadUtill.IsAuto}");
            isNewMaid = true;
        }

        ///
        [HarmonyPostfix, HarmonyPatch(typeof(ScoutMainScreenManager), "AddScoutMaid")]
        public static void AddScoutMaid(ScoutMainScreenManager __instance)
        {
            PresetLoadCtr.Log.LogMessage($"ScoutMainScreenManager.AddScoutMaid {PresetLoadUtill.IsAuto}" );
            isNewMaid = true;
        }


        [HarmonyPatch(typeof(SceneEdit), "OnCompleteFadeIn")]
        [HarmonyPostfix]
        public static void OnCompleteFadeIn() // Maid ___m_maid,SceneEdit __instance
        {
            PresetLoadCtr.Log.LogMessage($"SceneEdit.OnCompleteFadeIn {PresetLoadUtill.IsAuto}");
            if (PresetLoadUtill.IsAuto)
            {
                newMaidSetting();
            }
        }



        public static void newMaidSetting()
        {
            if (!isNewMaid)
            {
                return;
            }
            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            SetPersonalRandom(maid);
            PresetLoadUtill.RandPreset(maid);
            isNewMaid = false;
        }

        public static int SetPersonalRandom(Maid maid)
        {
            if (maid == null)
            {
                PresetLoadCtr.Log.LogFatal("maid null");
            }
            var p=Personal.GetAllDatas(true);

            int a = UnityEngine.Random.Range(0, p.Count);
            Personal.Data data = p[a];
            maid.status.SetPersonal(data);
            maid.status.firstName = data.uniqueName;

            return a;
        }

    }
}
