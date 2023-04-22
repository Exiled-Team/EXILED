// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939" /> event.
    /// </summary>
    [HarmonyPatch]
    internal static class Lunge
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.TriggerLunge))]
        private static IEnumerable<CodeInstruction> OnExecutingLunge(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Lunge), nameof(ProcessLunge))),
                new(OpCodes.Br, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ProcessLunge(Scp939LungeAbility lungeAbility)
        {
            var currentScp = Player.Get(lungeAbility.Owner);
            var ev = new LungingEventArgs(currentScp);

            Handlers.Scp939.OnLunging(ev);
            if (!ev.IsAllowed)
                return;

            lungeAbility.TriggerPos = lungeAbility.CurPos;
            lungeAbility.State = Scp939LungeState.Triggered;
        }
    }
}