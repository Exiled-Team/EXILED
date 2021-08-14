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
            Exiled.API.Features.Log.Debug($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Error(string message)
        {
            Exiled.API.Features.Log.Error($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Info(string message)
        {
            Exiled.API.Features.Log.Info($"[{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
