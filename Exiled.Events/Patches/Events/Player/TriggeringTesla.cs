// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using NorthwoodLib.Pools;
    using UnityEngine;

    using static HarmonyLib.AccessTools;
    using BaseTeslaGate = TeslaGate;


    /// <summary>
    ///     Patches <see cref="TeslaGateController.FixedUpdate" />.
    ///     Adds the <see cref="Handlers.Player.TriggeringTesla" /> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingItemEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder teslaGate = generator.DeclareLocal(typeof(TeslaGate));

            Label nullTeslaCheck = generator.DefineLabel();

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label continueLabel2 = generator.DefineLabel();
            Label isAllowed = generator.DefineLabel();
            Label isInIdleRange = generator.DefineLabel();

            const int offset1 = 6;
            int index1 = newInstructions.FindIndex(x => x.opcode == OpCodes.Br) + offset1;
            newInstructions.InsertRange(
                index1,
                new[]
                {
                    // if ((tesla = TeslaGate.Get(teslaGate)) is not null)
                    //      continue;
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, Method(typeof(TeslaGate), nameof(TeslaGate.Get), new[] { typeof(BaseTeslaGate) })),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_S, teslaGate.LocalIndex),
                    new CodeInstruction(OpCodes.Brfalse_S, nullTeslaCheck),
                });
            newInstructions.Find(x => x.opcode == OpCodes.Br).labels.Add(nullTeslaCheck);
            const int offset2 = 0;
            int index2 = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_3) + offset2;

            newInstructions.RemoveRange(index2, 25);
            newInstructions.InsertRange(
                index2,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldstr, "PlayerCheck"),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                    // if (Player.TryGet(hub.Key, out Player player) || !tesla.CanBeIdle(player))
                    //      continue;

                    // if (Player.TryGet(hub.Key, out Player player) ||
                    new CodeInstruction(OpCodes.Ldloca_S, 6),
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Key))),
                    new CodeInstruction(OpCodes.Ldloca_S, player.LocalIndex),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.TryGet), new[] { typeof(GameObject), typeof(Player).MakeByRefType() })),
                    new CodeInstruction(OpCodes.Brtrue_S, continueLabel),

                    // !tesla.CanBeIdle(player)
                    new CodeInstruction(OpCodes.Ldloc_S, teslaGate.LocalIndex),
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.CanBeIdle))),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Br_S, continueLabel2),

                    // Continue;
                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(continueLabel),
                    new CodeInstruction(OpCodes.Brtrue_S, returnLabel).WithLabels(continueLabel2),

                    new CodeInstruction(OpCodes.Ldstr, "TrigerringTesla"),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                    // TriggeringTeslaEventArgs ev = new(player, tesla);
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new CodeInstruction(OpCodes.Ldloc_S, teslaGate.LocalIndex),
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringTeslaEventArgs))[0]),
                    new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnTriggeringTesla(ev);
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTriggeringTesla))),

                    // if (ev.IsAllowed && !isTriggerable)
                    //      isTriggerable = ev.IsAllowed;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, isAllowed),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Brtrue, isAllowed),
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Stloc_3),

                    // if (ev.IsInIdleRange && !inIdleRange)
                    //      inIdleRange = ev.IsInIdleRange;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(isAllowed),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsInIdleRange))),
                    new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Brtrue_S, returnLabel),
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsInIdleRange))),
                    new CodeInstruction(OpCodes.Stloc_2),
                    new CodeInstruction(OpCodes.Nop).WithLabels(returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                Log.Info($"{newInstructions[z].opcode}: {newInstructions[z].operand}");
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
