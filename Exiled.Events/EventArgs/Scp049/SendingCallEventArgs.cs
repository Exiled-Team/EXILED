// -----------------------------------------------------------------------
// <copyright file="SendingCallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;

    using Interfaces;
    using PlayerRoles.PlayableScps.Scp049;

    /// <summary>
    /// Contains all information before SCP-049 Call is activated.
    /// </summary>
    public class SendingCallEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingCallEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SendingCallEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            Duration = Scp049CallAbility.EffectDuration;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the duration of the Call Ability.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the server will send 049 information on the call.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}