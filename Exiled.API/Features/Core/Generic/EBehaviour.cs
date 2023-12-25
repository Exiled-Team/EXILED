// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// <see cref="EBehaviour{T}"/> is a versatile component designed to enhance the functionality of various entities.
    /// <br>It can be easily integrated with various types of entities, making it a valuable tool for user-defined entity behaviours.</br>
    /// </summary>
    /// /// <typeparam name="T">The type of the entity to which the behaviour is applied.</typeparam>
    /// <remarks>
    /// This abstract class serves as a foundation for user-defined behaviours that can be applied to entities (such as playable characters)
    /// to extend and customize their functionality. It provides a modular and extensible architecture for enhancing gameplay elements.
    /// </remarks>
    public abstract class EBehaviour<T> : EActor
        where T : GameEntity
    {
        /// <summary>
        /// Gets or sets the owner of the <see cref="EBehaviour{T}"/>.
        /// </summary>
        public virtual T Owner { get; protected set; }

        /// <summary>
        /// Abstract method to find and set the owner for the current object.
        /// </summary>
        /// <remarks>
        /// This method is responsible for finding and setting the owner for the current object. Implementations should
        /// define the logic to locate and assign the appropriate owner to the object based on the specific context.
        /// </remarks>
        protected abstract void FindOwner();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FindOwner();
            if (!Owner)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (!Owner)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            if (!Owner)
                return;
        }

        /// <summary>
        /// Checks if the specified owner is not null and matches the stored owner.
        /// </summary>
        /// <param name="owner">The owner to be checked.</param>
        /// <returns><see langword="true"/> if the specified owner is not null and matches the stored owner; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method verifies if the provided owner is not null and matches the stored owner. It is typically used
        /// to ensure that the owner being checked is valid and corresponds to the expected owner for the current context.
        /// </remarks>
        protected virtual bool Check(T owner) => owner && Owner == owner;
    }
}