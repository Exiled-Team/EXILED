// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate"/>.
    /// Adds the <see cref="Player.TriggeringTesla"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            LocalBuilder internalTeslaGate = generator.DeclareLocal(typeof(API.Features.TeslaGate));
            LocalBuilder referenceHub = generator.DeclareLocal(typeof(ReferenceHub));
            LocalBuilder ev = generator.DeclareLocal(typeof(TriggeringTeslaEventArgs));

            Label cdc = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(index, new[]
            {
                // referenceHub = allKey.Value
                new CodeInstruction(OpCodes.Ldloca_S, 6),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),
                new CodeInstruction(OpCodes.Stloc, referenceHub.LocalIndex),

                // teslaGate = TeslaGate.Get(global::TeslaGate)
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.TeslaGate), nameof(API.Features.TeslaGate.Get), new[] { typeof(TeslaGate) })),
                new CodeInstruction(OpCodes.Stloc_S, internalTeslaGate.LocalIndex),

                // if (!teslaGate.PlayerInIdleRange(referenceHub))
                //    return;
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldloc_S, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.PlayerInIdleRange))),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // if (!internalTeslaGate::CanBeTriggered(Player::Get(referenceHub)))
                new CodeInstruction(OpCodes.Ldloc_S, internalTeslaGate.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(API.Features.TeslaGate), nameof(API.Features.TeslaGate.CanBeTriggered))),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // Player::Get(referenceHub)
                new CodeInstruction(OpCodes.Ldloc_S, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // internalTeslaGate
                new CodeInstruction(OpCodes.Ldloc_S, internalTeslaGate.LocalIndex),

                // internalTeslaGate::PlayerInHurtRange(referenceHub)
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.TeslaGate), nameof(API.Features.TeslaGate.Base))),
                new CodeInstruction(OpCodes.Ldloc_S, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.PlayerInHurtRange))),

                // referenceHub::characterClassManager::CurClass != RoleType::Spectator && teslaGate::PlayerInRange(referenceHub) && !teslaGate::InProgress
                new CodeInstruction(OpCodes.Ldloc_S, 4),

                // var ev = new TriggeringTeslaEventArgs(player, teslaGate, bool, bool)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringTeslaEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnTriggeringTesla(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTriggeringTesla))),

                // shouldIdle = ev.IsInIdleRange;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsInIdleRange))),
                new CodeInstruction(OpCodes.Stloc_3),

                // flag = ev.IsTriggerable;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringTeslaEventArgs), nameof(TriggeringTeslaEventArgs.IsTriggerable))),
                new CodeInstruction(OpCodes.Stloc_S, 4),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
