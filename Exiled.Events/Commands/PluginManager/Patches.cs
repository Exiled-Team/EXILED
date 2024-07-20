// -----------------------------------------------------------------------
// <copyright file="Patches.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using RemoteAdmin;

    /// <summary>
    /// The command to show all the patches done by plugins.
    /// </summary>
    public sealed class Patches : ICommand, IPermissioned
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
        public string Permission { get; } = "ee.showpatches";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();

            if (arguments.Count == 0)
            {
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
            }
            else
            {
                string name = arguments.At(0);
                IPlugin<IConfig> plugin = Loader.Loader.GetPlugin(name);
                sb.AppendLine($"Events to which plugin {name} subscribed:");

                foreach (PropertyInfo handler in typeof(Patches).Assembly.GetTypes().Where(x => x.Namespace != null && x.Namespace.Contains("Exiled.Events.Handlers")).SelectMany(x => x.GetProperties()))
                {
                    if (!handler.GetMethod.IsStatic)
                        continue;

                    object handlerInstance = handler.GetValue(null);
                    IReadOnlyCollection<Delegate> list = (IReadOnlyCollection<Delegate>)handler.PropertyType.GetProperty("SubscribedPlugins")?.GetValue(handlerInstance);

                    if (list == null)
                    {
                        Log.Error("List of subscribed plugins is null!");
                        continue;
                    }

                    if (list.Any(x => x.Method.DeclaringType?.Assembly == plugin.Assembly))
                        sb.AppendLine($"{handler.DeclaringType?.Name}.{handler.Name}");
                }
            }

            response = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return true;
        }
    }
}