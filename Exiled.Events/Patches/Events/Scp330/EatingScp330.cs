// -----------------------------------------------------------------------
// <copyright file="EatingScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp330;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp330Bag.ServerOnUsingCompleted" />.
    /// Adds the <see cref="Scp330.EatingScp330" /> and <see cref="Scp330.EatenScp330" /> event.
    /// </summary>
    [EventPatch(typeof(Scp330), nameof(Scp330.EatingScp330))]
    [EventPatch(typeof(Scp330), nameof(Scp330.EatenScp330))]
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal static class EatingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ICandy), nameof(ICandy.ServerApplyEffects)))) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // ICandy
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // EatingScp330EventArgs ev = new(player, candy, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatingScp330EventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp330.OnEatingScp330(ev)
                    new(OpCodes.Call, Method(typeof(Scp330), nameof(Scp330.OnEatingScp330))),

                    // if (!ev.IsAllowed)
                    //  return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EatingScp330EventArgs), nameof(EatingScp330EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerRefreshBag)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // ICandy
                    new(OpCodes.Ldloc_0),

                    // EatenScp330EventArgs ev = new(player, candy)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatenScp330EventArgs))[0]),

                    // Handlers.Scp330.OnEatenScp330(ev)
                    new(OpCodes.Call, Method(typeof(Scp330), nameof(Scp330.OnEatenScp330))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}