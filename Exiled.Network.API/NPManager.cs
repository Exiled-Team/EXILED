// -----------------------------------------------------------------------
// <copyright file="NPManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API
{
    using System.Collections.Generic;
    using System.IO;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Network Manager.
    /// </summary>
    public class NPManager
    {
        /// <summary>
        /// Gets or Sets singleton of npmanager.
        /// </summary>
        public static NPManager Singleton { get; set; }

        /// <summary>
        /// Gets the serializer for configs.
        /// </summary>
        public static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        /// <summary>
        /// Gets the deserializer for configs.
        /// </summary>
        public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Gets or sets Packet Processor.
        /// </summary>
        public NetPacketProcessor PacketProcessor { get; set; } = new NetPacketProcessor();

        /// <summary>
        /// Gets or sets Logger.
        /// </summary>
        public NPLogger Logger { get; set; }

        /// <summary>
        /// Gets or sets Network Listener.
        /// </summary>
        public NetManager NetworkListener { get; set; }

        /// <summary>
        /// Gets or sets dictionary of all loaded addons.
        /// </summary>
        public Dictionary<string, IAddon<IConfig>> Addons { get; set; } = new Dictionary<string, IAddon<IConfig>>();

        /// <summary>
        /// Gets or sets dictionary of all online servers.
        /// </summary>
        public Dictionary<NetPeer, NPServer> Servers { get; set; } = new Dictionary<NetPeer, NPServer>();

        private Dictionary<string, Dictionary<string, ICommand>> Commands { get; set; } = new Dictionary<string, Dictionary<string, ICommand>>();

        /// <summary>
        /// Register command from addon.
        /// </summary>
        /// <param name="addon">Addon.</param>
        /// <param name="command">Command interface.</param>
        public void RegisterCommand(IAddon<IConfig> addon, ICommand command)
        {
            if (!Commands.ContainsKey(addon.AddonId))
                Commands.Add(addon.AddonId, new Dictionary<string, ICommand>());

            Commands[addon.AddonId].Add(command.CommandName.ToUpper(), command);
            Logger.Info($"Command {command.CommandName.ToUpper()} registered in addon {addon.AddonName}");
        }

        /// <summary>
        /// Register command.
        /// </summary>
        /// <param name="cmd">Command.</param>
        public virtual void OnRegisterCommand(CommandInfoPacket cmd)
        {
        }

        /// <summary>
        /// Unregister command.
        /// </summary>
        /// <param name="cmd">Command.</param>
        public virtual void OnUnregisterCommand(CommandInfoPacket cmd)
        {
        }

        /// <summary>
        /// Execute addon command.
        /// </summary>
        /// <param name="plr">Executing player.</param>
        /// <param name="addonId">Addon ID.</param>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        public void ExecuteCommand(PlayerFuncs plr, string addonId, string commandName, List<string> arguments)
        {
            if (Commands[addonId].ContainsKey(commandName.ToUpper()))
            {
                Commands[addonId][commandName.ToUpper()].Invoke(plr, arguments);
            }
        }

        /// <summary>
        /// Load addon config.
        /// </summary>
        /// <param name="addonId">Addon ID.</param>
        public void LoadAddonConfig(string addonId)
        {
            if (Addons.TryGetValue(addonId, out IAddon<IConfig> npdi))
            {
                if (!Directory.Exists(Path.Combine(npdi.DefaultPath, npdi.AddonName)))
                    Directory.CreateDirectory(Path.Combine(npdi.DefaultPath, npdi.AddonName));

                if (!File.Exists(Path.Combine(npdi.DefaultPath, npdi.AddonName, "config.yml")))
                    File.WriteAllText(Path.Combine(npdi.DefaultPath, npdi.AddonName, "config.yml"), Serializer.Serialize(npdi.Config));

                var cfg = (IConfig)Deserializer.Deserialize(File.ReadAllText(Path.Combine(npdi.DefaultPath, npdi.AddonName, "config.yml")), npdi.Config.GetType());
                File.WriteAllText(Path.Combine(npdi.DefaultPath, npdi.AddonName, "config.yml"), Serializer.Serialize(cfg));
                npdi.Config.CopyProperties(cfg);
            }
        }

        /// <summary>
        /// Get all loaded command from addon.
        /// </summary>
        /// <param name="addonId">Addon ID.</param>
        /// <returns>List of CommandInfoPacket.</returns>
        public List<CommandInfoPacket> GetCommands(string addonId)
        {
            List<CommandInfoPacket> cmds = new List<CommandInfoPacket>();
            if (Commands.TryGetValue(addonId, out Dictionary<string, ICommand> outCmds))
            {
                foreach (var cmd in outCmds)
                {
                    cmds.Add(new CommandInfoPacket()
                    {
                        AddonID = addonId,
                        CommandName = cmd.Key,
                        Description = cmd.Value.Description,
                        Permission = cmd.Value.Permission,
                        IsRaCommand = cmd.Value.IsRaCommand,
                    });
                }
            }

            return cmds;
        }
    }
}
