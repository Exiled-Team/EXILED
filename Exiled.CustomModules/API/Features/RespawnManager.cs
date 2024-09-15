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
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.Events.EventArgs.CustomRoles;
    using Exiled.Events.EventArgs.Server;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    /// The actor which handles all team respawning tasks for roles.
    /// </summary>
    public class RespawnManager : StaticActor
    {
        private object nextKnownTeam;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when selecting the next known team.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<SelectingCustomTeamRespawnEventArgs> SelectingCustomTeamRespawnDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the next known team.
        /// </summary>
        public object NextKnownTeam
        {
            get => nextKnownTeam;
            set
            {
                switch (value)
                {
                    case SpawnableTeamType:
                        nextKnownTeam = value;
                        return;
                    case CustomTeam customTeam:
                        nextKnownTeam = customTeam.Id;
                        return;
                    case uint id when CustomTeam.TryGet(id, out CustomTeam _):
                        nextKnownTeam = id;
                        return;
                    default:
                        throw new ArgumentException("NextKnownTeam only accepts SpawnableTeamType, parsed uint & CustomTeam.");
                }
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
        public List<object> EnqueuedRoles { get; } = new();

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
            List<Player> toSpawn = Player.Get(x => x.Role is SpectatorRole { IsReadyToRespawn: true }).ToList();
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

                SelectingCustomTeamRespawnEventArgs @event = new((object)team.Id);
                SelectingCustomTeamRespawnDispatcher.InvokeAll(@event);

                NextKnownTeam = @event.Team;
                return;
            }

            NextKnownTeam ??= ev.Team;
        }

        private void OnPreRespawningTeam(PreRespawningTeamEventArgs ev)
        {
            PreviousKnownTeam = NextKnownTeam;

            if (NextKnownTeam is null or SpawnableTeamType)
                return;

            CustomTeam customTeam = CustomTeam.Get((uint)NextKnownTeam);
            if (customTeam is null)
                return;

            if (customTeam.TeamsOwnership.Any(t =>
                    t == (ev.NextKnownTeam is SpawnableTeamType.ChaosInsurgency ?
                        Team.ChaosInsurgency : Team.FoundationForces)))
            {
                ev.MaxWaveSize = customTeam.Size;
                return;
            }

            ev.IsAllowed = false;
            Spawn();
            Respawning.RespawnManager.Singleton?.RestartSequence();
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

            if (captain is null)
                return;

            EnqueuedRoles.Add(captain);
        }

        private void OnDeployingTeamRole(DeployingTeamRoleEventArgs ev)
        {
            if (NextKnownTeam is SpawnableTeamType)
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
                    ev.Player.Cast<Pawn>().SetRole(customRole);
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