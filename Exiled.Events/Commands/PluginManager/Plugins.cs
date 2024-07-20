// -----------------------------------------------------------------------
// <copyright file="Plugins.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Events.Features;
    using Utf8Json;

    /// <summary>
    /// A command to display list with all verified plugin.
    /// </summary>
    public class Plugins : IPermissioned, ICommand
    {
        /// <summary>
        /// Gets the static instance of the command.
        /// </summary>
        public static Plugins Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "plugins";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of all verified plugins.";

        /// <inheritdoc/>
        public string Permission { get; } = "pm.plugins";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage responseMessage = client.GetAsync("https://exiled.to/api/plugins").Result;
                string res = responseMessage.Content.ReadAsStringAsync().Result;
                List<VerifiedPlugin> list = JsonSerializer.Deserialize<List<VerifiedPlugin>>(res);
                response = "- " + string.Join("- ", list.Select(x => x.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                response = null;
                return false;
            }
        }
    }
}