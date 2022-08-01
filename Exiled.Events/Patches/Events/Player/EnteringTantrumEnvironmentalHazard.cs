// -----------------------------------------------------------------------
// <copyright file="EnteringTantrumEnvironmentalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Environmental;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="TantrumEnvironmentalHazard.OnEnter(ReferenceHub)"/>.
    ///     Adds the <see cref="Handlers.EnvironementalHazard.EnteringEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnEnter))]
    internal static class EnteringTantrumEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnEnter)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnteringEnvironmentalHazardEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.EnvironementalHazard), nameof(Handlers.EnvironementalHazard.OnEnteringEnvironmentalHazard))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringEnvironmentalHazardEventArgs), nameof(EnteringEnvironmentalHazardEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
