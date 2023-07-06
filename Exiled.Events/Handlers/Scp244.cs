// -----------------------------------------------------------------------
// <copyright file="Scp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.Features;

    /// <summary>
    ///     Scp244 related events.
    /// </summary>
    public static class Scp244
    {
        /// <summary>
        ///     Gets or sets invoked before using an <see cref="API.Features.Items.Item" />.
        /// </summary>
        public static Event<UsingScp244EventArgs> UsingScp244 { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before an Scp244 take damage.
        /// </summary>
        public static Event<DamagingScp244EventArgs> DamagingScp244 { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before an Scp244 open because the angle was too low.
        /// </summary>
        public static Event<OpeningScp244EventArgs> OpeningScp244 { get; set; } = new();

        /// <summary>
        ///     Called before using a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingScp244EventArgs" /> instance.</param>
        public static void OnUsingScp244(UsingScp244EventArgs ev) => UsingScp244.InvokeSafely(ev);

        /// <summary>
        ///     Called before an Scp244 take damage.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingScp244EventArgs" /> instance.</param>
        public static void OnDamagingScp244(DamagingScp244EventArgs ev) => DamagingScp244.InvokeSafely(ev);

        /// <summary>
        ///     Called before Scp244 open because the angle was too low.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingScp244EventArgs" /> instance.</param>
        public static void OnOpeningScp244(OpeningScp244EventArgs ev) => OpeningScp244.InvokeSafely(ev);
    }
}