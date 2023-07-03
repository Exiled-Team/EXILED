// -----------------------------------------------------------------------
// <copyright file="ApiProvider.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.GHApi
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Exiled.Updater.GHApi.Models;
    using Exiled.Updater.GHApi.Settings;

    using Utf8Json;

    public static class ApiProvider
    {
        public const string GetReleasesTemplate = "https://api.github.com/repositories/{0}/releases";

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