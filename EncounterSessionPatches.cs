using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace FTK_MultiMax_Rework {
    public static class EncounterSessionPatches {
        public static void XPModifierPatch(ref FTKPlayerID _recvPlayer, ref int _xp, ref int _gold) {

            CharacterOverworld characterOverworldByFid = FTKHub.Instance.GetCharacterOverworldByFID(_recvPlayer);

            float xpMod = characterOverworldByFid.m_CharacterStats.XpModifier;
            float goldMod = characterOverworldByFid.m_CharacterStats.GoldModifier;

            if (GameFlowMC.gMaxPlayers > 3) {
                _xp = Mathf.RoundToInt((float)((_xp * xpMod) * 1.5));
                _gold = Mathf.RoundToInt((float)((_gold * goldMod) * 1.5));
            }
        }
    }
}