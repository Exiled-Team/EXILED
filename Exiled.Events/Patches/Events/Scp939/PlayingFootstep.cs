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
            Label continueLabel = generator.DefineLabel();

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Callvirt && i.Calls(PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))));
            index += -7;

            newInstructions.RemoveRange(index, 7);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // if (base.CheckVisibility(model.OwnerHub))
                //    return;
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))),
                new(OpCodes.Call, Method(typeof(RippleTriggerBase), nameof(RippleTriggerBase.CheckVisibility), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Brfalse_S, continueLabel),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),

                // Player player = Player.Get(hub);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // true;
                new(OpCodes.Ldc_I4_1),

                // PlayingFootstepEventArgs ev = (player, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingFootstepEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp939.OnPlayingFootstep(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingFootstep))),

                // if (!IsAllowed)
                //      return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingFootstepEventArgs), nameof(PlayingFootstepEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // var for _syncPlayer
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
