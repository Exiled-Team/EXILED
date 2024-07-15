// -----------------------------------------------------------------------
// <copyright file="ZoneBlackoutEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MapGeneration;
    using PlayerRoles.PlayableScps.Scp079;

    /// <summary>
    /// Contains all information before SCP-079 lockdowns a room.
    /// </summary>
    public class ZoneBlackoutEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneBlackoutEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="zone">
        /// <inheritdoc cref="Zone" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        /// <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="blackoutduration">
        /// <inheritdoc cref="BlackoutDuration" />
        /// </param>
        /// <param name="cooldown">
        /// <inheritdoc cref="Cooldown" />
        /// </param>
        /// <param name="scp079HudTranslation">
        /// <inheritdoc cref="Scp079HudTranslation" />
        /// </param>
        public ZoneBlackoutEventArgs(ReferenceHub player, FacilityZone zone, float auxiliaryPowerCost, float blackoutduration, float cooldown, Scp079HudTranslation scp079HudTranslation)
        {
            Player = Player.Get(player);
            Scp079 = Player.Role.As<API.Features.Roles.Scp079Role>();
            Zone = zone.GetZone();
            AuxiliaryPowerCost = auxiliaryPowerCost;
            BlackoutDuration = blackoutduration;
            Cooldown = cooldown;
            Scp079HudTranslation = scp079HudTranslation;
            IsAllowed = scp079HudTranslation is Scp079HudTranslation.Zoom;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="RoomIdentifier" /> of the room that will be locked down.
        /// </summary>
        public ZoneType Zone { get; }

        /// <summary>
        /// Gets the <see cref="PlayerRoles.PlayableScps.Scp079.Scp079HudTranslation" /> send back to player.
        /// </summary>
        public Scp079HudTranslation Scp079HudTranslation { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to lockdown a room.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets the time of the blackout.
        /// </summary>
        public float BlackoutDuration { get; set; }

        /// <summary>
        /// Gets or sets the blackout cooldown duration.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-079 can lockdown a room.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public API.Features.Roles.Scp079Role Scp079 { get; }
    }
}
