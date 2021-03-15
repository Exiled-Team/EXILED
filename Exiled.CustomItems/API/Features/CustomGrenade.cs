// -----------------------------------------------------------------------
// <copyright file="CustomGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Components;
    using Exiled.Events.EventArgs;

    using Grenades;

    using MEC;

    using Mirror;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public abstract class CustomGrenade : CustomItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> to use for this item.
        /// </summary>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsThrowable())
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid grenade type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value that determines if the grenade should explode immediately when contacting any surface.
        /// </summary>
        public abstract bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        public abstract float FuseTime { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override float Durability { get; set; }

        /// <summary>
        /// Gets a value indicating what thrown grenades are currently being tracked.
        /// </summary>
        [YamlIgnore]
        protected HashSet<GameObject> Tracked { get; } = new HashSet<GameObject>();

        /// <summary>
        /// Spawns a live grenade object on the map.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to spawn the grenade at.</param>
        /// <param name="velocity">The <see cref="Vector3"/> directional velocity the grenade should move at.</param>
        /// <param name="fuseTime">The <see cref="float"/> fuse time of the grenade.</param>
        /// <param name="grenadeType">The <see cref="GrenadeType"/> of the grenade to spawn.</param>
        /// <param name="player">The <see cref="Player"/> to count as the thrower of the grenade.</param>
        /// <returns>The <see cref="Grenade"/> being spawned.</returns>
        public virtual Grenade Spawn(Vector3 position, Vector3 velocity, float fuseTime = 3f, ItemType grenadeType = ItemType.GrenadeFrag, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager grenadeManager = player.GrenadeManager;
            GrenadeSettings settings =
                grenadeManager.availableGrenades.FirstOrDefault(g => g.inventoryID == grenadeType);

            Grenade grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<Grenade>();

            grenade.FullInitData(grenadeManager, position, Quaternion.Euler(grenade.throwStartAngle), velocity, grenade.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            grenade.NetworkfuseTime = NetworkTime.time + fuseTime;

            Tracked.Add(grenade.gameObject);

            NetworkServer.Spawn(grenade.gameObject);

            if (ExplodeOnCollision)
                grenade.gameObject.AddComponent<CollisionHandler>().Init(player.GameObject, grenade);

            return grenade;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.ThrowingGrenade += OnInternalThrowing;
            Events.Handlers.Map.ExplodingGrenade += OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangingIntoGrenade += OnInternalChangingIntoGrenade;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.ThrowingGrenade -= OnInternalThrowing;
            Events.Handlers.Map.ExplodingGrenade -= OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangingIntoGrenade -= OnInternalChangingIntoGrenade;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles tracking thrown custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingGrenadeEventArgs"/>.</param>
        protected virtual void OnThrowing(ThrowingGrenadeEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking exploded custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ExplodingGrenadeEventArgs"/>.</param>
        protected virtual void OnExploding(ExplodingGrenadeEventArgs ev)
        {
        }

        /// <summary>
        /// Handles the tracking of custom grenade pickups that are changed into live grenades by a frag grenade explosion.
        /// </summary>
        /// <param name="ev"><see cref="ChangingIntoGrenadeEventArgs"/>.</param>
        protected virtual void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            Tracked.Clear();

            base.OnWaitingForPlayers();
        }

        /// <summary>
        /// Checks to see if the grenade is a tracked custom grenade.
        /// </summary>
        /// <param name="grenade">The <see cref="GameObject"/> of the grenade to check.</param>
        /// <returns>True if it is a custom grenade.</returns>
        protected bool Check(GameObject grenade) => Tracked.Contains(grenade);

        private void OnInternalThrowing(ThrowingGrenadeEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnThrowing(ev);

            if (!ev.IsAllowed)
                return;

            ev.IsAllowed = false;

            InsideInventories.Remove(ev.Player.CurrentItem.uniq);

            ev.Player.RemoveItem(ev.Player.CurrentItem);

            Timing.CallDelayed(1f, () =>
            {
                Vector3 position = ev.Player.CameraTransform.TransformPoint(new Vector3(0.0715f, 0.0225f, 0.45f));
                Spawn(position, ev.Player.CameraTransform.forward * 9f, FuseTime, Type, ev.Player);
            });
        }

        private void OnInternalExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
                OnExploding(ev);
        }

        private void OnInternalChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.FuseTime = FuseTime;
            ev.Type = Type;

            OnChangingIntoGrenade(ev);

            if (ev.IsAllowed)
            {
                Timing.CallDelayed(0.25f, () => Spawn(ev.Pickup.position, Vector3.zero, ev.FuseTime, ev.Type));
                ev.Pickup.Delete();
                ev.IsAllowed = false;
            }
        }
    }
}
