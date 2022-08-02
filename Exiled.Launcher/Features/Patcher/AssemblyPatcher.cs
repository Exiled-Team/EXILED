// -----------------------------------------------------------------------
// <copyright file="AssemblyPatcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

using FieldAttributes = dnlib.DotNet.FieldAttributes;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using MethodImplAttributes = dnlib.DotNet.MethodImplAttributes;
using PropertyAttributes = dnlib.DotNet.PropertyAttributes;
using TypeAttributes = dnlib.DotNet.TypeAttributes;

namespace Exiled.Launcher.Features.Patcher;

public static class AssemblyPatcher
{
    public static void Patch(string path)
    {
        ModuleDefMD assembly = ModuleDefMD.Load(path);

        if (assembly is null)
        {
            Console.WriteLine($"[Patcher] Assembly in {path} not found. Could not patch.");
            return;
        }

        assembly.Context = ModuleDef.CreateModuleContext();

        ((AssemblyResolver)assembly.Context.AssemblyResolver).AddToCache(assembly);

        if (HelpMethods.IsPatched(assembly))
        {
            Console.WriteLine("[Patcher] Assembly already patched. Skyping patching.");
            return;
        }

        Console.WriteLine("[Patcher] Finding ServerConsole::Start() method.");

        TypeDef? serverConsoleDef = HelpMethods.FindServerConsoleDefinition(assembly);

        if (serverConsoleDef is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole.");
            return;
        }

        MethodDef? startMethodDef = HelpMethods.FindMethodDefinition(serverConsoleDef, "Start");

        if (startMethodDef is null)
        {
            Console.WriteLine("[Patcher] Could not find ServerConsole::Start()");
            return;
        }

        Console.WriteLine("[Patcher] Hooking Exiled.Bootstrap to ServerConsole::Start()");

        TypeDef bootstrapType = new TypeDefUser("Exiled", "Bootstrap", assembly.CorLibTypes.Object.TypeDefOrRef);
        bootstrapType.Attributes = TypeAttributes.Public | TypeAttributes.Class;

        PropertyDef isLoadedDef = new PropertyDefUser("IsLoaded", new PropertySig(false, assembly.CorLibTypes.Boolean));
        bootstrapType.Properties.Add(isLoadedDef);

        MethodDef loadMethodDef = new MethodDefUser("Load", MethodSig.CreateStatic(assembly.CorLibTypes.Void),
            MethodImplAttributes.IL | MethodImplAttributes.Managed,
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig |
            MethodAttributes.ReuseSlot);
        bootstrapType.Methods.Add(loadMethodDef);

        CilBody loadMethodBody = new CilBody();
        loadMethodDef.Body = loadMethodBody;

        TypeRef serverConsoleRef = new TypeRefUser(assembly, "ServerConsole");
        MemberRef addLogRef = new MemberRefUser(assembly, "AddLog", MethodSig.CreateStatic(assembly.CorLibTypes.Void, assembly.CorLibTypes.String, assembly.CorLibTypes.GetTypeRef("System", "ConsoleColor").ToTypeSig()), serverConsoleRef);

        //loadMethodBody.Instructions.Add(OpCodes.Call.ToInstruction(isLoadedDef.GetMethod));
        //loadMethodBody.Instructions.Add(OpCodes.Stloc_0.ToInstruction());
        //loadMethodBody.Instructions.Add(OpCodes.Ldloc_0.ToInstruction());
        //loadMethodBody.Instructions.Add(OpCodes.Brfalse_S.ToInstruction());
        loadMethodBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("[Exiled.Bootstrap] Exiled has already been loaded!"));
        loadMethodBody.Instructions.Add(OpCodes.Ldc_I4_4.ToInstruction());
        loadMethodBody.Instructions.Add(OpCodes.Call.ToInstruction(addLogRef));
        loadMethodBody.Instructions.Add(OpCodes.Ret.ToInstruction());

        assembly.Types.Add(bootstrapType);
        startMethodDef.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(loadMethodDef));

        assembly.Write(path + "a");
    }
}
