// -----------------------------------------------------------------------
// <copyright file="InteractingLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Locker.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingLocker"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingLocker))]
    [HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
    internal static class InteractingLocker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.operand is MethodInfo methodInfo
            && methodInfo == Method(typeof(Locker), nameof(Locker.RpcPlayDenied))) + offset;

            Label openLockerLabel = newInstructions[index].labels[0];

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            Label runChecksLabel = newInstructions[index].labels[0];

            // Remove the default IsAllowed check.
            newInstructions.RemoveRange(index, 13);

            Label evLabel = generator.DefineLabel();
            Label trueLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ply);
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(runChecksLabel),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // this.__instance.Chambers[colliderId]
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Locker), nameof(Locker.Chambers))),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldelem_Ref),

                // colliderId
                new(OpCodes.Ldarg_2),

                // __instance.CheckPerms(__instance.Chambers[colliderId].RequiredPermissions, ply) || ply.serverRoles.BypassMode
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Locker), nameof(Locker.Chambers))),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldfld, Field(typeof(LockerChamber), nameof(LockerChamber.RequiredPermissions))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(Locker), nameof(Locker.CheckPerms), new[] { typeof(KeycardPermissions), typeof(ReferenceHub) })),
                new(OpCodes.Brtrue_S, trueLabel),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.serverRoles))),
                new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles.BypassMode))),
                new(OpCodes.Br_S, evLabel),
                new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(trueLabel),

                // var ev = new AddingTargetEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingLockerEventArgs))[0]).WithLabels(evLabel),

                // Handlers.Player.OnInteractingLocker(ev)
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingLocker))),

                // if (ev.IsAllowed) goto openLockerLabel
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingLockerEventArgs), nameof(InteractingLockerEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, openLockerLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
