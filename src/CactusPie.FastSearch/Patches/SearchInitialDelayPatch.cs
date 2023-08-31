using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Comfort.Common;
using EFT.InventoryLogic;
using HarmonyLib;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace CactusPie.FastSearch.Patches
{
    public class SearchInitialDelayPatch : ModulePatch
    {
        // GClass2658 in 0.13.5.0.25793
        private static readonly Type GClass = PatchConstants.EftTypes.Single(x => x.GetMethod("GetNextDiscoveryTime") != null);
        // A list with every field declared in our GClass, needed because our desired Fields doesn't have names
        private static readonly IList GClassFields = AccessTools.GetDeclaredFields(GClass);
        // GClass's 2nd field, gclass2554_0 in 0.13.5.0.25793
        private static readonly FieldInfo inventoryControllerField = (FieldInfo)GClassFields[1]; 
        // GClass's 3rd field, gclass2474_0 in 0.13.5.0.25793
        private static readonly FieldInfo searchableItemField = (FieldInfo)GClassFields[2];

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(GClass, "method_0");
        }

        [PatchPrefix]
        public static void PatchPrefix(Callback callback, bool isInstant, object __instance)
        {
            if (FastSearchPlugin.SearchInitialDelayEnabled.Value)
            {
                return;
            }

            var inventoryController = (InventoryControllerClass)inventoryControllerField.GetValue(__instance);
            var searchableItem = (SearchableItemClass)searchableItemField.GetValue(__instance);

            searchableItem.SetStatus(
                searchableItem.HasUnknownItems(inventoryController.ID)
                    ? SearchedState.Searched
                    : SearchedState.FullySearched, inventoryController.ID);
        }
    }
}