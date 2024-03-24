// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="WorkstationController.NetworkStatus" />.
    /// Adds the <see cref="Player.DeactivatingWorkstation" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.DeactivatingWorkstation))]
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.NetworkStatus), MethodType.Setter)]
    internal static class DeactivatingWorkstation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DeactivatingWorkstationEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brtrue) + offset;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (value != 2)
                    //    goto valueLabel;
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Bne_Un_S, continueLabel),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DeactivatingWorkstationEventArgs ev = new(WorkstationController, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeactivatingWorkstationEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnDeactivatingWorkstation(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDeactivatingWorkstation))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DeactivatingWorkstationEventArgs), nameof(DeactivatingWorkstationEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // value = ev.NewStatus
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DeactivatingWorkstationEventArgs), nameof(DeactivatingWorkstationEventArgs.NewStatus))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}