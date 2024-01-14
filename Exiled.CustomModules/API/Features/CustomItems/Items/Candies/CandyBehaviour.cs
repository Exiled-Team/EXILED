// -----------------------------------------------------------------------
// <copyright file="CandyBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generics;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Scp330;
    using UnityEngine;

    /// <summary>
    /// Represents the base class for custom candies behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game candies.
    /// </remarks>
    public abstract class CandyBehaviour : ItemBehaviour
    {
        /// <inheritdoc cref="ItemBehaviour.Settings"/>
        public CandySettings CandySettings => Settings.Cast<CandySettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>
        public Scp330 Scp330 => Owner.Cast<Scp330>();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner is not Scp330 _)
            {
                Log.Debug($"{CustomItem.Name} is not a Scp330 bag!", true);
                Destroy();
            }

            if (Settings is not CandySettings _)
            {
                Log.Debug($"{CustomItem.Name} settings is not suitable for candies!", true);
                Destroy();
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Scp330.EatingScp330 += OnInternalEatingCandy;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Scp330.EatingScp330 -= OnInternalEatingCandy;
        }

        /// <summary>
        /// Fired when player is eating custom candy.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs"/> event instance.</param>
        protected virtual void OnEatingCandy(EatingScp330EventArgs ev)
        {
        }

        /// <inheritdoc cref="OnEatingCandy"/>
        private protected void OnInternalEatingCandy(EatingScp330EventArgs ev)
        {
            if (ev.Candy.Kind == CandySettings.CandyType && Random.Range(0, 100) >= CandySettings.Weight)
            {
                ev.Candy = new BaseCandy(CandySettings);
                ev.Player.ShowTextDisplay(CandySettings.Message);
                OnEatingCandy(ev);
            }
        }
    }
}