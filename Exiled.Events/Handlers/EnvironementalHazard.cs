// -----------------------------------------------------------------------
// <copyright file="EnvironementalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Extensions;

    using static Events;

    /// <summary>
    ///     Cassie related events.
    /// </summary>
    public static class EnvironementalHazard
    {
        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<EnteringEnvironmentalHazardEventArgs> EnteringEnvironmentalHazard;

        /// <summary>
        ///     Invoked when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<StayingOnEnvironmentalHazardEventArgs> StayingOnEnvironmentalHazard;

        /// <summary>
        ///     Invoked when a <see cref="API.Features.Player"/> exists from an environmental hazard.
        /// </summary>
        public static event CustomEventHandler<ExitingEnvironmentalHazardEventArgs> ExitingEnvironmentalHazard;

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnEnteringEnvironmentalHazard(EnteringEnvironmentalHazardEventArgs ev) => EnteringEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="StayingOnEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnStayingOnEnvironmentalHazard(StayingOnEnvironmentalHazardEventArgs ev) => StayingOnEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> exits from an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="ExitingEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnExitingEnvironmentalHazard(ExitingEnvironmentalHazardEventArgs ev) => ExitingEnvironmentalHazard.InvokeSafely(ev);
    }
}
