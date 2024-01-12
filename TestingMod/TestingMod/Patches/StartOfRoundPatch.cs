using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Linq;
using TestingMod.TestingMod.Core;
using Unity.Netcode;
using UnityEngine;

namespace TestingMod.Patches
{
    internal class StartOfRoundPatch
    {
        static PlayerControllerB playerScript;

        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        static void LogOnce()
        {
            Plugin.log.LogInfo("once");
        }

        [HarmonyPatch(typeof(StartOfRound), "EndOfGame"), HarmonyPrefix]
        static void SetLivingPlayers()
        {
            StartOfRound.Instance.livingPlayers = -2;
            Plugin.log.LogInfo("Living players set to -2");
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty)), HarmonyPostfix]
        static void ChangeText(int playersDead, int bodiesInsured)
        {
            HUDManager.Instance.statsUIElements.penaltyAddition.text = $"FATASS";
            HUDManager.Instance.statsUIElements.penaltyTotal.text = $"PUSSY";
            Plugin.log.LogInfo("Stat info set");
        }
    }
}
