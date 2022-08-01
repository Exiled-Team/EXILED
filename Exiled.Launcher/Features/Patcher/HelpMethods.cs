using dnlib.DotNet;

namespace Exiled.Launcher.Features.Patcher;

public class HelpMethods
{
    public static TypeDef? FindServerConsoleDefinition(ModuleDefMD assembly)
    {
        foreach (var type in assembly.Types)
        {
            if (type.FullName == "ServerConsole")
                return type;
        }

        return null;
    }

    public static MethodDef? FindStartMethodDefinition(TypeDef serverConsole)
    {
        foreach (var method in serverConsole.Methods)
        {
            if (method.Name == "Start")
                return method;
        }

        return null;
    }
}
