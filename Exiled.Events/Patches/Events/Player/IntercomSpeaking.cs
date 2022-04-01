// -----------------------------------------------------------------------
// <copyright file="IntercomSpeaking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Intercom.UserCode_CmdSetTransmit(bool)"/>.
    /// Adds the <see cref="Player.IntercomSpeaking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.RequestTransmission))]
    internal static class IntercomSpeaking {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            newInstructions.InsertRange(0, new[]
            {
                // Player.Get(GameObject)
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // new IntercomSpeakingEventArgs(Player, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(IntercomSpeakingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnIntercomSpeaking))),

                // if (!IntercomSpeakingEventArgs.IsAllowed) return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(IntercomSpeakingEventArgs), nameof(IntercomSpeakingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
