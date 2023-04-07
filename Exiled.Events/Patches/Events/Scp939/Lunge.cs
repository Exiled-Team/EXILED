// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.TriggerLunge" />
    ///     to add the <see cref="Scp939.Lunged" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.TriggerLunge))]
    internal static class Lunge
    {
        private static bool Prefix(Scp939LungeAbility __instance)
        {
            __instance.TriggerPos = __instance.CurPos;
            __instance.State = Scp939LungeState.Triggered;

            Handlers.Scp939.OnLunged(new(__instance.Owner));
            return false;
        }
    }
}