// -----------------------------------------------------------------------
// <copyright file="Dummy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1201
namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Items;

    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A set of tools to handle NPCs more easily.
    /// </summary>
    public class Dummy
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Dummy"/> on the server.
        /// </summary>
        internal static readonly Dictionary<GameObject, Dummy> Dictionary = new Dictionary<GameObject, Dummy>();

        /// <summary>
        /// Gets a list of all the spawned <see cref="Dummy"/>.
        /// </summary>
        public static IEnumerable<Dummy> List => Dictionary.Values;

        /// <summary>
        /// Gets the <see cref="Player"/> wrapper to easily interact with the <see cref="Dummy"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the <see cref="Dummy"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets or sets the dummy's role.
        /// </summary>
        public RoleType Role
        {
            get => Player.Role;
            set
            {
                UnSpawn();
                Player.ReferenceHub.characterClassManager.CurClass = value;
                Spawn();
            }
        }

        /// <summary>
        /// Gets or sets the dummy's movement state.
        /// </summary>
        public PlayerMovementState MovementState
        {
            get => Player.MoveState;
            set
            {
                Player.ReferenceHub.animationController.MoveState = value;
                Player.ReferenceHub.animationController.RpcReceiveState((byte)value);
            }
        }

        /// <summary>
        /// Gets or sets the dummy's current item.
        /// </summary>
        public Item CurrentItem
        {
            get => Player.CurrentItem;
            set => Player.Inventory.NetworkCurItem = new InventorySystem.Items.ItemIdentifier(value.Type, 0);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the dummy is aiming with a weapon.
        /// </summary>
        public bool IsAimingDownWeapon
        {
            get => Player.IsAimingDownWeapon;
            set
            {
                if (!(CurrentItem is Firearm firearm))
                    return;

                firearm.Base.AdsModule.ServerAds = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dummy"/> class.
        /// </summary>
        /// <param name="position">A <see cref="Vector3"/> representing the spawn position of the <see cref="Dummy"/>.</param>
        /// <param name="scale">A <see cref="Vector3"/> representing the size of the <see cref="Dummy"/>.</param>
        /// <param name="role">The <see cref="RoleType"/> the <see cref="Dummy"/> will spawn as.</param>
        /// <param name="nick">The name of the <see cref="Dummy"/>.</param>
        public Dummy(Vector3 position, Vector3 scale, RoleType role = RoleType.Tutorial, string nick = "NPC")
        {
            GameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            ReferenceHub referenceHub = GameObject.GetComponent<ReferenceHub>();

            GameObject.transform.localScale = scale;
            GameObject.transform.position = position;

            referenceHub.queryProcessor.PlayerId = -1;
            referenceHub.queryProcessor.NetworkPlayerId = -1;
            referenceHub.queryProcessor._ipAddress = "127.0.0.WAN";
            referenceHub.characterClassManager.CurClass = role;
            referenceHub.playerStats.SetHPAmount(100);
            referenceHub.nicknameSync.Network_myNickSync = nick;

            NetworkServer.Spawn(GameObject);

            Player = new Player(GameObject);
            Dictionary.Add(GameObject, this);
            PlayerManager.AddPlayer(GameObject, CustomNetworkManager.slots);
        }

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to the <see cref="Features.Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Features.Player"/>.</param>
        /// <returns>The <see cref="Dummy"/> belonging to the <see cref="Features.Player"/>; otherwise <see langword="null"/> if not found.</returns>
        public static Dummy Get(Player player) => Get(player.GameObject);

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to the <see cref="UnityEngine.GameObject"/>, if any.
        /// </summary>
        /// <param name="gameObject">The dummy's <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>The <see cref="Dummy"/> belonging to the <see cref="UnityEngine.GameObject"/>; otherwise <see langword="null"/> if not found.</returns>
        public static Dummy Get(GameObject gameObject) => Dummy.List.FirstOrDefault(dummy => dummy.GameObject == gameObject);

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to the <see cref="ReferenceHub"/>, if any.
        /// </summary>
        /// <param name="referenceHub">The dummy's <see cref="ReferenceHub"/>.</param>
        /// <returns>The <see cref="Dummy"/> belonging to the <see cref="ReferenceHub"/>; otherwise <see langword="null"/> if not found.</returns>
        public static Dummy Get(ReferenceHub referenceHub) => Dummy.List.FirstOrDefault(dummy => dummy.Player.ReferenceHub == referenceHub);

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to a specific <see cref="uint">NetID</see>, if any.
        /// </summary>
        /// <param name="netId">The dummy's <see cref="NetworkIdentity.netId"/>.</param>
        /// <returns>The <see cref="Dummy"/> owning the <see cref="uint">NetID</see>; otherwise <see langword="null"></see> if not found.</returns>
        public static Dummy Get(uint netId) => Dummy.List.FirstOrDefault(dummy => dummy.Player.NetworkIdentity.netId == netId);

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to a specific <see cref="NetworkConnection"/>, if any.
        /// </summary>
        /// <param name="connection">The dummy's <see cref="NetworkConnection"/>.</param>
        /// <returns>The <see cref="Dummy"/> owning the <see cref="NetworkConnection"/>; otherwise <see langword="null"></see> if not found.</returns>
        public static Dummy Get(NetworkConnection connection) => Dummy.List.FirstOrDefault(dummy => dummy.Player.NetworkIdentity == connection.identity);

        /// <summary>
        /// Gets the <see cref="Dummy"/> belonging to a specific <see cref="NetworkIdentity"/>, if any.
        /// </summary>
        /// <param name="identity">The dummy's <see cref="NetworkIdentity"/>.</param>
        /// <returns>The <see cref="Dummy"/> owning the <see cref="NetworkIdentity"/>; otherwise <see langword="null"></see> if not found.</returns>
        public static Dummy Get(NetworkIdentity identity) => Dummy.List.FirstOrDefault(dummy => dummy.Player.NetworkIdentity == identity);

        /// <summary>
        /// Spawns the dummy if it is not spawned.
        /// </summary>
        public void Spawn() => NetworkServer.Spawn(GameObject);

        /// <summary>
        /// Un-spawns the dummy if it is spawned.
        /// </summary>
        public void UnSpawn() => NetworkServer.UnSpawn(GameObject);

        /// <summary>
        /// Destroys the dummy.
        /// </summary>
        public void Destroy()
        {
            PlayerManager.RemovePlayer(GameObject, CustomNetworkManager.slots);
            Object.Destroy(GameObject);
            Dictionary.Remove(GameObject);
        }

        /// <summary>
        /// Kills the dummy.
        /// </summary>
        /// <param name="spawnRagdoll">A value indicating whether or not the <see cref="Ragdoll"/> can be spawned.</param>
        /// <param name="damageType">The <see cref="DamageTypes.DamageType"/> to show as death cause.</param>
        public void Kill(bool spawnRagdoll = false, DamageTypes.DamageType damageType = default)
        {
            if (spawnRagdoll)
                Exiled.API.Features.Ragdoll.Spawn(Player, damageType, Player.Position, Quaternion.Euler(Player.Rotation), default, false, false);

            Role = RoleType.Spectator;
        }
    }
}
