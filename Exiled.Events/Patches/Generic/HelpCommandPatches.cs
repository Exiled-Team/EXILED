// -----------------------------------------------------------------------
// <copyright file="HelpCommandPatches.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Text;
    using System.Text.RegularExpressions;

    using CommandSystem;
    using CommandSystem.Commands.Shared;
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="HelpCommand.GetCommandList"/>.
    /// </summary>
    [HarmonyPatch(typeof(HelpCommand), nameof(HelpCommand.GetCommandList))]
    internal class HelpCommandPatches
    {
        private static readonly Regex ConsoleTagReplacer = new(@"<\/?[ib]>");

        private static bool Prefix(HelpCommand __instance, ICommandHandler handler, string header, ref string __result)
        {
            StringBuilder _helpBuilder = __instance._helpBuilder;
            _helpBuilder.Clear();
            _helpBuilder.Append(header);

            string previousName = string.Empty;
            foreach (ICommand command in handler.AllCommands)
            {
                command.GetType().Namespace.Split('.').TryGet(0, out string pluginName);
                if (previousName != pluginName)
                {
                    _helpBuilder.AppendLine();
                    _helpBuilder.Append("<size=40><b>");
                    _helpBuilder.Append(pluginName);
                    _helpBuilder.Append("</b></size>");
                    previousName = pluginName;
                }

                GetCommand(command);
            }

            __result = ConsoleTagReplacer.Replace(_helpBuilder.ToString(), string.Empty);
            return false;

            void GetCommand(ICommand command, int space = 0)
            {
                if (command is IHiddenCommand)
                    return;

                _helpBuilder.AppendLine();
                _helpBuilder.Append(new string(' ', space * 2));
                _helpBuilder.Append(" - <b>");
                _helpBuilder.Append(command.Command);
                _helpBuilder.Append("</b> ");
                if (command.Aliases != null && command.Aliases.Length != 0)
                {
                    _helpBuilder.Append("(");
                    _helpBuilder.Append(string.Join(", ", command.Aliases));
                    _helpBuilder.Append(")");
                }

                _helpBuilder.Append(" : <i><color=yellow>");
                _helpBuilder.Append(command.Description);
                _helpBuilder.Append("</color></i>");

                if (command is not ParentCommand parent)
                    return;

                foreach (ICommand subCommand in parent.AllCommands)
                {
                    GetCommand(subCommand, space + 1);
                }
            }
        }
    }
}