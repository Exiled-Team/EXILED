// -----------------------------------------------------------------------
// <copyright file="InteractingEvents.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp914;

    using global::Scp914;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp914Controller.ServerInteract"/>.
    ///     Adds the <see cref="Handlers.Scp914.Activating"/> and <see cref="Handlers.Scp914.ChangingKnobSetting"/> events.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class InteractingEvents
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder changingKnobSetting = generator.DeclareLocal(typeof(ChangingKnobSettingEventArgs));

            Label returnLabel = generator.DefineLabel();
            Label ifLabel = generator.DefineLabel();
            Label elseLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld) + offset;

            newInstructions.RemoveRange(index + 1, 22);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Bne_Un_S, ifLabel),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stloc_0),
                new(OpCodes.Br_S, elseLabel),
                new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(ifLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp914Controller), nameof(Scp914Controller._knobSetting))),
                new(OpCodes.Add),
                new(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(elseLabel),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingKnobSettingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, changingKnobSetting.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnChangingKnobSetting))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, changingKnobSetting.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.KnobSetting))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Scp914Controller), nameof(Scp914Controller.Network_knobSetting))),
            });

            offset = -3;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Stfld && (FieldInfo)instruction.operand == Field(typeof(Scp914Controller), nameof(Scp914Controller._remainingCooldown))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingScp914EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnActivating))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingScp914EventArgs), nameof(ActivatingScp914EventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
