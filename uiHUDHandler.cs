using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FTK_MultiMax_Rework {
    public static class uiHUDHandler {
        public static bool InitHUD(ref uiHudScroller __instance, uiPlayerMainHud _playerHud, ref int ___m_Index, ref Dictionary<uiPlayerMainHud, int> ___m_TargetIndex, ref List<uiPlayerMainHud> ___m_Huds, ref float ___m_HudWidth, ref float[] ___m_Positions) {
            int num = 0;
            CharacterOverworld cow = _playerHud.m_Cow;
            if (GameLogic.Instance.IsSinglePlayer()) {
                num = cow.m_FTKPlayerID.TurnIndex + 1;
                ___m_Index = 0;
            } else {
                num = cow.m_FTKPlayerID.TurnIndex + 1;
                ___m_Index = cow.m_FTKPlayerID.TurnIndex;
            }
            ___m_TargetIndex[_playerHud] = num;
            ___m_Huds.Add(_playerHud);
            RectTransform component = _playerHud.GetComponent<RectTransform>();
            ___m_HudWidth = component.rect.width;
            Vector3 localPosition = component.localPosition;
            localPosition.y = 0f - component.anchoredPosition.y;
            if (num >= ___m_Positions.Length) {
                float[] array = new float[num + 1];
                Array.Copy(___m_Positions, array, ___m_Positions.Length);
                ___m_Positions = array;
            }
            localPosition.x = ___m_Positions[num];
            component.localPosition = localPosition;
            return false;
        }

        public static void PlaceUI(ref uiPlayerMainHud __instance) {
            int turnIndex = __instance.m_Cow.m_FTKPlayerID.TurnIndex;
            int gMaxPlayers = GameFlowMC.gMaxPlayers;
            float num = -725f;
            float num2 = 725f;
            float num3 = num2 - num;
            float num4 = __instance.GetComponent<RectTransform>().rect.width - 220f;
            float num5 = num3 / (float)gMaxPlayers;
            float num6 = Mathf.Min(1f, num5 / num4);
            __instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(num, num2, (float)turnIndex / (float)(gMaxPlayers - 1)), __instance.GetComponent<RectTransform>().anchoredPosition.y);
            __instance.GetComponent<RectTransform>().localScale = new Vector3(num6, num6, num6);
        }

        public static void AddMorePlayersToUI(ref uiPortraitHolder __result) {
            for (int i = 2; i < GameFlowMC.gMaxPlayers; i++) {
                __result.m_PortraitActionPoints.Add(UnityEngine.Object.Instantiate(__result.m_PortraitActionPoints[__result.m_PortraitActionPoints.Count - 1], __result.m_PortraitActionPoints[__result.m_PortraitActionPoints.Count - 1].transform.parent));
            }
        }
    }
}
