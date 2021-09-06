// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using BaseTarget = InventorySystem.Items.Firearms.Utilities.ShootingTarget;

    /// <summary>
    /// Patches <see cref="BaseTarget.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingShootingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BaseTarget), nameof(BaseTarget.ServerInteract))]
    internal static class InteractingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_2) + offset;

            Label returnLabel = generator.DefineLabel();
            Label checkMaxHpLabel = generator.DefineLabel();
            Label checkAutoResetTimeLabel = generator.DefineLabel();
            Label rpcLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingShootingTargetEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_0),

                new CodeInstruction(OpCodes.Ldarg_2),

                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),
                new CodeInstruction(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(InteractingShootingTarget.GetNextValue))),

                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
                new CodeInstruction(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(InteractingShootingTarget.GetNextValue))),

                new CodeInstruction(OpCodes.Ldc_I4_1),

                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingShootingTargetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingShootingTarget))),

                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, checkMaxHpLabel),
            });

            offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(NetworkServer), nameof(NetworkServer.Destroy))) + offset;
            newInstructions[index] = new CodeInstruction(OpCodes.Br, checkMaxHpLabel);

            offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(BaseTarget), "set_Network_syncMode")) + offset;
            newInstructions[index] = new CodeInstruction(OpCodes.Br, checkMaxHpLabel);

            offset = -5;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(BaseTarget), nameof(BaseTarget.RpcSendInfo))) + offset;

            newInstructions[index].labels.Add(rpcLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(checkMaxHpLabel),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.MaxHp))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue, checkAutoResetTimeLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.MaxHp))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),

                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(checkAutoResetTimeLabel),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.AutoResetTime))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, rpcLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.AutoResetTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static int GetNextValue(byte buttonPressed, int targetButton, int curValue)
        {
            if (targetButton - buttonPressed < 0)
                return curValue;

            switch ((BaseTarget.TargetButton)buttonPressed)
            {
                case BaseTarget.TargetButton.IncreaseHP:
                    return Mathf.Clamp(curValue * 2, 1, 256);
                case BaseTarget.TargetButton.DecreaseHP:
                    return curValue / 2;
                case BaseTarget.TargetButton.IncreaseResetTime:
                    return Mathf.Clamp(curValue + 1, 0, 10);
                case BaseTarget.TargetButton.DecreaseResetTime:
                    return Mathf.Clamp(curValue - 1, 0, 10);
                default:
                    return curValue;
            }
        }
    }
}
