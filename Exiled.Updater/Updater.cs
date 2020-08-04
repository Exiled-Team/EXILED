// -----------------------------------------------------------------------
// <copyright file="Updater.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
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
        public static readonly uint SecondsWaitForAPI = 60;
        public static readonly uint SecondsWaitForDownload = 480;
        public static readonly PlatformID PlatformID = Environment.OSVersion.Platform;

        private readonly HttpClient httpClient = new HttpClient();
#pragma warning restore SA1600 // Elements should be documented

        /// <inheritdoc />
        public override string Author => "Exiled Team @ iRebbok";

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Log.Debug($"PlatformId: {PlatformID}");

            httpClient.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Updater (https://github.com/galaxy119/EXILED, {Assembly.GetName().Version})");

            base.OnEnabled();

            var result = FindUpdate().ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.Item1)
                Update(result.Item2, result.Item3).ConfigureAwait(false).GetAwaiter().GetResult();

            httpClient.Dispose();
        }

        /// <summary>
        /// Starts the updater.
        /// </summary>
        /// <param name="release">
        /// Release with installer.
        /// </param>
        /// <param name="asset">
        /// Installer asset.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Update(Release release, ReleaseAsset asset)
        {
            try
            {
                Log.Info("Downloading installer...");
                using (var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(SecondsWaitForDownload)))
                using (var installer = await httpClient.GetAsync(asset.BrowserDownloadUrl, cancellationToken.Token).ConfigureAwait(false))
                {
                    Log.Info("Downloaded!");

                    var serverPath = Environment.CurrentDirectory;
                    var installerPath = Path.Combine(serverPath, asset.Name);

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
                        Arguments = $"--exit --pre-releases {release.PreRelease} --target-release {release.TagName} --appdata \"{Paths.AppData}\"",
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
        public async Task<Tuple<bool, Release, ReleaseAsset>> FindUpdate()
        {
            try
            {
                var url = string.Format(GitHubGetReleasesTemplate, REPOID);

                using (var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(SecondsWaitForAPI)))
                using (var result = await httpClient.GetAsync(url, cancellationToken.Token).ConfigureAwait(false))
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
                            Log.Debug($"   - ID: {asset.Id} | NAME: {asset.Name} | SIZE: {asset.Size} | URL: {asset.Url} | DownloadURL: {asset.BrowserDownloadUrl}");
                        }
                    }

                    if (releases.Length != 0 && FindRelease(releases, out var targetRelease, out var includedPRE))
                    {
                        var installerNames = GetAvailableInstallerNames();
                        if (!FindAsset(installerNames, targetRelease, out var asset))
                        {
                            // Error: no asset
                            Log.Warn("Couldn't find the asset, the update will not be installed");
                        }
                        else
                        {
                            Log.Info($"Found asset - ID: {asset.Id} | NAME: {asset.Name} | SIZE: {asset.Size} | URL: {asset.Url} | DownloadURL: {asset.BrowserDownloadUrl}");
                            return new Tuple<bool, Release, ReleaseAsset>(true, targetRelease, asset);
                        }
                    }
                    else
                    {
                        // No errors
                        Log.Info("No new versions found, you're using the most recent version of Exiled!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FindUpdate)} throw an exception:");
                Log.Error(ex);
            }

            return new Tuple<bool, Release, ReleaseAsset>(false, default, default);
        }

        private string[] GetAvailableInstallerNames()
        {
            if (PlatformID == PlatformID.Win32NT)
            {
                Log.Debug("Using Win installer");
                return InstallerAssetNamesWin;
            }
            else if (PlatformID == PlatformID.Unix)
            {
                Log.Debug("Using Linux installer");
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
            var smallestExiledVersion = FindSmallestExiledVersion();
            if (smallestExiledVersion != null)
            {
                Log.Info($"Found the smallest version of Exiled - {smallestExiledVersion.FullName}");

                includedPRE = Config.ShouldDownloadTestingReleases || OneOfExiledIsPrerelease(releases);
                for (int z = 0; z < releases.Length; z++)
                {
                    release = releases[z];
#if DEBUG
                    var version = Version.Parse(release.TagName);
                    Log.Debug($"TV - {version} | CV - {smallestExiledVersion.Version} | TV >= CV - {VersionComparer.CustomVersionGreaterOrEquals(version, smallestExiledVersion.Version)}");
#endif
                    if (release.PreRelease && !includedPRE)
                        continue;

#if DEBUG
                    if (VersionComparer.CustomVersionGreaterOrEquals(version, smallestExiledVersion.Version))
#else
                    if (VersionComparer.CustomVersionGreater(Version.Parse(release.TagName), smallestExiledVersion.Version))
#endif
                        return true;
                }
            }
            else
            {
                Log.Error("Couldn't find smallest version of Exiled!");
                includedPRE = false;
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

        private AssemblyName FindSmallestExiledVersion()
        {
            return GetExiledLibs().OrderBy(name => name.Version, VersionComparer.Instance).FirstOrDefault();
        }

        private bool OneOfExiledIsPrerelease(Release[] releases)
        {
            var libs = GetExiledLibs();
            return releases.Any(r => r.PreRelease && libs.Any(lib => VersionComparer.CustomVersionEquals(Version.Parse(r.TagName), lib.Version)));
        }

        private IEnumerable<AssemblyName> GetExiledLibs()
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   let name = a.GetName()
                   where name.Name.StartsWith("Exiled", StringComparison.OrdinalIgnoreCase) &&
                   !(Config.ExcludeAssemblies?.Contains(name.Name, StringComparison.OrdinalIgnoreCase) ?? false)
                   select name;
        }
    }
}
