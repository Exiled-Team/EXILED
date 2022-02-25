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

    using MEC;

    using UnityEngine;

    using BaseTeslaGate = global::TeslaGate;

    /// <summary>
    /// The in-game tesla gate.
    /// </summary>
    public class TeslaGate
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="TeslaGate"/> on the map.
        /// </summary>
        internal static readonly List<TeslaGate> TeslasValue = new List<TeslaGate>(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="TeslaGate"/> class.
        /// </summary>
        /// <param name="baseTeslaGate">The <see cref="BaseTeslaGate"/> instance.</param>
        internal TeslaGate(BaseTeslaGate baseTeslaGate) => Base = baseTeslaGate;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> which contains all the <see cref="TeslaGate"/> instances.
        /// </summary>
        public static IEnumerable<TeslaGate> List => TeslasValue;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Player"/> which contains all the players ignored by tesla gates.
        /// </summary>
        public static HashSet<Player> IgnoredPlayers { get; set; } = new HashSet<Player>();

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="RoleType"/> which contains all the roles ignored by tesla gates.
        /// </summary>
        public static List<RoleType> IgnoredRoles { get; set; } = new List<RoleType>();

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="Team"/> which contains all the teams ignored by tesla gates.
        /// </summary>
        public static List<Team> IgnoredTeams { get; set; } = new List<Team>();

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
        public Vector3 Position => Base.localPosition;

        /// <summary>
        /// Gets the tesla gate's rotation.
        /// </summary>
        public Quaternion Rotation => Quaternion.Euler(Base.localRotation);

        /// <summary>
        /// Gets the tesla gate's <see cref="Features.Room"/> which is located in.
        /// </summary>
        public Room Room => Map.FindParentRoom(GameObject);

        /// <summary>
        /// Gets a value indicating whether the tesla gate' shock burst is in progess.
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
        /// Gets or sets a value indicating whether the tesla gate's next burst should be treated as instant burst.
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
        public List<GameObject> TantrumsToDestroy => Base.TantrumsToBeDestroyed;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the hurt range.
        /// </summary>
        public IEnumerable<Player> PlayersInHurtRange => Player.List.Where(player => Base.PlayerInHurtRange(player.GameObject));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the idle range.
        /// </summary>
        public IEnumerable<Player> PlayersInIdleRange => Player.List.Where(player => Base.PlayerInIdleRange(player.ReferenceHub));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all the players inside the trigger range.
        /// </summary>
        public IEnumerable<Player> PlayersInTriggerRange => Player.List.Where(player => Base.PlayerInRange(player.ReferenceHub));

        /// <summary>
        /// Gets the <see cref="TeslaGate"/> belonging to the <see cref="BaseTeslaGate"/>.
        /// </summary>
        /// <param name="baseTeslaGate">The <see cref="BaseTeslaGate"/> instance.</param>
        /// <returns>The corresponding <see cref="TeslaGate"/> instance.</returns>
        public static TeslaGate Get(BaseTeslaGate baseTeslaGate) => List.FirstOrDefault(teslaGate => teslaGate.Base == baseTeslaGate);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="TeslaGate"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<TeslaGate> Get(Func<TeslaGate, bool> predicate) => List.Where(predicate);

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
        public bool PlayerInHurtRange(Player player) => Base.PlayerInHurtRange(player.GameObject);

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the idle range of a specific tesla gate.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the idle range of the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool PlayerInIdleRange(Player player) => Base.PlayerInIdleRange(player.ReferenceHub);

        /// <summary>
        /// Gets a value indicating whether the <see cref="Player"/> is in the trigger range of a specific tesla gate.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> is in the trigger range of the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool PlayerInTriggerRange(Player player) => Base.PlayerInRange(player.ReferenceHub);

        /// <summary>
        /// Gets a value indicating whether the tesla gate can be idle by a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> can idle the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool CanBeIdle(Player player) => player.IsAlive && !IgnoredPlayers.Contains(player) && !IgnoredRoles.Contains(player.Role) &&
                                                     !IgnoredTeams.Contains(player.Role.Team) && PlayerInIdleRange(player);

        /// <summary>
        /// Gets a value indicating whether the tesla gate can be triggered by a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the given <see cref="Player"/> can trigger the tesla gate; otherwise, <see langword="false"/>.</returns>
        public bool CanBeTriggered(Player player) => !IgnoredPlayers.Contains(player) && !IgnoredRoles.Contains(player.Role) &&
                                                     !IgnoredTeams.Contains(player.Role.Team) && PlayerInTriggerRange(player);
    }
}
