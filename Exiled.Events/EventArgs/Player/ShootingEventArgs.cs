// -----------------------------------------------------------------------
// <copyright file="ShootingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Firearms.BasicMessages;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before a player fires a weapon.
    /// </summary>
    public class ShootingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ShootingEventArgs" /> class.
        /// </summary>
        /// <param name="shooter">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="msg">
        ///     <inheritdoc cref="ShotMessage" />
        /// </param>
        public ShootingEventArgs(Player shooter, ShotMessage msg)
        {
            Player = shooter;
            ShotMessage = msg;
        }

        /// <summary>
        ///     Gets or sets the <see cref="ShotMessage" /> for the event.
        /// </summary>
        public ShotMessage ShotMessage { get; set; }

        /// <summary>
        ///     Gets or sets the position of the shot.
        /// </summary>
        public Vector3 ShotPosition
        {
            get => ShotMessage.TargetPosition;
            set
            {
                ShotMessage msg = ShotMessage;
                ShotMessage = new ShotMessage
                {
                    ShooterPosition = msg.ShooterPosition,
                    ShooterCameraRotation = msg.ShooterCameraRotation,
                    ShooterCharacterRotation = msg.ShooterCharacterRotation,
                    ShooterWeaponSerial = msg.ShooterWeaponSerial,
                    TargetPosition = value,
                    TargetRotation = msg.TargetRotation,
                    TargetNetId = msg.TargetNetId,
                };
            }
        }

        /// <summary>
        ///     Gets or sets the netId of the target of the shot.
        /// </summary>
        public uint TargetNetId
        {
            get => ShotMessage.TargetNetId;
            set
            {
                ShotMessage msg = ShotMessage;
                ShotMessage = new ShotMessage
                {
                    ShooterPosition = msg.ShooterPosition,
                    ShooterCameraRotation = msg.ShooterCameraRotation,
                    ShooterCharacterRotation = msg.ShooterCharacterRotation,
                    ShooterWeaponSerial = msg.ShooterWeaponSerial,
                    TargetPosition = msg.TargetPosition,
                    TargetRotation = msg.TargetRotation,
                    TargetNetId = value,
                };
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the shot can be fired.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who's shooting.
        /// </summary>
        public Player Player { get; }
    }
}