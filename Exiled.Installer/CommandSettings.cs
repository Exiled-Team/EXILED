// -----------------------------------------------------------------------
// <copyright file="CommandSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Installer
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal sealed class CommandSettings
    {
        public static readonly RootCommand RootCommand = new()
        {
            new Option<DirectoryInfo?>(
                new[] { "-p", "--path" },
                (parsed) =>
                {
                    string path = parsed.Tokens.SingleOrDefault()?.Value ?? Directory.GetCurrentDirectory();

                    if (string.IsNullOrEmpty(path))
                    {
                        parsed.ErrorMessage = "--path is null or empty";
                        return null;
                    }

                    if (File.Exists(path))
                        parsed.ErrorMessage = "Can't be a file!";
                    else if (!Directory.Exists(path))
                        parsed.ErrorMessage = "Directory doesn't exist!";

                    return new DirectoryInfo(path); // return for default value
                },
                true,
                "Path to the folder with the SL server")
                { IsRequired = true },

            new Option<DirectoryInfo?>(
                "--appdata",
                (parsed) =>
                {
                    string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    if (string.IsNullOrEmpty(appdataPath))
                        Console.Error.WriteLine("AppData path is null, make sure it exists");

                    string path = parsed.Tokens.SingleOrDefault()?.Value ?? appdataPath;

                    if (string.IsNullOrEmpty(path))
                    {
                        parsed.ErrorMessage = "--AppData is null or empty, make sure the AppData folder exists";
                        return null;
                    }

                    return new DirectoryInfo(path);
                },
                true,
                "Forces the folder to be the AppData folder (useful for containers when pterodactyl runs as root)")
                { IsRequired = true },

             new Option<DirectoryInfo?>(
                "--exiled",
                (parsed) =>
                {
                    string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    if (string.IsNullOrEmpty(appdataPath))
                        Console.Error.WriteLine("Your Exiled path is null, make sure it exists");

                    string path = parsed.Tokens.SingleOrDefault()?.Value ?? appdataPath;

                    if (string.IsNullOrEmpty(path))
                    {
                        parsed.ErrorMessage = "--exiled is null or empty, make sure your Exiled folder exists";
                        return null;
                    }

                    return new DirectoryInfo(path);
                },
                true,
                "Indicates the root path of Exiled")
                { IsRequired = true },

            new Option<bool>(
                "--pre-releases",
                () => false,
                "Includes pre-releases")
                { IsRequired = false },

            new Option<string?>(
                "--target-port",
                "Target port for ExiledLoader installation")
                { IsRequired = false },

            new Option<string?>(
                "--target-version",
                "Target version for installation")
                { IsRequired = false },

            new Option<string?>(
                "--github-token",
                "Uses a token for auth in case the rate limit is exceeded (no permissions required)")
                { IsRequired = false },

            new Option<bool>(
                "--exit",
                "Automatically exits the application anyway")
                { IsRequired = false },

            new Option<bool>(
                "--get-versions",
                "Gets all possible versions for installation")
                { IsRequired = false },
        };

#nullable disable
        public DirectoryInfo Path { get; set; }

        public DirectoryInfo AppData { get; set; }

        public DirectoryInfo Exiled { get; set; }
#nullable restore

        public bool PreReleases { get; set; }
        public string? TargetPort { get; set; }

        public string? TargetVersion { get; set; }

        public string? GitHubToken { get; set; }

        public bool GetVersions { get; set; }

        public bool Exit { get; set; }

        public async static Task Parse(string[] args)
        {
            RootCommand.Handler = CommandHandler.Create<CommandSettings>(async args => await Program.MainSafe(args).ConfigureAwait(false));
            RootCommand.TreatUnmatchedTokensAsErrors = false;

            await RootCommand.InvokeAsync(args).ConfigureAwait(false);
        }
    }
}