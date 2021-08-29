// -----------------------------------------------------------------------
// <copyright file="LocalReporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

#pragma warning disable CS1570 // XML comment has badly formed XML

    /// <remarks>
    /// <para>
    ///     Code before patching:
    ///     <code>
    ///         if (!notifyGm) { /* ... */ }
    ///     </code>
    /// </para>
    /// <para>
    ///     IL before patching:
    ///     <code>
    ///         ldarg.s notifyGm (4)
    ///         brtrue Label13
    ///     </code>
    /// </para>
    /// <para>
    ///     IL after pathing:
    ///     <code>
    ///         ldarg.s notifyGm (4)
    ///         brtrue => Label13
    ///         ldloc.3
    ///         call static Exiled.API.Features.Player Exiled.API.Features.Player::Get(ReferenceHub referenceHub)
    ///         ldloc.2
    ///         call static Exiled.API.Features.Player Exiled.API.Features.Player::Get(ReferenceHub referenceHub)
    ///         ldarg.s 2
    ///         ldc.i4.1
    ///         newobj System.Void Exiled.Events.EventArgs.LocalReportingEventArgs::.ctor(Exiled.API.Features.Player issuer, Exiled.API.Features.Player target, System.String reason, System.Boolean isAllowed)
    ///         dup
    ///         stloc.s 5
    ///         call static System.Void Exiled.Events.Handlers.Server::OnLocalReporting(Exiled.Events.EventArgs.LocalReportingEventArgs ev)
    ///         ldloc.s 5
    ///         ldloc.0
    ///         ldflda System.String<>c__DisplayClass14_0::reason
    ///         call static System.Void Exiled.Events.Patches.Events.Server.LocalReporting::SuperReasonReplacer(Exiled.Events.EventArgs.LocalReportingEventArgs ev, ref System.String reason)
    ///         ldloc.s 5
    ///         ldarga.s 2
    ///         call static System.Void Exiled.Events.Patches.Events.Server.LocalReporting::SuperReasonReplacer(Exiled.Events.EventArgs.LocalReportingEventArgs ev, ref System.String reason)
    ///         ldloc.s 5
    ///         call System.String Exiled.Events.EventArgs.LocalReportingEventArgs::get_Reason()
    ///         ldc.i4.1
    ///         call static System.Void Exiled.API.Features.Log::Debug(System.Object message, System.Boolean canBeSent)
    ///         ldloc.0
    ///         ldfld System.String<>c__DisplayClass14_0::reason
    ///         ldc.i4.1
    ///         call static System.Void Exiled.API.Features.Log::Debug(System.Object message, System.Boolean canBeSent)
    ///         ldarg.s 2
    ///         ldc.i4.1
    ///         call static System.Void Exiled.API.Features.Log::Debug(System.Object message, System.Boolean canBeSent)
    ///         ldloc.s 5
    ///         call System.Boolean Exiled.Events.EventArgs.LocalReportingEventArgs::get_IsAllowed()
    ///         brtrue => Label17
    ///         br => Label27
    ///         Label17
    ///     </code>
    /// </para>
    /// <para>
    ///     Code after patching:
    ///     <code>
    ///         if (!notifyGm)
    ///         {
    ///             var ev = new LocalReportingEventArgs(Player.Get(reporter: ReferenceHub), Player.Get(reported: ReferenceHub), reason: string, true: bool);
    ///             Exiled.Events.Server.OnLocalReporting(ev);
    ///             Exiled.Events.Patches.Events.Server.LocalReporting.SuperReasonReplacer(ev, ref reason); // nested type field
    ///             Exiled.Events.Patches.Events.Server.LocalReporting.SuperReasonReplacer(ev, ref reason); // method arg
    ///             if (!ev.IsAllowed)
    ///                 return;
    ///         }
    ///     </code>
    /// </para>
    /// </remarks>
    /// <summary>
    /// Method decompiled by dnSpy v6.1.1 .NET Core.
    /// A call to our <see cref="Handlers.Server.OnLocalReporting"/> method
    /// is inserted into it which returns the bool value that determines further processing of the report.
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.UserCode_CmdReport), typeof(int), typeof(string), typeof(byte[]), typeof(bool))]
#pragma warning restore CS1570 // XML comment has badly formed XML
#pragma warning disable SA1604 // Element documentation should have summary
    internal static class LocalReporting
#pragma warning restore SA1604 // Element documentation should have summary
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Moving 2 indexes forward skipping the access itself and 'brtrue'
            const sbyte skipOpcodes = 2;

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int patchIndex = newInstructions.FindLastIndex((instr) => instr.opcode == OpCodes.Ldarg_S && (byte)instr.operand == 4);

#if DEBUG
            LogL($"Patch index: {patchIndex}");
