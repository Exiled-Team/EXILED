// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.TriggerLunge()" />
    ///     to add the <see cref="Handlers.Scp939.Lunging" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.TriggerLunge))]
    internal static class Lunge
    {
        private static void Postfix(Scp939LungeAbility __instance)
        {
            LungingEventArgs ev = new(Player.Get(__instance.Owner));
            Handlers.Scp939.OnLunging(ev);
        }
    }
}