using HarmonyLib;
using TestingMod.TestingMod.Core;

namespace TestingMod.Patches
{
    internal class LandMinePatch
    {
        [HarmonyPatch(typeof(Landmine), nameof(Landmine.SpawnExplosion)), HarmonyPrefix]
        static void TagExplosionPrefab()
        {
            Plugin.log.LogInfo("Begin Tagging Prefab");
            StartOfRound.Instance.explosionPrefab.AddComponent<TagComponent>().stringTag = "explosionObject";
            Plugin.log.LogInfo("End Tagging Prefab");
        }
    }
}
