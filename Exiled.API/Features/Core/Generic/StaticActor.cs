// -----------------------------------------------------------------------
// <copyright file="StaticActor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    /// <summary>
    /// This is a generic Singleton implementation for components.
    /// <br>Create a derived class where the type <typeparamref name="T"/> is the script you want to "Singletonize"</br>
    /// </summary>
    /// <typeparam name="T">The type of the class.</typeparam>
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
    public abstract class StaticActor<T> : EActor
        where T : EActor
    {
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

        /// <summary>
        /// Looks for an existing instance of the <see cref="StaticActor{T}"/>.
        /// </summary>
        /// <returns>The existing <typeparamref name="T"/> instance, or <see langword="null"/> if not found.</returns>
        public static T FindExistingInstance()
        {
            T[] existingInstances = FindActiveObjectsOfType<T>();
            return existingInstances?.Length == 0 ? null : existingInstances[0];
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StaticActor{T}"/>.
        /// </summary>
        /// <returns>The created <typeparamref name="T"/> instance, or <see langword="null"/> if not found.</returns>
        public static T CreateNewInstance()
        {
            EObject @object = CreateDefaultSubobject<T>();
            @object.Name = "__" + typeof(T).Name + " (StaticActor)";
            return @object.Cast<T>();
        }

        /// <summary>
        /// Gets or creates a new instance of <see cref="StaticActor{T}"/>.
        /// </summary>
        /// <returns>The found or created <typeparamref name="T"/> instance.</returns>
        public static T Get() => FindExistingInstance() ?? CreateNewInstance();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (FindExistingInstance())
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
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor{T}"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void PostInitialize_Static()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnBeginPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor{T}"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void BeginPlay_Static()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnEndPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the <see cref="StaticActor{T}"/> component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void EndPlay_Static()
        {
        }

        /// <summary>
        /// If a duplicated instance of a <see cref="StaticActor{T}"/> component is loaded into the scene this method will be called instead of <see cref="PostInitialize_Static()"/>.
        /// <br>That way you can customize what to do with repeated instances.</br>
        /// </summary>
        /// <remarks>
        /// The default approach is delete the duplicated component.
        /// </remarks>
        protected virtual void NotifyInstanceRepeated() => Destroy(GetComponent<T>());
    }
}