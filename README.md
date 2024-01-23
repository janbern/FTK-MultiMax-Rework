# FTK-MultiMax-Rework

This mod is a rework of "For The King Multi Max" by [samupo](https://www.nexusmods.com/fortheking/users/121409763).

The concept remains the same, having more than just 3 players per game.
All my tests were made with 5 players but you can try yourself with more it shouldn't be a problem.

**What has changed:**

- Better configuration file, now under: BepInEx\config\MultiMaxRework.cfg (This is were you set the max amount of players)
- Better game patching
- Gold and Item patches in order to be able to give items or gold to all the players that wasn't possible with the other mod (distance to the players rule still aplies)
- Added a slight exp and gold modifier after each combat if there are more than 3 players to compensate

**Note:**

- **IMPORTANT:** Set the exact number of players in the config so that not only the UI corresponds to the number of players but the extra buttons match too! (So if you play as 4 do not leave 5 in the config but set 4, same thing applies if you play with 3).

**How to install:**

1. Download and install [BepInEx](https://for-the-king.thunderstore.io/package/BepInEx/BepInExPack_ForTheKing/) by following their guide.
2. Download FTK MultiMax Rework .dll file from the [Releases](https://github.com/justedm/FTK-MultiMax-Rework/releases/tag/v1.4) and put it inside of the BepInEx plugins folder (or download from [Thunderstore](https://for-the-king.thunderstore.io/package/edm/FTKMultiMaxRework/)).
3. Launch the game!

**N.B:** The config file is generated after the first launch so if u need to set the value of the maxPlayers you can do so after (game needs to be restarted) otherwise you can download the config file and insert it manually in the config folder of BepInEx so you dont need to restart the game after the first launch.

If you encounter any errors open an issue.
Have fun, edm.
