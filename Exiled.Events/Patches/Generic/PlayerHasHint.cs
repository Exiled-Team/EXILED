// -----------------------------------------------------------------------
// <copyright file="PlayerHasHint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic {
    using System.Collections.Generic;

    using Exiled.API.Features;

    using HarmonyLib;

    using Hints;

    using MEC;

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(Hint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    public static class PlayerHasHint {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // Creating a list for coroutine check
        private static Dictionary<Player, CoroutineHandle> playerHasHintCoroutines = new Dictionary<Player, CoroutineHandle>();

        public static void Postfix(HintDisplay __instance, Hint hint) {
            // Try to get the player, if it doesn't exist, just return
            if (!(Player.Get(__instance.gameObject) is Player player))
                return;

            // If Player value has couroutine, kill it
            if (playerHasHintCoroutines.TryGetValue(player, out CoroutineHandle oldcoroutine)) {
                // Kill the coroutine
                Timing.KillCoroutines(oldcoroutine);
            }

            // Create a new couroutine and assing the value to the player
            playerHasHintCoroutines[player] = Timing.RunCoroutine(HasHintToFalse(player, hint.DurationScalar));

            // If it is false, then to true
            if (!player.HasHint)
                player.HasHint = true;
        }

        public static IEnumerator<float> HasHintToFalse(Player player, float duration) {
            // Waiting for the hint to end
            yield return Timing.WaitForSeconds(duration);

            // If player gameobject doesn't exists, break the coroutine
            if (player.GameObject == null)
                yield break;

            player.HasHint = false;
            playerHasHintCoroutines.Remove(player);
        }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
