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

    using Mirror;

    using PlayableScps;

    using PlayerStatsSystem;

    using UnityEngine;

    using static global::Ragdoll;

    using Object = UnityEngine.Object;
    using RagDoll = global::Ragdoll;

    /// <summary>
    /// A set of tools to handle the ragdolls more easily.
    /// </summary>
    public class Ragdoll
    {
        private readonly RagDoll ragdoll;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player">owner</see>.</param>
        /// <param name="handler">The player's <see cref="DamageHandlerBase"/>.</param>
        /// <param name="canBeSpawned">A value that represents whether the ragdoll can be spawned.</param>
        public Ragdoll(Player player, DamageHandlerBase handler, bool canBeSpawned = false)
        {
            GameObject modelRagdoll = player.ReferenceHub.characterClassManager.CurRole.model_ragdoll;
            if (modelRagdoll is null || !Object.Instantiate(modelRagdoll).TryGetComponent(out RagDoll ragdoll))
                return;
            ragdoll.NetworkInfo = new RagdollInfo(player.ReferenceHub, handler, modelRagdoll.transform.localPosition, modelRagdoll.transform.localRotation);
            this.ragdoll = ragdoll;
            Map.RagdollsValue.Add(this);
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
            if (modelRagdoll is null || !Object.Instantiate(modelRagdoll).TryGetComponent(out RagDoll ragdoll))
                return;
            ragdoll.NetworkInfo = ragdollInfo;
            this.ragdoll = ragdoll;
            Map.RagdollsValue.Add(this);
            if (canBeSpawned)
                Spawn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="RagDoll"/>.</param>
        internal Ragdoll(RagDoll ragdoll) => this.ragdoll = ragdoll;

        /// <summary>
        /// Gets or sets the <see cref="RagDoll"/>s clean up time.
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
        /// Gets the <see cref="RagDoll"/>.
        /// </summary>
        public RagDoll Base => ragdoll;

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the ragdoll.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets or sets the ragdoll's <see cref="RagdollInfo">NetworkInfo</see>.
        /// </summary>
        public RagdollInfo NetworkInfo
        {
            get => ragdoll.NetworkInfo;
            set => ragdoll.NetworkInfo = value;
        }

        /// <summary>
        /// Gets the ragdoll's <see cref="DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandler => NetworkInfo.Handler;

        /// <summary>
        /// Gets the ragdoll's <see cref="SpecialRigidbody"/>[].
        /// </summary>
        public SpecialRigidbody[] SpecialRigidbodies => ragdoll.SpecialRigidbodies;

        /// <summary>
        /// Gets all ragdoll's <see cref="DeathAnimation"/>[].
        /// </summary>
        public DeathAnimation[] DeathAnimations => ragdoll.AllDeathAnimations;

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll has been already cleaned up.
        /// </summary>
        public bool IsCleanedUp => ragdoll._cleanedUp;

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
        public bool IsPlayingAnimations => ragdoll._playingLocalAnims;

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll can play animations.
        /// </summary>
        public bool AllowAnimations
        {
            get => ragdoll._animationsDisabled;
            set => ragdoll._animationsDisabled = value;
        }

        /// <summary>
        /// Gets the ragdoll's name.
        /// </summary>
        public string Name => ragdoll.name;

        /// <summary>
        /// Gets the owner <see cref="Player"/>. Can be <see langword="null"/> if the ragdoll does not have an owner.
        /// </summary>
        public Player Owner => Player.Get(ragdoll.Info.OwnerHub);

        /// <summary>
        /// Gets the time that the ragdoll was spawned.
        /// </summary>
        public DateTime CreationTime => new((long)NetworkInfo.CreationTime);

        /// <summary>
        /// Gets the <see cref="RoleType"/> of the ragdoll.
        /// </summary>
        public RoleType Role => NetworkInfo.RoleType;

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll is respawnable by SCP-049.
        /// </summary>
        public bool AllowRecall => NetworkInfo.ExistenceTime > Scp049.ReviveEligibilityDuration;

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the ragdoll is located in.
        /// </summary>
        public Room Room => Map.FindParentRoom(GameObject);

        /// <summary>
        /// Gets or sets the ragdoll's position.
        /// </summary>
        public Vector3 Position
        {
            get => ragdoll.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => ragdoll.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => ragdoll.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets the ragdoll's death reason.
        /// </summary>
        public string DeathReason => DamageHandler.ServerLogsText;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> of <see cref="RagDoll"/>'s that will be ignored by clean up event.
        /// </summary>
        internal static HashSet<RagDoll> IgnoredRagdolls { get; set; } = new();

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> belonging to the <see cref="RagDoll"/>, if any.
        /// </summary>
        /// <param name="ragdoll">The <see cref="RagDoll"/> to get.</param>
        /// <returns>A <see cref="Ragdoll"/> or <see langword="null"/> if not found.</returns>
        public static Ragdoll Get(RagDoll ragdoll) => Map.Ragdolls.FirstOrDefault(rd => rd.Base == ragdoll);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(Player player) => Map.Ragdolls.Where(rd => rd.Owner == player);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/> belonging to the <see cref="IEnumerable{T}"/> of <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s to get.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ragdoll"/>.</returns>
        public static IEnumerable<Ragdoll> Get(IEnumerable<Player> players) => players.SelectMany(pl => Map.Ragdolls.Where(rd => rd.Owner == pl));

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="player">The ragdoll's <see cref="Player">owner</see>.</param>
        /// <param name="handler">The player's <see cref="DamageHandlerBase"/>.</param>
        public static void Spawn(Player player, DamageHandlerBase handler) => ServerSpawnRagdoll(player.ReferenceHub, handler);

        /// <summary>
        /// Spawns a <see cref="Ragdoll"/> on the map.
        /// </summary>
        /// <param name="ragdollInfo">The ragdoll's <see cref="RagdollInfo"/>.</param>
        public static void Spawn(RagdollInfo ragdollInfo) => ServerSpawnRagdoll(ragdollInfo.OwnerHub, ragdollInfo.Handler);

        /// <summary>
        /// Deletes the ragdoll.
        /// </summary>
        public void Delete()
        {
            Object.Destroy(GameObject);
            Map.RagdollsValue.Remove(this);
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
        public override string ToString() => $"{Owner} {Name} {DeathReason} {Role} {CreationTime} {AllowRecall}";
    }
}
