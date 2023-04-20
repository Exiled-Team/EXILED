﻿// -----------------------------------------------------------------------
// <copyright file="NpcBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Npcs
{
    using System.Collections.Generic;

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBase"/> class.
        /// </summary>
        /// /// <param name="scale">The Scale of the NPC.</param>
        /// <param name="roleTypeId">The RoleTypeId of the NPC.</param>
        /// <param name="name">The Nickname of the NPC.</param>
        /// <param name="badge">The Badge of the NPC.</param>
        public NpcBase(Vector3 scale, RoleTypeId roleTypeId, string name, string badge)
            : base(InstantiateReferenceHub())
        {
            LoadNpc(roleTypeId, name, badge);
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
        /// Gets or sets the badge of the NPC.
        /// </summary>
        public string Badge
        {
            get => ReferenceHub.serverRoles.FixedBadge;
            set => ReferenceHub.serverRoles.SetText(value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NPC has godmode.
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
        /// Destroys the NPC.
        /// </summary>
        public virtual void Destroy()
        {
            Dictionary.Remove(GameObject);
            NetworkServer.Destroy(GameObject);
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

        private void LoadNpc(RoleTypeId roleTypeId = RoleTypeId.CustomRole, string nickname = "npc", string badge = "NPC")
        {
            FakeConnection = new FakeConnection(Id);

            NetworkServer.AddPlayerForConnection(FakeConnection, GameObject);

            // Try Catch required to prevent errors while setting the UserId and the IpAddress of the NPC.
            try
            {
                ReferenceHub.characterClassManager._privUserId = "npc";
                ReferenceHub.queryProcessor._ipAddress = "127.0.0.WAN";
            }
            catch
            {
                // Ignore
            }

            ReferenceHub.characterClassManager.InstanceMode = ClientInstanceMode.Host;
            ReferenceHub.nicknameSync.Network_myNickSync = nickname;
            ReferenceHub.serverRoles.SetText(badge);

            ReferenceHub.roleManager.ServerSetRole(roleTypeId, RoleChangeReason.RemoteAdmin);

            SessionVariables.Add("IsNpc", true);
            Dictionary.Add(GameObject, this);
            Player.Dictionary.Add(GameObject, this);
        }
    }
}