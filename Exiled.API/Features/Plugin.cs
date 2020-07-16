// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;

    using RemoteAdmin;

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class Plugin<TConfig> : IPlugin<TConfig>
        where TConfig : IConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
        /// </summary>
        public Plugin()
        {
            Name = Assembly.GetName().Name;
            Prefix = Name.ToSnakeCase();
            Author = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly, typeof(AssemblyCompanyAttribute), false))?.Company;
            Version = Assembly.GetName().Version;
        }

        /// <inheritdoc/>
        public Assembly Assembly { get; } = Assembly.GetCallingAssembly();

        /// <inheritdoc/>
        public virtual string Name { get; }

        /// <inheritdoc/>
        public virtual string Prefix { get; }

        /// <inheritdoc/>
        public virtual string Author { get; }

        /// <inheritdoc/>
        public virtual PluginPriority Priority { get; }

        /// <inheritdoc/>
        public virtual Version Version { get; }

        /// <inheritdoc/>
        public virtual Version RequiredExiledVersion { get; } = new Version(2, 0, 1);

        /// <inheritdoc/>
        public Dictionary<Type, Dictionary<Type, ICommand>> Commands { get; } = new Dictionary<Type, Dictionary<Type, ICommand>>()
        {
            { typeof(RemoteAdminCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(GameConsoleCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(ClientCommandHandler), new Dictionary<Type, ICommand>() },
        };

        /// <inheritdoc/>
        public TConfig Config { get; } = new TConfig();

        /// <inheritdoc/>
        public virtual void OnEnabled() => Log.Info($"{Name} v{Version.Major}.{Version.Minor}.{Version.Build}, made by {Author}, has been enabled!");

        /// <inheritdoc/>
        public virtual void OnDisabled() => Log.Info($"{Name} has been disabled!");

        /// <inheritdoc/>
        public virtual void OnReloaded() => Log.Info($"{Name} has been reloaded!");

        /// <inheritdoc/>
        public virtual void OnRegisteringCommands()
        {
            Log.Debug($"Registering commands...");

            foreach (Type type in Assembly.GetTypes())
            {
                if (type.GetInterface("ICommand") != typeof(ICommand))
                    continue;

                if (!Attribute.IsDefined(type, typeof(CommandHandlerAttribute)))
                    continue;

                // Useless for now.
                CommandHandlerAttribute commandHandlerAttribute = type.GetCustomAttribute<CommandHandlerAttribute>();

                // Since I cannot determine the type of the command from the CommandHandlerAttribute, because it's not being saved anywhere inside that class,
                // I just register commands for both RemoteAdmin and GameConsole.
                try
                {
                    // Every command will be registered as RemoteAdminCommandHandler, so I'll just do one check.
                    if (!Commands[typeof(RemoteAdminCommandHandler)].TryGetValue(type, out ICommand command))
                        command = (ICommand)Activator.CreateInstance(type);

                    CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                    GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);

                    Commands[typeof(ClientCommandHandler)][type] = command;
                    Commands[typeof(RemoteAdminCommandHandler)][type] = command;
                    Commands[typeof(GameConsoleCommandHandler)][type] = command;

                    Log.Debug($"Successfully registered command {command.Command}");
                }
                catch (Exception exception)
                {
                    Log.Error($"An error has occurred while registering a command: {exception}");
                }

                // Register ClientCommand
                try
                {
                    bool foundClient = false;
                    foreach (CustomAttributeData data in type.CustomAttributes)
                    {
                        if (data.AttributeType == typeof(CommandHandlerAttribute))
                        {
                            if (((Type)data.ConstructorArguments[0].Value) == typeof(ClientCommandHandler))
                            {
                                foundClient = true;
                                break;
                            }
                        }
                    }

                    if (foundClient)
                    {
                        if (!Commands[typeof(ClientCommandHandler)].TryGetValue(type, out ICommand command))
                            command = (ICommand)Activator.CreateInstance(type);

                        QueryProcessor.DotCommandHandler.RegisterCommand(command);

                        Commands[typeof(ClientCommandHandler)][type] = command;

                        Log.Debug($"Successfully registered client command {command.Command}");
                    }
                }
                catch (Exception exception)
                {
                    Log.Error($"An error has occurred while registering a client command: {exception}");
                }
            }

            Log.Debug($"Commands have been registered successfully!");
        }

        /// <inheritdoc/>
        public virtual void OnUnregisteringCommands()
        {
            foreach (KeyValuePair<Type, Dictionary<Type, ICommand>> types in Commands)
            {
                foreach (ICommand command in types.Value.Values)
                {
                    if (types.Key == typeof(RemoteAdminCommandHandler))
                        CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(command);
                    else if (types.Key == typeof(GameConsoleCommandHandler))
                        GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(command);
                    else if (types.Key == typeof(ClientCommandHandler))
                        QueryProcessor.DotCommandHandler.UnregisterCommand(command);
                }
            }
        }
    }
}
