// -----------------------------------------------------------------------
// <copyright file="RespawnManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Interfaces;
    using Exiled.CustomModules.Events.EventArgs.CustomItems;
    using Exiled.CustomModules.Events.EventArgs.Tracking;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.RoleAssign;
    using Respawning;

    /// <summary>
    /// The actor which handles all team respawning tasks for roles.
    /// </summary>
    public class RespawnManager : StaticActor
    {
        private object nextKnownTeam;

        /// <summary>
        /// Gets or sets the next known team.
        /// </summary>
        public object NextKnownTeam
        {
            get => nextKnownTeam;
            set
            {
                if (value == nextKnownTeam)
                    return;

                if (value is SpawnableTeamType spawnableTeamType)
                {
                    nextKnownTeam = value;
                    return;
                }

                if (value is uint id && CustomTeam.TryGet(id, out CustomTeam _))
                {
                    nextKnownTeam = id;
                    return;
                }

                throw new ArgumentException("NextKnownTeam only accepts SpawnableTeamType and parsed uint.");
            }
        }

        /// <summary>
        /// Forces the spawn of a wave of the specified team.
        /// </summary>
        /// <param name="value">The specified team.</param>
        public void ForceSpawn(object value)
        {
            NextKnownTeam = value;

            if (NextKnownTeam is SpawnableTeamType team)
            {
                Respawning.RespawnManager.Singleton.ForceSpawnTeam(team);
                return;
            }

            Spawn();
            Respawning.RespawnManager.Singleton.RestartSequence();
        }

        /// <summary>
        /// Spawns the <see cref="NextKnownTeam"/> wave.
        /// </summary>
        public void Spawn()
        {
            CustomTeam team = CustomTeam.Get((uint)NextKnownTeam);
            List<Player> toSpawn = Player.Get(Team.Dead).ToList();
            CustomTeam.TrySpawn(toSpawn.Cast<Pawn>(), team);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Server.SelectingRespawnTeam += OnSelectingRespawnTeam;
            Exiled.Events.Handlers.Server.PreRespawningTeam += OnPreRespawningTeam;
            Exiled.Events.Handlers.Server.DeployingTeamRole += OnDeployingTeamRole;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Server.SelectingRespawnTeam -= OnSelectingRespawnTeam;
            Exiled.Events.Handlers.Server.PreRespawningTeam -= OnPreRespawningTeam;
            Exiled.Events.Handlers.Server.DeployingTeamRole -= OnDeployingTeamRole;
        }

        private void OnSelectingRespawnTeam(SelectingRespawnTeamEventArgs ev)
        {
            foreach (CustomTeam team in CustomTeam.List)
            {
                if (!team.EvaluateConditions || !team.CanSpawnByProbability)
                    continue;

                NextKnownTeam = team.Id;
            }
        }

        private void OnPreRespawningTeam(PreRespawningTeamEventArgs ev)
        {
            if (NextKnownTeam is not SpawnableTeamType team)
            {
                CustomTeam customTeam = CustomTeam.Get((uint)NextKnownTeam);

                if (customTeam.TeamsOwnership.Any(t => t == (ev.NextKnownTeam is SpawnableTeamType.ChaosInsurgency ? Team.ChaosInsurgency : Team.FoundationForces)))
                {
                    ev.MaxWaveSize = customTeam.Size;
                    return;
                }

                ev.IsAllowed = false;
                Spawn();
                return;
            }
        }

        private void OnDeployingTeamRole(DeployingTeamRoleEventArgs ev)
        {
            if (NextKnownTeam is SpawnableTeamType team)
                return;

            ev.Delegate = () => CustomTeam.TrySpawn(ev.Player.Cast<Pawn>(), (uint)NextKnownTeam);
        }
    }
}