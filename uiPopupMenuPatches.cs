using FTKItemName;
using Google2u;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static FTKInputFocus;
using static uiPopupMenu;

namespace FTK_MultiMax_Rework {
    public static class uiPopupMenuPatches {
        public static void Postfix(uiPopupMenu __instance) {
            if (__instance == null || __instance.m_Popups == null) {
                return;
            }

            PopupButton givePopup = __instance.m_Popups.FirstOrDefault(popup => popup.m_Action == uiPopupMenu.Action.Give);

            if (givePopup != null) {
                givePopup.m_Count = GameFlowMC.gMaxPlayers - 1;
            }
        }

    }

}