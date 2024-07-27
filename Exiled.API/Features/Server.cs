// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Exiled.API.Enums;
    using GameCore;

    using Interfaces;

    using MEC;

    using Mirror;

    using PlayerRoles.RoleAssign;

    using RoundRestarting;

    using UnityEngine;

    /// <summary>
    /// A set of tools to easily work with the server.
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Gets a dictionary that pairs assemblies with their associated plugins.
        /// </summary>
        public static Dictionary<Assembly, IPlugin<IConfig>> PluginAssemblies { get; } = new();

        /// <summary>
        /// Gets the Player of the server.
        /// Might be <see langword="null"/> when called when the server isn't loaded.
        /// </summary>
        public static Player Host { get; internal set; }

        /// <summary>
        /// Gets the cached <see cref="global::Broadcast"/> component.
        /// </summary>
        public static global::Broadcast Broadcast => global::Broadcast.Singleton;

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public static string Name
        {
            get => ServerConsole._serverName;
            set
            {
                ServerConsole._serverName = value;
                ServerConsole.singleton.RefreshServerName();
            }
        }

        /// <summary>
        /// Gets the server's version.
        /// </summary>
        public static string Version => GameCore.Version.VersionString;

        /// <summary>
        /// Gets a value indicating whether or not streaming of this version is allowed.
        /// </summary>
        public static bool StreamingAllowed => GameCore.Version.StreamingAllowed;

        /// <summary>
        /// Gets a value indicating whether or not this server is on a beta version of SCP:SL.
        /// </summary>
        public static bool IsBeta => GameCore.Version.PublicBeta || GameCore.Version.PrivateBeta;

        /// <summary>
        /// Gets a value indicating the type of build this server is hosted on.
        /// </summary>
        public static GameCore.Version.VersionType BuildType => GameCore.Version.BuildType;

        /// <summary>
        /// Gets the RemoteAdmin permissions handler.
        /// </summary>
        public static PermissionsHandler PermissionsHandler => ServerStatic.PermissionsHandler;

        /// <summary>
        /// Gets the Ip address of the server.
        /// </summary>
        public static string IpAddress => ServerConsole.Ip;

        /// <summary>
        /// Gets a value indicating whether or not this server is a dedicated server.
        /// </summary>
        public static bool IsDedicated => ServerStatic.IsDedicated;

        /// <summary>
        /// Gets the port of the server.
        /// </summary>
        public static ushort Port => ServerStatic.ServerPort;

        /// <summary>
        /// Gets the current TPS (Ticks per Second) of the Server.
        /// </summary>
        public static double Tps => Math.Round(1f / Time.smoothDeltaTime);

        /// <summary>
        /// Gets or sets the max TPS (Ticks per Second) of the Server.
        /// </summary>
        public static short MaxTps
        {
            get => ServerStatic.ServerTickrate;
            set => ServerStatic.ServerTickrate = value;
        }

        /// <summary>
        /// Gets the actual frametime of the server.
        /// </summary>
        public static double Frametime => Math.Round(1f / Time.deltaTime);

        /// <summary>
        /// Gets or sets a value indicating whether or not friendly fire is enabled.
        /// </summary>
        /// <seealso cref="Player.IsFriendlyFireEnabled"/>
        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set
            {
                ServerConsole.FriendlyFire = value;
                ServerConfigSynchronizer.Singleton.RefreshMainBools();

                PlayerStatsSystem.AttackerDamageHandler.RefreshConfigs();
            }
        }

        /// <summary>
        /// Gets the number of players currently on the server.
        /// </summary>
        /// <seealso cref="Player.List"/>
        public static int PlayerCount => Player.Dictionary.Count;

        /// <summary>
        /// Gets or sets the maximum number of players able to be on the server.
        /// </summary>
        public static int MaxPlayerCount
        {
            get => CustomNetworkManager.slots;
            set => CustomNetworkManager.slots = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not late join is enabled.
        /// </summary>
        public static bool LateJoinEnabled => LateJoinTime > 0;

        /// <summary>
        /// Gets the late join time, in seconds. If a player joins less than this many seconds into a game, they will be given a random class.
        /// </summary>
        public static float LateJoinTime => ConfigFile.ServerConfig.GetFloat(RoleAssigner.LateJoinKey, 0f);

        /// <summary>
        /// Gets or sets a value indicating whether the server is marked as Heavily Modded.
        /// <remarks>
        /// Read the VSR for more info about its usage.
        /// </remarks>
        /// </summary>
        public static bool IsHeavilyModded
        {
            get => ServerConsole.TransparentlyModdedServerConfig;
            set => ServerConsole.TransparentlyModdedServerConfig = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this server has the whitelist enabled.
        /// </summary>
        public static bool IsWhitelisted
        {
            get => ServerConsole.WhiteListEnabled;
            set => ServerConsole.WhiteListEnabled = value;
        }

        /// <summary>
        /// Gets the List of player currently whitelisted.
        /// </summary>
        public static HashSet<string> WhitelistedPlayers => WhiteList.Users;

        /// <summary>
        /// Gets a value indicating whether or not this server is verified.
        /// </summary>
        public static bool IsVerified => CustomNetworkManager.IsVerified;

        /// <summary>
        /// Gets or sets a value indicating whether or not idle mode is enabled.
        /// </summary>
        public static bool IsIdleModeEnabled
        {
            get => IdleMode.IdleModeEnabled;
            set => IdleMode.IdleModeEnabled = value;
        }

        /// <summary>
        /// Gets the dictionary of the server's session variables.
        /// <para>
        /// Session variables can be used to save temporary data. Data is stored in a <see cref="Dictionary{TKey, TValue}"/>.
        /// The key of the data is always a <see cref="string"/>, whereas the value can be any <see cref="object"/>.
        /// The data stored in a session variable can be accessed by different assemblies; it is recommended to uniquely identify stored data so that it does not conflict with other plugins that may also be using the same name.
        /// Data saved with session variables is not being saved on server restart. If the data must be saved after a restart, a database must be used instead.
        /// </para>
        /// </summary>
        public static Dictionary<string, object> SessionVariables { get; } = new();

        /// <summary>
        /// Gets or sets a list with all fake SyncVar that will be applied to player ass soon as he connects.
        /// <para>
        /// As string argument use a full name of Network property ('full type name'.'property name').
        /// As object argument use value that will be used instead of a current value.
        /// </para>
        /// </summary>
        public static Dictionary<string, object> FakeSyncVars { get; set; } = new();

        /// <summary>
        /// Adds a new value to <see cref="FakeSyncVars"/>.
        /// </summary>
        /// <param name="propertyInfo">A property of a sync var (starts with "Network").</param>
        /// <param name="newValue">The value that will replace actual one.</param>
        public static void AddFakeSyncVar(PropertyInfo propertyInfo, object newValue)
        {
            if (propertyInfo.PropertyType != newValue.GetType())
            {
                Log.Error($"Type mismatch between property info type ({propertyInfo.PropertyType}) and new value type ({newValue.GetType()})");
                return;
            }

            string fullName = propertyInfo.DeclaringType!.FullName + '.' + propertyInfo.Name;

            if (FakeSyncVars.ContainsKey(fullName))
                FakeSyncVars.Remove(fullName);

            FakeSyncVars.Add(fullName, newValue);
        }

        /// <summary>
        /// Adds a new value to <see cref="FakeSyncVars"/>.
        /// </summary>
        /// <param name="fieldInfo">The sync var field.</param>
        /// <param name="newValue">The new value that will replace actual one.</param>
        public static void AddFakeSyncVar(FieldInfo fieldInfo, object newValue) =>
            AddFakeSyncVar(fieldInfo.DeclaringType!.GetProperty("Network" + fieldInfo.Name), newValue);

        /// <summary>
        /// Adds a new value to <see cref="FakeSyncVars"/>.
        /// </summary>
        /// <param name="type">Type where sync var is declared.</param>
        /// <param name="name">Name of sync var field or property.</param>
        /// <param name="newValue">The new value that will replace actual one.</param>
        public static void AddFakeSyncVar(Type type, string name, object newValue)
        {
            name = name.Replace(type.Name, string.Empty);
            AddFakeSyncVar(name.StartsWith("Network") ? type.GetProperty(name) : type.GetProperty("Network" + name), newValue);
        }

        /// <summary>
        /// Restarts the server, reconnects all players.
        /// </summary>
        /// <seealso cref="RestartRedirect(ushort)"/>
        public static void Restart() => Round.Restart(false, true, ServerStatic.NextRoundAction.Restart);

        /// <summary>
        /// Shutdowns the server, disconnects all players.
        /// </summary>
        /// <seealso cref="ShutdownRedirect(ushort)"/>
        public static void Shutdown() => global::Shutdown.Quit();

        /// <summary>
        /// Redirects players to a server on another port, restarts the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns><see langword="true"/> if redirection was successful; otherwise, <see langword="false"/>.</returns>
        /// <remarks>If the returned value is <see langword="false"/>, the server won't restart.</remarks>
        public static bool RestartRedirect(ushort redirectPort)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Restart);

            return true;
        }

        /// <summary>
        /// Redirects players to a server on another port, shutdowns the current server.
        /// </summary>
        /// <param name="redirectPort">The port to redirect players to.</param>
        /// <returns><see langword="true"/> if redirection was successful; otherwise, <see langword="false"/>.</returns>
        /// <remarks>If the returned value is <see langword="false"/>, the server won't shutdown.</remarks>
        public static bool ShutdownRedirect(ushort redirectPort)
        {
            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.0f, redirectPort, true, false));
            Timing.CallDelayed(0.5f, Shutdown);

            return true;
        }

        /// <summary>
        /// Executes a server command.
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="sender">The <see cref="CommandSender"/> running the command.</param>
        /// <returns>Command response, if there is one; otherwise, <see langword="null"/>.</returns>
        public static string ExecuteCommand(string command, CommandSender sender = null) => GameCore.Console.singleton.TypeCommand(command, sender);

        /// <summary>
        /// Safely gets an <see cref="object"/> from <see cref="SessionVariables"/>, then casts it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="result">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter is used.</param>
        /// <returns><see langword="true"/> if the SessionVariables contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetSessionVariable<T>(string key, out T result)
        {
            if (SessionVariables.TryGetValue(key, out object value) && value is T type)
            {
                result = type;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Emulation of the method SCP:SL uses to change scene.
        /// </summary>
        /// <param name="newSceneName">The new Scene the client will load.</param>
        public static void ChangeSceneToAllClients(string newSceneName)
        {
            SceneMessage message = new()
            {
                sceneName = newSceneName,
            };

            NetworkServer.SendToAll(message);
        }

        /// <summary>
        /// Emulation of the method SCP:SL uses to change scene.
        /// </summary>
        /// <param name="scene">The new Scene the client will load.</param>
        public static void ChangeSceneToAllClients(ScenesType scene)
        {
            SceneMessage message = new()
            {
                sceneName = scene.ToString(),
            };

            NetworkServer.SendToAll(message);
        }
    }
}