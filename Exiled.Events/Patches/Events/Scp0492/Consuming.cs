// -----------------------------------------------------------------------
// <copyright file="Consuming.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp0492
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp0492;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerStatsSystem;

    /// <summary>
    /// Patches the <see cref="ZombieConsumeAbility.ServerComplete()"/> to add
    /// <see cref="Handlers.Scp0492.Consuming"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ZombieConsumeAbility), nameof(ZombieConsumeAbility.ServerComplete))]
    internal class Consuming
    {
        public static bool Prefix(ZombieConsumeAbility __instance)
        {
            if (__instance.CurRagdoll != null && !ZombieConsumeAbility.ConsumedRagdolls.Add(__instance.CurRagdoll))
            {
                return false;
            }

            ConsumingEventArgs ev = new(API.Features.Player.Get(__instance.Owner), Ragdoll.Get(__instance.CurRagdoll));
            Handlers.Scp0492.OnConsuming(ev);

            if (ev.IsAllowed)
                __instance.Owner.playerStats.GetModule<HealthStat>().ServerHeal(100f);

            return false;
        }
    }
}