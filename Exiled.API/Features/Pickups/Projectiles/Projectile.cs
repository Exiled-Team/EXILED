// -----------------------------------------------------------------------
// <copyright file="Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for Projectile.
    /// </summary>
    public class Projectile : Pickup, IWrapper<ThrownProjectile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ThrownProjectile"/> class.</param>
        internal Projectile(ThrownProjectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal Projectile(ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase) || itemBase is not ThrowableItem throwable)
                return;

            throwable.Projectile.gameObject.SetActive(false);
            base.Base = Base = Object.Instantiate(throwable.Projectile);
            throwable.Projectile.gameObject.SetActive(true);

            PickupSyncInfo psi = new()
            {
                ItemId = type,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = itemBase.Weight,
            };

            Info = psi;
            BaseToPickup.Add(Base, this);
        }

        /// <summary>
        /// Gets the <see cref="ThrownProjectile"/> that this class is encapsulating.
        /// </summary>
        public new ThrownProjectile Base { get; }

        /// <summary>
        /// Gets the <see cref="Enums.ProjectileType"/> of the item.
        /// </summary>
        public ProjectileType ProjectileType => Type.GetProjectileType();

        /// <summary>
        /// Creates and returns a new <see cref="Projectile"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="projectiletype"/>, the returned <see cref="Projectile"/> can be casted into a subclass to gain more control over the object.
        /// <br />The following have their own respective classes:
        /// <br />- FragGrenade can be casted to <see cref="ExplosionGrenadeProjectile"/>.
        /// <br />- Flashbang can be casted to <see cref="FlashbangProjectile"/>.
        /// <br />- Scp018 A and B variants can be casted to <see cref="Scp018Projectile"/>.
        /// <br />- Scp2176 can be casted to <see cref="Scp2176Projectile"/>.
        /// </para>
        /// <para>
        /// Projectile that are not listed will cause an Exception.
        /// </para>
        /// </summary>
        /// <param name="projectiletype">The <see cref="ProjectileType"/> of the pickup.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        public static Projectile Create(ProjectileType projectiletype) => projectiletype switch
        {
            ProjectileType.Scp018 => new Scp018Projectile(),
            ProjectileType.Flashbang => new FlashbangProjectile(),
            ProjectileType.Scp2176 => new Scp2176Projectile(),
            ProjectileType.FragGrenade => new ExplosionGrenadeProjectile(ItemType.GrenadeHE),
            _ => throw new System.Exception($"ProjectilType does not contain a valid value :{projectiletype}"),
        };

        /// <summary>
        /// Creates and spawns a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        /// <param name="position">The position to spawn the <see cref="Projectile"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Projectile"/>.</param>
        /// <param name="shouldBeActive">Whether the <see cref="Projectile"/> should be in active state after spawn.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The <see cref="Projectile"/>. See documentation of <see cref="Pickup.Create(ItemType)"/> for more information on casting.</returns>
        public static Projectile CreateAndSpawn(ProjectileType type, Vector3 position, Quaternion rotation, bool shouldBeActive = true, Player previousOwner = null) => Spawn(Create(type), position, rotation, shouldBeActive, previousOwner);

        /// <summary>
        /// Spawns a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="pickup">The <see cref="Projectile"/> too spawn.</param>
        /// <param name="position">The position to spawn the <see cref="Projectile"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Projectile"/>.</param>
        /// <param name="shouldBeActive">Whether the <see cref="Projectile"/> should be in active state after spawn.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The <see cref="Projectile"/> Spawn.</returns>
        public static Projectile Spawn(Projectile pickup, Vector3 position, Quaternion rotation, bool shouldBeActive = true, Player previousOwner = null)
        {
            pickup.Position = position;
            pickup.Rotation = rotation;
            pickup.PreviousOwner = previousOwner;
            pickup.Spawn();

            if (shouldBeActive)
                pickup.Active();

            return pickup;
        }

        /// <summary>
        /// Activates the current <see cref="Projectile"/>.
        /// </summary>
        public void Active() => Base.ServerActivate();

        /// <summary>
        /// Returns the ProjectilePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing ProjectilePickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";
    }
}
