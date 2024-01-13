using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FTK_MultiMax_Rework {
    public class uiCharacterCreateRootPatches {
        public static void AddMorePlayerSlotsInMenu(ref uiCharacterCreateRoot __instance) {
            Debug.Log("[MULTIMAX] : " + __instance);
            Debug.Log("[MULTIMAX] : " + __instance.m_CreateUITargets);
            Debug.Log("[MULTIMAX] : " + SelectScreenCamera.Instance.m_PlayerTargets.Length);
            if (__instance.m_CreateUITargets.Length < GameFlowMC.gMaxPlayers) {
                Transform[] array = new Transform[GameFlowMC.gMaxPlayers];
                Transform[] array2 = new Transform[GameFlowMC.gMaxPlayers];
                Vector3 position = SelectScreenCamera.Instance.m_PlayerTargets[0].position;
                Vector3 position2 = SelectScreenCamera.Instance.m_PlayerTargets[2].position;
                for (int i = 0; i < GameFlowMC.gMaxPlayers; i++) {
                    if (i < __instance.m_CreateUITargets.Length) {
                        array[i] = __instance.m_CreateUITargets[i];
                        array2[i] = SelectScreenCamera.Instance.m_PlayerTargets[i];
                    } else {
                        array[i] = UnityEngine.Object.Instantiate(array[i - 1], array[i - 1].parent);
                        array2[i] = UnityEngine.Object.Instantiate(array2[i - 1], array2[i - 1].parent);
                    }
                }
                __instance.m_CreateUITargets = array;
                SelectScreenCamera.Instance.m_PlayerTargets = array2;
                for (int j = 0; j < __instance.m_CreateUITargets.Length; j++) {
                    __instance.m_CreateUITargets[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-550f, 550f, (float)j / (float)(__instance.m_CreateUITargets.Length - 1)), 129f);
                }
                for (int k = 0; k < SelectScreenCamera.Instance.m_PlayerTargets.Length; k++) {
                    SelectScreenCamera.Instance.m_PlayerTargets[k].position = Vector3.Lerp(position, position2, (float)k / (float)(SelectScreenCamera.Instance.m_PlayerTargets.Length - 1));
                }
            }
            Debug.Log("[MULTIMAX] : SLOT COUNT " + __instance.m_CreateUITargets.Length);
        }
    }
}
