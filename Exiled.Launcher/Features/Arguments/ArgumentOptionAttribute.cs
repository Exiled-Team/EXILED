namespace Exiled.Launcher.Features.Arguments;

public class ArgumentOptionAttribute : Attribute
{
    public string ShortName;
    public string Name;
    public string HelpText;

    public ArgumentOptionAttribute(string shortName, string name, string helpText)
    {
        ShortName = shortName;
        Name = name;
        HelpText = helpText;
    }
}
