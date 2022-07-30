namespace Exiled.Launcher.Features.Arguments;

public class ArgumentOptionAttribute : Attribute
{
    public string ShortName;
    public string Name;
    public string HelpText;
    public bool Required;

    public ArgumentOptionAttribute(string shortName, string name, string helpText, bool required)
    {
        ShortName = shortName;
        Name = name;
        HelpText = helpText;
        Required = required;
    }
}
