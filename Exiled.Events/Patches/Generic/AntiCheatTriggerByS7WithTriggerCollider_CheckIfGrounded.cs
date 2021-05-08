// -----------------------------------------------------------------------
// <copyright file="AntiCheatTriggerByS7WithTriggerCollider_CheckIfGrounded.cs" company="Exiled Team">
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

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.CheckIfGrounded), new[] { typeof(Vector3) })]
    internal static class AntiCheatTriggerByS7WithTriggerCollider_CheckIfGrounded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Call)
                {
                    if ((MethodInfo)instr.operand == AccessTools.Method(
                        typeof(Physics),
                        nameof(Physics.OverlapSphereNonAlloc),
                        new[] { typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int) }))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1); // QueryTriggerInteraction.Ignore
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(
                            typeof(Physics),
                            nameof(Physics.OverlapSphereNonAlloc),
                            new[] { typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int), typeof(QueryTriggerInteraction) }));

                        continue;
                    }

                    if ((MethodInfo)instr.operand == AccessTools.Method(
                        typeof(Physics),
                        nameof(Physics.OverlapCapsuleNonAlloc),
                        new[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int) }))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1); // QueryTriggerInteraction.Ignore
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(
                            typeof(Physics),
                            nameof(Physics.OverlapCapsuleNonAlloc),
                            new[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int), typeof(QueryTriggerInteraction) }));

                        continue;
                    }
                }

                yield return instr;
            }
        }
    }
}
