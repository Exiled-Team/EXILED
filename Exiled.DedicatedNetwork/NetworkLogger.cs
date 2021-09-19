// -----------------------------------------------------------------------
// <copyright file="NetworkLogger.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.DedicatedNetwork
{
    using System;

    using LiteNetLib;

    /// <summary>
    /// Network Logger.
    /// </summary>
    public class NetworkLogger : INetLogger
    {
        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [{level}] {string.Format(str, args)}");
        }
    }
}
