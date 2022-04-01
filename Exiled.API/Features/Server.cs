// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System.Reflection;

    using MEC;

    using Mirror;

    using PlayerStatsSystem;

    using RoundRestarting;

    /// <summary>
    /// A set of tools to easily work with the server.
    /// </summary>
    public static class Server {
        private static Player host;
        private static global::Broadcast broadcast;
        private static BanPlayer banPlayer;
        private static MethodInfo sendSpawnMessage;

        /// <summary>
        /// Gets the player's host of the server.
        /// Might be null when called when the server isn't loaded.
        /// </summary>
        public static Player Host {
            get {
                if (host == null || host.ReferenceHub == null)
                    host = PlayerManager.localPlayer != null ? new Player(PlayerManager.localPlayer) : null;

                return host;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="Broadcast"/> component.
        /// </summary>
        public static global::Broadcast Broadcast {
            get {
                if (broadcast == null)
                    broadcast = PlayerManager.localPlayer.GetComponent<global::Broadcast>();

                return broadcast;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="BanPlayer"/> component.
        /// </summary>
        public static BanPlayer BanPlayer {
            get {
                if (banPlayer == null)
                    banPlayer = PlayerManager.localPlayer.GetComponent<BanPlayer>();

                return banPlayer;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="SendSpawnMessage"/> <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessage {
            get {
                if (sendSpawnMessage == null) {
                    sendSpawnMessage = typeof(NetworkServer).GetMethod(
                        "SendSpawnMessage",
                        BindingFlags.NonPublic | BindingFlags.Static);
                }

                return sendSpawnMessage;
            }
        }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public static string Name {
            get => ServerConsole._serverName;
            set {
                ServerConsole._serverName = value;
                ServerConsole.singleton.RefreshServerName();
            }
        }

        /// <summary>
        /// Gets the RemoteAdmin permissions handler.
        /// </summary>
        public static PermissionsHandler PermissionsHandler => ServerStatic.PermissionsHandler;

        /// <summary>
        /// Gets the Ip address of the server.
        /// </summary>
        public static string IpAddress => ServerConsole.Ip;

        /// <summary>
        /// Gets the port of the server.
        /// </summary>
        public static ushort Port => ServerStatic.ServerPort;

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is enabled or not.
        /// </summary>
        public static bool FriendlyFire {
            get => ServerConsole.FriendlyFire;
            set => ServerConsole.FriendlyFire = value;
        }

        /// <summary>
        /// Gets the number of players currently on the server.
        /// </summary>
        public static int PlayerCount => Player.Dictionary.Count;

        /// <summary>
        /// Gets or sets the maximum number of players able to be on the server.
        /// </summary>
        public static int MaxPlayerCount {
            get => CustomNetworkManager.slots;
            set => CustomNetworkManager.slots = value;
        }

        /// <summary>
        /// Restarts the server, reconnects all players.
        /// </summary>
        public static void Restart() {
            Round.Restart(false, true, ServerStatic.NextRoundAction.Restart);
        }

        /// <summary>
        /// Shutdowns the server, disconnects all players.
        /// </summary>
        public static void Shutdown() {
            global::Shutdown.Quit();
        }

        /// <summary>
        /// Redirects players to a server on another port, restarts the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns>true, if redirection was successful; otherwise, false.</returns>
        /// <remarks>If the returned value is false, the server won't restart.</remarks>
        public static bool RestartRedirect(ushort redirectPort) {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Restart);

            return true;
        }

        /// <summary>
        /// Redirects players to a server on another port, shutdowns the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns>true, if redirection was successful; otherwise, false.</returns>
        /// <remarks>If the returned value is false, the server won't shutdown.</remarks>
        public static bool ShutdownRedirect(ushort redirectPort) {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Shutdown);
            return true;
        }

        /// <summary>
        /// Runs a server command.
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="sender">The <see cref="CommandSender"/> running the command.</param>
        public static void RunCommand(string command, CommandSender sender = null) => GameCore.Console.singleton.TypeCommand(command, sender ?? host.Sender);
    }
}
