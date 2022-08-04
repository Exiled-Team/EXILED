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

        using ModuleDefinition assembly = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters()
        {
            AssemblyResolver = new CustomAssemblyResolver(path)
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

        TypeDefinition bootstrap = new TypeDefinition("Exiled", "Bootstrap", TypeAttributes.Public | TypeAttributes.Class);
        assembly.Types.Add(bootstrap);

        TypeReference boolRef = assembly.ImportReference(typeof(bool));
        TypeReference voidRef = assembly.ImportReference(typeof(void));

        FieldDefinition isLoadedField = new FieldDefinition("isLoaded", FieldAttributes.Private | FieldAttributes.Static, boolRef);
        bootstrap.Fields.Add(isLoadedField);

        MethodDefinition loadMethod = new MethodDefinition("Load",
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot,
            voidRef);
        bootstrap.Methods.Add(loadMethod);

        ModuleDefinition mscorlib = ModuleDefinition.ReadModule(Path.Combine(path, "mscorlib.dll"));
        ModuleDefinition nwLib = ModuleDefinition.ReadModule(Path.Combine(path, "NorthwoodLib.dll"));

        MethodReference getFolderPath = assembly.ImportReference(mscorlib.GetType("System", "Environment").GetMethod("GetFolderPath"));
        MethodReference pathCombine2 = assembly.ImportReference(mscorlib.GetType("System.IO", "Path").GetMethod("Combine", 2));
        MethodReference pathCombine3 = assembly.ImportReference(mscorlib.GetType("System.IO", "Path").GetMethod("Combine", 3));
        MethodReference directoryExists = assembly.ImportReference(mscorlib.GetType("System.IO", "Directory").GetMethod("Exists", 1));
        MethodReference getCurDir = assembly.ImportReference(mscorlib.GetType("System", "Environment").GetMethod("get_CurrentDirectory"));
        MethodReference stringContain = assembly.ImportReference(nwLib.GetType("NorthwoodLib", "StringUtils").GetMethod("Contains", 3));

        MethodDefinition? addLog = serverConsoleClass.GetMethod("AddLog");

        MethodBody loadBody = loadMethod.Body;
        ILProcessor generator = loadBody.GetILProcessor();

        Instruction loadBreakpoint = Instruction.Create(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled is loading...");
        Instruction testingBreakpoint = Instruction.Create(OpCodes.Ldloc_1);

        Instruction retLabel = Instruction.Create(OpCodes.Ret);

        // If isLoaded
        generator.Emit(OpCodes.Ldsfld, isLoadedField);
        generator.Emit(OpCodes.Brfalse_S, loadBreakpoint);

        // AddLog
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled has already been loaded!");
        generator.Emit(OpCodes.Ldc_I4_4);
        generator.Emit(OpCodes.Call, addLog);
        generator.Emit(OpCodes.Br, retLabel); // try with br

        // AddLog
        generator.Append(loadBreakpoint);
        generator.Emit(OpCodes.Ldc_I4_4);
        generator.Emit(OpCodes.Call, addLog);

        // Path.Combine
        generator.Emit(OpCodes.Ldc_I4, 26);
        generator.Emit(OpCodes.Call, getFolderPath);
        generator.Emit(OpCodes.Ldstr, "EXILED");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Stloc_1);

        generator.Emit(OpCodes.Call, getCurDir);
        generator.Emit(OpCodes.Ldstr, "testing");
        generator.Emit(OpCodes.Ldc_I4_5);
        generator.Emit(OpCodes.Call, stringContain);
        generator.Emit(OpCodes.Brfalse_S, testingBreakpoint);
        generator.Emit(OpCodes.Ldc_I4, 26);
        generator.Emit(OpCodes.Call, getFolderPath);
        generator.Emit(OpCodes.Ldstr, "EXILED-Testing");
        generator.Emit(OpCodes.Call, pathCombine2);
        generator.Emit(OpCodes.Stloc_1);

        generator.Emit(OpCodes.Ldstr, "Plugins");
        generator.Emit(OpCodes.Ldstr, "dependencies");
        generator.Emit(OpCodes.Call, pathCombine3);
        generator.Emit(OpCodes.Stloc_2);

        generator.Append(testingBreakpoint);
        generator.Emit(OpCodes.Call, directoryExists);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ceq);
        // todo: continue

        // End
        generator.Append(retLabel);

        serverConsoleStartMethod.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, loadMethod));

        assembly.Write(assemblyPath+"a");
    }
}
