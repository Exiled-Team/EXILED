using System.Reflection;

namespace Exiled.Launcher.Features.Patcher;

public class Bootstrap
{
     public static bool IsLoaded { get; private set; }

     public static void Load()
     {
         if (IsLoaded)
         {
             ServerConsole.AddLog("[Exiled.Bootstrap] Exiled has already been loaded!", ConsoleColor.DarkRed);
             return;
         }

         try
         {
             ServerConsole.AddLog("[Exiled.Bootstrap] Exiled is loading...", ConsoleColor.DarkRed);

             string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED");

             if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                 rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");

             string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

             if (!Directory.Exists(rootPath))
                 Directory.CreateDirectory(rootPath);

             if (!File.Exists(Path.Combine(rootPath, "Exiled.Loader.dll")))
             {
                 ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.Loader.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                 return;
             }

             if (!File.Exists(Path.Combine(dependenciesPath, "Exiled.API.dll")))
             {
                 ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.API.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                 return;
             }

             if (!File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
             {
                 ServerConsole.AddLog($"[Exiled.Bootstrap] YamlDotNet.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                 return;
             }

             Assembly.Load(File.ReadAllBytes(Path.Combine(rootPath, "Exiled.Loader.dll")))
                 .GetType("Exiled.Loader.Loader")
                 ?.GetMethod("Run")
                 ?.Invoke(
                     null,
                     new object[]
                     {
                         new[]
                         {
                             Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "Exiled.API.dll"))),
                             Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "YamlDotNet.dll"))),
                         },
                     });

             IsLoaded = true;
         }
         catch (Exception exception)
         {
             ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled loading error: {exception}", ConsoleColor.DarkRed);
         }
     }
}
