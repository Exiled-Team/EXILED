// -----------------------------------------------------------------------
// <copyright file="Scp0492.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp0492;
    using Exiled.Events.Extensions;
    using Exiled.Events.Patches.Events.Scp0492;

    /// <summary>
    /// <see cref="API.Features.Roles.Scp0492Role"/> related events.
    /// </summary>
    public class Scp0492
    {
        /// <summary>
        /// Called before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        public static event Events.CustomEventHandler<TriggeringBloodlustEventArgs> TriggeringBloodlust;

        /// <summary>
        /// Called before 049-2 attacks a player.
        /// </summary>
        public static event Events.CustomEventHandler<AttackingEventArgs> Attacking;

        /// <summary>
        /// Called before 049-2 gets his benefits from consuming ability.
        /// </summary>
        public static event Events.CustomEventHandler<ConsumingEventArgs> Consuming;

        /// <summary>
        /// Called before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringBloodlustEventArgs"/> instance.</param>
        public static void OnTriggeringBloodlust(TriggeringBloodlustEventArgs ev) => TriggeringBloodlust.InvokeSafely(ev);

        /// <summary>
        /// Invokes before 049-2 attacks a player.
        /// </summary>
        /// <param name="ev"><see cref="AttackingEventArgs"/> instance.</param>
        public static void OnAttacking(AttackingEventArgs ev) => Attacking.InvokeSafely(ev);

        /// <summary>
        /// Invokes before 049-2 gets his benefits from consuming ability.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="ConsumingEventArgs"/> instance.</param>
        public static void OnConsuming(ConsumingEventArgs ev) => Consuming.InvokeSafely(ev);
    }
}
