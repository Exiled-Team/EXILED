namespace Exiled.Launcher.Features.Arguments;

public class LauncherArguments
{
    [ArgumentOption("-sp", "--starting-point", "The application that is ran just after exiled is updated and installed.", false)]
    public string StartingPoint { get; set; } = "LocalAdmin.exe";

    [ArgumentOption("-v", "--exiled-version", "The desired exiled version, -1 to skip the exiled installation", false)]
    public string ExiledVersion { get; set; } = "latest";

    [ArgumentOption("-ie", "--inject-exiled", "Should Exiled be injected? Yes/No (y/n).", false)]
    public bool InjectExiled { get; set; } = true;
}
