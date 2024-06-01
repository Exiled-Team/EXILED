// -----------------------------------------------------------------------
// <copyright file="Patches.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.Events.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    /// <summary>
    /// The command to show all the patches done by plugins.
    /// </summary>
    public sealed class Patches : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Patches"/> command.
        /// </summary>
        public static Patches Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "patches";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "patched" };

        /// <inheritdoc/>
        public string Description { get; } = "Returns information about all patches (whether they are patched or not)";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string perm = "ee.showpatches";
            if (!sender.CheckPermission(perm) && (sender is PlayerCommandSender playerSender))
            {
                response = $"You can't show the unpatched patches, you don't have \"{perm}\" permissions.";
                return false;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent();

            sb.AppendLine("All patches:");
            sb.AppendLine("Patched:");

            foreach (Type patch in Patcher.GetAllPatchTypes().Where((type) => !Patcher.UnpatchedTypes.Contains(type)))
            {
                sb.AppendLine($"\t{patch.FullName}");
            }

            sb.AppendLine("Unpatched: ");

            foreach (Type patch in Patcher.UnpatchedTypes)
            {
                sb.AppendLine($"\t{patch.FullName}");
            }

            response = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return true;
        }
    }
}