// -----------------------------------------------------------------------
// <copyright file="SendingCall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049CallAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.SendingCall" /> event.
    /// </summary>
    // TODO: REWORK TRANSPILER
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.SendingCall))]
    [HarmonyPatch]
    public class SendingCall
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Scp049CallAbility), nameof(Scp049CallAbility.ServerProcessCmd))]
        private static IEnumerable<CodeInstruction> OnSendingCall(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(SendingCall), nameof(SendingCall.ProcessCall))),
                new(OpCodes.Br, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        /// <summary>
        /// Process's Scp049 call ability.
        /// </summary>
        /// <param name="callAbility"> <see cref="Scp049CallAbility"/>. </param>
        private static void ProcessCall(Scp049CallAbility callAbility)
        {
            Player currentScp = Player.Get(callAbility.Owner);
            float duration = Scp049CallAbility.EffectDuration;

            var ev = new SendingCallEventArgs(currentScp, duration, !callAbility._serverTriggered && callAbility.Cooldown.IsReady);
            Handlers.Scp049.OnSendingCall(ev);

            if (!ev.IsAllowed)
                return;
            callAbility.Duration.Trigger(ev.Duration);
            callAbility._serverTriggered = true;
            callAbility.ServerSendRpc(true);
        }
    }
}