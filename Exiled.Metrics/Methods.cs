// -----------------------------------------------------------------------
// <copyright file="Methods.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Metrics;

using System.Collections.Generic;
using System.Linq;
using System.Net;

using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using Exiled.Metrics.API.Enums;

/// <summary>
/// General methods.
/// </summary>
public class Methods
{
    private const string Url = "https://exiled.host/metrics/";
    private readonly Plugin plugin;

    /// <summary>
    /// Initializes a new instance of the <see cref="Methods"/> class.
    /// </summary>
    /// <param name="plugin">The <see cref="Plugin{TConfig}"/> class reference.</param>
    public Methods(Plugin plugin) => this.plugin = plugin;

    internal void SendMetrics(EventType type, params object[] args)
    {
        string serverIdentifier = $"{Server.IpAddress.GetHash()}-{Server.Port.ToString().GetHash()}";
        string exiledVersion = Events.Events.Instance.Version.ToString();
        int playerCount = Server.PlayerCount;
        int maxPlayers = Server.MaxPlayerCount;
        LeadingTeam? team = null;

        if (type == EventType.RoundEnd)
            team = (LeadingTeam)args[0];

        IEnumerable<IPlugin<IConfig>> plugins = Loader.Plugins;
        string pluginInfo = string.Empty;
        foreach (IPlugin<IConfig> plg in plugins)
            pluginInfo += $"{plg.Name}|{plg.Version}|{plg.Author}|{plg.Config.IsEnabled}||";

        string message = $"srvId={serverIdentifier}&exiled={exiledVersion}&players={playerCount}&playerMax={maxPlayers}&team={(team is null ? "None" : team)}&tps={Server.Tps}&plugins={pluginInfo}";

        HttpQuery.Post(Url, message, out bool success, out HttpStatusCode code);
        switch (success)
        {
            case true:
                Log.Debug($"{nameof(SendMetrics)}: Metrics posted.", plugin.Config.Debug);
                break;
            case false:
                Log.Warn($"{nameof(SendMetrics)}: Metrics unable to post: {code}");
                break;
        }
    }
}
