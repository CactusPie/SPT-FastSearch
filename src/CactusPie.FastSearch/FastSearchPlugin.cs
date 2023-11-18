using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace CactusPie.FastSearch
{
    [BepInPlugin("com.cactuspie.fastsearch", "CactusPie.FastSearch", "1.1.0")]
    public class FastSearchPlugin : BaseUnityPlugin
    {
        internal static ConfigEntry<float> SearchTimeMultiplier { get; private set; }
        internal static ConfigEntry<bool> SearchInitialDelayEnabled { get; private set; }
        
        internal static ManualLogSource SearchTimePluginLogger { get; private set; }
        
        [UsedImplicitly]
        internal void Start()
        {
            SearchTimePluginLogger = Logger;
            Logger.LogInfo("Search time reduction");

            const string sectionName = "Search time settings";
            
            SearchTimeMultiplier = Config.Bind
            (
                sectionName,
                "Search time multiplier",
                0f,
                new ConfigDescription
                (
                    "The time between revealing each item", 
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
            
            SearchInitialDelayEnabled = Config.Bind
            (
                sectionName,
                "Search initial delay",
                false,
                new ConfigDescription
                (
                    "Enable or disable the time it takes to start searching after opening a container"
                )
            );
            
            new SearchTimePatch().Enable();
            new SearchInitialDelayPatch().Enable();
        }
    }
}
