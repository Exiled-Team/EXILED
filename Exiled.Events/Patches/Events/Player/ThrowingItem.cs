// -----------------------------------------------------------------------
// <copyright file="ThrowingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableNetworkHandler.ServerProcessMessages"/>.
    /// Adds the <see cref="Handlers.Player.ThrowingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessMessages))]
    internal static class ThrowingItem {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 4;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ThrowingItemEventArgs));

            int moveOffset = -2;

            int moveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + moveOffset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[moveIndex]),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemMessage), nameof(ThrowableNetworkHandler.ThrowableItemMessage.Request))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingItem))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.Item))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Base))),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemMessage), nameof(ThrowableNetworkHandler.ThrowableItemMessage.Serial))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.RequestType))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemMessage), nameof(ThrowableNetworkHandler.ThrowableItemMessage.CameraRotation))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemMessage), nameof(ThrowableNetworkHandler.ThrowableItemMessage.CameraPosition))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemMessage), nameof(ThrowableNetworkHandler.ThrowableItemMessage.PlayerVelocity))),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowableNetworkHandler.ThrowableItemMessage))[0]),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
