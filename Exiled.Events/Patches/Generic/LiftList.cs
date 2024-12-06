// -----------------------------------------------------------------------
// <copyright file="LiftList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using HarmonyLib;
    using Interactables.Interobjects;

    using static HarmonyLib.AccessTools;

    // TODO: CHECK IF THIS WORKS AS ISTENDED.

    /// <summary>
    /// Patches <see cref="ElevatorManager.SpawnChamber"/> to register all ElevatorChambers inside the Lift wrapper.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.SpawnChamber))]
    internal static class LiftList
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnSpawnChamber(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_1);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Lift), nameof(Lift.Get), new[] { typeof(ElevatorChamber) })),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ElevatorChamber.OnDestroy"/> to remove the destroyed ElevatorChambers from the Lift wrapper list.
    /// </summary>
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.OnDestroy))]
    internal static class LiftListRemove
    {
        [HarmonyPrefix]
        private static void OnDestroy(ElevatorChamber __instance) => Lift.ElevatorChamberToLift.Remove(__instance);
    }
}