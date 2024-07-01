// -----------------------------------------------------------------------
// <copyright file="SlownessFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcMotor.DesiredMove"/> getter to fix Slowness effect.
    /// </summary>
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.DesiredMove), MethodType.Getter)]
    internal class SlownessFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.operand == (object)Field(typeof(FpcMotor), nameof(FpcMotor._lastMaxSpeed))) + offset;

            newInstructions.Insert(index, new(OpCodes.Call, Method(typeof(Mathf), nameof(Mathf.Abs), new[] { typeof(float) })));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="FpcMotor.UpdatePosition"/> method to fix Slowness effect.
    /// </summary>
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdatePosition))]
#pragma warning disable SA1402 // File may only contain a single type
    internal class SlownessFixPosition
#pragma warning restore SA1402 // File may only contain a single type
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(FpcMotor), nameof(FpcMotor.Speed)))) + offset;

            newInstructions.Insert(index, new(OpCodes.Call, Method(typeof(Mathf), nameof(Mathf.Abs), new[] { typeof(float) })));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
