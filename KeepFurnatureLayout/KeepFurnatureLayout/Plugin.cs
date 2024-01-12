using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using KeepFurnatureLayout.Patches;
using System.Collections.Generic;

namespace KeepFurnatureLayout
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "KeepLayoutMod";
        private const string modName = "Keep Layout Mod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        public static Config config;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            config = new Config(base.Config);

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(KeepFurnatureLayoutPatches));
            harmony.PatchAll(typeof(TestingPatch));
        }
    }

    public class Config
    {
        ConfigFile configFile;
        public List<ConfigEntry<bool>> configEntries = new List<ConfigEntry<bool>>();

        public Config(ConfigFile cfg)
        {
            configFile = cfg;
        }

        public void SetupConfig(UnlockableItem[] unlockableItems)
        {
            Plugin.log.LogInfo("Setting up config file");
            unlockableItems.Do(item =>
            {
                configEntries.Add(
                    configFile.Bind<bool>(
                    "General",
                    item.unlockableName,
                    true,
                    $"Save {item.unlockableName} when fired." + (item.unlockableType == 1 ? "Position Saved" : string.Empty) 
                    )
                );
            });
        }
    }
}
