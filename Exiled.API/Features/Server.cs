// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Reflection;

    using Mirror;

    /// <summary>
    /// A set of tools to easily work with the server.
    /// </summary>
    public static class Server
    {
        private static Player host;
        private static global::Broadcast broadcast;
        private static BanPlayer banPlayer;
        private static MethodInfo sendSpawnMessage;

        /// <summary>
        /// Gets the player's host of the server.
        /// </summary>
        public static Player Host
        {
            get
            {
                if (host == null || host.ReferenceHub == null)
                    host = new Player(PlayerManager.localPlayer);

                return host;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="Broadcast"/> component.
        /// </summary>
        public static global::Broadcast Broadcast
        {
            get
            {
                if (broadcast == null)
                    broadcast = PlayerManager.localPlayer.GetComponent<global::Broadcast>();

                return broadcast;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="BanPlayer"/> component.
        /// </summary>
        public static BanPlayer BanPlayer
        {
            get
            {
                if (banPlayer == null)
                    banPlayer = PlayerManager.localPlayer.GetComponent<BanPlayer>();

                return banPlayer;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="SendSpawnMessage"/> <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessage
        {
            get
            {
                if (sendSpawnMessage == null)
                {
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
        public static string Name
        {
            get => ServerConsole._serverName;
            set
            {
                ServerConsole._serverName = value;
                ServerConsole.singleton.RefreshServerName();
            }
        }

        /// <summary>
        /// Gets or sets the port of the server.
        /// </summary>
        public static ushort Port
        {
            get => ServerStatic.ServerPort;
            set => ServerStatic.ServerPort = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether friendly fire is enabled or not.
        /// </summary>
        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set => ServerConsole.FriendlyFire = value;
        }
    }
}
