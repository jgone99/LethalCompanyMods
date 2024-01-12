using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


namespace KeepFurnatureLayout.Patches
{
    internal class KeepFurnatureLayoutPatches
    {
        static List<Tuple<int, Vector3, Vector3>> shipItems = new List<Tuple<int, Vector3, Vector3>> ();
        static Tuple<int, Vector3, Vector3> cupboardInfo;
        static GameObject cupboard;
        static Vector3 parent2Pos;
        static Vector3 parent2Rot;
        static Vector3 parentPos;
        static Vector3 parentRot;

        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        static void GenerateConfigFile()
        {
            Plugin.config.SetupConfig(StartOfRound.Instance.unlockablesList.unlockables.ToArray());
            cupboard = GameObject.Find("StorageCloset");
        }

        [HarmonyPatch(typeof(StartOfRound), "playersFiredGameOver"), HarmonyPrefix]
        public static void storeUnlocked()
        {
            Plugin.log.LogInfo("Taking note of bought unlockables");
            shipItems.Clear ();
            List<UnlockableItem> unlockables = StartOfRound.Instance.unlockablesList.unlockables;
            Plugin.log.LogInfo(unlockables.Count);
            for (int i = 0; i < unlockables.Count; i++)
            {
                Plugin.log.LogInfo($"{unlockables[i].unlockableName} - unlocked({unlockables[i].hasBeenUnlockedByPlayer}) - Save({Plugin.config.configEntries[i].Value})");
                if (unlockables[i].hasBeenUnlockedByPlayer && Plugin.config.configEntries[i].Value)
                {
                    shipItems.Add(Tuple.Create(i, unlockables[i].placedPosition, unlockables[i].placedRotation));
                }
            }

            PlaceableShipObject placeableShipObject = cupboard.GetComponentInChildren<PlaceableShipObject>();

            if (placeableShipObject.parentObjectSecondary != null)
            {
                parent2Rot = placeableShipObject.parentObjectSecondary.transform.eulerAngles;
                parent2Pos = placeableShipObject.parentObjectSecondary.position;
            }
            else if (placeableShipObject.parentObject != null)
            {
                parentRot = placeableShipObject.parentObject.rotationOffset;
                parentPos = placeableShipObject.parentObject.positionOffset;
            }

            int cupboardID = cupboard.GetComponentInChildren<PlaceableShipObject>().unlockableID;
            Plugin.log.LogInfo($"{unlockables[cupboardID].unlockableName} - Save({Plugin.config.configEntries[cupboardID].Value})");
            cupboardInfo = Tuple.Create(cupboardID, unlockables[cupboardID].placedPosition, unlockables[cupboardID].placedRotation);
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ResetShip)), HarmonyPostfix]
        public static void loadUnlocked()
        {
            Plugin.log.LogInfo("Rebuying unlockables");
            if(shipItems is null)
            {
                Plugin.log.LogInfo("state is null");
                return;
            }

            UnlockableItem[] unlockables = StartOfRound.Instance.unlockablesList.unlockables.ToArray();
            foreach (Tuple<int, Vector3, Vector3> item in shipItems)
            {
                StartOfRound.Instance.BuyShipUnlockableServerRpc(item.Item1, TimeOfDay.Instance.quotaVariables.startingCredits);
                if (unlockables[item.Item1].unlockableType == 1)
                {
                    Plugin.log.LogInfo($"Placing: {unlockables[item.Item1].unlockableName}");
                    GameObject placingObject = StartOfRound.Instance.SpawnedShipUnlockables.GetValueOrDefault(item.Item1);
                    ShipBuildModeManager.Instance.PlaceShipObject(item.Item2, item.Item3, placingObject.GetComponentInChildren<PlaceableShipObject>(), placementSFX: false);
                    ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(item.Item2, item.Item3, placingObject.GetComponent<NetworkObject>(), (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                }
                else
                {
                    Plugin.log.LogInfo("Item is suit; not placing");
                }
            }

            PlaceableShipObject placeableShipObject = cupboard.GetComponentInChildren<PlaceableShipObject>();

            if (placeableShipObject.parentObjectSecondary != null)
            {
                placeableShipObject.parentObjectSecondary.transform.eulerAngles = parent2Rot;
                placeableShipObject.parentObjectSecondary.position = parent2Pos;
            }
            else if (placeableShipObject.parentObject != null)
            {
                placeableShipObject.parentObject.rotationOffset = parentRot;
                placeableShipObject.parentObject.positionOffset = parentPos;
            }

            Plugin.log.LogInfo($"Placing: {unlockables[cupboardInfo.Item1].unlockableName}");
            ShipBuildModeManager.Instance.PlaceShipObject(cupboardInfo.Item2, cupboardInfo.Item3, cupboard.GetComponentInChildren<PlaceableShipObject>(), placementSFX: false);
            ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(cupboardInfo.Item2, cupboardInfo.Item3, cupboard.GetComponent<NetworkObject>(), (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
            Plugin.log.LogInfo($"Count: {UnityEngine.Object.FindObjectsOfType<PlaceableShipObject>().Where(item => item.unlockableID == cupboardInfo.Item1).Count()}");
        }
    }
}
