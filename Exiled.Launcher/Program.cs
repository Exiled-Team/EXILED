using Exiled.Launcher.Features.Arguments;

LauncherArguments arguments = ArgumentParser.GetArguments(args);

if (arguments.Help)
{
    ArgumentParser.ShowHelp();
    return;
}
