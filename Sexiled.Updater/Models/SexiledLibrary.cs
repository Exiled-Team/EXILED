// -----------------------------------------------------------------------
// <copyright file="SexiledLibrary.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Updater.Models
{
    using System;
    using System.Reflection;

    using Version = SemVer.Version;

    public readonly struct SexiledLibrary : IComparable, IComparable<SexiledLibrary>
    {
        public readonly Assembly Library;
        public readonly Version Version;

        public SexiledLibrary(Assembly lib)
        {
            Library = lib;
            Version = Version.Parse(lib.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
        }

        public int CompareTo(object obj)
        {
            if (obj is SexiledLibrary l)
                return CompareTo(l);
            else
                return 0;
        }

        public int CompareTo(SexiledLibrary other) => Version.CompareTo(other.Version);
    }
}
