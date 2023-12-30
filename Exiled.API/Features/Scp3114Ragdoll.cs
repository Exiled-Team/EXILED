// -----------------------------------------------------------------------
// <copyright file="Scp3114Ragdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Exiled.API.Interfaces;
    using PlayerRoles;

    using BaseRagdoll = PlayerRoles.PlayableScps.Scp3114.Scp3114Ragdoll;

    /// <summary>
    /// A wrapper for SCP-3114 ragdolls.
    /// </summary>
    public class Scp3114Ragdoll : Ragdoll, IWrapper<BaseRagdoll>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp3114Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The base ragdoll to wrap. <inheritdoc cref="Base"/></param>
        internal Scp3114Ragdoll(BaseRagdoll ragdoll)
            : base(ragdoll)
        {
            Base = ragdoll;
        }

        /// <inheritdoc/>
        public new BaseRagdoll Base { get; }

        /// <summary>
        /// Gets or sets the role that the corpse is disguised as.
        /// </summary>
        public RoleTypeId DisguiseRole
        {
            get => Base._disguiseRole;
            set => Base.Network_disguiseRole = value;
        }

        /// <summary>
        /// Gets or sets the delay between when SCP-3114 can disguise this corpse.
        /// </summary>
        public float RevealDelay
        {
            get => Base._revealDelay;
            set => Base._revealDelay = value;
        }

        /// <summary>
        /// Gets or sets the time required to reveal this corpse.
        /// </summary>
        public float RevealDuration
        {
            get => Base._revealDuration;
            set => Base._revealDuration = value;
        }

        /// <summary>
        /// Gets or sets the current time of revealing this corpse.
        /// </summary>
        public float RevealElapsed
        {
            get => Base._revealElapsed;
            set => Base._revealElapsed = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this corpse will trigger animation.
        /// </summary>
        public bool IsPlayingAnimation
        {
            get => Base._playingAnimation;
            set => Base._playingAnimation = value;
        }
    }
}
