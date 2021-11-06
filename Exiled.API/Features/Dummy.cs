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
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A set of tools to handle npcs more easily.
    /// </summary>
    public class Dummy
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Dummy"/> on the server.
        /// </summary>
        public static readonly Dictionary<GameObject, Dummy> Dictionary = new Dictionary<GameObject, Dummy>();

        /// <summary>
        /// Gets a list of all the spawned <see cref="Dummy"/>.
        /// </summary>
        public static List<Dummy> List => Dictionary.Values.ToList();

        /// <summary>
        /// Gets the <see cref="Player"/> wrapper to easily interact with the <see cref="Dummy"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the <see cref="Dummy"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dummy"/> class.
        /// </summary>
        /// <param name="spawnPosition">A <see cref="Vector3"/> representing the spawn position of the <see cref="Dummy"/>.</param>
        /// <param name="scale">A <see cref="Vector3"/> representing the size of the <see cref="Dummy"/>.</param>
        /// <param name="role">The <see cref="RoleType"/> the <see cref="Dummy"/> will spawn as.</param>
        /// <param name="nick">The name of the <see cref="Dummy"/>.</param>
        /// <param name="hasGoodMode">A value indicating whether or not the <see cref="Dummy"/> has good mode.</param>
        public Dummy(Vector3 spawnPosition, Vector3 scale, RoleType role = RoleType.Tutorial, string nick = "NPC", bool hasGoodMode = true)
        {
            GameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var referenceHub = GameObject.GetComponent<ReferenceHub>();

            GameObject.transform.localScale = scale;
            GameObject.transform.position = spawnPosition;

            referenceHub.queryProcessor.PlayerId = -1;
            referenceHub.queryProcessor.NetworkPlayerId = -1;
            referenceHub.queryProcessor._ipAddress = "127.0.0.WAN";

            referenceHub.characterClassManager.CurClass = role;
            referenceHub.characterClassManager.GodMode = hasGoodMode;
            referenceHub.playerStats.SetHPAmount(100);

            referenceHub.nicknameSync.Network_myNickSync = nick;

            NetworkServer.Spawn(GameObject);
            Player = new Player(GameObject);

            Dictionary.Add(GameObject, this);
            PlayerManager.AddPlayer(GameObject, CustomNetworkManager.slots);
        }

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
        /// Destroys the dummy.
        /// </summary>
        /// <param name="spawnRagdoll">A value indicating whether or not it should spawn a <see cref="Ragdoll"/>.</param>
        public void Kill(bool spawnRagdoll = false)
        {
            if (spawnRagdoll)
                GameObject.GetComponent<RagdollManager>().SpawnRagdoll(GameObject.transform.position, GameObject.transform.rotation, Vector3.zero, (int)Player.Role, default, false, string.Empty, Player.Nickname, -1);

            UnSpawn();
            Player.Role = RoleType.Spectator;
            Spawn();
        }
    }
}
