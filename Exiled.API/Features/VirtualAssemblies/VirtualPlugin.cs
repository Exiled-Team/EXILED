// -----------------------------------------------------------------------
// <copyright file="VirtualPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.VirtualAssemblies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Attributes;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.VirtualAssemblies.EventArgs;
    using Exiled.API.Interfaces;
    using LiteNetLib.Utils;

    /// <summary>
    /// Represents the base class for a virtual plugin in the system.
    /// </summary>
    public abstract class VirtualPlugin : TypeCastObject<VirtualPlugin>
    {
        private static readonly List<VirtualPlugin> Registered = new();

        private VirtualPluginAttribute project;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before enabling a <see cref="VirtualPlugin"/>.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<EnablingVirtualPluginEventArgs> EnablingVirtualPluginDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before disabling a <see cref="VirtualPlugin"/>.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<DisablingVirtualPluginEventArgs> DisablingVirtualPluginDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before reloading a <see cref="VirtualPlugin"/>.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<ReloadingVirtualPluginEventArgs> ReloadingVirtualPluginDispatcher { get; set; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all registered <see cref="VirtualPlugin"/> instances.
        /// </summary>
        public static IEnumerable<VirtualPlugin> List => Registered;

        /// <summary>
        /// Gets or sets the <see cref="ConfigSubsystem"/> object.
        /// </summary>
        public abstract ConfigSubsystem Config { get; protected set; }

        /// <summary>
        /// Gets the plugin's master branch name.
        /// </summary>
        public string Master => Project.Master;

        /// <summary>
        /// Gets the plugin's name.
        /// </summary>
        public string Name => Project.Name;

        /// <summary>
        /// Gets the plugin's prefix.
        /// </summary>
        public string Prefix => Project.Prefix;

        /// <summary>
        /// Gets the plugin's <see cref="System.Version"/>.
        /// </summary>
        public abstract Version Version { get; }

        /// <summary>
        /// Gets the plugin's <see cref="UEBranchType"/>.
        /// </summary>
        public abstract UEBranchType BranchType { get; }

        /// <summary>
        /// Gets a value indicating whether the plugin is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        private VirtualPluginAttribute Project => project ??= GetType().GetCustomAttributes(typeof(VirtualPluginAttribute), true).FirstOrDefault() as VirtualPluginAttribute;

        /// <summary>
        /// Gets a <see cref="VirtualPlugin"/> given a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type of the <see cref="VirtualPlugin"/> to look for.</param>
        /// <returns>The corresponding <see cref="VirtualPlugin"/>, or <see langword="null"/> if not found.</returns>
        /// <exception cref="InvalidTypeException">Thrown when the provided <see cref="Type"/> does not inherit from <see cref="VirtualPlugin"/>.</exception>
        public static VirtualPlugin Get(Type type)
        {
            if (typeof(VirtualPlugin).IsAssignableFrom(type))
                throw new InvalidTypeException($"Type {type.Name} does not inherit from VirtualPlugin.");

            return List.FirstOrDefault(vp => vp.GetType() == type);
        }

        /// <summary>
        /// Gets a <see cref="VirtualPlugin"/> given a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="VirtualPlugin"/> to look for.</typeparam>
        /// <returns>The corresponding <see cref="VirtualPlugin"/> cast to <typeparamref name="T"/>, or <see langword="null"/> if not found.</returns>
        /// <exception cref="InvalidTypeException">Thrown when the provided <see cref="Type"/> does not inherit from <see cref="VirtualPlugin"/>.</exception>
        public static T Get<T>()
            where T : VirtualPlugin => Get(typeof(T)).Cast<T>();

        /// <summary>
        /// Reloads a <see cref="VirtualPlugin"/>.
        /// </summary>
        /// <param name="type">The type of the <see cref="VirtualPlugin"/> to reload.</param>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/> was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Reload(Type type)
        {
            VirtualPlugin vp = Get(type);

            if (!vp)
                return false;

            vp.OnReloaded();
            return true;
        }

        /// <summary>
        /// Reloads a <see cref="VirtualPlugin"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="VirtualPlugin"/> to reload.</typeparam>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/> was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Reload<T>()
            where T : VirtualPlugin => Reload(typeof(T));

        /// <summary>
        /// Reloads all the virtual plugins present in the assembly.
        /// </summary>
        public static void ReloadAll() => List.ForEach(vp => Reload(vp.GetType()));

        /// <summary>
        /// Reloads a <see cref="VirtualPlugin"/>'s config.
        /// </summary>
        /// <param name="type">The type of the <see cref="VirtualPlugin"/> to look for.</param>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/>'s config was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool ReloadConfig(Type type)
        {
            VirtualPlugin vp = Get(type);
            return vp && vp.ReloadConfig();
        }

        /// <summary>
        /// Reloads a <see cref="VirtualPlugin"/>'s config.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="VirtualPlugin"/> to look for.</typeparam>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/>'s config was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool ReloadConfig<T>() => ReloadConfig(typeof(T));

        /// <summary>
        /// Enables all virtual plugins present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="VirtualPlugin"/> containing all enabled virtual plugins.</returns>
        public static IEnumerable<VirtualPlugin> EnableAll()
        {
            List<VirtualPlugin> vps = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (!typeof(VirtualPlugin).IsAssignableFrom(type))
                    continue;

                VirtualPlugin vp = Activator.CreateInstance(type) as VirtualPlugin;
                vp.TryRegister(type.GetCustomAttribute(typeof(VirtualPluginAttribute)) as VirtualPluginAttribute);
                vps.Add(vp);
            }

            Log.Info($"{vps.Count()} virtual plugins have been successfully registered!");

            return vps;
        }

        /// <summary>
        /// Disables all virtual plugins present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="VirtualPlugin"/> containing all disabled virtual plugins.</returns>
        public static IEnumerable<VirtualPlugin> DisableAll()
        {
            List<VirtualPlugin> vps = new();
            vps.AddRange(List.Where(vpl => vpl.TryUnregister()));

            Log.Info($"{vps.Count()} virtual plugins have been successfully unregistered!");

            return vps;
        }

        /// <summary>
        /// Reloads the <see cref="VirtualPlugin"/>'s config.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/>'s config was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public bool ReloadConfig()
        {
            if (!Config)
                return false;

            ConfigSubsystem.Load(Config);
            return true;
        }

        /// <summary>
        /// Fired after enabling the <see cref="VirtualPlugin"/>.
        /// </summary>
        protected virtual void OnEnabled()
        {
            SubscribeEvents();
            Log.Info($"[VirtualPlugin] [{Prefix}] {Name} v{Version} has been enabled!");
            Log.Info($"[VirtualPlugin] [{Prefix}] {Name} Branch Type: {BranchType} - Master: {Master}");
            IsRunning = true;
        }

        /// <summary>
        /// Fired after disabling the <see cref="VirtualPlugin"/>.
        /// </summary>
        protected virtual void OnDisabled()
        {
            DisablingVirtualPluginEventArgs ev = new(this, true);
            DisablingVirtualPluginDispatcher.InvokeAll(ev);

            if (!ev.IsAllowed)
                return;

            DestroyInstance();
            Registered.Remove(this);
            UnsubscribeEvents();
            IsRunning = false;

            Log.Info($"[VirtualPlugin] [{Prefix}] {Name} v{Version} has been disabled!");
        }

        /// <summary>
        /// Fired after reloading the <see cref="VirtualPlugin"/>.
        /// </summary>
        protected virtual void OnReloaded()
        {
            ReloadingVirtualPluginEventArgs ev = new(this, true);
            ReloadingVirtualPluginDispatcher.InvokeAll(ev);

            if (!ev.IsAllowed)
                return;

            Log.Info($"[VirtualPlugin] [{Prefix}] {Name} v{Version} will be reloaded!");

            DestroyInstance();

            try
            {
                UnsubscribeEvents();
            }
            catch
            {
            }

            IsRunning = false;

            OnEnabled_Internal();
        }

        /// <summary>
        /// Fired before the <see cref="VirtualPlugin"/> is enabled.
        /// </summary>
        protected virtual void CreateInstance()
        {
            Singleton<VirtualPlugin>.Create(this);
        }

        /// <summary>
        /// Fired before the <see cref="VirtualPlugin"/> is disabled.
        /// </summary>
        protected virtual void DestroyInstance()
        {
            Config = null;
            Singleton<VirtualPlugin>.Destroy(this);
        }

        /// <summary>
        /// Fired after enabling the <see cref="VirtualPlugin"/>.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            DynamicEventManager.CreateFromTypeInstance(this);
        }

        /// <summary>
        /// Fired after disabling the <see cref="VirtualPlugin"/>.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            DynamicEventManager.DestroyFromTypeInstance(this);
        }

        /// <inheritdoc cref="OnEnabled"/>
        private void OnEnabled_Internal()
        {
            if (Config.Base is not IConfig config)
                throw new InvalidTypeException("Virtual plugin's config must inherit from IConfig.");

            EnablingVirtualPluginEventArgs ev = new(this, true);
            EnablingVirtualPluginDispatcher.InvokeAll(ev);

            if (!ev.IsAllowed)
                return;

            CreateInstance();

            if (!List.Contains(this))
                Registered.Add(this);

            if (!config.IsEnabled)
            {
                Log.Warn($"[VirtualPlugin] [{Prefix}] {Name} is not enabled.");
                return;
            }

            OnEnabled();
        }

        /// <summary>
        /// Tries to register a <see cref="VirtualPlugin"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/> was registered; otherwise, <see langword="false"/>.</returns>
        private bool TryRegister(VirtualPluginAttribute branchAttribute = null)
        {
            if (!List.Contains(this))
            {
                if (branchAttribute is not null)
                    project = branchAttribute;

                VirtualPlugin duplicate = List.FirstOrDefault(vpl => vpl.Name == Name);
                if (duplicate)
                {
                    Log.Error($"Unable to register {Name}. Another virtual plugin has been registered with the same Name: {duplicate.Name}");

                    return false;
                }

                OnEnabled_Internal();

                return true;
            }

            Log.Debug($"Unable to register {Name}. Another identical virtual plugin already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="VirtualPlugin"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="VirtualPlugin"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        private bool TryUnregister()
        {
            if (!List.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Virtual plugin is not yet registered.");

                return false;
            }

            OnDisabled();

            return true;
        }
    }
}
