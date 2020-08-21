namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using HarmonyLib;

    using Scp096 = PlayableScps.Scp096;

    [HarmonyPatch(typeof(Scp096), nameof(Scp096.MaxSpeed), MethodType.Getter)]
    internal static class Scp096MaxShield
    {
        private static bool Prefix(Scp096 __instance, ref int __result)
        {
            __result = Exiled.API.Features.Scp096.MaxShield;
            return false;
        }
    }
}
