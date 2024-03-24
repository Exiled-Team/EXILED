// -----------------------------------------------------------------------
// <copyright file="ScpSpawner.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerRoles.RoleAssign.ScpSpawner.SpawnScps"/> to add
    /// <see cref="Handlers.Server.PreAssigningScpRoles"/>, <see cref="Handlers.Server.AssigningScpRoles"/> and <see cref="Handlers.Server.DeployingScpRole"/> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.PreAssigningScpRoles))]
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.AssigningScpRoles))]
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.DeployingScpRole))]
    [HarmonyPatch(typeof(PlayerRoles.RoleAssign.ScpSpawner), nameof(PlayerRoles.RoleAssign.ScpSpawner.SpawnScps))]
    internal static class ScpSpawner
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AssigningScpRolesEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAssigningScpRolesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnPreAssigningScpRoles))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreAssigningScpRolesEventArgs), nameof(PreAssigningScpRolesEventArgs.Amount))),
                new(OpCodes.Starg_S, 0),
            });

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldsfld, Field(typeof(PlayerRoles.RoleAssign.ScpSpawner), nameof(PlayerRoles.RoleAssign.ScpSpawner.EnqueuedScps))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AssigningScpRolesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnAssigningScpRoles))),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AssigningScpRolesEventArgs), nameof(AssigningScpRolesEventArgs.ChosenPlayers))),
                new(OpCodes.Stloc_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AssigningScpRolesEventArgs), nameof(AssigningScpRolesEventArgs.Roles))),
                new(OpCodes.Stsfld, Field(typeof(PlayerRoles.RoleAssign.ScpSpawner), nameof(PlayerRoles.RoleAssign.ScpSpawner.EnqueuedScps))),
            });

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_0);
            newInstructions.RemoveRange(index, 4);

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldsfld);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldsfld, Field(typeof(PlayerRoles.RoleAssign.ScpSpawner), nameof(PlayerRoles.RoleAssign.ScpSpawner.EnqueuedScps))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeployingScpRoleEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnDeployingScpRole))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeployingScpRoleEventArgs), nameof(DeployingScpRoleEventArgs.Delegate))),
                new(OpCodes.Callvirt, Method(typeof(Action), nameof(Action.Invoke))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}