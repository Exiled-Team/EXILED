// -----------------------------------------------------------------------
// <copyright file="VoiceChatTpsFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using VoiceChat.Networking;

    /// <summary>
    /// Fixes <see cref="VoiceTransceiver.ServerReceiveMessage(Mirror.NetworkConnection, VoiceChat.Networking.VoiceMessage)"/> method.
    /// </summary>
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceChatTpsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newarr) + offset;

            // new array length 480
            newInstructions[index].operand = 480;

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}