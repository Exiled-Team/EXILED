// -----------------------------------------------------------------------
// <copyright file="Charging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp096ChargeAbility.ServerProcessCmd(NetworkReader)" />.
    ///     Adds the <see cref="Handlers.Scp096.Charging" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.Charging))]
    [HarmonyPatch(typeof(Scp096ChargeAbility), nameof(Scp096ChargeAbility.ServerProcessCmd))]
    internal static class Charging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // ChargingEventArgs ev = new ChargingEventArgs(this, Player, true);
            //
            // Handlers.Scp096.OnCharging(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChargingEventArgs ev = new(Scp096Role, Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChargingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp096.OnCharging(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnCharging))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChargingEventArgs), nameof(ChargingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}