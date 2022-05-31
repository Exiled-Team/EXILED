// -----------------------------------------------------------------------
// <copyright file="WalkingOnSinkhole.cs" company="Exiled Team">
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

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="SinkholeEnvironmentalHazard.DistanceChanged"/> to add the <see cref="Handlers.Player.WalkingOnSinkhole"/> event.
    /// </summary>
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.DistanceChanged))]
    internal static class WalkingOnSinkhole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Bgt_Un_S) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(WalkingOnSinkholeEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnWalkingOnSinkhole))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(WalkingOnSinkholeEventArgs), nameof(WalkingOnSinkholeEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
