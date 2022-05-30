// -----------------------------------------------------------------------
// <copyright file="Scp244Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp244;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public class Scp244Pickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244Pickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp244DeployablePickup"/> class.</param>
        public Scp244Pickup(Scp244DeployablePickup itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="Scp244DeployablePickup"/> that this class is encapsulating.
        /// </summary>
        public new Scp244DeployablePickup Base { get; }

        public float GrowSpeed => Base.GrowSpeed;

        public float TimeToGrow => Base.TimeToGrow;

        public float CurrentDiameter => Base.CurrentDiameter;

        public float CurrentSizePercent
        {
            get => Base.CurrentSizePercent;
            set => Base.CurrentSizePercent = value;
        }

        public float Health
        {
            get => Base._health;
            set => Base._health = value;
        }

        public Scp244State State
        {
            get => Base.State;
            set => Base.State = value;
        }

        public bool Damage(DamageHandler handler) => Base.Damage(handler.Damage, handler, Vector3.zero);

        /// <summary>
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Health}| -{State}- /{CurrentSizePercent}/";
    }
}
