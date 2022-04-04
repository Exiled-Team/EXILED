// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Installer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.Installer.Properties;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;

    using Octokit;

    using Version = SemVer.Version;

    internal enum PathResolution
    {
        Undefined,
        /// <summary>
        ///     Absolute path that is routed to AppData.
        /// </summary>
        Absolute,
        /// <summary>
        ///     The path that goes through the path of the game passing through the subfolders.
        /// </summary>
        Game
    }

    internal static class Program
    {
        private const long RepoID = 231269519;
        private const string ExiledAssetName = "exiled.tar.gz";
        internal const string TargetFileName = "Assembly-CSharp.dll";

        private static readonly string[] TargetSubfolders = { "SCPSL_Data", "Managed" };
        private static readonly string LinkedSubfolders = string.Join(Path.DirectorySeparatorChar.ToString(), TargetSubfolders);
        private static readonly Version VersionLimit = new Version("2.0.0");
        private static readonly uint SecondsWaitForDownload = 480;

        private static readonly string Header = $"{Assembly.GetExecutingAssembly().GetName().Name}-{Assembly.GetExecutingAssembly().GetName().Version}";
        private static readonly GitHubClient GitHubClient = new GitHubClient(
            new ProductHeaderValue(Header));

        // Force use of LF because the file uses LF
        private static readonly Dictionary<string, string> Markup = Resources.Markup.Trim().Split('\n').ToDictionary(s => s.Split(':')[0], s => s.Split(':', 2)[1]);

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false, false);
            await CommandSettings.Parse(args).ConfigureAwait(false);
        }

        internal static async Task MainSafe(CommandSettings args)
        {
            try
            {
                Console.WriteLine(Header);

                if (args.GetVersions)
                {
                    var releases1 = await GetReleases().ConfigureAwait(false);
                    Console.WriteLine(Resources.Program_MainSafe_____AVAILABLE_VERSIONS____);
                    foreach (var r in releases1)
                        Console.WriteLine(FormatRelease(r, true));

                    if (args.Exit)
                        Environment.Exit(0);
                }

                Console.WriteLine(Resources.Program_MainSafe_AppData_folder___0_, args.AppData.FullName);

                if (!ValidateServerPath(args.Path.FullName, out var targetFilePath))
                {
                    Console.WriteLine(Resources.Program_MainSafe_Couldn_t_find___0___in___1__, TargetFileName, targetFilePath);
                    throw new FileNotFoundException("Check the validation of the path parameter");
                }

                if (!(args.GitHubToken is null))
                {
                    Console.WriteLine(Resources.Program_MainSafe_Token_detected__Using_the_token___);
                    GitHubClient.Credentials = new Credentials(args.GitHubToken, AuthenticationType.Bearer);
                }

                Console.WriteLine(Resources.Program_MainSafe_Receiving_releases___);
                Console.WriteLine(Resources.Program_MainSafe_Prereleases_included____0_, args.PreReleases);
                Console.WriteLine(Resources.Program_MainSafe_Target_release_version____0_, (string.IsNullOrEmpty(args.TargetVersion) ? "(null)" : args.TargetVersion));

                var releases = await GetReleases().ConfigureAwait(false);
                Console.WriteLine(Resources.Program_MainSafe_Searching_for_the_latest_release_that_matches_the_parameters___);

                if (!TryFindRelease(args, releases, out var targetRelease))
                {
                    Console.WriteLine(Resources.Program_MainSafe_____RELEASES____);
                    Console.WriteLine(string.Join(Environment.NewLine, releases.Select(FormatRelease)));
                    throw new InvalidOperationException("Couldn't find release");
                }

                Console.WriteLine(Resources.Program_MainSafe_Release_found_);
                Console.WriteLine(FormatRelease(targetRelease!));

                var exiledAsset = targetRelease!.Assets.FirstOrDefault(a => a.Name.Equals(ExiledAssetName, StringComparison.OrdinalIgnoreCase));
                if (exiledAsset is null)
                {
                    Console.WriteLine(Resources.Program_MainSafe_____ASSETS____);
                    Console.WriteLine(string.Join(Environment.NewLine, targetRelease.Assets.Select(FormatAsset)));
                    throw new InvalidOperationException("Couldn't find asset");
                }

                Console.WriteLine(Resources.Program_MainSafe_Asset_found_);
                Console.WriteLine(FormatAsset(exiledAsset));

                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(SecondsWaitForDownload)
                };
                httpClient.DefaultRequestHeaders.Add("User-Agent", Header);

                using var downloadResult = await httpClient.GetAsync(exiledAsset.BrowserDownloadUrl).ConfigureAwait(false);
                using var downloadArchiveStream = await downloadResult.Content.ReadAsStreamAsync().ConfigureAwait(false);

                using var gzInputStream = new GZipInputStream(downloadArchiveStream);
                using var tarInputStream = new TarInputStream(gzInputStream, null);

                TarEntry entry;
                while (!((entry = tarInputStream.GetNextEntry()) is null))
                {
                    entry.Name = entry.Name.Replace('/', Path.DirectorySeparatorChar);
                    ProcessTarEntry(args, targetFilePath, tarInputStream, entry);
                }

                Console.WriteLine(Resources.Program_MainSafe_Installation_complete);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(Resources.Program_MainSafe_Read_the_exception_message__read_the_readme__and_if_you_still_don_t_understand_what_to_do__then_contact__support_in_our_discord_server_with_the_attached_screenshot_of_the_full_exception);
                if (!args.Exit)
                    Console.Read();
            }

            if (args.Exit)
                Environment.Exit(0);
        }

        private static async Task<IEnumerable<Release>> GetReleases()
        {
            var releases = (await GitHubClient.Repository.Release.GetAll(RepoID).ConfigureAwait(false))
                .Where(r => Version.TryParse(r.TagName, out Version version)
                    && (version > VersionLimit));

            return releases.OrderByDescending(r => r.CreatedAt.Ticks);
        }

        private static string FormatRelease(Release r)
            => FormatRelease(r, false);

        private static string FormatRelease(Release r, bool includeAssets)
        {
            var builder = new StringBuilder(30);
            builder.AppendLine($"PRE: {r.Prerelease} | ID: {r.Id} | TAG: {r.TagName}");
            if (includeAssets)
            {
                foreach (var asset in r.Assets)
                    builder.Append("   - ").AppendLine(FormatAsset(asset));
            }

            return builder.ToString().Trim('\r', '\n');
        }

        private static string FormatAsset(ReleaseAsset a)
        {
            return $"ID: {a.Id} | NAME: {a.Name} | SIZE: {a.Size} | URL: {a.Url} | DownloadURL: {a.BrowserDownloadUrl}";
        }

        private static void ResolvePath(CommandSettings args, string filePath, out string path)
        {
            path = Path.Combine(args.AppData.FullName, filePath);
        }

        private static void ProcessTarEntry(CommandSettings args, string targetFilePath, TarInputStream tarInputStream, TarEntry entry)
        {
            if (entry.IsDirectory)
            {
                var entries = entry.GetDirectoryEntries();
                for (int z = 0; z < entries.Length; z++)
                {
                    ProcessTarEntry(args, targetFilePath, tarInputStream, entries[z]);
                }
            }
            else
            {
                Console.WriteLine(Resources.Program_ProcessTarEntry_Processing___0__, entry.Name);

                if (entry.Name.Contains("example", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(Resources.Program_ProcessTarEntry_Extract_for__0__is_disabled, entry.Name);
                    return;
                }

                switch (ResolveEntry(entry))
                {
                    case PathResolution.Absolute:
                        ResolvePath(args, entry.Name, out var path);
                        ExtractEntry(tarInputStream, entry, path);
                        break;
                    case PathResolution.Game:
                        ExtractEntry(tarInputStream, entry, targetFilePath);
                        break;
                    default:
                        Console.WriteLine(Resources.Program_ProcessTarEntry_Couldn_t_resolve_path_for___0____update_installer, entry.Name);
                        break;
                }
            }
        }

        private static void ExtractEntry(TarInputStream tarInputStream, TarEntry entry, string path)
        {
            Console.WriteLine(Resources.Program_ExtractEntry_Extracting___0___into___1_____, Path.GetFileName(entry.Name), path);

            EnsureDirExists(Path.GetDirectoryName(path)!);

            FileStream? fs = null;
            try
            {
                fs = new FileStream(path, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
                tarInputStream.CopyEntryContents(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.Program_ExtractEntry_An_exception_occurred_while_trying_to_extract_a_file);
                Console.WriteLine(ex);
            }
            finally
            {
                fs?.Dispose();
            }
        }

        internal static bool ValidateServerPath(string serverPath, out string targetFilePath)
        {
            targetFilePath = Path.Combine(serverPath, LinkedSubfolders, TargetFileName);
            return File.Exists(targetFilePath);
        }

        private static void EnsureDirExists(string pathToDir)
        {
#if DEBUG
            Console.WriteLine(Resources.Program_EnsureDirExists_Ensuring_directory_path___0_, pathToDir);
            Console.WriteLine(Resources.Program_EnsureDirExists_Does_it_exist_____0_, Directory.Exists(pathToDir));
#endif
            if (!Directory.Exists(pathToDir))
                Directory.CreateDirectory(pathToDir);
        }

        private static PathResolution ResolveEntry(TarEntry entry)
        {
            static PathResolution TryParse(string s)
            {
                // We'll get UNDEFINED if it cannot be determined
                Enum.TryParse<PathResolution>(s, true, out var result);
                return result;
            }

            var fileName = entry.Name;
            var fileInFolder = !string.IsNullOrEmpty(Path.GetDirectoryName(fileName));
            foreach (var pair in Markup)
            {
                var isFolder = pair.Key.EndsWith('\\');
                if (fileInFolder && isFolder &&
                    pair.Key[0..^1].Equals(fileName.Substring(0, fileName.IndexOf(Path.DirectorySeparatorChar)), StringComparison.OrdinalIgnoreCase))
                {
                    return TryParse(pair.Value);
                }
                else if (!fileInFolder && !isFolder &&
                    pair.Key.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    return TryParse(pair.Value);
                }
            }

            return PathResolution.Undefined;
        }

        private static bool TryFindRelease(CommandSettings args, IEnumerable<Release> releases, out Release? release)
        {
            Console.WriteLine(Resources.Program_TryFindRelease_Trying_to_find_release__);
            Version targetVersion = args.TargetVersion is not null ? new Version(args.TargetVersion) : VersionLimit;

            foreach (var r in releases)
            {
                release = r;

                if ((targetVersion == new Version(r.TagName)))
                    return true;

                if ((r.Prerelease && args.PreReleases) || !r.Prerelease)
                    return true;
            }

            release = null;
            return false;
        }
    }
}
