// -----------------------------------------------------------------------
// <copyright file="ServerSendAudioMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FirearmExtensions.ServerSendAudioMessage"/> to prevent the sending of hit audio messages to NPCs (Null Connections).
    /// </summary>
    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class ServerSendAudioMessage
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnSendingAudioMessage(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_S);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, 5).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsNpc))),
                new(OpCodes.Brtrue_S, returnLabel),
            });

            const int offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.operand is MethodInfo { Name: "Send" }) + offset;
            newInstructions[index].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}