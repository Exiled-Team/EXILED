// -----------------------------------------------------------------------
// <copyright file="SendingCommand.cs" company="Exiled Team">
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
    using API.Features.Pools;

    using CommandSystem;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)" />.
    /// Adds the <see cref="Handlers.Player.SendingCommand" /> event.
    /// </summary>
    ///
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingCommand))]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal static class SendingCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(ret);
            LocalBuilder ev = generator.DeclareLocal(typeof(SendingCommandEventArgs));
            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(CommandHandler), nameof(CommandHandler.TryGetCommand)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Sender
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // Player.Get(CommandSender)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new Type[] { typeof(CommandSender) })),

                    // command
                    new CodeInstruction(OpCodes.Ldloc_1),

                    // query
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // SendingCommandEventArgs ev = new SendingCommandEventArgs(Player, ICommand, Querry)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCommandEventArgs))[0]),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnSendingCommand(ev)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingCommand))),

                    // isallowed == false
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SendingCommandEventArgs), nameof(SendingCommandEventArgs.IsAllowed))),

                    // ret
                    new CodeInstruction(OpCodes.Brfalse_S, ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
