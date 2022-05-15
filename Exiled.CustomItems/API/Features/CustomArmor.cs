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
    using Exiled.Events.EventArgs;

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
                if (!value.IsArmor() && value != ItemType.None)
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid armor type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the Weight of the armor.
        /// </summary>
        public virtual float ArmorWeight { get; set; } = 5.5f;

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

        /// <summary>
        /// Gets or sets how much the players movement speed should be affected when wearing this armor. (higher values = slower movement).
        /// </summary>
        [Description("The value must be above 0 and below 1")]
        public virtual float MovementSpeedMultiplier { get; set; } = 0.95f;

        /// <summary>
        /// Gets or sets how much worse <see cref="RoleType.ClassD"/> and <see cref="RoleType.Scientist"/>s are affected by wearing this armor.
        /// </summary>
        public virtual float CivilianDownsideMultiplier { get; set; } = 2f;

        /// <inheritdoc />
        public override void Give(Player player, bool displayMessage = true)
        {
            Item item = Item.Create(Type);

            if (item is Armor armor)
            {
                armor.Weight = ArmorWeight;

                armor.StaminaUseMultiplier = StaminaUseMultiplier;

                armor.CivilianDownsideMultiplier = CivilianDownsideMultiplier;
                armor.MovementSpeedMultiplier = MovementSpeedMultiplier;

                armor.VestEfficacy = VestEfficacy;
                armor.HelmetEfficacy = HelmetEfficacy;
            }

            player.AddItem(item);

            TrackedSerials.Add(item.Serial);

            Timing.CallDelayed(0.05f, () => OnAcquired(player));

            if(displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.PickingUpArmor += OnInternalPickingUpArmor;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.PickingUpArmor -= OnInternalPickingUpArmor;
            base.UnsubscribeEvents();
        }

        private void OnInternalPickingUpArmor(PickingUpArmorEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8)
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
