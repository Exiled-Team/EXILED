// -----------------------------------------------------------------------
// <copyright file="Kicked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ServerConsole.Disconnect(GameObject, string)" />.
    /// Adds the <see cref="Handlers.Player.Kicked" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Kicked))]
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Disconnect), typeof(GameObject), typeof(string))]
    internal static class Kicked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 3;

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Handlers.Player.OnKicked(new KickedEventArgs(Player.Get(player), message))
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KickedEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnKicked))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}