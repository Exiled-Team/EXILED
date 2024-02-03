// -----------------------------------------------------------------------
// <copyright file="SendingAdminChatMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessAdminChat(string, CommandSender)" />.
    /// Adds the <see cref="Handlers.Player.SendingAdminChatMessage" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingAdminChatMessage))]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessAdminChat))]
    internal static class SendingAdminChatMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label allowLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SendingAdminChatMessageEventsArgs));

            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloca_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(Hub)
                    new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(PlayerCommandSender), nameof(PlayerCommandSender.ReferenceHub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // message
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // SendingAdminChatMessageEventsArgs ev = new(Player, string, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingAdminChatMessageEventsArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnSendingAdminChatMessage(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingAdminChatMessage))),

                    // if (ev.IsAllowed)
                    //    goto allowLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingAdminChatMessageEventsArgs), nameof(SendingAdminChatMessageEventsArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, allowLabel),

                    // IsNotAllowedMessaqe(ev.Player);
                    // return;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(SendingAdminChatMessageEventsArgs), nameof(SendingAdminChatMessageEventsArgs.Player))),
                    new(OpCodes.Call, Method(typeof(SendingAdminChatMessage), nameof(SendingAdminChatMessage.IsNotAllowedMessaqe))),
                    new(OpCodes.Ret),

                    // message = ev.Message;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(allowLabel),
                    new(OpCodes.Call, PropertyGetter(typeof(SendingAdminChatMessageEventsArgs), nameof(SendingAdminChatMessageEventsArgs.Message))),
                    new(OpCodes.Starg_S, 0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void IsNotAllowedMessaqe(Player player)
        {
            if (player == null)
                return;
            player.RemoteAdminMessage("AdminChatMessage Cancel by a plugin", false, "EXILED:Event");
        }
    }
}