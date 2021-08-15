// -----------------------------------------------------------------------
// <copyright file="ExampleAddonDedicated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.NetworkExample
{
    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Models;

    using LiteNetLib.Utils;

    /// <summary>
    /// Example dedicated addon.
    /// </summary>
    [NPAddonInfo(
        addonId: "0dewadopsdap32",
        addonName: "ExampleAddon",
        addonAuthor: "Exiled Team",
        addonVersion: "1.0.0")]
    public class ExampleAddonDedicated : NPAddonDedicated<AddonConfig>
    {
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
    }
}
