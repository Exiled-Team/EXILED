// -----------------------------------------------------------------------
// <copyright file="MakingNoise.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl.Thirdperson;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AnimatedCharacterModel.PlayFootstep()"/>
    /// to add <see cref="Handlers.Player.MakingNoise"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.MakingNoise))]
    [HarmonyPatch(typeof(AnimatedCharacterModel), nameof(AnimatedCharacterModel.PlayFootstep))]
    internal class MakingNoise
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(MakingNoiseEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Callvirt, PropertyGetter(typeof(PlayerEffectsController), nameof(PlayerEffectsController.AllEffects)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(thus.OwnerHub);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnimatedCharacterModel), nameof(AnimatedCharacterModel.OwnerHub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // MakingNoiseEventArgs ev = new(player, this, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(MakingNoiseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnMakingNoise(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnMakingNoise))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(MakingNoiseEventArgs), nameof(MakingNoiseEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // distance = ev.Distance;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(MakingNoiseEventArgs), nameof(MakingNoiseEventArgs.Distance))),
                    new(OpCodes.Stloc_0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}