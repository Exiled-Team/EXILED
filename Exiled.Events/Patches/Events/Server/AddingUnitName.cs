// -----------------------------------------------------------------------
// <copyright file="AddingUnitName.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using Respawning.NamingRules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="UnitNamingRule.AddCombination"/>.
    /// Adds the <see cref="Handlers.Server.AddingUnitName"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.AddingUnitName))]
    [HarmonyPatch(typeof(UnitNamingRule), nameof(UnitNamingRule.AddCombination))]
    internal static class AddingUnitName
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingUnitNameEventArgs))[0]),

                new(OpCodes.Dup),
                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnAddingUnitName))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingUnitNameEventArgs), nameof(AddingUnitNameEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, skipLabel),

                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingUnitNameEventArgs), nameof(AddingUnitNameEventArgs.UnitName))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new(OpCodes.Br_S, returnLabel),
                new CodeInstruction(OpCodes.Pop).WithLabels(skipLabel),
                new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
