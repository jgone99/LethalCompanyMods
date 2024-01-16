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
        static List<Tuple<int, Vector3, Vector3, Vector3, Vector3>> shipItems = new List<Tuple<int, Vector3, Vector3, Vector3, Vector3>> ();

        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        static void GenerateConfigFile()
        {
            Plugin.config.SetupConfig(StartOfRound.Instance.unlockablesList.unlockables.ToArray());
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
                Plugin.log.LogInfo($"{unlockables[i].unlockableName} - Type({unlockables[i].unlockableType}) - placeable({unlockables[i].IsPlaceable})" +
                    $" - playerUnlocked({unlockables[i].hasBeenUnlockedByPlayer}) - Save({Plugin.config.configEntries[i].Value})");
                if (Plugin.config.configEntries[i].Value)
                {
                    if (unlockables[i].hasBeenUnlockedByPlayer || unlockables[i].alreadyUnlocked)
                    {
                        Vector3 parentRot = Vector3.zero;
                        Vector3 parentPos = Vector3.zero;

                        if (unlockables[i].unlockableType == 1 && unlockables[i].IsPlaceable)
                        {
                            PlaceableShipObject objectToSave;
                            if (unlockables[i].alreadyUnlocked)
                            {
                                Plugin.log.LogInfo("Already unlocked");
                                objectToSave = UnityEngine.Object.FindObjectsOfType<PlaceableShipObject>().FirstOrDefault(item => item.unlockableID == i);
                                Plugin.log.LogInfo($"Count: {UnityEngine.Object.FindObjectsOfType<PlaceableShipObject>().Where(item => item.unlockableID == i).Count()}");
                                StartOfRound.Instance.SpawnedShipUnlockables.TryAdd(i, objectToSave.parentObject.gameObject);
                            }
                            else
                            {
                                Plugin.log.LogInfo("Player unlocked");
                                Plugin.log.LogInfo(unlockables[i].prefabObject.name);
                                objectToSave = StartOfRound.Instance.SpawnedShipUnlockables.GetValueOrDefault(i).GetComponentInChildren<PlaceableShipObject>();
                            }

                            if (objectToSave.parentObjectSecondary != null)
                            {
                                parentRot = objectToSave.parentObjectSecondary.transform.eulerAngles;
                                parentPos = objectToSave.parentObjectSecondary.position;
                            }
                            else if (objectToSave.parentObject != null)
                            {
                                parentRot = objectToSave.parentObject.rotationOffset;
                                parentPos = objectToSave.parentObject.positionOffset;
                            }
                        }

                        Plugin.log.LogInfo($"{unlockables[i].unlockableName} saved");
                        shipItems.Add(Tuple.Create(i, unlockables[i].placedPosition, unlockables[i].placedRotation, parentRot, parentPos));
                    }
                }
            }
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

/*            StartOfRound.Instance.closetLeftDoor.TriggerAnimationNonPlayer();
            StartOfRound.Instance.closetRightDoor.TriggerAnimationNonPlayer();*/

            UnlockableItem[] unlockables = StartOfRound.Instance.unlockablesList.unlockables.ToArray();
            foreach (Tuple<int, Vector3, Vector3, Vector3, Vector3> item in shipItems)
            {
                Plugin.log.LogInfo($"Next item: {unlockables[item.Item1].unlockableName}");

                if (unlockables[item.Item1].IsPlaceable)
                {   
                    if (unlockables[item.Item1].alreadyUnlocked)
                    {
                        Plugin.log.LogInfo("Already unlocked");
                        PlaceableShipObject objectToRestore = StartOfRound.Instance.SpawnedShipUnlockables.GetValueOrDefault(item.Item1).GetComponentInChildren<PlaceableShipObject>();
                        

                        if (objectToRestore.parentObjectSecondary != null)
                        {
                            objectToRestore.parentObjectSecondary.transform.eulerAngles = item.Item4;
                            objectToRestore.parentObjectSecondary.position = item.Item5;
                        }
                        else if (objectToRestore.parentObject != null)
                        {
                            objectToRestore.parentObject.rotationOffset = item.Item4;
                            objectToRestore.parentObject.positionOffset = item.Item5;
                        }
                    }
                    else
                    {
                        Plugin.log.LogInfo("Player unlocked");
                        Plugin.log.LogInfo($"Buying: {unlockables[item.Item1].unlockableName}");
                        StartOfRound.Instance.BuyShipUnlockableServerRpc(item.Item1, TimeOfDay.Instance.quotaVariables.startingCredits);
                    }
                    
                    if (unlockables[item.Item1].unlockableType == 1)
                    {
                        Plugin.log.LogInfo($"Placing: {unlockables[item.Item1].unlockableName}");
                        GameObject placingObject = StartOfRound.Instance.SpawnedShipUnlockables.GetValueOrDefault(item.Item1);
                        Plugin.log.LogInfo($"IsSpawned: {placingObject.GetComponent<NetworkObject>().IsSpawned}");
                        ShipBuildModeManager.Instance.PlaceShipObject(item.Item2, item.Item3, placingObject.GetComponentInChildren<PlaceableShipObject>(), placementSFX: false);
                        ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(item.Item2, item.Item3, placingObject.GetComponent<NetworkObject>(), (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                    }
                }
                else
                {
                    Plugin.log.LogInfo($"Buying: {unlockables[item.Item1].unlockableName}");
                    StartOfRound.Instance.BuyShipUnlockableServerRpc(item.Item1, TimeOfDay.Instance.quotaVariables.startingCredits);
                }
            }
        }
    }
}
