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
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Enums;
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
        /// Gets or sets the previous spawned team.
        /// </summary>
        public object PreviousKnownTeam { get; protected set; }

        /// <summary>
        /// Gets all teams' tickets.
        /// </summary>
        public Dictionary<uint, Func<uint>> AllTickets { get; } = new();

        /// <summary>
        /// Gets all enqueued roles.
        /// </summary>
        public List<object> EnqueuedRoles { get; }

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

        /// <inheritdoc />
        protected override void PostInitialize_Static()
        {
            foreach (CustomTeam team in CustomTeam.Get(t => t.UseTickets))
                AllTickets[team.Id] = () => team.Tickets;
        }

        /// <inheritdoc />
        protected override void EndPlay_Static()
        {
            AllTickets.Clear();
            EnqueuedRoles.Clear();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Server.RestartedRespawnSequence += OnRestartedRespawnSequence;
            Exiled.Events.Handlers.Server.SelectingRespawnTeam += OnSelectingRespawnTeam;
            Exiled.Events.Handlers.Server.PreRespawningTeam += OnPreRespawningTeam;
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
            Exiled.Events.Handlers.Server.DeployingTeamRole += OnDeployingTeamRole;
            Exiled.Events.Handlers.Server.RespawnedTeam += OnRespawnedTeam;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Server.RestartedRespawnSequence -= OnRestartedRespawnSequence;
            Exiled.Events.Handlers.Server.SelectingRespawnTeam -= OnSelectingRespawnTeam;
            Exiled.Events.Handlers.Server.PreRespawningTeam -= OnPreRespawningTeam;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;
            Exiled.Events.Handlers.Server.DeployingTeamRole -= OnDeployingTeamRole;
            Exiled.Events.Handlers.Server.RespawnedTeam -= OnRespawnedTeam;
        }

        private void OnRestartedRespawnSequence(RestartedRespawnSequenceEventArgs ev)
        {
            if (PreviousKnownTeam is not uint id || !CustomTeam.TryGet(id, out CustomTeam team))
                return;

            ev.TimeForNextSequence = team.NextSequenceTime;
        }

        private void OnSelectingRespawnTeam(SelectingRespawnTeamEventArgs ev)
        {
            NextKnownTeam = null;
            foreach (CustomTeam team in CustomTeam.List)
            {
                if ((team.UseTickets && team.Tickets <= 0) || !team.EvaluateConditions || !team.CanSpawnByProbability)
                    continue;

                NextKnownTeam = team.Id;
                return;
            }

            if (NextKnownTeam is null)
                NextKnownTeam = ev.Team;
        }

        private void OnPreRespawningTeam(PreRespawningTeamEventArgs ev)
        {
            PreviousKnownTeam = NextKnownTeam;

            if (NextKnownTeam is not SpawnableTeamType team)
            {
                CustomTeam customTeam = CustomTeam.Get((uint)NextKnownTeam);

                if (!customTeam)
                    return;

                if (customTeam.TeamsOwnership.Any(t => t == (ev.NextKnownTeam is SpawnableTeamType.ChaosInsurgency ? Team.ChaosInsurgency : Team.FoundationForces)))
                {
                    ev.MaxWaveSize = customTeam.Size;
                    return;
                }

                ev.IsAllowed = false;
                Spawn();
                Respawning.RespawnManager.Singleton.RestartSequence();
                return;
            }
        }

        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (NextKnownTeam is not SpawnableTeamType team)
                return;

            foreach (CustomRole customRole in CustomRole.List)
            {
                if (!customRole.IsTeamUnit || customRole.AssignFromTeam != team || !customRole.EvaluateConditions)
                    continue;

                for (int i = customRole.Instances; i <= customRole.Instances; i++)
                {
                    if (!customRole.CanSpawnByProbability)
                        continue;

                    EnqueuedRoles.Add(customRole.Id);
                }
            }

            EnqueuedRoles.AddRange(ev.SpawnQueue.Cast<object>());

            object captain = EnqueuedRoles.FirstOrDefault(role => role is RoleTypeId rId && rId is RoleTypeId.NtfCaptain);

            if (captain is not null)
                EnqueuedRoles.Remove(captain);

            EnqueuedRoles.RemoveRange(ev.SpawnQueue.Count - 1, EnqueuedRoles.Count - ev.SpawnQueue.Count);

            if (captain is not null)
            {
                EnqueuedRoles.Add(captain);
                captain = null;
            }
        }

        private void OnDeployingTeamRole(DeployingTeamRoleEventArgs ev)
        {
            if (NextKnownTeam is SpawnableTeamType team)
            {
                object role = EnqueuedRoles.Random();

                if (role is not uint)
                {
                    EnqueuedRoles.Remove(role);
                    return;
                }

                ev.Delegate = () =>
                {
                    CustomRole customRole = CustomRole.Get((uint)role);
                    CustomRole.TrySpawn(ev.Player.Cast<Pawn>(), customRole);
                    EnqueuedRoles.Remove(role);
                };
            }

            ev.Delegate = () => CustomTeam.TrySpawn(ev.Player.Cast<Pawn>(), (uint)NextKnownTeam);
        }

        private void OnRespawnedTeam(RespawnedTeamEventArgs ev)
        {
            EnqueuedRoles.Clear();
        }
    }
}