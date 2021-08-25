// -----------------------------------------------------------------------
// <copyright file="CommandInfoPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    using CommandSystem;

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
        public ICommand Command;

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

            var cmd = new TemplateCommand();
            cmd.AssignedAddonID = AddonID;
            cmd.DummyCommand = CommandName;
            cmd.DummyDescription = Description;
            cmd.Permission = Permission;

            if (IsRaCommand)
                MainClass.Singleton.Commands[typeof(RemoteAdminCommandHandler)].Add(cmd.GetType(), cmd);
            else
                MainClass.Singleton.Commands[typeof(ClientCommandHandler)].Add(cmd.GetType(), cmd);
        }

        /// <summary>
        /// Unregister command.
        /// </summary>
        public void UnregisterCommand()
        {
            if (IsRaCommand)
                MainClass.Singleton.Commands[typeof(RemoteAdminCommandHandler)].Remove(Command.GetType());
            else
                MainClass.Singleton.Commands[typeof(ClientCommandHandler)].Remove(Command.GetType());
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
