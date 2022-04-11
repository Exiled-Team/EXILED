// -----------------------------------------------------------------------
// <copyright file="OverwatchSkipFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RunDefaultClassPicker(bool, out int[], out Dictionary{GameObject, RoleType})"/>.
    /// Fixes overwatch players not spawning correctly.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RunDefaultClassPicker))]
    internal static class OverwatchSkipFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset
            int offset = 2;
            int lastBrOffset = 6;

            // Find the first Fraction.Others call.
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_3) + offset;

            // Find labels.
            Label continueLabel = (Label)newInstructions[index - 1].operand;
            Label originalLabel = (Label)newInstructions[index - lastBrOffset].operand;

            // Fix original label.
            Label fixLabel = generator.DefineLabel();
            newInstructions[index - lastBrOffset].operand = fixLabel;

            // if (referenceHub.serverRoles.OverwatchEnabled)
            // {
            //      list.Add(RoleType.Spectator);
            //      playersRoleList.Add(referenceHub.gameObject, RoleType.Spectator);
            //      continue;
            // }
            newInstructions.InsertRange(index, new[]
            {
                // if (referenceHub.serverRoles.OverwatchEnabled)
                new CodeInstruction(OpCodes.Ldloc_S, 9),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.serverRoles))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles.OverwatchEnabled))),
                new CodeInstruction(OpCodes.Brfalse_S, originalLabel),

                // list.Add(RoleType.Spectator);
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(List<int>), nameof(List<int>.Add))),

                // playersRoleList.Add(referenceHub.gameObject, RoleType.Spectator);
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldind_Ref),
                new CodeInstruction(OpCodes.Ldloc_S, 9),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(Dictionary<GameObject, RoleType>), nameof(Dictionary<GameObject, RoleType>.Add))),

                // continue;
                new CodeInstruction(OpCodes.Br, continueLabel),
            });

            // Set original label.
            newInstructions[index].labels.Add(fixLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
