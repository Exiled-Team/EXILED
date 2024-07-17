// -----------------------------------------------------------------------
// <copyright file="HumanSpawner.cs" company="Exiled Team">
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
    /// Patches <see cref="PlayerRoles.RoleAssign.HumanSpawner.SpawnHumans"/> to add
    /// <see cref="Handlers.Server.PreAssigningHumanRoles"/>, <see cref="Handlers.Server.AssigningHumanRoles"/> and <see cref="Handlers.Server.DeployingHumanRole"/> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.PreAssigningHumanRoles))]
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.AssigningHumanRoles))]
    [HarmonyPatch(typeof(PlayerRoles.RoleAssign.HumanSpawner), nameof(PlayerRoles.RoleAssign.HumanSpawner.SpawnHumans))]
    internal static class HumanSpawner
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(PreAssigningHumanRolesEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAssigningHumanRolesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnPreAssigningHumanRoles))),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreAssigningHumanRolesEventArgs), nameof(PreAssigningHumanRolesEventArgs.Queue))),
                new(OpCodes.Starg_S, 0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreAssigningHumanRolesEventArgs), nameof(PreAssigningHumanRolesEventArgs.QueueLength))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AssigningHumanRolesEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnAssigningHumanRoles))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AssigningHumanRolesEventArgs), nameof(AssigningHumanRolesEventArgs.Roles))),
                new(OpCodes.Stloc_1),
            });

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call);
            newInstructions.RemoveAt(index);

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldelem_I1) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeployingHumanRoleEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnDeployingHumanRole))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeployingHumanRoleEventArgs), nameof(DeployingHumanRoleEventArgs.Delegate))),
                new(OpCodes.Callvirt, Method(typeof(Action), nameof(Action.Invoke))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
