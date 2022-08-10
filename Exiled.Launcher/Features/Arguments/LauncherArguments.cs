// -----------------------------------------------------------------------
// <copyright file="LauncherArguments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Launcher.Features.Arguments;

public class LauncherArguments
{
    [ArgumentOption("-h", "--help", "The command to see all possible arguments.")]
    public bool Help { get; set; }

    [ArgumentOption("-sp", "--starting-point", "The application that is ran just after exiled is updated and installed.")]
#if LINUX
    public string StartingPoint { get; set; } = "LocalAdmin";
#else
    public string StartingPoint { get; set; } = "LocalAdmin.exe";
#endif

    [ArgumentOption("-sf", "--server-folder", "The data folder of the server. (SCPSL_Data)")]
    public string ServerFolder { get; set; } = "SCPSL_Data";

    [ArgumentOption("-v", "--exiled-version", "The desired exiled version, (latest/none/ReleaseTag/ReleaseId).")]
    public string ExiledVersion { get; set; } = "latest";

    [ArgumentOption("-pr", "--pre-releases", "Download pre releases, Yes/No (y/n).")]
    public bool DownloadPreReleases { get; set; } = true;

    [ArgumentOption("-gt", "--github-token", "The github token to avoid rate limits.")]
    public string GithubToken { get; set; } = string.Empty;

    [ArgumentOption("-ie", "--inject-exiled", "Should Exiled be injected? Yes/No (y/n).")]
    public bool InjectExiled { get; set; } = true;

    [ArgumentOption("-ea", "--external-arguments", "The arguments to pass to the starting point.")]
    public List<string> ExternalArguments { get; } = new();
}
