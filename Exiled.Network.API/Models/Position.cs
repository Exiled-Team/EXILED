// -----------------------------------------------------------------------
// <copyright file="Position.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Models
{
    using LiteNetLib.Utils;

    /// <summary>
    /// Position.
    /// </summary>
    public struct Position : INetSerializable
    {
        /// <summary>
        /// Gets or sets x position.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets y position.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets z position.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
            Z = reader.GetFloat();
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
        }
    }
}
