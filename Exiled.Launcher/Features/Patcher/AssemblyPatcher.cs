// -----------------------------------------------------------------------
// <copyright file="AssemblyPatcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Mono.Cecil;
using Mono.Cecil.Cil;

using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace Exiled.Launcher.Features.Patcher;

public static class AssemblyPatcher
{
    public static void Patch(string path)
    {
        string assemblyPath = Path.Combine(path, "Assembly-CSharp.dll");

        DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(path);

        using ModuleDefinition assembly = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters()
        {
            AssemblyResolver = resolver,
            ReadWrite = true
        });

        if (assembly is null)
        {
            Console.WriteLine($"[Patcher] Assembly in {assemblyPath} not found. Could not patch.");
            return;
        }

        if (HelpMethods.IsPatched(assembly))
        {
            Console.WriteLine("[Patcher] Assembly already patched. Skyping patching.");
            return;
        }

        Console.WriteLine("[Patcher] Finding ServerConsole::Start() method.");

        TypeDefinition? serverConsoleClass = HelpMethods.GetType(assembly, "ServerConsole");

        if (serverConsoleClass is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole.");
            return;
        }

        MethodDefinition? serverConsoleStartMethod = serverConsoleClass.GetMethod("Start");

        if (serverConsoleStartMethod is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole::Start()");
            return;
        }

        Console.WriteLine("[Patcher] Hooking Exiled.Bootstrap to ServerConsole::Start()");

        TypeReference boolRef = assembly.ImportReference(typeof(bool));
        TypeReference voidRef = assembly.ImportReference(typeof(void));
        TypeReference stringRef = assembly.ImportReference(typeof(string));

        FieldDefinition isLoadedField = new FieldDefinition("isExiledLoaded", FieldAttributes.Private | FieldAttributes.Static, boolRef);
        serverConsoleClass.Fields.Add(isLoadedField);

        MethodDefinition loadMethod = new MethodDefinition("LoadExiled",
            MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot,
            voidRef);
        serverConsoleClass.Methods.Add(loadMethod);

        using ModuleDefinition mscorlib = ModuleDefinition.ReadModule(Path.Combine(path, "mscorlib.dll"));
        using ModuleDefinition nwLib = ModuleDefinition.ReadModule(Path.Combine(path, "NorthwoodLib.dll"));

        MethodReference getFolderPath = assembly.ImportReference(mscorlib.GetType("System", "Environment").GetMethod("GetFolderPath"));
        MethodReference getCurDir = assembly.ImportReference(mscorlib.GetType("System", "Environment").GetMethod("get_CurrentDirectory"));

        MethodReference pathCombine2 = assembly.ImportReference(mscorlib.GetType("System.IO", "Path").GetMethod("Combine", 2));
        MethodReference pathCombine3 = assembly.ImportReference(mscorlib.GetType("System.IO", "Path").GetMethod("Combine", 3));

        MethodReference fileExists = assembly.ImportReference(mscorlib.GetType("System.IO", "File").GetMethod("Exists", 1));
        MethodReference fileReadAllBytes = assembly.ImportReference(mscorlib.GetType("System.IO", "File").GetMethod("ReadAllBytes", 1));

        MethodReference directoryExists = assembly.ImportReference(mscorlib.GetType("System.IO", "Directory").GetMethod("Exists", 1));
        MethodReference directoryCreate = assembly.ImportReference(mscorlib.GetType("System.IO", "Directory").GetMethod("CreateDirectory", 1));

        TypeReference objectRef = assembly.ImportReference(mscorlib.GetType("System", "Object"));
        TypeReference assemblyRef = assembly.ImportReference(mscorlib.GetType("System.Reflection", "Assembly"));

        MethodReference assemblyLoad = assembly.ImportReference(mscorlib.GetType("System.Reflection", "Assembly").Methods.Last(x => x.Name == "Load" && x.Parameters.Count == 1));

        MethodReference assemblyGetType = assembly.ImportReference(mscorlib.GetType("System.Reflection", "Assembly").GetMethod("GetType", 1));
        MethodReference assemblyInvoke = assembly.ImportReference(mscorlib.GetType("System.Reflection", "MethodBase").GetMethod("Invoke", 2));

        MethodReference getMethod = assembly.ImportReference(mscorlib.GetType("System", "Type").GetMethod("GetMethod", 1));

        MethodReference stringContain = assembly.ImportReference(nwLib.GetType("NorthwoodLib", "StringUtils").GetMethod("Contains", 3));

        mscorlib.Dispose();
        nwLib.Dispose();

        MethodDefinition? addLog = serverConsoleClass.GetMethod("AddLog");

        MethodBody loadBody = loadMethod.Body;
        ILProcessor generator = loadBody.GetILProcessor();

        loadBody.Variables.Add(new VariableDefinition(stringRef)); // rootPath
        loadBody.Variables.Add(new VariableDefinition(stringRef)); // dependenciesPath

