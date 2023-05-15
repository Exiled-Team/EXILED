using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using PluginAPI.Core;
using PluginAPI.Events;
using static HarmonyLib.AccessTools;

namespace Exiled.Events.Patches.Events.Player
{
    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)"/>
    /// to add <see cref="Handlers.Player.PreAuthenticating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal class PreAuthenticating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.Is(OpCodes.Callvirt, PropertyGetter(typeof(NetManager), nameof(NetManager.ConnectedPeersCount)))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(PreAuthenticatingEventArgs));
            LocalBuilder str = generator.DeclareLocal(typeof(string));
            LocalBuilder writer = generator.DeclareLocal(typeof(NetDataWriter));

            Label continueLabel = generator.DefineLabel();
            Label forced = generator.DefineLabel();

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldloc_S, 10),

                    new(OpCodes.Ldloc_S, 16),

                    new(OpCodes.Ldloc_S, 11),

                    new(OpCodes.Ldloc_S, 17),

                    new(OpCodes.Ldloc_S, 13),

                    new(OpCodes.Ldloc_S, 14),

                    new(OpCodes.Ldarg_1),

                    new(OpCodes.Ldloc_S, 9),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreAuthenticatingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPreAuthenticating))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ldstr, "Player {0} tried to preauthenticated from endpoint {1}, but the request has been rejected by a plugin."),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.UserId))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PreAuthenticatingEventArgs), nameof(PreAuthenticatingEventArgs.IpAddress))),
                    new(OpCodes.Callvirt, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) })),
                    new(OpCodes.Stloc_S, str.LocalIndex),

                    new(OpCodes.Ldloc_S, str.LocalIndex),
                    new(OpCodes.Ldc_I4_7),
                    new(OpCodes.Call, Method(typeof(ServerConsole), nameof(ServerConsole.AddLog))),

                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldloc_S, str.LocalIndex),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Call, Method(typeof(ServerLogs), nameof(ServerLogs.AddLog))),

                    new(OpCodes.Ldnull),
                    new(OpCodes.Stloc_S, writer.LocalIndex),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Ldloca_S, writer.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(PreAuthenticating), nameof(IsForced), new[] { typeof(PreAuthenticatingEventArgs), typeof(NetDataWriter).MakeByRefType() })),
                    new(OpCodes.Brtrue_S, forced),

                    new(OpCodes.Ldloc_S, writer.LocalIndex),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(ConnectionRequest), nameof(ConnectionRequest.Reject), new[] { typeof(NetDataWriter) })),
                    new(OpCodes.Ret),

                    new CodeInstruction(OpCodes.Ldloc_S, writer.LocalIndex).WithLabels(forced),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(ConnectionRequest), nameof(ConnectionRequest.RejectForce), new[] { typeof(NetDataWriter) })),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool IsForced(PreAuthenticatingEventArgs ev, ref NetDataWriter writer)
        {
            writer = ev.CachedPreauthData.GenerateWriter(out var forced);
            return forced;
        }
    }
}