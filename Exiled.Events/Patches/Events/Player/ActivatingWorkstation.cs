// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstation.cs" company="Exiled Team">
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

    using InventorySystem.Items.Firearms.Attachments;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="WorkstationController.ServerInteract"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.ServerInteract))]
    internal static class ActivatingWorkstation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ActivatingWorkstationEventArgs));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWorkstationEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWorkstation))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWorkstationEventArgs), nameof(ActivatingWorkstationEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(ActivatingWorkstationEventArgs), nameof(ActivatingWorkstationEventArgs.NewStatus))),
                new(OpCodes.Call, PropertySetter(typeof(WorkstationController), nameof(WorkstationController.NetworkStatus))),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            int moveOffset = -5;
            int moveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + moveOffset;

            newInstructions[index].MoveLabelsTo(newInstructions[moveIndex]);

            newInstructions.RemoveRange(index, 3);

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
