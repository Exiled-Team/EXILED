// -----------------------------------------------------------------------
// <copyright file="Scp079AbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.PlayerAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for SCP-079 specific ability behaviors.
    /// </summary>
    public abstract class Scp079AbilityBehaviour : UnlockableAbilityBehaviour, ISelectableAbility
    {
        /// <summary>
        /// Gets or sets the <see cref="Scp079Role"/> of the owner.
        /// </summary>
        public virtual Scp079Role Scp079 { get; protected set; }

        /// <summary>
        /// Gets or sets the required tier to unlock the ability.
        /// </summary>
        public abstract byte Tier { get; protected set; }

        /// <summary>
        /// Gets or sets the gained experience after using the ability.
        /// </summary>
        public abstract int GainedExperience { get; protected set; }

        /// <summary>
        /// Gets or sets the required energy to use the ability.
        /// </summary>
        public abstract float RequiredEnergy { get; protected set; }

        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="ZoneType"/> containing all the zones in which SCP-079 can use the ability.
        /// </summary>
        public virtual IEnumerable<ZoneType> AllowedZones { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before SCP-079 gains experience.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<Scp079AbilityBehaviour> OnGainedExperienceDispatcher { get; protected set; }

        /// <inheritdoc/>
        protected override void FindOwner()
        {
            base.FindOwner();

            Scp079 = Owner.Role.As<Scp079Role>();
        }

        /// <inheritdoc/>
        protected override void OnActivating()
        {
            if (!AllowedZones.Contains(Owner.Zone) || Owner.Role.As<Scp079Role>().Energy < RequiredEnergy)
                return;

            base.OnActivating();
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            OnGainingExperience();
        }

        /// <inheritdoc/>
        protected override void OnReady()
        {
            base.OnReady();

            Owner.ShowTextDisplay(UnlockableAbilitySettings.OnReady);
        }

        /// <summary>
        /// Fired when the owner gains experience due to the activation of the ability.
        /// </summary>
        protected virtual void OnGainingExperience()
        {
            Scp079.Energy -= RequiredEnergy;
            Scp079.AddExperience(GainedExperience, PlayerRoles.PlayableScps.Scp079.Scp079HudTranslation.Experience);

            OnGainedExperienceDispatcher.InvokeAll(this);
        }
    }
}