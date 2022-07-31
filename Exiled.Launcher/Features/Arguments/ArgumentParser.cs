using System.Reflection;

namespace Exiled.Launcher.Features.Arguments;

public static class ArgumentParser
{
    public static LauncherArguments GetArguments(string[] args)
    {
        LauncherArguments launcherArguments = new LauncherArguments();

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg is "-h" or "--help")
            {
                launcherArguments.Help = true;
                return launcherArguments;
            }

            if (arg is "-ea" or "--external-arguments")
            {
                for (int j = i + 1; j < args.Length; j++)
                    launcherArguments.ExternalArguments.Add(args[j]);

                return launcherArguments;
            }

            foreach (var possibleArgument in typeof(LauncherArguments).GetProperties())
            {
                Attribute? attribute = possibleArgument.GetCustomAttribute(typeof(ArgumentOptionAttribute));

                if (attribute is null || attribute is not ArgumentOptionAttribute optionAttribute)
                    continue;

                if (arg != optionAttribute.Name && arg != optionAttribute.ShortName)
                    continue;

                i++;
                arg = args[i];

                if (possibleArgument.PropertyType == typeof(bool))
                {
                    possibleArgument.SetValue(launcherArguments, arg.ToLower() is "yes" or "y");
                    break;
                }

                possibleArgument.SetValue(launcherArguments, arg);
                break;
            }
        }

        return launcherArguments;
    }

    public static void ShowHelp()
    {
        Console.WriteLine("Command".FormatForHelp(45) + "Info");
        foreach (var possibleArgument in typeof(LauncherArguments).GetProperties())
        {
            Attribute? attribute = possibleArgument.GetCustomAttribute(typeof(ArgumentOptionAttribute));

            if (attribute is null || attribute is not ArgumentOptionAttribute optionAttribute)
                continue;

            Console.WriteLine(optionAttribute.ShortName.FormatForHelp(5) + optionAttribute.Name.FormatForHelp(40) + optionAttribute.HelpText);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static string FormatForHelp(this string text, int count)
    {
        int missing = count - text.Length;
        for (int i = 0; i < missing; i++)
            text +=  " ";

        return text;
    }
}
