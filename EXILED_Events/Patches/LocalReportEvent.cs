using EXILED.Extensions;
using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace EXILED.Patches
{
    /// <remarks>
    ///     The patch is located on line 263 of the <see cref="CheaterReport.CallCmdReport(int, string, byte[], bool)"/> method.
    ///     A call to our <see cref="InvokeLocalReport(CheaterReport, GameObject, string)"/> method 
    ///     is inserted into it which returns the bool value that determines further processing of the report.
    ///     
    ///     
    ///     Code before patching:
    ///     <code>
    ///         261: if (!notifyGm) 
    ///         262: {
    ///         263: ....
    ///     </code>
    ///     Code after pathing:
    ///     <code>
    ///         261: if (!notifyGm) 
    ///         262: {
    ///         263:    if (!InvokeLocalReport(reporter: CheaterReport, reportedTo: GameObject, reason: string))
    ///         264:        return;
    ///         265: }
    ///     </code>
    ///     
    ///     Line numbering is shown, as are names of variables with object types.
    /// </remarks>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.CallCmdReport),
        typeof(int), typeof(string), typeof(byte[]), typeof(bool))]
    public static class LocalReportEvent
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
            Log.Error($"LocalReportEvent ->>>>>>>>>>> {lastNotifyGm}: {lastNotifyGmInstruction.opcode} - {lastNotifyGmInstruction.operand}");
#endif
            // Get a label that can be used to return the value safely, 
            // if the condition is equal to.
            // Actually equal to {}
            var retEnd = generator.DefineLabel();
            list[lastNotifyGm].labels.Add(retEnd);
            list.InsertRange(lastNotifyGm, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),                   // this - issuer
                new CodeInstruction(OpCodes.Ldloc_2),                   // GameObject - target
                new CodeInstruction(OpCodes.Ldarg_2),                   // string - reason
                new CodeInstruction(OpCodes.Call, invokeLocalReport),   // InvokeLocalReport()
                new CodeInstruction(OpCodes.Brtrue_S, retEnd),          // if (true)
                new CodeInstruction(OpCodes.Ret)                        // return
            });

            return list;
        }

        // Reference to the method being called
        private static readonly MethodInfo invokeLocalReport = typeof(LocalReportEvent).GetMethod("InvokeLocalReport", BindingFlags.NonPublic | BindingFlags.Static);

        // Calling the method with bool is easier
        // It contains the code that processes the event
        private static bool InvokeLocalReport(CheaterReport reporter, GameObject reportedTo, string reason)
        {
            var issuer = reporter.gameObject.GetPlayer();
            var target = Player.GetPlayer(reportedTo);
            bool allow = true;
            Events.InvokeLocalReport(issuer, target, reason, ref allow);
            if (!allow)
                // If further processing isn't allowed
                // then send the success of sending the report
                reporter.GetComponent<GameConsoleTransmission>().SendToClient(reporter.connectionToClient, "[REPORTING] Player report successfully sent to local administrators.", "green");
            return allow;
        }
    }
}
