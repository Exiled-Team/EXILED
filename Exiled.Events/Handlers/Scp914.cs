// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Handles SCP-914 related events.
    /// </summary>
    public static class Scp914
    {
        /// <summary>
        /// Invoked before SCP-914 upgrades players and items.
        /// </summary>
        public static event CustomEventHandler<UpgradingItemsEventArgs> UpgradingItems;

        /// <summary>
        /// Invoked before activating the SCP-914 machine.
        /// </summary>
        public static event CustomEventHandler<ActivatingEventArgs> Activating;

        /// <summary>
        /// Invoked before changing the SCP-914 machine knob setting.
        /// </summary>
        public static event CustomEventHandler<ChangingKnobSettingEventArgs> ChangingKnobSetting;

        /// <summary>
        /// Called before SCP-914 upgrades players and items.
        /// </summary>
        /// <param name="ev">The <see cref="UpgradingItemsEventArgs"/> instance.</param>
        public static void OnUpgradingItems(UpgradingItemsEventArgs ev) => UpgradingItems.InvokeSafely(ev);

        /// <summary>
        /// Called before activating the SCP-914 machine.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingEventArgs"/> instance.</param>
        public static void OnActivating(ActivatingEventArgs ev) => Activating.InvokeSafely(ev);

        /// <summary>
        /// Called before changing the SCP-914 machine knob setting.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingKnobSettingEventArgs"/> instance.</param>
        public static void OnChangingKnobSetting(ChangingKnobSettingEventArgs ev) => ChangingKnobSetting.InvokeSafely(ev);
    }
}
