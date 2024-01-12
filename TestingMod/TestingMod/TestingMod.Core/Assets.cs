using System.Reflection;
using UnityEngine;

namespace TestingMod.Core
{
    using TestingMod.Core;
    public static class Assets
    {
        public static AssetBundle MainAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets(string mainAssetBundleName)
        {
            if (MainAssetBundle == null)
            {
                Plugin.log.LogInfo(GetAssemblyName() + "." + mainAssetBundleName);
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + ".Resources." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }
}
