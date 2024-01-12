using HarmonyLib;
using TestingMod.TestingMod.Core;

namespace TestingMod.Patches
{
    internal class TerminalAccessibleObjectPatch
    {
        [HarmonyPatch(typeof(TerminalAccessibleObject), nameof(TerminalAccessibleObject.CallFunctionFromTerminal)), HarmonyPrefix]
        static void TurretDisabled()
        {
            Plugin.log.LogInfo("Turret disabled");
        }
    }
}
