// -----------------------------------------------------------------------
// <copyright file="TaggedRelease.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Models
{
    using Exiled.Loader.GHApi.Models;

    using SemanticVersioning;

    /// <summary>
    /// An asset containing all information about a tagged release.
    /// </summary>
    public readonly struct TaggedRelease
    {
        /// <summary>
        /// The release.
        /// </summary>
        public readonly Release Release;

        /// <summary>
        /// The asset of the release.
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedRelease"/> struct.
        /// </summary>
        /// <param name="release"><inheritdoc cref="Release"/></param>
        public TaggedRelease(Release release)
        {
            Release = release;
            Version = Version.Parse(release.TagName);
        }
    }
}