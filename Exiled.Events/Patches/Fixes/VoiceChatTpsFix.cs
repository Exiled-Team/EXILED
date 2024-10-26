// -----------------------------------------------------------------------
// <copyright file="VoiceChatTpsFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using HarmonyLib;

    using VoiceChat.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes <see cref="VoiceTransceiver.ServerReceiveMessage(Mirror.NetworkConnection, VoiceChat.Networking.VoiceMessage)"/> method.
    /// </summary>
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceChatTpsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newarr) + offset;

            // new array length 480
            newInstructions[index].operand = 480;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
