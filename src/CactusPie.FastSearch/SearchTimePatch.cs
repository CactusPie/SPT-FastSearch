using System;
using System.Reflection;
using Aki.Reflection.Patching;
using TarkovDateTime = GClass1252;

namespace CactusPie.FastSearch
{
    public class SearchTimePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            MethodInfo method = typeof(GClass2517).GetMethod("GetNextDiscoveryTime", BindingFlags.Static | BindingFlags.Public);
            return method;
        }

        [PatchPostfix]
        public static void PatchPostFix(float speed, bool instant, ref DateTime __result)
        {
            if (instant)
            {
                return;
            }

            float searchTimeMultiplier = FastSearchPlugin.SearchTimeMultiplier.Value;

            if (searchTimeMultiplier == 0)
            {
                __result = TarkovDateTime.Now;
                return;
            }
            
            __result = TarkovDateTime.Now.AddTicks((long)((__result - TarkovDateTime.Now).Ticks * searchTimeMultiplier));
        }
    }
}