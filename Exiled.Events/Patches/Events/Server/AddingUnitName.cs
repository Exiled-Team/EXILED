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

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;

    using Respawning;
    using Respawning.NamingRules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="UnitNameMessageHandler.SendNew(SpawnableTeamType, UnitNamingRule)"/>.
    /// Adds the <see cref="Handlers.Server.AddingUnitName"/> event.
    /// </summary>
    [HarmonyPatch(typeof(UnitNameMessageHandler), nameof(UnitNameMessageHandler.SendNew))]
    internal static class AddingUnitName
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // rule
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // AddingUnitNameEventArgs ev = new(UnitNamingRule, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingUnitNameEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Server.OnAddingUnitName(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnAddingUnitName))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingUnitNameEventArgs), nameof(AddingUnitNameEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}