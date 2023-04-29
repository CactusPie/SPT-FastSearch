using System;
using System.Reflection;
using Aki.Reflection.Patching;
using Comfort.Common;
using EFT.InventoryLogic;

namespace CactusPie.FastSearch
{
    public class SearchInitialDelayPatch : ModulePatch
    {
        private static readonly FieldInfo InventoryControllerField = typeof(GClass2517).GetField("gclass2417_0", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo SearchableItemField = typeof(GClass2517).GetField("gclass2338_0", BindingFlags.Instance | BindingFlags.NonPublic);
        
        protected override MethodBase GetTargetMethod()
        {
            MethodInfo method = typeof(GClass2517).GetMethod("method_0", BindingFlags.NonPublic | BindingFlags.Instance);
            
            return method;
        }

        [PatchPrefix]
        public static void PatchPrefix(Callback callback, bool isInstant, GClass2517 __instance)
        {
            if (FastSearchPlugin.SearchInitialDelayEnabled.Value)
            {
                return;
            }

            var inventoryController = (InventoryControllerClass)InventoryControllerField.GetValue(__instance);
            var searchableItem = (SearchableItemClass)SearchableItemField.GetValue(__instance);

            if (__instance.Terminated)
            {
                return;
            }

            if (searchableItem.HasUnknownItems(inventoryController.ID))
            {
                searchableItem.SetStatus(SearchedState.Searched, inventoryController.ID);
            }
            else
            {
                searchableItem.SetStatus(SearchedState.FullySearched, inventoryController.ID);
            }
        }
    }
}