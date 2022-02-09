// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingDoor"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingDoorEventArgs));
            LocalBuilder bypassDenied = generator.DeclareLocal(typeof(bool));
            LocalBuilder allowInteracting = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x02 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x03 = generator.DeclareLocal(typeof(bool));

            Label jcc = generator.DefineLabel();
            Label jne = generator.DefineLabel();
            Label je = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label jnc = generator.DefineLabel();
            Label jns = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, bypassDenied.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, allowInteracting.LocalIndex),
            });

            offset = -2;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_2) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, bypassDenied.LocalIndex),
            });

            index += 5;
            newInstructions.RemoveRange(index, 5);

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Stloc_S, allowInteracting.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, allowInteracting.LocalIndex),
            });

            offset = -3;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_7) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, bypassDenied.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, jcc),
            });

            offset = -2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
            });

            index += 3;
            newInstructions.RemoveRange(index, 10);

            offset = -2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_2) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
            });

            index += 3;
            newInstructions.RemoveRange(index, 8);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jcc),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, je),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldloc_S, allowInteracting.LocalIndex).WithLabels(je),
                new CodeInstruction(OpCodes.Brtrue_S, jne),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(jne),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x01.LocalIndex).WithLabels(jmp),
                new CodeInstruction(OpCodes.Brfalse_S, jnc),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(DoorVariant), nameof(DoorVariant.TargetState))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(DoorVariant), nameof(DoorVariant._triggerPlayer))),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldloc_S, bypassDenied.LocalIndex).WithLabels(jnc),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, jns),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(DoorVariant), nameof(DoorVariant.LockBypassDenied))),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldloc_S, bypassDenied.LocalIndex).WithLabels(jns),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(DoorVariant), nameof(DoorVariant.PermissionsDenied))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(DoorEvents), nameof(DoorEvents.TriggerAction))),
                new CodeInstruction(OpCodes.Ret).WithLabels(ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
