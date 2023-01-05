// -----------------------------------------------------------------------
// <copyright file="CurrentHint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using API.Features;

    using HarmonyLib;

    using Hints;

    using MEC;

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(Hints.Hint)"/>.
    /// Implements <see cref="Player.CurrentHint"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    internal static class CurrentHint
    {
        // Creating a list for coroutine check
        private static readonly Dictionary<Player, CoroutineHandle> PlayerCurrentHints = new();

        private static void Postfix(HintDisplay __instance, Hints.Hint hint)
        {
            if (__instance == null || __instance.gameObject == null || Player.Get(__instance.gameObject) is not { } player || hint is not TextHint textHint)
                return;

            if (PlayerCurrentHints.TryGetValue(player, out CoroutineHandle oldCoroutine))
                Timing.KillCoroutines(oldCoroutine);

            PlayerCurrentHints[player] = Timing.RunCoroutine(CurrentHintToNull(player, hint.DurationScalar));
            player.CurrentHint = new API.Features.Hint(textHint.Text, textHint.DurationScalar);
        }

        private static IEnumerator<float> CurrentHintToNull(Player player, float duration)
        {
            yield return Timing.WaitForSeconds(duration);

            if (!player.IsConnected)
                yield break;

            player.CurrentHint = null;
            PlayerCurrentHints.Remove(player);
        }
    }
}