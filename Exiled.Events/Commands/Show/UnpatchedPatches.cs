namespace Exiled.Events.Commands.Show
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    /// <summary>
    /// The command to show all plugins.
    /// </summary>
    public sealed class UnpatchedPatches : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "unpatched";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "up" };

        /// <inheritdoc/>
        public string Description { get; } = "Get all plugins, names, authors and versions";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string perm = "ee.showunpatched";
            if (!sender.CheckPermission(perm) && (sender is PlayerCommandSender playerSender && !playerSender.QueryProcessor._roles.RaEverywhere))
            {
                response = $"You can't show the unpatched patches, you don't have \"{perm}\" permissions.";
                return false;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent();

            sb.AppendLine("All unpatched patches:");

            foreach (Type type in Patcher.UnpatchedPatches)
            {
                sb.AppendLine(type.FullName);
            }

            response = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return true;
        }
    }
}
