// -----------------------------------------------------------------------
// <copyright file="PlayerInfoPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    using LiteNetLib.Utils;

    /// <summary>
    /// Player info packet.
    /// </summary>
    public struct PlayerInfoPacket : INetSerializable
    {
        /// <summary>
        /// Gets userid.
        /// </summary>
        public string UserID;

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public void Deserialize(NetDataReader reader)
        {
            UserID = reader.GetString();
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserID);
        }
    }
}
