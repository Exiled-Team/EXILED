using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Scp049;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Subroutines;
using PluginAPI.Core;

namespace Exiled.Events.Patches.Events.Scp049
{
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

            // const int offset = 2;
            // int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(RagdollData), nameof(RagdollData.OwnerHub)))) + offset;

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Scp049CallAbility, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Sending049Call), nameof(Scp049SendingCall))),
                    new(OpCodes.Br, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Basically rewrites the ServerProcessCmd - It really is not worth it to not do this
        /// </summary>
        /// <param name="callAbility"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static void Scp049SendingCall(Scp049CallAbility callAbility, NetworkReader reader)
        {
            Sending049CallEventArgs sendingCallEvent = new Sending049CallEventArgs(API.Features.Player.Get(callAbility.Owner), reader);
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