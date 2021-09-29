// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.DedicatedNetwork
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Host instance.
        /// </summary>
        private static Host host;

        private static void Main(string[] args)
        {
            if (!Directory.Exists("dependencies"))
                Directory.CreateDirectory("dependencies");
            string[] depsFIles = Directory.GetFiles("dependencies", "*.dll");
            foreach (var deps in depsFIles)
            {
                Assembly a = Assembly.LoadFrom(deps);
            }

            host = new Host();
            while (true)
            {
                var line = Console.ReadLine();
                var proc = line.Split(' ');
                foreach (var h in host.Addons)
                {
                    h.Value.OnConsoleCommand(proc[0], proc.Skip(1).ToList());
                }
            }
        }
    }
}
