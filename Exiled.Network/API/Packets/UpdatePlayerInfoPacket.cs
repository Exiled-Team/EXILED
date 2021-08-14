// -----------------------------------------------------------------------
// <copyright file="UpdatePlayerInfoPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Update player info packet.
    /// </summary>
    public class UpdatePlayerInfoPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets player info type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets player info data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
