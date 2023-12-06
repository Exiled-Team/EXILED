// -----------------------------------------------------------------------
// <copyright file="SavingVoice.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MimicryRecorder.OnAnyPlayerKilled(ReferenceHub, DamageHandlerBase)" />
    /// to add the <see cref="Scp939.SavingVoice" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.SavingVoice))]
    [HarmonyPatch(typeof(MimicryRecorder), nameof(MimicryRecorder.OnAnyPlayerKilled))]
    internal static class SavingVoice
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            int offset = 3;
            int index = newInstructions.FindIndex(i => i.operand == (object)Method(typeof(MimicryRecorder), nameof(MimicryRecorder.IsPrivacyAccepted))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MimicryRecorder), nameof(MimicryRecorder.Owner))),

                // ply
                new CodeInstruction(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // SavingVoiceEventArgs ev = new(referenceHub, ply, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SavingVoiceEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp939.OnSavingVoice(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnSavingVoice))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SavingVoiceEventArgs), nameof(SavingVoiceEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}