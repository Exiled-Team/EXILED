// -----------------------------------------------------------------------
// <copyright file="CommandInfoPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    using Exiled.Network.API.Interfaces;

    using LiteNetLib.Utils;

    /// <summary>
    /// Command info.
    /// </summary>
    public struct CommandInfoPacket : INetSerializable
    {
        /// <summary>
        /// Gets addon id.
        /// </summary>
        public string AddonID;

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string CommandName;

        /// <summary>
        /// Gets command description.
        /// </summary>
        public string Description;

        /// <summary>
        /// Gets command permission.
        /// </summary>
        public string Permission;

        /// <summary>
        /// Gets if is remoteadmin command.
        /// </summary>
        public bool IsRaCommand;

        /// <summary>
        /// Gets generated command.
        /// </summary>
        public object Command;

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public void Deserialize(NetDataReader reader)
        {
            AddonID = reader.GetString();
            CommandName = reader.GetString();
            Description = reader.GetString();
            Permission = reader.GetString();
            IsRaCommand = reader.GetBool();

            NPManager.Singleton.OnRegisterCommand(this);
        }

        /// <summary>
        /// Unregister command.
        /// </summary>
        public void UnregisterCommand()
        {
            NPManager.Singleton.OnUnregisterCommand(this);
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(AddonID);
            writer.Put(CommandName);
            writer.Put(Description);
            writer.Put(Permission);
            writer.Put(IsRaCommand);
        }
    }
}
