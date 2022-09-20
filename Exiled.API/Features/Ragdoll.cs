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
    using Exiled.API.Enums;
    using Mirror;
    using PlayableScps;
    using PlayerStatsSystem;
    using UnityEngine;

    using static global::Ragdoll;

    using BaseRagdoll = global::Ragdoll;
    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to handle the ragdolls more easily.
    /// </summary>
    public class Ragdoll
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BaseRagdoll"/>s and their corresponding <see cref="Ragdoll"/>.
        /// </summary>
        internal static readonly Dictionary<BaseRagdoll, Ragdoll> BaseRagdollToRagdoll = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player">owner</see>.</param>
        /// <param name="handler">The player's <see cref="DamageHandlerBase"/>.</param>
        /// <param name="canBeSpawned">A value that represents whether the ragdoll can be spawned.</param>
        public Ragdoll(Player player, DamageHandlerBase handler, bool canBeSpawned = false)
        {
            GameObject modelRagdoll = player.ReferenceHub.characterClassManager.CurRole.model_ragdoll;
            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BaseRagdoll ragdoll))
                return;
            ragdoll.NetworkInfo = new RagdollInfo(player.ReferenceHub, handler, modelRagdoll.transform.localPosition, modelRagdoll.transform.localRotation);
            Base = ragdoll;
            BaseRagdollToRagdoll.Add(ragdoll, this);
            if (canBeSpawned)
                Spawn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdollInfo">The ragdoll's <see cref="RagdollInfo"/>.</param>
        /// <param name="canBeSpawned">A value that represents whether the ragdoll can be spawned.</param>
        public Ragdoll(RagdollInfo ragdollInfo, bool canBeSpawned = false)
        {
            GameObject modelRagdoll = CharacterClassManager._staticClasses.SafeGet(ragdollInfo.RoleType).model_ragdoll;
            if (modelRagdoll == null || !Object.Instantiate(modelRagdoll).TryGetComponent(out BaseRagdoll ragdoll))
                return;
            ragdoll.NetworkInfo = ragdollInfo;
            Base = ragdoll;
            BaseRagdollToRagdoll.Add(ragdoll, this);
            if (canBeSpawned)
                Spawn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="BaseRagdoll"/>.</param>
        internal Ragdoll(BaseRagdoll ragdoll)
        {
            Base = ragdoll;
            BaseRagdollToRagdoll.Add(ragdoll, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> which contains all the <see cref="Ragdoll"/> instances.
        /// </summary>
        public static IEnumerable<Ragdoll> List => BaseRagdollToRagdoll.Values;

        /// <summary>
        /// Gets or sets the <see cref="BaseRagdoll"/>s clean up time.
        /// </summary>
        public static int CleanUpTime
        {
            get => _cleanupTime;
            set => _cleanupTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the clean up event can be executed.
        /// </summary>
        public static bool AllowCleanUp
        {
            get => _cleanupEventSet;
            set => _cleanupEventSet = value;
        }

        /// <summary>
        /// Gets the base-game <see cref="BaseRagdoll"/> for this door.
        /// </summary>
        public BaseRagdoll Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the ragdoll.
        /// </summary>
        public GameObject GameObject
        {
            get => Base.gameObject;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/> of the ragdoll.
        /// </summary>
        public Transform Transform
        {
            get => Base.transform;
        }

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RagdollInfo">NetworkInfo</see>.
        /// </summary>
        public RagdollInfo NetworkInfo
        {
            get => Base.NetworkInfo;
            set => Base.NetworkInfo = value;
        }

        /// <summary>
        /// Gets the ragdoll's <see cref="DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandler
        {
            get => NetworkInfo.Handler;
        }

        /// <summary>
        /// Gets the ragdoll's <see cref="SpecialRigidbody"/>[].
        /// </summary>
        public SpecialRigidbody[] SpecialRigidbodies
        {
            get => Base.SpecialRigidbodies;
        }

        /// <summary>
        /// Gets all ragdoll's <see cref="DeathAnimation"/>[].
        /// </summary>
        public DeathAnimation[] DeathAnimations
        {
            get => Base.AllDeathAnimations;
        }

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has been already cleaned up.
        /// </summary>
        public bool IsCleanedUp
        {
            get => Base._cleanedUp;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can be cleaned up.
        /// </summary>
        public bool CanBeCleanedUp
        {
            get => IgnoredRagdolls.Contains(Base);
            set
            {
                if (!value)
                {
                    IgnoredRagdolls.Remove(Base);
                }
                else
                {
                    IgnoredRagdolls.Add(Base);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll is currently playing animations.
        /// </summary>
        public bool IsPlayingAnimations
        {
            get => Base._playingLocalAnims;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can play animations.
        /// </summary>
        public bool AllowAnimations
        {
            get => Base._animationsDisabled;
            set => Base._animationsDisabled = value;
        }

        /// <summary>
        /// Gets the ragdoll's name.
        /// </summary>
        public string Name
        {
            get => Base.name;
        }

        /// <summary>
        /// Gets the owner <see cref="Player"/>. Can be <see langword="null"/> if the ragdoll does not have an owner.
        /// </summary>
        public Player Owner
        {
            get => Player.Get(Base.Info.OwnerHub);
        }

        /// <summary>
        /// Gets the time that the ragdoll was spawned.
        /// </summary>
        public DateTime CreationTime
        {
            get => new((long)NetworkInfo.CreationTime);
        }

        /// <summary>
        /// Gets the <see cref="RoleType"/> of the ragdoll.
        /// </summary>
        public RoleType Role
        {
            get => NetworkInfo.RoleType;
        }

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll is respawnable by SCP-049.
        /// </summary>
        public bool AllowRecall
        {
            get => NetworkInfo.ExistenceTime > Scp049.ReviveEligibilityDuration;
        }

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the ragdoll is located in.
        /// </summary>
        public Room Room
        {
            get => Map.FindParentRoom(GameObject);
        }

        /// <summary>
        /// Gets the <see cref="ZoneType"/> the ragdoll is in.
        /// </summary>
        public ZoneType Zone
        {
            get => Room.Zone;
        }

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
        public string DeathReason
        {
            get => DamageHandler.ServerLogsText;
        }

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="BaseRagdoll"/>'s that will be ignored by clean up event.
        /// </summary>
        internal static HashSet<BaseRagdoll> IgnoredRagdolls { get; set; } = new();

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> belonging to the <see cref="BaseRagdoll"/>, if any.
        /// </summary>
        /// <param name="ragdoll">The <see cref="BaseRagdoll"/> to get.</param>
        /// <returns>A <see cref="Ragdoll"/> or <see langword="null"/> if not found.</returns>
        public static Ragdoll Get(BaseRagdoll ragdoll) => List.FirstOrDefault(rd => rd.Base == ragdoll);

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
        public static IEnumerable<Ragdoll> Get(IEnumerable<Player> players) => players.SelectMany(pl => List.Where(rd => rd.Owner == pl));

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player">owner</see>.</param>
        /// <param name="handler">The player's <see cref="DamageHandlerBase"/>.</param>
        [Obsolete("Use Spawn(Player, Exiled.API.Features.DamageHandlers.DamageHandlerBase) instead.", true)]
        public static void Spawn(Player player, DamageHandlerBase handler) => _ = new Ragdoll(player, handler, true);

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="ragdollInfo">The ragdoll's <see cref="RagdollInfo"/>.</param>
        [Obsolete("Use Spawn(Player, Exiled.API.Features.DamageHandlers.DamageHandlerBase) instead.", true)]
        public static void Spawn(RagdollInfo ragdollInfo) => _ = new Ragdoll(ragdollInfo, true);

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player"/> owner.</param>
        /// <param name="handler">The ragdoll's <see cref="DamageHandlerBase"/>.</param>
        /// <returns>The created <see cref="Ragdoll"/>.</returns>
        public static Ragdoll Spawn(Player player, DamageHandlers.DamageHandlerBase handler) => new(player, handler, true);

        /// <summary>
        /// Deletes the ragdoll.
        /// </summary>
        public void Delete()
        {
            BaseRagdollToRagdoll.Remove(Base);
            Object.Destroy(GameObject);
        }

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
        public override string ToString() => $"{Owner} ({Name}) [{DeathReason}] *{Role}* |{CreationTime}| ={AllowRecall}=";
    }
}