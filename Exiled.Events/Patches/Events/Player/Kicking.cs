// -----------------------------------------------------------------------
// <copyright file="Kicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Log = API.Features.Log;

    /// <summary>
    ///     Patches <see cref="BanPlayer.KickUser(ReferenceHub, ICommandSender , string)" />.
    ///     Adds the <see cref="Handlers.Player.Kicking" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ICommandSender), typeof(string))]
    internal static class Kicking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label jmp = generator.DefineLabel();

            const int offset = -5;

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Brtrue_S);
            newInstructions[index] = new CodeInstruction(OpCodes.Brtrue_S, jmp);

            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ServerConsole), nameof(ServerConsole.Disconnect), new Type[] { typeof(GameObject), typeof(string) }))) + offset;

            // remove base game logic
            newInstructions.RemoveRange(index, 7);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // jmp
                    // Kicking.Process(ReferenceHub target, ICommandSender issuer, string reason);
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jmp),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Call, Method(typeof(Kicking), nameof(Kicking.Process))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool Process(ReferenceHub target, ICommandSender issuer, string reason)
        {
            string message = $"You have been kicked. {(!string.IsNullOrEmpty(reason) ? "Reason: " + reason : string.Empty)}";

            KickingEventArgs ev = new(Player.Get(target), Player.Get(issuer), reason, message);
            Handlers.Player.OnKicking(ev);

            if (!ev.IsAllowed)
            {
                return false;
            }

            ServerConsole.Disconnect(target.gameObject, ev.FullMessage);
            return true;
        }
    }
}