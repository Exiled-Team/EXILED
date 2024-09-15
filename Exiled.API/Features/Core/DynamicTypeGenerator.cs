// -----------------------------------------------------------------------
// <copyright file="DynamicTypeGenerator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Provides methods for dynamically generating types at runtime.
    /// </summary>
    public static class DynamicTypeGenerator
    {
        /// <summary>
        /// Generates a dynamic type with a default constructor based on the provided base type.
        /// </summary>
        /// <param name="baseType">The base type to derive the new type from.</param>
        /// <param name="typeName">Optional name for the generated type.</param>
        /// <param name="classAttributes">Optional array containing all type attributes the class must implement to be dynamically generated.</param>
        /// <param name="rwOnly">Optional value indicating whether only read and write properties should be collected.</param>
        /// <param name="attributes">Optional array containing all type attributes the property must implement to be dynamically generated.</param>
        /// <returns>The dynamically generated type.</returns>
        public static Type GenerateDynamicTypeWithConstructorFromExistingType(
            this Type baseType,
            string typeName = null,
            CustomAttributeBuilder[] classAttributes = null,
            bool rwOnly = true,
            Type[] attributes = null)
        {
            AssemblyName assemblyName = new($"ExiledDynamic_{Guid.NewGuid()}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            string fullName = string.IsNullOrEmpty(typeName) ? baseType.Name + "Dynamic" : typeName;
            TypeBuilder typeBuilder = moduleBuilder.AddClass(fullName, baseType);

            if (classAttributes is not null && !classAttributes.IsEmpty())
                classAttributes.ForEach(att => typeBuilder.SetCustomAttribute(att));

            typeBuilder.AddConstructor();

            void GenerateFieldAndProperty(PropertyInfo property)
            {
                if ((!rwOnly && property.CanRead && !property.CanWrite) ||
                    (rwOnly && property.CanRead && property.CanWrite))
                {
                    typeBuilder.AddField(property.Name, property.PropertyType);
                    typeBuilder.AddProperty(property.Name, property.PropertyType);
                }
            }

            if (attributes is null || attributes.IsEmpty())
            {
                foreach (PropertyInfo property in baseType.GetProperties())
                    GenerateFieldAndProperty(property);
            }
            else
            {
                foreach (PropertyInfo property in baseType.GetProperties())
                {
                    if (property.GetCustomAttributes().Any(att => attributes.Contains(att.GetType())))
                        continue;

                    GenerateFieldAndProperty(property);
                }
            }

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Generates a dynamic type with a constructor, allowing external actions to modify the type before creation.
        /// </summary>
        /// <param name="typeName">The name of the generated type.</param>
        /// <param name="constructorParams">The types of the constructor parameters.</param>
        /// <param name="preCreateActions">A list of actions to modify the type before it is created.</param>
        /// <returns>The dynamically generated type with a constructor.</returns>
        public static Type GenerateDynamicTypeWithConstructor(
            string typeName,
            Type[] constructorParams = null,
            List<Action<TypeBuilder>> preCreateActions = null)
        {
            AssemblyName assemblyName = new($"ExiledDynamic_{Guid.NewGuid()}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            TypeBuilder typeBuilder = moduleBuilder.AddClass(typeName);

            typeBuilder.AddConstructor(constructorParams);

            preCreateActions?.ForEach(action => action(typeBuilder));

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Generates a dynamic type with custom attributes and external actions to modify the type before creation.
        /// </summary>
        /// <param name="typeName">The name of the generated type.</param>
        /// <param name="classAttributes">Optional array containing attributes to apply on the generated type.</param>
        /// <param name="propertyAttributes">Optional dictionary specifying attributes for each property.</param>
        /// <param name="fields">A dictionary where the key is the field name and the value is the field type.</param>
        /// <param name="preCreateActions">A list of actions to modify the type before it is created.</param>
        /// <returns>The dynamically generated type with custom class and property attributes.</returns>
        public static Type GenerateDynamicTypeWithAttributes(
            string typeName,
            CustomAttributeBuilder[] classAttributes = null,
            Dictionary<string, CustomAttributeBuilder[]> propertyAttributes = null,
            Dictionary<string, Type> fields = null,
            List<Action<TypeBuilder>> preCreateActions = null)
        {
            AssemblyName assemblyName = new($"ExiledDynamic_{Guid.NewGuid()}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            TypeBuilder typeBuilder = moduleBuilder.AddClass(typeName);

            if (classAttributes != null && classAttributes.Length > 0)
            {
                foreach (CustomAttributeBuilder att in classAttributes)
                    typeBuilder.SetCustomAttribute(att);
            }

            foreach (KeyValuePair<string, Type> field in fields)
            {
                typeBuilder.AddField(field.Key, field.Value);
                typeBuilder.AddProperty(field.Key, field.Value);

                if (propertyAttributes != null && propertyAttributes.TryGetValue(field.Key, out CustomAttributeBuilder[] attrs))
                {
                    foreach (CustomAttributeBuilder attr in attrs)
                        typeBuilder.SetCustomAttribute(attr);
                }
            }

            typeBuilder.AddConstructor();

            preCreateActions?.ForEach(action => action(typeBuilder));

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Adds a new class to the module builder.
        /// </summary>
        /// <param name="moduleBuilder">The module builder to add the class to.</param>
        /// <param name="className">The name of the new class.</param>
        /// <param name="parentType">Optional base type for the new class.</param>
        /// <returns>The newly created type builder.</returns>
        public static TypeBuilder AddClass(this ModuleBuilder moduleBuilder, string className, Type parentType = null) =>
            moduleBuilder.DefineType(
                className,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                parentType ?? typeof(object));

        /// <summary>
        /// Adds a new class to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the class to.</param>
        /// <param name="className">The name of the new class.</param>
        /// <param name="parentType">Optional base type for the new class.</param>
        /// <returns>The newly created type builder.</returns>
        public static TypeBuilder AddClass(this TypeBuilder typeBuilder, string className, Type parentType = null) =>
            ((ModuleBuilder)typeBuilder.Module).AddClass(className, parentType);

        /// <summary>
        /// Adds a new struct to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the struct to.</param>
        /// <param name="structName">The name of the new struct.</param>
        /// <returns>The newly created type builder.</returns>
        public static TypeBuilder AddStruct(this TypeBuilder typeBuilder, string structName)
        {
            return ((ModuleBuilder)typeBuilder.Module).DefineType(
                structName,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.BeforeFieldInit,
                typeof(ValueType));
        }

        /// <summary>
        /// Adds a new field to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the field to.</param>
        /// <param name="fieldName">The name of the new field.</param>
        /// <param name="fieldType">The type of the new field.</param>
        /// <returns>The newly created field builder.</returns>
        public static FieldBuilder AddField(this TypeBuilder typeBuilder, string fieldName, Type fieldType) =>
            typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);

        /// <summary>
        /// Adds a new method to the type builder with custom IL generation.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the method to.</param>
        /// <param name="methodName">The name of the new method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the method parameters.</param>
        /// <param name="ilGeneratorAction">Action to generate the IL for the method.</param>
        /// <returns>The newly created method builder.</returns>
        public static MethodBuilder AddMethod(
            this TypeBuilder typeBuilder,
            string methodName,
            Type returnType,
            Type[] parameterTypes,
            Action<ILGenerator> ilGeneratorAction)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                returnType,
                parameterTypes);

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGeneratorAction(ilGenerator);
            ilGenerator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        /// <summary>
        /// Adds a new field to the type builder with custom field attributes.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the field to.</param>
        /// <param name="fieldName">The name of the new field.</param>
        /// <param name="fieldType">The type of the new field.</param>
        /// <param name="fieldAttributes">Custom attributes for the field.</param>
        /// <returns>The newly created field builder.</returns>
        public static FieldBuilder AddField(this TypeBuilder typeBuilder, string fieldName, Type fieldType, FieldAttributes fieldAttributes = FieldAttributes.Public) =>
            typeBuilder.DefineField(fieldName, fieldType, fieldAttributes);

        /// <summary>
        /// Adds a new property to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the property to.</param>
        /// <param name="propertyName">The name of the new property.</param>
        /// <param name="propertyType">The type of the new property.</param>
        public static void AddProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getterMethod = typeBuilder.DefineMethod("get_" + propertyName, methodAttributes, propertyType, Type.EmptyTypes);
            ILGenerator getterIL = getterMethod.GetILGenerator();
            getterIL.Emit(OpCodes.Ldarg_0);
            getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getterMethod);

            MethodBuilder setterMethod = typeBuilder.DefineMethod("set_" + propertyName, methodAttributes, null, new[] { propertyType });
            ILGenerator setterIL = setterMethod.GetILGenerator();
            setterIL.Emit(OpCodes.Ldarg_0);
            setterIL.Emit(OpCodes.Ldarg_1);
            setterIL.Emit(OpCodes.Stfld, fieldBuilder);
            setterIL.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        /// <summary>
        /// Adds a default constructor to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the constructor to.</param>
        /// <param name="parameterTypes">The types of the constructor parameters.</param>
        public static void AddConstructor(this TypeBuilder typeBuilder, Type[] parameterTypes = null)
        {
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);

            ILGenerator constructorIL = constructorBuilder.GetILGenerator();

            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            if (parameterTypes is not null && !parameterTypes.IsEmpty())
            {
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    constructorIL.Emit(OpCodes.Ldarg_0);
                    constructorIL.Emit(OpCodes.Ldarg, i + 1);
                    FieldInfo field = typeBuilder.DefineField($"_param{i}", parameterTypes[i], FieldAttributes.Private);
                    constructorIL.Emit(OpCodes.Stfld, field);
                }
            }

            constructorIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Adds a new enum to the module builder.
        /// </summary>
        /// <param name="moduleBuilder">The module builder to add the enum to.</param>
        /// <param name="enumName">The name of the new enum.</param>
        /// <param name="underlyingType">The underlying type of the enum.</param>
        /// <returns>The newly created type builder for the enum.</returns>
        /// <exception cref="ArgumentException">Thrown when the underlying type is not valid.</exception>
        public static TypeBuilder AddEnum(this ModuleBuilder moduleBuilder, string enumName, Type underlyingType)
        {
            if (!underlyingType.IsEnum)
                throw new ArgumentException("Underlying type must be an enum.");

            if (underlyingType != typeof(byte) &&
                underlyingType != typeof(sbyte) &&
                underlyingType != typeof(short) &&
                underlyingType != typeof(ushort) &&
                underlyingType != typeof(int) &&
                underlyingType != typeof(uint) &&
                underlyingType != typeof(long) &&
                underlyingType != typeof(ulong))
            {
                throw new ArgumentException("Underlying type must be one of the valid enum underlying types.");
            }

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                enumName,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Serializable,
                typeof(Enum));

            typeBuilder.DefineField(
                "value__",
                underlyingType,
                FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName);

            return typeBuilder;
        }

        /// <summary>
        /// Adds a new enum to the type builder.
        /// </summary>
        /// <param name="typeBuilder">The type builder to add the enum to.</param>
        /// <param name="enumName">The name of the new enum.</param>
        /// <param name="underlyingType">The underlying type of the enum.</param>
        /// <returns>The newly created type builder for the enum.</returns>
        /// <exception cref="ArgumentException">Thrown when the underlying type is not valid.</exception>
        public static TypeBuilder AddEnum(this TypeBuilder typeBuilder, string enumName, Type underlyingType) =>
            ((ModuleBuilder)typeBuilder.Module).AddEnum(enumName, underlyingType);

        /// <summary>
        /// Adds a custom attribute to the specified field.
        /// </summary>
        /// <param name="fieldBuilder">The FieldBuilder to add the attribute to.</param>
        /// <param name="attributeType">The type of the attribute to add.</param>
        /// <param name="constructorArguments">Optional. Arguments to pass to the attribute's constructor.</param>
        /// <param name="namedArguments">Optional. Named property values to set on the attribute.</param>
        /// <exception cref="ArgumentException">Thrown when a matching constructor for the attribute is not found.</exception>
        public static void AddAttribute(
            this FieldBuilder fieldBuilder,
            Type attributeType,
            object[] constructorArguments = null,
            (string Name, object Value)[] namedArguments = null) =>
            fieldBuilder.SetCustomAttribute(BuildAttribute(attributeType, constructorArguments, namedArguments));

        /// <summary>
        /// Adds a custom attribute to the specified property.
        /// </summary>
        /// <param name="propertyBuilder">The PropertyBuilder to add the attribute to.</param>
        /// <param name="attributeType">The type of the attribute to add.</param>
        /// <param name="constructorArguments">Optional. Arguments to pass to the attribute's constructor.</param>
        /// <param name="namedArguments">Optional. Named property values to set on the attribute.</param>
        /// <exception cref="ArgumentException">Thrown when a matching constructor for the attribute is not found.</exception>
        public static void AddAttribute(
            this PropertyBuilder propertyBuilder,
            Type attributeType,
            object[] constructorArguments = null,
            (string Name, object Value)[] namedArguments = null) =>
            propertyBuilder.SetCustomAttribute(BuildAttribute(attributeType, constructorArguments, namedArguments));

        /// <summary>
        /// Adds a custom attribute to the specified method.
        /// </summary>
        /// <param name="methodBuilder">The MethodBuilder to add the attribute to.</param>
        /// <param name="attributeType">The type of the attribute to add.</param>
        /// <param name="constructorArguments">Optional. Arguments to pass to the attribute's constructor.</param>
        /// <param name="namedArguments">Optional. Named property values to set on the attribute.</param>
        /// <exception cref="ArgumentException">Thrown when a matching constructor for the attribute is not found.</exception>
        public static void AddAttribute(
            this MethodBuilder methodBuilder,
            Type attributeType,
            object[] constructorArguments = null,
            (string Name, object Value)[] namedArguments = null) =>
            methodBuilder.SetCustomAttribute(BuildAttribute(attributeType, constructorArguments, namedArguments));

        /// <summary>
        /// Builds a custom attribute for the specified <see cref="Type"/> using the provided constructor and named arguments.
        /// </summary>
        /// <param name="attributeType">The <see cref="Type"/> of the attribute to build.</param>
        /// <param name="constructorArguments">Optional constructor arguments to be passed to the attribute constructor.</param>
        /// <param name="namedArguments">Optional named arguments to set as properties on the attribute.</param>
        /// <returns>A <see cref="CustomAttributeBuilder"/> instance representing the custom attribute.</returns>
        /// <exception cref="ArgumentException">Thrown when a matching constructor cannot be found for the attribute type.</exception>
        public static CustomAttributeBuilder BuildAttribute(
            Type attributeType,
            object[] constructorArguments = null,
            (string Name, object Value)[] namedArguments = null)
        {
            ConstructorInfo constructorInfo = attributeType.GetConstructor(constructorArguments?.Select(a => a.GetType()).ToArray());

            if (constructorInfo == null)
                throw new ArgumentException("Cannot find a matching constructor for the attribute.");

            return new(
                constructorInfo,
                constructorArguments ?? Array.Empty<object>(),
                namedArguments?.Select(na => attributeType.GetProperty(na.Name)).ToArray() ?? Array.Empty<PropertyInfo>(),
                namedArguments?.Select(na => na.Value).ToArray() ?? Array.Empty<object>());
        }
    }
}