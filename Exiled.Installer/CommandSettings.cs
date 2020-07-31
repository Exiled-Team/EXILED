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
                () => null,
                "Path to the folder with the SL server")
                { IsRequired = false, },

            new Option<bool>(
                new[] { "--pre-releases" },
                () => false,
                "Includes pre-releases")
                { IsRequired = false, },
        };

        public readonly string? Path;
        public readonly bool IncludePreReleases;

        public CommandSettings(string? path, bool prereleases)
        {
            Path = path;
            IncludePreReleases = prereleases;
        }

        public static async Task Parse(string[] args)
        {
            RootCommand.Handler = CommandHandler.Create<CommandSettings>(async args => await Program.MainSafe(args));

            await RootCommand.InvokeAsync(args).ConfigureAwait(false);
        }
    }
}
