// -----------------------------------------------------------------------
// <copyright file="ExampleAddonHost.cs" company="Exiled Team">
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
    /// Example host addon.
    /// </summary>
    [NPAddonInfo(
        addonId: "0dewadopsdap32",
        addonName: "ExampleAddon",
        addonAuthor: "Exiled Team",
        addonVersion: "1.0.0")]
    public class ExampleAddonHost : NPAddonHost<AddonConfig>
    {
        /// <inheritdoc/>
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on HOST.");
        }

        /// <inheritdoc/>
        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" ) from server {server.ServerAddress}:{server.ServerPort}");
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(server.ServerAddress, server.ServerPort, writer);
        }

        /// <inheritdoc/>
        public override void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
            Logger.Info($"Received command response from server {server.FullAddress}, command name: {command}, response: {response}.");
        }
    }
}
