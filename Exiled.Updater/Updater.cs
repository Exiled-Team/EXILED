// -----------------------------------------------------------------------
// <copyright file="Updater.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.Updater.Models;

    using UnityEngine;

    using Utf8Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Automatically updates Exiled to the latest version.
    /// </summary>
    public sealed class Updater : Plugin<Config>
    {
#pragma warning disable SA1600 // Elements should be documented
        public const long REPOID = 231269519;
        public const string GitHubGetReleasesTemplate = "https://api.github.com/repositories/{0}/releases";

        public static readonly string[] InstallerAssetNamesLinux = { "Exiled.Installer-Linux", "Exiled.Installer" };
        public static readonly string[] InstallerAssetNamesWin = { "Exiled.Installer-Win.exe", "Exiled.Installer.exe" };
        public static readonly Encoding ProcessEncidong = new UTF8Encoding(false, false);

        private readonly HttpClient httpClient = new HttpClient();
#pragma warning restore SA1600 // Elements should be documented

        /// <inheritdoc />
        public override string Author => "Exiled Team @ iRebbok";

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"Exiled.Updater (https://github.com/galaxy119/EXILED, {Assembly.GetName().Version})");

            base.OnEnabled();

            var result = FindUpdate().ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.Item1)
                Update(result.Item2, result.Item3).ConfigureAwait(false).GetAwaiter().GetResult();

            httpClient.Dispose();
        }

        /// <summary>
        /// Starts the updater.
        /// </summary>
        /// <param name="asset">
        /// Installer asset.
        /// </param>
        /// <param name="allowPRE">
        /// Allow '--pre-release' argument passing.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Update(ReleaseAsset asset, bool allowPRE)
        {
            try
            {
                Log.Info("Downloading installer...");
                using (var installer = await httpClient.GetAsync(asset.BrowserDownloadUrl).ConfigureAwait(false))
                {
                    Log.Info("Downloaded!");

                    var serverPath = Environment.CurrentDirectory;
                    var installerPath = Path.Combine(serverPath, "Exiled.Installer-Win.exe");

                    using (var installerStream = await installer.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var fs = new FileStream(installerPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await installerStream.CopyToAsync(fs).ConfigureAwait(false);
                    }

                    if (!File.Exists(installerPath))
                    {
                        Log.Error("Couldn't find the downloaded installer!");
                    }

                    var startInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = serverPath,
                        FileName = installerPath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = allowPRE ? "--pre-releases --exit" : "--exit",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = ProcessEncidong,
                        StandardOutputEncoding = ProcessEncidong,
                    };

                    var installerProcess = Process.Start(startInfo);
                    installerProcess.OutputDataReceived += (s, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                            Log.Info($"[Installer] {args.Data}");
                    };
                    installerProcess.BeginOutputReadLine();
                    installerProcess.ErrorDataReceived += (s, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                            Log.Error($"[Installer] {args.Data}");
                    };
                    installerProcess.BeginErrorReadLine();

                    installerProcess.WaitForExit();

                    Log.Info($"Installer exit code: {installerProcess.ExitCode}");
                    Log.Info("Auto-update complete, restarting server...");

                    Application.Quit();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Update)} throw an exception");
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Check if there is an update available.
        /// </summary>
        /// <returns>Returns whether a new version of Exiled is available or not.</returns>
        public async Task<Tuple<bool, ReleaseAsset, bool>> FindUpdate()
        {
            try
            {
                var url = string.Format(GitHubGetReleasesTemplate, REPOID);
                using (var result = await httpClient.GetAsync(url).ConfigureAwait(false))
                {
                    using (var streamResult = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var releases = JsonSerializer.Deserialize<Release[]>(streamResult)
                            .Where(r => Version.TryParse(r.TagName, out _))
                            .OrderByDescending(r => r.CreatedAt.Ticks)
                            .ToArray();

                        Log.Debug($"Found {releases.Length} releases");
                        for (int z = 0; z < releases.Length; z++)
                        {
                            var release = releases[z];
                            Log.Debug($"PRE: {release.PreRelease} | ID: {release.Id} | TAG: {release.TagName}");

                            for (int x = 0; x < release.Assets.Length; x++)
                            {
                                var asset = release.Assets[x];
                                Log.Debug($"   - ID: {asset.Id} | NAME: {asset.Name} | SIZE: {asset.Size} | URL: {asset.BrowserDownloadUrl}");
                            }
                        }

                        if (releases.Length != 0 && FindRelease(releases, out var targetRelease, out var includedPRE))
                        {
                            var installerNames = GetAvailableInstallerNames();
                            if (!FindAsset(installerNames, targetRelease, out var asset))
                            {
                                // Error: no asset
                                Log.Warn("Couldn't found the asset, the update will not be installed");
                            }
                            else
                            {
                                return new Tuple<bool, ReleaseAsset, bool>(true, asset, includedPRE);
                            }
                        }
                        else
                        {
                            // No errors
                            Log.Info("No new versions found, you're using the most recent version of Exiled!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FindUpdate)} throw an exception:");
                Log.Error(ex);
            }

            return new Tuple<bool, ReleaseAsset, bool>(false, default, false);
        }

        private string[] GetAvailableInstallerNames()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return InstallerAssetNamesWin;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return InstallerAssetNamesLinux;
            }
            else
            {
                Log.Error("Can't determine your OS platform");
                Log.Error($"OSDesc: {RuntimeInformation.OSDescription}");
                Log.Error($"OSArch: {RuntimeInformation.OSArchitecture}");

                return null;
            }
        }

        private bool FindRelease(Release[] releases, out Release release, out bool includedPRE)
        {
            includedPRE = Config.ShouldDownloadTestingReleases || releases.Any(r => r.PreRelease && Version.Parse(r.TagName) == Version);
            for (int z = 0; z < releases.Length; z++)
            {
                release = releases[z];
#if DEBUG
                var version = Version.Parse(release.TagName);
                Log.Debug($"TV - {version} | CV - {Version} | TV >= CV - {CustomVersionGreaterOrEquals(version, Version)}");
#endif
                if (release.PreRelease && !includedPRE)
                    continue;

#if DEBUG
                if (CustomVersionGreaterOrEquals(version, Version))
#else
                if (CustomVersionGreater(Version.Parse(release.TagName), Version))
#endif
                    return true;
            }

            release = default;
            return false;
        }

        private bool FindAsset(string[] assetNames, Release release, out ReleaseAsset asset)
        {
            for (int z = 0; z < release.Assets.Length; z++)
            {
                asset = release.Assets[z];

                // Cannot use ref, out, or in parameter 'asset' inside an anonymous method, lambda expression, query expression, or local function
                var a = asset;
                if (assetNames.Any(an => an.Equals(a.Name, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            asset = default;
            return false;
        }

#if DEBUG
        private bool CustomVersionGreaterOrEquals(Version v1, Version v2)
        {
            return v1.Major >= v2.Major || v1.Minor >= v2.Minor || v1.Build >= v2.Build || v1.Revision >= v2.Revision;
        }
#endif

        private bool CustomVersionGreater(Version v1, Version v2)
        {
            return v1.Major > v2.Major || v1.Minor > v2.Minor || v1.Build > v2.Build || v1.Revision > v2.Revision;
        }
    }
}
