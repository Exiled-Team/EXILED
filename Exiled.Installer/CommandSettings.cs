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
                parseArgument: (parsed) =>
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
                    else if (!Program.ValidateServerPath(path, out string? targetFilePath))
                        parsed.ErrorMessage = $"Couldn't find '{Program.TargetFileName}' in '{targetFilePath}'";

                    return new(path); // return for default value
                },
                isDefault: true,
                description: "Path to the folder with the SL server")
            { IsRequired = true },

            new Option<DirectoryInfo?>(
                "--appdata",
                parseArgument: (parsed) =>
                {
                    string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (string.IsNullOrEmpty(appdataPath))
                    {
                        Console.Error.WriteLine("AppData path is missing, make sure it exists");
                    }

                    string path = parsed.Tokens.SingleOrDefault()?.Value ?? appdataPath;
                    if (string.IsNullOrEmpty(path))
                    {
                        parsed.ErrorMessage = "--AppData is null or empty, make sure the AppData folder exists";
                        return null;
                    }

                    return new(path);
                },
                isDefault: true,
                description: "Forces the folder to be the AppData folder (useful for containers when pterodactyl runs as root)")
            { IsRequired = true },

            new Option<bool>(
                "--pre-releases",
                getDefaultValue: () => false,
                description: "Includes pre-releases")
            { IsRequired = false, },

            new Option<string?>(
                "--target-version",
                description: "Target version for installation")
            { IsRequired = false },

            new Option<string?>(
                "--github-token",
                description: "Uses a token for auth in case the rate limit is exceeded (no permissions required)")
            { IsRequired = false },

            new Option<bool>(
                "--exit",
                description: "Automatically exits the application anyway")
            { IsRequired = false },

            new Option<bool>(
                "--get-versions",
                description: "Gets all possible versions for installation")
            { IsRequired = false }
        };

#nullable disable
        public DirectoryInfo Path { get; set; }

        public DirectoryInfo AppData { get; set; }
#nullable restore

        public bool PreReleases { get; set; }

        public string? TargetVersion { get; set; }

        public string? GitHubToken { get; set; }

        public bool GetVersions { get; set; }

        public bool Exit { get; set; }

        public static async Task Parse(string[] args)
        {
            RootCommand.Handler = CommandHandler.Create<CommandSettings>(async args => await Program.MainSafe(args).ConfigureAwait(false));
            RootCommand.TreatUnmatchedTokensAsErrors = false;

            await RootCommand.InvokeAsync(args).ConfigureAwait(false);
        }
    }
}
