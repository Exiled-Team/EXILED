// -----------------------------------------------------------------------
// <copyright file="ExecuteCommandPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Excute command packet.
    /// </summary>
    public class ExecuteCommandPacket
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets arguments.
        /// </summary>
        public string[] Arguments { get; set; }
    }
}
