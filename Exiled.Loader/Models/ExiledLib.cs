// -----------------------------------------------------------------------
// <copyright file="ExiledLib.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Models
{
    using System;
    using System.Reflection;

    using Version = SemanticVersioning.Version;

    /// <summary>
    /// An asset containing all information about an assembly's version.
    /// </summary>
    public readonly struct ExiledLib : IComparable, IComparable<ExiledLib>
    {
        /// <summary>
        /// The assembly.
        /// </summary>
        public readonly Assembly Library;

        /// <summary>
        /// The version.
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExiledLib"/> struct.
        /// </summary>
        /// <param name="lib"><inheritdoc cref="Library"/></param>
        public ExiledLib(Assembly lib)
        {
            Library = lib;
            Version = Version.Parse(lib.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(object obj) => obj is ExiledLib l ? CompareTo(l) : 0;

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(ExiledLib other) => Version.CompareTo(other.Version);
    }
}