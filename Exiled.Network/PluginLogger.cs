// -----------------------------------------------------------------------
// <copyright file="PluginLogger.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using System.Reflection;
    using Exiled.Network.API;

    /// <inheritdoc/>
    public class PluginLogger : NPLogger
    {
        /// <inheritdoc/>
        public override void Debug(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(MainClass).Assembly)
                Exiled.API.Features.Log.Debug(message);
            else
                Exiled.API.Features.Log.Debug($"[{callingAssembly.GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Error(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(MainClass).Assembly)
                Exiled.API.Features.Log.Error(message);
            else
                Exiled.API.Features.Log.Error($"[{callingAssembly.GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Info(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(MainClass).Assembly)
                Exiled.API.Features.Log.Info(message);
            else
                Exiled.API.Features.Log.Info($"[{callingAssembly.GetName().Name}] " + message);
        }
    }
}
