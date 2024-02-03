// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
<<<<<<< HEAD
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// <see cref="EBehaviour"/> is a versatile component designed to enhance the functionality of playable characters.
    /// <br>It can be easily integrated with various types of playable characters, making it a valuable tool for user-defined playable character behaviours.</br>
    /// </summary>
    /// <typeparam name="T">The type of user-defined playable character object.</typeparam>
    public abstract class EBehaviour<T> : EActor
        where T : Player
    {
        /// <summary>
        /// Gets the owner of the <see cref="EBehaviour"/>.
        /// </summary>
        protected virtual T Owner { get; private set; }
=======
    using Exiled.API.Features.Core;

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
        /// Abstract method to find and set the owner of the current object.
        /// </summary>
        /// <remarks>
        /// This method is responsible for finding and setting the owner for the current object.
        /// <br>Implementations should define the logic to locate and assign the appropriate owner to the object based on the specific context.</br>
        /// </remarks>
        protected abstract void FindOwner();
>>>>>>> apis-rework

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

<<<<<<< HEAD
            Owner = Player.Get(Base).Cast<T>();
            if (Owner is null)
=======
            FindOwner();
            if (!Owner && DisposeOnNullOwner)
>>>>>>> apis-rework
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

<<<<<<< HEAD
            BehaviourUpdate_Implementation();
=======
            if (!Owner && DisposeOnNullOwner)
            {
                Destroy();
                return;
            }
>>>>>>> apis-rework
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

<<<<<<< HEAD
            if (Owner is null)
=======
            if (!Owner && DisposeOnNullOwner)
>>>>>>> apis-rework
                return;
        }

        /// <summary>
<<<<<<< HEAD
        /// Ran every tick.
        /// <para>Code affecting the <see cref="EBehaviour"/>'s base implementation should be placed here.</para>
        /// </summary>
        protected virtual void BehaviourUpdate()
        {
        }

        /// <inheritdoc cref="BehaviourUpdate"/>
        private protected virtual void BehaviourUpdate_Implementation()
        {
            if (Owner is null)
            {
                Destroy();
                return;
            }

            BehaviourUpdate();
        }
=======
        /// Checks if the specified owner is not null and matches the stored owner.
        /// </summary>
        /// <param name="owner">The owner to be checked.</param>
        /// <returns><see langword="true"/> if the specified owner is not null and matches the stored owner; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method verifies if the provided owner is not null and matches the stored owner.
        /// <br/>It is typically used to ensure that the owner being checked is valid and corresponds to the expected owner for the current context.
        /// </remarks>
        protected virtual bool Check(T owner) => owner && Owner == owner;
>>>>>>> apis-rework
    }
}