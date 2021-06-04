// -----------------------------------------------------------------------
// <copyright file="AntiCheatTriggerByR1WithTriggerCollider.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using UnityEngine;

#pragma warning disable SA1600

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AnticheatRaycast), new[] { typeof(Vector3), typeof(bool) })]
    internal static class AntiCheatTriggerByR1WithTriggerCollider
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Call && (MethodInfo)instr.operand ==
                    AccessTools.Method(
                        typeof(Physics),
                        nameof(Physics.RaycastNonAlloc),
                        new[] { typeof(Ray), typeof(RaycastHit[]), typeof(float), typeof(int) }))
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1); // QueryTriggerInteraction.Ignore
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(Physics),
                            nameof(Physics.RaycastNonAlloc),
                            new[] { typeof(Ray), typeof(RaycastHit[]), typeof(float), typeof(int), typeof(QueryTriggerInteraction) }));

                    continue;
                }

                yield return instr;
            }
        }
    }
}
