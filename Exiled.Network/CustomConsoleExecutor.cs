// -----------------------------------------------------------------------
// <copyright file="CustomConsoleExecutor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using UnityEngine;

    /// <summary>
    /// Custom command executor for Exiled.Network.
    /// </summary>
    public class CustomConsoleExecutor : CommandSender
    {
        private readonly string command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomConsoleExecutor"/> class.
        /// </summary>
        /// <param name="client">Network Client.</param>
        /// <param name="cmd">Command.</param>
        public CustomConsoleExecutor(NPClient client, string cmd)
        {
            this.client = client;
            this.command = cmd;
        }

        private NPClient client;

        /// <summary>
        /// Gets sender id.
        /// </summary>
        public override string SenderId
        {
            get
            {
                return "GAME CONSOLE";
            }
        }

        /// <summary>
        /// Gets nickname.
        /// </summary>
        public override string Nickname
        {
            get
            {
                return "GAME CONSOLE";
            }
        }

        /// <summary>
        /// Gets permissions.
        /// </summary>
        public override ulong Permissions
        {
            get
            {
                return ServerStatic.GetPermissionsHandler().FullPerm;
            }
        }

        /// <summary>
        /// Gets kick power.
        /// </summary>
        public override byte KickPower
        {
            get
            {
                return byte.MaxValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether gets full permissions.
        /// </summary>
        public override bool FullPermissions
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Send RemoteAdmin response.
        /// </summary>
        /// <param name="text"> Response.</param>
        /// <param name="success"> Is Success.</param>
        /// <param name="logToConsole"> Log to console.</param>
        /// <param name="overrideDisplay"> Override display.</param>
        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
        {
            GameCore.Console.AddLog("[RA Reply] " + text, success ? Color.green : Color.red, false);
            this.client.PacketProcessor.Send<ConsoleResponsePacket>(this.client.NetworkListener, new ConsoleResponsePacket() { Command = command, isRemoteAdmin = true, Response = text }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Print console response.
        /// </summary>
        /// <param name="text"> Response.</param>
        public override void Print(string text)
        {
            GameCore.Console.AddLog(text, Color.green, false);
            this.client.PacketProcessor.Send<ConsoleResponsePacket>(this.client.NetworkListener, new ConsoleResponsePacket() { Command = command, isRemoteAdmin = false, Response = text }, DeliveryMethod.ReliableOrdered);
        }
    }
}
