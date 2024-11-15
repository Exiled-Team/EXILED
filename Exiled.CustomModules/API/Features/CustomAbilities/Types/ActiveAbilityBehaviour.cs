// -----------------------------------------------------------------------
// <copyright file="ActiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Features.CustomAbilities.Settings;

    using MEC;

    /// <summary>
    /// Represents the base class for active ability behaviors associated with a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    public abstract class ActiveAbilityBehaviour<TEntity> : AbilityBehaviourBase<TEntity>
        where TEntity : GameEntity
    {
        private bool isDurationBased;
        private CoroutineHandle onReadyHandle;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before a
        /// <typeparamref name="TEntity"/> <see cref="GameEntity"/> activates the ability.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnActivatingDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after a
        /// <typeparamref name="TEntity"/> <see cref="GameEntity"/> activates the ability.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnActivatedDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability expires.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnExpiredDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability is ready.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnReadyDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="Settings.ActiveAbilitySettings"/>.
        /// </summary>
        public ActiveAbilitySettings ActiveAbilitySettings { get; set; }

        /// <summary>
        /// Gets or sets the last time the ability was used.
        /// </summary>
        public DateTime LastUsed { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is ready.
        /// </summary>
        public virtual bool IsReady => DateTime.Now > LastUsed + TimeSpan.FromSeconds(ActiveAbilitySettings.Cooldown);

        /// <summary>
        /// Gets or sets the remaining cooldown.
        /// </summary>
        public virtual float RemainingCooldown
        {
            get => (float)Math.Max(0f, (LastUsed + TimeSpan.FromSeconds(ActiveAbilitySettings.Cooldown) - DateTime.Now).TotalSeconds);
            set => LastUsed = LastUsed.AddSeconds(value);
        }

        /// <summary>
        /// Forces the cooldown making the ability usable.
        /// </summary>
        /// <returns>The new <see cref="LastUsed"/> value, representing the updated cooldown timestamp.</returns>
        public virtual DateTime ForceCooldown()
        {
            OnReady();
            LastUsed = DateTime.Now - TimeSpan.FromSeconds(ActiveAbilitySettings.Cooldown + 1);
            return LastUsed;
        }

        /// <summary>
        /// Resets the cooldown making the ability not usable.
        /// </summary>
        /// <param name="forceExpiration">A value indicating whether the ability should expire, if active.</param>
        /// <returns>The new <see cref="LastUsed"/> value, representing the updated cooldown timestamp.</returns>
        public virtual DateTime ResetCooldown(bool forceExpiration = false)
        {
            if (forceExpiration)
                IsActive = false;

            if (onReadyHandle.IsRunning)
                Timing.KillCoroutines(onReadyHandle);

            onReadyHandle = Timing.CallDelayed((DateTime.Now + TimeSpan.FromSeconds(ActiveAbilitySettings.Cooldown)).Second + 1, () =>
            {
                if (IsReady)
                    OnReady();
            });

            return LastUsed = DateTime.Now;
        }

        /// <inheritdoc/>
        public override void AdjustAdditivePipe()
        {
            base.AdjustAdditivePipe();

            if (!Settings.Cast(out ActiveAbilitySettings settings))
            {
                Log.Debug($"{CustomAbility.Name}'s settings are not suitable for an Active Ability", true);
                Destroy();
            }

            ActiveAbilitySettings = settings;
            LastUsed = ActiveAbilitySettings.ForceCooldownOnAdded ? ForceCooldown() : ResetCooldown();
            isDurationBased = ActiveAbilitySettings.Duration > 0f;
        }

        /// <summary>
        /// Activates the ability.
        /// </summary>
        /// <param name="isForced">A value indicating whether the activation should be forced.</param>
        /// <returns><see langword="true"/> if the ability was activated; otherwise, <see langword="false"/>.</returns>
        public virtual bool Activate(bool isForced = false)
        {
            if (isForced || IsReady)
            {
                OnActivating();
                return true;
            }

            return false;
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

            UnsubscribeEvents_Static();
        }

        /// <summary>
        /// Fired before the ability is activated.
        /// </summary>
        protected virtual void OnActivating()
        {
            OnActivatingDispatcher.InvokeAll(this);

            Timing.CallDelayed(ActiveAbilitySettings.WindupTime, OnActivated);
        }

        /// <summary>
        /// Fired after the ability is activated.
        /// </summary>
        protected virtual void OnActivated()
        {
            Execute();

            if (isDurationBased)
            {
                IsActive = true;

                SubscribeEvents();
                Timing.CallDelayed(ActiveAbilitySettings.Duration, OnExpired);
            }
            else
            {
                ResetCooldown();
            }

            OnActivatedDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Fired when the ability time's up.
        /// <para>This method will be fired using duration-based abilities.</para>
        /// </summary>
        protected virtual void OnExpired()
        {
            IsActive = false;

            UnsubscribeEvents();
            ResetCooldown();

            OnExpiredDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Fired when the ability is ready.
        /// </summary>
        protected virtual void OnReady()
        {
            OnReadyDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Executes the ability once, or permanently if behaves passive.
        /// <para>This method will be fired as soon as the ability is activated.</para>
        /// </summary>
        protected abstract void Execute();
    }
}