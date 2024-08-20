// -----------------------------------------------------------------------
// <copyright file="PlayingAudioLog.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using MapGeneration.Spawnables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="AudioLog.ServerInteract" />.
    /// Adds the <see cref="Handlers.Player.PlayingAudioLog" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PlayingAudioLog))]
    [HarmonyPatch(typeof(AudioLog), nameof(AudioLog.ServerInteract))]
    internal static class PlayingAudioLog
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player player = Player.Get(ReferenceHub);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PlayingAudioLogEventArgs ev = new(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlayingAudioLogEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnPlayingAudioLog(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPlayingAudioLog))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayingAudioLogEventArgs), nameof(PlayingAudioLogEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}