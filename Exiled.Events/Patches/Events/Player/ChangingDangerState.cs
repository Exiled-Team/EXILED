// -----------------------------------------------------------------------
// <copyright file="ChangingDangerState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;

    using CustomPlayerEffects.Danger;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    /// <summary>
    /// Adds <see cref="ChangingDangerState"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDangerState))]
    [HarmonyPatch(typeof(DangerStackBase), nameof(DangerStackBase.IsActive), MethodType.Setter)]
    internal class ChangingDangerState
    {
        private static Dictionary<Type, DangerType> dangerToType = new()
        {
            { typeof(WarheadDanger), DangerType.Warhead },
            { typeof(CardiacArrestDanger), DangerType.CardiacArrest },
            { typeof(RageTargetDanger), DangerType.RageTarget },
            { typeof(CorrodingDanger), DangerType.Corroding },
            { typeof(PlayerDamagedDanger), DangerType.PlayerDamaged },
            { typeof(ScpEncounterDanger), DangerType.ScpEncounter },
            { typeof(ZombieEncounterDanger), DangerType.ZombieEncounter },
            { typeof(ArmedEnemyDanger), DangerType.ArmedEnemy },
        };

#pragma warning disable SA1313
        private static bool Prefix(DangerStackBase __instance, bool value)
#pragma warning restore SA1313
        {
            if (!dangerToType.TryGetValue(__instance.GetType(), out DangerType type))
                throw new ArgumentException($"{__instance} does not have a DangerType");

            if (value == __instance._isActive)
                return false;

            ChangingDangerStateEventArgs ev = new(Player.Get(__instance.Owner), __instance, type, value);
            Handlers.Player.OnChangingDangerState(ev);

            return ev.IsAllowed;
        }
    }
}