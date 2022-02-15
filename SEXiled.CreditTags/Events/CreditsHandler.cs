// -----------------------------------------------------------------------
// <copyright file="CreditsHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CreditTags.Events
{
    using SEXiled.Events.EventArgs;

    using MEC;

    using static CreditTags;

    /// <summary>
    /// Event Handlers for the <see cref="CreditTags"/> plugin of SEXiled.
    /// </summary>
    internal sealed class CreditsHandler
    {
        /// <summary>
        /// Handles checking if a player should have a credit tag or not upon joining.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="VerifiedEventArgs"/></param>
        public void OnPlayerVerify(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(0.5f, () => Instance.ShowCreditTag(ev.Player, null, null));
        }
    }
}
