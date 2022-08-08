// -----------------------------------------------------------------------
// <copyright file="KillingByCollision.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CheckpointKiller.OnTriggerEnter"/>.
    /// Adds the <see cref="KillingByCollision"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CheckpointKiller), nameof(CheckpointKiller.OnTriggerEnter))]
    internal static class KillingByCollision
    {
        private static bool Prefix(Collider other)
        {
            if (!ReferenceHub.TryGetHub(other.transform.root.gameObject, out ReferenceHub referenceHub))
                return false;

            KillingByCollisionEventArgs ev = new KillingByCollisionEventArgs(API.Features.Player.Get(referenceHub));
            Handlers.Player.OnKillingByCollision(ev);

            if (!ev.IsAllowed)
                return false;

            referenceHub.playerStats.DealDamage(new PlayerStatsSystem.UniversalDamageHandler(-1f, PlayerStatsSystem.DeathTranslations.Crushed));
            return false;
        }
    }
}
