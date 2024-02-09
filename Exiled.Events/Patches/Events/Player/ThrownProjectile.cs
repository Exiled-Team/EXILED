// -----------------------------------------------------------------------
// <copyright file="ThrownProjectile.cs" company="Exiled Team">
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

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableItem.ServerThrow"/>.
    /// Adds the <see cref="Handlers.Player.ThrownProjectile"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ThrownProjectile))]
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow))]
    internal static class ThrownProjectile
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) }))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // thrownProjectile
                new(OpCodes.Dup),

                // API.Features.Player.Get(this.Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowableItem), nameof(ThrowableItem.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // ThrownProjectile ev = new(thrownProjectile, player, this);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ThrownProjectileEventArgs))[0]),

                // Handlers.Player.OnThrownProjectile(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnThrownProjectile))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
