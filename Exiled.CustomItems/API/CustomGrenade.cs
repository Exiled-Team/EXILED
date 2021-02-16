// -----------------------------------------------------------------------
// <copyright file="CustomGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
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
        /// Initializes a new instance of the <see cref="CustomGrenade"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to be used.</param>
        /// <param name="itemId">The <see cref="int"/> custom ID to be used.</param>
        protected CustomGrenade(ItemType type, int itemId)
            : base(type, itemId)
        {
        }

        /// <inheritdoc/>
        public abstract override string Name { get; set; }

        /// <inheritdoc/>
        public abstract override string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value that determines if the grenade should explode immediately when contacting any surface.
        /// </summary>
        protected virtual bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        protected virtual float FuseTime { get; set; } = 3f;

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
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Grenade grenadeComponent = ev.Player.GrenadeManager.availableGrenades[0].grenadeInstance
                    .GetComponent<Grenade>();

                Timing.CallDelayed(1f, () =>
                {
                    Vector3 pos = ev.Player.CameraTransform.TransformPoint(grenadeComponent.throwStartPositionOffset);

                    if (ExplodeOnCollision)
                    {
                        GameObject grenade = SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, 1.5f, GetGrenadeType(ItemType), ev.Player).gameObject;
                        CollisionHandler collisionHandler = grenade.gameObject.AddComponent<CollisionHandler>();
                        collisionHandler.Owner = ev.Player.GameObject;
                        collisionHandler.Grenade = grenadeComponent;
                        TrackedGrenades.Add(grenade);
                    }
                    else
                    {
                        SpawnGrenade(pos, ev.Player.CameraTransform.forward * 9f, FuseTime, GetGrenadeType(ItemType), ev.Player);
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
