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

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(Role, PlayerStats.HitInfo, string)"/>.
    /// Adds the <see cref="Map.AnnouncingScpTermination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 0;
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldsfld) + offset;
            var returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.RHub))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject), })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
            {
                API.Features.Log.Debug("SCP LOGS");
                API.Features.Log.Debug(z);
                API.Features.Log.Debug(newInstructions[z]);
                API.Features.Log.Debug(newInstructions[z].operand);
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /*private static bool Prefix(Role scp, ref PlayerStats.HitInfo hit, ref string groupId)
        {
            AnnouncingScpTerminationEventArgs ev = new AnnouncingScpTerminationEventArgs(string.IsNullOrEmpty(hit.Attacker) ? null : API.Features.Player.Get(hit.Attacker), scp, hit, groupId);

            Map.OnAnnouncingScpTermination(ev);

            hit = ev.HitInfo;
            groupId = ev.TerminationCause;

            return ev.IsAllowed;
        }*/
    }
}
