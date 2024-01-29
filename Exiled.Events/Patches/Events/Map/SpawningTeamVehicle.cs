// -----------------------------------------------------------------------
// <copyright file="SpawningTeamVehicle.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.Attributes;

    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using Respawning;

    /// <summary>
    /// Patches <see cref="RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType, SpawnableTeamType)"/>.
    /// Adds the <see cref="Handlers.Map.SpawningTeamVehicle"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.SpawningTeamVehicle))]
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.ExecuteAllEffects))]
    internal static class SpawningTeamVehicle
    {
        private static bool Prefix(RespawnEffectsController.EffectType type, SpawnableTeamType team)
        {
            if (type != RespawnEffectsController.EffectType.Selection)
                return true;

            SpawningTeamVehicleEventArgs ev = new(team);
            Handlers.Map.OnSpawningTeamVehicle(ev);

            return ev.IsAllowed;
        }
    }
}