// -----------------------------------------------------------------------
// <copyright file="PlayingVoice.cs" company="Exiled Team">
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

    using Mirror;

    using PlayerRoles.PlayableScps.Scp939.Mimicry;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MimicryRecorder.ServerProcessCmd(NetworkReader)" />
    /// to add the <see cref="Scp939.PlayingVoice" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.PlayingVoice))]
    [HarmonyPatch(typeof(MimicryRecorder), nameof(MimicryRecorder.ServerProcessCmd))]
    internal static class PlayingVoice
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder refHub = generator.DeclareLocal(typeof(ReferenceHub));

            Label ret = generator.DefineLabel();

            // base-game code compiles inside sealed hidden class for delegate, so we create own local var
            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(global::Utils.Networking.ReferenceHubReaderWriter), nameof(global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new(OpCodes.Stloc_S, refHub.LocalIndex),
            });

            offset = 0;
            index = newInstructions.FindLastIndex(i => i.IsLdarg(0)) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // this.Owner
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MimicryRecorder), nameof(MimicryRecorder.Owner))),

                // target
                new(OpCodes.Ldloc_S, refHub.LocalIndex),

                // PlayingVoiceEventArgs ev = new(...)
                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingVoiceEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnPlayingVoice))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingVoiceEventArgs), nameof(PlayingVoiceEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
