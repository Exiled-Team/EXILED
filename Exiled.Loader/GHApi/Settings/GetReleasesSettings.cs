// -----------------------------------------------------------------------
// <copyright file="GetReleasesSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.GHApi.Settings
{
    using UnityEngine;

    /// <summary>
    /// An asset containing all settings to be used when getting releases.
    /// </summary>
    public readonly struct GetReleasesSettings
    {
        /// <summary>
        /// The amount of results per page to be shown.
        /// </summary>
        public readonly byte PerPage;

        /// <summary>
        /// The page.
        /// </summary>
        public readonly uint Page;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetReleasesSettings"/> struct.
        /// </summary>
        /// <param name="perPage"><inheritdoc cref="PerPage"/></param>
        /// <param name="page"><inheritdoc cref="Page"/></param>
        public GetReleasesSettings(byte perPage, uint page)
        {
            PerPage = (byte)Mathf.Clamp(perPage, 1, 100);
            Page = page;
        }

        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <returns>A query containing the specified settings.</returns>
        public string Build() => (PerPage == 0) && (Page == 0) ? string.Empty : $"?per_page={PerPage}&page={Page}";
    }
}