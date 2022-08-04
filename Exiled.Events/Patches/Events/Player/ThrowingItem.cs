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

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using NorthwoodLib.Pools;

    using Respawning;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableItem.ServerThrow"/>.
    /// Adds the <see cref="Handlers.Player.ThrowingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow))]
    internal static class ThrowingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(GameplayTickets), nameof(GameplayTickets.HandleItemTickets)))) + offset;

            newInstructions.RemoveRange(index, 3);

            offset = -3;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) }))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowableItem), nameof(ThrowableItem.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrowingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrowingItem))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowingItemEventArgs), nameof(ThrowingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                new(OpCodes.Ldsfld, Field(typeof(GameplayTickets), nameof(GameplayTickets.Singleton))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(GameplayTickets), nameof(GameplayTickets.HandleItemTickets))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
