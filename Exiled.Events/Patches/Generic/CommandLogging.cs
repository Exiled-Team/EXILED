// -----------------------------------------------------------------------
// <copyright file="CommandLogging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using HarmonyLib;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    using Events = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery"/> for command logging.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal class CommandLogging
    {
        /// <summary>
        /// Logs a command to the RA log file.
        /// </summary>
        /// <param name="query">The command being logged.</param>
        /// <param name="sender">The sender of the command.</param>
        public static void LogCommand(string query, CommandSender sender)
        {
            try
            {
                if (query.StartsWith("$", StringComparison.Ordinal))
                    return;

                Player player = sender is PlayerCommandSender playerCommandSender && sender != Server.Host.Sender
                    ? Player.Get(playerCommandSender)
                    : Server.Host;

                string logMessage = string.Empty;

                try
                {
                    logMessage =
                        $"[{DateTime.Now}] {(player == Server.Host ? "Server Console" : $"{player?.Nickname} ({player?.UserId}) {player?.IPAddress}")}" +
                        $" has run the command {query}.\n";
                }
                catch (Exception exception)
                {
                    Log.Error($"{nameof(CommandLogging)}: Failed to log command; unable to parse log message.\n{player is null}\n{exception}");
                }

                if (string.IsNullOrEmpty(logMessage))
                    return;

                string directory = Path.Combine(Paths.Exiled, "Logs");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string filePath = Path.Combine(directory, $"{Server.Port}-RAlog.txt");

                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                File.AppendAllText(filePath, logMessage);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(CommandLogging)}: Unable to log a command.\n{string.IsNullOrEmpty(query)} - {sender is null}\n{exception}");
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int index = 0;

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (!Events.Instance.Config.LogRaCommands)
                    //   goto continueLabel;
                    new(OpCodes.Call, PropertyGetter(typeof(Events), nameof(Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Events), nameof(Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.LogRaCommands))),
                    new(OpCodes.Brfalse, continueLabel),

                    // q
                    new(OpCodes.Ldarg_0),

                    // sender
                    new(OpCodes.Ldarg_1),

                    // LogCommand(q, sender)
                    new(OpCodes.Call, Method(typeof(CommandLogging), nameof(LogCommand))),

                    // continueLabel:
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}