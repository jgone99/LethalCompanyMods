using GameNetcodeStuff;
using HarmonyLib;

namespace DebugCheat.Patches
{
    internal class DebugCheatPatches
    {
        /*** PlayerControllerB Patches ***/

        [HarmonyPatch(typeof(PlayerControllerB), "Update"), HarmonyPostfix]
        static void InfiniteStaminaPatch(ref float ___sprintMeter, ref bool ___takingFallDamage)
        {
            ___sprintMeter = 1f;
            ___takingFallDamage = false;
        }


        /*** Terminal Patches ***/

        [HarmonyPatch(typeof(Terminal), "Start"), HarmonyPostfix]
        static void MoreStartingCredits(ref int ___groupCredits)
        {
            ___groupCredits = 1000;
        }
    }
}
