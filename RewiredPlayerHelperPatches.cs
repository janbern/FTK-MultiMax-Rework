using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTK_MultiMax_Rework {
    public class RewiredPlayerHelperPatches {
        public static bool FixRewire(int playerId, ref Player __result) {
            if (playerId < ReInput.players.playerCount) {
                return true;
            }
            __result = ReInput.players.GetPlayer(2);
            return false;
        }
    }
}