        Instruction loadBreakpoint = Instruction.Create(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled is loading...");
        Instruction testingBreakpoint = Instruction.Create(OpCodes.Ldloc_0);
        Instruction dependenciesBreakpoint = Instruction.Create(OpCodes.Ldloc_0);
        Instruction loaderExistsBreakpoint = Instruction.Create(OpCodes.Ldloc_1);
        Instruction apiExistsBreakpoint = Instruction.Create(OpCodes.Ldloc_1);
        Instruction yamlExistsBreakpoint = Instruction.Create(OpCodes.Ldloc_0);
        Instruction getMethodLabel = Instruction.Create(OpCodes.Ldnull);
        Instruction finalMethodLabel = Instruction.Create(OpCodes.Ldc_I4_1);

        Instruction retLabel = Instruction.Create(OpCodes.Ret);

        // If isLoaded
        generator.Emit(OpCodes.Ldsfld, isLoadedField);
        generator.Emit(OpCodes.Brfalse_S, loadBreakpoint);

        // AddLog
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled has already been loaded!");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, addLog);
        generator.Emit(OpCodes.Br, retLabel);

        // AddLog
        generator.Append(loadBreakpoint);
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, addLog);

        // Path.Combine
        generator.Emit(OpCodes.Ldc_I4, 26);
        generator.Emit(OpCodes.Call, getFolderPath);
        generator.Emit(OpCodes.Ldstr, "EXILED");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Stloc_0);

        // rootPath testing?
        generator.Emit(OpCodes.Call, getCurDir);
        generator.Emit(OpCodes.Ldstr, "testing");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, stringContain);
        generator.Emit(OpCodes.Brfalse_S, testingBreakpoint);
        generator.Emit(OpCodes.Ldc_I4, 26);
        generator.Emit(OpCodes.Call, getFolderPath);
        generator.Emit(OpCodes.Ldstr, "EXILED-Testing");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Stloc_0);
        generator.Append(testingBreakpoint);

        // dependenciesPath
        generator.Emit(OpCodes.Ldstr, "Plugins");
        generator.Emit(OpCodes.Ldstr, "dependencies");
        generator.Emit(OpCodes.Call, pathCombine3);
        generator.Emit(OpCodes.Stloc_1);
        generator.Emit(OpCodes.Ldloc_0);

        // Create root directory if not exists
        generator.Emit(OpCodes.Call, directoryExists);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ceq);
        generator.Emit(OpCodes.Brfalse_S, dependenciesBreakpoint);
        generator.Emit(OpCodes.Ldloc_0);
        generator.Emit(OpCodes.Call, directoryCreate);
        generator.Emit(OpCodes.Pop);

        // Check if exiled.loader.dll exists
        generator.Append(dependenciesBreakpoint); // ldloc_0 (rootPath)
        generator.Emit(OpCodes.Ldstr, "Exiled.Loader.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileExists);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ceq);
        generator.Emit(OpCodes.Brfalse_S, loaderExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled.Loader.dll was not found, Exiled won't be loaded!");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, addLog);
        generator.Emit(OpCodes.Br, retLabel);

        // Exiled.API.dll Exists
        generator.Append(loaderExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "Exiled.API.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileExists);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ceq);
        generator.Emit(OpCodes.Brfalse_S, apiExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled.API.dll was not found, Exiled won't be loaded!");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, addLog);
        generator.Emit(OpCodes.Br, retLabel);

        // YamlDotNet exists
        generator.Append(apiExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "YamlDotNet.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileExists);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ceq);
        generator.Emit(OpCodes.Brfalse_S, yamlExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] YamlDotNet.dll was not found, Exiled won't be loaded!");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, addLog);
        generator.Emit(OpCodes.Br, retLabel);

        // Exiled.Loader.Loader.Run([])
        generator.Append(yamlExistsBreakpoint);
        generator.Emit(OpCodes.Ldstr, "Exiled.Loader.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileReadAllBytes);
        generator.Emit(OpCodes.Call, assemblyLoad);
        generator.Emit(OpCodes.Ldstr, "Exiled.Loader.Loader");
        generator.Emit(OpCodes.Callvirt, assemblyGetType);
        generator.Emit(OpCodes.Ldstr, "Run");
        generator.Emit(OpCodes.Callvirt, getMethod);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Brtrue_S, getMethodLabel);
        generator.Emit(OpCodes.Pop);
        generator.Emit(OpCodes.Br_S, finalMethodLabel);
        generator.Append(getMethodLabel);
        generator.Emit(OpCodes.Ldc_I4_1);
        generator.Emit(OpCodes.Newarr, objectRef);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ldc_I4_2);
        generator.Emit(OpCodes.Newarr, assemblyRef);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ldloc_1);
        generator.Emit(OpCodes.Ldstr, "Exiled.API.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileReadAllBytes);
        generator.Emit(OpCodes.Call, assemblyLoad);
        generator.Emit(OpCodes.Stelem_Ref);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Ldc_I4_1);
        generator.Emit(OpCodes.Ldloc_1);
        generator.Emit(OpCodes.Ldstr, "YamlDotNet.dll");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Call, fileReadAllBytes);
        generator.Emit(OpCodes.Call, assemblyLoad);
        generator.Emit(OpCodes.Stelem_Ref);
        generator.Emit(OpCodes.Stelem_Ref);
        generator.Emit(OpCodes.Call, assemblyInvoke);
        generator.Emit(OpCodes.Pop);
        generator.Append(finalMethodLabel);
        generator.Emit(OpCodes.Stsfld, isLoadedField);

        generator.Append(retLabel);

        serverConsoleStartMethod.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, loadMethod));

        assembly.Write();
    }
}
