// -----------------------------------------------------------------------
// <copyright file="ExecuteConsoleCommandPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Execute console command packet.
    /// </summary>
    public class ExecuteConsoleCommandPacket
    {
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string Command { get; set; }
    }
}
