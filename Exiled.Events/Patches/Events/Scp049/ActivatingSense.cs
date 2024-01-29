// -----------------------------------------------------------------------
// <copyright file="ActivatingSense.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using Mirror;

    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.Subroutines;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049SenseAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.ActivatingSense" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd))]
    public class ActivatingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Label than return in the this.Cooldown.Trigger((double)ev.FailedCooldown)
            int offset = 3;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldnull) + offset;
            Label failedLabel = generator.DefineLabel();
            newInstructions[index].labels.Add(failedLabel);

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingRecallEventArgs));

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StandardSubroutine<Scp049Role>), nameof(StandardSubroutine<Scp049Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(this.Target)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ActivatingSenseEventArgs ev = new(player, target, isAllowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingSenseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp049.OnFinishingRecall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnActivatingSense))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, failedLabel),

                    // if (ev.Target == null)
                    //    return;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Target))),
                    new(OpCodes.Brfalse_S, failedLabel),

                    // this.Target = ev.Target.ReferenceHub;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Target))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                });

            // replace "this.Cooldown.Trigger(2.5)" with "this.Cooldown.Trigger((double)ev.FailedCooldown)"
            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // (double)ev.FailedCooldown
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.FailedCooldown))),
                    new(OpCodes.Conv_R8),
                });

            // replace "this.Duration.Trigger(20.0)" with "this.Duration.Trigger((double)ev.Duration)"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // (double)ev.Duration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Duration))),
                    new(OpCodes.Conv_R8),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}