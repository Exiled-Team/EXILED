// -----------------------------------------------------------------------
// <copyright file="Slapped.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp3114Slap.DamagePlayers" />.
    /// Adds the <see cref="Handlers.Scp3114.Slapped" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Slapped))]
    [HarmonyPatch(typeof(Scp3114Slap), nameof(Scp3114Slap.DamagePlayers))]
    internal class Slapped
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(instruction =>
                instruction.opcode == OpCodes.Ret);

            Label label = generator.DefineLabel();

            newInstructions[index] = new CodeInstruction(OpCodes.Br, label);

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // Player::Get(this.Owner)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(label).MoveLabelsFrom(newInstructions[newInstructions.Count - 1]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Slap), nameof(Scp3114Slap.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // base._syncAttack
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ScpAttackAbilityBase<Scp3114Role>), nameof(ScpAttackAbilityBase<Scp3114Role>._syncAttack))),

                // Player::Get(primaryTarget)
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // SlappedEventArgs ev = new(player, AttackResult, target);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SlappedEventArgs))[0]),

                // Handlers.Scp3114.OnSlapped(ev)
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnSlapped))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
