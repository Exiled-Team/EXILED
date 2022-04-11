// -----------------------------------------------------------------------
// <copyright file="Kicked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ServerConsole.Disconnect(GameObject, string)"/>.
    /// Adds the <see cref="Handlers.Player.Kicked"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Disconnect), new[] { typeof(GameObject), typeof(string) })]
    internal static class Kicked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 3;

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0) + offset;

            // Handlers.Player.OnKicked(new KickedEventArgs(Player.Get(player), message))
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(KickedEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnKicked))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
