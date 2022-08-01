// -----------------------------------------------------------------------
// <copyright file="AssemblyPatcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Exiled.Launcher.Features.Patcher;

public static class AssemblyPatcher
{
    public static void Patch(string path)
    {
        ModuleDefMD assembly = ModuleDefMD.Load(path);

        if (assembly is null)
        {
            Console.WriteLine($"[Patcher] Assembly in {path} not found. Could not patch.");
            return;
        }

        TypeDef? serverConsoleDef = HelpMethods.FindServerConsoleDefinition(assembly);

        if (serverConsoleDef is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole.");
            return;
        }

        MethodDef? startMethodDef = HelpMethods.FindStartMethodDefinition(serverConsoleDef);

        if (startMethodDef is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole::Start()");
            return;
        }



    }
}
