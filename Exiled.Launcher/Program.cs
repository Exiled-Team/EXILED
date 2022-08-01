// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Exiled.Launcher.Features.Arguments;

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

// Starting Point Launcher
ProcessStartInfo startInfo = new ProcessStartInfo(arguments.StartingPoint, string.Join(' ', arguments.ExternalArguments));
Process startingPoint = Process.Start(startInfo)!;

// Wait for starting point to exit
startingPoint.WaitForExit();
