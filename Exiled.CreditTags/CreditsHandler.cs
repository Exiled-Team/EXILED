// -----------------------------------------------------------------------
// <copyright file="CreditsHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    using System;

    using Exiled.Events.EventArgs;

    /// <summary>
    /// Event Handlers for the <see cref="CreditTags"/> plugin of Exiled.
    /// </summary>
    internal sealed class CreditsHandler
    {
        private readonly CreditTags plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditsHandler"/> class.
        /// </summary>
        /// <param name="plugin">This <see cref="CreditTags"/> instance initializing this class.</param>
        public CreditsHandler(CreditTags plugin) => this.plugin = plugin;

        /// <summary>
        /// Handles checking if a player should have a credit tag or not upon joining.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="VerifiedEventArgs"/></param>
        public void OnPlayerVerify(VerifiedEventArgs ev)
        {
            if (ev.Player.GlobalBadge?.IsGlobal == true)
                return;

            plugin.ShowCreditTag(ev.Player, null, null);
        }
    }
}
