// -----------------------------------------------------------------------
// <copyright file="Ragdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DeathAnimations;

    using Enums;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;

    using Mirror;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Ragdolls;

    using PlayerStatsSystem;

    using UnityEngine;

    using BaseScp3114Ragdoll = PlayerRoles.PlayableScps.Scp3114.Scp3114Ragdoll;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to handle the ragdolls more easily.
    /// </summary>
    public class Ragdoll : GameEntity, IWrapper<BasicRagdoll>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BasicRagdoll"/>s and their corresponding <see cref="Ragdoll"/>.
        /// </summary>
        internal static readonly Dictionary<BasicRagdoll, Ragdoll> BasicRagdollToRagdoll = new(250);

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="BasicRagdoll"/>.</param>
        internal Ragdoll(BasicRagdoll ragdoll)
        {
            Base = ragdoll;
            BasicRagdollToRagdoll.Add(ragdoll, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> which contains all the <see cref="Ragdoll"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Ragdoll> List => BasicRagdollToRagdoll.Values;

        /// <summary>
        /// Gets or sets the <see cref="BasicRagdoll"/>s clean up time.
        /// </summary>
        public static int FreezeTime
        {
            get => RagdollManager.FreezeTime;
            set => RagdollManager.FreezeTime = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the clean up event can be executed.
        /// </summary>
        public bool AllowCleanUp => NetworkInfo.ExistenceTime < FreezeTime;

        /// <summary>
        /// Gets the <see cref="BasicRagdoll"/> instance of the ragdoll.
        /// </summary>
        public BasicRagdoll Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the ragdoll.
        /// </summary>
        public override GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/> of the ragdoll.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RagdollData"/>.
        /// </summary>
        public RagdollData NetworkInfo
        {
            get => Base.NetworkInfo;
            set => Base.NetworkInfo = value;
        }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandler
        {
            get => NetworkInfo.Handler;
            set => NetworkInfo = new(NetworkInfo.OwnerHub, value, NetworkInfo.RoleType, NetworkInfo.StartPosition, NetworkInfo.StartRotation, NetworkInfo.Nickname, NetworkInfo.CreationTime);
        }

        /// <summary>
        /// Gets the ragdoll's <see cref="Rigidbody"/>[].
        /// </summary>
        public Rigidbody[] SpecialRigidbodies => Base is DynamicRagdoll ragdoll ? ragdoll.LinkedRigidbodies : Array.Empty<Rigidbody>();

        /// <summary>
        /// Gets all ragdoll's <see cref="DeathAnimation"/>[].
        /// </summary>
        public DeathAnimation[] DeathAnimations => Base.AllDeathAnimations;

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has been already cleaned up.
        /// </summary>
        public bool IsFrozen => Base._frozen;

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can be cleaned up.
        /// </summary>
        public bool CanBeCleanedUp
        {
            get => IgnoredRagdolls.Contains(Base);
            set
            {
                if (!value)
                    IgnoredRagdolls.Remove(Base);
                else
                    IgnoredRagdolls.Add(Base);
            }
        }

        /// <summary>
        /// Gets the ragdoll's name.
        /// </summary>
        public string Name => Base.name;

        /// <summary>
        /// Gets or sets the ragdoll's nickname.
        /// </summary>
        public string Nickname
        {
            get => NetworkInfo.Nickname;
            set => NetworkInfo = new(NetworkInfo.OwnerHub, NetworkInfo.Handler, NetworkInfo.RoleType, NetworkInfo.StartPosition, NetworkInfo.StartRotation, value, NetworkInfo.CreationTime);
        }

        /// <summary>
        /// Gets the ragdoll's existence time.
        /// </summary>
        public float ExistenceTime => NetworkInfo.ExistenceTime;

        /// <summary>
        /// Gets or sets the owner <see cref="Player"/>. Can be <see langword="null"/> if the ragdoll does not have an owner.
        /// </summary>
        public Player Owner
        {
            get => Player.Get(NetworkInfo.OwnerHub);
            set => NetworkInfo = new(value.ReferenceHub, NetworkInfo.Handler, NetworkInfo.RoleType, NetworkInfo.StartPosition, NetworkInfo.StartRotation, NetworkInfo.Nickname, NetworkInfo.CreationTime);
        }

        /// <summary>
        /// Gets or sets the time that the ragdoll was spawned.
        /// </summary>
        public DateTime CreationTime
        {
            get => DateTime.Now - TimeSpan.FromSeconds(NetworkInfo.ExistenceTime);
            set
            {
                float creationTime = (float)(NetworkTime.time - (DateTime.Now - value).TotalSeconds);
                NetworkInfo = new RagdollData(NetworkInfo.OwnerHub, NetworkInfo.Handler, NetworkInfo.RoleType, NetworkInfo.StartPosition, NetworkInfo.StartRotation, NetworkInfo.Nickname, creationTime);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> of the ragdoll.
        /// </summary>
        public RoleTypeId Role
        {
            get => NetworkInfo.RoleType;
            set => NetworkInfo = new(NetworkInfo.OwnerHub, NetworkInfo.Handler, value, NetworkInfo.StartPosition, NetworkInfo.StartRotation, NetworkInfo.Nickname, NetworkInfo.CreationTime);
        }

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has expired and SCP-049 is unable to revive it if was not being targets.
        /// <seealso cref="Roles.Scp049Role.CanResurrect(Ragdoll)"/>
        /// </summary>
        public bool IsExpired => NetworkInfo.ExistenceTime > PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.HumanCorpseDuration;

        /// <summary>
        /// Gets or sets a value indicating whether or not this ragdoll has been consumed by an SCP-049-2.
        /// </summary>
        public bool IsConsumed
        {
            get => ZombieConsumeAbility.ConsumedRagdolls.Contains(Base);
            set
            {
                if (value && !ZombieConsumeAbility.ConsumedRagdolls.Contains(Base))
                    ZombieConsumeAbility.ConsumedRagdolls.Add(Base);
                else if (!value && ZombieConsumeAbility.ConsumedRagdolls.Contains(Base))
                    ZombieConsumeAbility.ConsumedRagdolls.Remove(Base);
            }
        }

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the ragdoll is located in.
        /// </summary>
        public Room Room => Room.FindParentRoom(GameObject);

        /// <summary>
        /// Gets the <see cref="ZoneType"/> the ragdoll is in.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets or sets the ragdoll's position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.position = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.rotation = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => Base.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);

                Base.transform.localScale = value;

                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets the ragdoll's death reason.
        /// </summary>
        public string DeathReason => DamageHandler.ServerLogsText;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="BasicRagdoll"/>'s that will be ignored by clean up event.
        /// </summary>
        internal static HashSet<BasicRagdoll> IgnoredRagdolls { get; set; } = new();

        /// <summary>
        /// Gets the last ragdoll of the player.
        /// </summary>
        /// <param name="player">The player to get the last ragdoll.</param>
        /// <returns>The Last Ragdoll.</returns>
        public static Ragdoll GetLast(Player player) => Get(player).LastOrDefault();

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if ragdoll was successfully created. Otherwise, false.</returns>
        public static bool TryCreate(RagdollData networkInfo, out Ragdoll ragdoll)
        {
            ragdoll = null;

            if (networkInfo.RoleType.GetRoleBase() is not IRagdollRole ragdollRole)
                return false;

            GameObject modelRagdoll = ragdollRole.Ragdoll.gameObject;

            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BasicRagdoll basicRagdoll))
                return false;

            basicRagdoll.NetworkInfo = networkInfo;

            ragdoll = basicRagdoll is BaseScp3114Ragdoll scp3114Ragdoll ? new Scp3114Ragdoll(scp3114Ragdoll) : new Ragdoll(basicRagdoll)
            {
                Position = networkInfo.StartPosition,
                Rotation = networkInfo.StartRotation,
            };

            return true;
        }

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static bool TryCreate(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, out Ragdoll ragdoll, Player owner = null)
            => TryCreate(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, damageHandler, roleType, default, default, name, NetworkTime.time), out ragdoll);

        /// <summary>
        /// Creates a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="ragdoll">Created ragdoll. Will be <see langword="null"/> if method retunred <see langword="false"/>.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static bool TryCreate(RoleTypeId roleType, string name, string deathReason, out Ragdoll ragdoll, Player owner = null)
            => TryCreate(roleType: roleType, name: name, damageHandler: new CustomReasonDamageHandler(deathReason), out ragdoll, owner);

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="networkInfo">The data associated with the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RagdollData networkInfo)
        {
            if (!TryCreate(networkInfo, out Ragdoll doll))
                return null;

            doll.Spawn();

            return doll;
        }

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="damageHandler">The damage handler responsible for the ragdoll's death.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RoleTypeId roleType, string name, DamageHandlerBase damageHandler, Vector3 position, Quaternion rotation, Player owner = null)
            => CreateAndSpawn(new(owner?.ReferenceHub ?? Server.Host.ReferenceHub, damageHandler, roleType, position, rotation, name, NetworkTime.time));

        /// <summary>
        /// Creates and spawns a new ragdoll.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleTypeId"/> of the ragdoll.</param>
        /// <param name="name">The name of the ragdoll.</param>
        /// <param name="deathReason">The reason the ragdoll died.</param>
        /// <param name="position">The position of the ragdoll.</param>
        /// <param name="rotation">The rotation of the ragdoll.</param>
        /// <param name="owner">The optional owner of the ragdoll.</param>
        /// <returns>The ragdoll.</returns>
        public static Ragdoll CreateAndSpawn(RoleTypeId roleType, string name, string deathReason, Vector3 position, Quaternion rotation, Player owner = null)
            => CreateAndSpawn(roleType, name, new CustomReasonDamageHandler(deathReason), position, rotation, owner);

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> belonging to the <see cref="BasicRagdoll"/>, if any.
        /// </summary>
        /// <param name="ragdoll">The <see cref="BasicRagdoll"/> to get.</param>
        /// <returns>A <see cref="Ragdoll"/> or <see langword="null"/> if not found.</returns>
        public static Ragdoll Get(BasicRagdoll ragdoll) => ragdoll == null ? null :
            BasicRagdollToRagdoll.TryGetValue(ragdoll, out Ragdoll doll) ? doll : ragdoll is BaseScp3114Ragdoll scp3114Ragdoll ? new Scp3114Ragdoll(scp3114Ragdoll) : new Ragdoll(ragdoll);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(Player player) => List.Where(rd => rd.Owner == player);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="IEnumerable{T}"/> of <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(IEnumerable<Player> players) => players.SelectMany(pl => Ragdoll.List.Where(rd => rd.Owner == pl));

        /// <summary>
        /// Destroys the ragdoll.
        /// </summary>
        public void Destroy() => Object.Destroy(GameObject);

        /// <summary>
        /// Spawns the ragdoll.
        /// </summary>
        public void Spawn() => NetworkServer.Spawn(GameObject);

        /// <summary>
        /// Un-spawns the ragdoll.
        /// </summary>
        public void UnSpawn() => NetworkServer.UnSpawn(GameObject);

        /// <summary>
        /// Returns the Ragdoll in a human-readable format.
        /// </summary>
        /// <returns>A string containing Ragdoll-related data.</returns>
        public override string ToString() => $"{Owner} ({Name}) [{DeathReason}] *{Role}* |{CreationTime}| ={IsExpired}=";
    }
}
