// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using CameraShaking;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.EventArgs.Scp914;

    using MEC;

    using PlayerRoles;

    using UnityEngine;

    using static Example;

    /// <summary>
    /// Handles player events.
    /// </summary>
    internal sealed class PlayerHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        public void OnDied(DiedEventArgs ev)
        {
            if (ev.Player is null)
                return;

            Log.Info($"{ev.Player.Nickname} ({ev.Player.Role}) died from {ev.DamageHandler.Type}! {ev.Attacker.Nickname} ({ev.Attacker.Role}) killed him!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} ({ev.Player.Role}) is changing his role! The new role will be {ev.NewRole}!");

            if (ev.NewRole == RoleTypeId.Tutorial)
            {
                ev.Items.Clear();
                ev.Items.Add(ItemType.Flashlight);
                ev.Items.Add(ItemType.Medkit);

                Timing.CallDelayed(0.5f, () => ev.Player.AddItem(ItemType.Radio));
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingItem(ChangingItemEventArgs)"/>
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            Timing.CallDelayed(
                2f,
                () =>
                {
                    if (ev.Player?.CurrentItem is Firearm firearm)
                    {
                        Log.Info($"{ev.Player.Nickname} has a firearm!");
                        firearm.Recoil = new RecoilSettings(0, 0, 0, 0, 0);
                    }
                });

            Log.Info($"{ev.Player.Nickname} is changing his {(ev.Player?.CurrentItem is null ? "NONE" : ev.Player?.CurrentItem?.Type.ToString())} item to {(ev.Item is null ? "NONE" : ev.Item.Type.ToString())}!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp106.OnTeleporting(TeleportingEventArgs)"/>
        public void OnTeleporting(TeleportingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is teleporting to {ev.Position} as SCP-106!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnActivating(ActivatingEventArgs)"/>
        public void OnActivating(ActivatingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is activating SCP-914!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs)"/>
        public void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is failing to escape from the pocket dimension!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscapingPocketDimension(EscapingPocketDimensionEventArgs)"/>
        public void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is escaping from the pocket dimension and will be teleported at {ev.TeleportPosition}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnChangingKnobSetting(ChangingKnobSettingEventArgs)"/>
        public void OnChangingKnobSetting(ChangingKnobSettingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is changing the knob setting of SCP-914 to {ev.KnobSetting}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.Joined"/>
        public void OnVerified(VerifiedEventArgs ev)
        {
            if (!Instance.Config.JoinedBroadcast.Show)
                return;

            Log.Info($"{ev.Player.Nickname} has authenticated! Their Player ID is {ev.Player.Id} and UserId is {ev.Player.UserId}");
            ev.Player.Broadcast(Instance.Config.JoinedBroadcast.Duration, Instance.Config.JoinedBroadcast.Content, Instance.Config.JoinedBroadcast.Type, false);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.UnlockingGenerator"/>
        public void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is trying to unlock a generator in {ev.Player.CurrentRoom} room");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.Destroying"/>
        public void OnDestroying(DestroyingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} ({ev.Player.Role}) is leaving the server!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.Dying"/>
        public void OnDying(DyingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} ({ev.Player.Role}) is getting killed by {ev.Attacker?.Nickname ?? "None"} ({ev.Attacker?.Role?.ToString() ?? "None"})!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.PreAuthenticating"/>
        public void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            Log.Info($"{ev.UserId} is pre-authenticating from {ev.Country} ({ev.Request.RemoteEndPoint}) with flags {ev.Flags}!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnPickingUpItem(PickingUpItemEventArgs)"/>
        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} has picked up a {ev.Pickup.Type}! Weight: {ev.Pickup.Weight} Serial: {ev.Pickup.Serial}.");
            Log.Warn($"{ev.Pickup.Base.Info.Serial} -- {ev.Pickup.Base.NetworkInfo.Serial}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnUsingItem(UsingItemEventArgs)"/>
        public void OnUsingItem(UsingItemEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is trying to use {ev.Item.Type}.");

            if (ev.Item.Type == ItemType.Adrenaline)
            {
                Log.Info($"{ev.Player.Nickname} was stopped from using their {ev.Item.Type}!");
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        public void OnShooting(ShootingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is shooting a {ev.Player.CurrentItem.Type}! Target Pos: {ev.ShotPosition} Target object ID: {ev.TargetNetId} Allowed: {ev.IsAllowed}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnReloadingWeapon(ReloadingWeaponEventArgs)"/>
        public void OnReloading(ReloadingWeaponEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is reloading their {ev.Firearm.Type}. They have {ev.Firearm.Ammo} ammo. Using ammo type {ev.Firearm.AmmoType}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnReceivingEffect(ReceivingEffectEventArgs)"/>
        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is receiving effect {ev.Effect}. Duration: {ev.Duration} New Intensity: {ev.Intensity} Old Intensity: {ev.CurrentIntensity}");

            if (ev.Effect is Invigorated)
            {
                Log.Info($"{ev.Player.Nickname} is being rejected the {nameof(Invigorated)} effect!");
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnUpgradingPlayer(UpgradingPlayerEventArgs)"/>
        public void OnUpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            Log.Info($"SCP-914 is processing {ev.Player.Nickname} on {ev.KnobSetting}. Upgrade Items: {ev.UpgradeItems} Held Items only: {ev.HeldOnly}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} is dropping {ev.Item.Type}!");

            if (ev.Item.Type == ItemType.Adrenaline)
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSpawning(SpawningEventArgs)"/>
        public void OnSpawning(SpawningEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleTypeId.Scientist)
            {
                ev.Position = new Vector3(53f, 1020f, -44f);

                Timing.CallDelayed(1f, () => ev.Player.CurrentItem = Item.Create(ItemType.GunCrossvec));
                Timing.CallDelayed(1f, () => ev.Player.AddItem(ItemType.GunLogicer));
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscaping(EscapingEventArgs)"/>
        public void OnEscaping(EscapingEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scientist)
                ev.NewRole = RoleTypeId.Tutorial;

            Log.Info($"{ev.Player.Nickname} is trying to escape! Their new role will be {ev.NewRole}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.Hurting"/>
        public void OnHurting(HurtingEventArgs ev)
        {
            Log.Info($"{ev.Player} is being hurt by {ev.DamageHandler.Type}");

            if (ev.Player.Role == RoleTypeId.Scientist)
            {
                Log.Info("Target is a nerd, setting damage to 1 because it's mean to bully nerds.");
                ev.Amount = 1f;
            }
        }
    }
}
