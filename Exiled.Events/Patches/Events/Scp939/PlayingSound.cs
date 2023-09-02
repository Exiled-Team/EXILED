// -----------------------------------------------------------------------
// <copyright file="PlayingSound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;

    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp939AmnesticCloudAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.PlayingSound" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlayingSound))]
    [HarmonyPatch(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.ServerProcessCmd))]
    internal static class PlayingSound
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new();

            LocalBuilder option = generator.DeclareLocal(typeof(byte));
            LocalBuilder isReady = generator.DeclareLocal(typeof(bool));
            LocalBuilder ev = generator.DeclareLocal(typeof(PlayingSoundEventArgs));

            Label ret = generator.DefineLabel();

            // byte option = reader.ReadByte();
            // PlayingSoundEventArgs ev = new(Player::Get(Owner), Sequences[option], Cooldown.IsReady, _activationCooldown, Cooldown.IsReady);
            // Scp939.OnPlayingSound(ev);
            // if (ev.IsReady && ev.IsAllowed)
            // {
            //     _syncOption = option;
            //     Cooldown.Trigger(ev.Cooldown);
            //     ServerSendRpc(true);
            // }
            newInstructions.AddRange(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(NetworkReader), nameof(NetworkReader.ReadByte))),
                new(OpCodes.Stloc_S, option.LocalIndex),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Sequences))),

                new(OpCodes.Ldloc_S, option.LocalIndex),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Cooldown))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, isReady.LocalIndex),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry._activationCooldown))),

                new(OpCodes.Ldloc_S, isReady.LocalIndex),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingSoundEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingSound))),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingSoundEventArgs), nameof(PlayingSoundEventArgs.IsReady))),
                new(OpCodes.Brfalse_S, ret),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingSoundEventArgs), nameof(PlayingSoundEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, option.LocalIndex),
                new(OpCodes.Stfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry._syncOption))),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Cooldown))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingSoundEventArgs), nameof(PlayingSoundEventArgs.Cooldown))),
                new(OpCodes.Callvirt, Method(typeof(AbilityCooldown), nameof(AbilityCooldown.Trigger))),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, Method(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.ServerSendRpc))),

                new CodeInstruction(OpCodes.Ret).WithLabels(ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }
}
