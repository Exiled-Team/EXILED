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
    internal static class PickingUpScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueProcessing = generator.DefineLabel();

            // Tested by Yamato and Undid-Iridium
#pragma warning disable SA1118 // Parameter should not span multiple lines
            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;
            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[Scp244SearchCompletor Instance]
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // Load the field within the instance, since no get; set; we can use Field.
                new(OpCodes.Ldfld, Field(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Hub))),

                 // Using Owner call Player.Get static method with it (Reference hub) and get a Player back
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Load arg 0 (instance) again EStack[Scp244SearchCompletor Instance]
                new(OpCodes.Ldloc_0),

                // Pass all 3 variables to DamageScp244 New Object, get a new object in return EStack[PickingUpScp244EventArgs Instance]
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpScp244EventArgs))[0]),

                // Copy it for later use again EStack[PickingUpScp244EventArgs Instance, PickingUpScp244EventArgs Instance]
                new(OpCodes.Dup),

                // Call Method on Instance EStack[PickingUpScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnPickingUpScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp244EventArgs), nameof(PickingUpScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, return occurs below
                new(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
