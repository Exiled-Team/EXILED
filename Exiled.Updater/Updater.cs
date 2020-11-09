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

    using Exiled.API.Features;
    using Exiled.Updater.GHApi;
    using Exiled.Updater.GHApi.Models;
    using Exiled.Updater.Models;

    using MEC;

    using UnityEngine;

    public sealed class Updater : Plugin<Config>
    {
        private enum Stage
        {
            Free,
            Start,
            Installing,
            Installed,
        }

        public const long REPOID = 231269519;

        public static Updater Instance { get; } = new Updater();

        public static readonly string[] InstallerAssetNamesLinux = { "Exiled.Installer-Linux" };
        public static readonly string[] InstallerAssetNamesWin = { "Exiled.Installer-Win.exe" };
        public static readonly Encoding ProcessEncoding = new UTF8Encoding(false, false);
        public static readonly PlatformID PlatformId = Environment.OSVersion.Platform;

        private bool _firstLaunch = true;
        private volatile Stage _stage;

        /// <inheritdoc />
        public override string Author => "Exiled Team @ iRebbok";

        private Updater()
        {
        }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            if (!_firstLaunch)
            {
                Log.Info("Exiled won't be checked for an update because that's not the first launch of the plugin");
                return;
            }

            _firstLaunch = false;

            CheckUpdate(false);
        }

        public bool CheckUpdate(bool forced)
        {
            FixInvalidProxyHandling();

            if (_stage == Stage.Free)
            {
                Timing.RunCoroutine(_CheckUpdate(forced), Segment.EndOfFrame);
                return true;
            }

            return false;
        }

        private void FixInvalidProxyHandling()
        {
            // https://github.com/mono/mono/pull/12595
            if (PlatformId == PlatformID.Win32NT)
            {
                const string keyName = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

                var proxyEnabled = (int)Microsoft.Win32.Registry.GetValue(keyName, "ProxyEnable", 0);
                var strProxy = (string)Microsoft.Win32.Registry.GetValue(keyName, "ProxyServer", null);
                if (proxyEnabled > 0 && strProxy == null)
                {
                    Log.Info("HttpProxy detected, bypassing...");
                    Microsoft.Win32.Registry.SetValue(keyName, "ProxyEnable", 0);
                    Log.Info("Bypassed!");

                    GameCore.Console.HttpMode = HttpQueryMode.HttpClient;
                    GameCore.Console.LockHttpMode = false;
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(480),
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Updater (https://github.com/galaxy119/EXILED, {Assembly.GetName().Version.ToString(3)})");

            return client;
        }

#pragma warning disable SA1300 // Element should begin with upper-case letter
        private IEnumerator<float> _CheckUpdate(bool forced)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            _stage = Stage.Start;

            var updateThread = new Thread(() =>
            {
                using (var client = CreateHttpClient())
                {
                    if (FindUpdate(client, forced, out var newVersion))
                    {
                        _stage = Stage.Installing;
                        Update(client, newVersion);
                    }
                }
            })
            {
                IsBackground = false,
                Priority = System.Threading.ThreadPriority.AboveNormal,
            };

            updateThread.Start();

            while (updateThread.IsAlive)
            {
                if (_stage == Stage.Installing)
                {
                    updateThread.Join();
                }

                yield return 0f;
            }

            if (_stage == Stage.Installed)
                Application.Quit();

            _stage = Stage.Free;
        }

        private bool FindUpdate(HttpClient client, bool forced, out NewVersion newVersion)
        {
            try
            {
                var smallestExiledVersion = FindSmallestExiledVersion();

                var releases = client.GetReleases(REPOID).GetAwaiter().GetResult();
                #region Debug code
                Log.Debug($"Found {releases.Length} releases", Config.ShouldDebugBeShown);
                for (var z = 0; z < releases.Length; z++)
                {
                    var release = releases[z];
                    Log.Debug($"PRE: {release.PreRelease} | ID: {release.Id} | TAG: {release.TagName}", Config.ShouldDebugBeShown);

                    for (int x = 0; x < release.Assets.Length; x++)
                    {
                        var asset = release.Assets[x];
                        Log.Debug($"   - ID: {asset.Id} | NAME: {asset.Name} | SIZE: {asset.Size} | URL: {asset.Url} | DownloadURL: {asset.BrowserDownloadUrl}", Config.ShouldDebugBeShown);
                    }
                }
                #endregion

                var taggedReleases = TagReleases(releases);
                if (FindRelease(taggedReleases, out var targetRelease, smallestExiledVersion.GetName(), forced))
                {
                    if (!FindAsset(GetAvailableInstallerNames(), targetRelease, out var asset))
                    {
                        // Error: no asset
                        Log.Warn("Couldn't find the asset, the update will not be installed");
                    }
                    else
                    {
                        Log.Info($"Found asset - ID: {asset.Id} | NAME: {asset.Name} | SIZE: {asset.Size} | URL: {asset.Url} | DownloadURL: {asset.BrowserDownloadUrl}");
                        newVersion = new NewVersion(targetRelease, asset);
                        return true;
                    }
                }
                else
                {
                    // No errors
                    Log.Info("No new versions found, you're using the most recent version of Exiled!");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FindUpdate)} threw an exception:");
                Log.Error(ex);
            }

            newVersion = default;
            return false;
        }

        private TaggedRelease[] TagReleases(Release[] releases)
        {
            var arr = new TaggedRelease[releases.Length];
            for (var z = 0; z < arr.Length; z++)
            {
                arr[z] = new TaggedRelease(releases[z]);
            }

            return arr;
        }

        private void Update(HttpClient client, NewVersion newVersion)
        {
            try
            {
                Log.Info("Downloading installer...");
                using (var installer = client.GetAsync(newVersion.Asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult())
                {
                    Log.Info("Downloaded!");

                    var serverPath = Environment.CurrentDirectory;
                    var installerPath = Path.Combine(serverPath, newVersion.Asset.Name);

                    if (File.Exists(installerPath) && PlatformId == PlatformID.Unix)
                        LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

                    using (var installerStream = installer.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                    using (var fs = new FileStream(installerPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        installerStream.CopyToAsync(fs).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    if (PlatformId == PlatformID.Unix)
                        LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

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
                        Arguments = $"--exit --pre-releases {newVersion.Release.PreRelease} --target-version {newVersion.Release.TagName} --appdata \"{Paths.AppData}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = ProcessEncoding,
                        StandardOutputEncoding = ProcessEncoding,
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

                    _stage = Stage.Installed;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Update)} throw an exception");
                Log.Error(ex);
            }
        }

        private string[] GetAvailableInstallerNames()
        {
            if (PlatformId == PlatformID.Win32NT)
            {
                return InstallerAssetNamesWin;
            }
            else if (PlatformId == PlatformID.Unix)
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

        private bool FindRelease(TaggedRelease[] releases, out Release release, AssemblyName smallestExiledVersion, bool orEquals = false)
        {
            Log.Info($"Found the smallest version of Exiled - {smallestExiledVersion.FullName}");

            var includePRE = Config.ShouldDownloadTestingReleases || OneOfExiledIsPrerelease(releases);
            for (int z = 0; z < releases.Length; z++)
            {
                var taggedRelease = releases[z];
#if DEBUG
                Log.Debug($"TV - {taggedRelease.Version.Backwards} | CV - {smallestExiledVersion.Version} | TV >= CV - {VersionComparer.CustomVersionGreaterOrEquals(taggedRelease.Version.Backwards, smallestExiledVersion.Version)}", Config.ShouldDebugBeShown);
#endif
                if (taggedRelease.Release.PreRelease && !includePRE)
                    continue;

#if DEBUG
                if (VersionComparer.CustomVersionGreaterOrEquals(taggedRelease.Version.Backwards, smallestExiledVersion.Version))
#else
                if (!orEquals ?
                    VersionComparer.CustomVersionGreater(taggedRelease.Version.Backwards, smallestExiledVersion.Version)
                        :
                    VersionComparer.CustomVersionGreaterOrEquals(taggedRelease.Version.Backwards, smallestExiledVersion.Version))
#endif
                {
                    release = taggedRelease.Release;
                    return true;
                }
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

        private Assembly FindSmallestExiledVersion()
        {
            return GetExiledLibs().OrderBy(name => name.GetName().Version, VersionComparer.Instance).First();
        }

        private bool OneOfExiledIsPrerelease(TaggedRelease[] releases)
        {
            var libs = GetExiledLibs();
            return releases.Any(r => r.Release.PreRelease && libs.Any(lib => VersionComparer.CustomVersionEquals(r.Version.Backwards, lib.GetName().Version)));
        }

        private IEnumerable<Assembly> GetExiledLibs()
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   let name = a.GetName()
                   where name.Name.StartsWith("Exiled.", StringComparison.OrdinalIgnoreCase) &&
                   !(Config.ExcludeAssemblies?.Contains(name.Name, StringComparison.OrdinalIgnoreCase) ?? false)
                   select a;
        }
    }
}
