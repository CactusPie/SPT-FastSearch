using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CactusPie.FastSearch.Patches;

namespace CactusPie.FastSearch
{
    [BepInPlugin("com.cactuspie.fastsearch", "CactusPie.FastSearch", "1.1.0")]
    [BepInDependency("com.spt-aki.core", "3.5.0")]
    public class FastSearchPlugin : BaseUnityPlugin
    {
        internal static ConfigEntry<float> SearchTimeMultiplier { get; private set; }
        internal static ConfigEntry<bool> SearchInitialDelayEnabled { get; private set; }
        
        internal static ManualLogSource SearchTimePluginLogger { get; private set; }
        
        internal void Start()
        {
            SearchTimePluginLogger = Logger;
            Logger.LogInfo("Search time reduction");

            const string sectionName = "Settings";
            
            SearchTimeMultiplier = Config.Bind
            (
                sectionName,
                "Multiplier",
                0f,
                new ConfigDescription
                (
                    "The time between revealing each item.", 
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
            
            SearchInitialDelayEnabled = Config.Bind
            (
                sectionName,
                "Initial delay",
                false,
                new ConfigDescription
                (
                    "Enable/disable the time to start searching after opening a container."
                )
            );
            
            new SearchTimePatch().Enable();
            new SearchInitialDelayPatch().Enable();
        }
    }
}
