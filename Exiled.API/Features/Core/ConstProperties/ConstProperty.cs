// -----------------------------------------------------------------------
// <copyright file="ConstProperty.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.ConstProperties
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    /// <summary>
    /// A class to manipulate game's constants.
    /// </summary>
    /// <typeparam name="T">Constant type.</typeparam>
    public class ConstProperty<T>
    {
        private bool patched;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstProperty{T}"/> class.
        /// </summary>
        /// <param name="constantValue">An actual constant value.</param>
        /// <param name="typesToPatch">A collection of types where this constant is used.</param>
        public ConstProperty(T constantValue, params Type[] typesToPatch)
        {
            ConstantValue = constantValue;
            value = constantValue;
            TypesToPatch = typesToPatch;

            List.Add(this);
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

                if (!patched)
                {
                    Patch();
                    patched = true;
                }
            }
        }

        /// <summary>
        /// Gets a collection of types where <see cref="ConstantValue"/> should be replaced with <see cref="Value"/>.
        /// </summary>
        public Type[] TypesToPatch { get; }

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
                if (instruction.operand.GetType() != typeof(T))
                {
                    yield return instruction;
                    continue;
                }

                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Boolean or TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64:

                        ConstProperty<T> property = Get((T)instruction.operand, currentType);

                        if (instruction.opcode == OpCodes.Ldc_I4_S || instruction.opcode == OpCodes.Ldc_I4 || instruction.opcode == OpCodes.Ldc_I8)
                        {
                            yield return new(instruction.opcode, property.Value);
                            continue;
                        }

                        yield return new(OpCodes.Ldc_I4_S, property.Value);
                        continue;
                    case TypeCode.Char or TypeCode.String:

                        property = Get((T)instruction.operand, currentType);

                        if (property == null)
                            continue;

                        yield return new(OpCodes.Ldstr, property.Value);
                        continue;
                    case TypeCode.Single:

                        property = Get((T)instruction.operand, currentType);

                        if (property == null)
                            continue;

                        yield return new(OpCodes.Ldc_R4, property.Value);
                        continue;
                    case TypeCode.Double:

                        property = Get((T)instruction.operand, currentType);

                        if (property == null)
                            continue;

                        yield return new(OpCodes.Ldc_R8, property.Value);
                        continue;
                    case TypeCode.Empty:
                        yield return instruction;
                        continue;
                    case TypeCode.Object:

                        property = Get((T)instruction.operand, currentType);

                        if (property == null)
                            continue;

                        yield return new(OpCodes.Ldstr, property.Value);
                        continue;
                }

                yield return instruction;
            }
        }

        private void Patch()
        {
            foreach (MethodInfo methodInfo in TypesToPatch.SelectMany(x => x.GetMethods()))
            {
                Harmony.Patch(methodInfo, transpiler: new HarmonyMethod(typeof(ConstProperty<T>), nameof(Transpiler)));
            }

            patched = true;
        }
    }
}