// -----------------------------------------------------------------------
// <copyright file="FindingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049SenseAbility.CanFindTarget(out ReferenceHub)"/>
    /// to add <see cref="Scp049.FindingTarget"/> event.
    /// </summary>
    [EventPatch(typeof(Scp049), nameof(Scp049.FindingTarget))]
    [HarmonyPatch(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.CanFindTarget))]
    internal class FindingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(HashSet<ReferenceHub>.Enumerator), nameof(HashSet<ReferenceHub>.Enumerator.MoveNext))));
            Label continueLabel = newInstructions[index - 1].labels[0];

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_S && i.operand is byte and 12);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // player
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp049SenseAbility), nameof(Scp049SenseAbility.Owner))),

                // target
                new(OpCodes.Ldloc_S, 6),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // FindingTargetEventArgs ev = new(player, target)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FindingTargetEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp049.OnFindingTarget(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFindingTarget))),

                // if (!ev.IsAllowed)
                //    continue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(FindingTargetEventArgs), nameof(FindingTargetEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}