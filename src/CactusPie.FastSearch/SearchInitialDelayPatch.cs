using System;
using System.Reflection;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Comfort.Common;
using EFT.InventoryLogic;
using System.Linq;
using HarmonyLib;

namespace CactusPie.FastSearch
{
    public class SearchInitialDelayPatch : ModulePatch
    {
        private static Type _searchOperationClass;
        private static FieldInfo _inventoryControllerField;
        private static FieldInfo _searchableItemField;
        private static FieldInfo _terminatedField;

        protected override MethodBase GetTargetMethod()
        {
            _searchOperationClass = PatchConstants.EftTypes.Single(x => x.GetMethod("GetNextDiscoveryTime", BindingFlags.Static | BindingFlags.Public) != null);
            _terminatedField = _searchOperationClass.GetField("Terminated", BindingFlags.Instance | BindingFlags.Public);
            var searchOperationFields = _searchOperationClass.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            _inventoryControllerField = searchOperationFields.Single(x => x.FieldType == typeof(InventoryControllerClass));
            _searchableItemField = searchOperationFields.Single(x => x.FieldType == typeof(SearchableItemClass));

            return AccessTools.GetDeclaredMethods(_searchOperationClass).FirstOrDefault(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length >= 2
                && parameters[0].Name == "callback"
                && parameters[1].Name == "isInstant"
                && mi.ReturnType == typeof(System.Collections.IEnumerator);
        }

        [PatchPrefix]
        public static void PatchPrefix(Callback callback, bool isInstant, object __instance)
        {
            if (FastSearchPlugin.SearchInitialDelayEnabled.Value)
            {
                return;
            }

            var inventoryController = (InventoryControllerClass)_inventoryControllerField.GetValue(__instance);
            var searchableItem = (SearchableItemClass)_searchableItemField.GetValue(__instance);

            if ((bool)_terminatedField.GetValue(__instance))
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
