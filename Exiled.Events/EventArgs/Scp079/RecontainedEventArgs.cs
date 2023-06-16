// -----------------------------------------------------------------------
// <copyright file="RecontainedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains information after SCP-079 recontainment.
    /// </summary>
    public class RecontainedEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecontainedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public RecontainedEventArgs(Player target)
        {
            Player = target;
        }

        /// <inheritdoc />
        public Player Player { get; }
    }
}