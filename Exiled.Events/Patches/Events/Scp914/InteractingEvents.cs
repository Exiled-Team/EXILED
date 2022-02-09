// -----------------------------------------------------------------------
// <copyright file="InteractingEvents.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System;
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Scp914;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp914Controller.ServerInteract"/>.
    /// Adds the <see cref="Scp914.Activating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class InteractingEvents
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -8;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingKnobSettingEventArgs));
            LocalBuilder knobSetting = generator.DeclareLocal(typeof(Scp914KnobSetting));

            Label ret = generator.DefineLabel();
            Label jcc = generator.DefineLabel();

            newInstructions.RemoveRange(index, 22);

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp914Controller), nameof(Scp914Controller._knobSetting))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Conv_U1),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingKnobSettingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnChangingKnobSetting))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Stloc_S, knobSetting.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, knobSetting.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(Scp914Controller), nameof(Scp914Controller.Network_knobSetting))),
                new CodeInstruction(OpCodes.Ldtoken, typeof(Scp914KnobSetting)),
                new CodeInstruction(OpCodes.Call, Method(typeof(Type), nameof(Type.GetTypeFromHandle))),
                new CodeInstruction(OpCodes.Ldloc_S, knobSetting.LocalIndex),
                new CodeInstruction(OpCodes.Box, typeof(Scp914KnobSetting)),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enum), nameof(Enum.IsDefined))),
                new CodeInstruction(OpCodes.Brtrue_S, jcc),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(Scp914Controller), nameof(Scp914Controller.Network_knobSetting))),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jcc),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Scp914Controller), nameof(Scp914Controller.RpcPlaySound))),
                new CodeInstruction(OpCodes.Br_S, ret),
            });

            offset = -2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnActivating))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingEventArgs), nameof(ActivatingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
