// -----------------------------------------------------------------------
// <copyright file="DisplaySubscribed.cs" company="Exiled Team">
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

    using CommandSystem;
    using Exiled.API.Interfaces;

    /// <summary>
    /// A command to display plugins that are subscribed to the specific event.
    /// </summary>
    public class DisplaySubscribed : IPermissionCommand
    {
        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DisplaySubscribed Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "subscribed";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "displaysubcribed", "sub" };

        /// <inheritdoc/>
        public string Description { get; } = "Displays plugins that are subscribed to the event.";

        /// <inheritdoc/>
        public string Permission { get; } = "pm.subscribed";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "Usage: pmanager subscribed <handler name> OR pmanager subcribed <handler type>.<handler name>";
                return false;
            }

            string handlerName = arguments.At(0);
            PropertyInfo handler;

            if (handlerName.Contains("."))
            {
                string handlerType = handlerName.Split('.')[0];
                handlerName = handlerName.Split('.')[1];

                handler = typeof(DisplaySubscribed).Assembly.GetType("Exiled.Events.Handlers." + handlerType).GetProperty(handlerName);
            }
            else
            {
                handler = typeof(Patches).Assembly.GetTypes().Where(x => x.Namespace != null && x.Namespace.Contains("Exiled.Events.Handlers")).SelectMany(x => x.GetProperties()).FirstOrDefault(x => x.Name == handlerName);
            }

            if (handler == null)
            {
                response = $"Handler for event (name: {handlerName}) not found.";
                return false;
            }

            object handlerInstance = handler.GetValue(null);
            IReadOnlyCollection<string> subscribedPlugins = (IReadOnlyCollection<string>)handler.PropertyType.GetProperty("SubscribedPlugins")?.GetValue(handlerInstance);

            response = $"Subscribed plugins:\n{string.Join("\n -", subscribedPlugins)}";
            return true;
        }
    }
}