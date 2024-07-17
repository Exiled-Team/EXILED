// -----------------------------------------------------------------------
// <copyright file="RestartedRespawnSequence.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Respawning;
    using Respawning.NamingRules;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patch the <see cref="RespawnManager.RestartSequence"/>.
    /// Adds the <see cref="Server.RestartedRespawnSequence"/> event.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RestartedRespawnSequence))]
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.RestartSequence))]
    internal static class RestartedRespawnSequence
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RestartedRespawnSequenceEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRestartedRespawnSequence))),
            };

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret);
            newInstructions.InsertRange(index, toInsert);

            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ret);
            newInstructions.InsertRange(index, toInsert);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}