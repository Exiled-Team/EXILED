// -----------------------------------------------------------------------
// <copyright file="StartingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp049;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.StartingRecall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateAny))]
    internal static class StartingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ownerHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp049Role>), nameof(ScpStandardSubroutine<Scp049Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // base.CurRagdoll
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(RagdollAbilityBase<Scp049Role>), nameof(RagdollAbilityBase<Scp049Role>.CurRagdoll))),

                    // flag
                    new(OpCodes.Ldloc_1),

                    // StartingRecallEventArgs ev = new(Player, Player, BasicRagdoll, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnStartingRecall(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnStartingRecall))),

                    // flag = ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgs), nameof(StartingRecallEventArgs.IsAllowed))),
                    new(OpCodes.Stloc_1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}