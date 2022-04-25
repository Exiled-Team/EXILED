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
    internal static class UsingScp244Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();

            int index = 0;

            LocalBuilder exceptionObject = generator.DeclareLocal(typeof(Exception));

            // Our Catch (Try wrapper) block
            ExceptionBlock catchBlock = new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, typeof(Exception));

            // Our Exception handling start
            ExceptionBlock exceptionStart = new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, typeof(Exception));

            // Our Exception handling end
            ExceptionBlock exceptionEnd = new ExceptionBlock(ExceptionBlockType.EndExceptionBlock);

#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(index, new[]
            {
                // Load a try wrapper at start
                new CodeInstruction(OpCodes.Nop).WithBlocks(exceptionStart),

                // Load arg 0 (No param, instance of object) EStack[Scp244Item Instance]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Load arg 0 (No param, instance of object) EStack[Scp244Item Instance, Scp244Item Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load the field within the instance, since no get; set; we can use Field. EStack[Scp244Item Instance, Scp244Item.Owner]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp244Item), nameof(Scp244Item.Owner))),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Scp244Item Instance, Player ]
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Add isAllowed = true EStack[Scp244Item Instance, Player, true]
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // Pass all 2 variables to DamageScp244 New Object, get a new object in return EStack[PickingUpScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingScp244EventArgs))[0]),

                // Copy it for later use again EStack[DamagingScp244EventArgs Instance, DamagingScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Dup),

                // Call Method on Instance EStack[DamagingScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnUsingScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below
                new CodeInstruction(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true 
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
                new CodeInstruction(OpCodes.Leave_S, normalProcessing),

                // Load generic exception
                new CodeInstruction(OpCodes.Ldloc, exceptionObject),

                // Throw generic
                new CodeInstruction(OpCodes.Throw),

                // Start our catch block
                new CodeInstruction(OpCodes.Nop).WithBlocks(catchBlock),

                // Load the exception from stack
                new CodeInstruction(OpCodes.Stloc, exceptionObject.LocalIndex),

                // Load string with format
                new CodeInstruction(OpCodes.Ldstr, "UsingScp244Patch failed because of {0}"),

                // Load exception
                new CodeInstruction(OpCodes.Ldloc, exceptionObject.LocalIndex),

                // Call format on string with object to get new string
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object) })),

                // Load error
                new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Error), new[] { typeof(string) })),

                // End exception block, continue thereafter (Do you want an immediate return?)
                new CodeInstruction(OpCodes.Nop).WithBlocks(exceptionEnd),

                new CodeInstruction(OpCodes.Nop).WithLabels(normalProcessing),
            });


            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
