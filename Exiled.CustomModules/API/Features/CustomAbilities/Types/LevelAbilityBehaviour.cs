// -----------------------------------------------------------------------
// <copyright file="LevelAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Features.CustomAbilities.Settings;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a specific entity type, providing support for levels.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    public abstract class LevelAbilityBehaviour<TEntity> : ActiveAbilityBehaviour<TEntity>
        where TEntity : GameEntity
    {
        private byte level;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is changed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelChangedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelAddedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's level is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnLevelRemovedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability's max level has been reached.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnMaxLevelReachedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="Settings.LevelAbilitySettings"/>.
        /// </summary>
        public LevelAbilitySettings LevelAbilitySettings { get; set; }

        /// <summary>
        /// Gets or sets the level of the ability.
        /// </summary>
        public virtual byte Level
        {
            get => level;
            set => OnLevelChanged(value);
        }

        /// <inheritdoc/>
        public override void AdjustAdditivePipe()
        {
            base.AdjustAdditivePipe();

            if (!Settings.Cast(out LevelAbilitySettings settings))
            {
                Log.Debug($"{CustomAbility.Name}'s settings are not suitable for a Level Ability", true);
                Destroy();
            }

            LevelAbilitySettings = settings;
            Level = LevelAbilitySettings.DefaultLevel;
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
        /// Fired when the ability's max level has been reached.
        /// </summary>
        protected virtual void OnMaxLevelReached() => OnMaxLevelReachedDispatcher.InvokeAll(this);

        /// <summary>
        /// Fired before granting access to a new level.
        /// <br>It defines the condition to access the next level.</br>
        /// </summary>
        /// <returns><see langword="true"/> if the level can change; otherwise, <see langword="false"/>.</returns>
        protected abstract bool OnProcessingNextLevel();
    }
}