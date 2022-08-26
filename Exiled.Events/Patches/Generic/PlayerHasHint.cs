// -----------------------------------------------------------------------
// <copyright file="PlayerHasHint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using Exiled.API.Features;

    using HarmonyLib;

    using Hints;

    using MEC;

    using BaseHint = Hints.Hint;

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(BaseHint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    internal static class PlayerHasHint
    {
        private static void Postfix(HintDisplay __instance, BaseHint hint)
        {
            // Try to get the player, if it doesn't exist, just return
            if (__instance == null || __instance.gameObject == null || Player.Get(__instance.gameObject) is not Player player)
                return;

            // Kill the coroutine
            Timing.KillCoroutines(player.CurrentHintProccess.Key);

            // Create a new couroutine and assing the value to the player
            player.CurrentHintProccess = new(Timing.RunCoroutine(CurrentHint(player, hint.DurationScalar)), new API.Features.Hint(hint));
        }

        private static IEnumerator<float> CurrentHint(Player player, float duration)
        {
            // Waiting for the hint to end
            yield return Timing.WaitForSeconds(duration);

            // If player gameobject doesn't exists, break the coroutine
            if (!player.IsConnected)
                yield break;

            player.CurrentHintProccess = new();
        }
    }
}