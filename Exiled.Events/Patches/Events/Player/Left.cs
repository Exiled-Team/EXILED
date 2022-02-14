// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomNetworkManager.OnServerDisconnect(NetworkConnection)"/>.
    /// Adds the <see cref="Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), new[] { typeof(NetworkConnection) })]
    internal static class Left
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder gameObject = generator.DeclareLocal(typeof(UnityEngine.GameObject));
            LocalBuilder id = generator.DeclareLocal(typeof(int));

            Label cdc = generator.DefineLabel();

            newInstructions[0].labels.Add(cdc);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.gameObject))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, gameObject.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, gameObject.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(UnityEngine.GameObject) })),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsHost))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldstr, "Player "),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Nickname))),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Ldstr, " ("),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.UserId))),
                new CodeInstruction(OpCodes.Ldstr, ") ("),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Ldloca_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Id))),
                new CodeInstruction(OpCodes.Stloc_S, id.LocalIndex),
                new CodeInstruction(OpCodes.Ldloca_S, id.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(int), nameof(int.ToString))),
                new CodeInstruction(OpCodes.Ldstr, ") disconnected"),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string) })),
                new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Log), nameof(API.Features.Log.SendRaw), new[] { typeof(string), typeof(ConsoleColor) })),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(LeftEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnLeft))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
