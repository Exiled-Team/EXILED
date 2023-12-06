// -----------------------------------------------------------------------
// <copyright file="ThrowingRequest.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableNetworkHandler.ServerProcessRequest" />.
    /// Adds the <see cref="Handlers.Player.ThrowingRequest" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ThrowingRequest))]
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessRequest))]
    internal static class ThrowingRequest
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 5;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Isinst) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ThrowingRequestEventArgs));

            int moveOffset = -2;

            int moveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + moveOffset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(referenceHub)
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[moveIndex]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ThrowableItem
                new(OpCodes.Ldloc_1),

                // Networkconnection.Request
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Request))),

                // ThrowingRequestEventArgs ev = new(Player.Get(referenceHub), ThrowableItem,Networkconnection.Request, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingRequestEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnThrowingRequest(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingRequest))),

                // Networkconnection.Serial
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.Serial))),

                // ev.RequestType
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingRequestEventArgs), nameof(ThrowingRequestEventArgs.RequestType))),

                // Networkconnection.CameraRotation
                // Networkconnection.CameraPosition
                // Networkconnection.PlayerVelocity
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraRotation))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.CameraPosition))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage), nameof(ThrowableNetworkHandler.ThrowableItemRequestMessage.PlayerVelocity))),

                // new(Networkconnection.Serial, ev.RequestType, Networkconnection.CameraRotation, Networkconnection.CameraPosition, Networkconnection.PlayerVelocity);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowableNetworkHandler.ThrowableItemRequestMessage))[0]),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}