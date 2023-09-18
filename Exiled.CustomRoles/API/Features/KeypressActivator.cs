// -----------------------------------------------------------------------
// <copyright file="KeypressActivator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.CustomRoles.API.Features.Enums;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;

    using MEC;

    using PlayerRoles.FirstPersonControl;

    /// <summary>
    /// Control class for keypress ability actions.
    /// </summary>
    internal class KeypressActivator
    {
        private readonly Dictionary<Player, int> altTracker = DictionaryPool<Player, int>.Pool.Get();
        private readonly Dictionary<Player, CoroutineHandle> coroutineTracker = DictionaryPool<Player, CoroutineHandle>.Pool.Get();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypressActivator"/> class.
        /// </summary>
        internal KeypressActivator()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="KeypressActivator"/> class.
        /// </summary>
        ~KeypressActivator()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            DictionaryPool<Player, int>.Pool.Return(altTracker);
            DictionaryPool<Player, CoroutineHandle>.Pool.Return(coroutineTracker);
        }

        private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (ev.Player.IsNoclipPermitted)
                return;

            if (!ActiveAbility.AllActiveAbilities.ContainsKey(ev.Player))
                return;

            if (!altTracker.ContainsKey(ev.Player))
                altTracker.Add(ev.Player, 0);

            altTracker[ev.Player]++;

            if (!coroutineTracker.ContainsKey(ev.Player))
                coroutineTracker.Add(ev.Player, default);

            if (!coroutineTracker[ev.Player].IsRunning)
                coroutineTracker[ev.Player] = Timing.RunCoroutine(ProcessAltKey(ev.Player));
        }

        private void OnEndingRound(EndingRoundEventArgs ev)
        {
            altTracker.Clear();
            foreach (CoroutineHandle handle in coroutineTracker.Values)
                Timing.KillCoroutines(handle);
            coroutineTracker.Clear();
        }

        private IEnumerator<float> ProcessAltKey(Player player)
        {
            yield return Timing.WaitForSeconds(0.25f);

            if (!altTracker.TryGetValue(player, out int pressCount))
                yield break;

            Log.Debug($"{player.Nickname}: {pressCount} {(player.Role is FpcRole fpc ? fpc.MoveState : false)}");
            AbilityKeypressTriggerType type = pressCount switch
            {
                1 when player.Role is FpcRole { MoveState: PlayerMovementState.Sneaking } => AbilityKeypressTriggerType.DisplayInfo,
                1 => AbilityKeypressTriggerType.Activate,
                2 when player.Role is FpcRole { MoveState: PlayerMovementState.Sneaking } => AbilityKeypressTriggerType.SwitchBackward,
                2 => AbilityKeypressTriggerType.SwitchForward,
                _ => AbilityKeypressTriggerType.None,
            };

            bool preformed = PreformAction(player, type, out string response);
            switch (preformed)
            {
                case true when type == AbilityKeypressTriggerType.Activate:
                    string[] split = response.Split('|');
                    response = string.Format(CustomRoles.Instance.Config.UsedAbilityHint.Content, split);
                    break;
                case false:
                    response = string.Format(CustomRoles.Instance.Config.FailedActionHint.Content, response);
                    break;
            }

            float dur = preformed ? CustomRoles.Instance.Config.UsedAbilityHint.Duration : CustomRoles.Instance.Config.FailedActionHint.Duration;
            player.ShowHint(response, dur);
            altTracker[player] = 0;
        }

        private bool PreformAction(Player player, AbilityKeypressTriggerType type, out string response)
        {
            ActiveAbility? selected = player.GetSelectedAbility();
            if (type == AbilityKeypressTriggerType.Activate)
            {
                if (selected is null)
                {
                    response = "No selected abilities.";
                    return false;
                }

                if (!selected.CanUseAbility(player, out response, CustomRoles.Instance.Config.ActivateOnlySelected))
                    return false;
                response = $"{selected.Name}|{selected.Description}";
                selected.UseAbility(player);
                return true;
            }

            if (type is AbilityKeypressTriggerType.SwitchForward or AbilityKeypressTriggerType.SwitchBackward)
            {
                List<ActiveAbility> abilities = ListPool<ActiveAbility>.Pool.Get(player.GetActiveAbilities());

                if (abilities.Count == 0)
                {
                    response = "No abilities to switch to.";
                    return false;
                }

                if (selected is not null)
                {
                    int index = abilities.IndexOf(selected);
                    int mod = type == AbilityKeypressTriggerType.SwitchForward ? 1 : -1;
                    if (index + mod > abilities.Count - 1)
                        index = 0;
                    else if (index + mod < 0)
                        index = abilities.Count - 1;
                    else
                        index += mod;

                    if (index < 0 || index > abilities.Count - 1)
                    {
                        Log.Warn("Joker can't do math.");
                        response = "Jokey did a fucky wucky wif his maths";
                        return false;
                    }

                    if (abilities.Count <= 1)
                    {
                        response = "No abilities to switch to.";
                        return false;
                    }

                    selected.UnSelectAbility(player);
                    abilities[index].SelectAbility(player);
                    response = $"{abilities[index].Name} has been selected.";
                    return true;
                }

                abilities[0].SelectAbility(player);
                response = $"{abilities[0].Name} has been selected.";
                return true;
            }

            if (type == AbilityKeypressTriggerType.DisplayInfo)
            {
                if (selected is null)
                {
                    response = "No ability selected.";
                    return false;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get();
                builder.AppendLine(selected.Name);
                builder.AppendLine(selected.Description);
                builder.AppendLine(selected.Duration.ToString(CultureInfo.InvariantCulture)).Append(" (").Append(selected.Cooldown).Append(") ").AppendLine();
                builder.AppendLine($"Usable: ").Append(selected.CanUseAbility(player, out string res));
                if (!string.IsNullOrEmpty(res))
                    builder.Append(" [").Append(res).Append("]");
                response = StringBuilderPool.Pool.ToStringReturn(builder);
                return true;
            }

            response = $"Invalid action: {type}.";
            return false;
        }
    }
}