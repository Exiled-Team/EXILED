// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.Events.Handlers.Player;
    using TeslaGate = TeslaGate;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate"/>.
    /// Adds the <see cref="Handlers.Player.TriggeringTesla"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_3) + offset;
            const int instructionsToRemove = 25;
            LocalBuilder referenceHub = generator.DeclareLocal(typeof(ReferenceHub));
            LocalBuilder ev = generator.DeclareLocal(typeof(TriggeringTeslaEventArgs));
            Label returnLabel = generator.DefineLabel();

            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldstr, "Player triggering thing"),
                new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Error), new[] { typeof(string) })),

                // referenceHub = allKey.Value
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldloca_S, 6),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, referenceHub.LocalIndex),

                // if (!teslaGate.PlayerInIdleRange(referenceHub))
                //    return;
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.PlayerInIdleRange))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // player.Get(referenceHub);
                new CodeInstruction(OpCodes.Ldloc, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // teslaGate
                new CodeInstruction(OpCodes.Ldloc_2),

                // teslaGate.PlayerInHurtRange()
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldloc, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.PlayerInHurtRange))),

                // teslaGate.PlayerInRange()
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldloc, referenceHub.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TeslaGate), nameof(TeslaGate.PlayerInRange))),

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

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
