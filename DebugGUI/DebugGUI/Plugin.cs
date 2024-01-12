using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModGUI.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ModGUI
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "DebugGUIMod";
        private const string modName = "Debug GUI Mod";
        private const string modVersion = "1.0.0";

        public readonly Harmony harmony = new Harmony(modGUID);
        public static Plugin Instance;
        public static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        public static List<ModInfo> mods = new List<ModInfo>();
        static string modsFolderPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Lethal Company\\BepInEx\\plugins";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(ModGUIPatches));

            GetModInfo();
        }

        void GetModInfo()
        {
            if (Directory.Exists(modsFolderPath))
            {
                DirectoryInfo modDir = new DirectoryInfo(modsFolderPath);
                modDir.EnumerateFiles()
                    .Where(file => Assembly.LoadFrom(file.FullName) != Assembly.GetExecutingAssembly())
                    .ToList()
                    .ForEach(file =>
                    {
                        string name = file.Name[..^4];
                        Type[] types = Assembly.LoadFrom(file.FullName).DefinedTypes
                        .Where(typeInfo => !typeInfo.IsDefined(typeof(CompilerGeneratedAttribute), true)).ToArray();
                        Type[] patchTypes = types.Where(t => t.FullName.StartsWith(name + ".Patches.")).ToArray();
                        string modGUID = (string)AccessTools.Field(types.Where(t => t.FullName.EndsWith("Plugin")).First(), "modGUID").GetValue(null);
                        mods.Add(new ModInfo(name, patchTypes, modGUID, Assembly.LoadFrom(file.FullName)));
                    });
            }
        }
    }

    public class ModInfo
    {
        public string name;
        public Type[] patchTypes;
        public string modGUID;
        public Assembly assembly;
        public bool patched;
        public ModInfo(string name, Type[] patchTypes, string modGUID, Assembly assembly, bool patched = true)
        {
            this.name = name;
            this.patchTypes = patchTypes;
            this.modGUID = modGUID;
            this.assembly = assembly;
            this.patched = patched;
        }

        override public string ToString()
        {
            return $"[name={name}, type={patchTypes}, modGUID={modGUID}, patched={patched}]";
        }
    }
}
