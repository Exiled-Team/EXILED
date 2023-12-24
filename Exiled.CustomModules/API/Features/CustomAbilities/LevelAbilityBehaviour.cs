// -----------------------------------------------------------------------
// <copyright file="LevelAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a specific entity type, providing support for levels the ability.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    /// <typeparam name="TSettings">The type of settings associated with the ability behavior.</typeparam>
    public abstract class LevelAbilityBehaviour<TEntity, TSettings> : AbilityBehaviourBase<TEntity, TSettings>
        where TEntity : GameEntity
        where TSettings : AbilitySettings
    {
        private byte level;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is changed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelChangedDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelAddedDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelRemovedDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the level of the ability.
        /// </summary>
        public virtual byte Level
        {
            get => level;
            set => OnLevelChanged(value);
        }

        /// <inheritdoc/>
        public override void AdjustAddittivePipe()
        {
            base.AdjustAddittivePipe();

            Level = Settings.DefaultLevel;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            OnProcessingNextLevel();
        }

        /// <summary>
        /// Fired when the level is changed.
        /// </summary>
        /// <param name="newLevel">The new level.</param>
        protected virtual void OnLevelChanged(byte newLevel)
        {
            byte previousLevel = level;

            level = newLevel;

            if (previousLevel > level)
                OnLevelRemoved();
            else
                OnLevelAdded();

            OnLevelChangedDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Fired when the ability gains a level.
        /// </summary>
        protected virtual void OnLevelAdded() => OnLevelAddedDispatcher.InvokeAll(this);

        /// <summary>
        /// Fired when the ability loses a level.
        /// </summary>
        protected virtual void OnLevelRemoved() => OnLevelRemovedDispatcher.InvokeAll(this);

        /// <summary>
        /// Fired before granting access to a new level.
        /// <br>It defines the condition to access the next level.</br>
        /// </summary>
        /// <returns><see langword="true"/> if the level can change; otherwise, <see langword="false"/>.</returns>
        protected abstract bool OnProcessingNextLevel();
    }
}