// -----------------------------------------------------------------------
// <copyright file="Patcher.cs" company="Exiled Team">
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
    internal class Patcher
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

                Console.WriteLine("[Exiled] Loaded " + module.Name);

                Console.WriteLine("[Exiled-Assembly] Resolving References...");

                ModuleContext modCtx = ModuleDef.CreateModuleContext();

                module.Context = modCtx;

                ((AssemblyResolver)module.Context.AssemblyResolver).AddToCache(module);

                Console.WriteLine("[Injection] Injecting the Bootstrap Class.");

                ModuleDefMD bootstrap = ModuleDefMD.Load("Exiled.Bootstrap.dll");

                Console.WriteLine("[Injection] Loaded " + bootstrap.Name);

                TypeDef modClass = bootstrap.Types[0];

                foreach (var type in bootstrap.Types)
                {
                    if (type.Name == "Bootstrap")
                    {
                        modClass = type;
                        Console.WriteLine("[Injection] Hooked to: " + type.Namespace + "." + type.Name);
                    }
                }

                var modRefType = modClass;

                bootstrap.Types.Remove(modClass);

                modRefType.DeclaringType = null;

                module.Types.Add(modRefType);

                MethodDef call = FindMethod(modRefType, "Load");

                if (call == null)
                {
                    Console.WriteLine($"Failed to get the \"{call.Name}\" method! Maybe we don't have permission?");
                    return;
                }

                Console.WriteLine("[Injection] Injected!");

                Console.WriteLine("[Exiled] Injection completed!");

                Console.WriteLine("[Exiled] Patching code...");

                TypeDef def = FindType(module.Assembly, "ServerConsoleSender");

                MethodDef bctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

                if (FindMethod(def, ".ctor") != null)
                {
                    bctor = FindMethod(def, ".ctor");
                    Console.WriteLine("[Exiled] Re-using constructor.");
                }
                else
                {
                    def.Methods.Add(bctor);
                }

                CilBody body;
                bctor.Body = body = new CilBody();

                body.Instructions.Add(OpCodes.Call.ToInstruction(call));
                body.Instructions.Add(OpCodes.Ret.ToInstruction());

                module.Write("Assembly-CSharp-Exiled.dll");

                Console.WriteLine("[Exiled] Patching completed successfully!");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Exiled] An error has occurred while patching: {exception}");
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