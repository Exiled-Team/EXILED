// -----------------------------------------------------------------------
// <copyright file="StartingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp049;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateBegin))]
    internal static class StartingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Scp049SenseAbility
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(StartingRecall), nameof(ServerValidateProcess))),
                    new(OpCodes.Br, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ServerValidateProcess(Scp049ResurrectAbility instance, BasicRagdoll ragdoll)
        {
            if (!instance.ServerValidateAny())
                return true;
            StartingReviveEventArgs startingReviveEvent = new StartingReviveEventArgs(Player.Get(ragdoll.Info.OwnerHub), Player.Get(instance.Owner), ragdoll, instance.CheckBeginConditions(ragdoll));
            Handlers.Scp049.OnStartingRecall(startingReviveEvent);

            // NW funny 0 is good, 1 is bad
            return !startingReviveEvent.IsAllowed;
        }
    }
}