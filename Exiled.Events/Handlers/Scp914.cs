// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp914;
    using Exiled.Events.Extensions;

    using static Events;

    /// <summary>
    ///     Handles SCP-914 related events.
    /// </summary>
    public static class Scp914
    {
        /// <summary>
        ///     Invoked before SCP-914 upgrades a item.
        /// </summary>
        public static event CustomEventHandler<UpgradingItemEventArgs> UpgradingItem;

        /// <summary>
        ///     Invoked before SCP-914 upgrades an item in a player's inventory.
        /// </summary>
        public static event CustomEventHandler<UpgradingInventoryItemEventArgs> UpgradingInventoryItem;

        /// <summary>
        ///     Invoked before SCP-914 upgrades a player.
        /// </summary>
        public static event CustomEventHandler<UpgradingPlayerEventArgs> UpgradingPlayer;

        /// <summary>
        ///     Invoked before activating the SCP-914 machine.
        /// </summary>
        public static event CustomEventHandler<ActivatingScp914EventArgs> Activating;

        /// <summary>
        ///     Invoked before changing the SCP-914 machine knob setting.
        /// </summary>
        public static event CustomEventHandler<ChangingKnobSettingEventArgs> ChangingKnobSetting;

        /// <summary>
        ///     Called before SCP-914 upgrades a item.
        /// </summary>
        /// <param name="ev">The <see cref="UpgradingItemEventArgs" /> instance.</param>
        public static void OnUpgradingItem(UpgradingItemEventArgs ev)
        {
            UpgradingItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before SCP-914 upgrades an item in a player's inventory.
        /// </summary>
        /// <param name="ev">The <see cref="UpgradingInventoryItemEventArgs" /> instance.</param>
        public static void OnUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            UpgradingInventoryItem.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before SCP-914 upgrades a player.
        /// </summary>
        /// <param name="ev">The <see cref="UpgradingPlayerEventArgs" /> instance.</param>
        public static void OnUpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            UpgradingPlayer.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before activating the SCP-914 machine.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingScp914EventArgs" /> instance.</param>
        public static void OnActivating(ActivatingScp914EventArgs ev)
        {
            Activating.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before changing the SCP-914 machine knob setting.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingKnobSettingEventArgs" /> instance.</param>
        public static void OnChangingKnobSetting(ChangingKnobSettingEventArgs ev)
        {
            ChangingKnobSetting.InvokeSafely(ev);
        }
    }
}
