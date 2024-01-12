using BepInEx;
using BepInEx.Logging;
using DebugCheat.Patches;
using HarmonyLib;

namespace DebugCheat
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Zeus.DebugCheatMod";
        private const string modName = "Debug Cheat";
        private const string modVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(DebugCheatPatches));
        }
    }
}
