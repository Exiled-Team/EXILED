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
    using PlayerRoles.PlayableScps.Subroutines;
    using Utils.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049SenseAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.ActivatingSense" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp049), nameof(Handlers.Scp049.ActivatingSense))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerProcessCmd))]
    public class ActivatingSense
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.operand == (object)PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(FinishingRecallEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp049Role>), nameof(ScpStandardSubroutine<Scp049Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(this.Target)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
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
                    new(OpCodes.Brfalse_S, returnLabel),

                    // this.Target = ev.Target
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Target))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                    new(OpCodes.Call, PropertySetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Target))),
                });

            // replace "this.Cooldown.Trigger(2.5)" with "this.Cooldown.Trigger(ev.FailedCooldown)"
            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)PropertySetter(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.FailedCooldown
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.FailedCooldown))),
                });

            // replace "this.Duration.Trigger(20.0)" with "this.Duration.Trigger(ev.Duration)"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)PropertySetter(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.Duration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingSenseEventArgs), nameof(ActivatingSenseEventArgs.Duration))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}