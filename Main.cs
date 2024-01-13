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

        private ConfigEntry<int> maxPlayersConfig;

        void Awake() {
            GenerateConfig();
        }

        private void GenerateConfig() {
            string configFilePath = Path.Combine(Paths.ConfigPath, "MultiMaxRework.cfg");

            var configFile = new ConfigFile(configFilePath, true);

            maxPlayersConfig = configFile.Bind("General",
                                 "MaxPlayers",
                                 5,
                                 "The max number of players");

            if (!File.Exists(configFilePath)) {
                configFile.Save();
            }
        }

        public IEnumerator Start() {
            InitializeMaxPlayers();
            Debug.Log("MultiMax Rework - Initializing...");
            PatchMethods();
            while (FTKHub.Instance == null) {
                yield return null;
            }
            CreateDummies();
            Debug.Log("MultiMax Rework - Done");
        }

        private void InitializeMaxPlayers() {
            if (maxPlayersConfig != null) {
                GameFlowMC.gMaxPlayers = maxPlayersConfig.Value;
                GameFlowMC.gMaxEnemies = GameFlowMC.gMaxPlayers;
                uiQuickPlayerCreate.Default_Classes = new int[GameFlowMC.gMaxPlayers];
            } else {
                Debug.LogError("maxPlayersConfig is not initialized!");
            }
        }

        private void PatchMethods() {
            PatchMethod<uiCharacterCreateRoot>("Start", "AddMorePlayerSlotsInMenu", null);
            PatchMethod<ReInput.PlayerHelper>("GetPlayer", "FuckRewire", new Type[] { typeof(int) });
            //PatchMethod<uiGoldMenu>("Show", "GoldFix", null);
            PatchMethod<uiGoldMenu>("Awake", "GoldAwake", null);
        }

        private void PatchMethod<T>(string originalMethodName, string patchMethodName, Type[] parameterTypes = null) {
            MethodInfo original = AccessTools.Method(typeof(T), originalMethodName, parameterTypes);
            MethodInfo patch = AccessTools.Method(typeof(Main), patchMethodName, null, null);
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

        public static bool FuckRewire(int playerId, ref Player __result) {
            if (playerId < ReInput.players.playerCount) {
                return true;
            }
            __result = ReInput.players.GetPlayer(2);
            return false;
        }

        public static bool GoldAwake(uiGoldMenu __instance) {
            var m_GoldEntriesField = Traverse.Create(__instance).Field("m_GoldEntries");

            if (m_GoldEntriesField.GetValue() != null) {
                int maxEntries = (GameFlowMC.gMaxPlayers - 1);

                m_GoldEntriesField.GetValue<List<uiGoldMenuEntry>>().Add(__instance.m_FirstEntry);

                for (int i = 1; i < maxEntries; i++) {
                    uiGoldMenuEntry newEntry = UnityEngine.Object.Instantiate(__instance.m_FirstEntry);
                    newEntry.transform.SetParent(__instance.m_FirstEntry.transform.parent, worldPositionStays: false);
                    m_GoldEntriesField.GetValue<List<uiGoldMenuEntry>>().Add(newEntry);
                    Debug.LogWarning(newEntry.name);
                }
            }

            return false;
        }

        //public static void GoldFix(ref uiGoldMenu __instance, Vector2 _spos, CharacterOverworld _cow) {
        //    int maxPlayers = GameFlowMC.gMaxPlayers;
        //    if (maxPlayers == 3) {
        //        return;
        //    }

        //    var value = new CharacterOverworld[4];

        //    Traverse.Create(__instance).Field("m_OtherCows").SetValue(value);

        //    CharacterOverworld[] newArray = Traverse.Create(__instance).Field("m_OtherCows").GetValue() as CharacterOverworld[];
        //    if (newArray != null) {
        //        for (int i = 0; i < newArray.Length; i++) {
        //            Debug.LogWarning($"m_OtherCows[{i}] = {newArray[i]}");
        //        }
        //    } else {
        //        Debug.LogWarning("m_OtherCows is null");
        //    }
        //}

        //Debug.Log(Traverse.Create(__instance).Field("m_Cow").SetValue(_cow));

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