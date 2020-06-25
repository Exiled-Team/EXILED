// -----------------------------------------------------------------------
// <copyright file="Command.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Loader;
    using MEC;
    using UnityEngine;
    using static GameCore.ConfigFile;

    /// <summary>
    /// Handles the plugin commands.
    /// </summary>
    public class Command
    {
        /// <inheritdoc cref="Server.OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs)"/>
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            switch (ev.Name)
            {
                case "reconnectrs":
                    ev.IsAllowed = false;
                    PlayerManager.localPlayer.GetComponent<PlayerStats>()?.Roundrestart();

                    Timing.CallDelayed(1.5f, Application.Quit);
                    break;
                case "reload":
                    ev.IsAllowed = false;
                    if (ev.Arguments.Count < 1)
                    {
                        ev.Sender.RemoteAdminMessage("You must specify a config type to reload: exiled, plugin, gameplay, ra or all");
                        return;
                    }

                    switch (ev.Arguments[0].ToLower())
                    {
                        case "plugins":
                            ev.Sender.RemoteAdminMessage("Reloading plugins...");
                            PluginManager.LoadAll();
                            break;
                        case "configs":
                            ConfigManager.Reload();
                            break;
                        case "gameplay":
                            ReloadGameConfigs();
                            ev.Sender.RemoteAdminMessage("Gameplay configs reloaded.");
                            break;
                        case "remoteadmin":
                        case "ra":
                            ConfigManager.ReloadRemoteAdmin();
                            ev.Sender.RemoteAdminMessage("Remote admin configs reloaded.");
                            break;
                    }

                    break;
            }
        }
    }
}
