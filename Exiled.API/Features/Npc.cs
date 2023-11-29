// -----------------------------------------------------------------------
// <copyright file="Npc.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#nullable enable
namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CentralAuth;
    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Components;
    using Exiled.API.Features.Roles;
    using Footprinting;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;
    using MEC;
    using Mirror;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using RelativePositioning;
    using UnityEngine;

    using Firearm = Items.Firearm;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Wrapper class for handling NPC players.
    /// </summary>
    public class Npc : Player
    {
        /// <inheritdoc cref="Player" />
        public Npc(ReferenceHub referenceHub)
            : base(referenceHub)
        {
        }

        /// <inheritdoc cref="Player" />
        public Npc(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Gets a list of Npcs.
        /// </summary>
        public static new List<Npc> List => Player.List.OfType<Npc>().ToList();

        /// <summary>
        /// Retrieves the NPC associated with the specified ReferenceHub.
        /// </summary>
        /// <param name="rHub">The ReferenceHub to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ReferenceHub, or <c>null</c> if not found.</returns>
        public static new Npc? Get(ReferenceHub rHub) => Player.Get(rHub) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the GameObject, or <c>null</c> if not found.</returns>
        public static new Npc? Get(GameObject gameObject) => Player.Get(gameObject) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The user ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the user ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(string userId) => Player.Get(userId) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified ID.
        /// </summary>
        /// <param name="id">The ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(int id) => Player.Get(id) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified ICommandSender.
        /// </summary>
        /// <param name="sender">The ICommandSender to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the ICommandSender, or <c>null</c> if not found.</returns>
        public static new Npc? Get(ICommandSender sender) => Player.Get(sender) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified Footprint.
        /// </summary>
        /// <param name="footprint">The Footprint to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the Footprint, or <c>null</c> if not found.</returns>
        public static new Npc? Get(Footprint footprint) => Player.Get(footprint) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified CommandSender.
        /// </summary>
        /// <param name="sender">The CommandSender to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the CommandSender, or <c>null</c> if not found.</returns>
        public static new Npc? Get(CommandSender sender) => Player.Get(sender) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified Collider.
        /// </summary>
        /// <param name="collider">The Collider to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the Collider, or <c>null</c> if not found.</returns>
        public static new Npc? Get(Collider collider) => Player.Get(collider) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified net ID.
        /// </summary>
        /// <param name="netId">The net ID to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the net ID, or <c>null</c> if not found.</returns>
        public static new Npc? Get(uint netId) => Player.Get(netId) as Npc;

        /// <summary>
        /// Retrieves the NPC associated with the specified NetworkConnection.
        /// </summary>
        /// <param name="conn">The NetworkConnection to retrieve the NPC for.</param>
        /// <returns>The NPC associated with the NetworkConnection, or <c>null</c> if not found.</returns>
        public static new Npc? Get(NetworkConnection conn) => Player.Get(conn) as Npc;

        /// <summary>
        /// Spawns an NPC based on the given parameters.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="role">The RoleTypeId of the NPC.</param>
        /// <param name="id">The player ID of the NPC.</param>
        /// <param name="userId">The userID of the NPC. Use "ID_Dedicated" or <c>null</c> for VSR Compliant NPCs.</param>
        /// <param name="position">The position to spawn the NPC.</param>
        /// <returns>The <see cref="Npc"/> spawned.</returns>
        public static Npc Spawn(string name, RoleTypeId role, int id = 0, string userId = "", Vector3? position = null)
        {
            GameObject newObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            Npc npc = new(newObject)
            {
                IsVerified = userId != PlayerAuthenticationManager.DedicatedId && userId != null,
                IsNPC = true,
            };
            try
            {
                npc.ReferenceHub.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.None);
            }
            catch (Exception e)
            {
                Log.Debug($"Ignore: {e}");
            }

            if (RecyclablePlayerId.FreeIds.Contains(id))
            {
                RecyclablePlayerId.FreeIds.RemoveFromQueue(id);
            }
            else if (RecyclablePlayerId._autoIncrement >= id)
            {
                RecyclablePlayerId._autoIncrement = id = RecyclablePlayerId._autoIncrement + 1;
            }

            FakeConnection fakeConnection = new(id);
            NetworkServer.AddPlayerForConnection(fakeConnection, newObject);
            try
            {
                if (userId == PlayerAuthenticationManager.DedicatedId)
                {
                    npc.ReferenceHub.authManager.SyncedUserId = userId;
                    try
                    {
                        npc.ReferenceHub.authManager.InstanceMode = ClientInstanceMode.DedicatedServer;
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"Ignore: {e}");
                    }
                }
                else
                {
                    npc.ReferenceHub.authManager.UserId = userId == string.Empty ? $"Dummy@localhost" : userId;
                }
            }
            catch (Exception e)
            {
                Log.Debug($"Ignore: {e}");
            }

            npc.ReferenceHub.nicknameSync.Network_myNickSync = name;
            Dictionary.Add(newObject, npc);

            Timing.CallDelayed(
                0.3f,
                () =>
                {
                    npc.Role.Set(role, SpawnReason.RoundStart, position is null ? RoleSpawnFlags.All : RoleSpawnFlags.AssignInventory);
                });

            if (position is not null)
                Timing.CallDelayed(0.5f, () => npc.Position = position.Value);
            return npc;
        }

        /// <summary>
        /// Destroys the NPC.
        /// </summary>
        public void Destroy()
        {
            NetworkConnectionToClient conn = ReferenceHub.connectionToClient;
            if (ReferenceHub._playerId.Value <= RecyclablePlayerId._autoIncrement)
                ReferenceHub._playerId.Destroy();
            ReferenceHub.OnDestroy();
            CustomNetworkManager.TypedSingleton.OnServerDisconnect(conn);
            Dictionary.Remove(GameObject);
            Object.Destroy(GameObject);
        }

        /// <summary>
        /// Forces the NPC to look in the specified rotation.
        /// </summary>
        /// <param name="position">The position to look at.</param>
        public void LookAt(Vector3 position)
        {
            if (Role is FpcRole fpc)
            {
                Vector3 direction = position - Position;
                Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
                LookAt(quat);
            }
        }

        /// <summary>
        /// Forces the NPC to look in the specified rotation.
        /// </summary>
        /// <param name="rotation">The rotation to look towards.</param>
        public void LookAt(Quaternion rotation)
        {
            if (Role is not FpcRole fpc)
                return;

            if (rotation.eulerAngles.z != 0f)
                rotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);

            Vector2 angles = new Vector2(-rotation.eulerAngles.x, rotation.eulerAngles.y);

            ushort hor = (ushort)Mathf.RoundToInt(Mathf.Repeat(angles.y, 360f) * (ushort.MaxValue / 360f));
            ushort vert = (ushort)Mathf.RoundToInt(Mathf.Clamp(Mathf.Repeat(angles.x + 90f, 360f) - 2f, 0f, 176f) * (ushort.MaxValue / 176f));

            fpc.FirstPersonController.FpcModule.MouseLook.ApplySyncValues(hor, vert);
        }

        /// <summary>
        /// Forces the NPC to shoot their current <see cref="Firearm"></see>.
        /// </summary>
        /// <returns><see langword="true"/> if the weapon shot request is received. Returns <see langword="false"/> otherwise, or if the player is not an <see cref="IFpcRole"/> or is not holding a <see cref="Firearm"/>.</returns>
        public bool ShootWeapon()
        {
            if (CurrentItem is not Firearm firearm)
                return false;

            if (!firearm.Base.ActionModule.ServerAuthorizeShot())
                return false;

            ShotMessage message = new ShotMessage()
            {
                ShooterCameraRotation = CameraTransform.rotation,
                ShooterPosition = new RelativePosition(Transform.position),
                ShooterWeaponSerial = CurrentItem.Serial,
                TargetNetId = 0,
                TargetPosition = default,
                TargetRotation = Quaternion.identity,
            };

            Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, firearm.Base.BaseStats.MaxDistance(), StandardHitregBase.HitregMask);

            if (hit.transform && hit.collider.TryGetComponent(out IDestructible destructible) && destructible != null)
            {
                message.TargetNetId = destructible.NetworkId;
                message.TargetPosition = new RelativePosition(hit.transform.position);
                message.TargetRotation = hit.transform.rotation;
            }
            else if (hit.transform)
            {
                message.TargetPosition = new RelativePosition(hit.transform.position);
                message.TargetRotation = hit.transform.rotation;
            }

            FirearmBasicMessagesHandler.ServerShotReceived(ReferenceHub.connectionToClient, message);
            return true;
        }

        /// <summary>
        /// Sets the NPC's current <see cref="Firearm"></see> status for Aiming Down Sights.
        /// </summary>
        /// <param name="shouldADS">Should the player be aiming down sights.</param>
        /// <returns><see langword="true"/> if the weapon Aim Down Sights request is received. Returns <see langword="false"/> otherwise, or if the player is not an <see cref="IFpcRole"/> or is not holding a <see cref="Firearm"/>.</returns>
        public bool SetAimDownSight(bool shouldADS)
        {
            if (CurrentItem is Firearm firearm)
            {
                FirearmBasicMessagesHandler.ServerRequestReceived(ReferenceHub.connectionToClient, new RequestMessage(firearm.Serial, shouldADS ? RequestType.AdsIn : RequestType.AdsOut));
                return true;
            }

            return false;
        }
    }
}