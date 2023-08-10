// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#pragma warning disable SA1402
#pragma warning disable SA1201 // Elements should appear in the correct order

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CommandSystem;
    using Enums;
    using Extensions;
    using HarmonyLib;
    using Interfaces;
    using RemoteAdmin;

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class Plugin<TConfig> : IPlugin<TConfig>
        where TConfig : IConfig, new()
    {
        private static PropertyInfo SetCommand { get; } = typeof(ICommand).GetProperty(nameof(ICommand.Command), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
        /// </summary>
        public Plugin()
        {
            Assembly = Assembly.GetCallingAssembly();
            Name = Assembly.GetName().Name;
            Prefix = Name.ToSnakeCase();
            Author = Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Version = Assembly.GetName().Version;
        }

        /// <inheritdoc/>
        public Assembly Assembly { get; protected set; }

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
        public virtual Version RequiredExiledVersion { get; } = typeof(IPlugin<>).Assembly.GetName().Version;

        /// <inheritdoc/>
        public virtual bool IgnoreRequiredVersionCheck { get; } = false;

        /// <inheritdoc/>
        public Dictionary<Type, Dictionary<Type, ICommand>> Commands { get; } = new()
        {
            { typeof(RemoteAdminCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(GameConsoleCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(ClientCommandHandler), new Dictionary<Type, ICommand>() },
        };

        /// <inheritdoc/>
        public TConfig Config { get; } = new();

        /// <inheritdoc/>
        public ITranslation InternalTranslation { get; protected set; }

        /// <inheritdoc/>
        public string ConfigPath => Paths.GetConfigPath(Prefix);

        /// <inheritdoc/>
        public string TranslationPath => Paths.GetTranslationPath(Prefix);

        /// <inheritdoc/>
        public virtual void OnEnabled()
        {
            AssemblyInformationalVersionAttribute attribute = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Log.Info($"{Name} v{(Version is not null ? $"{Version.Major}.{Version.Minor}.{Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)} by {Author} has been enabled!");
        }

        /// <inheritdoc/>
        public virtual void OnDisabled() => Log.Info($"{Name} has been disabled!");

        /// <inheritdoc/>
        public virtual void OnReloaded() => Log.Info($"{Name} has been reloaded!");

        /// <inheritdoc/>
        public virtual void OnRegisteringCommands()
        {
            foreach (Type type in Assembly.GetTypes())
            {
                if (type.GetInterface("ICommand") != typeof(ICommand))
                    continue;

                if (!Attribute.IsDefined(type, typeof(CommandHandlerAttribute)))
                    continue;

                foreach (CustomAttributeData customAttributeData in type.CustomAttributes)
                {
                    try
                    {
                        if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute))
                            continue;

                        Type commandType = (Type)customAttributeData.ConstructorArguments?[0].Value;

                        if (!Commands.TryGetValue(commandType, out Dictionary<Type, ICommand> typeCommands))
                            continue;

                        if (!typeCommands.TryGetValue(type, out ICommand command))
                            command = (ICommand)Activator.CreateInstance(type);

                        command.Aliases.AddToArray(command.Command);
                        SetCommand.SetValue(command, Prefix + command.Command);

                        try
                        {
                            if (commandType == typeof(RemoteAdminCommandHandler))
                                CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                            else if (commandType == typeof(GameConsoleCommandHandler))
                                GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
                            else if (commandType == typeof(ClientCommandHandler))
                                QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        }
                        catch (ArgumentException e)
                        {
                            if (e.Message.StartsWith("An"))
                            {
                                Log.Warn($"Command with same name has already registered! Command: {command.Command}");
                            }
                            else
                            {
                                Log.Error($"An error has occurred while registering a command: {e}");
                            }
                        }

                        Commands[commandType][type] = command;
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"An error has occurred while registering a command: {exception}");
                    }
                }
            }
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

        /// <inheritdoc/>
        public int CompareTo(IPlugin<IConfig> other) => -Priority.CompareTo(other.Priority);
    }

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    /// <typeparam name="TTranslation">The translation type.</typeparam>
    public abstract class Plugin<TConfig, TTranslation> : Plugin<TConfig>
        where TConfig : IConfig, new()
        where TTranslation : ITranslation, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin{TConfig, TTranslation}"/> class.
        /// </summary>
        public Plugin()
        {
            Assembly = Assembly.GetCallingAssembly();
            InternalTranslation = new TTranslation();
        }

        /// <summary>
        /// Gets the plugin translations.
        /// </summary>
        public TTranslation Translation => (TTranslation)InternalTranslation;
    }
}