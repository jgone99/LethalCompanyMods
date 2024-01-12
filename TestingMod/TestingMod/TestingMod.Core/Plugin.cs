using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TestingMod.Patches;

namespace TestingMod.TestingMod.Core
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Zeus.TestingMod";
        private const string modName = "Testing Mod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
            }

            harmony.PatchAll(typeof(Plugin));

            //harmony.PatchAll(typeof(TerminalPatch));
            //harmony.PatchAll(typeof(LandMinePatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(TerminalAccessibleObjectPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
        }
    }
}
