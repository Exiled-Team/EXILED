// -----------------------------------------------------------------------
// <copyright file="CustomArmor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.ComponentModel;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;

    using MEC;

    /// <summary>
    /// The Custom Armor base class.
    /// </summary>
    public abstract class CustomArmor : CustomItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> to use for this armor.
        /// </summary>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsArmor() && (value != ItemType.None))
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid armor type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets how much faster stamina will drain when wearing this armor.
        /// </summary>
        [Description("The value must be above 1 and below 2")]
        public virtual float StaminaUseMultiplier { get; set; } = 1.15f;

        /// <summary>
        /// Gets or sets how strong the helmet on the armor is.
        /// </summary>
        [Description("The value must be above 0 and below 100")]
        public virtual int HelmetEfficacy { get; set; } = 80;

        /// <summary>
        /// Gets or sets how strong the vest on the armor is.
        /// </summary>
        [Description("The value must be above 0 and below 100")]
        public virtual int VestEfficacy { get; set; } = 80;

        /// <inheritdoc />
        public override void Give(Player player, bool displayMessage = true)
        {
            Armor armor = (Armor)Item.Create(Type);

            armor.Weight = Weight;
            armor.StaminaUseMultiplier = StaminaUseMultiplier;

            armor.VestEfficacy = VestEfficacy;
            armor.HelmetEfficacy = HelmetEfficacy;

            player.AddItem(armor);

            TrackedSerials.Add(armor.Serial);

            Timing.CallDelayed(0.05f, () => OnAcquired(player, armor, displayMessage));

            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUpItem;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUpItem;
            base.UnsubscribeEvents();
        }

        private void OnInternalPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8 || ev.Pickup is Exiled.API.Features.Pickups.BodyArmorPickup)
                return;

            OnPickingUp(ev);

            if (!ev.IsAllowed)
                return;

            ev.IsAllowed = false;

            TrackedSerials.Remove(ev.Pickup.Serial);
            ev.Pickup.Destroy();

            Give(ev.Player);
        }
    }
}