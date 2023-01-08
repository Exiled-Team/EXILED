// -----------------------------------------------------------------------
// <copyright file="DoctorSenseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Interfaces;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp049;

    /// <summary>
    ///     Contains all information before SCP-049 sense is sent to client.
    /// </summary>
    public class DoctorSenseEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorSenseEventArgs"/> class with information before SCP-049 sense is sent to client.
        /// </summary>
        /// <param name="scp049"> Scp049 <see cref="Player"/>. </param>
        /// <param name="target"> Target <see cref="Player"/>. </param>
        /// <param name="senseAbility"> Doctor's <see cref="Scp049SenseAbility"/> ability. </param>
        /// <param name="isAllowed"><inheritdoc cref="DoctorSenseEventArgs.IsAllowed"/></param>
        public DoctorSenseEventArgs(Player scp049, Player target, Scp049SenseAbility senseAbility, bool isAllowed = true)
        {
            Player = scp049;
            Target = target;
            SenseAbility = senseAbility;
            IsAllowed = isAllowed;
            Cooldown = 5f;
            Duration = 20f;
            Distance = senseAbility._distanceThreshold;
        }

        /// <summary>
        /// Gets or sets distance allowed for doctor to see players.
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Gets 049 Sense ability <see cref="Scp049SenseAbility"/>.
        /// </summary>
        public Scp049SenseAbility SenseAbility { get; }

        /// <summary>
        /// Gets or sets scp049 Duration of sense.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets scp049 Sense cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore NW original checks for cooldown and duration.
        /// </summary>
        public bool BypassChecks { get; set; }

        /// <summary>
        ///     Gets the player who is currently being targeted.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}