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

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Cassie;

    using Handlers;

    using HarmonyLib;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RespawnEffectsController.PlayCassieAnnouncement(string, bool, bool, bool)" />.
    /// Adds the <see cref="Cassie.SendingCassieMessage" /> event.
    /// </summary>
    [EventPatch(typeof(Cassie), nameof(Cassie.SendingCassieMessage))]
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class SendingCassieMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

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
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCassieMessageEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Cassie), nameof(Cassie.OnSendingCassieMessage))),
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.Words))),
                    new(OpCodes.Starg_S, 0),
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeHold))),
                    new(OpCodes.Starg_S, 1),
                    new(OpCodes.Call, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.MakeNoise))),
                    new(OpCodes.Starg_S, 2),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCassieMessageEventArgs), nameof(SendingCassieMessageEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}