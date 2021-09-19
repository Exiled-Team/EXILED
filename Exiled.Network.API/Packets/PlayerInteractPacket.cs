// -----------------------------------------------------------------------
// <copyright file="PlayerInteractPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Player interact packet.
    /// </summary>
    public class PlayerInteractPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets type of interaction.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets interaction data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
