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
    try
    {
        Stopwatch sw = Stopwatch.StartNew();
        AssemblyPatcher.Patch(Path.Combine(arguments.ServerFolder, "Managed"));
        sw.Stop();

        Console.WriteLine($"Assembly patched! {sw.ElapsedMilliseconds}ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred while patching the assembly:");
        Console.WriteLine(e);
        Thread.Sleep(5000);
        return;
    }
}

Thread.Sleep(10000);

// Starting Point Launcher
ProcessStartInfo startInfo = new ProcessStartInfo(arguments.StartingPoint, string.Join(' ', arguments.ExternalArguments));
Process startingPoint = Process.Start(startInfo)!;

// Wait for starting point to exit
startingPoint.WaitForExit();
