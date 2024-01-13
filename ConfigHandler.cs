using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FTK_MultiMax_Rework {
    public static class ConfigHandler {
        public static ConfigEntry<int> MaxPlayersConfig { get; private set; }

        public static void InitializeConfig() {
            string configFilePath = Path.Combine(Paths.ConfigPath, "MultiMaxRework.cfg");
            var configFile = new ConfigFile(configFilePath, true);

            MaxPlayersConfig = configFile.Bind("General",
                                               "MaxPlayers",
                                               5,
                                               "The max number of players");

            if (!File.Exists(configFilePath)) {
                configFile.Save();
            }
        }
    }
}
