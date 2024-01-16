// -----------------------------------------------------------------------
// <copyright file="Revealing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp3114Identity.Update" /> setter.
    /// Adds the <see cref="Handlers.Scp3114.Revealed" /> and <see cref="Handlers.Scp3114.Revealing" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealed))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealing))]
    [HarmonyPatch(typeof(Scp3114Identity), nameof(Scp3114Identity.Update))]
    internal class Revealing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Reveal), nameof(Scp3114Reveal.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // false (IsManualReveal)
                new(OpCodes.Ldc_I4_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // RevealingEventArgs ev = new RevealingEventArgs(Player, bool, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RevealingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.OnRevealing(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnRevealing))),

                // if (ev.IsAllowed)
                //     goto continueLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(RevealingEventArgs), nameof(RevealingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // this.RemainingDuration.Trigger(this._disguiseDurationSeconds);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp3114Identity), nameof(Scp3114Identity.RemainingDuration))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp3114Identity), nameof(Scp3114Identity._disguiseDurationSeconds))),
                new(OpCodes.Conv_R8),
                new(OpCodes.Call, Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))),

                // this.ServerResendIdentity();
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Scp3114Identity), nameof(Scp3114Identity.ServerResendIdentity))),

                // return;
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            offset = 1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // player
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                // false (IsManualReveal)
                new(OpCodes.Ldc_I4_0),

                // RevealedEventArgs ev = new RevealedEventArgs(Player, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RevealedEventArgs))[0]),

                // Handlers.Scp3114.OnRevealed(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnRevealed))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp3114Reveal.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Scp3114.Revealed" /> and <see cref="Handlers.Scp3114.Revealing" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealed))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealing))]
    [HarmonyPatch(typeof(Scp3114Reveal), nameof(Scp3114Reveal.ServerProcessCmd))]
    internal class RevealingKey
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Reveal), nameof(Scp3114Reveal.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // true (IsManualReveal)
                new(OpCodes.Ldc_I4_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // RevealingEventArgs ev = new RevealingEventArgs(Player, bool, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RevealingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.OnRevealing(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnRevealing))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(RevealingEventArgs), nameof(RevealingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // player
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // true (IsManualReveal)
                new(OpCodes.Ldc_I4_1),

                // RevealedEventArgs ev = new RevealedEventArgs(Player, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RevealedEventArgs))[0]),

                // Handlers.Scp3114.OnRevealed(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnRevealed))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}