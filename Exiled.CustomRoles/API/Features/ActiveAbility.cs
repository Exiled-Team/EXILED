// -----------------------------------------------------------------------
// <copyright file="ActiveAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features.Enums;

    using MEC;

    using YamlDotNet.Serialization;

    /// <summary>
    /// The base class for active (on-use) abilities.
    /// </summary>
    public abstract class ActiveAbility : CustomAbility
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> containing all players with active abilities, and the abilities they have access to.
        /// </summary>
        public static Dictionary<Player, HashSet<ActiveAbility>> AllActiveAbilities { get; } = new();

        /// <summary>
        /// Gets or sets how long the ability lasts.
        /// </summary>
        public abstract float Duration { get; set; }

        /// <summary>
        /// Gets or sets how long must go between ability uses.
        /// </summary>
        public abstract float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets an action to override the behavior of <see cref="CanUseAbility"/>.
        /// </summary>
        [YamlIgnore]
        public virtual Func<bool>? CanUseOverride { get; set; }

        /// <summary>
        /// Gets the last time this ability was used.
        /// </summary>
        [YamlIgnore]
        public Dictionary<Player, DateTime> LastUsed { get; } = new();

        /// <summary>
        /// Gets all players actively using this ability.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> ActivePlayers { get; } = new();

        /// <summary>
        /// Gets all players who have this ability selected.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> SelectedPlayers { get; } = new();

        /// <summary>
        /// Gets or sets the ability activation message.
        /// </summary>
        public virtual string? ActivationMessage { get; set; }

        /// <summary>
        /// Uses the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        public void UseAbility(Player player)
        {
            ActivePlayers.Add(player);
            LastUsed[player] = DateTime.Now;
            AbilityUsed(player);
            Timing.CallDelayed(Duration, () => EndAbility(player));
        }

        /// <summary>
        /// Uses the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        /// <param name="args">An array of additional arguments.</param>
        public void UseAbility(Player player, params object[] args)
        {
            ActivePlayers.Add(player);
            LastUsed[player] = DateTime.Now;
            AbilityUsed(player, args);
            Timing.CallDelayed(Duration, () => EndAbility(player));
        }

        /// <summary>
        /// Ends the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the ability is ended for.</param>
        public void EndAbility(Player player)
        {
            if (!ActivePlayers.Contains(player))
                return;

            ActivePlayers.Remove(player);
            AbilityEnded(player);
        }

        /// <summary>
        /// Selects the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to select the ability.</param>
        public void SelectAbility(Player player)
        {
            if (!SelectedPlayers.Contains(player))
            {
                SelectedPlayers.Add(player);
                Selected(player);
            }
        }

        /// <summary>
        /// Un-Selects the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to un-select the ability.</param>
        public void UnSelectAbility(Player player)
        {
            if (SelectedPlayers.Contains(player))
            {
                SelectedPlayers.Remove(player);
                if (Check(player, CheckType.Active))
                    EndAbility(player);
                Unselected(player);
            }
        }

        /// <summary>
        /// Checks if the specified player is using the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player is actively using the ability.</returns>
        public override bool Check(Player player) => Check(player, CheckType.Active);

        /// <summary>
        /// Checks if the specified <see cref="Player"/> meets certain check criteria.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <param name="type">The <see cref="CheckType"/> type of check to preform.</param>
        /// <returns>The results of the check.
        /// <see cref="CheckType.Active"/>: Checks if the ability is currently active for the player.
        /// <see cref="CheckType.Selected"/>: Checks if the player has the ability selected.
        /// <see cref="CheckType.Available"/>: Checks if the player has the ability.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">This should never happen unless Joker fucks up.</exception>
        public virtual bool Check(Player player, CheckType type)
        {
            if (player is null)
                return false;
            bool result = type switch
            {
                CheckType.Active => ActivePlayers.Contains(player),
                CheckType.Selected => SelectedPlayers.Contains(player),
                CheckType.Available => Players.Contains(player),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return result;
        }

        /// <summary>
        /// Checks to see if the ability is usable by the player.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="response">The response to send to the player.</param>
        /// <param name="selectedOnly">Whether to disallow usage if the ability is not selected.</param>
        /// <returns>True if the ability is usable.</returns>
        public virtual bool CanUseAbility(Player player, out string response, bool selectedOnly = false)
        {
            if (CanUseOverride is not null)
            {
                response = string.Empty;
                return CanUseOverride.Invoke();
            }

            if (selectedOnly && !SelectedPlayers.Contains(player))
            {
                response = $"{Name} not selected.";
                return false;
            }

            if (!LastUsed.ContainsKey(player))
            {
                response = string.Empty;
                return true;
            }

            DateTime usableTime = LastUsed[player] + TimeSpan.FromSeconds(Cooldown);
            if (DateTime.Now > usableTime)
            {
                response = string.Empty;

                return true;
            }

            response =
                $"You must wait another {Math.Round((usableTime - DateTime.Now).TotalSeconds, MidpointRounding.AwayFromZero)} seconds to use {Name}";

            return false;
        }

        /// <inheritdoc />
        protected override void AbilityAdded(Player player)
        {
            if (LastUsed.ContainsKey(player))
                LastUsed.Remove(player);

            if (!AllActiveAbilities.ContainsKey(player))
                AllActiveAbilities.Add(player, new());

            if (!AllActiveAbilities[player].Contains(this))
                AllActiveAbilities[player].Add(this);
            base.AbilityAdded(player);
        }

        /// <inheritdoc />
        protected override void AbilityRemoved(Player player)
        {
            if (!AllActiveAbilities.ContainsKey(player))
                return;

            SelectedPlayers.Remove(player);

            AllActiveAbilities[player].Remove(this);
            base.AbilityRemoved(player);
        }

        /// <summary>
        /// Called when the ability is used.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        protected virtual void AbilityUsed(Player player)
        {
        }

        /// <summary>
        /// Called when the ability is used.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        /// <param name="args">The additional arguments.</param>
        protected virtual void AbilityUsed(Player player, params object[] args)
        {
        }

        /// <summary>
        /// Called when the abilities duration has ended.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the ability has ended for.</param>
        protected virtual void AbilityEnded(Player player)
        {
        }

        /// <summary>
        /// Called when the ability is successfully used.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        [Obsolete("The Keypress Activator will already do this, you do not need to call this unless you are overwriting the keypress activator.", true)]
        protected virtual void ShowMessage(Player player) =>
            player.ShowHint(string.Format(CustomRoles.Instance!.Config.UsedAbilityHint.Content, Name, Description), CustomRoles.Instance.Config.UsedAbilityHint.Duration);

        /// <summary>
        /// Called when the ability is selected.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> selecting the ability.</param>
        protected virtual void Selected(Player player)
        {
        }

        /// <summary>
        /// Called when the ability is un-selected.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> un-selecting the ability.</param>
        protected virtual void Unselected(Player player)
        {
        }
    }
}