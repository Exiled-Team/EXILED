// -----------------------------------------------------------------------
// <copyright file="Healing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerStatsSystem;

    /// <summary>
    /// Patches
    /// <see cref="HealthStat.ServerHeal(float)" />.
    /// Adds the <see cref="Handlers.Player.Healing" /> and
    /// Adds the <see cref="Handlers.Player.Healed" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Healing))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Healed))]
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ServerHeal))]
    internal static class Healing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(HealingEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder lastHealth = generator.DeclareLocal(typeof(float));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_1);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // lastHealth = get_curValue;
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, lastHealth.LocalIndex),

                // player = Player.Get(this.Hub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealthStat), nameof(HealthStat.Hub))),
                new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new Type[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // HealingEventArgs ev = new(Player, amount)
                new(OpCodes.Ldloc_S, player),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(HealingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // OnHealing(ev)
                new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHealing))),

                // if (!ev.IsAllowed)
                //   return
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealingEventArgs), nameof(HealingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Pop),
                new(OpCodes.Pop),
                new(OpCodes.Ret),

                // healAmount = ev.Amount
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealingEventArgs), nameof(HealingEventArgs.Amount))),
                new(OpCodes.Starg_S, 1),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // HealedEventArgs ev = new(Player, lastAmount)
                new CodeInstruction(OpCodes.Ldloc_S, player),
                new CodeInstruction(OpCodes.Ldloc_S, lastHealth),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(HealedEventArgs))[0]),

                // OnHealed(ev)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHealed))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}