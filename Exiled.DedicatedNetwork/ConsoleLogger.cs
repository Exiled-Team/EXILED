// -----------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.DedicatedNetwork
{
    using System;
    using System.Reflection;
    using Exiled.Network.API;

    /// <inheritdoc/>
    public class ConsoleLogger : NPLogger
    {
        /// <inheritdoc/>
        public override void Debug(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Error(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [ERROR] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Info(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [INFO] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
