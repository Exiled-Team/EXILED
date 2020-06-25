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
                string path;

                if (args.Length != 1)
                {
                    Console.WriteLine("Missing file location argument!");
                    Console.WriteLine("Provide the location of Assembly-CSharp.dll:");

                    path = Console.ReadLine();
                }
                else
                {
                    path = args[0];
                }

                ModuleDefMD module = ModuleDefMD.Load(path);

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

                Console.WriteLine("[Exiled.Patcher] Loaded " + module.Name);

                Console.WriteLine("[Exiled.Patcher] Resolving References...");

                module.Context = ModuleDef.CreateModuleContext();

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
                    Console.WriteLine($"Failed to get the \"{call.Name}\" method! Maybe you don't have permission?");
                    return;
                }

                Console.WriteLine("[Injection] Injected!");

                Console.WriteLine("[Exiled.Patcher] Injection completed!");

                Console.WriteLine("[Exiled.Patcher] Patching code...");

                TypeDef typeDef = FindType(module.Assembly, "ServerConsole");

                MethodDef start = FindMethod(typeDef, "Start");

                if (start == null)
                {
                    start = new MethodDefUser("Start", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
                    typeDef.Methods.Add(start);
                }

                start.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(call));

                module.Write("Assembly-CSharp-Exiled.dll");

                Console.WriteLine("[Exiled.Patcher] Patching completed successfully!");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[Exiled.Patcher] An error has occurred while patching: {exception}");
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
