// -----------------------------------------------------------------------
// <copyright file="GainingExperienceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;

    using Scp079Role = API.Features.Roles.Scp079Role;

    /// <summary>
    /// Contains all information before SCP-079 gains experience.
    /// </summary>
    public class GainingExperienceEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GainingExperienceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="gainType">
        /// <inheritdoc cref="GainType" />
        /// </param>
        /// <param name="roleType">
        /// <inheritdoc cref="RoleType" />
        /// </param>
        /// <param name="amount">
        /// <inheritdoc cref="Amount" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public GainingExperienceEventArgs(Player player, Scp079HudTranslation gainType, int amount, RoleTypeId roleType, bool isAllowed = true)
        {
            Player = player;
            Scp079 = player.Role.As<API.Features.Roles.Scp079Role>();
            GainType = gainType;
            RoleType = roleType;
            Amount = amount;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the role that was used to gain experience.
        /// <remark>The RoleType will be <see cref="RoleTypeId.None"/> when it's not an assisted experience.</remark>
        /// </summary>
        public RoleTypeId RoleType { get; set; }

        /// <summary>
        /// Gets or sets the experience gain type.
        /// </summary>
        public Scp079HudTranslation GainType { get; set; }

        /// <summary>
        /// Gets or sets the amount of experience to be gained.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the experience is successfully granted.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public API.Features.Roles.Scp079Role Scp079 { get; }
    }
}