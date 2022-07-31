using Exiled.Launcher.Features.Arguments;

LauncherArguments arguments = ArgumentParser.GetArguments(args);

if (arguments.Help)
{
    ArgumentParser.ShowHelp();
    return;
}

// var f = new ProcessStartInfo(arguments.StartingPoint, arguments.ExternalArguments);
// Process.Start(f);
