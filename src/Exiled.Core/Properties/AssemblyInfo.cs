// -----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Exiled.Loader;

[assembly: Guid("1ABEC6CE-E209-4C38-AB45-2F3B7F6091CA")]
[assembly: AssemblyVersion(PluginManager._VERSION)]
[assembly: AssemblyFileVersion(PluginManager._VERSION)]
[assembly: AssemblyInformationalVersion(PluginManager._VERSION)]

[assembly: InternalsVisibleTo("Exiled.Events")]
[assembly: InternalsVisibleTo("Exiled.Updater")]
[assembly: InternalsVisibleTo("Exiled.Example")]
[assembly: InternalsVisibleTo("Exiled.Installer")]
[assembly: InternalsVisibleTo("Exiled.Permissions")]
