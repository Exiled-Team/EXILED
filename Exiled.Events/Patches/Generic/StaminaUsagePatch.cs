// -----------------------------------------------------------------------
// <copyright file="StaminaUsagePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Stamina.ProcessStamina()"/>.
    /// </summary>
    /// Adds <see cref="Player.IsUsingStamina"/> implementation.
    [HarmonyPatch(typeof(Stamina), nameof(Stamina.ProcessStamina))]
    internal class StaminaUsagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label label1 = generator.DefineLabel();
            Label label2 = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Stamina), nameof(Stamina._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub), })),
                    new(OpCodes.Dup),
                    new(OpCodes.Brtrue_S, label1),
                    new(OpCodes.Pop),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Br_S, label2),
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.IsUsingStamina))).WithLabels(label1),
                    new CodeInstruction(OpCodes.Nop).WithLabels(label2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}