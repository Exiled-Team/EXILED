// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Patcher
{
    using System;
    using dnlib.DotNet;
    using dnlib.DotNet.Emit;

    /// <summary>
    /// Takes a file path to your assembly as input, and will patch the assembly with the EXILED ModLoader class.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("Missing file location argument!");
                    return;
                }

                ModuleDefMD module = ModuleDefMD.Load(args[0]);

                if (module == null)
                {
                    Console.WriteLine("File not found!");
                    return;
                }

                module.IsILOnly = true;
                module.VTableFixups = null;
                module.IsStrongNameSigned = false;
                module.Assembly.PublicKey = null;
                module.Assembly.HasPublicKey = false;

                Console.WriteLine("[EXILED] Loaded " + module.Name);

                Console.WriteLine("[EXILED-ASSEMBLY] Resolving References...");

                ModuleContext modCtx = ModuleDef.CreateModuleContext();

                module.Context = modCtx;

                ((AssemblyResolver)module.Context.AssemblyResolver).AddToCache(module);

                Console.WriteLine("[INJECTION] Injecting the ModLoader Class.");

                ModuleDefMD modLoader = ModuleDefMD.Load("ModLoader.dll");

                Console.WriteLine("[INJECTION] Loaded " + modLoader.Name);

                TypeDef modClass = modLoader.Types[0];

                foreach (var type in modLoader.Types)
                {
                    if (type.Name == "ModLoader")
                    {
                        modClass = type;
                        Console.WriteLine("[INJECTION] Hooked to: " + type.Namespace + "." + type.Name);
                    }
                }

                var modRefType = modClass;

                modLoader.Types.Remove(modClass);

                modRefType.DeclaringType = null;

                module.Types.Add(modRefType);

                MethodDef call = FindMethod(modRefType, "LoadBoi");

                if (call == null)
                {
                    Console.WriteLine("Failed to get the 'LoadBoi' method! Maybe we don't have permission?");
                    return;
                }

                Console.WriteLine("[INJECTION] Injected!");

                Console.WriteLine("[EXILED] Completed injection!");

                Console.WriteLine("[EXILED] Patching code...");

                TypeDef def = FindType(module.Assembly, "ServerConsoleSender");

                MethodDef bctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

                if (FindMethod(def, ".ctor") != null)
                {
                    bctor = FindMethod(def, ".ctor");
                    Console.WriteLine("[EXILED] Re-using constructor.");
                }
                else
                {
                    def.Methods.Add(bctor);
                }

                CilBody body;
                bctor.Body = body = new CilBody();

                body.Instructions.Add(OpCodes.Call.ToInstruction(call));
                body.Instructions.Add(OpCodes.Ret.ToInstruction());

                module.Write("Assembly-CSharp-EXILED.dll");
                Console.WriteLine("[EXILED] COMPLETE!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.Read();
        }

        private static MethodDef FindMethod(TypeDef type, string methodName)
        {
            if (type != null)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Name == methodName)
                        return method;
                }
            }

            return null;
        }

        private static TypeDef FindType(AssemblyDef asm, string classPath)
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == classPath)
                        return type;
                }
            }

            return null;
        }
    }
}