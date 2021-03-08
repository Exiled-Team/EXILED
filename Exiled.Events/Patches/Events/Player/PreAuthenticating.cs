// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using LiteNetLib;
    using LiteNetLib.Utils;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)"/>.
    /// Adds the <see cref="Handlers.Player.PreAuthenticating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest), typeof(ConnectionRequest))]
    internal static class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            var offset = -1;

            // Search for the last "request.Accept()" and then removes the offset, to get "ldarg.1" index.
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt
            && (MethodInfo)instruction.operand == Method(typeof(ConnectionRequest), nameof(ConnectionRequest.Accept))) + offset;

            // Declare a string local variable.
            var failedMessage = generator.DeclareLocal(typeof(string));

            // Define an else label.
            var elseLabel = generator.DefineLabel();

            // Search for the operand of the last "br.s".
            var returnLabel = newInstructions.FindLast(instruction => instruction.opcode == OpCodes.Br_S).operand;

            // var ev = new PreAuthenticatingEventArgs(text, request, request.Data.Position, b3, text2, true);
            //
            // Player.OnPreAuthenticating(ev);
            //
            // if (!ev.IsAllowed)
            // {
            //   var failedMessage = string.Format($"Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin.", text, request.RemoteEndPoint);
            //
            //   ServerConsole.AddLog(failedMessage, ConsoleColor.Gray);
            //   ServerLogs.AddLog(ServerLogs.Modules.Networking, failedMessage, ServerLogs.ServerLogType.ConnectionUpdate, false);
            // }
            // else
            // {
            //   CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
            //   [...]
            newInstructions.InsertRange(index, new[]
            {
                // text (userId)
                new CodeInstruction(OpCodes.Ldloc_S, 9).MoveLabelsFrom(newInstructions[index]),

                // request
                new CodeInstruction(OpCodes.Ldarg_1),

                // request.Data.Position (readerStartPosition)
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ConnectionRequest), nameof(ConnectionRequest.Data))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetDataReader), nameof(NetDataReader.Position))),

                // b3 (flags)
                new CodeInstruction(OpCodes.Ldloc_S, 11),

                // text2 (country)
                new CodeInstruction(OpCodes.Ldloc_S, 12),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new PreAuthenticatingEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnPreAuthenticating(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPreAuthenticating))),

                // if (!ev.IsAllowed)
                // {
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, elseLabel),

                // var failedMessage = string.Format($"Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin.", text, request.RemoteEndPoint);
                new CodeInstruction(OpCodes.Ldstr, "Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin."),
                new CodeInstruction(OpCodes.Ldloc_S, 9),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ConnectionRequest), nameof(ConnectionRequest.RemoteEndPoint))),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, failedMessage.LocalIndex),

                // ServerConsole.AddLog(failedMessage, ConsoleColor.Gray)
                new CodeInstruction(OpCodes.Ldc_I4_7),
                new CodeInstruction(OpCodes.Call, Method(typeof(ServerConsole), nameof(ServerConsole.AddLog))),

                // ServerLogs.AddLog(ServerLogs.Modules.Networking, failedMessage, ServerLogs.ServerLogType.ConnectionUpdate, false)
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldloc_S, failedMessage.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(ServerLogs), nameof(ServerLogs.AddLog))),
                new CodeInstruction(OpCodes.Br_S, returnLabel),

                // }
                // else
                // {
                //   CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                //   [...]
                new CodeInstruction(OpCodes.Call, Method(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode))).WithLabels(elseLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
