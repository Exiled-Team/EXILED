// -----------------------------------------------------------------------
// <copyright file="CurrentPlayerHintsPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
    using HarmonyLib;
    using Hints;
    using MEC;

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(Hint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    public static class CurrentPlayerHintsPatch
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void Postfix(HintDisplay __instance, Hint hint)
        {
            // Try to get the player, if it doesn't exist, just return
            Player player = Player.Get(__instance.gameObject);
            if (player == null)
                return;

            // Set HasHint to true, and then delays to false for the duration of the hint
            player.HasHint = true;
            Timing.CallDelayed(hint.DurationScalar, () =>
            {
                player.HasHint = false;
            });
        }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
