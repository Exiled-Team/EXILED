// -----------------------------------------------------------------------
// <copyright file="HelpMethods.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using dnlib.DotNet;

namespace Exiled.Launcher.Features.Patcher;

public static class HelpMethods
{
    public static TypeDef? FindServerConsoleDefinition(ModuleDefMD assembly)
    {
        foreach (var type in assembly.Types)
        {
            if (type.FullName == "ServerConsole")
                return type;
        }

        return null;
    }

    public static MethodDef? FindStartMethodDefinition(TypeDef serverConsole)
    {
        foreach (var method in serverConsole.Methods)
        {
            if (method.Name == "Start")
                return method;
        }

        return null;
    }
}
