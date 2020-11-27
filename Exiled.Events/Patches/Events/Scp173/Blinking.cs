// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

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
                var triggers = ListPool<Player>.Shared.Rent();

                foreach (var player in Player.List)
                {
                    if (player.Team != Team.SCP && player.Team != Team.RIP)
                    {
                        Scp173PlayerScript playerScript = player.ReferenceHub.characterClassManager.Scp173;
                        if (playerScript.LookFor173(__instance.gameObject, true))
                            triggers.Add(player);
                    }
                }

                if (triggers.Count > 0)
                    Handlers.Scp173.OnBlinking(new BlinkingEventArgs(Player.Get(__instance.gameObject), triggers));

                ListPool<Player>.Shared.Return(triggers);
            }
        }
    }
}
