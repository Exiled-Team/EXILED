// -----------------------------------------------------------------------
// <copyright file="ConsoleResponsePacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Console response packet.
    /// </summary>
    public class ConsoleResponsePacket
    {
        /// <summary>
        /// Gets or sets a value indicating whether if is remoteadmin response.
        /// </summary>
        public bool IsRemoteAdmin { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets command response.
        /// </summary>
        public string Response { get; set; }
    }
}
