// -----------------------------------------------------------------------
// <copyright file="EatingScp330.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Scp330
{
    #pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp330Bag.ServerOnUsingCompleted"/>.
    /// Adds the <see cref="Handlers.Scp330.EatingScp330"/> and <see cref="Handlers.Scp330.EatenScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal static class EatingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -3;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Method(typeof(ICandy), nameof(ICandy.ServerApplyEffects))) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Owner)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ICandy
                new CodeInstruction(OpCodes.Ldloc_0),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new EatingScp330EventArgs(player, candy, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatingScp330EventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Scp330.OnEatingScp330(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnEatingScp330))),

                // if (!ev.IsAllowed)
                //  return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EatingScp330EventArgs), nameof(EatingScp330EventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerRefreshBag))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ICandy
                new CodeInstruction(OpCodes.Ldloc_0),

                // var ev = new EatenScp330EventArgs(player, candy)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatenScp330EventArgs))[0]),

                // Handlers.Scp330.OnEatenScp330(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnEatenScp330))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
