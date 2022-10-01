// -----------------------------------------------------------------------
// <copyright file="ThrowingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ThrowableNetworkHandler.ServerProcessRequest" />.
    ///     Adds the <see cref="Handlers.Player.ThrowingItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessRequest))]
    internal static class ThrowingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 4;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ThrowingItemEventArgs));

            int moveOffset = -2;

            int moveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + moveOffset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[moveIndex]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Request))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingItem))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.Item))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                    new(OpCodes.Stloc_1),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Serial))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.RequestType))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraRotation))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraPosition))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.PlayerVelocity))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage))[0]),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}