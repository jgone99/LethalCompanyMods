using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModGUI
{
    internal class ModGUI : MonoBehaviour
    {
        static Vector2 areaSize = new Vector2(Screen.width * .4f, Screen.height * 0.9f);
        static Vector2 areaPosition = new Vector2(Screen.width * 0.05f, Screen.height * 0.05f);
        static string patchString = "Patch";
        static string unpatchString = "Unpatch";
        static GUILayoutOption[] buttonOptions = new GUILayoutOption[] { GUILayout.Width(areaSize.x * 0.25f), GUILayout.Height(25) };
        static GUILayoutOption[] labelOptions = new GUILayoutOption[] { GUILayout.Width(areaSize.x * 0.25f), GUILayout.Height(50) };

        static List<Harmony> harmonyList = new List<Harmony>();

        void Awake()
        {
            enabled = false;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(areaPosition, areaSize), UnityEngine.GUI.skin.box);

            GameControlButtons();

            // Patch and Unpatch all mods
            OtherModsPatchButtons();

            GUILayout.EndArea();
        }

        string GetPatchButtonString(bool isPatched)
        {
            return isPatched ? unpatchString : patchString;
        }

        void PatchOrUnpatch(ModInfo modInfo)
        {
            if (modInfo.patched)
            {
                ModUnpatchAll(modInfo);
            }
            else
            {
                ModPatchAll(modInfo);
            }
            modInfo.patched = !modInfo.patched;
        }

        void ModPatchAll(ModInfo modInfo)
        {
            Harmony harmony = new Harmony(modInfo.modGUID);
            harmonyList.Add(harmony);
            Plugin.log.LogInfo(harmonyList.Count());
            modInfo.patchTypes.Do(patchType => harmony.PatchAll(patchType));
        }

        void ModUnpatchAll(ModInfo modInfo)
        {
            Harmony.UnpatchID(modInfo.modGUID);
            harmonyList.Remove(harmonyList.FirstOrDefault(harmony => harmony.Id == modInfo.modGUID));
        }

        void GameControlButtons()
        {
            if (GUILayout.Button("Fire Sequence", buttonOptions))
            {
                StartOfRound.Instance.ManuallyEjectPlayersServerRpc();
            }

            if (GUILayout.Button("Start Round", buttonOptions))
            {
                FindAnyObjectByType<StartMatchLever>().StartGame();
            }

            if (GUILayout.Button("End Round", buttonOptions))
            {
                FindAnyObjectByType<StartMatchLever>().EndGame();
            }
        }

        void OtherModsPatchButtons()
        {
            Plugin.mods.ForEach(modInfo =>
            {
                GUILayout.Label(modInfo.name, labelOptions);

                if (GUILayout.Button(GetPatchButtonString(modInfo.patched), buttonOptions))
                {
                    PatchOrUnpatch(modInfo);
                }
            });
        }
    }
}
