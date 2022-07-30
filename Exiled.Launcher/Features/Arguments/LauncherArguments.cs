namespace Exiled.Launcher.Features.Arguments;

public class LauncherArguments
{
    [ArgumentOption("-h", "--help", "The command to see all possible arguments.")]
    public bool Help { get; set; } = false;

    [ArgumentOption("-sp", "--starting-point", "The application that is ran just after exiled is updated and installed.")]
    public string StartingPoint { get; set; } = "LocalAdmin.exe";

    [ArgumentOption("-v", "--exiled-version", "The desired exiled version, accepts latest, a version number and none.")]
    public string ExiledVersion { get; set; } = "latest";

    [ArgumentOption("-pr", "--pre-releases", "Download pre releases, Yes/No (y/n).")]
    public bool DownloadPreReleases { get; set; } = true;

    [ArgumentOption("-ie", "--inject-exiled", "Should Exiled be injected? Yes/No (y/n).")]
    public bool InjectExiled { get; set; } = true;
}
