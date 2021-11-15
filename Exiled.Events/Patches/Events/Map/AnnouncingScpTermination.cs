// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(Role, PlayerStats.HitInfo, string)"/>.
    /// Adds the <see cref="Map.AnnouncingScpTermination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        // AnnouncingScpTerminationEventArgs ev = new AnnouncingScpTerminationEventArgs(string.IsNullOrEmpty(hit.Attacker) ? null : API.Features.Player.Get(hit.Attacker), scp, hit, groupId);
        //
        // Map.OnAnnouncingScpTermination(ev);
        //
        // if (!ev.IsAllowed) return;
        //
        // hit = ev.HitInfo;
        // groupId = ev.TerminationCause;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingScpTerminationEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            Label jcc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.Attacker))),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                new CodeInstruction(OpCodes.Brtrue_S, jcc),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.Attacker))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldnull).WithLabels(jcc),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.HitInfo))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                new CodeInstruction(OpCodes.Starg_S, 2),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
