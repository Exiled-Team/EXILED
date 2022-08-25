// -----------------------------------------------------------------------
// <copyright file="Reporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches
    ///     <see
    ///         cref="CheaterReport.IssueReport(GameConsoleTransmission, string, string, string, string, string, string, ref string, ref byte[], string, int, string, string)" />
    ///     and <see cref="CheaterReport.UserCode_CmdReport(int, string, byte[], bool)" />.
    ///     Adds the <see cref="Server.ReportingCheater" /> and <see cref="Server.LocalReporting" /> events.
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.UserCode_CmdReport))]
    internal static class Reporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(LocalReportingEventArgs));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(ReportingCheaterEventArgs));

            const int offset = -4;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_7);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_3),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LocalReportingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnLocalReporting))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(LocalReportingEventArgs), nameof(LocalReportingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(LocalReportingEventArgs), nameof(LocalReportingEventArgs.Reason))),
                new(OpCodes.Starg_S, 2),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_3).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldsfld, Field(typeof(ServerConsole), nameof(ServerConsole.Port))),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReportingCheaterEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnReportingCheater))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReportingCheaterEventArgs), nameof(ReportingCheaterEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ReportingCheaterEventArgs), nameof(ReportingCheaterEventArgs.Reason))),
                new(OpCodes.Starg_S, 2),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
