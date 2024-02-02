// -----------------------------------------------------------------------
// <copyright file="RoleAssigner.cs" company="Exiled Team">
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

    /// <summary>
    /// The actor which handles all role assignment tasks for roles.
    /// </summary>
    public class RoleAssigner : StaticActor
    {
        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before assigning human roles.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Events.EventArgs.CustomRoles.AssigningHumanRolesEventArgs> AssigningHumanRolesDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before assigning SCP roles.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Events.EventArgs.CustomRoles.AssigningScpRolesEventArgs> AssigningScpRolesDispatcher { get; private set; }

        /// <summary>
        /// Gets or sets all enqueued SCPs.
        /// </summary>
        public List<object> EnqueuedScps { get; protected set; } = new();

        /// <summary>
        /// Gets or sets all enqueued humans.
        /// </summary>
        public List<object> EnqueuedHumans { get; protected set; } = new();

        /// <summary>
        /// Gets or sets all enqueued players.
        /// </summary>
        public List<Pawn> EnqueuedPlayers { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the human roles queue.
        /// </summary>
        public Team[] HumanRolesQueue { get; protected set; } = { };

        /// <summary>
        /// Gets a filter to retrieve all available human custom roles.
        /// </summary>
        public Func<CustomRole, bool> FilterHumans => customRole =>
            customRole.AssignFromRole is RoleTypeId.None &&
            (HumanRolesQueue.Contains(RoleExtensions.GetTeam(customRole.Role)) || customRole.TeamsOwnership.Any(to => HumanRolesQueue.Contains(to)));

        /// <summary>
        /// Gets a filter to retrieve all available SCP custom roles.
        /// </summary>
        public Func<CustomRole, bool> FilterScps => customRole => customRole.IsScp && !customRole.IsTeamUnit && customRole.AssignFromRole is RoleTypeId.None;

        /// <summary>
        /// Spawns human players based on the provided queue of teams and the length of the queue.
        /// </summary>
        /// <param name="queue">An array of teams representing the queue of teams to spawn.</param>
        /// <param name="queueLength">The length of the queue.</param>
        public virtual void SpawnHumans(Team[] queue, int queueLength)
        {
            HumanRolesQueue = queue;
            EnqueuedHumans.Clear();

            IEnumerable<CustomRole> customRoles = CustomRole.Get(FilterHumans);
            List<uint> spawnable = GetCustomRolesByProbability(customRoles).ToList();

            spawnable.ShuffleList();

            IEnumerable<ReferenceHub> hubs = ReferenceHub.AllHubs.Where(PlayerRoles.RoleAssign.RoleAssigner.CheckPlayer)
                .Concat(EnqueuedPlayers.Select(player => player.ReferenceHub));

            EnqueuedPlayers.Clear();

            if (spawnable.Count > hubs.Count())
                spawnable.RemoveRange(0, spawnable.Count - hubs.Count());

            EnqueuedHumans.AddRange(spawnable.Cast<object>());

            // Unless the custom roles are enough to cover the entire queue, some default roles will be selected.
            int num = hubs.Count() - spawnable.Count;
            RoleTypeId[] array = num > 0 ? new RoleTypeId[num] : null;

            if (array is not null)
            {
                for (int i = 0; i < num; i++)
                    array[i] = HumanSpawner.NextHumanRoleToSpawn;

                array.ShuffleList();
                EnqueuedHumans.AddRange(array.Cast<object>());
            }

            Events.EventArgs.CustomRoles.AssigningHumanRolesEventArgs ev = new(EnqueuedHumans);
            AssigningHumanRolesDispatcher.InvokeAll(ev);
            EnqueuedHumans = ev.Roles;

            DistributeOrEnqueue(hubs.ToList(), EnqueuedHumans.Where(o => o is uint).Cast<uint>(), FilterHumans);
            EnqueuedHumans.RemoveAll(o => o is not RoleTypeId);

            for (int j = 0; j < num; j++)
                HumanSpawner.AssignHumanRoleToRandomPlayer((RoleTypeId)EnqueuedHumans[j]);
        }

        /// <summary>RoleTypeId
        /// Spawns SCPs based on the target SCP number.
        /// </summary>
        /// <param name="targetScpNumber">The target number of SCPs to spawn.</param>
        public virtual void SpawnScps(int targetScpNumber)
        {
            EnqueuedScps.Clear();

            IEnumerable<CustomRole> customScps = CustomRole.Get(FilterScps);
            List<uint> spawnable = GetCustomRolesByProbability(CustomRole.Get(FilterScps)).ToList();

            spawnable.ShuffleList();

            if (spawnable.Count > targetScpNumber)
                spawnable.RemoveRange(0, spawnable.Count - targetScpNumber);

            EnqueuedScps.AddRange(spawnable.Cast<object>());

            if (spawnable.Count < targetScpNumber)
            {
                for (int i = 0; i < targetScpNumber - spawnable.Count; i++)
                {
                    RoleTypeId nextScp = ScpSpawner.NextScp;
                    if (customScps.Any(scp => scp.OverrideScps.Contains(nextScp)))
                        continue;

                    EnqueuedScps.Add((object)nextScp);
                }
            }

            List<ReferenceHub> chosenPlayers = ScpPlayerPicker.ChoosePlayers(targetScpNumber);

            if (spawnable.Count < targetScpNumber)
                EnqueuedPlayers.AddRange(chosenPlayers.Select(rh => Player.Get(rh)).Take(targetScpNumber - spawnable.Count));

            Events.EventArgs.CustomRoles.AssigningScpRolesEventArgs ev = new(chosenPlayers, EnqueuedScps);
            AssigningScpRolesDispatcher.InvokeAll(ev);
            EnqueuedScps = ev.Roles;

            List<RoleTypeId> enqueuedScps = EnqueuedScps.Where(role => role is RoleTypeId).Cast<RoleTypeId>().ToList();
            while (enqueuedScps.Count > 0 && chosenPlayers.Count > 0)
            {
                RoleTypeId scp = enqueuedScps[0];
                enqueuedScps.RemoveAt(0);
                ScpSpawner.AssignScp(chosenPlayers, scp, enqueuedScps);
            }

            DistributeOrEnqueue(chosenPlayers, EnqueuedScps.Where(o => o is uint).Cast<uint>(), FilterScps);
            EnqueuedScps.Clear();
        }

        /// <summary>
        /// Distributes the given roles to all specified players based on a predicate.
        /// <para/>
        /// If a role cannot be assigned due to some circumstances, it will be enqueued until a new role is found.
        /// </summary>
        /// <param name="players">The players to assign the roles to.</param>
        /// <param name="roles">The roles to be assigned.</param>
        /// <param name="predicate">The predicate.</param>
        public void DistributeOrEnqueue(IEnumerable<Pawn> players, List<uint> roles, Func<CustomRole, bool> predicate) =>
            DistributeOrEnqueue(players.Select(player => player.ReferenceHub).ToList(), roles, predicate);

        /// <summary>
        /// Distributes the given roles to all specified players based on a predicate.
        /// <para/>
        /// If a role cannot be assigned due to some circumstances, it will be enqueued until a new role is found.
        /// </summary>
        /// <param name="players">The players to assign the roles to.</param>
        /// <param name="roles">The roles to be assigned.</param>
        /// <param name="predicate">The predicate.</param>
        public virtual void DistributeOrEnqueue(List<ReferenceHub> players, IEnumerable<uint> roles, Func<CustomRole, bool> predicate)
        {
            if (roles.IsEmpty())
                return;

            int spawned = 0;
            foreach (uint id in roles)
            {
                if (!CustomRole.TryGet(id, out CustomRole role) || (role.Instances >= role.MaxInstances || !role.EvaluateConditions))
                    continue;

                ReferenceHub target = players[0];
                Player.Get(target).Cast<Pawn>().SetRole(role);
                players.RemoveAt(0);
                ++spawned;
            }

            if (spawned < roles.Count())
            {
                for (int i = spawned; i == roles.Count(); i++)
                {
                    ReferenceHub target = players[0];
                    Player.Get(target).Cast<Pawn>().SetRole(FindAvailableRole(predicate));
                    players.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Finds an available <see cref="CustomRole"/> based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The first available <see cref="CustomRole"/>.</returns>
        public CustomRole FindAvailableRole(Func<CustomRole, bool> predicate) =>
            FindAvailableRole(CustomRole.Get(predicate).Shuffle().ToList());

        /// <summary>
        /// Finds the first available <see cref="CustomRole"/> in the specified list.
        /// </summary>
        /// <param name="toEvaluate">The list of roles to evaluate.</param>
        /// <returns>The first available <see cref="CustomRole"/>.</returns>
        public CustomRole FindAvailableRole(List<CustomRole> toEvaluate)
        {
            if (toEvaluate.IsEmpty())
            {
                throw new Exception(
                    "Couldn't find any alternative custom role to assign." +
                    "Common causes may be circular dependencies into conditions, overridden SCPs or a wrong defined amount of maximum allowed instances.");
            }

            CustomRole role = toEvaluate[0];

            if ((role.IsScp && !role.OverrideScps.IsEmpty()) || role.Instances >= role.MaxInstances ||
                (role.AssignFromRole is RoleTypeId.None && !role.EvaluateConditions))
            {
                toEvaluate.RemoveAt(0);
                return FindAvailableRole(toEvaluate);
            }

            return role;
        }

        /// <summary>
        /// Gets all custom roles after evaluating their probability.
        /// </summary>
        /// <param name="customRoles">The custom roles to evaluate.</param>
        /// <returns>All evaluated custom roles.</returns>
        public virtual IEnumerable<uint> GetCustomRolesByProbability(IEnumerable<CustomRole> customRoles)
        {
            foreach (CustomRole customRole in customRoles)
            {
                for (int i = 0; i == customRole.MaxInstances; i++)
                {
                    if (!customRole.CanSpawnByProbability)
                        continue;

                    yield return customRole.Id;
                }
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Server.PreAssigningHumanRoles += OnPreAssigningHumanRoles;
            Exiled.Events.Handlers.Server.PreAssigningScpRoles += OnPreAssigningScpRoles;

            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Spawning += OnSpawning;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Server.PreAssigningHumanRoles -= OnPreAssigningHumanRoles;
            Exiled.Events.Handlers.Server.PreAssigningScpRoles -= OnPreAssigningScpRoles;

            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawning;
        }

        private void OnPreAssigningHumanRoles(PreAssigningHumanRolesEventArgs ev)
        {
            SpawnHumans(ev.Queue, ev.QueueLength);
            ev.IsAllowed = false;
        }

        private void OnPreAssigningScpRoles(PreAssigningScpRolesEventArgs ev)
        {
            SpawnScps(ev.Amount);
            ev.IsAllowed = false;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!PlayerRoles.RoleAssign.RoleAssigner.CheckPlayer(ev.Player.ReferenceHub))
                return;

            EnqueuedPlayers.Add(ev.Player.Cast<Pawn>());
        }

        private void OnSpawning(SpawningEventArgs ev)
        {
            if (!EnqueuedPlayers.Contains(ev.Player) || ev.Player.Cast<Pawn>().HasCustomRole)
                return;

            CustomRole customRole = FindAvailableRole(role => role.AssignFromRole == ev.Player.Role);

            if (!customRole)
                return;

            customRole.ForceSpawn(ev.Player.Cast<Pawn>(), true);
        }
    }
}