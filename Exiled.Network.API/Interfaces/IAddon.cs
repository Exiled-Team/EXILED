// -----------------------------------------------------------------------
// <copyright file="IAddon.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Exiled.Network.API.Models;
    using LiteNetLib.Utils;

    /// <summary>
    /// Network Addon.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public interface IAddon<out TConfig> : IComparable<IAddon<IConfig>>
        where TConfig : IConfig
    {
        /// <summary>
        /// Gets or sets network manager.
        /// </summary>
        NPManager Manager { get; set; }

        /// <summary>
        /// Gets or sets logger.
        /// </summary>
        NPLogger Logger { get; set; }

        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        string AddonId { get; set; }

        /// <summary>
        /// Gets or sets default path.
        /// </summary>
        string DefaultPath { get; set; }

        /// <summary>
        /// Gets or sets addon path.
        /// </summary>
        string AddonPath { get; set; }

        /// <summary>
        /// Gets addon config.
        /// </summary>
        TConfig Config { get; }

        /// <summary>
        /// Called when the addon is enabled.
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when a message is received from a server.
        /// </summary>
        /// <param name="server">Received from.</param>
        /// <param name="reader">Reader data.</param>
        void OnMessageReceived(NPServer server, NetDataReader reader);

        /// <summary>
        /// On addon is ready.
        /// </summary>
        /// <param name="server">Received from.</param>
        void OnReady(NPServer server);

        /// <summary>
        /// Called when a console command is received.
        /// </summary>
        /// <param name="cmd"> Command name.</param>
        /// <param name="arguments"> Command arguments.</param>
        void OnConsoleCommand(string cmd, List<string> arguments);

        /// <summary>
        /// Called when a server console response is received.
        /// </summary>
        /// <param name="server"> Received from.</param>
        /// <param name="command"> Command name.</param>
        /// <param name="response"> Response.</param>
        /// <param name="isRa"> Is Ra Command.</param>
        void OnConsoleResponse(NPServer server, string command, string response, bool isRa);
    }
}
