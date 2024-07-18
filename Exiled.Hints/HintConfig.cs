using Exiled.API.Interfaces;

namespace Exiled.Hints;

public class HintConfig : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }
}