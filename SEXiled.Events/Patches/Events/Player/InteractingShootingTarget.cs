// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTarget.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using BaseTarget = AdminToys.ShootingTarget;

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
            Label setMaxHpLabel = generator.DefineLabel();
            Label rpcLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingShootingTargetEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ReferenceHub)
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // colliderId (ShootingTargetButton)
                new CodeInstruction(OpCodes.Ldarg_2),

                // GetNextValue(buttonPressed, ShootingTargetButton.DecreaseHp, BaseTarget._maxHp)
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),
                new CodeInstruction(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(InteractingShootingTarget.GetNextValue))),

                // GetNextValue(buttonPressed, ShootingTargetButton.DecreaseResetTime, BaseTarget._autoDestroyTime)
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
                new CodeInstruction(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(InteractingShootingTarget.GetNextValue))),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new InteractingShootingTargetEventArgs(Player, BaseTarget, ShootingTargetButton, MaxHp, ResetTime, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingShootingTargetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // OnInteractingShootingTarget(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingShootingTarget))),

                // if (!ev.IsAllowed)
                //   CheckMaxHp
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, setMaxHpLabel),
            });

            offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(NetworkServer), nameof(NetworkServer.Destroy))) + offset;
            newInstructions[index] = new CodeInstruction(OpCodes.Br, setMaxHpLabel);

            offset = 1;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(BaseTarget), "set_Network_syncMode")) + offset;
            newInstructions[index] = new CodeInstruction(OpCodes.Br, setMaxHpLabel);

            offset = -5;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(BaseTarget), nameof(BaseTarget.RpcSendInfo))) + offset;

            newInstructions[index].labels.Add(rpcLabel);

            newInstructions.InsertRange(index, new[]
            {
                // BaseTarget.MaxHp = ev.NewMaxHp
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(setMaxHpLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.NewMaxHp))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),

                // BaseTarget._autoDestroyTime = ev.NewAutoResetTime
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.NewAutoResetTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static int GetNextValue(byte buttonPressed, int targetButton, int curValue)
        {
            if (targetButton != buttonPressed && (targetButton - 1) != buttonPressed)
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
