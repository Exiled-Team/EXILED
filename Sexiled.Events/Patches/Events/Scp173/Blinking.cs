// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1313
    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp173PlayerScript.DoBlinkingSequence"/>.
    /// Adds the <see cref="Handlers.Scp173.Blinking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp173PlayerScript), nameof(Scp173PlayerScript.DoBlinkingSequence))]
    internal static class Blinking
    {
        private static void Prefix(Scp173PlayerScript __instance)
        {
            if (Scp173PlayerScript._remainingTime - Time.fixedDeltaTime < 0f)
            {
                var triggers = ListPool<API.Features.Player>.Shared.Rent();

                foreach (var player in API.Features.Player.List)
                {
                    if (player.Team != Team.SCP && player.Team != Team.RIP)
                    {
                        Scp173PlayerScript playerScript = player.ReferenceHub.characterClassManager.Scp173;

                        if (playerScript.LookFor173(__instance.gameObject, true))
                            triggers.Add(player);
                    }
                }

                if (triggers.Count > 0)
                    Handlers.Scp173.OnBlinking(new BlinkingEventArgs(API.Features.Player.Get(__instance.gameObject), triggers));

                ListPool<API.Features.Player>.Shared.Return(triggers);
            }
        }
    }
}
