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
        using ModuleDefinition assembly = ModuleDefinition.ReadModule(path, new ReaderParameters()
        {
            AssemblyResolver = new CustomAssemblyResolver(path)
        });

        if (assembly is null)
        {
            Console.WriteLine($"[Patcher] Assembly in {path} not found. Could not patch.");
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

        PropertyDefinition isLoadedProperty = new PropertyDefinition("IsLoaded", PropertyAttributes.None, new TypeReference("System", "Boolean", null, null, true));
        bootstrap.Properties.Add(isLoadedProperty);

        MethodDefinition loadMethod = new MethodDefinition("Load",
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot,
            new TypeReference("System", "Void", null, null));
        bootstrap.Methods.Add(loadMethod);

        MethodDefinition? addLog = serverConsoleClass.GetMethod("AddLog");

        if (addLog is null)
        {
            Console.WriteLine("[Patcher] Couldn't find method ServerConsole::AddLog()");
            return;
        }

        MethodBody loadBody = loadMethod.Body;
        ILProcessor generator = loadBody.GetILProcessor();

        Instruction loadedJmp = Instruction.Create(OpCodes.Nop);

       /* generator.Emit(OpCodes.Call, isLoadedProperty.GetMethod);
        generator.Emit(OpCodes.Stloc_0);
        generator.Emit(OpCodes.Ldloc_0);
        generator.Emit(OpCodes.Brfalse_S, loadedJmp);*/
        generator.Emit(OpCodes.Ldstr, "[Exiled.Bootstrap] Exiled has already been loaded!");
        generator.Emit(OpCodes.Ldc_I4_4);
        generator.Emit(OpCodes.Call, addLog);
        generator.Append(loadedJmp);
        generator.Emit(OpCodes.Ret);

        serverConsoleStartMethod.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, loadMethod));

        assembly.Write(path+"a");
    }
}
