// -----------------------------------------------------------------------
// <copyright file="Updater.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Automatically updates Exiled to the latest version.
    /// </summary>
    public class Updater : Plugin<Config>
    {
        private string versionUpdateUrl;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            if (IsUpdateAvailable())
                Start();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
        }

        /// <inheritdoc/>
        public override void OnReloaded()
        {
        }

        /// <summary>
        /// Starts the updater.
        /// </summary>
        public void Start()
        {
            try
            {
                Log.Info("There is an new version of Exiled available.");

                Log.Info($"Attempting auto-update...");
                Log.Info($"URL: {versionUpdateUrl ?? "none"}");

                if (versionUpdateUrl == null)
                {
                    Log.Error("Version update was queued but not URL was set. This error should never happen.");
                    return;
                }

                string tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                Log.Info($"Creating temporary directory: {tempDirectory}...");

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                string exiledTempPath = Path.Combine(tempDirectory, "Exiled.tar.gz");

                using (WebClient client = new WebClient())
                    client.DownloadFile(versionUpdateUrl, exiledTempPath);

                Log.Info("Download successful, extracting contents...");
                ExtractTarGz(exiledTempPath, tempDirectory);
                Log.Info($"Extraction complete, moving files...");

                string tempExiledMainPath = Path.Combine(Path.Combine(tempDirectory, "Exiled"), "Exiled.Loader.dll");
                string tempPluginsDirectory = Path.Combine(tempExiledMainPath, "Plugins");
                string tempExiledEventsPath = Path.Combine(tempPluginsDirectory, "Exiled.Events.dll");
                string tempPermissionsPath = Path.Combine(tempPluginsDirectory, "Exiled.Permissions.dll");
                string tempUpdaterPath = Path.Combine(tempPluginsDirectory, "Exiled.Updater.dll");
                string tempAssemblyPath = Path.Combine(tempDirectory, "Assembly-CSharp.dll");

                File.Delete(Path.Combine(Paths.Exiled, "Exiled.Loader.dll"));
                File.Delete(Path.Combine(Paths.Plugins, "Exiled.Events.dll"));
                File.Delete(Path.Combine(Paths.Plugins, "Exiled.Permissions.dll"));
                File.Delete(Path.Combine(Paths.Plugins, "Exiled.Idler.dll"));
                File.Delete(Path.Combine(Paths.Plugins, "Exiled.Updater.dll"));
                File.Delete(Path.Combine(Paths.ManagedAssemblies, "Assembly-CSharp.dll"));
                File.Move(tempExiledMainPath, Path.Combine(Paths.Exiled, "Exiled.Loader.dll"));
                File.Move(tempExiledEventsPath, Path.Combine(Paths.Plugins, "Exiled.Events.dll"));
                File.Move(tempPermissionsPath, Path.Combine(Paths.Plugins, "Exiled.Permissions.dll"));
                File.Move(tempUpdaterPath, Path.Combine(Paths.Plugins, "Exiled.Updater.dll"));
                File.Move(tempAssemblyPath, Path.Combine(Paths.ManagedAssemblies, "Assembly-CSharp.dll"));
                Log.Info($"Files moved, cleaning up...");
                Directory.Delete(tempDirectory, true);

                Log.Info("Auto-update complete, restarting server...");

                Application.Quit();
            }
            catch (Exception exception)
            {
                Log.Error($"Auto-update error: {exception}");
            }
        }

        /// <summary>
        /// Check if there is an update available.
        /// </summary>
        /// <returns>Returns whether a new version of Exiled is available or not.</returns>
        public bool IsUpdateAvailable()
        {
            try
            {
                string url = "https://github.com/galaxy119/EXILED/releases/" + (Config.ShouldDownloadTestingReleases ? string.Empty : "latest/");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{url}");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                if (stream == null)
                    throw new InvalidOperationException("No response from Github. This shouldn't happen, contact the Exiled Team on Discord.");

                StreamReader reader = new StreamReader(stream);
                string read = reader.ReadToEnd();
                string[] readArray = read.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                string line = readArray.FirstOrDefault(str => str.Contains("Exiled.tar.gz"));

                if (string.IsNullOrEmpty(line))
                    return false;

                string version = Between(line, "/galaxy119/EXILED/releases/download/", "/Exiled.tar.gz");
                string[] versionArray = version.Split('.');

                if (!int.TryParse(versionArray[0], out int major))
                {
                    Log.Error($"Unable to parse Exiled major version.");
                    return false;
                }

                if (!int.TryParse(versionArray[1], out int minor))
                {
                    Log.Error($"Unable to parse Exiled minor version.");
                    return false;
                }

                if (!int.TryParse(versionArray[2], out int build))
                {
                    Log.Error($"Unable to parse Exiled patch version.");
                    return false;
                }

                versionUpdateUrl = $"{url.Replace("latest/", string.Empty)}download/{version}/EXILED.tar.gz";

                if (RequiredExiledVersion < new Version(major, minor, build))
                {
                    Log.Info($"Your version is outdated: Current {RequiredExiledVersion.Major}.{RequiredExiledVersion.Minor}.{RequiredExiledVersion.Build}, New: {major}.{minor}.{build}");
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Updating error: {exception}");
            }

            return false;
        }

        private string Between(string str, string firstString, string lastString)
        {
            int firstPosition = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;

            return str.Substring(firstPosition, str.IndexOf(lastString, StringComparison.Ordinal) - firstPosition);
        }

        private void ExtractTarGz(string filename, string outputDir)
        {
            using (FileStream stream = File.OpenRead(filename))
                ExtractTarGz(stream, outputDir);
        }

        private void ExtractTarGz(Stream stream, string outputDir)
        {
            using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                const int chunk = 4096;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int read;
                    byte[] buffer = new byte[chunk];

                    do
                    {
                        read = gzip.Read(buffer, 0, chunk);
                        memoryStream.Write(buffer, 0, read);
                    }
                    while (read == chunk);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    ExtractTar(memoryStream, outputDir);
                }
            }
        }

        private void ExtractTar(Stream stream, string outputDir)
        {
            byte[] buffer = new byte[100];

            while (true)
            {
                try
                {
                    stream.Read(buffer, 0, 100);

                    string name = Encoding.ASCII.GetString(buffer).Trim('\0');

                    if (string.IsNullOrWhiteSpace(name))
                        break;

                    stream.Seek(24, SeekOrigin.Current);
                    stream.Read(buffer, 0, 12);

                    long size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                    stream.Seek(376L, SeekOrigin.Current);

                    string output = Path.Combine(outputDir, name);
                    if (!Directory.Exists(Path.GetDirectoryName(output)))
                        Directory.CreateDirectory(Path.GetDirectoryName(output));

                    if (!name.Equals("./", StringComparison.InvariantCulture))
                    {
                        using (FileStream str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] buf = new byte[size];
                            stream.Read(buf, 0, buf.Length);
                            str.Write(buf, 0, buf.Length);
                        }
                    }

                    long position = stream.Position;
                    long offset = 512 - (position % 512);

                    if (offset == 512)
                        offset = 0;

                    stream.Seek(offset, SeekOrigin.Current);
                }
                catch (Exception exception)
                {
                    Log.Error($"An error has occurred while extracting Exiled files: {exception}");
                }
            }
        }
    }
}
