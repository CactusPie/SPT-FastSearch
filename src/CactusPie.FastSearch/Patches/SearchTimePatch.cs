using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace CactusPie.FastSearch.Patches
{
    public class SearchTimePatch : ModulePatch
    {
        // GClass1184 in 0.13.5.0.25793 
        private static readonly Type dateTimeClassType =
            PatchConstants.EftTypes.Single(x => x.GetMethod("get_Now") != null);
        // GClass2658 in 0.13.5.0.25793
        private static readonly Type GClass =
            PatchConstants.EftTypes.Single(x => x.GetMethod("GetNextDiscoveryTime") != null);

        // Note: "GClass1184.Now" != System.DateTime.Now
        private static readonly PropertyInfo dateTime_now = AccessTools.Property(dateTimeClassType, "Now");
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(GClass, "GetNextDiscoveryTime");
        }

        [PatchPostfix]
        public static void PatchPostFix(float speed, bool instant, ref DateTime __result)
        {
            if (instant) return;

            var actualTime = (DateTime)dateTime_now.GetValue(__result);

            float searchTimeMultiplier = FastSearchPlugin.SearchTimeMultiplier.Value;
            if (searchTimeMultiplier == 0)
            {
                __result = actualTime;
                return;
            }

            __result = actualTime.AddTicks((long)((__result - actualTime).Ticks * searchTimeMultiplier));
        }
    }
}