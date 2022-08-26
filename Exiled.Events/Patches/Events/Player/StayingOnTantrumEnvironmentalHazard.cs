// -----------------------------------------------------------------------
// <copyright file="StayingOnTantrumEnvironmentalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TantrumEnvironmentalHazard"/>.
    /// <br>Adds the <see cref="Handlers.Player.StayingOnEnvironmentalHazard"/> event.</br>
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.StayingOnEnvironmentalHazard))]
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnUpdate))]
    internal static class StayingOnTantrumEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder cnt = generator.DeclareLocal(typeof(int));
            LocalBuilder lpAlive = generator.DeclareLocal(typeof(bool));

            Label lpHead = generator.DefineLabel();
            Label lpEnd = generator.DefineLabel();

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Stloc_S, cnt.LocalIndex),
                    new(OpCodes.Br_S, lpEnd),
                    new CodeInstruction(OpCodes.Nop).WithLabels(lpHead),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.AffectedPlayers))),
                    new(OpCodes.Ldloc_S, cnt.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(List<ReferenceHub>), "get_Item")),
                    new(OpCodes.Callvirt, Method(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnStay))),
                    new(OpCodes.Ldloc_S, cnt.LocalIndex),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Add),
                    new(OpCodes.Stloc_S, cnt.LocalIndex),
                    new CodeInstruction(OpCodes.Ldloc_S, cnt.LocalIndex).WithLabels(lpEnd),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.AffectedPlayers))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(List<ReferenceHub>), nameof(List<ReferenceHub>.Count))),
                    new(OpCodes.Clt),
                    new(OpCodes.Stloc_S, lpAlive.LocalIndex),
                    new(OpCodes.Ldloc_S, lpAlive.LocalIndex),
                    new(OpCodes.Brtrue_S, lpHead),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}