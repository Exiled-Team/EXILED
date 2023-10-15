// -----------------------------------------------------------------------
// <copyright file="ApiProvider.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.GHApi
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Exiled.Loader.GHApi.Models;
    using Exiled.Loader.GHApi.Settings;

    using Utf8Json;

    /// <summary>
    /// An API bridge to GitHub services.
    /// </summary>
    public static class ApiProvider
    {
        /// <summary>
        /// The API template to get releases.
        /// </summary>
        public const string GetReleasesTemplate = "https://api.github.com/repositories/{0}/releases";

        /// <summary>
        /// Gets all releases from a git repository.
        /// </summary>
        /// <param name="repoId">The repository from which get the releases.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="client">The <see cref="HttpClient"/>.</param>
        /// <returns>A <see cref="Release"/>[] containing all requested releases.</returns>
        public static async Task<Release[]> GetReleases(long repoId, GetReleasesSettings settings, HttpClient client)
        {
            string url = string.Format(GetReleasesTemplate, repoId) + settings.Build();
            using HttpResponseMessage httpResponse = await client.GetAsync(url).ConfigureAwait(false);
            using Stream streamContnet = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<Release[]>(streamContnet)
                .OrderByDescending(r => r.CreatedAt.Ticks)
                .ToArray();
        }
    }
}