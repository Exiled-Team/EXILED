// -----------------------------------------------------------------------
// <copyright file="SenseAbilityReducedCooldown.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.Scp079API
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features;

    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using BaseScp049Role = PlayerRoles.PlayableScps.Scp049.Scp049Role;
    using Scp049Role = API.Features.Roles.Scp049Role;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.ServerLoseTarget"/>.
    /// Adds the <see cref="Scp049Role.SenseAbilityReducedCooldown" /> property.
    /// </summary>
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.ServerLoseTarget))]
    internal class SenseAbilityReducedCooldown
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder scp049Role = generator.DeclareLocal(typeof(Scp049Role));

            // replace "this.Duration.Trigger(40.0)" with "this.Duration.Trigger((Player.Get(base.Owner).Role as Scp049Role).SenseAbilityReducedCooldown)"
            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.operand == (object)Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner).Role
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<BaseScp049Role>), nameof(ScpStandardSubroutine<BaseScp049Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),

                    // (Player.Get(base.Owner).Role as Scp049Role).SenseAbilityReducedCooldown
                    new(OpCodes.Isinst, typeof(Scp049Role)),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049Role), nameof(Scp049Role.SenseAbilityReducedCooldown))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}