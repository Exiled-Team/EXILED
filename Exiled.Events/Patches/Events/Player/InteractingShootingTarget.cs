// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using BaseTarget = AdminToys.ShootingTarget;

    /// <summary>
    /// Patches <see cref="BaseTarget.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingShootingTarget" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingShootingTarget))]
    [HarmonyPatch(typeof(BaseTarget), nameof(BaseTarget.ServerInteract))]
    internal static class InteractingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label setMaxHpLabel = generator.DefineLabel();
            Label rpcLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingShootingTargetEventArgs));

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_2) + offset;

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // colliderId (ShootingTargetButton)
                    new(OpCodes.Ldarg_2),

                    // GetNextValue(colliderId, ShootingTargetButton.DecreaseHp, this._maxHp)
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),
                    new(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(GetNextValue))),

                    // GetNextValue(colliderId, ShootingTargetButton.DecreaseResetTime, this._autoDestroyTime)
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
                    new(OpCodes.Call, Method(typeof(InteractingShootingTarget), nameof(GetNextValue))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // InteractingShootingTargetEventArgs ev = new(Player, ShootingTarget, ShootingTargetButton, int, int, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingShootingTargetEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnInteractingShootingTarget(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingShootingTarget))),

                    // if (!ev.IsAllowed)
                    //   goto setMaxHpLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, setMaxHpLabel),
                });

            offset = 1;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Destroy)))) + offset;

            newInstructions[index] = new CodeInstruction(OpCodes.Br, setMaxHpLabel);

            offset = 1;
            index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(BaseTarget), nameof(BaseTarget.Network_syncMode)))) + offset;

            newInstructions[index] = new CodeInstruction(OpCodes.Br, setMaxHpLabel);

            offset = -5;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(BaseTarget), nameof(BaseTarget.RpcSendInfo)))) + offset;

            newInstructions[index].WithLabels(rpcLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this.MaxHp = ev.NewMaxHp
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(setMaxHpLabel),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.NewMaxHp))),
                    new(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._maxHp))),

                    // this._autoDestroyTime = ev.NewAutoResetTime
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.NewAutoResetTime))),
                    new(OpCodes.Stfld, Field(typeof(BaseTarget), nameof(BaseTarget._autoDestroyTime))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static int GetNextValue(byte buttonPressed, int targetButton, int curValue)
        {
            if (targetButton != buttonPressed && targetButton - 1 != buttonPressed)
                return curValue;

            return (BaseTarget.TargetButton)buttonPressed switch
            {
                BaseTarget.TargetButton.IncreaseHP => Mathf.Clamp(curValue * 2, 1, 256),
                BaseTarget.TargetButton.DecreaseHP => curValue / 2,
                BaseTarget.TargetButton.IncreaseResetTime => Mathf.Clamp(curValue + 1, 0, 10),
                BaseTarget.TargetButton.DecreaseResetTime => Mathf.Clamp(curValue - 1, 0, 10),
                _ => curValue,
            };
        }
    }
}