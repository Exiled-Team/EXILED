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
    using Exiled.API.Extensions;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Network Manager.
    /// </summary>
    public class NPManager
    {
        /// <summary>
        /// Gets or sets dictionary of all loaded addons.
        /// </summary>
        public Dictionary<string, NPAddonItem> Addons = new Dictionary<string, NPAddonItem>();

        /// <summary>
        /// Gets or sets dictionary of all online servers.
        /// </summary>
        public Dictionary<NetPeer, NPServer> Servers = new Dictionary<NetPeer, NPServer>();

        private readonly Dictionary<string, Dictionary<string, ICommand>> commands = new Dictionary<string, Dictionary<string, ICommand>>();

        /// <summary>
        /// Register command from addon.
        /// </summary>
        /// <param name="addonId">Addon ID.</param>
        /// <param name="command">Command interface.</param>
        public void RegisterCommand(string addonId, ICommand command)
        {
            if (!commands.ContainsKey(addonId))
                commands.Add(addonId, new Dictionary<string, ICommand>());
            if (!commands[addonId].ContainsKey(command.CommandName.ToUpper()))
            {
                commands[addonId].Add(command.CommandName.ToUpper(), command);
                Logger.Info($"Command {command.CommandName.ToUpper()} registered in addon {addonId}");
            }
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
            if (commands[addonId].ContainsKey(commandName.ToUpper()))
            {
                commands[addonId][commandName.ToUpper()].Invoke(plr, arguments);
            }
        }

        /// <summary>
        /// Load addon config.
        /// </summary>
        /// <param name="addonId">Addon ID.</param>
        public void LoadAddonConfig(string addonId)
        {
            if (Addons.TryGetValue(addonId, out NPAddonItem npdi))
            {
                if (!Directory.Exists(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName)))
                    Directory.CreateDirectory(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName));

                if (!File.Exists(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName, "config.yml")))
                    File.WriteAllText(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName, "config.yml"), YamlDS.Serializer.Serialize(npdi.Addon.Config));

                var cfg = (IConfig)Loader.Loader.Deserializer.Deserialize(File.ReadAllText(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName, "config.yml")), npdi.Addon.Config.GetType());
                File.WriteAllText(Path.Combine(npdi.Addon.DefaultPath, npdi.Info.AddonName, "config.yml"), YamlDS.Serializer.Serialize(cfg));
                npdi.Addon.Config.CopyProperties(cfg);
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
            if (commands.TryGetValue(addonId, out Dictionary<string, ICommand> outCmds))
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

        /// <summary>
        /// Logger.
        /// </summary>
        public static NPLogger Logger;

        /// <summary>
        /// Packet Processor.
        /// </summary>
        public readonly NetPacketProcessor PacketProcessor = new NetPacketProcessor();

        /// <summary>
        /// Network Listener.
        /// </summary>
        public NetManager NetworkListener;
    }
}
