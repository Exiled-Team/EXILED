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
    using System.CommandLine.Parsing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented

    internal sealed class CommandSettings
    {
        public static readonly RootCommand RootCommand = new RootCommand
        {
            new Option<DirectoryInfo>(
                new[] { "-p", "--path" },
                parseArgument: (parsed) =>
                {
                    var path = parsed.Tokens.SingleOrDefault()?.Value ?? Directory.GetCurrentDirectory();

                    if (File.Exists(path))
                        parsed.ErrorMessage = "Can't be a file!";
                    else if (!Directory.Exists(path))
                        parsed.ErrorMessage = "Directory doesn't exist!";
                    else if (!Program.ValidateServerPath(path, out var targetFilePath))
                        parsed.ErrorMessage = $"Couldn't find '{Program.TARGET_FILE_NAME}' in '{targetFilePath}'";

                    return new DirectoryInfo(path); // return for default value
                },
                isDefault: true,
                description: "Path to the folder with the SL server")
            { IsRequired = true },

            new Option<DirectoryInfo>(
                "--appdata",
                getDefaultValue: () => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
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
                "--github--token",
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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public DirectoryInfo Path { get; set; }

        public DirectoryInfo AppData { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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
