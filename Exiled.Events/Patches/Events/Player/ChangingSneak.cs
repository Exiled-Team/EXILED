// -----------------------------------------------------------------------
// <copyright file="ChangingSneak.cs" company="Exiled Team">
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
    /// Patches <see cref="FirstPersonController.IsSneaking"/>.
    /// Adds the <see cref="Player.ChangingSneak"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.IsSneaking), MethodType.Setter)]
    internal static class ChangingSneak
    {
        private static void Prefix(FirstPersonController __instance, bool value)
        {
            var ev = new ChangingSneakEventArgs(API.Features.Player.Get(__instance.hub), value);

            Player.OnChangingSneak(ev);
        }
    }
}
