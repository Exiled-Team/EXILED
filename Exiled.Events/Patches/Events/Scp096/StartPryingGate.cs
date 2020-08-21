namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayableScps;

    /// <summary>
    /// Patches the <see cref="Scp096.PryGate"/> method.
    /// Adds the <see cref="Handlers.Scp096.StartPryingGate"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.PryGate))]
    internal static class StartPryingGate
    {
        private static bool Prefix(PlayableScps.Scp096 __instance, Door gate)
        {
            if (__instance.Charging && __instance.Enraged && (!gate.isOpen && gate.doorType == Door.DoorTypes.HeavyGate))
            {
                var ev = new StartPryingGateEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), gate);
                Exiled.Events.Handlers.Scp096.OnStartPryingGate(ev);
                return ev.IsAllowed;
            }

            return false;
        }
    }
}
