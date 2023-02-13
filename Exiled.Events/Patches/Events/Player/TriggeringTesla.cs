// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = 1;

            // remove the reference hub Foreach
            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))));

            newInstructions.RemoveRange(index, newInstructions.FindIndex(i => i.opcode == OpCodes.Endfinally) + offset - index);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // baseTeslaGate
                    new CodeInstruction(OpCodes.Ldloc_1),

                    // inIdleRange
                    new CodeInstruction(OpCodes.Ldloca_S, 2),

                    // isTriggerable
                    new CodeInstruction(OpCodes.Ldloca_S, 3),

                    // TriggeringTesla.TriggeringTeslaEvent(BaseTeslaGate baseTeslaGate, ref bool inIdleRange, ref bool isTriggerable)
                    new CodeInstruction(OpCodes.Call, Method(typeof(TriggeringTesla), nameof(TriggeringTeslaEvent), new[] { typeof(BaseTeslaGate), typeof(bool).MakeByRefType(), typeof(bool).MakeByRefType() })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void TriggeringTeslaEvent(BaseTeslaGate baseTeslaGate, ref bool inIdleRange, ref bool isTriggerable)
        {
            TeslaGate teslaGate = TeslaGate.Get(baseTeslaGate);

            foreach (Player player in Player.List)
            {
                if (player is null || !teslaGate.CanBeIdle(player))
                    continue;

                TriggeringTeslaEventArgs ev = new(player, teslaGate);

                Handlers.Player.OnTriggeringTesla(ev);

                if (ev.DisableTesla)
                {
                    isTriggerable = false;
                    inIdleRange = false;
                    break;
                }

                if (!ev.IsAllowed)
                    continue;

                if (ev.IsTriggerable && !isTriggerable)
                    isTriggerable = ev.IsTriggerable;

                if (ev.IsInIdleRange && !inIdleRange)
                    inIdleRange = ev.IsInIdleRange;
            }
        }
    }
}
