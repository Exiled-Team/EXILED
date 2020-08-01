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

    internal enum PathResolution
    {
        UNDEFINED,
        /// <summary>
        ///     Absolute path that is routed to AppData.
        /// </summary>
        ABSOLUTE,
        /// <summary>
        ///     The path that goes through the path of the game passing through the subfolders.
        /// </summary>
        GAME
    }

    internal static class Program
    {
        private const long REPO_ID = 231269519;
        private const string EXILED_ASSET_NAME = "exiled.tar.gz";
        private const string TARGET_FILE_NAME = "Assembly-CSharp.dll";

        private static readonly string ExiledTargetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string[] TargetSubfolders = { "SCPSL_Data", "Managed" };
        private static readonly Version VersionLimit = new Version(2, 0, 0);

        private static readonly GitHubClient GitHubClient = new GitHubClient(
            new ProductHeaderValue($"{Assembly.GetExecutingAssembly().GetName().Name}-{Assembly.GetExecutingAssembly().GetName().Version}"));

        // Force use of LF because the file uses LF
        private static readonly Dictionary<string, string> Markup = Resources.Markup.Trim().Split('\n').ToDictionary(s => s.Split(':')[0], s => s.Split(':', 2)[1]);

        private static async Task Main(string[] args)
        {
            await CommandSettings.Parse(args).ConfigureAwait(false);
        }

        internal static async Task MainSafe(CommandSettings args)
        {
            try
            {
                Console.WriteLine($"Exiled.Installer@{Assembly.GetExecutingAssembly().GetName().Version}");

                if (args.GetVersions)
                {
                    var releases1 = await GetReleases().ConfigureAwait(false);
                    Console.WriteLine("--- AVAILABLE VERSIONS ---");
                    foreach (var r in releases1)
                        Console.WriteLine(FormatRelease(r, true));

                    return;
                }

                if (!ProcessTargetFilePath(args.Path, out var targetFilePath))
                {
                    Console.WriteLine($"Couldn't find '{TARGET_FILE_NAME}' in '{targetFilePath}'");
                    throw new FileNotFoundException("Requires --path argument with the path to the game, read readme or invoke with --help");
                }

                EnsureDirExists(ExiledTargetPath);

                Console.WriteLine("Receiving releases...");
                Console.WriteLine($"Prereleases included - {args.PreReleases}");

                var releases = await GetReleases().ConfigureAwait(false);

                Console.WriteLine("Searching for the latest release that matches the parameters...");

                var latestRelease = releases.FirstOrDefault(r =>
                {
                    if (!(args.TargetVersion is null))
                        return r.TagName.Equals(args.TargetVersion, StringComparison.Ordinal);

                    return (r.Prerelease && args.PreReleases) || !r.Prerelease;
                });

                if (latestRelease is null)
                {
                    Console.WriteLine("--- RELEASES ---");
                    Console.WriteLine(string.Join(Environment.NewLine, releases.Select(FormatRelease)));
                    throw new InvalidOperationException("Couldn't find release");
                }

                Console.WriteLine("Release found!");
                Console.WriteLine(FormatRelease(latestRelease));

                var exiledAsset = latestRelease.Assets.FirstOrDefault(a => a.Name.Equals(EXILED_ASSET_NAME, StringComparison.OrdinalIgnoreCase));
                if (exiledAsset is null)
                {
                    Console.WriteLine("--- ASSETS ---");
                    Console.WriteLine(string.Join(Environment.NewLine, latestRelease.Assets.Select(FormatAsset)));
                    throw new InvalidOperationException("Couldn't find asset");
                }

                Console.WriteLine("Asset found!");
                Console.WriteLine(FormatAsset(exiledAsset));

                var downloadUrl = exiledAsset.BrowserDownloadUrl;
                using var httpClient = new HttpClient();
                var downloadResult = await httpClient.GetAsync(downloadUrl).ConfigureAwait(false);
                var downloadArchiveStream = await downloadResult.Content.ReadAsStreamAsync().ConfigureAwait(false);

                using var gzInputStream = new GZipInputStream(downloadArchiveStream);
                using var tarInputStream = new TarInputStream(gzInputStream);

                TarEntry entry;
                while (!((entry = tarInputStream.GetNextEntry()) is null))
                {
                    entry.Name = entry.Name.Replace('/', Path.DirectorySeparatorChar);
                    ProcessTarEntry(targetFilePath, tarInputStream, entry);
                }

                Console.WriteLine("Installation complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Read the exception message, read the readme, and if you still don't understand what to do, then contact #support in our discord server with the attached screenshot of the full exception");
                Console.Read();
            }
        }

        private static async Task<IEnumerable<Release>> GetReleases() =>
            (await GitHubClient.Repository.Release.GetAll(REPO_ID).ConfigureAwait(false))
                .Where(r => Version.TryParse(r.TagName, out var version) && version >= VersionLimit)
                .OrderByDescending(r => r.CreatedAt.Ticks);

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

            return builder.ToString();
        }

        private static string FormatAsset(ReleaseAsset a)
        {
            return $"ID: {a.Id} | NAME: {a.Name} | SIZE: {a.Size} | URL: {a.Url}";
        }

        private static void ResolvePath(string filePath, out string path)
        {
            path = Path.Combine(ExiledTargetPath, filePath);
        }

        private static void ProcessTarEntry(string targetFilePath, TarInputStream tarInputStream, TarEntry entry)
        {
            if (entry.IsDirectory)
            {
                var entries = entry.GetDirectoryEntries();
                for (int z = 0; z < entries.Length; z++)
                {
                    ProcessTarEntry(targetFilePath, tarInputStream, entries[z]);
                }
            }
            else
            {
                Console.WriteLine($"Processing '{entry.Name}'");

                if (entry.Name.Contains("example", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Extract for {entry.Name} is disabled");
                    return;
                }

                switch (ResolveEntry(entry))
                {
                    case PathResolution.ABSOLUTE:
                        ResolvePath(entry.Name, out var path);
                        ExtractEntry(tarInputStream, entry, path);
                        break;
                    case PathResolution.GAME:
                        ExtractEntry(tarInputStream, entry, targetFilePath);
                        break;
                    default:
                        Console.WriteLine($"Couldn't resolve path for '{entry.Name}', update installer");
                        break;
                }
            }
        }

        private static void ExtractEntry(TarInputStream tarInputStream, TarEntry entry, string path)
        {
            Console.WriteLine($"Extracting '{Path.GetFileName(entry.Name)}' into '{path}'...");

            EnsureDirExists(Path.GetDirectoryName(path)!);

            FileStream? fs = null;
            try
            {
                fs = new FileStream(path, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
                tarInputStream.CopyEntryContents(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while trying to extract a file");
                Console.WriteLine(ex);
            }
            finally
            {
                fs?.Dispose();
            }
        }

        private static bool ProcessTargetFilePath(string? argTargetPath, out string path)
        {
            var linkedSubfolders = string.Join(Path.DirectorySeparatorChar, TargetSubfolders);

            // can be null if couldn't be found
            if (!string.IsNullOrEmpty(argTargetPath))
            {
                var combined = Path.Combine(argTargetPath, linkedSubfolders, TARGET_FILE_NAME);
                if (File.Exists(combined))
                {
                    path = combined;
                    return true;
                }
            }

            var curDir = Directory.GetCurrentDirectory();
            var combined2 = Path.Combine(curDir, linkedSubfolders, TARGET_FILE_NAME);
            if (File.Exists(combined2))
            {
                path = combined2;
                return true;
            }

            path = string.Empty;
            return false;
        }

        private static void EnsureDirExists(string pathToDir)
        {
#if DEBUG
            Console.WriteLine($"Ensuring directory path: {pathToDir}");
            Console.WriteLine($"Does it exist? - {Directory.Exists(pathToDir)}");
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

            return PathResolution.UNDEFINED;
        }
    }
}
