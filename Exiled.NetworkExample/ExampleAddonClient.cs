// -----------------------------------------------------------------------
// <copyright file="ExampleAddonClient.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.NetworkExample
{
    using System.Collections.Generic;
    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Models;
    using LiteNetLib.Utils;
    using MEC;

    /// <summary>
    /// Example client addon.
    /// </summary>
    [NPAddonInfo(
        addonId: "0dewadopsdap32",
        addonName: "ExampleAddon",
        addonAuthor: "Exiled Team",
        addonVersion: "1.0.0")]
    public class ExampleAddonClient : NPAddonClient<AddonConfig>
    {
        /// <inheritdoc/>
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on CLIENT.");
            Timing.RunCoroutine(SendDatas());
        }

        /// <inheritdoc/>
        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" )");
        }

        private IEnumerator<float> SendDatas()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(3f);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Some string");
                SendData(writer);
            }
        }
    }
}
