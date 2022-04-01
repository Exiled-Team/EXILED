// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Cassie {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Respawning.RespawnEffectsController.PlayCassieAnnouncement(string, bool, bool, bool)"/>.
    /// Adds the <see cref="Handlers.Cassie.SendingCassieMessage"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Respawning.RespawnEffectsController), nameof(Respawning.RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class SendingCassieMessage {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            // SendingCassieMessageEventArgs ev = new SendingCassieMessageEventArgs(words, makeHold, makeNoise, true);
            // Cassie.OnSendingCassieMessage(ev);
            //
            // words = ev.Words;
            // makeHold = ev.MakeHold;
            // makeNoise = ev.MakeNoise;
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCassieMessageEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Cassie), nameof(Handlers.Cassie.OnSendingCassieMessage))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.Words))),
                new CodeInstruction(OpCodes.Starg_S, 0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeHold))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeNoise))),
                new CodeInstruction(OpCodes.Starg_S, 2),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
