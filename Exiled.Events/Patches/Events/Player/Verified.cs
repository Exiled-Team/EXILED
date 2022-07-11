// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ServerRoles.UserCode_CmdServerSignatureComplete"/>.
    /// Adds the <see cref="Handlers.Player.Verified"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Verified))]
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdServerSignatureComplete))]
    internal static class Verified
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label callJoined = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions))) + offset;

            // Player player;
            // if(!Player.UnverifiedPlayers.TryGetValue(_hub, out player)) {
            //     Means the player connected before WaitingForPlayers event is fired
            //     Let's call Joined event, since it wasn't called, to avoid breaking the logic of the order of event calls
            //     Blame NorthWood
            //
            //     Joined.CallEvent(_hub, out player);
            // }
            // #if DEBUG
            // Log.Debug("{player.Nickname} has verified!");
            // #endif
            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(Player), nameof(Player.UnverifiedPlayers))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                new(OpCodes.Ldloca_S, player.LocalIndex),
                new(OpCodes.Callvirt,  Method(typeof(ConditionalWeakTable<ReferenceHub, Player>), nameof(ConditionalWeakTable<ReferenceHub, Player>.TryGetValue))),
                new(OpCodes.Brtrue_S, callJoined),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ServerRoles), nameof(ServerRoles._hub))),
                new(OpCodes.Ldloca_S, player.LocalIndex),
                new(OpCodes.Call,  Method(typeof(Joined), nameof(Joined.CallEvent))),

                new CodeInstruction(OpCodes.Nop).WithLabels(callJoined),

                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VerifiedEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVerified))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
