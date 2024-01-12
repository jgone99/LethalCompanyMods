using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestingMod.Patches
{
    internal class RoundManagerPatch
    {
        static RoundManager instance;

        [HarmonyPatch(typeof(RoundManager), "Start"), HarmonyPostfix]
        static void SaveInstance(RoundManager __instance)
        {
            instance = __instance;
        }
    }
}
