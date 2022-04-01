// -----------------------------------------------------------------------
// <copyright file="ExiledLibrary.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Models {
    using System;
    using System.Reflection;

    using Version = SemVer.Version;

    public readonly struct ExiledLibrary : IComparable, IComparable<ExiledLibrary> {
        public readonly Assembly Library;
        public readonly Version Version;

        public ExiledLibrary(Assembly lib) {
            Library = lib;
            Version = Version.Parse(lib.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
        }

        public int CompareTo(object obj) {
            if (obj is ExiledLibrary l)
                return CompareTo(l);
            else
                return 0;
        }

        public int CompareTo(ExiledLibrary other) => Version.CompareTo(other.Version);
    }
}
