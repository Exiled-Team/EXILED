// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp106;

    using Extensions;

    using static Events;

    /// <summary>
    ///     SCP-106 related events.
    /// </summary>
    public static class Scp106
    {
        /// <summary>
        ///     Invoked before SCP-106 teleports using the hunter atlas.
        /// </summary>
        public static event CustomEventHandler<UsingHunterAtlasEventArgs> UsingAtlas;

        /// <summary>
        ///     Invoked before SCP-106 use the stalk ability.
        /// </summary>
        public static event CustomEventHandler<StalkingEventArgs> Stalking;

        /// <summary>
        ///     Called before SCP-106 teleports using the hunter atlas.
        /// </summary>
        /// <param name="ev">The <see cref="UsingHunterAtlasEventArgs" /> instance.</param>
        public static void OnUsingAtlas(UsingHunterAtlasEventArgs ev) => UsingAtlas.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-106 use the stalk ability.
        /// </summary>
        /// <param name="ev">The <see cref="StalkingEventArgs"/> instance.</param>
        public static void OnStalking(StalkingEventArgs ev) => Stalking.InvokeSafely(ev);
    }
}