// -----------------------------------------------------------------------
// <copyright file="LocalReporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <remarks>
    ///     Code before patching:
    ///     <code>
    ///         57: if (!notifyGm)
    ///         58: {
    ///         59:     ....
    ///         **: }
    ///     </code>
    ///     Code after pathing:
    ///     <code>
    ///         57: if (!notifyGm)
    ///         58: {
    ///         59:    if (!InvokeLocalReport(reporter: CheaterReport, reportedTo: GameObject, reason: string))
    ///         60:        return;
    ///         61:     ...
    ///         **: }
    ///     </code>
    ///
    ///     Line numbering is shown, as are names of variables with object types.
    /// </remarks>
    /// <summary>
    /// The patch is located on line 57 of the <see cref="CheaterReport.CallCmdReport(int, string, byte[], bool)"/> method that decompiled by dnSpy v6.1.1 .NET Core.
    /// A call to our <see cref="Server.OnLocalReporting(LocalReportingEventArgs)"/> method
    /// is inserted into it which returns the bool value that determines further processing of the report.
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.CallCmdReport), typeof(int), typeof(string), typeof(byte[]), typeof(bool))]
    internal static class LocalReporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Creating a list to use LastIndexOf
            var list = new List<CodeInstruction>(instructions);

            // Finding the last access to the fourth argument 'notifyGm'
            // Moving 2 indexes forward skipping the access itself and 'brtrue'
            var lastNotifyGm = list.FindLastIndex(ci => ci.opcode == OpCodes.Ldarg_S && (byte)ci.operand == 4) + 2;

            // If not found, then do not touch anything
            if (lastNotifyGm < 1)
                return list;

#if DEBUG
            var lastNotifyGmInstruction = list[lastNotifyGm];

            // Debugging for the developer
            // It's important that the method was used before calling the log to the console
            API.Features.Log.Error($"LocalReportEvent ->>>>>>>>>>> {lastNotifyGm}: {lastNotifyGmInstruction.opcode} - {lastNotifyGmInstruction.operand}");
#endif

            // Get a label that can be used to return the value safely,
            // if the condition is equal to.
            // Actually equal to {}
            var retEnd = generator.DefineLabel();
            list[lastNotifyGm].labels.Add(retEnd);

            // this - issuer
            // GameObject - target
            // string - reason
            // InvokeLocalReport()
            // if (true)
            // return
            list.InsertRange(lastNotifyGm, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldarga_S, 2),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(LocalReporting), nameof(InvokeLocalReport), new[] { typeof(CheaterReport), typeof(GameObject), typeof(string).MakeByRefType() })),
                new CodeInstruction(OpCodes.Brtrue_S, retEnd),
                new CodeInstruction(OpCodes.Ret),
            });

            return list;
        }

        // Calling the method with bool is easier
        // It contains the code that processes the event
        private static bool InvokeLocalReport(CheaterReport reporter, GameObject reportedTo, ref string reason)
        {
            var issuer = API.Features.Player.Get(reporter.gameObject);
            var target = API.Features.Player.Get(reportedTo);

            var ev = new LocalReportingEventArgs(issuer, target, reason);

            Server.OnLocalReporting(ev);

            reason = ev.Reason;

            // If further processing isn't allowed
            // then send the success of sending the report
            // --------------------------------------------------
            // gameObjects is compared cuz there is no comparison overload for Player,
            // it can be different objects
            if (!ev.IsAllowed && reporter.gameObject != reportedTo.gameObject)
            {
                reporter.GetComponent<GameConsoleTransmission>().SendToClient(
                    reporter.connectionToClient,
                    "[Reporting] Player report successfully sent to local administrators.",
                    "green");
            }

            return ev.IsAllowed;
        }
    }
}