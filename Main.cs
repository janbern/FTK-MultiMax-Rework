using BepInEx;
using BepInEx.Configuration;
using Google2u;
using HarmonyLib;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FTK_MultiMax_Rework {
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]

    public class Main : BaseUnityPlugin {
        private const string pluginGuid = "fortheking.edm.multimaxrework";
        private const string pluginName = "MultiMaxRework";
        private const string pluginVersion = "1.2";

        private static Harmony Harmony { get; set; } = new Harmony(pluginGuid);

        public IEnumerator Start() {
            Debug.Log("MultiMax Rework - Initializing...");
            ConfigHandler.InitializeConfig();
            ConfigHandler.InitializeMaxPlayers();
            Debug.Log("MultiMax Rework - Patching...");
            PatchMethods();

            typeof(uiQuickPlayerCreate).GetField("guiQuickPlayerCreates", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new uiQuickPlayerCreate[GameFlowMC.gMaxPlayers]);
            uiQuickPlayerCreate.Default_Classes = new int[GameFlowMC.gMaxPlayers];
            Harmony.Patch(AccessTools.Method(typeof(uiCharacterCreateRoot), "Start", null, null), null, new HarmonyMethod(AccessTools.Method(typeof(Main), "AddMorePlayerSlotsInMenu", null, null)));
            Harmony.Patch(AccessTools.Method(typeof(ReInput.PlayerHelper), "GetPlayer", new Type[1] { typeof(int) }, null), new HarmonyMethod(AccessTools.Method(typeof(Main), "FixRewire", null, null)), null, null);
            Harmony.Patch(AccessTools.Method(typeof(uiPortraitHolderManager), "Create", new Type[1] { typeof(HexLand) }, null), null, new HarmonyMethod(AccessTools.Method(typeof(Main), "AddMorePlayersToUI", null, null)));
            Harmony.Patch(AccessTools.Method(typeof(uiPlayerMainHud), "Update", null, null), new HarmonyMethod(AccessTools.Method(typeof(Main), "PlaceUI", null, null)));
            Harmony.Patch(AccessTools.Method(typeof(uiHudScroller), "Init", null, null), new HarmonyMethod(AccessTools.Method(typeof(Main), "InitHUD", null, null)));
            Harmony.Patch(AccessTools.Method(typeof(Diorama), "_resetTargetQueue", null, null), new HarmonyMethod(AccessTools.Method(typeof(Main), "DummySlide", null, null)), null, null);

            while (FTKHub.Instance == null) {
                yield return null;
            }
            DummiesHandler.CreateDummies();
            Debug.Log("MultiMax Rework - Done");
        }

        private void PatchMethods() {
            PatchMethod<uiGoldMenu>("Awake", typeof(uiGoldMenuPatches), "GoldAwake", null);
            PatchMethod<uiPopupMenu>("Awake", typeof(uiPopupMenuPatches), "PopupAwake", null);
            PatchMethod<EncounterSession>("GiveOutLootXPGold", typeof(EncounterSessionPatches), "XPModifierPatch", null);
        }

        private void PatchMethod<T>(string originalMethodName, Type patchClass, string patchMethodName, Type[] parameterTypes = null) {
            MethodInfo original = AccessTools.Method(typeof(T), originalMethodName, parameterTypes);
            MethodInfo patch = AccessTools.Method(patchClass, patchMethodName, null, null);
            Harmony.Patch(original, new HarmonyMethod(patch));
        }

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

        public static bool FixRewire(int playerId, ref Player __result) {
            if (playerId < ReInput.players.playerCount) {
                return true;
            }
            __result = ReInput.players.GetPlayer(2);
            return false;
        }

        public static void AddMorePlayerSlotsInMenu(ref uiCharacterCreateRoot __instance) {
            Debug.Log("[MULTIMAX REWORK] : " + __instance);
            Debug.Log("[MULTIMAX REWORK] : " + __instance.m_CreateUITargets);
            Debug.Log("[MULTIMAX REWORK] : " + SelectScreenCamera.Instance.m_PlayerTargets.Length);
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
            Debug.Log("[MULTIMAX REWORK] : SLOT COUNT " + __instance.m_CreateUITargets.Length);
        }

        public static void DummySlide() {
            DummyAttackSlide[] array = UnityEngine.Object.FindObjectsOfType<DummyAttackSlide>();
            foreach (DummyAttackSlide dummyAttackSlide in array) {
                if (dummyAttackSlide.m_Distances.Length < 1000) {
                    float[] array2 = new float[1000];
                    Array.Copy(dummyAttackSlide.m_Distances, array2, dummyAttackSlide.m_Distances.Length);
                    dummyAttackSlide.m_Distances = array2;
                    Debug.Log(dummyAttackSlide.m_Distances);
                }
            }
        }

    }
}