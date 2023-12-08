using System.Reflection;
using Aki.Reflection.Patching;

namespace CactusPie.FastSearch
{
    public class AutoUncoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(SearchableItemClass).GetMethod(nameof(SearchableItemClass.GetSearchState), BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static void PatchPrefix(string profileId, SearchableItemClass __instance)
        {
            if (FastSearchPlugin.InstantlyRevealEverything.Value)
            {
                __instance.UncoverAll(profileId);
            }
        }
    }
}
