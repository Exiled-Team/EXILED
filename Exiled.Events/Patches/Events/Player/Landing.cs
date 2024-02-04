// -----------------------------------------------------------------------
// <copyright file="Landing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl.Thirdperson;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AnimatedCharacterModel.OnGrounded" />
    /// Adds the <see cref="Player.Landing" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.Landing))]
    [HarmonyPatch(typeof(AnimatedCharacterModel), nameof(AnimatedCharacterModel.OnGrounded))]
    internal static class Landing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(AnimatedCharacterModel), nameof(AnimatedCharacterModel.PlayFootstep)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(base.OwnerHub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(CharacterModel), nameof(CharacterModel.OwnerHub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // LandingEventArgs ev = new(Player)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LandingEventArgs))[0]),

                    // Player.OnLanding(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnLanding))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}