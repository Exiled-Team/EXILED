// -----------------------------------------------------------------------
// <copyright file="Eating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    #pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp330Bag.ServerOnUsingCompleted"/>.
    /// Adds the <see cref="Handlers.Scp330.OnEating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal static class Eating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -3;

            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Method(typeof(ICandy), nameof(ICandy.ApplyEffects))) + offset;

            var returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ICandy
                new CodeInstruction(OpCodes.Ldloc_0),

                // True
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new EatingSCP330EventArgs(player, candy, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatingScp330EventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.SCP330.OnEating(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnEating))),

                // if(!ev.IsAllowed)
                //  return
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EatingScp330EventArgs), nameof(EatingScp330EventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
