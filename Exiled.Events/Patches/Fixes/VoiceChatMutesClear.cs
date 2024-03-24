// -----------------------------------------------------------------------
// <copyright file="VoiceChatMutesClear.cs" company="Exiled Team">
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

    using VoiceChat;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes <see cref="VoiceChatMutes.LoadMutes"/> method.
    /// </summary>
    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.LoadMutes))]
    internal static class VoiceChatMutesClear
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // VoiceChatMutes.Mutes.Clear()
                    new CodeInstruction(OpCodes.Ldsfld, Field(typeof(VoiceChatMutes), nameof(VoiceChatMutes.Mutes))),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<string>), nameof(HashSet<string>.Clear))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}