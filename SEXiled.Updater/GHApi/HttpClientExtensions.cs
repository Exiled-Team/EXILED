// -----------------------------------------------------------------------
// <copyright file="HttpClientExtensions.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Updater.GHApi
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using SEXiled.Updater.GHApi.Models;
    using SEXiled.Updater.GHApi.Settings;

    public static class HttpClientExtensions
    {
        public static async Task<Release[]> GetReleases(this HttpClient client, long repoId, GetReleasesSettings settings)
            => await ApiProvider.GetReleases(repoId, settings, client).ConfigureAwait(false);
    }
}
