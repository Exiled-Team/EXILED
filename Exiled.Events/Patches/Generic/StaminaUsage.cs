// -----------------------------------------------------------------------
// <copyright file="StaminaUsage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PlayerRoles.FirstPersonControl;

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="StaminaStat.ModifyAmount(float)"/>.
    /// </summary>
    [HarmonyPatch(typeof(StaminaStat), nameof(StaminaStat.CurValue), MethodType.Setter)]
    internal class StaminaUsage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldfld, Field(typeof(StaminaStat), nameof(StaminaStat.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Brfalse_S, skipLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsUsingStamina))),
                new(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}