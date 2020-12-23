// -----------------------------------------------------------------------
// <copyright file="ChangingSprint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Stamina._isSprinting"/>.
    /// Adds the <see cref="Player.ChangingSprint"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Stamina), nameof(Stamina._isSprinting), MethodType.Setter)]
    internal static class ChangingSprint
    {
        private static void Prefix(Stamina __instance, bool value)
        {
            var ev = new ChangingSprintEventArgs(API.Features.Player.Get(__instance._hub), value);

            Player.OnChangingSprint(ev);
        }
    }
}
