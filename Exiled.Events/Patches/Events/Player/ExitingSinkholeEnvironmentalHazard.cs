// -----------------------------------------------------------------------
// <copyright file="ExitingEnvironmentalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Patches.Fixes;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="EnvironmentalHazard.OnExit(ReferenceHub)"/> with <see cref="SinkholeEnvironmentalHazard"/>.
    /// Adds the <see cref="Handlers.Player.ExitingEnvironmentalHazard"/> event.
    /// <br>Adds the better effect logic.</br>
    /// </summary>
    /// <seealso cref="StayingOnSinkholeEnvironmentalHazard"/>
    /// <seealso cref="SinkholeEffectFix"/>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnExit))]
    internal static class ExitingSinkholeEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label cnt = generator.DefineLabel();

            // We add type check because SinkholeEnvironmentalHazard dont override OnExit method
            // Without type check, the TantrumEnvironmentalHazard::OnExit event will be called several times
            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Isinst, typeof(SinkholeEnvironmentalHazard)),
                new(OpCodes.Brfalse_S, cnt),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExitingEnvironmentalHazardEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnExitingEnvironmentalHazard))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExitingEnvironmentalHazardEventArgs), nameof(ExitingEnvironmentalHazardEventArgs.IsAllowed))),

                // If IsAllowed == false, we dont remove RefHub from AffectedPlayers, only dont removing effect
                new(OpCodes.Brfalse_S, cnt),

                // null check
                new(OpCodes.Ldarg_1),
                new(OpCodes.Brfalse_S, cnt),

                // SCP check
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                new(OpCodes.Callvirt, Method(typeof(CharacterClassManager), nameof(CharacterClassManager.IsAnyScp))),
                new(OpCodes.Brtrue_S, cnt),

                // exit effect for 1 second, cause we disable effect in OnStay
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),
                new(OpCodes.Ldc_R4, 1f),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.EnableEffect), new[] { typeof(float), typeof(bool) }, new[] { typeof(SinkHole) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(cnt);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
