// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;

using Exiled.Launcher.Features.Arguments;
using Exiled.Launcher.Features.Patcher;

LauncherArguments arguments = ArgumentParser.GetArguments(args);

if (arguments.Help)
{
    ArgumentParser.ShowHelp();
    return;
}


if (!File.Exists(arguments.StartingPoint))
{
    Console.WriteLine($"The starting point provided ({arguments.StartingPoint}) does not exist.");
    Console.WriteLine("Make sure Exiled.Launcher is inside the server folder.");
    Thread.Sleep(5000);
    return;
}

if (arguments.InjectExiled)
{
    string serverAssembly = Path.Combine(arguments.ServerFolder, "Managed", "Assembly-CSharp.dll");

    if (!File.Exists(serverAssembly))
    {
        Console.WriteLine("Could not inject exiled.");
        Console.WriteLine($"Game assembly not found in path {serverAssembly}. Make sure the specified server folder is right.");
        Thread.Sleep(5000);
        return;
    }

    AssemblyPatcher.Patch(serverAssembly);
}

Thread.Sleep(5000);

// Starting Point Launcher
ProcessStartInfo startInfo = new ProcessStartInfo(arguments.StartingPoint, string.Join(' ', arguments.ExternalArguments));
Process startingPoint = Process.Start(startInfo)!;

// Wait for starting point to exit
startingPoint.WaitForExit();