#endif

            if (patchIndex == -1)
            {
                API.Features.Log.Warn($"{nameof(LocalReporting)}-{nameof(Transpiler)}: Couldn't find index for patching!");
                yield break;
            }

            patchIndex += skipOpcodes;

            Label retEnd = generator.DefineLabel();
            newInstructions[patchIndex].WithLabels(retEnd);

            MethodInfo callPlayerGet = AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) });
            ConstructorInfo newLocalReportingEventArgs = AccessTools.Constructor(
                typeof(Exiled.Events.EventArgs.LocalReportingEventArgs),
                new[] { typeof(API.Features.Player), typeof(API.Features.Player), typeof(string), typeof(bool) });

            LocalBuilder locLocalReportEventArgs = generator.DeclareLocal(typeof(Exiled.Events.EventArgs.LocalReportingEventArgs));
            int locIndexLocalReportEventArgs = locLocalReportEventArgs.LocalIndex;
            const byte argIndexReason = 2;

            MethodInfo callOnLocalReporting = AccessTools.Method(typeof(Handlers.Server), nameof(Handlers.Server.OnLocalReporting), new[] { typeof(Exiled.Events.EventArgs.LocalReportingEventArgs) });
            MethodInfo callLocalReportingEventArgsGETIsAllowed = AccessTools.PropertyGetter(typeof(Exiled.Events.EventArgs.LocalReportingEventArgs), nameof(Exiled.Events.EventArgs.LocalReportingEventArgs.IsAllowed));
            MethodInfo callLocalReportingEventArgsGETReason = AccessTools.PropertyGetter(typeof(Exiled.Events.EventArgs.LocalReportingEventArgs), nameof(Exiled.Events.EventArgs.LocalReportingEventArgs.Reason));
            MethodInfo callSuperReasonReplacer = AccessTools.Method(typeof(LocalReporting), nameof(SuperReasonReplacer), new[] { typeof(Exiled.Events.EventArgs.LocalReportingEventArgs), typeof(string).MakeByRefType() });

            Type internalReportDataRootClass = typeof(CheaterReport);
            const string internalReportDataNestedClassName = "<>c__DisplayClass13_0";
            const string internalReportDataNestedClassReasonFieldName = "reason";

            Type internalReportDataNestedClass = internalReportDataRootClass.GetNestedType(internalReportDataNestedClassName, BindingFlags.NonPublic);
            FieldInfo fieldReason = AccessTools.Field(internalReportDataNestedClass, internalReportDataNestedClassReasonFieldName);
#if DEBUG
            void LogL(string value) =>
                API.Features.Log.Debug($"{nameof(LocalReporting)}-{nameof(LocalReporting.Transpiler)}: {value}", true);

            LogL($"{nameof(callPlayerGet)} is null: {callPlayerGet == null}");
            LogL($"{nameof(newLocalReportingEventArgs)} is null: {newLocalReportingEventArgs == null}");
            LogL($"{nameof(callOnLocalReporting)} is null: {callOnLocalReporting == null}");
            LogL($"{nameof(callLocalReportingEventArgsGETIsAllowed)} is null: {callLocalReportingEventArgsGETIsAllowed == null}");
            LogL($"{nameof(callLocalReportingEventArgsGETReason)} is null: {callLocalReportingEventArgsGETReason == null}");
            LogL($"{nameof(internalReportDataNestedClass)} is null: {internalReportDataNestedClass == null}");
            LogL($"{nameof(fieldReason)} is null: {fieldReason == null}");
            LogL($"{nameof(fieldReason)} is called '{fieldReason?.Name}'");
            if (internalReportDataNestedClass == null)
            {
                LogL($"I see, the {nameof(internalReportDataNestedClass)} is null, trying to find the {nameof(internalReportDataNestedClassReasonFieldName)} field in all nested types...");
                Type[] nestedTypes = internalReportDataNestedClass.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
                foreach (Type nestedType in nestedTypes)
                {
                    FieldInfo reasonField = nestedType.GetField(internalReportDataNestedClassReasonFieldName);
                    if (reasonField != null)
                    {
                        LogL($"Found the {nameof(internalReportDataNestedClassReasonFieldName)}! It's {reasonField.Name} in the {nestedType.FullName}!");
                    }
                }
            }
#endif

#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(patchIndex, new[]
            {
                // Issuer: Player
                new CodeInstruction(OpCodes.Ldloc_3), // reporter: ReferenceHub
                new CodeInstruction(OpCodes.Call, callPlayerGet),

                // Target: Player
                new CodeInstruction(OpCodes.Ldloc_2), // reported: ReferenceHub
                new CodeInstruction(OpCodes.Call, callPlayerGet),

                // Reason: string
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, fieldReason),

                // IsAllowed: bool
                new CodeInstruction(OpCodes.Ldc_I4_1), // true: bool

                // Creating object
                new CodeInstruction(OpCodes.Newobj, newLocalReportingEventArgs),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, locIndexLocalReportEventArgs),

                // Invoking the event
                new CodeInstruction(OpCodes.Call, callOnLocalReporting),

                // Replacing the reason

                // Replacing the reason in the nested type
                new CodeInstruction(OpCodes.Ldloc_S, locIndexLocalReportEventArgs),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldflda, fieldReason),
                new CodeInstruction(OpCodes.Call, callSuperReasonReplacer),

                // Replacing the reason in the arg
                new CodeInstruction(OpCodes.Ldloc_S, locIndexLocalReportEventArgs),
                new CodeInstruction(OpCodes.Ldarga_S, argIndexReason),
                new CodeInstruction(OpCodes.Call, callSuperReasonReplacer),

#if DEBUG
                new CodeInstruction(OpCodes.Ldstr, "Final nested type reason: "),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, fieldReason),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Log), nameof(API.Features.Log.Debug))),

                new CodeInstruction(OpCodes.Ldstr, "Final arg reason: "),
                new CodeInstruction(OpCodes.Ldarg_S, argIndexReason),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Log), nameof(API.Features.Log.Debug))),
#endif

                // Checking for availability to continue
                new CodeInstruction(OpCodes.Ldloc_S, locIndexLocalReportEventArgs),
                new CodeInstruction(OpCodes.Call, callLocalReportingEventArgsGETIsAllowed),
                new CodeInstruction(OpCodes.Brtrue_S, retEnd),
                new CodeInstruction(OpCodes.Ret),
            });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        // Try to replace the reason as a field,
        // it didn't work for me
        private static void SuperReasonReplacer(Exiled.Events.EventArgs.LocalReportingEventArgs ev, ref string reason) => reason = ev.Reason;
    }
}
