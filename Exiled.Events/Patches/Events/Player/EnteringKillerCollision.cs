// -----------------------------------------------------------------------
// <copyright file="EnteringKillerCollision.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CheckpointKiller.OnTriggerEnter"/>.
    /// Adds the <see cref="Handlers.Player.EnteringKillerCollision"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CheckpointKiller), nameof(CheckpointKiller.OnTriggerEnter))]
    internal static class EnteringKillerCollision
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            int offset = -5;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnteringKillerCollisionEventArgs))[0]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEnteringKillerCollision))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringKillerCollisionEventArgs), nameof(EnteringKillerCollisionEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
