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
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.Updater.GHApi;
    using Exiled.Updater.GHApi.Models;
    using Exiled.Updater.GHApi.Settings;
    using Exiled.Updater.Models;

    using MEC;

    using UnityEngine;

#pragma warning disable SA1124 // Do not use regions

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

        public static Updater Instance { get; } = new();

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
                Timing.RunCoroutine(CheckUpdateInternal(forced), Segment.EndOfFrame);
                return true;
            }

            return false;
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new()
            {
                Timeout = TimeSpan.FromSeconds(480),
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Updater (https://github.com/Exiled-Team/EXILED, {Assembly.GetName().Version.ToString(3)})");

            return client;
        }

        private IEnumerator<float> CheckUpdateInternal(bool forced)
        {
            _stage = Stage.Start;

            TaskCompletionSource<bool> updateCompleted = new();
            TaskCompletionSource<(bool, NewVersion)> findUpdateCompleted = new();

            Thread updateThread =
                new(() =>
                {
                    using HttpClient client = CreateHttpClient();
                    FindUpdate(client, forced).ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            Log.Error($"{nameof(Update)} threw an exception");
                            Log.Error(task.Exception);
                        }

                        findUpdateCompleted.SetResult(task.Result);
                    });

                    if (findUpdateCompleted.Task.Result.Item1)
                    {
                        _stage = Stage.Installing;
                        Update(client, findUpdateCompleted.Task.Result.Item2).ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                Log.Error($"{nameof(Update)} threw an exception");
                                Log.Error(task.Exception);
                            }

                            updateCompleted.SetResult(true);
                        });
                    }
                    else
                    {
                        updateCompleted.SetResult(false);
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

            updateThread.Abort();
            if (_stage == Stage.Installed)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, "Exiled scheduled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                ServerConsole.AddOutputEntry(default(ServerOutput.ExitActionRestartEntry));
            }

            _stage = Stage.Free;
        }

        #region Finders

        private async Task<(bool, NewVersion)> FindUpdate(HttpClient client, bool forced)
        {
            try
            {
                ExiledLibrary smallestVersion = FindSmallestExiledVersion();
                Log.Info($"Found the smallest version of Exiled - {smallestVersion.Library.GetName().Name}:{smallestVersion.Version}");

                // TODO: make it loop pages to find an update
                Release[] releases = await client.GetReleases(REPOID, new GetReleasesSettings(50, 1));

                TaggedRelease[] taggedReleases = releases.Select(r => new TaggedRelease(r)).ToArray();

                if (FindRelease(taggedReleases, out Release targetRelease, smallestVersion, forced))
                {
                    if (!FindAsset(GetInstallerName(), targetRelease, out ReleaseAsset asset))
                    {
                        // Error: no asset
                        Log.Warn("Couldn't find the asset, the update will not be installed");
                    }
                    else
                    {
                        Log.Info($"Found asset - Name: {asset.Name} | Size: {asset.Size} Download: {asset.BrowserDownloadUrl}");
                        return (true, new NewVersion(targetRelease, asset));
                    }
                }
                else
                {
                    // No errors
                    Log.Info("No new versions found, you're using the most recent version of Exiled!");
                }
            }
            catch (Utf8Json.JsonParsingException)
            {
                Log.Error("Encountered GitHub ratelimit, unable to check and download the latest version of Exiled.");
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FindUpdate)} threw an exception:\n{ex}");
            }

            return (false, default);
        }

        private bool FindRelease(TaggedRelease[] releases, out Release release, ExiledLibrary smallestVersion, bool forced = false)
        {
            bool includePRE = Config.ShouldDownloadTestingReleases || OneOfExiledIsPrerelease();

            for (int z = 0; z < releases.Length; z++)
            {
                TaggedRelease taggedRelease = releases[z];
                if (taggedRelease.Release.PreRelease && !includePRE)
                    continue;

                if (taggedRelease.Version > smallestVersion.Version || forced)
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

        private bool OneOfExiledIsPrerelease() => GetExiledLibs().Any(l => l.Version.PreRelease is not null);

        private IEnumerable<ExiledLibrary> GetExiledLibs() => from a in AppDomain.CurrentDomain.GetAssemblies()
                                                              let name = a.GetName().Name
                                                              where name.StartsWith("Exiled.", StringComparison.OrdinalIgnoreCase)
                                                                    && !(Config.ExcludeAssemblies?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)
                                                                    && (name != Assembly.GetName().Name)
                                                              select new ExiledLibrary(a);

        #endregion

        private async Task Update(HttpClient client, NewVersion newVersion)
        {
            try
            {
                Log.Info("Downloading installer...");
                HttpResponseMessage installerResponse = client.GetAsync(newVersion.Asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult();
                Log.Info("Downloaded!");

                string serverPath = Environment.CurrentDirectory;
                string installerPath = Path.Combine(serverPath, newVersion.Asset.Name);

                if (File.Exists(installerPath) && PlatformId == PlatformID.Unix)
                {
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);
                }

                using (Stream installerStream = await installerResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (FileStream fs = new(installerPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await installerStream.CopyToAsync(fs).ConfigureAwait(false);
                }

                if (PlatformId == PlatformID.Unix)
                {
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);
                }

                if (!File.Exists(installerPath))
                {
                    Log.Error("Couldn't find the downloaded installer!");
                    return;
                }

                ProcessStartInfo startInfo = new()
                {
                    WorkingDirectory = serverPath,
                    FileName = installerPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"--exit --target-version {newVersion.Release.TagName} --appdata \"{Paths.AppData}\" --exiled \"{Path.Combine(Paths.Exiled, "..")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardErrorEncoding = ProcessEncoding,
                    StandardOutputEncoding = ProcessEncoding,
                };

                using Process installerProcess = new()
                {
                    StartInfo = startInfo,
                };

                installerProcess.OutputDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.Info($"[Installer] {args.Data}");
                };
                installerProcess.ErrorDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.Error($"[Installer] {args.Data}");
                };

                installerProcess.Start();
                installerProcess.BeginOutputReadLine();
                installerProcess.BeginErrorReadLine();
                installerProcess.WaitForExit();

                Log.Info($"Installer exit code: {installerProcess.ExitCode}");

                if (installerProcess.ExitCode == 0)
                {
                    Log.Info("Auto-update complete, restarting server next round...");
                    _stage = Stage.Installed;
                }
                else
                {
                    Log.Error($"Installer error occurred.");
                    _stage = Stage.Free;
                }
            }
            catch (TaskCanceledException taskfailed)
            {
                Log.Error($"{nameof(Update)} threw an exception");
                Log.Error(taskfailed);
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Update)} threw an exception");
                Log.Error(ex);
            }
        }
    }
}