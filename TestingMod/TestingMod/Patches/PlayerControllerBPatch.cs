using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TestingMod.TestingMod.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestingMod.Patches
{
    internal class PlayerControllerBPatch
    {
        static Vector3 savedPosition;
        static List<GameObject> explosionObjects = new List<GameObject>();
        static bool f5Pressed = false;
        static NetworkBehaviour playerScript;

        [HarmonyPatch(typeof(PlayerControllerB), "SwitchToItemSlot")]
        [HarmonyTranspiler]
        // Removes annoying "HANDS FULL" overlay on the inventory when holding two handed items
        static IEnumerable<CodeInstruction> NoHandsFullMessageInInventory(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.DeclaredPropertyGetter(typeof(HUDManager), nameof(HUDManager.Instance))),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(HUDManager), nameof(HUDManager.holdingTwoHandedItem))),
                new CodeMatch(OpCodes.Ldc_I4_1))
                .RemoveInstructions(4)
                .InstructionEnumeration();
        }

/*        [HarmonyPatch(typeof(PlayerControllerB), "Update"), HarmonyPostfix]
        static void Input(PlayerControllerB __instance)
        {
            f5Pressed = Keyboard.current.f5Key.rea;


            if(Keyboard.current.f8Key.wasPressedThisFrame)
            {
                if(savedPosition != null)
                {
                    Landmine.SpawnExplosion(savedPosition, true);
                }
            }

            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                TagComponent[] tags = Object.FindObjectsOfType<TagComponent>();
                explosionObjects = tags.Select(tag => tag.gameObject).ToList();
                tags.Do(tag => Plugin.log.LogInfo(tag.gameObject.name + ", " + tag.gameObject.transform.position + ", " + tag.timeCreated));
            }

            if(Keyboard.current.f6Key.wasPressedThisFrame)
            {
                savedPosition = __instance.serverPlayerPosition;
            }

            if(!Keyboard.current.f5Key.pre)
            {
                if (Keyboard.current.f5Key.wasPressedThisFrame)
                {
                    *//*foreach (var explosion in explosionObjects)
                    {
                        Object.Destroy(explosion);
                    }*//*

                    Plugin.log.LogInfo("f5 pressed");
                    PrintPlayer();
                }
            }

        }*/

        static void PrintEnemies()
        {
            GameObject enemyPrefab = StartOfRound.Instance.currentLevel.Enemies.Where(enemy => enemy.enemyType.name == "Flowerman").First().enemyType.enemyPrefab;
            GameObject gameObject = Object.Instantiate(enemyPrefab, savedPosition, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);
            RoundManager.Instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
            if (!gameObject.TryGetComponent(out MeshFilter meshFilter))
            {
                Plugin.log.LogInfo("No MeshFilter found");
                return;
            }

            Plugin.log.LogInfo("meshFilter: " + meshFilter.GetType().Name);
        }

        static void PrintPlayer()
        {
            Object[] objs = playerScript.GetComponents<Object>();
            objs.Do(obj => Plugin.log.LogInfo(obj.GetType().Name));
        }
    }
}
