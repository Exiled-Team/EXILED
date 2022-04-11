// -----------------------------------------------------------------------
// <copyright file="EObject.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// The base class of all Exiled objects.
    /// </summary>
    public abstract class EObject : TypeCastObject<EObject>
    {
        private static readonly Dictionary<Type, List<string>> InternalRegisteredTypes = new();
        private static readonly List<EObject> InternalObjects = new();
        private bool destroyedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EObject"/> class.
        /// </summary>
        protected EObject()
            : base()
        {
            IsEditable = true;
            InternalObjects.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        protected EObject(GameObject gameObject = null)
            : this()
        {
            if (gameObject is not null)
                Base = gameObject;
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="EObject"/> instance.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the tag of the <see cref="EObject"/> instance.
        /// </summary>
        [YamlIgnore]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EObject"/> values can be edited.
        /// </summary>
        [YamlIgnore]
        public bool IsEditable { get; set; }

        /// <summary>
        /// Gets or sets the base <see cref="GameObject"/>.
        /// </summary>
        [YamlIgnore]
        public GameObject Base { get; protected set; }

        /// <summary>
        /// Gets all the registered <see cref="EObject"/> types.
        /// </summary>
        [YamlIgnore]
        protected static IReadOnlyDictionary<Type, List<string>> RegisteredTypes => InternalRegisteredTypes;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="EObject"/> containing all the active <see cref="EObject"/> instances.
        /// </summary>
        [YamlIgnore]
        protected static IReadOnlyCollection<EObject> Objects => InternalObjects;

        /// <summary>
        /// Gets a <see cref="Type"/> from a given type name.
        /// </summary>
        /// <param name="typeName">The type name to look for.</param>
        /// <returns>A <see cref="Type"/> matching the type name or <see langword="null"/> if not found.</returns>
        public static Type GetEObjectTypeByName(string typeName)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Name != typeName || type.IsSubclassOf(typeof(EObject)))
                    continue;

                return type;
            }

            return null;
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="name">The name of the registered type.</param>
        /// <returns>The registered <see cref="EObject"/> type.</returns>
        public static Type RegisterEObjectType<T>(string name)
            where T : EObject
        {
            Type matching = GetEObjectTypeFromRegisteredTypes<T>(name);
            if (matching is not null)
                return matching;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Name != typeof(T).Name)
                    continue;

                if (InternalRegisteredTypes[t] is not null)
                {
                    InternalRegisteredTypes[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name, };
                    InternalRegisteredTypes.Add(t, values);
                }

                return typeof(T);
            }

            throw new NullReferenceException($"Couldn't find a defined EObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="registeredType">The registered type.</param>
        /// <returns><see langword="true"/> if the type was registered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool RegisterEObjectType(Type type, string name, out Type registeredType)
        {
            registeredType = GetEObjectTypeFromRegisteredTypes(type, name);
            if (registeredType is not null)
                return false;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(item => item.IsSubclassOf(typeof(EObject))))
            {
                if (t.Name != type.Name)
                    continue;

                if (InternalRegisteredTypes.ContainsKey(t))
                {
                    InternalRegisteredTypes[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name, };
                    InternalRegisteredTypes.Add(t, values);
                }

                registeredType = t;
                return true;
            }

            throw new NullReferenceException($"Couldn't find a defined EObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterEObjectType(Type type) => InternalRegisteredTypes.Remove(type);

        /// <summary>
        /// Unregisters the specified <see cref="EObject"/> type.
        /// </summary>
        /// <param name="name">The name of the type to unregister.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterEObjectType(string name)
        {
            foreach (KeyValuePair<Type, List<string>> kvp in InternalRegisteredTypes)
            {
                if (kvp.Value.Contains(name))
                    continue;

                InternalRegisteredTypes.Remove(kvp.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the most accurate <see cref="Type"/> matching the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="ignoreAbstractTypes">A value indicating whether abstract types should be ignored.</param>
        /// <returns>The <see cref="Type"/> with the name that matches the given name.</returns>
        public static Type FindEObjectDefinedTypeByName(string name, bool ignoreAbstractTypes = true)
        {
            Type[] assemblyTypes = ignoreAbstractTypes ?
                Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.IsAbstract).ToArray() :
                Assembly.GetExecutingAssembly().GetTypes();
            List<int> matches = new();
            matches.AddRange(assemblyTypes.Select(type =>
            LevenshteinDistance(type.Name, name)));
            return assemblyTypes[matches.IndexOf(matches.Min())];
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        public static Type GetEObjectTypeFromRegisteredTypes<T>(string name)
            where T : EObject
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in InternalRegisteredTypes)
            {
                if (kvp.Key != typeof(T) || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetEObjectTypeFromRegisteredTypes(Type type, string name)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in InternalRegisteredTypes)
            {
                if (kvp.Key != type || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetEObjectTypeFromRegisteredTypes(string name)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in InternalRegisteredTypes)
            {
                if (kvp.Key.Name != name || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetEObjectTypeFromRegisteredTypesByName(string name) => InternalRegisteredTypes.FirstOrDefault(kvp => kvp.Value.Contains(name)).Key;

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>()
            where T : EObject => Activator.CreateInstance(typeof(T), true) is not EObject outer ? null : outer.Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type)
            where T : EObject => Activator.CreateInstance(type, true) is not EObject outer ? null : outer.Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(params object[] parameters)
            where T : EObject => Activator.CreateInstance(typeof(T), parameters) is not EObject outer ? null : outer.Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return Activator.CreateInstance(type, flags, null, parameters, null) is not EObject outer ? null : outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type to cast.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, params object[] parameters)
            where T : EObject => CreateDefaultSubobject(type, parameters).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name)
            where T : EObject
        {
            if (CreateDefaultSubobject<T>() is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer.Cast<T>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name, params object[] parameters)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(type);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name, params object[] parameters)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(type, parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, GameObject gameObject, string name)
        {
            if (CreateDefaultSubobject(type) is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, GameObject gameObject, string name, params object[] parameters)
        {
            if (CreateDefaultSubobject(type, parameters) is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Destroys all the active <see cref="EObject"/> instances.
        /// </summary>
        public static void DestroyAllObjects()
        {
            foreach (EObject @object in InternalObjects)
                @object.Destroy();

            InternalObjects.Clear();
        }

        /// <summary>
        /// Destroys all the active <typeparamref name="T"/> <see cref="EObject"/> instances.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        public static void DestroyAllObjectsOfType<T>()
            where T : EObject
        {
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    obj.Destroy();
            }
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>()
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(string name)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && obj.Name == name)
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> with the specified <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">The tag to look for.</param>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsWithTagOfType<T>(string tag)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && obj.Tag.ToLower().Contains(tag))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Destroys the current <see cref="EObject"/> instance.
        /// </summary>
        public void Destroy()
        {
            Destroy(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Fired before the current <see cref="EObject"/> instance is destroyed.
        /// </summary>
        protected virtual void OnBeginDestroy()
        {
        }

        /// <summary>
        /// Fired when the current <see cref="EObject"/> instance has been explicitly destroyed.
        /// </summary>
        protected virtual void OnDestroyed()
        {
        }

        /// <inheritdoc cref="Destroy()"/>
        protected virtual void Destroy(bool destroying)
        {
            if (!destroyedValue)
            {
                if (destroying)
                {
                    OnBeginDestroy();
                    InternalObjects.Remove(this);
                }

                OnDestroyed();
                destroyedValue = true;
            }
        }

        private static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            if (source.Length > target.Length)
            {
                string temp = target;
                target = source;
                source = temp;
            }

            int m = target.Length;
            int n = source.Length;
            int[,] distance = new int[2, m + 1];

            for (int j = 1; j <= m; j++)
                distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(
                        Math.Min(
                        distance[previousRow, j] + 1,
                        distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
        }
    }
}
