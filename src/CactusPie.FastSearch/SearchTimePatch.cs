using System;
using System.Reflection;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using HarmonyLib;
using System.Linq;

namespace CactusPie.FastSearch
{
    public class SearchTimePatch : ModulePatch
    {
        private static Type _searchOperationClass;
        private static PropertyInfo _tarkovDateTimeNow;

        protected override MethodBase GetTargetMethod()
        {
            Type tarkovDateTime = PatchConstants.EftTypes.Single(
                x => x.GetProperty("MoscowNow", BindingFlags.Static | BindingFlags.Public) != null && x.GetProperty("Now", BindingFlags.Static | BindingFlags.Public) != null
            );
            _tarkovDateTimeNow = AccessTools.Property(tarkovDateTime, "Now");

            _searchOperationClass = PatchConstants.EftTypes.Single(x => x.GetMethod("GetNextDiscoveryTime", BindingFlags.Static | BindingFlags.Public) != null);
            return _searchOperationClass.GetMethod("GetNextDiscoveryTime", BindingFlags.Static | BindingFlags.Public);
        }

        [PatchPostfix]
        public static void PatchPostFix(float speed, bool instant, ref DateTime __result)
        {
            if (instant)
            {
                return;
            }

            float searchTimeMultiplier = FastSearchPlugin.SearchTimeMultiplier.Value;

            DateTime now = (DateTime)_tarkovDateTimeNow.GetValue(null);
            if (searchTimeMultiplier == 0)
            {
                __result = now;
                return;
            }
            __result = now.AddTicks((long)((__result - now).Ticks * searchTimeMultiplier));
        }
    }
}
