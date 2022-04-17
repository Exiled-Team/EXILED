// -----------------------------------------------------------------------
// <copyright file="GlobalVariables.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Launcher.Features;

public static class GlobalVariables
{
    public static string LocalAdminExecutable = OperatingSystem.IsLinux() ? "LocalAdmin" : "LocalAdmin.exe";

}
