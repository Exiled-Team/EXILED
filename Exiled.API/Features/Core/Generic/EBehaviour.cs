// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// <see cref="EBehaviour{T}"/> is a versatile component designed to enhance the functionality of various entities.
    /// <br>It can be easily integrated with various types of entities, making it a valuable tool for user-defined entity behaviours.</br>
    /// </summary>
    /// <typeparam name="T">The type of the entity to which the behaviour is applied.</typeparam>
    /// <remarks>
    /// This abstract class serves as a foundation for user-defined behaviours that can be applied to entities (such as playable characters)
    /// to extend and customize their functionality. It provides a modular and extensible architecture for enhancing gameplay elements.
    /// </remarks>
    public abstract class EBehaviour<T> : EActor
        where T : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EBehaviour{T}"/> class.
        /// </summary>
        protected EBehaviour()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EBehaviour{T}"/> class.
        /// </summary>
        /// <param name="gameObject">The base <see cref="GameObject"/>.</param>
        protected EBehaviour(GameObject gameObject = null)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Gets or sets the owner of the <see cref="EBehaviour{T}"/>.
        /// </summary>
        public virtual T Owner { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EBehaviour{T}"/> should be automatically disposed
        /// when its owner is set to null.
        /// <para/>
        /// If set to true, the <see cref="EBehaviour{T}"/> will be disposed when the associated owner is set to null.
        /// <br/>
        /// This can be useful to manage resources and cleanup when the owning entity is no longer available.
        /// <br/>
        /// If set to false, the behaviour will remain active even when the owner is null, allowing for custom handling
        /// of such scenarios by the derived classes.
        /// </summary>
        public virtual bool DisposeOnNullOwner { get; protected set; } = true;

        /// <summary>
        /// Virtual method to find and set the owner of the current object.
        /// </summary>
        /// <remarks>
        /// This method is responsible for finding and setting the owner for the current object.
        /// <br>Implementations should define the logic to locate and assign the appropriate owner to the object based on the specific context.</br>
        /// </remarks>
        protected virtual void FindOwner()
        {
            MethodInfo method = typeof(T).GetMethod("Get", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(GameObject) }, null);

            if (method != null)
            {
                Owner = (T)method.Invoke(null, new object[] { Base });
            }
            else
            {
                throw new MissingMethodException($"Method 'Get(GameObject)' not found in class '{typeof(T).Name}'.");
            }
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            Log.InfoWithContext($"EBehaviour::{nameof(PostInitialize)}");

            FindOwner();

            Log.InfoWithContext($"EBehaviour::{nameof(FindOwner)}");

            if (!Owner && DisposeOnNullOwner)
            {
                Log.InfoWithContext($"Destroying Behaviour");

                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (!Owner && DisposeOnNullOwner)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            if (!Owner && DisposeOnNullOwner)
                return;
        }

        /// <summary>
        /// Checks if the specified owner is not null and matches the stored owner.
        /// </summary>
        /// <param name="owner">The owner to be checked.</param>
        /// <returns><see langword="true"/> if the specified owner is not null and matches the stored owner; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method verifies if the provided owner is not null and matches the stored owner.
        /// <br/>It is typically used to ensure that the owner being checked is valid and corresponds to the expected owner for the current context.
        /// </remarks>
        protected virtual bool Check(T owner) => owner && Owner == owner;
    }
}