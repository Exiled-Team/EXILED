// -----------------------------------------------------------------------
// <copyright file="ConstProperty.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;
    using Mono.Cecil;

    /// <summary>
    /// A class to manipulate game's constants.
    /// </summary>
    /// <typeparam name="T">Constant type.</typeparam>
    public class ConstProperty<T>
    {
        private readonly List<MethodInfo> internalPatched = new();
        private bool patched = false;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstProperty{T}"/> class.
        /// </summary>
        /// <param name="constantValue">An actual constant value.</param>
        /// <param name="typesToPatch">A collection of types where this constant is used.</param>
        /// <param name="skipMethods"><inheritdoc cref="SkipMethods"/></param>
        public ConstProperty(T constantValue, Type[] typesToPatch, MethodInfo[] skipMethods = null)
        {
            ConstantValue = constantValue;
            value = constantValue;
            TypesToPatch = typesToPatch;
            SkipMethods = skipMethods ?? Array.Empty<MethodInfo>();

            List.Add(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConstProperty{T}"/> class.
        /// </summary>
        ~ConstProperty()
        {
            foreach (MethodInfo methodInfo in PatchedMethods)
            {
                try
                {
                    Harmony.Unpatch(methodInfo, AccessTools.Method(typeof(ConstProperty<T>), nameof(Transpiler)));
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

        /// <summary>
        /// Gets the value of game's constant.
        /// </summary>
        public T ConstantValue { get; }

        /// <summary>
        /// Gets or sets a value which will replace <see cref="ConstantValue"/> in types.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                this.value = value;

                if (!patched && !EqualityComparer<T>.Default.Equals(value, ConstantValue))
                    Patch();
            }
        }

        /// <summary>
        /// Gets a collection of types where <see cref="ConstantValue"/> should be replaced with <see cref="Value"/>.
        /// </summary>
        public Type[] TypesToPatch { get; }

        /// <summary>
        /// Gets a collection of methods that should be skipped when patching.
        /// </summary>
        public MethodInfo[] SkipMethods { get; }

        /// <summary>
        /// Gets a collection of methods that are patched.
        /// </summary>
        public IReadOnlyCollection<MethodInfo> PatchedMethods => internalPatched;

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance for this constant property.
        /// </summary>
        internal static Harmony Harmony { get; } = new($"exiled.api-{typeof(T).FullName}");

        /// <summary>
        /// Gets the list of all <see cref="ConstProperty{T}"/>.
        /// </summary>
        internal static List<ConstProperty<T>> List { get; } = new();

        /// <summary>
        /// A converter to value's type.
        /// </summary>
        /// <param name="property">The <see cref="ConstProperty{T}"/> instance.</param>
        /// <returns>A <see cref="ConstProperty{T}"/> value.</returns>
        public static implicit operator T(ConstProperty<T> property) => property.Value;

        /// <summary>
        /// Gets the <see cref="ConstProperty{T}"/> by it's patched type and constant value.
        /// </summary>
        /// <param name="constValue">A game's constant value.</param>
        /// <param name="type">Type where this constant is using.</param>
        /// <returns>The <see cref="ConstProperty{T}"/> instance or <see langword="null"/>.</returns>
        internal static ConstProperty<T> Get(T constValue, Type type) => List.Find(x => x.TypesToPatch.Contains(type) && typeof(T) == x.ConstantValue.GetType() && EqualityComparer<T>.Default.Equals(constValue, x.ConstantValue));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type currentType = original.DeclaringType;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.operand == null || instruction.operand.GetType() != typeof(T))
                {
                    yield return instruction;
                    continue;
                }

                ConstProperty<T> property = Get((T)instruction.operand, currentType);

                if (property == null || !EqualityComparer<T>.Default.Equals((T)instruction.operand, property.ConstantValue))
                {
                    yield return instruction;
                    continue;
                }

                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Boolean or TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64:
                        yield return new(OpCodes.Ldc_I4_S, Convert.ToInt32(property.Value));
                        continue;
                    case TypeCode.Char or TypeCode.String:
                        yield return new(OpCodes.Ldstr, property.Value);
                        continue;
                    case TypeCode.Single:
                        yield return new(OpCodes.Ldc_R4, property.Value);
                        continue;
                    case TypeCode.Double:
                        yield return new(OpCodes.Ldc_R8, property.Value);
                        continue;
                    case TypeCode.Empty:
                        yield return instruction;
                        continue;
                    case TypeCode.Object:
                        yield return new(instruction.opcode, property.Value);
                        continue;
                }

                yield return instruction;
            }
        }

        private void Patch()
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.ManagedAssemblies, "Assembly-CSharp.dll"));

            foreach (MethodInfo methodInfo in TypesToPatch.SelectMany(x => x.GetProperties().Where(y => y.SetMethod != null && y.DeclaringType != null && y.DeclaringType == x)
                         .Select(y => y.SetMethod)))
            {
                if (Array.Exists(SkipMethods, x => x == methodInfo))
                    continue;

                MethodReference methodReference = assembly.MainModule.ImportReference(methodInfo);
                if (!methodReference.Resolve().Body.Instructions.Any(x => x.Operand is T t && EqualityComparer<T>.Default.Equals(t, ConstantValue)))
                    continue;

                try
                {
                    Harmony.Patch(methodInfo, transpiler: new HarmonyMethod(typeof(ConstProperty<T>), nameof(Transpiler)));
                    internalPatched.Add(methodInfo);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }

            foreach (MethodInfo methodInfo in TypesToPatch.SelectMany(x => x.GetMethods().Where(y => y.DeclaringType != null && y.DeclaringType == x)))
            {
                if (Array.Exists(SkipMethods, x => x == methodInfo))
                    continue;

                MethodReference methodReference = assembly.MainModule.ImportReference(methodInfo);
                if (!methodReference.Resolve().Body.Instructions.Any(x => x.Operand is T t && EqualityComparer<T>.Default.Equals(t, ConstantValue)))
                    continue;

                try
                {
                    Harmony.Patch(methodInfo, transpiler: new HarmonyMethod(typeof(ConstProperty<T>), nameof(Transpiler)));
                    internalPatched.Add(methodInfo);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }

            patched = true;
        }
    }
}