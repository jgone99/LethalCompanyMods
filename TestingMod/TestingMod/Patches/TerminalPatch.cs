using HarmonyLib;
using TestingMod.TestingMod.Core;
using UnityEngine;

namespace TestingMod.Patches
{
    internal class TerminalPatch
    {
        [HarmonyPatch(typeof(Terminal), nameof(Terminal.BeginUsingTerminal)), HarmonyPrefix]
        static void TerminalOpen(Terminal __instance)
        {
            Plugin.log.LogInfo("Terminal Open");
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.QuitTerminal)), HarmonyPostfix]
        static void TerminalClose()
        {
            Plugin.log.LogInfo("Terminal Close");
        }

        [HarmonyPatch(typeof(Terminal), "Awake"), HarmonyPrefix]
        static void TerminalAwake()
        {
            Plugin.log.LogInfo("Terminal Awake");
        }
    }
}
