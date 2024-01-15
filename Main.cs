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
            ConfigHandler.InitializeMaxPlayers();
            Debug.Log("MultiMax Rework - Patching...");
            PatchMethods();
            while (FTKHub.Instance == null) {
                yield return null;
            }
            DummiesHandler.CreateDummies();
            Debug.Log("MultiMax Rework - Done");
        }

        private void PatchMethods() {
            PatchMethod<uiCharacterCreateRoot>("Start", typeof(uiCharacterCreateRootPatches) ,"AddMorePlayerSlotsInMenu", null);
            PatchMethod<ReInput.PlayerHelper>("GetPlayer", typeof(RewiredPlayerPatches), "FixRewire", new Type[] { typeof(int) });
            PatchMethod<uiGoldMenu>("Awake", typeof(uiGoldMenuPatches), "GoldAwake", null);
            PatchMethod<uiPopupMenu>("Awake", typeof(uiPopupMenuPatches), "PopupAwake", null);
            PatchMethod<EncounterSession>("GiveOutLootXPGold", typeof(EncounterSessionPatches), "XPModifierPatch", null);
        }

        private void PatchMethod<T>(string originalMethodName, Type patchClass, string patchMethodName, Type[] parameterTypes = null) {
            MethodInfo original = AccessTools.Method(typeof(T), originalMethodName, parameterTypes);
            MethodInfo patch = AccessTools.Method(patchClass, patchMethodName, null, null);
            Harmony.Patch(original, new HarmonyMethod(patch));
        }

    }
}