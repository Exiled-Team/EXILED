// -----------------------------------------------------------------------
// <copyright file="CustomGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.Components;
    using Exiled.Events.EventArgs;

    using Grenades;

    using MEC;

    using Mirror;

    using UnityEngine;

    /// <inheritdoc />
    public abstract class CustomGrenade : CustomItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> to use for this item.
        /// </summary>
        public override ItemType Type
        {
            get => base.Type;
            protected set
            {
                if (!value.IsThrowable())
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid grenade type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether gets or sets a value that determines if the grenade should explode immediately when contacting any surface.
        /// </summary>
        public abstract bool ExplodeOnCollision { get; }

        /// <summary>
        /// Gets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        public abstract float FuseTime { get; }

        /// <summary>
        /// Gets a value indicating what thrown grenades are currently being tracked.
        /// </summary>
        protected List<GameObject> TrackedGrenades { get; } = new List<GameObject>();

        /// <inheritdoc/>
        public override void Init()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade += OnThrowingGrenade;
            base.Init();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Exiled.Events.Handlers.Player.ThrowingGrenade -= OnThrowingGrenade;
            base.Destroy();
        }

        /// <summary>
        /// Handles tracking thrown custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingGrenadeEventArgs"/>.</param>
        protected virtual void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance
                    .GetComponent<Grenade>();

                Timing.CallDelayed(1f, () =>
                {
                    Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);

                    if (ExplodeOnCollision)
                    {
                        GameObject grenade = SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, 1.5f, GetGrenadeType(Type), ev.Player).gameObject;
                        CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
                        collisionHandler.Owner = ev.Player.GameObject;
                        collisionHandler.Grenade = grenadeComponent;
                        TrackedGrenades.Add(grenade);
                    }
                    else
                    {
                        SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, FuseTime, GetGrenadeType(Type), ev.Player);
                    }

                    ev.Player.RemoveItem(ev.Player.CurrentItem);
                });
            }
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            TrackedGrenades.Clear();
            base.OnWaitingForPlayers();
        }

        /// <summary>
        /// Checks to see if the grenade is a tracked custom grenade.
        /// </summary>
        /// <param name="grenade">The <see cref="GameObject"/> of the grenade to check.</param>
        /// <returns>True if it is a custom grenade.</returns>
        protected bool CheckGrenade(GameObject grenade) => TrackedGrenades.Contains(grenade);

        /// <summary>
        /// Converts a <see cref="ItemType"/> into a <see cref="GrenadeType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to check.</param>
        /// <returns><see cref="GrenadeType"/>.</returns>
        protected GrenadeType GetGrenadeType(ItemType type)
        {
            switch (type)
            {
                case ItemType.GrenadeFlash:
                    return GrenadeType.Flashbang;
                case ItemType.SCP018:
                    return GrenadeType.Scp018;
                default:
                    return GrenadeType.FragGrenade;
            }
        }

        /// <summary>
        /// Spawns a live grenade object on the map.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to spawn the grenade at.</param>
        /// <param name="velocity">The <see cref="Vector3"/> directional velocity the grenade should move at.</param>
        /// <param name="fusetime">The <see cref="float"/> fuse time of the grenade.</param>
        /// <param name="grenadeType">The <see cref="GrenadeType"/> of the grenade to spawn.</param>
        /// <param name="player">The <see cref="Player"/> to count as the thrower of the grenade.</param>
        /// <returns>The <see cref="Grenade"/> being spawned.</returns>
        protected Grenade SpawnGrenade(Vector3 position, Vector3 velocity, float fusetime = 3f, GrenadeType grenadeType = GrenadeType.FragGrenade, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            GrenadeManager component = player.GrenadeManager;
            Grenade component2 = GameObject.Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance)
                .GetComponent<Grenade>();
            component2.FullInitData(component, position, Quaternion.Euler(component2.throwStartAngle), velocity, component2.throwAngularVelocity, player == Server.Host ? Team.RIP : player.Team);
            component2.NetworkfuseTime = NetworkTime.time + fusetime;
            NetworkServer.Spawn(component2.gameObject);

            return component2;
        }
    }
}
