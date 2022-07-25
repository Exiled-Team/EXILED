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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244Item"/> to add missing event handler to the <see cref="Scp244Item.ServerOnUsingCompleted"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244Item), nameof(Scp244Item.ServerOnUsingCompleted))]
    internal static class UsingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = 0;

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244Item), nameof(Scp244Item.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingScp244EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnUsingScp244))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingScp244EventArgs), nameof(UsingScp244EventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
