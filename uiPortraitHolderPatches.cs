using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace FTK_MultiMax_Rework {
    public static class uiPortraitHolderPatches {
        private static FieldInfo m_CarrierPassengersField;

        public static void LoadFix(uiPortraitHolder __instance) {
            Debug.Log("Before UpdateDisplay method");

            if (m_CarrierPassengersField == null) {
                m_CarrierPassengersField = AccessTools.Field(typeof(uiPortraitHolder), "m_CarrierPassengers");
            }

            if (m_CarrierPassengersField != null) {
                List<CharacterOverworld> carrierPassengers = m_CarrierPassengersField.GetValue(__instance) as List<CharacterOverworld>;

                if (carrierPassengers != null) {
                    Debug.Log($"m_CarrierPassengers count: {carrierPassengers.Count}");

                    if (__instance.m_PortraitActionPoints != null) {
                        Debug.Log($"m_PortraitActionPoints count: {__instance.m_PortraitActionPoints.Count}");

                        for (int i = 0; i < __instance.m_PortraitActionPoints.Count; i++) {
                            if (i < carrierPassengers.Count) {
                                Debug.Log($"Checking index {i} for m_PortraitActionPoints with CarrierPassenger");
                                __instance.m_PortraitActionPoints[i].CalculateShouldShow(carrierPassengers[i], _alwaysShowPortrait: true);
                            } else {
                                Debug.LogError($"Index {i} exceeds CarrierPassengers count");
                            }
                        }

                        for (int i = 0; i < __instance.m_PortraitActionPoints.Count; i++) {
                            if (i < __instance.m_HexLand.m_PlayersInHex.Count) {
                                Debug.Log($"Checking index {i} for m_PortraitActionPoints with PlayersInHex");
                                __instance.m_PortraitActionPoints[i].CalculateShouldShow(__instance.m_HexLand.m_PlayersInHex[i]);
                            } else {
                                Debug.LogError($"Index {i} exceeds PlayersInHex count");
                            }
                        }
                    } else {
                        Debug.Log("m_PortraitActionPoints is null");
                    }
                } else {
                    Debug.LogError("m_CarrierPassengers is null");
                }
            } else {
                Debug.LogError("m_CarrierPassengersField is null");
            }
        }
    }
}
