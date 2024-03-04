// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Reflection;

    using API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Spawn;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.NetworkMessages;
    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event.
    /// Fix for spawning in wrong location.
    /// </summary>
    [HarmonyPatch]
    internal static class Spawning
    {
        private static MethodInfo TargetMethod()
        {
            return Method(TypeByName("PlayerRoles.FirstPersonControl.Spawnpoints.RoleSpawnpointManager").GetNestedTypes(all)[1], "<Init>b__2_0");
        }

        private static bool Prefix(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (newRole.ServerSpawnReason == RoleChangeReason.Destroyed || !Player.TryGet(hub, out Player player))
                return true;

            Vector3 oldPosition = hub.transform.position;
            float oldRotation = (prevRole as IFpcRole)?.FpcModule.MouseLook.CurrentVertical ?? 0;

            if (newRole is IFpcRole fpcRole)
            {
                if (newRole.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint))
                {
                    SpawnLocation spawnLocation = newRole.RoleTypeId.GetRandomSpawnLocation();
                    if (spawnLocation != null)
                    {
                        Log.Info($"{player.DisplayNickname} ({prevRole.RoleTypeId} - {newRole.RoleTypeId}) - Old Pos: {Room.Get(oldPosition)?.Type ?? RoomType.Unknown} ({oldPosition}) - New Pos: {Room.Get(spawnLocation.Position)?.Type ?? RoomType.Unknown} ({spawnLocation.Position})");
                        oldPosition = spawnLocation.Position;
                        oldRotation = spawnLocation.HorizontalRotation;
                    }
                }

                SpawningEventArgs ev = new(player, oldPosition, oldRotation, prevRole);

                Handlers.Player.OnSpawning(ev);

                hub.transform.position = ev.Position;
                fpcRole.FpcModule.MouseLook.CurrentHorizontal = ev.HorizontalRotation;
                hub.connectionToClient.Send(new FpcOverrideMessage(ev.Position, ev.HorizontalRotation), 0);
            }
            else
            {
                Handlers.Player.OnSpawning(new(player, oldPosition, oldRotation, prevRole));
            }

            return false;
        }
    }
}