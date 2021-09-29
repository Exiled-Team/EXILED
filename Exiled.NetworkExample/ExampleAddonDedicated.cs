// -----------------------------------------------------------------------
// <copyright file="ExampleAddonDedicated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.NetworkExample
{
    using System;
    using System.Collections.Generic;

    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Models;

    using LiteNetLib.Utils;

    /// <summary>
    /// Example dedicated addon.
    /// </summary>
    public class ExampleAddonDedicated : NPAddonDedicated<AddonConfig>
    {
        /// <inheritdoc/>
        public override string AddonAuthor { get; } = "Exiled Team";

        /// <inheritdoc/>
        public override string AddonName { get; } = "ExampleAddon";

        /// <inheritdoc/>
        public override Version AddonVersion { get; } = new Version(1, 0, 0);

        /// <inheritdoc/>
        public override string AddonId { get; } = "0dewadopsdap32";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on DEDICATED HOST.");
        }

        /// <inheritdoc/>
        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received message from server {server.ServerAddress}:{server.ServerPort}");
            foreach (var plr in server.Players.Values)
            {
                Logger.Info($"Player {plr.UserName} {plr.UserID}");
            }

            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(server.ServerAddress, server.ServerPort, writer);
        }

        /// <inheritdoc/>
        public override void OnConsoleCommand(string cmd, List<string> arguments)
        {
            switch (cmd.ToUpper())
            {
                case "TEST":
                    Logger.Info("Test response");
                    break;
            }
        }

        /// <inheritdoc/>
        public override void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
            Logger.Info($"Received command response from server {server.FullAddress}, command name: {command}, response: {response}.");
        }
    }
}
