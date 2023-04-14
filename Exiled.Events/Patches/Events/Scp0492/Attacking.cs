// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp0492
{
    using Exiled.Events.EventArgs.Scp0492;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using RelativePositioning;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features.Pools;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.Subroutines;
    using Mirror;

    /// <summary>
    /// Patches <see cref="ScpAttackAbilityBase{T}.ServerProcessCmd(NetworkReader)"/>
    /// to add <see cref="Attacking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ScpAttackAbilityBase<ZombieRole>), nameof(ScpAttackAbilityBase<ZombieRole>.ServerProcessCmd))]
    internal class Attacking
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label ret = generator.DefineLabel();

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(x => x.Calls(AccessTools.PropertyGetter(typeof(RelativePosition), nameof(RelativePosition.Position)))) + offset;

            newInstructions[index + 1].WithLabels(ret);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ScpStandardSubroutine<ZombieRole>), nameof(ScpStandardSubroutine<ZombieRole>.Owner))),

                    new(OpCodes.Ldloc_S, 4),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(HurtingEventArgs))[0]),
                    new(OpCodes.Dup),

                    new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnHurting))),

                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, ret),
                });

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
