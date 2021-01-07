// -----------------------------------------------------------------------
// <copyright file="HttpClientExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.GHApi
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Exiled.Updater.GHApi.Models;

    public static class HttpClientExtensions
    {
        public static async Task<Release[]> GetReleases(this HttpClient client, long repoId)
            => await ApiProvider.GetReleases(repoId, client).ConfigureAwait(false);
    }
}
