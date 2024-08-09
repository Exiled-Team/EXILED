// -----------------------------------------------------------------------
// <copyright file="Singleton.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;

    /// <summary>
    /// A class to handle object instances.
    /// </summary>
    /// <typeparam name="T">The type of the object to handle the instance of.</typeparam>
    public sealed class Singleton<T> : TypeCastObject<T>
        where T : class
    {
        // @Nao, this is not realy a singleton if you have mutliple value.
        // NB: each Singleton<T> ave a new instance of dictionary.
        // exemple Singleton<int>.Instance is not the same Singleton<bool>.Instance
        private static readonly Dictionary<T, Singleton<T>> Instances = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton{T}"/> class.
        /// </summary>
        /// <param name="value">The branch to instantiate.</param>
        public Singleton(T value)
        {
            Destroy(value);
            Value = value;
            Instances.Add(value, this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Singleton{T}"/> class.
        /// </summary>
        // @Nao Probably never call, it self reference. Maybe an future issue doing memory leek
        // it will get call only after the instance get remove form Instances
        ~Singleton() => Instances.Remove(Value);

        /// <summary>
        /// Gets the relative value.
        /// </summary>
        public static T Instance => Instances.FirstOrDefault(@object => @object.Key.GetType() == typeof(T)).Value;

        /// <summary>
        /// Gets the singleton value.
        /// </summary>
        internal T Value { get; private set; }

        /// <summary>
        /// Converts the given <see cref="Singleton{T}"/> instance into <typeparamref name="T"/>.
        /// </summary>
        /// <param name="instance">The <see cref="Singleton{T}"/> instance to convert.</param>
        public static implicit operator T(Singleton<T> instance) => instance?.Value;

        /// <summary>
        /// Tries to get the relative value.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="instance">The object instance.</param>
        /// <returns><see langword="true"/> if the object instance is not null and can be casted as the specified type; otherwise, <see langword="false"/>.</returns>
        // @nao It will return the Instance.
        // But with this curent class definition you can register derived class of T.
        // They can be registred but not retrived. Also why make this method generic if this not to get
        // other value than the instance.
        public static bool TryGet<TObject>(out TObject instance)
            where TObject : class => (instance = Instance as TObject) is not null;

        /// <inheritdoc cref="Singleton{T}"/>
        public static void Create(T @object) => new Singleton<T>(@object);

        /// <summary>
        /// Destroys the given <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="object">The object to destroy.</param>
        /// <returns><see langword="true"/> if the instance was destroyed; otherwise, <see langword="false"/>.</returns>
        public static bool Destroy(T @object)
        {
            return Instances.Remove(@object);
        }
    }
}