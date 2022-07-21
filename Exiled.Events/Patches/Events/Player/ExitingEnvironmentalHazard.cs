// -----------------------------------------------------------------------
// <copyright file="ExitingEnvironmentalHazard.cs" company="Exiled Team">
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
    /// Patches <see cref="EnvironmentalHazard.OnExit(ReferenceHub)"/>.
    /// Adds the <see cref="Handlers.Player.ExitingEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnExit))]
    internal static class ExitingEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExitingEnvironmentalHazardEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnExitingEnvironmentalHazard))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExitingEnvironmentalHazardEventArgs), nameof(ExitingEnvironmentalHazardEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
