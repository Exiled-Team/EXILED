// -----------------------------------------------------------------------
// <copyright file="Npc.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Represents an NPC.
    /// </summary>
    public class Npc
    {
        private bool isSpawned;

        /// <summary>
        /// Initializes a new instance of the <see cref="Npc"/> class.
        /// </summary>
        /// <param name="roleType">The role of the NPC.</param>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="scale">The size of the NPC.</param>
        /// <param name="triggerScps">Whether the NPC should trigger SCPs.</param>
        public Npc(RoleType roleType, string name, Vector3 scale, bool triggerScps = false)
        {
            GameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            Dictionary.Add(GameObject, this);

            ReferenceHub = GameObject.GetComponent<ReferenceHub>();
            ReferenceHub.characterClassManager.CurClass = roleType;
            ReferenceHub.playerStats.StatModules[0].CurValue = 100;
            ReferenceHub.nicknameSync.Network_myNickSync = name;
            ReferenceHub.queryProcessor._ipAddress = "127.0.0.WAN";

            TriggerScps = triggerScps;

            ReferenceHub.characterClassManager.IsVerified = true;

            GameObject.transform.localScale = scale;

            PlayerManager.AddPlayer(GameObject, CustomNetworkManager.slots);

            Player = new Player(GameObject);
            Player.SessionVariables.Add("IsNPC", true);
            Player.Dictionary.Add(GameObject, Player);
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> containing all <see cref="Npc"/>s on the server.
        /// </summary>
        public static Dictionary<GameObject, Npc> Dictionary { get; } = new();

        /// <summary>
        /// Gets a list of all the <see cref="Npc"/>s on the server.
        /// </summary>
        public static IEnumerable<Npc> List => Dictionary.Values;

        /// <summary>
        /// Gets the attached <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets the attached ReferenceHub component.
        /// </summary>
        public ReferenceHub ReferenceHub { get; }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> to represent the NPC.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the NPC should trigger SCPs.
        /// </summary>
        public bool TriggerScps { get; set; }

        /// <summary>
        /// Gets or sets the NPC's position.
        /// </summary>
        public Vector3 Position
        {
            get => Player.Position;
            set => ReferenceHub.playerMovementSync.OverridePosition(value, null, true);
        }

        /// <summary>
        /// Gets or sets the NPC's held item.
        /// </summary>
        public ItemType HeldItem
        {
            get => ReferenceHub.inventory.CurItem.TypeId;
            set => ReferenceHub.inventory.NetworkCurItem = new InventorySystem.Items.ItemIdentifier(value, 0);
        }

        /// <summary>
        /// Gets or sets the NPC's name.
        /// </summary>
        public string Name
        {
            get => ReferenceHub.nicknameSync.Network_myNickSync;
            set
            {
                ReferenceHub.nicknameSync.Network_myNickSync = value;
                Respawn();
            }
        }

        /// <summary>
        /// Gets or sets the NPC's role.
        /// </summary>
        public RoleType Role
        {
            get => Player.Role.Type;
            set
            {
                Player.SetRole(value, SpawnReason.ForceClass, true);
                Respawn();
            }
        }

        /// <summary>
        /// Gets or sets the NPC's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => Player.Scale;
            set => Player.Scale = value;
        }

        /// <summary>
        /// Gets the <see cref="Npc"/> belonging to the <see cref="UnityEngine.GameObject"/>, if any.
        /// </summary>
        /// <param name="gameObject">The player's <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>A <see cref="Npc"/> or <see langword="null"/> if not found.</returns>
        public static Npc Get(GameObject gameObject) => Dictionary.ContainsKey(gameObject) ? Dictionary[gameObject] : null;

        /// <summary>
        /// Gets the <see cref="Npc"/> belonging to the <see cref="Player"/>, if any.
        /// </summary>
        /// <param name="player">The <see cref="Player"/>.</param>
        /// <returns>A <see cref="Npc"/> or <see langword="null"/> if not found.</returns>
        public static Npc Get(Player player) => Dictionary.ContainsKey(player.GameObject) ? Dictionary[player.GameObject] : null;

        /// <summary>
        /// Spawns the NPC.
        /// </summary>
        public void Spawn()
        {
            if (isSpawned)
                return;

            NetworkServer.Spawn(GameObject);
            isSpawned = true;
        }

        /// <summary>
        /// Despawns the NPC.
        /// </summary>
        public void Despawn()
        {
            if (!isSpawned)
                return;

            NetworkServer.UnSpawn(GameObject);
            isSpawned = false;
        }

        /// <summary>
        /// Destroys the NPC.
        /// </summary>
        public void Destroy()
        {
            Dictionary.Remove(GameObject);
            PlayerManager.RemovePlayer(GameObject, CustomNetworkManager.slots);
            NetworkServer.Destroy(GameObject);
        }

        /// <inheritdoc />
        public override string ToString() => Player.ToString();

        private void Respawn()
        {
            Despawn();
            Spawn();
        }
    }
}
