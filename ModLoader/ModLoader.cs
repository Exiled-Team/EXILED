// -----------------------------------------------------------------------
// <copyright file="ModLoader.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The assembly loader class for EXILED.
    /// </summary>
    public class ModLoader
    {
      /// <summary>
        /// Gets or sets a value indicating whether if EXILED has already loaded or not.
        /// </summary>
        public static bool Loaded { get; set; }

        /// <summary>
        /// Reads a file and returns the memory stream as a <see cref="byte"/> array.
        /// </summary>
        /// <param name="path">Path to the file to read.</param>
        /// <returns>A byte array of the file contents.</returns>
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

        /// <summary>
        /// Loads EXILED.
        /// </summary>
        public static void LoadBoi()
        {
          Loadxd();
        }

        /// <summary>
        /// Internally called loading method.
        /// </summary>
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
              MethodInfo methodInfo = Assembly.Load(rawAssembly).GetTypes().SelectMany(p => (IEnumerable<MethodInfo>)p.GetMethods()).FirstOrDefault(f => f.Name == "EntryPointForLoader");
              if (!(methodInfo != null))
                return;
              methodInfo.Invoke(null, null);
            }
            catch (Exception ex)
            {
              ServerConsole.AddLog($"EXILED load error: {(object)ex}");
            }
          }
          catch (Exception ex)
          {
            ServerConsole.AddLog(ex.ToString());
          }
        }
  }
}