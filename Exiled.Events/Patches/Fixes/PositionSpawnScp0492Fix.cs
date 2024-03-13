// -----------------------------------------------------------------------
// <copyright file="PositionSpawnScp0492Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.Ragdolls;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049ResurrectAbility.ServerComplete"/> delegate.
    /// Fix bug where Scp0492 respawn at wrong place partially fix nw bug (https://trello.com/c/T1P333XK/5482-scp049-able-to-revive-old-player-corpse?filter=SCP049).
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
    internal static class PositionSpawnScp0492Fix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int toRemove = 4;

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Component), nameof(Component.transform)))) + offset;

            // replace "ownerHub.transform.position = base.CastRole.FpcModule.Position;"
            // with "ownerHub.transform.position = base.CurRagdoll.Info.StartPosition;"
            newInstructions.RemoveRange(index, toRemove);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(RagdollAbilityBase<Scp049Role>), nameof(RagdollAbilityBase<Scp049Role>.CurRagdoll))),
                new(OpCodes.Ldflda, Field(typeof(BasicRagdoll), nameof(BasicRagdoll.Info))),
                new(OpCodes.Ldfld, Field(typeof(RagdollData), nameof(RagdollData.StartPosition))),
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
