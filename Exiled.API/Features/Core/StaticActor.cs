// -----------------------------------------------------------------------
// <copyright file="StaticActor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    /// <summary>
    /// This is a generic Singleton implementation for components.
    /// <br>Create a derived class of the script you want to "Singletonize"</br>
    /// </summary>
    /// <remarks>
    /// Do not redefine <see cref="PostInitialize()"/> <see cref="OnBeginPlay()"/> or <see cref="OnEndPlay()"/> in derived classes.
    /// <br>Instead, use <see langword="protected virtual"/> methods:</br>
    /// <br><see cref="PostInitialize_Static()"/></br>
    /// <br><see cref="BeginPlay_Static()"/></br>
    /// <br><see cref="EndPlay_Static()"/></br>
    /// <para>
    /// To perform the initialization and cleanup: those methods are guaranteed to only be called once in the entire lifetime of the component.
    /// </para>
    /// </remarks>
    public abstract class StaticActor : EActor
    {
        private static StaticActor instance;

        /// <summary>
        /// Gets a value indicating whether the <see cref="PostInitialize()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnBeginPlay()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnEndPlay()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsDestroyed { get; private set; }

        /// <summary>
        /// Gets the global access point to the unique instance of this class.
        /// </summary>
        public static StaticActor Instance => instance ? instance : IsDestroyed ? null : (instance = FindExistingInstance() ?? CreateNewInstance());

        /// <summary>
        /// Looks or an existing instance of the <see cref="StaticActor"/>.
        /// </summary>
        /// <returns>The existing <see cref="StaticActor"/> instance, or <see langword="null"/> if not found.</returns>
        public static StaticActor FindExistingInstance()
        {
            StaticActor[] existingInstances = FindActiveObjectsOfType<StaticActor>();
            return existingInstances == null || existingInstances.Length == 0 ? null : existingInstances[0];
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StaticActor"/>.
        /// </summary>
        /// <returns>The created <see cref="StaticActor"/> instance, or <see langword="null"/> if not found.</returns>
        public static StaticActor CreateNewInstance()
        {
            EObject @object = CreateDefaultSubobject<StaticActor>();
            @object.Name = "__" + typeof(StaticActor).Name + " (StaticActor)";
            return @object.Cast<StaticActor>();
        }

        /// <summary>
        /// Gets a <see cref="StaticActor"/> given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="StaticActor"/> to look for.</typeparam>
        /// <returns>The corresponding <see cref="StaticActor"/>, or <see langword="null"/> if not found.</returns>
        public static T Get<T>()
            where T : StaticActor
        {
            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (!actor.Cast(out StaticActor staticActor) || staticActor.GetType() != typeof(T))
                    continue;

                return actor.Cast<T>();
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="StaticActor"/> given the specified type.
        /// </summary>
        /// <param name="type">The the type of the <see cref="StaticActor"/> to look for.</param>
        /// <typeparam name="T">The type to cast the <see cref="StaticActor"/> to.</typeparam>
        /// <returns>The corresponding <see cref="StaticActor"/> of type <typeparamref name="T"/>, or <see langword="null"/> if not found.</returns>
        public static T Get<T>(Type type)
            where T : StaticActor
        {
            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (!actor.Cast(out StaticActor staticActor) || staticActor.GetType() != type)
                    continue;

                return actor.Cast<T>();
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="StaticActor"/> given the specified type.
        /// </summary>
        /// <param name="type">The the type of the <see cref="StaticActor"/> to look for.</param>
        /// <returns>The corresponding <see cref="StaticActor"/>, or <see langword="null"/> if not found.</returns>
        public static StaticActor Get(Type type)
        {
            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (!actor.Cast(out StaticActor staticActor) || staticActor.GetType() != type)
                    continue;

                return actor;
            }

            return null;
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            StaticActor ldarg_0 = GetComponent<StaticActor>();

            if (instance == null)
            {
                instance = ldarg_0;
            }
            else if (ldarg_0 != instance)
            {
                Log.Warn($"Found a duplicated instance of a StaticActor with type {GetType().Name} in the Actor {Name} that will be ignored");
                NotifyInstanceRepeated();
                return;
            }

            if (!IsInitialized)
            {
                Log.Debug($"Start() StaticActor with type {GetType().Name} in the Actor {Name}");
                PostInitialize_Static();
                IsInitialized = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (IsStarted)
                return;

            BeginPlay_Static();
            IsStarted = true;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            if (this != instance)
                return;

            IsDestroyed = true;
            EndPlay_Static();
        }

        /// <summary>
        /// Flushes the current actor.
        /// </summary>
        protected virtual void Flush() => Destroy();

        /// <summary>
        /// Fired on <see cref="PostInitialize()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void PostInitialize_Static()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnBeginPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void BeginPlay_Static()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnEndPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void EndPlay_Static()
        {
        }

        /// <summary>
        /// If a duplicated instance of a <see cref="StaticActor"/> component is loaded into the scene this method will be called instead of <see cref="PostInitialize_Static()"/>.
        /// <br>That way you can customize what to do with repeated instances.</br>
        /// </summary>
        /// <remarks>
        /// The default approach is delete the duplicated component.
        /// </remarks>
        protected virtual void NotifyInstanceRepeated() => Destroy(GetComponent<StaticActor>());
    }
}