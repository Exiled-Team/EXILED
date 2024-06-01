// -----------------------------------------------------------------------
// <copyright file="UsingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp244;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp244Item" /> to add missing event handler to the
    /// <see cref="Scp244.UsingScp244" />.
    /// </summary>
    [EventPatch(typeof(Scp244), nameof(Scp244.UsingScp244))]
    [HarmonyPatch(typeof(Scp244Item), nameof(Scp244Item.ServerOnUsingCompleted))]
    internal static class UsingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // Player.Get(base.Owner)
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244Item), nameof(Scp244Item.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UsingScp244EventArgs ev = new(Scp244Item, Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingScp244EventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp244.OnUsingScp244(ev)
                    new(OpCodes.Call, Method(typeof(Scp244), nameof(Scp244.OnUsingScp244))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingScp244EventArgs), nameof(UsingScp244EventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}