// -----------------------------------------------------------------------
// <copyright file="UsingBreakneckSpeeds.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.ServerDoBreakneckSpeeds"/>.
    /// Adds the <see cref="Handlers.Scp173.UsingBreakneckSpeeds"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerDoBreakneckSpeeds))]
    internal static class UsingBreakneckSpeeds
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            int offset = -1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
            (FieldInfo)instruction.operand == Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._breakneckSpeedsCooldownRemaining))) + offset;

            newInstructions.RemoveRange(index, 5);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.Hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._breakneckSpeedsCooldownRemaining))),
                new(OpCodes.Ldc_R4, 0f),
                new(OpCodes.Ceq),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingBreakneckSpeedsEventArgs))[0]),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnUsingBreakneckSpeeds))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingBreakneckSpeedsEventArgs), nameof(UsingBreakneckSpeedsEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
