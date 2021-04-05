// -----------------------------------------------------------------------
// <copyright file="PlayerHasHintPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using HarmonyLib;
    using Hints;
    using MEC;

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(Hint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    public static class PlayerHasHintPatch
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // Creating a list for coroutine check
        private static List<Player> plyHaveCoroutine = new List<Player>();

        public static void Postfix(HintDisplay __instance, Hint hint)
        {
            // Try to get the player, if it doesn't exist, just return
            Player player = Player.Get(__instance.gameObject);
            if (player == null)
                return;

            // Set HasHint to true, and then delays to false for the duration of the hint
            player.HasHint = true;

            // Set HintDisplayTime to the duration of the last hint
            // This prevents to set HasHint to false before the hint has finished
            player.HintRemainingTime = hint.DurationScalar;

            // If not contains, run coroutine
            if (!plyHaveCoroutine.Contains(player))
            {
                plyHaveCoroutine.Add(player);
                Timing.RunCoroutine(HasHintToFalse(player));
            }
        }

        public static IEnumerator<float> HasHintToFalse(Player player)
        {
            // if player hint remaining time is less than 0, stop the coroutine
            while (player.HintRemainingTime > 0)
            {
                yield return Timing.WaitForSeconds(1f);
                player.HintRemainingTime -= 1;
                if (player.HintRemainingTime == 0 || player.HintRemainingTime < 0)
                {
                    player.HasHint = false;
                    player.HintRemainingTime = 0;
                    plyHaveCoroutine.Remove(player);
                }
            }
        }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
