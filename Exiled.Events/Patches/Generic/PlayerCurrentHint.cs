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

    /// <summary>
    /// Patches <see cref="HintDisplay.Show(Hints.Hint)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    internal static class PlayerCurrentHint
    {
        private static void Postfix(HintDisplay __instance, Hints.Hint hint)
        {
            if (__instance == null || __instance.gameObject == null || Player.Get(__instance.gameObject) is not { } player)
                return;

            if (player.HintCoroutine.Value.IsRunning)
                Timing.KillCoroutines(player.HintCoroutine.Value);

            player.HintCoroutine = new KeyValuePair<API.Features.Hint, CoroutineHandle>(
                new API.Features.Hint() { Content = hint is TextHint textHint ? textHint.Text : null, Duration = hint.DurationScalar },
                Timing.RunCoroutine(ResetPlayerHint(player, hint.DurationScalar)));
        }

        private static IEnumerator<float> ResetPlayerHint(Player player, float duration)
        {
            yield return Timing.WaitForSeconds(duration);

            if (!player.IsConnected)
                yield break;

            player.HintCoroutine = new();
        }
    }
}
