// -----------------------------------------------------------------------
// <copyright file="ArmorBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Armors
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generics;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Items;

    /// <summary>
    /// Represents the base class for custom armor behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game armors.
    /// </remarks>
    public abstract class ArmorBehaviour : ItemBehaviour
    {
        /// <inheritdoc cref="ItemBehaviour.Settings"/>.
        public ArmorSettings ArmorSettings => Settings.Cast<ArmorSettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>
        public Armor Armor => Owner.Cast<Armor>();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner is not Armor _)
            {
                Log.Debug($"{CustomItem.Name} is not an Armor", true);
                Destroy();
            }

            if (!Settings.Cast(out ArmorSettings _))
            {
                Log.Debug($"{CustomItem.Name}'s settings are not suitable for an Armor", true);
                Destroy();
            }

            Armor.Weight = ArmorSettings.Weight;
            Armor.StaminaUseMultiplier = ArmorSettings.StaminaUseMultiplier;
            Armor.VestEfficacy = ArmorSettings.VestEfficacy;
            Armor.HelmetEfficacy = ArmorSettings.HelmetEfficacy;
        }
    }
}