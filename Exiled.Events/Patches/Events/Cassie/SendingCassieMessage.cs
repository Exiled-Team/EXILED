// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Cassie
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Respawning.RespawnEffectsController.PlayCassieAnnouncement(string, bool, bool)"/>.
    /// Adds the <see cref="Cassie.SendingCassieMessage"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Respawning.RespawnEffectsController), nameof(Respawning.RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class SendingCassieMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var offset = 1
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) - offset;

            var returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // string words
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // bool makeHold
                new CodeInstruction(OpCodes.Ldarg_1),

                // bool makeNoise
                new CodeInstruction(OpCodes.Ldarg_2),

                // var ev = SendingCassieMessageEventArgs(...)
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCassieMessageEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Cassie.SendingCassieMessageEventArgs(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Cassie), nameof(Cassie.OnSendingCassieMessage))),

                // if (!ev.IsAllowed) return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
