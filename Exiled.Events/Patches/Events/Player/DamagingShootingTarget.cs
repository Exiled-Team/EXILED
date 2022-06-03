// -----------------------------------------------------------------------
// <copyright file="DamagingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using AdminToys;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ShootingTarget.Damage(float, DamageHandlerBase, Vector3)"/>.
    /// Adds the <see cref="Handlers.Player.DamagingShootingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.Damage))]
    internal static class DamagingShootingTarget
    {
        private static bool Prefix(ShootingTarget __instance, ref bool __result, float damage, DamageHandlerBase handler, Vector3 exactHit)
        {
            try
            {
                if (handler is not AttackerDamageHandler attackerDamageHandler)
                {
                    __result = false;
                    return false;
                }

                if (attackerDamageHandler.Attacker.Hub is null)
                {
                    __result = false;
                    return false;
                }

                DamagingShootingTargetEventArgs ev = new(
                    Player.Get(attackerDamageHandler.Attacker.Hub),
                    damage,
                    Vector3.Distance(attackerDamageHandler.Attacker.Hub.transform.position, __instance._bullsEye.position),
                    exactHit,
                    __instance,
                    handler);
                Handlers.Player.OnDamagingShootingTarget(ev);

                if (!ev.IsAllowed)
                {
                    __result = false;
                    return false;
                }

                foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
                {
                    if (__instance._syncMode || referenceHub == attackerDamageHandler.Attacker.Hub)
                    {
                        __instance.TargetRpcReceiveData(referenceHub.characterClassManager.connectionToClient, ev.Amount, ev.Distance, exactHit, handler);
                    }
                }

                __result = true;
                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(DamagingShootingTarget).FullName}.{nameof(Prefix)}:\n{exception}");
                return false;
            }
        }
    }
}
