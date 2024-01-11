// -----------------------------------------------------------------------
// <copyright file="AbilityInputComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Exiled.API.Features;
    using Exiled.API.Features.Input;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomAbilities.Settings;
    using Exiled.CustomModules.API.Features.PlayerAbilities;
    using Exiled.Events.EventArgs.Player;

    using PlayerRoles.FirstPersonControl;

    using static Exiled.API.Enums.UUKeypressTriggerType;

    /// <summary>
    /// Represents a marker interface for custom ability.
    /// </summary>
    public class AbilityInputComponent : KeypressInputComponent
    {
        /// <summary>
        /// Gets or sets the last action's response.
        /// </summary>
        public string LastActionResponse { get; protected set; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// <see cref="KT_INPUT_0"/> defines an action which displays information about the ability.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected override bool InputCondition_KT0() => PressCount == 1 && Owner.Role is FpcRole { MoveState: PlayerMovementState.Sneaking };

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// <see cref="KT_INPUT_1"/> defines an action which activates the ability.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected override bool InputCondition_KT1() => PressCount == 1;

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// <see cref="KT_INPUT_2"/> defines an action which switches backward.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected override bool InputCondition_KT2() => PressCount == 2 && Owner.Role is FpcRole { MoveState: PlayerMovementState.Sneaking };

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// <see cref="KT_INPUT_3"/> defines an action which switches forward.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected override bool InputCondition_KT3() => PressCount == 2;

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// The paired action selects the ability.
        /// </summary>
        protected override void InputAction_KT0()
        {
            if (!Owner.Cast<Pawn>().SelectedAbilityBehaviour)
            {
                LastActionResponse = "No ability selected.";
                return;
            }

            ActiveAbilityBehaviour<Player> activeAbilityBehaviour = Owner.Cast<Pawn>().SelectedAbilityBehaviour.Cast<ActiveAbilityBehaviour<Player>>();
            PlayerAbility selected = Owner.Cast<Pawn>().SelectedAbility;
            bool isReady = activeAbilityBehaviour.IsReady;
            bool isLocked = activeAbilityBehaviour.Cast(out UnlockableAbilityBehaviour<Player> unlockableAbility) && !unlockableAbility.IsUnlocked;
            int remainingCooldown = (int)activeAbilityBehaviour.RemainingCooldown;
            StringBuilder builder = StringBuilderPool.Pool.Get();

            builder.AppendLine(selected.Name);
            builder.AppendLine(selected.Description);

            if (selected.Settings.Cast<ActiveAbilitySettings>().Duration > 0f)
            {
                builder.AppendLine(selected.Settings.Cast<ActiveAbilitySettings>().Duration.ToString(CultureInfo.InvariantCulture))
                    .Append($" ({selected.Settings.Cast<ActiveAbilitySettings>().Cooldown}) ").AppendLine();
            }

            builder.AppendLine($"State: ").Append(isReady ? "ready" : isLocked ? "locked" : "not ready");

            if (!isReady && !isLocked)
                builder.Append($" [Remaining cooldown: {remainingCooldown}]");

            LastActionResponse = StringBuilderPool.Pool.ToStringReturn(builder);
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// The paired action activates the ability.
        /// </summary>
        protected override void InputAction_KT1()
        {
            if (!Owner.Cast<Pawn>().SelectedAbilityBehaviour)
            {
                LastActionResponse = "No ability selected.";
                return;
            }

            Owner.Cast<Pawn>().SelectedAbilityBehaviour.Cast<ActiveAbilityBehaviour<Player>>().Activate();
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// The paired action switches ability backward.
        /// </summary>
        protected override void InputAction_KT2() => SwitchAbility(-1);

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// The paired action switches ability forward.
        /// </summary>
        protected override void InputAction_KT3() => SwitchAbility(1);

        private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!Check(ev.Player) || ev.Player.IsNoclipPermitted)
                return;

            PressCount++;
        }

        private void SwitchAbility(int direction)
        {
            ISelectableAbility[] abilities = Owner.Cast<Pawn>().AbilityBehaviours.Cast<ISelectableAbility>().ToArray();

            if (abilities.Length == 0)
            {
                LastActionResponse = "No abilities to switch to.";
                return;
            }

            if (!Owner.Cast<Pawn>().SelectedAbilityBehaviour)
            {
                abilities[0].Select();
                LastActionResponse = $"{Owner.Cast<Pawn>().SelectedAbility.Name}";
                return;
            }

            if (abilities.Length <= 1)
            {
                LastActionResponse = "No abilities to switch to.";
                return;
            }

            if (Owner.Cast<Pawn>().SelectedAbilityBehaviour is not ISelectableAbility selectableAbility)
            {
                LastActionResponse = "Unhandled.";
                return;
            }

            int index = abilities.IndexOf(selectableAbility);
            index = (direction > 0 ? index + 1 : index - 1 + abilities.Length) % abilities.Length;

            abilities[index].Select();
            LastActionResponse = $"{Owner.Cast<Pawn>().SelectedAbilityBehaviour.Name}";
            return;
        }
    }
}