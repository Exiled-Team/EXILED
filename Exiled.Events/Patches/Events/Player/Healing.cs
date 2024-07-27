// -----------------------------------------------------------------------
// <copyright file="Healing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
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
    /// the <see cref="Handlers.Player.Healed" /> events.
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
            Label skip1 = generator.DefineLabel();
            Label skip2 = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(HealingEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder lastHealth = generator.DeclareLocal(typeof(float));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_1);

            newInstructions.InsertRange(index, new[]
            {
                // lastHealth = get_curValue;
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, lastHealth.LocalIndex),

                // player = Player.Get(this.Hub);
                // if (player is null) skip
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealthStat), nameof(HealthStat.Hub))),
                new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, skip1),

                // HealingEventArgs args = new(player, amount)
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(HealingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player::OnHealing(args)
                new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHealing))),

                // if (!args.IsAllowed) return
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealingEventArgs), nameof(HealingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Pop),
                new(OpCodes.Pop),
                new(OpCodes.Ret),

                // healAmount = args.Amount
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealingEventArgs), nameof(HealingEventArgs.Amount))),
                new(OpCodes.Starg_S, 1),

                new CodeInstruction(OpCodes.Nop).WithLabels(skip1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(skip2);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                // if (player is null) skip
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, skip2),

                // HealedEventArgs args = new(player, lastAmount)
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldloc_S, lastHealth.LocalIndex),
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(HealedEventArgs))[0]),

                // Player::OnHealed(args)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHealed))),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}