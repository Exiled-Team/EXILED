// -----------------------------------------------------------------------
// <copyright file="NpcBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Npcs
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Items;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    /// <summary>
    /// Represents a NPC.
    /// </summary>
    public class NpcBase : Player
    {
        private bool isSpawned;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBase"/> class.
        /// </summary>
        public NpcBase()
            : base(InstantiateReferenceHub())
        {
            LoadNpc();
            GameObject.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBase"/> class.
        /// </summary>
        /// /// <param name="scale">The Scale of the NPC.</param>
        /// <param name="roleTypeId">The RoleTypeId of the NPC.</param>
        /// <param name="currentItem">The Current selected item of the NPC.</param>
        /// <param name="name">The Nickname of the NPC.</param>
        /// <param name="badge">The Badge of the NPC.</param>
        /// <param name="badgeColor">The Color of the Badge.</param>
        public NpcBase(Vector3 scale, RoleTypeId roleTypeId, ItemType currentItem, string name, string badge, string badgeColor)
            : base(InstantiateReferenceHub())
        {
            LoadNpc(roleTypeId, currentItem, name, badge, badgeColor);
            GameObject.transform.localScale = scale;
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> containing all <see cref="NpcBase"/>'s on the server.
        /// </summary>
        public static new Dictionary<GameObject, NpcBase> Dictionary { get; } = new();

        /// <summary>
        /// Gets or sets the position of the NPC.
        /// </summary>
        public new Vector3 Position
        {
            get => base.Position;
            set => ReferenceHub.TryOverridePosition(value, base.Rotation);
        }

        /// <summary>
        /// Gets or sets the rotation of the NPC.
        /// </summary>
        public new Vector3 Rotation
        {
            get => base.Rotation;
            set => ReferenceHub.TryOverridePosition(base.Position, value);
        }

        /// <summary>
        /// Gets or sets the nickname of the NPC.
        /// </summary>
        public string NickName
        {
            get => ReferenceHub.nicknameSync.Network_myNickSync;
            set
            {
                ReferenceHub.nicknameSync.Network_myNickSync = value;
                Respawn();
            }
        }

        /// <summary>
        /// Gets or sets the Color of the badge of the NPC.
        /// </summary>
        public string BadgeColor
        {
            get => ReferenceHub.serverRoles.Network_myColor;
            set => ReferenceHub.serverRoles.Network_myColor = value;
        }

        /// <summary>
        /// Gets or sets the badge of the NPC.
        /// </summary>
        public string Badge
        {
            get => ReferenceHub.serverRoles.Network_myText;
            set => ReferenceHub.serverRoles.Network_myText = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NPC has GodMode.
        /// </summary>
        public bool GodModeEnabled
        {
            get => ReferenceHub.characterClassManager.GodMode;
            set => ReferenceHub.characterClassManager.GodMode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NPC is spawned.
        /// </summary>
        public bool IsSpawned
        {
            get => isSpawned;
            set
            {
                if (isSpawned == value)
                    return;

                isSpawned = value;
                if (value)
                {
                    NetworkServer.Spawn(GameObject);
                    return;
                }

                NetworkServer.UnSpawn(GameObject);
            }
        }

        private FakeConnection FakeConnection { get; set; }

        /// <summary>
        /// Gets a NPC by its nickname.
        /// </summary>
        /// <param name="nickname">The nickname of the NPC.</param>
        /// <param name="npc">The NPC parameter.</param>
        /// <returns>The NPC with the specified nickname.</returns>
        public static bool Get(string nickname, out NpcBase npc)
        {
            foreach (var npcBase in Dictionary.Values.Where(npcBase => npcBase.NickName == nickname))
            {
                npc = npcBase;
                return true;
            }

            npc = null;
            return false;
        }

        /// <summary>
        /// Gets a NPC inside the <see cref="Dictionary"/>.
        /// </summary>
        /// <param name="npc">The NPC to get.</param>
        /// <returns>The specified NPC.</returns>
        public static bool Get(NpcBase npc)
        {
            return Dictionary.TryGetValue(npc.GameObject, out npc);
        }

        /// <summary>
        /// Gets a NPC by a specified <see cref="ReferenceHub"/>.
        /// </summary>
        /// <param name="referenceHub">The ReferenceHub of the NPC.</param>
        /// <param name="npc">The NPC parameter.</param>
        /// <returns>The NPC with the specified ReferenceHub.</returns>
        public static bool Get(ReferenceHub referenceHub, out NpcBase npc)
        {
            foreach (var npcBase in Dictionary.Values.Where(npcBase => npcBase.ReferenceHub == referenceHub))
            {
                npc = npcBase;
                return true;
            }

            npc = null;
            return false;
        }

        /// <summary>
        /// Gets a NPC by a specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The GameObject of the NPC.</param>
        /// <param name="npc">The NPC parameter.</param>
        /// <returns>The NPC with the specified GameObject.</returns>
        public static bool Get(GameObject gameObject, out NpcBase npc)
        {
            return Dictionary.TryGetValue(gameObject, out npc);
        }

        /// <summary>
        /// Gets a NPC by a specified ID.
        /// </summary>
        /// <param name="id">The ID of the NPC.</param>
        /// <param name="npc">The NPC parameter.</param>
        /// <returns>The NPC with the specified ID.</returns>
        public static bool Get(int id, out NpcBase npc)
        {
            foreach (var npcBase in Dictionary.Values.Where(npcBase => npcBase.Id == id))
            {
                npc = npcBase;
                return true;
            }

            npc = null;
            return false;
        }

        /// <summary>
        /// Destroys the NPC.
        /// </summary>
        public void Destroy()
        {
            Dictionary.Remove(GameObject);
            NetworkServer.Destroy(GameObject);
            FakeConnection.Disconnect();
        }

        /// <summary>
        /// Respawns the NPC.
        /// </summary>
        public void Respawn()
        {
            if (!IsSpawned)
                return;

            IsSpawned = false;
            IsSpawned = true;
        }

        private static ReferenceHub InstantiateReferenceHub()
        {
            var gameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            return gameObject.GetComponent<ReferenceHub>();
        }

        private void LoadNpc(RoleTypeId roleTypeId = RoleTypeId.CustomRole, ItemType currentItem = ItemType.None, string nickname = "npc", string badge = "NPC", string badgeColor = "yellow")
        {
            FakeConnection = new FakeConnection(Id);

            NetworkServer.AddPlayerForConnection(FakeConnection, GameObject);

            // Try Catch required to prevent errors while setting the UserId and the IpAddress of the NPC.
            try
            {
                ReferenceHub.characterClassManager.UserId = $"npc{Id}@server";
            }
            catch
            {
                // Ignore
            }

            ReferenceHub.characterClassManager.InstanceMode = ClientInstanceMode.Host;
            ReferenceHub.nicknameSync.Network_myNickSync = nickname;
            ReferenceHub.serverRoles.Network_myColor = badgeColor;
            ReferenceHub.serverRoles.Network_myText = badge;

            ReferenceHub.roleManager.ServerSetRole(roleTypeId, RoleChangeReason.RemoteAdmin);

            CurrentItem = currentItem == ItemType.None ? null : Item.Create(currentItem);

            SessionVariables.Add("IsNpc", true);
            Dictionary.Add(GameObject, this);
            Player.Dictionary.Add(GameObject, this);
        }
    }
}
