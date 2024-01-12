using ModGUI.GUI;
using GameNetcodeStuff;
using HarmonyLib;

namespace ModGUI.Patches
{
    internal class ModGUIPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "__initializeVariables"), HarmonyPostfix]
        static void AttachGUIScript(PlayerControllerB __instance)
        {
            if(__instance.isHostPlayerObject)
            {
                GUIController guiController = __instance.gameObject.AddComponent<GUIController>();
                guiController.guiScript = __instance.gameObject.AddComponent<ModGUI>();
            }
        }
    }
}
