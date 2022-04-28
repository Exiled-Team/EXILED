// -----------------------------------------------------------------------
// <copyright file="UsingScp244.cs" company="Exiled Team">
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
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244Item"/> to add missing event handler to the <see cref="Scp244Item.ServerOnUsingCompleted"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244Item), nameof(Scp244Item.ServerOnUsingCompleted))]
    internal static class UsingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();

            // Confirmed this works thus far.
            int index = 0;
#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[Scp244Item Instance]
                new(OpCodes.Ldarg_0),

                // Load arg 0 (No param, instance of object) EStack[Scp244Item Instance, Scp244Item Instance]
                new(OpCodes.Ldarg_0),

                // Load the field within the instance, since no get; set; we can use Field. EStack[Scp244Item Instance, Scp244Item.Owner]
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244Item), nameof(Scp244Item.Owner))),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Scp244Item Instance, Player ]
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Add isAllowed = true EStack[Scp244Item Instance, Player, true]
                new(OpCodes.Ldc_I4_1),

                // Pass all 2 variables to DamageScp244 New Object, get a new object in return EStack[PickingUpScp244EventArgs Instance]
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingScp244EventArgs))[0]),

                // Copy it for later use again EStack[DamagingScp244EventArgs Instance, DamagingScp244EventArgs Instance]
                new(OpCodes.Dup),

                // Call Method on Instance EStack[DamagingScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnUsingScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below
                new(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new(OpCodes.Ret),

                // Good route of is allowed being true.
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
