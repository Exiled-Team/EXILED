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

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Spawnpoints;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event.
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
            if (newRole.ServerSpawnReason != RoleChangeReason.Destroyed && Player.TryGet(hub, out Player player))
            {
                Vector3 oldPosition = hub.transform.position;
                float oldRotation = (prevRole as IFpcRole)?.FpcModule.MouseLook.CurrentVertical ?? 0;

                if (newRole is IFpcRole fpcRole)
                {
                    if (newRole.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint) && fpcRole.SpawnpointHandler != null && fpcRole.SpawnpointHandler.TryGetSpawnpoint(out Vector3 position, out float horizontalRot))
                    {
                        oldPosition = position;
                        oldRotation = horizontalRot;
                    }

                    SpawningEventArgs ev = new(player, oldPosition, oldRotation, prevRole);

                    Handlers.Player.OnSpawning(ev);

                    hub.transform.position = ev.Position;
                    fpcRole.FpcModule.MouseLook.CurrentHorizontal = ev.HorizontalRotation;
                }
                else
                {
                    Handlers.Player.OnSpawning(new(player, oldPosition, oldRotation, prevRole));
                }

                return false;
            }

            return true;
        }
    }
}