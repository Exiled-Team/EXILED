// -----------------------------------------------------------------------
// <copyright file="GetReleasesSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.GHApi.Settings
{
    using UnityEngine;

    public readonly struct GetReleasesSettings
    {
        public readonly byte PerPage;
        public readonly uint Page;

        public GetReleasesSettings(byte perPage, uint page)
        {
            PerPage = (byte)Mathf.Clamp(perPage, 1, 100);
            Page = page;
        }

        public string Build() => (PerPage == 0) && (Page == 0) ? string.Empty : $"?per_page={PerPage}&page={Page}";
    }
}