// -----------------------------------------------------------------------
// <copyright file="DroppingCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
#pragma warning disable SA1313

    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330NetworkHandler.ServerSelectMessageReceived"/> method to add the <see cref="Handlers.Scp330.DroppingUpScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal static class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();
            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DroppingUpScp330EventArgs));

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory)))) + offset;

            LocalBuilder exceptionObject = generator.DeclareLocal(typeof(Exception));

            // Our Catch (Try wrapper) block
            ExceptionBlock catchBlock = new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, typeof(Exception));

            // Our Exception handling start
            ExceptionBlock exceptionStart = new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, typeof(Exception));

            // Our Exception handling end
            ExceptionBlock exceptionEnd = new ExceptionBlock(ExceptionBlockType.EndExceptionBlock);

            /*
             * PickingUpScp244EventArgs ev = new(Player.Get(__instance.Hub), scp244DeployablePickup);
                Handlers.Scp244.OnPickingUpScp244(ev);
                if (!ev.IsAllowed)
                {
                    return false;
                }
            */
#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(index, new[]
            {
                // Load a try wrapper at start
                new CodeInstruction(OpCodes.Nop).WithBlocks(exceptionStart),

                // Load arg 0 (No param, instance of object) EStack[Referencehub Instance]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Player]
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Load arg 0 (No param, instance of object) EStack[Player Instance, Scp330Bag Instance]
                new CodeInstruction(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance]
                new CodeInstruction(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, SelectScp330Message Msg]
                new CodeInstruction(OpCodes.Ldarg_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, CandyID]
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(SelectScp330Message), nameof(SelectScp330Message.CandyID))),

                // EStack[Player Instance, Scp330Bag Instance, CandyKindID]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Scp330Bag), nameof(Scp330Bag.TryRemove))),

                // Pass all 2 variables to DamageScp244 New Object, get a new object in return EStack[DroppingUpScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingUpScp330EventArgs))[0]),

                new CodeInstruction(OpCodes.Stloc, eventHandler.LocalIndex),

                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call Method on Instance EStack[DamagingScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnDroppingUpScp330))),

                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DroppingUpScp330EventArgs), nameof(DroppingUpScp330EventArgs.IsAllowed))),

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

            int jumpOverOffset = 1;
            int jumpOverIndex = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner)))) + jumpOverOffset;

            int skipOverOffset = 3;
            int skipOverIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.TryRemove)))) + skipOverOffset;

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            Log.Info($" Index {index} jumpOverIndex {jumpOverIndex} skipOverIndex {skipOverIndex}");

            int count = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count}");
                count++;
            }
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
