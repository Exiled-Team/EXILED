using Mono.Cecil;

namespace Exiled.Launcher.Features.Patcher;

public class CustomAssemblyResolver : BaseAssemblyResolver
{
    private readonly DefaultAssemblyResolver _defaultResolver;
    private readonly string _baseDirectory;

    public CustomAssemblyResolver(string path)
    {
        _defaultResolver = new DefaultAssemblyResolver();
        _baseDirectory = path;
    }

    public override AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        string oldDirectory = Directory.GetCurrentDirectory();

        if (!string.IsNullOrEmpty(_baseDirectory))
            Directory.SetCurrentDirectory(_baseDirectory);

        var assembly = _defaultResolver.Resolve(name);

        Directory.SetCurrentDirectory(oldDirectory);

        return assembly;
    }
}
