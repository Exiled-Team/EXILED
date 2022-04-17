// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;

using Exiled.Launcher.Features;

if (!File.Exists(GlobalVariables.LocalAdminExecutable))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("LocalAdmin executable not found");
    Thread.Sleep(5000);
    return 1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Installing and updating EXILED!");
Installer.Run();

var f = new ProcessStartInfo(GlobalVariables.LocalAdminExecutable, string.Join(" ", args));
Process.Start(f);
return 0;
