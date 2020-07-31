// -----------------------------------------------------------------------
// <copyright file="CommandSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Installer
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.Parsing;
    using System.Threading.Tasks;

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented

    internal sealed class CommandSettings
    {
        public static readonly RootCommand RootCommand = new RootCommand
        {
            new Option<string?>(
                new[] { "-p", "--path" },
                description: "Path to the folder with the SL server")
            { IsRequired = false, },

            new Option<bool>(
                "--pre-releases",
                getDefaultValue: () => false,
                description: "Includes pre-releases")
            { IsRequired = false, },

            new Option<string?>(
                "--target-version",
                description: "Target version for installation")
            { IsRequired = false },

            new Option<bool>(
                "--get-versions",
                getDefaultValue: () => false,
                description: "Gets all possible versions for installation")
            { IsRequired = false },
        };

        public string? Path { get; set; }

        public bool PreReleases { get; set; }

        public string? TargetVersion { get; set; }

        public bool GetVersions { get; set; }

        public static async Task Parse(string[] args)
        {
            RootCommand.Handler = CommandHandler.Create<CommandSettings>(async args => await Program.MainSafe(args).ConfigureAwait(false));
            RootCommand.TreatUnmatchedTokensAsErrors = false;

            await RootCommand.InvokeAsync(args).ConfigureAwait(false);
        }
    }
}
