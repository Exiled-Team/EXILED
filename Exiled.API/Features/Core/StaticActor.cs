// -----------------------------------------------------------------------
// <copyright file="StaticActor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic;

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
#pragma warning disable SA1309
        private static readonly Dictionary<Type, StaticActor> __fastCall = new();
#pragma warning restore SA1309

        /// <summary>
        /// Gets a value indicating whether the <see cref="PostInitialize()"/> method has already been called by Unity.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnBeginPlay()"/> method has already been called by Unity.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnEndPlay()"/> method has already been called by Unity.
        /// </summary>
        public bool IsDestroyed { get; private set; }

        /// <inheritdoc cref="CreateNewInstance"/>
        /// <typeparam name="T"><inheritdoc path="/param[@name='type']" cref="CreateNewInstance"/></typeparam>
        public static StaticActor CreateNewInstance<T>()
            where T : new() => CreateNewInstance(typeof(T));

        /// <summary>
        /// Creates a new instance of the <see cref="StaticActor"/>.
        /// </summary>
        /// <param name="type">The type of the <see cref="StaticActor"/>.</param>
        /// <returns>The created or already existing <see cref="StaticActor"/> instance.</returns>
        public static StaticActor CreateNewInstance(Type type)
        {
            EObject @object = CreateDefaultSubobject(type);
            @object.Name = "__" + type.Name + " (StaticActor)";
            @object.SearchForHostObjectIfNull = true;
            StaticActor actor = @object.Cast<StaticActor>();
            actor.ComponentInitialize();
            __fastCall[type] = actor;
            return actor;
        }

        /// <summary>
        /// Gets a <see cref="StaticActor"/> given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="StaticActor"/> to look for.</typeparam>
        /// <returns>The corresponding <see cref="StaticActor"/>, or <see langword="null"/> if not found.</returns>
        public static T Get<T>()
            where T : StaticActor, new()
        {
            if (__fastCall.TryGetValue(typeof(T), out StaticActor staticActor))
                return staticActor.Cast<T>();

            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (actor.GetType() != typeof(T))
                    continue;

                __fastCall[typeof(T)] = actor.Cast<T>();
                return actor.Cast<T>();
            }

            return CreateNewInstance<T>().Cast<T>();
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
            if (__fastCall.TryGetValue(typeof(T), out StaticActor staticActor))
                return staticActor.Cast<T>();

            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (actor.GetType() != type)
                    continue;

                __fastCall[typeof(T)] = actor.Cast<T>();
                return actor.Cast<T>();
            }

            return CreateNewInstance(type).Cast<T>();
        }

        /// <summary>
        /// Gets a <see cref="StaticActor"/> given the specified type.
        /// </summary>
        /// <param name="type">The the type of the <see cref="StaticActor"/> to look for.</param>
        /// <returns>The corresponding <see cref="StaticActor"/>.</returns>
        public static StaticActor Get(Type type)
        {
            if (__fastCall.TryGetValue(type, out StaticActor staticActor))
                return staticActor;

            foreach (StaticActor actor in FindActiveObjectsOfType<StaticActor>())
            {
                if (actor.GetType() != type)
                    continue;

                __fastCall[type] = actor;
                return actor;
            }

            return CreateNewInstance(type);
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Get(GetType()) != this)
            {
                Log.Warn($"Found a duplicated instance of a StaticActor with type {GetType().Name} in the Actor {Name} that will be ignored");
                NotifyInstanceRepeated();
                return;
            }

            if (IsInitialized)
                return;

            PostInitialize_Static();
            IsInitialized = true;
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
            SubscribeEvents();
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
