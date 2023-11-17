// -----------------------------------------------------------------------
// <copyright file="VoiceLinesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information prior to sending voiceline SCP-3114.
    /// </summary>
    public class VoiceLinesEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceLinesEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="voiceLine">
        ///     <inheritdoc cref="VoiceLine" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public VoiceLinesEventArgs(ReferenceHub player, PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines.VoiceLinesDefinition voiceLine, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Scp3114 = Player.Role.As<Scp3114Role>();
            VoiceLine = voiceLine;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <inheritdoc cref="PlayerRoles.PlayableScps.Scp3114.Scp3114VoiceLines.VoiceLinesDefinition" />
        public VoiceLinesDefinition VoiceLine { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}