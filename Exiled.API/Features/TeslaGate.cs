// -----------------------------------------------------------------------
// <copyright file="TeslaGate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces;

    using Hazards;
    using MEC;

    using PlayerRoles;

    using UnityEngine;

    using BaseTeslaGate = global::TeslaGate;

    /// <summary>
    /// The in-game tesla gate.
    /// </summary>
    public class TeslaGate : IWrapper<BaseTeslaGate>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BaseTeslaGate"/>s and their corresponding <see cref="TeslaGate"/>.
        /// </summary>
        internal static readonly Dictionary<BaseTeslaGate, TeslaGate> BaseTeslaGateToTeslaGate = new(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="TeslaGate"/> class.
        /// </summary>
        /// <param name="baseTeslaGate">The <see cref="BaseTeslaGate"/> instance.</param>
        /// <param name="room">The <see cref="Exiled.API.Features.Room"/> for this tesla.</param>
        internal TeslaGate(BaseTeslaGate baseTeslaGate, Room room)
        {
            Base = baseTeslaGate;
            BaseTeslaGateToTeslaGate.Add(baseTeslaGate, this);
            Room = room;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> which contains all the <see cref="TeslaGate"/> instances.
        /// </summary>
        public static IReadOnlyCollection<TeslaGate> List => BaseTeslaGateToTeslaGate.Values;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Player"/> which contains all the players ignored by tesla gates.
        /// </summary>
        public static HashSet<Player> IgnoredPlayers { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="RoleTypeId"/> which contains all the roles ignored by tesla gates.
        /// </summary>
        public static List<RoleTypeId> IgnoredRoles { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Team"/> which contains all the teams ignored by tesla gates.
        /// </summary>
        public static List<Team> IgnoredTeams { get; set; } = new();

        /// <summary>
        /// Gets the base <see cref="BaseTeslaGate"/>.
        /// </summary>
        public BaseTeslaGate Base { get; }

        /// <summary>
        /// Gets the tesla gate's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the tesla gate's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the tesla gate's position.
        /// </summary>
        public Vector3 Position => Transform.position;

        /// <summary>
        /// Gets the tesla gate's rotation.
        /// </summary>
        public Quaternion Rotation => Quaternion.Euler(Base.localRotation);

        /// <summary>
        /// Gets the tesla gate's <see cref="Features.Room"/> which is located in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets a value indicating whether or not the tesla gate's shock burst is in progess.
        /// </summary>
        public bool IsShocking => Base.InProgress;

        /// <summary>
        /// Gets or sets the tesla gate's inactive time.
        /// </summary>
        public float InactiveTime
        {
            get => Base.NetworkInactiveTime;
            set => Base.NetworkInactiveTime = value;
        }

        /// <summary>
        /// Gets or sets the tesla gate's radius from which players can be hurted.
        /// </summary>
        public Vector3 HurtRange
        {
            get => Base.sizeOfKiller;
            set => Base.sizeOfKiller = value;
        }

        /// <summary>
        /// Gets or sets the tesla gate's distance from which can be triggered.
        /// </summary>
        public float TriggerRange
        {
            get => Base.sizeOfTrigger;
            set => Base.sizeOfTrigger = value;
        }

        /// <summary>
        /// Gets or sets the tesla gate's distance from which players must stand for it to enter idle mode.
        /// </summary>
        public float IdleRange
        {
            get => Base.distanceToIdle;
            set => Base.distanceToIdle = value;
        }

        /// <summary>
        /// Gets or sets the tesla gate's windup time to wait before generating the shock.
        /// </summary>
        public float ActivationTime
        {
            get => Base.windupTime;
            set => Base.windupTime = value;
        }

        /// <summary>
        /// Gets or sets the tesla gate's cooldown to wait before the next shock.
        /// </summary>
        public float CooldownTime
        {
            get => Base.cooldownTime;
            set => Base.cooldownTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tesla gate is idling.
        /// </summary>
        public bool IsIdling
        {
            get => Base.isIdling;
            set
            {
                if (value)
                    Base.RpcDoIdle();
                else
                    Base.RpcDoneIdling();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tesla gate's next burst should be treated as instant burst.
        /// <para>The instant burst ignores the standard cooldown time, reducing it to the cooldown time used for bursts triggered by SCP-079.</para>
        /// </summary>
        public bool UseInstantBurst
        {
            get => Base.next079burst;
            set => Base.next079burst = value;
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="UnityEngine.GameObject"/> which contains all the tantrums to destroy.
        /// </summary>
        public List<TantrumEnvironmentalHazard> TantrumsToDestroy => Base.TantrumsToBeDestroyed;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the hurt range.
        /// </summary>
        public IEnumerable<Player> PlayersInHurtRange => Player.List.Where(IsPlayerInHurtRange);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the idle range.
        /// </summary>
        public IEnumerable<Player> PlayersInIdleRange => Player.List.Where(IsPlayerInIdleRange);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the trigger range.
        /// </summary>
        public IEnumerable<Player> PlayersInTriggerRange => Player.List.Where(IsPlayerInTriggerRange);

        /// <summary>
        /// Gets the <see cref="TeslaGate"/> belonging to the <see cref="BaseTeslaGate"/>.
        /// </summary>
        /// <param name="baseTeslaGate">The <see cref="BaseTeslaGate"/> instance.</param>
        /// <returns>The corresponding <see cref="TeslaGate"/> instance.</returns>
        public static TeslaGate Get(BaseTeslaGate baseTeslaGate) => BaseTeslaGateToTeslaGate.TryGetValue(baseTeslaGate, out TeslaGate teslagate) ?
            teslagate :
            new(baseTeslaGate, Room.FindParentRoom(baseTeslaGate.gameObject));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<TeslaGate> Get(Func<TeslaGate, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Try-get a <see cref="TeslaGate"/> belonging to the <see cref="BaseTeslaGate"/>, if any.
        /// </summary>
        /// <param name="baseTeslaGate">The <see cref="BaseTeslaGate"/> instance.</param>
        /// <param name="gate">A <see cref="TeslaGate"/> or <see langword="null"/> if not found.</param>
        /// <returns>Whether or not the tesla gate was found.</returns>
        public static bool TryGet(BaseTeslaGate baseTeslaGate, out TeslaGate gate)
        {
            gate = Get(baseTeslaGate);
            return gate is not null;
        }

        /// <summary>
        /// Try-get a <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <param name="gates">A <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> which contains elements that satify the condition.</param>
        /// <returns>Whether or not at least one tesla gate was found.</returns>
        public static bool TryGet(Func<TeslaGate, bool> predicate, out IEnumerable<TeslaGate> gates)
        {
            gates = Get(predicate);
            return gates.Any();
        }

        /// <summary>
        /// Triggers the tesla gate.
        /// </summary>
        /// <param name="isInstantBurst">A value indicating whether the shock should be treated as instant burst.</param>
        public void Trigger(bool isInstantBurst = false)
        {
            if (isInstantBurst)
                Base.RpcInstantBurst();
            else
                Base.ServerSideCode();
        }

        /// <summary>
        /// Force triggers the tesla gate ignoring the delay between each burst.
        /// </summary>
        public void ForceTrigger()
        {
            Timing.RunCoroutine(Base.ServerSideWaitForAnimation());
            Base.RpcPlayAnimation();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the hurt range of a specific tesla gate.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the hurt range of the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool IsPlayerInHurtRange(Player player) => player is not null && Vector3.Distance(Position, player.Position) <= Base.sizeOfTrigger * 2.2f;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the idle range of a specific tesla gate.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the idle range of the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool IsPlayerInIdleRange(Player player) => player is not null && Base.IsInIdleRange(player.ReferenceHub);

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the trigger range of a specific tesla gate.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the trigger range of the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool IsPlayerInTriggerRange(Player player) => player is not null && Base.PlayerInRange(player.ReferenceHub);

        /// <summary>
        /// Gets a value indicating whether the tesla gate can be idle by a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> can idle the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool CanBeIdle(Player player) => player is not null && player.IsAlive && !IgnoredPlayers.Contains(player) && !IgnoredRoles.Contains(player.Role) &&
                                                !IgnoredTeams.Contains(player.Role.Team) && IsPlayerInIdleRange(player);

        /// <summary>
        /// Gets a value indicating whether the tesla gate can be triggered by a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> can trigger the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool CanBeTriggered(Player player) => player is not null && player.IsAlive && !IgnoredPlayers.Contains(player) && !IgnoredRoles.Contains(player.Role) &&
                                                     !IgnoredTeams.Contains(player.Role.Team) && IsPlayerInTriggerRange(player);
    }
}
