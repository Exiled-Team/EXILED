// -----------------------------------------------------------------------
// <copyright file="ReadonlyField.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Modifications
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
    /// A class to manipulate readonly fields.
    /// </summary>
    /// <typeparam name="T">Type of field.</typeparam>
    public class ReadonlyField<T>
    {
        /// <summary>
        /// Gets the list of all registered <see cref="ReadonlyField{T}"/>s.
        /// </summary>
        internal static readonly List<ReadonlyField<T>> Fields = new();

        private T value;
        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlyField{T}"/> class.
        /// </summary>
        /// <param name="fieldInfo"><inheritdoc cref="FieldInfo"/></param>
        /// <param name="value"><inheritdoc cref="StandardValue"/></param>
        /// <param name="typesToPatch"><inheritdoc cref="TypesToPatch"/></param>
        /// <exception cref="ArgumentException">Throws if <paramref name="fieldInfo"/> type mismatches <typeparamref name="T"/>.</exception>
        public ReadonlyField(FieldInfo fieldInfo, T value, params Type[] typesToPatch)
        {
            if (fieldInfo.FieldType != typeof(T))
                throw new ArgumentException($"Type mismatch. Type of FiledInfo::FieldType must be {typeof(T)}.", nameof(fieldInfo));

            FieldInfo = fieldInfo;
            TypesToPatch = typesToPatch;
            this.value = value;
            StandardValue = value;

            Fields.Add(this);
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.FieldInfo"/> that should be replaced.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets the array of types which contains methods that will be patched.
        /// </summary>
        public Type[] TypesToPatch { get; }

        /// <summary>
        /// Gets the standard value of the field, specified by the game.
        /// </summary>
        public T StandardValue { get; }

        /// <summary>
        /// Gets or sets the value which replaces actual field.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                this.value = value;

                if (!patched && !EqualityComparer<T>.Default.Equals(StandardValue, value))
                    Patch();
            }
        }

        internal static ReadonlyField<T> Get(FieldInfo fieldInfo) => Fields.FirstOrDefault(x => x.FieldInfo == fieldInfo);

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.operand is not FieldInfo fieldInfo)
                {
                    yield return instruction;
                    continue;
                }

                if (Fields.Any(x => x.FieldInfo == fieldInfo))
                {
                    /*yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReadonlyField<T>), nameof(Get)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ReadonlyField<T>), nameof(Value)));*/

                    ReadonlyField<T> field = Get(fieldInfo);

                    switch (Type.GetTypeCode(typeof(T)))
                    {
                        case TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Boolean or TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64:
                            yield return new(OpCodes.Ldc_I4_S, Convert.ToInt32(field.Value));
                            continue;
                        case TypeCode.Char or TypeCode.String:
                            yield return new(OpCodes.Ldstr, field.Value);
                            continue;
                        case TypeCode.Single:
                            yield return new(OpCodes.Ldc_R4, field.Value);
                            continue;
                        case TypeCode.Double:
                            yield return new(OpCodes.Ldc_R8, field.Value);
                            continue;
                        case TypeCode.Empty:
                            yield return instruction;
                            continue;
                        case TypeCode.Object:
                            yield return new(instruction.opcode, field.Value);
                            continue;
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        private void Patch()
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.ManagedAssemblies, "Assembly-CSharp.dll"));

            foreach (MethodInfo methodInfo in TypesToPatch.SelectMany(x => x.GetMethods().Where(y => y.DeclaringType != null && y.DeclaringType == x)))
            {
                MethodReference methodReference = assembly.MainModule.ImportReference(methodInfo);
                if (!methodReference.Resolve().Body.Instructions.Any(x => x.Operand is FieldInfo fieldInfo && fieldInfo == FieldInfo))
                    continue;

                try
                {
                    ConstProperty<T>.Harmony.Patch(methodInfo, transpiler: new HarmonyMethod(typeof(ReadonlyField<T>), nameof(Transpiler)));
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