// -----------------------------------------------------------------------
// <copyright file="Footstep.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerRoles.PlayableScps.Scp939.Ripples;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FootstepRippleTrigger.OnFootstepPlayed" />
    ///     to add the <see cref="Scp939.FootstepPlayed" /> event.
    /// </summary>
    [HarmonyPatch(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.OnFootstepPlayed))]
    internal static class Footstep
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
                {
                    // Player player = Player.Get(hub);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))),
                    new(OpCodes.Call, Method(typeof(Exiled.API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // true;
                    new(OpCodes.Ldc_I4_1),

                    // OnFootstepPlayedEventArgs ev = (player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FootstepEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp049.OnFootstepPlayed(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp939), nameof(Handlers.Scp939.OnFootstepPlayed))),

                    // if (!IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FootstepEventArgs), nameof(FootstepEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
