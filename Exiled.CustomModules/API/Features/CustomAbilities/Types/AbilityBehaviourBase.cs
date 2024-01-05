// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviourBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    public abstract class AbilityBehaviourBase<TEntity> : EBehaviour<TEntity>, IAbilityBehaviour, IAdditiveSettings<AbilitySettings>
        where TEntity : GameEntity
    {
        /// <summary>
        /// Gets the relative <see cref="CustomAbility{T}"/>.
        /// </summary>
        public CustomAbility<TEntity> CustomAbility { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ability is active.
        /// <para>This value is just affected by duration-based abilities.</para>
        /// </summary>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Gets or sets the ability settings.
        /// </summary>
        public AbilitySettings Settings { get; set; }

        /// <inheritdoc/>
        public virtual void AdjustAddittivePipe()
        {
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (CustomAbility<TEntity>.TryGet(GetType(), out CustomAbility<TEntity> customAbility))
                CustomAbility = customAbility;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            SubscribeEvents_Static();
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (IsActive)
                OnUsing();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            CustomAbility.Remove(Owner);

            UnsubscribeEvents_Static();
        }

        /// <summary>
        /// Fired every tick when the ability is still active.
        /// </summary>
        protected abstract void OnUsing();

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
        }

        /// <summary>
        /// Subscribes all the events statically.
        /// </summary>
        protected virtual void SubscribeEvents_Static() => StaticActor.Get<DynamicEventManager>().BindAllFromTypeInstance(this);

        /// <summary>
        /// Unsubscribes all the events statically.
        /// </summary>
        protected virtual void UnsubscribeEvents_Static() => StaticActor.Get<DynamicEventManager>().UnbindAllFromTypeInstance(this);
    }
}