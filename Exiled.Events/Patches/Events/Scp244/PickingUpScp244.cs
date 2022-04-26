// -----------------------------------------------------------------------
// <copyright file="PickingUpScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244SearchCompletor"/> to add missing event handler to the <see cref="Scp244SearchCompletor"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Complete))]
    internal static class Scp244SearchCompletorPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();

            Label isInst = generator.DefineLabel();
            Label isNotInst = generator.DefineLabel();
            Label cont = generator.DefineLabel();

#pragma warning disable SA1118 // Parameter should not span multiple lines

            newInstructions.InsertRange(newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brtrue_S), new[]
                        {
                 new CodeInstruction(OpCodes.Ldstr, "Start before the brtrue"),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                        });

            newInstructions.InsertRange(newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret), new[]
            {
                 new CodeInstruction(OpCodes.Ldstr, "Return false route"),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                 //new CodeInstruction(OpCodes.Ldstr, "Start of it all Scp244SearchCompletorPatch, isinst {0}"),
                 //new CodeInstruction(OpCodes.Ldarg_0),
                 //new CodeInstruction(OpCodes.Ldfld, Field(typeof(SearchCompletor), nameof(SearchCompletor.TargetPickup))),
                 //new CodeInstruction(OpCodes.Isinst, typeof(Scp244DeployablePickup)),
                 //new CodeInstruction(OpCodes.Brtrue, isInst),
                 //new CodeInstruction(OpCodes.Ldstr, "It was not equal to Scp244DeployablePickup"),
                 //new CodeInstruction(OpCodes.Br, cont),
                 //new CodeInstruction(OpCodes.Nop).WithLabels(isInst),
                 //new CodeInstruction(OpCodes.Ldstr, "It was equal to Scp244DeployablePickup"),

                 //new CodeInstruction(OpCodes.Nop).WithLabels(cont),
                 //new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(string) })),
                 //new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
            });

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldstr, "Scp244SearchCompletor we were allowed"),
                new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                // Load arg 0 (No param, instance of object) EStack[Scp244SearchCompletor Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load the field within the instance, since no get; set; we can use Field. 
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Hub))),

                 // Using Owner call Player.Get static method with it (Reference hub) and get a Player back
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Load arg 0 (instance) again EStack[Scp244SearchCompletor Instance]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Pass all 3 variables to DamageScp244 New Object, get a new object in return EStack[PickingUpScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpScp244EventArgs))[0]),

                // Copy it for later use again EStack[PickingUpScp244EventArgs Instance, PickingUpScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Dup),

                // Call Method on Instance EStack[PickingUpScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnPickingUpScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp244EventArgs), nameof(PickingUpScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below
                new CodeInstruction(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true 
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });
            newInstructions.InsertRange(newInstructions.Count, new[]
           {
                       new CodeInstruction(OpCodes.Ldstr, "Scp244SearchCompletor at the end.."),
                new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
            });
            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }


            Log.Info($" Index {index} ");

            int count = 0;
            int il_pos = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count} and {instr.operand} and {il_pos} and {instr.opcode.OperandType}");
                il_pos += instr.opcode.Size;
                count++;
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
