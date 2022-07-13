// -----------------------------------------------------------------------
// <copyright file="ChangingMoveState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AnimationController.UserCode_CmdChangeSpeedState"/>.
    /// Adds the <see cref="Player.ChangingMoveState"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.ChangingMoveState))]
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.UserCode_CmdChangeSpeedState))]
    internal static class ChangingMoveState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMoveStateEventArgs));

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(AnimationController), nameof(AnimationController._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.MoveState))),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMoveStateEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingMoveState))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(ChangingMoveStateEventArgs), nameof(ChangingMoveStateEventArgs.NewState))),
                new(OpCodes.Stloc_0),
            });

            offset = -3;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
            (FieldInfo)instruction.operand == Field(typeof(Role), nameof(Role.team))) + offset;

            newInstructions.RemoveRange(index, 10);

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
