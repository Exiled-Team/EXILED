// -----------------------------------------------------------------------
// <copyright file="Scp173Update.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.NPCs
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Extensions;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayableScps;

#pragma warning disable SA1118
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173.OnUpdate"/> to prevent issues with NPCs.
    /// </summary>
    [HarmonyPatch(typeof(Scp173), nameof(Scp173.OnUpdate))]
    internal static class Scp173Update
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp173), nameof(Scp173.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(NpcExtensions), nameof(NpcExtensions.IsNpc), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Brtrue_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
