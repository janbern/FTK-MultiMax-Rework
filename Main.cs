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
        private const string pluginVersion = "1.0";

        private static Harmony Harmony { get; set; } = new Harmony(pluginGuid);

        public IEnumerator Start() {
            Debug.Log("MultiMax Rework - Initializing...");
            ConfigHandler.InitializeConfig();
            InitializeMaxPlayers();
            Debug.Log("MultiMax Rework - Patching...");
            PatchMethods();
            while (FTKHub.Instance == null) {
                yield return null;
            }
            CreateDummies();
            Debug.Log("MultiMax Rework - Done");
        }

        private void InitializeMaxPlayers() {
            if (ConfigHandler.MaxPlayersConfig != null) {
                GameFlowMC.gMaxPlayers = ConfigHandler.MaxPlayersConfig.Value;
                GameFlowMC.gMaxEnemies = GameFlowMC.gMaxPlayers;
                uiQuickPlayerCreate.Default_Classes = new int[GameFlowMC.gMaxPlayers];
            } else {
                Debug.LogError("maxPlayersConfig is not initialized!");
            }
        }

        private void PatchMethods() {
            PatchMethod<uiCharacterCreateRoot>("Start", typeof(uiCharacterCreateRootPatches) ,"AddMorePlayerSlotsInMenu", null);
            PatchMethod<ReInput.PlayerHelper>("GetPlayer", typeof(RewiredPlayerHelperPatches), "FixRewire", new Type[] { typeof(int) });
            PatchMethod<uiGoldMenu>("Awake", typeof(uiGoldMenuPatches), "GoldAwake", null);
        }

        private void PatchMethod<T>(string originalMethodName, Type patchClass, string patchMethodName, Type[] parameterTypes = null) {
            MethodInfo original = AccessTools.Method(typeof(T), originalMethodName, parameterTypes);
            MethodInfo patch = AccessTools.Method(patchClass, patchMethodName, null, null);
            Harmony.Patch(original, new HarmonyMethod(patch));
        }

        private void CreateDummies() {
            Debug.Log("MAKING DUMMIES");
            List<GameObject> dummies = new List<GameObject>();

            for (int j = 0; j < Mathf.Max(3, GameFlowMC.gMaxPlayers); j++) {
                dummies.Add(CreateDummy(FTKHub.Instance.m_Dummies, j, "Player"));
            }

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxEnemies); i++) {
                dummies.Add(CreateDummy(FTKHub.Instance.m_Dummies, i + 3, "Enemy"));
            }

            FTKHub.Instance.m_Dummies = dummies.ToArray();

            foreach (GameObject go in FTKHub.Instance.m_Dummies) {
                Debug.Log("DUMMY");
                Debug.Log(go.name);
            }
        }

        private GameObject CreateDummy(GameObject[] source, int index, string prefix) {
            GameObject dummy;
            if (index < 3) {
                dummy = source[index];
            } else {
                dummy = UnityEngine.Object.Instantiate(source[2], source[2].transform.parent);
                dummy.name = $"{prefix} {index + 1} Dummy";
                dummy.GetComponent<PhotonView>().viewID = 3245 + index;
            }
            return dummy;
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

        public static bool CreateRoom(ref GameLogic __instance, string _roomName, bool _isOpen) {
            PhotonNetwork.offlineMode = false;
            RoomOptions roomOptions = new RoomOptions();
            TypedLobby typedLobby = new TypedLobby();
            roomOptions.IsOpen = _isOpen;
            roomOptions.IsVisible = _isOpen;
            if (__instance.m_GameMode == GameLogic.GameMode.SinglePlayer) {
                roomOptions.MaxPlayers = 1;
            } else {
                roomOptions.MaxPlayers = (byte)GameFlowMC.gMaxPlayers;
            }
            typedLobby.Type = LobbyType.Default;
            PhotonNetwork.CreateRoom(_roomName, roomOptions, typedLobby);
            return false;
        }

        public static void AddMorePlayersToUI(ref uiPortraitHolder __result) {
            for (int i = 2; i < GameFlowMC.gMaxPlayers; i++) {
                __result.m_PortraitActionPoints.Add(UnityEngine.Object.Instantiate(__result.m_PortraitActionPoints[__result.m_PortraitActionPoints.Count - 1], __result.m_PortraitActionPoints[__result.m_PortraitActionPoints.Count - 1].transform.parent));
            }
        }
    }
}