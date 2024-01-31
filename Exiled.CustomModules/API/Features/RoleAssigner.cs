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
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="ITrackable"/>.</typeparam>
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
        /// Spawns human players based on the provided queue of teams and the length of the queue.
        /// </summary>
        /// <param name="queue">An array of teams representing the queue of teams to spawn.</param>
        /// <param name="queueLength">The length of the queue.</param>
        public virtual void SpawnHumans(Team[] queue, int queueLength)
        {
            EnqueuedHumans.Clear();

            IEnumerable<CustomRole> customRoles = CustomRole.Get(queue);
            List<uint> toSpawn = new();

            foreach (CustomRole role in customRoles)
            {
                for (int i = 0; i == role.MaxInstances; i++)
                {
                    if (role.AssignFromRole is RoleTypeId.None || !role.CanSpawnByProbability)
                        continue;

                    toSpawn.AddItem(role.Id);
                }
            }

            toSpawn.ShuffleList();
            EnqueuedHumans.AddRange(toSpawn.Cast<object>());

            IEnumerable<ReferenceHub> hubs = ReferenceHub.AllHubs.Where(PlayerRoles.RoleAssign.RoleAssigner.CheckPlayer);
            int num = hubs.Count() - toSpawn.Count;
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

            EnqueuedHumans.Where(o => o is uint).ForEach(id => CustomRole.Get((uint)id).Spawn(Player.Get(hubs.Random()).Cast<Pawn>()));
            EnqueuedHumans.RemoveAll(o => o is uint);

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

            IEnumerable<CustomRole> customScps = CustomRole.Get(Team.SCPs);
            List<uint> spawnable = new();

            foreach (CustomRole scp in customScps)
            {
                for (int i = 0; i == scp.MaxInstances; i++)
                {
                    if (scp.AssignFromRole is RoleTypeId.None || !scp.CanSpawnByProbability)
                        continue;

                    spawnable.AddItem(scp.Id);
                }
            }

            spawnable.ShuffleList();

            if (spawnable.Count > targetScpNumber)
                spawnable.RemoveRange(0, spawnable.Count - targetScpNumber);

            EnqueuedScps.AddRange(customScps);

            if (spawnable.Count < targetScpNumber)
            {
                for (int i = 0; i < targetScpNumber - spawnable.Count; i++)
                    EnqueuedScps.Add(ScpSpawner.NextScp);
            }

            int index = 0;
            List<ReferenceHub> chosenPlayers = ScpPlayerPicker.ChoosePlayers(targetScpNumber);

            Events.EventArgs.CustomRoles.AssigningScpRolesEventArgs ev = new(chosenPlayers, EnqueuedScps);
            AssigningScpRolesDispatcher.InvokeAll(ev);
            EnqueuedScps = ev.Roles;

            foreach (object role in EnqueuedScps.ToList())
            {
                if (role is not uint id)
                    continue;

                ReferenceHub chosenPlayer = chosenPlayers[0];
                Pawn pawn = Player.Get(chosenPlayer).Cast<Pawn>();
                CustomRole.Get(id).Spawn(pawn);
                EnqueuedScps.Remove(role);
                chosenPlayers.RemoveAt(0);
                ++index;
            }

            EnqueuedScps.RemoveAll(o => o is uint);

            List<RoleTypeId> enqueuedScps = EnqueuedScps.Cast<RoleTypeId>().ToList();
            while (enqueuedScps.Count > 0 && chosenPlayers.Count > 0)
            {
                RoleTypeId scp = enqueuedScps[0];
                enqueuedScps.RemoveAt(0);
                ScpSpawner.AssignScp(chosenPlayers, scp, enqueuedScps);
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Server.PreAssigningHumanRoles += OnPreAssigningHumanRoles;
            Exiled.Events.Handlers.Server.PreAssigningScpRoles += OnPreAssigningScpRoles;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Server.PreAssigningHumanRoles -= OnPreAssigningHumanRoles;
            Exiled.Events.Handlers.Server.PreAssigningScpRoles -= OnPreAssigningScpRoles;
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
    }
}