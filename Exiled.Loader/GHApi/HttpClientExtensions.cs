// -----------------------------------------------------------------------
// <copyright file="HttpClientExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.GHApi
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Exiled.Loader.GHApi.Models;
    using Exiled.Loader.GHApi.Settings;

    /// <summary>
    /// A set of extensions to be used along with https clients.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Gets all releases from a git repository.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/>.</param>
        /// <param name="repoId">The repository from which get the releases.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>A <see cref="Release"/>[] containing all requested releases.</returns>
        public static async Task<Release[]> GetReleases(this HttpClient client, long repoId, GetReleasesSettings settings)
            => await ApiProvider.GetReleases(repoId, settings, client).ConfigureAwait(false);
    }
}