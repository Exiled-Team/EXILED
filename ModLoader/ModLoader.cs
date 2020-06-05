using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Loader
{
  public class ModLoader
  {
    public static bool Loaded;

    public static byte[] ReadFile(string path)
    {
      FileStream fileStream = File.Open(path, FileMode.Open);
      byte[] array;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        fileStream.CopyTo(memoryStream);
        array = memoryStream.ToArray();
      }
      fileStream.Close();
      return array;
    }

    public static void LoadBoi()
    {
      Loadxd();
    }

    public static void Loadxd()
    {
      if (Loaded)
        return;
      ServerConsole.AddLog("Hello, yes, EXILED is loading..");
      try
      {
        Loaded = true;
        string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-PTB");
        if (Environment.CurrentDirectory.ToLower().Contains("testing"))
          str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        if (File.Exists(Path.Combine(str, "Exiled.API.dll")))
          Assembly.LoadFile(Path.Combine(str, "Exiled.API.dll"));
        if (!File.Exists(Path.Combine(str, "Exiled.Loader.dll")))
          return;
        byte[] rawAssembly = ReadFile(Path.Combine(str, "Exiled.Loader.dll"));
        try
        {
          MethodInfo methodInfo = Assembly.Load(rawAssembly).GetTypes().SelectMany(p => (IEnumerable<MethodInfo>) p.GetMethods()).FirstOrDefault(f => f.Name == "EntryPointForLoader");
          if (!(methodInfo != null))
            return;
          methodInfo.Invoke(null, null);
        }
        catch (Exception ex)
        {
          ServerConsole.AddLog($"EXILED load error: {(object) ex}");
        }
      }
      catch (Exception ex)
      {
        ServerConsole.AddLog(ex.ToString());
      }
    }
  }
}