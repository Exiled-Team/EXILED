// -----------------------------------------------------------------------
// <copyright file="EnteringSinkholeEnvironmentalHazard.cs" company="Exiled Team">
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
    /// Patches <see cref="SinkholeEnvironmentalHazard.OnEnter(ReferenceHub)"/>.
    /// Adds the <see cref="Handlers.Player.EnteringEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnEnter))]
    internal static class EnteringSinkholeEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));

            Label cnt = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Stloc_S, isAllowed.LocalIndex),
                new(OpCodes.Br_S, cnt),
            });

            offset = -2;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnEnter)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(cnt),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, isAllowed.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnteringEnvironmentalHazardEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEnteringEnvironmentalHazard))),
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
