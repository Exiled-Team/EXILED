// -----------------------------------------------------------------------
// <copyright file="ExampleAddonClient.cs" company="Exiled Team">
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
    using MEC;

    /// <summary>
    /// Example client addon.
    /// </summary>
    public class ExampleAddonClient : NPAddonClient<AddonConfig>
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
