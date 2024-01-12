using HarmonyLib;

namespace KeepFurnatureLayout.Patches
{
    internal class TestingPatch
    {
        static Terminal terminal;

        [HarmonyPatch(typeof(Terminal), "Start"), HarmonyPostfix]
        static void StartPatch(Terminal __instance)
        {
            terminal = __instance;
            GimmeMoney();
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ResetShip)), HarmonyPostfix]
        static void ResetShipPatch()
        {
            GimmeMoney();
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), nameof(ShipBuildModeManager.PlaceShipObject)), HarmonyPostfix]
        static void PrintItem(PlaceableShipObject placeableObject)
        {
            Plugin.log.LogInfo(placeableObject.parentObject.gameObject.name);
        }

        static void GimmeMoney()
        {
            Plugin.log.LogInfo("All the money");
            terminal.groupCredits = 999999;
        }
    }
}
