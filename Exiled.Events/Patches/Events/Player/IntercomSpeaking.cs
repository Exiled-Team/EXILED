// -----------------------------------------------------------------------
// <copyright file="IntercomSpeaking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
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
    /// Adds the <see cref="IntercomSpeaking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.RequestTransmission))]
    internal static class IntercomSpeaking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(GameObject)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // new IntercomSpeakingEventArgs(Player, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(IntercomSpeakingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnIntercomSpeaking))),

                // if (!IntercomSpeakingEventArgs.IsAllowed) return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(IntercomSpeakingEventArgs), nameof(IntercomSpeakingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
