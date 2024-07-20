// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp173;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp173;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp173SnapAbility.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Scp173.Attacking" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.Attacking))]
    [HarmonyPatch(typeof(Scp173SnapAbility), nameof(Scp173SnapAbility.ServerProcessCmd))]
    internal static class Attacking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            // Let's locate the first instruction after the first return
            const int offset = 4;
            int index = newInstructions.FindIndex(instruction => instruction.OperandIs(GetDeclaredConstructors(typeof(Scp173SnapPlayerEvent))[0])) + offset;

            // Insert the new instructions in the index location
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Scp173SnapAbility::Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp173SnapAbility), nameof(Scp173SnapAbility.Owner))),

                // Player::Get(Scp173SnapAbility::Owner)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player::Get(Scp173SnapAbility::_targetHub)
                new(OpCodes.Ldfld, Field(typeof(Scp173SnapAbility), nameof(Scp173SnapAbility._targetHub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // AttackingEventArgs args = new(player, target, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp173::OnAttacking(args)
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnAttacking))),

                // if (!args.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}