// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp096;

    using UnityEngine;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp096HitHandler.ProcessHits" />.
    /// Adds the <see cref="Handlers.Scp096.Attacking" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.Attacking))]
    [HarmonyPatch(typeof(Scp096HitHandler), nameof(Scp096HitHandler.ProcessHits))]
    internal static class Attacking
    {
        private static bool Prefix(Scp096HitHandler __instance, ref Scp096HitResult __result, int count)
        {
            Scp096HitResult scp096HitResult = Scp096HitResult.None;
            Player scpPlayer = Player.Get(__instance._scpRole._lastOwner);

            for (int i = 0; i < count; i++)
            {
                Collider collider = Scp096HitHandler.Hits[i];
                __instance.CheckDoorHit(collider);

                if (!collider.TryGetComponent(out IDestructible destructible))
                    continue;

                int layerMask = (int)Scp096HitHandler.SolidObjectMask & ~(1 << collider.gameObject.layer);

                if (Physics.Linecast(__instance._scpRole.CameraPosition, destructible.CenterOfMass, layerMask) || !__instance._hitNetIDs.Add(destructible.NetworkId))
                    continue;

                if (destructible is BreakableWindow breakableWindow)
                {
                    if (!__instance.DealDamage(breakableWindow, __instance._windowDamage))
                        continue;

                    scp096HitResult |= Scp096HitResult.Window;
                    continue;
                }

                if (destructible is not HitboxIdentity hitBoxIdentity || !HitboxIdentity.IsEnemy(Team.SCPs, hitBoxIdentity.TargetHub.GetTeam()))
                    continue;

                Player target = Player.Get(hitBoxIdentity.TargetHub);
                bool isTarget = __instance._targetCounter.HasTarget(target.ReferenceHub);

                AttackingEventArgs args = new(scpPlayer, target, __instance._humanTargetDamage, __instance._humanNontargetDamage, true);
                Handlers.Scp096.OnAttacking(args);

                if (!args.IsAllowed)
                    continue;

                float damage = isTarget ? args.HumanDamage : args.NonTargetDamage;

                if (!__instance.DealDamage(hitBoxIdentity, damage))
                    continue;

                scp096HitResult |= Scp096HitResult.Human;
                if (!target.IsAlive)
                {
                    scp096HitResult |= Scp096HitResult.Lethal;
                }
            }

            __instance.HitResult |= scp096HitResult;
            __result = scp096HitResult;

            return false;
        }
    }
}