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

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateBegin))]
    internal static class StartingRecall
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ServerValidateBegin(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(StartingRecall), nameof(ReviveProcess))),
                new CodeInstruction(OpCodes.Br, retLabel),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ReviveProcess(Scp049ResurrectAbility resurrectAbility, BasicRagdoll ragdoll)
        {
            Player currentScp = Player.Get(resurrectAbility.Owner);
            Player targetPlayer = Player.Get(ragdoll.Info.OwnerHub);
            Ragdoll targetRagdoll = Ragdoll.Get(ragdoll);

            StartingRecallEventArgs ev = new(targetPlayer, currentScp, targetRagdoll);
            Handlers.Scp049.OnStartingRecall(ev);

            if (!ev.IsAllowed)
                return true;

            Scp049ResurrectAbility.ResurrectError resurrectError = resurrectAbility.CheckBeginConditions(ragdoll);
            if (resurrectError != Scp049ResurrectAbility.ResurrectError.None)
                return true;

            return !resurrectAbility.ServerValidateAny();
        }
    }
}
