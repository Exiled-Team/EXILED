// -----------------------------------------------------------------------
// <copyright file="Eaten.cs" company="Exiled Team">
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
    /// Adds the <see cref="Handlers.Scp330.OnEaten"/> event.
    /// </summary>
    [HarmonyDebug]
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal static class Eaten
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -1;
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerRefreshBag))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp330Bag), nameof(Scp330Bag.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ICandy
                new CodeInstruction(OpCodes.Ldloc_0),

                // var ev = new EatenSCP330EventArgs(player, candy)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EatenScp330EventArgs))[0]),

                // Handlers.SCP330.OnEaten(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnEaten))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
