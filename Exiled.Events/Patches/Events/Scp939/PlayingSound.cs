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

    using Exiled.API.Features.Pools;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
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
    [HarmonyPatch(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.ServerProcessCmd))]
    internal static class PlayingSound
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder option = generator.DeclareLocal(typeof(byte));

            Label ret = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.operand == (object)PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // option = reader.ReadByte()
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(NetworkReader), nameof(NetworkReader.ReadByte))),
                new(OpCodes.Stloc_S, option.LocalIndex),

                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Owner))),

                // EnvironmentalMimicry.Sequences[option]
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Sequences))),
                new(OpCodes.Ldloc, option.LocalIndex),
                new(OpCodes.Ldelem_Ref),

                // this.Cooldown.IsReady
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry.Cooldown))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))),

                // this._activationCooldown
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(EnvironmentalMimicry), nameof(EnvironmentalMimicry._activationCooldown))),

                // true
                new(OpCodes.Ldc_I4_1),

                // PlayingSoundEventArgs ev = new(referenceHub, sound, isReady, cooldown, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingSoundEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp939.OnPlayingSound(ev);
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingSound))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(LungingEventArgs), nameof(LungingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            offset = -1;
            index = newInstructions.FindIndex(i => i.operand == (object)PropertyGetter(typeof(NetworkReader), nameof(NetworkReader.ReadByte))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.Insert(index, new(OpCodes.Ldloc, option.LocalIndex));

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
