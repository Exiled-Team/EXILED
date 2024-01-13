// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.DamageHandlers;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches
    /// <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(ReferenceHub, PlayerStatsSystem.DamageHandlerBase)" />.
    /// Adds the <see cref="Map.AnnouncingScpTermination" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.AnnouncingScpTermination))]
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingScpTerminationEventArgs));

            Label ret = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_0) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(scp)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // hit
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // AnnouncingScpTerminationEventArgs ev = new(Player, DamageHandlerBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnAnnouncingScpTermination(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // hit = ev.DamageHandler.Base
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.DamageHandler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CustomDamageHandler), nameof(CustomDamageHandler.Base))),
                    new(OpCodes.Starg, 1),

                    // announcement = ev.TerminationCause
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}