// -----------------------------------------------------------------------
// <copyright file="PlayingFootstep.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerRoles.PlayableScps.Scp939.Ripples;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FootstepRippleTrigger.OnFootstepPlayed" />
    /// to add the <see cref="Scp939.PlayingFootstep" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlayingFootstep))]
    [HarmonyPatch(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.OnFootstepPlayed))]
    internal static class PlayingFootstep
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            const int offset = 2;

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // StandardSubroutine<>.Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.Owner))),

                // model.OwnerHub
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatedCharacterModel), nameof(AnimatedCharacterModel.OwnerHub))),

                // position2
                new(OpCodes.Ldloc_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // new PlayingFootstepÈventArgs(player, target, ripplePosition, isAllowed)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingFootstepEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp939.OnPlayingFootstep(ev)
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingFootstep))),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingFootstepEventArgs), nameof(PlayingFootstepEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
