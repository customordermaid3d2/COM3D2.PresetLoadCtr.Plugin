using COM3D2.LillyUtill;
using COM3D2.PresetLoadCtr.Plugin;
using HarmonyLib;
using scoutmode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COM3D2.PresetLoadCtr.Plugin
{
    //[MyHarmony(MyHarmonyType.Base)]
    class PresetLoadPatch
    {

        public static PresetType presetType = PresetType.none;

        private static bool isNewMaid = false;

        public enum PresetType
        {
            none,
            Wear,
            Body,
            All
        }

        // public void PresetSet(Maid f_maid, CharacterMgr.Preset f_prest, bool forceBody = false) // 157
        // public void PresetSet(Maid f_maid, CharacterMgr.Preset f_prest) // 155
        // 테스팅 완료
        [HarmonyPatch(typeof(CharacterMgr), "PresetSet", new Type[] { typeof(Maid), typeof(CharacterMgr.Preset) })]
        [HarmonyPrefix]
        public static void PresetSet(Maid f_maid, CharacterMgr.Preset f_prest)
        {
            PresetLoadCtr.myLog.Log("PresetSet.Prefix"
            , f_maid.status.fullNameEnStyle
            , f_prest.strFileName
            );
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
        /*
        public static Maid[] m_gcActiveMaid;

        [HarmonyPatch(typeof(CharacterMgr), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void CharacterMgrConstructor(Maid[] ___m_gcActiveMaid)
        {
            m_gcActiveMaid = ___m_gcActiveMaid;
            PresetLoadCtr.myLog.LogMessage("CharacterMgr.Constructor ");
        }
        */

       // public static string[] namesMaid = new string[18];
       /*
        [HarmonyPatch(typeof(CharacterMgr), "SetActive")]
        [HarmonyPostfix]
        public static void SetActive(Maid f_maid, int f_nActiveSlotNo, bool f_bMan)
        {
            //if (configEntryUtill["SetActive", false])
            //    MyLog.LogMessage("CharacterMgr.SetActive", f_nActiveSlotNo, MyUtill.GetMaidFullName(f_maid));
            if (!f_bMan)
                namesMaid[f_nActiveSlotNo] = f_maid.status.fullNameEnStyle;
                //namesMaid[f_nActiveSlotNo] = MyUtill.GetMaidFullName(f_maid);
        }

        // private void SetActive(Maid f_maid, int f_nActiveSlotNo, bool f_bMan)
        [HarmonyPatch(typeof(CharacterMgr), "Deactivate")]
        [HarmonyPrefix]
        public static void Deactivate(int f_nActiveSlotNo, bool f_bMan)
        {
            //if (configEntryUtill["Deactivate", false])
            //    MyLog.LogMessage("CharacterMgr.Deactivate", f_nActiveSlotNo);// HarmonyPrefix로 했는데도 m_gcActiveMaid 에선 제거되있네
            if (!f_bMan)
                namesMaid[f_nActiveSlotNo] = string.Empty;

        }
        */
        /// <summary>
        /// 고용 ok 누를시
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(MaidManagementMain), "Employment")]
        public static void Employment(string ___new_edit_label_)
        {
            PresetLoadCtr.myLog.LogMessage("MaidManagementMain.Employment", PresetLoadUtill.IsAuto);
            isNewMaid = true;
        }

        ///
        [HarmonyPostfix, HarmonyPatch(typeof(ScoutMainScreenManager), "AddScoutMaid")]
        public static void AddScoutMaid(ScoutMainScreenManager __instance)
        {
            PresetLoadCtr.myLog.LogMessage("ScoutMainScreenManager.AddScoutMaid", PresetLoadUtill.IsAuto);
            isNewMaid = true;
        }


        [HarmonyPatch(typeof(SceneEdit), "OnCompleteFadeIn")]
        [HarmonyPostfix]
        public static void OnCompleteFadeIn() // Maid ___m_maid,SceneEdit __instance
        {
            PresetLoadCtr.myLog.LogMessage("SceneEdit.OnCompleteFadeIn", PresetLoadUtill.IsAuto);
            if (PresetLoadUtill.IsAuto)
            {
                newMaidSetting();
            }
        }

        /// <summary>
        /// public Maid AddStockMaid()
        /// 로딩시에도 불러와서 이방법 위험
        /// </summary>
        // [HarmonyPatch(typeof(CharacterMgr), "AddStockMaid")]
        // [HarmonyPostfix]
        // public static void AddStockMaid(Maid __result) // Maid ___m_maid,SceneEdit __instance
        // {
        //     PresetUtill.SetMaidRandPreset(__result);            /// 로딩시에도 불러와서 이방법 위험
        // }


        public static void newMaidSetting()
        {
            if (!isNewMaid)
            {
                return;
            }
            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            PersonalUtill.SetPersonalRandom(maid);
            PresetLoadUtill.RandPreset(maid);
            isNewMaid = false;
        }

    }
}
