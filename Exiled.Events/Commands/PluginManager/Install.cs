// -----------------------------------------------------------------------
// <copyright file="Install.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The command to install plugin.
    /// </summary>
    public class Install : ICommand
    {
        /// <summary>
        /// Gets the <see cref="Install"/> class instance.
        /// </summary>
        public static Install Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "install";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "download", "inst", "dwnl" };

        /// <inheritdoc/>
        public string Description { get; } = "Installs a plugin";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string perm = "pm.install";

            if (!sender.CheckPermission(perm) && sender is PlayerCommandSender playerSender && !playerSender.FullPermissions)
            {
                response = $"You can't install a plugin, you don't have \"{perm}\" permissions.";
                return false;
            }

            if (arguments.Count is < 1 or > 4)
            {
                response = "Please, use: pluginmanager install [PluginAuthor/PluginName] {File Name} {String version} {Dependencies string version}";
                return false;
            }

            string fileName = arguments.At(0).Split('/')[1];

            if (arguments.Count > 1)
            {
                fileName = arguments.At(1);
            }

            string targetRelease = "latest";

            if (arguments.Count > 2)
            {
                targetRelease = $"tag/{arguments.At(2)}";
            }

            string downloadPath = $"https://github.com/{arguments.At(0)}/releases/{targetRelease}/download/{fileName}.dll";

            try
            {
                using WebClient client = new();
                client.DownloadFile(downloadPath, Path.Combine(Paths.Plugins, $"{fileName}.dll"));
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse httpResponse)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        response = $"Returned error 404: Path not found - https://github.com/{arguments.At(0)}/releases/{targetRelease} or file was not found - {fileName}.dll";
                        return false;
                    }

                    response = $"Returned error {(int)httpResponse.StatusCode}: {webException.Message}";
                    return false;
                }
            }

            if (arguments.Count > 3)
            {
                targetRelease = $"tag/{arguments.At(3)}";
            }

            string dependenciesPath = $"https://github.com/{arguments.At(0)}/releases/{targetRelease}/download/dependencies.zip";

            try
            {
                using HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Events-{Events.Instance.Version}");

                using HttpResponseMessage downloadResult = httpClient.GetAsync(dependenciesPath).Result;
                using Task<Stream> downloadArchiveStream = downloadResult.Content.ReadAsStreamAsync();

                using ZipArchive archive = new(downloadArchiveStream.GetAwaiter().GetResult());

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    FileStream fileStream = File.Create(Path.Combine(Paths.Dependencies, entry.Name));
                    entry.Open().CopyTo(fileStream);
                    fileStream.Close();
                }
            }
            catch (InvalidDataException)
            {
                response = $"Successfully installed {fileName}.dll without dependencies!";
                return true;
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse httpResponse && httpResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    response = $"Returned error {(int)httpResponse.StatusCode}: {webException.Message}";
                    return false;
                }

                response = $"Successfully installed {fileName}.dll without dependencies!\nDependencies path - {dependenciesPath}";
                return true;
            }
            catch (Exception exception)
            {
                response = exception.ToString();
                return false;
            }

            response = $"Successfully installed {fileName}.dll and it's dependencies!";
            return true;
        }
    }
}