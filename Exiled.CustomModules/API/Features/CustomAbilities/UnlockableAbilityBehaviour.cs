// -----------------------------------------------------------------------
// <copyright file="UnlockableAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System.Collections.Generic;

    using Exiled.API.Features.Core;

    using MEC;

    /// <summary>
    /// Represents the base class for ability behaviors associated with a specific entity type, providing support for levels and unlocking the ability.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    /// <typeparam name="TSettings">The type of settings associated with the ability behavior.</typeparam>
    public abstract class UnlockableAbilityBehaviour<TEntity, TSettings> : LevelAbilityBehaviour<TEntity, TSettings>
        where TEntity : GameEntity
        where TSettings : AbilitySettings
    {
        private CoroutineHandle processUnlockHandle;

        /// <summary>
        /// Gets or sets a value indicating whether the ability can be used regardless any conditions.
        /// </summary>
        public virtual bool IsUnlocked { get; protected set; }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            processUnlockHandle = Timing.RunCoroutine(ProcessConditions());
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Timing.KillCoroutines(processUnlockHandle);
        }

        /// <summary>
        /// Defines the conditions to be met in order to unlock the ability.
        /// </summary>
        /// <returns><see langword="true"/> if the ability can be unlocked; otherwise, <see langword="false"/>.</returns>
        protected abstract bool OnProcessingUnlockConditions();

        /// <summary>
        /// Fired before the ability has been unlocked.
        /// </summary>
        /// <param name="isUnlockable">A value indicating whether the ability should be unlocked.</param>
        protected virtual void OnUnlocking(bool isUnlockable)
        {
            if (IsUnlocked = isUnlockable)
                OnUnlocked();
        }

        /// <summary>
        /// Fired after the ability has been unlocked.
        /// </summary>
        protected virtual void OnUnlocked()
        {
        }

        /// <summary>
        /// Fired after activating the ability which hasn't been unlocked yet.
        /// </summary>
        protected virtual void DenyActivation()
        {
        }

        /// <inheritdoc/>
        protected override void OnActivating()
        {
            if (!IsUnlocked)
            {
                DenyActivation();
                return;
            }

            base.OnActivating();
        }

        private IEnumerator<float> ProcessConditions()
        {
            while (!IsUnlocked)
            {
                yield return Timing.WaitForSeconds(FixedTickRate);

                OnUnlocking(OnProcessingUnlockConditions());
            }

            yield break;
        }
    }
}