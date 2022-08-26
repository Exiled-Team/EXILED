// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using LiteNetLib;
    using LiteNetLib.Utils;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)" />.
    ///     Adds the <see cref="Handlers.Player.PreAuthenticating" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PreAuthenticating))]
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest), typeof(ConnectionRequest))]
    internal static class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            int offset = -1;

            // Search for the last "request.Accept()" and then removes the offset, to get "ldarg.1" index.
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == PropertyGetter(typeof(NetManager), nameof(NetManager.ConnectedPeersCount))) +
                        offset;

            // Declare a string local variable.
            LocalBuilder failedMessage = generator.DeclareLocal(typeof(string));

            // Define an else label.
            Label elseLabel = generator.DefineLabel();
            Label fullRejectLabel = generator.DefineLabel();
            newInstructions[index + 4].WithLabels(elseLabel);
            int rejectIndex = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Br_S) + 1;
            newInstructions[rejectIndex].WithLabels(fullRejectLabel);

            LocalBuilder ev = generator.DeclareLocal(typeof(PreAuthenticatingEventArgs));

            // Search for the operand of the last "br.s".
            object returnLabel = newInstructions.FindLast(instruction => instruction.opcode == OpCodes.Br_S).operand;

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
                new(OpCodes.Ldarg_1),

                // request.Data.Position (readerStartPosition)
                new(OpCodes.Dup),
                new(OpCodes.Ldfld, Field(typeof(ConnectionRequest), nameof(ConnectionRequest.Data))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetDataReader), nameof(NetDataReader.Position))),

                // b3 (flags)
                new(OpCodes.Ldloc_S, 11),

                // text2 (country)
                new(OpCodes.Ldloc_S, 12),

                // true
                new(OpCodes.Ldloc_S, 27),

                // var ev = new PreAuthenticatingEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnPreAuthenticating(ev)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnPreAuthenticating))),

                // if (!ev.IsAllowed)
                // {
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, elseLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.ServerFull))),
                new(OpCodes.Brtrue, fullRejectLabel),

                // var failedMessage = string.Format($"Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin.", text, request.RemoteEndPoint);
                new(OpCodes.Ldstr, "Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin."),
                new(OpCodes.Ldloc_S, 9),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ConnectionRequest), nameof(ConnectionRequest.RemoteEndPoint))),
                new(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, failedMessage.LocalIndex),

                // ServerConsole.AddLog(failedMessage, ConsoleColor.Gray)
                new(OpCodes.Ldc_I4_7),
                new(OpCodes.Call, Method(typeof(ServerConsole), nameof(ServerConsole.AddLog))),

                // ServerLogs.AddLog(ServerLogs.Modules.Networking, failedMessage, ServerLogs.ServerLogType.ConnectionUpdate, false)
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldloc_S, failedMessage.LocalIndex),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Call, Method(typeof(ServerLogs), nameof(ServerLogs.AddLog))),
                new(OpCodes.Br_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
