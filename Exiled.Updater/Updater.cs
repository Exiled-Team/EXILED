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
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    using Exiled.API.Features;
    using Exiled.Updater.GHApi;
    using Exiled.Updater.GHApi.Models;
    using Exiled.Updater.GHApi.Settings;
    using Exiled.Updater.Models;

    using MEC;

    using UnityEngine;

#pragma warning disable SA1124 // Do not use regions
#pragma warning disable SA1300 // Element should begin with upper-case letter

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

        public static readonly string InstallerAssetNameLinux = "Exiled.Installer-Linux";
        public static readonly string InstallerAssetNameWin = "Exiled.Installer-Win.exe";
        public static readonly Encoding ProcessEncoding = new UTF8Encoding(false, false);
        public static readonly PlatformID PlatformId = Environment.OSVersion.Platform;

        private bool _firstLaunch = true;
        private volatile Stage _stage;

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
            // FixInvalidProxyHandling();
            if (_stage == Stage.Free)
            {
                Timing.RunCoroutine(_CheckUpdate(forced), Segment.EndOfFrame);
                return true;
            }

            return false;
        }

        /* "I don't think that will be necessary." -Zabszk
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
        }*/

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(480),
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Updater (https://github.com/galaxy119/EXILED, {Assembly.GetName().Version.ToString(3)})");

            return client;
        }

        private IEnumerator<float> _CheckUpdate(bool forced)
        {
            _stage = Stage.Start;

            Thread updateThread = new Thread(() =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    if (FindUpdate(client, forced, out NewVersion newVersion))
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

        #region Finders

        private bool FindUpdate(HttpClient client, bool forced, out NewVersion newVersion)
        {
            try
            {
                ExiledLibrary smallestVersion = FindSmallestExiledVersion();
                Log.Info($"Found the smallest version of Exiled - {smallestVersion.Library.GetName().Name}:{smallestVersion.Version}");

                // TODO: make it loop pages to find an update
                TaggedRelease[] releases = TagReleases(client.GetReleases(REPOID, new GetReleasesSettings(50, 1)).GetAwaiter().GetResult());

                if (FindRelease(releases, out Release targetRelease, smallestVersion, forced))
                {
                    if (!FindAsset(GetInstallerName(), targetRelease, out ReleaseAsset asset))
                    {
                        // Error: no asset
                        Log.Warn("Couldn't find the asset, the update will not be installed");
                    }
                    else
                    {
                        Log.Info($"Found asset - Name: {asset.Name} | Size: {asset.Size} Download: {asset.BrowserDownloadUrl}");
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
                Log.Error($"{nameof(FindUpdate)} threw an exception:\n{ex}");
            }

            newVersion = default;
            return false;
        }

        private bool FindRelease(TaggedRelease[] releases, out Release release, ExiledLibrary smallestVersion, bool forced = false)
        {
            bool includePRE = Config.ShouldDownloadTestingReleases || OneOfExiledIsPrerelease();

            for (int z = 0; z < releases.Length; z++)
            {
                TaggedRelease taggedRelease = releases[z];
                if (taggedRelease.Release.PreRelease && !includePRE)
                    continue;

                if ((taggedRelease.Version > smallestVersion.Version) || forced)
                {
                    release = taggedRelease.Release;
                    return true;
                }
            }

            release = default;
            return false;
        }

        private bool FindAsset(string assetName, Release release, out ReleaseAsset asset)
        {
            for (int z = 0; z < release.Assets.Length; z++)
            {
                asset = release.Assets[z];
                if (assetName.Equals(asset.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            asset = default;
            return false;
        }

        #endregion

        #region Utils

        private TaggedRelease[] TagReleases(Release[] releases)
        {
            TaggedRelease[] arr = new TaggedRelease[releases.Length];
            for (int z = 0; z < arr.Length; z++)
            {
                arr[z] = new TaggedRelease(releases[z]);
            }

            return arr;
        }

        private string GetInstallerName()
        {
            if (PlatformId == PlatformID.Win32NT)
            {
                return InstallerAssetNameWin;
            }
            else if (PlatformId == PlatformID.Unix)
            {
                return InstallerAssetNameLinux;
            }
            else
            {
                Log.Error("Can't determine your OS platform");
                Log.Error($"OSDesc: {RuntimeInformation.OSDescription}");
                Log.Error($"OSArch: {RuntimeInformation.OSArchitecture}");

                return null;
            }
        }

        private ExiledLibrary FindSmallestExiledVersion() => GetExiledLibs().Min();

        private bool OneOfExiledIsPrerelease() => GetExiledLibs().Any(l => l.Version.PreRelease != null);

        private IEnumerable<ExiledLibrary> GetExiledLibs()
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   let name = a.GetName().Name
                   where name.StartsWith("Exiled.", StringComparison.OrdinalIgnoreCase)
                   && !(Config.ExcludeAssemblies?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)
                   && name != Assembly.GetName().Name
                   select new ExiledLibrary(a);
        }

        #endregion

        private void Update(HttpClient client, NewVersion newVersion)
        {
            try
            {
                Log.Info("Downloading installer...");
                using (HttpResponseMessage installer = client.GetAsync(newVersion.Asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult())
                {
                    Log.Info("Downloaded!");

                    string serverPath = Environment.CurrentDirectory;
                    string installerPath = Path.Combine(serverPath, newVersion.Asset.Name);

                    if (File.Exists(installerPath) && PlatformId == PlatformID.Unix)
                        LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

                    using (Stream installerStream = installer.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                    using (FileStream fs = new FileStream(installerPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        installerStream.CopyToAsync(fs).ConfigureAwait(false).GetAwaiter().GetResult();
                    }

                    if (PlatformId == PlatformID.Unix)
                        LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

                    if (!File.Exists(installerPath))
                    {
                        Log.Error("Couldn't find the downloaded installer!");
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = serverPath,
                        FileName = installerPath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = $"--exit --target-version {newVersion.Release.TagName} --appdata \"{Paths.AppData}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = ProcessEncoding,
                        StandardOutputEncoding = ProcessEncoding,
                    };

                    Process installerProcess = Process.Start(startInfo);
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
    }
}
