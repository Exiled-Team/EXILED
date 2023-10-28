// -----------------------------------------------------------------------
// <copyright file="StaminaUsage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;

    /// <summary>
    /// Patches <see cref="FpcStateProcessor.UpdateMovementState"/>.
    /// Implements <see cref="Player.IsUsingStamina"/>.
    /// </summary>
    [HarmonyPatch(typeof(FpcStateProcessor), nameof(FpcStateProcessor.UpdateMovementState))]
    internal class StaminaUsage
    {
        private static void Postfix(FpcStateProcessor __instance, PlayerMovementState state, ref PlayerMovementState __result)
        {
            if (Player.TryGet(__instance.Hub, out Player player) && !player.IsUsingStamina)
                __instance._stat.CurValue = __instance._stat.MaxValue;
        }
    }
}