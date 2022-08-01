// -----------------------------------------------------------------------
// <copyright file="ExecutingRespawnEffects.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Respawning;

    /// <summary>
    /// Patches <see cref="RespawnEffectsController.ExecuteAllEffects"/>.
    /// Adds the <see cref="Server.ExecutingRespawningEffects"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.ExecuteAllEffects))]
    internal static class ExecutingRespawnEffects
    {
        private static bool Prefix(RespawnEffectsController.EffectType type, SpawnableTeamType team)
        {
            ExecutingRespawningEffectsEventArgs ev = new ExecutingRespawningEffectsEventArgs(type, team);
            Handlers.Server.OnExecutingRespawningEffects(ev);

            return ev.IsAllowed;
        }
    }
}
