// -----------------------------------------------------------------------
// <copyright file="Sending049Call.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049CallAbility), nameof(Scp049CallAbility.ServerProcessCmd))]
    public class Sending049Call
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Scp049CallAbility
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Sending049Call), nameof(Scp049SendingCall))),
                    new(OpCodes.Br, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Process's Scp049 sending call.
        /// </summary>
        /// <param name="callAbility"> <see cref="Scp049CallAbility"/>. </param>
        private static void Scp049SendingCall(Scp049CallAbility callAbility)
        {
            Sending049CallEventArgs sendingCallEvent = new Sending049CallEventArgs(API.Features.Player.Get(callAbility.Owner));
            Handlers.Scp049.OnSendingCall(sendingCallEvent);

            if (!sendingCallEvent.IsAllowed)
            {
                return;
            }

            if (!sendingCallEvent.BypassChecks)
            {
                if (callAbility._serverTriggered || !callAbility.Cooldown.IsReady)
                {
                    return;
                }
            }

            callAbility.Duration.Trigger(sendingCallEvent.Duration);
            callAbility._serverTriggered = true;
            callAbility.ServerSendRpc(true);
        }
    }
}